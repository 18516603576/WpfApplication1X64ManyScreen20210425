using Bll;
using Model;
using System.Windows;
using WpfApplication1.manage;

namespace WpfApplication1.ManageWin
{
    /// <summary>
    /// EditLinkToWebWindow.xaml 的交互逻辑
    /// </summary>
    public partial class EditLinkToWebWindow : Window
    {

        private readonly DPageBll dPageBll = new DPageBll();
        private readonly DControlBll dControlBll = new DControlBll();
        private readonly FrameworkElement currElement;
        private readonly DControl currDControl;

        public EditLinkToWebWindow(Editing editing, FrameworkElement currElement)
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.currElement = currElement;
            currDControl = (DControl)currElement.Tag;
            initPageData(currDControl.linkToWeb);
        }

        /*
         * 初始化页面列表
        */
        private void initPageData(string linkToWeb)
        {
            this.linkToWeb.Text = linkToWeb;
        }

        private void Submit_Button_Click(object sender, RoutedEventArgs e)
        {
            //更新到数据库
            DControl dControl = dControlBll.get(currDControl.id);
            dControl.linkToWeb = linkToWeb.Text;
            currElement.Tag = dControl;
            dControlBll.update(dControl);

            Close();
        }



    }
}
