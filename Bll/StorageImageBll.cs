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
    public class StorageImageBll
    {

        private readonly StorageImageDal storageImageDal = new StorageImageDal();

        public StorageImage insert(string origFilePath, string url, Int32 actualWidth, Int32 actualHeight, Int32 folderId)
        {
            //补充idx,createTime两个参数
            FileInfo fileInfo = new FileInfo(origFilePath);
            int filesize = 0;
            if (fileInfo.Exists)
            {
                filesize = (int)fileInfo.Length;
            }
            String origFilename = FileUtil.getFilename(origFilePath);

            StorageImage storageImage = new StorageImage();
            storageImage.origFilename = origFilename;
            storageImage.url = url;
            storageImage.ext = fileInfo.Extension;
            storageImage.size = filesize;
            storageImage.createTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            storageImage.actualWidth = actualWidth;
            storageImage.actualHeight = actualHeight;
            storageImage.folderId = folderId;


            return storageImageDal.insert(storageImage);
        }
        /*
         * 获取下一页
         */
        public List<StorageImage> getNextPage(int currPage, int pageSize, int folderId)
        {
            return storageImageDal.getNextPage(currPage, pageSize, folderId);
        }

        /*
        * 获取一个
        */
        public StorageImage get(Int32 id)
        {
            return storageImageDal.get(id);
        }

        /*
         * 删除
         */
        public Int32 delete(StorageImage storageImage)
        {
            if (storageImage == null) return 0;
            if (string.IsNullOrWhiteSpace(storageImage.url)) return 0;
            //string fullFilePath = AppDomain.CurrentDomain.BaseDirectory + storageImage.url;
            //Boolean b = File.Exists(fullFilePath);
            //if (b)
            //{
            //    File.Delete(fullFilePath);
            //}  
            return storageImageDal.delete(storageImage.id);
        }

        /*
         * 获取文件夹下的图片
         */
        public List<StorageImage> getByFolderId(int folderId)
        {
            return storageImageDal.getByFolderId(folderId);
        }

        public int getPageCount(int pageSize, int folderId)
        {
            int pagecount = 0;
            int count = storageImageDal.getCount(folderId);
            if (count > 0)
            {
                double val = count * 1.0 / pageSize;
                pagecount = (int)Math.Ceiling(val);
            }
            return pagecount;
        }

        public List<StorageImage> getNextPage2(int currPage, int pageSize)
        {
            return storageImageDal.getNextPage2(currPage, pageSize);
        }

        /*
         * 更新图片
         */
        public StorageImage update(StorageImage one)
        {
            int rows = storageImageDal.upate(one);
            return one;
        }
    }
}
