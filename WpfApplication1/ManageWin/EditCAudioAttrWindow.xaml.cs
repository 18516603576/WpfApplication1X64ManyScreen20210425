using Bll;
using Common;
using Common.control;
using Common.Data;
using Model;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WpfApplication1.manage;

namespace WpfApplication1.ManageWin
{
    /// <summary>
    /// EditWordAttrWindow.xaml 的交互逻辑
    /// </summary>
    public partial class EditCAudioAttrWindow : Window
    {
        private readonly DControlBll dControlBll = new DControlBll();
        private readonly StorageFileBll storageFileBll = new StorageFileBll();
        private DControl currDControl;
        private readonly FrameworkElement currElement;
        private readonly Editing editing;
        private readonly StorageImageBll storageImageBll = new StorageImageBll();
        public EditCAudioAttrWindow(Editing editing, FrameworkElement currElement)
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
            StorageFile storageFile = storageFileBll.get(ctl.storageId);
            string wordFullPath = FileUtil.notExistsShowDefault(storageFile?.url, Params.CAudioNotExists);
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
            autoplay.IsChecked = ctl.autoplay;
            loop.IsChecked = ctl.loop;

            this.loadCoverImage();
            url.PreviewMouseLeftButtonUp += url_PreviewMouseLeftButtonUp;
        }
        private void url_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Label label = (Label)sender;
            string fullFolder = label.Tag?.ToString();

            FileUtil.openFile(fullFolder);

        } 
        /*
         * 加载音乐背景
         */
        private void loadCoverImage()
        { 
            //显示封面图片
            StorageImage storageImage = null;
            if (currDControl.storageIdOfCover > 0)
            {
                  storageImage = storageImageBll.get(currDControl.storageIdOfCover);
            }
            string coverImage = FileUtil.notExistsShowDefault(storageImage?.url, Params.CAudioImageNotExists);
            storageIdOfCover.Tag = storageImage;
            storageIdOfCover.Background = new ImageBrush
            {
                ImageSource = FileUtil.readImage(AppDomain.CurrentDomain.BaseDirectory + coverImage)
               ,
                Stretch = Stretch.Uniform
            };
            storageIdOfCover.Click += storageIdOfCover_Click;

            //删除
            removeBtn.Background = new ImageBrush
            {
                ImageSource = FileUtil.readImage(AppDomain.CurrentDomain.BaseDirectory + "/myfile/sysimg/ico-image-remove.png")
                ,
                Stretch = Stretch.UniformToFill
            };
            removeBtn.Click += removeBtn_Click;
        }

        /*
         * 打开上传图片对话框
         */
        private void storageIdOfCover_Click(object sender, RoutedEventArgs e)
        {
            EditBackgroundImageSelectorWindow win = new EditBackgroundImageSelectorWindow(storageIdOfCover);
            Boolean result = (Boolean)win.ShowDialog();
        }

        private void removeBtn_Click(object sender, RoutedEventArgs e)
        {
            storageIdOfCover.Tag = null;
            storageIdOfCover.Background = new ImageBrush
            {
                ImageSource = FileUtil.readImage(AppDomain.CurrentDomain.BaseDirectory + Params.Ico_Add_Image)
              ,
                Stretch = Stretch.Uniform
            };
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
            StorageImage storageImage = null;
            object tag = storageIdOfCover.Tag;
            if (tag != null)
            {
                storageImage = (StorageImage)tag;
            }

            //更新到数据库
            DControl dControl = dControlBll.get(currDControl.id);
            //  dControl.url = url.Text;
            dControl.width = int.Parse(width.Text);
            dControl.height = int.Parse(height.Text);
            dControl.left = int.Parse(left.Text);
            dControl.top = int.Parse(top.Text);
            dControl.opacity = int.Parse(opacity.Text);
            dControl.autoplay = (Boolean)autoplay.IsChecked;
            dControl.loop = (Boolean)loop.IsChecked;
            dControl.storageIdOfCover = (storageImage==null ? 0 : storageImage.id); 
            dControlBll.update(dControl);
            currDControl = dControl;
            currElement.Tag = currDControl;


            //更新页面控件信息  
            string audioCoverUrl = Params.CAudioImageNotExists;
            if (currDControl.storageIdOfCover > 0)
            {
                StorageImage storageImage1 = storageImageBll.get(currDControl.storageIdOfCover);
                audioCoverUrl = storageImage1?.url;
            }
            CAudio cAudio = (CAudio)currElement; 
            cAudio.updateElementAttr(dControl, audioCoverUrl, true);  
                
            //2.修改8个点的位置 及宽高 
            editing.updateEditingBorder(currDControl); 

            Close();

        }
    }
}
