using Common.util;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace ShowBox.util
{
    public class CoverBorderUtil
    {

        /*
         * 3左侧移入 - 右侧移出
         */
        public static void TranslateXMoveOut(Border CoverBorder, int pageWidth)
        {
            CoverBorder.RenderTransformOrigin = new System.Windows.Point(0.5, 0.5);
            TransformGroup group = new TransformGroup();
            CoverBorder.RenderTransform = group;

            TranslateTransform translateTransform = TransformGroupUtil.GetTranslateTransform(group);
            DoubleAnimation da = new DoubleAnimation(0, pageWidth, new Duration(TimeSpan.FromMilliseconds(400)));
            da.BeginTime = TimeSpan.FromMilliseconds(0);
            IEasingFunction easingFunction = new SineEase() { EasingMode = EasingMode.EaseIn };
            //  da.EasingFunction = easingFunction;
            //da.AccelerationRatio = 0.2;
            da.Completed += (sender1, e1) => CoverBorderMoveOut_Completed(CoverBorder);
            translateTransform.BeginAnimation(TranslateTransform.XProperty, da);
        }

        private static void CoverBorderMoveOut_Completed(Border coverBorder)
        {
            coverBorder.Background = Brushes.White;
            coverBorder.Visibility = Visibility.Collapsed;
        }


        /*
         * 4右侧拉伸 
         */
        public static void ScaleXMoveOut(Border CoverBorder)
        {
            CoverBorder.RenderTransformOrigin = new System.Windows.Point(1.0, 0.5);
            TransformGroup group = new TransformGroup();
            CoverBorder.RenderTransform = group;

            ScaleTransform scaleTransform = TransformGroupUtil.GetScaleTransform(group);
            DoubleAnimation da = new DoubleAnimation(1.0, 0, new Duration(TimeSpan.FromMilliseconds(500)));
            da.BeginTime = TimeSpan.FromMilliseconds(0);
            //da.EasingFunction = easingFunction;  
            da.Completed += (sender1, e1) => CoverBorderMoveOut_Completed(CoverBorder);
            scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, da);
        }

        /*
         * 5中心收缩
         */
        public static void CenterMoveOut(Border CoverBorder)
        {
            CoverBorder.RenderTransformOrigin = new System.Windows.Point(0.5, 0.5);
            TransformGroup group = new TransformGroup();
            CoverBorder.RenderTransform = group;

            ScaleTransform scaleTransform = TransformGroupUtil.GetScaleTransform(group);
            DoubleAnimation da = new DoubleAnimation(1.0, 0, new Duration(TimeSpan.FromMilliseconds(400)));
            da.BeginTime = TimeSpan.FromMilliseconds(0);
            IEasingFunction easingFunction = new SineEase() { EasingMode = EasingMode.EaseOut };
            da.EasingFunction = easingFunction;
            da.Completed += (sender1, e1) => CoverBorderMoveOut_Completed(CoverBorder);
            scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, da);
            scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, da);
        }
    }
}
