using Bll;
using Model;
using System;
using System.Windows;
using System.Windows.Controls;

namespace WpfApplication1.FolderWin
{
    /// <summary>
    /// StorageImageNewFolderWindow.xaml 的交互逻辑
    /// </summary>
    public partial class StorageImageNewFolderWindow : Window
    {
        private readonly StorageImageFolderBll storageImageFolderBll = new StorageImageFolderBll();
        //同级页面id
        private readonly Int32 parentId = 0;
        //页面树
        private readonly TreeView folderTreeColumn;

        public StorageImageNewFolderWindow(TreeView folderTreeColumn, Int32 parentId)
        {
            InitializeComponent();
            //this.folderTreeColumn = folderTreeColumn;
            this.folderTreeColumn = folderTreeColumn;
            this.parentId = parentId;

            StorageImageFolder lastFolder = storageImageFolderBll.getLastFolder();
            Int32 maxFolderId = lastFolder.id + 1;
            pagename.Text = "新文件夹" + maxFolderId;

            WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        private void Submit_Button_Click(object sender, RoutedEventArgs e)
        {
            StorageImageFolder tmp = new StorageImageFolder();
            tmp.name = pagename.Text;
            tmp.parentId = parentId;
            if (string.IsNullOrWhiteSpace(tmp.name))
            {
                MessageBox.Show("请填写页面名称");
                return;
            }
            else if (tmp.parentId == 0)
            {
                MessageBox.Show("父页面不存在，请重试");
                return;
            }

            tmp = storageImageFolderBll.insertChild(tmp);

            insertToTree(folderTreeColumn, tmp);

            Close();

        }

        /*
         * 将新添加的页面加入到页面树中
         */
        private void insertToTree(ItemsControl itemsControl, StorageImageFolder folder)
        {
            foreach (TreeViewItem item in itemsControl.Items)
            {
                Int32 folderId = (Int32)item.Tag;
                if (parentId == folderId)
                {
                    TreeViewItem newItem = new TreeViewItem();
                    newItem.Header = folder.name;
                    newItem.Tag = folder.id;
                    newItem.Padding = new Thickness(5);
                    // newItem.Foreground = Brushes.White;

                    item.Items.Add(newItem);
                    newItem.BringIntoView();
                    break;
                }
                else
                {
                    insertToTree(item, folder);
                }
            }
        }
    }
}
