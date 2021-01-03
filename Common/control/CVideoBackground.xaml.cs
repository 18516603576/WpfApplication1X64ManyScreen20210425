using Common.Data;
using Model.dto;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Vlc.DotNet.Core;
using Vlc.DotNet.Wpf;

namespace Common.control
{
    public partial class CVideoBackground : UserControl
    {
        //视频信息 
        private readonly StorageVideoDto storageVideoDto = null;
        //视频控件
        private VlcControl vlcControl;
        //视频地址
        private FileInfo fileInfo;


        public CVideoBackground(StorageVideoDto storageVideoDto, Boolean isDesign)
        {
            this.storageVideoDto = storageVideoDto;
            InitializeComponent();

            Video_Element_Init();
            Unloaded += UserControl_UnLoaded; 
        }
 
      

        private void UserControl_UnLoaded(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("CVideoBackground:UserControl_UnLoaded"); 
            vlcControl.SourceProvider.MediaPlayer.EndReached -= Video_Element_MediaEnded;
            this.Unloaded -= UserControl_UnLoaded;
            fileInfo = null;

            Video_Pause( );  
            vlcControl.Background = null;
            vlcControl?.Dispose();
            vlcControl = null;
            Video_Element.Content = null;
            Video_Element = null;

            Video_Element_Grid.Children.Clear();
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
            fileInfo = new FileInfo(videoFullPath);

            vlcControl?.Dispose();
            vlcControl = new VlcControl();
            Video_Element.Content = vlcControl; 
            vlcControl.SourceProvider.CreatePlayer(vlcLibDirectory);

            string[] options = new string[] { "input-repeat=65535"  };//avcodec-hw=any   input-repeat=10
            vlcControl.SourceProvider.MediaPlayer.SetMedia(fileInfo, options);
            vlcControl.SourceProvider.MediaPlayer.Position = 5;
           
            Video_Play( );

            //FileInfo shotFile = new FileInfo(@"D:\shot\1.jpg"); 
            //bool b = vlcControl.SourceProvider.MediaPlayer.TakeSnapshot(shotFile, 1920, 1080);
            //Console.WriteLine("截图结果：" + b);
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
            vlcControl.SourceProvider.MediaPlayer.SetMedia(fileInfo);
            vlcControl.SourceProvider.MediaPlayer.Position = 0;
            this.Video_Play( );
        }

        /*
         * 视频播放
         */
        private void Video_Play( )
        { 
            this.vlcControl.SourceProvider.MediaPlayer.Play();
        }
        /*
         * 视频暂停 
         */
        private void Video_Pause( )
        {
            this.vlcControl.SourceProvider.MediaPlayer.Pause();
        }


    }
}
