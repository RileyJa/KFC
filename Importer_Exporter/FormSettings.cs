using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Importer_Exporter
{
    public partial class FormSettings : Form
    {
        public FormSettings()
        {
            StringBuilder sb = new StringBuilder();
            InitializeComponent();
            BackColor = ColorTranslator.FromHtml("#ab1d37");
            if (!File.Exists("Settings.txt"))
            {
                File.Create("Settings.txt");
                if (File.Exists("Settings.txt"))
                {
                    Thread.Sleep(10);
                    using (FileStream fs = File.OpenRead("Settings.txt"))
                    {

                        string s = null;
                        byte[] bit = new byte[8];
                        while (fs.Read(bit, 0, bit.Length) > 0)
                        {
                            sb.Append(Encoding.Default.GetString(bit));
                        }
                    }
                }
            }
            else
            {
                using (FileStream fs = File.OpenRead("Settings.txt"))
                {
                    string s = null;
                    byte[] bit = new byte[32];
                    
                    while (fs.Read(bit, 0, bit.Length) > 0)
                    {
                        sb.Append(Encoding.UTF8.GetString(bit));
                    }
                }
            }
            maskedTextBox1.Text = sb.ToString();
        }

        private void BtnSettings_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            DialogResult dialog = fbd.ShowDialog();
            if (!string.IsNullOrEmpty(fbd.SelectedPath))
            {
                if (!File.Exists("Settings.txt"))
                {
                    File.Create("Settings.txt");
                    if (File.Exists("Settings.txt"))
                    {
                        TextWriter tw = new StreamWriter("Settings.txt", false);
                        tw.WriteLine(fbd.SelectedPath);
                        tw.Close();
                    }
                }
                else
                {
                    TextWriter tw = new StreamWriter("Settings.txt", false);
                    tw.WriteLine(fbd.SelectedPath);
                    tw.Close();
                }

                using (FileStream fs = File.OpenRead("Settings.txt"))
                {

                    string s = null;
                    byte[] bit = new byte[32];
                    while (fs.Read(bit, 0, bit.Length) > 0)
                    {
                        sb.Append(Encoding.UTF8.GetString(bit));
                    }
                }
                maskedTextBox1.Text = sb.ToString();
                //MessageBox.Show(storage[1]);
            }
        }

        
    }
}
