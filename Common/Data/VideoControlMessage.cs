using System;

namespace Common.Data
{
    public class VideoControlMessage
    {
        public VideoControlMessage(Int32 id, string message)
        {
            this.id = id;
            this.message = message;
        }
        public Int32 id { get; set; }

        public string message { get; set; }

    }
}
