using Bll;
using Common;
using Common.control;
using Common.Data;
using Common.util;
using Model;
using Model.dto;
using ShowBox.control;
using ShowBox.util;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using WinFormCef;

namespace ShowBox.manage
{
    public partial class InsertToPage
    {

        private DControlEventBll dControlEventBll = new DControlEventBll();
        /* 
         * 1.1 单击图片事件
         *  
         * 1.链接到页面
         * 
         * 2.跳转到外部网站
         * 
         * 3.全屏播放视频
         * 
         * 4.查看大图
         * 
         * 
         */
        public void imageButtonClick(object sender, RoutedEventArgs e)
        {
            Button imageButton = (Button)sender;
            imageButton.IsEnabled = false;
            DControlDto ctl = (DControlDto)imageButton.Tag;
            this.DifferentScreenDifferentLinkTo(ctl);
                 

            if (!string.IsNullOrWhiteSpace(ctl.linkToWeb))
            { 
                pageTemplate1.insertToPage.ClickShowWeb(ctl);   
            }
            else if (ctl.linkToVideoId > 0)
            {
                pageTemplate1.insertToPage.insertFullVideoToPage( ctl);
            }
            else if (ctl.isClickShow)
            {
                pageTemplate1.insertToPage.ClickShowBigImage(ctl); 
            }
            imageButton.IsEnabled = true;
        }

        /*
         * 不同的屏幕跳转到不同的页面 
         */ 
        private void DifferentScreenDifferentLinkTo(DControl ctl)
        {
            //不同屏幕，跳转到不同的页面  
            List<DControlEvent> list = dControlEventBll.getByDControlId(ctl.id);
            foreach (DControlEvent de in list)
            {
                DControl newDControl = DControlUtil.createFrom(ctl);
                newDControl.linkToPageId = de.linkToPageId;
                newDControl.isTransparentDialog = de.isTransparentDialog;
                newDControl.showInWhichCFrame = de.showInWhichCFrame;
                newDControl.isDialogLink = de.isDialogLink;

                PageTemplate AppPageTemplate = this.getScreen(de.screenCfgId);
                if (newDControl.linkToPageId > 0 && newDControl.isDialogLink)
                {
                    AppPageTemplate.insertToPage.insertFrameDialog(newDControl);
                    //insertFrameDialog(ctl); 
                }
                else if (newDControl.linkToPageId > 0)
                {
                    AppPageTemplate.insertToPage.linkToPageWithPageSwitchType(newDControl);
                    //  linkToPageWithPageSwitchType(ctl);
                }
            }
        }


        /*
        * 不同的屏幕跳转到不同的页面 
        */
        private void DifferentScreenDifferentLinkTo(DControl ctl,int turnPictureImagesId)
        {
            //不同屏幕，跳转到不同的页面  
            List<DControlEvent> list = dControlEventBll.getByTurnPictureImagesId(turnPictureImagesId);
            foreach (DControlEvent de in list)
            {
                DControl newDControl = DControlUtil.createFrom(ctl);
                newDControl.linkToPageId = de.linkToPageId;
                newDControl.isTransparentDialog = de.isTransparentDialog;
                newDControl.showInWhichCFrame = de.showInWhichCFrame;
                newDControl.isDialogLink = de.isDialogLink;

                PageTemplate AppPageTemplate = this.getScreen(de.screenCfgId);
                if (newDControl.linkToPageId > 0 && newDControl.isDialogLink)
                {
                    AppPageTemplate.insertToPage.insertFrameDialog(newDControl);
                    //insertFrameDialog(ctl); 
                }
                else if (newDControl.linkToPageId > 0)
                {
                    AppPageTemplate.insertToPage.linkToPageWithPageSwitchType(newDControl);
                    //  linkToPageWithPageSwitchType(ctl);
                }
            }
        }


        public void ClickShowWeb(DControlDto ctl)
        {
            Int32 maxPagePercent = FrameUtil.getMaxPercent(App.localStorage.cfg.screenWidth, App.localStorage.cfg.screenHeight);
            double screenWidth = SystemParameters.PrimaryScreenWidth;//得到屏幕整体宽度  

            Cfg pageCfg = PageWidthUtil.getPageCfg(dPage, App.localStorage.cfg);
            int winWidth = (int)(pageCfg.screenWidth * maxPagePercent / 100);
            int winHeight = (int)(pageCfg.screenHeight * maxPagePercent / 100);
            App.localStorage.currForm1 = new Form1(winWidth, winHeight, ctl.linkToWeb, screenWidth); 
            App.localStorage.currForm1.ShowDialog();  


            //Int32 maxPagePercent = FrameUtil.getMaxPercent(App.localStorage.cfg.screenWidth, App.localStorage.cfg.screenHeight);
            //double screenWidth = SystemParameters.PrimaryScreenWidth;//得到屏幕整体宽度  

            //Cfg pageCfg = PageWidthUtil.getPageCfg(dPage, App.localStorage.cfg);
            //int winWidth = (int)(pageCfg.screenWidth * maxPagePercent / 100);
            //int winHeight = (int)(pageCfg.screenHeight * maxPagePercent / 100);
            //App.localStorage.currForm1 = new Form1(winWidth, winHeight, ctl.linkToWeb, screenWidth);
            //App.localStorage.currForm1.Left = 500;
            //App.localStorage.currForm1.Top = pageTemplate1.screen.Bounds.Top;
            //App.localStorage.currForm1.Show();
            //App.localStorage.currForm1.BringToFront(); 

            //App.localStorage.currForm1.Click += currForm1_Click;

        }
 

        public void ClickShowBigImage(DControlDto ctl) {
            Cfg pageCfg = PageWidthUtil.getPageCfg(dPage, App.localStorage.cfg);
            ShowBigImageUtil.showBigImage(ctl.url, ctl, mainContainer, pageCfg); 
        }
           
        /*
         * 释放其他弹窗
         */ 
        private void realeaseOtherFrameDialog(Grid mainContainerTmp) {

            for(int i=0;i<mainContainerTmp.Children.Count;i++)
            {
                FrameworkElement Ei =(FrameworkElement) mainContainerTmp.Children[i];
                if(Ei is Canvas && Ei.Name== "frameDialogCanvas")
                {
                    Canvas frameDialogCanvas = (Canvas)Ei;

                    for (int j= 0;j < frameDialogCanvas.Children.Count;j++)
                    {
                        FrameworkElement Ej = (FrameworkElement)frameDialogCanvas.Children[j];
                        if (Ej is Canvas && Ej.Name == "innerCanvas")
                        {
                            Canvas innerCanvas = (Canvas)Ej;

                            ShowFrameDialogUtil.closeAnimation_Completed(frameDialogCanvas, innerCanvas, mainContainerTmp);
                        }
                     } 
                } 
            }  
        }

        /*
         *  1.2 弹窗显示页面  
         */
        public void insertFrameDialog(DControl dControl )
        {  
            //1.获取指定的显示位置，CFrame的宽高 
            CFrameTag cFrameTag = getCFrameTag(dControl.showInWhichCFrame);
            Grid mainContainerTmp = mainContainer;
            Frame mainFrameTmp = mainFrame; 
            if (cFrameTag != null)
            {
                //获取框架中的页面大小，及container
                PageTemplate pt = (PageTemplate)cFrameTag.currCFrame.Content;
                mainContainerTmp = pt.container;
                mainFrameTmp = cFrameTag.currCFrame;
            }
            this.realeaseOtherFrameDialog(mainContainerTmp);   

            Int32 linkToPageId = dControl.linkToPageId;
            if (linkToPageId <= 0) return;
            DPage linkToDPage = dPageBll.get(linkToPageId);
            if (linkToDPage == null) return;
            //窗口所在父页面
            //if (dControlDto.pageId <= 0) return;
            //DPage dPage = dPageBll.get(dControlDto.pageId);
            //if (dPage == null) return;

            //父页面宽高
            int dPageWidth = Convert.ToInt32(mainContainerTmp.ActualWidth);
            int dPageHeight = Convert.ToInt32(mainContainerTmp.ActualHeight);



            //弹窗
            int frameWidth = Convert.ToInt32(dPageWidth);
            int frameHeight = Convert.ToInt32(dPageHeight);
            if (linkToDPage.width > 0) frameWidth = linkToDPage.width;
            if (linkToDPage.height > 0) frameHeight = linkToDPage.height;
            Cfg cfg = new Cfg();
            cfg.screenWidth = frameWidth;
            cfg.screenHeight = frameHeight;

            


            Canvas frameDialogCanvas = new Canvas();
            frameDialogCanvas.Name = "frameDialogCanvas";
            frameDialogCanvas.HorizontalAlignment = HorizontalAlignment.Left;
            frameDialogCanvas.VerticalAlignment = VerticalAlignment.Top;
            frameDialogCanvas.Width = dPageWidth;
            frameDialogCanvas.Height = dPageHeight;
            frameDialogCanvas.Background = Brushes.Transparent;
            Panel.SetZIndex(frameDialogCanvas, 10004);

            //1.透明底层
            Border frameDialogBorder = new Border();
            frameDialogBorder.Name = "frameDialogBorder";
            frameDialogBorder.Width = dPageWidth;
            frameDialogBorder.Height = dPageHeight;
            frameDialogBorder.Background = Brushes.Black;
            frameDialogBorder.Opacity = 0.5;
            Panel.SetZIndex(frameDialogBorder, 1);
            frameDialogCanvas.Children.Add(frameDialogBorder);


            //2.弹窗
            Canvas innerCanvas = new Canvas();
            innerCanvas.Name = "innerCanvas";
            innerCanvas.Width = cfg.screenWidth;
            innerCanvas.Height = cfg.screenHeight;
            innerCanvas.Background = Brushes.Transparent;
            innerCanvas.HorizontalAlignment = HorizontalAlignment.Left;
            innerCanvas.VerticalAlignment = VerticalAlignment.Top;
            double left = dControl.left;
            double top = dControl.top;
            innerCanvas.SetValue(Canvas.LeftProperty, left);
            innerCanvas.SetValue(Canvas.TopProperty, top);
            Panel.SetZIndex(innerCanvas, 2);


           
            CFrame cFrame = new CFrame(mainFrameTmp, dControl, cfg,pageTemplate1.screen,pageTemplate1.mqServer);
            cFrame.BorderThickness = new Thickness(0);
            cFrame.Margin = new Thickness(0, 0, 0, 0);
            cFrame.Width = cfg.screenWidth;
            cFrame.Height = cfg.screenHeight;
            cFrame.HorizontalAlignment = HorizontalAlignment.Left;
            cFrame.VerticalAlignment = VerticalAlignment.Top;
            cFrame.Opacity = 100.0;
            Panel.SetZIndex(cFrame, 1);
            cFrame.Focusable = false;
            innerCanvas.Children.Add(cFrame);

            //4.关闭按钮
            double posR = -25.0;
            double posT = -25.0;
            if (dPageWidth == frameWidth && dPageHeight == frameHeight)
            {
                posR = 0;
                posT = 0;
            }
            Button closebtn = new Button();
            closebtn.Background = new ImageBrush
            {
                ImageSource = FileUtil.readImage(AppDomain.CurrentDomain.BaseDirectory + "/myfile/sysimg/ico-image-close.png")
                ,
                Stretch = Stretch.Fill
            };
            closebtn.Width = 50;
            closebtn.Height = 50;
            closebtn.BorderThickness = new Thickness(0);
            closebtn.HorizontalAlignment = HorizontalAlignment.Left;
            closebtn.VerticalAlignment = VerticalAlignment.Top;
            closebtn.SetValue(Canvas.RightProperty, posR);
            closebtn.SetValue(Canvas.TopProperty, posT);
            Panel.SetZIndex(closebtn, 2);
            innerCanvas.Children.Add(closebtn);


            TransformGroup group = new TransformGroup();
            double scaleX = dControl.width / innerCanvas.Width;
            double scaleY = dControl.height / innerCanvas.Height;
            if (scaleX / scaleY > 2 || scaleY / scaleX > 2)
            {
                scaleX = 0.1;
                scaleY = 0.1;
                left = left + dControl.width * (1 - 0.1) / 2;  //默认比例0.1
                top = top + dControl.height * (1 - 0.1) / 2;
                innerCanvas.SetValue(Canvas.LeftProperty, left);
                innerCanvas.SetValue(Canvas.TopProperty, top);
            }
            ScaleTransform scaleTransform = new ScaleTransform();
            scaleTransform.ScaleX = scaleX;
            scaleTransform.ScaleY = scaleY;
            group.Children.Add(scaleTransform);
            innerCanvas.RenderTransform = group;

            frameDialogCanvas.Children.Add(innerCanvas);

            closebtn.Click += (object sender1, RoutedEventArgs e1) => ShowFrameDialogUtil.closeAnimation(frameDialogCanvas, frameDialogBorder, innerCanvas, dControl, mainContainerTmp);
            frameDialogBorder.PreviewMouseLeftButtonDown += (object sender1, MouseButtonEventArgs e1) => ShowFrameDialogUtil.closeAnimation(frameDialogCanvas, frameDialogBorder, innerCanvas, dControl, mainContainerTmp);
            frameDialogBorder.PreviewTouchDown += (object sender1, TouchEventArgs e1) => ShowFrameDialogUtil.closeAnimation(frameDialogCanvas, frameDialogBorder, innerCanvas, dControl, mainContainerTmp);

            //动画，平移到指定位置
            double toLeft = (dPageWidth - innerCanvas.Width) / 2;
            double toTop = (dPageHeight - innerCanvas.Height) / 2;
            double toTranslateTransformX = toLeft - left;
            double toTranslateTransformY = toTop - top;


            ShowFrameDialogUtil.showAnimation(frameDialogBorder, innerCanvas, dControl, toTranslateTransformX, toTranslateTransformY);
            mainContainerTmp.Children.Add(frameDialogCanvas);

        }

        /*
         * 获取屏幕下的PageTemplate
         */
        private PageTemplate getScreen(int screenCfgId) {
            
            foreach (Window win in App.appWindowList)
            {
                string typename = win.GetType().Name;
                if (typename == "PreviewWindow")
                {
                    PreviewWindow curr = (PreviewWindow)win;
                    if (curr.screenCfgId == screenCfgId)
                    {
                        return curr.pageTemplate;
                    }
                }
                else if(typename == "MainWindow")
                {
                    MainWindow curr = (MainWindow)win;
                    if (curr.screenCfgId == screenCfgId)
                    {
                        return curr.pageTemplate;
                    }
                } 
            } 
             return this.pageTemplate1; 
        }

        /*
         * 1.3 跳转到指定的页面，并伴随页面切换效果
         */ 
        private void linkToPageWithPageSwitchType(DControl ctl )
        { 
            //1.如果显示位置不是 最顶层，则直接显示（无页面切换效果）
            CFrameTag cFrameTag = getCFrameTag(ctl.showInWhichCFrame);
            //2.如果是当前页面
            DPage linkToDPage = dPageBll.get(ctl.linkToPageId);
            if (linkToDPage == null) return;
            if (linkToDPage.pageSwitchType <= 0) linkToDPage.pageSwitchType = 1;

            //滚动条回到开始
            if (cFrameTag.parentFrame != null)
            {
                object parent = cFrameTag.currCFrame.Parent;
                if (parent is ScrollViewer)
                {
                    ScrollViewer sv = (ScrollViewer)parent;
                    sv.ScrollToVerticalOffset(0);
                    sv.ScrollToHorizontalOffset(0);
                }
            }

            //1.直接进入
            if (linkToDPage.pageSwitchType == 1)
            {
                //  cFrameTag.currCFrame.Background = Brushes.White;
                PageTemplate page1 = new PageTemplate(cFrameTag.currCFrame, ctl.linkToPageId, false,pageTemplate1.screen,pageTemplate1.mqServer);
                cFrameTag.currCFrame.NavigationService.Navigate(page1);
                return;
            }
            //2.淡出淡入
            if (linkToDPage.pageSwitchType == 2)
            {
                PageTemplate pt = (PageTemplate)cFrameTag.currCFrame.Content;
                cFrameTag.currCFrame.Background = Brushes.White;
                pt.RenderTransformOrigin = new System.Windows.Point(0.5, 0.5);
                TransformGroup group = new TransformGroup();
                pt.RenderTransform = group;

                DoubleAnimation da = new DoubleAnimation(1.0, 0, new Duration(TimeSpan.FromMilliseconds(400)));
                da.BeginTime = TimeSpan.FromMilliseconds(0);
                IEasingFunction easingFunction = new CubicEase() { EasingMode = EasingMode.EaseIn };
                da.EasingFunction = easingFunction;
                da.Completed += (sender1, e1) => fadeOut_Completed(ctl, cFrameTag );
                pt.BeginAnimation(UIElement.OpacityProperty, da);

                return;
            }

            //3.右侧移入
            if (linkToDPage.pageSwitchType == 3)
            {
                PageTemplate pt = (PageTemplate)cFrameTag.currCFrame.Content;
                string shotImage = ShotUtil.shot(pt);
                cFrameTag.currCFrame.Background = new ImageBrush
                {
                    ImageSource = FileUtil.readImage(AppDomain.CurrentDomain.BaseDirectory + shotImage)
                };
                PageTemplate page = new PageTemplate(cFrameTag.currCFrame, ctl.linkToPageId, true, pageTemplate1.screen,pageTemplate1.mqServer);
                cFrameTag.currCFrame.NavigationService.Navigate(page);
                FrameUtil.RemoveBackEntry(cFrameTag.currCFrame);
                return;
            }

            //4.右侧拉伸
            if (linkToDPage.pageSwitchType == 4)
            {
                PageTemplate pt = (PageTemplate)cFrameTag.currCFrame.Content;
                string shotImage = ShotUtil.shot(pt);
                cFrameTag.currCFrame.Background = new ImageBrush
                {
                    ImageSource = FileUtil.readImage(AppDomain.CurrentDomain.BaseDirectory + shotImage)
                };
                PageTemplate page = new PageTemplate(cFrameTag.currCFrame, ctl.linkToPageId, true, pageTemplate1.screen,pageTemplate1.mqServer);
                cFrameTag.currCFrame.NavigationService.Navigate(page);
                FrameUtil.RemoveBackEntry(cFrameTag.currCFrame);
                return;
            }

            //5.中心放大
            if (linkToDPage.pageSwitchType == 5)
            {
                PageTemplate pt = (PageTemplate)cFrameTag.currCFrame.Content;
                string shotImage = ShotUtil.shot(pt);
                cFrameTag.currCFrame.Background = new ImageBrush
                {
                    ImageSource = FileUtil.readImage(AppDomain.CurrentDomain.BaseDirectory + shotImage)
                };
                PageTemplate page = new PageTemplate(cFrameTag.currCFrame, ctl.linkToPageId, true, pageTemplate1.screen,pageTemplate1.mqServer);
                cFrameTag.currCFrame.NavigationService.Navigate(page);
                FrameUtil.RemoveBackEntry(cFrameTag.currCFrame);
                return;
            }
        }

        /*
        * 进入新页面
        * 
        * 2.淡出完成，开始淡入
        */
        private void fadeOut_Completed(DControl ctl, CFrameTag cFrameTag )
        { 
            PageTemplate pageTemplate = new PageTemplate(cFrameTag.currCFrame, ctl.linkToPageId, true, ctl.isTransparentDialog,pageTemplate1.screen,pageTemplate1.mqServer);
            cFrameTag.currCFrame.NavigationService.Navigate(pageTemplate);
            FrameUtil.RemoveBackEntry(cFrameTag.currCFrame);
        }

        /*
         *  1.4 链接到全屏视频
         */
        public void insertFullVideoToPage(  DControlDto dControlDto)
        {
            StorageVideo storageVideo = storageVideoBll.get(dControlDto.linkToVideoId);


            int frameWidth = App.localStorage.cfg.screenWidth;
            int frameHeight = App.localStorage.cfg.screenHeight;
            if (dPage.width > 0) frameWidth = dPage.width;
            if (dPage.height > 0) frameHeight = dPage.height;
            Cfg cfg = new Cfg();
            cfg.screenWidth = frameWidth;
            cfg.screenHeight = frameHeight;


            if (storageVideo == null)
            {
                storageVideo = new StorageVideo();
                storageVideo.url = "/myfile/sysimg/notExists/video.mp4";
                storageVideo.origFilename = "演示视频.mp4";
            }
            StorageVideoDto dto = StorageVideoUtil.convert(storageVideo);
            StorageImage storageImage = storageImageBll.get(dto.storageImageId);
            dto.storageImageUrl = storageImage?.url;

            DControl ctl = new DControl();
            ctl.id = 10003;
            ctl.pageId = 0;
            ctl.name = "cVideo";
            ctl.width = cfg.screenWidth;
            ctl.height = cfg.screenHeight;
            ctl.left = 0;
            ctl.top = 0;
            ctl.type = "Video";
            ctl.content = FileUtil.getFilenameTitle(storageVideo?.origFilename);
            ctl.idx = 0; /////
            ctl.linkToPageId = 0;
            ctl.isClickShow = false;
            ctl.linkToVideoId = 0;
            ctl.autoplay = true;
            ctl.loop = false;
            ctl.turnPictureSpeed = 0;
            ctl.storageId = storageVideo.id;


            Canvas fullVideoCanvas = new Canvas();
            fullVideoCanvas.Name = "fullVideoCanvas";
            fullVideoCanvas.Width = ctl.width;
            fullVideoCanvas.Height = ctl.height;
            fullVideoCanvas.HorizontalAlignment = HorizontalAlignment.Left;
            fullVideoCanvas.VerticalAlignment = VerticalAlignment.Top;
            TransformGroup group = new TransformGroup();
            double scaleX = dControlDto.width / Convert.ToDouble(ctl.width);
            double scaleY = dControlDto.height / Convert.ToDouble(ctl.height);
            ScaleTransform scaleTransform = new ScaleTransform();
            scaleTransform.ScaleX = scaleX;
            scaleTransform.ScaleY = scaleY;
            group.Children.Add(scaleTransform);
            fullVideoCanvas.RenderTransform = group;
            double left = dControlDto.left;
            double top = dControlDto.top;
            fullVideoCanvas.Margin = new Thickness(left, top, 0, 0);
            Panel.SetZIndex(fullVideoCanvas, 10003);

            CVideo cVideo = new CVideo(ctl, true, cfg, dto, pageTemplate1.mqServer,true);
            cVideo.Name = "cVideo";
            cVideo.HorizontalAlignment = HorizontalAlignment.Left;
            cVideo.VerticalAlignment = VerticalAlignment.Top;
            cVideo.Width = ctl.width;
            cVideo.Height = ctl.height;
            cVideo.Background = null;
            cVideo.Tag = ctl;


            //动画，平移到指定位置   
            double toTranslateTransformX = 0 - dControlDto.left;
            double toTranslateTransformY = 0 - dControlDto.top;
            fullVideoCanvas.Children.Add(cVideo);
            AnimationUtil.loadFullVideo(fullVideoCanvas, toTranslateTransformX, toTranslateTransformY);


            pageTemplate1.container.Children.Add(fullVideoCanvas);

            //是否显示关闭按钮
            Button closebtn = cVideo.GetClosebtn();
            closebtn.Visibility = Visibility.Visible;
            closebtn.Tag = true;
            closebtn.Click += (object sender, RoutedEventArgs e) => closeVideoAnimation(fullVideoCanvas, dControlDto, cVideo);

        }

        /*
          * 关闭视频
          * 
          * 动画，回到原位，移除大图
          */
        private void closeVideoAnimation(Canvas fullVideoCanvas, DControlDto dControlDto, CVideo cVideo)
        {
            //1.淡出 
            DoubleAnimation da = new DoubleAnimation(fullVideoCanvas.Opacity, 0, new Duration(TimeSpan.FromMilliseconds(100)));
            da.BeginTime = TimeSpan.FromMilliseconds(200);
            fullVideoCanvas.BeginAnimation(UIElement.OpacityProperty, da);

            //触摸缩放后
            double backToScaleX = dControlDto.width / fullVideoCanvas.Width;
            double backToScaleY = dControlDto.height / fullVideoCanvas.Height;
            Transform transform = fullVideoCanvas.RenderTransform;
            TransformGroup group = (TransformGroup)fullVideoCanvas.RenderTransform;

            //2.缩放
            ScaleTransform scaleTransform = TransformGroupUtil.GetScaleTransform(group);
            DoubleAnimation da1 = new DoubleAnimation(scaleTransform.ScaleX, backToScaleX, new Duration(TimeSpan.FromMilliseconds(300)));
            scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, da1);
            DoubleAnimation da2 = new DoubleAnimation(scaleTransform.ScaleY, backToScaleY, new Duration(TimeSpan.FromMilliseconds(300)));
            scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, da2);


            // 3.平移
            TranslateTransform translateTransform = TransformGroupUtil.GetTranslateTransform(group);
            DoubleAnimation da3 = new DoubleAnimation(translateTransform.X, 0, new Duration(TimeSpan.FromMilliseconds(300)));
            translateTransform.BeginAnimation(TranslateTransform.XProperty, da3);
            DoubleAnimation da4 = new DoubleAnimation(translateTransform.Y, 0, new Duration(TimeSpan.FromMilliseconds(300)));
            da4.Completed += (sender, e) => closeVideoAnimation_Completed(fullVideoCanvas, cVideo, dControlDto);
            translateTransform.BeginAnimation(TranslateTransform.YProperty, da4);
        }

        private void closeVideoAnimation_Completed(Canvas fullVideoCanvas, CVideo cVideo, DControlDto dto)
        {
            pageTemplate1.mqServer.SendMsg(new VideoControlMessage(dto.id, "fullScreenExit"));
            pageTemplate1.mqServer.SendMsg(new VideoControlMessage(dto.id, "pause"));
            cVideo = null;
            pageTemplate1.container.Children.Remove(fullVideoCanvas);
        }


        /*
         * 2.1 Word 滑动到底反馈取消
        */
        private void scrollViewerManipulationBoundaryFeedback(object sender, ManipulationBoundaryFeedbackEventArgs e)
        {
            e.Handled = true;
        }


        /*
         *  3.1 委托事件处理程序，链接到
         * 
         *  相册、层叠相册、流动相册
         * 
         */
        public void TurnPictureItemFun(TurnPictureImagesDto dto, double width, double height, Point point, int pageId)
        {
            if (dto == null) return;
            //当前主控件 相册  层叠相册  流动相册
            DControl mainDControl = dControlBll.get(dto.dControlId);
            if (mainDControl == null) return;
             
             
            DControlDto dControlDto = new DControlDto();
            dControlDto.id = dto.id; 
            dControlDto.width = Convert.ToInt32(width);
            dControlDto.height = Convert.ToInt32(height);
            dControlDto.left = Convert.ToInt32(point.X);
            dControlDto.top = Convert.ToInt32(point.Y);
            dControlDto.pageId = pageId;
            

            this.DifferentScreenDifferentLinkTo(dControlDto,dto.id);
            
            if (mainDControl.isClickShow)
            {
                int frameWidth = App.localStorage.cfg.screenWidth;
                int frameHeight = App.localStorage.cfg.screenHeight;
                if (dPage.width > 0) frameWidth = dPage.width;
                if (dPage.height > 0) frameHeight = dPage.height;

                Cfg cfg = new Cfg();
                cfg.screenWidth = frameWidth;
                cfg.screenHeight = frameHeight;

                ShowBigImageUtil.showBigImage(dto.url, dControlDto, mainContainer, cfg);
            }
        }

        /*
        * 4.1 返回按钮单击
        */
        public void backButtonClick(object sender, RoutedEventArgs e)
        {
            Button backButton = (Button)sender;
            DControl dControl = (DControl)backButton.Tag;

            DPage dPage = dPageBll.get(dControl.pageId);
            if (dPage.parentId <= 0) return;

            //设置返回哪一个页面 
            dControl.linkToPageId = dPage.parentId;

            int pageSwitchType = dPage.pageSwitchType;
            if (pageSwitchType <= 0) pageSwitchType = 1;
           
            //1.默认
            if (pageSwitchType == 1)
            {
                PageTemplate page = new PageTemplate(pageTemplate1.mainFrame, dPage.parentId, true, true, pageSwitchType,pageTemplate1.screen,pageTemplate1.mqServer);
                pageTemplate1.NavigationService.Navigate(page);
                return;
            }
            //2.淡出
            if (pageSwitchType == 2)
            {
                TransformGroup group = new TransformGroup();
                pageTemplate1.RenderTransform = group;

                DoubleAnimation da = new DoubleAnimation(1.0, 0, new Duration(TimeSpan.FromMilliseconds(500)));
                da.BeginTime = TimeSpan.FromMilliseconds(0);
                IEasingFunction easingFunction = new SineEase() { EasingMode = EasingMode.EaseIn };
                da.EasingFunction = easingFunction;
                da.Completed += (sender1, e1) => fadeOut_Completed(dControl,   true, pageSwitchType);
                pageTemplate1.BeginAnimation(UIElement.OpacityProperty, da);
                return;
            }

            //3.右侧移入 -> 左侧移出
            if (pageSwitchType == 3)
            {
                Border CoverBorder = null;
                CFrameTag tag = (CFrameTag)pageTemplate1.mainFrame.Tag;
                CoverBorder = tag.currCoverBorder;


                //截图  
                string shotImage = ShotUtil.shot(pageTemplate1);
                //显示上一页  
                BitmapImage bitmapImage = FileUtil.readImage(AppDomain.CurrentDomain.BaseDirectory + shotImage);
                CoverBorder.Background = new ImageBrush
                {
                    ImageSource = bitmapImage
                        ,
                    Stretch = Stretch.Fill
                };
                CoverBorder.Visibility = Visibility.Visible;
                CoverBorder.UpdateLayout();


                PageTemplate page = new PageTemplate(pageTemplate1.mainFrame, dPage.parentId, true, true, pageSwitchType, pageTemplate1.screen,pageTemplate1.mqServer);
                pageTemplate1.NavigationService.Navigate(page);


                //移出当前封面 
                int pageWidth = PageWidthUtil.getPageWidth(dPage.width, App.localStorage.cfg.screenWidth);
                CoverBorderUtil.TranslateXMoveOut(CoverBorder, pageWidth);

                return;
            }


            //4.右侧拉伸 -> 左侧收缩
            if (pageSwitchType == 4)
            {
                 
                pageTemplate1.mainFrame.Background = Brushes.White;
                Border CoverBorder = null;
                CFrameTag tag = (CFrameTag)pageTemplate1.mainFrame.Tag;
                CoverBorder = tag.currCoverBorder;
                //截图 
                string shotImage = ShotUtil.shot(pageTemplate1);
                CoverBorder.Background = new ImageBrush
                {
                    ImageSource = FileUtil.readImage(AppDomain.CurrentDomain.BaseDirectory + shotImage)
                         ,
                    Stretch = Stretch.Fill
                };
                CoverBorder.Visibility = Visibility.Visible;

                //显示上一页
                PageTemplate page = new PageTemplate(pageTemplate1.mainFrame, dPage.parentId, true, true, pageSwitchType, pageTemplate1.screen,pageTemplate1.mqServer);
                pageTemplate1.NavigationService.Navigate(page);

                //移出当前封面 
                CoverBorderUtil.ScaleXMoveOut(CoverBorder);
                return;
            }


            //5.中心放大 -> 中心收缩
            if (pageSwitchType == 5)
            {
                pageTemplate1.mainFrame.Background = Brushes.White;
                Border CoverBorder = null;
                CFrameTag tag = (CFrameTag)pageTemplate1.mainFrame.Tag;
                CoverBorder = tag.currCoverBorder;
                //截图 
                string shotImage = ShotUtil.shot(pageTemplate1);
                CoverBorder.Background = new ImageBrush
                {
                    ImageSource = FileUtil.readImage(AppDomain.CurrentDomain.BaseDirectory + shotImage)
                         ,
                    Stretch = Stretch.Fill
                };
                CoverBorder.Visibility = Visibility.Visible;

                //显示上一页
                PageTemplate page = new PageTemplate(pageTemplate1.mainFrame, dPage.parentId, true, true, pageSwitchType, pageTemplate1.screen,pageTemplate1.mqServer);
                pageTemplate1.NavigationService.Navigate(page);

                //移出当前封面 
                CoverBorderUtil.CenterMoveOut(CoverBorder);
                return;
            }

        }
        /*
        * 返回上一页
        * 
        * 2.淡出完成，开始淡入
        */
        private void fadeOut_Completed(DControl ctl,   Boolean isBack, int pageSwitchType)
        {
            PageTemplate pt = new PageTemplate(pageTemplate1.mainFrame, ctl.linkToPageId, true, isBack, pageSwitchType, pageTemplate1.screen,pageTemplate1.mqServer);
            pageTemplate1.mainFrame.NavigationService.Navigate(pt);
            FrameUtil.RemoveBackEntry(pageTemplate1.mainFrame);
        }

        /*
        * 5.1 单击回到首页 
        */
        public void homeButtonClick(object sender, RoutedEventArgs e)
        {
            PageTemplate page = new PageTemplate(pageTemplate1.mainFrame, 1, false, pageTemplate1.screen,pageTemplate1.mqServer);
            pageTemplate1.NavigationService.Navigate(page);
        }

        public void textBlock_PreviewTouchUp(object sender, TouchEventArgs e)
        {
            textBlockFun(sender); 
        }

        public void textBlock_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            textBlockFun(sender);
        }
        /* 
        * 6.1点击文字跳转
        * 
        * 1.单击文本跳转到页面
        * 
        * 2.跳转到外部网站
        * 
        * 3.单击文本全屏播放视频
        * 
        * 4.单击文本放大
        * 
        * 
        */
        private void textBlockFun(object sender)
        {
            TextBlock textBlock = (TextBlock)sender;
            DControl ctl = (DControl)textBlock.Tag;
            this.DifferentScreenDifferentLinkTo(ctl);
            if (!string.IsNullOrWhiteSpace(ctl.linkToWeb))
            {
                Int32 pagePercent = FrameUtil.getMaxPercent(App.localStorage.cfg.screenWidth, App.localStorage.cfg.screenHeight);
                double screenWidth = SystemParameters.PrimaryScreenWidth;//得到屏幕整体宽度  
                int winWidth = (int)(App.localStorage.cfg.screenWidth * pagePercent / 100);
                int winHeight = (int)(App.localStorage.cfg.screenHeight * pagePercent / 100);

                App.localStorage.currForm1 = new Form1(winWidth, winHeight, ctl.linkToWeb, screenWidth);
                App.localStorage.currForm1.ShowDialog();
            }
        }

        public void cAudio_PreviewTouchUp(object sender, TouchEventArgs e)
        {
            CAudio cAudio = (CAudio)sender;
            cAudio.togglePlay();
        }

        public void cAudio_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            CAudio cAudio = (CAudio)sender;
            cAudio.togglePlay();
        }

        public void gif_PreviewTouchUp(object sender, TouchEventArgs e)
        {
            this.gifFun(sender);
        }

        public void gif_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            this.gifFun(sender);
        }

        /* 
        * 7.1 单击gif跳转
        * 
        * 1.单击图片跳转到页面
        * 
        * 2.跳转到外部网站
        * 
        * 3.单击图片全屏播放视频
        * 
        * 4.单击图片放大
        * 
        * 
        */
        private void gifFun(object sender)
        {
            Gif gif = (Gif)sender;
            DControlDto ctl = (DControlDto)gif.Tag;
            this.DifferentScreenDifferentLinkTo(ctl);
             if (!string.IsNullOrWhiteSpace(ctl.linkToWeb))
            {
                Int32 pagePercent = FrameUtil.getMaxPercent(App.localStorage.cfg.screenWidth, App.localStorage.cfg.screenHeight);
                double screenWidth = SystemParameters.PrimaryScreenWidth;//得到屏幕整体宽度  
                int winWidth = (int)(App.localStorage.cfg.screenWidth * pagePercent / 100);
                int winHeight = (int)(App.localStorage.cfg.screenHeight * pagePercent / 100);

                App.localStorage.currForm1 = new Form1(winWidth, winHeight, ctl.linkToWeb, screenWidth);
                App.localStorage.currForm1.ShowDialog();
            }
            else if (ctl.linkToVideoId > 0)
            { 
                insertFullVideoToPage(  ctl);
            }
            else if (ctl.isClickShow)
            {
                int frameWidth = App.localStorage.cfg.screenWidth;
                int frameHeight = App.localStorage.cfg.screenHeight;
                if (dPage.width > 0) frameWidth = dPage.width;
                if (dPage.height > 0) frameHeight = dPage.height;

                Cfg cfg = new Cfg();
                cfg.screenWidth = frameWidth;
                cfg.screenHeight = frameHeight;

                StorageGif storageGif = storageGifBll.get(ctl.storageId);
                if (storageGif == null)
                {  //默认gif
                    storageGif = new StorageGif();
                    storageGif.id = 0;
                    storageGif.url = Params.GifNotExists;
                    storageGif.actualWidth = 550;
                    storageGif.actualHeight = 400;
                    storageGif.ext = ".gif";
                    storageGif.origFilename = "gif.gif";
                    storageGif.size = 304874;
                    storageGif.folderId = 1;
                }

                ShowBigGifUtil.showBigImage(storageGif, ctl, mainContainer, cfg);
            }
        }

    }
}
