using Bll;
using Common.util;
using Model;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WpfApplication1.PageWin;

/*
 * 左侧页面树操作
 * 
 * 添加页面，重命名
 */
namespace WpfApplication1
{
    class PageTree
    {

        private readonly DPageBll dPageBll = new DPageBll();
        private readonly DControlBll dControlBll = new DControlBll();
        //主窗口
        private readonly MainWindow mainWindow;
        //获取当前右击的页面
        private Int32 rightClickPageId = 0;
        private TreeViewItem rightClickTreeViewItem;
        //当前复制的页面
        private DPage currCopiedPage = null;
        private ContextMenu contextMenu;
        public PageTree(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
        }

        /*
         * 初始化左侧页面树
         */
        public void initLeftPageTree()
        {
            //1.初始化页面树
            Model.DPage dPage = dPageBll.get(1);
            TreeViewItem firstItem = new TreeViewItem();
            firstItem.IsExpanded = true;
            firstItem.Header = dPage.name;
            firstItem.Tag = dPage.id;
            firstItem.Padding = new Thickness(5);
            firstItem.Foreground = Brushes.White;

            getTreeViewItemChildren(firstItem);

            //2.初始化右击菜单
            initRightClickContextMenu();

            //添加单击、右键事件
            mainWindow.pageTreeColumn.MouseLeftButtonUp += (object sender, MouseButtonEventArgs e) => TreeView_MouseLeftButtonUp(sender, e);
            mainWindow.pageTreeColumn.MouseRightButtonUp += (object sender, MouseButtonEventArgs e) => TreeView_MouseRightButtonUp(sender, e);
            mainWindow.pageTreeColumn.Items.Add(firstItem);




        }

        /*
         * 初始化右击菜单
         */
        private void initRightClickContextMenu()
        {

            contextMenu = new ContextMenu();
            contextMenu.Name = "rightClickContextMenu";

            //1.新建页面
            MenuItem item1 = new MenuItem();
            item1.Name = "NewPage";
            item1.Header = "新建子页面";
            item1.Click += (object sender, RoutedEventArgs e) => newChildPageClick(sender, e);
            //MenuItem item1_1 =   new MenuItem();
            //item1_1.Header = "子页面";
            //item1_1.Name = "NewChildPage"; 
            //MenuItem item1_2 = new MenuItem();
            //item1_2.Name = "NewAfterPage";
            //item1_2.Header = "在当前页面之后";  
            //item1_2.Click += (object sender, RoutedEventArgs e) => newAfterPageClick(sender, e);
            //item1.Items.Add(item1_1);
            //item1.Items.Add(item1_2);

            //2.移动页面
            MenuItem item2 = new MenuItem();
            item2.Name = "MovePage";
            item2.Header = "移动页面";
            MenuItem item2_1 = new MenuItem();
            item2_1.Name = "MoveUp";
            item2_1.Header = "向上";
            item2_1.Click += pageMoveUpClick;
            MenuItem item2_2 = new MenuItem();
            item2_2.Name = "MoveDown";
            item2_2.Header = "向下";
            item2_2.Click += pageMoveDownClick;
            //MenuItem item2_3 = new MenuItem(); 
            //item2_3.Header = "指定位置"; 
            item2.Items.Add(item2_1);
            item2.Items.Add(item2_2);
            //item2.Items.Add(item2_3);


            //2.重命名
            MenuItem item3 = new MenuItem();
            item3.Header = "重命名";
            item3.Click += renameClick;

            //4.复制页面
            MenuItem item4 = new MenuItem();
            item4.Header = "复制页面";
            item4.Click += copyPageClick;

            //6.粘贴页面
            MenuItem item6 = new MenuItem();
            item6.Name = "PastPage";
            item6.Header = "粘贴页面";
            item6.Click += pastePageClick;


            //5.删除页面
            MenuItem item5 = new MenuItem();
            item5.Name = "DeletePage";
            item5.Header = "删除页面";
            item5.Click += (object sender, RoutedEventArgs e) => deletePage(sender, e);

            //7.属性
            MenuItem item7 = new MenuItem();
            item7.Header = "属性";
            item7.Click += attrClick;


            contextMenu.Items.Add(item1);
            contextMenu.Items.Add(item2);
            contextMenu.Items.Add(item3);
            contextMenu.Items.Add(item4);
            contextMenu.Items.Add(item6);
            contextMenu.Items.Add(item5);
            contextMenu.Items.Add(item7);

            mainWindow.pageTreeColumn.ContextMenu = contextMenu;
        }
        /*
         * 粘贴页面
         */
        private void pastePageClick(object sender, RoutedEventArgs e)
        {
            //1.粘贴页面
            DPage thatPage = dPageBll.get(rightClickPageId);
            DPage newDPage = DPageUtil.createFrom(currCopiedPage);
            newDPage.id = 0;
            newDPage.parentId = thatPage.id;
            newDPage.name = newDPage.name + "-副本";
            newDPage = dPageBll.insertChild(newDPage);

            //2.粘贴页面下的所有控件
            dControlBll.copyToPage(currCopiedPage, newDPage);

            //3.更新到树上
            TreeViewItem item = new TreeViewItem();
            item.Header = newDPage.name;
            item.Tag = newDPage.id;
            item.Padding = new Thickness(5);
            item.Foreground = Brushes.White;
            item.IsExpanded = true;
            rightClickTreeViewItem.Items.Add(item);
        }
        /*
         * 复制页面
         */
        private void copyPageClick(object sender, RoutedEventArgs e)
        {
            DPage dPage = dPageBll.get(rightClickPageId);
            currCopiedPage = dPage;
        }
        /*
         * 
         */
        private void renameClick(object sender, RoutedEventArgs e)
        {
            RenameWindow win = new RenameWindow(mainWindow.pageTreeColumn, rightClickPageId);
            win.ShowDialog();
        }
        /*
        * 移动页面 - 向上
        */
        private void pageMoveUpClick(object sender, RoutedEventArgs e)
        {
            TreeViewItem parent = (TreeViewItem)rightClickTreeViewItem.Parent;
            int index = parent.Items.IndexOf(rightClickTreeViewItem);
            if (index > 0)
            {
                TreeViewItem prevTreeViewItem = (TreeViewItem)parent.Items.GetItemAt(index - 1);
                int prevPageId = int.Parse(prevTreeViewItem.Tag.ToString());
                parent.Items.Remove(rightClickTreeViewItem);
                parent.Items.Insert(index - 1, rightClickTreeViewItem);
                rightClickTreeViewItem.IsSelected = true;

                dPageBll.moveUp(rightClickPageId, prevPageId);
            }


        }

        /*
         * 移动页面 - 向下
         */
        private void pageMoveDownClick(object sender, RoutedEventArgs e)
        {
            TreeViewItem parent = (TreeViewItem)rightClickTreeViewItem.Parent;
            int index = parent.Items.IndexOf(rightClickTreeViewItem);

            if (index < parent.Items.Count - 1)
            {
                TreeViewItem nextTreeViewItem = (TreeViewItem)parent.Items.GetItemAt(index + 1);
                int nextPageId = int.Parse(nextTreeViewItem.Tag.ToString());
                parent.Items.Remove(rightClickTreeViewItem);
                parent.Items.Insert(index + 1, rightClickTreeViewItem);
                rightClickTreeViewItem.IsSelected = true;

                dPageBll.moveDown(rightClickPageId, nextPageId);
            }


        }


        /*
         * 新建页面 - 子页面
         */
        private void newChildPageClick(object sender, RoutedEventArgs e)
        {
            NewPageWindow newPageWindow = new NewPageWindow(mainWindow.pageTreeColumn, rightClickPageId);
            newPageWindow.ShowDialog();
        }

        /*
         * 新建页面 - 在当前页面之前
         */
        private void newAfterPageClick(object sender, RoutedEventArgs e)
        {
            NewAfterPageWindow newPageWindow = new NewAfterPageWindow(mainWindow.pageTreeColumn, rightClickPageId);
            newPageWindow.ShowDialog();
        }

        /*
         * 删除页面
         */
        private void deletePage(object sender, RoutedEventArgs e)
        {
            DeletePageWindow deletePageWindow = new DeletePageWindow(mainWindow.pageTreeColumn, rightClickPageId);
            deletePageWindow.ShowDialog();
        }
        /*
        * 属性
        */
        private void attrClick(object sender, RoutedEventArgs e)
        {
            PageAttrWindow win = new PageAttrWindow(mainWindow.mainFrame, mainWindow.pageTreeColumn, rightClickPageId);
            win.ShowDialog();
        }

        /*
         * 获取当前页面的子页面 
         * 
         * @param TreeViewItem currItem 当前页面
         */
        private TreeViewItem getTreeViewItemChildren(TreeViewItem currItem)
        {
            Int32 id = (Int32)currItem.Tag;

            List<Model.DPage> list = dPageBll.getByParentId(id);
            if (list == null) return null;
            foreach (DPage one in list)
            {
                TreeViewItem child = new TreeViewItem();
                child.Header = one.name;
                child.Tag = one.id;
                child.Padding = new Thickness(5);
                child.Foreground = Brushes.White;
                //添加单击、右键事件
                // child.MouseLeftButtonUp += (object sender, MouseButtonEventArgs e) => TreeView_MouseLeftButtonUp(sender, e);
                //child.MouseRightButtonUp += (object sender, MouseButtonEventArgs e) => TreeView_MouseRightButtonUp(sender, e);
                currItem.Items.Add(child);
                getTreeViewItemChildren(child);
            }
            return currItem;
        }


        /*
        *右击树，显示菜单
        */
        private void TreeView_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
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
                    Int32 pageId = Int32.Parse(tag.ToString());
                    rightClickPageId = pageId;
                    treeViewItem.Focus();
                    mainWindow.pageTreeColumn.ContextMenu.IsOpen = true;


                    ItemCollection firstItems = mainWindow.pageTreeColumn.ContextMenu.Items;
                    FrameworkElementUtil.enableAllMenuItem(firstItems);
                    if (pageId == 1)
                    {
                        //  MenuItem NewAfterPage = FrameworkElementUtil.getByName(firstItems, "NewAfterPage");
                        MenuItem MoveUp = FrameworkElementUtil.getByName(firstItems, "MoveUp");
                        MenuItem MoveDown = FrameworkElementUtil.getByName(firstItems, "MoveDown");
                        MenuItem DeletePage = FrameworkElementUtil.getByName(firstItems, "DeletePage");
                        //    NewAfterPage.IsEnabled = false;
                        MoveUp.IsEnabled = false;
                        MoveDown.IsEnabled = false;
                        DeletePage.IsEnabled = false;
                    }
                    if (currCopiedPage == null)
                    {
                        MenuItem PastPage = FrameworkElementUtil.getByName(firstItems, "PastPage");
                        PastPage.IsEnabled = false;
                    }

                    // e.Handled = true;
                    //MessageBox.Show(pageId.ToString());
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
         * 单击页面树，显示当前页面内容
         */
        private void TreeView_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
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
                    Int32 pageId = Int32.Parse(tag.ToString());
                    DPage dPage = dPageBll.get(pageId);
                    int frameWidth = App.localStorage.cfg.screenWidth;
                    int frameHeight = App.localStorage.cfg.screenHeight;
                    if (dPage.width > 0) frameWidth = dPage.width;
                    if (dPage.height > 0) frameHeight = dPage.height;
                    mainWindow.mainFrame.Width = frameWidth;
                    mainWindow.mainFrame.Height = frameHeight;
                    mainWindow.mainFrame.UpdateLayout();
                    mainWindow.changeMainFramePercent(App.localStorage.cfg.pagePercent);

                    PageTemplate page = new PageTemplate(mainWindow.mainFrame, pageId, mainWindow.mqServer);
                    App.localStorage.currPageId = pageId;
                    mainWindow.pageTemplate.NavigationService.Navigate(page);
                    mainWindow.pageTemplate = page;
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
