using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace growtopia_texture_editor.RTTEX
{
    public static class Utils
    {
        public static string BytesToString(long byteCount)
        {
            string[] suffixes = { "B", "KB", "MB", "GB", "TB", "PB", "EB" };
            if (byteCount == 0)
            {
                return $"0{suffixes[0]}";
            }

            long bytes = Math.Abs(byteCount);
            int suffixIndex = (int)Math.Floor(Math.Log(bytes, 1024));
            double num = Math.Round(bytes / Math.Pow(1024, suffixIndex), 1);
            string suffix = suffixes[suffixIndex];

            return $"{Math.Sign(byteCount) * num}{suffix}";
        }
    }
}
