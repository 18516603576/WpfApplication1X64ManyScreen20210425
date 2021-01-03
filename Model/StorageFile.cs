using System;

namespace Model
{
    public class StorageFile
    {
        public Int32 id { get; set; }
        //原文件名
        public String origFilename { get; set; }
        //上传后的文件地址
        public String url { get; set; }
        //大小
        public Int32 size { get; set; }
        //后缀
        public string ext { get; set; }
        //创建时间
        public String createTime { get; set; }
        //文件夹id
        public Int32 folderId { get; set; }
        //音频时长
        public Int32 duration { get; set; }
    }
}
