using Bll;
using Model;
using System;
using System.Windows;
using System.Windows.Controls;

namespace WpfApplication1.FolderWin
{
    /// <summary>
    /// StorageImageRenameFolderWindow.xaml 的交互逻辑
    /// </summary>
    public partial class StorageImageRenameFolderWindow : Window
    {
        private readonly StorageImageFolderBll storageImageFolderBll = new StorageImageFolderBll();
        //同级页面id
        private readonly Int32 thatFolderId = 0;
        //页面树
        private readonly TreeView pageTreeColumn;

        //页面居中显示到最前面
        public StorageImageRenameFolderWindow(TreeView pageTreeColumn, Int32 folderId)
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.pageTreeColumn = pageTreeColumn;
            thatFolderId = folderId;
            StorageImageFolder thatFolder = storageImageFolderBll.get(thatFolderId);

            foldername.Text = thatFolder.name;
        }



        /*
         * 保存数据
         */
        private void Button_Click(object sender, RoutedEventArgs e)
        {

            StorageImageFolder storageImageFolder = storageImageFolderBll.get(thatFolderId);
            storageImageFolder.name = foldername.Text;

            if (string.IsNullOrWhiteSpace(storageImageFolder.name))
            {
                MessageBox.Show("请填写文件夹名称");
                return;
            }
            //Boolean b = storageImageFolderBll.hasSameName(storageImageFolder);
            //if (b)
            //{
            //    MessageBox.Show("文件夹名称重复，请更换");
            //    return;
            //}
            storageImageFolder = storageImageFolderBll.update(storageImageFolder);

            updateToTree(pageTreeColumn, storageImageFolder);

            Close();

        }

        /*
        * 将新添加的页面加入到页面树中
        */
        private void updateToTree(ItemsControl tree, StorageImageFolder storageVideoFolder)
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
