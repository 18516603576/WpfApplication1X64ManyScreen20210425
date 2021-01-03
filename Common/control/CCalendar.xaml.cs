using Common.util;
using Model;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Common.control
{
    /// <summary>
    /// CCalendar.xaml 的交互逻辑
    /// </summary>
    public partial class CCalendar : UserControl
    {
        //定时器 视频时间轴
        readonly DispatcherTimer timer = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(1000), IsEnabled = true };
        private readonly string[] weekDayArr = new String[] { "日", "一", "二", "三", "四", "五", "六" };
        private DControl currDControl = null;
        public CCalendar(DControl ctl, DPage dPage, Cfg cfg)
        {
            weekDayArr[0] = cfg.week7;
            weekDayArr[1] = cfg.week1;
            weekDayArr[2] = cfg.week2;
            weekDayArr[3] = cfg.week3;
            weekDayArr[4] = cfg.week4;
            weekDayArr[5] = cfg.week5;
            weekDayArr[6] = cfg.week6;

            InitializeComponent();
            currDControl = ctl;
            init();
            Unloaded += CCalendar_Unloaded;
            SizeChanged += CCalendar_SizeChanged;
        }

        private void CCalendar_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            currDControl.width = Convert.ToInt32(Width);
            currDControl.height = Convert.ToInt32(Height);
            textBlock.Width = currDControl.width;
            textBlock.Height = currDControl.width;
        }

        private void CCalendar_Unloaded(object sender, RoutedEventArgs e)
        {
            timer.Tick -= new EventHandler(Timer_Tick);
            timer.Stop();
            Unloaded -= CCalendar_Unloaded;
            SizeChanged -= CCalendar_SizeChanged;

            textBlock = null;
            this.Resources.Clear();
            GC.Collect();
        }

        /*
        * 初始化定时器
        */
        private void init()
        {

            textBlock.FontSize = FontUtil.getFontSize(currDControl.fontSize);
            textBlock.FontFamily = FontUtil.getFontFamily(currDControl.fontFamily);
            textBlock.Foreground = FontUtil.getFontColor(currDControl.fontColor);
            textBlock.FontWeight = FontUtil.getFontWeight(currDControl.fontWeight);
            textBlock.TextAlignment = FontUtil.getFontTextAlignment(currDControl.fontTextAlignment);
            TextBlock.SetLineHeight(textBlock, FontUtil.getFontLineHeight(currDControl.fontLineHeight));
            updateContent(currDControl.content);

            timer.Tick += new EventHandler(Timer_Tick); //超过计时间隔时发生 
            timer.Start(); //DT启动   
        }

        /*
        *  定时处理类
        */
        private void Timer_Tick(object sender, EventArgs e)
        {
            updateContent(currDControl.content);
        }

        private void updateContent(string content)
        {
            DateTime dt = DateTime.Now;
            int dayOfWeek = (int)dt.DayOfWeek;
            string dtStr = "";
            try
            {
                dtStr = string.Format("{0:" + content + "}", dt);
                dtStr = dtStr.Replace("week", weekDayArr[dayOfWeek]);
            }
            catch (Exception)
            {
                MessageBox.Show("日期格式不正确");
            }

            textBlock.Text = dtStr; 
        }

        /*
        * 编辑控件属性 - 更新页面显示
        */
        public void updateElementAttr(DControl dControl, bool isDesign)
        {
            currDControl = dControl;

            Width = dControl.width;
            Height = dControl.height;
            Margin = new Thickness(dControl.left, dControl.top, 0, 0);
            Opacity = dControl.opacity / 100.0;
            textBlock.Width = dControl.width;
            textBlock.Height = dControl.height;

        }



        /*
         * 四、编辑控件后 - 更新到页面
         */
        public void updateElement(DControl ctl, bool isDesign)
        {
            currDControl = ctl;
            textBlock.FontSize = FontUtil.getFontSize(currDControl.fontSize);
            textBlock.FontFamily = FontUtil.getFontFamily(currDControl.fontFamily);
            textBlock.Foreground = FontUtil.getFontColor(currDControl.fontColor);
            textBlock.FontWeight = FontUtil.getFontWeight(currDControl.fontWeight);
            textBlock.TextAlignment = FontUtil.getFontTextAlignment(currDControl.fontTextAlignment);
            TextBlock.SetLineHeight(textBlock, FontUtil.getFontLineHeight(currDControl.fontLineHeight));
            updateContent(currDControl.content);
        }

        public void updateFontSizeVal(string text, int fontSizeVal)
        {
            textBlock.FontSize = FontUtil.getFontSize(fontSizeVal);
            currDControl.content = text;
            updateContent(text);
        }
        public void updateFontColorVal(string text, string fontColorVal)
        {
            textBlock.Foreground = FontUtil.getFontColor(fontColorVal);
            currDControl.content = text;
            updateContent(text);
        }

        public void updateFontWeightVal(string text, string fontWeightVal)
        {
            textBlock.FontWeight = FontUtil.getFontWeight(fontWeightVal);
            currDControl.content = text;
            updateContent(text);
        }

        public void updateFontTextAlignment(string text, int fontTextAlignmentVal)
        {
            textBlock.TextAlignment = FontUtil.getFontTextAlignment(fontTextAlignmentVal);
            currDControl.content = text;
            updateContent(text);
        }

        public void updateFontFamilyVal(string text, string fontFamilyVal)
        {
            textBlock.FontFamily = FontUtil.getFontFamily(fontFamilyVal);
            currDControl.content = text;
            updateContent(text);
        }

        public void updateFontLineHeightlyVal(string text, int fontLineHeightVal)
        {
            TextBlock.SetLineHeight(textBlock, FontUtil.getFontLineHeight(fontLineHeightVal));
            currDControl.content = text;
            updateContent(text);
        }
    }
}
