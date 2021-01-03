using Bll;
using Model;
using System;
using System.Windows;
using System.Windows.Controls;

namespace WpfApplication1.PageWin
{
    /// <summary>
    /// RenameWindow.xaml 的交互逻辑
    /// </summary>
    public partial class RenameWindow : Window
    {
        private readonly DPageBll dPageBll = new DPageBll();
        //同级页面id
        private readonly Int32 thatPageId = 0;
        //页面树
        private readonly TreeView pageTreeColumn;



        //页面居中显示到最前面
        public RenameWindow(TreeView pageTreeColumn, Int32 thatPageId)
        {
            InitializeComponent();
            this.pageTreeColumn = pageTreeColumn;
            this.thatPageId = thatPageId;

            DPage thatPage = dPageBll.get(this.thatPageId);

            pagename.Text = thatPage.name;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }



        /*
         * 保存数据
         */
        private void Button_Click(object sender, RoutedEventArgs e)
        {

            DPage dPage = dPageBll.get(thatPageId);
            dPage.name = pagename.Text;

            if (string.IsNullOrWhiteSpace(dPage.name))
            {
                MessageBox.Show("请填写页面名称");
                return;
            }
            //Boolean b = dPageBll.hasSameName(dPage);
            //if (b)
            //{
            //    MessageBox.Show("页面名称重复，请更换");
            //    return;
            //}
            dPage = dPageBll.update(dPage);

            updateToTree(pageTreeColumn, dPage);

            Close();

        }

        /*
        * 将新添加的页面加入到页面树中
        */
        private void updateToTree(ItemsControl tree, DPage dPage)
        {
            foreach (TreeViewItem item in tree.Items)
            {
                Int32 pageId = (Int32)item.Tag;
                if (thatPageId == pageId)
                {
                    item.Header = dPage.name;
                    break;
                }
                else
                {
                    updateToTree(item, dPage);
                }
            }
        }
    }
}
