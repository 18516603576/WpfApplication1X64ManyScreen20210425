using Bll;
using Common;
using Common.Data;
using Common.util;
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
    /// EditBackgroundImageWindow.xaml 的交互逻辑
    /// </summary>
    public partial class EditBackgroundImageWindow : Window
    {
        private readonly StorageImageBll storageImageBll = new StorageImageBll();
        private readonly DControlBll dControlBll = new DControlBll();
        private readonly DPageBll dPageBll = new DPageBll();
        
        private DPage currDPage;
        private readonly Editing editing;
        // private Grid mainContainer;
        private readonly PageTemplate pageTemplate = null;
        public EditBackgroundImageWindow(PageTemplate pageTemplate, DPage dPage, Editing editing)
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.editing = editing;
            currDPage = dPage;
            // this.mainContainer = mainContainer;
            this.pageTemplate = pageTemplate;

            Int32 backgroundImageIdVal = currDPage.backgroundImageId; 
            loadPageData(backgroundImageIdVal);
        }

        private void loadPageData(Int32 backgroundImageIdVal)
        {
            //显示背景图片
            StorageImage storageImage = null;
            if (backgroundImageIdVal > 0)
            {
                storageImage = storageImageBll.get(backgroundImageIdVal);
            }
            backgroundImageId.Tag = storageImage; 
            string imgPath = FileUtil.notExistsShowDefault(storageImage?.url, Params.Ico_Add_Image); 
            backgroundImageId.Background = new ImageBrush
            {
                ImageSource = FileUtil.readImage(AppDomain.CurrentDomain.BaseDirectory + imgPath)
               ,
                Stretch = Stretch.Uniform
            };
            backgroundImageId.Click +=  btn_Click ;

            //删除
            removeBtn.Background = new ImageBrush
            {
                ImageSource = FileUtil.readImage(AppDomain.CurrentDomain.BaseDirectory + "/myfile/sysimg/ico-image-remove.png")
                ,
                Stretch = Stretch.UniformToFill
            };
            removeBtn.Click += removeBtnClick;

            url.PreviewMouseLeftButtonUp += url_PreviewMouseLeftButtonUp;
        }
        private void url_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        { 
            StorageImage storageImage = null;
            object tag = backgroundImageId.Tag;
            if (tag == null) return;
            storageImage = (StorageImage)tag;
            string fullFolder = FileUtil.getDirectory(AppDomain.CurrentDomain.BaseDirectory + storageImage.url);  
              
            FileUtil.openFile(fullFolder); 
        }



        /*
         * 清空控件中的图片
         */
        private void removeBtnClick(object sender, RoutedEventArgs e)
        {
              
            backgroundImageId.Tag = null;
            backgroundImageId.Background = new ImageBrush
            {
                ImageSource = FileUtil.readImage(AppDomain.CurrentDomain.BaseDirectory + Params.Ico_Add_Image)
              ,
                Stretch = Stretch.Uniform
            };

        }
         



        private void btn_Click(object sender, RoutedEventArgs e)
        {
            EditBackgroundImageSelectorWindow win = new EditBackgroundImageSelectorWindow(backgroundImageId);
            Boolean result = (Boolean)win.ShowDialog(); 
        }



        /*
         * 保存数据 
         */
        private void Submit_Button_Click(object sender, RoutedEventArgs e)
        {
            StorageImage storageImage = null;
            object tag = backgroundImageId.Tag;
            if (tag != null)
             {
                    storageImage = (StorageImage)tag;
             }
            
            //更新到数据库

            DPage dPage = dPageBll.get(currDPage.id);
            dPage.backgroundImageId = storageImage == null ? 0 : storageImage.id;
            dPageBll.update(dPage);
            currDPage = dPage;
            pageTemplate.dPage = dPage;


            //更新页面控件信息   
            if (FileUtil.imageIsExists(storageImage?.url))
            {
                pageTemplate.Background = new ImageBrush
                {
                    ImageSource = FileUtil.readImage(AppDomain.CurrentDomain.BaseDirectory + storageImage.url),
                    Stretch = Stretch.Fill
                };
            }
            else
            {
                pageTemplate.Background = Brushes.White;
            }

            Close();

        }

    }
}
