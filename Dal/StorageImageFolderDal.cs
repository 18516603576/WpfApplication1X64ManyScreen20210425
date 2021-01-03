using Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;

namespace Dal
{
    public class StorageImageFolderDal
    {
        /*
        * 获取子文件夹
        */
        public List<StorageImageFolder> getByParentId(int parentId)
        {
            String sql = "select * from storageImageFolder where parentId=@parentId  order by idx asc";
            SQLiteParameter[] parameters = {
                    new SQLiteParameter("@parentId", DbType.Int32,4)
            };
            parameters[0].Value = parentId;

            DataTable dt = Common.SQLiteHelper.ExecuteQuery(sql, parameters);

            List<StorageImageFolder> list = DataToEntity<StorageImageFolder>.FillModel(dt);
            return list;
        }
        /*
         * 获取最后一个文件夹
         */
        public StorageImageFolder getLastFolder()
        {
            String sql = "select * from storageImageFolder order by id desc limit 0,1";
            SQLiteParameter[] parameters = {

                      };
            DataTable dt = Common.SQLiteHelper.ExecuteQuery(sql, parameters);

            StorageImageFolder entity = DataToEntity<StorageImageFolder>.FillModel(dt.Rows[0]);
            return entity;
        }

        /*
         * 插入一个文件夹
         */
        public StorageImageFolder insert(StorageImageFolder entity)
        {
            String sql = "insert into storageImageFolder(name,parentId,idx) values(@name,@parentId,@idx);select last_insert_rowid();";
            SQLiteParameter[] parameters = {
                       new SQLiteParameter("@name", DbType.String,100),
                       new SQLiteParameter("@parentId", DbType.Int32,4),
                       new SQLiteParameter("@idx", DbType.Int32,4)
                      };
            parameters[0].Value = entity.name;
            parameters[1].Value = entity.parentId;
            parameters[2].Value = entity.idx;

            DataTable dt = Common.SQLiteHelper.ExecuteQuery(sql, parameters);
            int id = DataType.ToInt32(dt.Rows[0]["last_insert_rowid()"].ToString());
            entity.id = id;
            return entity;
        }
        /*
         * 更新
         */
        public int update(StorageImageFolder entity)
        {
            string sql = "update storageImageFolder set name=@name,parentId=@parentId,idx=@idx"
              + "  where id=@id";

            SQLiteParameter[] parameters = {
                       new SQLiteParameter("@name", DbType.String,100),
                       new SQLiteParameter("@parentId", DbType.Int32,4),
                       new SQLiteParameter("@idx", DbType.Int32,4),
                       new SQLiteParameter("@id", DbType.Int32,4)
                      };
            parameters[0].Value = entity.name;
            parameters[1].Value = entity.parentId;
            parameters[2].Value = entity.idx;
            parameters[3].Value = entity.id;

            int result = Common.SQLiteHelper.ExecuteNonQuery(sql, parameters);
            return result;
        }

        /*
        * 是否存在同名文件夹
        */
        public int hasSameName(StorageImageFolder entity)
        {
            string sql = "select count(*) from storageImageFolder where name=@name and id!=@id";
            SQLiteParameter[] parameters = {
                        new SQLiteParameter("@name", DbType.String,30),
                        new SQLiteParameter("@id", DbType.Int32,4)
                      };
            parameters[0].Value = entity.name;
            parameters[1].Value = entity.id;

            object obj = Common.SQLiteHelper.ExecuteScalar(sql, parameters);

            int result = int.Parse(obj.ToString());
            return result;
        }

        /*
         * 删除文件夹
         */
        public int delete(int id)
        {
            String sql = "delete from storageImageFolder where id=@id";
            SQLiteParameter[] parameters = {
                       new SQLiteParameter("@id", DbType.Int32,4)
                      };
            parameters[0].Value = id;

            Int32 rows = Common.SQLiteHelper.ExecuteNonQuery(sql, parameters);
            return rows;
        }

        /*
        * 获取文件夹
        */
        public StorageImageFolder get(int id)
        {
            String sql = "select * from storageImageFolder where id=@id";
            SQLiteParameter[] parameters = {
                       new SQLiteParameter("@id", DbType.Int32,4)
                      };
            parameters[0].Value = id;

            DataTable dt = Common.SQLiteHelper.ExecuteQuery(sql, parameters);
            if (dt == null || dt.Rows.Count == 0) return null;

            StorageImageFolder storageImageFolder = DataToEntity<StorageImageFolder>.FillModel(dt.Rows[0]);
            return storageImageFolder;
        }

        /*
         * 获取父栏目中最大的排序号
         */
        public StorageImageFolder getMaxIdxByParentId(int parentId)
        {
            String sql = "select max(idx) as idx from storageImageFolder where parentId=@parentId";
            SQLiteParameter[] parameters = {
                       new SQLiteParameter("@parentId", DbType.Int32,4)
                      };
            parameters[0].Value = parentId;

            DataTable dt = Common.SQLiteHelper.ExecuteQuery(sql, parameters);

            StorageImageFolder entity = DataToEntity<StorageImageFolder>.FillModel(dt.Rows[0]);
            return entity;
        }
    }
}
