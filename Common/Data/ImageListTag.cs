using Model;
using System;

namespace Common.Data
{
    public class ImageListTag
    {
        //是否选中
        public Boolean isSelected { get; set; }

        //控件绑定的数据
        public StorageImage storageImage { get; set; }
    }
}
