using Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;

namespace Dal
{
    public class StorageVideoDal
    {
        //1、插入图片
        public StorageVideo insert(StorageVideo entity)
        {
            String sql = "insert into StorageVideo(origFilename,url,ext,size,createTime,storageImageId,duration,folderId) values(@origFilename,@url,@ext,@size,@createTime,@storageImageId,@duration,@folderId);select last_insert_rowid();";
            SQLiteParameter[] parameters = {
                       new SQLiteParameter("@origFilename", DbType.String,100),
                       new SQLiteParameter("@url", DbType.String,255),
                       new SQLiteParameter("@ext", DbType.String,10),
                       new SQLiteParameter("@size", DbType.Int32,4),
                       new SQLiteParameter("@createTime", DbType.String,30),
                       new SQLiteParameter("@storageImageId", DbType.Int32,4),
                       new SQLiteParameter("@duration", DbType.Int32,4),
                       new SQLiteParameter("@folderId", DbType.Int32,4)
                      };
            parameters[0].Value = entity.origFilename;
            parameters[1].Value = entity.url;
            parameters[2].Value = entity.ext;
            parameters[3].Value = entity.size;
            parameters[4].Value = entity.createTime;
            parameters[5].Value = entity.storageImageId;
            parameters[6].Value = entity.duration;
            parameters[7].Value = entity.folderId;

            DataTable dt = Common.SQLiteHelper.ExecuteQuery(sql, parameters);
            int id = DataType.ToInt32(dt.Rows[0]["last_insert_rowid()"].ToString());
            entity.id = id;
            return entity;
        }
        //2.获取下一页视频
        public List<StorageVideo> getNextPage(int currPage, int pageSize, int folderId)
        {
            int start = (currPage - 1) * pageSize;
            int end = currPage * pageSize;

            String sql = "select * from storageVideo where folderId=@folderId order by id desc limit @start,@end";
            SQLiteParameter[] parameters = new SQLiteParameter[3];
            parameters[0] = new SQLiteParameter("@start", DbType.Int32, 4);
            parameters[1] = new SQLiteParameter("@end", DbType.Int32, 4);
            parameters[2] = new SQLiteParameter("@folderId", DbType.Int32, 4);
            parameters[0].Value = start;
            parameters[1].Value = end;
            parameters[2].Value = folderId;


            DataTable dt = Common.SQLiteHelper.ExecuteQuery(sql, parameters);

            List<StorageVideo> list = DataToEntity<StorageVideo>.FillModel(dt);
            return list;
        }



        public int upate(StorageVideo entity)
        {

            string sql = "update storageVideo set origFilename=@origFilename,url=@url,size=@size"
                  + ",ext=@ext,createTime=@createTime,duration=@duration"
                  + ",storageImageId=@storageImageId,folderId=@folderId"
                  + "  where id=@id";

            SQLiteParameter[] parameters = {
                       new SQLiteParameter("@origFilename", DbType.String,100),
                       new SQLiteParameter("@url", DbType.String,255),
                       new SQLiteParameter("@size", DbType.Int32,4),
                       new SQLiteParameter("@ext", DbType.String,10),
                       new SQLiteParameter("@createTime", DbType.String,30),
                       new SQLiteParameter("@duration", DbType.Int32,4),
                       new SQLiteParameter("@storageImageId", DbType.Int32,4),
                       new SQLiteParameter("@folderId", DbType.Int32,4),
                       new SQLiteParameter("@id", DbType.Int32,4)
                      };
            parameters[0].Value = entity.origFilename;
            parameters[1].Value = entity.url;
            parameters[2].Value = entity.size;
            parameters[3].Value = entity.ext;

            parameters[4].Value = entity.createTime;
            parameters[5].Value = entity.duration;
            parameters[6].Value = entity.storageImageId;
            parameters[7].Value = entity.folderId;
            parameters[8].Value = entity.id;

            int result = Common.SQLiteHelper.ExecuteNonQuery(sql, parameters);
            return result;
        }

        public List<StorageVideo> getNextPage2(int currPage, int pageSize)
        {
            int start = (currPage - 1) * pageSize;
            int end = currPage * pageSize;

            String sql = "select * from storageVideo   order by id desc limit @start,@end";
            SQLiteParameter[] parameters = new SQLiteParameter[2];
            parameters[0] = new SQLiteParameter("@start", DbType.Int32, 4);
            parameters[1] = new SQLiteParameter("@end", DbType.Int32, 4);
            parameters[0].Value = start;
            parameters[1].Value = end;


            DataTable dt = Common.SQLiteHelper.ExecuteQuery(sql, parameters);

            List<StorageVideo> list = DataToEntity<StorageVideo>.FillModel(dt);
            return list;
        }

        /*
         * 获取文件夹下的视频
         */
        public List<StorageVideo> getByFolderId(int folderId)
        {
            String sql = "select * from storageVideo where folderId=@folderId order by id desc";
            SQLiteParameter[] parameters = {
                       new SQLiteParameter("@folderId", DbType.Int32,4)
                      };
            parameters[0].Value = folderId;

            DataTable dt = Common.SQLiteHelper.ExecuteQuery(sql, parameters);

            List<StorageVideo> list = DataToEntity<StorageVideo>.FillModel(dt);
            return list;
        }

        public int delete(int id)
        {
            String sql = "delete from storageVideo where id=@id";
            SQLiteParameter[] parameters = {
                       new SQLiteParameter("@id", DbType.Int32,4)
                      };
            parameters[0].Value = id;

            Int32 rows = Common.SQLiteHelper.ExecuteNonQuery(sql, parameters);

            return rows;
        }



        //获取视频
        public StorageVideo get(int id)
        {
            String sql = "select * from storageVideo where id=@id";
            SQLiteParameter[] parameters = {
                       new SQLiteParameter("@id", DbType.Int32,4)
                      };
            parameters[0].Value = id;

            DataTable dt = Common.SQLiteHelper.ExecuteQuery(sql, parameters);
            if (dt == null || dt.Rows.Count == 0)
            {
                return null;
            }
            StorageVideo dControl = DataToEntity<StorageVideo>.FillModel(dt.Rows[0]);
            return dControl;
        }
        /*
         * 获取视频总数
         */
        public int getCount(int folderId)
        {
            String sql = "select count(*) from storageVideo where folderId=@folderId";
            SQLiteParameter[] parameters = new SQLiteParameter[1];
            parameters[0] = new SQLiteParameter("@folderId", DbType.Int32, 4);
            parameters[0].Value = folderId;

            object obj = Common.SQLiteHelper.ExecuteScalar(sql, parameters);
            return Convert.ToInt32(obj);
        }

    }
}
