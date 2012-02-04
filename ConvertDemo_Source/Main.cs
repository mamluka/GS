using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using CAVEditLib;

namespace ConvertDemo
{
    public partial class Main : Form
    {
        private struct HEADER_NAME
        {
            public string mName;
            public int mWidth;

            public HEADER_NAME(string name, int width)
            {
                mName = name;
                mWidth = width;
            }
        }

        private static HEADER_NAME[] HEADER_NAMES = {
                                new HEADER_NAME("Source File Name", 150), 
                                new HEADER_NAME("File Duration", 80), 
                                new HEADER_NAME("File Size", 70),
                                new HEADER_NAME("Output File Name", 150),
                                new HEADER_NAME("Progress", 70), 
                                new HEADER_NAME("FPS", 50), 
                                new HEADER_NAME("Converted Time", 100), 
                                new HEADER_NAME("Converted File Size", 120)};

        private const int COLUMN_INDEX_SOURCE_FILE_NAME = 0;
        private const int COLUMN_INDEX_SOURCE_FILE_DURATION = 1;
        private const int COLUMN_INDEX_SOURCE_FILE_SIZE = 2;
        private const int COLUMN_INDEX_OUTPUT_FILE_NAME = 3;
        private const int COLUMN_INDEX_CONVERT_PROGRESS = 4;
        private const int COLUMN_INDEX_FPS = 5;
        private const int COLUMN_INDEX_CONVERTED_TIME = 6;
        private const int COLUMN_INDEX_CONVERTED_FILE_SIZE = 7;

        private const string LIBAV_PATH = "\\LibAV";
        private const string CAVEDIT_LIB_NAME = "CAVEditLib.dll";

        private const int DURATION_RATIO = 1000000;

        private const string seed = "Demo";
        private const string key = "Demo";

        private OutputOptions mOptionForm = null;
        private ICAVConverter mConverter = null;

        private string mLogPath = "C:\\Log\\Log.txt";
        private CAVEditLib.CLogLevel mLogLevel = CLogLevel.cllInfo;
        private CAVEditLib.CThreadPriority mThreadPriority = CThreadPriority.ctpLower;
        private int mThreadCount = 1;
        private CustomThreadCount mThreadCountDlg = new CustomThreadCount();
        private ConvertSettings mConvertSettings = new ConvertSettings();
        private bool mPreview = false;

        public Main()
        {
            InitializeComponent();

            for (int i = 0; i < HEADER_NAMES.Length; i++)
            {
                listViewTask.Columns.Add(HEADER_NAMES[i].mName, HEADER_NAMES[i].mWidth);
            }
            mOptionForm = new OutputOptions();
            OpenConverter(true);
            mConverter.SetLicenseKey(seed, key);
            mOptionForm.SetConverter(mConverter);
        }

        private bool OpenConverter(bool first)
        {
            try
            {
                mConverter = new CAVConverter();
            }
            catch (System.Runtime.InteropServices.COMException ex)
            {
                if (first)
                {
                    if (ex.ErrorCode == -2147221164)
                    {
                        if (!RegisterCAVEditLib())
                        {
                            return false;
                        }
                        return OpenConverter(false);
                    }
                    else
                    {
                        richTextBoxLog.Text += "\r\n";
                        richTextBoxLog.Text += "*** " + ex.Message;
                        richTextBoxLog.Text += "\r\n";
                        return false;
                    }
                } 
                else
                {
                    richTextBoxLog.Text += "\r\n";
                    richTextBoxLog.Text += "*** " + ex.Message;
                    richTextBoxLog.Text += "\r\n";
                    return false;
                }
                
            }

            return true;
        }

        private void AddFile(ICInputOptions inputOptions, ICOutputOptions outputOptions)
        {
            int taskIndex = 0;

            if (mConverter == null || inputOptions == null || outputOptions == null)
            {
                return;
            }

            taskIndex = mConverter.AddTask(inputOptions.FileName, outputOptions.FileName);
            if (taskIndex < 0)
            {
                richTextBoxLog.Text += "\r\n";
                richTextBoxLog.Text += "*** Add Task Failed: " + mConverter.LastErrMsg;
                richTextBoxLog.Text += "\r\n";
                richTextBoxLog.Focus();
                richTextBoxLog.Select(richTextBoxLog.TextLength, 0);
                richTextBoxLog.ScrollToCaret();
                return;
            }

            ListViewItem lvItem = listViewTask.Items.Add(inputOptions.FileName);

            lvItem.Tag = taskIndex;

            long duration = mConverter.AVPrope.FileStreamInfo.Duration;
            if (duration > 0)
            {
                lvItem.SubItems.Add((duration / DURATION_RATIO).ToString());
            }
            else
            {
                lvItem.SubItems.Add("N/A");
            }
            lvItem.SubItems.Add(mConverter.AVPrope.FileSize.ToString());
            lvItem.SubItems.Add(outputOptions.FileName);
            lvItem.SubItems.Add("");
            lvItem.SubItems.Add("");
            lvItem.SubItems.Add("");
            lvItem.SubItems.Add("");

            richTextBoxLog.Text += "\r\n";
            richTextBoxLog.Text += "*** File has been added to the convert list";
            richTextBoxLog.Text += "\r\n";
            richTextBoxLog.Focus();
            richTextBoxLog.Select(richTextBoxLog.TextLength, 0);
            richTextBoxLog.ScrollToCaret();

            buttonClearTask.Enabled = true;
            buttonRemoveTask.Enabled = true;
            buttonStartStop.Enabled = true;
        }

        private void timerConvert_Tick(object sender, EventArgs e)
        {
            if (mConverter == null || mThreadCount <= 0)
            {
                return;
            }
            int i = 0;

            for (i = 0; i < mConverter.TasksCount; i++ )
            {
                ICProgressInfo progressInfo = null;

                progressInfo = mConverter.get_ProgressInfo(i);
                OnProgressInfo(progressInfo);

                ICTerminateInfo terminateInfo = null;

                terminateInfo = mConverter.get_TerminateInfo(i);
                OnTerminateInfo(terminateInfo);

                ICCustomHookInfo customHookInfo = null;

                customHookInfo = mConverter.get_CustomHookInfo(i);
                OnCustomHookInfo(customHookInfo);
            }
            
        }

        private void OnProgressInfo(ICProgressInfo progressInfo)
        {
            int taskIndex = progressInfo.TaskIndex;
            long currentDuration = progressInfo.CurrentDuration;
            long totalDuration = progressInfo.TotalDuration;
            long currentFPS = progressInfo.FPS;
            long convertedSize = progressInfo.CurrentSize;

            if (taskIndex < 0)
            {
                //OnConvertCompleted();
                return;
            }
            if (totalDuration > 0)
            {
                listViewTask.Items[taskIndex].SubItems[COLUMN_INDEX_CONVERT_PROGRESS].Text = (currentDuration * 100 / totalDuration).ToString() + "%";
            }
            else
            {
                listViewTask.Items[taskIndex].SubItems[COLUMN_INDEX_CONVERT_PROGRESS].Text = "N/A";
            }

            if (currentFPS > 0)
            {
                listViewTask.Items[taskIndex].SubItems[COLUMN_INDEX_FPS].Text = currentFPS.ToString();
            }

            listViewTask.Items[taskIndex].SubItems[COLUMN_INDEX_CONVERTED_TIME].Text = (currentDuration / 1000000).ToString();
            listViewTask.Items[taskIndex].SubItems[COLUMN_INDEX_CONVERTED_FILE_SIZE].Text = convertedSize.ToString();
        }

        private void OnTerminateInfo(ICTerminateInfo terminateInfo)
        {
            int taskIndex = terminateInfo.TaskIndex;
            bool bFinished = terminateInfo.Finished;
            bool bException = terminateInfo.Exception;
            string exceptionMsg = terminateInfo.ExceptionMsg;

            if (taskIndex < 0)
            {                
                return;
            }
            
            if (bFinished)
            {               
                listViewTask.Items[taskIndex].SubItems[COLUMN_INDEX_CONVERT_PROGRESS].Text = "100%";
            }
            else if (bException)
            {                
                richTextBoxLog.Text += "\r\n";
                richTextBoxLog.Text += "***File load error: " + exceptionMsg;
                richTextBoxLog.Text += "\r\n";
                richTextBoxLog.Focus();
                richTextBoxLog.Select(richTextBoxLog.TextLength, 0);
                richTextBoxLog.ScrollToCaret();
            }
            int completedTask = 0;

            for (int i = 0; i < listViewTask.Items.Count; i++)
            {
                if (listViewTask.Items[i].SubItems[COLUMN_INDEX_CONVERT_PROGRESS].Text == "100%")
                {
                    completedTask++;
                }                
            }
            if (completedTask >= listViewTask.Items.Count)
            {
                OnConvertCompleted();
            }
        }

        private void OnCustomHookInfo(ICCustomHookInfo customHookInfo)
        {

        }

        private void OnConvertCompleted()
        {
            timerConvert.Enabled = false;

            // set status of buttons
            buttonAddTask.Enabled = true;
            buttonClearTask.Enabled = true;

            buttonStartStop.Enabled = false;
            buttonStartStop.Text = "Start";
            buttonPauseResume.Enabled = false;
            buttonPauseResume.Text = "Pause";
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (mConverter != null)
            {
                mConverter.ClearTasks();
            }
            
            listViewTask.Clear();
        }
 
        private void listViewTask_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
            {
                return;
            }

            if (listViewTask.SelectedItems.Count == 1)
            {
                try
                {
                    string filename = listViewTask.SelectedItems[0].SubItems[COLUMN_INDEX_OUTPUT_FILE_NAME].Text;
                    if (System.IO.File.Exists(filename))
                    {
                        System.IO.FileInfo info = new System.IO.FileInfo(filename);
                        if (info.Length > 0)
                        {
                            System.Diagnostics.Process.Start(filename);
                        }                        
                    }                    
                }
                catch (System.ComponentModel.Win32Exception noBrowser)
                {
                    if (noBrowser.ErrorCode == -2147467259)
                        MessageBox.Show(noBrowser.Message);
                }
                catch (System.Exception other)
                {
                    MessageBox.Show(other.Message);
                }
            }           
        }

        private void listViewTask_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                buttonPauseResume_Click(null, null);
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void buttonAddTask_Click(object sender, EventArgs e)
        {
            if (mConverter == null)
            {
                return;
            }
            if (!mConverter.AVLibLoaded())
            {
                string path = null;

                path = Application.StartupPath;
                path += LIBAV_PATH;

                if (!mConverter.LoadAVLib(path))
                {
                    richTextBoxLog.Text += mConverter.LastErrMsg + "\r\n";
                    richTextBoxLog.Focus();
                    richTextBoxLog.Select(richTextBoxLog.TextLength, 0);
                    richTextBoxLog.ScrollToCaret();
                    return;
                }
            }

            OpenFileDialog dlg = new OpenFileDialog();

            if (dlg.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            if (!mConverter.AVPrope.LoadFile(dlg.FileName, ""))
            {
                richTextBoxLog.Text += "\r\n";
                richTextBoxLog.Text += "***File load error: " + mConverter.AVPrope.LastErrMsg;
                richTextBoxLog.Text += "\r\n";
                richTextBoxLog.Focus();
                richTextBoxLog.Select(richTextBoxLog.TextLength, 0);
                richTextBoxLog.ScrollToCaret();
                return;
            }
            try
            {
                mOptionForm.SetConverter(mConverter);
                if (mOptionForm.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }
                mOptionForm.GetInputOptions();
                mOptionForm.GetOutputOptions();
                AddFile(mConverter.InputOptions, mConverter.OutputOptions);
            }
            catch (System.Exception ex)
            {
                richTextBoxLog.Text += "\r\n";
                richTextBoxLog.Text += "*** Error: " + ex.Message;
                richTextBoxLog.Text += "\r\n";
                richTextBoxLog.Focus();
                richTextBoxLog.Select(richTextBoxLog.TextLength, 0);
                richTextBoxLog.ScrollToCaret();
            }
            finally
            {
                mConverter.AVPrope.CloseFile();
            }
        }

        private void buttonRemoveTask_Click(object sender, EventArgs e)
        {
            if (mConverter == null)
            {
                return;
            }
            try
            {
                int i = listViewTask.Items.Count;

                while (i > 0)
                {
                    i--;

                    if (listViewTask.Items[i].Selected)
                    {
                        mConverter.RemoveTask((int)listViewTask.Items[i].Tag);
                        listViewTask.Items.RemoveAt(i);
                    }
                }

                richTextBoxLog.Text += "\r\n";
                richTextBoxLog.Text += "*** Remove Task successfully";
                richTextBoxLog.Text += "\r\n";
                richTextBoxLog.Focus();
                richTextBoxLog.Select(richTextBoxLog.TextLength, 0);
                richTextBoxLog.ScrollToCaret();

                if (listViewTask.Items.Count == 0)
                {
                    buttonStartStop.Enabled = true;
                    buttonPauseResume.Enabled = true;
                }
            }
            catch (System.Exception ex)
            {
                richTextBoxLog.Text += "\r\n";
                richTextBoxLog.Text += "*** Remove Task failed, " + ex.Message;
                richTextBoxLog.Text += "\r\n";
                richTextBoxLog.Focus();
                richTextBoxLog.Select(richTextBoxLog.TextLength, 0);
                richTextBoxLog.ScrollToCaret();
            }
        }

        private void buttonClearTask_Click(object sender, EventArgs e)
        {
            if (mConverter == null)
            {
                return;
            }
            try
            {
                mConverter.ClearTasks();
                listViewTask.Items.Clear();

                richTextBoxLog.Text += "\r\n";
                richTextBoxLog.Text += "*** Clear Tasks successfully";
                richTextBoxLog.Text += "\r\n";
                richTextBoxLog.Focus();
                richTextBoxLog.Select(richTextBoxLog.TextLength, 0);
                richTextBoxLog.ScrollToCaret();

                buttonStartStop.Enabled = true;
                buttonPauseResume.Enabled = true;
            }
            catch (System.Exception ex)
            {
                richTextBoxLog.Text += "\r\n";
                richTextBoxLog.Text += "*** Clear Tasks failed, " + ex.Message;
                richTextBoxLog.Text += "\r\n";
                richTextBoxLog.Focus();
                richTextBoxLog.Select(richTextBoxLog.TextLength, 0);
                richTextBoxLog.ScrollToCaret();
            }
        }

        private void buttonStartStop_Click(object sender, EventArgs e)
        {
            if (mConverter == null)
            {
                return;
            }
            if (mLogPath.StartsWith("C:\\Log"))
            {
                if (!System.IO.Directory.Exists("C:\\Log"))
                {
                    System.IO.Directory.CreateDirectory("C:\\Log");
                }
            }
            if (listViewTask.Items.Count == 0)
            {
                return;
            }

            if (buttonStartStop.Text == "Start")
            {
                // set status of buttons
                if (mThreadCount > 0)
                {
                    buttonAddTask.Enabled = false;
                    buttonRemoveTask.Enabled = false;
                    buttonClearTask.Enabled = false;
                }

                buttonStartStop.Enabled = mThreadCount > 0;
                buttonPauseResume.Enabled = mThreadCount > 0;
                // procedure Start(AThreadCount: Integer);
                // AThreadCount: >  0, means do converting task in thread mode
                //               >  1, means do converting task with multiple files in the same time
                //               <= 0, means do converting task in main thread


                // do converting task in thread mode
                buttonStartStop.Text = "Stop";
                mConverter.ThreadPriority = mThreadPriority;
                mConverter.LogPath = mLogPath;
                mConverter.LogLevel = mLogLevel;
                mConverter.Start(mThreadCount);
                timerConvert.Enabled = true;
            }
            else
            {
                mConverter.Stop(true);
                buttonStartStop.Text = "Start";
                buttonStartStop.Enabled = false;
                buttonPauseResume.Enabled = false;
                buttonClearTask.Enabled = true;
            }            
        }

        private void buttonPauseResume_Click(object sender, EventArgs e)
        {
            if (mConverter == null)
            {
                return;
            }
            if (listViewTask.Items.Count == 0)
            {
                return;
            }

            if (buttonPauseResume.Text == "Pause")
            {
                buttonPauseResume.Text = "Resume";
                mConverter.Pause();

                timerConvert.Enabled = false;
                // set status of buttons
                buttonAddTask.Enabled = false;
                buttonRemoveTask.Enabled = false;
                buttonClearTask.Enabled = false;

                buttonStartStop.Enabled = mThreadCount > 0;
                buttonPauseResume.Enabled = mThreadCount > 0;
            }
            else
            {
                buttonPauseResume.Text = "Pause";
                mConverter.Resume();
                timerConvert.Enabled = true;
            }             
        }

        private void buttonAbout_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("http://www.ffmpeg-activex.com");
            }
            catch (System.ComponentModel.Win32Exception noBrowser)
            {
                if (noBrowser.ErrorCode == -2147467259)
                    MessageBox.Show(noBrowser.Message);
            }
            catch (System.Exception other)
            {
                MessageBox.Show(other.Message);
            } 
        }

        private void buttonSettings_Click(object sender, EventArgs e)
        {
            if (mConverter == null)
            {
                return;
            }
            if (mConvertSettings.ShowDialog() == DialogResult.OK)
            {
                mThreadCount = mConvertSettings.ThreadCount;
                mThreadPriority = mConvertSettings.ThreadPrority;
                mLogLevel = mConvertSettings.LogLevel;
                mLogPath = mConvertSettings.LogPath;
                mPreview = mConvertSettings.Preview;

                if (mConverter != null)
                {
                    mConverter.Preview = mPreview;
                }
            }
        }

        private bool RegisterCAVEditLib()
        {
            bool ret = true;
            try
            {
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();

                string libPath = Application.StartupPath + "\\" + CAVEDIT_LIB_NAME;
                string regsvr32Path = Environment.SystemDirectory +  "\\regsvr32.exe";

                startInfo.Arguments = "\"" + libPath + "\""+ " /s";
                startInfo.FileName = regsvr32Path;

                System.Diagnostics.Process regProcess = System.Diagnostics.Process.Start(startInfo);
                regProcess.WaitForExit();
            }
            catch (Exception e)
            {
                ret = false;
            }

            return ret;
        }
    }
}
