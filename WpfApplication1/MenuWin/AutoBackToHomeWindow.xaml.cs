using Bll;
using Common;
using Model;
using System.Windows;

namespace WpfApplication1.MenuWin
{
    /// <summary>
    /// BaseConfigWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AutoBackToHomeWindow : Window
    {

        private readonly CfgBll cfgBll = new CfgBll();
        private readonly StorageFileBll storageFileBll = new StorageFileBll();
        public AutoBackToHomeWindow()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            initData();
        }
        /*
         * 初始化页面数据
         */
        private void initData()
        {
            Cfg cfg = cfgBll.get(1);
            noActionTimeBackToHome.Text = cfg.noActionTimeBackToHome.ToString();
        }


        /*
         * 保存数据
         */
        private void Submit_Button_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(noActionTimeBackToHome.Text))
            {
                MessageBox.Show("请填写无操作时间；"); return;
            }
            else if (!DataUtil.isInt(noActionTimeBackToHome.Text.ToString()))
            {
                MessageBox.Show("无操作时间必须是整数；"); return;
            }


            //1.更新到数据库
            Cfg cfg = cfgBll.get(1);
            cfg.noActionTimeBackToHome = int.Parse(noActionTimeBackToHome.Text);
            cfgBll.update(cfg);
            //2.更新全局配置
            App.localStorage.cfg = cfg;

            Close();

        }


    }
}
