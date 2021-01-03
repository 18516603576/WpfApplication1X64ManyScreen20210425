using Bll;
using Common.Data;
using Model;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfApplication1.PageWin
{
    /// <summary>
    /// NewPageByMenuWindow.xaml 的交互逻辑
    /// </summary>
    public partial class NewPageByMenuWindow : Window
    {
        private readonly DPageBll dPageBll = new DPageBll();

        //页面树
        private readonly TreeView pageTreeColumn;

        //页面居中显示到最前面
        public NewPageByMenuWindow(TreeView pageTreeColumn, Int32 defaultId)
        {
            InitializeComponent();
            this.pageTreeColumn = pageTreeColumn;


            DPage lastPage = dPageBll.getLastPage();
            Int32 maxPageId = lastPage.id + 1;
            pagename.Text = "新页面" + maxPageId;
            initPageCombox(defaultId);

            initCombox(App.localStorage.cfg.pageSwitchType);

            WindowStartupLocation = WindowStartupLocation.CenterScreen;
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
          * 初始化页面列表
          */
        private void initPageCombox(int defaultId)
        {
            DPage firstDPage = dPageBll.get(1);

            ComboBoxItem firstItem = new ComboBoxItem();
            firstItem.Content = firstDPage.name;
            firstItem.Tag = firstDPage.id;
            if (firstDPage.id == defaultId)
            {
                firstItem.IsSelected = true;
            }
            parentIdComboBox.Items.Add(firstItem);


            int level = 0;
            getTreeViewItemChildren(firstItem, level, defaultId);
        }



        /*
        * 获取当前页面的子页面 
        * 
        * @param TreeViewItem currItem 当前页面
         * 
         * @param level层级
        */
        private void getTreeViewItemChildren(ComboBoxItem currItem, int level, int currLinkToPageId)
        {
            level = level + 1;
            Int32 id = (Int32)currItem.Tag;
            List<DPage> list = dPageBll.getByParentId(id);
            if (list == null) return;
            foreach (DPage one in list)
            {
                ComboBoxItem child = new ComboBoxItem();
                child.Content = one.name;
                child.Tag = one.id;
                child.Padding = new Thickness(20 * level, 0, 0, 0);
                if (one.id == currLinkToPageId)
                {
                    child.IsSelected = true;
                }
                parentIdComboBox.Items.Add(child);
                getTreeViewItemChildren(child, level, currLinkToPageId);

            }

        }

        /*
         * 保存数据
         */
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ComboBoxItem item = (ComboBoxItem)parentIdComboBox.SelectedItem;
            int parentIdVal = 0;
            if (item != null)
            {
                parentIdVal = (int)item.Tag;
            }
            ComboBoxItem item2 = (ComboBoxItem)pageSwitchType.SelectedItem;
            int pageSwitchTypeVal = 0;
            if (item2 != null)
            {
                pageSwitchTypeVal = (int)item2.Tag;
            }


            DPage dPage = new DPage();
            dPage.name = pagename.Text;
            dPage.parentId = parentIdVal;
            dPage.pageSwitchType = pageSwitchTypeVal;
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

            dPage = dPageBll.insertChild(dPage);

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
                if (dPage.parentId == pageId)
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
