using System;

namespace Model
{
    public class DControl
    {


        public Int32 id { get; set; }
        //页面id
        public Int32 pageId { get; set; }
        //控件名称
        public String name { get; set; }
        //宽度
        public Int32 width { get; set; }
        //高度
        public Int32 height { get; set; }
        //距左边
        public Int32 left { get; set; }
        //距上边
        public Int32 top { get; set; }
        //控件类型  Image, TurnPicture1,  TurnPicture2, Video, Web, Word 
        public String type { get; set; }
        //控件内容
        public String content { get; set; }

        //控件排放顺序
        public Int32 idx { get; set; }




        //链接到页面id
        public Int32 linkToPageId { get; set; }
        //点击放大图片  
        public Boolean isClickShow { get; set; }

        //点击全屏播放视频
        public Int32 linkToVideoId { get; set; }

        //自动播放 true
        public Boolean autoplay { get; set; }
        //循环播放 false
        public Boolean loop { get; set; }
        //图片切换间隔
        public Int32 turnPictureSpeed { get; set; }
        //链接到外部网站
        public String linkToWeb { get; set; }
        //视频id，图片id，wordid
        public Int32 storageId { get; set; }
        //是否显示相册左右箭头
        public Boolean isShowTurnPictureArrow { get; set; }
        //控件透明度
        public Int32 opacity { get; set; }
        //父控件id （滚动区域）
        public Int32 parentId { get; set; }
        //内容宽度
        public Int32 contentWidth { get; set; }
        //内容高度
        public Int32 contentHeight { get; set; }
        //滚动区背景图片
        public Int32 backgroundImageId { get; set; }


        //是否Tab
        public Boolean isTab { get; set; }
        //流动相册，一行显示几个
        public int rowNum { get; set; }
        //流动相册 间距
        public int spacing { get; set; }
        //旋转角度
        public int rotateAngle { get; set; }
        //是否弹窗显示页面
        public Boolean isDialogLink { get; set; }
        //显示在哪个CFrame中
        public Int32 showInWhichCFrame { get; set; }
        //弹窗是否为透明背景 
        public Boolean isTransparentDialog { get; set; }
        //字体
        public String fontFamily { get; set; }
        //文字大小
        public int fontSize { get; set; }
        //行高
        public int fontLineHeight { get; set; }
        //文本颜色
        public string fontColor { get; set; }
        //文本粗细
        public string fontWeight { get; set; }
        //文本框正在编辑中
        public Boolean fontIsEditing { get; set; }
        //文本对齐方式  0left  1right 2center 3Justify
        public int fontTextAlignment { get; set; }
        //视频 音频封面图片
        public int storageIdOfCover { get; set; }
        //显示到哪个屏幕中
        public int screenCfgId { get; set; }
        //首次加载是否隐藏视频控制台
        public Boolean isHideVideoConsoleOfFirstLoad { get; set; }
    }
}
