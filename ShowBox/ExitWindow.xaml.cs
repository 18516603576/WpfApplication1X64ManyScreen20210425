using Bll;
using Model;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ShowBox
{
    /// <summary>
    /// ExitWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ExitWindow : Window
    {
        private readonly MainWindow mainWindow;
        private readonly CfgBll cfgBll = new CfgBll();

        private readonly PasswordTag passwordTag = new PasswordTag();
        public ExitWindow(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            //初始化正确密码
            Cfg cfg = cfgBll.get(1);
            passwordTag.rightPassword = cfg.password;
            passwordTag.inputPassword = "";
            pointWrapPanel.Tag = passwordTag;

            initPoint(6);
        }

        /*
         * 初始化密码对应的点
         */
        private void initPoint(int num)
        {
            for (int i = 0; i < num; i++)
            {
                Button p = new Button();
                p.Tag = "";
                p.Width = 36;
                p.Height = 36;
                p.BorderThickness = new Thickness(0);
                p.Background = new ImageBrush
                {
                    ImageSource = new BitmapImage(new Uri("pack://application:,,,/ShowBox;component/Resources/ico_password_circle.png", UriKind.Absolute))
                    ,
                    Stretch = Stretch.Fill
                };
                pointWrapPanel.Children.Add(p);
            }


        }



        /*
         * 回撤键
         */
        private void Delete_Number_Click(object sender, RoutedEventArgs e)
        {
            int len = passwordTag.inputPassword.Length;
            if (len <= 0) return;
            passwordTag.inputPassword = passwordTag.inputPassword.Substring(0, len - 1);
            unselectOnePoint(len - 1);
        }

        /*
         * 点击数字按钮
         */
        private void Number_Button_Click(object sender, RoutedEventArgs e)
        {
            if (passwordTag.inputPassword.Length >= 6)
            {
                passwordTag.inputPassword = "";
                unselectAllPoint();
                return;
            };

            //2.更新passwordTag,选中点
            Button btn = (Button)sender;
            string val = btn.Content.ToString();
            passwordTag.inputPassword = passwordTag.inputPassword + val;
            selectOnePoint(passwordTag.inputPassword.Length - 1);


            //1.当前输入第6个数字
            if (passwordTag.inputPassword.Length == 6)
            {
                if (passwordTag.rightPassword == passwordTag.inputPassword)
                {

                    MessageBoxResult dr = MessageBox.Show("确认关闭软件？", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                    if (dr == MessageBoxResult.OK)
                    {
                        Close();
                        mainWindow.Close();
                    }
                    passwordTag.inputPassword = "";
                    unselectAllPoint();
                }
                else
                {
                    MessageBox.Show("密码输入错误，请重新输入");
                    passwordTag.inputPassword = "";
                    unselectAllPoint();
                }
            }
        }

        /*
         * 选中某个点
         */
        private void selectOnePoint(int idx)
        {
            if (idx < 0) { return; }
            UIElementCollection children = pointWrapPanel.Children;
            int i = 0;
            foreach (UIElement ui in children)
            {
                if (i == idx)
                {
                    Button btn1 = (Button)ui;
                    btn1.Background = new ImageBrush
                    {
                        ImageSource = new BitmapImage(new Uri("pack://application:,,,/ShowBox;component/Resources/ico_password_circle_active.png", UriKind.Absolute))
                    };

                    break;
                }
                i = i + 1;
            }

        }

        /*
         * 不选中某个点
         */
        private void unselectOnePoint(int idx)
        {
            if (idx < 0) { return; }
            UIElementCollection children = pointWrapPanel.Children;
            int i = 0;
            foreach (UIElement ui in children)
            {
                if (i == idx)
                {
                    Button btn1 = (Button)ui;
                    btn1.Background = new ImageBrush
                    {
                        ImageSource = new BitmapImage(new Uri("pack://application:,,,/ShowBox;component/Resources/ico_password_circle.png", UriKind.Absolute))
                    };

                    break;
                }
                i = i + 1;

            }

        }

        /*
         *  取消所有点
         */
        private void unselectAllPoint()
        {
            UIElementCollection children = pointWrapPanel.Children;
            foreach (UIElement ui in children)
            {
                Button btn1 = (Button)ui;
                btn1.Background = new ImageBrush
                {
                    ImageSource = new BitmapImage(new Uri("pack://application:,,,/ShowBox;component/Resources/ico_password_circle.png", UriKind.Absolute))
                };
            }
        }


    }
}
