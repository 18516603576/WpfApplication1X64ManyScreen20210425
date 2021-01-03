using Bll;
using Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowBox
{
    public class MainStartup
    {
        private ScreenCfgBll screenCfgBll = new ScreenCfgBll();
        private DPageBll dPageBll = new DPageBll();
        public MainStartup( )
        { 
            App.appWindowList.Clear();   
            List<ScreenCfg> list = screenCfgBll.findAll();
            //屏幕排序
            List<System.Windows.Forms.Screen> listScreen = new List<System.Windows.Forms.Screen>();
            foreach (System.Windows.Forms.Screen s in System.Windows.Forms.Screen.AllScreens)
            {
                listScreen.Add(s);
            }
            listScreen = this.sort(listScreen);


            for (int i = list.Count - 1; i >= 0; i--)
            {
                ScreenCfg sCfg = list[i];
                if (listScreen.Count>i && listScreen[i] != null)
                {
                    this.loadOneWindow(sCfg, listScreen[i]);
                }
                else
                {
                    this.loadOneWindow(sCfg, listScreen[0]);
                } 
            }
            
        }

        /*
         * 按照left，从小到大排序
         */
        private List<System.Windows.Forms.Screen> sort(List<System.Windows.Forms.Screen> listScreen)
        {
            for (int i = 0; i < listScreen.Count; i++)
            {
                int left1 = listScreen[i].Bounds.Left;
                for (int j = i + 1; j < listScreen.Count; j++)
                {
                    if (left1 > listScreen[j].Bounds.Left)
                    {
                        System.Windows.Forms.Screen tmp = listScreen[i];
                        listScreen[i] = listScreen[j];
                        listScreen[j] = tmp;
                    }
                }
            }
            return listScreen; 
        }

        private void loadOneWindow(ScreenCfg sCfg, System.Windows.Forms.Screen s )
        {
            //如果页面不存在显示首页
            int indexPageId = sCfg.indexPageId;
            DPage dPage =  dPageBll.get(indexPageId);
            if (dPage == null)
            {
                indexPageId = 1;
            }
             
            MainWindow win1 = new MainWindow(indexPageId, sCfg,s); 
            App.appWindowList.Add(win1);  
            Rectangle r1 = s.Bounds; 
            win1.Left = r1.Left;  
            win1.Top = r1.Top;
            win1.Show(); 
        }
    }
}
 