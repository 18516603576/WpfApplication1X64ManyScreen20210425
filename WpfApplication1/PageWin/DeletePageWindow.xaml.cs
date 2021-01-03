using Bll;
using Model;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace WpfApplication1.PageWin
{
    /// <summary>
    /// DeletePageWindow.xaml 的交互逻辑
    /// </summary>
    public partial class DeletePageWindow : Window
    {
        private readonly DPageBll dPageBll = new DPageBll();
        //页面id
        private readonly Int32 pageId = 0;
        //页面树
        private readonly TreeView pageTreeColumn;
        public DeletePageWindow(TreeView pageTreeColumn, Int32 pageId)
        {
            InitializeComponent();
            this.pageTreeColumn = pageTreeColumn;
            this.pageId = pageId;

            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            getPageNameForDelete();
        }

        /*
         * 获取要删除的页面信息
         */
        private void getPageNameForDelete()
        {
            DPage dPage = dPageBll.get(pageId);
            if (dPage != null)
            {
                pageName.Content = dPage.name;
            }

        }

        private void Submit_Button_Click(object sender, RoutedEventArgs e)
        {
            //1.删除页面
            deletePage(pageId);
            //2.从左侧树中移除
            removeFromTree(pageTreeColumn, pageId);
            //3.关闭窗口
            Close();
        }

        /*
         * 从页面树中移除 
        */
        private void removeFromTree(ItemsControl tree, Int32 currPageId)
        {
            foreach (TreeViewItem item in tree.Items)
            {
                Int32 pageId = (Int32)item.Tag;
                if (currPageId == pageId)
                {
                    tree.Items.Remove(item);
                    break;
                }
                else
                {
                    removeFromTree(item, currPageId);
                }

            }
        }


        /*
         * 级联删除页面
         */
        private void deletePage(Int32 pageId)
        {
            List<DPage> children = dPageBll.getByParentId(pageId);
            foreach (DPage dPage in children)
            {
                deletePage(dPage.id);
            }
            dPageBll.delete(pageId);
        }

        /*
         * 取消事件 
         */
        private void Cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

    }
}
