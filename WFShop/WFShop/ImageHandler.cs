using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;

namespace WFShop
{
    abstract class ImageHandler
    {
        public static string PathToFolder { get; set; }

        //private static FileInfo[] LoadFiles(string fileExtension = DEFAULT_EXT) =>
        //    new DirectoryInfo(PathToFolder).GetFiles("*." + fileExtension);

        //public static IEnumerable<Image> LoadImages()
        //{
        //    foreach (FileInfo fileInfo in LoadFiles())
        //    {
        //        yield return Image.FromFile(fileInfo.FullName);
        //    }
        //}

        private const string DEFAULT_EXT = "jpg";

        public static Image LoadImage(int serialNumber, string fileExtension = DEFAULT_EXT)
            => LoadImage(serialNumber.ToString(), fileExtension);

        public static Image LoadImage(string fileName, string fileExtension = DEFAULT_EXT)
        {
            string filePath = Path.Combine(PathToFolder, fileName + "." + fileExtension);
            if (File.Exists(filePath))
                return Image.FromFile(filePath);
            return null;
        }

        private static Image p_default;
        public static Image Default => p_default ?? (p_default = LoadImage("no-image"));
        
        // LoadImages(): IEnumerable<Image> <==> LoadImages(): List<Image>
        
        //public static List<Image> LoadImages()
        //{
        //    List<Image> images = new List<Image>();
        //    foreach (FileInfo fileInfo in LoadFiles())
        //    {
        //        images.Add(Image.FromFile(fileInfo.FullName));
        //    }
        //    return images;
        //}
    }
}
