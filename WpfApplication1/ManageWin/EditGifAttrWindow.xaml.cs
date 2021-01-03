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
    /// EditImageAttrWindow.xaml 的交互逻辑
    /// </summary>
    public partial class EditGifAttrWindow : Window
    {
        private readonly DControlBll dControlBll = new DControlBll();
        private readonly StorageGifBll storageGifBll = new StorageGifBll();
        private DControl currDControl;
        private readonly FrameworkElement currElement;
        private readonly Editing editing;
        public EditGifAttrWindow(Editing editing, FrameworkElement currElement)
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
            StorageGif storageGif = storageGifBll.get(ctl.storageId);
            string imgFullPath = FileUtil.notExistsShowDefault(storageGif?.url, Params.GifNotExists);
            string filename = FileUtil.getFilename(imgFullPath);
            string fullFolder = FileUtil.getDirectory(AppDomain.CurrentDomain.BaseDirectory + imgFullPath);

            url.Content = filename;
            url.Tag = fullFolder;
            width.Text = ctl.width.ToString();
            height.Text = ctl.height.ToString();
            left.Text = ctl.left.ToString();
            top.Text = ctl.top.ToString();
            isClickShow.IsChecked = ctl.isClickShow;
            opacity.Text = ctl.opacity.ToString();
            idx.Content = ctl.idx.ToString();
            isTab.IsChecked = ctl.isTab;
            rotateAngle.Text = ctl.rotateAngle.ToString();

            actualWidthHeight.Content = storageGif?.actualWidth + "*" + storageGif?.actualHeight;
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

            if (string.IsNullOrWhiteSpace(rotateAngle.Text))
            {
                MessageBox.Show("请填写旋转角度；"); return;
            }
            else if (!DataUtil.isInt(rotateAngle.Text.ToString()))
            {
                MessageBox.Show("旋转角度必须是整数；"); return;
            }
            else
            {
                Int32 rotateAngleVal = Convert.ToInt32(rotateAngle.Text.ToString());
                if (rotateAngleVal < 0 || rotateAngleVal > 360)
                {
                    MessageBox.Show("旋转角度为0-360内的整数；"); return;
                }
            }


            //更新到数据库
            DControl dControl = dControlBll.get(currDControl.id);
            dControl.width = int.Parse(width.Text);
            dControl.height = int.Parse(height.Text);
            dControl.left = int.Parse(left.Text);
            dControl.top = int.Parse(top.Text);
            dControl.isClickShow = (Boolean)isClickShow.IsChecked;
            dControl.opacity = int.Parse(opacity.Text);
            dControl.isTab = (Boolean)isTab.IsChecked;
            dControl.rotateAngle = int.Parse(rotateAngle.Text);
            dControlBll.update(dControl);



            //更新页面控件信息,更新isClickShow
            currDControl = dControl;
            currElement.Tag = currDControl;
            Gif button = (Gif)currElement;
            button.updateElementAttr(currDControl, true);

            //button.Width = currDControl.width;
            //button.Height = currDControl.height; 
            //button.Margin = new Thickness(currDControl.left,currDControl.top,0,0);
            //button.Opacity = currDControl.opacity / 100.0;
            //TransformGroup group = (TransformGroup) button.RenderTransform;
            //RotateTransform rotateTransform = TransformGroupUtil.GetRotateTransform(group);
            //rotateTransform.Angle = currDControl.rotateAngle;

            //2.修改8个点的位置 及宽高 
            editing.updateEditingBorder(currDControl);


            Close();

        }

    }
}
