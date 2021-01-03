using Bll;
using Model;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace WpfApplication1.FolderWin
{
    /// <summary>
    /// DeleteFileWindow.xaml 的交互逻辑
    /// </summary>
    public partial class DeleteFileWindow : Window
    {

        private readonly WrapPanel storageListWrap;
        private readonly List<StorageFile> list;
        private readonly List<Canvas> canvasList;

        private readonly StorageFileBll storageFileBll = new StorageFileBll();


        public DeleteFileWindow(WrapPanel storageListWrap, List<StorageFile> list, List<Canvas> canvasList)
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
            deleteInfo.Content = "确定要删除这 " + count + " 个文件吗？";
        }

        private void Submit_Button_Click(object sender, RoutedEventArgs e)
        {
            //1.删除数据库记录
            foreach (StorageFile storageFile in list)
            {
                int row = storageFileBll.delete(storageFile);
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
