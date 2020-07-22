using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace BM.DataAccess
{       
    /// <summary>
    /// TCP Socket Listener Class
    /// </summary>
    public class MorphoTcpSocketListener
    {
        #region Constants and Static

        private Socket _clientSocket;
        private bool _stopClient;
        private Thread _clientListenerThread;
        private bool _markedForDeletion;
        private DateTime _lastReceiveDateTime;
        private DateTime _currentReceiveDateTime;

        /// <summary>
        /// On Rules Communication Handler Event
        /// </summary>
        public event EventHandler OnRulesCommunicationHandler;
        public bool TimeAndAttendanceEnabled { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="clientSocket"></param>
        public MorphoTcpSocketListener(Socket clientSocket)
        {
            _clientSocket = clientSocket;
            TimeAndAttendanceEnabled = true;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Client SocketListener Destructor
        /// </summary>
        ~MorphoTcpSocketListener()
        {
            StopSocketListener();
        }

        /// <summary>
        /// Method that starts SocketListener Thread.
        /// </summary>
        public void StartSocketListener()
        {
            if (_clientSocket == null) return;
            _clientListenerThread = new Thread(SocketListenerThreadStart);
            _clientListenerThread.SetApartmentState(ApartmentState.STA);
            _clientListenerThread?.Start();
        }

        /// <summary>
        /// Method that stops Client SocketListening Thread.
        /// </summary>
        public void StopSocketListener()
        {
            if (_clientSocket == null) return;
            _stopClient = true;
            _clientSocket.Close();

            // Wait for one second for the the thread to stop.
            _clientListenerThread.Join(1000);

            // If still alive; Get rid of the thread.
            if (_clientListenerThread.IsAlive) _clientListenerThread.Abort();

            _clientListenerThread = null;
            _clientSocket = null;
            _markedForDeletion = true;
        }

        /// <summary>
        /// Method that returns the state of this object i.e. whether this
        /// object is marked for deletion or not.
        /// </summary>
        /// <returns></returns>
        public bool IsMarkedForDeletion()
        {
            return _markedForDeletion;
        }

        /// <summary>
        /// On Communication Event Handler
        /// </summary>
        /// <param name="appEventArgs"></param>
        public void OnApComms(ApplicationEventArguments appEventArgs)
        {
            try
            {
                var handler = OnRulesCommunicationHandler;
                if (handler == null) return;

                handler(null, appEventArgs);
            }
            catch (Exception)
            {
                // Do Nothing
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Thread method that does the communication to the client. This 
        /// thread tries to receive from client and if client sends any data
        /// then parses it and again wait for the client data to come in a
        /// loop. The recieve is an indefinite time receive.
        /// </summary>
        private void SocketListenerThreadStart()
        {
            var byteBuffer = new byte[1024];

            _lastReceiveDateTime = DateTime.Now;
            _currentReceiveDateTime = DateTime.Now;

            var timer = new Timer(CheckClientCommInterval, null, 15000, 15000);

            while (!_stopClient)
            {
                try
                {
                    _clientSocket.Receive(byteBuffer, SocketFlags.None);
                    _currentReceiveDateTime = DateTime.Now;

                    var message = byteBuffer[0];
                    if (message == 0x00)
                    {
                        ProcessRequestData(byteBuffer);
                    }
                    //Do nothing if not Access Granted
                }
                catch (SocketException)
                {
                    _stopClient = true;
                    _markedForDeletion = true;
                }
            }

            timer.Change(Timeout.Infinite, Timeout.Infinite);
        }


        private void ProcessRequestData(byte[] message)
        {
            try
            {
                var userIdLength = BitConverter.ToUInt16(message, 1);
                if (TimeAndAttendanceEnabled)
                {
                    //this means that the device is in TA mode
                    userIdLength -= 18;
                }

                var deviceCredentialId = Encoding.ASCII.GetString(message, 3, userIdLength);
                var taStatus = BitConverter.ToString(message, 3 + userIdLength, 1);
                var ipAddress = ((IPEndPoint)_clientSocket.RemoteEndPoint).Address.ToString();               
                
                OnApComms(new ApplicationEventArguments(deviceCredentialId, ipAddress, taStatus));
                
                
                _markedForDeletion = true;
                _stopClient = true;
            }
            catch (Exception exception)
            {
                //Ignore for now
                //Todo: Proper exception handling here

                _stopClient = true;
                _markedForDeletion = true;
            }
        }

       

        /// <summary>
        /// Method that checks whether there are any client calls for the
        /// last 15 seconds or not. If not this client SocketListener will
        /// be closed.
        /// </summary>
        /// <param name="o"></param>
        private void CheckClientCommInterval(object o)
        {
            if (_lastReceiveDateTime.Equals(_currentReceiveDateTime))
                StopSocketListener();
            else
                _lastReceiveDateTime = _currentReceiveDateTime;
        }

        #endregion

        #region Classes

        /// <summary>
        /// Application Event Arguments
        /// </summary>
        public class ApplicationEventArguments : EventArgs
        {
            #region Properties

            /// <summary>
            /// Message
            /// </summary>
            public string UserId
            {
                get; set;
            }

            public string DeviceIp
            {
                get; set;
            }

            public string TimeAndAttendanceStatus
            {
                get; set;
            }

            #endregion

            #region Constructor

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="userId"></param>
            /// <param name="deviceIp"></param>
            /// <param name="taStatus"></param>
            public ApplicationEventArguments(string userId, string deviceIp, string taStatus)
            {
                UserId = userId;
                DeviceIp = deviceIp;
                TimeAndAttendanceStatus = taStatus;
            }

            #endregion
        }

        #endregion
    }
}
