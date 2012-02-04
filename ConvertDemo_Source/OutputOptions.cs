using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using stdole;
using CAVEditLib;

namespace ConvertDemo
{
    public partial class OutputOptions : Form
    {
        private const int AV_TIME_BASE = 1000000;
        private static ulong AV_NOPTS_VALUE = 0x8000000000000000;
        private const string AUDIO_FILTER = "aac ac3 mp2 mp3 wav wma";
        private const string VIDEO_FILTER_DVD = ".vcd.svcd.dvd";

        private const string HOOK_TEXT = "CAVEDIT";
        private const string HOOK_PATH = "\\vhook\\";
        private const string IMLIB2_DLL = "imlib2.dll";
        private const string IMLIB2_IMG = "imlib2.jpg";
        private const string IMLIB2_DELPHI_DLL = "libImlib2-1.dll";
        private const string FREE_TYPE_DLL = "libfreetype-6.dll";
        private const string WATER_MARK_DLL = "watermark.dll";
        private const string WATER_MARK_IMG = "watermark.gif";
        private const string DELPHI_HOOK_DLL = "DelphiHook.dll";
        private const string DELPHI_HOOK_IMG = "DelphiHook.bmp";

        private const string SAME_AS_INPUT = "<same as input>";
        private const string VIEW_ACTUAL = "Click to view current picture in actual size";

        private const int MAX_VCD_VIDEO_BITRATE = 1150000;
        private const int MAX_SVCD_VIDEO_BITRATE = 1500000;
        private const int MAX_MPEG2_VIDEO_BITRATE = 8000000;
        private const int MAX_MP4_VIDEO_BITRATE = 600000;

        private const int MAX_AVI_VIDEO_BITRATE = 8000000;

        private enum HOOK_TYPE { NONE, IMLIB2, DELPHI };

        private string[] OUTPUT_FORMATS = new string[] {  
                                                        "AVI",
                                                        "MOV",
                                                        "RM10",
                                                        "M1V",
                                                        "MP2",
                                                        "SVCD",
                                                        "FLV",
                                                        "MP4",
                                                        "RM20",
                                                        "M2V",
                                                        "MP3",
                                                        "DVD",
                                                        "iPod",
                                                        "OGG",
                                                        "SWF",
                                                        "AAC",
                                                        "WAV",
                                                        "3GP",
                                                        "MKV",
                                                        "PSP",
                                                        "WMV",
                                                        "AC3",
                                                        "WMA"};

        private static int DEFAULT_OUTPUT_FORMAT_INDEX = 12;

        private static string[] NORMS = new string[] { "Auto", "PAL", "NTSC" };

        private static string[] AUDIO_CHANNELS = new string[] { "Auto", "Mono", "Stereo" };

        private static string[] AUDIO_SAMPLE_RATES = new string[] { "Auto", "8000", "11025", "22050", "44100" };

        private static string[] FRAME_SIZES = {
                                                "Auto - Original",
                                                "128x96 - sqcif",
                                                "176x144 - qcif",
                                                "352x288 - cif",
                                                "704x576 - 4cif",
                                                "160x120 - qqvga",
                                                "320x240 - qvga",
                                                "640x480 - vga",
                                                "800x600 - svga",
                                                "1024x768 - xga",
                                                "1600x1200 - uxga",
                                                "2048x1536 - qxga",
                                                "1280x1024 - sxga",
                                                "2560x2048 - qsxga",
                                                "5120x4096 - hsxga",
                                                "852x480 - wvga",
                                                "1366x768 - wxga",
                                                "1600x1024 - wsxga",
                                                "1920x1200 - wuxga",
                                                "2560x1600 - woxga",
                                                "3200x2048 - wqsxga",
                                                "3840x2400 - wquxga",
                                                "6400x4096 - whsxga",
                                                "7680x4800 - whuxga",
                                                "320x200 - cga",
                                                "640x350 - ega",
                                                "852x480 - hd480",
                                                "1280x720 - hd720",
                                                "1920x1080 - hd1080" };

        private static string[] VIDEO_FRAME_RATES = new string[] { "Auto", "10", "15", "25", "30" };

        private static string[] ASPECT_RATIOS = new string[] { "Auto", "4:3", "16:9", "37:20", "47:20" };

        private static string[] AUDIO_ENCODERS = new string[] { "Auto", "AAC", "AC3", "FLAG MP3" };

        private static string[] VIDEO_ENCODERS = new string[] { "Auto", "DivX", "XviD", "X264" };

        private static string[] VIDEO_FILTERS = new string[] { "Auto", "Vertically Flip", "Horizontally Flip", "Negate", "Scale", "Rotate", "SetPTS" };

        private static string[] VIDEO_BITRATES = new string[] {
            "Auto - Original", "100", "200", "400", "800", "1600", "3200", "6400", "12800"
        };

        private static string[] AUDIO_BITRATES = new string[] {
            "Auto - Original", "32", "64", "128", "192"
        };

        private const int SCALE_INDEX = 4;
        private const int ROTATE_INDEX = 5;
        private const int PTS_INDEX = 6;
        private const string DEFAULT_SCALE = "320:240";
        private const string DEFAULT_ROTATE = "45";
        private const string DEFAULT_PTS = "0.5*PTS";

        private ICAVConverter mConverter = null;
        private long mDuration = 0;
        private int mFrames = 0;
        private bool mChanging = false;
        private IPictureDisp mPictureDisp = null;
        private long mCurrentPTS = 0;
        private long mFirstPTS = long.MaxValue;
        private long mStartTime = long.MinValue;
        private long mEndTime = (long)AV_NOPTS_VALUE;
        private int mLastPosition = 0;

        // NOTICE: this demo application only shows first video stream &&/or first audio stream.
        private int mVideoStreamIndex = 0;
        private int mAudioStreamIndex = 0;


        private string mOutputFolder = null;

        private HOOK_TYPE mHookType = HOOK_TYPE.NONE;

        public OutputOptions()
        {
            InitializeComponent();

            foreach (string item in OUTPUT_FORMATS)
            {
                comboBoxOuputFormat.Items.Add(item);
            }
            comboBoxOuputFormat.SelectedIndex = DEFAULT_OUTPUT_FORMAT_INDEX;

            foreach (string item in NORMS)
            {
                comboBoxNorm.Items.Add(item);
            }
            comboBoxNorm.SelectedIndex = 0;

            foreach (string item in AUDIO_CHANNELS)
            {
                comboBoxAudioChannel.Items.Add(item);
            }
            comboBoxAudioChannel.SelectedIndex = 0;

            foreach (string item in AUDIO_SAMPLE_RATES)
            {
                comboBoxAudioSampleRate.Items.Add(item);
            }
            comboBoxAudioSampleRate.SelectedIndex = 0;

            foreach (string item in FRAME_SIZES)
            {
                comboBoxVideoFrameSize.Items.Add(item);
            }
            comboBoxVideoFrameSize.SelectedIndex = 0;

            foreach (string item in VIDEO_FRAME_RATES)
            {
                comboBoxVideoFrameRate.Items.Add(item);
            }
            comboBoxVideoFrameRate.SelectedIndex = 0;

            foreach (string item in ASPECT_RATIOS)
            {
                comboBoxAspectRatio.Items.Add(item);
            }
            comboBoxAspectRatio.SelectedIndex = 0;

            foreach (string item in AUDIO_ENCODERS)
            {
                comboBoxAudioEncoder.Items.Add(item);
            }
            comboBoxAudioEncoder.SelectedIndex = 0;

            foreach (string item in VIDEO_ENCODERS)
            {
                comboBoxVideoEncoder.Items.Add(item);
            }
            comboBoxVideoEncoder.SelectedIndex = 0;

            foreach (string item in VIDEO_FILTERS)
            {
                comboBoxVideoFilter.Items.Add(item);
            }
            comboBoxVideoFilter.SelectedIndex = 0;

            foreach (string item in VIDEO_BITRATES)
            {
                comboBoxVideoBitrate.Items.Add(item);
            }
            comboBoxVideoBitrate.SelectedIndex = 0;

            foreach (string item in AUDIO_BITRATES)
            {
                comboBoxAudioBitrate.Items.Add(item);
            }
            comboBoxAudioBitrate.SelectedIndex = 3;
            
            string hookDLL = Application.StartupPath + HOOK_PATH + DELPHI_HOOK_DLL;
            if (File.Exists(hookDLL))
            {
                mHookType = HOOK_TYPE.DELPHI;
            }

            hookDLL = Application.StartupPath + HOOK_PATH + IMLIB2_DLL;

            if (File.Exists(hookDLL))
            {
                mHookType = HOOK_TYPE.IMLIB2;
            }
            
            if (mHookType == HOOK_TYPE.NONE)
            {
                textBoxMixImage.Enabled = false;
                buttonBrowseMixImage.Enabled = false;
                textBoxMixText.Enabled = false;
            }
            else
            {
                textBoxMixImage.Enabled = true;
                buttonBrowseMixImage.Enabled = true;
                textBoxMixText.Enabled = true;
            }

            hookDLL = Application.StartupPath + HOOK_PATH + WATER_MARK_DLL;
            if (!File.Exists(hookDLL))
            {
                textBoxWaterMark.Enabled = false;
                buttonBrowseWaterMark.Enabled = false;
            }
            else
            {
                textBoxWaterMark.Enabled = true;
                buttonBrowseWaterMark.Enabled = true;
            }
        }

        public void SetConverter(ICAVConverter Converter)
        {
            ClearFrameView();
            mConverter = Converter;
            richTextBoxMediaInfo.Text = mConverter.AVPrope.FileInfoText;
            // NOTICE: this demo application only shows first video stream &&/or first audio stream.
            mVideoStreamIndex = mConverter.AVPrope.FirstVideoStreamIndex;
            mAudioStreamIndex = mConverter.AVPrope.FirstAudioStreamIndex;

            // whether we can seek or not
            trackBarPreview.Enabled = (mConverter.AVPrope.FileStreamInfo.Duration != (long)AV_NOPTS_VALUE) && (mConverter.AVPrope.FormatName != "image2");

            // file duration
            if (mConverter.AVPrope.FileStreamInfo.Duration != (long)AV_NOPTS_VALUE)
                mDuration = mConverter.AVPrope.FileStreamInfo.Duration;

            // calculate Frames of the first video stream, use it for the track bar max position
            if ((mConverter.AVPrope.VideoStreamCount > 0) && (mConverter.AVPrope.FirstVideoStreamInfo.FrameRate.den > 0))
            {
                if (mConverter.AVPrope.FirstVideoStreamInfo.DurationScaled > 0)
                    mFrames = (int)Math.Round((double)mConverter.AVPrope.FirstVideoStreamInfo.DurationScaled / AV_TIME_BASE *
                                mConverter.AVPrope.FirstVideoStreamInfo.FrameRate.num / mConverter.AVPrope.FirstVideoStreamInfo.FrameRate.den);
                else if (mConverter.AVPrope.FileStreamInfo.Duration != (long)AV_NOPTS_VALUE)
                    mFrames = (int)Math.Round((double)mConverter.AVPrope.FileStreamInfo.Duration / AV_TIME_BASE *
                                mConverter.AVPrope.FirstVideoStreamInfo.FrameRate.num / mConverter.AVPrope.FirstVideoStreamInfo.FrameRate.den);
            }
            else if (mConverter.AVPrope.FileStreamInfo.Duration != (long)AV_NOPTS_VALUE)
                mFrames = (int)Math.Round((double)mConverter.AVPrope.FileStreamInfo.Duration / AV_TIME_BASE * 10);

            // if video stream available, then read && draw the first frame
            if (mConverter.AVPrope.VideoStreamCount > 0)
                ReadAndDrawNextFrame();

            // setup track bar for seeking
            SetupTrackBar();
        }

        public void GetInputOptions()
        {
            ICInputOptions inputOptions = mConverter.InputOptions;

            inputOptions.Init();

            // please refer to the relational document for detail information of ICInputOptions

            inputOptions.FileName = mConverter.AVPrope.FileName;
            inputOptions.FileFormat = mConverter.AVPrope.ForceFormat; // !!!*RECOMMEND*!!!


            // cut a piece clip, see also outputOptions.TimeLength
#if _QUICK_CUT
            if (mEndTime > mStartTime)
                inputOptions.TimeStart = GetClipStartTime(); // start time offset
#endif


            // inputOptions.ExtOptions usage: extent options for flexibility
            // Format: name1=value1<CRLF>name2=value2<CRLF>...nameN=valueN<CRLF>
            //         name && value correspond to ffmpeg.exe's parameters
            //         e.g. "pix_fmt=yuv422p<CRLF>aspect=16:9<CRLF>"
            // you can use this options according to your special purpose
            if (AUDIO_FILTER.IndexOf(comboBoxOuputFormat.SelectedItem as string) >= 0)
            {
                inputOptions.ExtOptions = "vn=1"; // disable input video
            }
        }

        public void GetOutputOptions()
        {
            ICOutputOptions outputOptions = mConverter.OutputOptions;
            string fileExt;
            string temp;
            string baseName;
            string videoCodec;
            string audioCodec;
            int i;
            string hookDLL;
            string hookImg;
            string delimiter;
            string x, y;
            Bitmap lbitmap;

            outputOptions.Init();

            // please refer to the relational document for detail information of ICOutputOptions

            // here we use FileExt to indicate the output format
            fileExt = "." + (comboBoxOuputFormat.SelectedItem as string).ToLower();

            // Audio Channels
            if (comboBoxAudioChannel.SelectedIndex > 0)
            {
                if (comboBoxAudioChannel.SelectedItem == "Mono")
                {
                    outputOptions.AudioChannels = 1;
                }
                else
                {
                    outputOptions.AudioChannels = 2;
                }
            }

            // Audio Sample Rate: Hz value
            if (comboBoxAudioSampleRate.SelectedIndex > 0)
                outputOptions.AudioSampleRate = MyUtils.StrToIntWithDefValue(comboBoxAudioSampleRate.SelectedItem as string, 22050);

            // Audio Volume          
            outputOptions.AudioVolume = MyUtils.StrToIntWithDefValue(textBoxAudioVolume.Text, -1);

            // Video Frame Size: WxH or abbreviation
            if (comboBoxVideoFrameSize.SelectedIndex > 0)
            {
                temp = (string)comboBoxVideoFrameSize.SelectedItem;
                if (temp.IndexOf(' ') > 1)
                    temp = temp.Substring(0, temp.IndexOf(' '));
                outputOptions.FrameSize = temp;
            }

            // Frame Rate(FPS): Hz value, fraction or abbreviation
            if (comboBoxVideoFrameRate.SelectedIndex > 0)
                outputOptions.FrameRate = comboBoxVideoFrameRate.SelectedItem as string;

            // Aspect Ratio: 4:3, 16:9 or 1.3333, 1.7777
            if (comboBoxAspectRatio.SelectedIndex > 0)
                outputOptions.FrameAspectRatio = comboBoxAspectRatio.SelectedItem as string;

            // Video Bit Rate: in bits/s, bps!!! not Kbps!!! so multiply 1000
            if (comboBoxVideoBitrate.Text.Equals("Auto - Original"))
            {
                outputOptions.VideoBitrate = mConverter.AVPrope.FirstVideoStreamInfo.BitRate;
            }
            else
            {
                // NOTICE: by default, ffmpeg will make video bitrate as 200*1000, so here if you want to
                //    keep the original value, you must explicitly set it.
                // please refer to ffmpeg's libavcodec/utils.c, "b" of options[], AV_CODEC_DEFAULT_BITRATE
                // example to keep the original video bitrate
                // outputOptions.VideoBitrate = FAVPrope.VideoStreamInfos[FVideoStreamIndex].BitRate;
                outputOptions.VideoBitrate = 1000 * MyUtils.StrToIntWithDefValue(comboBoxVideoBitrate.Text, 200);
            }

            // Audio Bit Rate: in bits/s, bps!!! not Kbps!!! so multiply 1000
            if (comboBoxAudioBitrate.Text.Equals("Auto - Original"))
            {
                outputOptions.AudioBitrate = mConverter.AVPrope.FirstAudioStreamInfo.BitRate;
            }
            else
            {
                // NOTICE: by default, ffmpeg will make audio bitrate as 64*1000, so here if you want to
                //    keep the original value, you must explicitly set it.
                // please refer to ffmpeg's libavcodec/utils.c, "ab" of options[]
                // example to keep the original audio bitrate
                // outputOptions.AudioBitrate = FAVPrope.AudioStreamInfos[FAudioStreamIndex].BitRate;
                outputOptions.AudioBitrate = 1000 * MyUtils.StrToIntWithDefValue(comboBoxAudioBitrate.Text, 64);
            }
            
            // Video Codec(Encoder)
            if (comboBoxVideoEncoder.SelectedIndex > 0)
            {
                videoCodec = (comboBoxVideoEncoder.SelectedItem as string).ToLower();
                if (videoCodec == "divx")
                {
                    outputOptions.VideoCodec = "mpeg4";      //*{*/Do not Localize}
                    outputOptions.VideoTag = "DIVX";         //*{*/Do not Localize}
                }
                else if (videoCodec == "xvid")
                    outputOptions.VideoCodec = "libxvid";      ////{Do not Localize}
                else if (videoCodec == "x264")
                {
                    outputOptions.VideoCodec = "libx264";     ////{Do not Localize}
                    outputOptions.VideoPreset = "libx264-default";
                }
                // TODO: add more video codecs option
            }

            // Audio Codec(Encoder)
            if (comboBoxAudioEncoder.SelectedIndex > 0)
            {
                audioCodec = (comboBoxAudioEncoder.SelectedItem as string).ToLower();
                if (audioCodec == "aac")
                    outputOptions.AudioCodec = "libfaac";     ////{Do not Localize}
                else if (audioCodec == "ac3")
                    outputOptions.AudioCodec = "ac3";          ////{Do not Localize}
                else if (audioCodec == "flac")
                    outputOptions.AudioCodec = "flac";         ////{Do not Localize}
                else if (audioCodec == "mp3")
                    outputOptions.AudioCodec = "libmp3lame";  ////{Do not Localize}
                // TODO: add more audio codecs option
            }

            // cut a piece clip
#if !_QUICK_CUT
            if (mEndTime > mStartTime)
                outputOptions.TimeStart = GetClipStartTime(); // start time offset
#endif


            // cut a piece clip, see also IO.StartTime
            if (mEndTime > mStartTime)
                outputOptions.TimeLength = GetClipTimeLength(); // duration of the new piece clip

            // **********Below code shows Video Hook usage*************

            // here we use ExtOptions to define vhook parameters
            // outputOptions.ExtOptions usage: extent options for flexibility
            // Format: name1=value1<CRLF>name2=value2<CRLF>...nameN=valueN<CRLF>
            //         name && value correspond to ffmpeg.exe's parameters
            //         e.g. "pix_fmt=yuv422p<CRLF>aspect=16:9<CRLF>"

            // one or more vhooks can be defined in the same time.

            // Official Video Hook: please refer to http://ffmpeg.mplayerhq.hu/hooks.html
            // NOTICE: if vhook parameters(HookDLL or FileName) have SPACE characters,
            //         the delimiter between vhook parameters must be set to '|'.
            //         this feature is an increment to original FFmpeg's vhook.

            // Imlib2 Video Hook comes from official ffmpeg
            if (mHookType == HOOK_TYPE.IMLIB2 && !string.IsNullOrEmpty(textBoxMixImage.Text))
            { // Imlib2 Hook (Image usage)
                hookDLL = Application.StartupPath + HOOK_PATH + IMLIB2_DLL;
                hookImg = textBoxMixImage.Text;
                if ((hookDLL.IndexOf(' ') > 0) || (hookImg.IndexOf(' ') > 0))
                    delimiter = "|"; ////{Do not Localize}
                else
                    delimiter = " ";
                if (checkBoxImageDancing.Checked)
                {
                    x = "(W-w)*(0.5+0.5*sin(N/47*PI))";
                    y = "(H-h)*(0.5+0.5*cos(N/97*PI))";
                }
                else
                {
                    x = "W-w";
                    y = "H-h";
                }
                outputOptions.ExtOptions = outputOptions.ExtOptions +
                    string.Format("vhook={0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}{12}\r\n",
                        hookDLL, delimiter,
                        "-i", delimiter, hookImg, delimiter,
                        "-x", delimiter, x, delimiter,
                        "-y", delimiter, y);
            }
            if (mHookType == HOOK_TYPE.IMLIB2 && !string.IsNullOrEmpty(textBoxMixText.Text))
            {   // Imlib2 Hook (Text usage)
                hookDLL = Application.StartupPath + HOOK_PATH + IMLIB2_DLL;
                if (hookDLL.IndexOf(' ') > 0)
                    delimiter = "|"; ////{Do not Localize}
                else
                    delimiter = " ";
                if (checkBoxMixTextScrolling.Checked)
                {
                    x = "10";
                    //      Y = 'abs(H-25-1.0*N)';
                    y = "(H-25)*(0.5+0.50*cos(N/97*PI))";
                }
                else
                {
                    x = "10";
                    y = "10";
                }
                textBoxMixText.Text = textBoxMixText.Text.Trim();
                if (textBoxMixText.Text == "")
                    textBoxMixText.Text = HOOK_TEXT;
                outputOptions.ExtOptions = outputOptions.ExtOptions +
                    string.Format("vhook={0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}{12}{13}{14}{15}{16}{17}{18}{19}{20}{21}{22}{23}{24}{25}{26}{27}{28}\r\n",
                        hookDLL, delimiter,
                        "-R", delimiter, "255", delimiter,
                        "-G", delimiter, "255", delimiter,
                        "-B", delimiter, "255", delimiter,
                        "-F", delimiter, "Tahoma.ttf/16", delimiter,  //{Imlib2 Hook need fontfile to draw text}
                        "-t", delimiter, textBoxMixText.Text, delimiter,
                        "-x", delimiter, x, delimiter,
                        "-y", delimiter, y);
            }

            // WaterMark Video Hook comes from official ffmpeg
            if (!string.IsNullOrEmpty(textBoxWaterMark.Text))
            {
                hookDLL = Application.StartupPath + HOOK_PATH + WATER_MARK_DLL;
                hookImg = textBoxWaterMark.Text;
                if ((hookDLL.IndexOf(' ') > 0) || (hookImg.IndexOf(' ') > 0))
                    delimiter = "|"; //{Do not Localize}
                else
                    delimiter = " ";
                outputOptions.ExtOptions = outputOptions.ExtOptions +
                    string.Format("vhook={0}{1}{2}{3}{4}{5}{6}{7}{8:D}\r\n",
                        hookDLL, delimiter,
                        "-f", delimiter, hookImg, delimiter,
                        "-m", delimiter, checkBoxWaterMarkMode.Checked ? 1 : 0);
            }

            // Delphi Video Hook: Draw Image Overlay &&/or Draw Text Overlay
            //    it is written in Delphi Language.
            //    Delphi Source of DelphiHook.dll is available in the full source version component.
            //    Delphi Source of DelphiNull.dll is available in all version component.
            //    in fact, you can write your own Video Hook DLLs in any develop language.
            // Image Overlay parameters
            //    -i image filename (*.bmp 24b format)
            //    -ix image x
            //    -iy image y
            //    -mc RRGGBB (transparent color, optional)
            // Text Overlay parameters
            //    -t text
            //    -tx text x
            //    -ty text y
            //    -fc RRGGBB
            //    -fn fontname
            //    -fs fontsize
            //    -fst BIUS {one or all of Bold, Italic, Underline, StrikeOut}
            if (mHookType == HOOK_TYPE.DELPHI && !string.IsNullOrEmpty(textBoxMixImage.Text))
            { // Delphi Hook (Image usage)
                hookDLL = Application.StartupPath + HOOK_PATH + DELPHI_HOOK_DLL;
                hookImg = textBoxMixImage.Text;
                lbitmap = null;

                try
                {
                    if (File.Exists(hookImg))
                        lbitmap = new Bitmap(hookImg);
                    else
                    {
                        lbitmap = new Bitmap(64, 64);
                    }
                    if ((hookDLL.IndexOf(' ') > 0) || (hookImg.IndexOf(' ') > 0))
                        delimiter = "|"; //{Do not Localize}
                    else
                        delimiter = " ";
                    outputOptions.ExtOptions = outputOptions.ExtOptions +
                        string.Format("vhook={0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}{12}{13}{14}{15}{16}\r\n",
                            hookDLL, delimiter,
                            "-i", delimiter, hookImg, delimiter,
                            "-mc", delimiter, "FFFFFF", delimiter,
                            "-ix", delimiter, (mConverter.AVPrope.get_VideoStreamInfos(mVideoStreamIndex).Width - lbitmap.Width - 10).ToString(), delimiter,
                            "-iy", delimiter, (mConverter.AVPrope.get_VideoStreamInfos(mVideoStreamIndex).Height - lbitmap.Height - 10).ToString());
                }
                catch
                {
                }
                finally
                {
                    lbitmap.Dispose();
                }
            }

            if (mHookType == HOOK_TYPE.DELPHI && !string.IsNullOrEmpty(textBoxMixText.Text))
            { // Delphi Hook (Text usage)
                hookDLL = Application.StartupPath + HOOK_PATH + DELPHI_HOOK_DLL;
                if (hookDLL.IndexOf(' ') > 0)
                    delimiter = "|"; //{Do not Localize}
                else
                    delimiter = " ";
                textBoxMixText.Text = textBoxMixText.Text.Trim();
                if (textBoxMixText.Text == "")
                    textBoxMixText.Text = HOOK_TEXT;
                outputOptions.ExtOptions = outputOptions.ExtOptions +
                    string.Format("vhook={0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}{12}{13}{14}{15}{16}{17}{18}{19}{20}{21}{22}{23}{24}{25}{26}{27}{28}\r\n",
                        hookDLL, delimiter,
                        "-t", delimiter, textBoxMixText.Text, delimiter,
                        "-tx", delimiter, "10", delimiter,
                        "-ty", delimiter, "10", delimiter,
                        "-fc", delimiter, "FFFFFF", delimiter,
                        "-fn", delimiter, "Tahoma", delimiter,
                        "-fs", delimiter, "16", delimiter,
                        "-fst", delimiter, "BU");
            }

            // whether enable OnCustomHook event, this feature is an increment to
            //        create your onw custom video hook behavior.
            //outputOptions.CustomHook = radioButtonInternalHook.Checked;

            // Copy Timestamp: solve the problem that audio does not synchronize with video
            outputOptions.CopyTimestamp = checkBoxCopyTimeStamp.Checked;

            // **********Below code shows Video Filter usage*************

            // libavfilter: please refer to http://svn.mplayerhq.hu/soc/libavfilter/README?view=co
            // currently only several Video Filters are present.
            // NOTICE: What I know about libavfilter is also very limited.
            //         so you have to get help of it by yourself.

            // vflip: Vertically Flip Video Frame
            if (comboBoxVideoFilter.SelectedIndex == 1)
                if (!string.IsNullOrEmpty(outputOptions.VideoFilters))
                    outputOptions.VideoFilters = outputOptions.VideoFilters + ",vflip"; //{Do not Localize}
                else
                    outputOptions.VideoFilters = "vflip"; //{Do not Localize}

            // hflip: Horizontally Flip Video Frame
            if (comboBoxVideoFilter.SelectedIndex == 2)
                if (!string.IsNullOrEmpty(outputOptions.VideoFilters))
                    outputOptions.VideoFilters = outputOptions.VideoFilters + ",hflip"; //{Do not Localize}
                else
                    outputOptions.VideoFilters = "hflip"; //{Do not Localize}

            // negate: Negate Video Frame
            if (comboBoxVideoFilter.SelectedIndex == 3)
                if (!string.IsNullOrEmpty(outputOptions.VideoFilters))
                    outputOptions.VideoFilters = outputOptions.VideoFilters + ",negate"; //{Do not Localize}
                else
                    outputOptions.VideoFilters = "negate"; //{Do not Localize}

            // scale: Scale Video Frame, parameter: width:height
            if (comboBoxVideoFilter.SelectedIndex == 4)
            {
                temp = textBoxVideoFilter.Text.Trim();
                if (!string.IsNullOrEmpty(temp))
                {
                    if (!string.IsNullOrEmpty(outputOptions.VideoFilters))
                        outputOptions.VideoFilters = outputOptions.VideoFilters + ",scale=" + temp; //{Do not Localize}
                    else
                        outputOptions.VideoFilters = "scale=" + temp; //{Do not Localize}
                }
            }

            // rotate: Rotate Video Frame, parameter: degree
            if (comboBoxVideoFilter.SelectedIndex == 5)
            {
                temp = MyUtils.StrToIntWithDefValue(textBoxVideoFilter.Text.Trim(), 45).ToString();
                if (!string.IsNullOrEmpty(outputOptions.VideoFilters))
                    outputOptions.VideoFilters = outputOptions.VideoFilters + ",rotate=" + temp; //{Do not Localize}
                else
                    outputOptions.VideoFilters = "rotate=" + temp; //{Do not Localize}
            }

            /*
              # A few usage examples follow, usable too as test scenarios.

              # Start counting PTS from zero
              ffmpeg -i input.avi -vfilters setpts=PTS-STARTPTS output.avi

              # Fast motion
              ffmpeg -i input.avi -vfilters setpts=0.5*PTS output.avi

              # Fixed rate 25 fps
              ffmpeg -i input.avi -vfilters setpts=N*AVTB/25 output.avi

              # Fixed rate 25 fps with some jitter
              ffmpeg -i input.avi -vfilters 'setpts=AVTB/25*(N+0.05*sin(N*2*PI/25))' output.avi
            */
            // setpts: Set PTS of Video Frame
            if (comboBoxVideoFilter.SelectedIndex == 6)
            {
                temp = textBoxVideoFilter.Text.Trim();
                if (!string.IsNullOrEmpty(temp))
                {
                    if (!string.IsNullOrEmpty(outputOptions.VideoFilters))
                        outputOptions.VideoFilters = outputOptions.VideoFilters + ",setpts=" + temp; //{Do not Localize}
                    else
                        outputOptions.VideoFilters = "setpts=" + temp; //{Do not Localize}
                }
            }


            // special options depend on output formats
            if ((fileExt == ".flv") || (fileExt == ".swf"))
            {
                // FLV/SWF only supports this three sample rate, (44100, 22050, 11025).
                if ((outputOptions.AudioSampleRate != 11025) && (outputOptions.AudioSampleRate != 22050) && (outputOptions.AudioSampleRate != 44100))
                    outputOptions.AudioSampleRate = 22050;
            }
            else if (fileExt == ".ogg")
            {
                outputOptions.AudioCodec = "libvorbis"; //{Do not Localize}
                // Ogg with video
                outputOptions.VideoCodec = "libtheora"; //{Do not Localize}
                // Ogg Vorbis Audio only
                //outputOptions.DisableVideo = True;
            }
            else if (fileExt == ".psp")
            {
                /*
                please refer to http://ffmpeg.mplayerhq.hu/faq.html#SEC26

                3.14 How do I encode videos which play on the PSP?
                `needed stuff'
                -acodec libfaac -vcodec mpeg4 width*height<=76800 width%16=0 height%16=0 -ar 24000 -r 30000/1001 or 15000/1001 -f psp
                `working stuff'
                    4mv, title
                `non-working stuff'
                    B-frames
                `example command line'
                    ffmpeg -i input -acodec libfaac -ab 128kb -vcodec mpeg4 -b 1200kb -ar 24000 -mbd 2 -flags +4mv+trell -aic 2 -cmp 2 -subcmp 2 -s 368x192 -r 30000/1001 -title X -f psp output.mp4
                `needed stuff for H.264'
                    -acodec libfaac -vcodec libx264 width*height<=76800 width%16=0? height%16=0? -ar 48000 -coder 1 -r 30000/1001 or 15000/1001 -f psp
                `working stuff for H.264'
                    title, loop filter
                `non-working stuff for H.264'
                    CAVLC
                `example command line'
                    ffmpeg -i input -acodec libfaac -ab 128kb -vcodec libx264 -b 1200kb -ar 48000 -mbd 2 -coder 1 -cmp 2 -subcmp 2 -s 368x192 -r 30000/1001 -title X -f psp -flags loop -trellis 2 -partitions parti4x4+parti8x8+partp4x4+partp8x8+partb8x8 output.mp4
                `higher resolution for newer PSP firmwares, width<=480, height<=272'
                    -vcodec libx264 -level 21 -coder 1 -f psp
                `example command line'
                    ffmpeg -i input -acodec libfaac -ab 128kb -ac 2 -ar 48000 -vcodec libx264 -level 21 -b 640kb -coder 1 -f psp -flags +loop -trellis 2 -partitions +parti4x4+parti8x8+partp4x4+partp8x8+partb8x8 -g 250 -s 480x272 output.mp4
                */
                outputOptions.FileFormat = "psp";       //{Do not Localize}
                outputOptions.VideoCodec = "mpeg4";     //{Do not Localize}
                outputOptions.AudioCodec = "libfaac";   //{Do not Localize}
                outputOptions.AudioSampleRate = 24000;
                outputOptions.FrameRate = "15000/1001";
                fileExt = "_psp.mp4";
            }
            else if (fileExt == ".ipod")
            {
                /*
                please refer to http://ffmpeg.mplayerhq.hu/faq.html#SEC25

                3.13 How do I encode videos which play on the iPod?
                `needed stuff'
                    -acodec libfaac -vcodec mpeg4 width<=320 height<=240
                `working stuff'
                    4mv, title
                `non-working stuff'
                    B-frames
                `example command line'
                    ffmpeg -i input -acodec libfaac -ab 128kb -vcodec mpeg4 -b 1200kb -mbd 2 -flags +4mv+trell -aic 2 -cmp 2 -subcmp 2 -s 320x180 -title X output.mp4
                */
                outputOptions.VideoCodec = "mpeg4";     //{Do not Localize}
                outputOptions.AudioCodec = "libfaac";   //{Do not Localize}
                outputOptions.FrameSize = "320x240";
                fileExt = "_ipod.mp4";
            }
            else if (fileExt == ".rm10")
            {
                outputOptions.VideoCodec = "rv10";      //{Do not Localize}
                fileExt = "_rv10.rm";
            }
            else if (fileExt == ".rm20")
            {
                outputOptions.VideoCodec = "rv20";      //{Do not Localize}
                fileExt = "_rv20.rm";
            }
            else if (fileExt == ".mp4")
            {
                outputOptions.AudioChannels = 1;
                outputOptions.AudioSampleRate = 16000;
                outputOptions.FrameRate = "15";
                outputOptions.FrameAspectRatio = "1.222";
            }
            else if (fileExt == ".wmv")
            {
                outputOptions.VideoCodec = "wmv2";      //{Do not Localize}
                outputOptions.AudioCodec = "wmav2";     //{Do not Localize}
                //outputOptions.VideoBitrate = 256 * 1024;
            }
            else if (fileExt == ".mkv")
            {
                outputOptions.VideoCodec = "libx264";   //{Do not Localize}
                outputOptions.VideoPreset = "libx264-default";
                //outputOptions.AudioCodec = 'ac3';       //{Do not Localize}
            }
            else if (fileExt == ".wma")
            {
                outputOptions.DisableVideo = true;
                outputOptions.AudioCodec = "wmav2";     //{Do not Localize}
            }
            else if (fileExt == ".3gp")
            {
                // Only mono supported
                outputOptions.AudioChannels = 1;
                // Only 8000Hz sample rate supported
                outputOptions.AudioSampleRate = 8000;
                // bitrate not supported: use one of
                //    4.75k, 5.15k, 5.9k, 6.7k, 7.4k, 7.95k, 10.2k or 12.2k
                /*
                AMR_bitrates rates[]={ {4750,MR475},
                                       {5150,MR515},
                                       {5900,MR59},
                                       {6700,MR67},
                                       {7400,MR74},
                                       {7950,MR795},
                                       {10200,MR102},
                                       {12200,MR122},
                                             };
                */
                outputOptions.AudioBitrate = 7400;
                // [h263 @ 6BF9AE00]The specified picture size of 320x240 is not valid for the H.263 codec.
                //    Valid sizes are 128x96, 176x144, 352x288, 704x576, && 1408x1152. Try H.263+.
                outputOptions.FrameSize = "128x96";
            }
            else if (VIDEO_FILTER_DVD.IndexOf(fileExt) > 0)
            {
                if (fileExt == ".vcd")
                    outputOptions.TargetType = COptionTargetType.cttVCD;
                else if (fileExt == ".svcd")
                    outputOptions.TargetType = COptionTargetType.cttSVCD;
                else // if Ext = '.dvd' then
                    outputOptions.TargetType = COptionTargetType.cttDVD;

                if (comboBoxNorm.SelectedIndex == 0)
                    outputOptions.NormType = COptionNormType.cntAuto;
                else if (comboBoxNorm.SelectedIndex == 1)
                    outputOptions.NormType = COptionNormType.cntPAL;
                else
                    outputOptions.NormType = COptionNormType.cntNTSC;

                outputOptions.NormDefault = (int)COptionNormType.cntPAL; // or ntNTSC, according the locale region
            }

            outputOptions.Info.TimeStamp = DateTime.Now; // 0 means current time: Now()
            {
                outputOptions.Info.Title = "cc title";
                outputOptions.Info.Author = "cc author";
                outputOptions.Info.Copyright = "cc copyright";
                outputOptions.Info.Comment = "cc comment";
                outputOptions.Info.Album = "cc album";
                outputOptions.Info.Year = 2008;
                outputOptions.Info.Track = 5;
                outputOptions.Info.Genre = "Dance Hall";
            }

            if (checkBoxCrop.Checked)
            {
                // crop size must be a multiple of 2
                outputOptions.CropTop = MyUtils.StrToIntWithDefValue(textBoxCropTop.Text, 0) / 2 * 2;
                outputOptions.CropBottom = MyUtils.StrToIntWithDefValue(textBoxCropBottom.Text, 0) / 2 * 2;
                outputOptions.CropLeft = MyUtils.StrToIntWithDefValue(textBoxCropLeft.Text, 0) / 2 * 2;
                outputOptions.CropRight = MyUtils.StrToIntWithDefValue(textBoxCropRight.Text, 0) / 2 * 2;
            }

            if (checkBoxPad.Checked)
            {
                // pad size must be a multiple of 2
                outputOptions.PadTop = MyUtils.StrToIntWithDefValue(textBoxPadTop.Text, 0) / 2 * 2;
                outputOptions.PadBottom = MyUtils.StrToIntWithDefValue(textBoxPadBottom.Text, 0) / 2 * 2;
                outputOptions.PadLeft = MyUtils.StrToIntWithDefValue(textBoxPadLeft.Text, 0) / 2 * 2;
                outputOptions.PadRight = MyUtils.StrToIntWithDefValue(textBoxPadRight.Text, 0) / 2 * 2;
                outputOptions.PadColor = (uint)ColorTranslator.ToOle(Color.Black);
            }

            {
                outputOptions.AudioLanguage = "eng";
                outputOptions.SubtitleLanguage = "eng";
            }

            // TODO: more options

            // change vcd/svcd/dvd file ext to .mpg
            if (VIDEO_FILTER_DVD.IndexOf(fileExt) > 0)
            {
                fileExt = "_" + fileExt.Substring(1);
                if (outputOptions.NormType == COptionNormType.cntPAL)
                    fileExt = fileExt + "_pal.mpg";
                else if (outputOptions.NormType == COptionNormType.cntNTSC)
                    fileExt = fileExt + "_ntsc.mpg";
                else
                    fileExt = fileExt + "_auto.mpg";
            }
            
            // generate output filename automatically
            string fileNanme = mConverter.AVPrope.FileName;

            int pos = 0;

            outputOptions.FileExt = fileExt;
            if (textBoxOutputFolder.Text == SAME_AS_INPUT)
            {
                pos = fileNanme.LastIndexOf('.');
                if (pos > 0)
                {
                    baseName = fileNanme.Substring(0, pos);
                }
                else
                {
                    baseName = fileNanme;
                }
            }
            else
            {
                pos = fileNanme.LastIndexOf('\\');
                if (pos > 0)
                {
                    fileNanme = fileNanme.Substring(pos + 1);
                }
                pos = fileNanme.LastIndexOf('.');
                if (pos > 0)
                {
                    baseName = textBoxOutputFolder.Text + "\\" + fileNanme.Substring(0, pos);
                }
                else
                {
                    baseName = textBoxOutputFolder.Text + "\\" + fileNanme;
                }
            }
            outputOptions.FileName = baseName + "_(new)" + outputOptions.FileExt;
            if (File.Exists(outputOptions.FileName) && !checkBoxOverWriteExistingFile.Checked)
            {
                i = 1;
                while (File.Exists(baseName + "_(new_" + i.ToString() + ")" + outputOptions.FileExt))
                    i++;
                outputOptions.FileName = baseName + "_(new_" + i.ToString() + ")" + outputOptions.FileExt;
            }

            // NOTICE: the component will use OutputFileName's FileExt to guess
            //         the output format when IO.FileFormat isn't defined.

            outputOptions.DisableSubtitle = true;
        }        

        private void trackBarPreview_Scroll(object sender, EventArgs e)
        {
            int desirePTS = 0;
            if (mConverter.AVPrope.VideoStreamCount <= 0)
            {
                // if no video stream, that is, only audio stream
                // update current pts && current position
                if (trackBarPreview.Maximum > 0)
                    mCurrentPTS = (int)Math.Round((double)trackBarPreview.Value / trackBarPreview.Maximum * mDuration);
                else
                    mCurrentPTS = 0;
                labelCurrentTime.Text = DurationToStr(mCurrentPTS);
                return;
            }
            // check the situation for seeking
            if (mChanging || !trackBarPreview.Enabled || (mLastPosition == trackBarPreview.Value))
                return;

            mChanging = true;
            try
            {
                // position rate
                if (trackBarPreview.Maximum > 0)
                    desirePTS = (int)Math.Round((double)trackBarPreview.Value / trackBarPreview.Maximum * mDuration);
                else
                    desirePTS = 0;

                if (trackBarPreview.Value == mLastPosition + 1)
                {
                    // next frame
                    if (mConverter.AVPrope.Decode(-1))
                        DrawCurrentFrame();
                }
                else if (trackBarPreview.Value == mLastPosition - 1)
                {
                    if (mConverter.AVPrope.DecodePreviousFrame(-1))
                        DrawCurrentFrame();
                }
                else
                {
                    // try to seek the frame
                    if (mConverter.AVPrope.Seek(desirePTS, CSeekFlag.csfNone) ||
                        mConverter.AVPrope.Seek(desirePTS, CSeekFlag.csfBackward) ||
                        mConverter.AVPrope.Seek(desirePTS, CSeekFlag.csfAny))
                    {
                        // decode the next frame
                        while (mConverter.AVPrope.Decode(-1))
                        {
                            // if the frame is the one we desire
                            if ((mConverter.AVPrope.FrameInfo.PTS >= desirePTS) || (desirePTS <= 0))
                            {
                                DrawCurrentFrame();
                                return;
                            }
                        }
                    }
                }
            }
            catch
            {

            }
            finally
            {
                mLastPosition = trackBarPreview.Value;
                mChanging = false;
            }
        }

        private void buttonSetStart_Click(object sender, EventArgs e)
        {
            textBoxStartTime.Text = DurationToStr(mCurrentPTS);
            mStartTime = mCurrentPTS;
        }

        private void buttonSetEnd_Click(object sender, EventArgs e)
        {
            textBoxEndTime.Text = DurationToStr(mCurrentPTS);
            mEndTime = mCurrentPTS;
        }

        private void buttonNextFrame_Click(object sender, EventArgs e)
        {
            ReadAndDrawNextFrame();
        }

        private void buttonSaveFrame_Click(object sender, EventArgs e)
        {
            SaveFileDialog sdlg = new SaveFileDialog();

            if (sdlg.ShowDialog(this) == DialogResult.OK)
            {
                mConverter.AVPrope.SaveCurrentFrame(sdlg.FileName);
            }
        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fdlg = new FolderBrowserDialog();

            if (fdlg.ShowDialog(this) == DialogResult.OK)
            {
                textBoxOutputFolder.Text = fdlg.SelectedPath;
                mOutputFolder = fdlg.SelectedPath;
            }
        }

        private void buttonBrowseMixImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog fdlg = new OpenFileDialog();

            if (fdlg.ShowDialog(this) == DialogResult.OK)
            {
                textBoxMixImage.Text = fdlg.FileName;
            }
        }

        private void buttonBrowseWaterMark_Click(object sender, EventArgs e)
        {
            OpenFileDialog fdlg = new OpenFileDialog();

            if (fdlg.ShowDialog(this) == DialogResult.OK)
            {
                textBoxWaterMark.Text = fdlg.FileName;
            }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (mOutputFolder != null)
            {
                DriveInfo drvInfo = new DriveInfo(textBoxOutputFolder.Text);

                if (drvInfo.DriveType == DriveType.CDRom)
                {
                    MessageBox.Show(this, "Output folder is located in CD-ROM, please select output folder.");
                    buttonBrowse.Focus();
                    return;
                }
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        public string DurationToStr(Int64 duration)
        {
            string result = null;

            result = String.Format("{0:D2}:{1:D2}:{2:D2}:{3:D3}",
                duration / AV_TIME_BASE / 60 / 60,
                duration / AV_TIME_BASE / 60 % 60,
                duration / AV_TIME_BASE % 60,
                duration % AV_TIME_BASE * 1000 / AV_TIME_BASE);

            return result;
        }

        private void SetupTrackBar()
        {
            bool bEnabled = false;
            double dPosRate = 0;

            bEnabled = trackBarPreview.Enabled;
            trackBarPreview.Enabled = false;

            if (trackBarPreview.Maximum > 0)
                dPosRate = trackBarPreview.Value / trackBarPreview.Maximum;
            else
                dPosRate = 0;

            if (mFrames > 0)
                trackBarPreview.Maximum = mFrames;

            if (trackBarPreview.Maximum > 10)
                trackBarPreview.TickFrequency = trackBarPreview.Maximum / 10;
            else
                trackBarPreview.TickFrequency = 1;

            if (trackBarPreview.Maximum < 10)
                trackBarPreview.LargeChange = 2;
            else if (trackBarPreview.Maximum < 100)
                trackBarPreview.LargeChange = 5;
            else
                trackBarPreview.LargeChange = trackBarPreview.Maximum / 20;

            trackBarPreview.Value = (int)Math.Round(trackBarPreview.Maximum * dPosRate);
            mLastPosition = trackBarPreview.Value;

            trackBarPreview.Enabled = bEnabled;
            trackBarPreview.Visible = bEnabled;
        }

        private void ClearFrameView()
        {
            mDuration = 0;
            mFrames = 0;
            mCurrentPTS = 0;
            mFirstPTS = Int64.MaxValue;
            mStartTime = Int64.MinValue;
            mEndTime = (Int64)AV_NOPTS_VALUE;

            pictureBoxPreview.BackColor = Color.AliceBlue;

            trackBarPreview.Enabled = false;
            trackBarPreview.Value = 0;
            trackBarPreview.Maximum = 0;
            trackBarPreview.Minimum = 0;
            SetupTrackBar();

            textBoxStartTime.Text = "N/A";
            textBoxEndTime.Text = "N/A";

            labelCurrentTime.Text = DurationToStr(0);
        }

        private void SafeFreeIPictureDisp()
        {
            if (mPictureDisp != null)
            {
                mPictureDisp = null;
                GC.Collect();
            }
        }

        private void ReadAndDrawNextFrame()
        {
            // next frame
            if (mConverter.AVPrope.Decode(-1))
                DrawCurrentFrame();
        }

        private void DrawCurrentFrame()
        {
            int position = 0;

            // update current pts && current position
            mCurrentPTS = mConverter.AVPrope.FrameInfo.PTS;
            labelCurrentTime.Text = DurationToStr(mCurrentPTS);
            if (!mChanging && (mDuration > 0))
            {
                mChanging = true;
                try
                {
                    position = (int)Math.Round((double)trackBarPreview.Maximum * mCurrentPTS / mDuration);
                    if (position > trackBarPreview.Maximum)
                        position = trackBarPreview.Maximum;
                    trackBarPreview.Value = position;
                }
                catch
                {

                }
                finally
                {
                    mChanging = false;
                }
            }
            // copy the frame to bitmap
            SafeFreeIPictureDisp();
            mPictureDisp = mConverter.AVPrope.CurrentPicture;

            if (mPictureDisp != null)
            {
                pictureBoxPreview.BackColor = Color.Black;
                pictureBoxPreview.Image = PictureDispHost.GetPictureFromIPicture(mPictureDisp);
            }
        }

        private Int64 GetClipStartTime()
        {
            Int64 result = 0;

            // start time offset
            if ((mFirstPTS != Int64.MaxValue) && (mFirstPTS < 0))
                result = mStartTime - mFirstPTS;
            else
                result = mStartTime;

            return result;
        }

        private Int64 GetClipTimeLength()
        {
            return mEndTime - mStartTime;
        }

        private void checkBoxCrop_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxCrop.Checked)
            {
                textBoxCropBottom.Enabled = true;
                textBoxCropLeft.Enabled = true;
                textBoxCropRight.Enabled = true;
                textBoxCropTop.Enabled = true;
            }
            else
            {
                textBoxCropBottom.Enabled = false;
                textBoxCropLeft.Enabled = false;
                textBoxCropRight.Enabled = false;
                textBoxCropTop.Enabled = false;
            }
        }

        private void checkBoxPad_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxPad.Checked)
            {
                textBoxPadBottom.Enabled = true;
                textBoxPadLeft.Enabled = true;
                textBoxPadRight.Enabled = true;
                textBoxPadTop.Enabled = true;
            } 
            else
            {
                textBoxPadBottom.Enabled = false;
                textBoxPadLeft.Enabled = false;
                textBoxPadRight.Enabled = false;
                textBoxPadTop.Enabled = false;
            }
        }

        private void comboBoxVideoFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxVideoFilter.SelectedIndex == SCALE_INDEX)
            {
                textBoxVideoFilter.Text = DEFAULT_SCALE;
            }
            else if (comboBoxVideoFilter.SelectedIndex == ROTATE_INDEX)
            {
                textBoxVideoFilter.Text = DEFAULT_ROTATE;
            }
            else if (comboBoxVideoFilter.SelectedIndex == PTS_INDEX)
            {
                textBoxVideoFilter.Text = DEFAULT_PTS;
            }
            else
            {
                textBoxVideoFilter.Text = "";
            }
        }        
    }
}
