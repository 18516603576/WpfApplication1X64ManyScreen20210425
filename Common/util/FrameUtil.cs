
using System;
using System.Windows;
using System.Windows.Controls;

namespace Common.util
{
    public class FrameUtil
    {
        /*
         * 从历史记录移除最近的页面，避免内存溢出
         */
        public static Boolean RemoveBackEntry(Frame frame)
        {
            bool b = false;
            if (frame.CanGoBack == true)
            {
                frame.RemoveBackEntry();
                b = true;
            }
            return b;
        }

        /*
        * 获取最优百分比
        */
        public static int getMaxPercent(double cfgWidth, double cfgHeight)
        {
            double currScreenWidth = SystemParameters.PrimaryScreenWidth;//得到屏幕整体宽度
            double currentScreenHeight = SystemParameters.PrimaryScreenHeight;//得到屏幕整体高度 

            double w = currScreenWidth / cfgWidth;
            double h = currentScreenHeight / cfgHeight;

            int percent = 100;

            //1.当前屏幕小于配置 
            if (w < h)
            {
                percent = (int)Math.Floor(w * 100);
            }
            else
            {
                percent = (int)Math.Floor(h * 100);
            }
            if (percent > 100) percent = 100;
            return percent;
        }
    }
}
