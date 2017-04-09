using System;
using System.ComponentModel;
using System.Threading;
using System.Windows.Media.Imaging;
using BM.Boulder.IDlinkInterface.Properties;
using Brushes = System.Windows.Media.Brushes;

namespace BM.Boulder.IDlinkInterface.Controls
{
    /// <summary>
    /// Interaction logic for ResultPage.xaml
    /// </summary>
    public partial class ResultPage
    {
        private const string CorrectImage = "/Resources/Correct.jpg";
        private const string NotFoundImage = "/Resources/NotFound.jpg";
        private readonly BackgroundWorker _backgroundWorker = new BackgroundWorker();
        private readonly MainWindow _mainForm;


        public ResultPage(string memberId, string name, MainWindow mainForm)
        {
            InitializeComponent();
            
            StatusLabel.Content = "Success";
            MemberIdLabel.Content = memberId;
            NameLabel.Content = name;
            StatusBorder.Background = Brushes.Green;           
            StatusImage.Source = new BitmapImage(new Uri($"pack://application:,,,{CorrectImage}"));

            _mainForm = mainForm;
            _backgroundWorker.DoWork += BackgroundWorker_DoWork;
            _backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
            _backgroundWorker.RunWorkerAsync();
        }

        public ResultPage(MainWindow mainForm)
        {
            InitializeComponent();
            StatusLabel.Content = "Member was not found";
            MemberIdLabel.Content = "";
            NameLabel.Content = "";
            StatusBorder.Background = Brushes.Red;
            StatusImage.Source = new BitmapImage(new Uri($"pack://application:,,,{NotFoundImage}"));

            _mainForm = mainForm;
            _backgroundWorker.DoWork += BackgroundWorker_DoWork;
            _backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
            _backgroundWorker.RunWorkerAsync();
        }

        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _mainForm.SelectPageUserInputPage();
        }

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Thread.Sleep(new TimeSpan(0, 0, Settings.Default.TimeToWaitAfterIdentification));
        }

        
    }
}
