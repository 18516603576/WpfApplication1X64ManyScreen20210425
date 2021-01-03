using System.Windows;
using WpfApplication1.ManageWin;

namespace WpfApplication1.manage
{
    /*
     * 控件右击-触发事件
     */
    public partial class Editing
    {

        /*
        *  f1 编辑图片
        */
        internal void editImageClick(object sender, RoutedEventArgs e)
        {
            EditImageWindow win = new EditImageWindow(this, rightClickEle);
            win.ShowDialog();
        }
        /*
         * f1 编辑图片属性
         */
        internal void editImageAttrClick(object sender, RoutedEventArgs e)
        {
            EditImageAttrWindow win = new EditImageAttrWindow(this, rightClickEle);
            win.ShowDialog();
        }
        /*
         * f2 编辑文字
         */
        internal void editTextBlockClick(object sender, RoutedEventArgs e)
        {
            EditTextBlockWindow editWin = new EditTextBlockWindow(mainFrame, rightClickEle);
            editWin.ShowDialog();
        }
        /*
        * f2 编辑文本属性
        */
        internal void editTextBlockAttrClick(object sender, RoutedEventArgs e)
        {
            EditTextBlockAttrWindow win = new EditTextBlockAttrWindow(this, rightClickEle);
            win.ShowDialog();
        }
        /*
         * f3 编辑相册
         */
        internal void editTurnPictureClick(object sender, RoutedEventArgs e)
        {

            EditTurnPictureWindow editTurnPicture = new EditTurnPictureWindow(this, rightClickEle);
            editTurnPicture.ShowDialog();

        }
        /*
        * f3 编辑轮播图片属性
        */
        internal void editTurnPictureAttrClick(object sender, RoutedEventArgs e)
        {
            EditTurnPictureAttrWindow win = new EditTurnPictureAttrWindow(this, rightClickEle);
            win.ShowDialog();
        }

        /*
       * f4 编辑流动相册
       */
        internal void editMarqueClick(object sender, RoutedEventArgs e)
        {

            EditMarqueWindow editMarque = new EditMarqueWindow(this, rightClickEle);
            editMarque.ShowDialog();

        }
        /*
        * f4 编辑流动相册属性
        */
        internal void editMarqueAttrClick(object sender, RoutedEventArgs e)
        {
            EditMarqueAttrWindow win = new EditMarqueAttrWindow(this, rightClickEle);
            win.ShowDialog();
        }

        /*
        * f5 编辑层叠相册
        */
        internal void editMarqueLayerClick(object sender, RoutedEventArgs e)
        {

            EditMarqueLayerWindow win = new EditMarqueLayerWindow(this, rightClickEle);
            win.ShowDialog();

        }
        /*
        * f5 编辑层叠相册属性
        */
        internal void editMarqueLayerAttrClick(object sender, RoutedEventArgs e)
        {
            EditMarqueLayerAttrWindow win = new EditMarqueLayerAttrWindow(this, rightClickEle);
            win.ShowDialog();
        }

        /*
        * f6 编辑视频
        */
        internal void editVideoClick(object sender, RoutedEventArgs e)
        {
            EditVideoWindow editVideo = new EditVideoWindow(this, rightClickEle);
            editVideo.ShowDialog();
        }
        /*
         * f6 编辑视频属性
         */
        internal void editVideoAttrClick(object sender, RoutedEventArgs e)
        {
            EditVideoAttrWindow win = new EditVideoAttrWindow(this, rightClickEle);
            win.ShowDialog();
        }
        /*
        * f7 编辑返回按钮
        */

        internal void editBackButtonClick(object sender, RoutedEventArgs e)
        {
            EditImageWindow win = new EditImageWindow(this, rightClickEle);
            win.ShowDialog();
        }
        /*
        * f7 编辑返回按钮属性
        */
        internal void editBackButtonAttrClick(object sender, RoutedEventArgs e)
        {
            EditBackButtonAttrWindow win = new EditBackButtonAttrWindow(this, rightClickEle);
            win.ShowDialog();
        }
        /*
         * f8 编辑首页按钮
         */

        internal void editHomeButtonClick(object sender, RoutedEventArgs e)
        {
            EditImageWindow win = new EditImageWindow(this, rightClickEle);
            win.ShowDialog();
        }

        /*
         * f8 编辑首页按钮属性
         */
        internal void editHomeButtonAttrClick(object sender, RoutedEventArgs e)
        {
            EditHomeButtonAttrWindow win = new EditHomeButtonAttrWindow(this, rightClickEle);
            win.ShowDialog();
        }
        /*
       * f9 编辑小窗口
       */
        internal void editCFrameClick(object sender, RoutedEventArgs e)
        {
            EditCFrameWindow editWin = new EditCFrameWindow(mainFrame, pageTemplate.dPage, rightClickEle);
            editWin.ShowDialog();
        }
        /*
         * f9 编辑小窗口属性
         */
        internal void editCFrameAttrClick(object sender, RoutedEventArgs e)
        {
            EditCFrameAttrWindow win = new EditCFrameAttrWindow(this, rightClickEle);
            win.ShowDialog();
        }
        /*
         * 10 编辑Gif
         */
        internal void editGifClick(object sender, RoutedEventArgs e)
        {
            EditGifWindow editWin = new EditGifWindow(this, rightClickEle);
            editWin.ShowDialog();
        }
        /*
         * f10 编辑Gif属性
         */
        internal void editGifAttrClick(object sender, RoutedEventArgs e)
        {
            EditGifAttrWindow win = new EditGifAttrWindow(this, rightClickEle);
            win.ShowDialog();
        }
        /*
        * f11 编辑日期
        */
        internal void editCCalendarClick(object sender, RoutedEventArgs e)
        {
            EditCCalendarWindow editWin = new EditCCalendarWindow(mainFrame, rightClickEle);
            editWin.ShowDialog();
        }
        /*
         * f11 编辑日期属性
         */
        internal void editCCalendarAttrClick(object sender, RoutedEventArgs e)
        {
            EditCCalendarAttrWindow win = new EditCCalendarAttrWindow(this, rightClickEle);
            win.ShowDialog();
        }



        /*
        * f12 编辑Word
        */
        internal void editWordClick(object sender, RoutedEventArgs e)
        {
            EditWordWindow editWord = new EditWordWindow(this, rightClickEle);
            editWord.ShowDialog();
        }



        /*
         * f12 编辑Word属性
         */
        internal void editWordAttrClick(object sender, RoutedEventArgs e)
        {
            EditWordAttrWindow win = new EditWordAttrWindow(this, rightClickEle);
            win.ShowDialog();
        }


        /*
      * f13 编辑音频
      */
        internal void editCAudioClick(object sender, RoutedEventArgs e)
        {
            EditCAudioWindow editWin = new EditCAudioWindow(mainFrame, rightClickEle);
            editWin.ShowDialog();
        }
        /*
         * f13 编辑音频属性  
         */
        internal void editCAudioAttrClick(object sender, RoutedEventArgs e)
        {
            EditCAudioAttrWindow win = new EditCAudioAttrWindow(this, rightClickEle);
            win.ShowDialog();
        }
    }
}
