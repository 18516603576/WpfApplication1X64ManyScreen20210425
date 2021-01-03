using Dal;
using Model;
using System.Collections.Generic;

namespace Bll
{
    public class DControlAnimationBll
    {

        private readonly DControlAnimationDal dControlAnimationDal = new DControlAnimationDal();
        /*
         * 添加
         */
        public DControlAnimation insert(DControlAnimation dControlAnimation)
        {
            return dControlAnimationDal.insert(dControlAnimation);
        }

        /*
         * 获取控件下所有动画
         */
        public List<DControlAnimation> getByDControlId(int dControlId)
        {
            return dControlAnimationDal.getByDControlId(dControlId);
        }

        /*
         * 删除动画
         */
        public int delete(int id)
        {
            return dControlAnimationDal.delete(id);
        }
        /*
         * 更新动画
         */
        public int update(DControlAnimation animation)
        {
            return dControlAnimationDal.update(animation);
        }

        /*
         * 复制动画
         */
        public void copyFromDControlId(int fromDControlId, int toDControlId)
        {
            List<DControlAnimation> list = getByDControlId(fromDControlId);
            foreach (DControlAnimation one in list)
            {
                insert(one, toDControlId); ;
            }
        }
        /*
         * 删除控件下的所有动画
         * 
         * @param dControlId  控件编号
         */
        internal int deleteByDControlId(int dControlId)
        {
            return dControlAnimationDal.deleteByDControlId(dControlId);
        }

        /*
         * 复制添加
         */
        private void insert(DControlAnimation dControlAnimation, int toDControlId)
        {
            DControlAnimation entity = new DControlAnimation();
            entity.dControlId = toDControlId;
            entity.name = dControlAnimation.name;
            entity.type = dControlAnimation.type;
            entity.delaySeconds = dControlAnimation.delaySeconds;
            entity.durationSeconds = dControlAnimation.durationSeconds;
            entity.playTimes = dControlAnimation.playTimes;
            entity.isSameOpacity = dControlAnimation.isSameOpacity;
            entity.isSameSpeed = dControlAnimation.isSameSpeed;
            insert(entity);
        }
    }
}
