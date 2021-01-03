using ShowBox.control;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ShowBox.util
{
    public class ReleaseUtil
    {
        /*
         * 1 释放所有弹窗
         */
        public static void   realeaseAllFrameDialog(Grid mainContainerTmp)
        { 
            for (int i = 0; i < mainContainerTmp.Children.Count; i++)
            {
                FrameworkElement Ei = (FrameworkElement)mainContainerTmp.Children[i];
                if (Ei is Canvas && Ei.Name == "frameDialogCanvas")
                {
                    Canvas frameDialogCanvas = (Canvas)Ei; 
                    ReleaseOneFrameDialog( mainContainerTmp, frameDialogCanvas); 
                }
            }
        }

        /*
         * 1.1 释放一个弹窗
         */
        public static void ReleaseOneFrameDialog( Grid mainGrid, Canvas frameDialogCanvas )
        {
            for (int j = 0; j < frameDialogCanvas.Children.Count; j++)
            {
                FrameworkElement Ej = (FrameworkElement)frameDialogCanvas.Children[j];
                if (Ej is Canvas && Ej.Name == "innerCanvas")
                {
                    Canvas innerCanvas = (Canvas)Ej;

                    foreach (FrameworkElement ele in innerCanvas.Children)
                    {
                        if (ele is CFrame)
                        {
                            CFrame cFrame = (CFrame)ele;
                            cFrame.Content = null;
                            cFrame = null;
                        }
                        else if (ele is Button)
                        {
                            Button closebtn = (Button)ele;
                            closebtn.Background = null;
                        }
                    }

                    break; 
                }
            } 
            mainGrid.Children.Remove(frameDialogCanvas);  
        }

        /*
         * 2.1 释放一个大图
         */
        private static void ReleaseOneBigImage(Canvas bigImageCanvas,Grid mainGrid)
        {
            foreach (FrameworkElement ele in bigImageCanvas.Children)
            {
                if (ele is Canvas)
                {
                    Canvas innerCanvas = (Canvas)ele;
                    foreach (FrameworkElement ele2 in innerCanvas.Children)
                    {
                        if (ele2 is Image)
                        {
                            Image image = (Image)ele2;
                            image.Source = null;
                            image = null;
                        }
                        if (ele2 is Button)
                        {
                            Button closebtn = (Button)ele2;
                            closebtn.Background = null;
                            closebtn = null;
                        }
                    } 
                }
            } 
            mainGrid.Children.Remove(bigImageCanvas); 
        }


        /*
         * 3.1 释放一个浏览器
         */
        private static void ReleaseOneBrowser(Canvas bigImageCanvas, Grid mainGrid)
        { 
            //如果浏览器打开了，则关闭
            if (App.localStorage.currForm1 != null && App.localStorage.currForm1.IsDisposed == false)
            {
                App.localStorage.currForm1.Close();
                App.localStorage.currForm1.Dispose();
                App.localStorage.currForm1 = null;
            } 
        }


        /*
         * 4.1 释放一个全屏视频
         */
        public static void ReleaseOneFullScreenVideo(Grid mainGrid, Canvas frameDialogCanvas)
        {
            for (int j = 0; j < frameDialogCanvas.Children.Count; j++)
            {
                FrameworkElement Ej = (FrameworkElement)frameDialogCanvas.Children[j];
                if (Ej is Canvas && Ej.Name == "innerCanvas")
                {
                    Canvas innerCanvas = (Canvas)Ej;

                    foreach (FrameworkElement ele in innerCanvas.Children)
                    {
                        if (ele is CFrame)
                        {
                            CFrame cFrame = (CFrame)ele;
                            cFrame.Content = null;
                            cFrame = null;
                        }
                        else if (ele is Button)
                        {
                            Button closebtn = (Button)ele;
                            closebtn.Background = null;
                        }
                    }

                    break;
                }
            }
            mainGrid.Children.Remove(frameDialogCanvas);
            GC.Collect();
        }


    }
}
