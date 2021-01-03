using Bll;
using Model;
using System;
using System.Windows;
using System.Windows.Controls;

namespace WpfApplication1.FolderWin
{
    /// <summary>
    /// StorageImageDeleteFolderWindow.xaml 的交互逻辑
    /// </summary>
    public partial class StorageImageDeleteFolderWindow : Window
    {
        private readonly StorageImageFolderBll storageImageFolderBll = new StorageImageFolderBll();
        //页面id
        private readonly Int32 folderId = 0;
        //页面树
        private readonly TreeView folderTreeColumn;
        public StorageImageDeleteFolderWindow(TreeView folderTreeColumn, Int32 folderId)
        {
            InitializeComponent();
            this.folderTreeColumn = folderTreeColumn;
            this.folderId = folderId;

            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            getPageNameForDelete();
        }

        /*
         * 获取要删除的图片
         */
        private void getPageNameForDelete()
        {
            StorageImageFolder storageImageFolder = storageImageFolderBll.get(folderId);
            if (storageImageFolder != null)
            {
                pageName.Content = storageImageFolder.name;
            }

        }

        private void Submit_Button_Click(object sender, RoutedEventArgs e)
        {
            //1.删除页面
            storageImageFolderBll.deleteCascade(folderId);

            //2.从左侧树中移除
            removeFromTree(folderTreeColumn, folderId);

            //3.关闭窗口
            Close();
        }

        /*
         * 从页面树中移除 
        */
        private void removeFromTree(ItemsControl tree, Int32 currFolderId)
        {
            foreach (TreeViewItem item in tree.Items)
            {
                Int32 folderId = (Int32)item.Tag;
                if (currFolderId == folderId)
                {
                    tree.Items.Remove(item);
                    break;
                }
                else
                {
                    removeFromTree(item, currFolderId);
                }

            }
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
