using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;


namespace GemScope
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();

            Image img1 = Image.FromFile("c:\\1.jpg");
            
            imageList1.Images.Add(img1);

            listView1.LargeImageList = imageList1;

              

            listView1.Items.Add(new ListViewItem("dfgdfg",0));
            
        }

        private void splitContainer2_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
