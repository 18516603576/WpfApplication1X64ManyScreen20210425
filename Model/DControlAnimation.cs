using System;

namespace Model
{
    public class DControlAnimation
    {
        public Int32 id { get; set; }
        //控件id
        public Int32 dControlId { get; set; }
        //动画名称
        public String name { get; set; }
        //动画类型
        public Int32 type { get; set; }
        //开始时间
        public Int32 delaySeconds { get; set; }
        //动画持续时间
        public Int32 durationSeconds { get; set; }
        //播放次数  0循环播放
        public Int32 playTimes { get; set; }
        //匀速播放
        public Boolean isSameSpeed { get; set; }
        //相同透明度
        public Boolean isSameOpacity { get; set; }
    }
}
