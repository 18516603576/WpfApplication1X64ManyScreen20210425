using Model;
using System;

namespace Common.Data
{
    public class AnimationListTag
    {
        //是否选中 
        public Boolean isSelected { get; set; }

        //控件绑定的数据
        public DControlAnimation dControlAnimation { get; set; }
    }
}
