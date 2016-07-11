using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;

namespace ImageConverters.Infrastructure
{
    public static class ImageHelper
    {
        // Fields
        private static float[][] ColorMatrix = null;

        // Methods
        static ImageHelper()
        {
            float[][] numArray = new float[5][];
            numArray[0] = new float[] { 0.299f, 0.299f, 0.299f, 0f, 0f };
            numArray[1] = new float[] { 0.587f, 0.587f, 0.587f, 0f, 0f };
            numArray[2] = new float[] { 0.114f, 0.114f, 0.114f, 0f, 0f };
            float[] numArray2 = new float[5];
            numArray2[3] = 1f;
            numArray[3] = numArray2;
            numArray2 = new float[5];
            numArray2[4] = 1f;
            numArray[4] = numArray2;
            ColorMatrix = numArray;
        }

        public static Bitmap ConstructRGB24Bitmap(byte[] coreData, int width, int height)
        {
            Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            BitmapData bitmapdata = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
            Marshal.Copy(coreData, 0, bitmapdata.Scan0, coreData.Length);
            bitmap.UnlockBits(bitmapdata);
            return bitmap;
        }

        public static Image Convert(byte[] buff)
        {
            MemoryStream stream = new MemoryStream(buff);
            Image image = Image.FromStream(stream);
            stream.Close();
            return image;
        }

        public static byte[] Convert(Image img)
        {
            Image image = CopyImageDeeply(img);
            MemoryStream stream = new MemoryStream();
            image.Save(stream, ImageFormat.Jpeg);
            byte[] buffer = stream.ToArray();
            stream.Close();
            image.Dispose();
            return buffer;
        }

        public static Bitmap ConvertToGrey(Image origin)
        {
            Bitmap image = new Bitmap(origin);
            using (Graphics graphics = Graphics.FromImage(image))
            {
                ImageAttributes imageAttr = new ImageAttributes();
                ColorMatrix newColorMatrix = new ColorMatrix(ColorMatrix);
                imageAttr.SetColorMatrix(newColorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                graphics.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imageAttr);
            }
            return image;
        }

        public static Icon ConvertToIcon(Image img, int iconLength)
        {
            using (Bitmap bitmap = new Bitmap(img, new Size(iconLength, iconLength)))
            {
                return Icon.FromHandle(bitmap.GetHicon());
            }
        }

        public static Image ConvertToJPG(Image img)
        {
            MemoryStream stream = new MemoryStream();
            img.Save(stream, ImageFormat.Jpeg);
            Image image = Image.FromStream(stream);
            stream.Close();
            return image;
        }

        public static Image CopyImageDeeply(Image img)
        {
            Bitmap image = new Bitmap(img.Width, img.Height, img.PixelFormat);
            using (Graphics graphics = Graphics.FromImage(image))
            {
                graphics.DrawImage(img, 0, 0, img.Width, img.Height);
            }
            return image;
        }

        public static Bitmap GetPart(Image origin, Rectangle rect)
        {
            Bitmap image = new Bitmap(rect.Width, rect.Height);
            using (Graphics graphics = Graphics.FromImage(image))
            {
                graphics.DrawImage(origin, rect);
            }
            return image;
        }

        public static byte[] GetRGB24CoreData(Bitmap bm)
        {
            byte[] destination = new byte[(bm.Width * bm.Height) * 3];
            BitmapData bitmapdata = bm.LockBits(new Rectangle(0, 0, bm.Width, bm.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            Marshal.Copy(bitmapdata.Scan0, destination, 0, destination.Length);
            bm.UnlockBits(bitmapdata);
            return destination;
        }

        public static bool IsGif(Image img)
        {
            FrameDimension dimension = new FrameDimension(img.FrameDimensionsList[0]);
            return (img.GetFrameCount(dimension) > 1);
        }

        public static byte[] ReviseRGB24Data(byte[] origin, Size originSize, Size newSize)
        {
            Bitmap image = ConstructRGB24Bitmap(origin, originSize.Width, originSize.Height);
            Bitmap bitmap2 = new Bitmap(newSize.Width, newSize.Height);
            using (Graphics graphics = Graphics.FromImage(bitmap2))
            {
                graphics.DrawImage(image, 0f, 0f, new RectangleF(0f, 0f, (float)newSize.Width, (float)newSize.Height), GraphicsUnit.Pixel);
            }
            return GetRGB24CoreData(bitmap2);
        }

        public static Bitmap RoundSizeByNumber(Bitmap origin, int number)
        {
            if (((origin.Width % number) == 0) && ((origin.Height % number) == 0))
            {
                return origin;
            }
            int width = (origin.Width / number) * number;
            int height = (origin.Height / number) * number;
            Bitmap image = new Bitmap(width, height);
            using (Graphics graphics = Graphics.FromImage(image))
            {
                graphics.DrawImage(origin, new Rectangle(0, 0, width, height));
            }
            return image;
        }

        public static void Save(Image img, string path, ImageFormat format)
        {
            if ((img != null) && (path != null))
            {
                CopyImageDeeply(img).Save(path, format);
            }
        }

        public static Bitmap Zoom(Image origin, float zoomCoef)
        {
            Bitmap image = new Bitmap((int)(origin.Width * zoomCoef), (int)(origin.Height * zoomCoef));
            using (Graphics graphics = Graphics.FromImage(image))
            {
                graphics.DrawImage(origin, new Rectangle(0, 0, image.Width, image.Height));
            }
            return image;
        }
    }
}