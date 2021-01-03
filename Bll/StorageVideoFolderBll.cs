using Dal;
using Model;
using System;
using System.Collections.Generic;

namespace Bll
{
    public class StorageVideoFolderBll
    {

        private readonly StorageVideoFolderDal storageVideoFolderDal = new StorageVideoFolderDal();

        private readonly StorageVideoBll storageVideoBll = new StorageVideoBll();

        /*
         * 获取
         */
        public StorageVideoFolder get(int id)
        {
            return storageVideoFolderDal.get(id);
        }

        public List<StorageVideoFolder> getByParentId(int parentId)
        {
            return storageVideoFolderDal.getByParentId(parentId);
        }

        /*
         * 获取最后一个文件夹
         */
        public StorageVideoFolder getLastFolder()
        {
            return storageVideoFolderDal.getLastFolder();
        }

        /*
         * 插入子文件夹
         */
        public StorageVideoFolder insertChild(StorageVideoFolder tmp)
        {
            //补充idx,createTime两个参数
            StorageVideoFolder maxFolder = getMaxIdxByParentId(tmp.parentId);
            if (maxFolder == null)
            {
                tmp.idx = 1;
            }
            else
            {
                tmp.idx = maxFolder.idx + 1;
            }

            return storageVideoFolderDal.insert(tmp);
        }

        /*
         * 获取父栏目中最大的排序号
         */
        private StorageVideoFolder getMaxIdxByParentId(int parentId)
        {
            return storageVideoFolderDal.getMaxIdxByParentId(parentId);
        }

        /*
         * 删除一个文件夹及文件夹下视频
         */
        private void deleteOneFolder(int folderId)
        {
            List<StorageVideo> list = storageVideoBll.getByFolderId(folderId);
            foreach (StorageVideo one in list)
            {
                storageVideoBll.delete(one);
            }
            int rows = storageVideoFolderDal.delete(folderId);
        }

        /*
         * 更新
         */
        public StorageVideoFolder update(StorageVideoFolder storageVideoFolder)
        {
            int rows = storageVideoFolderDal.update(storageVideoFolder);
            return storageVideoFolder;
        }

        /*
        * 是否存在同名文件夹
        */
        public bool hasSameName(StorageVideoFolder storageVideoFolder)
        {
            int rows = storageVideoFolderDal.hasSameName(storageVideoFolder);
            return rows > 0 ? true : false;
        }


        /*
         * 级联删除文件夹
         */
        public void deleteCascade(Int32 folderId)
        {
            List<StorageVideoFolder> children = getByParentId(folderId);
            foreach (StorageVideoFolder tmp in children)
            {
                deleteCascade(tmp.id);
            }
            deleteOneFolder(folderId);
        }
    }
}
