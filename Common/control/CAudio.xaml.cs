using Common.Data;
using Common.MQ;
using Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Vlc.DotNet.Core;
using Vlc.DotNet.Wpf;

namespace Common.control
{
    /// <summary>
    /// CAudio.xaml 的交互逻辑
    /// </summary>
    public partial class CAudio : UserControl
    {
        //定时器 视频时间轴
        private readonly DispatcherTimer dispatcherTimer = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(400), IsEnabled = true };

        private DControl currDControl = null;
        private List<BitmapImage> listBitmap = new List<BitmapImage>();
        private int idx = 2;
        //视频控件 
        private VlcControl vlcControl;
        //是否正在播放
        private Boolean isPlaying = false;
        //音频地址
        private string audioUrl;
        //音频封面
        private string audioCoverUrl;
        private BitmapImage audioCoverBitmapImage = null;
        //消息服务器
        private readonly MqServer mqServer;
       


        public CAudio(DControl currDControl, DPage dPage, Cfg appCfg, string audioUrl,string audioCoverUrl, MqServer mqServer)
        {
            this.currDControl = currDControl;
            this.mqServer = mqServer;
            this.audioUrl = audioUrl;
            this.audioCoverUrl = audioCoverUrl;
            InitializeComponent();
            mqServer.sendMsgEvent += Client_ReceiveMsgEvent;
            loadPageData();
            Unloaded += this_Unloaded;
        }

        private void this_Unloaded(object sender, RoutedEventArgs e)
        { 
            if (isPlaying)
            {
                Video_Pause();
            }
            else
            {
                Video_Pause_ByMessage();
            }
            mqServer.sendMsgEvent -= Client_ReceiveMsgEvent;
            dispatcherTimer.Tick -= new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Stop();
            vlcControl.SourceProvider.MediaPlayer.EndReached -= Video_Element_MediaEnded; 
            Unloaded -= this_Unloaded;

            image.Source = null; 
            vlcControl?.Dispose();
            Video_Element.Content = null;

            mainGrid.Children.Clear();
            this.Resources.Clear(); 
            GC.Collect(); 
        }

        /*
         * 加载页面数据
         */
        private void loadPageData()
        {
            listBitmap = Params.cAudioImageList;

            //默认显示封面
            audioCoverUrl = FileUtil.notExistsShowDefault(audioCoverUrl, Params.CAudioImageNotExists);
            this.audioCoverBitmapImage = FileUtil.readImage(AppDomain.CurrentDomain.BaseDirectory + audioCoverUrl);
 
             
            changeImage(audioCoverBitmapImage);
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick); //超过计时间隔时发生  
            dispatcherTimer.Stop();
            Video_Element_Init();
        }

        /*
         * 点击切换播放/暂停
         */
        public void togglePlay()
        {
            if (vlcControl.SourceProvider.MediaPlayer.IsPlaying())
            {
                Video_Pause();
            }
            else
            {
                Video_Play();
            }
        }

        /*
         * 1.页面加载首次播放
         */
        private void Video_Element_Init()
        {
            string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            DirectoryInfo vlcLibDirectory = new DirectoryInfo(System.IO.Path.Combine(currentDirectory, "libvlc", IntPtr.Size == 4 ? "win-x86" : "win-x64"));

            audioUrl = FileUtil.notExistsShowDefault(audioUrl, Params.CAudioNotExists);
            string videoFullPath = AppDomain.CurrentDomain.BaseDirectory + audioUrl;
            FileInfo fileInfo = new FileInfo(videoFullPath);

            vlcControl?.Dispose();
            vlcControl = new VlcControl();
            Video_Element.Content = vlcControl;
            vlcControl.SourceProvider.CreatePlayer(vlcLibDirectory);

            string[] options = new string[] { "input-repeat=0" };//avcodec-hw=any   input-repeat=10
            if (currDControl.loop)
            {
                options = new string[] { "input-repeat=65535" };//avcodec-hw=any   input-repeat=10
            }
            vlcControl.SourceProvider.MediaPlayer.SetMedia(fileInfo, options);
            vlcControl.SourceProvider.MediaPlayer.Position = 0;
            vlcControl.Background = System.Windows.Media.Brushes.Transparent;
            vlcControl.SourceProvider.MediaPlayer.EndReached += Video_Element_MediaEnded;
            if (currDControl.autoplay)
            {
                Video_Play();
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
            if (!currDControl.loop)
            {
                Video_Pause();

                string videoFullPath = AppDomain.CurrentDomain.BaseDirectory + audioUrl;
                FileInfo fileInfo = new FileInfo(videoFullPath);
                vlcControl.SourceProvider.MediaPlayer.SetMedia(fileInfo);
                vlcControl.SourceProvider.MediaPlayer.Position = 0;
            }
        }
        /*
         * 三张图片定时切换
         */
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            idx = idx + 1;
            if (idx >= listBitmap.Count)
            {
                idx = 0;
            }
            changeImage(listBitmap[idx]);
        }
        private void changeImage(BitmapImage toImage)
        {
            image.Source = toImage;
        }

        public void updateElement(DControl dControl, bool isDesign, string audioUrl)
        {
            dispatcherTimer.Stop();
            Video_Pause();
            currDControl = dControl;
            this.audioUrl = audioUrl;
            Video_Element_Init();
        }
        /*
        * 编辑属性后 - 更新页面属性  
        */
        public void updateElementAttr(DControl ctl,string audioCoverUrl, bool isDesign)
        {
            Width = ctl.width;
            Height = ctl.height;
            Margin = new Thickness(ctl.left, ctl.top, 0, 0);
            Opacity = ctl.opacity / 100.0;
            //默认显示封面
            audioCoverUrl = FileUtil.notExistsShowDefault(audioCoverUrl, Params.CAudioImageNotExists);
            this.audioCoverBitmapImage = FileUtil.readImage(AppDomain.CurrentDomain.BaseDirectory + audioCoverUrl);
        

            currDControl = ctl;
            if (ctl.autoplay)
            {
                Video_Play();
            }
            else
            {
                Video_Pause();
            }
        }
        /*
       * 客户端消息接收并处理
       */
        private void Client_ReceiveMsgEvent(VideoControlMessage msg)
        { 
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
               // Video_Pause_ByMessage();
            }
            //4.关闭全屏视频，背景音乐按钮显示，如果是自动播放，则恢复播放
            else if (msg.message == "fullScreenExit")
            {

            }
        }


        /*
        * 音频播放
        */
        private void Video_Play()
        {
            Video_Play_ByMessage();
            //暂停所有视频  
            mqServer.SendMsg(new VideoControlMessage(currDControl.id, "play"));
        }
        /*
         * 音频暂停
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
                if (vlcControl == null) return;
                //当某个视频之前鼠标点击暂停播放，如果再次暂停则进入播放状态。所以加上此判断 
                if (!isPlaying) return;
                dispatcherTimer.Stop(); 
                changeImage(this.audioCoverBitmapImage);
                vlcControl.SourceProvider.MediaPlayer.SetPause(true);
                isPlaying = false;
            }
            catch (Exception e)
            {
                MessageBox.Show("消息音频暂停异常：" + e.Message);
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
                if (vlcControl == null) return;
                //当某个视频之前鼠标点击暂停播放，如果再次暂停则进入播放状态。所以加上此判断 
                if (isPlaying) return;
                dispatcherTimer.Start();
                vlcControl.SourceProvider.MediaPlayer.Play();
                isPlaying = true;
            }
            catch (Exception e)
            {
                MessageBox.Show("消息音频播放异常：" + e.Message);
                MessageBox.Show(vlcControl.IsInitialized.ToString());
            }
        }
    }
}
