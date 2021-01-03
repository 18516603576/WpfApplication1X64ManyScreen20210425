using Model;
using Model.dto;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Point = System.Windows.Point;

namespace Common.control
{
    /// <summary>
    /// TurnPicture.xaml 的交互逻辑
    /// </summary>
    public partial class Marque : UserControl
    {
        //定时器 
        DispatcherTimer turnImage_Timer = null;
        //private delegate void TimerDispatcherDelegate(); 

        //mouseDown时，flag=true；mouseup时，flag=false
        private Boolean isMouseDown = false;
        //mouseDown时的坐标X
        private double startX = 0;
        //ListBox卷起的宽度
        private double offsetLeft = 0;
        //本次鼠标移动的距离
        private double moveX = 0;
        //当前切换到第几个图片了
        private int turnImageWhich = 0;

        private DControl currDControl;
        //相册列表
        private List<TurnPictureImagesDto> list = null;
        //是否支持触摸
        private Boolean isSupportTouch = false;
         
        private int itemWidth = 0;
        //如果是单击，则isMoving=false 
        private bool isMoving = false;
        //当前点击的图片
        private Button currItem = null;

        //三、声明委托 全屏则隐藏音乐播放按钮
        public delegate void MarqueItemHandler(TurnPictureImagesDto dto, double width, double height, Point point, int pageId);
        public event MarqueItemHandler MarqueItemEvent;
        public void RaiseEvent(TurnPictureImagesDto dto, double width, double height, Point point)
        {
            if (MarqueItemEvent != null)
            {
                MarqueItemEvent(dto, width, height, point, currDControl.pageId);
            }
        }


        public Marque(DControl ctl, Boolean isDesign, List<TurnPictureImagesDto> list)
        {
            InitializeComponent();

            this.list = list;
            currDControl = ctl;
            double ms = 1000 / 60.0 / currDControl.turnPictureSpeed * 1000;
            turnImage_Timer = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(ms), IsEnabled = true };
            init(ctl, isDesign, list);
            //页面加载完成
            Loaded += UserControl_Loaded;
            Unloaded += UserControl_UnLoaded;
        }

        private void UserControl_UnLoaded(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Marque:UserControl_UnLoaded");
            Loaded -= UserControl_Loaded;
            Unloaded -= UserControl_UnLoaded;
            SizeChanged -= userControl1_SizeChanged;

            turnImage_Timer.Tick -= new EventHandler(Timer_Turn); //超过计时间隔时发生  
            turnImage_Timer.Stop();  
             
            foreach (Button item in marque_stackPanel.Children)
            {
                item.Background = null;
                item.Style = null;
                item.PreviewMouseDown -= item_Click;
                item.PreviewTouchDown -= item_Click;
            }
            foreach (Button btn in pagePanel.Children)
            {
                btn.Style = null;
            }

            marque_scrollViewer.PreviewMouseDown -= marque_scrollViewer_PreviewMouseDown;
            marque_scrollViewer.PreviewMouseMove -= marque_scrollViewer_PreviewMouseMove;
            marque_scrollViewer.PreviewMouseUp -= marque_scrollViewer_PreviewMouseUp;
            marque_scrollViewer.TouchDown -= marque_scrollViewer_PreviewTouchDown;
            marque_scrollViewer.TouchMove -= marque_stackPanel_PreviewTouchMove;
            marque_scrollViewer.TouchUp -= marque_stackPanel_PreviewTouchUp;
            marque_scrollViewer.ManipulationBoundaryFeedback -= Marque_scrollViewer_ManipulationBoundaryFeedback;
            marque_scrollViewer.ScrollChanged -= Marque_scrollViewer_ScrollChanged;
            marque_scrollViewer.ManipulationInertiaStarting -= marque_scrollViewer_ManipulationInertiaStarting;


            pagePanel.Children.Clear();
            marque_stackPanel.Children.Clear();
            mainContainer.Children.Clear();
            this.Resources.Clear(); 
            GC.Collect();

        }



        /*
         * 初始化并绑定鼠标滑动效果
         */
        private void init(DControl ctl, Boolean isDesign, List<TurnPictureImagesDto> list)
        {
            itemWidth = (int)(ctl.width - ctl.spacing * (ctl.rowNum - 1)) / ctl.rowNum;
            fillImage(ctl, list);

            //非设计状态下，绑定滑动
            if (!isDesign)
            {
                marque_scrollViewer.PreviewMouseDown += marque_scrollViewer_PreviewMouseDown;
                marque_scrollViewer.PreviewMouseMove += marque_scrollViewer_PreviewMouseMove;
                marque_scrollViewer.PreviewMouseUp += marque_scrollViewer_PreviewMouseUp; 

                marque_scrollViewer.TouchDown += marque_scrollViewer_PreviewTouchDown;
                marque_scrollViewer.TouchMove += marque_stackPanel_PreviewTouchMove;
                marque_scrollViewer.TouchUp += marque_stackPanel_PreviewTouchUp;
                marque_scrollViewer.ManipulationBoundaryFeedback += Marque_scrollViewer_ManipulationBoundaryFeedback;
                marque_scrollViewer.ScrollChanged += Marque_scrollViewer_ScrollChanged;
                marque_scrollViewer.ManipulationInertiaStarting += marque_scrollViewer_ManipulationInertiaStarting;

            }
            else
            {
                //设计模式禁用水平滑动
                marque_scrollViewer.SetValue(ScrollViewer.HorizontalScrollBarVisibilityProperty, ScrollBarVisibility.Disabled);
                turnImage_Timer = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(3600000), IsEnabled = false };
            }

        }

        private void marque_scrollViewer_ManipulationInertiaStarting(object sender, ManipulationInertiaStartingEventArgs e)
        {
            //e.TranslationBehavior.DesiredDeceleration = 40.0 * 96.0 / (1000.0 * 1000.0);
            //  e.TranslationBehavior.DesiredDeceleration = 1.0 * 96.0 / (1000.0 * 1000.0);
            //Trace.WriteLine(e.TranslationBehavior.InitialVelocity);

        }


        private void Marque_scrollViewer_ManipulationBoundaryFeedback(object sender, ManipulationBoundaryFeedbackEventArgs e)
        {
            e.Handled = true;
        }

        private void marque_stackPanel_PreviewTouchUp(object sender, TouchEventArgs e)
        {
            isMouseDown = false;
            if (!isMoving && currItem != null)
            {
                TurnPictureImagesDto dto = (TurnPictureImagesDto)currItem.Tag;
               
                GeneralTransform generalTransform1 = currItem.TransformToAncestor(this);
                Point point = generalTransform1.Transform(new Point(currDControl.left, currDControl.top));

                RaiseEvent(dto, currItem.Width, currItem.Height, point);
               

            }
            turnImage_Timer.Start();
            marque_scrollViewer.ReleaseTouchCapture(e.TouchDevice);
            e.Handled = true;

        }

        private void marque_stackPanel_PreviewTouchMove(object sender, TouchEventArgs e)
        {
            if (isMouseDown)
            {
                double currX = e.GetTouchPoint(this).Position.X;
                moveX = (startX - currX);
                double finalOffsetLeft = moveX + offsetLeft;
                if (moveX != 0) isMoving = true;
                marque_scrollViewer.ScrollToHorizontalOffset(finalOffsetLeft);
            }
        }

        private void marque_scrollViewer_PreviewTouchDown(object sender, TouchEventArgs e)
        {
            isSupportTouch = true;
            turnImage_Timer.Stop();
            isMouseDown = true;
            isMoving = false;

            offsetLeft = marque_scrollViewer.HorizontalOffset;
            startX = e.GetTouchPoint(this).Position.X;
            marque_scrollViewer.CaptureTouch(e.TouchDevice);
            e.Handled = true;
        }



        private void marque_scrollViewer_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (isSupportTouch) return;
            isMouseDown = false;
            if (!isMoving && currItem != null)
            {
                TurnPictureImagesDto dto = (TurnPictureImagesDto)currItem.Tag;
                
                GeneralTransform generalTransform1 = currItem.TransformToAncestor(this);
                Point point = generalTransform1.Transform(new Point(currDControl.left, currDControl.top));
                RaiseEvent(dto, currItem.Width, currItem.Height, point);


            }

            turnImage_Timer.Start();
            marque_scrollViewer.ReleaseMouseCapture();

        }

        private void marque_scrollViewer_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (isSupportTouch) return;
            if (isMouseDown)
            {
                double currX = e.GetPosition(this).X;
                moveX = (startX - currX);
                double finalOffsetLeft = moveX + offsetLeft;
                if (moveX != 0) isMoving = true;
                marque_scrollViewer.ScrollToHorizontalOffset(finalOffsetLeft);

            }
        }

        private void marque_scrollViewer_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (isSupportTouch) return;
            turnImage_Timer.Stop();
            isMouseDown = true;
            isMoving = false;

            offsetLeft = marque_scrollViewer.HorizontalOffset;
            startX = e.GetPosition(this).X;
            marque_scrollViewer.CaptureMouse();
        }




        /*
         * 填充轮播图中的图片，设置宽度，并绑定点按钮事件
         */
        private void fillImage(DControl ctl, List<TurnPictureImagesDto> list)
        {
            marque_scrollViewer.Width = ctl.width;

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
                Button item = new Button();
                item.Name = "item" + i;
                item.Width = itemWidth;
                item.Height = ctl.height;
                item.BorderThickness = new Thickness(0);
                item.AllowDrop = true;
                //   item.Style = itemStyle; 
                item.Background = System.Windows.Media.Brushes.Transparent;
                FileUtil.readImage2Button(item, AppDomain.CurrentDomain.BaseDirectory + dto.url, ctl.width / 3, Stretch.Fill);

                if (i > 1)
                {
                    item.Margin = new Thickness(ctl.spacing, 0, 0, 0);
                }
                item.Tag = dto;
                item.PreviewMouseDown += item_Click;
                item.PreviewTouchDown += item_Click;
                marque_stackPanel.Children.Add(item);


                ////添加point
                Button btn = new Button();
                btn.Width = 8;
                btn.Height = 8;
                btn.Margin = new Thickness(3, 0, 3, 0);
                btn.Padding = new Thickness(0);
                btn.BorderThickness = new Thickness(0);
                btn.Style = turnPicturePageButtonStyle;
                pagePanel.Children.Add(btn);
            }


            if (list.Count > ctl.rowNum)
            {
                for (int a = 0; a < ctl.rowNum; a++)
                {
                    TurnPictureImagesDto dto = list[a];
                    Button item = new Button();
                    item.Name = "itema" + a;
                    item.Width = itemWidth;
                    item.Height = ctl.height;
                    item.BorderThickness = new Thickness(0);
                    item.AllowDrop = true;
                    //   item.Style = itemStyle; 
                    item.Background = System.Windows.Media.Brushes.Transparent;
                    FileUtil.readImage2Button(item, AppDomain.CurrentDomain.BaseDirectory + dto.url, ctl.width / 3, Stretch.Fill);
                    item.Margin = new Thickness(ctl.spacing, 0, 0, 0);
                    item.Tag = dto;
                    item.PreviewMouseDown += item_Click;
                    item.PreviewTouchDown += item_Click;

                    marque_stackPanel.Children.Add(item);
                }

            }


        }

        /*
         * 点击图片 链接到
         */
        private void item_Click(object sender, RoutedEventArgs e)
        {
            Button item = (Button)sender;
            currItem = item;
        }

        /*
         * 定时切换
         */
        private void Timer_Turn(object sender, EventArgs e)
        {
            if (list.Count == 0) return;
            double offsetLeft = marque_scrollViewer.HorizontalOffset;
            double fullWidth = marque_stackPanel.ActualWidth - Width;
            if (offsetLeft >= fullWidth)
            {
                marque_scrollViewer.ScrollToHorizontalOffset(0);
                offsetLeft = 0;
            }

            double ps = offsetLeft;
            int a = (int)Math.Ceiling(ps / (itemWidth + currDControl.spacing));
            int x = (a - 1) % list.Count;
            if (x < 0) x = 0;
            turnImageWhich = x;
            Set_Button_Active(pagePanel.Children[x], System.Drawing.Brushes.Black);


            //int n = turnImageWhich + 1;
            // this.Set_ListBoxItem_ActiveAsync(marque_stackPanel, n);
            double pos = offsetLeft + 1;
            marque_scrollViewer.ScrollToHorizontalOffset(pos);
            if (pos < 5)
            {
                // positionLabel.Content = positionLabel.Content+","+pos;
            }

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
            marque_scrollViewer.UpdateLayout();
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
        }

        /*
         * 控件尺寸改变
         */
        private void userControl1_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            itemWidth = (int)(currDControl.width - currDControl.spacing * (currDControl.rowNum - 1)) / currDControl.rowNum;
            marque_scrollViewer.Width = Width;
            marque_scrollViewer.Height = Height;
            foreach (Button item in marque_stackPanel.Children)
            {
                item.Width = itemWidth;
                item.Height = Height;
            }
            currDControl.width = Convert.ToInt32(Width);
            currDControl.height = Convert.ToInt32(Height);

        }



        /*
        * 编辑控件，更新页面显示
        */
        public void updateElement(DControl ctl, Boolean isDesign, List<TurnPictureImagesDto> list)
        {
            foreach (Button item in marque_stackPanel.Children)
            {
                item.Background = null;
                item.Style = null;
            }
            foreach (Button btn in pagePanel.Children)
            {
                btn.Style = null;
            }
            turnImageWhich = 0;
            marque_stackPanel.Children.Clear();
            pagePanel.Children.Clear();
            GC.Collect();

            this.list = list;
            init(ctl, false, list);

            marque_scrollViewer.UpdateLayout();
            UpdateLayout();

        }

        public void updateElementAttr(DControl dControl, bool p)
        {
            Width = dControl.width;
            Height = dControl.height;
            Margin = new Thickness(dControl.left, dControl.top, 0, 0);
            Opacity = dControl.opacity / 100.0;
            currDControl = dControl;
            //设置循环 自动播放 速度，添加属性，及初始化时的配置 
            // turnImage_Timer = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(currDControl.turnPictureSpeed * 1000), IsEnabled = true };


            //2.清空图片，重新添加
            foreach (Button item in marque_stackPanel.Children)
            {
                item.Background = null;
                item.Style = null;
            }
            foreach (Button btn in pagePanel.Children)
            {
                btn.Style = null;
            }
            turnImageWhich = 0;
            marque_stackPanel.Children.Clear();
            pagePanel.Children.Clear();
            GC.Collect();


            init(dControl, false, list);

            marque_scrollViewer.UpdateLayout();
            UpdateLayout();
        }


        /*
         * 触摸屏惯性滑动到结尾，从头开始
         */
        private void Marque_scrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (list.Count == 0) return;
            double offsetLeft = marque_scrollViewer.HorizontalOffset;
            double fullWidth = marque_stackPanel.ActualWidth - Width;
            if (offsetLeft >= fullWidth)
            {
                marque_scrollViewer.PanningMode = PanningMode.None;
                marque_scrollViewer.IsHitTestVisible = false;
                // positionLabel.Content = positionLabel.Content + ",scrollchanged结尾";  
                turnImage_Timer.Start();
                marque_scrollViewer.PanningMode = PanningMode.HorizontalOnly;
                marque_scrollViewer.IsHitTestVisible = true;
            }
        }
        /*
         * 移动到最后，回到开始
         * 
         * 获取当前所在point
         */
        private void Marque_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            //if (list.Count == 0) return;
            //double offsetLeft = marque_scrollViewer.HorizontalOffset;
            //double fullWidth = marque_stackPanel.ActualWidth - this.Width;
            //if (offsetLeft >= fullWidth)
            //{
            //    marque_scrollViewer.ScrollChanged -= this.Marque_ScrollChanged;
            //    marque_scrollViewer.ScrollToHorizontalOffset(0);
            //    positionLabel.Content = positionLabel.Content + "," + 0+"_"+ offsetLeft;
            //    marque_scrollViewer.ScrollChanged += this.Marque_ScrollChanged;  
            //}

            //double ps = offsetLeft;
            //int a = (int) Math.Ceiling( ps / (itemWidth + currDControl.spacing));
            //int x = (a-1) % list.Count;
            //if (x < 0) x = 0;
            //this.turnImageWhich = x; 

            //Set_Button_Active(pagePanel.Children[x], System.Drawing.Brushes.Black);

        }

        /*
         * 移动到最后，回到开始
         * 
         * 获取当前所在point
         */
        private void Marque_ScrollChanged_bak(object sender, ScrollChangedEventArgs e)
        {
            //if (list.Count == 0) return;
            //double offsetLeft = marque_scrollViewer.HorizontalOffset;
            //double fullWidth = marque_stackPanel.ActualWidth - Width;
            //if (offsetLeft >= fullWidth)
            //{
            //    marque_scrollViewer.ScrollToHorizontalOffset(0);
            //}

            //double ps = offsetLeft + Width / 2;
            //int a = (int)Math.Ceiling(ps / (itemWidth + currDControl.spacing));
            //int x = (a - 1) % list.Count;
            //turnImageWhich = x;

            //Set_Button_Active(pagePanel.Children[x], System.Drawing.Brushes.Black);

        }
    }
}