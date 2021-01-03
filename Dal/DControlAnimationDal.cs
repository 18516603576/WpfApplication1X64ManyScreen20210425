using Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;

namespace Dal
{
    public class DControlAnimationDal
    {

        /*
         * 添加一个控件
         */
        public DControlAnimation insert(DControlAnimation entity)
        {

            String sql = "insert into dControlAnimation(dControlId,name,type,delaySeconds,durationSeconds,playTimes,isSameSpeed,isSameOpacity) ";
            sql = sql + " values(@dControlId,@name,@type,@delaySeconds,@durationSeconds,@playTimes,@isSameSpeed,@isSameOpacity);select last_insert_rowid();";
            SQLiteParameter[] parameters = {
                       new SQLiteParameter("@dControlId", DbType.Int32,4),
                       new SQLiteParameter("@name", DbType.String,30),
                       new SQLiteParameter("@type", DbType.Int32,4),
                       new SQLiteParameter("@delaySeconds", DbType.Int32,4),
                       new SQLiteParameter("@durationSeconds", DbType.Int32,4),
                       new SQLiteParameter("@playTimes", DbType.Int32,4),
                       new SQLiteParameter("@isSameSpeed", DbType.Int32,4),
                       new SQLiteParameter("@isSameOpacity", DbType.Int32,4)
                      };
            parameters[0].Value = entity.dControlId;
            parameters[1].Value = entity.name;
            parameters[2].Value = entity.type;
            parameters[3].Value = entity.delaySeconds;
            parameters[4].Value = entity.durationSeconds;
            parameters[5].Value = entity.playTimes;
            parameters[6].Value = entity.isSameSpeed;
            parameters[7].Value = entity.isSameOpacity;


            DataTable dt = Common.SQLiteHelper.ExecuteQuery(sql, parameters);
            int id = DataType.ToInt32(dt.Rows[0]["last_insert_rowid()"].ToString());
            entity.id = id;
            return entity;
        }

        /*
         * 更新
         */
        public int update(DControlAnimation entity)
        {
            string sql = "update dControlAnimation set dControlId=@dControlId,name=@name,type=@type"
                + ",delaySeconds=@delaySeconds,durationSeconds=@durationSeconds"
                + ",playTimes=@playTimes,isSameSpeed=@isSameSpeed,isSameOpacity=@isSameOpacity"
                + "  where id=@id";

            SQLiteParameter[] parameters = {
                       new SQLiteParameter("@dControlId", DbType.Int32,4),
                       new SQLiteParameter("@name", DbType.String,30),
                       new SQLiteParameter("@type", DbType.Int32,4),

                       new SQLiteParameter("@delaySeconds", DbType.Int32,4),
                       new SQLiteParameter("@durationSeconds", DbType.Int32,4),
                       new SQLiteParameter("@playTimes", DbType.Int32,4),
                       new SQLiteParameter("@isSameSpeed", DbType.Int32,4),
                       new SQLiteParameter("@isSameOpacity", DbType.Int32,4),
                       new SQLiteParameter("@id", DbType.Int32,4)
                      };
            parameters[0].Value = entity.dControlId;
            parameters[1].Value = entity.name;
            parameters[2].Value = entity.type;

            parameters[3].Value = entity.delaySeconds;
            parameters[4].Value = entity.durationSeconds;
            parameters[5].Value = entity.playTimes;
            parameters[6].Value = entity.isSameSpeed;
            parameters[7].Value = entity.isSameOpacity;
            parameters[8].Value = entity.id;

            int result = Common.SQLiteHelper.ExecuteNonQuery(sql, parameters);
            return result;
        }

        /*
         * 删除动画下所有控件
         */
        public int deleteByDControlId(int dControlId)
        {
            String sql = "delete from dControlAnimation where dControlId=@dControlId";
            SQLiteParameter[] parameters = {
                       new SQLiteParameter("@dControlId", DbType.Int32,4)
                      };
            parameters[0].Value = dControlId;

            Int32 rows = Common.SQLiteHelper.ExecuteNonQuery(sql, parameters);

            return rows;
        }



        /*
         * 获取控件下所有动画
         */
        public List<DControlAnimation> getByDControlId(int dControlId)
        {
            String sql = "select * from dControlAnimation where dControlId=@dControlId order by id asc";
            SQLiteParameter[] parameters = {
                       new SQLiteParameter("@dControlId", DbType.Int32,4)
                      };
            parameters[0].Value = dControlId;

            DataTable dt = Common.SQLiteHelper.ExecuteQuery(sql, parameters);

            List<DControlAnimation> list = DataToEntity<DControlAnimation>.FillModel(dt);

            return list;
        }

        /*
         *删除
         */
        public int delete(int id)
        {
            String sql = "delete from dControlAnimation where id=@id";
            SQLiteParameter[] parameters = {
                       new SQLiteParameter("@id", DbType.Int32,4)
                      };
            parameters[0].Value = id;

            Int32 rows = Common.SQLiteHelper.ExecuteNonQuery(sql, parameters);

            return rows;
        }
    }
}
