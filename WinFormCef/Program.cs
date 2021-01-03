using System;
using System.Windows.Forms;

namespace WinFormCef
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1(1366, 768, "https://www.baidu.com", 1366));
        }
    }
}
