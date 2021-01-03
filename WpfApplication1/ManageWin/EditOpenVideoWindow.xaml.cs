using Bll;
using Common;
using Common.Data;
using Common.util;
using Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Vlc.DotNet.Core;
using Vlc.DotNet.Wpf;
using WpfApplication1.manage;

namespace WpfApplication1.ManageWin
{
    /// <summary>
    /// EditOpenVideoWindow.xaml 的交互逻辑
    /// </summary>
    public partial class EditOpenVideoWindow : Window
    {
        private readonly StorageVideoBll storageVideoBll = new StorageVideoBll();
        private readonly StorageImageBll storageImageBll = new StorageImageBll();

        private readonly DControlBll dControlBll = new DControlBll();
        private readonly StorageVideoFolderBll storageVideoFolderBll = new StorageVideoFolderBll();
        //当前控件
        private readonly FrameworkElement currElement;
        private readonly DControl currDControl;
        //当前页码
        private Int32 currPage = 0;
        private readonly Int32 pageSize = 10;
        private Int32 folderId = 1;
        //是否最后一页
        private Boolean isLastPage = false;
        private Boolean isLoading = false;



        public EditOpenVideoWindow(Editing editing, FrameworkElement currElement)
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.currElement = currElement;
            currDControl = (DControl)currElement.Tag;
            init(currDControl);
            StorageVideo_InitFolder();
        }

        private void init(DControl ctl)
        {
            if (!isLoading && !isLastPage)
            {
                isLoading = true;
                videoListWrap.Children.Clear();
                initVideoList();
                isLoading = false;
            }
            videoListScrollViewer.ScrollChanged += videoListScrollViewerChanged;

            //初始化选中按钮
            if (ctl.linkToVideoId > 0)
            {
                StorageVideo storageVideo = storageVideoBll.get(ctl.linkToVideoId);
                showSelectedVideo(storageVideo);
            }
        }
        //显示选中的控件
        private void showSelectedVideo(StorageVideo storageVideo)
        {

            if (storageVideo == null) return;
            selectedVideoWrapPanel.Children.Clear();
            Canvas selectedVideoCanvas = initSelectedVideo(storageVideo);
            selectedVideoWrapPanel.Children.Add(selectedVideoCanvas);

        }
        /*
         * 添加已选择的视频
         */
        private Canvas initSelectedVideo(StorageVideo video)
        {
            VideoListTag tag = new VideoListTag();
            tag.isSelected = false;
            tag.storageVideo = video;
            Canvas selectedVideoCanvas = new Canvas();
            selectedVideoCanvas.Name = "selectedVideoCanvas";
            selectedVideoCanvas.Width = 100;
            selectedVideoCanvas.Height = 100;
            selectedVideoCanvas.Margin = new Thickness(10);
            selectedVideoCanvas.Tag = tag;

            //1.按钮
            StorageImage storageImage = storageImageBll.get(video.storageImageId);
            string imgFullPath = FileUtil.notExistsShowDefault(storageImage?.url, Params.ImageNotExists);
            imgFullPath = AppDomain.CurrentDomain.BaseDirectory + imgFullPath; 
            Image videoImage = new Image();
            videoImage.Name = "videoImage1";
            videoImage.Width = 100;
            videoImage.Height = 75;
            videoImage.Source = FileUtil.readImage2(imgFullPath, 200);
            videoImage.Stretch = Stretch.UniformToFill;

            //2.按钮行
            Canvas bg = new Canvas();
            bg.Name = "bg";
            bg.Background = Brushes.Black;
            bg.Width = 100;
            bg.Height = 24;
            bg.Opacity = 0.6;
            bg.SetValue(Canvas.BottomProperty, 25.0);
            bg.SetValue(Canvas.LeftProperty, 0.0);

            //时长
            string mmss = VideoUtil.duration2mmss(video.duration);
            Label lLabel = new Label();
            lLabel.Width = 50;
            lLabel.Height = 24;
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
                ImageSource = FileUtil.readImage(AppDomain.CurrentDomain.BaseDirectory + App.localStorage.icoRemove)
                ,
                Stretch = Stretch.UniformToFill
            };
            rbtn.SetValue(Canvas.RightProperty, -8.0);
            rbtn.SetValue(Canvas.TopProperty, -8.0);
            rbtn.Click += rbtnRemoveSelectedVideoClick;


            //标题
            string videoFullPath = FileUtil.notExistsShowDefault(video?.url, Params.VideoNotExists); 
            string fullFolder = FileUtil.getDirectory(AppDomain.CurrentDomain.BaseDirectory + videoFullPath);

            Label titleLabel = new Label();
            titleLabel.Width = 100;
            titleLabel.Height = 25;
            titleLabel.Content = video.origFilename;
            titleLabel.Tag = fullFolder;
            titleLabel.SetValue(Canvas.LeftProperty, 0.0);
            titleLabel.SetValue(Canvas.BottomProperty, 0.0);
            titleLabel.ToolTip = video.origFilename;
            titleLabel.PreviewMouseLeftButtonUp += titleLabel_PreviewMouseLeftButtonUp;

            selectedVideoCanvas.Children.Add(videoImage);
            selectedVideoCanvas.Children.Add(bg);
            selectedVideoCanvas.Children.Add(titleLabel);
            selectedVideoCanvas.Children.Add(rbtn);
            return selectedVideoCanvas;
        }

        private void titleLabel_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Label label = (Label)sender;
            string fullFolder = label.Tag?.ToString();

            FileUtil.openFile(fullFolder);
        }

        //移除当前选中的视频
        private void rbtnRemoveSelectedVideoClick(object sender, RoutedEventArgs e)
        {
            Button rbtn = (Button)sender;
            selectedVideoWrapPanel.Children.Clear();
            currDControl.linkToVideoId = 0;

            unselectedAllVideo();
        }

        private void videoListScrollViewerChanged(object sender, ScrollChangedEventArgs e)
        {
            ScrollViewer sc = (ScrollViewer)sender;
            double scrolledHeight = (double)sc.VerticalOffset;
            double height = videoListScrollViewer.ScrollableHeight;
            //Open_Dialogue.Content = height + "__" + scrolledHeight;

            if (scrolledHeight >= height)
            {
                if (!isLoading && !isLastPage)
                {
                    isLoading = true;
                    initVideoList();  //加载下一页
                    isLoading = false;
                }


            }

        }
        /*
         * 加载所有视频
         */
        private void initVideoList()
        {
            currPage = currPage + 1;
            // Open_Dialogue.Content = this.currPage.ToString();
            List<StorageVideo> list = storageVideoBll.getNextPage(currPage, pageSize, folderId);
            if (list == null || list.Count == 0)
            {
                isLastPage = true;
            }
            foreach (StorageVideo video in list)
            {
                Canvas videoCanvas = initOneVideo(video);
                videoListWrap.Children.Add(videoCanvas);
            }
        }

        /*
       * 初始化一个图片控件
       */
        private Canvas initOneVideo(StorageVideo video)
        {

            VideoListTag tag = new VideoListTag();
            tag.isSelected = false;
            tag.storageVideo = video;
            Canvas videoCanvas = new Canvas();
            videoCanvas.Name = "videoCanvas";
            videoCanvas.Width = 100;
            videoCanvas.Height = 100;
            videoCanvas.Margin = new Thickness(10);
            videoCanvas.Tag = tag;

            //1.按钮 
            StorageImage storageImage = storageImageBll.get(video.storageImageId);
            string imgFullPath = FileUtil.notExistsShowDefault(storageImage?.url, Params.ImageNotExists);
            imgFullPath = AppDomain.CurrentDomain.BaseDirectory + imgFullPath;
            Image videoImage = new Image();
            videoImage.Name = "videoImage1";
            videoImage.Width = 100;
            videoImage.Height = 75;
            videoImage.Source = FileUtil.readImage2(imgFullPath, 200);
            videoImage.Stretch = Stretch.UniformToFill;

            //2.按钮行
            Canvas bg = new Canvas();
            bg.Name = "bg";
            bg.Background = Brushes.Black;
            bg.Width = 100;
            bg.Height = 24;
            bg.Opacity = 0.6;
            bg.SetValue(Canvas.BottomProperty, 25.0);
            bg.SetValue(Canvas.LeftProperty, 0.0);

            //时长
            string mmss = VideoUtil.duration2mmss(video.duration);
            Label lLabel = new Label();
            lLabel.Width = 50;
            lLabel.Height = 24;
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
            titleLabel.Width = 100;
            titleLabel.Height = 25;
            titleLabel.Content = video.origFilename;
            titleLabel.SetValue(Canvas.LeftProperty, 0.0);
            titleLabel.SetValue(Canvas.BottomProperty, 0.0);
            titleLabel.ToolTip = video.origFilename;

            //勾选 
            Button selectButton = new Button();
            selectButton.Name = "selectButton";
            selectButton.Tag = video.id;
            selectButton.Width = 24;
            selectButton.Height = 24;
            selectButton.BorderThickness = new Thickness(0);
            selectButton.SetValue(Canvas.LeftProperty, 7.0);
            selectButton.SetValue(Canvas.TopProperty, 7.0);

            selectButton.Background = new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri(@"Resources/ico_media_select.png", UriKind.Relative)),
                Stretch = Stretch.UniformToFill
            };
            selectButton.Visibility = Visibility.Hidden;




            videoCanvas.MouseEnter += videoCanvasMouseEnter;
            videoCanvas.MouseLeave += videoCanvasMouseLeave;
            videoCanvas.MouseLeftButtonDown += selectButtonClick;
            selectButton.Click += selectButtonClick;



            videoCanvas.Children.Add(videoImage);
            videoCanvas.Children.Add(bg);
            videoCanvas.Children.Add(titleLabel);
            videoCanvas.Children.Add(selectButton);
            // videoCanvas.Children.Add(rbtn);

            return videoCanvas;
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

            selectButton(videoCanvas);
        }

        //勾选视频
        private void selectButton(Canvas videoCanvas)
        {
            //移除所有
            unselectedAllVideo();

            //选中当前  
            selectCurrVideo(videoCanvas);

            //更新当前控件数据
            VideoListTag tag = (VideoListTag)videoCanvas.Tag;
            // this.currDControl.url = tag.storageVideo.url;
            //  this.currDControl.imgs = tag.storageVideo.img;

            //显示选中的控件
            showSelectedVideo(tag.storageVideo);
            currDControl.linkToVideoId = tag.storageVideo.id;
        }



        //取消所有视频选中状态
        private void unselectedAllVideo()
        {
            foreach (Canvas video in videoListWrap.Children)
            {
                VideoListTag tag = (VideoListTag)video.Tag;
                if (tag.isSelected)
                {
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
            }

        }
        //选中当前视频
        private void selectCurrVideo(Canvas videoCanvas)
        {
            Button selectButton = (Button)FrameworkElementUtil.getByName(videoCanvas, "selectButton");
            if (selectButton != null)
            {
                selectButton.Visibility = Visibility.Visible;
                selectButton.Background = new ImageBrush
                {
                    ImageSource = new BitmapImage(new Uri(@"Resources/ico_media_select_active.png", UriKind.Relative)),
                    Stretch = Stretch.UniformToFill
                };
            }
            VideoListTag tag = (VideoListTag)videoCanvas.Tag;
            tag.isSelected = true;
            videoCanvas.Tag = tag;

        }


        //鼠标移动到视频上
        private void videoCanvasMouseEnter(object sender, MouseEventArgs e)
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
        private void videoCanvasMouseLeave(object sender, MouseEventArgs e)
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

        private VlcControl vlcControl = null;
        private string destFilePath = "";
        private string sourceFilePath = "";
        private void Open_Dialogue_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;

            System.Windows.Forms.OpenFileDialog ofld = new System.Windows.Forms.OpenFileDialog();
            ofld.Filter = "视频|*.mp4";
            ofld.Multiselect = false;
            if (ofld.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            { 
                this.sourceFilePath = ofld.FileName;
                if (!string.IsNullOrWhiteSpace(sourceFilePath))
                {
                    this.destFilePath = uploadVideo(btn, sourceFilePath);
                    this.captureImageAndTime(destFilePath);
                }
            }
        }

        /*
       * 截图并且获取时长
       */
        public void captureImageAndTime(string sourceFP)
        {
            string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            DirectoryInfo vlcLibDirectory = new DirectoryInfo(System.IO.Path.Combine(currentDirectory, "libvlc", IntPtr.Size == 4 ? "win-x86" : "win-x64"));

            string videoFullPath = AppDomain.CurrentDomain.BaseDirectory + sourceFP;
            if (!FileUtil.imageIsExists(sourceFP))
            {
                return;
            }


            this.vlcControl?.Dispose();
            vlcControl = new VlcControl();
            vlcControl.SourceProvider.CreatePlayer(vlcLibDirectory);
            vlcControl.SourceProvider.MediaPlayer.SetMedia(new Uri(videoFullPath));
            vlcControl.SourceProvider.MediaPlayer.Position = 0;
            vlcControl.SourceProvider.MediaPlayer.Play();
            vlcControl.SourceProvider.MediaPlayer.TimeChanged += MediaPlayer_TimeChanged;
        }


        private void MediaPlayer_TimeChanged(object sender, VlcMediaPlayerTimeChangedEventArgs e)
        {
            vlcControl.SourceProvider.MediaPlayer.TimeChanged -= MediaPlayer_TimeChanged;
            string shotJpg = FileUtil.replaceExt(destFilePath, "jpg");
            FileInfo shotFile = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + shotJpg);

            ThreadPool.QueueUserWorkItem(_ =>
            {
                vlcControl.SourceProvider.MediaPlayer.TakeSnapshot(shotFile, 0, 0);
                long length = vlcControl.SourceProvider.MediaPlayer.Length;
                int duration = Convert.ToInt32(length / 1000.0);

                int jpgWidth = 0;
                int jpgHeight = 0;
                try
                {
                    System.Drawing.Image sourceImage = System.Drawing.Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + shotJpg);
                    jpgWidth = sourceImage.Width;
                    jpgHeight = sourceImage.Height;
                }
                catch (Exception)
                {

                }
                StorageVideo storageVideo = storageVideoBll.insert(this.sourceFilePath, destFilePath, duration, shotJpg, jpgWidth, jpgHeight, folderId);


                //插入一个视频到列表  
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    Canvas newVideo = initOneVideo(storageVideo);
                    videoListWrap.Children.Insert(0, newVideo);
                    selectButton(newVideo);
                }));

                vlcControl?.Dispose();
            });
        }

        /*
          * 上传视频并截图，复制的软件目录下
          */
        private String uploadVideo(Button btn, string sourceFP)
        {
            string ext = FileUtil.getExt(sourceFP);
            //1.复制到软件目录
            String filename = FileUtil.getFilename(sourceFP);
            string yyyyMMdd = DateTime.Now.ToString("yyyyMMdd");
            string guid = System.Guid.NewGuid().ToString("N");
            String destFP = @"/myfile/upload/Video/" + yyyyMMdd + @"/" + guid + @"/" + filename;

            String fullDestFP = AppDomain.CurrentDomain.BaseDirectory + destFP;
            FileUtil.createDirectoryIfNotExits(fullDestFP);
            File.Copy(sourceFP, fullDestFP);
            return destFP;
        }
        /*
         * 保存视频
         */
        private void Submit_Button_Click(object sender, RoutedEventArgs e)
        {
            DControl currDControl = (DControl)currElement.Tag;
            //更新到数据库
            DControl dControl = dControlBll.get(currDControl.id);
            dControl.linkToVideoId = this.currDControl.linkToVideoId;
            dControlBll.update(dControl);
            currElement.Tag = dControl;

            //更新页面控件信息
            Close();

        }

        /*
         * 素材中心 - 视频
         */
        private void videoManagerClick(object sender, RoutedEventArgs e)
        {
            StorageManagerWindow win = new StorageManagerWindow("videoItem");
            win.ShowDialog();
        }
        /*
         * 文件夹树 - 触摸滚动反馈
         */
        private void folderTreeColumn_ManipulationBoundaryFeedback(object sender, ManipulationBoundaryFeedbackEventArgs e)
        {
            e.Handled = true;
        }

        /*
        * 初始化文件文件夹树
        */
        private void StorageVideo_InitFolder()
        {

            //1.初始化页面树
            StorageVideoFolder storageVideoFolder = storageVideoFolderBll.get(1);
            if (storageVideoFolder == null) return;

            TreeViewItem firstItem = new TreeViewItem();
            firstItem.IsExpanded = true;
            firstItem.Header = storageVideoFolder.name;
            firstItem.Tag = storageVideoFolder.id;
            firstItem.Padding = new Thickness(5);

            StorageVideo_GetTreeViewItemChildren(firstItem);

            //添加单击、右键事件
            folderTreeColumn.MouseLeftButtonUp += (object sender, MouseButtonEventArgs e) => StorageVideo_TreeView_MouseLeftButtonUp(sender, e);

            folderTreeColumn.Items.Clear();
            folderTreeColumn.Items.Add(firstItem);

        }

        /*
      * 获取当前页面的子页面 
      * 
      * @param TreeViewItem currItem 当前页面
      */
        private TreeViewItem StorageVideo_GetTreeViewItemChildren(TreeViewItem currItem)
        {
            Int32 id = (Int32)currItem.Tag;

            List<StorageVideoFolder> list = storageVideoFolderBll.getByParentId(id);
            if (list == null) return null;
            foreach (StorageVideoFolder one in list)
            {
                TreeViewItem child = new TreeViewItem();
                child.Header = one.name;
                child.Tag = one.id;
                child.Padding = new Thickness(5);
                // child.Foreground = Brushes.White;

                currItem.Items.Add(child);
                StorageVideo_GetTreeViewItemChildren(child);
            }
            return currItem;
        }

        /*
       * 单击页面树，显示当前页面内容
       */
        private void StorageVideo_TreeView_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (e.Source.GetType().Name != "TreeViewItem")
            {
                e.Handled = true;
                return;
            }

            try
            {
                TreeViewItem treeViewItem = (TreeViewItem)e.Source;
                Object tag = treeViewItem.Tag;
                if (tag != null)
                {

                    isLoading = false;
                    isLastPage = false;
                    currPage = 0;

                    Int32 folderId = Int32.Parse(tag.ToString());
                    this.folderId = folderId;

                    init(currDControl);
                    //VideoItemPage videoItemPage = new VideoItemPage(folderId);
                    //mainFrame.Navigate(videoItemPage);
                    //FrameUtil.RemoveBackEntry(mainFrame);

                }
                else
                {
                    MessageBox.Show("没有tag:" + treeViewItem.Header.ToString());
                }

            }
            catch (ArgumentNullException)
            {
                MessageBox.Show("页面不能为空");
            }
            catch (ArgumentException)
            {
                MessageBox.Show("页面地址异常");
            }
            catch (UriFormatException)
            {
                MessageBox.Show("页面地址格式化异常");
            }
            catch (Exception e4)
            {
                MessageBox.Show("找不到此页面" + e4.Message.ToString() + e4.StackTrace);
            }

        }
    }
}
