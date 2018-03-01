using ABPaint.Elements;
using ABPaint.Objects;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABPaint.Tools.Backend
{
    public class SaveData
    {
        public int topZIndex = 0;
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
            if (File.Exists(path))
            {
                LoadData(path);
                currentFile = path;
            }
            else
                ShowPathNoExistError();
        }

        public static void SaveFile(string path, bool NewFile = false)
        {

            if (File.Exists(path))
                SaveFilePrivate(path);
            else if (NewFile)
                SaveFilePrivate(path);
            else
                ShowPathNoExistError();         
        }

        private static void SaveFilePrivate(string path)
        {
            //string compressed;

            var outStream = File.OpenWrite(path);
            using (var tinyStream = new GZipStream(outStream, CompressionMode.Compress))
            using (var mStream = new MemoryStream(Encoding.UTF8.GetBytes(SaveData())))
                mStream.CopyTo(tinyStream);

                //compressed = Encoding.ASCII.GetString(outStream.ToArray());

            //File.WriteAllText(path, compressed);
            currentFile = path;
        }

        private static void ShowPathNoExistError()
        {
            System.Windows.Forms.MessageBox.Show("The path that this file was saved to no longer exists. As a result, it cannot be saved. If you wish to save it elsewhere, use 'Save As'. Or, insert the media required for the path to be avalible", "Error saving file", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
        }

        public static void LoadData(string path)
        {
            string decompressedData = "";

            var inStream = File.OpenRead(path);
            using (var bigStream = new GZipStream(inStream, CompressionMode.Decompress))
            using (var bigStreamOut = new MemoryStream()) {
                bigStream.CopyTo(bigStreamOut);

                decompressedData = Encoding.ASCII.GetString(bigStreamOut.ToArray());
            }

            savedata = ABJson.GDISupport.JsonClassConverter.ConvertJsonToObject<SaveData>(decompressedData);
        }

        public static string SaveData()
        {
            return ABJson.GDISupport.JsonClassConverter.ConvertObjectToJson(savedata, ABJson.GDISupport.JsonFormatting.Compact);
        }
        #endregion

        #region Other Images
        public static void ImportFile(string path)
        {
            ImageE newElement = new ImageE(ImportData(path));

            newElement.Width = newElement.mainImage.Size.Width;
            newElement.Height = newElement.mainImage.Size.Height;

            Core.AddElement(newElement);
        }

        public static void ExportFile(string path)
        {
            ExportData(path);
            currentFile = path;
        }

        public static Bitmap ImportData(string path)
        {
            return (Bitmap)Image.FromFile(path);
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
