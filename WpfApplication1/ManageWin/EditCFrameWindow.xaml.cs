using Bll;
using Model;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using WpfApplication1.control;

namespace WpfApplication1.ManageWin
{
    /// <summary>
    /// EditCFrameWindow.xaml 的交互逻辑
    /// </summary>
    public partial class EditCFrameWindow : Window
    {
        private readonly DPageBll dPageBll = new DPageBll();
        private readonly DControlBll dControlBll = new DControlBll();
        private readonly FrameworkElement currElement;
        private readonly DControl currDControl;
        private readonly Frame mainFrame;

        public EditCFrameWindow(Frame mainFrame, DPage dPage, FrameworkElement currElement)
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.mainFrame = mainFrame;
            this.currElement = currElement;
            currDControl = (DControl)currElement.Tag;
            initCombox(currDControl.pageId, currDControl.linkToPageId);
        }

        /*
         * 初始化页面列表
         */
        private void initCombox(int pageId, int currLinkToPageId)
        {
            DPage firstDPage = dPageBll.get(1);
            //int currLinkToPageId = 2;

            ComboBoxItem defaultItem = new ComboBoxItem();
            defaultItem.Content = "-- 请选择 --";
            defaultItem.Tag = 0;
            if (0 == currLinkToPageId)
            {
                defaultItem.IsSelected = true;
            }
            linkToPageId.Items.Add(defaultItem);

            ComboBoxItem firstItem = new ComboBoxItem();
            firstItem.Content = firstDPage.name;
            firstItem.Tag = firstDPage.id;
            if (firstDPage.id == currLinkToPageId)
            {
                firstItem.IsSelected = true;
            }
            if (firstDPage.id == pageId)
            {
                //归属页面，不可选
                firstItem.IsEnabled = false;
            }
            linkToPageId.Items.Add(firstItem);

            int lv = 0;
            getTreeViewItemChildren(firstItem, lv, currLinkToPageId, pageId);
        }

        private void Submit_Button_Click(object sender, RoutedEventArgs e)
        {
            ComboBoxItem item = (ComboBoxItem)linkToPageId.SelectedItem;
            int selectedVal = 0;
            if (item != null)
            {
                selectedVal = (int)item.Tag;
            }
            DPage dPage = dPageBll.get(selectedVal);
            if (dPage == null)
            {
                MessageBox.Show("页面不存在");
                return;
            }

            Boolean result = dControlBll.isNestedOfCurrPageId(dPage.id, currDControl.pageId);
            if (result)
            {
                MessageBox.Show("选择的页面中，嵌套了当前页面，不可用");
                return;
            }




            //更新到数据库
            DControl dControl = dControlBll.get(currDControl.id);
            dControl.linkToPageId = selectedVal;
            currElement.Tag = dControl;
            dControlBll.update(dControl);

            CFrame cFrame = (CFrame)currElement;
            //DPage dPage1 = dPageBll.get(dControl.linkToPageId);
            ////  if (dPage1 == null) return;
            //int frameWidth = App.localStorage.cfg.screenWidth;
            //int frameHeight = App.localStorage.cfg.screenHeight;
            //if (dPage1 != null && dPage1.width > 0) frameWidth = dPage1.width;
            //if (dPage1 != null && dPage1.height > 0) frameHeight = dPage1.height;
            //PageTemplate pageTemplate = new PageTemplate(mainFrame, dControl.linkToPageId);

            cFrame.updateElement(dControl, App.localStorage.cfg);

            Close();
        }


        /*
         * 指定的小窗口是否有嵌套当前页面的
         * 
         * @linkToPageId  CFrame中显示的页面id
         * 
         * @currPageId  控件所在窗口
         */
        //private Boolean isIn(int linkToPageId,int currPageId) {
        //    Boolean result = false;
        //    List<DControl> list = dControlBll.getCFrameByPageId(linkToPageId);
        //    foreach (DControl dc in list)
        //    {
        //        if (dc.linkToPageId == currPageId)
        //        {
        //            return true;
        //        }
        //        else { 
        //            result = this.isIn(dc.linkToPageId, currPageId);
        //        }
        //    }
        //    return result;
        //}

        /*
        * 获取当前页面的子页面 
        * 
        * @param TreeViewItem currItem 当前页面
        */
        private void getTreeViewItemChildren(ComboBoxItem currItem, int lv, int currLinkToPageId, int pageId)
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
                if (one.id == pageId)
                {
                    //归属页面，不可选
                    child.IsEnabled = false;
                }
                linkToPageId.Items.Add(child);
                getTreeViewItemChildren(child, lv, currLinkToPageId, pageId);

            }
        }
    }
}
