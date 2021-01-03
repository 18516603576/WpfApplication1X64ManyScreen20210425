using Bll;
using Common.Data;
using Model;
using Model.dto;
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
    public partial class MoreScreenWindow : Window
    { 
        private readonly ScreenCfgBll screenCfgBll = new ScreenCfgBll();



        //页面居中显示到最前面
        public MoreScreenWindow( )
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.loadPageData();
            Insert_Button.Click += Insert_Button_Click;
        }

       

        private void loadPageData() {
            this.refreshList();
        }



        /*
         * 保存数据
         */
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //DPage dPage = new DPage();
            //dPage.name = pagename.Text;
            //dPage.parentId = parentId;
            //if (string.IsNullOrWhiteSpace(dPage.name))
            //{
            //    MessageBox.Show("请填写页面名称");
            //    return;
            //}
            //else if (dPage.parentId == 0)
            //{
            //    MessageBox.Show("父页面不存在，请重试");
            //    return;
            //}
            //ComboBoxItem item = (ComboBoxItem)pageSwitchType.SelectedItem;
            //int pageSwitchTypeVal = 0;
            //if (item != null)
            //{
            //    pageSwitchTypeVal = (int)item.Tag;
            //}
            //dPage.pageSwitchType = pageSwitchTypeVal;

            //dPage = dPageBll.insertChild(dPage);
            //if (App.localStorage.cfg.pageSwitchType != dPage.pageSwitchType)
            //{
            //    App.localStorage.cfg.pageSwitchType = dPage.pageSwitchType;
            //    cfgBll.update(App.localStorage.cfg);
            //}

            //insertToTree(pageTreeColumn, dPage);
            //Close();
        }
        /*
        * 添加
        */
        private void Insert_Button_Click(object sender, RoutedEventArgs e)
        {
            InsertMoreScreenWindow win = new InsertMoreScreenWindow(); 
            bool  result = (Boolean)win.ShowDialog();
            if (result)
                this.refreshList();
        }

        /*
         * 刷新列表 
         */
        private void refreshList()
        {
            List<ScreenCfgDto> list = screenCfgBll.findAllWithPageName();
            dataGrid.DataContext = list;
        }

        /* 
        * 编辑
        */
        private void Edit_Button_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender; 
            int id = Convert.ToInt32( btn.Tag);
            EditMoreScreenWindow win = new EditMoreScreenWindow(id);
            bool result = (Boolean)win.ShowDialog();
            if (result) 
                this.refreshList();
             
        }
        /*
         * 删除
         */
        private void Delete_Button_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            int id = Convert.ToInt32(btn.Tag);
            if (id == 1)
            {
                MessageBox.Show("主屏幕不能删除");
                return;
            }
            DeleteMoreScreenWindow win = new DeleteMoreScreenWindow(id);
            bool result = (Boolean)win.ShowDialog();
            if (result)
                this.refreshList();
        }
    }
}
