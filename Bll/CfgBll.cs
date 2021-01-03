using Dal;
using Model;
using System;

namespace Bll
{
    public class CfgBll
    {
        private readonly CfgDal cfgDal = new CfgDal();
        //通过id获取cfg
        public Cfg get(Int32 id)
        {

            Cfg cfg = cfgDal.get(id);

            return cfg;
        }

        /*
         * 更新配置信息
         */
        public Cfg update(Cfg cfg)
        {
            int rows = cfgDal.update(cfg);
            return cfg;
        }
    }
}
