using Common.util;
using Model;
using Model.dto;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Common.control
{
    /// <summary>
    /// TurnPicture.xaml 的交互逻辑
    /// </summary>
    public partial class TurnPicture : UserControl
    {
        //定时器 
        DispatcherTimer turnImage_Timer = null;
       // private delegate void TimerDispatcherDelegate();
         

        //mouseDown时，flag=true；mouseup时，flag=false
        private Boolean isMouseDown = false;
        //mouseDown时的坐标X
        private double startX = 0;
        //ListBox卷起的宽度
        private double offsetLeft = 0;
        //本次鼠标移动的距离
        private double moveX = 0;
        //鼠标按下时，距离控件边缘的位置
        private double controlX = 0;
        //当前切换到第几个图片了
        private int turnImageWhich = 0;
        //如果是单击，则isMoving=false 
        private bool isMoving = false;

        private DControl currDControl;
        //相册列表
        private List<TurnPictureImagesDto> list = null;

        private BitmapImage ico_arrowLeft = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "/myfile/sysimg/TurnPicture/ico_turnPicture_arrow_left.png"));
        private BitmapImage ico_arrowLeft_active = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "/myfile/sysimg/TurnPicture/ico_turnPicture_arrow_left_active.png"));
        private BitmapImage ico_arrowRight = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "/myfile/sysimg/TurnPicture/ico_turnPicture_arrow_right.png"));
        private BitmapImage ico_arrowRight_active = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "/myfile/sysimg/TurnPicture/ico_turnPicture_arrow_right_active.png"));

        private readonly Storyboard storyboard = new Storyboard();

        //三、声明委托 全屏则隐藏音乐播放按钮
        public delegate void TurnPictureItemHandler(TurnPictureImagesDto dto, double width, double height, System.Windows.Point point, int pageId);
        public event TurnPictureItemHandler TurnPictureItemEvent;
        public void RaiseEvent(TurnPictureImagesDto dto, double width, double height, System.Windows.Point point)
        {
            if (TurnPictureItemEvent != null)
            {
                TurnPictureItemEvent(dto, width, height, point, currDControl.pageId);
            }
        }

        public TurnPicture(DControl ctl, Boolean isDesign, List<TurnPictureImagesDto> list)
        {
            InitializeComponent();
            this.list = list;
            currDControl = ctl;
            turnImage_Timer = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(currDControl.turnPictureSpeed * 1000), IsEnabled = true };
            init(ctl, isDesign, list);
            //页面加载完成
            Loaded += UserControl_Loaded;
            Unloaded += UserControl_UnLoaded;

        }

        private void UserControl_UnLoaded(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("TurnPicture:UserControl_UnLoaded");
            storyboard.Stop();
            storyboard.Children.Clear();
            Loaded -= UserControl_Loaded;
            Unloaded -= UserControl_UnLoaded; 
            turnImage_Timer.Tick -= new EventHandler(Timer_Turn); //超过计时间隔时发生  
            turnImage_Timer.Stop(); 
            SizeChanged -= userControl1_SizeChanged;
             
             
            foreach (ListBoxItem item in turnImage_listbox.Items)
            {
                item.Background = null;
                item.Style = null;
            }
            foreach (Button btn in pagePanel.Children)
            {
                btn.Style = null;
                btn.Click -= pagePanel_Button_Click;
            }
            arrowLeft.Background = null;
            arrowRight.Background = null;
            arrowLeft.Click -= arrowLeft_Button_Click;
            arrowRight.Click -= arrowRight_Button_Click;

            turnImage_listbox.PreviewMouseDown -= listbox_PreviewMouseDown;
            turnImage_listbox.PreviewMouseMove -= listbox_PreviewMouseMove;
            turnImage_listbox.PreviewMouseUp -= listbox_PreviewMouseUp;
            turnImage_listbox.PreviewTouchDown -= listbox_PreviewTouchDown;
            turnImage_listbox.PreviewTouchMove -= listbox_PreviewTouchMove;
            turnImage_listbox.PreviewTouchUp -= listbox_PreviewTouchUp;

            pagePanel.Children.Clear();
            turnImage_listbox.Items.Clear();
            mainContainer.Children.Clear();
            this.Resources.Clear(); 
            GC.Collect();
        }


        /*
         * 初始化并绑定鼠标滑动效果
         */
        private void init(DControl ctl, Boolean isDesign, List<TurnPictureImagesDto> list)
        {
            fillImage(ctl, list);
            showArrow(ctl);

            //非设计状态下，绑定滑动
            if (!isDesign)
            {
                turnImage_listbox.PreviewMouseDown += listbox_PreviewMouseDown;
                turnImage_listbox.PreviewMouseMove += listbox_PreviewMouseMove;
                turnImage_listbox.PreviewMouseUp += listbox_PreviewMouseUp;


                turnImage_listbox.PreviewTouchDown += listbox_PreviewTouchDown;
                turnImage_listbox.PreviewTouchMove += listbox_PreviewTouchMove;
                turnImage_listbox.PreviewTouchUp += listbox_PreviewTouchUp;

                arrowLeft.Click += arrowLeft_Button_Click;
                arrowRight.Click += arrowRight_Button_Click;
            }
            else
            {
                //设计模式禁用水平滑动
                turnImage_listbox.SetValue(ScrollViewer.HorizontalScrollBarVisibilityProperty, ScrollBarVisibility.Disabled);
                turnImage_Timer = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(3600000), IsEnabled = false };
            }

        }
         

        /*
        * 显示左右按钮
        */
        private void showArrow(DControl ctl)
        {
            if (ctl.isShowTurnPictureArrow)
            {
                arrow.Visibility = Visibility.Visible;
            }
            else
            {
                arrow.Visibility = Visibility.Collapsed;
                return;
            }

            arrowLeft.Background = new ImageBrush
            {
                ImageSource = ico_arrowLeft
                ,
                Stretch = Stretch.Uniform
            };
            arrowRight.Background = new ImageBrush
            {
                ImageSource = ico_arrowRight
                ,
                Stretch = Stretch.Uniform
            };
            if (turnImageWhich > 0)
            {
                arrowLeft.Background = new ImageBrush
                {
                    ImageSource = ico_arrowLeft_active
                               ,
                    Stretch = Stretch.Uniform
                };
            }
            int count = turnImage_listbox.Items.Count;
            if (turnImageWhich < count - 2 || currDControl.loop)
            {
                arrowRight.Background = new ImageBrush
                {
                    ImageSource = ico_arrowRight_active
                   ,
                    Stretch = Stretch.Uniform
                };
            }

            double top = (ctl.height - arrowLeft.Height) / 2;
            arrowLeft.SetValue(Canvas.TopProperty, top);
            arrowRight.SetValue(Canvas.TopProperty, top); 
        }


        private void listbox_PreviewTouchDown(object sender, TouchEventArgs e)
        {
            isMoving = false;
            storyboard.Seek(TimeSpan.FromMilliseconds(400));
            ListBox listbox1 = (ListBox)sender;
            ScrollViewer sv = FindVisualChild<ScrollViewer>(listbox1);

            isMouseDown = true;
            startX = e.GetTouchPoint(this).Position.X;
            controlX = e.GetTouchPoint(this).Position.X;
            // this.offsetLeft = sv.HorizontalOffset;
            offsetLeft = turnImageWhich * listbox1.Width;
            moveX = 0;
            turnImage_Timer.Stop();
            //  listbox1.CaptureMouse();
            //textbox1.Text = "开始拖动" + offsetLeft;
            listbox1.CaptureTouch(e.TouchDevice);
            e.Handled = true;
        }

        private void listbox_PreviewTouchMove(object sender, TouchEventArgs e)
        {
            if (isMouseDown)
            {
                e.Handled = false;
                double currX = e.GetTouchPoint(this).Position.X;
                moveX = (startX - currX);
                double finalOffsetLeft = moveX + offsetLeft;

                if (moveX > 0)
                {
                    int count = turnImage_listbox.Items.Count;
                    if (!currDControl.loop && turnImageWhich >= count - 2)
                    {
                        e.Handled = true;
                        return;
                    }
                }
                if (moveX != 0) isMoving = true;

                ListBox listbox1 = (ListBox)sender;
                double width = listbox1.Width;

                // textbox1.Text = controlX +"___"+ moveX.ToString();  
                // ListBox listbox1 = (ListBox)sender;  
                ScrollViewer sv = FindVisualChild<ScrollViewer>(listbox1);
                sv.ScrollToHorizontalOffset(finalOffsetLeft);

                if (moveX > 0)
                {
                    if (moveX >= controlX)
                    {
                        listbox_PreviewTouchUp(sender, e);
                    }
                }
                else
                {
                    if (width + moveX <= controlX)
                    {
                        listbox_PreviewTouchUp(sender, e);
                    }
                }
            }
        }

        private void listbox_PreviewTouchUp(object sender, TouchEventArgs e)
        {
            if (!isMoving)
            {
                TurnPictureImagesDto dto = list[turnImageWhich];
                // if (dto.linkToPageId <= 0) return;
                System.Windows.Point point = new System.Windows.Point();
                point.X = currDControl.left;
                point.Y = currDControl.top;
                RaiseEvent(dto, currDControl.width, currDControl.height, point);
            }

            //  textbox1.Text = "进入listbox_mouseup"; //MessageBox.Show("进入listbox_mouseup");
            ListBox listbox1 = (ListBox)sender;
            ScrollViewer sv = FindVisualChild<ScrollViewer>(listbox1);
            double ox = 40;

            double width = listbox1.Width;
            int count = listbox1.Items.Count;
            int n = (int)Math.Floor(offsetLeft / width);


            if (moveX > 0)
            {
                if (moveX > ox)
                {
                    n = n + 1;
                    if (n >= count)
                    {
                        n = count;
                    }

                }

            }
            else if (moveX < 0)
            {
                if (moveX < -ox)
                {
                    n = n - 1;
                    if (n < 0) { n = 0; }
                }
            }

            //   textbox1.Text = "当前第几个：" + n; 
            Set_ListBoxItem_ActiveAsync(listbox1, n);


            isMouseDown = false;
            listbox1.ReleaseTouchCapture(e.TouchDevice);
            e.Handled = true;
        }


        /*
         * 填充轮播图中的图片，设置宽度，并绑定点按钮事件
         */
        private void fillImage(DControl ctl, List<TurnPictureImagesDto> list)
        {
            turnImage_listbox.Width = ctl.width;

            if (list == null || list.Count == 0)
            {
                //return;
            }

            System.Windows.Style itemStyle = (System.Windows.Style)FindResource("ListBoxItemStyle1");
            System.Windows.Style turnPicturePageButtonStyle = (System.Windows.Style)FindResource("TurnPicturePageButtonStyle");
            int i = 0;
            foreach (TurnPictureImagesDto dto in list)
            {
                i = i + 1;
                ListBoxItem item = new ListBoxItem();
                item.Name = "item" + i;
                item.Width = ctl.width;
                item.BorderThickness = new Thickness(0);
                item.AllowDrop = true;
                item.Style = itemStyle;
                item.Background = null;
                item.Focusable = false;
                turnImage_listbox.Items.Add(item);



                ////添加point
                Button btn = new Button();
                btn.Width = 15;
                btn.Height = 15;
                btn.Margin = new Thickness(5, 0, 5, 0);
                btn.Padding = new Thickness(0);
                btn.BorderThickness = new Thickness(0);
                btn.Style = turnPicturePageButtonStyle;
                pagePanel.Children.Add(btn);
            }
            //绑定点按钮
            pagePanel_Button_bindClick();


            ListBoxItem item1 = new ListBoxItem();
            item1.Name = "item";
            item1.Width = ctl.width;
            item1.BorderThickness = new Thickness(0);
            item1.AllowDrop = true;
            item1.Style = itemStyle;
            item1.Background = null;
            turnImage_listbox.Items.Add(item1);
        }


        private void pagePanel_Button_bindClick()
        {
            UIElementCollection elements = pagePanel.Children;
            for (int i = 0; i < elements.Count; i++)
            {
                Button btn = (Button)elements[i];
                btn.Tag = i;

                btn.Click += pagePanel_Button_Click;
            }
        }

        /*
         * 向左切换一个位置
         */
        private void arrowLeft_Button_Click(object sender, RoutedEventArgs e)
        {
            int count = turnImage_listbox.Items.Count;
            int n = turnImageWhich - 1;
            Set_ListBoxItem_ActiveAsync(turnImage_listbox, n);
        }


        /*
         * 向右切换一个位置
         */
        private void arrowRight_Button_Click(object sender, RoutedEventArgs e)
        {
            int count = turnImage_listbox.Items.Count;
            int n = turnImageWhich + 1;
            Set_ListBoxItem_ActiveAsync(turnImage_listbox, n);
        }

        private void pagePanel_Button_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            string tag = btn.Tag.ToString();
            Int32 n = int.Parse(tag);
            Set_ListBoxItem_ActiveAsync(turnImage_listbox, n);
        }
        /*
         * 定时切换
         */
        private void Timer_Turn(object sender, EventArgs e)
        {
            int n = turnImageWhich + 1;
            Set_ListBoxItem_ActiveAsync(turnImage_listbox, n);
        }



        /*
         * ListBox鼠标按下
         */
        private void listbox_PreviewMouseDown(object sender, MouseEventArgs e)
        {
            isMoving = false;
            storyboard.Seek(TimeSpan.FromMilliseconds(400));
            ListBox listbox1 = (ListBox)sender;
            ScrollViewer sv = FindVisualChild<ScrollViewer>(listbox1);


            isMouseDown = true;
            startX = e.GetPosition(this).X;
            controlX = e.GetPosition(this).X;
            // this.offsetLeft = sv.HorizontalOffset;
            offsetLeft = turnImageWhich * listbox1.Width;
            moveX = 0;
            turnImage_Timer.Stop();
            //  listbox1.CaptureMouse();
            //textbox1.Text = "开始拖动" + offsetLeft;
            listbox1.CaptureMouse();
            e.Handled = true;


        }
        /*
         * ListBox鼠标移动
         */
        private void listbox_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (isMouseDown)
            {
                e.Handled = false;
                double currX = e.GetPosition(this).X;
                moveX = (startX - currX);
                double finalOffsetLeft = moveX + offsetLeft;

                if (moveX > 0)
                {
                    int count = turnImage_listbox.Items.Count;
                    if (!currDControl.loop && turnImageWhich >= count - 2)
                    {
                        e.Handled = true;
                        return;
                    }
                }
                if (moveX != 0) isMoving = true;

                ListBox listbox1 = (ListBox)sender;
                double width = listbox1.Width;

                // textbox1.Text = controlX +"___"+ moveX.ToString(); 

                // ListBox listbox1 = (ListBox)sender;
                ScrollViewer sv = FindVisualChild<ScrollViewer>(listbox1);
                sv.ScrollToHorizontalOffset(finalOffsetLeft);

                if (moveX > 0)
                {
                    if (moveX >= controlX)
                    {
                        listbox_PreviewMouseUp(sender, e);
                    }
                }
                else
                {
                    if (width + moveX <= controlX)
                    {
                        listbox_PreviewMouseUp(sender, e);
                    }
                }


            }
        }



        /*
         *  如果moveX>20，则进入下一个;如果 0<moveX<=20,回到原来位置
         *  如果moveX<-20，则进入上一个
         *  如果到达最后一个，则回到起始位置 
         */
        private void listbox_PreviewMouseUp(object sender, MouseEventArgs e)
        {
            if (!isMoving)
            {
                TurnPictureImagesDto dto = list[turnImageWhich];
                //  if (dto.linkToPageId <= 0) return;
                System.Windows.Point point = new System.Windows.Point();
                point.X = currDControl.left;
                point.Y = currDControl.top;
                RaiseEvent(dto, currDControl.width, currDControl.height, point);
            }

            ListBox listbox1 = (ListBox)sender;
            ScrollViewer sv = FindVisualChild<ScrollViewer>(listbox1);
            double ox = 40;

            double width = listbox1.Width;
            int count = listbox1.Items.Count;
            int n = (int)Math.Floor(offsetLeft / width);



            if (moveX > 0)
            {
                if (moveX > ox)
                {
                    n = n + 1;
                    if (n >= count)
                    {
                        n = count;
                    }

                }

            }
            else if (moveX < 0)
            {
                if (moveX < -ox)
                {
                    n = n - 1;
                    if (n < 0) { n = 0; }
                }
            }

            //   textbox1.Text = "当前第几个：" + n; 
            Set_ListBoxItem_ActiveAsync(listbox1, n);


            isMouseDown = false;
            listbox1.ReleaseMouseCapture();
            e.Handled = true;

        }
        /*
         * 选中图片,及按钮
         * 
         */
        private void Set_ListBoxItem_ActiveAsync(ListBox listbox1, int n)
        {
            turnImage_Timer.Stop();
            storyboard.Children.Clear(); 

            int count = listbox1.Items.Count;
            if (count <= 0) { return; }

            if (n < 0) { n = 0; }
            if (n >= count - 1)
            {
                if (currDControl.loop)
                {
                    // n = 0;
                }
                else
                {
                    n = count - 1; return;
                }
            }

            ScrollViewer sv = FindVisualChild<ScrollViewer>(listbox1); //此处出现获取不到ScrollViewer的情况
            double width = listbox1.Width;

            //到达最后一个时，返回第一个
            int x = n;
            if (n == count - 1) { x = 0; }
            turnImageWhich = x;
            Set_Button_Active(pagePanel.Children[x], System.Drawing.Brushes.Black);
            loadImage(n);
            showArrow(currDControl);


            DoubleAnimation doubleAnimation = new DoubleAnimation(sv.HorizontalOffset, width * n, new Duration(TimeSpan.FromMilliseconds(400.0)));
            doubleAnimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
            Storyboard.SetTarget(doubleAnimation, sv);
            Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath(ScrollViewerProperty.HorizontalOffsetProperty));
            storyboard.Children.Add(doubleAnimation);

            // 创建第2个动画
            if (n == count - 1)
            {
                DoubleAnimation doubleAnimation2 = new DoubleAnimation(sv.HorizontalOffset, 0, new Duration(TimeSpan.FromMilliseconds(0)));
                doubleAnimation2.BeginTime = TimeSpan.FromMilliseconds(400.0);
                Storyboard.SetTarget(doubleAnimation2, sv);
                Storyboard.SetTargetProperty(doubleAnimation2, new PropertyPath(ScrollViewerProperty.HorizontalOffsetProperty));
                storyboard.Children.Add(doubleAnimation2);
            } 
            storyboard.Begin();
            turnImage_Timer.Start();
        }


        private async void loadImage(int n)
        {
            int count = turnImage_listbox.Items.Count;
            if (count <= 0) { return; }

            try
            {
                for (int a = 0; a < count; a++)
                {
                    ListBoxItem item = (ListBoxItem)turnImage_listbox.Items[a];
                    //左右，第一个，最后一个，第二个
                    if ((a >= n - 1 && a <= n + 1) || a == 0 || a == count - 1 || (n == count - 1 && a == 1))
                    {

                        if (item.Background == null)
                        {
                            int x = a;
                            if (a == count - 1) { x = 0; }
                            string imgUrl = AppDomain.CurrentDomain.BaseDirectory + list[x].url;
                            Console.WriteLine(n + "__" + a);
                            Border border = FrameworkElementUtil.GetChildObject<Border>(item, "Loading");
                            if (border != null)
                            {
                                border.Visibility = Visibility.Visible;
                            }

                            ImageSource source = await Task.Run<ImageSource>(() =>
                            {
                                BitmapImage bitmapImage = new BitmapImage();
                                //打开文件流
                                // using (Stream stream = File.OpenRead(imgUrl))
                                using (Stream stream = new MemoryStream(File.ReadAllBytes(imgUrl)))
                                {
                                    bitmapImage.BeginInit();
                                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                                    bitmapImage.StreamSource = stream;
                                    bitmapImage.DecodePixelWidth = currDControl.width;
                                    bitmapImage.EndInit();
                                    //这一句很重要，少了UI线程就不认了。
                                    bitmapImage.Freeze();
                                }
                                return bitmapImage;
                            });
                            //出炉
                            item.Background = new ImageBrush
                            {
                                ImageSource = source
                            ,
                                Stretch = Stretch.Fill
                            };
                            if (border != null)
                            {
                                border.Visibility = Visibility.Hidden;
                            }
                        }

                    }
                    else
                    {

                        item.Background = null;
                        Console.WriteLine("清空" + n + "__" + a);
                    }

                }


                // bigImage.Source = source;
            }
            catch { }
            GC.Collect();
        }



        /*
         * 
         * 设置选中的点
         * 
         * 
         */
        private void Set_Button_Active(object sender, System.Drawing.Brush activeBrush)
        {
            UIElementCollection children = pagePanel.Children;
            var bc = new BrushConverter();
            foreach (UIElement ele in children)
            {
                Button tmp = (Button)ele;
                tmp.IsDefault = false;
            }

            Button button = (Button)sender;
            button.IsDefault = true;

            //  button.Background = activeBrush;

        }



        /*
         * 获取ListBox下的ScrollView 
         */
        public static childItem FindVisualChild<childItem>(DependencyObject obj)
 where childItem : DependencyObject
        {
            // Iterate through all immediate children
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);

                if (child != null && child is childItem)
                    return (childItem)child;

                else
                {
                    childItem childOfChild = FindVisualChild<childItem>(child);

                    if (childOfChild != null)
                        return childOfChild;
                }
            }

            return null;
        }



        /*
         * 控件加载后执行
         */
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            turnImage_listbox.UpdateLayout();
            UpdateLayout();

            turnImage_Timer.Tick += new EventHandler(Timer_Turn); //超过计时间隔时发生 
            if (currDControl.autoplay)
            {
                turnImage_Timer.Start(); //DT启动   
            }
            else
            {
                turnImage_Timer.Stop(); //DT启动   
            }
            SizeChanged += userControl1_SizeChanged;

            Set_ListBoxItem_ActiveAsync(turnImage_listbox, 0);

        }

        /*
         * 控件尺寸改变
         */
        private void userControl1_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            turnImage_listbox.Width = Width;
            turnImage_listbox.Height = Height;
            foreach (ListBoxItem item in turnImage_listbox.Items)
            {
                item.Width = Width;
                item.Height = Height;
            }
            currDControl.width = Convert.ToInt32(Width);
            currDControl.height = Convert.ToInt32(Height);
            showArrow(currDControl);
        }

        //private void turnImage_listbox_TouchLeave(object sender, TouchEventArgs e)
        //{
        //    listbox_PreviewTouchUp(sender, e); 
        //}

        /*
        * 编辑控件，更新页面显示
        */
        public void updateElement(DControl ctl, Boolean isDesign, List<TurnPictureImagesDto> list)
        {
            foreach (ListBoxItem item in turnImage_listbox.Items)
            {
                item.Background = null;
                item.Style = null;
            }
            foreach (Button btn in pagePanel.Children)
            {
                btn.Style = null;
                btn.Click -= pagePanel_Button_Click;
            }
            turnImageWhich = 0;
            turnImage_listbox.Items.Clear();
            pagePanel.Children.Clear();
            GC.Collect();

            this.list = list;
            fillImage(ctl, list);

            turnImage_listbox.UpdateLayout();
            UpdateLayout();

            //设置默认选中按钮
            Set_ListBoxItem_ActiveAsync(turnImage_listbox, 0);
        }

        public void updateElementAttr(DControl dControl, bool p)
        {
            Width = dControl.width;
            Height = dControl.height;
            Margin = new Thickness(dControl.left, dControl.top, 0, 0);
            Opacity = dControl.opacity / 100.0;
            currDControl = dControl;
            //设置循环 自动播放 速度，添加属性，及初始化时的配置 
            turnImage_Timer = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(currDControl.turnPictureSpeed * 1000), IsEnabled = true };
            if (currDControl.autoplay)
            {
                turnImage_Timer.Start(); //DT启动   
            }
            else
            {
                turnImage_Timer.Stop(); //DT启动   
            }

            showArrow(currDControl);
        }


    }
}
