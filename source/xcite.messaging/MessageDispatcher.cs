using System;
using System.Linq;
using System.Threading;
using xcite.collections;
using xcite.csharp;

namespace xcite.messaging {
    /// <summary> Defines a handler for a message dispatch error. </summary>
    /// <param name="message">Error message</param>
    public delegate void MessageDispatchErrorHandler(string message);

    /// <summary> Defines the signature of an UI thread dispatcher. </summary>
    /// <param name="action">Action to invoke on the UI thread</param>
    public delegate void UIThreadDispatcher(Action action);

    /// <summary> Provides a common message dispatcher. </summary>
    public class MessageDispatcher : GenericSingleton<MessageDispatcher> {
        private readonly AutoLockStruct<bool> _isAlive = new AutoLockStruct<bool>();

        private readonly AutoLockObject<LinearList<ConsumerEntry>> _registeredConsumers = new AutoLockObject<LinearList<ConsumerEntry>>(new LinearList<ConsumerEntry>());
        private readonly AutoLockObject<LinearList<EnqueuedEvent>> _eventQueue = new AutoLockObject<LinearList<EnqueuedEvent>>(new LinearList<EnqueuedEvent>());
        private readonly ManualResetEventSlim _processQueueEvent = new ManualResetEventSlim(false);
        private UIThreadDispatcher _uiThreadDispatcher;

        /// <summary> Event that is raised when a dispatch error occurred. </summary>
        public event MessageDispatchErrorHandler Error;

        /// <summary> Returns the UI Thread dispatcher or does set it. </summary>
        public UIThreadDispatcher UIThreadDispatcher {
            get {
                lock (this) {
                    return _uiThreadDispatcher;
                }
            }
            set {
                lock (this) {
                    _uiThreadDispatcher = value;
                }
            }
        }

        /// <summary>
        /// Subscribes the given <paramref name="messageConsumer"/> to the dispatcher.
        /// </summary>
        /// <param name="messageConsumer">Message consumer</param>
        /// <param name="dataObject">Additional data object that is passed to the consumer each time</param>
        /// <param name="uiThreadDispatch">Requires UI thread dispatching</param>
        public void Subscribe(IMessageConsumer messageConsumer, object dataObject = null, bool uiThreadDispatch = false) {
            if (messageConsumer == null) return;

            _registeredConsumers.Access(_ => _.Add(new ConsumerEntry(messageConsumer, dataObject, uiThreadDispatch)));
        }

        /// <summary>
        /// Unsubscribes the given <paramref name="messageConsumer"/> from the dispatcher.
        /// </summary>
        /// <param name="messageConsumer">Message consumer</param>
        public void Unsubscribe(IMessageConsumer messageConsumer) {
            if (messageConsumer == null) return;

            ConsumerEntry[] registeredConsumer = _registeredConsumers.Access(_ => _.ToArray());
            ConsumerEntry entry = registeredConsumer.FirstOrDefault(_ => _.MessageConsumer == messageConsumer);
            if (entry == null) return;

            _registeredConsumers.Access(_ => _.Remove(entry));
        }

        /// <summary>
        /// Enqueues a new message with the given arguments. The message is being dispatched the next time 
        /// the dispatcher gains control.
        /// </summary>
        /// <param name="sender">Message sender</param>
        /// <param name="eventArguments">Message arguments</param>
        public void Enqueue(object sender, object eventArguments) {
            _eventQueue.Access(_ => _.Add(new EnqueuedEvent(sender, eventArguments)));
            _processQueueEvent.Set();
        }

        /// <summary>
        /// Is invoked when the message dispatcher thread has been started.
        /// </summary>
        /// <param name="o">Reference to the message dispatcher</param>
        private void ReadAndDispatch(object o) {
            MessageDispatcher messageDispatcher = (MessageDispatcher) o;

            while (_isAlive.Get()) {
                // Wait until a signal is received
                messageDispatcher._processQueueEvent.Wait();

                // Reset
                messageDispatcher._processQueueEvent.Reset();

                // Collect consumer and events
                EnqueuedEvent[] queuedEvents = messageDispatcher._eventQueue.Access(list => {
                    var buffer = list.ToArray();
                    list.Clear();
                    return buffer;
                });
                ConsumerEntry[] registeredConsumers = messageDispatcher._registeredConsumers.Access(_ => _.ToArray());

                // Get the set UI thread dispatcher or set a fake one
                UIThreadDispatcher uiThreadDispatcher = UIThreadDispatcher ?? (_ =>_());

                // Dispatch
                for (int i = -1; ++i != queuedEvents.Length;) {
                    EnqueuedEvent queuedEvent = queuedEvents[i];

                    for (int j = -1; ++j != registeredConsumers.Length;) {
                        ConsumerEntry consumerEntry = registeredConsumers[j];

                        // Anonymous function to dispatch the event
                        void dispatchEvent() {
                            try {
                                consumerEntry.MessageConsumer.OnMessageReceived(queuedEvent.Sender,
                                    queuedEvent.EventArguments, consumerEntry.DataObject);
                            } catch (Exception ex) {
                                string errorMessage = $"Error on dispatching event to '{consumerEntry.MessageConsumer.GetType()}'. Reason: {ex}";
                                Error?.Invoke(errorMessage);
                            }
                        }

                        // Dispatch event using the configured thread
                        try {
                            if (consumerEntry.UIThreadDispatch)
                                uiThreadDispatcher(dispatchEvent);
                            else 
                                dispatchEvent();
                        } catch (Exception ex) {
                            string errorMessage = $"Error on invoking event dispatch to '{consumerEntry.MessageConsumer.GetType()}'. Reason: {ex}";
                            Error?.Invoke(errorMessage);
                        }
                    }
                }
            }
        }

        /// <inheritdoc />
        protected override void OnInitialize() {
            _isAlive.Set(true);
            Thread thread = new Thread(ReadAndDispatch) {Name = "xcite msg dispatcher", IsBackground = true};
            thread.Start(this);
        }

        /// <inheritdoc />
        protected override void OnDestroy() {
            _isAlive.Set(false);
        }

        /// <summary>  Describes an enqueued event that waits for dispatching. </summary>
        class EnqueuedEvent {
            /// <summary>
            /// Creates a new instance with the given arguments.
            /// </summary>
            /// <param name="sender">Message sender</param>
            /// <param name="eventArguments">Message arguments</param>
            public EnqueuedEvent(object sender, object eventArguments) {
                Sender = sender;
                EventArguments = eventArguments;
            }

            /// <summary> Returns the message sender. </summary>
            public object Sender { get; }

            /// <summary> Returns the message arguments. </summary>
            public object EventArguments { get; }
        }

        /// <summary>  Describes an registered message consumer. </summary>
        class ConsumerEntry {
            /// <summary>
            /// Creates a new instance with the given arguments.
            /// </summary>
            /// <param name="messageConsumer">Message consumer</param>
            /// <param name="dataObject">Additional data object</param>
            /// <param name="uiThreadDispatch">Requires UI thread dispatching</param>
            public ConsumerEntry(IMessageConsumer messageConsumer, object dataObject, bool uiThreadDispatch) {
                MessageConsumer = messageConsumer;
                DataObject = dataObject;
                UIThreadDispatch = uiThreadDispatch;
            }

            /// <summary> Returns the message consumer. </summary>
            public IMessageConsumer MessageConsumer { get; }

            /// <summary> Returns the additional data object. </summary>
            public object DataObject { get; }

            /// <summary> Returns TRUE if the event shall be dispatched by the UI thread. </summary>
            public bool UIThreadDispatch { get; }
        }
    }
}
