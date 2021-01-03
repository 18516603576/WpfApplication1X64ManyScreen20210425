using Model.dto;

namespace Common.Data
{
    public class MarqueItemTagUtil
    {
        public static MarqueItemTag convert(TurnPictureImagesDto dto)
        {

            MarqueItemTag tag = new MarqueItemTag();
            tag.id = dto.id;
            tag.dControlId = dto.dControlId;
            tag.storageImageId = dto.storageImageId;
            tag.isLink = dto.isLink; 
            tag.url = dto.url;

            return tag;

        }
    }
}
