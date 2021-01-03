using Bll;
using Common.Data;
using Model;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfApplication1
{
    /// <summary>
    /// NewPageWindow.xaml 的交互逻辑
    /// </summary>
    ///  
    public partial class NewPageWindow : Window
    {
        private readonly DPageBll dPageBll = new DPageBll();
        //父页面id
        private readonly Int32 parentId = 0;
        //页面树
        private readonly TreeView pageTreeColumn;

        private readonly CfgBll cfgBll = new CfgBll();



        //页面居中显示到最前面
        public NewPageWindow(TreeView pageTreeColumn, Int32 parentId)
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.pageTreeColumn = pageTreeColumn;
            this.parentId = parentId;

            DPage lastPage = dPageBll.getLastPage();
            Int32 maxPageId = lastPage.id + 1;
            pagename.Text = "新页面" + maxPageId;

            initCombox(App.localStorage.cfg.pageSwitchType);
        }

        /*
         * 初始化切换方式列表
         */
        private void initCombox(int pageSwitchTypeVal)
        {
            foreach (PageSwitchTypeDto dto in Params.pageSwitchTypeList)
            {
                ComboBoxItem one = new ComboBoxItem();
                one.Content = dto.name;
                one.Tag = dto.id;
                if (dto.id == pageSwitchTypeVal)
                {
                    one.IsSelected = true;
                }
                pageSwitchType.Items.Add(one);
            }

        }




        /*
         * 保存数据
         */
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DPage dPage = new DPage();
            dPage.name = pagename.Text;
            dPage.parentId = parentId;
            if (string.IsNullOrWhiteSpace(dPage.name))
            {
                MessageBox.Show("请填写页面名称");
                return;
            }
            else if (dPage.parentId == 0)
            {
                MessageBox.Show("父页面不存在，请重试");
                return;
            }
            ComboBoxItem item = (ComboBoxItem)pageSwitchType.SelectedItem;
            int pageSwitchTypeVal = 0;
            if (item != null)
            {
                pageSwitchTypeVal = (int)item.Tag;
            }
            dPage.pageSwitchType = pageSwitchTypeVal;

            dPage = dPageBll.insertChild(dPage);
            if (App.localStorage.cfg.pageSwitchType != dPage.pageSwitchType)
            {
                App.localStorage.cfg.pageSwitchType = dPage.pageSwitchType;
                cfgBll.update(App.localStorage.cfg);
            }

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
                if (parentId == pageId)
                {
                    TreeViewItem newItem = new TreeViewItem();
                    newItem.Header = dPage.name;
                    newItem.Tag = dPage.id;
                    newItem.Padding = new Thickness(5);
                    newItem.Foreground = Brushes.White;

                    item.Items.Add(newItem);
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
