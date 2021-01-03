using Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;

namespace Dal
{
    public class DControlDal
    {
        //获取页面下的所有控件
        public List<DControl> getByPageId(Int32 pageId)
        {
            String sql = "select * from dControl where pageId=@pageId and parentId=0 order by idx asc";
            SQLiteParameter[] parameters = {
                       new SQLiteParameter("@pageId", DbType.Int32,4)
                      };
            parameters[0].Value = pageId;

            DataTable dt = Common.SQLiteHelper.ExecuteQuery(sql, parameters);

            List<DControl> list = DataToEntity<DControl>.FillModel(dt);
            return list;
        }

        /*
         * 删除单个控件
         */
        public int delete(int id)
        {
            String sql = "delete from dControl where id=@id";
            SQLiteParameter[] parameters = {
                       new SQLiteParameter("@id", DbType.Int32,4)
                      };
            parameters[0].Value = id;

            Int32 rows = Common.SQLiteHelper.ExecuteNonQuery(sql, parameters);

            return rows;
        }

        /*
         * 添加一个控件
         */
        public DControl insert(DControl entity)
        {


            String sql = "insert into dControl(pageId,name,content,width,height,left,top,type,idx,linkToPageId,isClickShow,linkToVideoId,autoplay,loop,turnPictureSpeed,linkToWeb,storageId,isShowTurnPictureArrow,opacity,parentId,contentWidth,contentHeight,backgroundImageId,isTab,rowNum,spacing,rotateAngle,isDialogLink,showInWhichCFrame,isTransparentDialog,fontFamily,fontSize,fontLineHeight,fontColor,fontWeight,fontTextAlignment,storageIdOfCover,screenCfgId,isHideVideoConsoleOfFirstLoad) ";
            sql = sql + " values(@pageId,@name,@content,@width,@height,@left,@top,@type,@idx,@linkToPageId,@isClickShow,@linkToVideoId,@autoplay,@loop,@turnPictureSpeed,@linkToWeb,@storageId,@isShowTurnPictureArrow,@opacity,@parentId,@contentWidth,@contentHeight,@backgroundImageId,@isTab,@rowNum,@spacing,@rotateAngle,@isDialogLink,@showInWhichCFrame,@isTransparentDialog,@fontFamily,@fontSize,@fontLineHeight,@fontColor,@fontWeight,@fontTextAlignment,@storageIdOfCover,@screenCfgId,@isHideVideoConsoleOfFirstLoad);select last_insert_rowid();";
            SQLiteParameter[] parameters = {
                       new SQLiteParameter("@pageId", DbType.Int32,4),
                       new SQLiteParameter("@name", DbType.String,30),
                       new SQLiteParameter("@content", DbType.String,4000),
                       new SQLiteParameter("@width", DbType.Int32,4),
                       new SQLiteParameter("@height", DbType.Int32,4),
                       new SQLiteParameter("@left", DbType.Int32,4),
                       new SQLiteParameter("@top", DbType.Int32,4),
                       new SQLiteParameter("@type", DbType.String,30),
                       //new SQLiteParameter("@url", DbType.String,255),
                       //new SQLiteParameter("@imgs", DbType.String,4000),
                       new SQLiteParameter("@idx", DbType.Int32,4),
                       new SQLiteParameter("@linkToPageId", DbType.Int32,4),
                       new SQLiteParameter("@isClickShow", DbType.Int32,4),
                       new SQLiteParameter("@linkToVideoId", DbType.Int32,4),
                       new SQLiteParameter("@autoplay", DbType.Int32,4),
                       new SQLiteParameter("@loop", DbType.Int32,4),
                       new SQLiteParameter("@turnPictureSpeed", DbType.Int32,4),
                       new SQLiteParameter("@linkToWeb", DbType.String,255),
                       new SQLiteParameter("@storageId", DbType.Int32,4),
                       new SQLiteParameter("@isShowTurnPictureArrow", DbType.Int32,4),
                       new SQLiteParameter("@opacity", DbType.Int32,4),
                       new SQLiteParameter("@parentId", DbType.Int32,4),
                       new SQLiteParameter("@contentWidth", DbType.Int32,4),
                       new SQLiteParameter("@contentHeight", DbType.Int32,4),
                       new SQLiteParameter("@backgroundImageId", DbType.Int32,4),
                       new SQLiteParameter("@isTab", DbType.Int32,4),
                       new SQLiteParameter("@rowNum", DbType.Int32,4),
                       new SQLiteParameter("@spacing", DbType.Int32,4),
                       new SQLiteParameter("@rotateAngle", DbType.Int32,4),
                       new SQLiteParameter("@isDialogLink", DbType.Int32,4),
                       new SQLiteParameter("@showInWhichCFrame", DbType.Int32,4),
                       new SQLiteParameter("@isTransparentDialog", DbType.Int32,4),
                       new SQLiteParameter("@fontFamily", DbType.String,30),
                       new SQLiteParameter("@fontSize", DbType.Int32,4),
                       new SQLiteParameter("@fontLineHeight", DbType.Int32,4),
                       new SQLiteParameter("@fontColor", DbType.String,30),
                       new SQLiteParameter("@fontWeight", DbType.String,30),
                       new SQLiteParameter("@fontTextAlignment", DbType.Int32,4),
                       new SQLiteParameter("@storageIdOfCover", DbType.Int32,4),
                       new SQLiteParameter("@screenCfgId", DbType.Int32,4),
                       new SQLiteParameter("@isHideVideoConsoleOfFirstLoad", DbType.Int32,4)
                      };
            parameters[0].Value = entity.pageId;
            parameters[1].Value = entity.name;
            parameters[2].Value = entity.content;
            parameters[3].Value = entity.width;
            parameters[4].Value = entity.height;
            parameters[5].Value = entity.left;
            parameters[6].Value = entity.top;
            parameters[7].Value = entity.type;
            parameters[8].Value = entity.idx;
            parameters[9].Value = entity.linkToPageId;
            parameters[10].Value = entity.isClickShow;
            parameters[11].Value = entity.linkToVideoId;
            parameters[12].Value = entity.autoplay;
            parameters[13].Value = entity.loop;
            parameters[14].Value = entity.turnPictureSpeed;
            parameters[15].Value = entity.linkToWeb;
            parameters[16].Value = entity.storageId;
            parameters[17].Value = entity.isShowTurnPictureArrow;
            parameters[18].Value = entity.opacity;

            parameters[19].Value = entity.parentId;
            parameters[20].Value = entity.contentWidth;
            parameters[21].Value = entity.contentHeight;
            parameters[22].Value = entity.backgroundImageId;
            parameters[23].Value = entity.isTab;
            parameters[24].Value = entity.rowNum;
            parameters[25].Value = entity.spacing;
            parameters[26].Value = entity.rotateAngle;
            parameters[27].Value = entity.isDialogLink;
            parameters[28].Value = entity.showInWhichCFrame;
            parameters[29].Value = entity.isTransparentDialog;

            parameters[30].Value = entity.fontFamily;
            parameters[31].Value = entity.fontSize;
            parameters[32].Value = entity.fontLineHeight;
            parameters[33].Value = entity.fontColor;
            parameters[34].Value = entity.fontWeight;
            parameters[35].Value = entity.fontTextAlignment;
            parameters[36].Value = entity.storageIdOfCover;
            parameters[37].Value = entity.screenCfgId;
            parameters[38].Value = entity.isHideVideoConsoleOfFirstLoad;

            DataTable dt = Common.SQLiteHelper.ExecuteQuery(sql, parameters);
            int id = DataType.ToInt32(dt.Rows[0]["last_insert_rowid()"].ToString());
            entity.id = id;
            return entity;
        }
        /*
        *  获取当前页面所关联的CFrame
        */
        public List<DControl> getParentCFrameByLinkToPageId(int pageId)
        {
            string sql = "select * from dControl where linkToPageId=@pageId and name='CFrame' order by idx asc";
            SQLiteParameter[] parameters = {
                       new SQLiteParameter("@pageId", DbType.Int32,4)
                      };
            parameters[0].Value = pageId;

            DataTable dt = Common.SQLiteHelper.ExecuteQuery(sql, parameters);

            List<DControl> list = DataToEntity<DControl>.FillModel(dt);
            return list;
        }

        /*
         * 获取页面下的所有CFrame控件
         */
        public List<DControl> getCFrameByPageId(int pageId)
        {
            String sql = "select * from dControl where pageId=@pageId and name='CFrame' order by idx asc";
            SQLiteParameter[] parameters = {
                       new SQLiteParameter("@pageId", DbType.Int32,4)
                      };
            parameters[0].Value = pageId;

            DataTable dt = Common.SQLiteHelper.ExecuteQuery(sql, parameters);

            List<DControl> list = DataToEntity<DControl>.FillModel(dt);
            return list;
        }


        /*
         * 获取滚动区下的控件
         */
        public List<DControl> getByParentId(int parentId)
        {
            String sql = "select * from dControl where parentId=@parentId order by idx asc";
            SQLiteParameter[] parameters = {
                       new SQLiteParameter("@parentId", DbType.Int32,4)
                      };
            parameters[0].Value = parentId;

            DataTable dt = Common.SQLiteHelper.ExecuteQuery(sql, parameters);

            List<DControl> list = DataToEntity<DControl>.FillModel(dt);
            return list;
        }

        /*
         * 获取页面中最大的排序idx
         */
        public int getMaxIdxByPageId(int pageId)
        {
            String sql = "select max(idx) as idx from dControl where pageId=@pageId and parentId=0";
            SQLiteParameter[] parameters = {
                       new SQLiteParameter("@pageId", DbType.Int32,4)
                      };
            parameters[0].Value = pageId;

            DataTable dt = Common.SQLiteHelper.ExecuteQuery(sql, parameters);

            if (dt == null || dt.Rows[0] == null) return 0;
            if (dt.Rows[0]["idx"] == DBNull.Value || dt.Rows[0]["idx"] == null) return 0;

            return Convert.ToInt32(dt.Rows[0]["idx"]);
        }

        /*
        * 获取滚动区域中最大的排序idx
        */
        public int getMaxIdxByParentId(int parentId)
        {
            String sql = "select max(idx) as idx from dControl where parentId=@parentId";
            SQLiteParameter[] parameters = {
                       new SQLiteParameter("@parentId", DbType.Int32,4)
                      };
            parameters[0].Value = parentId;

            DataTable dt = Common.SQLiteHelper.ExecuteQuery(sql, parameters);

            if (dt == null || dt.Rows.Count <= 0) return 0;
            if (dt.Rows[0]["idx"] == DBNull.Value || dt.Rows[0]["idx"] == null) return 0;

            return Convert.ToInt32(dt.Rows[0]["idx"]);
        }

        /*
         * 更新 
         */
        public Int32 update(DControl entity)
        {

            string sql = "update dControl set pageId=@pageId,name=@name,content=@content"
                + ",width=@width,height=@height"
                + ",left=@left,top=@top"
                + ",idx=@idx,linkToPageId=@linkToPageId"
                + ",isClickShow=@isClickShow,linkToVideoId=@linkToVideoId"
                + ",autoplay=@autoplay,loop=@loop,turnPictureSpeed=@turnPictureSpeed"
                + ",linkToWeb=@linkToWeb,storageId=@storageId,isShowTurnPictureArrow=@isShowTurnPictureArrow"
                + ",opacity=@opacity"
                + ",parentId=@parentId,contentWidth=@contentWidth,contentHeight=@contentHeight"
                + ",backgroundImageId=@backgroundImageId,isTab=@isTab"
                + ",rowNum=@rowNum,spacing=@spacing,rotateAngle=@rotateAngle,isDialogLink=@isDialogLink"
                + ",showInWhichCFrame=@showInWhichCFrame,isTransparentDialog=@isTransparentDialog"
                 + ",fontFamily=@fontFamily,fontSize=@fontSize,fontLineHeight=@fontLineHeight"
                 + ",fontColor=@fontColor,fontWeight=@fontWeight,fontTextAlignment=@fontTextAlignment"
                  + ",storageIdOfCover=@storageIdOfCover,screenCfgId=@screenCfgId,isHideVideoConsoleOfFirstLoad=@isHideVideoConsoleOfFirstLoad"

                + "  where id=@id";

            SQLiteParameter[] parameters = {
                       new SQLiteParameter("@pageId", DbType.Int32,4),
                       new SQLiteParameter("@name", DbType.String,30),
                       new SQLiteParameter("@content", DbType.String,4000),

                       new SQLiteParameter("@width", DbType.Int32,4),
                       new SQLiteParameter("@height", DbType.Int32,4),
                       new SQLiteParameter("@left", DbType.Int32,4),
                       new SQLiteParameter("@top", DbType.Int32,4),
                       new SQLiteParameter("@type", DbType.String,30),

                       new SQLiteParameter("@idx", DbType.Int32,4),
                       new SQLiteParameter("@linkToPageId", DbType.Int32,4),
                       new SQLiteParameter("@isClickShow", DbType.Int32,4),
                       new SQLiteParameter("@linkToVideoId", DbType.Int32,4),
                       new SQLiteParameter("@autoplay", DbType.Int32,4),
                       new SQLiteParameter("@loop", DbType.Int32,4),
                       new SQLiteParameter("@turnPictureSpeed", DbType.Int32,4),
                       new SQLiteParameter("@linkToWeb", DbType.String,255),
                       new SQLiteParameter("@storageId", DbType.Int32,4),
                       new SQLiteParameter("@isShowTurnPictureArrow", DbType.Int32,4),
                       new SQLiteParameter("@opacity", DbType.Int32,4),
                       new SQLiteParameter("@parentId", DbType.Int32,4),
                       new SQLiteParameter("@contentWidth", DbType.Int32,4),
                       new SQLiteParameter("@contentHeight", DbType.Int32,4),
                       new SQLiteParameter("@backgroundImageId", DbType.Int32,4),
                       new SQLiteParameter("@isTab", DbType.Int32,4),
                       new SQLiteParameter("@rowNum", DbType.Int32,4),
                       new SQLiteParameter("@spacing", DbType.Int32,4),
                       new SQLiteParameter("@rotateAngle", DbType.Int32,4),
                       new SQLiteParameter("@isDialogLink", DbType.Int32,4),
                       new SQLiteParameter("@showInWhichCFrame", DbType.Int32,4),
                       new SQLiteParameter("@isTransparentDialog", DbType.Int32,4),
                       new SQLiteParameter("@fontFamily", DbType.String,30),
                       new SQLiteParameter("@fontSize", DbType.Int32,4),
                       new SQLiteParameter("@fontLineHeight", DbType.Int32,4),
                       new SQLiteParameter("@fontColor", DbType.String,30),
                       new SQLiteParameter("@fontWeight", DbType.String,30),
                       new SQLiteParameter("@fontTextAlignment", DbType.Int32,4),
                       new SQLiteParameter("@storageIdOfCover", DbType.Int32,4),
                       new SQLiteParameter("@screenCfgId", DbType.Int32,4),
                       new SQLiteParameter("@isHideVideoConsoleOfFirstLoad", DbType.Int32,4),
                       new SQLiteParameter("@id", DbType.Int32,4)
                      };
            parameters[0].Value = entity.pageId;
            parameters[1].Value = entity.name;
            parameters[2].Value = entity.content;

            parameters[3].Value = entity.width;
            parameters[4].Value = entity.height;
            parameters[5].Value = entity.left;
            parameters[6].Value = entity.top;
            parameters[7].Value = entity.type;

            parameters[8].Value = entity.idx;
            parameters[9].Value = entity.linkToPageId;
            parameters[10].Value = entity.isClickShow;
            parameters[11].Value = entity.linkToVideoId;
            parameters[12].Value = entity.autoplay;
            parameters[13].Value = entity.loop;
            parameters[14].Value = entity.turnPictureSpeed;
            parameters[15].Value = entity.linkToWeb;
            parameters[16].Value = entity.storageId;
            parameters[17].Value = entity.isShowTurnPictureArrow;
            parameters[18].Value = entity.opacity;
            parameters[19].Value = entity.parentId;
            parameters[20].Value = entity.contentWidth;
            parameters[21].Value = entity.contentHeight;
            parameters[22].Value = entity.backgroundImageId;
            parameters[23].Value = entity.isTab;
            parameters[24].Value = entity.rowNum;
            parameters[25].Value = entity.spacing;
            parameters[26].Value = entity.rotateAngle;
            parameters[27].Value = entity.isDialogLink;
            parameters[28].Value = entity.showInWhichCFrame;
            parameters[29].Value = entity.isTransparentDialog;

            parameters[30].Value = entity.fontFamily;
            parameters[31].Value = entity.fontSize;
            parameters[32].Value = entity.fontLineHeight;
            parameters[33].Value = entity.fontColor;
            parameters[34].Value = entity.fontWeight;
            parameters[35].Value = entity.fontTextAlignment;
            parameters[36].Value = entity.storageIdOfCover;
            parameters[37].Value = entity.screenCfgId;
            parameters[38].Value = entity.isHideVideoConsoleOfFirstLoad;
            parameters[39].Value = entity.id;
            int result = Common.SQLiteHelper.ExecuteNonQuery(sql, parameters);
            return result;
        }

        /*
         * 获取控件
         */
        public DControl get(int id)
        {
            String sql = "select * from dControl where id=@id";
            SQLiteParameter[] parameters = {
                       new SQLiteParameter("@id", DbType.Int32,4)
                      };
            parameters[0].Value = id;

            DataTable dt = Common.SQLiteHelper.ExecuteQuery(sql, parameters);
            if (dt == null) return null;

            DControl dControl = DataToEntity<DControl>.FillModel(dt.Rows[0]);
            return dControl;
        }

        /*
        * 删除页面后，链接到当前页面的控件全部重置   linkToPageId=0
        */
        public int updateLinkToPageId(int pageId)
        {
            string sql = "update dControl set linkToPageId=0"
                + "  where linkToPageId=@linkToPageId";

            SQLiteParameter[] parameters = {
                       new SQLiteParameter("@linkToPageId", DbType.Int32,4)
                      };
            parameters[0].Value = pageId;

            int result = Common.SQLiteHelper.ExecuteNonQuery(sql, parameters);
            return result;
        }
    }
}
