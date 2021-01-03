using Common.control;
using Common.Data;
using Common.MQ;
using Model;
using Model.dto;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Xps.Packaging;

namespace Common.util
{
    /*
     *  触屏精灵
     * 
     *  插入控件
     */
    public class NewControlUtil
    {
        /*
         * 1 插入图片
         */
        public static Button newImage(DControl ctl, StorageImage storageImage)
        {
            string imgFullPath = FileUtil.notExistsShowDefault(storageImage?.url, Params.ImageNotExists);
            DControlDto dto = DControlUtil.convert(ctl);
            dto.url = imgFullPath;


            Button imageButton = new Button();
            imageButton.Name = ctl.type+dto.id;
            imageButton.Content = ctl.content;
            //imageButton.Background = new ImageBrush
            //{
            //    ImageSource = FileUtil.readImage2(AppDomain.CurrentDomain.BaseDirectory + imgFullPath, ctl.width)
            //    ,
            //    Stretch = Stretch.Fill
            //}; 
            imageButton.Background = Brushes.Transparent;
            FileUtil.readImage2Button(imageButton, AppDomain.CurrentDomain.BaseDirectory + imgFullPath, ctl.width, Stretch.Fill);
            imageButton.BorderThickness = new Thickness(0);
            imageButton.Margin = new Thickness(ctl.left, ctl.top, 0, 0);
            imageButton.Width = ctl.width;
            imageButton.Height = ctl.height;
            imageButton.HorizontalAlignment = HorizontalAlignment.Left;
            imageButton.VerticalAlignment = VerticalAlignment.Top;
            imageButton.Opacity = ctl.opacity / 100.0;
            Panel.SetZIndex(imageButton, ctl.idx);
            imageButton.Tag = dto;
            imageButton.Focusable = false;


            TransformGroup group = new TransformGroup();
            RotateTransform rotateTransform = TransformGroupUtil.GetRotateTransform(group);
            rotateTransform.Angle = ctl.rotateAngle;
            imageButton.RenderTransform = group;
            imageButton.RenderTransformOrigin = new Point(0.5, 0.5);

            return imageButton;
        }

        /*
         * 2 插入word
         */
        public static DocumentViewer newWord(DControl ctl, StorageFile storageFile)
        {
            string wordFullPath = FileUtil.notExistsShowDefault(storageFile?.url, Params.WordNotExists);
            wordFullPath = AppDomain.CurrentDomain.BaseDirectory + wordFullPath;

            DocumentViewer docViewer = new DocumentViewer();
            docViewer.Name = "Word";
            docViewer.HorizontalAlignment = HorizontalAlignment.Left;
            docViewer.VerticalAlignment = VerticalAlignment.Top;
            docViewer.Margin = new Thickness(ctl.left, ctl.top, 0, 0);
            docViewer.Width = ctl.width;
            docViewer.Height = ctl.height;
            docViewer.Background = Brushes.Transparent;
            docViewer.BorderThickness = new Thickness(0);
            docViewer.ShowPageBorders = false;
            docViewer.Opacity = ctl.opacity / 100.0;
            Panel.SetZIndex(docViewer, ctl.idx);
            docViewer.Tag = ctl;
            TransformGroup group = new TransformGroup();
            docViewer.RenderTransform = group;
            docViewer.RenderTransformOrigin = new Point(0.5, 0.5);
            docViewer.Visibility = Visibility.Visible;
            docViewer.Document = loadWord(wordFullPath);
            docViewer.FitToWidth();

            return docViewer;
        }

        /*
        * 加载word 文件
        * 
        * @param DocumentViewer 显示容器
        * 
        * @param DControl ctl 控件信息
        */
        private static FixedDocumentSequence loadWord(string wordFullFile)
        {
            try
            {
                XpsDocument xpsDoc = new XpsDocument(wordFullFile, FileAccess.Read, CompressionOption.SuperFast);
                FixedDocumentSequence fixedDoc = xpsDoc.GetFixedDocumentSequence();
                return fixedDoc;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /*
         * 3 插入相册
         */
        public static TurnPicture newTurnPicture(DControl ctl, List<TurnPictureImagesDto> list, Boolean isDesign)
        {
            TurnPicture turnPicture = new TurnPicture(ctl, isDesign, list);
            turnPicture.HorizontalAlignment = HorizontalAlignment.Left;
            turnPicture.VerticalAlignment = VerticalAlignment.Top;
            turnPicture.Margin = new Thickness(ctl.left, ctl.top, 0, 0);
            turnPicture.Width = ctl.width;
            turnPicture.Height = ctl.height;

            turnPicture.Background = null;
            turnPicture.Opacity = ctl.opacity / 100.0;
            Panel.SetZIndex(turnPicture, ctl.idx);
            turnPicture.Tag = ctl;
            TransformGroup group = new TransformGroup();
            turnPicture.RenderTransform = group;
            turnPicture.RenderTransformOrigin = new Point(0.5, 0.5);
            return turnPicture;
        }

        /*
         * 4 流动相册
         */
        public static Marque newMarque(DControl ctl, List<TurnPictureImagesDto> list, Boolean isDesign)
        {
            Marque marque = new Marque(ctl, isDesign, list);
            marque.HorizontalAlignment = HorizontalAlignment.Left;
            marque.VerticalAlignment = VerticalAlignment.Top;
            marque.Margin = new Thickness(ctl.left, ctl.top, 0, 0);
            marque.Width = ctl.width;
            marque.Height = ctl.height;

            marque.Background = null;
            marque.Opacity = ctl.opacity / 100.0;
            Panel.SetZIndex(marque, ctl.idx);
            marque.Tag = ctl;
            TransformGroup group = new TransformGroup();
            marque.RenderTransform = group;
            marque.RenderTransformOrigin = new Point(0.5, 0.5);
            return marque;
        }

        /*
        * 5 层叠相册
        */
        public static MarqueLayer newMarqueLayer(DControl ctl, List<TurnPictureImagesDto> list, Boolean isDesign)
        {
            MarqueLayer marqueLayer = new MarqueLayer(ctl, isDesign, list);
            marqueLayer.HorizontalAlignment = HorizontalAlignment.Left;
            marqueLayer.VerticalAlignment = VerticalAlignment.Top;
            marqueLayer.Margin = new Thickness(ctl.left, ctl.top, 0, 0);
            marqueLayer.Width = ctl.width;
            marqueLayer.Height = ctl.height;

            marqueLayer.Background = null;
            marqueLayer.Opacity = ctl.opacity / 100.0;
            Panel.SetZIndex(marqueLayer, ctl.idx);
            marqueLayer.Focusable = false;
            marqueLayer.Tag = ctl;
            TransformGroup group = new TransformGroup();
            marqueLayer.RenderTransform = group;
            marqueLayer.RenderTransformOrigin = new Point(0.5, 0.5);
            return marqueLayer;
        }

        /*
        * 6 视频
        */
        public static CVideo newCVideo(DControl ctl, StorageVideoDto dto, Cfg pageCfg, MqServer mqServer, Boolean isDesign)
        {
            CVideo cVideo = new CVideo(ctl, isDesign, pageCfg, dto, mqServer);
            cVideo.HorizontalAlignment = HorizontalAlignment.Left;
            cVideo.VerticalAlignment = VerticalAlignment.Top;
            cVideo.Margin = new Thickness(ctl.left, ctl.top, 0, 0);
            cVideo.Width = ctl.width;
            cVideo.Height = ctl.height;
            cVideo.Background = null;
            cVideo.Opacity = ctl.opacity / 100.0;
            Panel.SetZIndex(cVideo, ctl.idx);
            cVideo.Tag = ctl;
            TransformGroup group = new TransformGroup();
            cVideo.RenderTransform = group;
            cVideo.RenderTransformOrigin = new Point(0.5, 0.5);
            return cVideo;
        }
        /*
        * 7 返回按钮
        */
        public static Button newBackButton(DControl ctl, StorageImage storageImage)
        {
            string imgFullPath = FileUtil.notExistsShowDefault(storageImage?.url, Params.BackButtonNotExists);

            Button backButton = new Button();
            backButton.Name = ctl.type;
            backButton.Content = ctl.content;
            backButton.Background = Brushes.Transparent;
            FileUtil.readImage2Button(backButton, AppDomain.CurrentDomain.BaseDirectory + imgFullPath, ctl.width, Stretch.Fill);

            backButton.Foreground = Brushes.Black;
            backButton.BorderThickness = new Thickness(0);
            backButton.Margin = new Thickness(ctl.left, ctl.top, 0, 0);
            backButton.Width = ctl.width;
            backButton.Height = ctl.height;
            backButton.HorizontalAlignment = HorizontalAlignment.Left;
            backButton.VerticalAlignment = VerticalAlignment.Top;
            backButton.HorizontalContentAlignment = HorizontalAlignment.Center;
            backButton.VerticalContentAlignment = VerticalAlignment.Center;
            backButton.Opacity = ctl.opacity / 100.0;
            Panel.SetZIndex(backButton, ctl.idx);
            backButton.Tag = ctl;
            TransformGroup group = new TransformGroup();
            backButton.RenderTransform = group;
            backButton.RenderTransformOrigin = new Point(0.5, 0.5);
            backButton.Focusable = false;

            return backButton;
        }

        /*
       * 8 首页按钮
       */
        public static Button newHomeButton(DControl ctl, StorageImage storageImage)
        {
            string imgFullPath = FileUtil.notExistsShowDefault(storageImage?.url, Params.HomeButtonNotExists);


            Button homeButton = new Button();
            homeButton.Name = ctl.type;
            homeButton.Content = ctl.content;
            homeButton.Background = Brushes.Transparent;
            FileUtil.readImage2Button(homeButton, AppDomain.CurrentDomain.BaseDirectory + imgFullPath, ctl.width, Stretch.Fill);

            homeButton.Foreground = Brushes.Black;
            homeButton.BorderThickness = new Thickness(0);
            homeButton.Margin = new Thickness(ctl.left, ctl.top, 0, 0);
            homeButton.Width = ctl.width;
            homeButton.Height = ctl.height;
            homeButton.HorizontalAlignment = HorizontalAlignment.Left;
            homeButton.VerticalAlignment = VerticalAlignment.Top;
            homeButton.HorizontalContentAlignment = HorizontalAlignment.Center;
            homeButton.VerticalContentAlignment = VerticalAlignment.Center;
            homeButton.Opacity = ctl.opacity / 100.0;
            Panel.SetZIndex(homeButton, ctl.idx);
            homeButton.Tag = ctl;
            TransformGroup group = new TransformGroup();
            homeButton.RenderTransform = group;
            homeButton.RenderTransformOrigin = new Point(0.5, 0.5);
            homeButton.Focusable = false;
            return homeButton;
        }
        ///*
        // * 9 小窗口
        // */
        //public static CFrame newCFrame(DControl ctl, Frame mainFrame, Cfg appCfg)
        //{
        //    CFrame cFrame = new CFrame(mainFrame, ctl, appCfg);
        //    cFrame.BorderThickness = new Thickness(0);
        //    cFrame.Margin = new Thickness(ctl.left, ctl.top, 0, 0);
        //    cFrame.Width = ctl.width;
        //    cFrame.Height = ctl.height;
        //    cFrame.HorizontalAlignment = HorizontalAlignment.Left;
        //    cFrame.VerticalAlignment = VerticalAlignment.Top;
        //    cFrame.Opacity = ctl.opacity / 100.0;
        //    Panel.SetZIndex(cFrame, ctl.idx);
        //    cFrame.Tag = ctl;
        //    TransformGroup group = new TransformGroup();
        //    cFrame.RenderTransform = group;
        //    cFrame.RenderTransformOrigin = new Point(0.5, 0.5); 
        //    cFrame.Focusable = false;
        //    return cFrame; 
        //}

        /*
        * 10 文本
        */
        public static TextBlock newTextBlock(DControl ctl)
        {
            TextBlock textBlock = new TextBlock();
            textBlock.HorizontalAlignment = HorizontalAlignment.Left;
            textBlock.VerticalAlignment = VerticalAlignment.Top;
            textBlock.Margin = new Thickness(ctl.left, ctl.top, 0, 0);
            textBlock.Width = ctl.width;
            textBlock.Height = ctl.height;
            textBlock.Background = Brushes.Transparent;

            //textBox.AcceptsReturn = true;
            textBlock.TextWrapping = TextWrapping.WrapWithOverflow;
            textBlock.Text = ctl.content;
            textBlock.FontSize = FontUtil.getFontSize(ctl.fontSize);
            textBlock.Foreground = FontUtil.getFontColor(ctl.fontColor);
            textBlock.FontWeight = FontUtil.getFontWeight(ctl.fontWeight);
            textBlock.TextAlignment = FontUtil.getFontTextAlignment(ctl.fontTextAlignment);
            textBlock.FontFamily = FontUtil.getFontFamily(ctl.fontFamily);
            TextBlock.SetLineHeight(textBlock, FontUtil.getFontLineHeight(ctl.fontLineHeight));


            textBlock.Opacity = ctl.opacity / 100.0;
            Panel.SetZIndex(textBlock, ctl.idx);
            textBlock.Focusable = false;
            textBlock.Tag = ctl;
            TransformGroup group = new TransformGroup();
            textBlock.RenderTransform = group;
            textBlock.RenderTransformOrigin = new Point(0.5, 0.5);
            textBlock.Visibility = Visibility.Visible;
            return textBlock;
        }

        /*
       * 10.2 设计版文本
       */
        public static TextBox newTextBox(DControl ctl)
        {
            TextBox textBox = new TextBox();
            textBox.HorizontalAlignment = HorizontalAlignment.Left;
            textBox.VerticalAlignment = VerticalAlignment.Top;
            textBox.Margin = new Thickness(ctl.left, ctl.top, 0, 0);
            textBox.Width = ctl.width;
            textBox.Height = ctl.height;
            textBox.Background = Brushes.Transparent;
            textBox.BorderThickness = new Thickness(0);
            textBox.AcceptsReturn = true;
            textBox.TextWrapping = TextWrapping.WrapWithOverflow;
            textBox.Text = ctl.content;

            textBox.FontSize = FontUtil.getFontSize(ctl.fontSize);
            textBox.FontFamily = FontUtil.getFontFamily(ctl.fontFamily);
            textBox.Foreground = FontUtil.getFontColor(ctl.fontColor);
            textBox.FontWeight = FontUtil.getFontWeight(ctl.fontWeight);
            textBox.TextAlignment = FontUtil.getFontTextAlignment(ctl.fontTextAlignment);
            TextBlock.SetLineHeight(textBox, FontUtil.getFontLineHeight(ctl.fontLineHeight));


            textBox.Opacity = ctl.opacity / 100.0;
            Panel.SetZIndex(textBox, ctl.idx);
            textBox.Tag = ctl;
            TransformGroup group = new TransformGroup();
            textBox.RenderTransform = group;
            textBox.RenderTransformOrigin = new Point(0.5, 0.5);
            textBox.Visibility = Visibility.Visible;
            return textBox;
        }

        /*
        * 11 日期
        */
        public static CCalendar newCCalendar(DControl ctl, DPage dPage, Cfg appCfg)
        {
            CCalendar cCalendar = new CCalendar(ctl, dPage, appCfg);
            cCalendar.HorizontalAlignment = HorizontalAlignment.Left;
            cCalendar.VerticalAlignment = VerticalAlignment.Top;
            cCalendar.Margin = new Thickness(ctl.left, ctl.top, 0, 0);
            cCalendar.Width = ctl.width;
            cCalendar.Height = ctl.height;
            cCalendar.Background = Brushes.Transparent;

            cCalendar.Opacity = ctl.opacity / 100.0;
            Panel.SetZIndex(cCalendar, ctl.idx);
            cCalendar.Focusable = false;
            cCalendar.Tag = ctl;
            TransformGroup group = new TransformGroup();
            cCalendar.RenderTransform = group;
            cCalendar.RenderTransformOrigin = new Point(0.5, 0.5);
            cCalendar.Visibility = Visibility.Visible;
            return cCalendar;
        }

        /*
       * 11 音频
       */
        public static CAudio newCAudio(DControl ctl, DPage dPage, Cfg appCfg, string audioUrl,string audioCoverUrl ,MqServer mqServer)
        {
            CAudio cAudio = new CAudio(ctl, dPage, appCfg, audioUrl,audioCoverUrl, mqServer);
            cAudio.HorizontalAlignment = HorizontalAlignment.Left;
            cAudio.VerticalAlignment = VerticalAlignment.Top;
            cAudio.Margin = new Thickness(ctl.left, ctl.top, 0, 0);
            cAudio.Width = ctl.width;
            cAudio.Height = ctl.height;
            cAudio.Background = Brushes.Transparent;

            cAudio.Opacity = ctl.opacity / 100.0;
            Panel.SetZIndex(cAudio, ctl.idx);
            cAudio.Focusable = false;
            cAudio.Tag = ctl;
            TransformGroup group = new TransformGroup();
            cAudio.RenderTransform = group;
            cAudio.RenderTransformOrigin = new Point(0.5, 0.5);
            cAudio.Visibility = Visibility.Visible;
            return cAudio;
        }

        /*
         * 12 Gif
        */
        public static Gif newGif(DControl ctl, StorageGif storageGif, Boolean isDesign)
        {
            string imgPath = FileUtil.notExistsShowDefault(storageGif?.url, Params.GifNotExists);
            string imgFullPath = AppDomain.CurrentDomain.BaseDirectory + imgPath;

            DControlDto dto = DControlUtil.convert(ctl);
            dto.url = imgFullPath; 
            Gif gif = new Gif(imgPath, isDesign);
            gif.HorizontalAlignment = HorizontalAlignment.Left;
            gif.VerticalAlignment = VerticalAlignment.Top;
            gif.Margin = new Thickness(ctl.left, ctl.top, 0, 0);
            gif.Width = ctl.width;
            gif.Height = ctl.height;
            gif.Background = Brushes.Transparent;

            gif.Opacity = ctl.opacity / 100.0;
            Panel.SetZIndex(gif, ctl.idx);
            gif.Focusable = false;
            gif.Tag = dto;
            TransformGroup group = new TransformGroup();
            gif.RenderTransform = group;
            gif.RenderTransformOrigin = new Point(0.5, 0.5);
            RotateTransform rotateTransform = TransformGroupUtil.GetRotateTransform(group);
            rotateTransform.Angle = ctl.rotateAngle;
            gif.Visibility = Visibility.Visible;
            return gif;
        }


    }
}
