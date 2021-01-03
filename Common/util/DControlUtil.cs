using Model;
using Model.dto;

namespace Common.util
{
    public class DControlUtil
    {
        public static DControl createFrom(DControl dControl)
        {

            DControl newDControl = new DControl();
            newDControl.id = dControl.id;
            newDControl.pageId = dControl.pageId;
            newDControl.name = dControl.name;
            newDControl.width = dControl.width;
            newDControl.height = dControl.height;
            newDControl.left = dControl.left;
            newDControl.top = dControl.top;
            newDControl.type = dControl.type;
            newDControl.content = dControl.content;
            newDControl.idx = dControl.idx;

            newDControl.linkToPageId = dControl.linkToPageId;
            newDControl.isClickShow = dControl.isClickShow;
            newDControl.linkToVideoId = dControl.linkToVideoId;
            newDControl.autoplay = dControl.autoplay;
            newDControl.loop = dControl.loop;
            newDControl.turnPictureSpeed = dControl.turnPictureSpeed;
            newDControl.linkToWeb = dControl.linkToWeb;
            newDControl.storageId = dControl.storageId;
            newDControl.isShowTurnPictureArrow = dControl.isShowTurnPictureArrow;

            newDControl.opacity = dControl.opacity;
            newDControl.parentId = dControl.parentId;
            newDControl.contentWidth = dControl.contentWidth;
            newDControl.contentHeight = dControl.contentHeight;
            newDControl.backgroundImageId = dControl.backgroundImageId;
            newDControl.isTab = dControl.isTab;
            newDControl.rowNum = dControl.rowNum;
            newDControl.spacing = dControl.spacing;
            newDControl.rotateAngle = dControl.rotateAngle;
            newDControl.isDialogLink = dControl.isDialogLink;
            newDControl.showInWhichCFrame = dControl.showInWhichCFrame;
            newDControl.isTransparentDialog = dControl.isTransparentDialog;
            newDControl.fontFamily = dControl.fontFamily;
            newDControl.fontSize = dControl.fontSize;
            newDControl.fontLineHeight = dControl.fontLineHeight;
            newDControl.fontColor = dControl.fontColor;
            newDControl.fontWeight = dControl.fontWeight;
            newDControl.fontTextAlignment = dControl.fontTextAlignment;
            newDControl.storageIdOfCover = dControl.storageIdOfCover;
            newDControl.screenCfgId = dControl.screenCfgId;
            newDControl.isHideVideoConsoleOfFirstLoad = dControl.isHideVideoConsoleOfFirstLoad;
            return newDControl;

        }


        public static DControlDto convert(DControl dControl)
        {

            DControlDto dto = new DControlDto();
            dto.id = dControl.id;
            dto.pageId = dControl.pageId;
            dto.name = dControl.name;
            dto.width = dControl.width;
            dto.height = dControl.height;
            dto.left = dControl.left;
            dto.top = dControl.top;
            dto.type = dControl.type;
            dto.content = dControl.content;
            dto.idx = dControl.idx;

            dto.linkToPageId = dControl.linkToPageId;
            dto.isClickShow = dControl.isClickShow;
            dto.linkToVideoId = dControl.linkToVideoId;
            dto.autoplay = dControl.autoplay;
            dto.loop = dControl.loop;
            dto.turnPictureSpeed = dControl.turnPictureSpeed;
            dto.linkToWeb = dControl.linkToWeb;
            dto.storageId = dControl.storageId;
            dto.isShowTurnPictureArrow = dControl.isShowTurnPictureArrow;

            dto.opacity = dControl.opacity;
            dto.parentId = dControl.parentId;
            dto.contentWidth = dControl.contentWidth;
            dto.contentHeight = dControl.contentHeight;
            dto.backgroundImageId = dControl.backgroundImageId;
            dto.isTab = dControl.isTab;
            dto.rowNum = dControl.rowNum;
            dto.spacing = dControl.spacing;
            dto.rotateAngle = dControl.rotateAngle;
            dto.isDialogLink = dControl.isDialogLink;
            dto.showInWhichCFrame = dControl.showInWhichCFrame;
            dto.isTransparentDialog = dControl.isTransparentDialog;
            dto.fontFamily = dControl.fontFamily;
            dto.fontSize = dControl.fontSize;
            dto.fontLineHeight = dControl.fontLineHeight;
            dto.fontColor = dControl.fontColor;
            dto.fontWeight = dControl.fontWeight;
            dto.fontTextAlignment = dControl.fontTextAlignment;
            dto.storageIdOfCover = dControl.storageIdOfCover;
            dto.screenCfgId = dControl.screenCfgId;
            dto.isHideVideoConsoleOfFirstLoad = dControl.isHideVideoConsoleOfFirstLoad;
            return dto;

        }
    }
}
