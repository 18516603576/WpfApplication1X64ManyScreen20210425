using Bll;
using Model;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace WpfApplication1.FolderWin
{
    /// <summary>
    /// StorageImageMoveToFolderWindow.xaml 的交互逻辑
    /// </summary>
    public partial class StorageImageMoveToFolderWindow : Window
    {
        private readonly StorageImageFolderBll storageImageFolderBll = new StorageImageFolderBll();
        private readonly StorageImageBll storageImageBll = new StorageImageBll();
        private readonly WrapPanel storageListWrap;
        private readonly List<StorageImage> list;
        private readonly List<Canvas> canvasList;

        public StorageImageMoveToFolderWindow(WrapPanel storageListWrap, List<StorageImage> list, List<Canvas> canvasList)
        {
            InitializeComponent();
            this.storageListWrap = storageListWrap;
            this.list = list;
            this.canvasList = canvasList;
            initCombox();
        }

        /*
        * 初始化页面列表
        */
        private void initCombox()
        {
            StorageImageFolder firstFolder = storageImageFolderBll.get(1);
            if (firstFolder == null) return;

            ComboBoxItem defaultItem = new ComboBoxItem();
            defaultItem.Content = "-- 请选择 --";
            defaultItem.Tag = 0;
            defaultItem.IsSelected = true;
            moveToFolderId.Items.Add(defaultItem);

            ComboBoxItem firstItem = new ComboBoxItem();
            firstItem.Content = firstFolder.name;
            firstItem.Tag = firstFolder.id;
            moveToFolderId.Items.Add(firstItem);

            int level = 0;
            getTreeViewItemChildren(firstItem, level, 0);
        }

        /*
        * 获取当前页面的子页面 
        * 
        * @param TreeViewItem currItem 当前页面
        */
        private void getTreeViewItemChildren(ComboBoxItem currItem, int level, int currLinkToPageId)
        {
            level = level + 1;
            Int32 id = (Int32)currItem.Tag;
            List<StorageImageFolder> list = storageImageFolderBll.getByParentId(id);
            if (list == null) return;
            foreach (StorageImageFolder one in list)
            {
                ComboBoxItem child = new ComboBoxItem();
                child.Content = one.name;
                child.Tag = one.id;
                child.Padding = new Thickness(20 * level, 0, 0, 0);
                if (one.id == currLinkToPageId)
                {
                    child.IsSelected = true;
                }
                moveToFolderId.Items.Add(child);
                getTreeViewItemChildren(child, level, currLinkToPageId);
            }
        }

        private void Submit_Button_Click(object sender, RoutedEventArgs e)
        {


            //1.更新数据库
            ComboBoxItem item = (ComboBoxItem)moveToFolderId.SelectedItem;
            int selectedVal = 0;
            if (item != null)
            {
                selectedVal = (int)item.Tag;
            }
            if (selectedVal <= 0)
            {
                MessageBox.Show("请选择文件夹");
                return;
            }



            foreach (StorageImage one in list)
            {
                one.folderId = selectedVal;
                storageImageBll.update(one);
            }

            //2.页面中移除
            //4.从页面移除选中项
            foreach (Canvas canvas in canvasList)
            {
                storageListWrap.Children.Remove(canvas);
            }

            Close();
        }
    }
}
