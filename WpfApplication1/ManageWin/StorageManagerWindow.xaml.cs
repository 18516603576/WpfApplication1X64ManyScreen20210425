using Bll;
using Common.util;
using Model;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WpfApplication1.FolderWin;

namespace WpfApplication1.ManageWin
{
    /// <summary>
    /// StorageManagerWindow.xaml 的交互逻辑
    /// </summary>
    public partial class StorageManagerWindow : Window
    {
        private readonly SolidColorBrush colorActive = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2aa1df"));

        private readonly SolidColorBrush grayBtnBg = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#fafafa"));
        private readonly SolidColorBrush grayBtnFont = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#cccccc"));
        private readonly SolidColorBrush activeBtnBg = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#dddddd"));


        private readonly StorageImageBll storageImageBll = new StorageImageBll();
        private readonly StorageVideoFolderBll storageVideoFolderBll = new StorageVideoFolderBll();
        private readonly StorageImageFolderBll storageImageFolderBll = new StorageImageFolderBll();
        private readonly StorageFileFolderBll storageFileFolderBll = new StorageFileFolderBll();
        //获取当前右击的页面
        private Int32 rightClickFolderId = 0;
        private TreeViewItem rightClickTreeViewItem;

        public StorageManagerWindow(string itemName)
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            init(itemName);
        }
        /*
        *  初始化
        */
        private void init(string itemName)
        {
            initBtnBackground();
            SelectItemFun(itemName);
        }


        /*
        * 按钮背景初始化
        */
        private void initBtnBackground()
        {
            imageButton.Background = new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri("Resources/storage/ico_imageItem.png", UriKind.Relative))
                     ,
                Stretch = Stretch.Uniform
            };
            videoButton.Background = new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri("Resources/storage/ico_videoItem.png", UriKind.Relative))
                 ,
                Stretch = Stretch.Uniform
            };
            fileButton.Background = new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri("Resources/storage/ico_fileItem.png", UriKind.Relative))
                 ,
                Stretch = Stretch.Uniform
            };
        }

        /*  
         *  ===================================================
         *  鼠标点击 - 选中某一项
         */
        private void ItemMouseUp(object sender, MouseButtonEventArgs e)
        {
            Canvas itemCanvas = (Canvas)sender;
            string name = itemCanvas.Name;
            SelectItemFun(name);
        }

        /*
         * 选项卡按钮选中
         */
        private void SelectItemFun(string currItemName)
        {
            foreach (Canvas canvas in itemWrapPanel.Children)
            {
                if (currItemName == canvas.Name)
                {
                    //选中当前
                    SelectThisItem(canvas, canvas.Name);

                    if (currItemName == "imageItem")
                    {
                        StorageImage_InitFolder();

                        ImageItemPage imageItemPage = new ImageItemPage(1);
                        mainFrame.Navigate(imageItemPage);
                    }
                    else if (currItemName == "videoItem")
                    {
                        //获取视频文件夹
                        StorageVideo_InitFolder();

                        VideoItemPage videoItemPage = new VideoItemPage(1);
                        mainFrame.Navigate(videoItemPage);
                    }
                    else if (currItemName == "fileItem")
                    {
                        StorageFile_InitFolder();

                        FileItemPage fileItemPage = new FileItemPage(1);
                        mainFrame.Navigate(fileItemPage);
                    }
                }
                else
                {
                    //其他取消选中
                    UnSelectThisItem(canvas, canvas.Name);
                }
            }
        }



        /*
        * 取消选中项
        */
        private void UnSelectThisItem(Canvas canvas, string currItemName)
        {
            string tag = canvas.Tag.ToString();
            if (tag == "1")
            {
                Button btn = (Button)canvas.Children[0];
                Label label = (Label)canvas.Children[1];
                btn.Background = new ImageBrush
                {
                    ImageSource = new BitmapImage(new Uri("Resources/storage/ico_" + currItemName + ".png", UriKind.Relative))
                 ,
                    Stretch = Stretch.Uniform
                };
                label.Foreground = Brushes.Gray;
                canvas.Tag = "0";
            }

        }


        /*
        * 选中项
        */
        private void SelectThisItem(Canvas canvas, string currItemName)
        {
            string tag = canvas.Tag.ToString();
            if (tag == "0")
            {
                Button btn = (Button)canvas.Children[0];
                Label label = (Label)canvas.Children[1];
                btn.Background = new ImageBrush
                {
                    ImageSource = new BitmapImage(new Uri("Resources/storage/ico_" + currItemName + "_active.png", UriKind.Relative))
                 ,
                    Stretch = Stretch.Uniform
                };
                label.Foreground = colorActive;
                canvas.Tag = "1";
            }
        }

        /*
         * 触摸滑动反馈终止
         */
        private void folderTreeColumn_ManipulationBoundaryFeedback(object sender, ManipulationBoundaryFeedbackEventArgs e)
        {
            e.Handled = true;
        }


        /*
        * 初始化文件文件夹树
       */
        private void StorageFile_InitFolder()
        {

            //1.初始化页面树
            StorageFileFolder storageFileFolder = storageFileFolderBll.get(1);
            if (storageFileFolder == null) return;

            TreeViewItem firstItem = new TreeViewItem();
            firstItem.IsExpanded = true;
            firstItem.Header = storageFileFolder.name;
            firstItem.Tag = storageFileFolder.id;
            firstItem.Padding = new Thickness(5);

            StorageFile_GetTreeViewItemChildren(firstItem);

            //2.初始化右击菜单
            StorageFile_InitRightClickContextMenu();

            //添加单击、右键事件
            folderTreeColumn.MouseLeftButtonUp += (object sender, MouseButtonEventArgs e) => StorageFile_TreeView_MouseLeftButtonUp(sender, e);
            folderTreeColumn.MouseRightButtonUp += (object sender, MouseButtonEventArgs e) => StorageVideo_TreeView_MouseRightButtonUp(sender, e);
            folderTreeColumn.Items.Clear();
            folderTreeColumn.Items.Add(firstItem);

        }

        /*
       * 获取当前页面的子页面 
       * 
       * @param TreeViewItem currItem 当前页面
      */
        private TreeViewItem StorageFile_GetTreeViewItemChildren(TreeViewItem currItem)
        {
            Int32 id = (Int32)currItem.Tag;
            List<StorageFileFolder> list = storageFileFolderBll.getByParentId(id);
            if (list == null) return null;
            foreach (StorageFileFolder one in list)
            {
                TreeViewItem child = new TreeViewItem();
                child.Header = one.name;
                child.Tag = one.id;
                child.Padding = new Thickness(5);
                // child.Foreground = Brushes.White;

                currItem.Items.Add(child);
                StorageFile_GetTreeViewItemChildren(child);
            }
            return currItem;
        }
        /*
     * 初始化右击菜单
     */
        private void StorageFile_InitRightClickContextMenu()
        {

            ContextMenu contextMenu = new ContextMenu();
            contextMenu.Name = "rightClickContextMenu";

            //1.新建页面
            MenuItem item1 = new MenuItem();
            item1.Name = "NewPage";
            item1.Header = "新建文件夹";
            item1.Click += (object sender, RoutedEventArgs e) => StorageFile_NewFolderClick(sender, e);


            //2.移动页面
            MenuItem item2 = new MenuItem();
            item2.Name = "MovePage";
            item2.Header = "移动文件夹";
            MenuItem item2_1 = new MenuItem();
            item2_1.Name = "MoveUp";
            item2_1.Header = "向上";
            //     item2_1.Click += pageMoveUpClick;
            MenuItem item2_2 = new MenuItem();
            item2_2.Name = "MoveDown";
            item2_2.Header = "向下";
            //item2_2.Click += pageMoveDownClick;
            //MenuItem item2_3 = new MenuItem(); 
            //item2_3.Header = "指定位置"; 
            item2.Items.Add(item2_1);
            item2.Items.Add(item2_2);
            //item2.Items.Add(item2_3);


            //2.重命名
            MenuItem item3 = new MenuItem();
            item3.Header = "重命名";
            item3.Click += StorageFile_RenameClick;

            //5.删除页面
            MenuItem item5 = new MenuItem();
            item5.Name = "DeleteFolder";
            item5.Header = "删除文件夹";
            item5.Click += (object sender, RoutedEventArgs e) => StorageFile_DeleteFolderClick(sender, e);


            contextMenu.Items.Add(item1);
            contextMenu.Items.Add(item2);
            contextMenu.Items.Add(item3);
            contextMenu.Items.Add(item5);

            folderTreeColumn.ContextMenu = contextMenu;
        }
        /*
       * 新建文件夹
       */
        private void StorageFile_NewFolderClick(object sender, RoutedEventArgs e)
        {
            StorageFileNewFolderWindow win = new StorageFileNewFolderWindow(folderTreeColumn, rightClickFolderId);
            win.ShowDialog();
        }

        /*
        * 重命名文件夹
        */
        private void StorageFile_RenameClick(object sender, RoutedEventArgs e)
        {
            StorageFileRenameFolderWindow win = new StorageFileRenameFolderWindow(folderTreeColumn, rightClickFolderId);
            win.ShowDialog();
        }

        /*
         * 删除页面 
         */
        private void StorageFile_DeleteFolderClick(object sender, RoutedEventArgs e)
        {
            StorageFileDeleteFolderWindow win = new StorageFileDeleteFolderWindow(folderTreeColumn, rightClickFolderId);
            win.ShowDialog();
        }
        /*
     * 单击页面树，显示当前页面内容
     */
        private void StorageFile_TreeView_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (e.Source.GetType().Name != "TreeViewItem")
            {
                e.Handled = true;
                return;
            }

            try
            {
                TreeViewItem treeViewItem = (TreeViewItem)e.Source;
                Object tag = treeViewItem.Tag;
                if (tag != null)
                {

                    Int32 folderId = Int32.Parse(tag.ToString());
                    FileItemPage fileItemPage = new FileItemPage(folderId);
                    mainFrame.Navigate(fileItemPage);
                    FrameUtil.RemoveBackEntry(mainFrame);
                }
                else
                {
                    MessageBox.Show("没有tag:" + treeViewItem.Header.ToString());
                }

            }
            catch (ArgumentNullException)
            {
                MessageBox.Show("页面不能为空");
            }
            catch (ArgumentException)
            {
                MessageBox.Show("页面地址异常");
            }
            catch (UriFormatException)
            {
                MessageBox.Show("页面地址格式化异常");
            }
            catch (Exception e4)
            {
                MessageBox.Show("找不到此页面" + e4.Message.ToString() + e4.StackTrace);
            }

        }


        /*
         * 初始化图片文件夹树
        */
        private void StorageImage_InitFolder()
        {
            //1.初始化页面树
            StorageImageFolder storageImageFolder = storageImageFolderBll.get(1);
            if (storageImageFolder == null) return;

            TreeViewItem firstItem = new TreeViewItem();
            firstItem.IsExpanded = true;
            firstItem.Header = storageImageFolder.name;
            firstItem.Tag = storageImageFolder.id;
            firstItem.Padding = new Thickness(5);

            StorageImage_GetTreeViewItemChildren(firstItem);

            //2.初始化右击菜单
            StorageImage_InitRightClickContextMenu();

            //添加单击、右键事件
            folderTreeColumn.MouseLeftButtonUp += (object sender, MouseButtonEventArgs e) => StorageImage_TreeView_MouseLeftButtonUp(sender, e);
            folderTreeColumn.MouseRightButtonUp += (object sender, MouseButtonEventArgs e) => StorageVideo_TreeView_MouseRightButtonUp(sender, e);
            folderTreeColumn.Items.Clear();
            folderTreeColumn.Items.Add(firstItem);

        }
        /*
         * 获取当前页面的子页面 
         * 
         * @param TreeViewItem currItem 当前页面
        */
        private TreeViewItem StorageImage_GetTreeViewItemChildren(TreeViewItem currItem)
        {
            Int32 id = (Int32)currItem.Tag;
            List<StorageImageFolder> list = storageImageFolderBll.getByParentId(id);
            if (list == null) return null;
            foreach (StorageImageFolder one in list)
            {
                TreeViewItem child = new TreeViewItem();
                child.Header = one.name;
                child.Tag = one.id;
                child.Padding = new Thickness(5);
                // child.Foreground = Brushes.White;

                currItem.Items.Add(child);
                StorageImage_GetTreeViewItemChildren(child);
            }
            return currItem;
        }

        /*
       * 初始化右击菜单
       */
        private void StorageImage_InitRightClickContextMenu()
        {

            ContextMenu contextMenu = new ContextMenu();
            contextMenu.Name = "rightClickContextMenu";

            //1.新建页面
            MenuItem item1 = new MenuItem();
            item1.Name = "NewPage";
            item1.Header = "新建文件夹";
            item1.Click += (object sender, RoutedEventArgs e) => StorageImage_NewFolderClick(sender, e);


            //2.移动页面
            MenuItem item2 = new MenuItem();
            item2.Name = "MovePage";
            item2.Header = "移动文件夹";
            MenuItem item2_1 = new MenuItem();
            item2_1.Name = "MoveUp";
            item2_1.Header = "向上";
            //     item2_1.Click += pageMoveUpClick;
            MenuItem item2_2 = new MenuItem();
            item2_2.Name = "MoveDown";
            item2_2.Header = "向下";
            //item2_2.Click += pageMoveDownClick;
            //MenuItem item2_3 = new MenuItem(); 
            //item2_3.Header = "指定位置"; 
            item2.Items.Add(item2_1);
            item2.Items.Add(item2_2);
            //item2.Items.Add(item2_3);


            //2.重命名
            MenuItem item3 = new MenuItem();
            item3.Header = "重命名";
            item3.Click += StorageImage_RenameClick;

            //5.删除页面
            MenuItem item5 = new MenuItem();
            item5.Name = "DeleteFolder";
            item5.Header = "删除文件夹";
            item5.Click += (object sender, RoutedEventArgs e) => StorageImage_DeleteFolderClick(sender, e);


            contextMenu.Items.Add(item1);
            contextMenu.Items.Add(item2);
            contextMenu.Items.Add(item3);
            contextMenu.Items.Add(item5);

            folderTreeColumn.ContextMenu = contextMenu;
        }

        /*
        * 新建文件夹
        */
        private void StorageImage_NewFolderClick(object sender, RoutedEventArgs e)
        {
            StorageImageNewFolderWindow win = new StorageImageNewFolderWindow(folderTreeColumn, rightClickFolderId);
            win.ShowDialog();
        }

        /*
        * 重命名文件夹
        */
        private void StorageImage_RenameClick(object sender, RoutedEventArgs e)
        {
            StorageImageRenameFolderWindow win = new StorageImageRenameFolderWindow(folderTreeColumn, rightClickFolderId);
            win.ShowDialog();
        }

        /*
         * 删除页面 
         */
        private void StorageImage_DeleteFolderClick(object sender, RoutedEventArgs e)
        {
            StorageImageDeleteFolderWindow win = new StorageImageDeleteFolderWindow(folderTreeColumn, rightClickFolderId);
            win.ShowDialog();
        }

        /*
       * 单击页面树，显示当前页面内容
       */
        private void StorageImage_TreeView_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (e.Source.GetType().Name != "TreeViewItem")
            {
                e.Handled = true;
                return;
            }

            try
            {
                TreeViewItem treeViewItem = (TreeViewItem)e.Source;
                Object tag = treeViewItem.Tag;
                if (tag != null)
                {

                    Int32 folderId = Int32.Parse(tag.ToString());
                    ImageItemPage imageItemPage = new ImageItemPage(folderId);
                    mainFrame.Navigate(imageItemPage);
                    FrameUtil.RemoveBackEntry(mainFrame);
                }
                else
                {
                    MessageBox.Show("没有tag:" + treeViewItem.Header.ToString());
                }

            }
            catch (ArgumentNullException)
            {
                MessageBox.Show("页面不能为空");
            }
            catch (ArgumentException)
            {
                MessageBox.Show("页面地址异常");
            }
            catch (UriFormatException)
            {
                MessageBox.Show("页面地址格式化异常");
            }
            catch (Exception e4)
            {
                MessageBox.Show("找不到此页面" + e4.Message.ToString() + e4.StackTrace);
            }

        }




        /*
         * 初始化视频文件夹树
         */
        private void StorageVideo_InitFolder()
        {
            //1.初始化页面树
            StorageVideoFolder storageVideoFolder = storageVideoFolderBll.get(1);
            if (storageVideoFolder == null) return;

            TreeViewItem firstItem = new TreeViewItem();
            firstItem.IsExpanded = true;
            firstItem.Header = storageVideoFolder.name;
            firstItem.Tag = storageVideoFolder.id;
            firstItem.Padding = new Thickness(5);

            StorageVideo_GetTreeViewItemChildren(firstItem);

            //2.初始化右击菜单
            StorageVideo_InitRightClickContextMenu();

            //添加单击、右键事件
            folderTreeColumn.MouseLeftButtonUp += (object sender, MouseButtonEventArgs e) => StorageVideo_TreeView_MouseLeftButtonUp(sender, e);
            folderTreeColumn.MouseRightButtonUp += (object sender, MouseButtonEventArgs e) => StorageVideo_TreeView_MouseRightButtonUp(sender, e);
            folderTreeColumn.Items.Clear();
            folderTreeColumn.Items.Add(firstItem);

        }

        /*
        * 获取当前页面的子页面 
        * 
        * @param TreeViewItem currItem 当前页面
        */
        private TreeViewItem StorageVideo_GetTreeViewItemChildren(TreeViewItem currItem)
        {
            Int32 id = (Int32)currItem.Tag;

            List<StorageVideoFolder> list = storageVideoFolderBll.getByParentId(id);
            if (list == null) return null;
            foreach (StorageVideoFolder one in list)
            {
                TreeViewItem child = new TreeViewItem();
                child.Header = one.name;
                child.Tag = one.id;
                child.Padding = new Thickness(5);
                // child.Foreground = Brushes.White;

                currItem.Items.Add(child);
                StorageVideo_GetTreeViewItemChildren(child);
            }
            return currItem;
        }

        /*
        * 初始化右击菜单
        */
        private void StorageVideo_InitRightClickContextMenu()
        {

            ContextMenu contextMenu = new ContextMenu();
            contextMenu.Name = "rightClickContextMenu";

            //1.新建页面
            MenuItem item1 = new MenuItem();
            item1.Name = "NewPage";
            item1.Header = "新建文件夹";
            item1.Click += (object sender, RoutedEventArgs e) => StorageVideo_NewFolderClick(sender, e);


            //2.移动页面
            MenuItem item2 = new MenuItem();
            item2.Name = "MovePage";
            item2.Header = "移动文件夹";
            MenuItem item2_1 = new MenuItem();
            item2_1.Name = "MoveUp";
            item2_1.Header = "向上";
            //     item2_1.Click += pageMoveUpClick;
            MenuItem item2_2 = new MenuItem();
            item2_2.Name = "MoveDown";
            item2_2.Header = "向下";
            //item2_2.Click += pageMoveDownClick;
            //MenuItem item2_3 = new MenuItem(); 
            //item2_3.Header = "指定位置"; 
            item2.Items.Add(item2_1);
            item2.Items.Add(item2_2);
            //item2.Items.Add(item2_3);


            //2.重命名
            MenuItem item3 = new MenuItem();
            item3.Header = "重命名";
            item3.Click += StorageVideo_RenameClick;

            //5.删除页面
            MenuItem item5 = new MenuItem();
            item5.Name = "DeleteFolder";
            item5.Header = "删除文件夹";
            item5.Click += (object sender, RoutedEventArgs e) => StorageVideo_DeleteFolderClick(sender, e);


            contextMenu.Items.Add(item1);
            contextMenu.Items.Add(item2);
            contextMenu.Items.Add(item3);
            contextMenu.Items.Add(item5);

            folderTreeColumn.ContextMenu = contextMenu;
        }

        /*
         * 重命名文件夹
         */
        private void StorageVideo_RenameClick(object sender, RoutedEventArgs e)
        {
            StorageVideoRenameFolderWindow win = new StorageVideoRenameFolderWindow(folderTreeColumn, rightClickFolderId);
            win.ShowDialog();
        }

        /*
         * 删除页面 
         */
        private void StorageVideo_DeleteFolderClick(object sender, RoutedEventArgs e)
        {
            StorageVideoDeleteFolderWindow win = new StorageVideoDeleteFolderWindow(folderTreeColumn, rightClickFolderId);
            win.ShowDialog();
        }

        /*
         * 新建文件夹
         */
        private void StorageVideo_NewFolderClick(object sender, RoutedEventArgs e)
        {
            StorageVideoNewFolderWindow win = new StorageVideoNewFolderWindow(folderTreeColumn, rightClickFolderId);
            win.ShowDialog();
        }


        /*
        * 单击页面树，显示当前页面内容
        */
        private void StorageVideo_TreeView_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (e.Source.GetType().Name != "TreeViewItem")
            {
                e.Handled = true;
                return;
            }

            try
            {
                TreeViewItem treeViewItem = (TreeViewItem)e.Source;
                Object tag = treeViewItem.Tag;
                if (tag != null)
                {

                    Int32 folderId = Int32.Parse(tag.ToString());
                    VideoItemPage videoItemPage = new VideoItemPage(folderId);
                    mainFrame.Navigate(videoItemPage);
                    FrameUtil.RemoveBackEntry(mainFrame);

                }
                else
                {
                    MessageBox.Show("没有tag:" + treeViewItem.Header.ToString());
                }

            }
            catch (ArgumentNullException)
            {
                MessageBox.Show("页面不能为空");
            }
            catch (ArgumentException)
            {
                MessageBox.Show("页面地址异常");
            }
            catch (UriFormatException)
            {
                MessageBox.Show("页面地址格式化异常");
            }
            catch (Exception e4)
            {
                MessageBox.Show("找不到此页面" + e4.Message.ToString() + e4.StackTrace);
            }

        }

        /*
       *右击树，显示菜单
       */
        private void StorageVideo_TreeView_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            // MessageBox.Show(e.Source.GetType().Name);
            if (e.Source.GetType().Name != "TreeViewItem")
            {
                e.Handled = true;
                return;
            }
            //  object or =   e.Source;
            try
            {
                TreeViewItem treeViewItem = (TreeViewItem)e.Source;
                rightClickTreeViewItem = treeViewItem;
                Object tag = treeViewItem.Tag;
                if (tag != null)
                {
                    Int32 folderId = Int32.Parse(tag.ToString());
                    rightClickFolderId = folderId;
                    treeViewItem.Focus();
                    folderTreeColumn.ContextMenu.IsOpen = true;


                    ItemCollection firstItems = folderTreeColumn.ContextMenu.Items;
                    FrameworkElementUtil.enableAllMenuItem(firstItems);
                    if (folderId == 1)
                    {
                        //   MenuItem NewAfterPage = FrameworkElementUtil.getByName(firstItems, "NewAfterPage");
                        MenuItem MoveUp = FrameworkElementUtil.getByName(firstItems, "MoveUp");
                        MenuItem MoveDown = FrameworkElementUtil.getByName(firstItems, "MoveDown");
                        MenuItem DeleteFolder = FrameworkElementUtil.getByName(firstItems, "DeleteFolder");
                        //    NewAfterPage.IsEnabled = false;
                        MoveUp.IsEnabled = false;
                        MoveDown.IsEnabled = false;
                        DeleteFolder.IsEnabled = false;
                    }

                }
                else
                {
                    MessageBox.Show("没有tag:" + treeViewItem.Header.ToString());
                }
            }
            catch (ArgumentNullException)
            {
                MessageBox.Show("页面不能为空");
            }
            catch (ArgumentException)
            {
                MessageBox.Show("页面地址异常");
            }
            catch (UriFormatException)
            {
                MessageBox.Show("页面地址格式化异常");
            }
            catch (Exception e4)
            {
                MessageBox.Show("找不到此页面" + e4.Message.ToString() + e4.StackTrace);
            }


        }

    }
}
