using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DShowNET;
using DShowNET.Device;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Forms;

namespace GemScopeWPF.WebcamFacade
{
    class CaptureDirectShowV1
    {
        /// <summary> flag to detect first Form appearance </summary>
	        private bool					firstActive;
		        /// <summary> file name for AVI. </summary>
	        private string					fileName;

		        /// <summary> list of installed video devices. </summary>
	        private List<DsDevice>				capDevices;

		        /// <summary> base filter of the actually used video devices. </summary>
	        private IBaseFilter				capFilter;

		        /// <summary> graph builder interface. </summary>
	        private IGraphBuilder			graphBuilder;

		        /// <summary> capture graph builder interface. </summary>
	        private ICaptureGraphBuilder2	capGraph;

		        /// <summary> video window interface. </summary>
	        private IVideoWindow			videoWin;

		        /// <summary> control interface. </summary>
	        private IMediaControl			mediaCtrl;

		        /// <summary> event interface. </summary>
	        private IMediaEventEx			mediaEvt;

	        private const int WM_GRAPHNOTIFY	= 0x00008001;	// message from graph

	        private const int WS_CHILD			= 0x40000000;	// attributes for video window
	        private const int WS_CLIPCHILDREN	= 0x02000000;
	        private const int WS_CLIPSIBLINGS	= 0x04000000;

	        #if DEBUG
		        private int		rotCookie = 0;

                private void videoPanel_Paint(object sender, PaintEventArgs e)
                {

                }
	        #endif


        private Border VideoBorder { get; set; }
        public Window OwnerWindow { get; set; }

        public CaptureDirectShowV1(Border image,Window window)
        {
            VideoBorder = image;
            OwnerWindow = window;
        }
        public bool StartCamera()
        {



            if (firstActive)
                return false;
            firstActive = true;

            if (!DsUtils.IsCorrectDirectXVersion())
            {
                System.Windows.Forms.MessageBox.Show("DirectX 8.1 NOT installed!", "DirectShow.NET", MessageBoxButtons.OK, MessageBoxIcon.Stop);
               return false;
            }

            if (!DsDev.GetDevicesOfCat(FilterCategory.VideoInputDevice, out capDevices))
            {
                System.Windows.Forms.MessageBox.Show("No video capture devices found!", "DirectShow.NET", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                this.Close(); return false;
            }

            //SaveFileDialog sd = new SaveFileDialog();
            //sd.FileName = @"CaptureNET.avi";
            //sd.Title = "Save Video Stream as...";
            //sd.Filter = "Video file (*.avi)|*.avi";
            //sd.FilterIndex = 1;
            //if (sd.ShowDialog() != DialogResult.OK)
            //{ this.Close(); return false; }
            fileName = "c:\\a.avi";

            DsDevice dev = null;
            //if (capDevices.Count == 1)
            //    dev = capDevices[0] as DsDevice;
            //else
            //{
            //    DeviceSelector selector = new DeviceSelector(capDevices);
            //    selector.ShowDialog(this);
            //    dev = selector.SelectedDevice;
            //}
            dev = capDevices[1];

            if (dev == null)
            {
                this.Close(); return false;
            }

            if (!StartupVideo(dev.Mon))
                this.Close();


            return true;
    
                
            
        }

        private void Close()
        {
            throw new NotImplementedException();
        }

        bool StartupVideo(UCOMIMoniker mon)
        {
            int hr;
            try
            {
                if (!CreateCaptureDevice(mon))
                    return false;

                if (!GetInterfaces())
                    return false;

                if (!SetupGraph())
                    return false;

                if (!SetupVideoWindow())
                    return false;

            #if DEBUG
                DsROT.AddGraphToRot(graphBuilder, out rotCookie);		// graphBuilder capGraph
            #endif

                hr = mediaCtrl.Run();
                if (hr < 0)
                    Marshal.ThrowExceptionForHR(hr);
                return true;
            }
            catch (Exception ee)
            {
                System.Windows.Forms.MessageBox.Show("Could not start video stream\r\n" + ee.Message, "DirectShow.NET", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return false;
            }
        }


        /// <summary> build the capture graph. </summary>
        bool SetupGraph()
        {
            int hr;
            IBaseFilter mux = null;
            IFileSinkFilter sink = null;

            try
            {
                hr = capGraph.SetFiltergraph(graphBuilder);
                if (hr < 0)
                    Marshal.ThrowExceptionForHR(hr);

                hr = graphBuilder.AddFilter(capFilter, "Ds.NET Video Capture Device");
                if (hr < 0)
                    Marshal.ThrowExceptionForHR(hr);

                HwndSource hwndSource = PresentationSource.FromVisual(OwnerWindow) as HwndSource;

                //DsUtils.ShowCapPinDialog(capGraph, capFilter, hwndSource.Handle);

                Guid sub = MediaSubType.Avi;
                hr = capGraph.SetOutputFileName(ref sub, fileName, out mux, out sink);
                if (hr < 0)
                    Marshal.ThrowExceptionForHR(hr);

                Guid cat = PinCategory.Capture;
                Guid med = MediaType.Video;
                hr = capGraph.RenderStream(ref cat, ref med, capFilter, null, mux); // stream to file 
                
                if (hr < 0)
                    Marshal.ThrowExceptionForHR(hr);

                cat = PinCategory.Preview;
                med = MediaType.Video;
                hr = capGraph.RenderStream(ref cat, ref med, capFilter, null, null); // preview window
                if (hr < 0)
                    Marshal.ThrowExceptionForHR(hr);

                return true;
            }
            catch (Exception ee)
            {
                System.Windows.Forms.MessageBox.Show("Could not setup graph\r\n" + ee.Message, "DirectShow.NET", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return false;
            }
            finally
            {
                if (mux != null)
                    Marshal.ReleaseComObject(mux); mux = null;
                if (sink != null)
                    Marshal.ReleaseComObject(sink); sink = null;
            }
        }


        /// <summary> create the used COM components and get the interfaces. </summary>
        bool GetInterfaces()
        {
            Type comType = null;
            object comObj = null;
            try
            {
                comType = Type.GetTypeFromCLSID(Clsid.FilterGraph);
                if (comType == null)
                    throw new NotImplementedException(@"DirectShow FilterGraph not installed/registered!");
                comObj = Activator.CreateInstance(comType);
                graphBuilder = (IGraphBuilder)comObj; comObj = null;

                Guid clsid = Clsid.CaptureGraphBuilder2;
                Guid riid = typeof(ICaptureGraphBuilder2).GUID;
                comObj = DsBugWO.CreateDsInstance(ref clsid, ref riid);
                capGraph = (ICaptureGraphBuilder2)comObj; comObj = null;

                mediaCtrl = (IMediaControl)graphBuilder;
                videoWin = (IVideoWindow)graphBuilder;
                mediaEvt = (IMediaEventEx)graphBuilder;
                return true;
            }
            catch (Exception ee)
            {
                System.Windows.Forms.MessageBox.Show("Could not get interfaces\r\n" + ee.Message, "DirectShow.NET", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return false;
            }
            finally
            {
                if (comObj != null)
                    Marshal.ReleaseComObject(comObj); comObj = null;
            }
        }


        /// <summary> create the user selected capture device. </summary>
        bool CreateCaptureDevice(UCOMIMoniker mon)
        {
            object capObj = null;
            try
            {
                Guid gbf = typeof(IBaseFilter).GUID;
                mon.BindToObject(null, null, ref gbf, out capObj);
                capFilter = (IBaseFilter)capObj; capObj = null;
                return true;
            }
            catch (Exception ee)
            {
                System.Windows.Forms.MessageBox.Show( "Could not create capture device\r\n" + ee.Message, "DirectShow.NET", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return false;
            }
            finally
            {
                if (capObj != null)
                    Marshal.ReleaseComObject(capObj); capObj = null;
            }

        }

        /// <summary> make the video preview window to show in videoPanel. </summary>
        bool SetupVideoWindow()
        {
            int hr;
            try
            {
                // Set the video window to be a child of the main window

                HwndSource hwndSource = PresentationSource.FromVisual(OwnerWindow) as HwndSource;

                

                hr = videoWin.put_Owner(hwndSource.Handle);
                if (hr < 0)
                    Marshal.ThrowExceptionForHR(hr);

                // Set video window style
                hr = videoWin.put_WindowStyle(WS_CHILD | WS_CLIPCHILDREN);
                if (hr < 0)
                    Marshal.ThrowExceptionForHR(hr);

                // Use helper function to position video window in client rect of owner window
                ResizeVideoWindow();

                // Make the video window visible, now that it is properly positioned
                hr = videoWin.put_Visible(DsHlp.OATRUE);
                if (hr < 0)
                    Marshal.ThrowExceptionForHR(hr);

                hr = mediaEvt.SetNotifyWindow(hwndSource.Handle, WM_GRAPHNOTIFY, IntPtr.Zero);
                if (hr < 0)
                    Marshal.ThrowExceptionForHR(hr);
                return true;
            }
            catch (Exception ee)
            {
                System.Windows.Forms.MessageBox.Show("Could not setup video window\r\n" + ee.Message, "DirectShow.NET", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return false;
            }
        }
        void ResizeVideoWindow()
        {
            if (videoWin != null)
            {
                Rect rc = GetBoundingBox(VideoBorder, OwnerWindow);
                videoWin.SetWindowPosition((int)rc.Left, (int)rc.Top, (int)VideoBorder.Width, (int)VideoBorder.Height);
                //videoWin.SetWindowPosition(0, 0, (int)VideoImage.Width, (int)VideoImage.Height);
            }
        }
        private Rect GetBoundingBox(FrameworkElement element, Window containerWindow)
        {
            GeneralTransform transform = element.TransformToAncestor(containerWindow);
            Point topLeft = transform.Transform(new Point(0, 0));
            Point bottomRight = transform.Transform(new Point(element.ActualWidth, element.ActualHeight));
            return new Rect(topLeft, bottomRight);
        }

        //protected override void WndProc(ref Message m)
        //{
        //    if (m.Msg == WM_GRAPHNOTIFY)
        //    {
        //        if (mediaEvt != null)
        //            OnGraphNotify();
        //        return;
        //    }
        //    base.WndProc(ref m);
        //}

        ///// <summary> graph event (WM_GRAPHNOTIFY) handler. </summary>
        //void OnGraphNotify()
        //{
        //    DsEvCode code;
        //    int p1, p2, hr = 0;
        //    do
        //    {
        //        hr = mediaEvt.GetEvent(out code, out p1, out p2, 0);
        //        if (hr < 0)
        //            break;
        //        hr = mediaEvt.FreeEventParams(code, p1, p2);
        //    }
        //    while (hr == 0);
        //}


        /// <summary> do cleanup and release DirectShow. </summary>
        public void CloseInterfaces()
        {
            int hr;
            try
            {
                #if DEBUG
                                if (rotCookie != 0)
                                    DsROT.RemoveGraphFromRot(ref rotCookie);
                #endif

                if (mediaCtrl != null)
                {
                    hr = mediaCtrl.Stop();
                    mediaCtrl = null;
                }

                if (mediaEvt != null)
                {
                    hr = mediaEvt.SetNotifyWindow(IntPtr.Zero, WM_GRAPHNOTIFY, IntPtr.Zero);
                    mediaEvt = null;
                }

                if (videoWin != null)
                {
                    hr = videoWin.put_Visible(DsHlp.OAFALSE);
                    hr = videoWin.put_Owner(IntPtr.Zero);
                    videoWin = null;
                }

                if (capGraph != null)
                    Marshal.ReleaseComObject(capGraph); capGraph = null;

                if (graphBuilder != null)
                    Marshal.ReleaseComObject(graphBuilder); graphBuilder = null;

                if (capFilter != null)
                    Marshal.ReleaseComObject(capFilter); capFilter = null;

                if (capDevices != null)
                {
                    foreach (DsDevice d in capDevices)
                        d.Dispose();
                    capDevices = null;
                }
            }
            catch (Exception)
            { }
        }
    }
}
