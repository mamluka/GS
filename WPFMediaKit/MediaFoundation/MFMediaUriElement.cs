using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using WPFMediaKit.DirectShow.Controls;
using WPFMediaKit.DirectShow.MediaPlayers;

namespace WPFMediaKit.MediaFoundation
{
    public class MFMediaUriElement : D3DRenderer
    {
        private HiddenWindow window;

        public MFMediaUriElement()
        {
            window = new HiddenWindow();
            window.CreateHandle(new CreateParams());

            Player = new MFMediaUriPlayer(window.Handle, window.Handle);
            Player.NewAllocatorFrame += new Action(Player_NewAllocatorFrame);
            Player.NewAllocatorSurface += new NewAllocatorSurfaceDelegate(Player_NewAllocatorSurface);
            Player.OpenURL(@"E:\olddesktop\Coral_Reef_Adventure_1080.wmv");
            Thread.Sleep(2500);
            Player.Pause();
            Thread.Sleep(2500);
            Player.Play();
            
        }

        void Player_NewAllocatorSurface(object sender, IntPtr pSurface)
        {
            SetBackBuffer(pSurface);
        }

        void Player_NewAllocatorFrame()
        {
            InvalidateVideoImage();
        }

        protected MFMediaUriPlayer Player
        {
            get;
            set;
        }
    }
}
