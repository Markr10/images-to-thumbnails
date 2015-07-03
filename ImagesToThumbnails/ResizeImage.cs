using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
                using (var thumbnail = image.GetThumbnailImage(image.Width / 10, image.Height / 10, null, IntPtr.Zero))
                {
                    thumbnail.Save(openFileDialog1.FileName + "thumb.png");
                }
            }
        }
    }
}
