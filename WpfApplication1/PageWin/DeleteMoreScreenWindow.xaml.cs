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
    public partial class DeleteMoreScreenWindow : Window
    { 
        private readonly ScreenCfgBll screenCfgBll = new ScreenCfgBll();
        private readonly DPageBll dPageBll = new DPageBll();
        private ScreenCfg screenCfg = null;


        //页面居中显示到最前面 
        public DeleteMoreScreenWindow(int id)
        {
             
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.loadPageData(id);
            Submit_Button.Click += Submit_Button_Click;
        }

        private void loadPageData(int id) {
            this.screenCfg =  screenCfgBll.get(id);
            if (screenCfg == null) {
                MessageBox.Show("当前屏幕不存在");
            } 
            this.diyName.Content = screenCfg.diyName; 
        }



        /*
         * 保存数据
         */
        private void Submit_Button_Click(object sender, RoutedEventArgs e)
        { 
            int rows = screenCfgBll.delete(screenCfg.id);
            this.DialogResult = true;
            Close();
        }
 
 
    }
}
