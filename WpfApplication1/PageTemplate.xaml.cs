using Bll;
using Common;
using Common.control;
using Common.Data;
using Common.MQ;
using Common.util;
using Model;
using Model.dto;
using ShowBox.control;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WpfApplication1.manage;
using Border = System.Windows.Controls.Border;
using Frame = System.Windows.Controls.Frame;

namespace WpfApplication1
{
    /// <summary>
    /// PageTemplate.xaml 的交互逻辑
    /// </summary>
    public partial class PageTemplate : System.Windows.Controls.Page
    {

        private readonly DPageBll dPageBll = new DPageBll();
        private readonly DControlBll dControlBll = new DControlBll();
        private readonly StorageImageBll storageImageBll = new StorageImageBll();
        private readonly StorageVideoBll storageVideoBll = new StorageVideoBll();
        //拖拽控件
        public Editing editing = null;
        public DPage dPage = null;

        private readonly Frame mainFrame = null;
        //是否显示默认的窗口背景
        private readonly Boolean showDefaultBackgroundInCFrameDialog;
        //当前屏幕下的消息服务器
        public MqServer mqServer = null;


        public PageTemplate(Frame mainFrame, Int32 pageId,MqServer mqServer)
        {
            InitializeComponent();
            this.mainFrame = mainFrame;
            App.localStorage.currPageId = pageId;
            this.mqServer = mqServer; 
            FrameUtil.RemoveBackEntry(mainFrame);
            initControl(pageId);
            Unloaded += PageTemplate_UnLoaded;
        }


        public PageTemplate(Frame mainFrame, Int32 pageId, Boolean isTransparentDialog,MqServer mqServer)
        {
            InitializeComponent();
            this.mainFrame = mainFrame;
            FrameUtil.RemoveBackEntry(mainFrame);
            Background = Brushes.Transparent;
            if (!isTransparentDialog)
                showDefaultBackgroundInCFrameDialog = true;
            this.mqServer = mqServer;  
            initControl(pageId);
            Unloaded += PageTemplate_UnLoaded;
        }
       
        private void PageTemplate_UnLoaded(object sender, RoutedEventArgs e)
        {
            foreach (UIElement element in container.Children)
            {
                if (element is Button)
                {
                    Button btn = element as Button;
                    btn.Background = null;
                    btn = null;
                }
                else if (element is DocumentViewer)
                {
                    DocumentViewer documentViewer = element as DocumentViewer;
                    documentViewer.Style = null;
                    documentViewer.PreviewMouseLeftButtonDown -= editing.control_MouseDown;
                    documentViewer.PreviewMouseMove -= editing.control_MouseMove;
                    documentViewer.PreviewMouseLeftButtonUp -= editing.control_MouseUp;
                    //控件上右击显示菜单 
                    documentViewer.MouseRightButtonUp -= editing.control_MouseRightButtonUp;
                    documentViewer.SizeChanged -= WordUtil.Word_SizeChanged;

                    documentViewer.Document = null;
                    documentViewer = null;
                }
                else if (element is TurnPicture)
                {
                    TurnPicture turnPicture = element as TurnPicture;
                    turnPicture = null;
                }
                else if (element is CVideo)
                {
                    CVideo cVideo = element as CVideo;
                    cVideo = null;
                }

                else if (element is CFrame)
                {
                    CFrame cFrame = element as CFrame;
                    cFrame.Content = null;
                    cFrame = null;
                }
                else if (element is Gif)
                {
                    Gif gif = element as Gif;
                    gif = null;
                }
                else
                {
                    FrameworkElement fe = element as FrameworkElement;
                    fe = null;
                }
            }
            //释放视频背景
            foreach (FrameworkElement ele in backgroundVideo.Children)
            {
                if (ele.Name == "CVideoBackground")
                {
                    CVideoBackground cVideoBackground = (CVideoBackground)ele; 
                    cVideoBackground = null;
                }
            }
            backgroundVideo.Children.Clear(); 

            Background = null;
            editing = null;
            Content = null;
            container.Children.Clear();
            GC.Collect();
            //GC.WaitForPendingFinalizers();
            //GC.Collect(); 
        }

        /*
         * 初始化页面控件
         */
        private void initControl(Int32 pageId)
        {
            container.Children.Clear();
            dPage = dPageBll.get(pageId);
            if (dPage == null)
            {
                return;
            }
            //1加载背景图片
            StorageImage storageImage = null;
            if (dPage.backgroundImageId > 0)
            {
                storageImage = storageImageBll.get(dPage.backgroundImageId);
            }
            if (FileUtil.imageIsExists(storageImage?.url))
            {
                Background = Brushes.Transparent;
                FileUtil.readImage2Page(this, AppDomain.CurrentDomain.BaseDirectory + storageImage.url, App.localStorage.cfg.screenWidth, Stretch.Fill);
            }
            else if (showDefaultBackgroundInCFrameDialog)
            {
                Background = Brushes.Transparent;
                FileUtil.readImage2Page(this, AppDomain.CurrentDomain.BaseDirectory + Params.CFrameDialogDefaultBackground, App.localStorage.cfg.screenWidth, Stretch.Fill);
            }
            else
            {
                Background = Brushes.White;
            }

            //2加载视频背景
            if (dPage.backgroundVideoId > 0)
            {
                StorageVideo storageVideo = storageVideoBll.get(dPage.backgroundVideoId);
                if (storageVideo != null)
                {
                    int pageWidth = App.localStorage.cfg.screenWidth;
                    int pageHeight = App.localStorage.cfg.screenHeight;
                    if (dPage.width > 0)
                    {
                        pageWidth = dPage.width;
                        pageHeight = dPage.height;
                    }
                    insertCVideoBackground(storageVideo, pageWidth, pageHeight);
                }
            }

            //编辑框，及页面空白处点击
            editing = new Editing(mainFrame, this);


        }

        /*
         * 插入视频背景
         * 
         * @param storageVideo 视频信息
         * 
         * @param pageWidth  页面宽度
         * 
         * @param pageHeight  页面高度 
         * 
         */
        private void insertCVideoBackground(StorageVideo storageVideo, int pageWidth, int pageHeight)
        {
            DControl ctl = new DControl();
            ctl.name = "CVideoBackground";
            ctl.width = pageWidth;
            ctl.height = pageHeight;
            ctl.left = 0;
            ctl.top = 0;
            ctl.storageId = storageVideo.id;

            StorageVideoDto dto = StorageVideoUtil.convert(storageVideo);
            StorageImage storageImage = storageImageBll.get(dto.storageImageId);
            dto.storageImageUrl = storageImage?.url;
            CVideoBackground cVideoBackground1 = new CVideoBackground(dto, true);
            cVideoBackground1.Name = "CVideoBackground";
            cVideoBackground1.HorizontalAlignment = HorizontalAlignment.Left;
            cVideoBackground1.VerticalAlignment = VerticalAlignment.Top;
            cVideoBackground1.Margin = new Thickness(ctl.left, ctl.top, 0, 0);
            cVideoBackground1.Width = ctl.width;
            cVideoBackground1.Height = ctl.height;
            backgroundVideo.Children.Add(cVideoBackground1);
        }

        /*
         * 上下左右，移动控件
         */
        public Boolean KeyDownMove(Key key)
        {
            Boolean result = false;

            FrameworkElement element = FrameworkElementUtil.getByName(container, "editingBorder");
            if (element == null) return result;

            Border editingBorder = (Border)element;
            FrameworkElement control = (FrameworkElement)editingBorder.Tag;
            Thickness margin = control.Margin;


            if (key == Key.Up)
            {
                margin.Top = margin.Top - 1;
                control.Margin = margin;
                editingBorder.Margin = margin;
                result = true;
            }
            else if (key == Key.Down)
            {
                margin.Top = margin.Top + 1;
                control.Margin = margin;
                editingBorder.Margin = margin;
                result = true;
            }
            else if (key == Key.Left)
            {
                margin.Left = margin.Left - 1;
                control.Margin = margin;
                editingBorder.Margin = margin;
                result = true;
            }
            else if (key == Key.Right)
            {
                margin.Left = margin.Left + 1;
                control.Margin = margin;
                editingBorder.Margin = margin;
                result = true;
            }
            if (result)
            {
                DControl dc = (DControl)control.Tag;
                dc.left = (Int32)control.Margin.Left;
                dc.top = (Int32)control.Margin.Top;
                control.Tag = dc;
                dControlBll.update(dc);
            }

            return result;
        }

    }
}
