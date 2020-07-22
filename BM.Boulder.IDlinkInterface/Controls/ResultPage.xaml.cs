using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using BM.Boulder.IDlinkInterface.Properties;
using BM.DataAccess;
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
        private readonly MainWindow _mainForm;
        private static List<Task> delayTasks = new List<Task>();
        private bool cancelled;


        public ResultPage(Member member, MainWindow mainForm)
        {
            InitializeComponent();

            StatusLabel.Content = !string.IsNullOrEmpty(member.Action) ? $"Success: ({member.Action})" : "Success";
            MemberIdLabel.Content = member.MemberNumber;
            NameLabel.Content = $"{member.FirstName} {member.LastName}";
            StatusBorder.Background = Brushes.Green;
            StatusImage.Source = member.Photo == null
                ? new BitmapImage(new Uri($"pack://application:,,,{CorrectImage}"))
                : Utils.ImageFromBuffer(member.Photo);

            _mainForm = mainForm;
            RunDelay();
        }

        public ResultPage(MainWindow mainForm)
        {
            InitializeComponent();
            StatusLabel.Content = "Member was not found";
            MemberIdLabel.Content = "";
            NameLabel.Content = "";
            StatusBorder.Background = Brushes.White;
            StatusImage.Source = new BitmapImage(new Uri($"pack://application:,,,{NotFoundImage}"));

            _mainForm = mainForm;
            RunDelay();
        }


        private void RunDelay()
        {
            delayTasks.Add(Task.Factory.StartNew(() =>
            {
                Thread.Sleep(new TimeSpan(0, 0, Settings.Default.TimeToWaitAfterIdentification));
            }).ContinueWith(x =>
            {
                if (delayTasks.Count == 1)
                    _mainForm.SelectPageUserInputPage();
                delayTasks.RemoveAt(0);
            }, TaskScheduler.FromCurrentSynchronizationContext()));
        }
    }
}
