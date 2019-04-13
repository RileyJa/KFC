using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Importer_Exporter
{
    public partial class Form1 : Form
    {
        Color Red = Color.FromArgb(163, 8, 12);
        string directory = null;
        string[] storage = new string[2];
        Image[] image = new Image[100];
        TextBox[] textBox = new TextBox[100];
        Font FLable = new Font("Times New Roman", 24.0f, FontStyle.Bold);
        Font FName = new Font("Bookman Old Style", 14.0f, FontStyle.Bold);
        Font FPrice = new Font("Bahnschrift", 14.0f);
        int ViewTop = 0;
        int ViewLeft = 0;
        bool i = false;
        public Form1()
        {
            InitializeComponent();
            Width = 1500;
            Height = 800;
            panel2.Dock = DockStyle.Fill;
            panel2.AutoSize = false;
            panel1.BackColor = Red;
            panel2.Height = Height;
            panel2.Width = Width;
            panel2.HorizontalScroll.Enabled = false;
            panel2.HorizontalScroll.Visible = false;
            panel2.AutoScroll = true;
            //MessageBox.Show(Width + "," + panel2.Width);
            //FastTxt(@"X:\KFC\Меню");
            Dir(@"X:\KFC\Меню");
            //MessageBox.Show(Width + "," + panel2.Width + "," + V);


        }
        public void FastTxt(string Direc)
        {
            DirectoryInfo dir = new DirectoryInfo(Direc);
            foreach (var item in dir.GetDirectories())
            {
                var b = item.GetFiles();
                if (b.Length > 0)
                {
                    foreach (var i in item.GetFiles())
                    {
                        string path = item.FullName + @"\" + i.Name + ".txt";
                        if (!File.Exists(path))
                        {
                            File.Create(path);
                            if (!File.Exists(path))
                            {
                                TextWriter tw = new StreamWriter(path);
                                tw.WriteLine("name=" + '"' + '"');
                                tw.WriteLine("image=" + '"' + i.Name + '"');
                                tw.WriteLine("price=");
                                tw.Close();
                            }

                        }
                        else if (File.Exists(path))
                        {
                            TextWriter tw = new StreamWriter(path);
                            tw.WriteLine("name=" + '"' + '"');
                            tw.WriteLine("image=" + '"' + i.Name + '"');
                            tw.WriteLine("price=");
                            tw.Close();
                        }

                    }
                }
                else
                {
                    Panel panel = new Panel();

                    FastTxt(item.FullName);
                }

            }
        }
        //Чтение из Каталоа
        public void Dir(string Direc)
        {
            DirectoryInfo dir = new DirectoryInfo(Direc);
            foreach (var item in dir.GetDirectories())
            {
                var b = item.GetFiles();
                if (b.Length > 0)
                {
                    if(i)
                    {
                        ViewTop += panel2.Width / 5 + 60;
                        ViewLeft = 0;
                    }
                    
                    Label label = new Label();
                    label.Text = item.Name;
                    label.AutoSize = false;
                    label.Font = FLable;
                    label.Width = panel2.Width;
                    label.Height = 40;
                    label.Top = ViewTop;
                    label.Left = 0;
                    ViewLeft = 0;
                    ViewTop += label.Height;
                    i = true;
                    panel2.Controls.Add(label);
                    foreach (var i in item.GetFiles())
                    {

                        StringBuilder sb = new StringBuilder();

                        try
                        {
                            using (FileStream fs = i.OpenRead())
                            {
                                string s = null;
                                byte[] bit = new byte[8];



                                while (fs.Read(bit, 0, bit.Length) > 0)
                                {
                                    sb.Append(Encoding.Default.GetString(bit));
                                }
                            }
                            IniFile iniFile = new IniFile(sb.ToString());
                            if (iniFile.Name.Length > 0)
                            {
                                Push(iniFile, item);
                            }
                        }
                        catch (Exception)
                        {

                        }
                    }
                }
                else
                {
                    Panel panel = new Panel();

                    Dir(item.FullName);
                }

            }
        }
        //Отправка в БД и Рисовка
        public void Push(IniFile ini, DirectoryInfo info)
        {
            int Width = panel2.Width / 5;
            using (CatalogModel db = new CatalogModel())
            {
                string folder = info.FullName + @"\" + ini.Image;
                //MessageBox.Show(folder);
                //Process.Start("explorer.exe", folder);
                Image image = Image.FromFile(folder);

                PictureBox picture = new PictureBox();
                picture.Image = image;
                picture.SizeMode = PictureBoxSizeMode.StretchImage;
                picture.Width = Width;
                picture.Height = Width;
                picture.Left = ViewLeft;
                picture.Top = ViewTop;
                picture.BorderStyle = BorderStyle.FixedSingle;
                panel2.Controls.Add(picture);

                //Название
                Label textBox = new Label();
                textBox.Text = ini.Name;
                textBox.Font = FName;
                textBox.Width = Width;
                textBox.Top = ViewTop + Width;
                textBox.Left = ViewLeft;
                textBox.TextAlign = ContentAlignment.MiddleCenter;
                textBox.BorderStyle = BorderStyle.FixedSingle;

                panel2.Controls.Add(textBox);

                //Цена
                Label tb = new Label();
                
                tb.Text = ini.Price.ToString() +'\u20BD';
                tb.Font = FPrice;
                tb.Left = ViewLeft;
                tb.Width = Width;
                tb.TextAlign = ContentAlignment.MiddleCenter;
                tb.Top = ViewTop + Width + textBox.Height;
                tb.BorderStyle = BorderStyle.FixedSingle;

                panel2.Controls.Add(tb);
                ViewLeft += Width;
                if ((ViewLeft + 5) >= panel2.Width)
                {
                    ViewTop += Width + tb.Height + textBox.Height;
                    ViewLeft = 0;
                }

                Catalog catalog = new Catalog();
                MemoryStream ms = new MemoryStream();
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                byte[] jpg = ms.ToArray();
                catalog.image = jpg;
                catalog.Name = ini.Name;
                catalog.Price = ini.Price;
                catalog.Folder = info.FullName;
                //db.Catalog.Add(catalog);
                //db.SaveChanges();
            }
        }
        private void Add()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                FileInfo fi = new FileInfo(ofd.FileName);
                MessageBox.Show(fi.FullName);
            }
        }
        private void BtnWrite_Click(object sender, EventArgs e)
        {
            DialogResult dialog = MessageBox.Show("Вы действительно хотите записать каталог в БД?", "Предупреждение", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dialog == DialogResult.Yes)
            {
                Dir(@"X:\Каталог");
                MessageBox.Show("Запись в базу завершена");
            }
        }

        private void BtnRead_Click(object sender, EventArgs e)
        {
            Parallel.Invoke(() => ChangeDirect());
        }

        public void ChangeDirect()
        {
            using (CatalogModel db = new CatalogModel())
            {
                Catalog cat = new Catalog();
                cat = db.Catalog.FirstOrDefault(c => c.Folder != null);
                directory = cat.Folder;
                char ch = '\u005C';
                string[] folder = cat.Folder.Split(ch);
                storage[0] = folder[0];
                storage[1] = folder[0];

                //Изменение основной папки
                if (MessageBox.Show("Вы хотите изменить деректорию сохранения?", "",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    FolderBrowserDialog fbd = new FolderBrowserDialog();
                    DialogResult dialog = fbd.ShowDialog();
                    if (!string.IsNullOrEmpty(fbd.SelectedPath))
                    {
                        storage[1] = fbd.SelectedPath;
                        //MessageBox.Show(storage[1]);
                    }
                }
                //Создание папок с файлами
                foreach (var item in db.Catalog.Where(c => c.Folder != null))
                {
                    directory = item.Folder;
                    directory = directory.Replace(storage[0], storage[1]);
                    Directory.CreateDirectory(directory);
                    using (var str = new MemoryStream(item.image))
                    {
                        Image image = Image.FromStream(str);
                        image.Save(directory + @"\" + item.Name + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                    }
                    string path = directory + @"\text.txt";
                    if (!File.Exists(path))
                    {
                        File.Create(path);
                        if (!File.Exists(path))
                        {
                            TextWriter tw = new StreamWriter(path);
                            tw.WriteLine("name=" + '"' + item.Name + '"');
                            tw.WriteLine("image=" + '"' + item.Name + '"');
                            tw.WriteLine("price=" + item.Price);
                            tw.Close();
                        }
                    }
                    else if (File.Exists(path))
                    {
                        TextWriter tw = new StreamWriter(path);
                        tw.WriteLine("name=" + '"' + item.Name + '"');
                        tw.WriteLine("image=" + '"' + item.Name + '"');
                        tw.WriteLine("price=" + item.Price);
                        tw.Close();
                    }

                }


            }
            MessageBox.Show("Запись завершена");
        }


    }

    public class IniFile
    {
        /// <summary>
        /// Разбиение свойств
        /// </summary>
        /// <param name="sb"></param>
        public IniFile(string sb)
        {
            //sb.Replace(" ", "");
            char[] Delete = { '"' };
            if (sb[0] == 'n')
            {
                string[] data = sb.Split('=', '\n');
                Name = data[1].Replace('"', ' ').TrimEnd('\r', '\n').ToUpper();
                Image = data[3].Replace('"', ' ').Replace(" ", "").TrimEnd('\r', '\n');
                Price = Convert.ToInt32(data[5].Replace('"', ' ').Replace(" ", "").TrimEnd('\r', '\n'));
            }
        }
        public string Name;
        public string Image;
        public int Price;
        public string Folder;
    }
}
