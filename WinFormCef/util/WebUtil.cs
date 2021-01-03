using System.Windows.Forms;

namespace WinFormCef.util
{
    public class WebUtil
    {
        //url可能是本地文件，所以需要拼接上安装文件夹
        public static string getFullUrl(string url)
        {

            if (url.StartsWith(@"http://") || url.StartsWith(@"https://"))
            {

            }
            else
            {
                url = Application.StartupPath + url;
            }
            return url;
        }
    }
}
