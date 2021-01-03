using Dal;
using Model;
using Model.dto;
using System;
using System.Collections.Generic;

namespace Bll
{
    public class TurnPictureImagesBll
    {
        private readonly TurnPictureImagesDal turnPictureImagesDal = new TurnPictureImagesDal();
        /*
         * 添加图片列表
         * 
         * @param dControlId  控件编号
         * 
         * @param storageImageIds 图片列表
         * 
         */
        public Int32 insert(List<TurnPictureImagesDto> list)
        {
            int rows = 0;
            foreach (TurnPictureImagesDto dto in list)
            {
                TurnPictureImages turnPictureImages = new TurnPictureImages();
                turnPictureImages.dControlId = dto.dControlId;
                turnPictureImages.storageImageId = dto.storageImageId;
                turnPictureImages.isLink = dto.isLink;

                insert(turnPictureImages);
                rows = rows + 1;
            }
            return rows;
        }

        /*
         *  插入一个
         *   
         */
        public TurnPictureImages insert(TurnPictureImages turnPictureImages)
        {

            return turnPictureImagesDal.insert(turnPictureImages); ;
        }

        /*
         * 获取图片列表
         */
        public List<TurnPictureImagesDto> getByDControlId(int dControlId)
        {
            return turnPictureImagesDal.getByDControlId(dControlId);
        }

        /*
          *  更新每一行
          * 
          */
        public void updateByDControlId(DControl currDControl, List<TurnPictureImagesDto> list)
        {
            foreach (TurnPictureImagesDto dto in list)
            {
                update(dto);
            }
        }

        /*
         *  删除相册下的图片
         */
        public Int32 deleteByDControlId(int dControlId)
        {
            return turnPictureImagesDal.deleteByDControlId(dControlId);
        }
        /*
         * 粘贴相册，同时复制旗下的图片
         */
        public void copyFromDControlId(int fromDControlId, int toDControlId)
        {
            List<TurnPictureImagesDto> list = getByDControlId(fromDControlId);
            foreach (TurnPictureImages dto in list)
            {
                TurnPictureImages turnPictureImages = new TurnPictureImages();
                turnPictureImages.dControlId = toDControlId;
                turnPictureImages.storageImageId = dto.storageImageId;
                turnPictureImages.isLink = dto.isLink;

                insert(turnPictureImages);
            }
        }

        public TurnPictureImages update(TurnPictureImages turnPictureImages)
        {
            int rows = turnPictureImagesDal.update(turnPictureImages);
            return turnPictureImages;
        }

        /*
         * 获取图片
         */
        public TurnPictureImages get(int id)
        {
            return turnPictureImagesDal.get(id);
        }
        /*
        * 删除一行
        */
        public int delete(int id)
        {
            return turnPictureImagesDal.delete(id);
        }
    }
}
