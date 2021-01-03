using Model;
using System;

namespace Common.Data
{
    public class VideoListTag
    {
        //是否选中
        public Boolean isSelected { get; set; }

        //控件绑定的数据
        public StorageVideo storageVideo { get; set; }
    }
}
