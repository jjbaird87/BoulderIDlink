using BM.Boulder.IDlinkInterface.Controls;
using BM.Boulder.IDlinkInterface.Properties;

namespace BM.Boulder.IDlinkInterface
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        readonly UserInputPage _userInputPage;
        public MainWindow()
        {
            InitializeComponent();
            _userInputPage = new UserInputPage(this);
            Content = _userInputPage;

            Closing += MainWindow_Closing;
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Settings.Default.EnableTCPListener)
                UDPListener.StopServer();
        }

        public void SelectPageUserInputPage()
        {
            _userInputPage.TbMemberId.Clear();
            _userInputPage.TbMemberId.Focus();
            Content = _userInputPage;
        }
    }
}
