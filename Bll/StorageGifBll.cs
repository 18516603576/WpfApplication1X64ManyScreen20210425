using Common;
using Dal;
using Model;
using System;
using System.Collections.Generic;
using System.IO;


/*
 * 图片库
 */
namespace Bll
{
    public class StorageGifBll
    {

        private readonly StorageGifDal storageGifDal = new StorageGifDal();

        public StorageGif insert(string origFilePath, string url, Int32 actualWidth, Int32 actualHeight, Int32 folderId)
        {
            //补充idx,createTime两个参数
            FileInfo fileInfo = new FileInfo(origFilePath);
            int filesize = 0;
            if (fileInfo.Exists)
            {
                filesize = (int)fileInfo.Length;
            }
            String origFilename = FileUtil.getFilename(origFilePath);

            StorageGif storageGif = new StorageGif();
            storageGif.origFilename = origFilename;
            storageGif.url = url;
            storageGif.ext = fileInfo.Extension;
            storageGif.size = filesize;
            storageGif.createTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            storageGif.actualWidth = actualWidth;
            storageGif.actualHeight = actualHeight;
            storageGif.folderId = folderId;


            return storageGifDal.insert(storageGif);
        }
        /*
         * 获取下一页
         */
        public List<StorageGif> getNextPage(int currPage, int pageSize, int folderId)
        {
            return storageGifDal.getNextPage(currPage, pageSize, folderId);
        }

        /*
        * 获取一个
        */
        public StorageGif get(Int32 id)
        {
            return storageGifDal.get(id);
        }

        /*
         * 删除
         */
        public Int32 delete(StorageGif storageGif)
        {
            if (storageGif == null) return 0;
            if (string.IsNullOrWhiteSpace(storageGif.url)) return 0;
            //string fullFilePath = AppDomain.CurrentDomain.BaseDirectory + storageGif.url;
            //Boolean b = File.Exists(fullFilePath);
            //if (b)
            //{
            //    File.Delete(fullFilePath);
            //}  
            return storageGifDal.delete(storageGif.id);
        }

        /*
         * 获取文件夹下的图片
         */
        public List<StorageGif> getByFolderId(int folderId)
        {
            return storageGifDal.getByFolderId(folderId);
        }

        public int getPageCount(int pageSize, int folderId)
        {
            int pagecount = 0;
            int count = storageGifDal.getCount(folderId);
            if (count > 0)
            {
                double val = count * 1.0 / pageSize;
                pagecount = (int)Math.Ceiling(val);
            }
            return pagecount;
        }

        public List<StorageGif> getNextPage2(int currPage, int pageSize)
        {
            return storageGifDal.getNextPage2(currPage, pageSize);
        }

        /*
         * 更新图片
         */
        public StorageGif update(StorageGif one)
        {
            int rows = storageGifDal.upate(one);
            return one;
        }
    }
}
