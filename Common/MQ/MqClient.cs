using System;
using System.Windows;

namespace Common.MQ
{
    public class MqClient
    {
        // 订阅事件：客户端订阅服务端推送消息的功能
        public void Subscription()
        {
            Console.WriteLine("客户端订阅了推送事件！");
            // MqServer.sendMsgEvent += Server_sendMsgEvent; // 实例化，给事件绑定方法
        }

        private void Server_sendMsgEvent(string msg)
        {
            MessageBox.Show("客户端接收到的推送消息：" + msg);
        }

    }
}
