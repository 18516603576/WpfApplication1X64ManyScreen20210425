using Bll;
using Model;
using System.Windows;

namespace WpfApplication1.ManageWin
{
    /// <summary>
    /// CfgWeekWindow.xaml 的交互逻辑
    /// </summary>
    public partial class EditCfgWeekWindow : Window
    {
        private readonly CfgBll cfgBll = new CfgBll();
        public EditCfgWeekWindow()
        {
            InitializeComponent();
            loadPageData();
        }

        private void loadPageData()
        {
            week1.Text = App.localStorage.cfg.week1;
            week2.Text = App.localStorage.cfg.week2;
            week3.Text = App.localStorage.cfg.week3;
            week4.Text = App.localStorage.cfg.week4;
            week5.Text = App.localStorage.cfg.week5;
            week6.Text = App.localStorage.cfg.week6;
            week7.Text = App.localStorage.cfg.week7;
        }

        private void Submit_Button_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(week1.Text))
            {
                MessageBox.Show("请填写星期一文字；"); return;
            }
            if (string.IsNullOrWhiteSpace(week2.Text))
            {
                MessageBox.Show("请填写星期二文字；"); return;
            }
            if (string.IsNullOrWhiteSpace(week3.Text))
            {
                MessageBox.Show("请填写星期三文字；"); return;
            }
            if (string.IsNullOrWhiteSpace(week4.Text))
            {
                MessageBox.Show("请填写星期四文字；"); return;
            }
            if (string.IsNullOrWhiteSpace(week5.Text))
            {
                MessageBox.Show("请填写星期五文字；"); return;
            }
            if (string.IsNullOrWhiteSpace(week6.Text))
            {
                MessageBox.Show("请填写星期六文字；"); return;
            }
            if (string.IsNullOrWhiteSpace(week7.Text))
            {
                MessageBox.Show("请填写星期日文字；"); return;
            }


            //更新到数据库
            Cfg cfg = cfgBll.get(1);
            cfg.week1 = week1.Text;
            cfg.week2 = week2.Text;
            cfg.week3 = week3.Text;
            cfg.week4 = week4.Text;
            cfg.week5 = week5.Text;
            cfg.week6 = week6.Text;
            cfg.week7 = week7.Text;
            cfgBll.update(cfg);


            App.localStorage.cfg = cfg;

            Close();

        }
    }
}
