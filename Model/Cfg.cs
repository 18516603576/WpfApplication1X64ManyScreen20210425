using System;

namespace Model
{
    public class Cfg
    {

        public Int32 id { get; set; }
        //页面缩放百分比
        public Int32 pagePercent { get; set; }
        //屏幕宽度
        public Int32 screenWidth { get; set; }
        //屏幕高度
        public Int32 screenHeight { get; set; }
        //背景音乐
        // public String backgroundMusic { get; set; }
        //是否启用屏保
        public Boolean screenSaverIsable { get; set; }
        //无操作时间 秒
        public Int32 screenSaverNoActionTime { get; set; }
        //屏保图片播放时间
        public Int32 screenSaverImgsSpeed { get; set; }
        //屏保图片
        public String screenSaverImgs { get; set; }

        public Int32 defaultTemplate { get; set; }

        //密码
        public String password { get; set; }
        //是否禁用密码退出
        //  public Boolean isCancelPasswordWay { get; set; }


        //背景音乐自动播放
        public Boolean backgroundMusicAutoplay { get; set; }
        //循环播放
        public Boolean backgroundMusicLoop { get; set; }
        //是否显示按钮
        public Boolean backgroundMusicShow { get; set; }
        //按钮左边距
        public Int32 backgroundMusicButtonLeft { get; set; }
        //按钮右边距
        public Int32 backgroundMusicButtonTop { get; set; }
        //按钮宽度
        public Int32 backgroundMusicButtonWidth { get; set; }
        //按钮高度
        public Int32 backgroundMusicButtonHeight { get; set; }
        //音乐文件id
        public Int32 backgroundMusicId { get; set; }
        //按钮背景图id
        public Int32 backgroundMusicButtonImageId { get; set; }
        //页面切换方式
        public Int32 pageSwitchType { get; set; }
        //多久无操作自动返回首页
        public Int32 noActionTimeBackToHome { get; set; }
        //星期转文字
        public string week1 { get; set; }
        public string week2 { get; set; }
        public string week3 { get; set; }
        public string week4 { get; set; }
        public string week5 { get; set; }
        public string week6 { get; set; }
        public string week7 { get; set; }



    }
}
