using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABPaint.Tools.Backend
{
    public class SaveData
    {
        public List<Element> imageElements = new List<Element>();
    }

    public static class SaveSystem
    {
        public static SaveData savedata = new SaveData();

        public static void LoadFile(string path)
        {
            System.Windows.Forms.MessageBox.Show(path);
        }

        public static void SaveFile(string path)
        {

        }

        public static void LoadData(string data)
        {
            
        }

        public static void SaveData()
        {

        }
    }
}
