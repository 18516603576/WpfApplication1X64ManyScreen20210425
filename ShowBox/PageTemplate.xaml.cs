using Bll;
using Common;
using Common.control;
using Common.Data;
using Common.MQ;
using Common.util;
using Model;
using Model.dto;
using ShowBox.control;
using ShowBox.manage;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace ShowBox
{
    /// <summary>
    /// PageTemplate.xaml 的交互逻辑
    /// </summary>
    public partial class PageTemplate : System.Windows.Controls.Page
    {
        public InsertToPage insertToPage;
        public Frame mainFrame = null;
        private readonly DPageBll dPageBll = new DPageBll();
        private readonly DControlBll dControlBll = new DControlBll();
        private readonly StorageImageBll storageImageBll = new StorageImageBll();
        private readonly StorageVideoBll storageVideoBll = new StorageVideoBll();
        private readonly Boolean isBack = false;
        private readonly int backPageSwitchType = 0;
        //弹窗显示默认背景
        private readonly Boolean showDefaultBackgroundInCFrameDialog = false;
        //当前页面
        public DPage dPage = null;
        public System.Windows.Forms.Screen screen = null;
        public MqServer mqServer = null;

        /*
         * 弹窗
         */
        public PageTemplate(Frame mainFrame, Int32 pageId, Boolean isMoveIn, Boolean isTransparentDialog,System.Windows.Forms.Screen screen,MqServer mqServer)
        {
            InitializeComponent();
            this.mainFrame = mainFrame;
            this.screen = screen;
            this.mqServer = mqServer;
            FrameUtil.RemoveBackEntry(mainFrame);
            if (isTransparentDialog)
            {
                Background = Brushes.Transparent;
            }
            if (!isTransparentDialog)
                showDefaultBackgroundInCFrameDialog = true;

            initControl(pageId);
            Unloaded += PageTemplate_UnLoaded;
            if (isMoveIn)
            {
                Loaded += moveIn;
            }
        }

        /*
         *  返回上一页
         * 
         *  @param mainFrame 所在框架
         *  
         *  @param pageId  页面id
         *  
         *  @param  isMoveIn  是否加载动画
         *  
         *  @param isBack 是否返回
         *  
         *  @param  pageSwitchType  页面切换方式
         */
        public PageTemplate(Frame mainFrame, Int32 pageId, Boolean isMoveIn, Boolean isBack, int backPageSwitchType, System.Windows.Forms.Screen screen,MqServer mqServer)
        {
            InitializeComponent();
            this.mainFrame = mainFrame;
            this.screen = screen;
            this.mqServer = mqServer;
            FrameUtil.RemoveBackEntry(mainFrame);
            CFrameTag cFrameTag = (CFrameTag)mainFrame.Tag;
            if (cFrameTag.parentFrame != null)
            {
                Background = Brushes.Transparent;
                showDefaultBackgroundInCFrameDialog = true;
            }
            initControl(pageId);
            Unloaded += PageTemplate_UnLoaded;
            this.isBack = isBack;
            this.backPageSwitchType = backPageSwitchType;
            if (isMoveIn)
            {
                Loaded += moveIn;
            }
        }


        /*
         *  标准进入页面
         */
        public PageTemplate(Frame mainFrame, Int32 pageId, Boolean isMoveIn, System.Windows.Forms.Screen screen,MqServer mqServer)
        {
            InitializeComponent();
            this.mainFrame = mainFrame;
            this.screen = screen;
            this.mqServer = mqServer;
            FrameUtil.RemoveBackEntry(mainFrame);
            CFrameTag cFrameTag = (CFrameTag)mainFrame.Tag;
            if (cFrameTag.parentFrame != null)
            {
                Background = Brushes.Transparent;
                showDefaultBackgroundInCFrameDialog = true;
            }
            initControl(pageId);
            Unloaded += PageTemplate_UnLoaded;

            if (isMoveIn)
            {
                Loaded += moveIn;
            } 
        } 

        public void PageTemplate_UnLoaded(object sender, RoutedEventArgs e)
        {
            Unloaded -= PageTemplate_UnLoaded;
            foreach (FrameworkElement element in container.Children)
            {
                if (element is Button)
                {
                    Button btn = element as Button;
                    if(btn.Name == "Image")
                    {
                        btn.Click -= insertToPage.imageButtonClick;
                    }
                    else if(btn.Name == "BackButton")
                    {
                        btn.Click -= insertToPage.backButtonClick;
                    }
                    else if (btn.Name == "HomeButton")
                    {
                        btn.Click -= insertToPage.homeButtonClick; 
                    }
                    btn.Background = null;
                    btn = null; 
                }
                else if (element is DocumentViewer)
                {
                    DocumentViewer documentViewer = element as DocumentViewer;
                    documentViewer.Document = null;
                    documentViewer = null;
                }
                else if (element is TurnPicture)
                {
                    TurnPicture turnPicture = element as TurnPicture;
                    turnPicture = null;
                }
                else if (element is Marque)
                {
                    Marque marque = element as Marque;
                    marque = null;
                }
                else if (element is MarqueLayer)
                {
                    MarqueLayer marqueLayer = element as MarqueLayer;
                    marqueLayer = null;
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
                else if (element is TextBlock)
                {
                    TextBlock textBlock = element as TextBlock;
                    textBlock.PreviewMouseUp -= insertToPage.textBlock_PreviewMouseUp;
                    textBlock.PreviewTouchUp -= insertToPage.textBlock_PreviewTouchUp;
                    textBlock = null;
                } 
                else if (element is Gif)
                {
                    Gif gif = element as Gif;
                    gif.PreviewMouseUp -= insertToPage.gif_PreviewMouseUp;
                    gif.PreviewTouchUp -= insertToPage.gif_PreviewTouchUp; 
                    gif = null;
                }
                else if (element is CAudio)
                {
                    CAudio cAudio = element as CAudio;
                    cAudio.PreviewMouseUp += insertToPage.cAudio_PreviewMouseUp;
                    cAudio.PreviewTouchUp += insertToPage.cAudio_PreviewTouchUp;
                    cAudio = null;
                }
               
                else if (element is CCalendar)
                {
                    CCalendar cCalendar = element as CCalendar;
                    cCalendar = null;
                }
                else if (element is Canvas && element.Name == "frameDialogCanvas")
                {
                    releaseFrameDialogCanvas(element);
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
            insertToPage = null;
            Content = null;
            container.Children.Clear();

            GC.Collect();
        }

        /*
         * 释放页面弹窗
         */
        private void releaseFrameDialogCanvas(FrameworkElement element)
        {
            Canvas frameDialogCanvas = (Canvas)element;
            Canvas innerCanvas = null;
            foreach (FrameworkElement ele in frameDialogCanvas.Children)
            {
                if (ele is Canvas)
                {
                    Canvas tmp = (Canvas)ele;
                    if (tmp.Name == "innerCanvas")
                    {
                        innerCanvas = tmp;
                    }
                }
            }

            if (innerCanvas != null)
            {
                foreach (FrameworkElement ele in innerCanvas.Children)
                {
                    if (ele is CFrame)
                    {
                        CFrame cFrame = (CFrame)ele;
                        cFrame.Content = null;
                        cFrame = null;
                    }
                    else if (ele is Button)
                    {
                        Button closebtn = (Button)ele;
                        closebtn.Background = null;
                    }
                }
            }

        }


        private void moveIn(object sender, EventArgs e)
        {
            Int32 pageSwitchType = dPage.pageSwitchType;
            if (isBack && backPageSwitchType > 0)
            {
                pageSwitchType = backPageSwitchType;
            }
            if (pageSwitchType <= 0) { pageSwitchType = 1; }
            int pageWidth = dPage.width;
            if (pageWidth <= 0) pageWidth = App.localStorage.cfg.screenWidth;


            //1.默认
            if (pageSwitchType == 1)
            {
                mainFrame.Background = Brushes.Transparent;
                return;
            }
            //2.淡入
            else if (pageSwitchType == 2)
            {
                TransformGroup group = new TransformGroup();
                RenderTransform = group;
                DoubleAnimation da = new DoubleAnimation(0, 1.0, new Duration(TimeSpan.FromMilliseconds(400)));
                da.BeginTime = TimeSpan.FromMilliseconds(0);
                IEasingFunction easingFunction = new SineEase() { EasingMode = EasingMode.EaseOut };
                da.EasingFunction = easingFunction;
                da.Completed += a2_Completed;
                BeginAnimation(UIElement.OpacityProperty, da);
                return;
            }

            //3.右侧移入
            else if (pageSwitchType == 3)
            {
                if (isBack) return;



                TransformGroup group = new TransformGroup();
                RenderTransform = group;

                double middlePos = pageWidth / 2;
                TranslateTransform translateTransform = TransformGroupUtil.GetTranslateTransform(group);
                DoubleAnimation da = new DoubleAnimation(middlePos, 0, new Duration(TimeSpan.FromMilliseconds(500)));
                da.BeginTime = TimeSpan.FromMilliseconds(0);
                IEasingFunction easingFunction = new SineEase() { EasingMode = EasingMode.EaseOut };
                da.AccelerationRatio = 0.9;
                //  da.EasingFunction = easingFunction; 
                da.Completed += a2_Completed;
                translateTransform.BeginAnimation(TranslateTransform.XProperty, da);


                Opacity = 0;
                DoubleAnimation da1 = new DoubleAnimation(0, 1.0, new Duration(TimeSpan.FromMilliseconds(100)));
                da1.BeginTime = TimeSpan.FromMilliseconds(0);
                da1.EasingFunction = easingFunction;
                BeginAnimation(UIElement.OpacityProperty, da1);
                return;
            }

            //4.右侧拉伸
            else if (pageSwitchType == 4)
            {
                if (isBack) return;
                RenderTransformOrigin = new System.Windows.Point(1, 0);
                TransformGroup group = new TransformGroup();
                RenderTransform = group;

                ScaleTransform scaleTransform = TransformGroupUtil.GetScaleTransform(group);
                DoubleAnimation da = new DoubleAnimation(0, 1.0, new Duration(TimeSpan.FromMilliseconds(500)));
                da.BeginTime = TimeSpan.FromMilliseconds(0);
                //da.EasingFunction = easingFunction; 
                da.Completed += a2_Completed;
                scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, da);
                return;
            }

            //5.中心放大
            else if (pageSwitchType == 5)
            {
                if (isBack) return;
                RenderTransformOrigin = new System.Windows.Point(0.5, 0.5);
                TransformGroup group = new TransformGroup();
                RenderTransform = group;


                ScaleTransform scaleTransform = TransformGroupUtil.GetScaleTransform(group);
                DoubleAnimation da = new DoubleAnimation(0.2, 1.0, new Duration(TimeSpan.FromMilliseconds(500)));
                da.BeginTime = TimeSpan.FromMilliseconds(0);
                IEasingFunction easingFunction = new SineEase() { EasingMode = EasingMode.EaseOut };
                da.EasingFunction = easingFunction;
                da.Completed += a2_Completed;
                scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, da);
                scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, da);


                Opacity = 0;
                DoubleAnimation da1 = new DoubleAnimation(0, 1.0, new Duration(TimeSpan.FromMilliseconds(100)));
                da1.BeginTime = TimeSpan.FromMilliseconds(0);
                da1.EasingFunction = easingFunction;
                BeginAnimation(UIElement.OpacityProperty, da1);
                return;
            }
        }

        private void a2_Completed(object sender, EventArgs e)
        {
            CFrameTag tag = (CFrameTag)mainFrame.Tag;
            if (tag.parentFrame != null)
            {
                mainFrame.Background = Brushes.Transparent;
            }

        }

        /*
         * 初始化页面控件 
        */
        private void initControl(Int32 pageId)
        {
            DPage dPage = dPageBll.get(pageId);
            this.dPage = dPage;
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
                FileUtil.readImage2Page(this, AppDomain.CurrentDomain.BaseDirectory + storageImage.url, Convert.ToInt32(ActualWidth), Stretch.Fill);
            }
            else if (showDefaultBackgroundInCFrameDialog)
            {

                Background = Brushes.Transparent;
                FileUtil.readImage2Page(this, AppDomain.CurrentDomain.BaseDirectory + Params.CFrameDialogDefaultBackground, Convert.ToInt32(ActualWidth), Stretch.Fill);
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


            insertToPage = new InsertToPage(mainFrame, container, dPage, this);
            List<DControl> list = dControlBll.getByPageId(pageId);
            if (list == null) { return; }
            foreach (DControl ctl in list)
            {
                insertToPage.insertControl(ctl);
            }
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

    }
}
