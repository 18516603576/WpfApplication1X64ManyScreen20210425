using Bll;
using Model;
using System;
using System.Windows;
using System.Windows.Controls;

namespace WpfApplication1.FolderWin
{
    /// <summary>
    /// RenameFolderWindow.xaml 的交互逻辑
    /// </summary>
    public partial class StorageVideoRenameFolderWindow : Window
    {
        private readonly StorageVideoFolderBll storageVideoFolderBll = new StorageVideoFolderBll();
        //同级页面id
        private readonly Int32 thatFolderId = 0;
        //页面树
        private readonly TreeView pageTreeColumn;

        //页面居中显示到最前面
        public StorageVideoRenameFolderWindow(TreeView pageTreeColumn, Int32 folderId)
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.pageTreeColumn = pageTreeColumn;
            thatFolderId = folderId;
            StorageVideoFolder thatFolder = storageVideoFolderBll.get(thatFolderId);

            foldername.Text = thatFolder.name;
        }



        /*
         * 保存数据
         */
        private void Button_Click(object sender, RoutedEventArgs e)
        {

            StorageVideoFolder storageVideoFolder = storageVideoFolderBll.get(thatFolderId);
            storageVideoFolder.name = foldername.Text;

            if (string.IsNullOrWhiteSpace(storageVideoFolder.name))
            {
                MessageBox.Show("请填写文件夹名称");
                return;
            }
            //Boolean b = storageVideoFolderBll.hasSameName(storageVideoFolder);
            //if (b)
            //{
            //    MessageBox.Show("文件夹名称重复，请更换");
            //    return;
            //}
            storageVideoFolder = storageVideoFolderBll.update(storageVideoFolder);

            updateToTree(pageTreeColumn, storageVideoFolder);

            Close();

        }

        /*
        * 将新添加的页面加入到页面树中
        */
        private void updateToTree(ItemsControl tree, StorageVideoFolder storageVideoFolder)
        {
            foreach (TreeViewItem item in tree.Items)
            {
                Int32 pageId = (Int32)item.Tag;
                if (thatFolderId == pageId)
                {
                    item.Header = storageVideoFolder.name;
                    break;
                }
                else
                {
                    updateToTree(item, storageVideoFolder);
                }
            }
        }
    }
}
