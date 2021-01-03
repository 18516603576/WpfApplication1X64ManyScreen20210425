using Model;
using System;

namespace Common.Data
{
    public class GifListTag
    {
        //是否选中
        public Boolean isSelected { get; set; }

        //控件绑定的数据 
        public StorageGif storageGif { get; set; }
    }
}
