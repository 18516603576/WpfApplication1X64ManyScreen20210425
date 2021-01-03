using Dal;
using Model;
using System;
using System.Collections.Generic;

namespace Bll
{
    public class DPageBll
    {

        private readonly DPageDal dPageDal = new DPageDal();

        private readonly DControlBll dControlBll = new DControlBll();

        public DPage insertChild(DPage dPage)
        {
            //补充idx,createTime两个参数
            DPage maxDPage = getMaxIdxByParentId(dPage.parentId);
            if (maxDPage == null)
            {
                dPage.idx = 1;
            }
            else
            {
                dPage.idx = maxDPage.idx + 1;
            }
            dPage.createTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            return dPageDal.insert(dPage);
        }

        /*
         * 获取子页面中排序最大的页面 
         */
        private DPage getMaxIdxByParentId(Int32 parentId)
        {
            return dPageDal.getMaxIdxByParentId(parentId);
        }

        /*
         * 获取子页面
         */
        public List<DPage> getByParentId(Int32 id)
        {
            return dPageDal.getByParentId(id);
        }
        /*
         * 获取某个页面
         */
        public DPage get(Int32 id)
        {
            return dPageDal.get(id);
        }

        /*
         * 先删除控件
         * 再删除页面
         */
        public Int32 delete(Int32 pageId)
        {
            Int32 rows = dControlBll.deleteByPageId(pageId);
            //  Int32 rows2 = dControlBll.updateLinkToPageId(pageId); 
            Int32 row = dPageDal.delete(pageId);
            return row;
        }

        /*
         * 更新页面
         */
        public DPage update(DPage dPage)
        {
            Int32 rows = dPageDal.update(dPage);
            return dPage;
        }
        /*
         * 获取后一个页面 
         */
        public DPage getLastPage()
        {
            return dPageDal.getLastPage();
        }
        /*
         * 新建页面 - 在当前页面之后
         */
        public DPage insertAfter(DPage dPage)
        {
            //补充createTime两个参数 
            dPage.createTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            int rows = dPageDal.updateAfterIdx(dPage);

            return dPageDal.insert(dPage);
        }

        /*
         * 向上移动
         */
        public void moveUp(int currPageId, int prevPageId)
        {
            DPage currDPage = dPageDal.get(currPageId);
            DPage prevDPage = dPageDal.get(prevPageId);

            int currIdx = currDPage.idx;
            int prevIdx = prevDPage.idx;

            currDPage.idx = prevIdx;
            prevDPage.idx = currIdx;

            dPageDal.update(currDPage);
            dPageDal.update(prevDPage);
        }
        /*
         * 向下移动
         */
        public void moveDown(int currPageId, int nextPageId)
        {
            DPage currDPage = dPageDal.get(currPageId);
            DPage nextDPage = dPageDal.get(nextPageId);

            int currIdx = currDPage.idx;
            int nextIdx = nextDPage.idx;

            currDPage.idx = nextIdx;
            nextDPage.idx = currIdx;

            dPageDal.update(currDPage);
            dPageDal.update(nextDPage);
        }

        /*
         * 是否存在同名页面
         */
        public bool hasSameName(DPage dPage)
        {
            int rows = dPageDal.hasSameName(dPage);
            return rows > 0 ? true : false;
        }
    }
}
