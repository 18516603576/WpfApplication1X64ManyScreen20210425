﻿using Bll;
using Common;
using Common.Data;
using Common.MQ;
using Common.util;
using Model;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Vlc.DotNet.Core;
using Vlc.DotNet.Wpf;

namespace ShowBox
{
    /// <summary>
    /// PreviewWindow.xaml 的交互逻辑
    /// </summary>
    public partial class PreviewWindow : Window
    {
        private readonly CfgBll cfgBll = new CfgBll();
        private readonly DPageBll dPageBll = new DPageBll();
        private readonly DControlBll dControlBll = new DControlBll();
        //第一预览页
        private readonly int currPageId = 1;
        private readonly StorageFileBll storageFileBll = new StorageFileBll();
        private readonly StorageImageBll storageImageBll = new StorageImageBll();

        //音乐按钮动画句柄 
        private Storyboard musicStoryboard = null;
        //音乐控件 
        private ContentControl musicMediaElement = null;
        //vlc播放器
        private VlcControl vlcControl = null;
        //音乐路径 /myfile/1.mp3
        private string musicFilePath = "";
        //音乐按钮
        private Button musicButton = null;
        public PageTemplate pageTemplate; 
        //当前窗口所在的屏幕id
        public int screenCfgId = 0;
        //是否主屏幕
        private ScreenCfg screenCfg =null ;
        //当前所在屏幕
        public System.Windows.Forms.Screen screen;
        

        //定时器 无操作返回首页
        readonly DispatcherTimer backToHomeTimer = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(1000), IsEnabled = true };
        //当前屏幕下的消息服务器
        public  MqServer mqServer = new MqServer();
        public PreviewWindow(int currPageId,ScreenCfg screenCfg,System.Windows.Forms.Screen s)
        {
            InitializeComponent();
            App.localStorage.currWindow = this;
            this.currPageId = currPageId;
            this.screenCfgId = screenCfg.id;
            this.screenCfg = screenCfg; 
            this.screen = s;
            this.mqServer.sendMsgEvent += Client_ReceiveMsgEvent;  

            init();
            Unloaded += Window_Unloaded;
            this.Loaded += Window_Loaded;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Maximized; 
        }

        /*
         * 释放内存
         */
        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("PreviewWindow:Window_Unloaded");
            FrameUtil.RemoveBackEntry(pageTemplate.mainFrame);
            pageTemplate = null; 
            mainFrame = null;
            Loaded -= Window_Loaded;
            Unloaded -= Window_Unloaded;
            this.mqServer.sendMsgEvent -= Client_ReceiveMsgEvent;
            clearMusic();
            mainGrid.Children.Clear();  
            GC.Collect();
        }
        private void clearMusic()
        {
            //1.移除原有背景音乐
            if (musicMediaElement != null)
            {
                musicPause_ByMessage(); 
                vlcControl?.Dispose();
                mainGrid.Children.Remove(musicMediaElement);
            }
            //2.移除音乐按钮
            if (musicButton != null)
            {
                musicButton.Background = null; 
                musicStoryboard.Stop(musicButton);
                mainGrid.Children.Remove(musicButton);
            }
        }

        private void init()
        {
            Cfg cfg = cfgBll.get(1);
            App.localStorage.cfg = cfg;

            CFrameTag indexTag = new CFrameTag();
            indexTag.currCFrame = mainFrame;
            indexTag.currCoverBorder = CoverBorder;
            indexTag.parentFrame = null;
            mainFrame.Tag = indexTag;
            initHeaderMenu();
            showMusicButton();     //如果页面有视频或音频，则暂停背景音乐
            openDefaultPage();

            if (cfg.noActionTimeBackToHome > 0)
            {
                backToHomeTimer.Tick += new EventHandler(BackToHomeTimer_Tick); //超过计时间隔时发生 
                backToHomeTimer.Start(); //DT启动   
            }

        }

        /*
         * 打开首页
         */
        private void openDefaultPage()
        {
            pageTemplate = new PageTemplate(mainFrame, currPageId, false,screen,this.mqServer);
            mainFrame.Navigate(pageTemplate); 

        }


        /*
         * 初始化头部菜单
         */
        private void initHeaderMenu()
        {

            DPage dPage = dPageBll.get(currPageId);
            int frameWidth = App.localStorage.cfg.screenWidth;
            int frameHeight = App.localStorage.cfg.screenHeight;
            if (dPage.width > 0) frameWidth = dPage.width;
            if (dPage.height > 0) frameHeight = dPage.height;

            Int32 pagePercent = FrameUtil.getMaxPercent(frameWidth, frameHeight);
            mainFrame.Width = frameWidth;
            mainFrame.Height = frameHeight;

            //首次加载切换为默认百分比
            changeMainFramePercent(pagePercent);
        }




        /*
         * 改变mainFrame显示百分比
         * 
         * @param Int32 percent 百分比
         */
        private void changeMainFramePercent(Int32 percent)
        {

            double scale = percent / 100.0;
            mainFrame.RenderTransform = new ScaleTransform(scale, scale);
            App.localStorage.cfg.pagePercent = percent;

            double designScreenWidth = SystemParameters.PrimaryScreenWidth;
            double designScreenHeight = SystemParameters.PrimaryScreenHeight;

            double offsetLeft = (mainFrame.Width - designScreenWidth) / 2;
            double offsetTop = (mainFrame.Height - designScreenHeight) / 2;

            mainFrameScrollViewer.ScrollToHorizontalOffset(offsetLeft);
            mainFrameScrollViewer.ScrollToVerticalOffset(offsetTop);

        }


        /*
         * 关闭软件
         */
        private void winCloseButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (Window win in App.appWindowList)
            {
                win.Close();
                //string typename = win.GetType().Name;
                //if (typename == "PreviewWindow")
                //{
                //    ShowBox.PreviewWindow curr = (ShowBox.PreviewWindow)win;
                //    if (curr.screenCfgId == screenCfgId)
                //    {
                //        return curr.pageTemplate;
                //    }
                //}
                //else if (typename == "MainWindow")
                //{
                //    MainWindow curr = (MainWindow)win;
                //    if (curr.screenCfgId == screenCfgId)
                //    {
                //        return curr.pageTemplate;
                //    }
                //}
            }

            //Window_Unloaded(sender, e);
            //DataContext = null;
            //Close();




        }

        private void mainFrameScrollViewer_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true;
        }



        /*
        * 加载音乐按钮及视频控件
        */
        private void showMusicButton()
        {
            if (!this.screenCfg.isPrimary) return;
            if (!App.localStorage.cfg.backgroundMusicShow) return;
            StorageImage storageImage = storageImageBll.get(App.localStorage.cfg.backgroundMusicButtonImageId);
            string imageFullPath = FileUtil.notExistsShowDefault(storageImage?.url, Params.BackgroundMusicImageNotExists);
            loadBackgroundMusic();

            double scalex = App.localStorage.cfg.pagePercent / 100.0;
            double emptyLeft = App.localStorage.cfg.screenWidth * (1 - scalex) / 2;
            double emptyTop = App.localStorage.cfg.screenHeight * (1 - scalex) / 2;

            double designScreenWidth = SystemParameters.PrimaryScreenWidth;
            double designScreenHeight = SystemParameters.PrimaryScreenHeight;
            double pageWidth = App.localStorage.cfg.screenWidth * scalex;
            double pageHeight = App.localStorage.cfg.screenHeight * scalex;
            double startLeft = (designScreenWidth - pageWidth) / 2;
            double startTop = (designScreenHeight - pageHeight) / 2;

            musicButton = new Button();
            musicButton.Width = App.localStorage.cfg.backgroundMusicButtonWidth * scalex;
            musicButton.Height = App.localStorage.cfg.backgroundMusicButtonHeight * scalex;
            musicButton.HorizontalAlignment = HorizontalAlignment.Left;
            musicButton.VerticalAlignment = VerticalAlignment.Top;
            musicButton.Margin = new Thickness(startLeft + App.localStorage.cfg.backgroundMusicButtonLeft * scalex, startTop + App.localStorage.cfg.backgroundMusicButtonTop * scalex, 0, 0);
            musicButton.BorderThickness = new Thickness(0);
            musicButton.RenderTransformOrigin = new Point(0.5, 0.5);
            musicButton.Tag = false;
            musicButton.UseLayoutRounding = true;
            musicButton.Background = Brushes.Transparent;
            FileUtil.readImage2Button(musicButton, AppDomain.CurrentDomain.BaseDirectory + imageFullPath, App.localStorage.cfg.backgroundMusicButtonWidth, Stretch.Uniform);

            musicButton.Click += musicButtonClick;
            mainGrid.Children.Add(musicButton);
            Panel.SetZIndex(musicButton, 10001);

            initRotate(musicButton);
            if (App.localStorage.cfg.backgroundMusicAutoplay)
            {
                musicPlay();
            }
            Console.WriteLine("musicButton Tag=" + musicButton.Tag.ToString());
        }



        private void musicButtonClick(object sender, RoutedEventArgs e)
        {
            Boolean b = (Boolean)musicButton.Tag;
            Console.WriteLine("musicButtonClick Tag=" + musicButton.Tag.ToString());
            if (b)
            {
                musicPause();
            }
            else
            {
                musicPlay();
            }
        }

        /*
         * 初始化动画 - 图片控件旋转
         */
        public void initRotate(Button musicButton)
        {
            musicStoryboard = new Storyboard();//创建故事板
            DoubleAnimation doubleAnimation = new DoubleAnimation();//实例化一个Double类型的动画
            RotateTransform rotate = new RotateTransform();//旋转转换实例 
            musicButton.RenderTransform = rotate;//给图片空间一个转换的实例
            musicStoryboard.RepeatBehavior = RepeatBehavior.Forever;//设置重复为 一直重复
            musicStoryboard.SpeedRatio = 2;//播放的数度

            doubleAnimation.From = 0;  //设置从0 旋转360度
            doubleAnimation.To = 360;
            doubleAnimation.Duration = new Duration(new TimeSpan(0, 0, 2));//播放时间长度为2秒
            Storyboard.SetTarget(doubleAnimation, musicButton);//给动画指定对象
            Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath("RenderTransform.Angle"));//给动画指定依赖的属性
            musicStoryboard.Children.Add(doubleAnimation);//将动画添加到动画板中 
                                                          // musicStoryboard.Begin(musicButton);//启动动画 
        }

        /*
         * 加载背景音乐控件
         */
        private void loadBackgroundMusic()
        {
            StorageFile storageFile = storageFileBll.get(App.localStorage.cfg.backgroundMusicId);
            musicFilePath = FileUtil.notExistsShowDefault(storageFile?.url, Params.BackgroundMusicNotExists);

            musicMediaElement = new ContentControl();
            musicMediaElement.Name = "BackgroundMusic";
            musicMediaElement.Width = 0;
            musicMediaElement.Height = 0;

            //初始化视频控件 
            string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            DirectoryInfo vlcLibDirectory = new DirectoryInfo(System.IO.Path.Combine(currentDirectory, "libvlc", IntPtr.Size == 4 ? "win-x86" : "win-x64"));

            vlcControl?.Dispose();
            vlcControl = new VlcControl();
            musicMediaElement.Content = vlcControl;
            vlcControl.SourceProvider.CreatePlayer(vlcLibDirectory);
            vlcControl.SourceProvider.MediaPlayer.SetMedia(new Uri(AppDomain.CurrentDomain.BaseDirectory + musicFilePath));
            vlcControl.SourceProvider.MediaPlayer.Position = 0;
            vlcControl.Background = Brushes.Transparent;
            vlcControl.SourceProvider.MediaPlayer.EndReached += Music_Media_Element_MediaEnded;
        }

        private void Music_Media_Element_MediaEnded(object sender, VlcMediaPlayerEndReachedEventArgs e)
        {
            vlcControl.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(Video_Element_Media_Action));
        }

        /*
        *  音乐播放到结尾
        */
        private void Video_Element_Media_Action()
        {
            vlcControl.SourceProvider.MediaPlayer.SetMedia(new Uri(AppDomain.CurrentDomain.BaseDirectory + musicFilePath));
            vlcControl.SourceProvider.MediaPlayer.Position = 0;

            if (App.localStorage.cfg.backgroundMusicLoop)
            {
                // ThreadPool.QueueUserWorkItem(_ => vlcControl.SourceProvider.MediaPlayer.Play());
                musicPlay();
            }
            else
            {
                //ThreadPool.QueueUserWorkItem(_ => vlcControl.SourceProvider.MediaPlayer.Pause());
                //musicStoryboard.Pause(musicButton);
                //musicButton.Tag = false;
                musicPause();
            }
        }



        private void mainFrame_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            if (e.Content is PageTemplate)
            {
                pageTemplate = (PageTemplate)e.Content;
            }

            // MessageBox.Show("跳转"); 
        }


        //当一段时间无操作时，自动执行返回初始界面 
        [StructLayout(LayoutKind.Sequential)]
        struct LASTINPUTINFO
        {
            [MarshalAs(UnmanagedType.U4)]
            public int cbSize;
            [MarshalAs(UnmanagedType.U4)]
            public uint dwTime;
        }
        [DllImport("user32.dll")]
        static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

        static long GetLastInputTime()
        {
            LASTINPUTINFO vLastInputInfo = new LASTINPUTINFO();
            vLastInputInfo.cbSize = Marshal.SizeOf(vLastInputInfo);
            if (!GetLastInputInfo(ref vLastInputInfo))
                return 0;
            return Environment.TickCount - (long)vLastInputInfo.dwTime;
        }

        //10s无操作，返回首页    
        private void BackToHomeTimer_Tick(object sender, EventArgs e)
        {
            long lastinput = GetLastInputTime();
            //Console.WriteLine(lastinput); 
            if (GetLastInputTime() / 1000 == App.localStorage.cfg.noActionTimeBackToHome)    //正好等于10s             
            {
                // Console.WriteLine("60s无操作");
                //判断当前是否首页
                if (pageTemplate != null && pageTemplate.dPage != null && pageTemplate.dPage.id != this.screenCfg.indexPageId)
                {
                    pageTemplate = new PageTemplate(mainFrame, this.screenCfg.indexPageId, false,screen,this.mqServer);
                    mainFrame.Navigate(pageTemplate);
                    //  Console.WriteLine("回到首页");
                }
                //如果浏览器打开了，则关闭
                if (App.localStorage.currForm1 != null && App.localStorage.currForm1.IsDisposed == false)
                {
                    App.localStorage.currForm1.Close();
                    App.localStorage.currForm1.Dispose();
                    App.localStorage.currForm1 = null;
                }
            }
        }

        /*
         * 背景音乐 - 用户点击暂停
         */
        private void musicPause()
        {
            musicPause_ByMessage();
            musicButton.Tag = false;

            Console.WriteLine("musicButton Pause Tag=" + musicButton.Tag.ToString());
            this.mqServer.SendMsg(new VideoControlMessage(0, "pause"));
        }

        /*
         * 背景音乐 - 用户点击播放
         */
        private void musicPlay()
        {
            musicPlay_ByMessage();
            musicButton.Tag = true;
            //  Console.WriteLine("musicButtonPlay Tag=" + musicButton.Tag.ToString());
            this.mqServer.SendMsg(new VideoControlMessage(0, "play"));
        }

        /*
         *  背景音乐 - 暂停 - 接收到消息
         */
        private void musicPause_ByMessage()
        {
            try
            {
                //Console.WriteLine("musicPause_ByMessage:"+ this.vlcControl.SourceProvider.MediaPlayer.IsPlaying());
                if (vlcControl == null) return;
                //当某个视频之前鼠标点击暂停播放，如果再次暂停则进入播放状态。所以加上此判断
                if (!vlcControl.SourceProvider.MediaPlayer.IsPlaying()) return;

                vlcControl.SourceProvider.MediaPlayer.Pause();
                musicStoryboard.Pause(musicButton);

                // Console.WriteLine("musicPause_ByMessage2:" + this.vlcControl.SourceProvider.MediaPlayer.IsPlaying());
            }
            catch (Exception e)
            {
                MessageBox.Show("消息音乐暂停异常：" + e.Message);
                MessageBox.Show(vlcControl.IsInitialized.ToString());
            }
        }

        /*
         * 背景音乐 - 播放 - 接收到消息
         */
        private void musicPlay_ByMessage()
        {
            try
            {
                if (vlcControl == null) return;
                //当某个视频之前鼠标点击暂停播放，如果再次暂停则进入播放状态。所以加上此判断
                if (vlcControl.SourceProvider.MediaPlayer.IsPlaying()) return;

                // Console.WriteLine("musicPlay_ByMessage:" + this.vlcControl.SourceProvider.MediaPlayer.IsPlaying());
                musicStoryboard.Begin(musicButton, true);
                vlcControl.SourceProvider.MediaPlayer.Play();
                // Console.WriteLine("musicPlay_ByMessage2:" + this.vlcControl.SourceProvider.MediaPlayer.IsPlaying());
            }
            catch (Exception e)
            {
                MessageBox.Show("消息音乐播放异常：" + e.Message);
                MessageBox.Show(vlcControl.IsInitialized.ToString());
            }
        }

        /*
         * 客户端消息接收并处理
         */
        private void Client_ReceiveMsgEvent(VideoControlMessage msg)
        {
              Console.WriteLine("musicClient_ReceiveMsgEvent:" + msg.id + "__" + msg.message);
            if (0 == msg.id) return;
            if (!this.screenCfg.isPrimary) return;
            if (!App.localStorage.cfg.backgroundMusicShow) return;
            //1.某个视频切换为播放
            //暂停当前视频
            if (msg.message == "play")
            {
                musicPause_ByMessage();
            }
            //2.某个视频切换为关闭了
            //背景音乐如果是自动播放，则恢复播放
            else if (msg.message == "pause")
            {
                Boolean b = (Boolean)musicButton.Tag;
                if (b)
                {
                    musicPlay_ByMessage();
                }
            }
            //3、打开全屏视频并播放
            //暂停当前视频
            //背景音乐按钮隐藏，并暂停播放
            else if (msg.message == "fullScreenEnter")
            {
               // musicPause_ByMessage();
                musicButton.Visibility = Visibility.Collapsed;
            }
            //4.退出全屏视频
            //背景音乐按钮显示，如果是自动播放，则恢复播放
            else if (msg.message == "fullScreenExit")
            {
                musicButton.Visibility = Visibility.Visible;
            }
        }

        private void mainFrame_Navigating(object sender, System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            if (e.Content is PageTemplate)
            {
                pageTemplate = (PageTemplate)e.Content;
            }
            else
            {
                e.Cancel = true;
                //   MessageBox.Show("mainFrame_Navigating终止");
                return;
            }

        }
    }
}
