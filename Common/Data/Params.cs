using Model;
using System;
using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace Common.Data
{
    public class Params
    {
        public static string salt1 = "jioew43jt43lgdfL4fdIDKEJ_Y";
        public static string salt2 = "dsgf904IHJMKED_dfsfedfgfdECVf";
        //图片存在时
        public static string ImageNotExists = "/myfile/sysimg/notExists/image.png";
        //Gif不存在时
        public static string GifNotExists = "/myfile/sysimg/notExists/gif.gif";
        //Flash不存在时
        public static string FlashNotExists = "/myfile/sysimg/notExists/flash.swf";
        //word文件不存在时
        public static string WordNotExists = "/myfile/sysimg/notExists/word.xps";
        //视频不存在时
        public static string VideoNotExists = "/myfile/sysimg/notExists/video.mp4";
        public static string VideoImageNotExists = "/myfile/sysimg/notExists/video.jpg";
        //音乐不存在时
        public static string BackgroundMusicNotExists = "/myfile/sysimg/notExists/music.mp3";
        //音乐图标不存在时
        public static string BackgroundMusicImageNotExists = "/myfile/sysimg/notExists/music-image.png";
        //音频文件不存在时
        public static string CAudioNotExists = "/myfile/sysimg/notExists/caudio.mp3";
        //音频文件不存在时
        public static string CAudioImageNotExists = "/myfile/sysimg/notExists/caudio-image.png";

        //返回按钮不存在时
        public static string BackButtonNotExists = "/myfile/sysimg/notExists/back-button.png";
        //首页按钮不存在时
        public static string HomeButtonNotExists = "/myfile/sysimg/notExists/home-button.png";
        //相册中图片不存在时
        public static string[] TurnPictureNotExists = new string[] {
            "/myfile/sysimg/notExists/turnPicture-0.jpg"
            ,"/myfile/sysimg/notExists/turnPicture-1.jpg"
            ,"/myfile/sysimg/notExists/turnPicture-2.jpg"
            ,"/myfile/sysimg/notExists/turnPicture-3.jpg"
            ,"/myfile/sysimg/notExists/turnPicture-4.jpg"
            ,"/myfile/sysimg/notExists/turnPicture-5.jpg"
        };
        public static List<CFrameTag> CFrameList = new List<CFrameTag> { };


        //弹窗默认背景
        public static string CFrameDialogDefaultBackground = "/myfile/sysimg/notExists/cFrameDialogDefaultBackground.png";

        //截图的图片
        public static string shotImage = "shot.png";

        //上传图片按钮
        public static string Ico_Add_Image = "/myfile/sysimg/ico-add-image.png";
        public static string Ico_Add_Video = "/myfile/sysimg/ico-add-video.png";


        //进入动画列表
        public static List<DControlAnimation> animationInList = new List<DControlAnimation> {
               new DControlAnimation{ name="淡入",type=1001, delaySeconds=0, durationSeconds=1000, playTimes=1 ,isSameSpeed=false ,isSameOpacity=false }
              ,new DControlAnimation{ name="从左移入",type=1002, delaySeconds=0, durationSeconds=1000, playTimes=1 ,isSameSpeed=false ,isSameOpacity=false }
              ,new DControlAnimation{ name="从右移入",type=1003, delaySeconds=0, durationSeconds=1000, playTimes=1 ,isSameSpeed=false ,isSameOpacity=false  }
              ,new DControlAnimation{ name="从上移入",type=1004, delaySeconds=0, durationSeconds=1000, playTimes=1 ,isSameSpeed=false ,isSameOpacity=false  }
              ,new DControlAnimation{ name="从下移入",type=1005, delaySeconds=0, durationSeconds=1000, playTimes=1 ,isSameSpeed=false ,isSameOpacity=false  }
              ,new DControlAnimation{ name="放大",type=1006, delaySeconds=0, durationSeconds=1000, playTimes=1 ,isSameSpeed=false ,isSameOpacity=false  }
              ,new DControlAnimation{ name="缩小",type=1007, delaySeconds=0, durationSeconds=1000, playTimes=1 ,isSameSpeed=false ,isSameOpacity=false  }

              ,new DControlAnimation{ name="从左翻入",type=1101, delaySeconds=0, durationSeconds=1000, playTimes=1 ,isSameSpeed=false ,isSameOpacity=false  }
              ,new DControlAnimation{ name="从右翻入",type=1102, delaySeconds=0, durationSeconds=1000, playTimes=1 ,isSameSpeed=false ,isSameOpacity=false  }
              ,new DControlAnimation{ name="从上翻入",type=1103, delaySeconds=0, durationSeconds=1000, playTimes=1 ,isSameSpeed=false ,isSameOpacity=false  }
              ,new DControlAnimation{ name="从下翻入",type=1104, delaySeconds=0, durationSeconds=1000, playTimes=1 ,isSameSpeed=false ,isSameOpacity=false  }

              ,new DControlAnimation{ name="从左弹入",type=1201, delaySeconds=0, durationSeconds=1000, playTimes=1 ,isSameSpeed=false ,isSameOpacity=false  }
              ,new DControlAnimation{ name="从右弹入",type=1202, delaySeconds=0, durationSeconds=1000, playTimes=1 ,isSameSpeed=false ,isSameOpacity=false  }
              ,new DControlAnimation{ name="从上弹入",type=1203, delaySeconds=0, durationSeconds=1000, playTimes=1 ,isSameSpeed=false ,isSameOpacity=false  }
              ,new DControlAnimation{ name="从下弹入",type=1204, delaySeconds=0, durationSeconds=1000, playTimes=1 ,isSameSpeed=false ,isSameOpacity=false  }
              ,new DControlAnimation{ name="中心弹入",type=1205, delaySeconds=0, durationSeconds=1000, playTimes=1 ,isSameSpeed=false ,isSameOpacity=false  }

              ,new DControlAnimation{ name="从左斜入",type=1301, delaySeconds=0, durationSeconds=1000, playTimes=1 ,isSameSpeed=false ,isSameOpacity=false  }
              ,new DControlAnimation{ name="从右斜入",type=1302, delaySeconds=0, durationSeconds=1000, playTimes=1 ,isSameSpeed=false ,isSameOpacity=false  }
              ,new DControlAnimation{ name="从上斜入",type=1303, delaySeconds=0, durationSeconds=1000, playTimes=1 ,isSameSpeed=false ,isSameOpacity=false  }
              ,new DControlAnimation{ name="从下斜入",type=1304, delaySeconds=0, durationSeconds=1000, playTimes=1 ,isSameSpeed=false ,isSameOpacity=false  }

               ,new DControlAnimation{ name="从左绕入",type=1401, delaySeconds=0, durationSeconds=1000, playTimes=1 ,isSameSpeed=false ,isSameOpacity=false  }
              ,new DControlAnimation{ name="从右绕入",type=1402, delaySeconds=0, durationSeconds=1000, playTimes=1 ,isSameSpeed=false ,isSameOpacity=false  }
              ,new DControlAnimation{ name="从上绕入",type=1403, delaySeconds=0, durationSeconds=1000, playTimes=1 ,isSameSpeed=false ,isSameOpacity=false  }
              ,new DControlAnimation{ name="从下绕入",type=1404, delaySeconds=0, durationSeconds=1000, playTimes=1 ,isSameSpeed=false ,isSameOpacity=false  }

               ,new DControlAnimation{ name="翻开",type=1501, delaySeconds=0, durationSeconds=1000, playTimes=1 ,isSameSpeed=false ,isSameOpacity=false  }
              //,new DControlAnimation{ name="摇摆",type=1502, delaySeconds=0, durationSeconds=1000, playTimes=1 ,isSameSpeed=false ,isSameOpacity=false  } 
               ,new DControlAnimation{ name="旋转",type=1502, delaySeconds=0, durationSeconds=1000, playTimes=1 ,isSameSpeed=true ,isSameOpacity=true  }
               ,new DControlAnimation{ name="来回缩放",type=1601, delaySeconds=0, durationSeconds=2000, playTimes=0 ,isSameSpeed=true ,isSameOpacity=true  }

        };



        //页面切换方式列表
        public static List<PageSwitchTypeDto> pageSwitchTypeList = new List<PageSwitchTypeDto> {
               new PageSwitchTypeDto{id=1, name="默认"  }
               ,new PageSwitchTypeDto{id=2, name="淡入"  }
               ,new PageSwitchTypeDto{id=3, name="右侧移入"  }
               ,new PageSwitchTypeDto{id=4, name="右侧拉伸"  }
               ,new PageSwitchTypeDto{id=5, name="中心放大"  }

        };

        /*
         * 音频三张图
         */
        public static List<BitmapImage> cAudioImageList = new List<BitmapImage> {
              FileUtil.readImage(AppDomain.CurrentDomain.BaseDirectory + "/myfile/sysimg/CAudio/ico0.png")
               ,FileUtil.readImage(AppDomain.CurrentDomain.BaseDirectory + "/myfile/sysimg/CAudio/ico1.png")
               ,FileUtil.readImage(AppDomain.CurrentDomain.BaseDirectory + "/myfile/sysimg/CAudio/ico2.png")
        };






    }
}
