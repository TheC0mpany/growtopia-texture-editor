using System;
using System.Drawing.Imaging;
using System.Drawing;
using System.Runtime.InteropServices;

namespace growtopia_texture_editor.RTTEX
{
    public class DirectBitmap : IDisposable
    {
        public Bitmap Bitmap { get; private set; }
        public Int32[] Bits { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public bool Disposed { get; private set; }
        private readonly GCHandle _bitsHandle;

        public DirectBitmap(int width, int height)
        {
            Width = width;
            Height = height;
            Bits = new Int32[width * height];
            _bitsHandle = GCHandle.Alloc(Bits, GCHandleType.Pinned);
            Bitmap = new Bitmap(width, height, width * 4, PixelFormat.Format32bppArgb, _bitsHandle.AddrOfPinnedObject());
        }

        public void Dispose()
        {
            if (Disposed) return;

            Disposed = true;
            Bitmap.Dispose();
            _bitsHandle.Free();
        }
    }
}
