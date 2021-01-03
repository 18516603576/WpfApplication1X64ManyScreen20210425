using Common;
using Dal;
using Model;
using System;
using System.Collections.Generic;
using System.IO;

namespace Bll
{
    public class StorageFileBll
    {
        private readonly StorageFileDal storageFileDal = new StorageFileDal();

        public StorageFile insert(string origFilePath, string url, Int32 folderId,Int32 duration=0)
        {
            //补充idx,createTime两个参数
            FileInfo fileInfo = new FileInfo(origFilePath);
            String origFilename = FileUtil.getFilename(origFilePath);

            StorageFile storageFile = new StorageFile();
            storageFile.origFilename = origFilename;
            storageFile.url = url;
            storageFile.ext = fileInfo.Extension;
            storageFile.size = (int)fileInfo.Length;
            storageFile.createTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            storageFile.folderId = folderId;
            storageFile.duration = duration;

            return storageFileDal.insert(storageFile);
        }

        /*
        * 获取一个
        */
        public StorageFile get(Int32 id)
        {
            return storageFileDal.get(id);
        }

        /*
        * 获取当前页面列表 
        */
        public List<StorageFile> getNextPage(int currPage, int pageSize, int folderId)
        {
            return storageFileDal.getNextPage(currPage, pageSize, folderId);
        }

        /*
         * 获取Word列表
         */
        public List<StorageFile> getNextPageOfWord(int currPage, int pageSize, int folderId)
        {
            return storageFileDal.getNextPageOfWord(currPage, pageSize, folderId);
        }
        /*
         * 获取文件总数
         */
        public int getPageCount(int pageSize, int folderId)
        {
            int pagecount = 0;
            int count = storageFileDal.getCount(folderId);
            if (count > 0)
            {
                double val = count * 1.0 / pageSize;
                pagecount = (int)Math.Ceiling(val);
            }
            return pagecount;
        }

        internal List<StorageFile> getByFolderId(int folderId)
        {
            return storageFileDal.getByFolderId(folderId);
        }

        /*
         * 删除
         */
        public int delete(StorageFile storageFile)
        {
            //2.删除本地文件  
            if (string.IsNullOrWhiteSpace(storageFile.url)) return 0;
            //string fullFilePath = AppDomain.CurrentDomain.BaseDirectory + storageFile.url;
            //Boolean b = File.Exists(fullFilePath);
            //if (b)
            //{
            //    File.Delete(fullFilePath);
            //}

            return storageFileDal.delete(storageFile.id);
        }

        /*
         * 更新
         */
        public StorageFile update(StorageFile one)
        {
            int rows = storageFileDal.upate(one);
            return one;
        }
    }
}
