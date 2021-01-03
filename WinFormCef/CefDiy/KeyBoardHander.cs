using CefSharp;
using System;
using System.Windows.Forms;

namespace WinformCef.CefDiy
{
    class KeyBoardHander : IKeyboardHandler
    {

        public KeyBoardHander()
        {

        }

        public bool OnKeyEvent(IWebBrowser browserControl, IBrowser browser, KeyType type, int windowsKeyCode, int nativeKeyCode, CefEventFlags modifiers, bool isSystemKey)
        {
            var key = (Keys)windowsKeyCode;
            if (type == KeyType.KeyUp && Enum.IsDefined(typeof(Keys), windowsKeyCode))
            {

                switch (key)
                {
                    case Keys.F5:
                        browser.Reload();
                        break;

                }
            }


            if (key == Keys.S)
            {
                //if (modifiers == (CefEventFlags.ControlDown | CefEventFlags.ShiftDown | CefEventFlags.AltDown))
                //{

                //}
                // browser.Reload(); //此处可以添加想要实现的代码段
                IBrowserHost host = browser.GetHost();
                if (host != null)
                {
                    host.SetFocus(true);
                }

            }

            return false;
        }

        public bool OnPreKeyEvent(IWebBrowser browserControl, IBrowser browser, KeyType type, int windowsKeyCode, int nativeKeyCode, CefEventFlags modifiers, bool isSystemKey, ref bool isKeyboardShortcut)
        {


            return false;
        }
    }
}
