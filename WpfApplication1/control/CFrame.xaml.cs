using Bll;
using Common;
using Common.MQ;
using Model;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace WpfApplication1.control
{
    /// <summary>
    /// CFrame.xaml 的交互逻辑
    /// </summary>
    public partial class CFrame : UserControl
    {
        private readonly DControlBll dControlBll = new DControlBll();
        private readonly StorageImageBll storageImageBll = new StorageImageBll();
        private readonly DPageBll dPageBll = new DPageBll();
        private DControl currDControl;
        private Cfg cfg;
        private MqServer mqServer = null;
        public CFrame(DControl currDControl, Cfg cfg,MqServer mqServer)
        {
            InitializeComponent();
            this.currDControl = currDControl;
            this.cfg = cfg;
            this.mqServer = mqServer;
            mainFrame.Tag = currDControl;

            initControl();
            Unloaded += UserControl_UnLoaded;
        }

        private void UserControl_UnLoaded(object sender, RoutedEventArgs e)
        {
            mainFrame.Content = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }



        /*
        * 初始化页面控件
        */
        private void initControl()
        {
            DPage dPage1 = dPageBll.get(currDControl.linkToPageId);
            int frameWidth = App.localStorage.cfg.screenWidth;
            int frameHeight = App.localStorage.cfg.screenHeight;
            if (dPage1 != null && dPage1.width > 0) frameWidth = dPage1.width;
            if (dPage1 != null && dPage1.height > 0) frameHeight = dPage1.height;
            PageTemplate pageTemplate = new PageTemplate(mainFrame, currDControl.linkToPageId, currDControl.isTransparentDialog,this.mqServer);


            //如果页面不存在，则显示透明背景
            if (pageTemplate.editing == null)
            {
                string transparentBg = AppDomain.CurrentDomain.BaseDirectory + "/myfile/sysimg/transparent.jpg";
                scrollViewer.Background = new ImageBrush
                {
                    ImageSource = FileUtil.readImage2(transparentBg, 16)
                    ,
                    Stretch = Stretch.Fill
                    ,
                    TileMode = TileMode.Tile
                    ,
                    Viewport = new Rect(0, 0, 16, 16)
                    ,
                    ViewportUnits = BrushMappingMode.Absolute
                };
            }
            else
            {
                scrollViewer.Background = Brushes.Transparent;
            }
            mainFrame.Width = frameWidth;
            mainFrame.Height = frameHeight;
            mainFrame.Content = pageTemplate;
        }

        /*
         * 编辑控件 - 更新页面显示
         */
        internal void updateElement(DControl ctl, Cfg cfg)
        {
            //清空原有控件
            currDControl = ctl;
            this.cfg = cfg;


            //重新更新页面控件
            initControl();
        }
        /*
         * 编辑控件属性 - 更新页面显示
         */
        internal void updateElementAttr(DControl dControl, bool isDesign)
        {
            Width = dControl.width;
            Height = dControl.height;
            Margin = new Thickness(dControl.left, dControl.top, 0, 0);
            Opacity = dControl.opacity / 100.0;
            currDControl = dControl;
        }
        private void ScrollViewerManipulationBoundaryFeedback(object sender, ManipulationBoundaryFeedbackEventArgs e)
        {
            e.Handled = true;
        }

    }
}
