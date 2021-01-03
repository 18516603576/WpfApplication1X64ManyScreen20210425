using Bll;
using Common;
using Common.control;
using Common.Data;
using Common.util;
using Model;
using Model.dto;
using ShowBox.control;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using static Common.control.Marque;
using static Common.control.MarqueLayer;
using static Common.control.TurnPicture;

namespace ShowBox.manage
{
    public partial class InsertToPage
    {
        private readonly Grid mainContainer;
        private readonly DPage dPage;
        private readonly PageTemplate pageTemplate1;
        private readonly StorageVideoBll storageVideoBll = new StorageVideoBll();
        private readonly StorageImageBll storageImageBll = new StorageImageBll();
        private readonly StorageGifBll storageGifBll = new StorageGifBll();
        private readonly StorageFileBll storageFileBll = new StorageFileBll();
        private readonly TurnPictureImagesBll turnPictureImagesBll = new TurnPictureImagesBll();
        private readonly DPageBll dPageBll = new DPageBll();
        private readonly DControlAnimationBll dControlAnimationBll = new DControlAnimationBll();
        private readonly Frame mainFrame;
        private readonly DControlBll dControlBll = new DControlBll();

        //PageTemplate pageTemplate, Int32 pageId 
        public InsertToPage(Frame mainFrame, Grid mainContainer, DPage dPage, PageTemplate pageTemplate)
        {
            this.mainFrame = mainFrame;
            this.mainContainer = mainContainer;
            this.dPage = dPage;
            pageTemplate1 = pageTemplate;
        }
        /*
        * 将控件插入到当前页面
        */
        public void insertControl(DControl ctl)
        {
            if (ctl.type == "Image")
            {
                insertImageToPage(ctl);
            }
            else if (ctl.type == "Word")
            {
                insertWordToPage(ctl);
            }
            else if (ctl.type == "TurnPicture")
            {
                insertTurnPictureToPage(ctl);
            }
            else if (ctl.type == "Marque")
            {
                insertMarqueToPage(ctl);
            }
            else if (ctl.type == "MarqueLayer")
            {
                insertMarqueLayerToPage(ctl);
            }
            else if (ctl.type == "Video")
            { 
                insertVideoToPage(ctl); 
            }
            else if (ctl.type == "BackButton")
            {
                insertBackButtonToPage(ctl);
            }
            else if (ctl.type == "HomeButton")
            {
                insertHomeButtonToPage(ctl);
            }

            else if (ctl.type == "CFrame")
            {
                insertCFrameToPage(ctl);
            }
            else if (ctl.type == "TextBlock")
            {
                insertTextBlockToPage(ctl);
            }
            else if (ctl.type == "Gif")
            {
                insertGifToPage(ctl);
            }
            else if (ctl.type == "CCalendar")
            {
                insertCCalendarToPage(ctl);
            }
            else if (ctl.type == "CAudio")
            {
                insertCAudioToPage(ctl);
            }
        }

      

        /*
         * 3.1插入图片
         */
        public void insertImageToPage(DControl ctl)
        {
            StorageImage storageImage = storageImageBll.get(ctl.storageId);
            Button imageButton = NewControlUtil.newImage(ctl, storageImage);

            loadAllAnimation(imageButton, ctl);
            imageButton.Click += imageButtonClick;
            mainContainer.Children.Add(imageButton);
        }


        /*
         * 获取显示位置
         * 
         * 当前窗口，则返回null
         * 
         * 窗口不存在，返回null
         * 
         * 其他，返回CFrameTag
         */
        private CFrameTag getCFrameTag(int showInWhichCFrame)
        {
            
          //  int showInWhichCFrame = ctl.showInWhichCFrame;
            //DControl cFrameDControl = dControlBll.get(ctl.showInWhichCFrame);
            //if (cFrameDControl.linkToPageId == ctl.pageId) { 
            //   //当前页面
            //} 
            //1.获取当前页面下的CFrame
            foreach (FrameworkElement ele in mainContainer.Children)
            {
                if (ele is CFrame)
                {
                    CFrame cFrame = (CFrame)ele;
                    DControl dc = (DControl)cFrame.Tag;
                    if (showInWhichCFrame == dc.id)
                    {
                        Frame frame = cFrame.getFrame();
                        CFrameTag cFrameTag1 = (CFrameTag)frame.Tag;
                        return cFrameTag1;
                    }
                }
            }

            //2.当前即顶层frame
            object tag = pageTemplate1.mainFrame.Tag;
            CFrameTag cFrameTag = (CFrameTag)tag;
            if (cFrameTag.parentFrame == null)
            {
                return cFrameTag;
            }
            //3.判断是否当前frame 
            if (showInWhichCFrame == cFrameTag.currDControl.id || showInWhichCFrame == 0)
            {
                //当前页面 
                return cFrameTag;
            }

            //4.判断是否父框架
            CFrameTag isInParent = isInParentCFrame(cFrameTag.parentFrame, showInWhichCFrame);
            return isInParent;

        }
        /*
         * 是否要显示在某个父窗口中、
         */
        private CFrameTag isInParentCFrame(Frame frame, int showInWhichCFrame)
        {
            // CFrameTag result = null;
            object tag = frame.Tag;
            CFrameTag cFrameTag = (CFrameTag)tag;
            //如果不显示在子窗口，则显示于主窗口
            if (cFrameTag.parentFrame == null)
            {
                return cFrameTag;
            }

            if (showInWhichCFrame == cFrameTag.currDControl.id)
            {
                //当前页面
                return cFrameTag;
            }
            else
            {
                return isInParentCFrame(cFrameTag.parentFrame, showInWhichCFrame);
            }
            // return result; 
        }


        /*
         * 3.2插入Word
         */
        public void insertWordToPage(DControl ctl)
        {
            StorageFile storageFile = storageFileBll.get(ctl.storageId);
            DocumentViewer docViewer = NewControlUtil.newWord(ctl, storageFile);

            System.Windows.Style myStyle = (System.Windows.Style)mainContainer.FindResource("DocumentViewerDefaultStyle");
            docViewer.Style = myStyle;
            docViewer.SizeChanged += WordUtil.Word_SizeChanged;
            docViewer.ManipulationBoundaryFeedback += scrollViewerManipulationBoundaryFeedback;

            loadAllAnimation(docViewer, ctl);
            mainContainer.Children.Add(docViewer);
        }




        /*
         * 3.3插入轮播图  TurnPicture
         */
        public void insertTurnPictureToPage(DControl ctl)
        {
            List<TurnPictureImagesDto> list = turnPictureImagesBll.getByDControlId(ctl.id);
            for (int i = 0; i < list.Count; i++)
            {
                TurnPictureImagesDto dto = list[i];
                if (dto == null) { list.Remove(dto); }
                string imgNotExists = Params.ImageNotExists;
                if (i < 6 && dto.storageImageId == 0)
                {
                    imgNotExists = Params.TurnPictureNotExists[i];
                }
                string imgFullPath = FileUtil.notExistsShowDefault(dto.url, imgNotExists);
                dto.url = imgFullPath;
            }

            TurnPicture turnPicture = NewControlUtil.newTurnPicture(ctl, list, false);
            loadAllAnimation(turnPicture, ctl);
            mainContainer.Children.Add(turnPicture);
            turnPicture.TurnPictureItemEvent += new TurnPictureItemHandler(TurnPictureItemFun);
        }

        /*
         * 3.3插入流动相册
         */
        public void insertMarqueToPage(DControl ctl)
        {
            List<TurnPictureImagesDto> list = turnPictureImagesBll.getByDControlId(ctl.id);
            for (int i = 0; i < list.Count; i++)
            {
                TurnPictureImagesDto dto = list[i];
                if (dto == null) { list.Remove(dto); }
                string imgNotExists = Params.ImageNotExists;
                if (i < 6 && dto.storageImageId == 0)
                {
                    imgNotExists = Params.TurnPictureNotExists[i];
                }
                string imgFullPath = FileUtil.notExistsShowDefault(dto.url, imgNotExists);
                dto.url = imgFullPath;
            }
            Marque marque = NewControlUtil.newMarque(ctl, list, false);
            loadAllAnimation(marque, ctl);
            mainContainer.Children.Add(marque);
            marque.MarqueItemEvent += new MarqueItemHandler(TurnPictureItemFun);
        }

        /*
         * 3.3插入层叠相册
         */
        public void insertMarqueLayerToPage(DControl ctl)
        {
            List<TurnPictureImagesDto> list = turnPictureImagesBll.getByDControlId(ctl.id);
            for (int i = 0; i < list.Count; i++)
            {
                TurnPictureImagesDto dto = list[i];
                if (dto == null) { list.Remove(dto); }
                string imgNotExists = Params.ImageNotExists;
                if (i < 6 && dto.storageImageId == 0)
                {
                    imgNotExists = Params.TurnPictureNotExists[i];
                }
                string imgFullPath = FileUtil.notExistsShowDefault(dto.url, imgNotExists);
                dto.url = imgFullPath;
            }

            MarqueLayer marqueLayer = NewControlUtil.newMarqueLayer(ctl, list, false);
            loadAllAnimation(marqueLayer, ctl);
            mainContainer.Children.Add(marqueLayer);
            marqueLayer.MarqueLayerItemEvent += new MarqueLayerItemHandler(TurnPictureItemFun);
        }


        /*
        * 3.4插入视频  Video
        */
        public void insertVideoToPage(DControl ctl)
        {
            //获取视频所在的集合
            StorageVideo storageVideo = storageVideoBll.get(ctl.storageId);
            if (storageVideo == null)
            {
                storageVideo = new StorageVideo();
                storageVideo.url = "/myfile/sysimg/notExists/video.mp4";
                storageVideo.origFilename = "演示视频.mp4";
            }
            StorageVideoDto dto = StorageVideoUtil.convert(storageVideo);
            StorageImage storageImage = storageImageBll.get(dto.storageImageId);
            dto.storageImageUrl = storageImage?.url;

            //foreach (Window win in App.appWindowList)
            //{
            //    string type = win.GetType().Name;
            //    Console.WriteLine("窗口类型：" + type);
            //}


            Cfg pageCfg = PageWidthUtil.getPageCfg(dPage, App.localStorage.cfg);
            CVideo cVideo = NewControlUtil.newCVideo(ctl, dto, pageCfg, pageTemplate1.mqServer, false); 
            loadAllAnimation(cVideo, ctl);
            mainContainer.Children.Add(cVideo);
        }


        /*
         * 插入返回按钮
         */
        internal void insertBackButtonToPage(DControl ctl)
        {
            StorageImage storageImage = storageImageBll.get(ctl.storageId);
            Button backButton = NewControlUtil.newBackButton(ctl, storageImage);
            loadAllAnimation(backButton, ctl);
            backButton.Click += backButtonClick;
            mainContainer.Children.Add(backButton);
        }




        internal void insertHomeButtonToPage(DControl ctl)
        {
            StorageImage storageImage = storageImageBll.get(ctl.storageId);
            Button imageButton = NewControlUtil.newHomeButton(ctl, storageImage);
            loadAllAnimation(imageButton, ctl);
            imageButton.Click += homeButtonClick;
            mainContainer.Children.Add(imageButton);
        }



        /*
         * 插入小窗口到页面
         */
        internal void insertCFrameToPage(DControl ctl)
        {
            CFrame cFrame = new CFrame(mainFrame, ctl, App.localStorage.cfg, pageTemplate1.screen,pageTemplate1.mqServer);
            cFrame.BorderThickness = new Thickness(0);
            cFrame.Margin = new Thickness(ctl.left, ctl.top, 0, 0);
            cFrame.Width = ctl.width;
            cFrame.Height = ctl.height;
            cFrame.HorizontalAlignment = HorizontalAlignment.Left;
            cFrame.VerticalAlignment = VerticalAlignment.Top;
            cFrame.Opacity = ctl.opacity / 100.0;
            Panel.SetZIndex(cFrame, ctl.idx);
            cFrame.Tag = ctl;
            TransformGroup group = new TransformGroup();
            cFrame.RenderTransform = group;
            cFrame.RenderTransformOrigin = new Point(0.5, 0.5);
            cFrame.Focusable = false;

            loadAllAnimation(cFrame, ctl);
            mainContainer.Children.Add(cFrame);
        }

        /*
        * 3.2插入一行文字
        */
        public void insertTextBlockToPage(DControl ctl)
        {
            TextBlock textBlock = NewControlUtil.newTextBlock(ctl);
            loadAllAnimation(textBlock, ctl);
            textBlock.PreviewMouseUp +=  textBlock_PreviewMouseUp ;
            textBlock.PreviewTouchUp +=  textBlock_PreviewTouchUp ;
            mainContainer.Children.Add(textBlock);
        }


        /*
* 3.2插入日期
*/
        public void insertCCalendarToPage(DControl ctl)
        {
            CCalendar cCalendar = NewControlUtil.newCCalendar(ctl, dPage, App.localStorage.cfg);
            loadAllAnimation(cCalendar, ctl);
            mainContainer.Children.Add(cCalendar);
            cCalendar.Visibility = Visibility.Visible;
        }
        /*
     * 3.2插入音频
     */
        public void insertCAudioToPage(DControl ctl)
        {
            string audioUrl = Params.CAudioNotExists;
            if (ctl.storageId > 0)
            {
                StorageFile storageFile = storageFileBll.get(ctl.storageId);
                audioUrl = storageFile?.url;
            }
            string audioCoverUrl = Params.CAudioImageNotExists;
            if (ctl.storageIdOfCover > 0)
            {
                StorageImage storageImage = storageImageBll.get(ctl.storageIdOfCover);
                audioCoverUrl = storageImage?.url;
            }
            CAudio cAudio = NewControlUtil.newCAudio(ctl, dPage, App.localStorage.cfg, audioUrl, audioCoverUrl, pageTemplate1.mqServer);
            loadAllAnimation(cAudio, ctl);
            mainContainer.Children.Add(cAudio); 
            cAudio.Visibility = Visibility.Visible;
            cAudio.PreviewMouseUp += cAudio_PreviewMouseUp;
            cAudio.PreviewTouchUp += cAudio_PreviewTouchUp;
        }

       

        /*
       * 3.2插入一行文字
       */
        public void insertGifToPage(DControl ctl)
        {
            StorageGif storageGif = storageGifBll.get(ctl.storageId);
            Gif gif = NewControlUtil.newGif(ctl, storageGif, false);
            loadAllAnimation(gif, ctl);
            gif.PreviewMouseUp +=  gif_PreviewMouseUp;
            gif.PreviewTouchUp +=   gif_PreviewTouchUp;
            mainContainer.Children.Add(gif);
        }




        /*
        * 加载所有动画效果
        */
        public void loadAllAnimation(FrameworkElement currElement, DControl currDControl)
        {
            List<DControlAnimation> list = dControlAnimationBll.getByDControlId(currDControl.id);
            AnimationUtil.loadAllAnimation(currElement, currDControl, list, App.localStorage.cfg);
        }

    }
}
