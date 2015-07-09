using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImagesToThumbnails
{
    public partial class ResizeImage : Form
    {
        public ResizeImage()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            string filename = openFileDialog1.FileName;
            using (var image = Image.FromFile(filename))
            {
                Bitmap target = new Bitmap(image.Width / 10, image.Height / 10);
                using (Graphics graphics = Graphics.FromImage(target))
                {
                    graphics.DrawImage(image, 0, 0, target.Width, target.Height);
                    using (FileStream file = new FileStream(openFileDialog1.FileName + "thumb.png", FileMode.Create, System.IO.FileAccess.Write))
                    {
                        target.Save(file, ImageFormat.Png);
                    }
                }
            }
        }
    }
}
