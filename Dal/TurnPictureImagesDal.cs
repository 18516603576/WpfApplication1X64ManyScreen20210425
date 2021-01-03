using Model;
using Model.dto;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;

namespace Dal
{
    public class TurnPictureImagesDal
    {
        //1、插入图片
        public TurnPictureImages insert(TurnPictureImages entity)
        {
            String sql = "insert into turnPictureImages(dControlId,storageImageId,isLink) values(@dControlId,@storageImageId,@isLink);select last_insert_rowid();";
            SQLiteParameter[] parameters = {
                       new SQLiteParameter("@dControlId", DbType.Int32,4),
                       new SQLiteParameter("@storageImageId", DbType.Int32,4),
                       new SQLiteParameter("@isLink", DbType.Int32,4) 
                      };
            parameters[0].Value = entity.dControlId;
            parameters[1].Value = entity.storageImageId;
            parameters[2].Value = entity.isLink; 

            DataTable dt = Common.SQLiteHelper.ExecuteQuery(sql, parameters);
            int id = DataType.ToInt32(dt.Rows[0]["last_insert_rowid()"].ToString());
            entity.id = id;
            return entity;
        }
        //获取图片列表
        public List<TurnPictureImagesDto> getByDControlId(int dControlId)
        {
            String sql = "select a.*,b.url from turnPictureImages a left join storageImage b on a.storageImageId=b.id where a.dControlId=@dControlId order by a.id asc";
            SQLiteParameter[] parameters = {
                       new SQLiteParameter("@dControlId", DbType.Int32,4)
                      };
            parameters[0].Value = dControlId;

            DataTable dt = Common.SQLiteHelper.ExecuteQuery(sql, parameters);
            if (dt == null)
            {
                return new List<TurnPictureImagesDto>();
            }
            List<TurnPictureImagesDto> list = DataToEntity<TurnPictureImagesDto>.FillModel(dt);
            return list;
        }

        /*
         *  删除相册下的图片
         */
        public int deleteByDControlId(int dControlId)
        {
            String sql = "delete from turnPictureImages where dControlId=@dControlId";
            SQLiteParameter[] parameters = {
                       new SQLiteParameter("@dControlId", DbType.Int32,4)
                      };
            parameters[0].Value = dControlId;

            Int32 rows = Common.SQLiteHelper.ExecuteNonQuery(sql, parameters);

            return rows;
        }

        public TurnPictureImages get(int id)
        {
            String sql = "select * from turnPictureImages where id=@id";
            SQLiteParameter[] parameters = {
                       new SQLiteParameter("@id", DbType.Int32,4)
                      };
            parameters[0].Value = id;

            DataTable dt = Common.SQLiteHelper.ExecuteQuery(sql, parameters);

            TurnPictureImages entity = DataToEntity<TurnPictureImages>.FillModel(dt.Rows[0]);
            return entity;
        }

        public int update(TurnPictureImages entity)
        {
            string sql = "update turnPictureImages set dControlId=@dControlId,storageImageId=@storageImageId,isLink=@isLink"
               + "  where id=@id";

            SQLiteParameter[] parameters = {
                       new SQLiteParameter("@dControlId", DbType.Int32,4),
                       new SQLiteParameter("@storageImageId", DbType.Int32,4),
                       new SQLiteParameter("@isLink", DbType.Int32,4),
                       new SQLiteParameter("@id", DbType.Int32,4)
                      };
            parameters[0].Value = entity.dControlId;
            parameters[1].Value = entity.storageImageId;
            parameters[2].Value = entity.isLink; 
            parameters[3].Value = entity.id;

            int result = Common.SQLiteHelper.ExecuteNonQuery(sql, parameters);
            return result;
        }

        /*
        * 删除一行
        */
        public int delete(int id)
        {
            String sql = "delete from turnPictureImages where id=@id";
            SQLiteParameter[] parameters = {
                       new SQLiteParameter("@id", DbType.Int32,4)
                      };
            parameters[0].Value = id;

            Int32 rows = Common.SQLiteHelper.ExecuteNonQuery(sql, parameters);
            return rows;
        }
    }
}
