using Bll;
using Common;
using Common.Data;
using Common.util;
using Model;
using System;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Vlc.DotNet.Core;
using Vlc.DotNet.Wpf;

namespace WpfApplication1.MenuWin
{
    /// <summary>
    /// EditBackgroundMusicWindow.xaml 的交互逻辑
    /// </summary>
    public partial class EditBackgroundMusicWindow : Window
    {

        private readonly CfgBll cfgBll = new CfgBll();
        private readonly StorageFileBll storageFileBll = new StorageFileBll();
        private readonly StorageImageBll storageImageBll = new StorageImageBll();
        private readonly MainWindow mainWindow;

        public EditBackgroundMusicWindow(MainWindow mainWindow)
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.mainWindow = mainWindow;
            initData();
        }
        /*
         * 初始化页面数据
         */
        private void initData()
        {
            Cfg cfg = cfgBll.get(1);
            backgroundMusicShow.IsChecked = cfg.backgroundMusicShow;
            if (cfg.backgroundMusicId > 0)
            {
                StorageFile storageFile = storageFileBll.get(cfg.backgroundMusicId);
                string musicFullPath = FileUtil.notExistsShowDefault(storageFile?.url, Params.BackgroundMusicNotExists);
                backgroundMusicUrl.Text = musicFullPath;
                backgroundMusicUrl.Tag = storageFile;
            }
            backgroundMusicButtonLeft.Text = cfg.backgroundMusicButtonLeft.ToString();
            backgroundMusicButtonTop.Text = cfg.backgroundMusicButtonTop.ToString();
            backgroundMusicButtonWidth.Text = cfg.backgroundMusicButtonWidth.ToString();
            backgroundMusicButtonHeight.Text = cfg.backgroundMusicButtonHeight.ToString();
            backgroundMusicButtonLeft.Text = cfg.backgroundMusicButtonLeft.ToString();
            backgroundMusicAutoplay.IsChecked = cfg.backgroundMusicAutoplay;
            backgroundMusicLoop.IsChecked = cfg.backgroundMusicLoop;

            //显示按钮背景图
            StorageImage storageImage = storageImageBll.get(cfg.backgroundMusicButtonImageId);
            Canvas buttonCanvas = initBackgroundMusicButton(storageImage);
            BackgroundMusicButtonImageWrapPanel.Children.Add(buttonCanvas);
        }

        /*
         * 初始化一个图片控件
        */
        private Canvas initBackgroundMusicButton(StorageImage storageImage)
        {

            Canvas buttonImageCanvas = new Canvas();
            buttonImageCanvas.Name = "buttonImageCanvas";
            buttonImageCanvas.Width = 120;
            buttonImageCanvas.Height = 120;
            buttonImageCanvas.Tag = storageImage;

            //1.按钮 
            string imgFullPath = FileUtil.notExistsShowDefault(storageImage?.url, Params.BackgroundMusicImageNotExists);
            imgFullPath = AppDomain.CurrentDomain.BaseDirectory + imgFullPath;
            Button btn = new Button();
            btn.Name = "image";
            btn.Width = 120;
            btn.Height = 120;
            btn.BorderThickness = new Thickness(0);
            //btn.Background = new ImageBrush
            //{
            //    ImageSource = FileUtil.readImage2(imgFullPath,200)
            //  ,
            //    Stretch = Stretch.Uniform
            //};
            btn.Background = Brushes.Transparent;
            FileUtil.readImage2Button(btn, imgFullPath, 200, Stretch.Uniform);
            btn.Click += Open_Dialog_ButtonImage_Click;

            buttonImageCanvas.Children.Add(btn);
            return buttonImageCanvas;
        }



        /*
         * 保存数据
         */
        private void Submit_Button_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(backgroundMusicButtonLeft.Text))
            {
                MessageBox.Show("请填写按钮左边距；"); return;
            }
            else if (!DataUtil.isInt(backgroundMusicButtonLeft.Text.ToString()))
            {
                MessageBox.Show("按钮左边距必须是整数；"); return;
            }
            if (string.IsNullOrWhiteSpace(backgroundMusicButtonTop.Text.ToString()))
            {
                MessageBox.Show("请填写按钮上边距；"); return;
            }
            else if (!DataUtil.isInt(backgroundMusicButtonTop.Text.ToString()))
            {
                MessageBox.Show("按钮上边距必须是整数；"); return;
            }

            if (string.IsNullOrWhiteSpace(backgroundMusicButtonWidth.Text))
            {
                MessageBox.Show("请填写按钮宽度；"); return;
            }
            else if (!DataUtil.isInt(backgroundMusicButtonLeft.Text.ToString()))
            {
                MessageBox.Show("按钮宽度必须是整数；"); return;
            }
            if (string.IsNullOrWhiteSpace(backgroundMusicButtonHeight.Text.ToString()))
            {
                MessageBox.Show("请填写按钮高度；"); return;
            }
            else if (!DataUtil.isInt(backgroundMusicButtonTop.Text.ToString()))
            {
                MessageBox.Show("按钮高度必须是整数；"); return;
            }

            //1.更新到数据库
            Cfg cfg = cfgBll.get(1);
            cfg.backgroundMusicShow = (bool)backgroundMusicShow.IsChecked;
            cfg.backgroundMusicAutoplay = (bool)backgroundMusicAutoplay.IsChecked;
            cfg.backgroundMusicLoop = (bool)backgroundMusicLoop.IsChecked;

            cfg.backgroundMusicButtonWidth = int.Parse(backgroundMusicButtonWidth.Text);
            cfg.backgroundMusicButtonHeight = int.Parse(backgroundMusicButtonHeight.Text);
            cfg.backgroundMusicButtonLeft = int.Parse(backgroundMusicButtonLeft.Text);
            cfg.backgroundMusicButtonTop = int.Parse(backgroundMusicButtonTop.Text);


            StorageFile storageFile = (StorageFile)backgroundMusicUrl.Tag;
            if (storageFile != null && !string.IsNullOrWhiteSpace(backgroundMusicUrl.Text))
            {
                cfg.backgroundMusicId = storageFile.id;
            }
            else
            {
                cfg.backgroundMusicId = 0;
            }

            Canvas buttonImageCanvas = FrameworkElementUtil.GetChildObject<Canvas>(BackgroundMusicButtonImageWrapPanel, "buttonImageCanvas");
            StorageImage storageImage = (StorageImage)buttonImageCanvas.Tag;
            if (storageImage != null)
            {
                cfg.backgroundMusicButtonImageId = storageImage.id;
            }
            else
            {
                cfg.backgroundMusicButtonImageId = 0;
            }
            cfgBll.update(cfg);

            //2.更新全局配置
            App.localStorage.cfg = cfg;
            //3.重新加载页面
            mainWindow.reloadWindow();

            Close();

        }

        /*
         * 上传音乐
         * 
         */
        private VlcControl vlcControl = null;
        private string destFilePath = "";
        private string sourceFilePath = "";
        private void Open_Dialog_Music_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;

            System.Windows.Forms.OpenFileDialog ofld = new System.Windows.Forms.OpenFileDialog();
            ofld.Filter = "音乐|*.mp3";
            ofld.Multiselect = false;
            if (ofld.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.sourceFilePath = ofld.FileName;
                if (!string.IsNullOrWhiteSpace(sourceFilePath))
                {
                    this.destFilePath = UploadUtil.uploadFile(sourceFilePath);
                    this.saveToDB(destFilePath); 
                }
            }
        }
        /*
    * 截图并且获取时长
    */
        public void saveToDB(string sourceFP)
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

            ThreadPool.QueueUserWorkItem(_ =>
            {

                long length = vlcControl.SourceProvider.MediaPlayer.Length;
                int duration = Convert.ToInt32(length / 1000.0);
                 
                
                StorageFile storageFile = storageFileBll.insert(this.sourceFilePath, destFilePath, 1, duration);

                //直接变更控件背景 
                App.Current.Dispatcher.Invoke((Action)(() =>
                { 
                    backgroundMusicUrl.Text = destFilePath;
                    backgroundMusicUrl.Tag = storageFile;
                }));

                vlcControl?.Dispose();
            });
        }







        //上传按钮背景图
        private void Open_Dialog_ButtonImage_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;

            System.Windows.Forms.OpenFileDialog ofld = new System.Windows.Forms.OpenFileDialog();
            ofld.Filter = "图片|*.jpg;*.jpeg;*.png;*.gif;*.bmp";
            ofld.Multiselect = false;
            if (ofld.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {

                string filename = ofld.FileName;
                if (filename != "" || filename != null)
                {
                    System.Drawing.Image sourceImage = System.Drawing.Image.FromFile(filename);
                    String url = UploadUtil.uploadImage(filename);

                    StorageImage storageImage = storageImageBll.insert(filename, url, sourceImage.Width, sourceImage.Height, 1);
                    //直接变更控件背景 
                    btn.Background = new ImageBrush
                    {
                        ImageSource = FileUtil.readImage2(AppDomain.CurrentDomain.BaseDirectory + storageImage.url, 200)
                        ,
                        Stretch = Stretch.Uniform
                    };
                    Canvas buttonImageCanvas = (Canvas)btn.Parent;
                    buttonImageCanvas.Tag = storageImage;
                }
            }
        }

    }
}
