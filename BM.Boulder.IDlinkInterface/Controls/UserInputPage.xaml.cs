using System;
using System.ComponentModel;
using System.Threading;
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
        private BackgroundWorker _backgroundWorker;

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
            _mainForm.Content = new LoadingPage();
            _backgroundWorker = new BackgroundWorker();
            _backgroundWorker.DoWork += (o, args) =>
            {                
                var dataAccess = new CTCData(Settings.Default.Connection);
                member = dataAccess.GetMemberByMemberId(args.Argument.ToString());
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
            _backgroundWorker.RunWorkerAsync(TbMemberId.Text);
        }
    }
}
