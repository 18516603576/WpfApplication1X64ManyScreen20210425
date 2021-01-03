using System.Runtime.InteropServices;

namespace Common.util
{
    /*
     * 阻止系统进入屏保，将上一次输入时间更新为当前GetLastInputInfo
     * 
     */
    public class ScreenSaverUtil
    {
        [DllImport("user32.dll")]
        public static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

        /*
         * 向系统发送键盘输入消息
         * 
         * 输入空
         * 
         */
        public static void NoSleep()
        {
             keybd_event(124, 0, 0, 0);//模拟发送回车消息  
        }

    }
}
