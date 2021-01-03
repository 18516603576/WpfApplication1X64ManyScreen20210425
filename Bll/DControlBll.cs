using Common.util;
using Dal;
using Model;
using System;
using System.Collections.Generic;

namespace Bll
{
    public class DControlBll
    {
        private readonly DControlDal dControlDal = new DControlDal();
        private readonly TurnPictureImagesBll turnPictureImagesBll = new TurnPictureImagesBll();
        private readonly DControlAnimationBll dControlAnimationBll = new DControlAnimationBll();

        public List<DControl> getByPageId(Int32 pageId)
        {

            List<DControl> list = dControlDal.getByPageId(pageId);

            return list;
        }


        /*
         * 删除页面下的所有控件
         */
        internal int deleteByPageId(Int32 pageId)
        {
            Int32 affectRows = 0;
            List<DControl> list = getByPageId(pageId);
            foreach (DControl dControl in list)
            {
                int rows = delete(dControl.id);
                affectRows = affectRows + rows;
            }
            return affectRows;
        }

        /*
         * 删除单个控件
         */
        public int delete(Int32 id)
        {
            DControl tmp = get(id);
            //1.删除控件动画
            int rows = dControlAnimationBll.deleteByDControlId(id);

            //2.删除轮播图中的图片
            if (tmp.type == "TurnPicture" || tmp.type == "Marque" || tmp.type == "MarqueLayer")
            {
                turnPictureImagesBll.deleteByDControlId(tmp.id);
            }


            return dControlDal.delete(id);
        }
        /*
         * 插入一个控件
         */
        public DControl insert(DControl entity)
        {
            //补充idx参数
            int maxIdx = 1;
            if (entity.parentId > 0)
            {
                maxIdx = getMaxIdxByParentId(entity.parentId);
            }
            else
            {
                maxIdx = getMaxIdxByPageId(entity.pageId);
            }

            if (maxIdx <= 0)
            {
                entity.idx = 1;
            }
            else
            {
                entity.idx = maxIdx + 1;
            }
            entity.opacity = 100;
            entity.rotateAngle = 0;

            //  entity.isTab = false;


            return dControlDal.insert(entity);
        }


        /*
         * 获取页面下的所有CFrame控件
         */
        public List<DControl> getCFrameByPageId(int pageId)
        {
            return dControlDal.getCFrameByPageId(pageId);
        }


        /*
         * 插入一个控件,来自复制的控件
         */
        public DControl insertFromPaste(DControl entity)
        {
            //补充idx参数
            int maxIdx = 1;
            if (entity.parentId > 0)
            {
                maxIdx = getMaxIdxByParentId(entity.parentId);
            }
            else
            {
                maxIdx = getMaxIdxByPageId(entity.pageId);
            }

            if (maxIdx <= 0)
            {
                entity.idx = 1;
            }
            else
            {
                entity.idx = maxIdx + 1;
            }
            return dControlDal.insert(entity);
        }


        /*
         * 获取滚动区域下的最大的排序idx
         */
        private int getMaxIdxByParentId(int parentId)
        {
            return dControlDal.getMaxIdxByParentId(parentId);
        }

        /*
        * 获取页面下最大的排序idx
        */
        private int getMaxIdxByPageId(int pageId)
        {
            return dControlDal.getMaxIdxByPageId(pageId);
        }

        /*
         * 更新控件数据
         */
        public DControl update(DControl entity)
        {
            int rows = dControlDal.update(entity);
            return entity;
        }

        /*
         * 获取控件
         */
        public DControl get(int id)
        {
            return dControlDal.get(id);
        }
        /*
         * 删除页面后，链接到当前页面的控件全部重置   linkToPageId=0
         */
        internal int updateLinkToPageId(int pageId)
        {
            return dControlDal.updateLinkToPageId(pageId);
        }

        /*
         * 滚动区下的控件
         */
        public List<DControl> getByParentId(int id)
        {
            return dControlDal.getByParentId(id);
        }
        /*
         *  获取当前页面所关联的CFrame
         */
        public List<DControl> getParentCFrameByLinkToPageId(int pageId)
        {
            return dControlDal.getParentCFrameByLinkToPageId(pageId);
        }

        /*
         * 复制当前页面下的所有控件到新页面
         * 
         * @param dPage 当前页
         * 
         * @param  newDPage 新页面
         */
        public void copyToPage(DPage dPage, DPage newDPage)
        {
            List<DControl> list = getByPageId(dPage.id);
            foreach (DControl dControl in list)
            {
                //dControl.pageId = newDPage.id;
                //Int32 fromDControlId = dControl.id;
                //DControl newDControl = this.insert(dControl);
                //if (dControl.type == "TurnPicture")
                //{
                //    turnPictureImagesBll.copyFromDControlId(fromDControlId, newDControl.id);  
                //} 

                DControl newDControl = DControlUtil.createFrom(dControl);
                newDControl.pageId = newDPage.id;
                pasteDControl(dControl, newDControl, dPage);


            }
        }


        /*
        * 粘贴一个控件及其子控件 到数据库
        */
        public DControl pasteDControl(DControl dControl, DControl newDControl, DPage dPage)
        {

            //3、插入控件数据库
            newDControl = insertFromPaste(newDControl);
            if (dControl.type == "TurnPicture" || dControl.type == "Marque" || dControl.type == "MarqueLayer")
            {
                turnPictureImagesBll.copyFromDControlId(dControl.id, newDControl.id);
            }
            dControlAnimationBll.copyFromDControlId(dControl.id, newDControl.id);



            return newDControl;
        }

        /*
         * 移动层次
         */
        public void moveUpIdx(DControl dControl, DControl eleDControl)
        {
            update(dControl);
            update(eleDControl);
        }

        /*
         * 移动到顶层
         */
        public List<DControl> moveupTopIdx(DControl dControl)
        {
            List<DControl> list = new List<DControl>();
            if (dControl.parentId > 0)
            {
                //获取同一个滚动区下的所有控件
                list = getByParentId(dControl.parentId);
            }
            else
            {
                list = getByPageId(dControl.pageId);
            }

            //重新排序其他
            int i = 0;
            foreach (DControl ctl in list)
            {
                if (ctl.id == dControl.id) continue;
                i = i + 1;
                ctl.idx = i;
                update(ctl);
            }
            //更新当前为最大
            foreach (DControl ctl in list)
            {
                if (ctl.id == dControl.id)
                {
                    ctl.idx = i + 1;
                    update(ctl);
                }
            }
            return list;
        }

        /*
         * 置于底层
         */
        public List<DControl> movedownBottomIdx(DControl dControl)
        {
            List<DControl> list = new List<DControl>();
            if (dControl.parentId > 0)
            {
                //获取同一个滚动区下的所有控件
                list = getByParentId(dControl.parentId);
            }
            else
            {
                list = getByPageId(dControl.pageId);
            }
            //重新排序其他
            int i = 1;
            foreach (DControl ctl in list)
            {
                if (ctl.id == dControl.id)
                {
                    ctl.idx = 1;
                    update(ctl);
                    continue;
                }
                i = i + 1;
                ctl.idx = i;
                update(ctl);
            }

            return list;
        }

        /*
        * 指定的小窗口是否有嵌套当前页面的
        * 
        * @linkToPageId  CFrame中显示的页面id
        * 
        * @currPageId  控件所在窗口
        */
        public Boolean isNestedOfCurrPageId(int linkToPageId, int currPageId)
        {
            Boolean result = false;
            if (linkToPageId == currPageId) return true;

            List<DControl> list = getCFrameByPageId(linkToPageId);
            foreach (DControl dc in list)
            {
                if (dc.linkToPageId == currPageId)
                {
                    return true;
                }
                else
                {
                    result = isNestedOfCurrPageId(dc.linkToPageId, currPageId);
                }
            }
            return result;
        }
    }
}
