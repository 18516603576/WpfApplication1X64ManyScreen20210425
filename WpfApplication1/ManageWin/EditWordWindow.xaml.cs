using Bll;
using Common;
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
using WpfApplication1.manage;

namespace WpfApplication1.ManageWin
{
    /// <summary>
    /// EditWordWindow.xaml 的交互逻辑
    /// </summary>
    public partial class EditWordWindow : Window
    {

        private readonly StorageImageBll storageImageBll = new StorageImageBll();
        private readonly StorageFileBll storageFileBll = new StorageFileBll();
        private readonly DControlBll dControlBll = new DControlBll();
        private readonly StorageFileFolderBll storageFileFolderBll = new StorageFileFolderBll();
        private readonly Editing editing;
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



        public EditWordWindow(Editing editing, FrameworkElement currElement)
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.editing = editing;
            this.currElement = currElement;
            currDControl = (DControl)currElement.Tag;
            init(currDControl);
            StorageFile_InitFolder();
        }

        private void init(DControl ctl)
        {
            if (!isLoading && !isLastPage)
            {
                isLoading = true;
                imageListWrap.Children.Clear();
                initWordList();
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
                    initWordList();  //加载下一页
                    isLoading = false;
                }
            }

        }
        /*
         * 加载所有视频
         */
        private void initWordList()
        {
            currPage = currPage + 1;
            List<StorageFile> list = storageFileBll.getNextPageOfWord(currPage, pageSize, folderId);
            if (list == null || list.Count == 0)
            {
                isLastPage = true;
            }
            foreach (StorageFile storageFile in list)
            {
                Canvas fileCanvas = initOneFile(storageFile);
                imageListWrap.Children.Add(fileCanvas);
            }
        }

        /*
       * 初始化一个图片控件
       */
        private Canvas initOneFile(StorageFile storageFile)
        {
            FileListTag tag = new FileListTag();
            tag.isSelected = false;
            tag.storageFile = storageFile;
            Canvas fileCanvas = new Canvas();
            fileCanvas.Name = "fileCanvas";
            fileCanvas.Width = 100;
            fileCanvas.Height = 100;
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
            image.Width = 100;
            image.Height = 75;
            image.Source = FileUtil.readImage2(imgFullPath, 200);
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
            titleLabel.Width = 100;
            titleLabel.Height = 25;
            titleLabel.Content = storageFile.origFilename;
            titleLabel.SetValue(Canvas.LeftProperty, 0.0);
            titleLabel.SetValue(Canvas.BottomProperty, 0.0);
            titleLabel.ToolTip = titleLabel.Content;


            //勾选 
            Button selectButton = new Button();
            selectButton.Name = "selectButton";
            selectButton.Tag = storageFile.id;
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

            selectButton(fileCanvas);
        }

        //勾选视频
        private void selectButton(Canvas fileCanvas)
        {
            //移除所有
            unselectedAllImage();

            //选中当前  
            selectCurrImage(fileCanvas);

            //更新当前控件数据
            FileListTag tag = (FileListTag)fileCanvas.Tag;
            currDControl.storageId = tag.storageFile.id;
        }

        //取消所有视频选中状态
        private void unselectedAllImage()
        {
            foreach (Canvas fileCanvas in imageListWrap.Children)
            {
                FileListTag tag = (FileListTag)fileCanvas.Tag;
                if (tag.isSelected)
                {
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
            }

        }
        //选中当前视频
        private void selectCurrImage(Canvas fileCanvas)
        {
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
            FileListTag tag = (FileListTag)fileCanvas.Tag;
            tag.isSelected = true;
            fileCanvas.Tag = tag;

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

        private void Open_Dialogue_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;

            System.Windows.Forms.OpenFileDialog ofld = new System.Windows.Forms.OpenFileDialog();
            ofld.Filter = "文档|*.doc;*.docx;*.xps;*.pdf";
            ofld.Multiselect = false;
            if (ofld.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {

                String filename = ofld.FileName;
                if (filename != "" || filename != null)
                {
                    String url = uploadWord(btn, filename);
                    StorageFile storageWord = storageFileBll.insert(filename, url, folderId);
                    //this.currDControl.url = url;
                    //this.currDControl.imgs = jpgPath;

                    //插入一个视频到列表 
                    Canvas newImage = initOneFile(storageWord);
                    imageListWrap.Children.Insert(0, newImage);
                    selectButton(newImage);
                }
            }
        }

        /*
         * 上传图片，复制的软件目录下
         */
        private String uploadWord(Button btn, string sourceFilePath)
        {
            string ext = FileUtil.getExt(sourceFilePath);
            if (ext != "doc" && ext != "docx" && ext != "xps" && ext != "pdf") return "";

            //1.复制到软件目录
            String origFilename = FileUtil.getFilename(sourceFilePath);
            string yyyyMMdd = DateTime.Now.ToString("yyyyMMdd");
            string guid = System.Guid.NewGuid().ToString("N");
            String destFilePath = "/myfile/upload/Word/" + yyyyMMdd + "/" + guid + "/" + origFilename;

            String fullDestFilePath = AppDomain.CurrentDomain.BaseDirectory + destFilePath;
            FileUtil.createDirectoryIfNotExits(fullDestFilePath);
            File.Copy(sourceFilePath, fullDestFilePath);


            //转换为xps
            if (ext == "doc" || ext == "docx" || ext == "pdf")
            {
                string newDestFilePath = FileUtil.replaceExt(destFilePath, "xps");
                // XpsDocument xpsDocument = WordUtil.ConvertWordToXPS(fullDestFilePath, AppDomain.CurrentDomain.BaseDirectory + newDestFilePath);
                // xpsDocument.Close();
                Boolean b = WordUtil.ConvertToXPS(fullDestFilePath, AppDomain.CurrentDomain.BaseDirectory + newDestFilePath);
                destFilePath = newDestFilePath;
            }

            return destFilePath;
        }



        /*
         * 保存视频
         */
        private void Submit_Button_Click(object sender, RoutedEventArgs e)
        {

            DControl currDControl = (DControl)currElement.Tag;
            StorageFile storageFile = storageFileBll.get(currDControl.storageId);
            string ext = storageFile.ext;

            if (ext != ".doc" && ext != ".docx" && ext != ".xps" && ext != ".pdf") {
                MessageBox.Show("请选择文档格式：doc,docx,pdf,xps");
                return;
            }
            //更新到数据库
            DControl dControl = dControlBll.get(currDControl.id);
            dControl.storageId = this.currDControl.storageId;
            dControlBll.update(dControl);


            //更新页面控件信息   
           
            string wordFullPath = FileUtil.notExistsShowDefault(storageFile?.url, Params.WordNotExists);
            wordFullPath = AppDomain.CurrentDomain.BaseDirectory + wordFullPath;

            DocumentViewer docViewer = (DocumentViewer)currElement;
            //  cWord.updateElement(dControl, true, wordFullPath); 

            docViewer.Document = WordUtil.loadWord(wordFullPath);
            docViewer.FitToWidth();
            //pageTemplate.editing.updateEditingBorder(dControl); 

            currElement.Tag = dControl;
            Close();

        }

        /*
         * 进入图片管理
         */
        private void fileManagerClick(object sender, RoutedEventArgs e)
        {
            StorageManagerWindow win = new StorageManagerWindow("fileItem");
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
        private void StorageFile_InitFolder()
        {

            //1.初始化页面树
            StorageFileFolder storageFileFolder = storageFileFolderBll.get(1);
            if (storageFileFolder == null) return;

            TreeViewItem firstItem = new TreeViewItem();
            firstItem.IsExpanded = true;
            firstItem.Header = storageFileFolder.name;
            firstItem.Tag = storageFileFolder.id;
            firstItem.Padding = new Thickness(5);

            StorageFile_GetTreeViewItemChildren(firstItem);

            //添加单击、右键事件
            folderTreeColumn.MouseLeftButtonUp += (object sender, MouseButtonEventArgs e) => StorageFile_TreeView_MouseLeftButtonUp(sender, e);

            folderTreeColumn.Items.Clear();
            folderTreeColumn.Items.Add(firstItem);

        }

        /*
      * 获取当前页面的子页面 
      * 
      * @param TreeViewItem currItem 当前页面
      */
        private TreeViewItem StorageFile_GetTreeViewItemChildren(TreeViewItem currItem)
        {
            Int32 id = (Int32)currItem.Tag;

            List<StorageFileFolder> list = storageFileFolderBll.getByParentId(id);
            if (list == null) return null;
            foreach (StorageFileFolder one in list)
            {
                TreeViewItem child = new TreeViewItem();
                child.Header = one.name;
                child.Tag = one.id;
                child.Padding = new Thickness(5);
                // child.Foreground = Brushes.White;

                currItem.Items.Add(child);
                StorageFile_GetTreeViewItemChildren(child);
            }
            return currItem;
        }

        /*
       * 单击页面树，显示当前页面内容
       */
        private void StorageFile_TreeView_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
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
