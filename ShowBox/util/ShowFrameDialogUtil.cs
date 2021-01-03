using Common.util;
using Model;
using ShowBox.control;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace ShowBox.util
{
    public class ShowFrameDialogUtil
    {


        /*
         * 查看大图
         * 
         * 加载动画
         */
        public static void showAnimation(Border borderBg, Canvas currElement, DControl dControlDto, double toTranslateTransformX, double toTranslateTransformY)
        {
            //淡入
            borderBg.RenderTransformOrigin = new Point(0.5, 0.5);
            DoubleAnimation da = new DoubleAnimation(0, borderBg.Opacity, new Duration(TimeSpan.FromMilliseconds(400)));
            borderBg.BeginAnimation(UIElement.OpacityProperty, da);


            //放大
            TransformGroup group = (TransformGroup)currElement.RenderTransform;
            ScaleTransform scaleTransform = TransformGroupUtil.GetScaleTransform(group);
            DoubleAnimation da1 = new DoubleAnimation(scaleTransform.ScaleX, 1.0, new Duration(TimeSpan.FromMilliseconds(400)));
            scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, da1);
            DoubleAnimation da2 = new DoubleAnimation(scaleTransform.ScaleY, 1.0, new Duration(TimeSpan.FromMilliseconds(400)));
            scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, da2);


            // 平移  
            TranslateTransform translateTransform = TransformGroupUtil.GetTranslateTransform(group);
            DoubleAnimation da3 = new DoubleAnimation(0, toTranslateTransformX, new Duration(TimeSpan.FromMilliseconds(400)));
            translateTransform.BeginAnimation(TranslateTransform.XProperty, da3);
            DoubleAnimation da4 = new DoubleAnimation(0, toTranslateTransformY, new Duration(TimeSpan.FromMilliseconds(400)));
            translateTransform.BeginAnimation(TranslateTransform.YProperty, da4);
        }

        /*
           * 关闭大图
           * 
           * 动画，回到原位，移除大图
           */
        public static void closeAnimation(Canvas frameDialogCanvas, Border borderBg, Canvas innerCanvas, DControl dControlDto, Grid mainContainer)
        {
            double scaleX = dControlDto.width / innerCanvas.Width;
            double scaleY = dControlDto.height / innerCanvas.Height;
            if (scaleX / scaleY > 2 || scaleY / scaleX > 2)
            {
                scaleX = 0.1;
                scaleY = 0.1;
            }


            //1.淡出
            borderBg.RenderTransformOrigin = new Point(0.5, 0.5);
            DoubleAnimation da = new DoubleAnimation(borderBg.Opacity, 0, new Duration(TimeSpan.FromMilliseconds(300)));
            borderBg.BeginAnimation(UIElement.OpacityProperty, da);

            //2.缩放
            TransformGroup group = (TransformGroup)innerCanvas.RenderTransform;
            //  innerCanvas.RenderTransformOrigin = new Point(0.5, 0.5);
            ScaleTransform scaleTransform = TransformGroupUtil.GetScaleTransform(group);
            DoubleAnimation da2 = new DoubleAnimation(scaleTransform.ScaleY, scaleY, new Duration(TimeSpan.FromMilliseconds(300)));
            da2.Completed += (sender, e) => closeAnimation_Completed(frameDialogCanvas, innerCanvas, mainContainer);
            scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, da2);
            DoubleAnimation da22 = new DoubleAnimation(scaleTransform.ScaleX, scaleX, new Duration(TimeSpan.FromMilliseconds(300)));
            scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, da22);

            // 平移  
            TranslateTransform translateTransform = TransformGroupUtil.GetTranslateTransform(group);
            DoubleAnimation da31 = new DoubleAnimation(translateTransform.X, 0, new Duration(TimeSpan.FromMilliseconds(300)));
            translateTransform.BeginAnimation(TranslateTransform.XProperty, da31);
            DoubleAnimation da32 = new DoubleAnimation(translateTransform.Y, 0, new Duration(TimeSpan.FromMilliseconds(300)));
            translateTransform.BeginAnimation(TranslateTransform.YProperty, da32);


            // 淡出模式
            DoubleAnimation da4 = new DoubleAnimation(innerCanvas.Opacity, 0, new Duration(TimeSpan.FromMilliseconds(300)));
            da4.Completed += (sender, e) => closeAnimation_Completed(frameDialogCanvas, innerCanvas, mainContainer);
            innerCanvas.BeginAnimation(UIElement.OpacityProperty, da4);
        }

        /*
         * 关闭弹窗动画完成，开始释放弹窗控件
         */
        public static void closeAnimation_Completed(Canvas frameDialogCanvas, Canvas innerCanvas, Grid mainGrid)
        {
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
            mainGrid.Children.Remove(frameDialogCanvas);

            GC.Collect();
        }


    }
}
