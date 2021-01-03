using Bll;
using Common;
using Common.MQ;
using Model;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WpfApplication1.ManageWin;
using WpfApplication1.MenuWin;
using WpfApplication1.PageWin;

namespace WpfApplication1
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

        private readonly CfgBll cfgBll = new CfgBll();
        private readonly DPageBll dPageBll = new DPageBll();
        private readonly DControlBll dControlBll = new DControlBll();
        private readonly StorageFileBll storageFileBll = new StorageFileBll();
        private readonly StorageImageBll storageImageBll = new StorageImageBll();
        public PageTemplate pageTemplate = null;
        //当前屏幕下的消息服务器
        public MqServer mqServer = new MqServer();


        public MainWindow()
        {
            InitializeComponent();

            init();
        }


        private void init()
        {
            Cfg cfg = cfgBll.get(1);
            App.localStorage.cfg = cfg;
            FullScreen();
            initLayout();
        }

        /*
        * 更改配置信息后，重新加载窗口
        */
        public void reloadWindow()
        {
            initHeaderMenu();
            openDefaultPage(App.localStorage.currPageId);
        }

        /*
         * 全屏显示
         */
        private void FullScreen()
        {
            // this.WindowState =  WindowState.Maximized;
            // this.WindowStyle = WindowStyle.None;
            // this.Topmost = true;

            Left = 0;
            Top = 0;
        }

        /*
         * 初始化常规布局 
         */
        private void initLayout()
        {

            initHeaderMenu();
            PageTree pageTree = new PageTree(this);
            pageTree.initLeftPageTree();
            openDefaultPage(1);
        }
        /*
         * 打开首页
         */
        private void openDefaultPage(int pageId)
        {
            pageTemplate = new PageTemplate(mainFrame, pageId, mqServer);
            mainFrame.Navigate(pageTemplate);

        }

        /*
         * 初始化头部菜单
         */
        private void initHeaderMenu()
        {
            Int32 pagePercent = App.localStorage.cfg.pagePercent;
            mainFrame.Width = App.localStorage.cfg.screenWidth;
            mainFrame.Height = App.localStorage.cfg.screenHeight;

            initPagePercentComboBox(pagePercent);
            changeMainFramePercent(pagePercent);
        }



        /*
         * 改变mainFrame显示百分比
         * 
         * @param Int32 percent 百分比
         */
        public void changeMainFramePercent(Int32 percent)
        {
            double scale = percent / 100.0;
            mainFrame.RenderTransform = new ScaleTransform(scale, scale);
            App.localStorage.cfg.pagePercent = percent;

            double designScreenWidth = SystemParameters.PrimaryScreenWidth;
            double designScreenHeight = SystemParameters.PrimaryScreenHeight;

            double offsetLeft = (mainFrame.Width - designScreenWidth) / 2;
            double offsetTop = (mainFrame.Height - designScreenHeight) / 2;
            mainFrameScrollViewer.ScrollToHorizontalOffset(offsetLeft);
            mainFrameScrollViewer.ScrollToVerticalOffset(offsetTop);

        }


        /*
         * 初始化百分比下拉菜单
         * 
         * @param currPagePercent 当前页面百分比
         */
        private void initPagePercentComboBox(Int32 currPagePercent)
        {
            if (pagePercentComboBox.Items.Count > 0) return;
            for (Int32 i = 10; i > 0; i--)
            {
                Int32 pct = i * 10;
                ComboBoxItem item = new ComboBoxItem();
                item.Content = pct + "%";
                item.Tag = pct;
                if (pct == currPagePercent)
                {
                    item.IsSelected = true;

                }
                pagePercentComboBox.Items.Add(item);
            }
        }

        /*
         * 关闭软件
         */
        private void winCloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        /*
         * 百分比选项发生改变触发事件
         */
        private void pagePercentComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            ComboBoxItem item = (ComboBoxItem)comboBox.SelectedItem;
            Int32 pct = DataUtil.ToInt(item.Tag.ToString());
            if (pct > 0 && pct <= 100)
            {
                changeMainFramePercent(pct);

                Cfg cfg = cfgBll.get(1);
                cfg.pagePercent = pct;
                cfg = cfgBll.update(cfg);
                App.localStorage.cfg = cfg;
            }
        }

        /*
         * 新建页面 通过顶部菜单
         */
        private void MenuNewPageByMenuClick(object sender, RoutedEventArgs e)
        {
            NewPageByMenuWindow win = new NewPageByMenuWindow(pageTreeColumn, 1);
            win.ShowDialog();
        }
        /*
         * 预览项目
         */
        private void MenuPreviewProjectClick(object sender, RoutedEventArgs e)
        {
            ShowBox.PreviewStartup previewStartup = new ShowBox.PreviewStartup(App.localStorage.currPageId);
            

            //ShowBox.PreviewWindow win = new ShowBox.PreviewWindow(App.localStorage.currPageId);
            //win.ShowDialog();
        }
        /*
         * 触摸屏信息
         */
        private void MenuBaseConfigClick(object sender, RoutedEventArgs e)
        {
            BaseConfigWindow win = new BaseConfigWindow(this);
            win.ShowDialog();
        }
        /*
         * 页面切换方式
         */
        private void MenuPageSwitchTypeClick(object sender, RoutedEventArgs e)
        {
            PageSwitchTypeWindow win = new PageSwitchTypeWindow(this);
            win.ShowDialog();
        }
        /*
         * 全屏如何退出
         */
        private void MenuHowExitClick(object sender, RoutedEventArgs e)
        {
            HowExitWindow win = new HowExitWindow();
            win.ShowDialog();
        }
        /*
        * 多屏扩展
        */
        private void MenuMoreScreenClick(object sender, RoutedEventArgs e)
        {
            MoreScreenWindow win = new MoreScreenWindow();
            win.ShowDialog(); 
        }

        /*
         * 无操作自动返回首页
         */
        private void AutoBackToHomeClick(object sender, RoutedEventArgs e)
        {
            AutoBackToHomeWindow win = new AutoBackToHomeWindow();
            win.ShowDialog();
        }


        private void pageTreeColumn_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //TreeView treeView = (TreeView)sender; 
            //LeftPageTreeBg.Width = treeView.ActualWidth;
            //LeftPageTreeBg.Height = treeView.ActualHeight;
            //LeftPageTreeGrid.Width = treeView.ActualWidth;
            //LeftPageTreeGrid.Height = treeView.ActualHeight;

        }

        /*
         * 最小化按钮
         */
        private void winMinButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        /*
         * 导出内容 
         */
        private void MenuImportProjectClick(object sender, RoutedEventArgs e)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + @"\myfile";
            ProcessStartInfo StartInformation = new ProcessStartInfo(); 
            StartInformation.FileName = path; 
            Process process = Process.Start(StartInformation);
             
        }

        private void MenuInsertImageClick(object sender, RoutedEventArgs e)
        {
            pageTemplate.editing.insertImageClick(sender, e);
        }

        private void MenuInsertWordClick(object sender, RoutedEventArgs e)
        {
            pageTemplate.editing.insertWordClick(sender, e);
        }

        private void MenuInsertTurnPictureClick(object sender, RoutedEventArgs e)
        {
            pageTemplate.editing.insertTurnPictureClick(sender, e);
        }

        private void MenuInsertVideoClick(object sender, RoutedEventArgs e)
        {
            pageTemplate.editing.insertVideoClick(sender, e);
        }

        private void MenuInsertBackButtonClick(object sender, RoutedEventArgs e)
        {
            pageTemplate.editing.insertBackButtonClick(sender, e);
        }

        private void MenuInsertHomeButtonClick(object sender, RoutedEventArgs e)
        {
            pageTemplate.editing.insertHomeButtonClick(sender, e);
        }


        private void MenuInsertFrameClick(object sender, RoutedEventArgs e)
        {
            pageTemplate.editing.insertCFrameClick(sender, e);
        }

        private void MenuInsertCCalendarClick(object sender, RoutedEventArgs e)
        {
            pageTemplate.editing.insertCCalendarClick(sender, e);
        }
        private void MenuInsertCAudioClick(object sender, RoutedEventArgs e)
        {
            pageTemplate.editing.insertCAudioClick(sender, e);
        }


        private void MenuEditBackgroundImageClick(object sender, RoutedEventArgs e)
        {
            pageTemplate.editing.editBackgroundImageClick(sender, e);
        }

        private void MenuEditCVideoBackgroundClick(object sender, RoutedEventArgs e)
        {
            pageTemplate.editing.editCVideoBackgroundClick(sender, e);
        }

        private void MenuPastControlClick(object sender, RoutedEventArgs e)
        {
            pageTemplate.editing.pastControlClick(sender, e);

        }


        private void MenuBackgroundMusicClick(object sender, RoutedEventArgs e)
        {
            EditBackgroundMusicWindow win = new EditBackgroundMusicWindow(this);
            win.ShowDialog();
        }

        private void MenuAboutUsClick(object sender, RoutedEventArgs e)
        {
            //bool b = this.mySerialPort.IsOpen;
            //MessageBox.Show(b.ToString());
            AboutUsWindow win = new AboutUsWindow();
            win.ShowDialog();
        }

        private void MenuOperationDocumentClick(object sender, RoutedEventArgs e)
        {
            OperationDocumentWindow win = new OperationDocumentWindow();
            win.ShowDialog();
        }

        /*
         * 素材中心
         */
        private void StorageManageClick(object sender, RoutedEventArgs e)
        {
            StorageManagerWindow win = new StorageManagerWindow("imageItem");
            win.ShowDialog();
        }

        private void SoftRegClick(object sender, RoutedEventArgs e)
        {
            SoftRegWindow win = new SoftRegWindow();
            win.ShowDialog();
        }

        /*
         * 左侧菜单触摸滑动反馈
         */
        private void pageTreeColumn_ManipulationBoundaryFeedback(object sender, ManipulationBoundaryFeedbackEventArgs e)
        {
            e.Handled = true;
        }


        /*
         * 上下左右移动控件位置
         */
        private void MainWindowPreviewKeyDown(object sender, KeyEventArgs e)
        {
            Boolean result = pageTemplate.KeyDownMove(e.Key);
            if (result)
            {
                e.Handled = true;
            }
        }

    }
}
