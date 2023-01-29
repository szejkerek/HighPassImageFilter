//
// Author: Bartłomiej Gordon sem.5 2022/2023
//
// Topic: Implementation of High - Pass image filter in both: CPPand
//        ASM with time comparison using given convolution mask:
//
//		-1 - 1 - 1
//		-1   9 - 1
//		-1 - 1 - 1
//
// Implementation of of interface for Bitmap class

using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace JA_Projekt.Main
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
    internal class CustomBitmap
    {
        private const PixelFormat Rgb24Format = PixelFormat.Format24bppRgb;
        private int _pixelStride = 3;
        private int _stride;

        private byte[] _bufferOriginal;
        private byte[] _bufferNew;
        private int _width;
        private int _height;
        private int _size;    

        public byte[] BufferOld { get => _bufferOriginal; set => _bufferOriginal = value; }
        public byte[] BufferNew { get => _bufferNew; set => _bufferNew = value; }
        public int Width { get => _width; }
        public int Height { get => _height; }
        public int Size { get => _size; }
        public int PixelStride { get => _pixelStride; }
        public int Stride { get => _stride; set => _stride = value; }

        public void LoadFromFile(String path)
        {
            Bitmap originalBitmap = new Bitmap(Image.FromFile(path));
            _bufferOriginal = BitmapToByteArray(originalBitmap);
            _bufferNew = BitmapToByteArray(originalBitmap, false);
        }

        public void SaveToFile(String path)
        {
            Bitmap newBitmap = ByteArrayToBitmap(BufferNew);
            newBitmap.Save(path);
        }

        public Bitmap ByteArrayToBitmap(byte[] byteIn)
        {
            Bitmap outputBitmap = new Bitmap(_width, _height, Rgb24Format);
            BitmapData bmpData = outputBitmap.LockBits(new Rectangle(0, 0, _width, _height), ImageLockMode.WriteOnly, Rgb24Format);
            Marshal.Copy(byteIn, 0, bmpData.Scan0, _size);
            outputBitmap.UnlockBits(bmpData);
            return outputBitmap;
        }

        byte[] BitmapToByteArray(Bitmap bmp, bool fillWithData = true)
        {
            _width = bmp.Width;
            _height = bmp.Height;
            BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, _width, _height), ImageLockMode.ReadOnly, Rgb24Format);
            _stride = bmpData.Stride;
            _size = (Math.Abs(bmpData.Stride) * _height);
            byte[] outputBytes = new byte[_size];

            if (fillWithData)
            {
                Marshal.Copy(bmpData.Scan0, outputBytes, 0, _size);
            }

            bmp.UnlockBits(bmpData);
            return outputBytes;
        }
    }
}
