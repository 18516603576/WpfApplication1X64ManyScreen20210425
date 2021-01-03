using System;

namespace Model
{
    public class ScreenCfg
    {

        public Int32 id { get; set; }
        //系统给屏幕的名称
        public string deviceName { get; set; }
        //自定义名称
        public string diyName { get; set; }
        //屏幕宽度
        public Int32 width { get; set; }
        //屏幕高度
        public Int32 height { get; set; }
        //是否主屏幕
        public Boolean isPrimary { get; set; }
        //当前屏幕首页
        public Int32 indexPageId { get; set; }
          
    }
}
