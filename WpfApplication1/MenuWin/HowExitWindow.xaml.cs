using Bll;
using Model;
using System.Windows;

namespace WpfApplication1.MenuWin
{
    /// <summary>
    /// HowExitWindow.xaml 的交互逻辑
    /// </summary>
    public partial class HowExitWindow : Window
    {
        private readonly CfgBll cfgBll = new CfgBll();
        public HowExitWindow()
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
            password.Text = cfg.password;
        }

        /*
        * 保存数据
        */
        private void Submit_Button_Click(object sender, RoutedEventArgs e)
        {

            //1.更新到数据库
            Cfg cfg = cfgBll.get(1);
            cfg.password = password.Text;
            cfgBll.update(cfg);
            //2.更新全局配置
            App.localStorage.cfg = cfg;

            Close();
        }
    }
}
