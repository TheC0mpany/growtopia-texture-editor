using System;
using System.Drawing;
using System.IO;
using static growtopia_texture_editor.RTTEX.Constants;

namespace growtopia_texture_editor.RTTEX.Formats
{
    class RTTEX
    {
        public Bitmap Texture { get; private set; }
        public int Height { get; private set; }
        public int Width { get; private set; }
        public GL_FORMATS Format { get; private set; }
        public int OriginalHeight { get; private set; }
        public int OriginalWidth { get; private set; }
        public bool UsesAlpha { get; private set; }
        public bool IsCompressed { get; private set; }
        public int MipmapCount { get; private set; }

        public RTTEX(BinaryReader reader)
        {
            // Read RTTEX header
            reader.BaseStream.Seek(2, SeekOrigin.Current);
            Height = reader.ReadInt32();
            Width = reader.ReadInt32();
            Format = (GL_FORMATS)reader.ReadInt32();

            if (Format != GL_FORMATS.OGL_RGBA_8888)
            {
                throw new NotImplementedException("Only OGL_RGBA_8888 (4 bytes per pixel) formats are supported yet.");
            }

            OriginalHeight = reader.ReadInt32();
            OriginalWidth = reader.ReadInt32();
            UsesAlpha = reader.ReadByte() == 1;
            IsCompressed = reader.ReadByte() == 1;

            short reservedFlags = reader.ReadInt16();
            MipmapCount = reader.ReadInt32();
            int[] rttexReserved = new int[16];
            for (int i = 0; i < 16; i++)
            {
                rttexReserved[i] = reader.ReadInt32();
            }
            reader.BaseStream.Seek(24, SeekOrigin.Current);

            // Read pixel data
            var texture = new DirectBitmap(Width, Height);
            for (int y = Height - 1; y >= 0; y--)
            {
                for (int x = 0; x < Width; x++)
                {
                    int pixel;
                    if (UsesAlpha)
                    {
                        pixel = (reader.ReadByte() << 16) | (reader.ReadByte() << 8) | reader.ReadByte() | (reader.ReadByte() << 24);
                    }
                    else
                    {
                        pixel = (reader.ReadByte() << 16) | (reader.ReadByte() << 8) | reader.ReadByte() | unchecked((int)0xFF000000);
                    }
                    texture.Bits[x + y * Width] = pixel;
                }
            }

            reader.Dispose();
            Texture = texture.Bitmap;
        }
    }
}
