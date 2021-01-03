using Dal;
using Model;
using System;
using System.Collections.Generic;

namespace Bll
{
    public class StorageFileFolderBll
    {

        private readonly StorageFileFolderDal storageFileFolderDal = new StorageFileFolderDal();
        private readonly StorageFileBll storageFileBll = new StorageFileBll();

        /*
         * 获取
         */
        public StorageFileFolder get(int id)
        {
            return storageFileFolderDal.get(id);
        }

        public List<StorageFileFolder> getByParentId(int parentId)
        {
            return storageFileFolderDal.getByParentId(parentId);
        }

        /*
         * 获取最后一个文件夹
         */
        public StorageFileFolder getLastFolder()
        {
            return storageFileFolderDal.getLastFolder();
        }

        /*
         * 插入子文件夹
         */
        public StorageFileFolder insertChild(StorageFileFolder tmp)
        {
            //补充idx,createTime两个参数
            StorageFileFolder maxFolder = getMaxIdxByParentId(tmp.parentId);
            if (maxFolder == null)
            {
                tmp.idx = 1;
            }
            else
            {
                tmp.idx = maxFolder.idx + 1;
            }

            return storageFileFolderDal.insert(tmp);
        }

        /*
         * 获取父栏目中最大的排序号
         */
        private StorageFileFolder getMaxIdxByParentId(int parentId)
        {
            return storageFileFolderDal.getMaxIdxByParentId(parentId);
        }

        /*
         * 删除一个文件夹及文件夹下视频
         */
        private void deleteOneFolder(int folderId)
        {
            List<StorageFile> list = storageFileBll.getByFolderId(folderId);
            foreach (StorageFile one in list)
            {
                storageFileBll.delete(one);
            }
            int rows = storageFileFolderDal.delete(folderId);
        }

        /*
         * 更新
         */
        public StorageFileFolder update(StorageFileFolder storageFileFolder)
        {
            int rows = storageFileFolderDal.update(storageFileFolder);
            return storageFileFolder;
        }

        /*
        * 是否存在同名文件夹
        */
        public bool hasSameName(StorageFileFolder storageFileFolder)
        {
            int rows = storageFileFolderDal.hasSameName(storageFileFolder);
            return rows > 0 ? true : false;
        }


        /*
         * 级联删除文件夹
         */
        public void deleteCascade(Int32 folderId)
        {
            List<StorageFileFolder> children = getByParentId(folderId);
            foreach (StorageFileFolder tmp in children)
            {
                deleteCascade(tmp.id);
            }
            deleteOneFolder(folderId);
        }
    }
}
