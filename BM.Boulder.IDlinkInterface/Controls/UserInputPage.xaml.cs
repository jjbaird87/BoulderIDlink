using System.Windows.Input;
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
        public UserInputPage(MainWindow mainForm)
        {
            InitializeComponent();
            _mainForm = mainForm;
            TbMemberId.Focus();

        }

        private void TbMemberId_KeyUp(object sender, KeyEventArgs e)
        {            
            if (e.Key != Key.Enter) return;
            if (TbMemberId.Text == "") return;

            var dataAccess = new CTCData(Settings.Default.Connection);
            var member = dataAccess.GetMemberByMemberId(TbMemberId.Text);

            _mainForm.Content = member == null
                ? new ResultPage(_mainForm)
                : new ResultPage(member.MemberNumber, $"{member.FirstName} {member.LastName}", _mainForm);
        }
    }
}
