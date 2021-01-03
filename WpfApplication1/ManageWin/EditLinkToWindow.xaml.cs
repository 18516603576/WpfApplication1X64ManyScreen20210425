using Bll;
using Model;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfApplication1.ManageWin
{
    /// <summary>
    /// EditLinkToWindow.xaml 的交互逻辑
    /// </summary>
    public partial class EditLinkToWindow : Window
    {
        private readonly DPageBll dPageBll = new DPageBll();
        private readonly DControlBll dControlBll = new DControlBll();
        private readonly FrameworkElement currElement;
        private readonly DControl currDControl;
        private readonly ScreenCfgBll screenCfgBll = new ScreenCfgBll();
        private readonly DControlEventBll dControlEventBll = new DControlEventBll();



        public EditLinkToWindow(Frame mainFrame, FrameworkElement currElement)
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            this.currElement = currElement;
            currDControl = (DControl)currElement.Tag;

            this.loadPageData();
            Submit_Button.Click += Submit_Button_Click; 
        }

        

        private void loadPageData() {
            List<ScreenCfg> screenCfgList = screenCfgBll.findAll();
            TabControl tabControl = new TabControl();
            tabControl.Width = 900;
            tabControl.Height = 500;
            tabControl.TabStripPlacement = Dock.Top;
            tabControl.BorderThickness = new Thickness(0, 1, 0, 0);
            tabControl.Padding = new Thickness(0, 0, 0, 0); 
            tabControl.HorizontalAlignment = HorizontalAlignment.Left;
            tabControl.VerticalAlignment = VerticalAlignment.Top;

            foreach(ScreenCfg sc in screenCfgList)
            {
                DControlEvent dControlEvent = dControlEventBll.getByDControlIdScreenCfgId(currDControl.id, sc.id);

                TabItem tabItem = new TabItem();
                tabItem.Header = sc.diyName;
                tabItem.Tag = sc.id;
                tabItem.Padding = new Thickness(15,5,15, 5);

                Grid grid = new Grid();
                grid.Background = Brushes.White;
                tabItem.Content = grid;

                //1.选择页面
                Label label1 = new Label();
                label1.Content = "请选择页面：";
                label1.Margin = new Thickness(127, 51, 0, 0);
                label1.Width = 110;
                label1.Height = 30;
                label1.HorizontalAlignment = HorizontalAlignment.Left;
                label1.VerticalAlignment = VerticalAlignment.Top;
                grid.Children.Add(label1);
               
                ComboBox comboBoxLinkToPageId = new ComboBox();
                comboBoxLinkToPageId.Name = "linkToPageId" + sc.id; 
                comboBoxLinkToPageId.Margin = new Thickness(135, 80, 0, 0);
                comboBoxLinkToPageId.Width = 250;
                comboBoxLinkToPageId.Height = 30;
                comboBoxLinkToPageId.HorizontalAlignment = HorizontalAlignment.Left;
                comboBoxLinkToPageId.VerticalAlignment = VerticalAlignment.Top;
                comboBoxLinkToPageId.VerticalContentAlignment = VerticalAlignment.Center;
                grid.Children.Add(comboBoxLinkToPageId);


                //2.显示位置 
                Label label2= new Label();
                label2.Content = "显示位置：";
                label2.Margin = new Thickness(135, 142, 0, 0);
                label2.Width = 110;
                label2.Height = 30;
                label2.HorizontalAlignment = HorizontalAlignment.Left;
                label2.VerticalAlignment = VerticalAlignment.Top;
                grid.Children.Add(label2);
                 
                ComboBox comboBoxShowInWhichCFrame = new ComboBox();
                comboBoxShowInWhichCFrame.Name = "showInWhichCFrame" + sc.id;
                comboBoxShowInWhichCFrame.Margin = new Thickness(135, 176, 0, 0);
                comboBoxShowInWhichCFrame.MinWidth = 200;
                comboBoxShowInWhichCFrame.Height = 30;
                comboBoxShowInWhichCFrame.HorizontalAlignment = HorizontalAlignment.Left;
                comboBoxShowInWhichCFrame.VerticalAlignment = VerticalAlignment.Top;
                comboBoxShowInWhichCFrame.VerticalContentAlignment = VerticalAlignment.Center;
                grid.Children.Add(comboBoxShowInWhichCFrame);

                //3.弹窗
                CheckBox checkBoxIsDialogLink = new CheckBox();
                checkBoxIsDialogLink.Name = "isDialogLink" + sc.id;
                checkBoxIsDialogLink.Margin = new Thickness(420, 90, 0, 0);
                checkBoxIsDialogLink.Content = "弹窗";
                checkBoxIsDialogLink.HorizontalAlignment = HorizontalAlignment.Left;
                checkBoxIsDialogLink.VerticalAlignment = VerticalAlignment.Top;
                grid.Children.Add(checkBoxIsDialogLink);

                CheckBox checkBoxIsTransparentDialog = new CheckBox();
                checkBoxIsTransparentDialog.Name = "isTransparentDialog" + sc.id;
                checkBoxIsTransparentDialog.Margin = new Thickness(520, 90, 0, 0);
                checkBoxIsTransparentDialog.Content = "透明弹窗";
                checkBoxIsTransparentDialog.HorizontalAlignment = HorizontalAlignment.Left;
                checkBoxIsTransparentDialog.VerticalAlignment = VerticalAlignment.Top;
                grid.Children.Add(checkBoxIsTransparentDialog); 


                int linkToPageIdVal = dControlEvent == null ? 0 : dControlEvent.linkToPageId;
                int showInWhichCFrameVal = dControlEvent == null ? 0 : dControlEvent.showInWhichCFrame;
                bool isTransparentDialogVal = dControlEvent == null ? false : dControlEvent.isTransparentDialog;
                bool isDialogLinkVal = dControlEvent == null ? false : dControlEvent.isDialogLink;
                //如果不是弹窗，则显示选择的页面 
                 
                this.initCombox_linkToPageId(comboBoxLinkToPageId, linkToPageIdVal);
                this.initCombox_showInWhichCFrame(comboBoxShowInWhichCFrame, showInWhichCFrameVal);
                checkBoxIsTransparentDialog.IsChecked = isTransparentDialogVal;
                checkBoxIsDialogLink.IsChecked = isDialogLinkVal;


                tabControl.Items.Add(tabItem); 
            }
            mainGrid.Children.Add(tabControl); 
        }
         
      
        private void Submit_Button_Click(object sender, RoutedEventArgs e)
        {
            /*
             *  获取不同屏幕下 linkToPageId,showInWhichCFrame的值
             *  
             *  如果数据库中有数据行则更新，没有则插入
             */
            foreach(FrameworkElement ele in mainGrid.Children)
            {
                if (ele is TabControl == false) continue;
                TabControl tabControl = (TabControl)ele;
                foreach(TabItem tabItem in tabControl.Items)
                {
                    int screenCfgId = Convert.ToInt32(tabItem.Tag);
                    int linToPageIdVal = 0;
                    int showInWhichCFrameVal = 0;
                    bool isTransparentDialogVal = false;
                    bool isDialogLinkVal = false;
                    Grid grid = (Grid)tabItem.Content;
                    foreach(FrameworkElement fe in grid.Children)
                    {
                        if(fe.Name == "linkToPageId" + screenCfgId)
                        {
                            ComboBox comboBoxLinkToPageId = (ComboBox)fe;
                            ComboBoxItem item = (ComboBoxItem)comboBoxLinkToPageId.SelectedItem; 
                            if (item != null)
                            {
                                linToPageIdVal = (int)item.Tag;
                            }
                        }

                        if (fe.Name == "showInWhichCFrame" + screenCfgId)
                        {
                            ComboBox comboBoxShowInWhichCFrame = (ComboBox)fe;
                            ComboBoxItem item = (ComboBoxItem)comboBoxShowInWhichCFrame.SelectedItem; 
                            if (item != null)
                            {
                                showInWhichCFrameVal = (int)item.Tag;
                            }
                        }

                        if (fe.Name == "isTransparentDialog" + screenCfgId)
                        {
                            CheckBox checkBoxIsTransparentDialog = (CheckBox)fe;
                            isTransparentDialogVal = (Boolean)checkBoxIsTransparentDialog.IsChecked;
                        }

                        if (fe.Name == "isDialogLink" + screenCfgId)
                        {
                            CheckBox checkBoxIsDialogLink = (CheckBox)fe;
                            isDialogLinkVal = (Boolean)checkBoxIsDialogLink.IsChecked;
                        }
                    }


                    DControlEvent dControlEvent = dControlEventBll.getByDControlIdScreenCfgId(currDControl.id, screenCfgId); 
                    if (dControlEvent != null)
                    {
                         dControlEvent.linkToPageId = linToPageIdVal;
                         dControlEvent.showInWhichCFrame = showInWhichCFrameVal; 
                         dControlEvent.isDialogLink = isDialogLinkVal;
                         dControlEvent.isTransparentDialog = isTransparentDialogVal;
                         
                         dControlEventBll.update(dControlEvent);
                    }
                    else
                    {
                        dControlEvent = new DControlEvent();
                        dControlEvent.dControlId = currDControl.id;
                        dControlEvent.screenCfgId = screenCfgId;
                        dControlEvent.linkToPageId = linToPageIdVal;
                        dControlEvent.showInWhichCFrame = showInWhichCFrameVal;
                        dControlEvent.isDialogLink = isDialogLinkVal;
                        dControlEvent.isTransparentDialog = isTransparentDialogVal;
                        dControlEventBll.insert(dControlEvent);
                    }

                }

            } 

            Close();
        }
        /*
         * 初始化页面列表
         */
        private void initCombox_linkToPageId(ComboBox comboBoxLinkToPageId, int currLinkToPageId)
        {
            DPage firstDPage = dPageBll.get(1); 

            ComboBoxItem defaultItem = new ComboBoxItem();
            defaultItem.Content = "-- 请选择 --";
            defaultItem.Tag = 0;
            if (0 == currLinkToPageId)
            {
                defaultItem.IsSelected = true;
            }
            comboBoxLinkToPageId.Items.Add(defaultItem);

            ComboBoxItem firstItem = new ComboBoxItem();
            firstItem.Content = firstDPage.name;
            firstItem.Tag = firstDPage.id;
            if (firstDPage.id == currLinkToPageId)
            {
                firstItem.IsSelected = true;
            }
            comboBoxLinkToPageId.Items.Add(firstItem);

            int lv = 0;
            this.getTreeViewItemChildren(comboBoxLinkToPageId, firstItem, lv, currLinkToPageId);
        }
        /*
        * 获取当前页面的子页面 
        * 
        * @param TreeViewItem currItem 当前页面
        */
        private void getTreeViewItemChildren(ComboBox comboBoxLinkToPageId, ComboBoxItem currItem, int lv, int currLinkToPageId)
        {
            lv = lv + 1;
            Int32 id = (Int32)currItem.Tag;
            List<DPage> list = dPageBll.getByParentId(id);
            if (list == null) return;
            foreach (DPage one in list)
            {
                ComboBoxItem child = new ComboBoxItem();
                child.Content = one.name;
                child.Tag = one.id;
                child.Padding = new Thickness(20 * lv, 0, 0, 0);
                if (one.id == currLinkToPageId)
                {
                    child.IsSelected = true;
                }
                comboBoxLinkToPageId.Items.Add(child);
                this.getTreeViewItemChildren(comboBoxLinkToPageId,child, lv, currLinkToPageId);

            }
        }

        /*
       * 初始化frame列表
       */
        private void initCombox_showInWhichCFrame(ComboBox comboBoxShowInWhichCFrame, int currShowInWhichCFrame)
        {
            //所有相关的窗口
            List<List<DControl>> lists = new List<List<DControl>>();

            //1.获取当前页面下的CFrame
            List<DControl> dContorlList1 = dControlBll.getCFrameByPageId(currDControl.pageId);
            lists.Add(dContorlList1); 

            //2.获取当前控件所在页面currDControl.pageId
            //获取链接到currDControl.pageId 的窗口CFrame，并追加到lists
            fillParent(currDControl.pageId, lists);

            //3.获取当前控件所属的CFrame的编号  
            ComboBoxItem defaultItem = new ComboBoxItem();
            defaultItem.Content = "-- 默认 --";
            defaultItem.Tag = 0;
            if (currShowInWhichCFrame == 0)
            {
                defaultItem.IsSelected = true;
            }
            comboBoxShowInWhichCFrame.Items.Add(defaultItem);


            ComboBoxItem firstItem = new ComboBoxItem();
            firstItem.Content = "主窗口";
            firstItem.Tag = -1;
            if (currShowInWhichCFrame == -1)
            {
                firstItem.IsSelected = true;
            }
            comboBoxShowInWhichCFrame.Items.Add(firstItem);

            int lv = 0;
            for (int i = lists.Count - 1; i >= 0; i--)
            {
                List<DControl> tmp = lists[i];
                lv = lv + 1;
                foreach (DControl dc in tmp)
                {
                    ComboBoxItem child = new ComboBoxItem();
                    child.Content = dc.content;
                    child.Tag = dc.id;
                    child.Padding = new Thickness(20 * lv, 0, 0, 0);
                    if (dc.id == currShowInWhichCFrame)
                    {
                        child.IsSelected = true;
                    }
                    comboBoxShowInWhichCFrame.Items.Add(child);
                }
            }

        }

        private void fillParent(int pageId, List<List<DControl>> lists)
        {
            //2.获取当前页面所在的CFrame
            //获取pageId，然后从dContorl表中找到对应CFrame
            List<DControl> dContorlList = dControlBll.getParentCFrameByLinkToPageId(pageId);
            lists.Add(dContorlList);
            foreach (DControl dc in dContorlList)
            {
                //3.获取当前CFrame的上一层CFrame
                fillParent(dc.pageId, lists);
            }
        }

    }
}
