using System;

namespace Common.Data
{
    public class BaseResult
    {
        public Boolean result { get; set; }

        public Int32 errorCode { get; set; }

        public string message { get; set; }
    }
}
