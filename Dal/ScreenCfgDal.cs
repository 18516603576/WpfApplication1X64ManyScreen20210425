using Model;
using Model.dto;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;

namespace Dal
{
    public class ScreenCfgDal
    {
        //通过id获取cfg
        public ScreenCfg get(int id)
        {

            String sql = "select * from screenCfg where id=@id";
            SQLiteParameter[] parameters = {
                       new SQLiteParameter("@id", DbType.Int32,4)
                      };
            parameters[0].Value = id; 
            DataTable dt = Common.SQLiteHelper.ExecuteQuery(sql, parameters);

            ScreenCfg screenCfg = DataToEntity<ScreenCfg>.FillModel(dt.Rows[0]);
            return screenCfg;
        }
        /*
         * 获取所有
         */
        public List<ScreenCfg> findAll()
        {
            String sql = "select * from screenCfg order by  id asc";
            SQLiteParameter[] parameters = {
                      
                      }; 

            DataTable dt = Common.SQLiteHelper.ExecuteQuery(sql, parameters);

            List<ScreenCfg> list = DataToEntity<ScreenCfg>.FillModel(dt);
            return list;
        }

        /*
        * 获取所有
        */
        public List<ScreenCfgDto> findAllWithPageName()
        {
            String sql = "select screenCfg.*,name  pageName from screenCfg left join dPage on screenCfg.indexPageId=dPage.id order by  id asc";
            SQLiteParameter[] parameters = {

                      };

            DataTable dt = Common.SQLiteHelper.ExecuteQuery(sql, parameters);

            List<ScreenCfgDto> list = DataToEntity<ScreenCfgDto>.FillModel(dt);
            return list;
        }

        public int delete(int id)
        {
            String sql = "delete from screenCfg where id=@id";
            SQLiteParameter[] parameters = {
                       new SQLiteParameter("@id", DbType.Int32,4)
                      };
            parameters[0].Value = id;

            Int32 rows = Common.SQLiteHelper.ExecuteNonQuery(sql, parameters);

            return rows;
        }

        public ScreenCfg insert(ScreenCfg entity)
        { 
        String sql = "insert into screenCfg(deviceName,diyName,width,height,isPrimary,indexPageId) "
                + "values(@deviceName,@diyName,@width,@height,@isPrimary,@indexPageId);select last_insert_rowid();";
            SQLiteParameter[] parameters = { 
                       new SQLiteParameter("@deviceName", DbType.String,100),
                       new SQLiteParameter("@diyName", DbType.String,100),
                       new SQLiteParameter("@width", DbType.Int32,4),
                       new SQLiteParameter("@height", DbType.Int32,4),
                       new SQLiteParameter("@isPrimary", DbType.Int32,4),
                       new SQLiteParameter("@indexPageId", DbType.Int32,4) 
                      };
            parameters[0].Value = entity.deviceName;
            parameters[1].Value = entity.diyName;
            parameters[2].Value = entity.width;
            parameters[3].Value = entity.height;
            parameters[4].Value = entity.isPrimary;
            parameters[5].Value = entity.indexPageId; 

            DataTable dt = Common.SQLiteHelper.ExecuteQuery(sql, parameters);
            int id = DataType.ToInt32(dt.Rows[0]["last_insert_rowid()"].ToString());
            entity.id = id;
            return entity;
        }

        /*
         * 更新配置
         */
        public int update(ScreenCfg entity)
        {
            string sql = "update screenCfg set deviceName=@deviceName,diyName=@diyName,width=@width"
                 + ",height=@height,isPrimary=@isPrimary,indexPageId=@indexPageId"
                 + "  where id=@id";

            SQLiteParameter[] parameters = {
                       new SQLiteParameter("@deviceName", DbType.String,255),
                       new SQLiteParameter("@diyName", DbType.String,255),
                       new SQLiteParameter("@width", DbType.Int32,4),
                       new SQLiteParameter("@height", DbType.Int32,4),
                       new SQLiteParameter("@isPrimary", DbType.Int32,4),
                       new SQLiteParameter("@indexPageId", DbType.Int32,4),
                       new SQLiteParameter("@id", DbType.Int32,4)
                      };
            parameters[0].Value = entity.deviceName;
            parameters[1].Value = entity.diyName;
            parameters[2].Value = entity.width;
            parameters[3].Value = entity.height;
            parameters[4].Value = entity.isPrimary;
            parameters[5].Value = entity.indexPageId;
            parameters[6].Value = entity.id;
            int result = Common.SQLiteHelper.ExecuteNonQuery(sql, parameters);
            return result;
        }
    }
}
