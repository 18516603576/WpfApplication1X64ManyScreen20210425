using System;

namespace Common.Data
{
    public class TaskInfo
    {
        //任务状态  1：待执行  2：执行中
        public Int16 taskStatus { get; set; }

        //加载页面  1：加载页面   2：移除页面
        public Boolean isLoadPage { get; set; }
    }
}
