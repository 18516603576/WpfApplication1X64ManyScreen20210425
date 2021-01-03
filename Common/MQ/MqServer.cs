using Common.Data;
using System;

namespace Common.MQ
{

    // 1、更新视频音乐同时只有一个声音
    public delegate void DelSendMsg(VideoControlMessage msg);
    //更新日期
    public delegate void DelUpdateCCalendar(DateTime dt);


    public class MqServer
    {
        // 更新视频音乐同时只有一个声音
        public event DelSendMsg sendMsgEvent;
        public void SendMsg(VideoControlMessage msg)
        {
            if (sendMsgEvent != null)
            {
                sendMsgEvent(msg);
            }
        }




        //更新日期事件
        public event DelUpdateCCalendar sendUpdateCCalendarEvent;
        public void sendUpdateCCalendar(DateTime dt)
        {
            if (sendUpdateCCalendarEvent != null)
            {
                // 执行委托（执行事件，就是执行它注册的方法）
                sendUpdateCCalendarEvent(dt);
            }
        }




    }
}
