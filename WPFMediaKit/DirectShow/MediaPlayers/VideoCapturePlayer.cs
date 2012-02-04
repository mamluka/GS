using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using NLog;
using WPFMediaKit.DirectShow.Interop;
using WPFMediaKit.MediaFoundation.Interop.Misc;
using AMMediaType=WPFMediaKit.DirectShow.Interop.AMMediaType;
using VideoInfoHeader=WPFMediaKit.DirectShow.Interop.VideoInfoHeader;

namespace WPFMediaKit.DirectShow.MediaPlayers
{
    public class VideoSampleArgs : EventArgs
    {
        public Bitmap VideoFrame { get; internal set; }
    }

    /// <summary>
    /// A Player that plays video from a video capture device.
    /// </summary>
    public class VideoCapturePlayer : MediaPlayerBase, ISampleGrabberCB
    {
        [DllImport("Kernel32.dll", EntryPoint = "RtlMoveMemory")]
        private static extern void CopyMemory(IntPtr destination, IntPtr source, [MarshalAs(UnmanagedType.U4)] int length);

        #region Locals
        /// <summary>
        /// Helps the GPFBridge
        /// </summary>
        const long NEVER = long.MaxValue;

        // Are we actively capturing?
        private bool m_Capturing = false;

        /// <summary>
        /// The video capture pixel height
        /// </summary>
        private int m_desiredHeight = 240;

        /// <summary>
        /// The video capture pixel width
        /// </summary>
        private int m_desiredWidth = 320;

        /// <summary>
        /// The video capture's frames per second
        /// </summary>
        private int m_fps = 30;

        /// <summary>
        /// Our DirectShow filter graph
        /// </summary>
        private IGraphBuilder m_graph;

        // The graph that contains the file writer
        private IGraphBuilder m_pCaptureGraph;
        /// <summary>
        /// The DirectShow video renderer
        /// </summary>
        private IBaseFilter m_renderer;

        /// <summary>
        /// The capture device filter
        /// </summary>
        private IBaseFilter m_captureDevice;

        /// <summary>
        /// The name of the video capture source device
        /// </summary>
        private string m_videoCaptureSource;

        /// <summary>
        /// Flag to detect if the capture source has changed
        /// </summary>
        private bool m_videoCaptureSourceChanged;

        /// <summary>
        /// The video capture device
        /// </summary>
        private DsDevice m_videoCaptureDevice;

        /// <summary>
        /// Flag to detect if the capture source device has changed
        /// </summary>
        private bool m_videoCaptureDeviceChanged;

        /// <summary>
        /// The sample grabber interface used for getting samples in a callback
        /// </summary>
        private ISampleGrabber m_sampleGrabber;

        // The bridge controller that stands between the two graphs
        private IGMFBridgeController m_pBridge;

        // The capture pin of the capture device (or the smarttee if
        // the capture device has no preview pin)
        private IPin m_pCapOutput;

        // The source side of the bridge
        private IBaseFilter m_pSourceGraphSinkFilter;

        // The other side of the bridge
        private IBaseFilter m_pCaptureGraphSourceFilter;

#if DEBUG
        private DsROTEntry m_rotEntry;
#endif
        #endregion

        /// <summary>
        /// Gets or sets if the instance fires an event for each of the samples
        /// </summary>
        public bool EnableSampleGrabbing { get; set; }

        /// <summary>
        /// Fires when a new video sample is ready
        /// </summary>
        public event EventHandler<VideoSampleArgs> NewVideoSample;

        private void InvokeNewVideoSample(VideoSampleArgs e)
        {
            EventHandler<VideoSampleArgs> sample = NewVideoSample;
            if (sample != null) sample(this, e);
        }

        private static Logger logger = LogManager.GetCurrentClassLogger();
        /// <summary>
        /// The name of the video capture source to use
        /// </summary>
        public string VideoCaptureSource
        {
            get
            {
                VerifyAccess();
                return m_videoCaptureSource;
            }
            set
            {
                VerifyAccess();
                m_videoCaptureSource = value;
                m_videoCaptureSourceChanged = true;

                /* Free our unmanaged resources when
                 * the source changes */
                FreeResources();
            }
        }

        public DsDevice VideoCaptureDevice
        {
            get
            {
                VerifyAccess();
                return m_videoCaptureDevice;
            }
            set
            {
                VerifyAccess();
                m_videoCaptureDevice = value;
                m_videoCaptureDeviceChanged = true;

                /* Free our unmanaged resources when
                 * the source changes */
                FreeResources();
            }
        }

        /// <summary>
        /// The frames per-second to play
        /// the capture device back at
        /// </summary>
        public int FPS
        {
            get
            {
                VerifyAccess();
                return m_fps;
            }
            set
            {
                VerifyAccess();

                /* We support only a minimum of
                 * one frame per second */
                if (value < 1)
                    value = 1;

                m_fps = value;
            }
        }

        /// <summary>
        /// Gets or sets if Yuv is the prefered color space
        /// </summary>
        public bool UseYuv { get; set; }

        /// <summary>
        /// The desired pixel width of the video
        /// </summary>
        public int DesiredWidth
        {
            get
            {
                VerifyAccess();
                return m_desiredWidth;
            }
            set
            {
                VerifyAccess();
                m_desiredWidth = value;
            }
        }

        /// <summary>
        /// The desired pixel height of the video
        /// </summary>
        public int DesiredHeight
        {
            get
            {
                VerifyAccess();
                return m_desiredHeight;
            }
            set
            {
                VerifyAccess();
                m_desiredHeight = value;
            }
        }

        private string fileName;
        public string FileName
        {
            get
            {
                //VerifyAccess();
                return fileName;
            }
            set
            {
                //VerifyAccess();
                fileName = value;
            }
        }

        /// <summary>
        /// Plays the video capture device
        /// </summary>
        public override void Play()
        {
            VerifyAccess();

            if (m_graph == null)
            {
                SetupGraph();
              //  SetupGraph2();
            }
            base.Play();
        }

        /// <summary>
        /// Pauses the video capture device
        /// </summary>
        public override void Pause()
        {
            VerifyAccess();

            if (m_graph == null)
                SetupGraph();

            base.Pause();
        }

        public void ShowCapturePropertyPages(IntPtr hwndOwner)
        {
            VerifyAccess();

            if (m_captureDevice == null)
                return;

            using(var dialog = new PropertyPageHelper(m_captureDevice))
            {
                dialog.Show(hwndOwner);
            }
        }

        /// <summary>
        /// Configures the DirectShow graph to play the selected video capture
        /// device with the selected parameters
        /// </summary>
        private void SetupGraph()
        {
            /* Clean up any messes left behind */
            FreeResources();

            try
            {
                logger.Info("Graph Setup");
                /* Create a new graph */

                m_pBridge = (IGMFBridgeController)new GMFBridgeController();

                int hr = m_pBridge.AddStream(true, eFormatType.MuxInputs, true);
                DsError.ThrowExceptionForHR(hr);
                
                m_graph = (IGraphBuilder)new FilterGraphNoThread();

                #if DEBUG
                    m_rotEntry = new DsROTEntry(m_graph);
                #endif

                /* Create a capture graph builder to help 
                 * with rendering a capture graph */
                var graphBuilder = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();

                hr = m_pBridge.InsertSinkFilter(m_graph, out m_pSourceGraphSinkFilter);
                DsError.ThrowExceptionForHR(hr);
                /* Set our filter graph to the capture graph */
                logger.Info("VideoCaptureSource:" + VideoCaptureSource);
                if (VideoCaptureDevice != null)
                {
                    logger.Info("VideoCaptureDevice.DevicePath:" + VideoCaptureDevice.DevicePath);
                }
                
                /* Add our capture device source to the graph */
                if (m_videoCaptureSourceChanged)
                {
                    m_captureDevice = AddFilterByName(m_graph,
                                                      FilterCategory.VideoInputDevice,
                                                      VideoCaptureSource);

                    m_videoCaptureSourceChanged = false;

                }
                else if (m_videoCaptureDeviceChanged)
                {
                    m_captureDevice = AddFilterByDevicePath(m_graph,
                                                            FilterCategory.VideoInputDevice,
                                                            VideoCaptureDevice.DevicePath);

                    m_videoCaptureDeviceChanged = false;
                }

                

                /* If we have a null capture device, we have an issue */
                if (m_captureDevice == null)
                    throw new Exception(string.Format("Capture device {0} not found or could not be created", VideoCaptureSource));

                object crossbar;

                var a = graphBuilder.FindInterface(null,
                                            null,
                                            m_captureDevice as IBaseFilter,
                                            typeof(IAMCrossbar).GUID,
                                            out crossbar);

                if(UseYuv && !EnableSampleGrabbing)
                {
                    /* Configure the video output pin with our parameters and if it fails
                     * then just use the default media subtype*/
                    if(!SetVideoCaptureParameters(graphBuilder, m_captureDevice, MediaSubType.YUY2))
                        SetVideoCaptureParameters(graphBuilder, m_captureDevice, Guid.Empty);

                    
                }
                else
                    /* Configure the video output pin with our parameters */
                    SetVideoCaptureParameters(graphBuilder, m_captureDevice, new Guid("73646976-0000-0010-8000-00AA00389B71"));

                


                
                var rendererType = VideoRendererType.VideoMixingRenderer9;

                /* Creates a video renderer and register the allocator with the base class */
                m_renderer = CreateVideoRenderer(rendererType, m_graph, 1);

                if (rendererType == VideoRendererType.VideoMixingRenderer9)
                {
                    var mixer = m_renderer as IVMRMixerControl9;

                    if (mixer != null && !EnableSampleGrabbing && UseYuv)
                    {
                        VMR9MixerPrefs dwPrefs;
                        mixer.GetMixingPrefs(out dwPrefs);
                        dwPrefs &= ~VMR9MixerPrefs.RenderTargetMask;
                        dwPrefs |= VMR9MixerPrefs.RenderTargetYUV;
                        /* Prefer YUV */
                        mixer.SetMixingPrefs(dwPrefs);
                    }
                }

                if (EnableSampleGrabbing)
                {
                    m_sampleGrabber = (ISampleGrabber)new SampleGrabber();
                    SetupSampleGrabber(m_sampleGrabber);
                    hr = m_graph.AddFilter(m_sampleGrabber as IBaseFilter, "SampleGrabber");
                    DsError.ThrowExceptionForHR(hr);
                }

                hr = graphBuilder.SetFiltergraph(m_graph);
                DsError.ThrowExceptionForHR(hr);


                IBaseFilter mux = null;
                IFileSinkFilter sink = null;
                if (!string.IsNullOrEmpty(this.fileName))
                {
                }

                hr = graphBuilder.RenderStream(PinCategory.Preview,
                                               MediaType.Video,
                                               m_captureDevice,
                                               null,
                                               m_renderer);




               
                DsError.ThrowExceptionForHR(hr);

                hr = graphBuilder.RenderStream(PinCategory.Capture, MediaType.Video, m_captureDevice, null, m_pSourceGraphSinkFilter);
                DsError.ThrowExceptionForHR(hr);

                hr = graphBuilder.FindPin(m_captureDevice, PinDirection.Output, PinCategory.Capture, MediaType.Video, false, 0, out m_pCapOutput);
                if (hr >= 0)
                {
                    IAMStreamControlBridge pSC = (IAMStreamControlBridge)m_pCapOutput;
                    pSC.StartAt(NEVER, 0);  // Ignore any error
                }


                /* Register the filter graph 
                 * with the base classes */
                SetupFilterGraph(m_graph);

                /* Sets the NaturalVideoWidth/Height */
                SetNativePixelSizes(m_renderer);
                

                HasVideo = true;

                /* Make sure we Release() this COM reference */
                if (mux != null)
                {
                    Marshal.ReleaseComObject(mux);
                }
                if (sink != null)
                {
                    Marshal.ReleaseComObject(sink);
                }

                Marshal.ReleaseComObject(graphBuilder);
            }
            catch (Exception ex)
            {
                /* Something got fuct up */
                FreeResources();
                InvokeMediaFailed(new MediaFailedEventArgs(ex.Message, ex));
            }

            /* Success */
            InvokeMediaOpened();
        }

        public void SetNextFilename(string pFile)
        {
            VerifyAccess();

            int hr;

            ICaptureGraphBuilder2 pBuilder = null;
            IBaseFilter pfMux = null;
            IFileSinkFilter pSink = null;

            if (pFile != null)
            {
                if (m_captureDevice != null)
                {
                    ReleaseFilenameMembers();

                    m_pCaptureGraph = (IGraphBuilder)new FilterGraph();
                    try
                    {
                        //m_rot2 = new DsROTEntry(m_pCaptureGraph);

                        // Use the bridge to add the sourcefilter to the graph
                        hr = m_pBridge.InsertSourceFilter(m_pSourceGraphSinkFilter, m_pCaptureGraph, out m_pCaptureGraphSourceFilter);
                        DsError.ThrowExceptionForHR(hr);

                        // use capture graph builder to create mux/writer stage
                        pBuilder = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();

                        hr = pBuilder.SetFiltergraph(m_pCaptureGraph);
                        DsError.ThrowExceptionForHR(hr);

                        // create the mux/writer
                        hr = pBuilder.SetOutputFileName(MediaSubType.Avi, pFile, out pfMux, out pSink);
                        DsError.ThrowExceptionForHR(hr);

                        //IConfigAsfWriter lConfig = pfMux as IConfigAsfWriter;

                        //Guid cat = new Guid("7A747920-2449-4D76-99CB-FDB0C90484D4");


                        //hr = lConfig.ConfigureFilterUsingProfileGuid(cat);
                        //Marshal.ThrowExceptionForHR(hr);


                        // render source output to mux
                        hr = pBuilder.RenderStream(null, null, m_pCaptureGraphSourceFilter, null, pfMux);
                        DsError.ThrowExceptionForHR(hr);

                        // Store the file name for later use
                        FileName = pFile;
                    }
                    catch
                    {
                        FreeResources();
                    }
                    finally
                    {
                        if (pBuilder != null)
                        {
                            Marshal.ReleaseComObject(pBuilder);
                        }

                        if (pfMux != null)
                        {
                            Marshal.ReleaseComObject(pfMux);
                        }

                        if (pSink != null)
                        {
                            Marshal.ReleaseComObject(pSink);
                        }
                    }
                }
                else
                {
                    throw new Exception("Device not selected");
                }
            }
            else
            {
                throw new Exception("Invalid parameter");
            }
        }

        public void StartCapture()
        {

            VerifyAccess();

            int hr;

            if (m_captureDevice != null)
            {
                if (FileName != null)
                {
                    // re-enable capture stream
                    IAMStreamControl pSC = (IAMStreamControl)m_pCapOutput;

                    // immediately!
                    pSC.StartAt(null, 0); // Ignore any error

                    // start capture graph
                    IMediaControl pMC = (IMediaControl)m_pCaptureGraph;
                    hr = pMC.Run();
                    DsError.ThrowExceptionForHR(hr);

                    hr = m_pBridge.BridgeGraphs(m_pSourceGraphSinkFilter, m_pCaptureGraphSourceFilter);
                    DsError.ThrowExceptionForHR(hr);

                    // If we make it here, we are capturing
                    m_Capturing = true;
                }
                else
                {
                    throw new Exception("File name not specified");
                }
            }
            else
            {
                throw new Exception("Device not selected");
            }
        }

        // Stop the file capture (leave the preview running)
        public void StopCapture()
        {
            int hr;

            // Are we capturing?
            if (m_captureDevice != null)
            {
                // disconnect segments
                hr = m_pBridge.BridgeGraphs(null, null);
                DsError.ThrowExceptionForHR(hr);

                // stop capture graph
                IMediaControl pMC = (IMediaControl)m_pCaptureGraph;

                hr = pMC.Stop();
                DsError.ThrowExceptionForHR(hr);

                // disable capture stream (to save resources)
                IAMStreamControlBridge pSC = (IAMStreamControlBridge)m_pCapOutput;

                pSC.StartAt(NEVER, 0); // Ignore any error

                m_Capturing = false;
            }
        }
        public void PauseCapture()
        {
            int hr;

            // Are we capturing?
            if (m_captureDevice != null)
            {
                // disconnect segments
                hr = m_pBridge.BridgeGraphs(null, null);
                DsError.ThrowExceptionForHR(hr);

                // stop capture graph
                IMediaControl pMC = (IMediaControl)m_pCaptureGraph;

                hr = pMC.Pause();
                DsError.ThrowExceptionForHR(hr);

                // disable capture stream (to save resources)
                IAMStreamControlBridge pSC = (IAMStreamControlBridge)m_pCapOutput;

               pSC.StartAt(NEVER, 0); // Ignore any error

                // m_Capturing = false;
            }
        }
        private void ReleaseFilenameMembers()
        {
            //if (m_rot2 != null)
            //{
            //    m_rot2.Dispose();
            //    m_rot2 = null;
            //}

            if (m_pCaptureGraphSourceFilter != null)
            {
                Marshal.ReleaseComObject(m_pCaptureGraphSourceFilter);
                m_pCaptureGraphSourceFilter = null;
            }
            if (m_pCaptureGraph != null)
            {
                Marshal.ReleaseComObject(m_pCaptureGraph);
                m_pCaptureGraph = null;
            }
        }

        /// <summary>
        /// Sets the capture parameters for the video capture device
        /// </summary>
        private bool SetVideoCaptureParameters(ICaptureGraphBuilder2 capGraph, IBaseFilter captureFilter, Guid mediaSubType)
        {
            /* The stream config interface */
            object streamConfig;

            /* Get the stream's configuration interface */
            int hr = capGraph.FindInterface(PinCategory.Capture,
                                            MediaType.Video,
                                            captureFilter,
                                            typeof(IAMStreamConfig).GUID,
                                            out streamConfig);

            DsError.ThrowExceptionForHR(hr);

            var videoStreamConfig = streamConfig as IAMStreamConfig;

            /* If QueryInterface fails... */
            if (videoStreamConfig == null)
            {
                throw new Exception("Failed to get IAMStreamConfig");
            }

            /* The media type of the video */
            AMMediaType media;

            /* Get the AMMediaType for the video out pin */
            hr = videoStreamConfig.GetFormat(out media);
            DsError.ThrowExceptionForHR(hr);

            /* Make the VIDEOINFOHEADER 'readable' */
            var videoInfo = new VideoInfoHeader();
            Marshal.PtrToStructure(media.formatPtr, videoInfo);

            /* Setup the VIDEOINFOHEADER with the parameters we want */
            videoInfo.AvgTimePerFrame = DSHOW_ONE_SECOND_UNIT / FPS;
            videoInfo.BmiHeader.Width = DesiredWidth;
            videoInfo.BmiHeader.Height = DesiredHeight;
           
            
            if(mediaSubType != Guid.Empty)
            {
                videoInfo.BmiHeader.Compression = new FourCC(mediaSubType).ToInt32();
                media.subType = mediaSubType;
            }

            /* Copy the data back to unmanaged memory */
            Marshal.StructureToPtr(videoInfo, media.formatPtr, false);

            /* Set the format */
            hr = videoStreamConfig.SetFormat(media);

            /* We don't want any memory leaks, do we? */
            DsUtils.FreeAMMediaType(media);

            if (hr < 0)
                return false;

            return true;
        }

        private Bitmap m_videoFrame;

        private void InitializeBitmapFrame(int width, int height)
        {
            if(m_videoFrame != null)
            {
                m_videoFrame.Dispose();
            }

            m_videoFrame = new Bitmap(width, height, PixelFormat.Format24bppRgb);
        }

        #region ISampleGrabberCB Members

        int ISampleGrabberCB.SampleCB(double sampleTime, IMediaSample pSample)
        {
            var mediaType = new AMMediaType();

            /* We query for the media type the sample grabber is using */
            int hr = m_sampleGrabber.GetConnectedMediaType(mediaType);

            var videoInfo = new VideoInfoHeader();

            /* 'Cast' the pointer to our managed struct */
            Marshal.PtrToStructure(mediaType.formatPtr, videoInfo);

            /* The stride is "How many bytes across for each pixel line (0 to width)" */
            int stride = Math.Abs(videoInfo.BmiHeader.Width * (videoInfo.BmiHeader.BitCount / 8 /* eight bits per byte */));
            int width = videoInfo.BmiHeader.Width;
            int height = videoInfo.BmiHeader.Height;

            if(m_videoFrame == null)
                InitializeBitmapFrame(width, height);

            if (m_videoFrame == null)
                return 0;

            BitmapData bmpData = m_videoFrame.LockBits(new Rectangle(0, 0, width, height), 
                                                       ImageLockMode.ReadWrite, 
                                                       PixelFormat.Format24bppRgb);
           
            /* Get the pointer to the pixels */
            IntPtr pBmp = bmpData.Scan0;

            IntPtr samplePtr;

            /* Get the native pointer to the sample */
            pSample.GetPointer(out samplePtr);

            int pSize = stride * height;

            /* Copy the memory from the sample pointer to our bitmap pixel pointer */
            CopyMemory(pBmp, samplePtr, pSize);

            m_videoFrame.UnlockBits(bmpData);

            InvokeNewVideoSample(new VideoSampleArgs { VideoFrame = m_videoFrame });

            DsUtils.FreeAMMediaType(mediaType);

            /* Dereference the sample COM object */
            Marshal.ReleaseComObject(pSample);
            return 0;
        }

        int ISampleGrabberCB.BufferCB(double sampleTime, IntPtr pBuffer, int bufferLen)
        {
            throw new NotImplementedException();
        }

        #endregion

        private void SetupSampleGrabber(ISampleGrabber sampleGrabber)
        {
            var mediaType = new AMMediaType
                            {
                                majorType = MediaType.Video,
                                subType = MediaSubType.RGB24,
                                formatType = FormatType.VideoInfo
                            };

            int hr = sampleGrabber.SetMediaType(mediaType);

            DsUtils.FreeAMMediaType(mediaType);
            DsError.ThrowExceptionForHR(hr);

            hr = sampleGrabber.SetCallback(this, 0);
            DsError.ThrowExceptionForHR(hr);
        }

        protected override void FreeResources()
        {
            /* We run the StopInternal() to avoid any 
             * Dispatcher VeryifyAccess() issues */
            StopInternal();

            /* Let's clean up the base 
             * class's stuff first */
            base.FreeResources();

            #if DEBUG
            if(m_rotEntry != null)
                m_rotEntry.Dispose();

            m_rotEntry = null;
            #endif
            if(m_videoFrame != null)
            {
                m_videoFrame.Dispose();
                m_videoFrame = null;
            }
            if (m_renderer != null)
            {
                Marshal.FinalReleaseComObject(m_renderer);
                m_renderer = null;
            }
            if (m_captureDevice != null)
            {
                Marshal.FinalReleaseComObject(m_captureDevice);
                m_captureDevice = null;
            }
            if (m_sampleGrabber != null)
            {
                Marshal.FinalReleaseComObject(m_sampleGrabber);
                m_sampleGrabber = null;
            }
            if (m_graph != null)
            {
                Marshal.FinalReleaseComObject(m_graph);
                m_graph = null;

                InvokeMediaClosed(new EventArgs());
            }
            if (m_pBridge != null)
            {
                Marshal.ReleaseComObject(m_pBridge);
                m_pBridge = null;
            }

            if (m_pSourceGraphSinkFilter != null)
            {
                Marshal.ReleaseComObject(m_pSourceGraphSinkFilter);
                m_pSourceGraphSinkFilter = null;
            }

            if (m_pSourceGraphSinkFilter != null)
            {
                Marshal.ReleaseComObject(m_pSourceGraphSinkFilter);
                m_pSourceGraphSinkFilter = null;
            }
        }
    }
}