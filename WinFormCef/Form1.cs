using CefSharp;
using CefSharp.WinForms;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using WinformCef.CefDiy;
using WinFormCef.CefDiy;
using WinFormCef.control;
using WinFormCef.util;

namespace WinFormCef
{
    public partial class Form1 : Form
    {
        //配置信息
        private readonly BrowserParam browserParam;
        private ChromiumWebBrowser wb;
        //底部菜单高度
        private readonly int menuHeight = 132;
        private ImageButton backBtn;
        private ImageButton advanceBtn;


        public Form1(int winWidth, int winHeight, string url, double wpfScreenWidth)
        {
            InitializeComponent();
            wpfScreenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;//得到屏幕整体宽度  
            double winFormScreenWidth = Screen.PrimaryScreen.Bounds.Width;
            double scale = winFormScreenWidth / wpfScreenWidth;

            int finalWidth = (int)(winWidth * scale);
            int finalHeight = (int)(winHeight * scale);


            browserParam = new BrowserParam();
            browserParam.winWidth = finalWidth;
            browserParam.winHeight = finalHeight;
            browserParam.url = url;


            init(); 
        }

        /*
         * 初始化
         */
        private void init()
        {
            initCefSetting();
            ////设置全屏无边框 
            showWindowsSizeByConfig();

            //是否显示底部菜单
            showMenu();

            AddBrowser();
            
        }

        /*
        * 初始化cef配置
        */
        private void initCefSetting()
        {

            Control.CheckForIllegalCrossThreadCalls = false;
            CefSharpSettings.LegacyJavascriptBindingEnabled = true;


            var settings = new CefSettings();
            settings.Locale = "zh-CN";
            //chrome缓存目录 
            settings.CachePath = Application.StartupPath + @"\cache\";
            //自带flash插件
            //settings.CefCommandLineArgs.Add("--enable-system-flash", "0");
            //settings.CefCommandLineArgs.Add("ppapi-flash-path", Application.StartupPath + @"\myfile\sysimg\ppflash\29_0_0_171\pepflashplayer32_29_0_0_171.dll");
            //settings.CefCommandLineArgs.Add("ppapi-flash-version", "29.0.0.171");
            settings.CefCommandLineArgs.Add("ppapi-flash-path", Application.StartupPath + @"\myfile\sysimg\ppflash\33_0_0_401\pepflashplayer64_33_0_0_401.dll");
            settings.CefCommandLineArgs.Add("ppapi-flash-version", "33.0.0.401");
            settings.CefCommandLineArgs.Add("enable-media-stream", "1");

            settings.IgnoreCertificateErrors = true;
            settings.LogSeverity = LogSeverity.Verbose;
            if (!Cef.IsInitialized)
            {
                Cef.Initialize(settings);
            }

        }
        //软件窗口大小
        //如果实际屏幕宽度与编辑宽度相同，则全屏 screenWidth == editWidth
        public void showWindowsSizeByConfig()
        {

            HideTask(true);
            FormBorderStyle = FormBorderStyle.None;
            //this.WindowState = FormWindowState.Maximized; 

            Width = browserParam.winWidth;
            Height = browserParam.winHeight;



            //这个区域包括任务栏，就是屏幕显示的物理范围
            Rectangle ScreenArea = System.Windows.Forms.Screen.GetBounds(this);
            int swidth = ScreenArea.Width;
            int sheight = ScreenArea.Height;
            int x = (swidth - Width) / 2;
            int y = (sheight - Height) / 2;
            Location = new System.Drawing.Point(x, y); //指定窗体显示在右下角

            AutoScroll = true;
            AutoSize = true;


        }



        private void AddBrowser()
        {
            Panel CefPanel = new Panel();
            CefPanel.Name = "CefPanel";
            CefPanel.Width = browserParam.winWidth;
            CefPanel.Height = browserParam.winHeight - menuHeight;  //窗口高度 - 菜单高度
            CefPanel.Left = 0;
            CefPanel.Top = 0;

            string webUrl = WebUtil.getFullUrl(browserParam.url);
            wb = new ChromiumWebBrowser(webUrl);
            wb.LifeSpanHandler = new OpenPageSelf();
            wb.Name = "Cef";
            wb.KeyboardHandler = new KeyBoardHander();  //F5刷新 
            wb.MenuHandler = new MenuHandler();   //屏蔽右击   
            wb.DownloadHandler = new DownloadHandler();



            //允许C# js通信 
            wb.RegisterJsObject("jsObj", new BoundObject());

            wb.IsAccessible = true;
            wb.LoadingStateChanged += (sender, e) => loadingStateChanaged(sender, e);
            wb.FrameLoadEnd += (sender, e) => browserEnd(sender, e);


            CefPanel.Controls.Add(wb);
            Controls.Add(CefPanel);
            // CefPanel.SendToBack();//---------
        }

        private void browserEnd(object sender, EventArgs e)
        {
            try
            {
                ChromiumWebBrowser wb = (ChromiumWebBrowser)sender;
                wb.ExecuteScriptAsync("	 var inputFun2 = function(e){  var target = e.target; var nodeName =  target.nodeName;  var type = target.type;   if( (nodeName=='INPUT' && (!type || type=='' || type=='text'||type=='password')) || nodeName=='TEXTAREA'){ jsObj.openKeyboard(); }else{ jsObj.closeKeyboard(); } }; document.addEventListener('touchstart',inputFun2,false);  var input = document.getElementsByTagName('input'),textarea = document.getElementsByTagName('textarea');var inputFun = function(e){ 	jsObj.openKeyboard(); e.stopPropagation();	};for(var i=0;i<input.length;i++){var type = input[i].getAttribute('type');if(!type||type==''||type=='text'){	 input[i].addEventListener('touchstart', inputFun,false);}}; for(var i=0;i<textarea.length;i++){ textarea[i].addEventListener('touchstart', inputFun,false); }");
            }
            catch (Exception)
            {

            }

        }

        /*
         * 浏览器加载状态变更，更新按钮背景
         */
        private void loadingStateChanaged(object sender, EventArgs e)
        {
            if (wb == null) return;
            IBrowser iBrowser = wb.GetBrowser();

            if (iBrowser.CanGoBack)
            {

                backBtn.BackgroundImage = Properties.Resources.ico_cef_back_active;

            }
            else
            {

                backBtn.BackgroundImage = Properties.Resources.ico_cef_back;

            }
            if (iBrowser.CanGoForward)
            {

                advanceBtn.BackgroundImage = Properties.Resources.ico_cef_advance_active;

            }
            else
            {
                advanceBtn.BackgroundImage = Properties.Resources.ico_cef_advance;
            }

        }




        #region
        //显示底部菜单栏  首页、上一页、下一页
        public void showMenu()
        {
            MenuPanel menuPanel; ImageButton refreshBtn; ImageButton homeBtn; ImageButton closeBtn;
            menuPanel = new MenuPanel();
            menuPanel.Height = menuHeight;
            menuPanel.Location = new Point(0, 0);
            menuPanel.BackColor = Color.Transparent;
            menuPanel.ForeColor = Color.Transparent;
            menuPanel.Dock = DockStyle.Bottom;
            //   menuPanel.BorderStyle = BorderStyle.FixedSingle;

            //if (!string.IsNullOrEmpty(menuBg))
            //{
            //    menuPanel.BackgroundImage = Image.FromFile(Application.StartupPath + menuBg);
            //    menuPanel.BackgroundImageLayout = ImageLayout.Tile;
            //}

            int btnWidth = 100;
            int start = (browserParam.winWidth - btnWidth * 5 - 200) / 2;

            homeBtn = new ImageButton();
            homeBtn.Size = new Size(btnWidth, btnWidth);
            homeBtn.Name = "首页";
            homeBtn.Location = new Point(start, 16);
            homeBtn.BackgroundImage = Properties.Resources.ico_cef_home;

            homeBtn.Click += (sender, e) => homeClick(sender, e);


            backBtn = new ImageButton();
            backBtn.Size = new Size(btnWidth, btnWidth);
            backBtn.Name = "backBtn";
            backBtn.Location = new Point(start + 50 + btnWidth, 16);
            backBtn.BackgroundImage = Properties.Resources.ico_cef_back;
            backBtn.Click += (sender, e) => backClick(sender, e);


            advanceBtn = new ImageButton();
            advanceBtn.Size = new Size(btnWidth, btnWidth);
            advanceBtn.Name = "前进";
            advanceBtn.Location = new Point(start + 100 + btnWidth * 2, 16);
            advanceBtn.BackgroundImage = Properties.Resources.ico_cef_advance;
            advanceBtn.Click += (sender, e) => advanceClick(sender, e);

            refreshBtn = new ImageButton();
            refreshBtn.Size = new Size(btnWidth, btnWidth);
            refreshBtn.Name = "刷新";
            refreshBtn.Location = new Point(start + 150 + btnWidth * 3, 16);
            refreshBtn.BackgroundImage = Properties.Resources.ico_cef_refresh;
            refreshBtn.Click += (sender, e) => refreshClick(sender, e);

            closeBtn = new ImageButton();
            closeBtn.Size = new Size(btnWidth, btnWidth);
            closeBtn.Name = "关闭";
            closeBtn.Location = new Point(start + 200 + btnWidth * 4, 16);
            closeBtn.BackgroundImage = Properties.Resources.ico_cef_close;
            closeBtn.Click += (sender, e) => closeClick(sender, e);

            menuPanel.Controls.Add(homeBtn);
            menuPanel.Controls.Add(backBtn);
            menuPanel.Controls.Add(advanceBtn);
            menuPanel.Controls.Add(refreshBtn);
            menuPanel.Controls.Add(closeBtn);
            Controls.Add(menuPanel);
        }

        private void closeClick(object sender, EventArgs e)
        {
            Close();
        }

        private void backClick(object sender, EventArgs e)
        {
            wb.Back();
            wb.Focus();

        }

        private void advanceClick(object sender, EventArgs e)
        {
            wb.Forward();
            wb.Focus();
        }

        private void homeClick(object sender, EventArgs e)
        {
            string url = WebUtil.getFullUrl(browserParam.url);
            wb.Load(url);
            wb.Focus();
        }

        private void refreshClick(object sender, EventArgs e)
        {
            wb.Reload();
            wb.Focus();
        }

        #endregion




        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            HideTask(false);
        }

        //隐藏任务栏
        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(ref Point lpPoint);

        [DllImport("user32.dll", EntryPoint = "FindWindowEx", SetLastError = true)]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("user32.dll", EntryPoint = "ShowWindow", SetLastError = true)]
        public static extern bool ShowWindow(IntPtr hWnd, uint nCmdShow);

        public static Point GetCursorPos()
        {
            Point point = new Point();
            GetCursorPos(ref point);
            return point;
        }
        public static void HideTask(bool isHide)
        {
            try
            {
                IntPtr trayHwnd = FindWindowEx(IntPtr.Zero, IntPtr.Zero, "Shell_TrayWnd", null);
                IntPtr hStar = FindWindowEx(IntPtr.Zero, IntPtr.Zero, "Button", null);

                if (isHide)
                {
                    ShowWindow(trayHwnd, 0);
                    ShowWindow(hStar, 0);
                }
                else
                {
                    ShowWindow(trayHwnd, 1);
                    ShowWindow(hStar, 1);
                }
            }
            catch { }
        }


    }
}
