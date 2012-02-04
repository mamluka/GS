using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CAVEditLib;

namespace ConvertDemo
{
    public partial class ConvertSettings : Form
    {
        private CLogLevel mLogLevel = CLogLevel.cllInfo;
        private string mLogPath = "C:\\Log\\Log.txt";
        private CThreadPriority mThreadPrority = CThreadPriority.ctpLower;
        private int mThreadCount = 1;
        private bool mPreview = false;

        public ConvertSettings()
        {
            InitializeComponent();
        }

        private void groupBoxLogSettings_Enter(object sender, EventArgs e)
        {

        }

        public CLogLevel LogLevel
        {
            get
            {
                return mLogLevel;
            }
        }

        public string LogPath
        {
            get
            {
                return mLogPath;
            }
        }

        public CThreadPriority ThreadPrority
        {
            get
            {
                return mThreadPrority;
            }
        }

        public int ThreadCount
        {
            get
            {
                return mThreadCount;
            }
        }

        public bool Preview
        {
            get
            {
                return mPreview;
            }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {            
            if (radioButtonDebug.Checked)
            {
                mLogLevel = CLogLevel.cllDebug;
            }
            if (radioButtonError.Checked)
            {
                mLogLevel = CLogLevel.cllError;
            }
            if (radioButtonFatal.Checked)
            {
                mLogLevel = CLogLevel.cllFatal;
            }
            if (radioButtonPanic.Checked)
            {
                mLogLevel = CLogLevel.cllPanic;
            }
            if (radioButtonQuiet.Checked)
            {
                mLogLevel = CLogLevel.cllQuiet;
            }
            if (radioButtonVerbose.Checked)
            {
                mLogLevel = CLogLevel.cllVerbose;
            }
            if (radioButtonWarning.Checked)
            {
                mLogLevel = CLogLevel.cllWarning;
            }
            
            mLogPath = textBoxLogPath.Text;
            if (radioButtonIdle.Checked)
            {
                mThreadPrority = CThreadPriority.ctpIdle;
            }
            if (radioButtonLower.Checked)
            {
                mThreadPrority = CThreadPriority.ctpLower;
            }
            if (radioButtonLowest.Checked)
            {
                mThreadPrority = CThreadPriority.ctpLowest;
            }
            if (radioButtonNormal.Checked)
            {
                mThreadPrority = CThreadPriority.ctpNormal;
            }
            try
            {
                mThreadCount = int.Parse(textBoxThreadCount.Text);
            }
            catch (System.Exception ex)
            {
                mThreadCount = 1;
            }
            
            mPreview = radioButtonPreview.Checked;

            DialogResult = DialogResult.OK;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            switch (mLogLevel)
            {
            case CLogLevel.cllDebug:
                radioButtonDebug.Checked = true;
            	break;
            case CLogLevel.cllError:
                radioButtonError.Checked = true;
                break;
            case CLogLevel.cllFatal:
                radioButtonFatal.Checked = true;
                break;
            case CLogLevel.cllInfo:
                radioButtonInfo.Checked = true;
                break;
            case CLogLevel.cllPanic:
                radioButtonPanic.Checked = true;
                break;
            case CLogLevel.cllQuiet:
                radioButtonQuiet.Checked = true;
                break;
            case CLogLevel.cllVerbose:
                radioButtonVerbose.Checked = true;
                break;
            case CLogLevel.cllWarning:
                radioButtonWarning.Checked = true;
                break;
            default:
                break;
            }
            textBoxLogPath.Text = mLogPath;
            textBoxThreadCount.Text = mThreadCount.ToString();
            switch (mThreadPrority)
            {
            case CThreadPriority.ctpIdle:
                radioButtonIdle.Checked = true;
            	break;
            case CThreadPriority.ctpLower:
                radioButtonLower.Checked = true;
                break;
            case CThreadPriority.ctpLowest:
                radioButtonLowest.Checked = true;
                break;
            case CThreadPriority.ctpNormal:
                radioButtonNormal.Checked = true;
                break;
            default:
                break;
            }
            radioButtonPreview.Checked = mPreview;

            DialogResult = DialogResult.Cancel;
        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                textBoxLogPath.Text = dlg.FileName;
            }
        }
    }
}
