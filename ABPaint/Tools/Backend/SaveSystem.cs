﻿using ABPaint.Elements;
using ABPaint.Objects;
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
                LoadData(File.ReadAllText(path));
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
            File.WriteAllText(path, SaveData());
            currentFile = path;
        }

        private static void ShowPathNoExistError()
        {
            System.Windows.Forms.MessageBox.Show("The path that this file was saved to no longer exists. As a result, it cannot be saved. If you wish to save it elsewhere, use 'Save As'. Or, insert the media required for the path to be avalible", "Error saving file", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
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
            ImageE newElement = new ImageE(ImportData(path));

            newElement.Width = newElement.image.Size.Width;
            newElement.Height = newElement.image.Size.Height;

            savedata.imageElements.Add(newElement);
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
