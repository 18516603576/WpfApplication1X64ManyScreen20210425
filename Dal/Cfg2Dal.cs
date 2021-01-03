using Model;
using System;
using System.Data;
using System.Data.SQLite;

namespace Dal
{
    public class Cfg2Dal
    {
        //通过id获取cfg
        public Cfg2 get(int id)
        {
            String sql = "select * from cfg2 where id=@id";
            SQLiteParameter[] parameters = {
                       new SQLiteParameter("@id", DbType.Int32,4)
                      };
            parameters[0].Value = id;

            DataTable dt = Common.SQLiteHelper.ExecuteQuery(sql, parameters, Common.SQLiteHelper.connectionString_reg);

            Cfg2 cfg = DataToEntity<Cfg2>.FillModel(dt.Rows[0]);
            return cfg;
        }

        /*
         * 更 新
         */
        public int update(Cfg2 entity)
        {

            string sql = "update cfg2 set sd=@sd,cd=@cd,ld=@ld"
                 + ",rg1=@rg1,rg3=@rg3,validateCode=@validateCode"
                 + "  where id=@id";

            SQLiteParameter[] parameters = {
                       new SQLiteParameter("@sd", DbType.String,200),
                       new SQLiteParameter("@cd", DbType.String,200),
                       new SQLiteParameter("@ld", DbType.String,200),
                       new SQLiteParameter("@rg1", DbType.String,200),
                       new SQLiteParameter("@rg3", DbType.String,200),
                       new SQLiteParameter("@validateCode", DbType.String,200),
                       new SQLiteParameter("@id", DbType.Int32,4)
                      };
            parameters[0].Value = entity.sd;
            parameters[1].Value = entity.cd;
            parameters[2].Value = entity.ld;
            parameters[3].Value = entity.rg1;
            parameters[4].Value = entity.rg3;
            parameters[5].Value = entity.validateCode;
            parameters[6].Value = entity.id;
            int result = Common.SQLiteHelper.ExecuteNonQuery(sql, parameters, Common.SQLiteHelper.connectionString_reg);
            return result;
        }
    }
}
