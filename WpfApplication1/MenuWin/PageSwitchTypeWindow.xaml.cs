using Bll;
using Model;
using System;
using System.Windows;
using System.Windows.Controls;

namespace WpfApplication1.MenuWin
{
    /// <summary>
    /// PageSwitchTypeWindow.xaml 的交互逻辑
    /// </summary>
    public partial class PageSwitchTypeWindow : Window
    {
        private readonly CfgBll cfgBll = new CfgBll();
        private readonly MainWindow mainWindow;
        public PageSwitchTypeWindow(MainWindow mainWindow)
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
            // cfg.pageSwitchType;

            selectRadio(PageSwitchTypeCanvas.Children, "pageSwitchType", cfg.pageSwitchType);


            //  isAutoStartup.IsChecked = this.getIsAutoStartup("ShowBox");
        }



        /*
         * 保存数据
         */
        private void Submit_Button_Click(object sender, RoutedEventArgs e)
        {
            Int32 pageSwitchType = getRadioSelectedValue(PageSwitchTypeCanvas.Children, "pageSwitchType");
            if (pageSwitchType < 1) { pageSwitchType = 1; }


            //1.更新到数据库
            Cfg cfg = cfgBll.get(1);
            cfg.pageSwitchType = pageSwitchType;
            cfgBll.update(cfg);
            //2.更新全局配置
            App.localStorage.cfg = cfg;


            Close();

        }

        /*
         * 选中单选按钮
         */
        private void selectRadio(UIElementCollection children, string groupName, Int32 currValue)
        {
            foreach (UIElement element in children)
            {
                if (element is RadioButton)
                {
                    RadioButton rb = (RadioButton)element;
                    if (rb.GroupName != groupName) continue;
                    object tag = rb.Tag;
                    if (tag == null) continue;
                    Int32 val = Int32.Parse(tag.ToString());
                    if (val == currValue)
                    {
                        rb.IsChecked = true;
                        break;
                    }
                }
            }
        }
        /*
         * 获取选中值
         */
        private Int32 getRadioSelectedValue(UIElementCollection children, string groupName)
        {
            Int32 result = 0;
            foreach (UIElement element in children)
            {
                if (element is RadioButton)
                {
                    RadioButton rb = (RadioButton)element;
                    if (rb.GroupName != groupName) continue;
                    Boolean b = (Boolean)rb.IsChecked;
                    if (b)
                    {
                        object tag = rb.Tag;
                        if (tag == null) continue;
                        result = Int32.Parse(tag.ToString());
                        break;
                    }
                }
            }
            return result;
        }
    }
}
