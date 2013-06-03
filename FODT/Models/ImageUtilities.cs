using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;

namespace FODT.Models
{
    public class ImageUtilities
    {
        public static Bitmap LoadBitmap(byte[] image)
        {
            using (var s = new MemoryStream(image))
            {
                using (var b = new Bitmap(s))
                {
                    return CopyBitmap(b);
                }
            }
        }

        public static Bitmap Resize(Bitmap source, float maxWidth, float maxHeight)
        {
            if (source.Width <= maxWidth && source.Height <= maxHeight)
            {
                return CopyBitmap(source);
            }

            var scale = Math.Min(maxWidth / (float)source.Width, maxHeight / (float)source.Height);

            var targetWidth = (int)(source.Width * scale);
            var targetHeight = (int)(source.Height * scale);
            var target = new Bitmap(targetWidth, targetHeight);
            target.SetResolution(source.HorizontalResolution, source.VerticalResolution);

            using (var g = Graphics.FromImage(target))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(source, 0, 0, targetWidth, targetHeight);
                return target;
            }
        }

        public static Bitmap CopyBitmap(Bitmap bitmap)
        {
            var retval = new Bitmap(bitmap.Width, bitmap.Height);
            retval.SetResolution(bitmap.HorizontalResolution, bitmap.VerticalResolution);
            using (Graphics g = Graphics.FromImage(retval))
            {
                g.DrawImage(bitmap, 0, 0, bitmap.Width, bitmap.Height);
            }
            return retval;
        }
    }
}