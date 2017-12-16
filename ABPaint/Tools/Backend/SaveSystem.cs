using System;
using System.Collections.Generic;
using System.Drawing;
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

        public static void LoadFile(string path)
        {
            LoadData(System.IO.File.ReadAllText(path));
            currentFile = path;
        }

        public static void SaveFile(string path)
        {
            System.IO.File.WriteAllText(path, SaveData());
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
    }
}
