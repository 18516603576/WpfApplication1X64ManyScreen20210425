using Model;
using System.Windows.Controls;

namespace Common.Data
{
    public class CFrameTag
    {
        //当前Frame DControl
        public DControl currDControl { get; set; }

        public Frame currCFrame { get; set; }
        //当前框架的覆盖封面
        public Border currCoverBorder { get; set; }

        //父Frame句柄
        public Frame parentFrame { get; set; }
        //父Frame DControl
        public DControl parentDControl { get; set; }

        public Border parentCoverBorder { get; set; }

    }
}
