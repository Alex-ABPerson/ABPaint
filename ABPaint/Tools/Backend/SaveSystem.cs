// ***********************************************************************
// Assembly         : ABPaint
// Author           : Alex
// Created          : 12-07-2017
//
// Last Modified By : Alex
// Last Modified On : 03-29-2018
// ***********************************************************************
// <copyright file="SaveSystem.cs" company="">
//     . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
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
        private int _TopZindex;

        public int TopZindex
        {
            get
            {
                return _TopZindex;
            }
            set
            {
                _TopZindex = value;
            }
        }
        private List<Element> _ImageElements = new List<Element>();

        public List<Element> ImageElements
        {
            get
            {
                return _ImageElements;
            }
            set
            {
                _ImageElements = value;
            }
        }
        private Size _ImageSize = new Size(800, 600);

        public Size ImageSize
        {
            get
            {
                return _ImageSize;
            }
            set
            {
                _ImageSize = value;
            }
        }
    }

    public static class SaveSystem
    {
        public static SaveData CurrentSave = new SaveData();
        public static string CurrentFile = "";

        #region ABPaint Images
        public static void LoadFile(string path)
        {
            if (File.Exists(path))
            {
                LoadData(path);
                CurrentFile = path;
            }
            else
                ShowPathNoExistError();
        }

        public static void SaveFile(string path, bool newFile = false)
        {
            if (File.Exists(path))
                SaveFilePrivate(path);
            else if (newFile)
                SaveFilePrivate(path);
            else
                ShowPathNoExistError();         
        }

        private static void SaveFilePrivate(string path)
        {
            //string compressed;

            using (var outStream = File.OpenWrite(path))
            {
                using (var tinyStream = new GZipStream(outStream, CompressionMode.Compress))
                using (var mStream = new MemoryStream(Encoding.UTF8.GetBytes(Save())))
                    mStream.CopyTo(tinyStream);

                //compressed = Encoding.ASCII.GetString(outStream.ToArray());

                //File.WriteAllText(path, compressed);
                CurrentFile = path;
            }
        }

        private static void ShowPathNoExistError()
        {
            System.Windows.Forms.MessageBox.Show("The path that this file was saved to no longer exists. As a result, it cannot be saved. If you wish to save it elsewhere, use 'Save As'. Or, insert the media required for the path to be avalible", "Error saving file", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
        }

        public static void LoadData(string path)
        {
            string decompressedData = "";

            using (var inStream = File.OpenRead(path))
            {
                using (var bigStream = new GZipStream(inStream, CompressionMode.Decompress))
                using (var bigStreamOut = new MemoryStream())
                {
                    bigStream.CopyTo(bigStreamOut);

                    decompressedData = Encoding.ASCII.GetString(bigStreamOut.ToArray());
                }

                CurrentSave = ABJson.GDISupport.JsonClassConverter.ConvertJsonToObject<SaveData>(decompressedData);
            }
        }

        public static string Save()
        {
            return ABJson.GDISupport.JsonClassConverter.ConvertObjectToJson(CurrentSave, ABJson.GDISupport.JsonFormatting.Compact);
        }
        #endregion

        #region Other Images
        public static void ImportFile(string path)
        {
            ImageE newElement = new ImageE(ImportData(path));

            newElement.Width = newElement.MainImage.Size.Width;
            newElement.Height = newElement.MainImage.Size.Height;

            Core.AddElement(newElement);
        }

        public static void ExportFile(string path)
        {
            ExportData(path);
            CurrentFile = path;
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
