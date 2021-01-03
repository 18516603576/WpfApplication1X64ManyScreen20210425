using Bll;
using Common;
using Common.control;
using Common.Data;
using Common.util;
using Model;
using Model.dto;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WpfApplication1.manage;

namespace WpfApplication1.ManageWin
{
    /// <summary>
    /// EditBackgroundImageWindow.xaml 的交互逻辑
    /// </summary>
    public partial class EditCVideoBackgroundWindow : Window
    {
        private readonly StorageImageBll storageImageBll = new StorageImageBll();
        private readonly StorageVideoBll storageVideoBll = new StorageVideoBll();
        private readonly DControlBll dControlBll = new DControlBll();
        private readonly DPageBll dPageBll = new DPageBll();
        //默认图片背景
        private readonly string defaultIcoAddVideo = "/myfile/sysimg/ico-add-video.png";
        private DPage currDPage;
        // private Grid mainContainer;
        private readonly PageTemplate pageTemplate = null;
        private readonly Editing editing = null;
        public EditCVideoBackgroundWindow(PageTemplate pageTemplate, DPage dPage, Editing editing)
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            currDPage = dPage;
            this.pageTemplate = pageTemplate;
            this.editing = editing;

            init(currDPage.backgroundVideoId);
        }

        private void init(Int32 backgroundVideoIdVal)
        {

            StorageImage storageImage = null;
            StorageVideo storageVideo = null;
            if (backgroundVideoIdVal > 0)
            {
                storageVideo = storageVideoBll.get(backgroundVideoIdVal);
                if (storageVideo != null)
                {
                    storageImage = storageImageBll.get(storageVideo.storageImageId);
                }
            }
            //Canvas canvas = initOneImage(storageVideo, storageImage);
            //imgList.Children.Add(canvas);


            //显示背景图片 
            backgroundVideoId.Tag = storageVideo;
            string imgPath = FileUtil.notExistsShowDefault(storageImage?.url, Params.Ico_Add_Image);
            backgroundVideoId.Background = new ImageBrush
            {
                ImageSource = FileUtil.readImage(AppDomain.CurrentDomain.BaseDirectory + imgPath)
               ,
                Stretch = Stretch.Uniform
            };
            backgroundVideoId.Click += btn_Click;

            //删除
            removeBtn.Background = new ImageBrush
            {
                ImageSource = FileUtil.readImage(AppDomain.CurrentDomain.BaseDirectory + "/myfile/sysimg/ico-image-remove.png")
                ,
                Stretch = Stretch.UniformToFill
            };
            removeBtn.Click += removeBtnClick; 

            url.PreviewMouseLeftButtonUp += url_PreviewMouseLeftButtonUp;
        }
        private void url_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            StorageVideo storageVideo = null;
            object tag = backgroundVideoId.Tag;
            if (tag == null) return;
            storageVideo = (StorageVideo)tag;
            string fullFolder = FileUtil.getDirectory(AppDomain.CurrentDomain.BaseDirectory + storageVideo.url);

            FileUtil.openFile(fullFolder);
        }


        /*
         * 初始化一个图片控件
         */
        //private Canvas initOneImage(StorageVideo storageVideo, StorageImage storageImage)
        //{

        //    Canvas imgCanvas = new Canvas();
        //    imgCanvas.Name = "imgCanvas";
        //    imgCanvas.Width = 576;
        //    imgCanvas.Height = 324;
        //    imgCanvas.Margin = new Thickness(0);
        //    imgCanvas.MouseEnter += btnMouseEnter;
        //    imgCanvas.MouseLeave += btnMouseLeave;
        //    imgCanvas.Tag = storageVideo;
        //    //1.按钮
        //    //string btnImg = AppDomain.CurrentDomain.BaseDirectory + img;
        //    //if (string.IsNullOrWhiteSpace(img)) {
        //    //    btnImg = AppDomain.CurrentDomain.BaseDirectory + defaultIcoAddImage;
        //    //}  
        //    string imgFullPath = FileUtil.notExistsShowDefault(storageImage?.url, defaultIcoAddVideo);
        //    imgFullPath = AppDomain.CurrentDomain.BaseDirectory + imgFullPath;
        //    Button btn = new Button();
        //    btn.Name = "imgBtn";
        //    btn.Width = 576;
        //    btn.Height = 324;
        //    btn.Background = new ImageBrush
        //    {
        //        ImageSource = FileUtil.readImage(imgFullPath)
        //       ,
        //        Stretch = Stretch.Uniform
        //    };
        //    btn.Click += (sender, e) => btn_Click(imgCanvas, btn);

        //    //2.按钮行
        //    Canvas bg = new Canvas();
        //    bg.Name = "bg";
        //    bg.Background = Brushes.Black;
        //    bg.Width = 576;
        //    bg.Height = 24;
        //    bg.Opacity = 0.6;
        //    bg.SetValue(Canvas.BottomProperty, 0.0);
        //    bg.SetValue(Canvas.LeftProperty, 0.0);
        //    bg.Visibility = Visibility.Hidden;



        //    Button removeBtn = new Button();
        //    removeBtn.Width = 20;
        //    removeBtn.Height = 20;
        //    removeBtn.BorderThickness = new Thickness(0);
        //    removeBtn.Background = new ImageBrush
        //    {
        //        ImageSource = FileUtil.readImage(AppDomain.CurrentDomain.BaseDirectory + "/myfile/sysimg/ico-image-remove.png")
        //        ,
        //        Stretch = Stretch.UniformToFill
        //    };
        //    removeBtn.SetValue(Canvas.LeftProperty, (bg.Width - removeBtn.Width) / 2);
        //    removeBtn.SetValue(Canvas.TopProperty, 2.0);
        //    bg.Children.Add(removeBtn);
        //    removeBtn.Click += removeBtnClick;



        //    imgCanvas.Children.Add(btn);
        //    imgCanvas.Children.Add(bg);

        //    return imgCanvas;
        //}


        /*
         * 清空控件中的图片
         */
        private void removeBtnClick(object sender, RoutedEventArgs e)
        { 
            backgroundVideoId.Tag = null;
            backgroundVideoId.Background = new ImageBrush
            {
                ImageSource = FileUtil.readImage(AppDomain.CurrentDomain.BaseDirectory + Params.Ico_Add_Video)
              ,
                Stretch = Stretch.Uniform
            };
        }
         

        private void btn_Click(object sender, RoutedEventArgs e)
        { 
            EditCVideoBackgroundSelectorWindow win = new EditCVideoBackgroundSelectorWindow(backgroundVideoId);
            Boolean result = (Boolean)win.ShowDialog();
        }

        /*
         * 保存数据 
         */
        private void Submit_Button_Click(object sender, RoutedEventArgs e)
        {
            
            StorageVideo storageVideo = null;
            object tag = backgroundVideoId.Tag;
            if (tag != null)
            {
                storageVideo = (StorageVideo)tag;
            }

            //更新到数据库  
            DPage dPage = dPageBll.get(currDPage.id);
            dPage.backgroundVideoId = storageVideo == null ? 0 : storageVideo.id;
            dPageBll.update(dPage);
            currDPage = dPage;
            pageTemplate.dPage = dPage;


            //更新页面控件信息   

            //1.清空视频背景  
            foreach (FrameworkElement ele in pageTemplate.backgroundVideo.Children)
            {
                if (ele.Name == "CVideoBackground")
                {
                    CVideoBackground cVideoBackground = (CVideoBackground)ele;
                   
                    cVideoBackground = null;
                }
            }
            pageTemplate.backgroundVideo.Children.Clear();
            if (storageVideo != null)
            {
                insertCVideoBackground(storageVideo);
            } 

            Close();
        }

        private void insertCVideoBackground(StorageVideo storageVideo)
        {

            Cfg cfg = new Cfg();
            cfg.screenWidth = Convert.ToInt32(pageTemplate.ActualWidth);
            cfg.screenHeight = Convert.ToInt32(pageTemplate.ActualHeight);

            DControl ctl = new DControl();
            ctl.name = "CVideoBackground";
            ctl.width = cfg.screenWidth;
            ctl.height = cfg.screenHeight;
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
            pageTemplate.backgroundVideo.Children.Add(cVideoBackground1);
        }

    }
}
