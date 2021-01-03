using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace WinFormCef.CefDiy
{
    class BoundObject
    {

        //打开软键盘 
        public void openKeyboard()
        {
            //  string ProgramFiles = System.Environment.GetEnvironmentVariable("ProgramFiles"); 
            string file = @"C:\Program Files\Common Files\Microsoft Shared\ink\TabTip.exe";
            if (!System.IO.File.Exists(file))
                return;
            Process.Start(file);
        }
        public void closeKeyboard()
        {
            IntPtr TouchhWnd = new IntPtr(0);
            TouchhWnd = FindWindow("IPTip_Main_Window", null);
            if (TouchhWnd == IntPtr.Zero)
                return;
            PostMessage(TouchhWnd, WM_SYSCOMMAND, SC_CLOSE, 0);
        }

        //关闭软键盘
        private const Int32 WM_SYSCOMMAND = 274;
        private const UInt32 SC_CLOSE = 61536;
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool PostMessage(IntPtr hWnd, int Msg, uint wParam, uint lParam);




    }
}
