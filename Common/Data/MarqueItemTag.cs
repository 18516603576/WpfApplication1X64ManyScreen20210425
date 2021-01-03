using Model.dto;

namespace Common.Data
{
    public class MarqueItemTag : TurnPictureImagesDto
    {
        public double scale { get; set; }
        //层叠起伏 水平偏移
        public double translateX { get; set; }
        //层叠起伏 编号
        public int idx { get; set; }
    }
}
