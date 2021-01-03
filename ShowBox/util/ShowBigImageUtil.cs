using Common;
using Common.Data;
using Common.util;
using Microsoft.Expression.Interactivity.Input;
using Model;
using Model.dto;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace ShowBox.util
{
    public class ShowBigImageUtil
    {
        /*
         * 显示大图
         */
        public static void showBigImage(string imagePath, DControlDto ctl, Grid mainContainer, Cfg cfg)
        {
            FrameworkElement bigImageElement = FrameworkElementUtil.getByName(mainContainer, "bigImageCanvas");
            if (bigImageElement == null)
            {
                Canvas bigImageCanvas = new Canvas();
                bigImageCanvas.Name = "bigImageCanvas";
                bigImageCanvas.Width = cfg.screenWidth;
                bigImageCanvas.Height = cfg.screenHeight;
                bigImageCanvas.Background = Brushes.Transparent;
                Panel.SetZIndex(bigImageCanvas, 10002);  

                //图片背景，点击隐藏
                Border borderBg = new Border();
                borderBg.Width = cfg.screenWidth;
                borderBg.Height = cfg.screenHeight;
                borderBg.Background = Brushes.Black;
                borderBg.Opacity = 0.6;
                borderBg.HorizontalAlignment = HorizontalAlignment.Left;
                borderBg.VerticalAlignment = VerticalAlignment.Top;
                borderBg.SetValue(Canvas.LeftProperty, 0.0);
                borderBg.SetValue(Canvas.TopProperty, 0.0);
                Panel.SetZIndex(borderBg, 1);
                bigImageCanvas.Children.Add(borderBg);



                //显示大图
                double maxWidth = cfg.screenWidth * 0.9;
                double maxHeight = cfg.screenHeight * 0.9;
                string img = FileUtil.notExistsShowDefault(imagePath, Params.ImageNotExists);
                BitmapImage bitmapImage = FileUtil.readImage(AppDomain.CurrentDomain.BaseDirectory + img);
                double showWidth = bitmapImage.Width;
                double showHeight = bitmapImage.Height;
                double w = bitmapImage.Width / maxWidth;
                double h = bitmapImage.Height / maxHeight;
                if (w > 1 && h > 1)
                {
                    if (w > h)
                    {
                        showWidth = maxWidth;
                        showHeight = bitmapImage.Height / w;
                    }
                    else
                    {
                        showHeight = maxHeight;
                        showWidth = bitmapImage.Width / h;
                    }
                }
                else if (w > 1)
                {
                    showWidth = maxWidth;
                    showHeight = bitmapImage.Height / w;
                }
                else if (h > 1)
                {
                    showHeight = maxHeight;
                    showWidth = bitmapImage.Width / h;
                }

                Canvas innerCanvas = new Canvas();
                innerCanvas.Width = showWidth;
                innerCanvas.Height = showHeight;
                double left = ctl.left;
                double top = ctl.top;
                innerCanvas.SetValue(Canvas.LeftProperty, left);
                innerCanvas.SetValue(Canvas.TopProperty, top);
                Panel.SetZIndex(innerCanvas, 2);


                Image image = new Image();
                image.Source = bitmapImage;
                image.Width = showWidth;
                image.Height = showHeight;
                Panel.SetZIndex(image, 1);
                innerCanvas.Children.Add(image);

                Button closebtn = new Button();
                closebtn.Background = new ImageBrush
                {
                    ImageSource = FileUtil.readImage(AppDomain.CurrentDomain.BaseDirectory + "/myfile/sysimg/ico-image-close.png")
                    ,
                    Stretch = Stretch.Fill
                };

                closebtn.Width = 50;
                closebtn.Height = 50;
                closebtn.BorderThickness = new Thickness(0);
                closebtn.HorizontalAlignment = HorizontalAlignment.Right;
                closebtn.VerticalAlignment = VerticalAlignment.Top;
                closebtn.SetValue(Canvas.RightProperty, -25.0);
                closebtn.SetValue(Canvas.TopProperty, -25.0);
                Panel.SetZIndex(closebtn, 2);

                closebtn.Click += (sender, e) => imageCloseBtnClick(bigImageCanvas, borderBg, innerCanvas, ctl, mainContainer);
                closebtn.PreviewTouchDown += imageCloseBtnTouchDown;
                closebtn.PreviewTouchMove += imageCloseBtnTouchMove;
                closebtn.PreviewTouchUp += (sender, e) => imageCloseBtnClick(bigImageCanvas, borderBg, innerCanvas, ctl, mainContainer);
                innerCanvas.Children.Add(closebtn);


                bigImageCanvas.Children.Add(innerCanvas);
                borderBg.PreviewMouseUp += (sender, e) => imageCloseBtnClick(bigImageCanvas, borderBg, innerCanvas, ctl, mainContainer);
                borderBg.PreviewTouchUp += (sender, e) => imageCloseBtnClick(bigImageCanvas, borderBg, innerCanvas, ctl, mainContainer);

                //手指缩放 移动 旋转
                BehaviorCollection behaviors = Interaction.GetBehaviors(innerCanvas);
                TranslateZoomRotateBehavior tz = new TranslateZoomRotateBehavior();
                tz.TranslateFriction = 0.3;
                tz.RotationalFriction = 0.4;
                tz.ConstrainToParentBounds = true;
                tz.SupportedGestures = ManipulationModes.All;
                behaviors.Add(tz);

                TransformGroup group = new TransformGroup();
                double scaleX = ctl.width / innerCanvas.Width;
                double scaleY = ctl.height / innerCanvas.Height;
                ScaleTransform scaleTransform = new ScaleTransform();
                scaleTransform.ScaleX = scaleX;
                scaleTransform.ScaleY = scaleY;
                group.Children.Add(scaleTransform);
                innerCanvas.RenderTransform = group;

                //动画，平移到指定位置
                double toLeft = (cfg.screenWidth - innerCanvas.Width) / 2;
                double toTop = (cfg.screenHeight - innerCanvas.Height) / 2;
                double toTranslateTransformX = toLeft - ctl.left;
                double toTranslateTransformY = toTop - ctl.top;

                ShowBigImageUtil.showAnimation(borderBg, innerCanvas, ctl, toTranslateTransformX, toTranslateTransformY);

                mainContainer.Children.Add(bigImageCanvas);

            }

        }



        /*
         * 查看大图
         * 
         * 加载动画
         */
        public static void showAnimation(Border borderBg, Canvas currElement, DControlDto ctl, double toTranslateTransformX, double toTranslateTransformY)
        {
            //淡入
            borderBg.RenderTransformOrigin = new Point(0.5, 0.5);
            DoubleAnimation da = new DoubleAnimation(0, borderBg.Opacity, new Duration(TimeSpan.FromMilliseconds(300)));
            borderBg.BeginAnimation(UIElement.OpacityProperty, da);


            //放大
            TransformGroup group = (TransformGroup)currElement.RenderTransform;
            ScaleTransform scaleTransform = TransformGroupUtil.GetScaleTransform(group);
            DoubleAnimation da1 = new DoubleAnimation(scaleTransform.ScaleX, 1.0, new Duration(TimeSpan.FromMilliseconds(300)));
            scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, da1);
            DoubleAnimation da2 = new DoubleAnimation(scaleTransform.ScaleY, 1.0, new Duration(TimeSpan.FromMilliseconds(300)));
            scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, da2);


            // 平移  
            TranslateTransform translateTransform = TransformGroupUtil.GetTranslateTransform(group);
            DoubleAnimation da3 = new DoubleAnimation(0, toTranslateTransformX, new Duration(TimeSpan.FromMilliseconds(300)));
            translateTransform.BeginAnimation(TranslateTransform.XProperty, da3);
            DoubleAnimation da4 = new DoubleAnimation(0, toTranslateTransformY, new Duration(TimeSpan.FromMilliseconds(300)));
            translateTransform.BeginAnimation(TranslateTransform.YProperty, da4);
        }

        /*
           * 关闭大图
           * 
           * 动画，回到原位，移除大图
           */
        public static void closeAnimation(Canvas bigImageCanvas, Border borderBg, Canvas innerCanvas, DControlDto ctl, Grid mainContainer)
        {

            //1.淡出
            borderBg.RenderTransformOrigin = new Point(0.5, 0.5);
            DoubleAnimation da = new DoubleAnimation(borderBg.Opacity, 0, new Duration(TimeSpan.FromMilliseconds(300)));
            borderBg.BeginAnimation(UIElement.OpacityProperty, da);

            //触摸缩放后
            double backToScaleX = ctl.width / innerCanvas.Width;
            double backToScaleY = ctl.height / innerCanvas.Height;

            Transform transform = innerCanvas.RenderTransform;
            TransformGroup group = null;
            if (transform is TransformGroup)
            {
                group = (TransformGroup)innerCanvas.RenderTransform;
            }
            else
            {
                group = new TransformGroup();
                TranslateTransform translateTransform1 = new TranslateTransform();
                MatrixTransform matrixTransform = (MatrixTransform)innerCanvas.RenderTransform;
                translateTransform1.X = matrixTransform.Matrix.OffsetX;
                translateTransform1.Y = matrixTransform.Matrix.OffsetY;
                group.Children.Add(translateTransform1);

                ScaleTransform scaleTransform1 = new ScaleTransform();
                scaleTransform1.ScaleX = matrixTransform.Matrix.M11;
                scaleTransform1.ScaleY = matrixTransform.Matrix.M22;
                group.Children.Add(scaleTransform1);

                innerCanvas.RenderTransform = group;
            }
            //2.缩放
            ScaleTransform scaleTransform = TransformGroupUtil.GetScaleTransform(group);
            DoubleAnimation da1 = new DoubleAnimation(scaleTransform.ScaleX, backToScaleX, new Duration(TimeSpan.FromMilliseconds(300)));
            scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, da1);
            DoubleAnimation da2 = new DoubleAnimation(scaleTransform.ScaleY, backToScaleY, new Duration(TimeSpan.FromMilliseconds(300)));
            scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, da2);


            // 3.平移
            TranslateTransform translateTransform = TransformGroupUtil.GetTranslateTransform(group);
            DoubleAnimation da3 = new DoubleAnimation(translateTransform.X, 0, new Duration(TimeSpan.FromMilliseconds(300)));
            translateTransform.BeginAnimation(TranslateTransform.XProperty, da3);
            DoubleAnimation da4 = new DoubleAnimation(translateTransform.Y, 0, new Duration(TimeSpan.FromMilliseconds(300)));
            da4.Completed += (sender, e) => closeBigImageAnimation_Completed(bigImageCanvas, mainContainer);
            translateTransform.BeginAnimation(TranslateTransform.YProperty, da4);
        }

        /*
         * 关闭大图动画完成，开始释放大图控件
         */
        private static void closeBigImageAnimation_Completed(Canvas bigImageCanvas, Grid mainGrid)
        { 
            releaseBigImage(bigImageCanvas);
            mainGrid.Children.Remove(bigImageCanvas);
        }

        /*
        * 释放大图
        */
        private static void releaseBigImage(Canvas bigImageCanvas)
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

        }




        /*
        * 单击关闭图片
        */
        private static void imageCloseBtnClick(Canvas bigImageCanvas, Border borderBg, Canvas innerCanvas, DControlDto ctl, Grid mainContainer)
        {

            closeAnimation(bigImageCanvas, borderBg, innerCanvas, ctl, mainContainer);

        }
        /*
          * 当单击关闭按钮时，避免进入Manipulation
         */
        private static void imageCloseBtnTouchDown(object sender, TouchEventArgs e)
        {
            e.Handled = true;
        }
        private static void imageCloseBtnTouchMove(object sender, TouchEventArgs e)
        {
            e.Handled = false;
        }

    }
}
