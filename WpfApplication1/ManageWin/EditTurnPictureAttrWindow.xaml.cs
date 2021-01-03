using Bll;
using Common;
using Common.control;
using Model;
using Model.dto;
using System;
using System.Collections.Generic;
using System.Windows;
using WpfApplication1.manage;

namespace WpfApplication1.ManageWin
{
    /// <summary>
    /// EditTurnPictureAttrWindow.xaml 的交互逻辑
    /// </summary>
    public partial class EditTurnPictureAttrWindow : Window
    {
        private readonly DControlBll dControlBll = new DControlBll();
        private readonly StorageVideoBll storageMedia = new StorageVideoBll();
        private readonly TurnPictureImagesBll turnPictureImagesBll = new TurnPictureImagesBll();
        private DControl currDControl;
        private readonly FrameworkElement currElement;
        private readonly Editing editing;
        public EditTurnPictureAttrWindow(Editing editing, FrameworkElement currElement)
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.editing = editing;
            this.currElement = currElement;
            currDControl = (DControl)currElement.Tag;
            List<TurnPictureImagesDto> list = turnPictureImagesBll.getByDControlId(currDControl.id);

            loadPageData(currDControl, list);
        }

        private void loadPageData(DControl ctl, List<TurnPictureImagesDto> list)
        {

            imageNum.Text = list.Count + "张";
            width.Text = ctl.width.ToString();
            height.Text = ctl.height.ToString();
            left.Text = ctl.left.ToString();
            top.Text = ctl.top.ToString();
            autoplay.IsChecked = ctl.autoplay;
            loop.IsChecked = ctl.loop;
            turnPictureSpeed.Text = ctl.turnPictureSpeed.ToString();
            opacity.Text = ctl.opacity.ToString();
            idx.Content = ctl.idx.ToString();
            isShowTurnPictureArrow.IsChecked = ctl.isShowTurnPictureArrow;
            isClickShow.IsChecked = ctl.isClickShow;
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
            if (string.IsNullOrWhiteSpace(turnPictureSpeed.Text.ToString()))
            {
                MessageBox.Show("请填写自动切换速度；"); return;
            }
            else if (!DataUtil.isInt(turnPictureSpeed.Text.ToString()))
            {
                MessageBox.Show("自动切换速度请填写整数；"); return;
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

            int turnPictureSpeed1 = int.Parse(turnPictureSpeed.Text.ToString());
            if (turnPictureSpeed1 <= 0)
            {
                turnPictureSpeed1 = 8;
            }



            //更新到数据库
            DControl dControl = dControlBll.get(currDControl.id);
            dControl.width = int.Parse(width.Text);
            dControl.height = int.Parse(height.Text);
            dControl.left = int.Parse(left.Text);
            dControl.top = int.Parse(top.Text);
            dControl.autoplay = (Boolean)autoplay.IsChecked;
            dControl.loop = (Boolean)loop.IsChecked;
            dControl.turnPictureSpeed = turnPictureSpeed1;
            dControl.isShowTurnPictureArrow = (Boolean)isShowTurnPictureArrow.IsChecked;
            dControl.isClickShow = (Boolean)isClickShow.IsChecked;
            dControl.opacity = int.Parse(opacity.Text);
            dControlBll.update(dControl);
            currDControl = dControl;
            currElement.Tag = currDControl;

            //更新页面控件信息  
            TurnPicture turnPicture = (TurnPicture)currElement;
            turnPicture.updateElementAttr(dControl, true);
            editing.updateEditingBorder(dControl);
            Close();

        }

    }
}
