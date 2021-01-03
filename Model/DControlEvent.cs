using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    /*
     * 控件链接到及弹窗，可在多屏幕中同时跳转
     */
    public class DControlEvent
    {
        public Int32 id { get; set; }
        //控件id
        public Int32 dControlId { get; set; } 
        //显示到哪个屏幕
        public Int32 screenCfgId { get; set; }
        //链接到某个页面 
        public Int32 linkToPageId { get; set; }
        //是否弹窗链接
        public Boolean isDialogLink { get; set; }
        //显示到哪个Frame中
        public Int32 showInWhichCFrame { get; set; }
        //是否透明弹窗 
        public Boolean isTransparentDialog { get; set; }
        //轮播图片id--或者点击控件跳转，或者点击轮播中某一图片跳转
        public Int32 turnPictureImagesId { get; set; }

    }
}
