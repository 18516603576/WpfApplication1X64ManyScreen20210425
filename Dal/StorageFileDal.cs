using Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;

namespace Dal
{
    public class StorageFileDal
    {
        //1、插入图片
        public StorageFile insert(StorageFile entity)
        {
            String sql = "insert into storageFile(origFilename,url,ext,size,createTime,folderId,duration) values(@origFilename,@url,@ext,@size,@createTime,@folderId,@duration);select last_insert_rowid();";
            SQLiteParameter[] parameters = {
                       new SQLiteParameter("@origFilename", DbType.String,100),
                       new SQLiteParameter("@url", DbType.String,255),
                       new SQLiteParameter("@ext", DbType.String,10),
                       new SQLiteParameter("@size", DbType.Int32,4),
                       new SQLiteParameter("@createTime", DbType.String,30),
                       new SQLiteParameter("@folderId", DbType.Int32,4),
                       new SQLiteParameter("@duration", DbType.Int32,4)
                      };
            parameters[0].Value = entity.origFilename;
            parameters[1].Value = entity.url;
            parameters[2].Value = entity.ext;
            parameters[3].Value = entity.size;
            parameters[4].Value = entity.createTime;
            parameters[5].Value = entity.folderId;
            parameters[6].Value = entity.duration;

            DataTable dt = Common.SQLiteHelper.ExecuteQuery(sql, parameters);
            int id = DataType.ToInt32(dt.Rows[0]["last_insert_rowid()"].ToString());
            entity.id = id;
            return entity;
        }




        //获取文件
        public StorageFile get(Int32 id)
        {
            String sql = "select * from storageFile where id=@id";
            SQLiteParameter[] parameters = {
                       new SQLiteParameter("@id", DbType.Int32,4)
                      };
            parameters[0].Value = id;

            DataTable dt = Common.SQLiteHelper.ExecuteQuery(sql, parameters);
            if (dt == null || dt.Rows.Count == 0)
            {
                return null;
            }
            StorageFile storageFile = DataToEntity<StorageFile>.FillModel(dt.Rows[0]);
            return storageFile;
        }

        /*
         * 获取文件夹下所有图片
         */
        public List<StorageFile> getByFolderId(int folderId)
        {
            String sql = "select * from storageFile where folderId=@folderId order by id desc";
            SQLiteParameter[] parameters = {
                       new SQLiteParameter("@folderId", DbType.Int32,4)
                      };
            parameters[0].Value = folderId;

            DataTable dt = Common.SQLiteHelper.ExecuteQuery(sql, parameters);

            List<StorageFile> list = DataToEntity<StorageFile>.FillModel(dt);
            return list;
        }

        public int upate(StorageFile entity)
        { 
            string sql = "update storageFile set origFilename=@origFilename,url=@url,size=@size"
                   + ",ext=@ext,createTime=@createTime"
                   + ",folderId=@folderId,duration=@duration"
                   + "  where id=@id";

            SQLiteParameter[] parameters = {
                       new SQLiteParameter("@origFilename", DbType.String,100),
                       new SQLiteParameter("@url", DbType.String,255),
                       new SQLiteParameter("@size", DbType.String,4),
                       new SQLiteParameter("@ext", DbType.String,10),
                       new SQLiteParameter("@createTime", DbType.String,30),
                       new SQLiteParameter("@folderId", DbType.Int32,4), 
                       new SQLiteParameter("@duration", DbType.Int32,4),
                       new SQLiteParameter("@id", DbType.Int32,4)
                      };
            parameters[0].Value = entity.origFilename;
            parameters[1].Value = entity.url;
            parameters[2].Value = entity.size;
            parameters[3].Value = entity.ext;

            parameters[4].Value = entity.createTime;
            parameters[5].Value = entity.folderId;
            parameters[6].Value = entity.duration;
            parameters[7].Value = entity.id;

            int result = Common.SQLiteHelper.ExecuteNonQuery(sql, parameters);
            return result;
        }




        /*
        * 获取页面列表
        */
        public List<StorageFile> getNextPage(int currPage, int pageSize, int folderId)
        {
            int start = (currPage - 1) * pageSize;
            String sql = "select * from storageFile where folderId=@folderId  order by id desc limit @start,@pageSize";
            SQLiteParameter[] parameters = {
                       new SQLiteParameter("@start", DbType.Int32,4),
                       new SQLiteParameter("@pageSize", DbType.Int32,4),
                       new SQLiteParameter("@folderId", DbType.Int32,4)
                      };
            parameters[0].Value = start;
            parameters[1].Value = pageSize;
            parameters[2].Value = folderId;

            DataTable dt = Common.SQLiteHelper.ExecuteQuery(sql, parameters);
            List<StorageFile> list = DataToEntity<StorageFile>.FillModel(dt);
            return list;
        }


        public List<StorageFile> getNextPageOfWord(int currPage, int pageSize, int folderId)
        {
            int start = (currPage - 1) * pageSize;
            String sql = "select * from storageFile where   folderId=@folderId  order by id desc limit @start,@pageSize";
            SQLiteParameter[] parameters = {
                       new SQLiteParameter("@start", DbType.Int32,4),
                       new SQLiteParameter("@pageSize", DbType.Int32,4),
                       new SQLiteParameter("@folderId", DbType.Int32,4)
                      };
            parameters[0].Value = start;
            parameters[1].Value = pageSize;
            parameters[2].Value = folderId;

            DataTable dt = Common.SQLiteHelper.ExecuteQuery(sql, parameters);
            List<StorageFile> list = DataToEntity<StorageFile>.FillModel(dt);
            return list;
        }

        /*
         * 获取文件总数
         */
        public int getCount(int folderId)
        {
            String sql = "select count(*) from storageFile where folderId=@folderId";
            SQLiteParameter[] parameters = new SQLiteParameter[1];
            parameters[0] = new SQLiteParameter("@folderId", DbType.Int32, 4);
            parameters[0].Value = folderId;
            object obj = Common.SQLiteHelper.ExecuteScalar(sql, parameters);


            return Convert.ToInt32(obj);
        }

        /*
         * 删除文件
         */
        public int delete(int id)
        {
            String sql = "delete from storageFile where id=@id";
            SQLiteParameter[] parameters = {
                       new SQLiteParameter("@id", DbType.Int32,4)
                      };
            parameters[0].Value = id;

            Int32 rows = Common.SQLiteHelper.ExecuteNonQuery(sql, parameters);

            return rows;
        }
    }
}
