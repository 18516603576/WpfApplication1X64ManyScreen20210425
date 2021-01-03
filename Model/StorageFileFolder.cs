using System;

namespace Model
{
    public class StorageFileFolder
    {
        public Int32 id { get; set; }
        //文件夹名
        public String name { get; set; }
        //父文件夹id
        public Int32 parentId { get; set; }
        //排序
        public Int32 idx { get; set; }
    }
}
