using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using BM.Boulder.IDlinkInterface.Properties;
using BM.DataAccess;

namespace BM.Boulder.IDlinkInterface.Controls
{
    /// <summary>
    /// Interaction logic for UserInputPage.xaml
    /// </summary>
    public partial class UserInputPage
    {
        private readonly MainWindow _mainForm;
        private BackgroundWorker _backgroundWorker;
        

        public UserInputPage(MainWindow mainForm)
        {
            InitializeComponent();
            _mainForm = mainForm;
            TbMemberId.Focus();

            if (Settings.Default.EnableTCPListener)
            {          
                if (!UDPListener.ServerStarted)
                {
                    UDPListener.StartServer();
                }
            }

            UDPListener.UserDataReceived += UDPListener_UserDataReceived; ;
        }

        private void UDPListener_UserDataReceived(object sender, UserReceivedEventArgs e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                (Action)(() =>
                {
                    if (!Equals(_mainForm.Content, this))
                    {
                        Thread.Sleep(Settings.Default.TimeToWaitAfterIdentification);
                    }
                    var userId = e.CredentialID;
                    //var lastThree = userId.Substring(userId.Length - 3, 3);
                    //userId = userId.Remove(userId.Length - 3, 3);
                    //userId = userId.PadLeft(4, '0') + "-" + lastThree;
                    ProcessUser(userId, e.TaStatus);
                }
                ));
        }

        private void TbMemberId_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;
            if (TbMemberId.Text == "") return;

            ProcessUser(TbMemberId.Text);
        }

        private void ProcessUser(string userId, byte? action = null)
        {
            Member member = null;
            _mainForm.Content = new LoadingPage();
            _backgroundWorker = new BackgroundWorker();
            _backgroundWorker.DoWork += (o, args) =>
            {
                var dataAccess = new CtcData(Settings.Default.Connection);
                member = dataAccess.GetMemberByMemberId(args.Argument.ToString());                
                if (member != null && action != null)
                    member.Action = GetAction((byte)action);
            };
            _backgroundWorker.RunWorkerCompleted += (o, args) =>
            {
                if (args.Error != null)
                {
                    _mainForm.SelectPageUserInputPage();
                    MessageBox.Show($"{args.Error.Message}{Environment.NewLine}{Environment.NewLine}{args.Error}");
                    return;
                }
                Thread.Sleep(200);
                _mainForm.Content = member == null
                    ? new ResultPage(_mainForm)
                    : new ResultPage(member, _mainForm);
            };
            //User ID is input to the background thread
            _backgroundWorker.RunWorkerAsync(userId);
        }

        private string GetAction(byte action)
        {
            switch (action)
            {
                case 1:
                    return Settings.Default.Key1;
                case 2:
                    return Settings.Default.Key2;
                case 3:
                    return Settings.Default.Key3;
                case 4:
                    return Settings.Default.Key4;
                case 5:
                    return Settings.Default.Key5;
                case 6:
                    return Settings.Default.Key6;
                case 7:
                    return Settings.Default.Key7;
                case 8:
                    return Settings.Default.Key8;
                case 9:
                    return Settings.Default.Key9;
                case 10:
                    return Settings.Default.Key10;
                case 11:
                    return Settings.Default.Key11;
                case 12:
                    return Settings.Default.Key12;
                case 13:
                    return Settings.Default.Key13;
                case 14:
                    return Settings.Default.Key14;
                case 15:
                    return Settings.Default.Key15;
                case 16:
                    return Settings.Default.Key16;
                default:
                    return "";
            }
        }
    }

}
