using Dal;
using Model;
using System;
using System.Collections.Generic;

namespace Bll
{
    public class StorageGifFolderBll
    {
        private readonly StorageGifFolderDal storageGifFolderDal = new StorageGifFolderDal();
        private readonly StorageGifBll storageGifBll = new StorageGifBll();

        /*
         * 获取
         */
        public StorageGifFolder get(int id)
        {
            return storageGifFolderDal.get(id);
        }

        public List<StorageGifFolder> getByParentId(int parentId)
        {
            return storageGifFolderDal.getByParentId(parentId);
        }

        /*
         * 获取最后一个文件夹
         */
        public StorageGifFolder getLastFolder()
        {
            return storageGifFolderDal.getLastFolder();
        }

        /*
         * 插入子文件夹
         */
        public StorageGifFolder insertChild(StorageGifFolder tmp)
        {
            //补充idx,createTime两个参数
            StorageGifFolder maxFolder = getMaxIdxByParentId(tmp.parentId);
            if (maxFolder == null)
            {
                tmp.idx = 1;
            }
            else
            {
                tmp.idx = maxFolder.idx + 1;
            }

            return storageGifFolderDal.insert(tmp);
        }

        /*
         * 获取父栏目中最大的排序号
         */
        private StorageGifFolder getMaxIdxByParentId(int parentId)
        {
            return storageGifFolderDal.getMaxIdxByParentId(parentId);
        }

        /*
         * 删除一个文件夹及文件夹下视频
         */
        private void deleteOneFolder(int folderId)
        {
            List<StorageGif> list = storageGifBll.getByFolderId(folderId);
            foreach (StorageGif one in list)
            {
                storageGifBll.delete(one);
            }
            int rows = storageGifFolderDal.delete(folderId);
        }

        /*
         * 更新
         */
        public StorageGifFolder update(StorageGifFolder storageGifFolder)
        {
            int rows = storageGifFolderDal.update(storageGifFolder);
            return storageGifFolder;
        }

        /*
        * 是否存在同名文件夹
        */
        public bool hasSameName(StorageGifFolder storageGifFolder)
        {
            int rows = storageGifFolderDal.hasSameName(storageGifFolder);
            return rows > 0 ? true : false;
        }


        /*
         * 级联删除文件夹
         */
        public void deleteCascade(Int32 folderId)
        {
            List<StorageGifFolder> children = getByParentId(folderId);
            foreach (StorageGifFolder tmp in children)
            {
                deleteCascade(tmp.id);
            }
            deleteOneFolder(folderId);
        }
    }
}
