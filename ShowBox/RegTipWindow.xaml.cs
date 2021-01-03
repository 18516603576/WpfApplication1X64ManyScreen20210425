using Common.Data;
using System;
using System.Windows;

namespace ShowBox
{
    /// <summary>
    /// RegTipWindow.xaml 的交互逻辑
    /// </summary>
    public partial class RegTipWindow : Window
    {
        public RegTipWindow()
        {
            InitializeComponent();
        }
        private readonly MainWindow mainWindow;
        private readonly BaseResult baseResult;
        public RegTipWindow(MainWindow mainWindow, BaseResult baseResult)
        {
            this.mainWindow = mainWindow;
            this.baseResult = baseResult;
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            init(baseResult);
        }

        private void init(BaseResult baseResult)
        {
            tipContent.Content = baseResult.message;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (baseResult.errorCode == 1001)
            {
                mainWindow.Close();
            }
        }

        private void Ok_Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
