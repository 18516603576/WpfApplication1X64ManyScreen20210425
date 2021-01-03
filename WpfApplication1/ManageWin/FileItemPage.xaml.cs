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
    /// FileItemPage.xaml 的交互逻辑
    /// </summary>
    public partial class FileItemPage : Page
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
        private readonly StorageFileBll storageFileBll = new StorageFileBll();

        public FileItemPage(Int32 folderId)
        {
            InitializeComponent();
            this.folderId = folderId;
            initImageManager("fileItem");
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
        private void initImageManager(string currItemName)
        {
            Int32 count = storageFileBll.getPageCount(pageSize, folderId);
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
            List<StorageFile> list = new List<StorageFile>();


            if (currPage <= pageCount)
            {
                list = storageFileBll.getNextPage(currPage, pageSize, folderId);
            }

            storageListWrap.Children.Clear();
            foreach (StorageFile storageFile in list)
            {
                Canvas fileCanvas = initOneImage(storageFile);
                storageListWrap.Children.Add(fileCanvas);
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

            List<StorageFile> list = new List<StorageFile>();
            if (currPage <= pageCount)
            {
                list = storageFileBll.getNextPage(currPage, pageSize, folderId);
            }

            storageListWrap.Children.Clear();
            foreach (StorageFile storageFile in list)
            {
                Canvas fileCanvas = initOneImage(storageFile);
                storageListWrap.Children.Add(fileCanvas);
            }
            currPageLabel.Content = currPage;
            SelectAll.IsChecked = false;
        }


        /*
         * 初始化一个图片控件
         */
        private Canvas initOneImage(StorageFile storageFile)
        {
            FileListTag tag = new FileListTag();
            tag.isSelected = false;
            tag.storageFile = storageFile;
            Canvas fileCanvas = new Canvas();
            fileCanvas.Name = "fileCanvas";
            fileCanvas.Width = 125;
            fileCanvas.Height = 150;
            fileCanvas.Margin = new Thickness(10);
            fileCanvas.Tag = tag;

            //1.按钮 
            string imgFullPath = "/myfile/sysimg/file/ico_other.png";
            if (storageFile.ext == ".doc" || storageFile.ext == ".docx" || storageFile.ext == ".xps")
            {
                imgFullPath = "/myfile/sysimg/file/ico_word.png";
            }
            else if (storageFile.ext == ".pdf")
            {
                imgFullPath = "/myfile/sysimg/file/ico_pdf.png";
            }
            else if (storageFile.ext == ".mp3")
            {
                imgFullPath = "/myfile/sysimg/file/ico_mp3.png";
            }
            imgFullPath = FileUtil.notExistsShowDefault(imgFullPath, Params.ImageNotExists);
            imgFullPath = AppDomain.CurrentDomain.BaseDirectory + imgFullPath;
            Image image = new Image();
            image.Name = "image";
            image.Width = 125;
            image.Height = 125;
            image.Source = FileUtil.readImage2(imgFullPath, 200); ;
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
            Label lLabel = new Label();
            lLabel.Width = 125;
            lLabel.Height = 25;
            lLabel.Content = FileUtil.ByteToKB(storageFile.size);
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
            titleLabel.Content = storageFile.origFilename;
            titleLabel.SetValue(Canvas.LeftProperty, 0.0);
            titleLabel.SetValue(Canvas.BottomProperty, 0.0);
            titleLabel.ToolTip = titleLabel.Content;


            //勾选 
            Button selectButton = new Button();
            selectButton.Name = "selectButton";
            selectButton.Tag = storageFile.id;
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




            fileCanvas.MouseEnter += imageCanvasMouseEnter;
            fileCanvas.MouseLeave += imageCanvasMouseLeave;
            fileCanvas.MouseLeftButtonDown += selectButtonClick;
            selectButton.Click += selectButtonClick;



            fileCanvas.Children.Add(image);
            fileCanvas.Children.Add(bg);
            fileCanvas.Children.Add(titleLabel);
            fileCanvas.Children.Add(selectButton);
            // videoCanvas.Children.Add(rbtn);

            return fileCanvas;
        }

        //鼠标移动到视频上
        private void imageCanvasMouseEnter(object sender, MouseEventArgs e)
        {
            Canvas fileCanvas = (Canvas)sender;
            FileListTag tag = (FileListTag)fileCanvas.Tag;
            if (fileCanvas != null && !tag.isSelected)
            {
                FrameworkElement selectButton = FrameworkElementUtil.getByName(fileCanvas, "selectButton");
                if (selectButton != null)
                {
                    selectButton.Visibility = Visibility.Visible;
                }
            }
        }

        //鼠标离开视频
        private void imageCanvasMouseLeave(object sender, MouseEventArgs e)
        {
            Canvas fileCanvas = (Canvas)sender;
            FileListTag tag = (FileListTag)fileCanvas.Tag;
            if (fileCanvas != null && !tag.isSelected)
            {
                FrameworkElement selectButton = FrameworkElementUtil.getByName(fileCanvas, "selectButton");
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
            Canvas fileCanvas = null;
            FrameworkElement fe = (FrameworkElement)sender;
            if (fe.Name == "fileCanvas")
            {
                fileCanvas = (Canvas)fe;
            }
            else if (fe.Name == "selectButton")
            {
                fileCanvas = (Canvas)VisualTreeHelper.GetParent(fe);
            }
            FileListTag tag = (FileListTag)fileCanvas.Tag;
            toggleSelectStatus(fileCanvas);
        }
        /*
         * 切换选中状态
         */
        private void toggleSelectStatus(Canvas fileCanvas)
        {
            FileListTag tag = (FileListTag)fileCanvas.Tag;
            if (tag.isSelected)
            {
                unSelectThisImage(fileCanvas);
            }
            else
            {
                selectThisImage(fileCanvas);
            }

        }


        //取消所有选中
        private void UnSelectAllFun()
        {
            foreach (Canvas fileCanvas in storageListWrap.Children)
            {
                FileListTag tag = (FileListTag)fileCanvas.Tag;
                if (tag.isSelected)
                {
                    unSelectThisImage(fileCanvas);
                }
            }

        }

        //选中所有
        private void SelectAllFun()
        {
            foreach (Canvas fileCanvas in storageListWrap.Children)
            {
                FileListTag tag = (FileListTag)fileCanvas.Tag;
                if (!tag.isSelected)
                {
                    selectThisImage(fileCanvas);
                }
            }

        }

        /*
         * 选中当前图片
         */
        private void selectThisImage(Canvas fileCanvas)
        {
            FileListTag tag = (FileListTag)fileCanvas.Tag;
            Button selectButton = (Button)FrameworkElementUtil.getByName(fileCanvas, "selectButton");
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
            fileCanvas.Tag = tag;
        }

        /*
         * 取消选中当前图片
         */
        private void unSelectThisImage(Canvas fileCanvas)
        {
            FileListTag tag = (FileListTag)fileCanvas.Tag;
            Button selectButton = (Button)FrameworkElementUtil.getByName(fileCanvas, "selectButton");
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
            fileCanvas.Tag = tag;
        }

        /*
                * 点击 - 批量删除图片
                */
        private void Batch_Delete_Click(object sender, RoutedEventArgs e)
        {
            List<StorageFile> list = new List<StorageFile>();//创建了一个空列表
            List<Canvas> canvasList = new List<Canvas>();
            UIElementCollection children = storageListWrap.Children;

            //1.获取所有勾选图片,同时移除选中
            for (int i = 0; i < children.Count; i++)
            {
                Canvas canvas = (Canvas)children[i];
                FileListTag tag = (FileListTag)canvas.Tag;
                if (tag.isSelected)
                {
                    list.Add(tag.storageFile);
                    canvasList.Add(canvas);
                }
            }

            DeleteFileWindow win = new DeleteFileWindow(storageListWrap, list, canvasList);
            win.ShowDialog();


            ////3.删除数据库记录
            //foreach (StorageFile storageFile in list)
            //{
            //    int row = storageFileBll.delete(storageFile);
            //}


            ////4.从页面移除选中项
            //foreach (Canvas canvas in canvasList)
            //{
            //    storageListWrap.Children.Remove(canvas);
            //}
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



            List<StorageFile> list = new List<StorageFile>();
            if (currPage <= pageCount)
            {
                list = storageFileBll.getNextPage(currPage, pageSize, folderId);
            }

            storageListWrap.Children.Clear();
            foreach (StorageFile storageFile in list)
            {
                Canvas fileCanvas = initOneImage(storageFile);
                storageListWrap.Children.Add(fileCanvas);
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
            List<StorageFile> list = new List<StorageFile>();//创建了一个空列表
            List<Canvas> canvasList = new List<Canvas>();
            UIElementCollection children = storageListWrap.Children;

            //1.获取所有勾选图片,同时移除选中
            for (int i = 0; i < children.Count; i++)
            {
                Canvas canvas = (Canvas)children[i];
                FileListTag tag = (FileListTag)canvas.Tag;
                if (tag.isSelected)
                {
                    list.Add(tag.storageFile);
                    canvasList.Add(canvas);
                }
            }
            StorageFileMoveToFolderWindow win = new StorageFileMoveToFolderWindow(storageListWrap, list, canvasList);
            win.ShowDialog();
        }
    }
}
