using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Threading;
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
        private readonly BackgroundWorker _backgroundWorker = new BackgroundWorker();
        private readonly MainWindow _mainForm;


        public ResultPage(Member member, MainWindow mainForm)
        {
            InitializeComponent();
            
            StatusLabel.Content = "Success";
            MemberIdLabel.Content = member.MemberNumber;
            NameLabel.Content = $"{member.FirstName} {member.LastName}";
            StatusBorder.Background = Brushes.Green;
            var image = LoadImage(member);
            StatusImage.Source = image == null ? new BitmapImage(new Uri($"pack://application:,,,{CorrectImage}")) : image;
            if (image != null)
            {
                StatusImage.Width = image.Width;
                StatusImage.Height = image.Height;
            }

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
            StatusBorder.Background = Brushes.White;
            StatusImage.Source = new BitmapImage(new Uri($"pack://application:,,,{NotFoundImage}"));

            _mainForm = mainForm;
            _backgroundWorker.DoWork += BackgroundWorker_DoWork;
            _backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
            _backgroundWorker.RunWorkerAsync();
        }

        private BitmapImage LoadImage(Member member)
        {
            if  (member.Photo == null || (member.Photo.ImageType == ImageTypeEnum.XMLSerialization && member.Photo.XMLSerialization.BMPBytes.Length == 0)) return null;
            if (member.Photo.ImageType == ImageTypeEnum.Path && !File.Exists(member.Photo.FileName)) return null;
            
            var image = new BitmapImage();
            switch (member.Photo.ImageType)
            {
                case ImageTypeEnum.None:
                    return null;
                case ImageTypeEnum.XMLSerialization:
                    using (var mem = new MemoryStream(member.Photo.XMLSerialization.BMPBytes))
                    {
                        mem.Position = 0;
                        image.BeginInit();
                        image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                        image.CacheOption = BitmapCacheOption.OnLoad;
                        image.UriSource = null;
                        image.StreamSource = mem;
                        image.EndInit();
                    }
                    image.Freeze();
                    
                    break;
                case ImageTypeEnum.Path:
                    using (var fs = File.Open(member.Photo.FileName, FileMode.Open))
                    {
                        var dImg = new Bitmap(fs);
                        var ms = new MemoryStream();
                        dImg.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                        var bImg = new BitmapImage();
                        bImg.BeginInit();
                        bImg.StreamSource = new MemoryStream(ms.ToArray());
                        bImg.EndInit();
                        //img is an Image control.                        
                        image = bImg;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return image;                        
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
