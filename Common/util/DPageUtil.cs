using Model;

namespace Common.util
{
    public class DPageUtil
    {
        public static DPage createFrom(DPage dPage)
        {

            DPage newDPage = new DPage();
            newDPage.id = dPage.id;
            newDPage.name = dPage.name;
            newDPage.parentId = dPage.parentId;
            newDPage.idx = dPage.idx;
            newDPage.createTime = dPage.createTime;
            newDPage.backgroundImageId = dPage.backgroundImageId;
            newDPage.width = dPage.width;
            newDPage.height = dPage.height;
            newDPage.pageSwitchType = dPage.pageSwitchType;
            newDPage.backgroundVideoId = dPage.backgroundVideoId;

            return newDPage;

        }
    }
}
