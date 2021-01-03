using Common.Data;
using Common.MQ;
using System;
using System.Collections.Generic;
using System.Windows;

namespace ShowBox
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public static LocalStorage localStorage = new LocalStorage()
        {
            cfg = null
        };
        
        
        //窗口列表
        public static List<Window> appWindowList = new List<Window>();

        public App()  
        {
            this.Startup += this_Startup;
          
            DispatcherUnhandledException += App_DispatcherUnhandledException;
        }

        private void this_Startup(object sender, StartupEventArgs e)
        {
            MainStartup mainStartup = new MainStartup();
        }


        /*
         * 未捕获的线程异常
         */
        void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show("Error:" + Environment.NewLine + e.Exception.Message);
            //Shutdown(1);
            e.Handled = true;
        } 

        //LierdaCracker cracker = new LierdaCracker();
        //protected override void OnStartup(StartupEventArgs e)
        //{
        //    cracker.Cracker(60);//垃圾回收间隔时间 
        //    base.OnStartup(e);
        //}
    }
}
