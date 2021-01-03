using Common;
using Dal;
using Model;
using System;
using System.Collections.Generic;
using System.IO;

namespace Bll
{
    public class StorageVideoBll
    {
        private readonly StorageVideoDal storageVideoDal = new StorageVideoDal();
        private readonly StorageImageBll storageImageBll = new StorageImageBll();

        public StorageVideo insert(string origFilePath, string url, int duration, string imgPath, Int32 imgWidth, Int32 imgHeight, Int32 folderId)
        {

            StorageImage storageImage = storageImageBll.insert(AppDomain.CurrentDomain.BaseDirectory + imgPath, imgPath, imgWidth, imgHeight, 1);

            //补充idx,createTime两个参数
            FileInfo fileInfo = new FileInfo(origFilePath);
            String origFilename = FileUtil.getFilename(origFilePath);

            StorageVideo storageVideo = new StorageVideo();
            storageVideo.origFilename = origFilename;
            storageVideo.url = url;
            storageVideo.ext = fileInfo.Extension;
            storageVideo.size = (int)fileInfo.Length;
            storageVideo.createTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            storageVideo.storageImageId = storageImage.id;
            storageVideo.duration = duration; 
            storageVideo.folderId = folderId;  //放在默认文件夹下

            return storageVideoDal.insert(storageVideo);
        }

        /*
         * 分页获取视频 
         */
        public List<StorageVideo> getNextPage(int currPage, int pageSize, int folderId)
        {

            return storageVideoDal.getNextPage(currPage, pageSize, folderId);
        }



        public StorageVideo get(int id)
        {
            return storageVideoDal.get(id);
        }

        public int getPageCount(int pageSize, int folderId)
        {
            int pagecount = 0;
            int count = storageVideoDal.getCount(folderId);
            if (count > 0)
            {
                double val = count * 1.0 / pageSize;
                pagecount = (int)Math.Ceiling(val);
            }
            return pagecount;
        }

        /*
         * 获取文件夹下的所有视频
         */
        public List<StorageVideo> getByFolderId(int folderId)
        {
            return storageVideoDal.getByFolderId(folderId);
        }

        public Int32 delete(StorageVideo storageVideo)
        {
            //1.删除物理文件
            if (string.IsNullOrWhiteSpace(storageVideo.url)) return 0;
            //string fullFilePath = AppDomain.CurrentDomain.BaseDirectory + storageVideo.url;
            //Boolean b = File.Exists(fullFilePath);
            //if (b)
            //{
            //    File.Delete(fullFilePath);
            //}

            //2.同时删除缩略图
            StorageImage storageImage = storageImageBll.get(storageVideo.storageImageId);
            storageImageBll.delete(storageImage);

            //3.数据库记录删除
            return storageVideoDal.delete(storageVideo.id);
        }

        public List<StorageVideo> getNextPage2(int currPage, int pageSize)
        {
            return storageVideoDal.getNextPage2(currPage, pageSize);
        }

        /*
         * 更新视频
         */
        public StorageVideo update(StorageVideo one)
        {
            int rows = storageVideoDal.upate(one);
            return one;
        }
    }
}
