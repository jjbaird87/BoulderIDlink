using BM.Boulder.IDlinkInterface.Controls;

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
        }

        public void SelectPageUserInputPage()
        {
            _userInputPage.TbMemberId.Clear();
            _userInputPage.TbMemberId.Focus();
            Content = _userInputPage;
        }
    }
}
