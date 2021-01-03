using Bll;
using Model;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace WpfApplication1.FolderWin
{
    /// <summary>
    /// DeleteVideoWindow.xaml 的交互逻辑
    /// </summary>
    public partial class DeleteVideoWindow : Window
    {

        private readonly WrapPanel storageListWrap;
        private readonly List<StorageVideo> list;
        private readonly List<Canvas> canvasList;

        private readonly StorageVideoBll storageVideoBll = new StorageVideoBll();


        public DeleteVideoWindow(WrapPanel storageListWrap, List<StorageVideo> list, List<Canvas> canvasList)
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.storageListWrap = storageListWrap;
            this.list = list;
            this.canvasList = canvasList;
            initDeleteInfo();
        }

        /*
         * 获取要删除的页面信息
         */
        private void initDeleteInfo()
        {
            int count = list.Count;
            deleteInfo.Content = "确定要删除这 " + count + " 个视频吗？";
        }

        private void Submit_Button_Click(object sender, RoutedEventArgs e)
        {
            //1.删除数据库记录
            foreach (StorageVideo storageVideo in list)
            {
                storageVideoBll.delete(storageVideo);
            }

            //2.从页面移除选中项
            foreach (Canvas canvas in canvasList)
            {
                storageListWrap.Children.Remove(canvas);
            }
            //3.关闭窗口
            Close();
        }

        /*
         * 取消事件 
         */
        private void Cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
