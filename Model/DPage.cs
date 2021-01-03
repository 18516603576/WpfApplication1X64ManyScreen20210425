using System;

namespace Model
{
    public class DPage
    {

        public Int32 id { get; set; }
        //页面名称
        public String name { get; set; }
        //上一级页面
        public Int32 parentId { get; set; }
        //排序
        public Int32 idx { get; set; }
        //创建时间
        public String createTime { get; set; }
        //背景图片
        public Int32 backgroundImageId { get; set; }
        //弹窗页面宽度
        public Int32 width { get; set; }
        //弹窗页面高度
        public Int32 height { get; set; }
        //页面进入方式
        public Int32 pageSwitchType { get; set; }
        //视频背景
        public Int32 backgroundVideoId { get; set; }
    }
}
