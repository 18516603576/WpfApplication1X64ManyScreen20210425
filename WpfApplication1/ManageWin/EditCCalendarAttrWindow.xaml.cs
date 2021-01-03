using Bll;
using Common;
using Common.control;
using Model;
using System;
using System.Windows;
using WpfApplication1.manage;

namespace WpfApplication1.ManageWin
{
    /// <summary>
    /// EditMediaAttrWindow.xaml 的交互逻辑
    /// </summary>
    public partial class EditCCalendarAttrWindow : Window
    {

        private readonly DControlBll dControlBll = new DControlBll();
        private readonly StorageVideoBll storageVideoBll = new StorageVideoBll();
        private DControl currDControl;
        private readonly FrameworkElement currElement;
        private readonly Editing editing;
        public EditCCalendarAttrWindow(Editing editing, FrameworkElement currElement)
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.editing = editing;
            this.currElement = currElement;
            currDControl = (DControl)currElement.Tag;
            loadPageData(currDControl);
        }

        private void loadPageData(DControl ctl)
        {


            width.Text = ctl.width.ToString();
            height.Text = ctl.height.ToString();
            left.Text = ctl.left.ToString();
            top.Text = ctl.top.ToString();

            opacity.Text = ctl.opacity.ToString();
            idx.Content = ctl.idx.ToString();
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
            dControl.opacity = int.Parse(opacity.Text);
            dControlBll.update(dControl);
            currDControl = dControl;
            currElement.Tag = currDControl;

            //更新页面控件信息  
            CCalendar cCalendar = (CCalendar)currElement;
            cCalendar.updateElementAttr(currDControl, true);


            editing.updateEditingBorder(dControl);
            Close();
        }







    }
}
