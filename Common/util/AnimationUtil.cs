using Model;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Common.util
{
    public class AnimationUtil
    {
        /*
         * 加载所有动画效果
         * 
         * @param currElement 当前控件
         * 
         * @param currDControl  当前控件数据
         * 
         * @param  list 动画列表
         */
        public static void loadAllAnimation(FrameworkElement currElement, DControl currDControl, List<DControlAnimation> list, Cfg cfg)
        {
            double currOpacity = currDControl.opacity / 100.0;
            TransformGroup group = (TransformGroup)currElement.RenderTransform;


            // group.Children.Clear();   多次加载动画需要归零
            foreach (DControlAnimation animation in list)
            {
                if (animation.type == 1001)
                {
                    //淡入
                    currElement.RenderTransformOrigin = new Point(0.5, 0.5);
                    currElement.Opacity = 0;
                    IEasingFunction easingFunction = new CubicEase() { EasingMode = EasingMode.EaseIn };
                    DoubleAnimation da = DoubleAnimationUtil.initDoubleAnimation(animation, 0, currOpacity, easingFunction);
                    currElement.BeginAnimation(UIElement.OpacityProperty, da);
                     
                }
                else if (animation.type == 1002)
                {
                    //从左移入
                    currElement.RenderTransformOrigin = new Point(0.5, 0.5);


                    TranslateTransform translateTransform = TransformGroupUtil.GetTranslateTransform(group);
                    DoubleAnimation da = DoubleAnimationUtil.initDoubleAnimation(animation, -currDControl.left - currDControl.width, 0);
                    translateTransform.BeginAnimation(TranslateTransform.XProperty, da);

                    DoubleAnimationUtil.andBeginOpacityAnimation(currElement, animation, 0, currOpacity);
                }
                else if (animation.type == 1003)
                {
                    //从右移入
                    currElement.RenderTransformOrigin = new Point(0.5, 0.5);

                    TranslateTransform translateTransform = TransformGroupUtil.GetTranslateTransform(group);
                    DoubleAnimation da = DoubleAnimationUtil.initDoubleAnimation(animation, cfg.screenWidth - currDControl.left, 0);
                    translateTransform.BeginAnimation(TranslateTransform.XProperty, da);

                    DoubleAnimationUtil.andBeginOpacityAnimation(currElement, animation, 0, currOpacity);
                }
                else if (animation.type == 1004)
                {
                    //从上移入
                    currElement.RenderTransformOrigin = new Point(0.5, 0.5);

                    TranslateTransform translateTransform = TransformGroupUtil.GetTranslateTransform(group);
                    DoubleAnimation da = DoubleAnimationUtil.initDoubleAnimation(animation, -currDControl.top - currDControl.height, 0);

                    translateTransform.BeginAnimation(TranslateTransform.YProperty, da);

                    DoubleAnimationUtil.andBeginOpacityAnimation(currElement, animation, 0, currOpacity);
                }
                else if (animation.type == 1005)
                {
                    //从下移入
                    currElement.RenderTransformOrigin = new Point(0.5, 0.5);

                    TranslateTransform translateTransform = TransformGroupUtil.GetTranslateTransform(group);
                    DoubleAnimation da = DoubleAnimationUtil.initDoubleAnimation(animation, cfg.screenHeight - currDControl.top, 0);

                    translateTransform.BeginAnimation(TranslateTransform.YProperty, da);

                    DoubleAnimationUtil.andBeginOpacityAnimation(currElement, animation, 0, currOpacity);
                }

                else if (animation.type == 1006)
                {
                    //放大
                    currElement.RenderTransformOrigin = new Point(0.5, 0.5);

                    ScaleTransform scaleTransform = TransformGroupUtil.GetScaleTransform(group);
                    DoubleAnimation da = DoubleAnimationUtil.initDoubleAnimation(animation, 0.5, 1.0);
                    scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, da);
                    scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, da);


                    DoubleAnimationUtil.andBeginOpacityAnimation(currElement, animation, 0, currOpacity);
                }
                else if (animation.type == 1007)
                {
                    //缩小
                    currElement.RenderTransformOrigin = new Point(0.5, 0.5);

                    ScaleTransform scaleTransform = TransformGroupUtil.GetScaleTransform(group);
                    DoubleAnimation da = DoubleAnimationUtil.initDoubleAnimation(animation, 1.5, 1.0);
                    scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, da);
                    scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, da);

                    DoubleAnimationUtil.andBeginOpacityAnimation(currElement, animation, 0, currOpacity);

                }
                else if (animation.type == 1101)
                {
                    //从左旋转 
                    currElement.RenderTransformOrigin = new Point(0.5, 0.5);

                    RotateTransform rotateTransform = TransformGroupUtil.GetRotateTransform(group);
                    DoubleAnimation da = DoubleAnimationUtil.initDoubleAnimation(animation, 180 + currDControl.rotateAngle, 360 + currDControl.rotateAngle);
                    rotateTransform.BeginAnimation(RotateTransform.AngleProperty, da);


                    TranslateTransform translateTransform = TransformGroupUtil.GetTranslateTransform(group);
                    DoubleAnimation da2 = DoubleAnimationUtil.initDoubleAnimation(animation, -currDControl.width * 1, 0);
                    translateTransform.BeginAnimation(TranslateTransform.XProperty, da2);


                    DoubleAnimationUtil.andBeginOpacityAnimation(currElement, animation, 0, currOpacity);
                }
                else if (animation.type == 1102)
                {
                    //从右旋转
                    currElement.RenderTransformOrigin = new Point(0.5, 0.5);

                    RotateTransform rotateTransform = TransformGroupUtil.GetRotateTransform(group);
                    DoubleAnimation da = DoubleAnimationUtil.initDoubleAnimation(animation, 180 + currDControl.rotateAngle, 360 + currDControl.rotateAngle);

                    rotateTransform.BeginAnimation(RotateTransform.AngleProperty, da);


                    TranslateTransform translateTransform = TransformGroupUtil.GetTranslateTransform(group);
                    DoubleAnimation da2 = DoubleAnimationUtil.initDoubleAnimation(animation, currDControl.width * 1, 0);
                    translateTransform.BeginAnimation(TranslateTransform.XProperty, da2);


                    DoubleAnimationUtil.andBeginOpacityAnimation(currElement, animation, 0, currOpacity);
                }
                else if (animation.type == 1103)
                {
                    //从上旋转
                    currElement.RenderTransformOrigin = new Point(0.5, 0.5);

                    RotateTransform rotateTransform = TransformGroupUtil.GetRotateTransform(group);
                    DoubleAnimation da = DoubleAnimationUtil.initDoubleAnimation(animation, 180 + currDControl.rotateAngle, 360 + currDControl.rotateAngle);
                    rotateTransform.BeginAnimation(RotateTransform.AngleProperty, da);


                    TranslateTransform translateTransform = TransformGroupUtil.GetTranslateTransform(group);
                    DoubleAnimation da2 = DoubleAnimationUtil.initDoubleAnimation(animation, -currDControl.height * 1, 0);
                    translateTransform.BeginAnimation(TranslateTransform.YProperty, da2);


                    DoubleAnimationUtil.andBeginOpacityAnimation(currElement, animation, 0, currOpacity);
                }
                else if (animation.type == 1104)
                {
                    //从下旋转
                    currElement.RenderTransformOrigin = new Point(0.5, 0.5);

                    RotateTransform rotateTransform = TransformGroupUtil.GetRotateTransform(group);
                    DoubleAnimation da = DoubleAnimationUtil.initDoubleAnimation(animation, 180 + currDControl.rotateAngle, 360 + currDControl.rotateAngle);
                    rotateTransform.BeginAnimation(RotateTransform.AngleProperty, da);


                    TranslateTransform translateTransform = TransformGroupUtil.GetTranslateTransform(group);
                    DoubleAnimation da2 = DoubleAnimationUtil.initDoubleAnimation(animation, currDControl.height * 1, 0);
                    translateTransform.BeginAnimation(TranslateTransform.YProperty, da2);


                    DoubleAnimationUtil.andBeginOpacityAnimation(currElement, animation, 0, currOpacity);
                }
                else if (animation.type == 1201)
                {

                    //从左弹入
                    currElement.RenderTransformOrigin = new Point(0.5, 0.5);

                    TranslateTransform translateTransform12 = TransformGroupUtil.GetTranslateTransform(group);
                    IEasingFunction easingFunction = new BackEase() { EasingMode = EasingMode.EaseInOut };
                    DoubleAnimation da = DoubleAnimationUtil.initDoubleAnimation(animation, -currDControl.left - currDControl.width, 0, easingFunction);
                    translateTransform12.BeginAnimation(TranslateTransform.XProperty, da);


                    DoubleAnimationUtil.andBeginOpacityAnimation(currElement, animation, 0, currOpacity);
                }
                else if (animation.type == 1202)
                {
                    //从右弹入
                    currElement.RenderTransformOrigin = new Point(0.5, 0.5);

                    TranslateTransform translateTransform = TransformGroupUtil.GetTranslateTransform(group);
                    IEasingFunction easingFunction = new BackEase() { EasingMode = EasingMode.EaseInOut };
                    DoubleAnimation da = DoubleAnimationUtil.initDoubleAnimation(animation, cfg.screenWidth - currDControl.left, 0, easingFunction);
                    translateTransform.BeginAnimation(TranslateTransform.XProperty, da);

                    DoubleAnimationUtil.andBeginOpacityAnimation(currElement, animation, 0, currOpacity);
                }
                else if (animation.type == 1203)
                {
                    //从上弹入
                    currElement.RenderTransformOrigin = new Point(0.5, 0.5);

                    TranslateTransform translateTransform = TransformGroupUtil.GetTranslateTransform(group);
                    IEasingFunction easingFunction = new BackEase() { EasingMode = EasingMode.EaseInOut };
                    DoubleAnimation da = DoubleAnimationUtil.initDoubleAnimation(animation, -currDControl.top - currDControl.height, 0, easingFunction);

                    translateTransform.BeginAnimation(TranslateTransform.YProperty, da);


                    DoubleAnimationUtil.andBeginOpacityAnimation(currElement, animation, 0, currOpacity);
                }
                else if (animation.type == 1204)
                {
                    //从下弹入
                    currElement.RenderTransformOrigin = new Point(0.5, 0.5);

                    TranslateTransform translateTransform = TransformGroupUtil.GetTranslateTransform(group);
                    IEasingFunction easingFunction = new BackEase() { EasingMode = EasingMode.EaseInOut };
                    DoubleAnimation da = DoubleAnimationUtil.initDoubleAnimation(animation, cfg.screenHeight - currDControl.top, 0, easingFunction);

                    translateTransform.BeginAnimation(TranslateTransform.YProperty, da);

                    DoubleAnimationUtil.andBeginOpacityAnimation(currElement, animation, 0, currOpacity);
                }
                else if (animation.type == 1205)
                {
                    //中心弹入
                    currElement.RenderTransformOrigin = new Point(0.5, 0.5);

                    ScaleTransform scaleTransform = TransformGroupUtil.GetScaleTransform(group);
                    IEasingFunction easingFunction = new BackEase() { EasingMode = EasingMode.EaseInOut };
                    DoubleAnimation da = DoubleAnimationUtil.initDoubleAnimation(animation, 0.5, 1.0, easingFunction);
                    scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, da);
                    scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, da);


                    DoubleAnimationUtil.andBeginOpacityAnimation(currElement, animation, 0, currOpacity);
                }
                else if (animation.type == 1301)
                {
                    //从左斜入 
                    currElement.RenderTransformOrigin = new Point(0.5, 0.5);

                    SkewTransform skewTransform = TransformGroupUtil.GetSkewTransform(group);
                    IEasingFunction easingFunction = new BackEase() { EasingMode = EasingMode.EaseInOut };
                    DoubleAnimation da = DoubleAnimationUtil.initDoubleAnimation(animation, 140, 180, easingFunction);
                    skewTransform.BeginAnimation(SkewTransform.AngleXProperty, da);


                    TranslateTransform translateTransform = TransformGroupUtil.GetTranslateTransform(group);
                    DoubleAnimation da1 = DoubleAnimationUtil.initDoubleAnimation(animation, -currDControl.width * 1, 0, easingFunction);
                    translateTransform.BeginAnimation(TranslateTransform.XProperty, da1);


                    DoubleAnimationUtil.andBeginOpacityAnimation(currElement, animation, 0, currOpacity);
                }
                else if (animation.type == 1302)
                {
                    //从右斜入 
                    currElement.RenderTransformOrigin = new Point(0.5, 0.5);

                    SkewTransform skewTransform = TransformGroupUtil.GetSkewTransform(group);
                    IEasingFunction easingFunction = new BackEase() { EasingMode = EasingMode.EaseInOut };
                    DoubleAnimation da = DoubleAnimationUtil.initDoubleAnimation(animation, 40, 0, easingFunction);
                    skewTransform.BeginAnimation(SkewTransform.AngleXProperty, da);


                    TranslateTransform translateTransform = TransformGroupUtil.GetTranslateTransform(group);
                    DoubleAnimation da1 = DoubleAnimationUtil.initDoubleAnimation(animation, currDControl.width, 0, easingFunction);
                    translateTransform.BeginAnimation(TranslateTransform.XProperty, da1);


                    DoubleAnimationUtil.andBeginOpacityAnimation(currElement, animation, 0, currOpacity);
                }
                else if (animation.type == 1303)
                {

                    //从上斜入
                    currElement.RenderTransformOrigin = new Point(0.5, 0.5);

                    ScaleTransform scaleTransform = TransformGroupUtil.GetScaleTransform(group);
                    DoubleAnimation da = DoubleAnimationUtil.initDoubleAnimation(animation, 0.1, 1.0);
                    scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, da);



                    SkewTransform skewTransform = TransformGroupUtil.GetSkewTransform(group);
                    IEasingFunction easingFunction = new BackEase() { EasingMode = EasingMode.EaseInOut };
                    DoubleAnimation da1 = DoubleAnimationUtil.initDoubleAnimation(animation, 60, 0, easingFunction);
                    skewTransform.BeginAnimation(SkewTransform.AngleXProperty, da1);


                    TranslateTransform translateTransform = TransformGroupUtil.GetTranslateTransform(group);
                    DoubleAnimation da2 = DoubleAnimationUtil.initDoubleAnimation(animation, -currDControl.height, 0);
                    translateTransform.BeginAnimation(TranslateTransform.YProperty, da2);

                    DoubleAnimationUtil.andBeginOpacityAnimation(currElement, animation, 0, currOpacity);
                }
                else if (animation.type == 1304)
                {

                    //从下斜入
                    currElement.RenderTransformOrigin = new Point(0.5, 0.5);

                    ScaleTransform scaleTransform = TransformGroupUtil.GetScaleTransform(group);
                    DoubleAnimation da = DoubleAnimationUtil.initDoubleAnimation(animation, 0.1, 1.0);
                    scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, da);

                    SkewTransform skewTransform = TransformGroupUtil.GetSkewTransform(group);
                    IEasingFunction easingFunction = new BackEase() { EasingMode = EasingMode.EaseInOut };
                    DoubleAnimation da1 = DoubleAnimationUtil.initDoubleAnimation(animation, 60, 0, easingFunction);
                    skewTransform.BeginAnimation(SkewTransform.AngleXProperty, da1);


                    TranslateTransform translateTransform = TransformGroupUtil.GetTranslateTransform(group);
                    DoubleAnimation da2 = DoubleAnimationUtil.initDoubleAnimation(animation, currDControl.height, 0);
                    translateTransform.BeginAnimation(TranslateTransform.YProperty, da2);


                    DoubleAnimationUtil.andBeginOpacityAnimation(currElement, animation, 0, currOpacity);
                }
                else if (animation.type == 1401)
                {
                    //从左绕入

                    currElement.RenderTransformOrigin = new Point(0, 0);
                    RotateTransform rotateTransform = TransformGroupUtil.GetRotateTransform(group);
                    DoubleAnimation da = DoubleAnimationUtil.initDoubleAnimation(animation, 360 + currDControl.rotateAngle, 0 + currDControl.rotateAngle);
                    rotateTransform.BeginAnimation(RotateTransform.AngleProperty, da);


                    ScaleTransform scaleTransform = TransformGroupUtil.GetScaleTransform(group);
                    DoubleAnimation da1 = DoubleAnimationUtil.initDoubleAnimation(animation, 0, 1.0);
                    scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, da1);
                    scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, da1);

                    DoubleAnimationUtil.andBeginOpacityAnimation(currElement, animation, 0, currOpacity);
                }
                else if (animation.type == 1402)
                {
                    //从右绕入 

                    currElement.RenderTransformOrigin = new Point(1, 1);
                    RotateTransform rotateTransform = TransformGroupUtil.GetRotateTransform(group);
                    DoubleAnimation da = DoubleAnimationUtil.initDoubleAnimation(animation, 360 + currDControl.rotateAngle, 0 + currDControl.rotateAngle);
                    rotateTransform.BeginAnimation(RotateTransform.AngleProperty, da);


                    ScaleTransform scaleTransform = TransformGroupUtil.GetScaleTransform(group);
                    DoubleAnimation da1 = DoubleAnimationUtil.initDoubleAnimation(animation, 0, 1.0);
                    scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, da1);
                    scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, da1);


                    DoubleAnimationUtil.andBeginOpacityAnimation(currElement, animation, 0, currOpacity);
                }
                else if (animation.type == 1403)
                {
                    //从上绕入 


                    currElement.RenderTransformOrigin = new Point(1, 0);
                    RotateTransform rotateTransform = TransformGroupUtil.GetRotateTransform(group);
                    DoubleAnimation da = DoubleAnimationUtil.initDoubleAnimation(animation, 360 + currDControl.rotateAngle, 0 + currDControl.rotateAngle);
                    rotateTransform.BeginAnimation(RotateTransform.AngleProperty, da);

                    ScaleTransform scaleTransform = TransformGroupUtil.GetScaleTransform(group);
                    DoubleAnimation da1 = DoubleAnimationUtil.initDoubleAnimation(animation, 0, 1.0);
                    scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, da1);
                    scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, da1);

                    DoubleAnimationUtil.andBeginOpacityAnimation(currElement, animation, 0, currOpacity);
                }
                else if (animation.type == 1404)
                {


                    //从下绕入 

                    currElement.RenderTransformOrigin = new Point(0, 1);
                    RotateTransform rotateTransform = TransformGroupUtil.GetRotateTransform(group);
                    DoubleAnimation da = DoubleAnimationUtil.initDoubleAnimation(animation, 0 + currDControl.rotateAngle, 360 + currDControl.rotateAngle);
                    rotateTransform.BeginAnimation(RotateTransform.AngleProperty, da);


                    ScaleTransform scaleTransform = TransformGroupUtil.GetScaleTransform(group);
                    DoubleAnimation da1 = DoubleAnimationUtil.initDoubleAnimation(animation, 0, 1.0);
                    scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, da1);
                    scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, da1);


                    DoubleAnimationUtil.andBeginOpacityAnimation(currElement, animation, 0, currOpacity);
                }
                else if (animation.type == 1501)
                {
                    //翻开
                    currElement.RenderTransformOrigin = new Point(0.5, 0.5);

                    SkewTransform skewTransform = TransformGroupUtil.GetSkewTransform(group);
                    IEasingFunction easingFunction = new BackEase() { EasingMode = EasingMode.EaseInOut, Amplitude = 0.1 };
                    DoubleAnimation da = DoubleAnimationUtil.initDoubleAnimation(animation, 10, 0, easingFunction);
                    skewTransform.BeginAnimation(SkewTransform.AngleXProperty, da);


                    ScaleTransform scaleTransform = TransformGroupUtil.GetScaleTransform(group);
                    IEasingFunction easingFunction1 = new BackEase() { EasingMode = EasingMode.EaseInOut };
                    DoubleAnimation da1 = DoubleAnimationUtil.initDoubleAnimation(animation, 0.4, 1.0, easingFunction1);
                    scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, da1);


                    IEasingFunction easingFunction2 = new CubicEase() { EasingMode = EasingMode.EaseIn };
                    DoubleAnimationUtil.andBeginOpacityAnimation(currElement, animation, 0, currOpacity, easingFunction2);
                }
                else if (animation.type == 1502)
                {
                    //旋转
                    currElement.RenderTransformOrigin = new Point(0.5, 0.5);

                    RotateTransform rotateTransform = TransformGroupUtil.GetRotateTransform(group);
                    DoubleAnimation da = DoubleAnimationUtil.initDoubleAnimation(animation, 0 + currDControl.rotateAngle, 360 + currDControl.rotateAngle);
                    rotateTransform.BeginAnimation(RotateTransform.AngleProperty, da);

                    IEasingFunction easingFunction2 = new CubicEase() { EasingMode = EasingMode.EaseIn };
                    DoubleAnimationUtil.andBeginOpacityAnimation(currElement, animation, 0, currOpacity, easingFunction2);
                }
                else if (animation.type == 1601)
                {
                    //光晕 来回缩放 
                    currElement.RenderTransformOrigin = new Point(0.5, 0.5);

                    double halfSeconds = animation.durationSeconds / 2.0;
                    DoubleAnimationUsingKeyFrames da = new DoubleAnimationUsingKeyFrames();
                    if (animation.playTimes <= 0)
                    {
                        da.RepeatBehavior = RepeatBehavior.Forever;
                    }
                    else
                    {
                        da.RepeatBehavior = new RepeatBehavior(animation.playTimes);
                    }
                    ScaleTransform scaleTransform = TransformGroupUtil.GetScaleTransform(group);
                    var keyFrames = da.KeyFrames;
                    IEasingFunction easingFunction = new CubicEase() { EasingMode = EasingMode.EaseIn };
                    keyFrames.Add(new LinearDoubleKeyFrame(1.0, TimeSpan.FromMilliseconds(0)));
                    keyFrames.Add(new LinearDoubleKeyFrame(0.78, TimeSpan.FromMilliseconds(halfSeconds)));
                    keyFrames.Add(new LinearDoubleKeyFrame(1.0, TimeSpan.FromMilliseconds(animation.durationSeconds)));
                    scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, da);
                    scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, da);
                }
            }

        }



        /*
         * 加载全屏视频
         * 
         * 由中心放大
         */
        public static void loadFullVideo(FrameworkElement currElement, double toTranslateTransformX, double toTranslateTransformY)
        {
            //放大
            TransformGroup group = (TransformGroup)currElement.RenderTransform;
            ScaleTransform scaleTransform = TransformGroupUtil.GetScaleTransform(group);
            DoubleAnimation da = new DoubleAnimation(scaleTransform.ScaleX, 1.0, new Duration(TimeSpan.FromSeconds(0.3)));
            scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, da);
            DoubleAnimation da2 = new DoubleAnimation(scaleTransform.ScaleY, 1.0, new Duration(TimeSpan.FromSeconds(0.3)));
            scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, da2);

            // 平移  
            TranslateTransform translateTransform = TransformGroupUtil.GetTranslateTransform(group);
            DoubleAnimation da3 = new DoubleAnimation(0, toTranslateTransformX, new Duration(TimeSpan.FromMilliseconds(300)));
            translateTransform.BeginAnimation(TranslateTransform.XProperty, da3);
            DoubleAnimation da4 = new DoubleAnimation(0, toTranslateTransformY, new Duration(TimeSpan.FromMilliseconds(300)));
            translateTransform.BeginAnimation(TranslateTransform.YProperty, da4);
        }

    }
}
