using Bll;
using Common.Data;
using Common.MQ;
using ShowBox;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Navigation;

namespace WpfApplication1
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
        
        public static System.Windows.Forms.ColorDialog colorDialog = new System.Windows.Forms.ColorDialog();

        
        //预览窗口列表
        public static List<PreviewWindow> previewWindowList = new List<PreviewWindow>();

        //窗口列表
        public static List<Window> appWindowList = new List<Window>();



        public App()
        { 
            
        }
  
        //LierdaCracker cracker = new LierdaCracker();
        //protected override void OnStartup(StartupEventArgs e)
        //{
        //    cracker.Cracker(60);//垃圾回收间隔时间 
        //    base.OnStartup(e);
        //} 
    }
}
