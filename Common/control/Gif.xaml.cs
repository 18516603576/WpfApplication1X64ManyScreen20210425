using Common.Data;
using Common.util;
using Model;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WpfAnimatedGif;

namespace Common.control
{
    /// <summary>
    /// GifImage.xaml 的交互逻辑
    /// </summary>
    public partial class Gif : UserControl
    {

        private BitmapImage bitmapImage = null;

        public Gif(string imgPath, Boolean isDesign)
        {
            InitializeComponent();

            imgPath = FileUtil.notExistsShowDefault(imgPath, Params.GifNotExists);
            bitmapImage = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + imgPath));

            ImageBehavior.SetAnimatedSource(image, bitmapImage);
            Unloaded += This_Unloaded;
        }



        /*
         * 清空
         */
        private void This_Unloaded(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("GIF:This_Unloaded");
            Unloaded -= This_Unloaded;
            bitmapImage = null;
            ImageBehavior.SetAutoStart(image, false);
            ImageBehavior.SetAnimatedSource(image, null);
            
            image.Source = null;
            mainGrid.Children.Clear();
            this.Resources.Clear(); 
            GC.Collect(); 
        }

       

        /*
          * 编辑控件，更新页面显示
          */
        public void updateElement(string imgFullPath, Boolean isDesign)
        {
            bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.UriSource = new Uri(imgFullPath);
            bitmapImage.EndInit();
            ImageBehavior.SetAnimatedSource(image, bitmapImage);
        }


        public void updateElementAttr(DControl dControl, bool isDesign)
        {
            Width = dControl.width;
            Height = dControl.height;
            Margin = new Thickness(dControl.left, dControl.top, 0, 0);
            Opacity = dControl.opacity / 100.0;
            //this.currDControl = dControl;
            TransformGroup group = (TransformGroup)RenderTransform;
            RotateTransform rotateTransform = TransformGroupUtil.GetRotateTransform(group);
            rotateTransform.Angle = dControl.rotateAngle;
        }
    }
}
