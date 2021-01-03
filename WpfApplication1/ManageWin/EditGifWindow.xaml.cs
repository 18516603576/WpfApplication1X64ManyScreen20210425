using Bll;
using Common;
using Common.control;
using Common.Data;
using Common.util;
using Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using WpfApplication1.manage;

namespace WpfApplication1.ManageWin
{
    /// <summary>
    /// EditImageWindow.xaml 的交互逻辑
    /// </summary>
    public partial class EditGifWindow : Window
    {
        private readonly StorageGifBll storageGifBll = new StorageGifBll();
        private readonly DControlBll dControlBll = new DControlBll();
        private readonly StorageGifFolderBll storageGifFolderBll = new StorageGifFolderBll();
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



        public EditGifWindow(Editing editing, FrameworkElement currElement)
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.currElement = currElement;
            currDControl = (DControl)currElement.Tag;
            init(currDControl);
            StorageGif_InitFolder();
        }

        private void init(DControl ctl)
        {
            if (!isLoading && !isLastPage)
            {
                isLoading = true;
                imageListWrap.Children.Clear();
                initImageList();
                isLoading = false;
            }

            imageListScrollViewer.ScrollChanged += imageListScrollViewerChanged;
        }

        private void imageListScrollViewerChanged(object sender, ScrollChangedEventArgs e)
        {
            ScrollViewer sc = (ScrollViewer)sender;
            double scrolledHeight = (double)sc.VerticalOffset;
            double height = imageListScrollViewer.ScrollableHeight;

            if (scrolledHeight >= height)
            {
                if (!isLoading && !isLastPage)
                {
                    isLoading = true;
                    initImageList();  //加载下一页
                    isLoading = false;
                }
            }

        }

        /*
         * 加载所有视频
         */
        private void initImageList()
        {
            currPage = currPage + 1;
            List<StorageGif> list = storageGifBll.getNextPage(currPage, pageSize, folderId);
            if (list == null || list.Count == 0)
            {
                isLastPage = true;
            }
            foreach (StorageGif storageGif in list)
            {
                imageListWrap.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action<StorageGif>(addItem), storageGif);
            }
        }

        private void addItem(StorageGif storageGif)
        {
            Canvas imageCanvas = initOneImage(storageGif);
            imageListWrap.Children.Add(imageCanvas);
        }

        //public void DoEvents()
        //{
        //    DispatcherFrame frame = new DispatcherFrame();
        //    Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,
        //        new DispatcherOperationCallback(ExitFrames), frame);
        //    Dispatcher.PushFrame(frame);
        //}

        //public object ExitFrames(object f)
        //{
        //    ((DispatcherFrame)f).Continue = false;

        //    return null;
        //}

        /*
       * 初始化一个图片控件
       */
        private Canvas initOneImage(StorageGif storageGif)
        {
            GifListTag tag = new GifListTag();
            tag.isSelected = false;
            tag.storageGif = storageGif;
            Canvas imageCanvas = new Canvas();
            imageCanvas.Name = "imageCanvas";
            imageCanvas.Width = 100;
            imageCanvas.Height = 100;
            imageCanvas.Margin = new Thickness(10);
            imageCanvas.Tag = tag;

            //1.按钮 
            string imgFullPath = FileUtil.notExistsShowDefault(storageGif.url, Params.ImageNotExists);
            imgFullPath = AppDomain.CurrentDomain.BaseDirectory + imgFullPath;

            Image image = new Image();
            image.Name = "image";
            image.Width = 100;
            image.Height = 75;
            image.Source = FileUtil.readImage2(imgFullPath, 200); ;
            image.Stretch = Stretch.Uniform;


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
            Label lLabel = new Label();
            lLabel.Width = 100;
            lLabel.Height = 24;
            lLabel.Content = storageGif.actualWidth + "×" + storageGif.actualHeight;
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
            titleLabel.Content = storageGif.origFilename;
            titleLabel.SetValue(Canvas.LeftProperty, 0.0);
            titleLabel.SetValue(Canvas.BottomProperty, 0.0);
            titleLabel.ToolTip = titleLabel.Content;


            //勾选 
            Button selectButton = new Button();
            selectButton.Name = "selectButton";
            selectButton.Tag = storageGif.id;
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




            imageCanvas.MouseEnter += imageCanvasMouseEnter;
            imageCanvas.MouseLeave += imageCanvasMouseLeave;
            imageCanvas.MouseLeftButtonDown += selectButtonClick;
            selectButton.Click += selectButtonClick;



            imageCanvas.Children.Add(image);
            imageCanvas.Children.Add(bg);
            imageCanvas.Children.Add(titleLabel);
            imageCanvas.Children.Add(selectButton);
            // videoCanvas.Children.Add(rbtn);

            return imageCanvas;
        }
        //点击勾选视频
        private void selectButtonClick(object sender, RoutedEventArgs e)
        {

            Canvas imageCanvas = null;
            FrameworkElement fe = (FrameworkElement)sender;
            if (fe.Name == "imageCanvas")
            {
                imageCanvas = (Canvas)fe;
            }
            else if (fe.Name == "selectButton")
            {
                imageCanvas = (Canvas)VisualTreeHelper.GetParent(fe);
            }

            selectButton(imageCanvas);
        }

        //勾选视频
        private void selectButton(Canvas imageCanvas)
        {
            //移除所有
            unselectedAllImage();

            //选中当前  
            selectCurrImage(imageCanvas);

            //更新当前控件数据
            GifListTag tag = (GifListTag)imageCanvas.Tag;
            currDControl.storageId = tag.storageGif.id;
        }

        //取消所有视频选中状态
        private void unselectedAllImage()
        {
            foreach (Canvas image in imageListWrap.Children)
            {
                GifListTag tag = (GifListTag)image.Tag;
                if (tag.isSelected)
                {
                    Button selectButton = (Button)FrameworkElementUtil.getByName(image, "selectButton");
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
                    image.Tag = tag;
                }
            }

        }
        //选中当前视频
        private void selectCurrImage(Canvas imageCanvas)
        {
            Button selectButton = (Button)FrameworkElementUtil.getByName(imageCanvas, "selectButton");
            if (selectButton != null)
            {
                selectButton.Visibility = Visibility.Visible;
                selectButton.Background = new ImageBrush
                {
                    ImageSource = new BitmapImage(new Uri(@"Resources/ico_media_select_active.png", UriKind.Relative)),
                    Stretch = Stretch.UniformToFill
                };
            }
            GifListTag tag = (GifListTag)imageCanvas.Tag;
            tag.isSelected = true;
            imageCanvas.Tag = tag;

        }


        //鼠标移动到视频上
        private void imageCanvasMouseEnter(object sender, MouseEventArgs e)
        {
            Canvas imageCanvas = (Canvas)sender;
            GifListTag tag = (GifListTag)imageCanvas.Tag;
            if (imageCanvas != null && !tag.isSelected)
            {
                FrameworkElement selectButton = FrameworkElementUtil.getByName(imageCanvas, "selectButton");
                if (selectButton != null)
                {
                    selectButton.Visibility = Visibility.Visible;
                }
            }
        }

        //鼠标离开视频
        private void imageCanvasMouseLeave(object sender, MouseEventArgs e)
        {
            Canvas imageCanvas = (Canvas)sender;
            GifListTag tag = (GifListTag)imageCanvas.Tag;
            if (imageCanvas != null && !tag.isSelected)
            {
                FrameworkElement selectButton = FrameworkElementUtil.getByName(imageCanvas, "selectButton");
                if (selectButton != null)
                {
                    selectButton.Visibility = Visibility.Hidden;
                }
            }
        }

        private void Open_Dialogue_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;

            System.Windows.Forms.OpenFileDialog ofld = new System.Windows.Forms.OpenFileDialog();
            ofld.Filter = "GIF图片|*.gif;";
            ofld.Multiselect = false;
            if (ofld.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {

                String filename = ofld.FileName;
                if (filename != "" || filename != null)
                {
                    System.Drawing.Image sourceImage = System.Drawing.Image.FromFile(filename);
                    String url = uploadImage(btn, filename);
                    StorageGif storageGif = storageGifBll.insert(filename, url, sourceImage.Width, sourceImage.Height, folderId);
                    //this.currDControl.url = url;
                    //this.currDControl.imgs = jpgPath;

                    //插入一个视频到列表 
                    Canvas newImage = initOneImage(storageGif);
                    imageListWrap.Children.Insert(0, newImage);
                    selectButton(newImage);
                }
            }
        }

        /*
          * 上传图片，复制到软件目录下
          */
        private String uploadImage(Button btn, string sourceFilePath)
        {

            //1.复制到软件目录
            String origFilename = FileUtil.getFilename(sourceFilePath);
            string yyyyMMdd = DateTime.Now.ToString("yyyyMMdd");
            string guid = System.Guid.NewGuid().ToString("N");
            String destFilePath = "/myfile/upload/Gif/" + yyyyMMdd + "/" + guid + "/" + origFilename;

            String fullDestFilePath = AppDomain.CurrentDomain.BaseDirectory + destFilePath;
            FileUtil.createDirectoryIfNotExits(fullDestFilePath);
            File.Copy(sourceFilePath, fullDestFilePath);

            return destFilePath;
        }



        /*
         * 保存视频
         */
        private void Submit_Button_Click(object sender, RoutedEventArgs e)
        {
            DControl currDControl = (DControl)currElement.Tag;
            //更新到数据库
            DControl dControl = dControlBll.get(currDControl.id);
            dControl.storageId = this.currDControl.storageId;
            dControlBll.update(dControl);

            //更新页面控件信息   
            StorageGif storageGif = storageGifBll.get(dControl.storageId);
            string imgFullPath = FileUtil.notExistsShowDefault(storageGif?.url, Params.ImageNotExists);
            imgFullPath = AppDomain.CurrentDomain.BaseDirectory + imgFullPath;

            Gif gif = (Gif)currElement;
            gif.updateElement(imgFullPath, true);
            currElement.Tag = dControl;

            Close();

        }

        /*
         * 进入图片管理
         */
        private void imageManagerClick(object sender, RoutedEventArgs e)
        {
            StorageManagerWindow win = new StorageManagerWindow("imageItem");
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
        private void StorageGif_InitFolder()
        {

            //1.初始化页面树
            StorageGifFolder storageGifFolder = storageGifFolderBll.get(1);
            if (storageGifFolder == null) return;

            TreeViewItem firstItem = new TreeViewItem();
            firstItem.IsExpanded = true;
            firstItem.Header = storageGifFolder.name;
            firstItem.Tag = storageGifFolder.id;
            firstItem.Padding = new Thickness(5);

            StorageGif_GetTreeViewItemChildren(firstItem);

            //添加单击、右键事件
            folderTreeColumn.MouseLeftButtonUp += (object sender, MouseButtonEventArgs e) => StorageGif_TreeView_MouseLeftButtonUp(sender, e);

            folderTreeColumn.Items.Clear();
            folderTreeColumn.Items.Add(firstItem);

        }

        /*
      * 获取当前页面的子页面 
      * 
      * @param TreeViewItem currItem 当前页面
      */
        private TreeViewItem StorageGif_GetTreeViewItemChildren(TreeViewItem currItem)
        {
            Int32 id = (Int32)currItem.Tag;

            List<StorageGifFolder> list = storageGifFolderBll.getByParentId(id);
            if (list == null) return null;
            foreach (StorageGifFolder one in list)
            {
                TreeViewItem child = new TreeViewItem();
                child.Header = one.name;
                child.Tag = one.id;
                child.Padding = new Thickness(5);
                // child.Foreground = Brushes.White;

                currItem.Items.Add(child);
                StorageGif_GetTreeViewItemChildren(child);
            }
            return currItem;
        }

        /*
       * 单击页面树，显示当前页面内容
       */
        private void StorageGif_TreeView_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
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
