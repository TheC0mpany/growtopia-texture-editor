using System;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using growtopia_texture_editor.RTTEX.Formats;
using Microsoft.Win32;
using static growtopia_texture_editor.RTTEX.Constants;

namespace growtopia_texture_editor
{
    public class Helper
    {
        [DllImport("Shell32.dll")]
        private static extern int SHOpenFolderAndSelectItems(IntPtr pidlFolder, uint cidl, IntPtr[] apidl, uint dwFlags);

        [DllImport("Shell32.dll", CharSet = CharSet.Unicode)]
        private static extern int SHParseDisplayName(string pszName, IntPtr pbc, out IntPtr ppidl, uint sfgaoIn, out uint psfgaoOut);

        public static string exePath = System.Reflection.Assembly.GetEntryAssembly().Location;
        public static string exeDirPath = Path.GetDirectoryName(exePath);

        public static void OpenFileLocation(string filePath)
        {
            string directoryPath = Path.GetDirectoryName(filePath);

            IntPtr pidlFolder;
            uint pdwAttributes = 0;
            int result = SHParseDisplayName(directoryPath, IntPtr.Zero, out pidlFolder, pdwAttributes, out pdwAttributes);

            if (result != 0)
            {
                //throw new Win32Exception(result);
            }

            IntPtr pidlFile;
            result = SHParseDisplayName(filePath, IntPtr.Zero, out pidlFile, pdwAttributes, out pdwAttributes);

            if (result != 0)
            {
                //throw new Win32Exception(result);
            }

            IntPtr[] apidl = new IntPtr[] { pidlFile };
            result = SHOpenFolderAndSelectItems(pidlFolder, 1, apidl, 0);

            if (result != 0)
            {
                //throw new Win32Exception(result);
            }

            Marshal.FreeCoTaskMem(pidlFolder);
            Marshal.FreeCoTaskMem(pidlFile);
        }
        public static bool ConvertRTPACKFile(string filename, string outputDirectory)
        {
            using (FileStream fs = new FileStream(filename, FileMode.Open))
            using (BinaryReader br = new BinaryReader(fs))
            {
                string rtpack_magic = new string(br.ReadChars(6));
                if (String.Equals(rtpack_magic, "RTTXTR"))
                {
                    var textureData = new growtopia_texture_editor.RTTEX.Formats.RTTEX(br);
                    textureData.Texture.Save($@"{outputDirectory}\{Path.GetFileNameWithoutExtension(filename)}.png");
                }
                else if (String.Equals(rtpack_magic, "RTPACK"))
                {
                    byte version = br.ReadByte();
                    byte reserved = br.ReadByte();

                    uint compressedSize = br.ReadUInt32();
                    uint decompressedSize = br.ReadUInt32();
                    eCompressionType compressionType = (eCompressionType)br.ReadByte();

                    fs.Seek(15, SeekOrigin.Current);
                    fs.ReadByte();
                    fs.ReadByte();

                    using (MemoryStream ms = new MemoryStream())
                    {
                        if (compressionType == eCompressionType.C_COMPRESSION_ZLIB)
                        {
                            using (DeflateStream zs = new DeflateStream(fs, CompressionMode.Decompress))
                            {
                                zs.CopyTo(ms);
                            }
                        }
                        else
                        {
                            fs.CopyTo(ms);
                        }
                        ms.Position = 0;
                        BinaryReader bdr = new BinaryReader(ms);

                        string decomp_magic = new string(bdr.ReadChars(6));
                        if (String.Equals(decomp_magic, "RTTXTR"))
                        {
                            var textureData = new growtopia_texture_editor.RTTEX.Formats.RTTEX(bdr);
                            textureData.Texture.Save($@"{outputDirectory}\{Path.GetFileNameWithoutExtension(filename)}.png");
                        }
                    }
                    return true;
                }
            }
            return false;
        }
        public static bool IsValidPath(string path)
        {
            char[] invalidChars = Path.GetInvalidPathChars();
            return !string.IsNullOrEmpty(path) && !path.Any(c => invalidChars.Contains(c)) && Directory.Exists(Path.GetDirectoryName(Path.GetFullPath(path)));
        }

        public static string GetGrowtopiaPath()
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Growtopia"))
            {
                if (key != null)
                {
                    return (string)key.GetValue("path");
                }
                else
                {
                    return null;
                }
            }
        }
    }
}