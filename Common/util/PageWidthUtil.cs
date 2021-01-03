using Model;

namespace Common.util
{
    public class PageWidthUtil
    {
        /*
         * 获取页面的宽度
         * 
         * pageWidth=0，则返回触摸屏的宽度
         */
        public static int getPageWidth(int pageWidth, int cfgWidth)
        {
            int result = pageWidth;
            if (result <= 0) result = cfgWidth;
            return result;
        }

        /*
        * 获取页面的宽度
        * 
        * pageWidth=0，则返回触摸屏的宽度
        */
        public static Cfg getPageCfg(DPage dPage, Cfg appCfg)
        {
            Cfg result = new Cfg();
            result.screenWidth = dPage.width;
            result.screenHeight = dPage.height;

            if (result.screenWidth <= 0 || result.screenHeight <= 0)
            {
                result.screenWidth = appCfg.screenWidth;
                result.screenHeight = appCfg.screenHeight;
            }
            return result;
        }
    }
}
