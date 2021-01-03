using Bll;
using Model;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfApplication1.PageWin
{
    /// <summary>
    /// NewAfterPageWindow.xaml 的交互逻辑
    /// </summary>
    public partial class NewAfterPageWindow : Window
    {
        private readonly DPageBll dPageBll = new DPageBll();
        //同级页面id
        private readonly Int32 thatPageId = 0;
        //页面树
        private readonly TreeView pageTreeColumn;



        //页面居中显示到最前面
        public NewAfterPageWindow(TreeView pageTreeColumn, Int32 thatPageId)
        {
            InitializeComponent();
            this.pageTreeColumn = pageTreeColumn;
            this.thatPageId = thatPageId;


            DPage lastPage = dPageBll.getLastPage();
            Int32 maxPageId = lastPage.id + 1;

            pagename.Text = "新页面" + maxPageId;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }



        /*
         * 保存数据
         */
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DPage thatPage = dPageBll.get(thatPageId);


            DPage dPage = new DPage();
            dPage.name = pagename.Text;
            dPage.parentId = thatPage.parentId;
            dPage.idx = thatPage.idx + 1;

            if (string.IsNullOrWhiteSpace(dPage.name))
            {
                MessageBox.Show("请填写页面名称");
                return;
            }
            else if (string.IsNullOrWhiteSpace(thatPage.name))
            {
                MessageBox.Show("同级页面不存在，请重试");
                return;
            }
            dPage = dPageBll.insertAfter(dPage);

            insertToTree(pageTreeColumn, dPage);

            Close();

        }

        /*
        * 将新添加的页面加入到页面树中
        */
        private void insertToTree(ItemsControl tree, DPage dPage)
        {
            foreach (TreeViewItem item in tree.Items)
            {
                Int32 pageId = (Int32)item.Tag;
                if (thatPageId == pageId)
                {
                    int index = tree.Items.IndexOf(item);
                    TreeViewItem newItem = new TreeViewItem();
                    newItem.Header = dPage.name;
                    newItem.Tag = dPage.id;
                    newItem.Padding = new Thickness(5);
                    newItem.Foreground = Brushes.White;

                    tree.Items.Insert(index + 1, newItem);
                    break;
                }
                else
                {
                    insertToTree(item, dPage);
                }
            }
        }
    }
}
