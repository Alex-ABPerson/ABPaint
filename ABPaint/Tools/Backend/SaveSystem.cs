using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABPaint.Tools.Backend
{
    public class SaveData
    {
        public List<Element> imageElements = new List<Element>();
        public Size imageSize = new Size(800, 600);
    }

    public static class SaveSystem
    {
        public static SaveData savedata = new SaveData();
        public static string currentFile = "";

        #region ABPaint Images
        public static void LoadFile(string path)
        {
            LoadData(File.ReadAllText(path));
            currentFile = path;
        }

        public static void SaveFile(string path)
        {
            File.WriteAllText(path, SaveData());
            currentFile = path;
        }

        public static void LoadData(string data)
        {
            savedata = ABJson.GDISupport.JsonClassConverter.ConvertJsonToObject<SaveData>(data);
        }

        public static string SaveData()
        {
            return ABJson.GDISupport.JsonClassConverter.ConvertObjectToJson(savedata, ABJson.GDISupport.JsonFormatting.Compact);
        }
        #endregion

        #region Other Images
        public static void ImportFile(string path)
        {
            ImportData(File.ReadAllText(path));
            currentFile = path;
        }

        public static void ExportFile(string path)
        {
            ExportData(path);
            currentFile = path;
        }

        public static Image ImportData(string path)
        {
            return Image.FromFile(path);
        }

        public static void ExportData(string path)
        {
            ImageFormat imgFormat;

            switch (Path.GetExtension(path))
            {
                case ".bmp":
                    imgFormat = ImageFormat.Bmp;
                    break;
                case ".gif":
                    imgFormat = ImageFormat.Gif;
                    break;
                case ".jpg":
                case ".jpeg":
                    imgFormat = ImageFormat.Jpeg;
                    break;
                case ".png":
                    imgFormat = ImageFormat.Png;
                    break;
                case ".tif":
                case ".tiff":
                    imgFormat = ImageFormat.Tiff;
                    break;
                default:
                    imgFormat = ImageFormat.Png;
                    break;
            }

            Core.PaintPreview().Save(path, imgFormat);
        }
        #endregion
    }
}
