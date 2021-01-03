using Model;
using System;
using System.Windows;
using WinFormCef;

namespace Common.Data
{
    public class LocalStorage
    {
        public Cfg cfg { get; set; }
        ////图片、视频不存在 
        //public string imageNotExists = "/myfile/sysimg/notExists/image.png";
        ////word文件不存在，则显示
        //public string wordNotExists = "/myfile/sysimg/notExists/word.xps";
        ////视频不存在，则显示
        //public string videoNotExists = "/myfile/sysimg/notExists/video.mp4";
        ////音乐不存在，则显示
        //public string backgroundMusicNotExists = "/myfile/sysimg/notExists/music.mp4";
        //添加图片背景
        public string icoAddImage = "/myfile/sysimg/ico-add-image.png";
        //删除视频 删除图片
        public string icoRemove = "/Resources/ico_remove.png";
        //当前复制的控件
        public FrameworkElement currCopiedEle = null;
        //当前页面
        public Int32 currPageId = 1;

        public Window currWindow = null;
        //浏览器
        public Form1 currForm1 = null;

        //public List<DControlAnimation> animationList = new List<DControlAnimation> { 
        //       new DControlAnimation{ name="淡入",type=1, delaySeconds=0, durationSeconds=2 }
        //      ,new DControlAnimation{ name="左侧移入",type=2, delaySeconds=0, durationSeconds=2 }
        //      ,new DControlAnimation{ name="右侧移入",type=3, delaySeconds=0, durationSeconds=2  }
        //      ,new DControlAnimation{ name="顶部移入",type=4, delaySeconds=0, durationSeconds=2  }
        //      ,new DControlAnimation{ name="底部移入",type=5, delaySeconds=0, durationSeconds=2  }
        //      ,new DControlAnimation{ name="旋转",type=6, delaySeconds=0, durationSeconds=2  }
        //      ,new DControlAnimation{ name="放大",type=7, delaySeconds=0, durationSeconds=2  }
        //      ,new DControlAnimation{ name="缩小",type=8, delaySeconds=0, durationSeconds=2  }
        //};
    }
}
