using Common.Data;
using Common.MQ;
using Common.util;
using Model;
using Model.dto;
using System;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Vlc.DotNet.Core;
using Vlc.DotNet.Wpf;

namespace Common.control
{
    public partial class CVideo : UserControl
    {
        //暂停按钮图片  
        private readonly BitmapImage pauseImage = new BitmapImage(new Uri("pack://application:,,,/Common;component/Resources/ico_media_stop.png", UriKind.Absolute));
        private readonly BitmapImage playImage = new BitmapImage(new Uri("pack://application:,,,/Common;component/Resources/ico_media_play.png", UriKind.Absolute));
        //当前控件数据
        private DControl currDControl;
        //传递过来的配置类
        private readonly Cfg cfg;
        //视频是否播放结束
        private Boolean isEnded;
        //是否全屏
        private Boolean isFullScreen = false;
        //是否正在播放
        private Boolean isPlaying = false;
        //控制台是否显示
        private Boolean isShowVideoConsole = false;

        //定时器 视频时间轴
        readonly DispatcherTimer timelineSliderTimer = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(100), IsEnabled = true };

        //定时器 自动隐藏控制台
        readonly DispatcherTimer hideVideoConsoleTimer = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(6000), IsEnabled = true };

        private StorageVideoDto storageVideoDto = null;
        //视频控件
        private VlcControl vlcControl;
        //消息服务器
        private readonly MqServer mqServer; 
        //是链接到全屏视频
        private Boolean isLinkToFullVideo = false;  


        public CVideo(DControl ctl, Boolean isDesign, Cfg cfg, StorageVideoDto storageVideoDto, MqServer mqServer,Boolean isLinkToFullVideo)
        :this(  ctl,   isDesign,   cfg,   storageVideoDto,   mqServer)
        {
            this.isLinkToFullVideo = isLinkToFullVideo; 
        }

        public CVideo(DControl ctl, Boolean isDesign, Cfg cfg, StorageVideoDto storageVideoDto, MqServer mqServer)
        {
            currDControl = ctl;
            this.cfg = cfg;
            this.storageVideoDto = storageVideoDto;
            this.mqServer = mqServer;
            InitializeComponent();
            mqServer.sendMsgEvent += Client_ReceiveMsgEvent;
            initSize();

            Video_Element_Init();
            Loaded += UserControl_Loaded;
            Unloaded += UserControl_UnLoaded;
            Play_Button.Click += Play_Button_Click;
            Fullscreen_Button.Click += Fullscreen_Button_Click; 
        }



        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (Video_Element.Content == null) return;



            timelineSliderTimer.Tick += new EventHandler(TimelineSliderTimer_Tick); //超过计时间隔时发生 
            timelineSliderTimer.Start(); //DT启动   

            hideVideoConsoleTimer.Tick += new EventHandler(HideVideoConsoleTimer_Tick); //超过计时间隔时发生 
            hideVideoConsoleTimer.Start(); //DT启动    
        }

        /*
         * 初始化头部，底部控件尺寸
         */
        private void initSize()
        {
            double scaleX = cfg.screenWidth / 1920.0;
            double scaleY = cfg.screenHeight / 1080.0;
            double scale = scaleY;
            if (scaleX < scaleY)
            {
                scale = scaleX;
            }
            if (scale == 1.0) { return; }

            Main_Grid_Row1.Height = new GridLength(50 * scale);
            Main_Grid_Row3.Height = new GridLength(70 * scale);

            Video_Header_Col1.Width = new GridLength(60 * scale);
            Video_Header_Col3.Width = new GridLength(60 * scale);
            Video_Logo.Width = Video_Logo.Width * scale;
            Video_Logo.Height = Video_Logo.Height * scale;
            Video_Title.FontSize = Video_Title.FontSize * scale;
            closebtn.Width = closebtn.Width * scale;
            closebtn.Height = closebtn.Height * scale;

            Video_Console_Col1.Width = new GridLength(80 * scale);
            Video_Console_Col2.Width = new GridLength(60 * scale);
            Video_Console_Col4.Width = new GridLength(60 * scale);
            Video_Console_Col5.Width = new GridLength(80 * scale);
            Play_Button.Width = Play_Button.Width * scale;
            Play_Button.Height = Play_Button.Height * scale;
            playedTime.FontSize = playedTime.FontSize * scale;
            totalTime.FontSize = totalTime.FontSize * scale;
            Fullscreen_Button.Width = Fullscreen_Button.Width * scale;
            Fullscreen_Button.Height = Fullscreen_Button.Height * scale;

            TimelineSlider.Height = TimelineSlider.Height * scale;
            double mleft = TimelineSlider.Margin.Left * scale;
            TimelineSlider.Margin = new Thickness(mleft);


            Style slider_Line = (Style)FindResource("Slider_Line");
            foreach (Setter setter in slider_Line.Setters)
            {
                if (setter.Property.Name == "Height")
                {
                    Int32 h = int.Parse(setter.Value.ToString());
                    setter.Value = h * scale;
                }
            }

            Style slider_Line_Active = (Style)FindResource("Slider_Line_Active");
            foreach (Setter setter in slider_Line_Active.Setters)
            {
                if (setter.Property.Name == "Height")
                {
                    Int32 h = int.Parse(setter.Value.ToString());
                    setter.Value = h * scale;
                }
            }

            Style slider_Thumb = (Style)FindResource("Slider_Thumb");
            foreach (Setter setter in slider_Thumb.Setters)
            {
                if (setter.Property.Name == "Width")
                {
                    Int32 w = int.Parse(setter.Value.ToString());
                    setter.Value = w * scale;
                }
                if (setter.Property.Name == "Height")
                {
                    Int32 h = int.Parse(setter.Value.ToString());
                    setter.Value = h * scale;
                }
            }



            Style slider1 = (Style)FindResource("Slider1");
            TimelineSlider.Style = slider1;
        }

        private void UserControl_UnLoaded(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("CVideo：UserControl_UnLoaded");
            if (isFullScreen)
            {
                mqServer.SendMsg(new VideoControlMessage(currDControl.id, "fullScreenExit"));
            }
            if (isPlaying)
            {
                Video_Pause();
            }
            else
            {
                Video_Pause_ByMessage();
            }
            mqServer.sendMsgEvent -= Client_ReceiveMsgEvent;
            timelineSliderTimer.Tick -= new EventHandler(TimelineSliderTimer_Tick);
            timelineSliderTimer.Stop();
            hideVideoConsoleTimer.Tick -= new EventHandler(HideVideoConsoleTimer_Tick);
            hideVideoConsoleTimer.Stop();
            vlcControl.SourceProvider.MediaPlayer.EndReached -= Video_Element_MediaEnded;

            Video_Element_Grid.PreviewMouseLeftButtonDown -= Video_Element_Grid_MouseLeftButtonDown;
            Video_Element_Grid.PreviewMouseMove -= Video_Element_Grid_MouseMove;
            Video_Element_Grid.PreviewMouseLeftButtonUp -= TimelineSlider_MouseLeftButtonUp;
            Video_Console.PreviewMouseLeftButtonUp -= Video_Console_MouseUp;

            TimelineSlider.PreviewMouseLeftButtonDown -= TimelineSlider_MouseDown;
            TimelineSlider.PreviewMouseLeftButtonUp -= TimelineSlider_MouseUp;
            TimelineSlider.PreviewTouchDown -= TimelineSlider_TouchDown;
            TimelineSlider.PreviewTouchUp -= TimelineSlider_TouchUp;


            Loaded -= UserControl_Loaded;
            Unloaded -= UserControl_UnLoaded;
            Play_Button.Click -= Play_Button_Click;
            Fullscreen_Button.Click -= Fullscreen_Button_Click; 
            

            vlcControl.Background = null;
            vlcControl?.Dispose();
            Video_Element.Content = null;

            //logo  关闭按钮  播放按钮  全屏按钮 时间线
            Video_Logo.Source = null;
            closebtn.Background = null;
            Play_Button.Background = null;
            Fullscreen_Button.Background = null;

            Video_Title = null;
            playedTime = null;
            totalTime = null;
            Main_Grid.Children.Clear();
            this.Resources.Clear(); 
            GC.Collect(); 
        }



        /*
         * 1.页面加载首次播放
         */
        private void Video_Element_Init()
        {
            string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            DirectoryInfo vlcLibDirectory = new DirectoryInfo(System.IO.Path.Combine(currentDirectory, "libvlc", IntPtr.Size == 4 ? "win-x86" : "win-x64"));


            string videoFullPath = FileUtil.notExistsShowDefault(storageVideoDto?.url, Params.VideoNotExists);
            videoFullPath = AppDomain.CurrentDomain.BaseDirectory + videoFullPath;

            string videoImageFullPath = FileUtil.notExistsShowDefault(storageVideoDto?.storageImageUrl, Params.VideoImageNotExists);
            videoImageFullPath = AppDomain.CurrentDomain.BaseDirectory + videoImageFullPath;

            //初始化视频控件及按钮及标题   
            isEnded = false;


            this.vlcControl?.Dispose();
            vlcControl = new VlcControl(); 
            Video_Element.Content = vlcControl;
            vlcControl.SourceProvider.CreatePlayer(vlcLibDirectory);
            vlcControl.SourceProvider.MediaPlayer.SetMedia(new Uri(videoFullPath));
            vlcControl.SourceProvider.MediaPlayer.Position = 0;
           
            vlcControl.Background = new ImageBrush
            {
                ImageSource = FileUtil.readImage2(videoImageFullPath, currDControl.width)
                ,
                Stretch = Stretch.Uniform
            };  //初始化暂停图片
            //vlcControl.Background = Brushes.Transparent;
            //FileUtil.readImage2VlcControl(vlcControl, videoImageFullPath, currDControl.width, Stretch.Uniform);
            vlcControl.SourceProvider.MediaPlayer.EndReached += Video_Element_MediaEnded;


            Video_Title.Text = FileUtil.getFilenameTitle(storageVideoDto.origFilename);
            if (currDControl.autoplay || this.isLinkToFullVideo)
            {
                Video_Play();
            }
            else
            {
                 Video_Pause_ByMessage();  
            } 

            if (currDControl.width == cfg.screenWidth && currDControl.height == cfg.screenHeight)
            { 
                fullScreenEnter();
            }

            Video_Element_Grid.PreviewMouseLeftButtonDown += Video_Element_Grid_MouseLeftButtonDown;
            Video_Element_Grid.PreviewMouseMove += Video_Element_Grid_MouseMove;
            Video_Element_Grid.PreviewMouseLeftButtonUp += TimelineSlider_MouseLeftButtonUp;

            Video_Console.PreviewMouseLeftButtonUp += Video_Console_MouseUp;

            TimelineSlider.PreviewMouseLeftButtonDown += TimelineSlider_MouseDown;
            TimelineSlider.PreviewMouseLeftButtonUp += TimelineSlider_MouseUp;
            TimelineSlider.PreviewTouchDown += TimelineSlider_TouchDown;
            TimelineSlider.PreviewTouchUp += TimelineSlider_TouchUp;

            
            

           

        }




        //滑动 视频前进后退
        private Boolean moveSliderFlag = false;
        //是否双击视频
        private Boolean isDoubleClick = false;
        private void TimelineSlider_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (moveSliderFlag)
            { 
                //Video_Element.Position = new TimeSpan(0, 0, 0, 0, (int)TimelineSlider.Value);
                //long totalTimeStamp = GetTotalTimeStamp();
                //long currTime = (long)(TimelineSlider.Value * totalTimeStamp / TimelineSlider.Maximum);
                //this.vlcControl.SourceProvider.MediaPlayer.Time = currTime;   
                timelineSliderTimer.Start();
                Video_Play();
                moveSliderFlag = false;
                return;
            }
            if (isDoubleClick)
            {
                isDoubleClick = false;
                return;
            }
            if (isShowVideoConsole)
            {
                HideVideoConsole();
            }
            else
            {
                ShowVideoConsole();
            }

        }

        private Point sliderPosition = new Point(0, 0);

        private void Video_Element_Grid_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            { 
                moveSliderFlag = true;
                timelineSliderTimer.Stop();
                System.Windows.Point position = e.GetPosition(TimelineSlider);

                //时间轴变化
                double d = 1.0d / TimelineSlider.ActualWidth * (position.X - sliderPosition.X);
                sliderPosition = position;
                double p = TimelineSlider.Maximum * d + TimelineSlider.Value;


                //视频位置变化
                long totalTimeStamp = GetTotalTimeStamp();
                long currTime = (long)(p * totalTimeStamp / TimelineSlider.Maximum);
                vlcControl.SourceProvider.MediaPlayer.Time = currTime;

                TimelineSlider.Value = p;
                Console.WriteLine("move：" + currTime.ToString() + "__" + totalTimeStamp);
               // Console.WriteLine("move："+currTime.ToString()+"__"+ TimelineSlider.ActualWidth+"__"+ TimelineSlider.Maximum+"___"+(position.X - sliderPosition.X));
            }
        }



        /*
        * 点击切换播放状态
        */
        private void Play_Button_Click(object sender, RoutedEventArgs e)
        {
            currDControl.isHideVideoConsoleOfFirstLoad = false;
            togglePlayStatus(); 
        }
        /*
         * 视频播放
         */
        private void Video_Play()
        {
            Video_Play_ByMessage();
            //暂停所有视频  
            mqServer.SendMsg(new VideoControlMessage(currDControl.id, "play"));
        }
        /*
         * 视频暂停
         */
        private void Video_Pause()
        {
            Video_Pause_ByMessage(); 
            mqServer.SendMsg(new VideoControlMessage(currDControl.id, "pause"));
        }

        /*
        * 其他视频播放，暂停当前视频，来自消息
        */
        private void Video_Pause_ByMessage()
        {
            try
            {
                 Console.WriteLine("Video_Pause_ByMessage:"+currDControl.id );
                if (vlcControl == null) return;
                //当某个视频之前鼠标点击暂停播放，如果再次暂停则进入播放状态。所以加上此判断 
                //if (!isPlaying) return;
                vlcControl.SourceProvider.MediaPlayer.SetPause(true);
                //  Console.WriteLine("Video_Pause_ByMessage:" + currDControl.id + "__IsPausable2:" + this.vlcControl.SourceProvider.MediaPlayer.IsPausable() + "__IsPlaying2" + this.vlcControl.SourceProvider.MediaPlayer.IsPlaying());
                Play_Button.Background = new ImageBrush
                {
                    ImageSource = playImage
                };
                isPlaying = false; 
                //显示控制行 
                ShowVideoConsole(); 
            }
            catch (Exception e)
            {
                MessageBox.Show("消息视频暂停异常：" + e.Message);
                MessageBox.Show(vlcControl.IsInitialized.ToString());
            }
        }

        /*
         * 其他视频播放，暂停当前视频，来自消息
         */
        private void Video_Play_ByMessage()
        {
            try
            { 
                Console.WriteLine("Video_Play_ByMessage:" + currDControl.id); 
                // Console.WriteLine("Video_Play_ByMessage:"+currDControl.id+ "__IsPausable:" + this.vlcControl.SourceProvider.MediaPlayer.IsPausable()+"__IsPlaying"+this.vlcControl.SourceProvider.MediaPlayer.IsPlaying());
                if (vlcControl == null) return;
                //当某个视频之前鼠标点击暂停播放，如果再次暂停则进入播放状态。所以加上此判断 
                // if (isPlaying) return;

                vlcControl.SourceProvider.MediaPlayer.Play();
                vlcControl.Background = Brushes.Transparent;
                Play_Button.Background = new ImageBrush
                {
                    ImageSource = pauseImage
                }; 
                isPlaying = true;
                isEnded = false; 
                //显示控制行  
                ShowVideoConsole(); 
            }
            catch (Exception e)
            {
                MessageBox.Show("消息视频播放异常：" + e.Message);
                MessageBox.Show(vlcControl.IsInitialized.ToString());
            }
        }


        /*
         * 视频播放结束
         */
        private void Video_Element_MediaEnded(object sender, VlcMediaPlayerEndReachedEventArgs e)
        { 
            vlcControl.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(Video_Element_Media_Action));
        }
        private void Video_Element_Media_Action()
        {
            Video_Pause();
            isPlaying = false;
            isEnded = true;
            if (currDControl.loop)
            {
                Thread.Sleep(1500);
                playNextVideo();
            }
            else
            {
                string videoFullPath = FileUtil.notExistsShowDefault(storageVideoDto?.url, Params.VideoNotExists);
                videoFullPath = AppDomain.CurrentDomain.BaseDirectory + videoFullPath;
                vlcControl.SourceProvider.MediaPlayer.SetMedia(new Uri(videoFullPath));

                vlcControl.SourceProvider.MediaPlayer.Position = 0;
                TimelineSlider.Value = 0; 
            }
        }

        /*
         * 播放下一个视频
         */
        private void playNextVideo()
        { 
            string videoFullPath = FileUtil.notExistsShowDefault(storageVideoDto?.url, Params.VideoNotExists);
            videoFullPath = AppDomain.CurrentDomain.BaseDirectory + videoFullPath; 

            vlcControl.SourceProvider.MediaPlayer.SetMedia(new Uri(videoFullPath));
            Video_Title.Text = FileUtil.getFilenameTitle(storageVideoDto.origFilename);
            vlcControl.SourceProvider.MediaPlayer.Position = 0;
            TimelineSlider.Value = 0;

            vlcControl.Background = Brushes.Gray;  //初始化暂停图片
            ////  control.SourceProvider.MediaPlayer.EndReached += Video_Element_MediaEnded;  
            Video_Play();
        }



        /*
        * 拖动时间轴，按下
        */
        private void TimelineSlider_MouseDown(object sender, EventArgs e)
        {
            timelineSliderTimer.Stop();
        }
        private void TimelineSlider_MouseUp(object sender, EventArgs e)
        { 
            long totalTimeStamp = GetTotalTimeStamp();
            long currTime = (long)(TimelineSlider.Value * totalTimeStamp / TimelineSlider.Maximum);
            vlcControl.SourceProvider.MediaPlayer.Time = currTime;

            timelineSliderTimer.Start();
            Video_Play();
        }

        private void TimelineSlider_TouchDown(object sender, TouchEventArgs e)
        {
            timelineSliderTimer.Stop();
        }
        private void TimelineSlider_TouchUp(object sender, TouchEventArgs e)
        {

            long totalTimeStamp = GetTotalTimeStamp();
            long currTime = (long)(TimelineSlider.Value * totalTimeStamp / TimelineSlider.Maximum);
            vlcControl.SourceProvider.MediaPlayer.Time = currTime;

            timelineSliderTimer.Start();
            Video_Play();
        }

        /*
         * 点击全屏播放0
         */
        private void Fullscreen_Button_Click(object sender, RoutedEventArgs e)
        {
            currDControl.isHideVideoConsoleOfFirstLoad = false;
            Button fullscreenbtn = (Button)sender;
            if (isFullScreen)
            {
                fullScreenExit();
            }
            else
            {
                fullScreenEnter();
                Video_Play();
            }
        }

        /*
         * 进入全屏,并播放
         */
        private void fullScreenEnter()
        {
            isFullScreen = true;
            double screenWidth = cfg.screenWidth;   //得到屏幕整体宽度
            double screenHeight = cfg.screenHeight;  //得到屏幕整体高度 

            Margin = new Thickness(0, 0, 0, 0);
            Width = screenWidth;
            Height = screenHeight;
            Panel.SetZIndex(this, 10003);
            mqServer.SendMsg(new VideoControlMessage(currDControl.id, "fullScreenEnter"));
            
        }
        /*
         * 退出全屏
         */
        private void fullScreenExit()
        {
            isFullScreen = false;
            Margin = new Thickness(currDControl.left, currDControl.top, 0, 0);
            Width = currDControl.width;
            Height = currDControl.height;

            Panel.SetZIndex(this, currDControl.idx); 
            mqServer.SendMsg(new VideoControlMessage(currDControl.id, "fullScreenExit"));
        }

        /*
         * 视频时间轴 - 定时处理类
         */
        private void TimelineSliderTimer_Tick(object sender, EventArgs e)
        {
            if (!vlcControl.SourceProvider.MediaPlayer.IsPlaying()) return;

            long currentTimeStamp = GetCurrentTimeStamp();
            long totalTimeStamp = GetTotalTimeStamp();
            string playedTimeStr = ToHHmmss(currentTimeStamp);
            string totalTimeStr = ToHHmmss(totalTimeStamp);
            playedTime.Content = playedTimeStr;
            totalTime.Content = totalTimeStr;

            TimelineSlider.ToolTip = playedTimeStr;
            TimelineSlider.Value = currentTimeStamp * TimelineSlider.Maximum / totalTimeStamp;
            ScreenSaverUtil.NoSleep();
        }


        /*
         * 二、显示控制台
         */
        private void ShowVideoConsole()
        {
            if (currDControl.isHideVideoConsoleOfFirstLoad) return;
            Video_Console.Visibility = Visibility.Visible;
            Video_Header.Visibility = Visibility.Visible;
            isShowVideoConsole = true;
            hideVideoConsoleTimer.Stop();
            hideVideoConsoleTimer.Start(); 
        }
        /*
        * 隐藏控制台
        */
        private void HideVideoConsole()
        {
            Video_Console.Visibility = Visibility.Hidden;
            Video_Header.Visibility = Visibility.Hidden;
            isShowVideoConsole = false;
            hideVideoConsoleTimer.Stop();
        }

        /*
         * 切换播放状态
         */
        private void togglePlayStatus()
        {
            if (isPlaying)
            {
                Video_Pause();
            }
            else
            {
                if (isEnded)
                {
                    vlcControl.SourceProvider.MediaPlayer.Time = 0;
                }
                Video_Play();
            }
        }

        /*
         * 双击-切换播放状态
         * 
         * 单击视频 - 切换控制台状态
         */
        private void Video_Element_Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            currDControl.isHideVideoConsoleOfFirstLoad = false; 
            //双击切换视频状态
            if (e.ClickCount == 2)
            {
                isDoubleClick = true;
                togglePlayStatus();
                return;
            } 
            sliderPosition = e.GetPosition(TimelineSlider);
        }


        private void Video_Element_Grid_TouchUp(object sender, TouchEventArgs e)
        {

            if (isShowVideoConsole)
            {
                HideVideoConsole();
            }
            else
            {
                ShowVideoConsole();
            }
            e.Handled = true;
        }

        /*
         * 定时执行 隐藏控制台
         */
        private void HideVideoConsoleTimer_Tick(object sender, EventArgs e)
        {
            if (isShowVideoConsole && isPlaying)
            {
                HideVideoConsole();
            }
        }

        /*
         * 操作控制台时，定时器从头开始 
         */
        private void Video_Console_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ShowVideoConsole();
        }
        private void Video_Console_TouchUp(object sender, TouchEventArgs e)
        {
            ShowVideoConsole();
        }

        /*
         * 鼠标在控件上移动，始终显示控制台
         */
        private void this_MouseMove(object sender, MouseEventArgs e)
        {
            ShowVideoConsole();
        }

        /*
         * 四、编辑控件后 - 更新到页面
         */

        public void updateElement(DControl ctl, bool isDesign, StorageVideoDto storageVideoDto)
        {
            timelineSliderTimer.Stop();
            Video_Pause();
            currDControl = ctl;
            this.storageVideoDto = storageVideoDto;
            Video_Element_Init();
        }

        /*
         * 编辑属性后 - 更新页面属性 
         */
        public void updateElementAttr(DControl ctl, bool p)
        {
            Width = ctl.width;
            Height = ctl.height;
            Margin = new Thickness(ctl.left, ctl.top, 0, 0);
            Opacity = ctl.opacity / 100.0;
           // Video_Title.Text = ctl.content;
            currDControl = ctl;
            if (ctl.autoplay)
            {
                Video_Play();
            }
            else
            {
                Video_Pause();
            }
            if (ctl.loop && isEnded)
            {
                vlcControl.SourceProvider.MediaPlayer.Time = 0;
                Video_Play();
            }
            if (ctl.isHideVideoConsoleOfFirstLoad)
            {
                HideVideoConsole();
            }
            else
            {
                ShowVideoConsole();
            }
        }

        /*
        * 获取控件的关闭按钮，并显示
        */
        public Button GetClosebtn()
        {
            return closebtn;
        }

        /*
         * 转为时分秒
         * 
         * @param int s 秒
         */
        private string ToHHmmss(long ms)
        {
            int s = (int)(ms / 1000);

            int min = s / 60;
            s = s - min * 60;
            int hour = min / 60;
            min = min - hour * 60;

            string mm = ((min < 10) ? "0" : "") + min.ToString();
            string ss = ((s < 10) ? "0" : "") + s.ToString();

            string time = "";
            if (hour > 0)
            {
                time = time + hour + ":";
            }
            time = time + mm + ":" + ss;
            return time;
        }

        /*
         * 获取当前已播放时长
         */
        private long GetCurrentTimeStamp()
        {
            if (vlcControl == null)
            {
                return 0;
            }

            return vlcControl.SourceProvider.MediaPlayer.Time;
        }

        /*
       * 获取视频总时长
       */
        private long GetTotalTimeStamp()
        {
            if (vlcControl == null)
            {
                return 0;
            }  
           // return Convert.ToInt64( vlcControl.SourceProvider.MediaPlayer.GetMedia().Duration.TotalMilliseconds);
            return vlcControl.SourceProvider.MediaPlayer.Length;
        }

        /*
        * 客户端消息接收并处理
        */
        private void Client_ReceiveMsgEvent(VideoControlMessage msg)
        {
           // Console.WriteLine(currDControl.id + "_" + msg.id + ":" + msg.message);
            if (currDControl.id == msg.id) return;
            //1.某个视频切换为播放
            //暂停当前视频
            if (msg.message == "play")
            {
                Video_Pause_ByMessage(); 
            }
            //2.某个视频切换为关闭了
            //背景音乐如果是自动播放，则恢复播放
            else if (msg.message == "pause")
            {

            }
            //3、打开全屏视频，
            //暂停当前视频
            //背景音乐按钮隐藏，并暂停播放
            else if (msg.message == "fullScreenEnter")
            {
                //Video_Pause_ByMessage();
            }
            //4.关闭全屏视频，背景音乐按钮显示，如果是自动播放，则恢复播放
            else if (msg.message == "fullScreenExit")
            {

            }
        }

    }
}
