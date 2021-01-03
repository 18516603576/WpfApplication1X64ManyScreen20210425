using Bll;
using Common.util;
using Model;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

/*
 * 编辑状态
 * 
 * 1.鼠标按下控件获得焦点
 * 2.移动控件
 * 3.空白处点击取消较低那
 */
namespace WpfApplication1.manage
{

    public partial class Editing
    {
        //控件鼠标是否按下
        private Boolean control_MouseDown_Flag = false;
        //控件鼠标按下时的坐标
        private Point control_MouseDown_Point;
        //编辑框鼠标是否按下
        private Boolean editingBorder_MouseDown_Flag = false;
        //编辑框鼠标按下时的坐标
        private Point editingBorder_MouseDown_Point;
        //8个点鼠标是否按下
        private Boolean point8_MouseDown_Flag = false;
        //8个点鼠标按下时的坐标
        private Point point8_MouseDown_Point;
        //8个点的宽度,必须是单数
        public double pointWidth = 13;
        //编辑框
        public Border editingBorder;
        //右击的当前控件
        public FrameworkElement rightClickEle;
        //页面空白处右击位置
        public Point rightClickLocation;

        private readonly StorageImageBll storageImageBll = new StorageImageBll();
        private readonly DControlBll dControlBll = new DControlBll();
        private readonly TurnPictureImagesBll turnPictureImagesBll = new TurnPictureImagesBll();
        private readonly DControlAnimationBll dControlAnimationBll = new DControlAnimationBll();

        private readonly Frame mainFrame = null;
        private readonly PageTemplate pageTemplate = null;


        public Editing(Frame mainFrame, PageTemplate pageTemplate)
        {
            this.mainFrame = mainFrame;
            this.pageTemplate = pageTemplate;


            InsertToPage();
            RightContextMenu();

            //页面空白处单击释放
            pageTemplate.MouseLeftButtonUp += Page_MouseLeftButtonUp;
        }

        /*
        * 让当前控件获得焦点
        */
        private Border control_GetFocus(object sender, MouseButtonEventArgs e)
        {
            pageTemplate.container.Children.Remove(editingBorder);
            FrameworkElement control = (FrameworkElement)sender;
            DControl dControl = (DControl)control.Tag;
            SolidColorBrush borderColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF109CEC"));


            //point宽度的一般
            double bhalf = Math.Floor(pointWidth / 2);
            //point水平居中坐标
            double centerX = (control.Width - pointWidth) / 2;
            //point垂直居中坐标
            double centerY = (control.Height - pointWidth) / 2;


            //1左上角
            Border border1 = new Border();
            border1.Name = "leftTopBorder";
            border1.Width = pointWidth;
            border1.Height = pointWidth;
            // border1.Margin = new Thickness(-bhalf,-bhalf,0,0);
            border1.BorderThickness = new Thickness(1);
            border1.BorderBrush = borderColor;
            border1.Background = Brushes.White;
            border1.SetValue(Canvas.LeftProperty, -bhalf);
            border1.SetValue(Canvas.TopProperty, -bhalf);
            //2右上角
            Border border2 = new Border();
            border2.Name = "rightTopBorder";
            border2.Width = pointWidth;
            border2.Height = pointWidth;
            // border2.Margin = new Thickness(0, -bhalf, -bhalf, 0);
            border2.BorderThickness = new Thickness(1);
            border2.BorderBrush = borderColor;
            border2.Background = Brushes.White;
            border2.SetValue(Canvas.TopProperty, -bhalf);
            border2.SetValue(Canvas.RightProperty, -bhalf);

            //3右下角
            Border border3 = new Border();
            border3.Name = "rightBottomBorder";
            border3.Width = pointWidth;
            border3.Height = pointWidth;
            // border3.Margin = new Thickness(0, 0, -bhalf, -bhalf);
            border3.BorderThickness = new Thickness(1);
            border3.BorderBrush = borderColor;
            border3.Background = Brushes.White;
            border3.SetValue(Canvas.RightProperty, -bhalf);
            border3.SetValue(Canvas.BottomProperty, -bhalf);
            border3.MouseEnter += border3_MouseEnter;
            border3.MouseLeave += border3_MouseLeave;
            border3.PreviewMouseDown += border3_MouseDown;
            border3.PreviewMouseMove += border3_MouseMove;
            border3.PreviewMouseUp += border3_MouseUp;

            //4左下角
            Border border4 = new Border();
            border4.Name = "leftBottomBorder";
            border4.Width = pointWidth;
            border4.Height = pointWidth;
            // border4.Margin = new Thickness(-bhalf, 0, 0, -bhalf);
            border4.BorderThickness = new Thickness(1);
            border4.BorderBrush = borderColor;
            border4.Background = Brushes.White;
            border4.SetValue(Canvas.LeftProperty, -bhalf);
            border4.SetValue(Canvas.BottomProperty, -bhalf);

            //5左中角
            Border border5 = new Border();
            border5.Name = "leftCenterBorder";
            border5.Width = pointWidth;
            border5.Height = pointWidth;
            // border5.Margin = new Thickness(-bhalf, centerY, 0, 0);
            border5.BorderThickness = new Thickness(1);
            border5.BorderBrush = borderColor;
            border5.Background = Brushes.White;
            border5.SetValue(Canvas.LeftProperty, -bhalf);
            border5.SetValue(Canvas.TopProperty, centerY);

            //6上中角
            Border border6 = new Border();
            border6.Name = "topCenterBorder";
            border6.Width = pointWidth;
            border6.Height = pointWidth;
            // border6.Margin = new Thickness(centerX, -bhalf, 0, 0);
            border6.BorderThickness = new Thickness(1);
            border6.BorderBrush = borderColor;
            border6.Background = Brushes.White;
            border6.SetValue(Canvas.LeftProperty, centerX);
            border6.SetValue(Canvas.TopProperty, -bhalf);

            //7右中角
            Border border7 = new Border();
            border7.Name = "rightCenterBorder";
            border7.Width = pointWidth;
            border7.Height = pointWidth;
            //   border7.Margin = new Thickness(0, centerY, -bhalf, 0);
            border7.BorderThickness = new Thickness(1);
            border7.BorderBrush = borderColor;
            border7.Background = Brushes.White;
            border7.SetValue(Canvas.RightProperty, -bhalf);
            border7.SetValue(Canvas.TopProperty, centerY);

            //8下中角
            Border border8 = new Border();
            border8.Name = "bottomCenterBorder";
            border8.Width = pointWidth;
            border8.Height = pointWidth;
            //  border8.Margin = new Thickness(centerX, 0, 0, -bhalf);
            border8.BorderThickness = new Thickness(1);
            border8.BorderBrush = borderColor;
            border8.Background = Brushes.White;
            border8.SetValue(Canvas.LeftProperty, centerX);
            border8.SetValue(Canvas.BottomProperty, -bhalf);


            Canvas editingCanvas = new Canvas();
            editingCanvas.Name = "editingCanvas";
            editingCanvas.Width = control.Width;
            editingCanvas.Height = control.Height;
            editingCanvas.VerticalAlignment = VerticalAlignment.Top;
            editingCanvas.HorizontalAlignment = HorizontalAlignment.Left;
            editingCanvas.Margin = new Thickness(-1, -1, 0, 0);
            editingCanvas.Children.Add(border1);
            editingCanvas.Children.Add(border2);
            editingCanvas.Children.Add(border3);
            editingCanvas.Children.Add(border4);
            editingCanvas.Children.Add(border5);
            editingCanvas.Children.Add(border6);
            editingCanvas.Children.Add(border7);
            editingCanvas.Children.Add(border8);


            editingBorder = new Border();
            editingBorder.Name = "editingBorder";
            editingBorder.Width = control.Width;
            editingBorder.Height = control.Height;
            editingBorder.Margin = control.Margin;
            editingBorder.BorderThickness = new Thickness(1);
            editingBorder.BorderBrush = borderColor;
            editingBorder.Background = Brushes.Transparent;
            editingBorder.VerticalAlignment = VerticalAlignment.Top;
            editingBorder.HorizontalAlignment = HorizontalAlignment.Left;
            Grid.SetZIndex(editingBorder, 10001);
            editingBorder.Child = editingCanvas;
            editingBorder.Tag = control;
            // control.Tag = editingBorder;
            TransformGroup group = new TransformGroup();
            RotateTransform rotateTransform = TransformGroupUtil.GetRotateTransform(group);
            rotateTransform.Angle = dControl.rotateAngle;
            editingBorder.RenderTransform = group;
            editingBorder.RenderTransformOrigin = new Point(0.5, 0.5);


            editingBorder.MouseLeftButtonDown += editingBorder_MouseDown;
            editingBorder.MouseMove += editingBorder_MouseMove;
            editingBorder.MouseLeftButtonUp += editingBorder_MouseUp;
            //editingBorder.PreviewTouchDown += editingBorder_PreviewTouchDown;
            //editingBorder.TouchMove += editingBorder_TouchMove;
            //editingBorder.PreviewTouchUp += editingBorder_PreviewTouchUp;
            //右击控件 
            editingBorder.MouseRightButtonUp += control_MouseRightButtonUp;


            pageTemplate.container.Children.Add(editingBorder);
            return editingBorder;
        }












        /*
         * 修改编辑框宽高位置
         *  修改8个点的位置  
         */
        public void updateEditingBorder(DControl ctl)
        {
            //外框宽高位置
            editingBorder.Width = ctl.width;
            editingBorder.Height = ctl.height;
            editingBorder.Margin = new Thickness(ctl.left, ctl.top, 0, 0);

            //8个点的位置 
            Canvas editingCanvas = (Canvas)editingBorder.Child;
            editingCanvas.Width = editingBorder.Width;
            editingCanvas.Height = editingBorder.Height;
            foreach (UIElement ele in editingCanvas.Children)
            {
                Border b = (Border)ele;
                if (b.Name == "leftCenterBorder")
                {
                    b.SetValue(Canvas.TopProperty, (editingCanvas.Height - pointWidth) / 2);
                }
                else if (b.Name == "topCenterBorder")
                {
                    b.SetValue(Canvas.LeftProperty, (editingCanvas.Width - pointWidth) / 2);
                }
                else if (b.Name == "rightCenterBorder")
                {
                    b.SetValue(Canvas.TopProperty, (editingCanvas.Height - pointWidth) / 2);
                }
                else if (b.Name == "bottomCenterBorder")
                {
                    b.SetValue(Canvas.LeftProperty, (editingCanvas.Width - pointWidth) / 2);
                }
            }

        }



        /*
         * 控件鼠标按下，获得焦点
         */
        public void control_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //编辑框存在，并且是当前控件
            bool isEditingCurr = false;
            if (editingBorder != null)
            {
                object tag = editingBorder.Tag;
                if (tag == sender)
                {
                    isEditingCurr = true;
                }
            }
            if (!isEditingCurr)
            {
                editingBorder = control_GetFocus(sender, e);
            }

            FrameworkElement control = (FrameworkElement)sender;
            control_MouseDown_Flag = true;
            control_MouseDown_Point = e.GetPosition(pageTemplate.container);
            control.CaptureMouse();
            if (control is TextBox)
            {
                TextBox tb = (TextBox)control;
                if (editingBorder.Background == null)
                {
                    tb.SelectionLength = 0;
                    tb.Focusable = true;
                    tb.Focus();
                    control.ReleaseMouseCapture();
                }
                else
                {
                    tb.Focusable = false;
                }
            }
            if (editingBorder.Background == null)
            {
                control_MouseDown_Flag = false;
                // control.Focus(); 
            }

        }


        /*
         * 控件鼠标移动
         */
        public void control_MouseMove(object sender, MouseEventArgs e)
        {
            if (control_MouseDown_Flag)
            {
                FrameworkElement control = (FrameworkElement)sender;

                //当控件很小时，移动控件却没有动，当到其他控件时，其他控件变动
                //只有验证是否为鼠标按下时的控件 
                double pagePercent = (double)App.localStorage.cfg.pagePercent / 100;


                Thickness currMargin = control.Margin;


                Point p = Mouse.GetPosition(e.Source as FrameworkElement);
                Point location = (e.Source as FrameworkElement).PointToScreen(p);

                double x = currMargin.Left + e.GetPosition(pageTemplate.container).X - control_MouseDown_Point.X;
                double y = currMargin.Top + e.GetPosition(pageTemplate.container).Y - control_MouseDown_Point.Y;

                control_MouseDown_Point = e.GetPosition(pageTemplate.container);
                control.Margin = new Thickness(x, y, 0, 0);
                editingBorder.Margin = new Thickness(x, y, 0, 0);

            }

        }
        /*
         * 控件鼠标松开
         */
        public void control_MouseUp(object sender, MouseButtonEventArgs e)
        {
            //  Grid.SetZIndex(editingBorder, 11);
            control_MouseDown_Flag = false;
            FrameworkElement control = (FrameworkElement)sender;
            control.ReleaseMouseCapture();
            e.Handled = true;


            //更新控件数据 位置移动
            DControl dc = (DControl)control.Tag;
            dc.left = (Int32)control.Margin.Left;
            dc.top = (Int32)control.Margin.Top;
            control.Tag = dc;
            dControlBll.update(dc);
        }
        /*
         * 文本框失去焦点，保存文本
         */
        public void textBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            object tag = textBox.Tag;
            if (tag == null) return;
            DControl ctl = (DControl)tag;
            ctl.content = textBox.Text;
            dControlBll.update(ctl);
            textBox.Tag = ctl;

        }

        /*
         * 编辑框鼠标按下
         */
        public void editingBorder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            editingBorder_MouseDown_Flag = true;
            editingBorder_MouseDown_Point = e.GetPosition(pageTemplate.container);
            FrameworkElement control = (FrameworkElement)sender;
            // control.CaptureMouse();

            if (e.ClickCount >= 2)
            {
                textBox_PreviewMouseDoubleClick(sender, e);
                editingBorder_MouseDown_Flag = false;
            }
            //已经是编辑状态了，再点击
            if (editingBorder.Background == null)
            {
                editingBorder_MouseDown_Flag = false;
            }
        }
        /*
         * 编辑框鼠标移动
         */
        public void editingBorder_MouseMove(object sender, MouseEventArgs e)
        {
            if (editingBorder_MouseDown_Flag)
            {
                Border border = (Border)sender;
                FrameworkElement control = (FrameworkElement)border.Tag;
                Thickness currMargin = border.Margin;

                double x = e.GetPosition(pageTemplate.container).X - editingBorder_MouseDown_Point.X;
                double y = e.GetPosition(pageTemplate.container).Y - editingBorder_MouseDown_Point.Y;

                editingBorder_MouseDown_Point = e.GetPosition(pageTemplate.container);
                border.Margin = new Thickness(currMargin.Left + x, currMargin.Top + y, 0, 0);
                control.Margin = new Thickness(currMargin.Left + x, currMargin.Top + y, 0, 0);
            }


        }
        /*
         * 编辑框鼠标松开
         */
        public void editingBorder_MouseUp(object sender, MouseButtonEventArgs e)
        {

            editingBorder_MouseDown_Flag = false;
            e.Handled = true;

            FrameworkElement editingBorder1 = (FrameworkElement)sender;
            editingBorder1.ReleaseMouseCapture();

            FrameworkElement control = (FrameworkElement)editingBorder1.Tag;

            //更新控件数据 位置移动
            DControl dc = (DControl)control.Tag;
            dc.left = (Int32)control.Margin.Left;
            dc.top = (Int32)control.Margin.Top;
            control.Tag = dc;
            dControlBll.update(dc);
        }




        /*
        * border4鼠标移入
        */
        public void border3_MouseEnter(object sender, MouseEventArgs e)
        {
            pageTemplate.container.Cursor = Cursors.ArrowCD;

        }



        /*
       * border4鼠标移入
       */
        public void border3_MouseLeave(object sender, MouseEventArgs e)
        {
            pageTemplate.container.Cursor = Cursors.Arrow;

        }

        private Double currControlWidth;
        private Double currControlHeight;
        private Double currControlLeft;
        private Double currControlTop;

        /*
        * 编辑框鼠标按下
        */
        public void border3_MouseDown(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement pointBorder = (FrameworkElement)sender;
            point8_MouseDown_Flag = true;
            point8_MouseDown_Point = e.GetPosition(pageTemplate.container);
            e.Handled = true;
            pointBorder.CaptureMouse();
            pointBorder.Cursor = Cursors.Hand;


            FrameworkElement control = (FrameworkElement)editingBorder.Tag;
            DControl dControl = (DControl)control.Tag;
            currControlWidth = dControl.width;
            currControlHeight = dControl.height;
            currControlLeft = dControl.left;
            currControlTop = dControl.top;

        }


        /*
         * 编辑框鼠标移动
         */
        public void border3_MouseMove(object sender, MouseEventArgs e)
        {
            if (point8_MouseDown_Flag)
            {
                //  double pagePercent = (double)App.localStorage.cfg.pagePercent / 100;
                Border pointBorder = (Border)sender;
                Canvas canvas = (Canvas)VisualTreeHelper.GetParent(pointBorder);
                Border editingBorder1 = (Border)VisualTreeHelper.GetParent(canvas);

                FrameworkElement control = (FrameworkElement)editingBorder1.Tag;
                // double currWidth = control.Width;
                // double currHeight = control.Height;

                Point currMouseDown_point = e.GetPosition(pageTemplate.container);
                double x = currMouseDown_point.X - point8_MouseDown_Point.X;
                double y = currMouseDown_point.Y - point8_MouseDown_Point.Y;

                point8_MouseDown_Point = currMouseDown_point;


                currControlWidth = currControlWidth + x;
                currControlHeight = currControlHeight + y;
                //1.修改editingBorder宽度高度 
                double x1, y1;

                if (currControlWidth < 0)
                {
                    x1 = currControlLeft + currControlWidth;
                }
                else
                {
                    x1 = currControlLeft;
                }
                if (currControlHeight < 0)
                {
                    y1 = currControlTop + currControlHeight;

                }
                else
                {
                    y1 = currControlTop;
                }
                editingBorder1.Margin = new Thickness(x1, y1, 0, 0);
                control.Margin = new Thickness(x1, y1, 0, 0);

                //   control.RenderTransform = new ScaleTransform(scaleX, scaleY);
                // control.RenderTransformOrigin = new Point(0.5,0.5);

                // editingBorder1.RenderTransform = new ScaleTransform(sx1, sy1);



                editingBorder1.Width = Math.Abs(currControlWidth);
                editingBorder1.Height = Math.Abs(currControlHeight);
                canvas.Width = editingBorder1.Width;
                canvas.Height = editingBorder1.Height;
                control.Width = editingBorder1.Width;
                control.Height = editingBorder1.Height;


                //2.修改8个点的位置  
                foreach (UIElement ele in canvas.Children)
                {
                    Border b = (Border)ele;
                    if (b.Name == "leftCenterBorder")
                    {
                        b.SetValue(Canvas.TopProperty, (canvas.Height - pointWidth) / 2);
                    }
                    else if (b.Name == "topCenterBorder")
                    {
                        b.SetValue(Canvas.LeftProperty, (canvas.Width - pointWidth) / 2);
                    }
                    else if (b.Name == "rightCenterBorder")
                    {
                        b.SetValue(Canvas.TopProperty, (canvas.Height - pointWidth) / 2);
                    }
                    else if (b.Name == "bottomCenterBorder")
                    {
                        b.SetValue(Canvas.LeftProperty, (canvas.Width - pointWidth) / 2);
                    }
                }


                e.Handled = true;
            }
        }
        /*
         * 编辑框鼠标松开
         */
        public void border3_MouseUp(object sender, MouseButtonEventArgs e)
        {
            point8_MouseDown_Flag = false;
            FrameworkElement pointBorder = (FrameworkElement)sender;
            pointBorder.ReleaseMouseCapture();
            pointBorder.Cursor = Cursors.Arrow;
            e.Handled = true;

            Canvas canvas = (Canvas)VisualTreeHelper.GetParent(pointBorder);
            Border editingBorder1 = (Border)VisualTreeHelper.GetParent(canvas);
            FrameworkElement control = (FrameworkElement)editingBorder1.Tag;
            //更新控件数据 位置变更及宽高
            DControl dc = (DControl)control.Tag;
            dc.left = (Int32)control.Margin.Left;
            dc.top = (Int32)control.Margin.Top;
            dc.width = (Int32)control.Width;
            dc.height = (Int32)control.Height;
            control.Tag = dc;
            dControlBll.update(dc);
        }


        /*
         * 页面上点击删除编辑框
         */
        public void Page_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            control_MouseDown_Flag = false;
            editingBorder_MouseDown_Flag = false;
            point8_MouseDown_Flag = false;

            pageTemplate.container.Children.Remove(editingBorder);
            editingBorder = null;
        }


        /*
         * 双击文本框
         */
        internal void textBox_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement ele = (FrameworkElement)sender;
            if (ele.GetType().Name == "Border")
            {
                //Border edtingBorder = (Border)ele;
                editingBorder.Background = null;
                ele = (FrameworkElement)ele.Tag;
            }

            if (ele is TextBox == false) return;
            TextBox textBox = (TextBox)ele;
            textBox.Focusable = true;
            textBox.Focus();
            textBox.SelectAll();
            e.Handled = true;
            return;
        }

    }
}
