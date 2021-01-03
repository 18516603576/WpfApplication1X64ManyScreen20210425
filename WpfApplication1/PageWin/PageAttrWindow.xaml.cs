using Bll;
using Common;
using Common.Data;
using Model;
using System;
using System.Windows;
using System.Windows.Controls;

namespace WpfApplication1.PageWin
{
    /// <summary>
    /// PageAttrWindow.xaml 的交互逻辑
    /// </summary>
    public partial class PageAttrWindow : Window
    {
        private readonly DPageBll dPageBll = new DPageBll();
        //父页面id
        private readonly Int32 pageId = 0;
        //页面树
        private readonly TreeView pageTreeColumn;
        //内容区
        private readonly Frame mainFrame;
        private readonly CfgBll cfgBll = new CfgBll();

        //页面居中显示到最前面
        public PageAttrWindow(Frame mainFrame, TreeView pageTreeColumn, Int32 pageId)
        {
            InitializeComponent();
            this.mainFrame = mainFrame;
            this.pageTreeColumn = pageTreeColumn;
            this.pageId = pageId;

            DPage dPage = dPageBll.get(pageId);

            pagename.Text = dPage.name;
            width.Text = dPage.width.ToString();
            height.Text = dPage.height.ToString();

            if (dPage.pageSwitchType <= 0)
            {
                dPage.pageSwitchType = 1;
            }

            initCombox(dPage.pageSwitchType);


            WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }
        /*
     * 初始化切换方式列表
     */
        private void initCombox(int pageSwitchTypeVal)
        {
            foreach (PageSwitchTypeDto dto in Params.pageSwitchTypeList)
            {
                ComboBoxItem one = new ComboBoxItem();
                one.Content = dto.name;
                one.Tag = dto.id;
                if (dto.id == pageSwitchTypeVal)
                {
                    one.IsSelected = true;
                }
                pageSwitchType.Items.Add(one);
            }

        }


        /*
         * 保存数据
         */
        private void Button_Click(object sender, RoutedEventArgs e)
        {

            DPage dPage = dPageBll.get(pageId);
            dPage.name = pagename.Text;

            if (string.IsNullOrWhiteSpace(dPage.name))
            {
                MessageBox.Show("请填写页面名称");
                return;
            }
            //Boolean b = dPageBll.hasSameName(dPage);
            //if (b)
            //{
            //    MessageBox.Show("页面名称重复，请更换");
            //    return;
            //}

            if (string.IsNullOrWhiteSpace(width.Text))
            {
                MessageBox.Show("请填写宽度；"); return;
            }
            else if (!DataUtil.isInt(width.Text.ToString()))
            {
                MessageBox.Show("宽度必须是整数；"); return;
            }
            if (string.IsNullOrWhiteSpace(height.Text.ToString()))
            {
                MessageBox.Show("请填写高度；"); return;
            }
            else if (!DataUtil.isInt(height.Text.ToString()))
            {
                MessageBox.Show("高度必须是整数；"); return;
            }
            dPage.width = int.Parse(width.Text);
            dPage.height = int.Parse(height.Text);

            ComboBoxItem item = (ComboBoxItem)pageSwitchType.SelectedItem;
            int pageSwitchTypeVal = 0;
            if (item != null)
            {
                pageSwitchTypeVal = (int)item.Tag;
            }
            dPage.pageSwitchType = pageSwitchTypeVal;

            dPage = dPageBll.update(dPage);
            if (App.localStorage.cfg.pageSwitchType != dPage.pageSwitchType)
            {
                App.localStorage.cfg.pageSwitchType = dPage.pageSwitchType;
                cfgBll.update(App.localStorage.cfg);
            }
            updateToTree(pageTreeColumn, dPage);

            if (dPage.id == App.localStorage.currPageId)
            {
                int frameWidth = App.localStorage.cfg.screenWidth;
                int frameHeight = App.localStorage.cfg.screenHeight;
                if (dPage.width > 0) frameWidth = dPage.width;
                if (dPage.height > 0) frameHeight = dPage.height;

                mainFrame.Width = frameWidth;
                mainFrame.Height = frameHeight;
                mainFrame.UpdateLayout();
            }


            Close();

        }


        /*
        * 将新添加的页面加入到页面树中
        */
        private void updateToTree(ItemsControl tree, DPage dPage)
        {
            foreach (TreeViewItem item in tree.Items)
            {
                Int32 id = (Int32)item.Tag;
                if (pageId == id)
                {
                    item.Header = dPage.name;
                    break;
                }
                else
                {
                    updateToTree(item, dPage);
                }
            }
        }
    }
}
