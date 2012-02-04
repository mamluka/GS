using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace ConvertDemo
{
    /// <summary>
    /// Conversion thunk.
    /// </summary>
    class PictureDispHost : AxHost
    {
        /// <summary>
        /// Default Constructor, required by the framework.
        /// </summary>
        private PictureDispHost() : base(string.Empty) { }
        /// <summary>
        /// Convert the image to an picturedisp.
        /// </summary>
        /// <param name="image">The image instance</param>
        /// <returns>The picture dispatch object.</returns>
        public new static object GetIPictureDispFromPicture(Image image)
        {
            return AxHost.GetIPictureDispFromPicture(image);
        }
        /// <summary>
        /// Convert the dispatch interface into an image object.
        /// </summary>
        /// <param name="picture">The picture interface</param>
        /// <returns>An image instance.</returns>
        public new static Image GetPictureFromIPicture(object picture)
        {
            return AxHost.GetPictureFromIPicture(picture);
        }
    }
}
