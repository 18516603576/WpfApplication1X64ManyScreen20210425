using Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;

namespace Dal
{
    public class StorageVideoFolderDal
    {


        /*
         * 获取子文件夹
         */
        public List<StorageVideoFolder> getByParentId(int parentId)
        {
            String sql = "select * from storageVideoFolder where parentId=@parentId  order by idx asc";
            SQLiteParameter[] parameters = {
                    new SQLiteParameter("@parentId", DbType.Int32,4)
            };
            parameters[0].Value = parentId;

            DataTable dt = Common.SQLiteHelper.ExecuteQuery(sql, parameters);

            List<StorageVideoFolder> list = DataToEntity<StorageVideoFolder>.FillModel(dt);
            return list;
        }
        /*
         * 获取最后一个文件夹
         */
        public StorageVideoFolder getLastFolder()
        {
            String sql = "select * from storageVideoFolder order by id desc limit 0,1";
            SQLiteParameter[] parameters = {

                      };
            DataTable dt = Common.SQLiteHelper.ExecuteQuery(sql, parameters);

            StorageVideoFolder entity = DataToEntity<StorageVideoFolder>.FillModel(dt.Rows[0]);
            return entity;
        }

        /*
         * 插入一个文件夹
         */
        public StorageVideoFolder insert(StorageVideoFolder entity)
        {
            String sql = "insert into storageVideoFolder(name,parentId,idx) values(@name,@parentId,@idx);select last_insert_rowid();";
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
        public int update(StorageVideoFolder entity)
        {
            string sql = "update storageVideoFolder set name=@name,parentId=@parentId,idx=@idx"
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
        public int hasSameName(StorageVideoFolder entity)
        {
            string sql = "select count(*) from storageVideoFolder where name=@name and id!=@id";
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
            String sql = "delete from storageVideoFolder where id=@id";
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
        public StorageVideoFolder get(int id)
        {
            String sql = "select * from storageVideoFolder where id=@id";
            SQLiteParameter[] parameters = {
                       new SQLiteParameter("@id", DbType.Int32,4)
                      };
            parameters[0].Value = id;

            DataTable dt = Common.SQLiteHelper.ExecuteQuery(sql, parameters);
            if (dt == null || dt.Rows.Count == 0) return null;

            StorageVideoFolder storageVideoFolder = DataToEntity<StorageVideoFolder>.FillModel(dt.Rows[0]);
            return storageVideoFolder;
        }

        /*
         * 获取父栏目中最大的排序号
         */
        public StorageVideoFolder getMaxIdxByParentId(int parentId)
        {
            String sql = "select max(idx) as idx from storageVideoFolder where parentId=@parentId";
            SQLiteParameter[] parameters = {
                       new SQLiteParameter("@parentId", DbType.Int32,4)
                      };
            parameters[0].Value = parentId;

            DataTable dt = Common.SQLiteHelper.ExecuteQuery(sql, parameters);

            StorageVideoFolder entity = DataToEntity<StorageVideoFolder>.FillModel(dt.Rows[0]);
            return entity;
        }
    }
}
