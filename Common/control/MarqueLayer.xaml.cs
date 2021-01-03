using Common.Data;
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
using Point = System.Windows.Point;

namespace Common.control
{
    /// <summary>
    /// TurnPicture.xaml 的交互逻辑
    /// </summary>
    public partial class MarqueLayer : UserControl
    {
        //定时器 
        DispatcherTimer turnImage_Timer = null; 

        //mouseDown时，flag=true；mouseup时，flag=false
        private Boolean isMouseDown = false;
        //mouseDown时的坐标X
        private double startX = 0;
        //ListBox卷起的宽度
        private double offsetLeft = 0;
        //本次鼠标移动的距离
        private double moveX = 0;
        //增量，鼠标按下已滑过的item数量
        private int percentIncrement = 0;
        //当前切换到第几个图片了
        private int turnImageWhich = 3;
        //最中间的图片是第几张 
        private int middleIdx = 0;
        //初始化时，距离左边像素（当rownum为偶数时）
        private double initOffsetLeft = 0;

        private DControl currDControl;
        //相册列表
        private List<TurnPictureImagesDto> list = null;
        //是否支持触摸
        private Boolean isSupportTouch = false;
        //剩余
        private double percent = 0;
        private readonly Storyboard storyboard = new Storyboard();
        private Boolean All_Animation_Completed = true;


        private double itemWidth = 0;
        //如果是单击，则isMoving=false 
        private bool isMoving = false;
        //当前点击的图片
        private Button currItem = null;

        //三、声明委托 全屏则隐藏音乐播放按钮
        public delegate void MarqueLayerItemHandler(TurnPictureImagesDto dto, double width, double height, Point point, int pageId);
        public event MarqueLayerItemHandler MarqueLayerItemEvent;
        public void RaiseEvent(TurnPictureImagesDto dto, double width, double height, Point point)
        {
            if (MarqueLayerItemEvent != null)
            {
                MarqueLayerItemEvent(dto, width, height, point, currDControl.pageId);
            }
        }


        public MarqueLayer(DControl ctl, Boolean isDesign, List<TurnPictureImagesDto> list)
        {
            InitializeComponent();


            this.list = list;
            currDControl = ctl;
            double ms = currDControl.turnPictureSpeed;
            turnImage_Timer = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(ms), IsEnabled = true };
            init(ctl, isDesign, list);
            //页面加载完成
            Loaded += UserControl_Loaded;
            Unloaded += UserControl_UnLoaded;
        }

        private void UserControl_UnLoaded(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("MarqueLayer:UserControl_UnLoaded");
            storyboard.Stop();
            storyboard.Children.Clear(); 
            Loaded -= UserControl_Loaded;
            Unloaded -= UserControl_UnLoaded; 
            turnImage_Timer.Tick -= new EventHandler(Timer_Turn); //超过计时间隔时发生  
            turnImage_Timer.Stop();
            SizeChanged -= userControl1_SizeChanged;
           

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


            pagePanel.Children.Clear();
            marque_stackPanel.Children.Clear();
            mainContainer.Children.Clear();
            this.Resources.Clear();  
            GC.Collect();
        }

        /*
         * 获取最中间的图片，是第几张
         * 
         * 5张，则为3
         * 
         * 6张，则为4
         */
        private int getMiddleIdx()
        {
            int idx = 0;
            int remainders = currDControl.rowNum % 2;
            if (remainders == 1)
            {
                idx = (int)Math.Floor(currDControl.rowNum / 2.0) + 1;
                initOffsetLeft = 0;
            }
            else
            {
                idx = currDControl.rowNum / 2 + 1;
                initOffsetLeft = itemWidth / 2.0;
            }
            return idx;
        }


        /*
         * 初始化并绑定鼠标滑动效果
         */
        private void init(DControl ctl, Boolean isDesign, List<TurnPictureImagesDto> list)
        {

            itemWidth = (int)ctl.width / 2.0;

            //一行显示数量必须是奇数
            if (ctl.rowNum % 2 == 0)
                ctl.rowNum = ctl.rowNum + 1;

            middleIdx = getMiddleIdx();
            turnImageWhich = middleIdx;
            if (list.Count > ctl.rowNum && currDControl.loop)
                turnImageWhich = turnImageWhich + ctl.rowNum + 1;

            fillImage(ctl, list);


            // marque_scrollViewer.ScrollToHorizontalOffset(this.initOffsetLeft);
            //非设计状态下，绑定滑动
            if (!isDesign)
            {
                marque_scrollViewer.PreviewMouseDown += marque_scrollViewer_PreviewMouseDown;
                marque_scrollViewer.PreviewMouseMove += marque_scrollViewer_PreviewMouseMove;
                marque_scrollViewer.MouseUp += marque_scrollViewer_PreviewMouseUp;


                marque_scrollViewer.TouchDown += marque_scrollViewer_PreviewTouchDown;
                marque_scrollViewer.TouchMove += marque_stackPanel_PreviewTouchMove;
                marque_scrollViewer.TouchUp += marque_stackPanel_PreviewTouchUp;
                marque_scrollViewer.ManipulationBoundaryFeedback += Marque_scrollViewer_ManipulationBoundaryFeedback;
                marque_scrollViewer.ScrollChanged += Marque_scrollViewer_ScrollChanged;

            }
            else
            { 
                //设计模式禁用水平滑动
                // marque_scrollViewer.SetValue( ScrollViewer.HorizontalScrollBarVisibilityProperty, ScrollBarVisibility.Disabled);
                turnImage_Timer = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(3600000), IsEnabled = false };
            }

        }



        private void Marque_scrollViewer_ManipulationBoundaryFeedback(object sender, ManipulationBoundaryFeedbackEventArgs e)
        {
            e.Handled = true;
        }

        private void marque_stackPanel_PreviewTouchUp(object sender, TouchEventArgs e)
        {
            All_Animation_Completed = false;
            marque_scrollViewer.IsHitTestVisible = false;
            isMouseDown = false;
            if (!isMoving && currItem != null)
            {
                MarqueItemTag tag = (MarqueItemTag)currItem.Tag;
                 
                GeneralTransform generalTransform1 = currItem.TransformToAncestor(this);
                Point point = generalTransform1.Transform(new Point(currDControl.left, currDControl.top));

                RaiseEvent(tag, currItem.Width, currItem.Height, point);

            }


            turnImageWhich = turnImageWhich + percentIncrement;
            double leavePercent = percent - percentIncrement;


            if (leavePercent > 0.2)
                turnImageWhich = turnImageWhich + 1;
            else if (leavePercent < -0.2)
                turnImageWhich = turnImageWhich - 1;
            // Console.WriteLine("mouseup this.leavePercent:"+ leavePercent+","+percent);



            Set_ListBoxItem_ActiveAsync(marque_stackPanel, turnImageWhich);
            //this.updateItemTag();

            if (currDControl.autoplay)
            {
                turnImage_Timer.Start(); //DT启动   
            }
            else
            {
                turnImage_Timer.Stop(); //DT启动   
            }
            marque_scrollViewer.ReleaseAllTouchCaptures();
            e.Handled = true;
            //  Console.WriteLine("TouchUp");

            marque_scrollViewer.IsHitTestVisible = true;

        }


        private void marque_stackPanel_PreviewTouchMove(object sender, TouchEventArgs e)
        { 
            if (isMouseDown)
            {
                double currX = e.GetTouchPoint(this).Position.X;
                moveX = (startX - currX);
                //  double finalOffsetLeft = this.moveX + offsetLeft;  
                if (moveX != 0) isMoving = true;
                //  marque_scrollViewer.ScrollToHorizontalOffset(finalOffsetLeft); 
                //MessageBox.Show("触摸点：" + e.GetIntermediateTouchPoints(this).Count + "___" + this.moveX);

                //左右图片宽高缩放
                double total = itemWidth;
                percent = moveX / total;
                //每跨过一个，则更新他的Tag
                if (percent - percentIncrement >= 1.0)
                {
                    percentIncrement = percentIncrement + 1;
                    updateItemTag();

                    Console.WriteLine(">=1.0 this.percent:" + percent);
                }
                else if (percent - percentIncrement <= -1.0)
                {
                    percentIncrement = percentIncrement - 1;
                    updateItemTag();
                }
                changeSize(percent);
            }
            e.Handled = true;
        }


        private void marque_scrollViewer_PreviewTouchDown(object sender, TouchEventArgs e)
        {
            isSupportTouch = true;
            turnImage_Timer.Stop();
            isMouseDown = true;
            isMoving = false;


            updateItemTag();
            marque_scrollViewer.IsHitTestVisible = true;

            //Button item = (Button)sender;
            //TransformGroup group = (TransformGroup)item.RenderTransform;
            //TranslateTransform translateTransform = (TranslateTransform)group.Children[1];
            offsetLeft = 0;
            startX = e.GetTouchPoint(this).Position.X;
            percentIncrement = 0;
            percent = 0;
            marque_scrollViewer.CaptureTouch(e.TouchDevice);
            e.Handled = true;
            Console.WriteLine("TouchDown");
        }



        private void marque_scrollViewer_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (isSupportTouch) return;
            isMouseDown = false;

            marque_scrollViewer.IsHitTestVisible = false;
            if (!isMoving && currItem != null)
            {
                MarqueItemTag tag = (MarqueItemTag)currItem.Tag;
                
                GeneralTransform generalTransform1 = currItem.TransformToAncestor(this);
                Point point = generalTransform1.Transform(new Point(currDControl.left, currDControl.top));
                RaiseEvent(tag, currItem.Width, currItem.Height, point);

            }

            turnImageWhich = turnImageWhich + percentIncrement;
            double leavePercent = percent - percentIncrement;


            if (leavePercent > 0.2)
                turnImageWhich = turnImageWhich + 1;
            else if (leavePercent < -0.2)
                turnImageWhich = turnImageWhich - 1;
            // Console.WriteLine("mouseup this.leavePercent:"+ leavePercent+","+percent); 

            Set_ListBoxItem_ActiveAsync(marque_stackPanel, turnImageWhich);


            if (currDControl.autoplay)
            {
                turnImage_Timer.Start(); //DT启动   
            }
            else
            {
                turnImage_Timer.Stop(); //DT启动   
            }
            marque_scrollViewer.ReleaseMouseCapture();
            // Console.WriteLine("MouseUp");  
            marque_scrollViewer.IsHitTestVisible = true;

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

                //左右图片宽高缩放
                double total = itemWidth;
                percent = moveX / total;
                //每跨过一个，则更新他的Tag
                if (percent - percentIncrement >= 1.0)
                {
                    percentIncrement = percentIncrement + 1;
                    updateItemTag();
                }
                else if (percent - percentIncrement <= -1.0)
                {
                    percentIncrement = percentIncrement - 1;
                    updateItemTag();
                }
                //  if (All_Animation_Completed)
                //  {
                changeSize(percent);
                // }
            }
            e.Handled = true;
        }

        private void changeSize(double percent)
        {
            int i = 0;
            int pt = (int)Math.Ceiling(percent);
            if (percent < 0)
            {
                pt = (int)Math.Floor(percent);
            }
            int next = turnImageWhich + pt;


            int pt2 = (int)Math.Floor(percent);
            if (percent < 0)
            {
                pt2 = (int)Math.Ceiling(percent);
            }
            int curr = turnImageWhich + pt2;


            Storyboard sb = new Storyboard();
            double leavePercent = percent - percentIncrement;

            if (leavePercent > 0.5)
            {
                curr = curr + 1;
            }
            if (leavePercent < -0.5)
            {
                curr = curr - 1;
            }

            foreach (Button item in marque_stackPanel.Children)
            {
                i = i + 1;

                int index = 1;
                if (i <= curr)
                {
                    index = i;
                }
                else
                {
                    index = curr - (i - curr);
                }
                Panel.SetZIndex(item, index);
                if (Math.Abs(next - i) < middleIdx + 1)
                {
                    TransformGroup group = (TransformGroup)item.RenderTransform;
                    ScaleTransform scaleTransform = (ScaleTransform)group.Children[0];

                    MarqueItemTag tag = (MarqueItemTag)item.Tag;
                    double startScale = tag.scale;
                    double toScale = 1 - Math.Abs(next - i) * 0.2;
                    double finalScale = startScale + (toScale - startScale) * Math.Abs(leavePercent);
                    scaleTransform.ScaleX = finalScale;
                    scaleTransform.ScaleY = finalScale;



                    //偏移
                    double scale = 1 - Math.Abs(i - next) * 0.2;
                    if (scale < 0.2)
                    {
                        scale = 0.2;
                    }
                    double translateX = tag.translateX;
                    if (next - i < middleIdx && next - i > 0)
                    {
                        //7,4 
                        double p = 0;
                        if (next - i == middleIdx - 1)
                        {
                            p = 0;
                        }
                        else if (next - i == 1)
                        {
                            p = itemWidth * 0.5 * 0.3;
                        }
                        else if (i > 1)
                        {   //0,25%,50%
                            double half2 = itemWidth * 0.5 * 0.3;
                            if (middleIdx - 2 > 0)
                            {
                                p = half2 / (middleIdx - 2) * Math.Abs(next - i - 1);
                            }
                        }

                        translateX = -itemWidth * Math.Abs(i - 1) - (1 - scale) * itemWidth * 0.5 + p; 
                    }

                    if (next - i == 0)
                    {
                        translateX = -itemWidth * Math.Abs(i - 1) + itemWidth * 0.5;
                    }

                    if (next - i < 0)
                    {
                        //最后一个偏移0，第一个偏移50%
                        double p = 0;
                        if (next - i == -middleIdx + 1)
                        {
                            p = 0;
                        }
                        else if (next - i == -1)
                        {
                            p = itemWidth * 0.5 * 0.3;
                        }

                        else
                        {
                            //50% 33.6% 16.6% 0
                            //     7     8    9
                            double half2 = itemWidth * 0.5 * 0.3;
                            if (middleIdx - 2 > 0)
                            {
                                p = half2 / (middleIdx - 2) * Math.Abs(next - i + 1);
                            }
                        }
                        double p2 = itemWidth * 2 - scale * itemWidth - p;
                        translateX = -itemWidth * Math.Abs(i - 1) - (1 - scale) * itemWidth * 0.5 + p2;

                    }

                    TranslateTransform translateTransform = (TranslateTransform)group.Children[1];
                    double increase = (translateX - tag.translateX) * Math.Abs(leavePercent);


                    //最后一个
                    if (percent > 0)
                    {
                        if (next - i == -middleIdx + 1)
                        {
                            translateX = -itemWidth * Math.Abs(i - 1) - (1 - scale) * itemWidth * 0.5 + itemWidth * 2 - scale * itemWidth;
                            increase = (translateX - tag.translateX);
                        }
                    }
                    //第一个
                    if (percent < 0)
                    {
                        if (next - i == middleIdx - 1)
                        {
                            increase = translateX - tag.translateX;
                        }
                        if (next - i == -middleIdx)
                        {
                            increase = 0;
                        }
                    }


                    translateTransform.X = tag.translateX + increase;
                    item.Opacity = 1.0;
                }
                else
                {
                    // item.Opacity = 0;
                }
            }
            sb.Begin();
        }

        /*
         * 更新item.Tag
         * 
         * 1.鼠标按下
         * 
         * 2.移动超过一个item
         * 
         */
        private void updateItemTag()
        {

            foreach (Button item in marque_stackPanel.Children)
            {
                TransformGroup group = (TransformGroup)item.RenderTransform;
                ScaleTransform scaleTransform = (ScaleTransform)group.Children[0];
                TranslateTransform translateTransform = (TranslateTransform)group.Children[1];
                double startScale = scaleTransform.ScaleX;
                double translateX = translateTransform.X;

                MarqueItemTag tag = (MarqueItemTag)item.Tag;
                tag.scale = startScale;
                tag.translateX = translateX;


                item.Tag = tag;
                item.UpdateLayout();
            }

        }

        private void marque_scrollViewer_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        { 
            if (isSupportTouch) return;
            storyboard.Seek(TimeSpan.FromMilliseconds(500));

            turnImage_Timer.Stop();
            isMouseDown = true;
            isMoving = false;

            updateItemTag();

            offsetLeft = 0;
            startX = e.GetPosition(this).X;
            percentIncrement = 0;
            percent = 0;

            marque_scrollViewer.CaptureMouse();
            //Console.WriteLine("MouseDown"); 

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
            double minScale = 1 - middleIdx * 0.2;
            if (minScale < 0.2)
            {
                minScale = 0.2;
            }


            //1.左侧
            System.Windows.Style itemStyle = (System.Windows.Style)FindResource("ListBoxItemStyle1");
            System.Windows.Style turnPicturePageButtonStyle = (System.Windows.Style)FindResource("TurnPicturePageButtonStyle");
            int i = 0;
            if (list.Count > ctl.rowNum && ctl.loop == true)
            {
                for (int a = ctl.rowNum + 1; a > 0; a--)
                {
                    i = i + 1;
                    double scale = minScale;
                    TurnPictureImagesDto dto = list[list.Count - a];
                    MarqueItemTag tag = MarqueItemTagUtil.convert(dto);  
                    tag.scale = scale;
                    tag.idx = list.Count - a;

                    Button item = new Button();
                    item.Name = "item";
                    item.Width = itemWidth;
                    item.Height = ctl.height;
                    item.BorderThickness = new Thickness(0);
                    item.AllowDrop = true;
                    //   item.Style = itemStyle; 
                    item.Background = null;
                    //item.Background = new ImageBrush
                    //{
                    //    ImageSource = FileUtil.readImage2(AppDomain.CurrentDomain.BaseDirectory + dto.url, (int)itemWidth),
                    //    Stretch = Stretch.Fill
                    //};
                    item.Margin = new Thickness(0);
                    item.Focusable = false;
                    item.Tag = tag;
                    //  item.Opacity = 0.1;
                    item.PreviewMouseDown += item_Click;
                    item.PreviewTouchDown += item_Click;
                    TransformGroup group = new TransformGroup();
                    ScaleTransform scaleTransform = new ScaleTransform();
                    scaleTransform.ScaleX = scale;
                    scaleTransform.ScaleY = scale;
                    TranslateTransform translateTransform = new TranslateTransform();
                    translateTransform.X = 0;
                    group.Children.Add(scaleTransform);
                    group.Children.Add(translateTransform);
                    item.RenderTransform = group;
                    item.RenderTransformOrigin = new Point(0.5, 0.5);

                    marque_stackPanel.Children.Add(item);
                }
            }

            //中间
            //int i = 0;
            for (int a = 0; a < list.Count; a++)
            {
                TurnPictureImagesDto dto = list[a];
                i = i + 1;

                double scale = minScale;
                double translateX = 0;
                //if (Math.Abs(this.turnImageWhich - i) < middleIdx)

                if (Math.Abs(turnImageWhich - i) < middleIdx || turnImageWhich - i == middleIdx)
                {
                    scale = 1 - Math.Abs(i - turnImageWhich) * 0.2;
                    if (scale < 0.2)
                    {
                        scale = 0.2;
                    }

                    //与中间图片，偏移个数
                    int offsetNum = Math.Abs(i - turnImageWhich);
                    //下一个偏移
                    double beforeScale = 1 - Math.Abs(i + 1 - turnImageWhich) * 0.2;




                    if (turnImageWhich - i < middleIdx && turnImageWhich - i > 0)
                    {
                        //    translateX = -(1-0.6)*itemWidth*0.5; 
                        //}
                        //1.左边的宽度是  itemWidth*0.5
                        //2.第二个起始位置 为左边宽度的一半
                        //3.第一个偏移0，最后一个偏移50%
                        double p = 0;
                        if (turnImageWhich - i == middleIdx - 1)
                        {
                            p = 0;
                        }
                        else if (turnImageWhich - i == 1)
                        {
                            p = itemWidth * 0.5 * 0.3;
                        }
                        else if (turnImageWhich - i > 1)
                        {  //0,25%,50%
                            double half2 = itemWidth * 0.5 * 0.3;
                            if (middleIdx - 2 > 0)
                            {
                                p = half2 / (middleIdx - 2) * Math.Abs(turnImageWhich - i - 1);
                            }
                        }

                        translateX = -itemWidth * Math.Abs(i - 1) - (1 - scale) * itemWidth * 0.5 + p;

                    }

                    if (turnImageWhich - i == 0)
                    {
                        translateX = -itemWidth * Math.Abs(i - 1) + itemWidth * 0.5;
                    }

                    if (turnImageWhich - i < 0)
                    {
                        //最后一个偏移0，第一个偏移50%
                        double p = 0;
                        if (turnImageWhich - i == -middleIdx + 1)
                        {
                            p = 0;
                        }
                        else if (turnImageWhich - i == -1)
                        {
                            p = itemWidth * 0.5 * 0.3;
                        }
                        else
                        {
                            //50% 33.6% 16.6% 0
                            //     7     8    9
                            double half2 = itemWidth * 0.5 * 0.3;
                            if (middleIdx - 2 > 0)
                            {
                                p = half2 / (middleIdx - 2) * Math.Abs(turnImageWhich - i + 1);
                            }
                        }
                        double p2 = itemWidth * 2 - scale * itemWidth - p;
                        translateX = -itemWidth * Math.Abs(i - 1) - (1 - scale) * itemWidth * 0.5 + p2;

                    }
                    // Console.WriteLine(i + ":"  + translateX);
                }


                int index = 1;
                if (i <= turnImageWhich)
                {
                    index = i;
                }
                else
                {

                    index = turnImageWhich - (i - turnImageWhich);
                }



                Button item = new Button();
                item.Name = "item";
                item.Width = itemWidth;
                item.Height = ctl.height;
                item.BorderThickness = new Thickness(0);
                item.AllowDrop = true;
                //   item.Style = itemStyle; 
                item.Background = null;
                if (Math.Abs(turnImageWhich - i) < middleIdx || turnImageWhich - i == middleIdx)
                {
                    item.Background = System.Windows.Media.Brushes.Transparent;
                    FileUtil.readImage2Button(item, AppDomain.CurrentDomain.BaseDirectory + dto.url, (int)itemWidth, Stretch.Fill);
                }
                Panel.SetZIndex(item, index);
                 
                MarqueItemTag tag = MarqueItemTagUtil.convert(dto); 
                tag.scale = scale;
                tag.idx = a;
                item.Margin = new Thickness(0);
                item.Focusable = false;

                item.Tag = tag;
                item.PreviewMouseDown += item_Click;
                item.PreviewTouchDown += item_Click;
                TransformGroup group = new TransformGroup();
                ScaleTransform scaleTransform = new ScaleTransform();
                scaleTransform.ScaleX = scale;
                scaleTransform.ScaleY = scale;

                TranslateTransform translateTransform = new TranslateTransform();
                translateTransform.X = translateX;
                group.Children.Add(scaleTransform);
                group.Children.Add(translateTransform);
                item.RenderTransform = group;
                item.RenderTransformOrigin = new Point(0.5, 0.5);
                marque_stackPanel.Children.Add(item);
                // marque_stackPanel.UpdateLayout();


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

            //3.右侧
            if (list.Count > ctl.rowNum && ctl.loop == true)
            {
                for (int a = 0; a < ctl.rowNum + 1; a++)
                {
                    double scale = minScale;
                    TurnPictureImagesDto dto = list[a];
                    MarqueItemTag tag = MarqueItemTagUtil.convert(dto); 
                    tag.scale = scale;
                    tag.idx = a;

                    Button item = new Button();
                    item.Name = "item";
                    item.Width = itemWidth;
                    item.Height = ctl.height;
                    item.BorderThickness = new Thickness(0);
                    item.AllowDrop = true;
                    //   item.Style = itemStyle;
                    item.Background = null;
                    //item.Background = new ImageBrush
                    //{
                    //    ImageSource = FileUtil.readImage2(AppDomain.CurrentDomain.BaseDirectory + dto.url, (int)itemWidth),
                    //    Stretch = Stretch.Fill
                    //};
                    item.Margin = new Thickness(0);
                    item.Focusable = false;
                    item.Tag = tag;
                    item.PreviewMouseDown += item_Click;
                    item.PreviewTouchDown += item_Click;
                    TransformGroup group = new TransformGroup();
                    ScaleTransform scaleTransform = new ScaleTransform();
                    scaleTransform.ScaleX = scale;
                    scaleTransform.ScaleY = scale;
                    TranslateTransform translateTransform = new TranslateTransform();
                    translateTransform.X = 0;
                    group.Children.Add(scaleTransform);
                    group.Children.Add(translateTransform);
                    item.RenderTransform = group;
                    item.RenderTransformOrigin = new Point(0.5, 0.5);

                    marque_stackPanel.Children.Add(item);
                }

            }


        }


        /*
         * 更新item尺寸和位置
         */
        private void updateItemSizeAndPosition1(DControl ctl)
        {
            marque_scrollViewer.Width = ctl.width;

            System.Windows.Style itemStyle = (System.Windows.Style)FindResource("ListBoxItemStyle1");
            System.Windows.Style turnPicturePageButtonStyle = (System.Windows.Style)FindResource("TurnPicturePageButtonStyle");

            int i = 0;
            foreach (Button item in marque_stackPanel.Children)
            {
                i = i + 1;
                double minScale = 1 - middleIdx * 0.2;
                if (minScale < 0.2)
                {
                    minScale = 0.2;
                }
                double scale = minScale;
                double translateX = 0;
                if (Math.Abs(turnImageWhich - i) < middleIdx)
                {
                    scale = 1 - Math.Abs(i - turnImageWhich) * 0.2;
                    if (scale < 0.2)
                    {
                        scale = 0.2;
                    }

                    //与中间图片，偏移个数
                    int offsetNum = Math.Abs(i - turnImageWhich);
                    //下一个偏移
                    double beforeScale = 1 - Math.Abs(i + 1 - turnImageWhich) * 0.2;

                    if (i < middleIdx)
                    {
                        //1.左边的宽度是  itemWidth*0.5
                        //2.第二个起始位置 为左边宽度的一半
                        //3.第一个偏移0，最后一个偏移50%
                        double p = 0;
                        if (i == 1)
                        {
                            p = 0;
                        }
                        else if (i == middleIdx - 1)
                        {
                            p = itemWidth * 0.5 * 0.3;
                        }
                        else if (i > 1)
                        {  //0,25%,50%
                            double half2 = itemWidth * 0.5 * 0.3;
                            if (middleIdx - 2 > 0)
                            {
                                p = half2 / (middleIdx - 2) * Math.Abs(i - 1);
                            }
                        }

                        translateX = -itemWidth * Math.Abs(i - 1) - (1 - scale) * itemWidth * 0.5 + p;

                    }

                    if (i == middleIdx)
                    {
                        translateX = -itemWidth * Math.Abs(i - 1) + itemWidth * 0.5;
                    }

                    if (i > middleIdx)
                    {
                        //最后一个偏移0，第一个偏移50%
                        double p = 0;
                        if (i == middleIdx * 2 - 1)
                        {
                            p = 0;
                        }
                        else if (i == middleIdx + 1)
                        {
                            p = itemWidth * 0.5 * 0.3;
                        }
                        else
                        {
                            //50% 33.6% 16.6% 0
                            //     7     8    9
                            double half2 = itemWidth * 0.5 * 0.3;
                            if (middleIdx - 2 > 0)
                            {
                                p = half2 / (middleIdx - 2) * Math.Abs(middleIdx * 2 - 1 - i);
                            }
                        }
                        double p2 = itemWidth * 2 - scale * itemWidth - p;
                        translateX = -itemWidth * Math.Abs(i - 1) - (1 - scale) * itemWidth * 0.5 + p2;

                    }
                    // Console.WriteLine(i + ":"  + translateX);
                }



                item.Width = itemWidth;
                item.Height = ctl.height;
                MarqueItemTag tag = (MarqueItemTag)item.Tag;
                tag.scale = scale;
                TransformGroup group = (TransformGroup)item.RenderTransform;
                ScaleTransform scaleTransform = (ScaleTransform)group.Children[0];
                scaleTransform.ScaleX = scale;
                scaleTransform.ScaleY = scale;

                TranslateTransform translateTransform = (TranslateTransform)group.Children[1];
                translateTransform.X = translateX;
            }

        }


        /*
        * 更新item尺寸和位置
        */
        private void updateItemSizeAndPosition(DControl ctl)
        {
            marque_scrollViewer.Width = ctl.width;

            System.Windows.Style itemStyle = (System.Windows.Style)FindResource("ListBoxItemStyle1");
            System.Windows.Style turnPicturePageButtonStyle = (System.Windows.Style)FindResource("TurnPicturePageButtonStyle");

            int i = 0;
            foreach (Button item in marque_stackPanel.Children)
            {
                i = i + 1;
                double minScale = 1 - middleIdx * 0.2;
                if (minScale < 0.2)
                {
                    minScale = 0.2;
                }
                double scale = minScale;
                double translateX = 0;
                MarqueItemTag tag = (MarqueItemTag)item.Tag;
                if (Math.Abs(turnImageWhich - i) < middleIdx || turnImageWhich - i == middleIdx)
                {
                    // if (Math.Abs(this.turnImageWhich - i) < middleIdx)
                    //{
                    scale = 1 - Math.Abs(turnImageWhich - i) * 0.2;
                    if (scale < 0.2)
                    {
                        scale = 0.2;
                    }

                    //与中间图片，偏移个数
                    int offsetNum = Math.Abs(i - turnImageWhich);
                    //下一个偏移
                    double beforeScale = 1 - Math.Abs(i + 1 - turnImageWhich) * 0.2;

                    translateX = tag.translateX;
                    if (turnImageWhich - i < middleIdx && turnImageWhich - i > 0)
                    {
                        //1.左边的宽度是  itemWidth*0.5
                        //2.第二个起始位置 为左边宽度的一半
                        //3.第一个偏移0，最后一个偏移50% 
                        double p = 0;
                        if (turnImageWhich - i == middleIdx - 1)
                        {
                            p = 0;
                        }
                        else if (turnImageWhich - i == 1)
                        {
                            p = itemWidth * 0.5 * 0.3;
                        }
                        else if (turnImageWhich - i > 1)
                        {   //0,25%,50%
                            double half2 = itemWidth * 0.5 * 0.3;
                            if (middleIdx - 2 > 0)
                            {
                                p = half2 / (middleIdx - 2) * Math.Abs(turnImageWhich - i - 1);
                            }
                        }
                        translateX = -itemWidth * Math.Abs(i - 1) - (1 - scale) * itemWidth * 0.5 + p;

                    }

                    if (turnImageWhich - i == 0)
                    {
                        translateX = -itemWidth * Math.Abs(i - 1) + itemWidth * 0.5;
                    }

                    if (turnImageWhich - i < 0)
                    {
                        //最后一个偏移0，第一个偏移50%
                        double p = 0;
                        if (turnImageWhich - i == -middleIdx + 1)
                        {
                            p = 0;
                        }
                        else if (turnImageWhich - i == -1)
                        {
                            //中间左侧第一个占0.7
                            p = itemWidth * 0.5 * 0.3;
                        }
                        else
                        {
                            double half2 = itemWidth * 0.5 * 0.3;
                            if (middleIdx - 2 > 0)
                            {
                                p = half2 / (middleIdx - 2) * Math.Abs(turnImageWhich - i + 1);
                            }
                        }
                        double p2 = itemWidth * 2 - scale * itemWidth - p;
                        translateX = -itemWidth * Math.Abs(i - 1) - (1 - scale) * itemWidth * 0.5 + p2;

                    }
                    // Console.WriteLine(i + ":"  + translateX);
                }



                item.Width = itemWidth;
                item.Height = ctl.height;
                tag.scale = scale;
                TransformGroup group = (TransformGroup)item.RenderTransform;
                ScaleTransform scaleTransform = (ScaleTransform)group.Children[0];
                scaleTransform.ScaleX = scale;
                scaleTransform.ScaleY = scale;

                TranslateTransform translateTransform = (TranslateTransform)group.Children[1];
                translateTransform.X = translateX;
            }

        }

        /*
         * 点击图片 链接到
         */
        private void item_Click(object sender, RoutedEventArgs e)
        {
            Button item = (Button)sender;
            currItem = item;
            //positionLabel.Content = positionLabel.Content + "item_click";
        }

        /*
         * 定时切换
         */
        private void Timer_Turn(object sender, EventArgs e)
        {
            if (list.Count == 0) return;
            percent = 0;
            turnImageWhich = turnImageWhich + 1;

            // Console.WriteLine("Timer_Turn:"+turnImageWhich);
            Set_ListBoxItem_ActiveAsync(marque_stackPanel, turnImageWhich);
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
         * 选中图片,及按钮
         * 
         */
        private void Set_ListBoxItem_ActiveAsync(StackPanel listbox1, int n)
        {

            All_Animation_Completed = false;
            turnImage_Timer.Stop();

            if (n < 1)
                n = 1;
            if ((n > list.Count && !currDControl.loop) || (list.Count <= currDControl.rowNum && n > list.Count))
                n = list.Count;
            turnImageWhich = n;


            int count = listbox1.Children.Count;
            if (count <= 0) { return; }



            //到达第一个时，返回第一个 （导航圆点）
            //int bx =   (n-1) % list.Count; 
            //if (bx < 0) bx = 0;
            Button item1 = (Button)marque_stackPanel.Children[n - 1];
            MarqueItemTag tag1 = (MarqueItemTag)item1.Tag;


            Set_Button_Active(pagePanel.Children[tag1.idx], System.Drawing.Brushes.Black);
            loadImage(n - 1);
            // this.showArrow(this.currDControl);



            storyboard.Children.Clear();
            int i = 0;
            foreach (Button item in marque_stackPanel.Children)
            {
                i = i + 1;

                int index = 1;
                if (i <= n)
                {
                    index = i;
                }
                else
                {
                    index = n - (i - n);
                }
                Panel.SetZIndex(item, index);

                TransformGroup group = (TransformGroup)item.RenderTransform;
                ScaleTransform scaleTransform = (ScaleTransform)group.Children[0];
                TranslateTransform translateTransform = (TranslateTransform)group.Children[1];
                MarqueItemTag tag = (MarqueItemTag)item.Tag;

                if (Math.Abs(n - i) < middleIdx || n - i == middleIdx)
                {
                    double toScale = 1 - Math.Abs(n - i) * 0.2;

                    DoubleAnimation doubleAnimation1 = new DoubleAnimation(scaleTransform.ScaleX, toScale, new Duration(TimeSpan.FromMilliseconds(500.0)));
                    doubleAnimation1.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
                    doubleAnimation1.FillBehavior = FillBehavior.Stop;
                    doubleAnimation1.Completed += (sender, e) => ScaleX_Completed_Actived(sender, e, item, toScale);
                    Storyboard.SetTarget(doubleAnimation1, item);
                    Storyboard.SetTargetProperty(doubleAnimation1, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleX)"));
                    storyboard.Children.Add(doubleAnimation1);


                    DoubleAnimation doubleAnimation2 = new DoubleAnimation(scaleTransform.ScaleY, toScale, new Duration(TimeSpan.FromMilliseconds(500.0)));
                    doubleAnimation2.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
                    doubleAnimation2.FillBehavior = FillBehavior.Stop;
                    doubleAnimation2.Completed += (sender, e) => ScaleY_Completed_Actived(sender, e, item, toScale);
                    Storyboard.SetTarget(doubleAnimation2, item);
                    Storyboard.SetTargetProperty(doubleAnimation2, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)"));
                    storyboard.Children.Add(doubleAnimation2);

                    //偏移
                    double scale = 1 - Math.Abs(i - turnImageWhich) * 0.2;
                    if (scale < 0.2)
                    {
                        scale = 0.2;
                    }
                    double translateX = tag.translateX;
                    if (n - i < middleIdx && n - i > 0)
                    {
                        //7,4


                        double p = 0;
                        if (n - i == middleIdx - 1)
                        {
                            p = 0;
                        }
                        else if (n - i == 1)
                        {
                            p = itemWidth * 0.5 * 0.3;
                        }
                        else if (n - i > 1)
                        {   //0,25%,50%
                            double half2 = itemWidth * 0.5 * 0.3;
                            if (middleIdx - 2 > 0)
                            {
                                p = half2 / (middleIdx - 2) * Math.Abs(n - i - 1);
                            }
                        }



                        translateX = -itemWidth * Math.Abs(i - 1) - (1 - scale) * itemWidth * 0.5 + p;


                    }

                    if (n - i == 0)
                    {
                        translateX = -itemWidth * Math.Abs(i - 1) + itemWidth * 0.5;
                    }

                    if (n - i < 0)
                    {
                        //最后一个偏移0，第一个偏移50%
                        double p = 0;
                        if (n - i == -middleIdx + 1)
                        {
                            p = 0;
                        }
                        else if (n - i == -1)
                        {
                            //中间左侧第一个占0.7
                            p = itemWidth * 0.5 * 0.3;
                        }
                        else
                        {
                            double half2 = itemWidth * 0.5 * 0.3;
                            if (middleIdx - 2 > 0)
                            {
                                p = half2 / (middleIdx - 2) * Math.Abs(n - i + 1);
                            }
                        }
                        double p2 = itemWidth * 2 - scale * itemWidth - p;
                        translateX = -itemWidth * Math.Abs(i - 1) - (1 - scale) * itemWidth * 0.5 + p2;

                    }


                    double startTranslateX = translateTransform.X;


                    //左边将要消失的那个
                    if (n - i == middleIdx)
                    {
                        translateX = translateTransform.X;
                    }




                    //最后一个
                    if (percent >= 0 && (n - i == -middleIdx + 1))
                    {

                        // translateX = -itemWidth * Math.Abs(i - 1) - (1 - scale) * itemWidth * 0.5 + itemWidth * 2 - scale * itemWidth;
                        translateTransform.X = translateX;

                        //DoubleAnimation doubleAnimation31 = new DoubleAnimation(startTranslateX, translateX, new Duration(TimeSpan.FromMilliseconds(1.0)));
                        //doubleAnimation31.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
                        //doubleAnimation31.FillBehavior = FillBehavior.Stop;
                        //doubleAnimation31.Completed += (sender, e) => All_Completed_Actived(sender, e, item, translateX);
                        //Storyboard.SetTarget(doubleAnimation31, item);
                        //Storyboard.SetTargetProperty(doubleAnimation31, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[1].(TranslateTransform.X)"));
                        //storyboard.Children.Add(doubleAnimation31);
                    }
                    else
                    {


                        DoubleAnimation doubleAnimation3 = new DoubleAnimation(startTranslateX, translateX, new Duration(TimeSpan.FromMilliseconds(500.0)));
                        doubleAnimation3.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
                        doubleAnimation3.FillBehavior = FillBehavior.Stop;
                        doubleAnimation3.Completed += (sender, e) => All_Completed_Actived(sender, e, item, translateX);
                        Storyboard.SetTarget(doubleAnimation3, item);
                        Storyboard.SetTargetProperty(doubleAnimation3, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[1].(TranslateTransform.X)"));
                        storyboard.Children.Add(doubleAnimation3);
                        //  item.Opacity = 1.0;
                    }
                    DoubleAnimation doubleAnimation5 = new DoubleAnimation(item.Opacity, 1.0, new Duration(TimeSpan.FromMilliseconds(500.0)));
                    doubleAnimation5.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
                    doubleAnimation5.FillBehavior = FillBehavior.Stop;
                    doubleAnimation5.Completed += (sender, e) => doubleAnimation3_Completed(sender, e, item, 1.0);
                    Storyboard.SetTarget(doubleAnimation5, item);
                    Storyboard.SetTargetProperty(doubleAnimation5, new PropertyPath(Button.OpacityProperty));
                    storyboard.Children.Add(doubleAnimation5);
                    continue;

                }

                //scale = scale - 0.2;
                //if (scale < 0.2)
                //{
                //    scale = 0.2;
                //}
                double minScale = 1 - middleIdx * 0.2;
                if (minScale < 0.2)
                {
                    minScale = 0.2;
                }
                DoubleAnimation doubleAnimation6 = new DoubleAnimation(scaleTransform.ScaleX, minScale, new Duration(TimeSpan.FromMilliseconds(500.0)));
                //doubleAnimation6.BeginTime = TimeSpan.FromMilliseconds(0.0);
                doubleAnimation6.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
                doubleAnimation6.FillBehavior = FillBehavior.Stop;
                doubleAnimation6.Completed += (sender, e) => ScaleX_Completed_Actived(sender, e, item, minScale);
                Storyboard.SetTarget(doubleAnimation6, item);
                Storyboard.SetTargetProperty(doubleAnimation6, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleX)"));
                storyboard.Children.Add(doubleAnimation6);


                DoubleAnimation doubleAnimation7 = new DoubleAnimation(scaleTransform.ScaleY, minScale, new Duration(TimeSpan.FromMilliseconds(500.0)));
                //doubleAnimation7.BeginTime = TimeSpan.FromMilliseconds(0.0);
                doubleAnimation7.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
                doubleAnimation7.FillBehavior = FillBehavior.Stop;
                doubleAnimation7.Completed += (sender, e) => ScaleY_Completed_Actived(sender, e, item, minScale);
                Storyboard.SetTarget(doubleAnimation7, item);
                Storyboard.SetTargetProperty(doubleAnimation7, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)"));
                storyboard.Children.Add(doubleAnimation7);

                DoubleAnimation doubleAnimation4 = new DoubleAnimation(item.Opacity, 0, new Duration(TimeSpan.FromMilliseconds(500.0)));
                doubleAnimation4.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
                doubleAnimation4.FillBehavior = FillBehavior.Stop;
                doubleAnimation4.Completed += (sender, e) => doubleAnimation3_Completed(sender, e, item, 0);
                Storyboard.SetTarget(doubleAnimation4, item);
                Storyboard.SetTargetProperty(doubleAnimation4, new PropertyPath(Button.OpacityProperty));
                storyboard.Children.Add(doubleAnimation4);



            }

            // 创建第2个动画
            // 
            if (n <= middleIdx + 1 && list.Count > currDControl.rowNum && currDControl.loop)
            {
                //
                // Console.WriteLine("回到开始");

                //this.turnImageWhich = this.middleIdx + currDControl.rowNum + 1;

                turnImageWhich = list.Count + n;
                i = 0;
                int n2 = turnImageWhich;
                foreach (Button item in marque_stackPanel.Children)
                {
                    i = i + 1;

                    if (Math.Abs(n2 - i) < middleIdx)
                    {
                        TransformGroup group = (TransformGroup)item.RenderTransform;
                        ScaleTransform scaleTransform = (ScaleTransform)group.Children[0];
                        double toScale = 1 - Math.Abs(n2 - i) * 0.2;


                        //scaleTransform.ScaleX = toScale;
                        //scaleTransform.ScaleY = toScale;
                        //item.RenderTransform = group;
                        // int idx = Panel.GetZIndex(item); 
                        int index = 1;
                        if (i <= n2)
                        {
                            index = i;
                        }
                        else
                        {
                            index = n2 - (i - n2);
                        }
                        int idx = Panel.GetZIndex(item);
                        Panel.SetZIndex(item, index);


                        DoubleAnimation doubleAnimation11 = new DoubleAnimation(scaleTransform.ScaleX, toScale, new Duration(TimeSpan.FromMilliseconds(500)));
                        doubleAnimation11.BeginTime = TimeSpan.FromMilliseconds(0.0);
                        doubleAnimation11.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
                        doubleAnimation11.FillBehavior = FillBehavior.Stop;
                        doubleAnimation11.Completed += (sender, e) => ScaleX_Completed_Actived(sender, e, item, toScale);
                        Storyboard.SetTarget(doubleAnimation11, item);
                        Storyboard.SetTargetProperty(doubleAnimation11, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleX)"));
                        storyboard.Children.Add(doubleAnimation11);


                        DoubleAnimation doubleAnimation12 = new DoubleAnimation(scaleTransform.ScaleY, toScale, new Duration(TimeSpan.FromMilliseconds(500)));
                        doubleAnimation12.BeginTime = TimeSpan.FromMilliseconds(0.0);
                        doubleAnimation12.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
                        doubleAnimation12.FillBehavior = FillBehavior.Stop;
                        doubleAnimation12.Completed += (sender, e) => ScaleY_Completed_Actived(sender, e, item, toScale);
                        Storyboard.SetTarget(doubleAnimation12, item);
                        Storyboard.SetTargetProperty(doubleAnimation12, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)"));
                        storyboard.Children.Add(doubleAnimation12);


                        double translateX = 0;
                        if (n2 - i < middleIdx && n2 - i > 0)
                        {
                            //7,4

                            double p = 0;
                            if (n2 - i == middleIdx - 1)
                            {
                                p = 0;
                            }
                            else if (n2 - i == 1)
                            {
                                p = itemWidth * 0.5 * 0.3;
                            }
                            else if (i > 1)
                            {   //0,25%,50%
                                double half2 = itemWidth * 0.5 * 0.3;
                                if (middleIdx - 2 > 0)
                                {
                                    p = half2 / (middleIdx - 2) * Math.Abs(n2 - i - 1);
                                }
                            }

                            translateX = -itemWidth * Math.Abs(i - 1) - (1 - toScale) * itemWidth * 0.5 + p;
                        }

                        if (n2 - i == 0)
                        {
                            translateX = -itemWidth * Math.Abs(i - 1) + itemWidth * 0.5;
                        }

                        if (n2 - i < 0)
                        {
                            //最后一个偏移0，第一个偏移50%
                            double p = 0;
                            if (n2 - i == -middleIdx + 1)
                            {
                                p = 0;
                            }
                            else if (n2 - i == -1)
                            {
                                p = itemWidth * 0.5 * 0.3;
                            }

                            else
                            {
                                //50% 33.6% 16.6% 0
                                //     7     8    9
                                double half2 = itemWidth * 0.5 * 0.3;
                                if (middleIdx - 2 > 0)
                                {
                                    p = half2 / (middleIdx - 2) * Math.Abs(n2 - i + 1);
                                }
                            }
                            double p2 = itemWidth * 2 - toScale * itemWidth - p;
                            translateX = -itemWidth * Math.Abs(i - 1) - (1 - toScale) * itemWidth * 0.5 + p2;
                        }
                        //moudown，动画提前结束

                        TransformGroup group2 = (TransformGroup)item.RenderTransform;
                        TranslateTransform translateTransform = (TranslateTransform)group2.Children[1];
                        double x = translateTransform.X;
                        // translateTransform.X = translateX;



                        DoubleAnimation doubleAnimation3 = new DoubleAnimation(x, translateX, new Duration(TimeSpan.FromMilliseconds(500)));
                        doubleAnimation3.BeginTime = TimeSpan.FromMilliseconds(0);
                        doubleAnimation3.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
                        doubleAnimation3.FillBehavior = FillBehavior.Stop;
                        doubleAnimation3.Completed += (sender, e) => TranslateX_Completed_Actived(sender, e, item, translateX);
                        Storyboard.SetTarget(doubleAnimation3, item);
                        Storyboard.SetTargetProperty(doubleAnimation3, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[1].(TranslateTransform.X)"));
                        storyboard.Children.Add(doubleAnimation3);
                        //item.Opacity = 1;

                        DoubleAnimation doubleAnimation4 = new DoubleAnimation(item.Opacity, 1, new Duration(TimeSpan.FromMilliseconds(0)));
                        doubleAnimation4.BeginTime = TimeSpan.FromMilliseconds(500.0);
                        doubleAnimation4.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
                        doubleAnimation4.FillBehavior = FillBehavior.Stop;
                        doubleAnimation4.Completed += (sender, e) => doubleAnimation3_Completed(sender, e, item, 1);
                        Storyboard.SetTarget(doubleAnimation4, item);
                        Storyboard.SetTargetProperty(doubleAnimation4, new PropertyPath(Button.OpacityProperty));
                        storyboard.Children.Add(doubleAnimation4);

                    }
                    else
                    {
                        //item.Opacity = 0;
                        //TransformGroup group2 = (TransformGroup)item.RenderTransform;
                        //TranslateTransform translateTransform = (TranslateTransform)group2.Children[1];
                        //translateTransform.X = 0;

                        //DoubleAnimation doubleAnimation2 = new DoubleAnimation(translateTransform.X, i * itemWidth, new Duration(TimeSpan.FromMilliseconds(0)));
                        //doubleAnimation2.BeginTime = TimeSpan.FromMilliseconds(500.0);
                        //doubleAnimation2.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
                        //Storyboard.SetTarget(doubleAnimation2, item);
                        //Storyboard.SetTargetProperty(doubleAnimation2, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[1].(TranslateTransform.X)"));
                        //storyboard.Children.Add(doubleAnimation2); 

                        DoubleAnimation doubleAnimation3 = new DoubleAnimation(item.Opacity, 0, new Duration(TimeSpan.FromMilliseconds(1.0)));
                        doubleAnimation3.BeginTime = TimeSpan.FromMilliseconds(0);
                        doubleAnimation3.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
                        doubleAnimation3.FillBehavior = FillBehavior.Stop;
                        doubleAnimation3.Completed += (sender, e) => doubleAnimation3_Completed(sender, e, item, 0);
                        Storyboard.SetTarget(doubleAnimation3, item);
                        Storyboard.SetTargetProperty(doubleAnimation3, new PropertyPath(Button.OpacityProperty));
                        storyboard.Children.Add(doubleAnimation3);
                        // item.Opacity = 0.0;

                        // Console.WriteLine("opacity0:"+i);
                    }

                }


                //DoubleAnimation doubleAnimation2 = new DoubleAnimation(marque_scrollViewer.HorizontalOffset, this.initOffsetLeft, new Duration(TimeSpan.FromMilliseconds(0)));
                //doubleAnimation2.BeginTime = TimeSpan.FromMilliseconds(500.0);
                //Storyboard.SetTarget(doubleAnimation2, marque_scrollViewer);
                //Storyboard.SetTargetProperty(doubleAnimation2, new PropertyPath(ScrollViewerProperty.HorizontalOffsetProperty));
                //storyboard.Children.Add(doubleAnimation2);
            }


            // 创建第3个动画
            if (n >= list.Count + middleIdx + currDControl.rowNum + 1 && currDControl.loop)
            {
                // Console.WriteLine("回到开始");
                // this.turnImageWhich = this.middleIdx + currDControl.rowNum +1;
                turnImageWhich = n - list.Count;
                i = 0;
                int n2 = turnImageWhich;
                foreach (Button item in marque_stackPanel.Children)
                {
                    i = i + 1;

                    if (Math.Abs(n2 - i) < middleIdx)
                    {
                        TransformGroup group = (TransformGroup)item.RenderTransform;
                        ScaleTransform scaleTransform = (ScaleTransform)group.Children[0];
                        double toScale = 1 - Math.Abs(n2 - i) * 0.2;


                        //scaleTransform.ScaleX = toScale;
                        //scaleTransform.ScaleY = toScale;
                        //item.RenderTransform = group;
                        // int idx = Panel.GetZIndex(item); 
                        int index = 1;
                        if (i <= n2)
                        {
                            index = i;
                        }
                        else
                        {
                            index = n2 - (i - n2);
                        }
                        int idx = Panel.GetZIndex(item);
                        Panel.SetZIndex(item, index);


                        DoubleAnimation doubleAnimation11 = new DoubleAnimation(scaleTransform.ScaleX, toScale, new Duration(TimeSpan.FromMilliseconds(500)));
                        doubleAnimation11.BeginTime = TimeSpan.FromMilliseconds(0.0);
                        doubleAnimation11.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
                        doubleAnimation11.FillBehavior = FillBehavior.Stop;
                        doubleAnimation11.Completed += (sender, e) => ScaleX_Completed_Actived(sender, e, item, toScale);
                        Storyboard.SetTarget(doubleAnimation11, item);
                        Storyboard.SetTargetProperty(doubleAnimation11, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleX)"));
                        storyboard.Children.Add(doubleAnimation11);


                        DoubleAnimation doubleAnimation12 = new DoubleAnimation(scaleTransform.ScaleY, toScale, new Duration(TimeSpan.FromMilliseconds(500)));
                        doubleAnimation12.BeginTime = TimeSpan.FromMilliseconds(0.0);
                        doubleAnimation12.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
                        doubleAnimation12.FillBehavior = FillBehavior.Stop;
                        doubleAnimation12.Completed += (sender, e) => ScaleY_Completed_Actived(sender, e, item, toScale);
                        Storyboard.SetTarget(doubleAnimation12, item);
                        Storyboard.SetTargetProperty(doubleAnimation12, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)"));
                        storyboard.Children.Add(doubleAnimation12);


                        double translateX = 0;
                        if (n2 - i < middleIdx && n2 - i > 0)
                        {
                            //7,4

                            double p = 0;
                            if (n2 - i == middleIdx - 1)
                            {
                                p = 0;
                            }
                            else if (n2 - i == 1)
                            {
                                p = itemWidth * 0.5 * 0.3;
                            }
                            else if (i > 1)
                            {   //0,25%,50%
                                double half2 = itemWidth * 0.5 * 0.3;
                                if (middleIdx - 2 > 0)
                                {
                                    p = half2 / (middleIdx - 2) * Math.Abs(n2 - i - 1);
                                }
                            }

                            translateX = -itemWidth * Math.Abs(i - 1) - (1 - toScale) * itemWidth * 0.5 + p;
                        }

                        if (n2 - i == 0)
                        {
                            translateX = -itemWidth * Math.Abs(i - 1) + itemWidth * 0.5;
                        }

                        if (n2 - i < 0)
                        {
                            //最后一个偏移0，第一个偏移50%
                            double p = 0;
                            if (n2 - i == -middleIdx + 1)
                            {
                                p = 0;
                            }
                            else if (n2 - i == -1)
                            {
                                p = itemWidth * 0.5 * 0.3;
                            }

                            else
                            {
                                //50% 33.6% 16.6% 0
                                //     7     8    9
                                double half2 = itemWidth * 0.5 * 0.3;
                                if (middleIdx - 2 > 0)
                                {
                                    p = half2 / (middleIdx - 2) * Math.Abs(n2 - i + 1);
                                }
                            }
                            double p2 = itemWidth * 2 - toScale * itemWidth - p;
                            translateX = -itemWidth * Math.Abs(i - 1) - (1 - toScale) * itemWidth * 0.5 + p2;
                        }
                        //moudown，动画提前结束

                        TransformGroup group2 = (TransformGroup)item.RenderTransform;
                        TranslateTransform translateTransform = (TranslateTransform)group2.Children[1];
                        double x = translateTransform.X;
                        // translateTransform.X = translateX;



                        DoubleAnimation doubleAnimation3 = new DoubleAnimation(x, translateX, new Duration(TimeSpan.FromMilliseconds(500)));
                        doubleAnimation3.BeginTime = TimeSpan.FromMilliseconds(0);
                        doubleAnimation3.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
                        doubleAnimation3.FillBehavior = FillBehavior.Stop;
                        doubleAnimation3.Completed += (sender, e) => TranslateX_Completed_Actived(sender, e, item, translateX);
                        Storyboard.SetTarget(doubleAnimation3, item);
                        Storyboard.SetTargetProperty(doubleAnimation3, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[1].(TranslateTransform.X)"));
                        storyboard.Children.Add(doubleAnimation3);
                        //item.Opacity = 1;

                        DoubleAnimation doubleAnimation4 = new DoubleAnimation(item.Opacity, 1, new Duration(TimeSpan.FromMilliseconds(0)));
                        doubleAnimation4.BeginTime = TimeSpan.FromMilliseconds(500.0);
                        doubleAnimation4.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
                        doubleAnimation4.FillBehavior = FillBehavior.Stop;
                        doubleAnimation4.Completed += (sender, e) => doubleAnimation3_Completed(sender, e, item, 1);
                        Storyboard.SetTarget(doubleAnimation4, item);
                        Storyboard.SetTargetProperty(doubleAnimation4, new PropertyPath(Button.OpacityProperty));
                        storyboard.Children.Add(doubleAnimation4);

                    }
                    else
                    {
                        //item.Opacity = 0;
                        //TransformGroup group2 = (TransformGroup)item.RenderTransform;
                        //TranslateTransform translateTransform = (TranslateTransform)group2.Children[1];
                        //translateTransform.X = 0;

                        //DoubleAnimation doubleAnimation2 = new DoubleAnimation(translateTransform.X, i * itemWidth, new Duration(TimeSpan.FromMilliseconds(0)));
                        //doubleAnimation2.BeginTime = TimeSpan.FromMilliseconds(500.0);
                        //doubleAnimation2.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
                        //Storyboard.SetTarget(doubleAnimation2, item);
                        //Storyboard.SetTargetProperty(doubleAnimation2, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[1].(TranslateTransform.X)"));
                        //storyboard.Children.Add(doubleAnimation2); 

                        DoubleAnimation doubleAnimation3 = new DoubleAnimation(item.Opacity, 0, new Duration(TimeSpan.FromMilliseconds(1.0)));
                        doubleAnimation3.BeginTime = TimeSpan.FromMilliseconds(0);
                        doubleAnimation3.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
                        doubleAnimation3.FillBehavior = FillBehavior.Stop;
                        doubleAnimation3.Completed += (sender, e) => doubleAnimation3_Completed(sender, e, item, 0);
                        Storyboard.SetTarget(doubleAnimation3, item);
                        Storyboard.SetTargetProperty(doubleAnimation3, new PropertyPath(Button.OpacityProperty));
                        storyboard.Children.Add(doubleAnimation3);
                        // item.Opacity = 0.0;

                        // Console.WriteLine("opacity0:"+i);
                    }

                }


                //DoubleAnimation doubleAnimation2 = new DoubleAnimation(marque_scrollViewer.HorizontalOffset, this.initOffsetLeft, new Duration(TimeSpan.FromMilliseconds(0)));
                //doubleAnimation2.BeginTime = TimeSpan.FromMilliseconds(500.0);
                //Storyboard.SetTarget(doubleAnimation2, marque_scrollViewer);
                //Storyboard.SetTargetProperty(doubleAnimation2, new PropertyPath(ScrollViewerProperty.HorizontalOffsetProperty));
                //storyboard.Children.Add(doubleAnimation2);
            }


            storyboard.Begin();
            turnImage_Timer.Start();
        }



        private async void loadImage(int n)
        {
            int count = marque_stackPanel.Children.Count;
            if (count <= 0) { return; }

            try
            {
                int n2 = list.Count + middleIdx + 1;  //向左滑动，回到中间
                int n3 = middleIdx + currDControl.rowNum + 1;  //向右滑动，回到中间


                for (int a = 0; a < count; a++)
                {
                    Button item = (Button)marque_stackPanel.Children[a];
                    //左右，第一个，最后一个，第二个

                    if ((a >= n - middleIdx && a <= n + middleIdx) || (a >= n2 - middleIdx && a <= n2 + middleIdx) || (a >= n3 - middleIdx && a <= n3 + middleIdx))
                    {

                        if (item.Background == null)
                        {
                            MarqueItemTag tag = (MarqueItemTag)item.Tag;
                            int x = int.Parse(tag.idx.ToString());

                            string imgUrl = AppDomain.CurrentDomain.BaseDirectory + list[x].url;
                            // Console.WriteLine(n + "__" + a);
                            //Border border = FrameworkElementUtil.GetChildObject<Border>(item, "Loading");
                            //if (border != null)
                            //{
                            //    border.Visibility = Visibility.Visible;
                            //}

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
                            //if (border != null)
                            //{
                            //    border.Visibility = Visibility.Hidden;
                            //}
                        }

                    }
                    else
                    {

                        item.Background = null;
                        //  Console.WriteLine("清空" + n + "__" + a);
                    }

                }


                // bigImage.Source = source;
            }
            catch { }
            GC.Collect();
        }
        private void All_Completed_Actived(object sender, EventArgs e, Button item, double translateX)
        {
            TransformGroup group2 = (TransformGroup)item.RenderTransform;
            TranslateTransform translateTransform = (TranslateTransform)group2.Children[1];
            translateTransform.X = translateX;
            item.RenderTransform = group2;
            All_Animation_Completed = true;
        }
        private void TranslateX_Completed_Actived(object sender, EventArgs e, Button item, double translateX)
        {
            TransformGroup group2 = (TransformGroup)item.RenderTransform;
            TranslateTransform translateTransform = (TranslateTransform)group2.Children[1];
            translateTransform.X = translateX;
            item.RenderTransform = group2;
        }

        private void ScaleX_Completed_Actived(object sender, EventArgs e, Button item, double toScale)
        {
            TransformGroup group = (TransformGroup)item.RenderTransform;
            ScaleTransform scaleTransform = (ScaleTransform)group.Children[0];
            scaleTransform.ScaleX = toScale;
            item.RenderTransform = group;

        }
        private void ScaleY_Completed_Actived(object sender, EventArgs e, Button item, double toScale)
        {
            TransformGroup group = (TransformGroup)item.RenderTransform;
            ScaleTransform scaleTransform = (ScaleTransform)group.Children[0];
            scaleTransform.ScaleY = toScale;
            item.RenderTransform = group;

        }

        private void doubleAnimation3_Completed(object sender, EventArgs e, Button item, double opacity)
        {
            item.Opacity = opacity;
            // Console.WriteLine("完成");
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
            //itemWidth = (int)(currDControl.width - currDControl.spacing * (currDControl.rowNum - 1)) / currDControl.rowNum;
            marque_scrollViewer.Width = Width;
            marque_scrollViewer.Height = Height;
            //foreach(Button item in marque_stackPanel.Children)
            //{
            //    item.Width = itemWidth;
            //    item.Height = this.Height; 
            //}
            currDControl.width = Convert.ToInt32(Width);
            currDControl.height = Convert.ToInt32(Height);

            itemWidth = (int)currDControl.width / 2.0;
            updateItemSizeAndPosition(currDControl);

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
            //if (list.Count == 0) return;
            //double offsetLeft = marque_scrollViewer.HorizontalOffset;
            //double fullWidth = marque_stackPanel.ActualWidth - this.Width;
            //if (offsetLeft >= fullWidth)
            //{ 
            //    turnImage_Timer.Start(); 
            //} 
        }

    }
}