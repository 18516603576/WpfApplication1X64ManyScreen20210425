using System;
using System.Diagnostics;
using System.IO;

namespace Common.util
{
    public class VideoUtil
    { 
        /*
         * 视频时长转分秒mm:ss
         */
        public static string   duration2mmss(int seconds)
        { 
             int mm = (int)System.Math.Floor(seconds / 60.0); 
             int ss = Convert.ToInt32( seconds % 60.0);

            string mmStr = mm.ToString() ;
            if (mm < 10)
            {
                mmStr = "0" + mm;
            }
            string ssStr = ss.ToString();
            if (ss < 10)
            {
                ssStr = "0" + ss;
            }
            
            
             return mmStr + ":"+ ssStr;

        }
    }
}
