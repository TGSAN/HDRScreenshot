using System;
using System.IO;
using System.Runtime.InteropServices;

namespace HDRScreenshot
{
    internal class FolderNameHelper
    {
        public static string GetLocalizedFolderName(string folderPath)
        {
            IntPtr pszName;
            IntPtr pidl = Win32.ILCreateFromPathW(folderPath);

            if (pidl != IntPtr.Zero)
            {
                try
                {
                    if (Win32.SHGetNameFromIDList(pidl, Win32.SIGDN.NORMALDISPLAY, out pszName) == 0)
                    {
                        string name = Marshal.PtrToStringAuto(pszName);
                        Marshal.FreeCoTaskMem(pszName); // 释放内存
                        return name;
                    }
                }
                finally
                {
                    Win32.ILFree(pidl);
                }
            }
            return Path.GetFileName(folderPath); // 失败时回退
        }
    }
}
