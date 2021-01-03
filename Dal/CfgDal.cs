using Model;
using System;
using System.Data;
using System.Data.SQLite;

namespace Dal
{
    public class CfgDal
    {
        //通过id获取cfg
        public Cfg get(int id)
        {

            String sql = "select * from cfg where id=@id";
            SQLiteParameter[] parameters = {
                       new SQLiteParameter("@id", DbType.Int32,4)
                      };
            parameters[0].Value = id;

            DataTable dt = Common.SQLiteHelper.ExecuteQuery(sql, parameters);

            Cfg cfg = DataToEntity<Cfg>.FillModel(dt.Rows[0]);
            return cfg;
        }

        /*
         * 更新配置
         */
        public int update(Cfg entity)
        {
            string sql = "update cfg set pagePercent=@pagePercent,screenWidth=@screenWidth,screenHeight=@screenHeight"
                 + ",screenSaverIsable=@screenSaverIsable,screenSaverNoActionTime=@screenSaverNoActionTime,screenSaverImgsSpeed=@screenSaverImgsSpeed"
                 + ",screenSaverImgs=@screenSaverImgs,defaultTemplate=@defaultTemplate"
                 + ",password=@password"
                + ",backgroundMusicAutoplay=@backgroundMusicAutoplay,backgroundMusicLoop=@backgroundMusicLoop"
                + ",backgroundMusicShow=@backgroundMusicShow,backgroundMusicButtonLeft=@backgroundMusicButtonLeft"
                + ",backgroundMusicButtonTop=@backgroundMusicButtonTop,backgroundMusicButtonWidth=@backgroundMusicButtonWidth"
                + ",backgroundMusicButtonHeight=@backgroundMusicButtonHeight,backgroundMusicId=@backgroundMusicId"
                + ",backgroundMusicButtonImageId=@backgroundMusicButtonImageId,pageSwitchType=@pageSwitchType,noActionTimeBackToHome=@noActionTimeBackToHome"
                + ",week1=@week1,week2=@week2,week3=@week3,week4=@week4,week5=@week5,week6=@week6,week7=@week7"
                 + "  where id=@id";

            SQLiteParameter[] parameters = {
                       new SQLiteParameter("@pagePercent", DbType.Int32,4),
                       new SQLiteParameter("@screenWidth", DbType.Int32,4),
                       new SQLiteParameter("@screenHeight", DbType.Int32,4),
                       new SQLiteParameter("@screenSaverIsable", DbType.Int32,4),
                       new SQLiteParameter("@screenSaverNoActionTime", DbType.Int32,4),
                       new SQLiteParameter("@screenSaverImgsSpeed", DbType.Int32,4),
                       new SQLiteParameter("@screenSaverImgs", DbType.String,4000),
                       new SQLiteParameter("@defaultTemplate", DbType.Int32,4),
                       new SQLiteParameter("@password", DbType.String,30),
                       new SQLiteParameter("@backgroundMusicAutoplay", DbType.Boolean,4),
                       new SQLiteParameter("@backgroundMusicLoop", DbType.Boolean,4),
                       new SQLiteParameter("@backgroundMusicShow", DbType.Boolean,4),
                       new SQLiteParameter("@backgroundMusicButtonLeft", DbType.Int32,4),
                       new SQLiteParameter("@backgroundMusicButtonTop", DbType.Int32,4),
                       new SQLiteParameter("@backgroundMusicButtonWidth", DbType.Int32,4),
                       new SQLiteParameter("@backgroundMusicButtonHeight", DbType.Int32,4),
                       new SQLiteParameter("@backgroundMusicId", DbType.Int32,4),
                       new SQLiteParameter("@backgroundMusicButtonImageId", DbType.Int32,4),
                       new SQLiteParameter("@pageSwitchType", DbType.Int32,4),
                       new SQLiteParameter("@noActionTimeBackToHome", DbType.Int32,4),
                       new SQLiteParameter("@week1", DbType.String,20),
                       new SQLiteParameter("@week2", DbType.String,20),
                       new SQLiteParameter("@week3", DbType.String,20),
                       new SQLiteParameter("@week4", DbType.String,20),
                       new SQLiteParameter("@week5", DbType.String,20),
                       new SQLiteParameter("@week6", DbType.String,20),
                       new SQLiteParameter("@week7", DbType.String,20),
                       new SQLiteParameter("@id", DbType.Int32,4)
                      };
            parameters[0].Value = entity.pagePercent;
            parameters[1].Value = entity.screenWidth;
            parameters[2].Value = entity.screenHeight;
            parameters[3].Value = entity.screenSaverIsable;
            parameters[4].Value = entity.screenSaverNoActionTime;
            parameters[5].Value = entity.screenSaverImgsSpeed;
            parameters[6].Value = entity.screenSaverImgs;
            parameters[7].Value = entity.defaultTemplate;
            parameters[8].Value = entity.password;
            parameters[9].Value = entity.backgroundMusicAutoplay;
            parameters[10].Value = entity.backgroundMusicLoop;
            parameters[11].Value = entity.backgroundMusicShow;
            parameters[12].Value = entity.backgroundMusicButtonLeft;
            parameters[13].Value = entity.backgroundMusicButtonTop;
            parameters[14].Value = entity.backgroundMusicButtonWidth;
            parameters[15].Value = entity.backgroundMusicButtonHeight;
            parameters[16].Value = entity.backgroundMusicId;
            parameters[17].Value = entity.backgroundMusicButtonImageId;
            parameters[18].Value = entity.pageSwitchType;
            parameters[19].Value = entity.noActionTimeBackToHome;
            parameters[20].Value = entity.week1;
            parameters[21].Value = entity.week2;
            parameters[22].Value = entity.week3;
            parameters[23].Value = entity.week4;
            parameters[24].Value = entity.week5;
            parameters[25].Value = entity.week6;
            parameters[26].Value = entity.week7;
            parameters[27].Value = entity.id;
            int result = Common.SQLiteHelper.ExecuteNonQuery(sql, parameters);
            return result;
        }
    }
}
