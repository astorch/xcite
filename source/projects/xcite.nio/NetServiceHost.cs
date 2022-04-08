using System;
using System.Net;
using System.Net.Sockets;

namespace xcite.nio {
    
    public class NetServiceHost<TService> : IDisposable {
        private readonly object _access = new object();

        public NetServiceHost(string ipAddress, int  port) 
            : this(IPAddress.Parse(ipAddress), port) {

        }

        public NetServiceHost(IPAddress ipAddress, int port) {
            IpAddress = ipAddress;
            Port = port;
        }

        /// <summary> Is raised when an unexpected error occurred. </summary>
        public virtual event TraceEventHandler Error;

        protected virtual TcpListener TcpListener { get; set; }

        protected virtual IPAddress IpAddress { get; }

        protected virtual int Port { get; }

        public virtual void Open() {
            lock (_access) {
                if (TcpListener != null) return;
                TcpListener = new TcpListener(IpAddress, Port);
                TcpListener.BeginAcceptTcpClient(OnAcceptTcpClient, null);
                TcpListener.Start();
            }
        }

        public virtual void Close() {
            lock (_access) {
                if (TcpListener == null) return;
                TcpListener.Stop();
                TcpListener = null;
            }
        }

        /// <inheritdoc />
        public virtual void Dispose() {
            Close();
        }

        protected virtual void ProcessClientConnection(TcpClient tcpClient) {

        }
        
        protected virtual void OnAcceptTcpClient(IAsyncResult ar) {
            TcpClient tcpClient;
            lock (_access) {
                if (TcpListener == null) return; // Service has been stopped
                
                // Open connection to new client
                tcpClient = TcpListener.EndAcceptTcpClient(ar);

                // Listen for further incoming connections (again)
                TcpListener.BeginAcceptTcpClient(OnAcceptTcpClient, null);
            }

            try {
                using (tcpClient) {
                    ProcessClientConnection(tcpClient);
                }
            } catch (Exception ex) {
                Error?.Invoke("Uncaught exception on processing client connection.", ex);
            }
        }
    }
}