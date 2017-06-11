namespace xcite.messaging {
    /// <summary>
    /// Defines a message consumer that is being notified by the <see cref="MessageDispatcher"/>.
    /// </summary>
    public interface IMessageConsumer {
        /// <summary>
        /// Is invoked when a message is being dispatched by the <see cref="MessageDispatcher"/>.
        /// </summary>
        /// <param name="eventSource">Message sender</param>
        /// <param name="eventArguments">Message arguments</param>
        /// <param name="dataObject">Additional data object</param>
        void OnMessageReceived(object eventSource, object eventArguments, object dataObject);
    }
}