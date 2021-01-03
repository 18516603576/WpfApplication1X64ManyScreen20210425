using Bll;
using Common.Data;
using Common.MQ;
using Model;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ShowBox.control
{
    /// <summary>
    /// CFrame.xaml 的交互逻辑
    /// </summary>
    public partial class CFrame : UserControl
    {
        private readonly DControlBll dControlBll = new DControlBll();
        private readonly StorageImageBll storageImageBll = new StorageImageBll();
        private readonly DPageBll dPageBll = new DPageBll();
        private readonly DControl currDControl;
        private readonly Cfg cfg;
        private System.Windows.Forms.Screen screen = null;
        private MqServer mqServer = null;
        public CFrame(Frame parentFrame, DControl currDControl, Cfg cfg,System.Windows.Forms.Screen screen,MqServer mqServer)
        {
            InitializeComponent();
            this.currDControl = currDControl;
            this.cfg = cfg;
            this.screen = screen;
            this.mqServer = mqServer;

            object parentTag = parentFrame.Tag;
            DControl parentDControl = null;
            Border parentCoverBorder = null;
            if (parentTag is CFrameTag)
            {
                CFrameTag tmp = (CFrameTag)parentTag;
                parentDControl = tmp.currDControl;
                parentCoverBorder = tmp.currCoverBorder;
            }


            CFrameTag tag = new CFrameTag();
            tag.currCFrame = mainFrame;
            tag.currDControl = currDControl;
            tag.currCoverBorder = CoverBorder;
            tag.parentFrame = parentFrame;
            tag.parentDControl = parentDControl;
            tag.parentCoverBorder = parentCoverBorder;
            mainFrame.Tag = tag;

            initControl();
            Unloaded += UserControl_UnLoaded;

            //this.scrollViewer.PreviewTouchDown += scrollViewer_PreviewTouchDown;
            //this.scrollViewer.PreviewTouchUp += scrollViewer_PreivewTouchUp;
        }

        private void scrollViewer_PreivewTouchUp(object sender, TouchEventArgs e)
        {
            ReleaseAllTouchCaptures();
            e.Handled = false;
        }

        private void scrollViewer_PreviewTouchDown(object sender, TouchEventArgs e)
        {
            CaptureTouch(e.TouchDevice);
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
            //如果页面不存在，则显示透明背景 

            //scrollViewer 内嵌Frame
            // if (ctl.linkToPageId <= 0) return;
            DPage dPage1 = dPageBll.get(currDControl.linkToPageId);
            //  if (dPage1 == null) return;
            int frameWidth = App.localStorage.cfg.screenWidth;
            int frameHeight = App.localStorage.cfg.screenHeight;
            if (dPage1 != null && dPage1.width > 0) frameWidth = dPage1.width;
            if (dPage1 != null && dPage1.height > 0) frameHeight = dPage1.height;
            mainFrame.Width = frameWidth;
            mainFrame.Height = frameHeight;


            PageTemplate pageTemplate = new PageTemplate(mainFrame, currDControl.linkToPageId, false, currDControl.isTransparentDialog,screen,mqServer);
            mainFrame.Content = pageTemplate;
        }


        private void ScrollViewerManipulationBoundaryFeedback(object sender, ManipulationBoundaryFeedbackEventArgs e)
        {
            e.Handled = true;
        }


        public Frame getFrame()
        {

            return mainFrame;
        }
    }
}
