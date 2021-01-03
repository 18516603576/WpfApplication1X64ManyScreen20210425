using Model;
using System;

namespace Common.Data
{
    public class FileListTag
    {
        //是否选中
        public Boolean isSelected { get; set; }

        //控件绑定的数据
        public StorageFile storageFile { get; set; }
    }
}
