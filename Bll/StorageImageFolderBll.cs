using Dal;
using Model;
using System;
using System.Collections.Generic;

namespace Bll
{
    public class StorageImageFolderBll
    {
        private readonly StorageImageFolderDal storageImageFolderDal = new StorageImageFolderDal();
        private readonly StorageImageBll storageImageBll = new StorageImageBll();

        /*
         * 获取
         */
        public StorageImageFolder get(int id)
        {
            return storageImageFolderDal.get(id);
        }

        public List<StorageImageFolder> getByParentId(int parentId)
        {
            return storageImageFolderDal.getByParentId(parentId);
        }

        /*
         * 获取最后一个文件夹
         */
        public StorageImageFolder getLastFolder()
        {
            return storageImageFolderDal.getLastFolder();
        }

        /*
         * 插入子文件夹
         */
        public StorageImageFolder insertChild(StorageImageFolder tmp)
        {
            //补充idx,createTime两个参数
            StorageImageFolder maxFolder = getMaxIdxByParentId(tmp.parentId);
            if (maxFolder == null)
            {
                tmp.idx = 1;
            }
            else
            {
                tmp.idx = maxFolder.idx + 1;
            }

            return storageImageFolderDal.insert(tmp);
        }

        /*
         * 获取父栏目中最大的排序号
         */
        private StorageImageFolder getMaxIdxByParentId(int parentId)
        {
            return storageImageFolderDal.getMaxIdxByParentId(parentId);
        }

        /*
         * 删除一个文件夹及文件夹下视频
         */
        private void deleteOneFolder(int folderId)
        {
            List<StorageImage> list = storageImageBll.getByFolderId(folderId);
            foreach (StorageImage one in list)
            {
                storageImageBll.delete(one);
            }
            int rows = storageImageFolderDal.delete(folderId);
        }

        /*
         * 更新
         */
        public StorageImageFolder update(StorageImageFolder storageImageFolder)
        {
            int rows = storageImageFolderDal.update(storageImageFolder);
            return storageImageFolder;
        }

        /*
        * 是否存在同名文件夹
        */
        public bool hasSameName(StorageImageFolder storageImageFolder)
        {
            int rows = storageImageFolderDal.hasSameName(storageImageFolder);
            return rows > 0 ? true : false;
        }


        /*
         * 级联删除文件夹
         */
        public void deleteCascade(Int32 folderId)
        {
            List<StorageImageFolder> children = getByParentId(folderId);
            foreach (StorageImageFolder tmp in children)
            {
                deleteCascade(tmp.id);
            }
            deleteOneFolder(folderId);
        }
    }
}
