
using AuthTool.util;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AuthTool
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DateTime start = DateTime.Now;
            DateTime end = start.AddDays(90);
            calendar1.DisplayDateStart = start;
            calendar1.DisplayDateEnd = end;

            //隐藏日历
            PreviewMouseUp += mainContainer_PreviewMouseUp;
            TouchUp += mainContainer_TouchUp;


        }
        /*
        * 提交 
        */
        private void Reg_Submit_Button_Click(object sender, RoutedEventArgs e)
        {
            string machineCodeVal = machineCodeText.Text;
            string regCodeVal = regCodeText.Text;
            string limitDayVal = limitDayText.Text;
            if (string.IsNullOrWhiteSpace(machineCodeVal))
            {
                MessageBox.Show("请填写机器码");
                return;
            }
            if (string.IsNullOrWhiteSpace(regCodeVal))
            {
                MessageBox.Show("请填写永久注册码");
                return;
            }
            if (string.IsNullOrWhiteSpace(limitDayVal))
            {
                MessageBox.Show("请填写有效期");
                return;
            }
            bool isRight = RegCodeUtil.isRightOfRegCode(regCodeVal, machineCodeVal);
            if (!isRight)
            {
                MessageBox.Show("永久注册码无效，请检查机器码是否正确");
                return;
            }


            DateTimeFormatInfo dtFormat = new DateTimeFormatInfo();
            dtFormat.ShortDatePattern = "yyyy-MM-dd";
            DateTime dt = Convert.ToDateTime(limitDayVal, dtFormat);
            TimeSpan ts1 = dt - DateTime.Parse("1970-1-1");


            //更新注册码到数据库，并显示注册成功 
            int days = (int)Math.Floor(ts1.TotalDays);
            string limitDayRegCodeVal = RegCodeUtil.generateWithLimitDay(days, machineCodeVal);
            limitDayRegCodeText.Text = limitDayRegCodeVal;


            calendar1.Visibility = Visibility.Collapsed;
            limitDayCanvas.Visibility = Visibility.Visible;

        }

        /*
         * 获取并填写选中值 - 日历
         */
        private void Calendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            // 单选的情况
            if (calendar1.SelectedDate.HasValue)
            {
                DateTime date = calendar1.SelectedDate.Value.Date;
                limitDayText.Text = string.Format("{0:yyyy-MM-dd}", date);
            }
        }
        /*
       * 显示隐藏日历控件
       */
        private void limitDayText_MouseUp(object sender, MouseButtonEventArgs e)
        {
            calendar1.Visibility = Visibility.Visible;
        }

        private void limitDayText_TouchUp(object sender, TouchEventArgs e)
        {
            calendar1.Visibility = Visibility.Visible;
        }

        /*
         * 单击空白处，隐藏日历
         */
        private void mainContainer_TouchUp(object sender, TouchEventArgs e)
        {
            if (e.Source is MainWindow)
            {
                calendar1.Visibility = Visibility.Collapsed;
            }
        }

        private void mainContainer_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.Source is MainWindow)
            {
                calendar1.Visibility = Visibility.Collapsed;
            }

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
