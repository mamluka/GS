using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ConvertDemo
{
    public partial class CustomThreadCount : Form
    {
        private int mThreadCount = 0;

        public CustomThreadCount()
        {
            InitializeComponent();
        }

        public int GetThreadCount()
        {
            return mThreadCount;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            try
            {
                mThreadCount = Int16.Parse(textBoxThreadCount.Text);
            }
            catch (System.Exception ex)
            {
                mThreadCount = 0;
            }
            
            DialogResult = DialogResult.OK;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}
