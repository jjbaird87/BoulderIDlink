using BM.Boulder.IDlinkInterface.Properties;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BM.Boulder.IDlinkInterface
{
    public static class UDPListener
    {
        public static event EventHandler<UserReceivedEventArgs> UserDataReceived;
        public static bool ServerStarted = false;
        private static CancellationTokenSource _taskCancellationTokenSource = new CancellationTokenSource();

        public static void StartServer()
        {
            Task.Factory.StartNew(() =>
            {
                ServerStarted = true;
                while (ServerStarted)
                {
                    var task = Task.Factory.StartNew(() =>
                    {
                        return ListenForData();
                    }, _taskCancellationTokenSource.Token).ContinueWith(x => {
                        if (x.IsCanceled)
                            return;
                        if (x.Exception == null)
                            if (x.Result != null && x.Result[0] == 0x00)
                                UserDataReceived?.Invoke(null, new UserReceivedEventArgs(x.Result, ""));
                        else
                            Debug.WriteLine($"User Data Received Error: {x.Exception}");
                    });
                    Task.WaitAll(task);
                }                
            }, _taskCancellationTokenSource.Token);
            
        }

        public static void StopServer()
        {
            _taskCancellationTokenSource?.Cancel();
            ServerStarted = false;
            UserDataReceived = null;
        }

        private static byte[] ListenForData()
        {
            byte[] result = null;
            int listenPort = Settings.Default.ListeningPort;
            using (UdpClient listener = new UdpClient(listenPort))
            {
                IPEndPoint listenEndPoint = new IPEndPoint(IPAddress.Any, listenPort);
                result = listener.Receive(ref listenEndPoint);                
            }
            return result;
        }
    }

    public class UserReceivedEventArgs : EventArgs
    {
        public string CredentialID { get; private set; }
        public byte TaStatus { get; private set; }
        public string IpAddress { get; private set; }

        public UserReceivedEventArgs(byte[] message, string ipAddress)
        {
            bool TimeAndAttendanceEnabled = Settings.Default.TimeAndAttendanceEnabled;
            var userIdLength = BitConverter.ToUInt16(message, 1);
            if (TimeAndAttendanceEnabled)
            {
                //this means that the device is in TA mode
                userIdLength -= 18;
            }

            CredentialID = Encoding.ASCII.GetString(message, 3, userIdLength);
            TaStatus = message[3 + userIdLength];
            //Todo: Get IP address information
            //var ipAddress = ((IPEndPoint)_clientSocket.RemoteEndPoint).Address.ToString();
        }
    }
}
