using Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;

namespace Dal
{
    public class DPageDal
    {
        //1、通过id获取页面
        public DPage get(Int32 id)
        {
            String sql = "select * from dPage where id=@id";
            SQLiteParameter[] parameters = {
                       new SQLiteParameter("@id", DbType.Int32,4)
                      };
            parameters[0].Value = id;

            DataTable dt = Common.SQLiteHelper.ExecuteQuery(sql, parameters);
            if (dt == null || dt.Rows.Count == 0)
            {
                return null;
            }

            DPage page = DataToEntity<DPage>.FillModel(dt.Rows[0]);
            return page;
        }

        //2、获取子页面
        public List<DPage> getByParentId(Int32 parentId)
        {
            String sql = "select * from dPage where parentId=@parentId order by idx asc";
            SQLiteParameter[] parameters = {
                       new SQLiteParameter("@parentId", DbType.Int32,4)
                      };
            parameters[0].Value = parentId;

            DataTable dt = Common.SQLiteHelper.ExecuteQuery(sql, parameters);

            List<DPage> list = DataToEntity<DPage>.FillModel(dt);

            return list;
        }

        //3、添加子页面
        public DPage insert(DPage entity)
        {
            String sql = "insert into dPage(name,parentId,idx,createTime,backgroundImageId,width,height,pageSwitchType,backgroundVideoId) values(@name,@parentId,@idx,@createTime,@backgroundImageId,@width,@height,@pageSwitchType,@backgroundVideoId);select last_insert_rowid();";
            SQLiteParameter[] parameters = {
                       new SQLiteParameter("@name", DbType.String,100),
                       new SQLiteParameter("@parentId", DbType.Int32,4),
                       new SQLiteParameter("@idx", DbType.Int32,4),
                       new SQLiteParameter("@createTime", DbType.String,30),
                       new SQLiteParameter("@backgroundImageId", DbType.Int32,4),
                       new SQLiteParameter("@width", DbType.Int32,4),
                       new SQLiteParameter("@height", DbType.Int32,4),
                       new SQLiteParameter("@pageSwitchType", DbType.Int32,4),
                       new SQLiteParameter("@backgroundVideoId", DbType.Int32,4)
                      };
            parameters[0].Value = entity.name;
            parameters[1].Value = entity.parentId;
            parameters[2].Value = entity.idx;
            parameters[3].Value = entity.createTime;
            parameters[4].Value = entity.backgroundImageId;
            parameters[5].Value = entity.width;
            parameters[6].Value = entity.height;
            parameters[7].Value = entity.pageSwitchType;
            parameters[8].Value = entity.backgroundVideoId;

            DataTable dt = Common.SQLiteHelper.ExecuteQuery(sql, parameters);
            int id = DataType.ToInt32(dt.Rows[0]["last_insert_rowid()"].ToString());
            entity.id = id;
            return entity;
        }

        /*
         * 获取最大排序的那个页面
         */
        public DPage getMaxIdxByParentId(Int32 parentId)
        {
            String sql = "select max(idx) as idx from dPage where parentId=@parentId";
            SQLiteParameter[] parameters = {
                       new SQLiteParameter("@parentId", DbType.Int32,4)
                      };
            parameters[0].Value = parentId;

            DataTable dt = Common.SQLiteHelper.ExecuteQuery(sql, parameters);

            DPage entity = DataToEntity<DPage>.FillModel(dt.Rows[0]);
            return entity;
        }

        /*
         * 删除某个页面
         */
        public int delete(Int32 pageId)
        {
            String sql = "delete from dPage where id=@pageId";
            SQLiteParameter[] parameters = {
                       new SQLiteParameter("@pageId", DbType.Int32,4)
                      };
            parameters[0].Value = pageId;

            Int32 rows = Common.SQLiteHelper.ExecuteNonQuery(sql, parameters);

            return rows;
        }
        /*
         * 更新页面
         */
        public Int32 update(DPage entity)
        {
            string sql = "update dPage set name=@name,parentId=@parentId,idx=@idx"
               + ",createTime=@createTime,backgroundImageId=@backgroundImageId"
               + ",width=@width,height=@height,pageSwitchType=@pageSwitchType,backgroundVideoId=@backgroundVideoId"
               + "  where id=@id";

            SQLiteParameter[] parameters = {
                       new SQLiteParameter("@name", DbType.String,30),
                       new SQLiteParameter("@parentId", DbType.Int32,4),
                       new SQLiteParameter("@idx", DbType.Int32,4),
                       new SQLiteParameter("@createTime", DbType.String,30),
                       new SQLiteParameter("@backgroundImageId", DbType.Int32,4),
                       new SQLiteParameter("@width", DbType.Int32,4),
                       new SQLiteParameter("@height", DbType.Int32,4),
                        new SQLiteParameter("@pageSwitchType", DbType.Int32,4),
                        new SQLiteParameter("@backgroundVideoId", DbType.Int32,4),
                       new SQLiteParameter("@id", DbType.Int32,4)
                      };
            parameters[0].Value = entity.name;
            parameters[1].Value = entity.parentId;
            parameters[2].Value = entity.idx;

            parameters[3].Value = entity.createTime;
            parameters[4].Value = entity.backgroundImageId;
            parameters[5].Value = entity.width;
            parameters[6].Value = entity.height;
            parameters[7].Value = entity.pageSwitchType;
            parameters[8].Value = entity.backgroundVideoId;
            parameters[9].Value = entity.id;

            int result = Common.SQLiteHelper.ExecuteNonQuery(sql, parameters);
            return result;

        }
        /*
         * 获取最后一个页面
         */
        public DPage getLastPage()
        {
            String sql = "select * from dPage order by id desc limit 0,1";
            SQLiteParameter[] parameters = {

                      };
            DataTable dt = Common.SQLiteHelper.ExecuteQuery(sql, parameters);

            DPage entity = DataToEntity<DPage>.FillModel(dt.Rows[0]);
            return entity;
        }
        /*
         *   新建页面 在当前页面之后
         * 
         *   parentId=entity.parentId and idx>= entity.idx
         *   
         *   更新 idx = idx + 1 
         */
        public int updateAfterIdx(DPage entity)
        {
            string sql = "update dPage set idx=idx+1"
             + "  where parentId=@parentId and idx>=@idx";

            SQLiteParameter[] parameters = {
                        new SQLiteParameter("@parentId", DbType.Int32,4),
                        new SQLiteParameter("@idx", DbType.Int32,4)
                      };
            parameters[0].Value = entity.parentId;
            parameters[1].Value = entity.idx;

            int result = Common.SQLiteHelper.ExecuteNonQuery(sql, parameters);
            return result;
        }

        /*
         * 是否存在同名页面
         */
        public int hasSameName(DPage entity)
        {
            string sql = "select count(*) from dPage where name=@name and id!=@id ";
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
    }
}
