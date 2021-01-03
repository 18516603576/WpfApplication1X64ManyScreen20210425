using Bll;
using Common;
using Common.Data;
using Common.util;
using Model;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using WpfApplication1.manage;

namespace WpfApplication1.ManageWin
{
    /// <summary>
    /// EditImageAnimationWindow.xaml 的交互逻辑
    /// </summary>
    public partial class EditImageAnimationWindow : Window
    {

        private readonly FrameworkElement currElement;
        private readonly DControl currDControl;
        private readonly DControlAnimationBll dControlAnimationBll = new DControlAnimationBll();
        private Int32 animationNum = 0;
        private readonly SolidColorBrush selectedBorderColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF109CEC"));

        public EditImageAnimationWindow(Editing editing, FrameworkElement rightClickEle)
        {
            InitializeComponent();
            initLocation();
            currElement = rightClickEle;
            currDControl = (DControl)rightClickEle.Tag;
            loadSelectAnimation();
            initEditingListWrap(currDControl.id);

        }

        private void initLocation()
        {
            double ScreenWidth = SystemParameters.PrimaryScreenWidth;
            double ScreenHeight = SystemParameters.PrimaryScreenHeight;
            double x = ScreenWidth - Width - 5;
            double y = (ScreenHeight - Height) / 2;

            Left = x;
            Top = y;
        }

        /*
         * 初始化动画编辑列表
         */
        private void initEditingListWrap(Int32 dControlId)
        {
            List<DControlAnimation> list = dControlAnimationBll.getByDControlId(dControlId);
            foreach (DControlAnimation animation in list)
            {
                Canvas canvas = initOneAnimationEditingCanvas(animation);
                editingListWrap.Children.Add(canvas);
            }
        }


        /*
         * 点击添加动画
         */
        private void ShowAnimationWin(object sender, RoutedEventArgs e)
        {
            AnimationSelectorGrid.Visibility = Visibility.Visible;
        }

        /*
         * 加载所有可选择的动画
         */
        private void loadSelectAnimation()
        {
            initBackBtn();
            foreach (DControlAnimation animation in Params.animationInList)
            {
                Button btn = initOneSelectAnimation(animation);
                AnimationSelectorWrap.Children.Add(btn);
            }
        }

        private void initBackBtn()
        {

            backBtn.Background = new ImageBrush
            {
                ImageSource = FileUtil.readImage(AppDomain.CurrentDomain.BaseDirectory + "/myfile/sysimg/animation/ico_animation_back.png")
                ,
                Stretch = Stretch.Uniform
            };
            backBtn.Click += backBtnClick;
        }
        /*
         * 返回
         */
        private void backBtnClick(object sender, RoutedEventArgs e)
        {
            AnimationSelectorGrid.Visibility = Visibility.Collapsed;
        }
        /*
         * 加载一个
         */
        private Button initOneSelectAnimation(DControlAnimation dControlAnimation)
        {
            Button btn = new Button();
            btn.Name = "animationBtn";
            btn.Width = 80;
            btn.Height = 80;
            btn.BorderBrush = Brushes.LightGray;
            btn.HorizontalContentAlignment = HorizontalAlignment.Center;
            btn.VerticalContentAlignment = VerticalAlignment.Center;
            btn.Margin = new Thickness(5);
            btn.Tag = dControlAnimation;
            btn.Background = new ImageBrush
            {
                ImageSource = FileUtil.readImage(AppDomain.CurrentDomain.BaseDirectory + "/myfile/sysimg/animation/ico_animation_bg" + dControlAnimation.type + ".png")
            };
            btn.MouseEnter += previewAnimation_MouseEnter;
            //oneCanvas.MouseLeave += imageCanvasMouseLeave;
            btn.Click += selectedClick;

            return btn;
        }

        /*
         * 选中动画加入到数据
         */
        private void selectedClick(object sender, RoutedEventArgs e)
        {
            Button animationBtn = (Button)sender;
            DControlAnimation dControlAnimation = (DControlAnimation)animationBtn.Tag;
            //1.加入到数据
            dControlAnimation.dControlId = currDControl.id;
            dControlAnimation = dControlAnimationBll.insert(dControlAnimation);

            //2.隐藏选择动画面板 
            AnimationSelectorGrid.Visibility = Visibility.Collapsed;

            //3.添加到页面 
            Canvas canvas = initOneAnimationEditingCanvas(dControlAnimation);
            editingListWrap.Children.Add(canvas);
        }

        /*
         * 初始化一个动画编辑框
         */
        private Canvas initOneAnimationEditingCanvas(DControlAnimation dControlAnimation)
        {

            SolidColorBrush wColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF666666"));

            Canvas canvas = new Canvas();
            canvas.Width = 300;
            canvas.Height = 130;
            canvas.Margin = new Thickness(0, 5, 0, 5);
            canvas.Background = Brushes.White;
            canvas.Tag = dControlAnimation;

            //1.标题行
            Canvas titleRow = new Canvas();
            titleRow.Width = 280;
            titleRow.Height = 30;
            titleRow.SetValue(Canvas.LeftProperty, 10.0);
            titleRow.SetValue(Canvas.TopProperty, 10.0);
            //titleRow.Background = Brushes.LightCyan;
            canvas.Children.Add(titleRow);

            animationNum = animationNum + 1;
            Label label1 = new Label();
            label1.Content = "动画" + animationNum;
            label1.Height = 24;
            label1.Padding = new Thickness(3.0);
            label1.FontWeight = FontWeights.Bold;
            titleRow.Children.Add(label1);

            Label changeBtn = new Label();
            changeBtn.Width = 70;
            changeBtn.Height = 24;
            changeBtn.Padding = new Thickness(3.0);
            changeBtn.SetValue(Canvas.LeftProperty, 50.0);
            changeBtn.Content = dControlAnimation.name + ">";
            changeBtn.FontWeight = FontWeights.Bold;
            titleRow.Children.Add(changeBtn);

            Button playBtn = new Button();
            playBtn.Width = 20;
            playBtn.Height = 20;
            playBtn.Padding = new Thickness(5.0);
            playBtn.BorderThickness = new Thickness(0);
            playBtn.SetValue(Canvas.RightProperty, 60.0);
            playBtn.ToolTip = "播放";
            playBtn.Background = new ImageBrush
            {
                ImageSource = FileUtil.readImage(AppDomain.CurrentDomain.BaseDirectory + "/myfile/sysimg/animation/ico_animation_play.png")
               ,
                Stretch = Stretch.Uniform
            };
            playBtn.Click += playBtnClick;
            titleRow.Children.Add(playBtn);


            Button removeBtn = new Button();
            removeBtn.Width = 20;
            removeBtn.Height = 20;
            removeBtn.Padding = new Thickness(5.0);
            removeBtn.BorderThickness = new Thickness(0);
            removeBtn.SetValue(Canvas.RightProperty, 25.0);
            removeBtn.ToolTip = "删除";
            removeBtn.Background = new ImageBrush
            {
                ImageSource = FileUtil.readImage(AppDomain.CurrentDomain.BaseDirectory + "/myfile/sysimg/animation/ico_animation_delete.png")
               ,
                Stretch = Stretch.Uniform
            };
            removeBtn.Click += removeBtnClick;
            titleRow.Children.Add(removeBtn);


            //2.表单行
            Canvas formRow = new Canvas();
            formRow.Width = 280;
            formRow.Height = 90;
            formRow.SetValue(Canvas.LeftProperty, 10.0);
            formRow.SetValue(Canvas.TopProperty, 40.0);
            canvas.Children.Add(formRow);

            Label startLabel = new Label();
            startLabel.Content = "开始时间";
            startLabel.Width = 60;
            startLabel.SetValue(Canvas.LeftProperty, 0.0);
            startLabel.Foreground = wColor;
            formRow.Children.Add(startLabel);

            TextBox startTextBox = new TextBox();
            startTextBox.Name = "delaySeconds";
            startTextBox.Width = 60;
            startTextBox.Height = 24;
            startTextBox.Text = (dControlAnimation.delaySeconds / 1000.0).ToString();
            startTextBox.VerticalContentAlignment = VerticalAlignment.Center;
            startTextBox.SetValue(Canvas.LeftProperty, 60.0);
            startTextBox.LostFocus += startTextBox_LostFocus;
            formRow.Children.Add(startTextBox);

            Label durationLabel = new Label();
            durationLabel.Content = "持续时间";
            durationLabel.Width = 60;
            durationLabel.SetValue(Canvas.LeftProperty, 130.0);
            durationLabel.Foreground = wColor;
            formRow.Children.Add(durationLabel);

            TextBox durationTextBox = new TextBox();
            durationTextBox.Name = "durationSeconds";
            durationTextBox.Width = 60;
            durationTextBox.Height = 24;
            durationTextBox.Text = (dControlAnimation.durationSeconds / 1000.0).ToString();
            durationTextBox.VerticalContentAlignment = VerticalAlignment.Center;
            durationTextBox.SetValue(Canvas.LeftProperty, 190.0);
            durationTextBox.LostFocus += durationTextBox_LostFocus;
            formRow.Children.Add(durationTextBox);

            Label playTimesLabel = new Label();
            playTimesLabel.Content = "播放次数";
            playTimesLabel.Width = 60;
            playTimesLabel.SetValue(Canvas.LeftProperty, 0.0);
            playTimesLabel.SetValue(Canvas.TopProperty, 30.0);
            playTimesLabel.Foreground = wColor;
            formRow.Children.Add(playTimesLabel);

            TextBox playTimesTextBox = new TextBox();
            playTimesTextBox.Name = "playTimes";
            playTimesTextBox.Width = 60;
            playTimesTextBox.Height = 24;
            playTimesTextBox.Text = dControlAnimation.playTimes.ToString();
            playTimesTextBox.VerticalContentAlignment = VerticalAlignment.Center;
            playTimesTextBox.SetValue(Canvas.LeftProperty, 60.0);
            playTimesTextBox.SetValue(Canvas.TopProperty, 30.0);
            playTimesTextBox.LostFocus += playTimesTextBox_LostFocus;
            formRow.Children.Add(playTimesTextBox);


            Label playTimesTipsLabel = new Label();
            playTimesTipsLabel.Content = "  0 表示循环播放";
            playTimesTipsLabel.Width = 120;
            playTimesTipsLabel.SetValue(Canvas.LeftProperty, 130.0);
            playTimesTipsLabel.SetValue(Canvas.TopProperty, 30.0);
            playTimesTipsLabel.Foreground = wColor;
            formRow.Children.Add(playTimesTipsLabel);



            CheckBox isSameSpeedCheckBox = new CheckBox();
            isSameSpeedCheckBox.Name = "isSameSpeed";
            isSameSpeedCheckBox.Width = 110;
            isSameSpeedCheckBox.Height = 24;
            isSameSpeedCheckBox.Content = "匀速播放";
            isSameSpeedCheckBox.IsChecked = dControlAnimation.isSameSpeed;
            isSameSpeedCheckBox.VerticalContentAlignment = VerticalAlignment.Center;
            isSameSpeedCheckBox.SetValue(Canvas.LeftProperty, 10.0);
            isSameSpeedCheckBox.SetValue(Canvas.TopProperty, 60.0);
            isSameSpeedCheckBox.Foreground = wColor;
            isSameSpeedCheckBox.Click += isSameSpeedCheckBox_Click;
            formRow.Children.Add(isSameSpeedCheckBox);


            CheckBox isSameOpacityCheckBox = new CheckBox();
            isSameOpacityCheckBox.Name = "isSameOpacity";
            isSameOpacityCheckBox.Width = 110;
            isSameOpacityCheckBox.Height = 24;
            isSameOpacityCheckBox.Content = "恒透明度";
            isSameOpacityCheckBox.IsChecked = dControlAnimation.isSameOpacity;
            isSameOpacityCheckBox.VerticalContentAlignment = VerticalAlignment.Center;
            isSameOpacityCheckBox.SetValue(Canvas.LeftProperty, 140.0);
            isSameOpacityCheckBox.SetValue(Canvas.TopProperty, 60.0);
            isSameOpacityCheckBox.Foreground = wColor;
            isSameOpacityCheckBox.Click += isSameOpacityCheckBox_Click;
            formRow.Children.Add(isSameOpacityCheckBox);




            return canvas;
        }
        /*
         * 持续时间输入框丢失焦点 - 更新数据
         */
        private void durationTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textbox = (TextBox)sender;
            Canvas formRow = (Canvas)textbox.Parent;
            Canvas canvas = (Canvas)formRow.Parent;
            DControlAnimation animation = (DControlAnimation)canvas.Tag;
            //1.从数据库删除
            string text = textbox.Text;
            if (string.IsNullOrWhiteSpace(text))
            {
                textbox.BorderBrush = Brushes.Red;
                return;
            }
            else if (!DataUtil.IsDouble(text))
            {
                textbox.BorderBrush = Brushes.Red;
                return;
            }
            textbox.BorderBrush = Brushes.Gray;


            double tmp = DataUtil.ToDouble(text);
            Int32 durationSeconds = (int)Math.Floor(tmp * 1000);
            animation.durationSeconds = durationSeconds;
            dControlAnimationBll.update(animation);
        }

        /*
         * 开始时间丢失焦点 - 更新数据
         */
        private void startTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textbox = (TextBox)sender;
            Canvas formRow = (Canvas)textbox.Parent;
            Canvas canvas = (Canvas)formRow.Parent;
            DControlAnimation animation = (DControlAnimation)canvas.Tag;
            //1.从数据库删除
            string text = textbox.Text;
            if (string.IsNullOrWhiteSpace(text))
            {
                textbox.BorderBrush = Brushes.Red;
                return;
            }
            else if (!DataUtil.IsDouble(text))
            {
                textbox.BorderBrush = Brushes.Red;
                return;
            }

            textbox.BorderBrush = Brushes.Gray;
            double tmp = DataUtil.ToDouble(text);
            Int32 delaySeconds = (int)Math.Floor(tmp * 1000);
            animation.delaySeconds = delaySeconds;
            dControlAnimationBll.update(animation);
        }

        /*
         * 播放次数输入框丢失焦点 - 更新数据
         */
        private void playTimesTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textbox = (TextBox)sender;
            Canvas formRow = (Canvas)textbox.Parent;
            Canvas canvas = (Canvas)formRow.Parent;
            DControlAnimation animation = (DControlAnimation)canvas.Tag;
            //1.从数据库删除
            string text = textbox.Text;
            if (string.IsNullOrWhiteSpace(text))
            {
                textbox.BorderBrush = Brushes.Red;
                return;
            }
            else if (!DataUtil.isInt(text))
            {
                textbox.BorderBrush = Brushes.Red;
                return;
            }
            textbox.BorderBrush = Brushes.Gray;

            Int32 playTimes = DataUtil.ToInt(text);
            animation.playTimes = playTimes;
            dControlAnimationBll.update(animation);
        }

        /*
        * 匀速播放复选框丢失焦点 - 更新数据
        */
        private void isSameSpeedCheckBox_Click(object sender, RoutedEventArgs e)
        {
            CheckBox checkbox = (CheckBox)sender;
            Canvas formRow = (Canvas)checkbox.Parent;
            Canvas canvas = (Canvas)formRow.Parent;
            DControlAnimation animation = (DControlAnimation)canvas.Tag;
            //1.从数据库删除
            Boolean isSameSpeed = (Boolean)checkbox.IsChecked;
            checkbox.BorderBrush = Brushes.Gray;


            animation.isSameSpeed = isSameSpeed;
            dControlAnimationBll.update(animation);
        }

        /*
         * 恒透明度复选框丢失焦点 - 更新数据
        */
        private void isSameOpacityCheckBox_Click(object sender, RoutedEventArgs e)
        {
            CheckBox checkbox = (CheckBox)sender;
            Canvas formRow = (Canvas)checkbox.Parent;
            Canvas canvas = (Canvas)formRow.Parent;
            DControlAnimation animation = (DControlAnimation)canvas.Tag;
            //1.从数据库删除
            Boolean isSameOpacity = (Boolean)checkbox.IsChecked;
            checkbox.BorderBrush = Brushes.Gray;


            animation.isSameOpacity = isSameOpacity;
            dControlAnimationBll.update(animation);
        }

        /*
         * 删除动画
         */
        private void removeBtnClick(object sender, RoutedEventArgs e)
        {
            FrameworkElement removeBtn = (FrameworkElement)sender;
            Canvas titleRow = (Canvas)removeBtn.Parent;
            Canvas canvas = (Canvas)titleRow.Parent;
            DControlAnimation animation = (DControlAnimation)canvas.Tag;
            //1.从数据库删除
            dControlAnimationBll.delete(animation.id);

            //2.从页面移除控件
            editingListWrap.Children.Remove(canvas);
        }

        /*
         * 点击play按钮 - 预览动画
         */
        private void playBtnClick(object sender, RoutedEventArgs e)
        {
            FrameworkElement playBtn = (FrameworkElement)sender;
            Canvas titleRow = (Canvas)playBtn.Parent;
            Canvas canvas = (Canvas)titleRow.Parent;
            DControlAnimation animation = (DControlAnimation)canvas.Tag;
            previewAnimation(animation);
        }

        /*
         * 鼠标经过 - 预览动画
         */
        private void previewAnimation_MouseEnter(object sender, MouseEventArgs e)
        {
            Button animationBtn = (Button)sender;
            DControlAnimation animation = (DControlAnimation)animationBtn.Tag;

            //1.选中当前
            foreach (UIElement element in AnimationSelectorWrap.Children)
            {
                Button btn = (Button)element;
                btn.BorderBrush = Brushes.LightGray;
            }
            animationBtn.BorderBrush = selectedBorderColor;

            //2.预览动画
            previewAnimation(animation);
        }





        /*
         * 预览动画
         */
        private void previewAnimation(DControlAnimation animation)
        {
            double currOpacity = currDControl.opacity / 100.0;
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
                TransformGroup group = (TransformGroup)currElement.RenderTransform;
                group.Children.Clear();

                TranslateTransform translateTransform = TransformGroupUtil.GetTranslateTransform(group);
                DoubleAnimation da = DoubleAnimationUtil.initDoubleAnimation(animation, -currDControl.left - currDControl.width, 0);
                translateTransform.BeginAnimation(TranslateTransform.XProperty, da);

                DoubleAnimationUtil.andBeginOpacityAnimation(currElement, animation, 0, currOpacity);
            }
            else if (animation.type == 1003)
            {
                //从右移入
                currElement.RenderTransformOrigin = new Point(0.5, 0.5);
                TransformGroup group = (TransformGroup)currElement.RenderTransform;
                group.Children.Clear();
                TranslateTransform translateTransform = TransformGroupUtil.GetTranslateTransform(group);
                DoubleAnimation da = DoubleAnimationUtil.initDoubleAnimation(animation, App.localStorage.cfg.screenWidth - currDControl.left, 0);
                translateTransform.BeginAnimation(TranslateTransform.XProperty, da);

                DoubleAnimationUtil.andBeginOpacityAnimation(currElement, animation, 0, currOpacity);
            }
            else if (animation.type == 1004)
            {
                //从上移入
                currElement.RenderTransformOrigin = new Point(0.5, 0.5);
                TransformGroup group = (TransformGroup)currElement.RenderTransform;
                group.Children.Clear();
                TranslateTransform translateTransform = TransformGroupUtil.GetTranslateTransform(group);
                DoubleAnimation da = DoubleAnimationUtil.initDoubleAnimation(animation, -currDControl.top - currDControl.height, 0);

                translateTransform.BeginAnimation(TranslateTransform.YProperty, da);

                DoubleAnimationUtil.andBeginOpacityAnimation(currElement, animation, 0, currOpacity);
            }
            else if (animation.type == 1005)
            {
                //从下移入
                currElement.RenderTransformOrigin = new Point(0.5, 0.5);
                TransformGroup group = (TransformGroup)currElement.RenderTransform;
                group.Children.Clear();
                TranslateTransform translateTransform = TransformGroupUtil.GetTranslateTransform(group);
                DoubleAnimation da = DoubleAnimationUtil.initDoubleAnimation(animation, App.localStorage.cfg.screenHeight - currDControl.top, 0);

                translateTransform.BeginAnimation(TranslateTransform.YProperty, da);

                DoubleAnimationUtil.andBeginOpacityAnimation(currElement, animation, 0, currOpacity);
            }

            else if (animation.type == 1006)
            {
                //放大
                currElement.RenderTransformOrigin = new Point(0.5, 0.5);
                TransformGroup group = (TransformGroup)currElement.RenderTransform;
                group.Children.Clear();
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
                TransformGroup group = (TransformGroup)currElement.RenderTransform;
                group.Children.Clear();
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
                TransformGroup group = currElement.RenderTransform as TransformGroup;
                group.Children.Clear();
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
                TransformGroup group = currElement.RenderTransform as TransformGroup;
                group.Children.Clear();
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
                TransformGroup group = currElement.RenderTransform as TransformGroup;
                group.Children.Clear();
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
                TransformGroup group = currElement.RenderTransform as TransformGroup;
                group.Children.Clear();
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
                TransformGroup group = (TransformGroup)currElement.RenderTransform;
                group.Children.Clear();

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
                TransformGroup group = (TransformGroup)currElement.RenderTransform;
                group.Children.Clear();
                TranslateTransform translateTransform = TransformGroupUtil.GetTranslateTransform(group);
                IEasingFunction easingFunction = new BackEase() { EasingMode = EasingMode.EaseInOut };
                DoubleAnimation da = DoubleAnimationUtil.initDoubleAnimation(animation, App.localStorage.cfg.screenWidth - currDControl.left, 0, easingFunction);
                translateTransform.BeginAnimation(TranslateTransform.XProperty, da);

                DoubleAnimationUtil.andBeginOpacityAnimation(currElement, animation, 0, currOpacity);
            }
            else if (animation.type == 1203)
            {
                //从上弹入
                currElement.RenderTransformOrigin = new Point(0.5, 0.5);
                TransformGroup group = (TransformGroup)currElement.RenderTransform;
                group.Children.Clear();
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
                TransformGroup group = (TransformGroup)currElement.RenderTransform;
                group.Children.Clear();
                TranslateTransform translateTransform = TransformGroupUtil.GetTranslateTransform(group);
                IEasingFunction easingFunction = new BackEase() { EasingMode = EasingMode.EaseInOut };
                DoubleAnimation da = DoubleAnimationUtil.initDoubleAnimation(animation, App.localStorage.cfg.screenHeight - currDControl.top, 0, easingFunction);

                translateTransform.BeginAnimation(TranslateTransform.YProperty, da);

                DoubleAnimationUtil.andBeginOpacityAnimation(currElement, animation, 0, currOpacity);
            }
            else if (animation.type == 1205)
            {
                //中心弹入
                currElement.RenderTransformOrigin = new Point(0.5, 0.5);
                TransformGroup group = (TransformGroup)currElement.RenderTransform;
                group.Children.Clear();
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
                TransformGroup group = currElement.RenderTransform as TransformGroup;
                group.Children.Clear();
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
                TransformGroup group = currElement.RenderTransform as TransformGroup;
                group.Children.Clear();

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
                TransformGroup group = (TransformGroup)currElement.RenderTransform;
                group.Children.Clear();
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
                TransformGroup group = (TransformGroup)currElement.RenderTransform;
                group.Children.Clear();
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
                TransformGroup group = currElement.RenderTransform as TransformGroup;
                group.Children.Clear();
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
                TransformGroup group = currElement.RenderTransform as TransformGroup;
                group.Children.Clear();
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

                TransformGroup group = currElement.RenderTransform as TransformGroup;
                group.Children.Clear();
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
                TransformGroup group = currElement.RenderTransform as TransformGroup;
                group.Children.Clear();
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
                TransformGroup group = currElement.RenderTransform as TransformGroup;
                group.Children.Clear();
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
                TransformGroup group = currElement.RenderTransform as TransformGroup;
                group.Children.Clear();
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
                TransformGroup group = currElement.RenderTransform as TransformGroup;
                group.Children.Clear();

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
            //else if (animation.type == 1502)
            //{
            //    //摇摆
            //    currElement.RenderTransformOrigin = new Point(0.5, 0);
            //    TransformGroup group = currElement.RenderTransform as TransformGroup;
            //    group.Children.Clear();
            //    //RotateTransform rotateTransform = TransformGroupUtil.GetRotateTransform(group);
            //    //DoubleAnimation da = new DoubleAnimation(15, -15, new Duration(TimeSpan.FromSeconds(animation.durationSeconds)));
            //    //da.BeginTime = TimeSpan.FromSeconds(animation.delaySeconds);
            //    //da.EasingFunction = new SineEase() { EasingMode = EasingMode.EaseInOut  }; 
            //    //rotateTransform.BeginAnimation(RotateTransform.AngleProperty, da);

            //    Storyboard storyboard = new Storyboard();
            //    #region 创建第一个动画
            //    DoubleAnimation doubleAnimation = new DoubleAnimation(15, -15, new Duration(TimeSpan.FromSeconds(animation.durationSeconds)));
            //    doubleAnimation.EasingFunction = new SineEase() { EasingMode = EasingMode.EaseIn };
            //    RotateTransform rotateTransform = TransformGroupUtil.GetRotateTransform(group);
            //    rotateTransform.Angle = 15;
            //    Storyboard.SetTarget(doubleAnimation, currElement);
            //    Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath("RenderTransform.(TransformGroup.Children)[0].(RotateTransform.Angle)"));
            //    storyboard.Children.Add(doubleAnimation);

            //    // 创建第2个动画
            //    DoubleAnimation doubleAnimation2 = new DoubleAnimation(-15, 15, new Duration(TimeSpan.FromSeconds(animation.durationSeconds)));
            //    doubleAnimation2.EasingFunction = new SineEase() { EasingMode = EasingMode.EaseOut };
            //    doubleAnimation2.BeginTime = TimeSpan.FromSeconds(2.0);
            //    RotateTransform rotateTransform2 = TransformGroupUtil.GetRotateTransform(group);
            //    rotateTransform2.Angle = 15;

            //    Storyboard.SetTarget(doubleAnimation2, currElement);
            //    Storyboard.SetTargetProperty(doubleAnimation2, new PropertyPath("RenderTransform.(TransformGroup.Children)[0].(RotateTransform.Angle)"));
            //    storyboard.Children.Add(doubleAnimation2);

            //    #endregion
            //    storyboard.Begin();
            //}

        }

        /*
        * 预览控件上所有动画
        */
        private void previewAllAnimation()
        {
            double currOpacity = currDControl.opacity / 100.0;
            List<DControlAnimation> list = dControlAnimationBll.getByDControlId(currDControl.id);
            TransformGroup group = (TransformGroup)currElement.RenderTransform;
            group.Children.Clear();
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
                    DoubleAnimation da = DoubleAnimationUtil.initDoubleAnimation(animation, App.localStorage.cfg.screenWidth - currDControl.left, 0);
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
                    DoubleAnimation da = DoubleAnimationUtil.initDoubleAnimation(animation, App.localStorage.cfg.screenHeight - currDControl.top, 0);

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
                    DoubleAnimation da = DoubleAnimationUtil.initDoubleAnimation(animation, App.localStorage.cfg.screenWidth - currDControl.left, 0, easingFunction);
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
                    DoubleAnimation da = DoubleAnimationUtil.initDoubleAnimation(animation, App.localStorage.cfg.screenHeight - currDControl.top, 0, easingFunction);

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
         * 预览所有动画
         */
        private void PreviewAllAnimationClick(object sender, RoutedEventArgs e)
        {
            previewAllAnimation();
        }
    }
}
