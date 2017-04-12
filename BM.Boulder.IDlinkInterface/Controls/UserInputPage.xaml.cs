using System;
using System.ComponentModel;
using System.Windows;
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
        private readonly BackgroundWorker _backgroundWorker = new BackgroundWorker();

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

            Member member = null;
            _backgroundWorker.DoWork += (o, args) =>
            {
                var dataAccess = new CTCData(Settings.Default.Connection);
                member = dataAccess.GetMemberByMemberId(TbMemberId.Text);
            };
            _backgroundWorker.RunWorkerCompleted += (o, args) =>
            {
                if (args.Error != null)
                {
                    MessageBox.Show($"{args.Error.Message}{Environment.NewLine}{Environment.NewLine}{args.Error}");
                    return;                    
                }
                _mainForm.Content = member == null
                    ? new ResultPage(_mainForm)
                    : new ResultPage(member.MemberNumber, $"{member.FirstName} {member.LastName}", _mainForm);
            };
            _backgroundWorker.RunWorkerAsync();
        }
    }
}
