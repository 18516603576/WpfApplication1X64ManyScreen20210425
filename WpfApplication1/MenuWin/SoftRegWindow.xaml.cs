using Bll;
using Common;
using Common.util;
using Model;
using System;
using System.Windows;

namespace WpfApplication1.MenuWin
{
    /// <summary>
    /// SoftRegWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SoftRegWindow : Window
    {

        private readonly Cfg2Bll cfg2Bll = new Cfg2Bll();
        private string machineCodeVal = null;
        public SoftRegWindow()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            init();
        }
        /*
         *  初始化
         */
        private void init()
        {

            //1.获取软件是否已注册
            Boolean b = isRegistered();
            if (b)
            {
                successGrid.Visibility = Visibility.Visible;
                regGrid.Visibility = Visibility.Collapsed;
                return;
            }



            //2.如果未注册，则显示注册界面 
            successGrid.Visibility = Visibility.Collapsed;
            regGrid.Visibility = Visibility.Visible;
            machineCodeText.Text = machineCodeVal;

            //获取截止日期
            int leaveDays = 30;
            TimeSpan ts1 = DateTime.Now - DateTime.Parse("1970-1-1");
            int currDay = (int)Math.Floor(ts1.TotalDays);

            Cfg2 cfg2 = cfg2Bll.get(1);

            string ld = cfg2.ld;
            if (!string.IsNullOrWhiteSpace(ld))
            {
                string tmp = EncryptionUtil.decode(ld, machineCodeVal);
                if (DataUtil.isInt(tmp))
                {
                    int limitDay = int.Parse(tmp);
                    leaveDays = limitDay - currDay;
                }
            }
            rtipsLabel.Content = leaveDays + "天后，试用到期";


        }

        /*
         * 软件是否已注册
         */
        private Boolean isRegistered()
        {
            //1.获取数据库信息
            Cfg2 cfg2 = cfg2Bll.get(1);
            string machineCode = MachineCodeUtil.GetMachineCodeString();
            machineCodeVal = machineCode;

            //2.数据完整性校验
            if (RegCodeUtil.validate(cfg2, machineCode) == false) return false;

            //3.注册码验证 
            if (RegCodeUtil.isRightOfRegCode(cfg2.rg3, machineCode) == false) return false;


            return true;
        }




        /*
         * 提交注册码
         * 
         * 1.永久注册码
         * 
         * 2.有有效期的注册码
         */
        private void Reg_Submit_Button_Click(object sender, RoutedEventArgs e)
        {
            string regCodeVal = regCodeText.Text;
            if (string.IsNullOrWhiteSpace(regCodeVal))
            {
                MessageBox.Show("请填写注册码");
                return;
            }
            bool isRight = RegCodeUtil.isRightOfRegCode(regCodeVal, machineCodeVal);

            //1.注册码正确
            if (isRight)
            {
                //更新注册码到数据库，并显示注册成功
                Cfg2 cfg21 = cfg2Bll.get(1);
                cfg21.rg3 = regCodeVal;
                string validateCode1 = RegCodeUtil.generateValidateCode(cfg21, machineCodeVal);
                cfg21.validateCode = validateCode1;
                cfg2Bll.update(cfg21);

                successGrid.Visibility = Visibility.Visible;
                regGrid.Visibility = Visibility.Collapsed;

                return;
            }



            // 2.判断是否为有效期注册码 
            int limitDay = RegCodeUtil.isRightOfLimitDayRegCode(regCodeVal, machineCodeVal);
            if (limitDay <= 0)
            {
                MessageBox.Show("注册码无效，请重试");
                return;
            }
            //更新注册码到数据库 
            Cfg2 cfg2 = cfg2Bll.get(1);
            cfg2.ld = EncryptionUtil.encode(limitDay.ToString(), machineCodeVal);
            string validateCode = RegCodeUtil.generateValidateCode(cfg2, machineCodeVal);
            cfg2.validateCode = validateCode;
            cfg2Bll.update(cfg2);
            //显示剩余试用天数
            TimeSpan ts1 = DateTime.Now - DateTime.Parse("1970-1-1");
            int currDay = (int)Math.Floor(ts1.TotalDays);
            int leaveDays = limitDay - currDay;
            tipsLabel.Content = leaveDays + "天后，试用到期";
            tips.Visibility = Visibility.Visible;

            successGrid.Visibility = Visibility.Visible;
            regGrid.Visibility = Visibility.Collapsed;
        }

        /*
         * 关闭窗口
         */
        private void Ok_Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
