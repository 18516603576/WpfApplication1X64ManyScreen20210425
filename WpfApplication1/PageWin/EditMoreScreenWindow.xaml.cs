using Bll;
using Common.Data;
using Model;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfApplication1
{
    /// <summary>
    /// NewPageWindow.xaml 的交互逻辑
    /// </summary>
    ///  
    public partial class EditMoreScreenWindow : Window
    { 
        private readonly ScreenCfgBll screenCfgBll = new ScreenCfgBll();
        private readonly DPageBll dPageBll = new DPageBll();
        private ScreenCfg screenCfg = null;


        //页面居中显示到最前面 
        public EditMoreScreenWindow(int id)
        {
             
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.loadPageData(id);
            Submit_Button.Click += Submit_Button_Click;
        }

        private void loadPageData(int id) {
             this.screenCfg =  screenCfgBll.get(id);
            if (screenCfg == null) return;
             this.id.Content = screenCfg.id;
             this.diyName.Text = screenCfg.diyName;
             this.initCombox_linkToPageId(indexPageId, screenCfg.indexPageId); 
        }



        /*
         * 保存数据
         */
        private void Submit_Button_Click(object sender, RoutedEventArgs e)
        {
            string diyNameVal = diyName.Text; 

            int indexPageIdVal = 0; 
            ComboBoxItem item = (ComboBoxItem)indexPageId.SelectedItem;
            if (item != null)
            {
                indexPageIdVal = (int)item.Tag;
            }

            if (string.IsNullOrWhiteSpace(diyNameVal))
            {
                MessageBox.Show("屏幕名称不能为空");
                return; 
            }
             

            ScreenCfg tmp = screenCfgBll.get(screenCfg.id);
            tmp.indexPageId = indexPageIdVal;
            tmp.diyName = diyNameVal;
            screenCfgBll.update(tmp);
            this.DialogResult = true;
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
                this.getTreeViewItemChildren(comboBoxLinkToPageId, child, lv, currLinkToPageId);

            }
        }
    }
}
