using Bll;
using Common;
using Common.control;
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
    /// EditMediaAttrWindow.xaml 的交互逻辑
    /// </summary>
    public partial class EditVideoAttrWindow : Window
    {

        private readonly DControlBll dControlBll = new DControlBll();
        private readonly StorageVideoBll storageVideoBll = new StorageVideoBll();
        private DControl currDControl;
        private readonly FrameworkElement currElement;
        private readonly Editing editing;
        public EditVideoAttrWindow(Editing editing, FrameworkElement currElement)
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.editing = editing;
            this.currElement = currElement;
            currDControl = (DControl)currElement.Tag;
            init(currDControl);
        }

        private void init(DControl ctl)
        {

            StorageVideo storageVideo = storageVideoBll.get(ctl.storageId);
            string videoFullPath = FileUtil.notExistsShowDefault(storageVideo?.url, Params.VideoNotExists);
            string filename = FileUtil.getFilename(videoFullPath);
            string fullFolder = FileUtil.getDirectory(AppDomain.CurrentDomain.BaseDirectory + videoFullPath);

            url.Content = filename;
            url.Tag = fullFolder;
            width.Text = ctl.width.ToString();
            height.Text = ctl.height.ToString();
            left.Text = ctl.left.ToString();
            top.Text = ctl.top.ToString();
            autoplay.IsChecked = ctl.autoplay;
            loop.IsChecked = ctl.loop;
            opacity.Text = ctl.opacity.ToString();
            idx.Content = ctl.idx.ToString();
            isHideVideoConsoleOfFirstLoad.IsChecked = ctl.isHideVideoConsoleOfFirstLoad;
            url.PreviewMouseLeftButtonUp += url_PreviewMouseLeftButtonUp;
        }
        private void url_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Label label = (Label)sender;
            string fullFolder = label.Tag?.ToString();
            FileUtil.openFile(fullFolder);
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
            dControl.width = int.Parse(width.Text);
            dControl.height = int.Parse(height.Text);
            dControl.left = int.Parse(left.Text);
            dControl.top = int.Parse(top.Text);
            dControl.autoplay = (Boolean)autoplay.IsChecked;
            dControl.loop = (Boolean)loop.IsChecked;
            dControl.opacity = int.Parse(opacity.Text);
            dControl.isHideVideoConsoleOfFirstLoad = (Boolean)isHideVideoConsoleOfFirstLoad.IsChecked;
            dControlBll.update(dControl);
            currDControl = dControl;
            currElement.Tag = currDControl;

            //更新页面控件信息  
            CVideo cVideo = (CVideo)currElement;
            cVideo.updateElementAttr(dControl, true);

            editing.updateEditingBorder(dControl);
            Close();
        }







    }
}
