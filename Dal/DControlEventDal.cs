using Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;

namespace Dal
{
    public class DControlEventDal
    {
        //获取页面下的所有控件
        public List<DControlEvent> getByDControlId(Int32 dControlId)
        {
            String sql = "select * from dControlEvent where dControlId=@dControlId order by id asc";
            SQLiteParameter[] parameters = {
                       new SQLiteParameter("@dControlId", DbType.Int32,4)
                      };
            parameters[0].Value = dControlId; 
            DataTable dt = Common.SQLiteHelper.ExecuteQuery(sql, parameters);

            List<DControlEvent> list = DataToEntity<DControlEvent>.FillModel(dt);
            return list;
        }

        public List<DControlEvent> getByTurnPictureImagesId(int turnPictureImagesId)
        {
            String sql = "select * from dControlEvent where turnPictureImagesId=@turnPictureImagesId order by id asc";
            SQLiteParameter[] parameters = {
                       new SQLiteParameter("@turnPictureImagesId", DbType.Int32,4)
                      };
            parameters[0].Value = turnPictureImagesId;
            DataTable dt = Common.SQLiteHelper.ExecuteQuery(sql, parameters);

            List<DControlEvent> list = DataToEntity<DControlEvent>.FillModel(dt);
            return list;
        }

        /*
         * 插入
         */
        public DControlEvent insert(DControlEvent entity)
        {
            String sql = "insert into dControlEvent(dControlId,turnPictureImagesId,screenCfgId,linkToPageId,isDialogLink,showInWhichCFrame,isTransparentDialog) ";
            sql = sql + " values(@dControlId,@turnPictureImagesId,@screenCfgId,@linkToPageId,@isDialogLink,@showInWhichCFrame,@isTransparentDialog);select last_insert_rowid();";
            SQLiteParameter[] parameters = {
                      new SQLiteParameter("@dControlId", DbType.Int32,4),
                       new SQLiteParameter("@turnPictureImagesId", DbType.Int32,4),
                       new SQLiteParameter("@screenCfgId", DbType.Int32,4),

                       new SQLiteParameter("@linkToPageId", DbType.Int32,4),
                       new SQLiteParameter("@isDialogLink", DbType.Int32,4),
                       new SQLiteParameter("@showInWhichCFrame", DbType.Int32,4),
                       new SQLiteParameter("@isTransparentDialog", DbType.Int32,4) 
                      };
            parameters[0].Value = entity.dControlId;
            parameters[1].Value = entity.turnPictureImagesId;
            parameters[2].Value = entity.screenCfgId;
            parameters[3].Value = entity.linkToPageId;
            parameters[4].Value = entity.isDialogLink;
            parameters[5].Value = entity.showInWhichCFrame;
            parameters[6].Value = entity.isTransparentDialog;
           

            DataTable dt = Common.SQLiteHelper.ExecuteQuery(sql, parameters);
            int id = DataType.ToInt32(dt.Rows[0]["last_insert_rowid()"].ToString());
            entity.id = id;
            return entity;
        }

      
        /*
         * 更新
         */
        public int update(DControlEvent entity)
        {
            string sql = "update dControlEvent set dControlId=@dControlId,turnPictureImagesId=@turnPictureImagesId,screenCfgId=@screenCfgId"
                + ",linkToPageId=@linkToPageId,isDialogLink=@isDialogLink"
                + ",showInWhichCFrame=@showInWhichCFrame,isTransparentDialog=@isTransparentDialog"
                + "  where id=@id";

            SQLiteParameter[] parameters = {
                       new SQLiteParameter("@dControlId", DbType.Int32,4),
                       new SQLiteParameter("@turnPictureImagesId", DbType.Int32,4),
                       new SQLiteParameter("@screenCfgId", DbType.Int32,4),

                       new SQLiteParameter("@linkToPageId", DbType.Int32,4),
                       new SQLiteParameter("@isDialogLink", DbType.Int32,4),
                       new SQLiteParameter("@showInWhichCFrame", DbType.Int32,4),
                       new SQLiteParameter("@isTransparentDialog", DbType.Int32,4), 
                       new SQLiteParameter("@id", DbType.Int32,4)
                      };
            parameters[0].Value = entity.dControlId;
            parameters[1].Value = entity.turnPictureImagesId;
            parameters[2].Value = entity.screenCfgId;

            parameters[3].Value = entity.linkToPageId;
            parameters[4].Value = entity.isDialogLink;
            parameters[5].Value = entity.showInWhichCFrame;
            parameters[6].Value = entity.isTransparentDialog;
            parameters[7].Value = entity.id;
             
            int result = Common.SQLiteHelper.ExecuteNonQuery(sql, parameters);
            return result;
        }

        /*
         * 获取控件在某个屏幕对应的跳转
         */
        public DControlEvent getByDControlIdScreenCfgId(int dControlId, int screenCfgId)
        {
            String sql = "select * from dControlEvent where dControlId=@dControlId and screenCfgId=@screenCfgId ";
            SQLiteParameter[] parameters = {
                       new SQLiteParameter("@dControlId", DbType.Int32,4),
                       new SQLiteParameter("@screenCfgId", DbType.Int32,4)
                      };
            parameters[0].Value = dControlId;
            parameters[1].Value = screenCfgId;
            DataTable dt = Common.SQLiteHelper.ExecuteQuery(sql, parameters); 
            if (dt == null || dt.Rows.Count<=0) return null;

            DControlEvent entity = DataToEntity<DControlEvent>.FillModel(dt.Rows[0]);
            return entity;

             
        }
        /*
         * 获取轮播图片在某个屏幕对应的跳转
         */
        public DControlEvent getByTurnPictureImagesIdScreenCfgId(int turnPictureImagesId, int screenCfgId)
        {
            String sql = "select * from dControlEvent where turnPictureImagesId=@turnPictureImagesId and screenCfgId=@screenCfgId ";
            SQLiteParameter[] parameters = {
                       new SQLiteParameter("@turnPictureImagesId", DbType.Int32,4),
                       new SQLiteParameter("@screenCfgId", DbType.Int32,4)
                      };
            parameters[0].Value = turnPictureImagesId;
            parameters[1].Value = screenCfgId;
            DataTable dt = Common.SQLiteHelper.ExecuteQuery(sql, parameters);
            if (dt == null || dt.Rows.Count <= 0) return null;

            DControlEvent entity = DataToEntity<DControlEvent>.FillModel(dt.Rows[0]);
            return entity;
        }

    }
}
