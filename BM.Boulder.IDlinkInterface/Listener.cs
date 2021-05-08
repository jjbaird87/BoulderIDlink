//using System;
//using System.Collections;
//using System.Net;
//using System.Net.Sockets;
//using System.Threading;
//using System.Windows;
//using BM.Boulder.IDlinkInterface.Properties;
//using BM.DataAccess;

//namespace BM.Boulder.IDlinkInterface
//{
    //public static class Listener
    //{
    //    public static event EventHandler OnRulesCommunicationHandler;
    //    public static bool Started = false;

    //    private static Thread _purgingThread;
    //    private static TcpListener _tcpListener;
    //    private static Thread _mServerThread;
    //    private static ArrayList _mSocketListenersList = new ArrayList();
    //    private static bool _mStopPurging;
    //    private static bool _mStopServer;

    //    public static void StartServer()
    //    {
    //        try
    //        {
    //            if (_tcpListener == null)
    //            {
    //                _tcpListener =
    //                    new TcpListener(new IPEndPoint(IPAddress.Parse(Settings.Default.LocalIP),
    //                        Settings.Default.ListeningPort));
    //            }

    //            // Create a ArrayList for storing SocketListeners before
    //            // starting the server.
    //            lock (_mSocketListenersList)
    //            {
    //                _mSocketListenersList = new ArrayList();
    //            }

    //            // Start the Server and start the thread to listen client // requests.
    //            _tcpListener.Start();
    //            _mServerThread = new Thread(ServerThreadStart);
    //            _mServerThread.SetApartmentState(ApartmentState.STA);
    //            _mServerThread.Start();

    //            // Create a low priority thread that checks and deletes client
    //            // SocktConnection objcts that are marked for deletion.
    //            _purgingThread = new Thread(PurgingThreadStart) { Priority = ThreadPriority.Lowest };
    //            _purgingThread.Start();

    //            Started = true;
    //        }
    //        catch(Exception e)
    //        {
    //            //Do nothing for now
    //            MessageBox.Show(string.Format("Error in starting server: {0}", e.Message));
    //            Started = false;
    //        }
    //    }

    //    /// <summary>
    //    ///     Method that stops the TCP/IP Server.
    //    /// </summary>
    //    public static void StopServer()
    //    {
    //        if (_tcpListener == null) return;
    //        // It is important to Stop the server first before doing
    //        // any cleanup. If not so, clients might being added as
    //        // server is running, but supporting data structures
    //        // (such as m_socketListenersList) are cleared. This might
    //        // cause exceptions.

    //        // Stop the TCP/IP Server.
    //        _mStopServer = true;
    //        _tcpListener.Stop();

    //        // Wait for one second for the the thread to stop.
    //        if (_mServerThread == null) return;
    //        _mServerThread.Join(1000);

    //        // If still alive; Get rid of the thread.
    //        if (_mServerThread.IsAlive) _mServerThread.Abort();
    //        _mServerThread = null;

    //        _mStopPurging = true;
    //        _purgingThread.Join(1000);
    //        if (_purgingThread.IsAlive) _purgingThread.Abort();

    //        _purgingThread = null;

    //        // Free Server Object.
    //        _tcpListener = null;

    //        // Stop All clients.
    //        StopAllSocketListers();

    //        Started = false;
    //    }

    //    /// <summary>
    //    ///     Method that stops all clients and clears the list.
    //    /// </summary>
    //    private static void StopAllSocketListers()
    //    {
    //        lock (_mSocketListenersList)
    //        {
    //            foreach (MorphoTcpSocketListener socketListener in _mSocketListenersList)
    //            {
    //                socketListener.StopSocketListener();
    //            }
    //        }
    //        // Remove all elements from the list.
    //        lock (_mSocketListenersList)
    //        {
    //            _mSocketListenersList.Clear();
    //        }
    //        lock (_mSocketListenersList)
    //        {
    //            _mSocketListenersList = null;
    //        }
    //    }

    //    private static void ServerThreadStart()
    //    {
    //        // Client Socket variable;
    //        while (!_mStopServer)
    //        {
    //            try
    //            {

    //                // Wait for any client requests and if there is any 
    //                // request from any client accept it (Wait indefinitely).
    //                var clientSocket = _tcpListener.AcceptSocket();

    //                // Create a SocketListener object for the client.
    //                var socketListener = new MorphoTcpSocketListener(clientSocket);
    //                socketListener.OnRulesCommunicationHandler += socketListener_OnRulesCommsHandler;

    //                // Add the socket listener to an array list in a thread 
    //                // safe fashion.
    //                //Monitor.Enter(m_socketListenersList);
    //                lock (_mSocketListenersList)
    //                {
    //                    _mSocketListenersList.Add(socketListener);
    //                }
    //                //Monitor.Exit(m_socketListenersList);

    //                // Start a communicating with the client in a different
    //                // thread.
    //                socketListener.StartSocketListener();
    //            }
    //            catch (SocketException)
    //            {
    //                _mStopServer = true;
    //            }
    //        }
    //    }

    //    private static void PurgingThreadStart()
    //    {
    //        while (!_mStopPurging)
    //        {
    //            var deleteList = new ArrayList();

    //            // Check for any clients SocketListeners that are to be
    //            // deleted and put them in a separate list in a thread sage
    //            // fashon.
    //            //Monitor.Enter(m_socketListenersList);
    //            lock (_mSocketListenersList)
    //            {
    //                foreach (MorphoTcpSocketListener socketListener in _mSocketListenersList)
    //                {
    //                    if (!socketListener.IsMarkedForDeletion()) continue;
    //                    deleteList.Add(socketListener);
    //                    socketListener.StopSocketListener();
    //                }

    //                // Delete all the client SocketConnection ojects which are
    //                // in marked for deletion and are in the delete list.
    //                foreach (var t in deleteList)
    //                {
    //                    _mSocketListenersList.Remove(t);
    //                }
    //            }
    //            //Monitor.Exit(m_socketListenersList);

    //            Thread.Sleep(10000);
    //        }
    //    }

    //    private static void socketListener_OnRulesCommsHandler(object sender, EventArgs e)
    //    {
    //        try
    //        {
    //            OnApplicationCommunication((MorphoTcpSocketListener.ApplicationEventArguments)e);
    //        }
    //        catch (Exception)
    //        {
    //            // Do Nothing
    //        }
    //    }

    //    public static void OnApplicationCommunication(MorphoTcpSocketListener.ApplicationEventArguments eventArg)
    //    {
    //        try
    //        {
    //            var handler = OnRulesCommunicationHandler;
    //            if (handler == null) return;

    //            handler(null, eventArg);
    //        }
    //        catch (Exception e)
    //        {
    //            // Do Nothing
    //        }
    //    }
    //}
//}
