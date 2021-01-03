using Common.Data;
using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Common.util
{
    /*
     * 页面切换 - 截图
     */
    public class ShotUtil
    {

        public static string shot(FrameworkElement ui)
        {
            string filename = Params.shotImage;
            try
            {
                FileStream ms = new FileStream(filename, System.IO.FileMode.Create);
                RenderTargetBitmap bmp = new RenderTargetBitmap((int)ui.ActualWidth, (int)ui.ActualHeight, 96d, 96d, System.Windows.Media.PixelFormats.Default);
                bmp.Render(ui);
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(System.Windows.Media.Imaging.BitmapFrame.Create(bmp));
                encoder.Save(ms);
                ms.Close();
                return FileUtil.notExistsShowDefault(filename, Params.ImageNotExists);
            }
            catch (Exception ex)
            {
                //记录异常
                Console.WriteLine("截图失败：" + ex.Message);
                return FileUtil.notExistsShowDefault(filename, Params.ImageNotExists);
            }



        }
    }
}
