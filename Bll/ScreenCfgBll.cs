using Dal;
using Model;
using Model.dto;
using System;
using System.Collections.Generic;

namespace Bll
{
    public class ScreenCfgBll
    {
        private readonly ScreenCfgDal screenCfgDal = new ScreenCfgDal();
        //通过id获取cfg
        public ScreenCfg get(Int32 id)
        { 
            ScreenCfg screenCfg = screenCfgDal.get(id); 
            return screenCfg;
        }
        /*
         * 获取所有
         */
        public List<ScreenCfg> findAll()
        {
            return screenCfgDal.findAll(); 
        }
        /*
        * 获取所有
        */
        public List<ScreenCfgDto> findAllWithPageName()
        {
            return screenCfgDal.findAllWithPageName();
        }

        /*
         * 更新配置信息
         */
        public ScreenCfg update(ScreenCfg screenCfg)
        {
            int rows = screenCfgDal.update(screenCfg);
            return screenCfg; 
        }
        /*
         * 添加
         */
        public ScreenCfg insert(ScreenCfg screenCfg)
        {
            return screenCfgDal.insert(screenCfg);
        }

        /*
         * 删除
         */
        public int delete(int id)
        {
            return screenCfgDal.delete(id);
        }
    }
}
