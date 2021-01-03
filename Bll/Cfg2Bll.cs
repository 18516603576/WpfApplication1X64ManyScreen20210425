using Dal;
using Model;
using System;

namespace Bll
{
    public class Cfg2Bll
    {
        private readonly Cfg2Dal cfg2Dal = new Cfg2Dal();
        //通过id获取cfg
        public Cfg2 get(Int32 id)
        {

            Cfg2 cfg = cfg2Dal.get(id);

            return cfg;
        }

        /*
         * 更新
         */
        public Cfg2 update(Cfg2 cfg2)
        {
            int rows = cfg2Dal.update(cfg2);
            return cfg2;
        }
    }
}
