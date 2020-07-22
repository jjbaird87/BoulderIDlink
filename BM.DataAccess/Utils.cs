using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;

namespace BM.DataAccess
{
    public static class Utils
    {
        public static BitmapImage ImageFromBuffer(byte[] bytes)
        {
            var stream = new MemoryStream(bytes);
            var image = new BitmapImage();
            image.BeginInit();
            image.StreamSource = stream;
            image.EndInit();
            return image;
        }
    }
}
