using Bll;
using Common;
using Common.Data;
using Model;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WpfApplication1.manage;

namespace WpfApplication1.ManageWin
{
    /// <summary>
    /// EditWordAttrWindow.xaml 的交互逻辑
    /// </summary>
    public partial class EditWordAttrWindow : Window
    {
        private readonly DControlBll dControlBll = new DControlBll();
        private readonly StorageFileBll storageFileBll = new StorageFileBll();
        private DControl currDControl;
        private readonly FrameworkElement currElement;
        private readonly Editing editing;
        public EditWordAttrWindow(Editing editing, FrameworkElement currElement)
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.editing = editing;
            this.currElement = currElement;
            this.currDControl = (DControl)currElement.Tag;
            loadPageData(currDControl); 
        }

        private void url_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Label label = (Label)sender;
            string fullFolder = label.Tag?.ToString();

            FileUtil.openFile(fullFolder);  

        }

        private void loadPageData(DControl ctl)
        {
            StorageFile storageFile = storageFileBll.get(ctl.storageId);
            string wordFullPath = FileUtil.notExistsShowDefault(storageFile?.url, Params.WordNotExists);
            string filename = FileUtil.getFilename(wordFullPath); 
            string fullFolder = FileUtil.getDirectory(AppDomain.CurrentDomain.BaseDirectory + wordFullPath); 
 
            url.Content = filename;
            url.Tag = fullFolder;
            width.Text = ctl.width.ToString();
            height.Text = ctl.height.ToString();
            left.Text = ctl.left.ToString();
            top.Text = ctl.top.ToString();
            opacity.Text = ctl.opacity.ToString();
            idx.Content = ctl.idx.ToString();
            url.PreviewMouseLeftButtonUp += url_PreviewMouseLeftButtonUp;
        }
        //
        private void Submit_Button_Click(object sender, RoutedEventArgs e)
        {

            if (string.IsNullOrWhiteSpace(width.Text))
            {
                MessageBox.Show("请填写宽度；"); return;
            }
            else if (!DataUtil.isInt(width.Text.ToString()))
            {
                MessageBox.Show("宽度必须是整数；"); return;
            }
            if (string.IsNullOrWhiteSpace(height.Text.ToString()))
            {
                MessageBox.Show("请填写高度；"); return;
            }
            else if (!DataUtil.isInt(height.Text.ToString()))
            {
                MessageBox.Show("高度必须是整数；"); return;
            }
            if (string.IsNullOrWhiteSpace(left.Text.ToString()))
            {
                MessageBox.Show("请填写左边距；"); return;
            }
            else if (!DataUtil.isInt(left.Text.ToString()))
            {
                MessageBox.Show("左边距请填写整数；"); return;
            }
            if (string.IsNullOrWhiteSpace(top.Text.ToString()))
            {
                MessageBox.Show("请填写上边距；"); return;
            }
            else if (!DataUtil.isInt(top.Text.ToString()))
            {
                MessageBox.Show("上边距请填写整数；"); return;
            }
            if (string.IsNullOrWhiteSpace(opacity.Text.ToString()))
            {
                MessageBox.Show("请填写透明度；"); return;
            }
            else if (!DataUtil.isInt(opacity.Text.ToString()))
            {
                MessageBox.Show("透明度请填写整数；"); return;
            }
            else
            {
                Int32 opacityVal = Convert.ToInt32(opacity.Text.ToString());
                if (opacityVal < 0 || opacityVal > 100)
                {
                    MessageBox.Show("透明度为0-100内的整数；"); return;
                }
            }


            //更新到数据库
            DControl dControl = dControlBll.get(currDControl.id);
            //  dControl.url = url.Text;
            dControl.width = int.Parse(width.Text);
            dControl.height = int.Parse(height.Text);
            dControl.left = int.Parse(left.Text);
            dControl.top = int.Parse(top.Text);
            dControl.opacity = int.Parse(opacity.Text);
            dControlBll.update(dControl);
            currDControl = dControl;
            currElement.Tag = currDControl;


            //更新页面控件信息,更新isClickShow
            //CWord cWord = (CWord)currElement;
            //cWord.Width = currDControl.width;
            //cWord.Height = currDControl.height;
            //cWord.Margin = new Thickness(currDControl.left,currDControl.top,0,0); 
            //cWord.fitToWidth();

            DocumentViewer docViewer = (DocumentViewer)currElement;
            docViewer.Width = dControl.width;
            docViewer.Height = dControl.height;
            docViewer.Margin = new Thickness(dControl.left, dControl.top, 0, 0);
            docViewer.Opacity = dControl.opacity / 100.0;
            docViewer.FitToWidth();

            //2.修改8个点的位置 及宽高 
            editing.updateEditingBorder(currDControl);


            Close();

        }
    }
}
