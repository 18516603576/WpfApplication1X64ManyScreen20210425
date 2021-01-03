using Bll;
using Common;
using Common.Data;
using Common.util;
using Model;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WpfApplication1.FolderWin;

namespace WpfApplication1.ManageWin
{
    /// <summary>
    /// VideoItemPage.xaml 的交互逻辑
    /// </summary>
    public partial class VideoItemPage : Page
    {
        private readonly SolidColorBrush grayBtnBg = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#fafafa"));
        private readonly SolidColorBrush grayBtnFont = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#cccccc"));
        private readonly SolidColorBrush activeBtnBg = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#dddddd"));
        //当前页码
        private Int32 currPage = 0;
        private readonly Int32 pageSize = 18;
        private Int32 pageCount = 0;
        private readonly Int32 folderId = 0;
        private readonly StorageImageBll storageImageBll = new StorageImageBll();
        private readonly StorageVideoBll storageVideoBll = new StorageVideoBll();

        /*
         * 获取某个文件夹下视频
         */
        public VideoItemPage(Int32 folderId)
        {
            InitializeComponent();
            this.folderId = folderId;
            initStorageManager("videoItem");
            Unloaded += Page_UnLoaded;
        }

        private void Page_UnLoaded(object sender, RoutedEventArgs e)
        {
            foreach (UIElement element in storageListWrap.Children)
            {
                if (element is Canvas)
                {
                    Canvas canvas = (Canvas)element;
                    foreach (UIElement ele in canvas.Children)
                    {
                        if (ele is Image)
                        {
                            Image image = (Image)ele;
                            if (image.Name == "image")
                            {
                                image.Source = null;
                            }
                        }
                    }

                }
            }
            storageListWrap.Children.Clear();
        }

        /*
        * 1.初始化图片管理
        */
        private void initStorageManager(string currItemName)
        {
            Int32 count = storageVideoBll.getPageCount(pageSize, folderId);
            pageCount = count;
            pageCountLabel.Content = pageCount;

            NextPageClick(null, null);
        }

        /*
        * 上一页
        */
        private void PrevPageClick(object sender, RoutedEventArgs e)
        {

            if (prevPageBtn.Tag.ToString() == "0") return;

            currPage = currPage - 1;
            if (currPage < 1)
            {
                currPage = 1;
            }
            if (currPage == 1)
            {
                setBtnGray(prevPageBtn);
            }
            else
            {
                setBtnActive(prevPageBtn);
            }
            if (currPage < pageCount)
            {
                setBtnActive(nextPageBtn);
            }
            else
            {
                setBtnGray(nextPageBtn);
            }

            List<StorageVideo> list = new List<StorageVideo>();
            if (currPage <= pageCount)
            {
                list = storageVideoBll.getNextPage(currPage, pageSize, folderId);
            }
            storageListWrap.Children.Clear();
            foreach (StorageVideo storageVideo in list)
            {
                Canvas imageCanvas = initOneImage(storageVideo);
                storageListWrap.Children.Add(imageCanvas);
            }
            currPageLabel.Content = currPage;
            SelectAll.IsChecked = false;
        }
        /*
         * 下一页
         */
        private void NextPageClick(object sender, RoutedEventArgs e)
        {
            if (nextPageBtn.Tag.ToString() == "0") return;

            currPage = currPage + 1;
            if (currPage >= pageCount)
            {
                setBtnGray(nextPageBtn);
            }
            else
            {
                setBtnActive(nextPageBtn);
            }
            if (currPage > 1)
            {
                setBtnActive(prevPageBtn);
            }
            else
            {
                setBtnGray(prevPageBtn);
            }

            List<StorageVideo> list = new List<StorageVideo>();
            if (currPage <= pageCount)
            {
                list = storageVideoBll.getNextPage(currPage, pageSize, folderId);
            }

            storageListWrap.Children.Clear();
            foreach (StorageVideo storageVideo in list)
            {
                Canvas videoCanvas = initOneImage(storageVideo);
                storageListWrap.Children.Add(videoCanvas);
            }
            currPageLabel.Content = currPage;
            SelectAll.IsChecked = false;
        }


        /*
         * 初始化一个图片控件
         */
        private Canvas initOneImage(StorageVideo storageVideo)
        {
            VideoListTag tag = new VideoListTag();
            tag.isSelected = false;
            tag.storageVideo = storageVideo;
            Canvas videoCanvas = new Canvas();
            videoCanvas.Name = "videoCanvas";
            videoCanvas.Width = 125;
            videoCanvas.Height = 150;
            videoCanvas.Margin = new Thickness(10);
            videoCanvas.Tag = tag;

            //1.按钮 
            StorageImage storageImage = storageImageBll.get(storageVideo.storageImageId);
            string imgFullPath = FileUtil.notExistsShowDefault(storageImage?.url, Params.ImageNotExists);
            imgFullPath = AppDomain.CurrentDomain.BaseDirectory + imgFullPath;
            Image image = new Image();
            image.Name = "image";
            image.Width = 125;
            image.Height = 125;
            image.Source = FileUtil.readImage2(imgFullPath, 200);
            image.Stretch = Stretch.Uniform;


            //2.按钮行
            Canvas bg = new Canvas();
            bg.Name = "bg";
            bg.Background = Brushes.Black;
            bg.Width = 125;
            bg.Height = 25;
            bg.Opacity = 0.4;
            bg.SetValue(Canvas.BottomProperty, 25.0);
            bg.SetValue(Canvas.LeftProperty, 0.0);

            //时长 
            string mmss = VideoUtil.duration2mmss(storageVideo.duration);
            Label lLabel = new Label();
            lLabel.Width = 125;
            lLabel.Height = 25;
            lLabel.Content = mmss;
            lLabel.Foreground = Brushes.White;
            lLabel.SetValue(Canvas.LeftProperty, 0.0);
            lLabel.SetValue(Canvas.TopProperty, 0.0);
            bg.Children.Add(lLabel);

            //删除按钮
            Button rbtn = new Button();
            rbtn.Width = 16;
            rbtn.Height = 16;
            rbtn.BorderThickness = new Thickness(0);
            rbtn.Background = new ImageBrush
            {
                ImageSource = FileUtil.readImage(AppDomain.CurrentDomain.BaseDirectory + "/myfile/sysimg/ico-image-remove.png")
                ,
                Stretch = Stretch.UniformToFill
            };
            rbtn.SetValue(Canvas.RightProperty, -8.0);
            rbtn.SetValue(Canvas.TopProperty, -8.0);
            // bg.Children.Add(rbtn);
            // rbtn.Click += rbtnClick;


            //标题
            Label titleLabel = new Label();
            titleLabel.Width = 125;
            titleLabel.Height = 25;
            titleLabel.Content = storageVideo.origFilename;
            titleLabel.SetValue(Canvas.LeftProperty, 0.0);
            titleLabel.SetValue(Canvas.BottomProperty, 0.0);
            titleLabel.ToolTip = titleLabel.Content;


            //勾选 
            Button selectButton = new Button();
            selectButton.Name = "selectButton";
            selectButton.Tag = storageVideo.id;
            selectButton.Width = 25;
            selectButton.Height = 25;
            selectButton.BorderThickness = new Thickness(0);
            selectButton.SetValue(Canvas.LeftProperty, 7.0);
            selectButton.SetValue(Canvas.TopProperty, 7.0);

            selectButton.Background = new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri(@"Resources/ico_media_select.png", UriKind.Relative)),
                Stretch = Stretch.UniformToFill
            };
            selectButton.Visibility = Visibility.Hidden;




            videoCanvas.MouseEnter += imageCanvasMouseEnter;
            videoCanvas.MouseLeave += imageCanvasMouseLeave;
            videoCanvas.MouseLeftButtonDown += selectButtonClick;
            selectButton.Click += selectButtonClick;



            videoCanvas.Children.Add(image);
            videoCanvas.Children.Add(bg);
            videoCanvas.Children.Add(titleLabel);
            videoCanvas.Children.Add(selectButton);
            // videoCanvas.Children.Add(rbtn);

            return videoCanvas;
        }

        //鼠标移动到视频上
        private void imageCanvasMouseEnter(object sender, MouseEventArgs e)
        {
            Canvas videoCanvas = (Canvas)sender;
            VideoListTag tag = (VideoListTag)videoCanvas.Tag;
            if (videoCanvas != null && !tag.isSelected)
            {
                FrameworkElement selectButton = FrameworkElementUtil.getByName(videoCanvas, "selectButton");
                if (selectButton != null)
                {
                    selectButton.Visibility = Visibility.Visible;
                }
            }
        }

        //鼠标离开视频
        private void imageCanvasMouseLeave(object sender, MouseEventArgs e)
        {
            Canvas videoCanvas = (Canvas)sender;
            VideoListTag tag = (VideoListTag)videoCanvas.Tag;
            if (videoCanvas != null && !tag.isSelected)
            {
                FrameworkElement selectButton = FrameworkElementUtil.getByName(videoCanvas, "selectButton");
                if (selectButton != null)
                {
                    selectButton.Visibility = Visibility.Hidden;
                }
            }
        }


        /*
         * 点击 - 切换全选
         */
        private void ToggleSelectAll_Click(object sender, RoutedEventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            bool b = (bool)cb.IsChecked;
            if (b)
            {
                SelectAllFun();
            }
            else
            {
                UnSelectAllFun();
            }
        }

        //点击勾选视频
        private void selectButtonClick(object sender, RoutedEventArgs e)
        {
            Canvas videoCanvas = null;
            FrameworkElement fe = (FrameworkElement)sender;
            if (fe.Name == "videoCanvas")
            {
                videoCanvas = (Canvas)fe;
            }
            else if (fe.Name == "selectButton")
            {
                videoCanvas = (Canvas)VisualTreeHelper.GetParent(fe);
            }
            VideoListTag tag = (VideoListTag)videoCanvas.Tag;
            toggleSelectStatus(videoCanvas);
        }
        /*
         * 切换选中状态
         */
        private void toggleSelectStatus(Canvas video)
        {
            VideoListTag tag = (VideoListTag)video.Tag;
            if (tag.isSelected)
            {
                unSelectThisImage(video);
            }
            else
            {
                selectThisImage(video);
            }

        }


        //取消所有选中
        private void UnSelectAllFun()
        {
            foreach (Canvas video in storageListWrap.Children)
            {
                VideoListTag tag = (VideoListTag)video.Tag;
                if (tag.isSelected)
                {
                    unSelectThisImage(video);
                }
            }

        }

        //选中所有
        private void SelectAllFun()
        {
            foreach (Canvas video in storageListWrap.Children)
            {
                VideoListTag tag = (VideoListTag)video.Tag;
                if (!tag.isSelected)
                {
                    selectThisImage(video);
                }
            }

        }

        /*
         * 选中当前图片
         */
        private void selectThisImage(Canvas video)
        {
            VideoListTag tag = (VideoListTag)video.Tag;
            Button selectButton = (Button)FrameworkElementUtil.getByName(video, "selectButton");
            if (selectButton != null)
            {
                selectButton.Visibility = Visibility.Visible;
                selectButton.Background = new ImageBrush
                {
                    ImageSource = new BitmapImage(new Uri(@"Resources/ico_media_select_active.png", UriKind.Relative)),
                    Stretch = Stretch.UniformToFill
                };
            }
            tag.isSelected = true;
            video.Tag = tag;
        }

        /*
         * 取消选中当前图片
         */
        private void unSelectThisImage(Canvas video)
        {
            VideoListTag tag = (VideoListTag)video.Tag;
            Button selectButton = (Button)FrameworkElementUtil.getByName(video, "selectButton");
            if (selectButton != null)
            {
                selectButton.Visibility = Visibility.Hidden;
                selectButton.Background = new ImageBrush
                {
                    ImageSource = new BitmapImage(new Uri(@"Resources/ico_media_select.png", UriKind.Relative)),
                    Stretch = Stretch.UniformToFill
                };
            }
            tag.isSelected = false;
            video.Tag = tag;
        }

        /*
                * 点击 - 批量删除图片
                */
        private void Batch_Delete_Click(object sender, RoutedEventArgs e)
        {
            List<StorageVideo> list = new List<StorageVideo>();//创建了一个空列表
            List<Canvas> canvasList = new List<Canvas>();
            UIElementCollection children = storageListWrap.Children;

            //1.获取所有勾选图片,同时移除选中
            for (int i = 0; i < children.Count; i++)
            {
                Canvas canvas = (Canvas)children[i];
                VideoListTag tag = (VideoListTag)canvas.Tag;
                if (tag.isSelected)
                {
                    list.Add(tag.storageVideo);
                    canvasList.Add(canvas);
                }
            }

            DeleteVideoWindow win = new DeleteVideoWindow(storageListWrap, list, canvasList);
            win.ShowDialog();
        }

        /*
         * 跳转到某一页 
         */
        private void toWhichPageClick(object sender, RoutedEventArgs e)
        {
            string text = this.toWhichPage.Text;
            if (string.IsNullOrWhiteSpace(text)) return;
            if (!DataUtil.isInt(text)) return;
            int toWhichPage = Int32.Parse(text);

            currPage = toWhichPage;
            if (currPage < 1)
            {
                currPage = 1;
            }
            if (currPage == 1)
            {
                setBtnGray(prevPageBtn);
            }
            else
            {
                setBtnActive(prevPageBtn);
            }
            if (currPage < pageCount)
            {
                setBtnActive(nextPageBtn);
            }
            else
            {
                setBtnGray(nextPageBtn);
            }



            List<StorageVideo> list = new List<StorageVideo>();
            if (currPage <= pageCount)
            {
                list = storageVideoBll.getNextPage(currPage, pageSize, folderId);
            }

            storageListWrap.Children.Clear();
            foreach (StorageVideo storageVideo in list)
            {
                Canvas videoCanvas = initOneImage(storageVideo);
                storageListWrap.Children.Add(videoCanvas);
            }
            currPageLabel.Content = currPage;
            SelectAll.IsChecked = false;


        }

        private void setBtnGray(Button btn)
        {
            btn.Background = grayBtnBg;  //灰色 
            btn.Foreground = grayBtnFont;
            btn.BorderBrush = grayBtnFont;
            btn.Tag = "0";
        }

        private void setBtnActive(Button btn)
        {
            btn.Background = activeBtnBg;  //灰色 
            btn.Foreground = Brushes.Black;
            btn.BorderBrush = Brushes.Gray;
            btn.Tag = "1";
        }

        /*
         * 移动到
         */
        private void Batch_Move_To_Click(object sender, RoutedEventArgs e)
        {
            List<StorageVideo> list = new List<StorageVideo>();//创建了一个空列表
            List<Canvas> canvasList = new List<Canvas>();
            UIElementCollection children = storageListWrap.Children;

            //1.获取所有勾选图片,同时移除选中
            for (int i = 0; i < children.Count; i++)
            {
                Canvas canvas = (Canvas)children[i];
                VideoListTag tag = (VideoListTag)canvas.Tag;
                if (tag.isSelected)
                {
                    list.Add(tag.storageVideo);
                    canvasList.Add(canvas);
                }
            }
            StorageVideoMoveToFolderWindow win = new StorageVideoMoveToFolderWindow(storageListWrap, list, canvasList);
            win.ShowDialog();
        }
    }
}
