using Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;

namespace Dal
{
    public class StorageImageDal
    {
        //1、插入图片
        public StorageImage insert(StorageImage entity)
        {
            String sql = "insert into storageImage(origFilename,url,ext,size,createTime,actualWidth,actualHeight,folderId) values(@origFilename,@url,@ext,@size,@createTime,@actualWidth,@actualHeight,@folderId);select last_insert_rowid();";
            SQLiteParameter[] parameters = {
                       new SQLiteParameter("@origFilename", DbType.String,100),
                       new SQLiteParameter("@url", DbType.String,255),
                       new SQLiteParameter("@ext", DbType.String,10),
                       new SQLiteParameter("@size", DbType.Int32,4),
                       new SQLiteParameter("@createTime", DbType.String,30),
                       new SQLiteParameter("@actualWidth", DbType.String,4),
                       new SQLiteParameter("@actualHeight", DbType.Int32,4),
                       new SQLiteParameter("@folderId", DbType.Int32,4)
                      };
            parameters[0].Value = entity.origFilename;
            parameters[1].Value = entity.url;
            parameters[2].Value = entity.ext;
            parameters[3].Value = entity.size;
            parameters[4].Value = entity.createTime;
            parameters[5].Value = entity.actualWidth;
            parameters[6].Value = entity.actualHeight;
            parameters[7].Value = entity.folderId;

            DataTable dt = Common.SQLiteHelper.ExecuteQuery(sql, parameters);
            int id = DataType.ToInt32(dt.Rows[0]["last_insert_rowid()"].ToString());
            entity.id = id;
            return entity;
        }




        /*
* 获取下一页
*/
        public List<StorageImage> getNextPage(int currPage, int pageSize, int folderId)
        {
            int start = (currPage - 1) * pageSize;


            String sql = "select * from storageImage where folderId=@folderId order by id desc limit @start,@pageSize";
            SQLiteParameter[] parameters = {
                       new SQLiteParameter("@start", DbType.Int32,4),
                       new SQLiteParameter("@pageSize", DbType.Int32,4),
                       new SQLiteParameter("@folderId",DbType.Int32,4)
                      };
            parameters[0].Value = start;
            parameters[1].Value = pageSize;
            parameters[2].Value = folderId;

            DataTable dt = Common.SQLiteHelper.ExecuteQuery(sql, parameters);

            List<StorageImage> list = DataToEntity<StorageImage>.FillModel(dt);
            return list;
        }

        public List<StorageImage> getNextPage2(int currPage, int pageSize)
        {
            int start = (currPage - 1) * pageSize;
            int end = currPage * pageSize;

            String sql = "select * from storageImage   order by id desc limit @start,@end";
            SQLiteParameter[] parameters = new SQLiteParameter[2];
            parameters[0] = new SQLiteParameter("@start", DbType.Int32, 4);
            parameters[1] = new SQLiteParameter("@end", DbType.Int32, 4);
            parameters[0].Value = start;
            parameters[1].Value = end;

            DataTable dt = Common.SQLiteHelper.ExecuteQuery(sql, parameters);

            List<StorageImage> list = DataToEntity<StorageImage>.FillModel(dt);
            return list;
        }

        /*
         * 更新
         */
        public int upate(StorageImage entity)
        {

            string sql = "update storageImage set origFilename=@origFilename,url=@url,size=@size"
                  + ",ext=@ext,createTime=@createTime,actualWidth=@actualWidth"
                  + ",actualHeight=@actualHeight,folderId=@folderId"
                  + "  where id=@id";

            SQLiteParameter[] parameters = {
                       new SQLiteParameter("@origFilename", DbType.String,100),
                       new SQLiteParameter("@url", DbType.String,255),
                       new SQLiteParameter("@size", DbType.Int32,4),
                       new SQLiteParameter("@ext", DbType.String,10),
                       new SQLiteParameter("@createTime", DbType.String,30),
                       new SQLiteParameter("@actualWidth", DbType.Int32,4),
                       new SQLiteParameter("@actualHeight", DbType.Int32,4),
                       new SQLiteParameter("@folderId", DbType.Int32,4),
                       new SQLiteParameter("@id", DbType.Int32,4)
                      };
            parameters[0].Value = entity.origFilename;
            parameters[1].Value = entity.url;
            parameters[2].Value = entity.size;
            parameters[3].Value = entity.ext;

            parameters[4].Value = entity.createTime;
            parameters[5].Value = entity.actualWidth;
            parameters[6].Value = entity.actualHeight;
            parameters[7].Value = entity.folderId;
            parameters[8].Value = entity.id;

            int result = Common.SQLiteHelper.ExecuteNonQuery(sql, parameters);
            return result;
        }

        /*
         * 获取文件夹下所有图片
         */
        public List<StorageImage> getByFolderId(int folderId)
        {
            String sql = "select * from storageImage where folderId=@folderId order by id desc";
            SQLiteParameter[] parameters = {
                       new SQLiteParameter("@folderId", DbType.Int32,4)
                      };
            parameters[0].Value = folderId;

            DataTable dt = Common.SQLiteHelper.ExecuteQuery(sql, parameters);

            List<StorageImage> list = DataToEntity<StorageImage>.FillModel(dt);
            return list;
        }




        //获取图片
        public StorageImage get(Int32 id)
        {
            String sql = "select * from storageImage where id=@id";
            SQLiteParameter[] parameters = {
                       new SQLiteParameter("@id", DbType.Int32,4)
                      };
            parameters[0].Value = id;

            DataTable dt = Common.SQLiteHelper.ExecuteQuery(sql, parameters);
            if (dt == null || dt.Rows.Count == 0)
            {
                return null;
            }
            StorageImage storageImage = DataToEntity<StorageImage>.FillModel(dt.Rows[0]);
            return storageImage;
        }

        /*
         * 删除
         */
        public int delete(int id)
        {
            String sql = "delete from storageImage where id=@id";
            SQLiteParameter[] parameters = {
                       new SQLiteParameter("@id", DbType.Int32,4)
                      };
            parameters[0].Value = id;

            Int32 rows = Common.SQLiteHelper.ExecuteNonQuery(sql, parameters);

            return rows;
        }

        /*
         * 获取图片总数
         */
        public int getCount(int folderId)
        {
            String sql = "select count(*) from storageImage where folderId=@folderId";
            SQLiteParameter[] parameters = new SQLiteParameter[1];
            parameters[0] = new SQLiteParameter("@folderId", DbType.Int32, 4);
            parameters[0].Value = folderId;
            object obj = Common.SQLiteHelper.ExecuteScalar(sql, parameters);


            return Convert.ToInt32(obj);
        }
    }
}
