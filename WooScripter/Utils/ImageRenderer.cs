using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Windows.Media;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.ComponentModel;
using System.Threading;
using System.Drawing;
using Microsoft.Win32;

namespace WooScripter
{
    public enum RS_SCALE
    {
        RS_QUARTER,
        RS_THIRD,
        RS_HALF,
        RS_ONE,
        RS_TWO,
        RS_FOUR
    };

    public class ImageRenderer
    {
        System.Windows.Controls.Image _Image;
        string _XML;
        bool _Continuous;
        int _RenderWidth;
        int _RenderHeight;

        int _Width;
        int _Height;

        public Colour _MinColour { get; set; }
        public Colour _MaxColour { get; set; }

        [DllImport(@"coretracer.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void StartRender();

        [DllImport(@"coretracer.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void StopRender();

        [DllImport(@"coretracer.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void CopyBuffer(float[] buffer);

        [DllImport(@"coretracer.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SyncRender(float[] buffer);

        [DllImport(@"coretracer.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void InitialiseRender(string description);

        [DllImport(@"coretracer.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetCamera(string description);

        [DllImport(@"coretracer.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetViewport(string description);

        public ImageRenderer(System.Windows.Controls.Image image, string xml, int renderWidth, int renderHeight, bool continuous)
        {
            _Image = image;
            _XML = xml;
            _RenderWidth = renderWidth;
            _RenderHeight = renderHeight;
            _Continuous = continuous;

            InitialiseRender(_XML);
        }

        public void UpdateCamera(string cameraXML)
        {
            SetCamera(cameraXML);
        }

        public void GetMinMax()
        {
            _MinColour = new Colour(1000000.0f, 1000000.0f, 1000000.0f);
            _MaxColour = new Colour(0.0f, 0.0f, 0.0f);

            for (int y = 0; y < _Height; y++)
            {
                for (int x = 0; x < _Width; x++)
                {
                    int idx = (x + y * _Width) * 3;
                    float red = _Buffer[idx], green = _Buffer[idx+1], blue = _Buffer[idx+2];
                    
                    if (red < _MinColour._Red) _MinColour._Red = red;
                    if (green < _MinColour._Green) _MinColour._Green = green;
                    if (blue < _MinColour._Blue) _MinColour._Blue = blue;

                    if (red > _MaxColour._Red) _MaxColour._Red = red;
                    if (green > _MaxColour._Green) _MaxColour._Green = green;
                    if (blue > _MaxColour._Blue) _MaxColour._Blue = blue;
                }
            }
        }

        public enum Transfer { Ramp, Exposure };
        public Transfer _TransferType;
        public double _MaxValue;
        public double _ExposureFactor;

        public void TransferFloatToInt(byte[] pixels, int width, int height)
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int red, green, blue;
                    if (_TransferType == Transfer.Ramp)
                    {
                        red = (int)(_Buffer[(x + y * width) * 3] * 255 / _MaxValue);
                        green = (int)(_Buffer[(x + y * width) * 3 + 1] * 255 / _MaxValue);
                        blue = (int)(_Buffer[(x + y * width) * 3 + 2] * 255 / _MaxValue);
                    }
                    else
                    {
                        red = (int)((1 - Math.Exp(-_Buffer[(x + y * width) * 3] * _ExposureFactor)) * 255);
                        green = (int)((1 - Math.Exp(-_Buffer[(x + y * width) * 3 + 1] * _ExposureFactor)) * 255);
                        blue = (int)((1 - Math.Exp(-_Buffer[(x + y * width) * 3 + 2] * _ExposureFactor)) * 255);
                    }

                    red = Math.Min(red, 255);
                    green = Math.Min(green, 255);
                    blue = Math.Min(blue, 255);

                    pixels[(width * y + x) * 4 + 0] = (byte)(blue);
                    pixels[(width * y + x) * 4 + 1] = (byte)(green);
                    pixels[(width * y + x) * 4 + 2] = (byte)(red);
                    pixels[(width * y + x) * 4 + 3] = (byte)(255);
                }
            }
        }

        byte[] _Pixels;
        public void TransferFloatToInt()
        {
            TransferFloatToInt(_Pixels, _Width, _Height);

            Int32Rect rect = new Int32Rect(0, 0, (int)_Width, (int)_Height);

            _WriteableBitmap.WritePixels(rect, _Pixels, _Width * 4, (int)0);

            _Image.Source = _WriteableBitmap;
        }

        float[] _Buffer;

        public void ZoomCopy(float[] srcBuffer, int srcWidth, int srcHeight, float[] destBuffer, int destWidth, int destHeight)
        {
            // lets work out if we're 1:1 on ratio
            float srcRatio = (float)srcWidth/(float)srcHeight;
            float dstRatio = (float)destWidth/(float)destHeight;
            int dstXStart = 0;
            int dstYStart = 0;
            int dstXWidth = destWidth;
            int dstYHeight = destHeight;
            if (Math.Abs(srcRatio-dstRatio)>0.05)
            {
                if (dstRatio>srcRatio)
                {
                    dstXWidth = destHeight*srcWidth/srcHeight;
                    dstXStart = (int)(((float)destWidth - dstXWidth) * 0.5f);
                }
                else
                {
                    dstYHeight = destWidth * srcHeight / srcWidth;
                    dstYStart = (int)(((float)destHeight - dstYHeight) * 0.5f);
                }
            }

            float srcPosX = 0;
            float srcPosY = 0;
            float srcDeltaX = ((float)(srcWidth - 1)) / ((float)(dstXWidth - 1));
            float srcDeltaY = ((float)(srcHeight - 1)) / ((float)(dstYHeight - 1)); 

            for (int y = dstYStart; y < dstYStart+dstYHeight; y++)
            {
                for (int x = dstXStart; x < dstXStart+dstXWidth; x++)
                {
                    destBuffer[3 * (y * destWidth + x)] = srcBuffer[3 * (((int)srcPosY * srcWidth) + (int)srcPosX)];
                    destBuffer[3 * (y * destWidth + x) + 1] = srcBuffer[3 * (((int)srcPosY * srcWidth) + (int)srcPosX) + 1];
                    destBuffer[3 * (y * destWidth + x) + 2] = srcBuffer[3 * (((int)srcPosY * srcWidth) + (int)srcPosX) + 2];

                    srcPosX += srcDeltaX;
                }
                srcPosY += srcDeltaY;
                srcPosX = 0;
            }
        }

        int idx = 0;

        public void TransferLatest()
        {
            float[] renderBuffer = new float[_RenderHeight * _RenderWidth * 3];
            CopyBuffer(renderBuffer);

            ZoomCopy(renderBuffer, _RenderWidth, _RenderHeight, _Buffer, _Width, _Height);

            GetMinMax();

//            _MaxValue = Math.Max(_MaxColour._Red, Math.Max(_MaxColour._Green, _MaxColour._Blue));

            TransferFloatToInt();
            idx++;
        }

        WriteableBitmap _WriteableBitmap;
        public void Render()
        {
            _Width = (int)_Image.Width;
            _Height = (int)_Image.Height;
            _Buffer = new float[_Height * _Width * 3];
            _Pixels = new byte[4 * _Height * _Width];
            _WriteableBitmap = new WriteableBitmap((int)_Width, (int)_Height, 96, 96, PixelFormats.Bgra32, null);
                
            string XML = @"
<VIEWPORT width=" + _RenderWidth + @" height=" + _RenderHeight + @"/>";
            SetViewport(XML);

            if (_Continuous)
            {
                StartRender();
            }
            else
            {
                float[] renderBuffer = new float[_RenderHeight * _RenderWidth * 3];
                SyncRender(renderBuffer);
                ZoomCopy(renderBuffer, _RenderWidth, _RenderHeight, _Buffer, _Width, _Height);

                GetMinMax();

                _MaxValue = Math.Max(_MaxColour._Red, Math.Max(_MaxColour._Green, _MaxColour._Blue));
                _TransferType = Transfer.Ramp;

                TransferFloatToInt();
            }

        }

        public void Stop()
        {
            StopRender();
        }

        public void Save()
        {
            // Save image
            float[] renderBuffer = new float[_RenderHeight * _RenderWidth * 3];
            SyncRender(renderBuffer);
            _Buffer = new float[_RenderHeight * _RenderWidth * 3];
            ZoomCopy(renderBuffer, _RenderWidth, _RenderHeight, _Buffer, _RenderWidth, _RenderHeight);

            byte[] pixels = new byte[_RenderHeight * _RenderWidth * 4];

            TransferFloatToInt(pixels, _RenderWidth, _RenderHeight);

            Bitmap bmp = new Bitmap(_RenderWidth, _RenderHeight);
            int x, y;
            for (y = 0; y < _RenderHeight; y++)
            {
                for (x = 0; x < _RenderWidth; x++)
                {
                    bmp.SetPixel(x, y, System.Drawing.Color.FromArgb(pixels[(x + y * _RenderWidth)*4 + 2],
                        pixels[(x + y * _RenderWidth)*4 + 1],
                        pixels[(x + y * _RenderWidth)*4 + 0]));
                }
            }

            string store = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + "\\WooScripter\\Exports";
            if (!System.IO.Directory.Exists(store))
            {
                System.IO.Directory.CreateDirectory(store);
            }

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.InitialDirectory = store;
            saveFileDialog1.Filter = "PNG (*.png)|*.png";
            saveFileDialog1.FilterIndex = 1;

            if (saveFileDialog1.ShowDialog() == true)
            {
                bmp.Save(saveFileDialog1.FileName, System.Drawing.Imaging.ImageFormat.Png);
                bmp.Save(saveFileDialog1.FileName.Replace(".png", ".jpg"), System.Drawing.Imaging.ImageFormat.Jpeg);
            }
        }
    }
}
