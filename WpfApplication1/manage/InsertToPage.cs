using Bll;
using Common;
using Common.control;
using Common.Data;
using Common.util;
using Model;
using Model.dto;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WpfApplication1.control;

namespace WpfApplication1.manage
{
    public partial class Editing
    {
        private readonly StorageVideoBll storageVideoBll = new StorageVideoBll();
        private readonly StorageGifBll storageGifBll = new StorageGifBll();
        private readonly StorageFileBll storageFileBll = new StorageFileBll();
        private readonly DPageBll dPageBll = new DPageBll();

        /*
         * 初始化插入数据到页面
         */
        private void InsertToPage()
        {
            List<DControl> list = null;
            list = dControlBll.getByPageId(pageTemplate.dPage.id);
            if (list == null) { return; }


            object tag = mainFrame.Tag;
            if (tag != null)
            {
                DControl cFrameDControl = (DControl)tag;

            }
            foreach (DControl ctl in list)
            {
                insertOneControl(ctl);
            }
        }
        /*
        * 将控件插入到当前页面
        */
        public void insertOneControl(DControl ctl)
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
            imageButton.Style = pageTemplate.container.TryFindResource("ImageButtonStyle") as System.Windows.Style;
            //控件拖动
            imageButton.PreviewMouseLeftButtonDown += control_MouseDown;
            imageButton.PreviewMouseMove += control_MouseMove;
            imageButton.PreviewMouseLeftButtonUp += control_MouseUp;
            //控件上右击显示菜单 
            imageButton.MouseRightButtonUp += control_MouseRightButtonUp;

            pageTemplate.container.Children.Add(imageButton);
        }


        /*
   * 3.2插入一行文字
   */
        public void insertTextBlockToPage(DControl ctl)
        {
            TextBox textBox = NewControlUtil.newTextBox(ctl);
            System.Windows.Style myStyle = (System.Windows.Style)pageTemplate.container.FindResource("TextBoxDefaultStyle");
            textBox.Style = myStyle;

            textBox.PreviewMouseLeftButtonDown += control_MouseDown;
            textBox.PreviewMouseMove += control_MouseMove;
            textBox.PreviewMouseLeftButtonUp += control_MouseUp;
            textBox.LostFocus += textBox_LostFocus;
            //控件上右击显示菜单 
            textBox.MouseRightButtonUp += control_MouseRightButtonUp;
            textBox.SizeChanged += WordUtil.Word_SizeChanged;

            pageTemplate.container.Children.Add(textBox);
        }


        /*ControlTemplate.TargetType”时引发了异常。”，行号为“8”，行位置为“18”。”

       * 3.2插入Word
       */
        public void insertWordToPage(DControl ctl)
        {
            StorageFile storageFile = storageFileBll.get(ctl.storageId);
            DocumentViewer docViewer = NewControlUtil.newWord(ctl, storageFile);
            System.Windows.Style myStyle = (System.Windows.Style)pageTemplate.container.FindResource("DocumentViewerDesignDefaultStyle");
            docViewer.Style = myStyle;


            docViewer.PreviewMouseLeftButtonDown += control_MouseDown;
            docViewer.PreviewMouseMove += control_MouseMove;
            docViewer.PreviewMouseLeftButtonUp += control_MouseUp;
            //控件上右击显示菜单 
            docViewer.MouseRightButtonUp += control_MouseRightButtonUp;
            docViewer.SizeChanged += WordUtil.Word_SizeChanged;
            pageTemplate.container.Children.Add(docViewer);
        }



        /*
         * 3.3插入轮播图  TurnPicture
         */
        public void insertTurnPictureToPage(DControl ctl)
        {
            List<TurnPictureImagesDto> list = turnPictureImagesBll.getByDControlId(ctl.id);
            //如果storageImageId=0,则显示替代的Params.TurnPictureNotExists[i]
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
            TurnPicture turnPicture = NewControlUtil.newTurnPicture(ctl, list, true);
            System.Windows.Style myStyle = (System.Windows.Style)pageTemplate.container.FindResource("DefaultTurnPictureStyle");
            turnPicture.Style = myStyle;

            turnPicture.PreviewMouseLeftButtonDown += control_MouseDown;
            turnPicture.PreviewMouseMove += control_MouseMove;
            turnPicture.PreviewMouseLeftButtonUp += control_MouseUp;
            //控件上右击显示菜单 
            turnPicture.MouseRightButtonUp += control_MouseRightButtonUp;
            pageTemplate.container.Children.Add(turnPicture);
        }

        /*
        * 3.3插入Gif
        */
        public void insertGifToPage(DControl ctl)
        {
            StorageGif storageGif = storageGifBll.get(ctl.storageId);
            string imgFullPath = FileUtil.notExistsShowDefault(storageGif?.url, Params.GifNotExists);
            imgFullPath = AppDomain.CurrentDomain.BaseDirectory + imgFullPath;

            Gif gifImage = NewControlUtil.newGif(ctl, storageGif, true);
            gifImage.Style = pageTemplate.container.TryFindResource("GifImageStyle") as System.Windows.Style;

            //控件拖动
            gifImage.PreviewMouseLeftButtonDown += control_MouseDown;
            gifImage.PreviewMouseMove += control_MouseMove;
            gifImage.PreviewMouseLeftButtonUp += control_MouseUp;

            //控件上右击显示菜单 
            gifImage.MouseRightButtonUp += control_MouseRightButtonUp;
            pageTemplate.container.Children.Add(gifImage);
        }

        /*
       * 3.3插入流动相册
       */
        public void insertMarqueToPage(DControl ctl)
        {
            List<TurnPictureImagesDto> list = turnPictureImagesBll.getByDControlId(ctl.id);
            //如果storageImageId=0,则显示替代的Params.TurnPictureNotExists[i]
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
            Marque marque = NewControlUtil.newMarque(ctl, list, true);
            System.Windows.Style myStyle = (System.Windows.Style)pageTemplate.container.FindResource("DefaultMarqueStyle");
            marque.Style = myStyle;

            marque.PreviewMouseLeftButtonDown += control_MouseDown;
            marque.PreviewMouseMove += control_MouseMove;
            marque.PreviewMouseLeftButtonUp += control_MouseUp;
            //控件上右击显示菜单 
            marque.MouseRightButtonUp += control_MouseRightButtonUp;
            pageTemplate.container.Children.Add(marque);
        }

        /*
      * 3.3插入层叠相册
      */
        public void insertMarqueLayerToPage(DControl ctl)
        {
            List<TurnPictureImagesDto> list = turnPictureImagesBll.getByDControlId(ctl.id);
            //如果storageImageId=0,则显示替代的Params.TurnPictureNotExists[i]
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
            MarqueLayer marqueLayer = NewControlUtil.newMarqueLayer(ctl, list, true);

            System.Windows.Style myStyle = (System.Windows.Style)pageTemplate.container.FindResource("DefaultMarqueLayerStyle");
            marqueLayer.Style = myStyle;

            marqueLayer.PreviewMouseLeftButtonDown += control_MouseDown;
            marqueLayer.PreviewMouseMove += control_MouseMove;
            marqueLayer.PreviewMouseLeftButtonUp += control_MouseUp;
            //控件上右击显示菜单 
            marqueLayer.MouseRightButtonUp += control_MouseRightButtonUp;
            pageTemplate.container.Children.Add(marqueLayer);
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

          
            Cfg pageCfg = PageWidthUtil.getPageCfg(pageTemplate.dPage, App.localStorage.cfg);
            CVideo cVideo = NewControlUtil.newCVideo(ctl, dto, pageCfg, pageTemplate.mqServer, true);
            System.Windows.Style myStyle = (System.Windows.Style)pageTemplate.container.FindResource("DefaultCVideoStyle");
            cVideo.Style = myStyle;
            cVideo.PreviewMouseLeftButtonDown += control_MouseDown;
            cVideo.PreviewMouseMove += control_MouseMove;
            cVideo.PreviewMouseLeftButtonUp += control_MouseUp;
            //控件上右击显示菜单 
            cVideo.MouseRightButtonUp += control_MouseRightButtonUp;
            pageTemplate.container.Children.Add(cVideo);
        }


        /*
         * 插入返回按钮
         */
        internal void insertBackButtonToPage(DControl ctl)
        {
            StorageImage storageImage = storageImageBll.get(ctl.storageId);

            Button imageButton = NewControlUtil.newBackButton(ctl, storageImage);
            imageButton.Style = pageTemplate.container.TryFindResource("ImageButtonStyle") as System.Windows.Style;
            //控件拖动
            imageButton.PreviewMouseLeftButtonDown += control_MouseDown;
            imageButton.PreviewMouseMove += control_MouseMove;
            imageButton.PreviewMouseLeftButtonUp += control_MouseUp;
            //控件上右击显示菜单 
            imageButton.MouseRightButtonUp += control_MouseRightButtonUp;
            pageTemplate.container.Children.Add(imageButton);
        }

        /*
        * 插入日期
        */
        internal void insertCCalendarToPage(DControl ctl)
        {
            CCalendar cCalendar = NewControlUtil.newCCalendar(ctl, pageTemplate.dPage, App.localStorage.cfg);
            cCalendar.Style = pageTemplate.container.TryFindResource("CCalendarStyle") as System.Windows.Style;

            //控件拖动
            cCalendar.PreviewMouseLeftButtonDown += control_MouseDown;
            cCalendar.PreviewMouseMove += control_MouseMove;
            cCalendar.PreviewMouseLeftButtonUp += control_MouseUp;

            //控件上右击显示菜单 
            cCalendar.MouseRightButtonUp += control_MouseRightButtonUp;

            pageTemplate.container.Children.Add(cCalendar);
        }
        /*
       * 插入日期
       */
        internal void insertCAudioToPage(DControl ctl)
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
            CAudio cAudio = NewControlUtil.newCAudio(ctl, pageTemplate.dPage, App.localStorage.cfg, audioUrl,audioCoverUrl, pageTemplate.mqServer);
            cAudio.Style = pageTemplate.container.TryFindResource("CAudioStyle") as System.Windows.Style;

            //控件拖动
            cAudio.PreviewMouseLeftButtonDown += control_MouseDown;
            cAudio.PreviewMouseMove += control_MouseMove;
            cAudio.PreviewMouseLeftButtonUp += control_MouseUp;

            //控件上右击显示菜单 
            cAudio.MouseRightButtonUp += control_MouseRightButtonUp;
            pageTemplate.container.Children.Add(cAudio);
        }


        internal void insertHomeButtonToPage(DControl ctl)
        {
            StorageImage storageImage = storageImageBll.get(ctl.storageId);

            Button imageButton = NewControlUtil.newHomeButton(ctl, storageImage);
            imageButton.Style = pageTemplate.container.TryFindResource("ImageButtonStyle") as System.Windows.Style;

            //控件拖动
            imageButton.PreviewMouseLeftButtonDown += control_MouseDown;
            imageButton.PreviewMouseMove += control_MouseMove;
            imageButton.PreviewMouseLeftButtonUp += control_MouseUp;
            //控件上右击显示菜单 
            imageButton.MouseRightButtonUp += control_MouseRightButtonUp;

            pageTemplate.container.Children.Add(imageButton);
        }



        /*
         * 插入小窗口到页面
         */
        internal void insertCFrameToPage(DControl ctl)
        {
            CFrame cFrame = new CFrame(ctl, App.localStorage.cfg, pageTemplate.mqServer);
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


            System.Windows.Style myStyle = (System.Windows.Style)pageTemplate.container.FindResource("DefaultCFrameStyle");
            cFrame.Style = myStyle;
            cFrame.Focusable = false;
            cFrame.Visibility = Visibility.Visible;
            //控件拖动
            cFrame.PreviewMouseLeftButtonDown += control_MouseDown;
            cFrame.PreviewMouseMove += control_MouseMove;
            cFrame.PreviewMouseLeftButtonUp += control_MouseUp;
            //控件上右击显示菜单 
            cFrame.MouseRightButtonUp += control_MouseRightButtonUp;


            pageTemplate.container.Children.Add(cFrame);

        }




    }
}
