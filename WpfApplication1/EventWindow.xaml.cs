using System;
using System.Windows;

namespace WpfApplication1
{
    /// <summary>
    /// EventWindow.xaml 的交互逻辑
    /// </summary>
    public partial class EventWindow : Window
    {
        public EventWindow()
        {
            InitializeComponent();
            
            insertButton.PreviewMouseUp +=  insertButton_Click ;
            unBindEventButton.Click += unBindEventButton_Click;
        }

        private void unBindEventButton_Click(object sender, RoutedEventArgs e)
        {
            // insertButton.Click -=(sender1,e1) => insertButton_Click(sender1,e1,""); 
            insertButton.PreviewMouseUp -= insertButton_Click ;
        }

        private void insertButton_Click(object sender, RoutedEventArgs e )
        {
            string dt = DateTime.Now.ToString();
            textBox1.Text = textBox1.Text + dt;
        }
    }
}
