using Common.util;
using Dal;
using Model;
using System;
using System.Collections.Generic;

namespace Bll
{
    public class DControlEventBll
    {
        private readonly DControlEventDal dControlEventDal = new DControlEventDal();
         
        /*
         * 点击控件，多屏幕跳转
         */
        public List<DControlEvent> getByDControlId(Int32 dControlId)
        {

            List<DControlEvent> list = dControlEventDal.getByDControlId(dControlId);

            return list;
        }
        /*
         * 点击某张轮播图，多屏幕跳转
         */
        public List<DControlEvent> getByTurnPictureImagesId(Int32 turnPictureImagesId)
        { 
            List<DControlEvent> list = dControlEventDal.getByTurnPictureImagesId(turnPictureImagesId); 
            return list;
        }

        

        /*
         * 更新
         */
        public DControlEvent update(DControlEvent dControlEvent)
        {
            int rows = dControlEventDal.update(dControlEvent);
            return dControlEvent;
        }

        /*
         * 插入
         */
        public DControlEvent insert(DControlEvent dControlEvent)
        {
            return dControlEventDal.insert(dControlEvent);
        }
        /*
         * 获取某个控件在某个屏幕下的链接信息
         */
        public DControlEvent getByDControlIdScreenCfgId(int dControlId, int screenCfgId)
        {
            return dControlEventDal.getByDControlIdScreenCfgId(dControlId, screenCfgId);
        }
        /*
        * 获取轮播图片在某个屏幕下的链接信息
        */
        public DControlEvent getByTurnPictureImagesIdScreenCfgId(int TurnPictureImagesId, int screenCfgId)
        {
            return dControlEventDal.getByTurnPictureImagesIdScreenCfgId(TurnPictureImagesId, screenCfgId);
        }
    }
}
