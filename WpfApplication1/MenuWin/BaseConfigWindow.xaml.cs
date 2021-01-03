using Bll;
using Common;
using Microsoft.Win32;
using Model;
using System;
using System.Windows;

namespace WpfApplication1.MenuWin
{
    /// <summary>
    /// BaseConfigWindow.xaml 的交互逻辑
    /// </summary>
    public partial class BaseConfigWindow : Window
    {

        private readonly CfgBll cfgBll = new CfgBll();
        private readonly StorageFileBll storageFileBll = new StorageFileBll();
        private readonly MainWindow mainWindow;
        public BaseConfigWindow(MainWindow mainWindow)
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.mainWindow = mainWindow;
            initData();
        }
        /*
         * 初始化页面数据
         */
        private void initData()
        {
            Cfg cfg = cfgBll.get(1);
            screenWidth.Text = cfg.screenWidth.ToString();
            screenHeight.Text = cfg.screenHeight.ToString();
            isAutoStartup.IsChecked = getIsAutoStartup("ShowBox");
        }

        //判断软件是否开机启动
        private bool getIsAutoStartup(String softName)
        {
            bool b = false;
            RegistryKey local = Registry.CurrentUser;
            RegistryKey run = local.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
            if (run.GetValue(softName) != null)
            {
                b = true;
            }
            return b;
        }


        /*
         * 保存数据
         */
        private void Submit_Button_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(screenWidth.Text))
            {
                MessageBox.Show("请填写页面宽度；"); return;
            }
            else if (!DataUtil.isInt(screenWidth.Text.ToString()))
            {
                MessageBox.Show("页面宽度必须是整数；"); return;
            }
            if (string.IsNullOrWhiteSpace(screenHeight.Text.ToString()))
            {
                MessageBox.Show("请填写页面高度；"); return;
            }
            else if (!DataUtil.isInt(screenHeight.Text.ToString()))
            {
                MessageBox.Show("页面高度必须是整数；"); return;
            }

            //1.更新到数据库
            Cfg cfg = cfgBll.get(1);
            cfg.screenWidth = int.Parse(screenWidth.Text);
            cfg.screenHeight = int.Parse(screenHeight.Text);
            cfgBll.update(cfg);
            //2.更新全局配置
            App.localStorage.cfg = cfg;
            //3.重新加载页面
            mainWindow.reloadWindow();

            Close();

        }

        private void isAutoStartup_Click(object sender, RoutedEventArgs e)
        {
            bool b = (bool)isAutoStartup.IsChecked;
            try
            {
                string path = AppDomain.CurrentDomain.BaseDirectory + "ShowBox.exe";
                if (b)
                {
                    RegistryKey RKey = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
                    RKey.SetValue("ShowBox", path);
                    RKey.Close();
                    //MessageBox.Show("自动启动");
                }
                else
                {

                    RegistryKey RKey = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run");
                    RKey.DeleteValue("ShowBox", false);
                    RKey.Close();
                    //  MessageBox.Show("取消自动启动");
                }

            }
            catch (Exception e1)
            {
                MessageBox.Show(e1.Message + "（请关闭杀毒软件再尝试！）");
            }
        }
    }
}
