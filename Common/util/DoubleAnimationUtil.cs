using Model;
using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace Common.util
{
    public class DoubleAnimationUtil
    {
        /*
         * 初始化一个动作1
         * 
         * @param DControlAnimation animation 动画数据
         * @param double fromValue  开始值
         * @param double toValue  结束值
         * 
         */
        public static DoubleAnimation initDoubleAnimation(DControlAnimation animation, double fromValue, double toValue)
        {
            IEasingFunction easingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
            return initDoubleAnimation(animation, fromValue, toValue, easingFunction);
        }
        /*
          * 初始化一个动作2
          * 
          * @param DControlAnimation animation 动画数据
          * @param double fromValue  开始值
          * @param double toValue  结束值
          * @param IEasingFunction easingFunction 缓动函数
          * 
          */
        public static DoubleAnimation initDoubleAnimation(DControlAnimation animation, double fromValue, double toValue, IEasingFunction easingFunction)
        {
            DoubleAnimation da = new DoubleAnimation(fromValue, toValue, new Duration(TimeSpan.FromMilliseconds(animation.durationSeconds)));
            da.BeginTime = TimeSpan.FromMilliseconds(animation.delaySeconds);
            if (!animation.isSameSpeed)
            {
                da.EasingFunction = easingFunction;
            }
            if (animation.playTimes <= 0)
            {
                da.RepeatBehavior = RepeatBehavior.Forever;
            }
            else
            {
                da.RepeatBehavior = new RepeatBehavior(animation.playTimes);
            }
            return da;
        }

        /*
         * 伴随动透明度动画1
         * 
         * @param DControlAnimation animation 动画数据
         * @param double fromValue  开始值
         * @param double toValue  结束值 
         * 
         */
        public static void andBeginOpacityAnimation(FrameworkElement element, DControlAnimation animation, double fromValue, double toValue)
        {
            IEasingFunction easingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
            andBeginOpacityAnimation(element, animation, fromValue, toValue, easingFunction);
        }

        /*
         * 伴随动透明度动画2
         * 
         * @param DControlAnimation animation 动画数据
         * @param double fromValue  开始值
         * @param double toValue  结束值 
         * @param IEasingFunction easingFunction 缓动函数
         * 
         */
        public static void andBeginOpacityAnimation(FrameworkElement element, DControlAnimation animation, double fromValue, double toValue, IEasingFunction easingFunction)
        {
            if (animation.isSameOpacity) return;
            element.Opacity = 0;
            DoubleAnimation da = new DoubleAnimation(fromValue, toValue, new Duration(TimeSpan.FromMilliseconds(animation.durationSeconds)));
            da.BeginTime = TimeSpan.FromMilliseconds(animation.delaySeconds);
            if (!animation.isSameSpeed)
            {
                da.EasingFunction = easingFunction;
            }
            if (animation.playTimes <= 0)
            {
                da.RepeatBehavior = RepeatBehavior.Forever;
            }
            else
            {
                da.RepeatBehavior = new RepeatBehavior(animation.playTimes);
            }
            element.BeginAnimation(UIElement.OpacityProperty, da);
        }
    }
}
