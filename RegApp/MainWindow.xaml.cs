using Common.util;
using System.Windows;

namespace RegApp
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        /*
         * 点击 - 生成注册码
         */
        private void Submit_Button_Click(object sender, RoutedEventArgs e)
        {
            string machineCode = machineCodeText.Text;
            if (string.IsNullOrWhiteSpace(machineCode))
            {
                MessageBox.Show("请填写机器码");
                return;
            }
            regCodeText.Text = createRegCode(machineCode);
        }

        /*
         * 生成注册码
         */
        private string createRegCode(string machineCode)
        {

            if (string.IsNullOrWhiteSpace(machineCode)) return "";

            string rightRegCode = RegCodeUtil.generate(machineCode);


            return rightRegCode;

        }
    }
}
