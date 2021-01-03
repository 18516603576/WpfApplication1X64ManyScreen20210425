using Model;
using Model.dto;

namespace Common.util
{
    public class StorageVideoUtil
    {

        public static StorageVideoDto convert(StorageVideo sv)
        {
            StorageVideoDto dto = new StorageVideoDto();
            dto.id = sv.id;
            dto.origFilename = sv.origFilename;
            dto.url = sv.url;
            dto.size = sv.size;
            dto.ext = sv.ext;
            dto.createTime = sv.createTime;

            dto.duration = sv.duration;
            dto.storageImageId = sv.storageImageId;
            dto.folderId = sv.folderId;
            return dto;

        }
    }
}
