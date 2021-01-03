using Common.util;
using Model;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WpfApplication1.manage
{
    public partial class Editing
    {
        //private Frame mainFrame = null;
        //private PageTemplate pageTemplate = null;

        public void RightContextMenu()
        {
            initContextMenu();
            initRightClickContextMenu();
            //页面空白处右击
            pageTemplate.MouseRightButtonUp += Page_MouseRightButtonUp; 
        }
        /*
         * 1.2、初始化-右击控件菜单
         */
        private void initContextMenu()
        {
            initImageContextMenu();
            initTurnPictureContextMenu();
            initMarqueContextMenu();
            initMarqueLayerContextMenu();
            initWordContextMenu();
            initVideoContextMenu();
            initBackButtonContextMenu();
            initHomeButtonContextMenu();
            initCFrameContextMenu();
            initTextBlockContextMenu();
            initGifContextMenu();
            initCCalendarContextMenu();
            initCAudioContextMenu();
        }

        private void initImageContextMenu()
        {
            ContextMenu contextMenu = new ContextMenu();
            // contextMenu.Name = "ImageContextMenu";

            //1.编辑图片
            MenuItem item1 = new MenuItem();
            item1.Header = "编辑图片";
            item1.Click += editImageClick;

            //2.编辑属性
            MenuItem item2 = new MenuItem();
            item2.Header = "编辑属性";
            item2.Click += editImageAttrClick;

            //2.编辑属性
            MenuItem item8 = new MenuItem();
            item8.Header = "编辑动画";
            item8.Click += editImageAnimationClick;

            //2.链接到
            MenuItem item3 = new MenuItem();
            item3.Header = "链接到";
            item3.Click += linkToClick;
 

            //7.链接到
            MenuItem item7 = new MenuItem();
            item7.Header = "链接到外部网站";
            item7.Click += linkToWebClick;

            //2.
            MenuItem item6 = new MenuItem();
            item6.Header = "链接到全屏视频";
            item6.Click += editOpenVideoClick;


            //4.删除图片
            MenuItem item4 = new MenuItem();
            item4.Header = "删除";
            item4.Click += DeleteImageClick;

            //5.复制控件
            MenuItem item5 = new MenuItem();
            item5.Header = "复制";
            item5.Click += copyControlClick;

            //5.上移一层
            MenuItem item9 = new MenuItem();
            item9.Header = "上移一层";
            item9.Click += moveupIdxClick;

            // 5.下移一层
            MenuItem item10 = new MenuItem();
            item10.Header = "下移一层";
            item10.Click += movedownIdxClick;

            // 5.置于顶层
            MenuItem item11 = new MenuItem();
            item11.Header = "置于顶层";
            item11.Click += moveupTopIdxClick;

            // 5.置于底层
            MenuItem item12 = new MenuItem();
            item12.Header = "置于底层";
            item12.Click += movedownBottomIdxClick;

            MenuItem item13 = new MenuItem();
            item13.Header = "层顺序";
            item13.Items.Add(item11);
            item13.Items.Add(item9);
            item13.Items.Add(item10);
            item13.Items.Add(item12);


            contextMenu.Items.Add(item1);
            contextMenu.Items.Add(item2);
            contextMenu.Items.Add(item8); 
            contextMenu.Items.Add(item3);
            contextMenu.Items.Add(item7);
            contextMenu.Items.Add(item6);

            contextMenu.Items.Add(item5);
            contextMenu.Items.Add(item4);
            contextMenu.Items.Add(item13);


            Grid grid = new Grid();
            grid.Name = "ImageContextMenu";
            grid.ContextMenu = contextMenu;
            pageTemplate.container.Children.Add(grid);
        }
        private void initTurnPictureContextMenu()
        {
            ContextMenu contextMenu = new ContextMenu();

            //1.编辑图片
            MenuItem item1 = new MenuItem();
            item1.Header = "编辑相册";
            item1.Click += editTurnPictureClick;

            //2.编辑属性
            MenuItem item2 = new MenuItem();
            item2.Header = "编辑属性";
            item2.Click += editTurnPictureAttrClick;

            //2.编辑属性
            MenuItem item8 = new MenuItem();
            item8.Header = "编辑动画";
            item8.Click += editImageAnimationClick;

            //4.删除图片
            MenuItem item4 = new MenuItem();
            item4.Header = "删除";
            item4.Click += DeleteImageClick;

            //5.复制控件
            MenuItem item5 = new MenuItem();
            item5.Header = "复制";
            item5.Click += copyControlClick;

            //5.上移一层
            MenuItem item9 = new MenuItem();
            item9.Header = "上移一层";
            item9.Click += moveupIdxClick;

            // 5.下移一层
            MenuItem item10 = new MenuItem();
            item10.Header = "下移一层";
            item10.Click += movedownIdxClick;

            // 5.置于顶层
            MenuItem item11 = new MenuItem();
            item11.Header = "置于顶层";
            item11.Click += moveupTopIdxClick;

            // 5.置于底层
            MenuItem item12 = new MenuItem();
            item12.Header = "置于底层";
            item12.Click += movedownBottomIdxClick;

            MenuItem item13 = new MenuItem();
            item13.Header = "层顺序";
            item13.Items.Add(item11);
            item13.Items.Add(item9);
            item13.Items.Add(item10);
            item13.Items.Add(item12);


            contextMenu.Items.Add(item1);
            contextMenu.Items.Add(item2);
            contextMenu.Items.Add(item8);
            contextMenu.Items.Add(item5);
            contextMenu.Items.Add(item4);
            contextMenu.Items.Add(item13);


            Grid grid = new Grid();
            grid.Name = "TurnPictureContextMenu";
            grid.ContextMenu = contextMenu;
            pageTemplate.container.Children.Add(grid);

        }
        private void initMarqueContextMenu()
        {
            ContextMenu contextMenu = new ContextMenu();
            // contextMenu.Name = "ImageContextMenu";

            //1.编辑图片
            MenuItem item1 = new MenuItem();
            item1.Header = "编辑流动相册";
            item1.Click += editMarqueClick;

            //2.编辑属性
            MenuItem item2 = new MenuItem();
            item2.Header = "编辑属性";
            item2.Click += editMarqueAttrClick;

            //2.编辑属性
            MenuItem item8 = new MenuItem();
            item8.Header = "编辑动画";
            item8.Click += editImageAnimationClick;

            //4.删除图片
            MenuItem item4 = new MenuItem();
            item4.Header = "删除";
            item4.Click += DeleteImageClick;

            //5.复制控件
            MenuItem item5 = new MenuItem();
            item5.Header = "复制";
            item5.Click += copyControlClick;

            //5.上移一层
            MenuItem item9 = new MenuItem();
            item9.Header = "上移一层";
            item9.Click += moveupIdxClick;

            // 5.下移一层
            MenuItem item10 = new MenuItem();
            item10.Header = "下移一层";
            item10.Click += movedownIdxClick;

            // 5.置于顶层
            MenuItem item11 = new MenuItem();
            item11.Header = "置于顶层";
            item11.Click += moveupTopIdxClick;

            // 5.置于底层
            MenuItem item12 = new MenuItem();
            item12.Header = "置于底层";
            item12.Click += movedownBottomIdxClick;

            MenuItem item13 = new MenuItem();
            item13.Header = "层顺序";
            item13.Items.Add(item11);
            item13.Items.Add(item9);
            item13.Items.Add(item10);
            item13.Items.Add(item12);


            contextMenu.Items.Add(item1);
            contextMenu.Items.Add(item2);
            contextMenu.Items.Add(item8);
            contextMenu.Items.Add(item5);
            contextMenu.Items.Add(item4);
            contextMenu.Items.Add(item13);


            Grid grid = new Grid();
            grid.Name = "MarqueContextMenu";
            grid.ContextMenu = contextMenu;
            pageTemplate.container.Children.Add(grid);

        }

        private void initMarqueLayerContextMenu()
        {
            ContextMenu contextMenu = new ContextMenu();
            // contextMenu.Name = "ImageContextMenu";

            //1.编辑图片
            MenuItem item1 = new MenuItem();
            item1.Header = "编辑层叠相册";
            item1.Click += editMarqueLayerClick;

            //2.编辑属性
            MenuItem item2 = new MenuItem();
            item2.Header = "编辑属性";
            item2.Click += editMarqueLayerAttrClick;

            //2.编辑属性
            MenuItem item8 = new MenuItem();
            item8.Header = "编辑动画";
            item8.Click += editImageAnimationClick;

            //4.删除图片
            MenuItem item4 = new MenuItem();
            item4.Header = "删除";
            item4.Click += DeleteImageClick;

            //5.复制控件
            MenuItem item5 = new MenuItem();
            item5.Header = "复制";
            item5.Click += copyControlClick;

            //5.上移一层
            MenuItem item9 = new MenuItem();
            item9.Header = "上移一层";
            item9.Click += moveupIdxClick;

            // 5.下移一层
            MenuItem item10 = new MenuItem();
            item10.Header = "下移一层";
            item10.Click += movedownIdxClick;

            // 5.置于顶层
            MenuItem item11 = new MenuItem();
            item11.Header = "置于顶层";
            item11.Click += moveupTopIdxClick;

            // 5.置于底层
            MenuItem item12 = new MenuItem();
            item12.Header = "置于底层";
            item12.Click += movedownBottomIdxClick;

            MenuItem item13 = new MenuItem();
            item13.Header = "层顺序";
            item13.Items.Add(item11);
            item13.Items.Add(item9);
            item13.Items.Add(item10);
            item13.Items.Add(item12);


            contextMenu.Items.Add(item1);
            contextMenu.Items.Add(item2);
            contextMenu.Items.Add(item8);
            contextMenu.Items.Add(item5);
            contextMenu.Items.Add(item4);
            contextMenu.Items.Add(item13);


            Grid grid = new Grid();
            grid.Name = "MarqueLayerContextMenu";
            grid.ContextMenu = contextMenu;
            pageTemplate.container.Children.Add(grid);

        }
        private void initWordContextMenu()
        {
            ContextMenu contextMenu = new ContextMenu();
            // contextMenu.Name = "ImageContextMenu";

            //1.编辑图片
            MenuItem item1 = new MenuItem();
            item1.Header = "编辑文章";
            item1.Click += editWordClick;

            //2.编辑属性
            MenuItem item2 = new MenuItem();
            item2.Header = "编辑属性";
            item2.Click += editWordAttrClick;

            //2.编辑属性
            MenuItem item8 = new MenuItem();
            item8.Header = "编辑动画";
            item8.Click += editImageAnimationClick;

            //2.链接到
            //    MenuItem item3 = new MenuItem();
            //    item3.Header = "链接到";


            //4.删除图片
            MenuItem item4 = new MenuItem();
            item4.Header = "删除";
            item4.Click += DeleteImageClick;

            //5.复制控件
            MenuItem item5 = new MenuItem();
            item5.Header = "复制";
            item5.Click += copyControlClick;

            //5.上移一层
            MenuItem item9 = new MenuItem();
            item9.Header = "上移一层";
            item9.Click += moveupIdxClick;

            // 5.下移一层
            MenuItem item10 = new MenuItem();
            item10.Header = "下移一层";
            item10.Click += movedownIdxClick;

            // 5.置于顶层
            MenuItem item11 = new MenuItem();
            item11.Header = "置于顶层";
            item11.Click += moveupTopIdxClick;

            // 5.置于底层
            MenuItem item12 = new MenuItem();
            item12.Header = "置于底层";
            item12.Click += movedownBottomIdxClick;

            MenuItem item13 = new MenuItem();
            item13.Header = "层顺序";
            item13.Items.Add(item11);
            item13.Items.Add(item9);
            item13.Items.Add(item10);
            item13.Items.Add(item12);


            contextMenu.Items.Add(item1);
            contextMenu.Items.Add(item2);
            contextMenu.Items.Add(item8);
            contextMenu.Items.Add(item5);
            contextMenu.Items.Add(item4);
            contextMenu.Items.Add(item13);


            Grid grid = new Grid();
            grid.Name = "WordContextMenu";
            grid.ContextMenu = contextMenu;
            pageTemplate.container.Children.Add(grid);

        }
        /*
         * 加载视频右击菜单
         */
        private void initVideoContextMenu()
        {
            ContextMenu contextMenu = new ContextMenu();
            // contextMenu.Name = "ImageContextMenu";

            //1.编辑图片
            MenuItem item1 = new MenuItem();
            item1.Header = "编辑视频";
            item1.Click += editVideoClick;

            //2.编辑属性
            MenuItem item2 = new MenuItem();
            item2.Header = "编辑属性";
            item2.Click += editVideoAttrClick;

            //2.编辑属性
            MenuItem item8 = new MenuItem();
            item8.Header = "编辑动画";
            item8.Click += editImageAnimationClick;


            //4.删除图片
            MenuItem item4 = new MenuItem();
            item4.Header = "删除";
            item4.Click += DeleteImageClick;

            //5.复制控件
            MenuItem item5 = new MenuItem();
            item5.Header = "复制";
            item5.Click += copyControlClick;

            //5.上移一层
            MenuItem item9 = new MenuItem();
            item9.Header = "上移一层";
            item9.Click += moveupIdxClick;

            // 5.下移一层
            MenuItem item10 = new MenuItem();
            item10.Header = "下移一层";
            item10.Click += movedownIdxClick;

            // 5.置于顶层
            MenuItem item11 = new MenuItem();
            item11.Header = "置于顶层";
            item11.Click += moveupTopIdxClick;

            // 5.置于底层
            MenuItem item12 = new MenuItem();
            item12.Header = "置于底层";
            item12.Click += movedownBottomIdxClick;

            MenuItem item13 = new MenuItem();
            item13.Header = "层顺序";
            item13.Items.Add(item11);
            item13.Items.Add(item9);
            item13.Items.Add(item10);
            item13.Items.Add(item12);


            contextMenu.Items.Add(item1);
            contextMenu.Items.Add(item2);
            contextMenu.Items.Add(item8);
            contextMenu.Items.Add(item5);
            contextMenu.Items.Add(item4);
            contextMenu.Items.Add(item13);


            Grid grid = new Grid();
            grid.Name = "VideoContextMenu";
            grid.ContextMenu = contextMenu;
            pageTemplate.container.Children.Add(grid);

        }
        /*
         * 返回按钮菜单
         */
        private void initBackButtonContextMenu()
        {
            ContextMenu contextMenu = new ContextMenu();
            // contextMenu.Name = "ImageContextMenu";

            //1.编辑图片
            MenuItem item1 = new MenuItem();
            item1.Header = "编辑返回按钮";
            item1.Click += editBackButtonClick;

            //2.编辑属性
            MenuItem item2 = new MenuItem();
            item2.Header = "编辑属性";
            item2.Click += editBackButtonAttrClick;

            //2.编辑属性
            MenuItem item8 = new MenuItem();
            item8.Header = "编辑动画";
            item8.Click += editImageAnimationClick;

            //4.删除图片
            MenuItem item4 = new MenuItem();
            item4.Header = "删除";
            item4.Click += DeleteImageClick;

            //5.复制控件
            MenuItem item5 = new MenuItem();
            item5.Header = "复制";
            item5.Click += copyControlClick;


            //5.上移一层
            MenuItem item9 = new MenuItem();
            item9.Header = "上移一层";
            item9.Click += moveupIdxClick;

            // 5.下移一层
            MenuItem item10 = new MenuItem();
            item10.Header = "下移一层";
            item10.Click += movedownIdxClick;

            // 5.置于顶层
            MenuItem item11 = new MenuItem();
            item11.Header = "置于顶层";
            item11.Click += moveupTopIdxClick;

            // 5.置于底层
            MenuItem item12 = new MenuItem();
            item12.Header = "置于底层";
            item12.Click += movedownBottomIdxClick;

            MenuItem item13 = new MenuItem();
            item13.Header = "层顺序";
            item13.Items.Add(item11);
            item13.Items.Add(item9);
            item13.Items.Add(item10);
            item13.Items.Add(item12);


            contextMenu.Items.Add(item1);
            contextMenu.Items.Add(item2);
            contextMenu.Items.Add(item8);
            contextMenu.Items.Add(item5);
            contextMenu.Items.Add(item4);
            contextMenu.Items.Add(item13);


            Grid grid = new Grid();
            grid.Name = "BackButtonContextMenu";
            grid.ContextMenu = contextMenu;
            pageTemplate.container.Children.Add(grid);

        }
        /*
       * 首页按钮菜单
       */
        private void initHomeButtonContextMenu()
        {
            ContextMenu contextMenu = new ContextMenu();
            // contextMenu.Name = "ImageContextMenu";

            //1.编辑图片
            MenuItem item1 = new MenuItem();
            item1.Header = "编辑首页按钮";
            item1.Click += editHomeButtonClick;

            //2.编辑属性
            MenuItem item2 = new MenuItem();
            item2.Header = "编辑属性";
            item2.Click += editHomeButtonAttrClick;

            //2.编辑属性
            MenuItem item8 = new MenuItem();
            item8.Header = "编辑动画";
            item8.Click += editImageAnimationClick;

            //4.删除图片
            MenuItem item4 = new MenuItem();
            item4.Header = "删除";
            item4.Click += DeleteImageClick;

            //5.复制控件
            MenuItem item5 = new MenuItem();
            item5.Header = "复制";
            item5.Click += copyControlClick;

            //5.上移一层
            MenuItem item9 = new MenuItem();
            item9.Header = "上移一层";
            item9.Click += moveupIdxClick;

            // 5.下移一层
            MenuItem item10 = new MenuItem();
            item10.Header = "下移一层";
            item10.Click += movedownIdxClick;

            // 5.置于顶层
            MenuItem item11 = new MenuItem();
            item11.Header = "置于顶层";
            item11.Click += moveupTopIdxClick;

            // 5.置于底层
            MenuItem item12 = new MenuItem();
            item12.Header = "置于底层";
            item12.Click += movedownBottomIdxClick;

            MenuItem item13 = new MenuItem();
            item13.Header = "层顺序";
            item13.Items.Add(item11);
            item13.Items.Add(item9);
            item13.Items.Add(item10);
            item13.Items.Add(item12);


            contextMenu.Items.Add(item1);
            contextMenu.Items.Add(item2);
            contextMenu.Items.Add(item8);
            contextMenu.Items.Add(item5);
            contextMenu.Items.Add(item4);
            contextMenu.Items.Add(item13);


            Grid grid = new Grid();
            grid.Name = "HomeButtonContextMenu";
            grid.ContextMenu = contextMenu;
            pageTemplate.container.Children.Add(grid);

        }
        private void initCFrameContextMenu()
        {
            ContextMenu contextMenu = new ContextMenu();

            //1.编辑图片
            MenuItem item1 = new MenuItem();
            item1.Header = "编辑小窗口";
            item1.Click += editCFrameClick;

            //2.编辑属性
            MenuItem item2 = new MenuItem();
            item2.Header = "编辑属性";
            item2.Click += editCFrameAttrClick;

            //2.编辑属性
            MenuItem item8 = new MenuItem();
            item8.Header = "编辑动画";
            item8.Click += editImageAnimationClick;

            //4.删除图片
            MenuItem item4 = new MenuItem();
            item4.Header = "删除";
            item4.Click += DeleteImageClick;

            //5.复制控件
            MenuItem item5 = new MenuItem();
            item5.Header = "复制";
            item5.Click += copyControlClick;

            //5.上移一层
            MenuItem item9 = new MenuItem();
            item9.Header = "上移一层";
            item9.Click += moveupIdxClick;

            // 5.下移一层
            MenuItem item10 = new MenuItem();
            item10.Header = "下移一层";
            item10.Click += movedownIdxClick;

            // 5.置于顶层
            MenuItem item11 = new MenuItem();
            item11.Header = "置于顶层";
            item11.Click += moveupTopIdxClick;

            // 5.置于底层
            MenuItem item12 = new MenuItem();
            item12.Header = "置于底层";
            item12.Click += movedownBottomIdxClick;

            MenuItem item13 = new MenuItem();
            item13.Header = "层顺序";
            item13.Items.Add(item11);
            item13.Items.Add(item9);
            item13.Items.Add(item10);
            item13.Items.Add(item12);


            contextMenu.Items.Add(item1);
            contextMenu.Items.Add(item2);
            contextMenu.Items.Add(item8);
            contextMenu.Items.Add(item5);
            contextMenu.Items.Add(item4);
            contextMenu.Items.Add(item13);


            Grid grid = new Grid();
            grid.Name = "CFrameContextMenu";
            grid.ContextMenu = contextMenu;
            pageTemplate.container.Children.Add(grid);

        }
        private void initTextBlockContextMenu()
        {
            ContextMenu contextMenu = new ContextMenu();

            //1.编辑图片
            MenuItem item1 = new MenuItem();
            item1.Header = "编辑文本";
            item1.Click += editTextBlockClick;

            //2.编辑属性
            MenuItem item2 = new MenuItem();
            item2.Header = "编辑属性";
            item2.Click += editTextBlockAttrClick;

            //2.编辑属性
            MenuItem item8 = new MenuItem();
            item8.Header = "编辑动画";
            item8.Click += editImageAnimationClick;
 

            //2.链接到
            MenuItem item3 = new MenuItem();
            item3.Header = "链接到";
            item3.Click += linkToClick;

            //7.链接到
            MenuItem item7 = new MenuItem();
            item7.Header = "链接到外部网站";
            item7.Click += linkToWebClick;

            //4.删除图片
            MenuItem item4 = new MenuItem();
            item4.Header = "删除";
            item4.Click += DeleteImageClick;

            //5.复制控件
            MenuItem item5 = new MenuItem();
            item5.Header = "复制";
            item5.Click += copyControlClick;

            //5.上移一层
            MenuItem item9 = new MenuItem();
            item9.Header = "上移一层";
            item9.Click += moveupIdxClick;

            // 5.下移一层
            MenuItem item10 = new MenuItem();
            item10.Header = "下移一层";
            item10.Click += movedownIdxClick;

            // 5.置于顶层
            MenuItem item11 = new MenuItem();
            item11.Header = "置于顶层";
            item11.Click += moveupTopIdxClick;

            // 5.置于底层
            MenuItem item12 = new MenuItem();
            item12.Header = "置于底层";
            item12.Click += movedownBottomIdxClick;

            MenuItem item13 = new MenuItem();
            item13.Header = "层顺序";
            item13.Items.Add(item11);
            item13.Items.Add(item9);
            item13.Items.Add(item10);
            item13.Items.Add(item12);


            contextMenu.Items.Add(item1);
            contextMenu.Items.Add(item2);
            contextMenu.Items.Add(item8); 
            contextMenu.Items.Add(item3);
            contextMenu.Items.Add(item7);
            contextMenu.Items.Add(item5);
            contextMenu.Items.Add(item4);
            contextMenu.Items.Add(item13);


            Grid grid = new Grid();
            grid.Name = "TextBlockContextMenu";
            grid.ContextMenu = contextMenu;
            pageTemplate.container.Children.Add(grid);

        }
        private void initGifContextMenu()
        {
            ContextMenu contextMenu = new ContextMenu();

            //1.编辑图片
            MenuItem item1 = new MenuItem();
            item1.Header = "编辑Gif";
            item1.Click += editGifClick;

            //2.编辑属性
            MenuItem item2 = new MenuItem();
            item2.Header = "编辑属性";
            item2.Click += editGifAttrClick;

            //2.编辑属性
            MenuItem item8 = new MenuItem();
            item8.Header = "编辑动画";
            item8.Click += editImageAnimationClick;

             

            //2.链接到
            MenuItem item3 = new MenuItem();
            item3.Header = "链接到";
            item3.Click += linkToClick;
             
            //7.链接到
            MenuItem item7 = new MenuItem();
            item7.Header = "链接到外部网站";
            item7.Click += linkToWebClick;

            //2.
            MenuItem item6 = new MenuItem();
            item6.Header = "链接到全屏视频";
            item6.Click += editOpenVideoClick;



            //4.删除图片
            MenuItem item4 = new MenuItem();
            item4.Header = "删除";
            item4.Click += DeleteImageClick;

            //5.复制控件
            MenuItem item5 = new MenuItem();
            item5.Header = "复制";
            item5.Click += copyControlClick;

            //5.上移一层
            MenuItem item9 = new MenuItem();
            item9.Header = "上移一层";
            item9.Click += moveupIdxClick;

            // 5.下移一层
            MenuItem item10 = new MenuItem();
            item10.Header = "下移一层";
            item10.Click += movedownIdxClick;

            // 5.置于顶层
            MenuItem item11 = new MenuItem();
            item11.Header = "置于顶层";
            item11.Click += moveupTopIdxClick;

            // 5.置于底层
            MenuItem item12 = new MenuItem();
            item12.Header = "置于底层";
            item12.Click += movedownBottomIdxClick;

            MenuItem item13 = new MenuItem();
            item13.Header = "层顺序";
            item13.Items.Add(item11);
            item13.Items.Add(item9);
            item13.Items.Add(item10);
            item13.Items.Add(item12);


            contextMenu.Items.Add(item1);
            contextMenu.Items.Add(item2);
            contextMenu.Items.Add(item8); 
            contextMenu.Items.Add(item3);
            contextMenu.Items.Add(item7);
            contextMenu.Items.Add(item6);
            contextMenu.Items.Add(item5);
            contextMenu.Items.Add(item4);
            contextMenu.Items.Add(item13);


            Grid grid = new Grid();
            grid.Name = "GifContextMenu";
            grid.ContextMenu = contextMenu;
            pageTemplate.container.Children.Add(grid);

        }
        private void initCCalendarContextMenu()
        {
            ContextMenu contextMenu = new ContextMenu();

            //1.编辑图片
            MenuItem item1 = new MenuItem();
            item1.Header = "编辑日期";
            item1.Click += editCCalendarClick;

            //2.编辑属性
            MenuItem item2 = new MenuItem();
            item2.Header = "编辑属性";
            item2.Click += editCCalendarAttrClick;

            //2.编辑属性
            MenuItem item8 = new MenuItem();
            item8.Header = "编辑动画";
            item8.Click += editImageAnimationClick;



            //4.删除图片
            MenuItem item4 = new MenuItem();
            item4.Header = "删除";
            item4.Click += DeleteImageClick;

            //5.复制控件
            MenuItem item5 = new MenuItem();
            item5.Header = "复制";
            item5.Click += copyControlClick;

            //5.上移一层
            MenuItem item9 = new MenuItem();
            item9.Header = "上移一层";
            item9.Click += moveupIdxClick;

            // 5.下移一层
            MenuItem item10 = new MenuItem();
            item10.Header = "下移一层";
            item10.Click += movedownIdxClick;

            // 5.置于顶层
            MenuItem item11 = new MenuItem();
            item11.Header = "置于顶层";
            item11.Click += moveupTopIdxClick;

            // 5.置于底层
            MenuItem item12 = new MenuItem();
            item12.Header = "置于底层";
            item12.Click += movedownBottomIdxClick;

            MenuItem item13 = new MenuItem();
            item13.Header = "层顺序";
            item13.Items.Add(item11);
            item13.Items.Add(item9);
            item13.Items.Add(item10);
            item13.Items.Add(item12);


            contextMenu.Items.Add(item1);
            contextMenu.Items.Add(item2);
            contextMenu.Items.Add(item8);
            contextMenu.Items.Add(item5);
            contextMenu.Items.Add(item4);
            contextMenu.Items.Add(item13);


            Grid grid = new Grid();
            grid.Name = "CCalendarContextMenu";
            grid.ContextMenu = contextMenu;
            pageTemplate.container.Children.Add(grid);

        }

        private void initCAudioContextMenu()
        {
            ContextMenu contextMenu = new ContextMenu();

            //1.编辑图片
            MenuItem item1 = new MenuItem();
            item1.Header = "编辑音频";
            item1.Click += editCAudioClick;

            //2.编辑属性
            MenuItem item2 = new MenuItem();
            item2.Header = "编辑属性";
            item2.Click += editCAudioAttrClick;

            //2.编辑属性
            MenuItem item8 = new MenuItem();
            item8.Header = "编辑动画";
            item8.Click += editImageAnimationClick;



            //4.删除图片
            MenuItem item4 = new MenuItem();
            item4.Header = "删除";
            item4.Click += DeleteImageClick;

            //5.复制控件
            MenuItem item5 = new MenuItem();
            item5.Header = "复制";
            item5.Click += copyControlClick;

            //5.上移一层
            MenuItem item9 = new MenuItem();
            item9.Header = "上移一层";
            item9.Click += moveupIdxClick;

            // 5.下移一层
            MenuItem item10 = new MenuItem();
            item10.Header = "下移一层";
            item10.Click += movedownIdxClick;

            // 5.置于顶层
            MenuItem item11 = new MenuItem();
            item11.Header = "置于顶层";
            item11.Click += moveupTopIdxClick;

            // 5.置于底层
            MenuItem item12 = new MenuItem();
            item12.Header = "置于底层";
            item12.Click += movedownBottomIdxClick;

            MenuItem item13 = new MenuItem();
            item13.Header = "层顺序";
            item13.Items.Add(item11);
            item13.Items.Add(item9);
            item13.Items.Add(item10);
            item13.Items.Add(item12);


            contextMenu.Items.Add(item1);
            contextMenu.Items.Add(item2);
            contextMenu.Items.Add(item8);
            contextMenu.Items.Add(item5);
            contextMenu.Items.Add(item4);
            contextMenu.Items.Add(item13);


            Grid grid = new Grid();
            grid.Name = "CAudioContextMenu";
            grid.ContextMenu = contextMenu;
            pageTemplate.container.Children.Add(grid);

        }


        /*
        * 2.2、初始化-页面空白处右击菜单
        */
        private void initRightClickContextMenu()
        {

            ContextMenu contextMenu = new ContextMenu();
            contextMenu.Name = "pageRightClickContextMenu";

            //1.插入图片
            MenuItem item1 = new MenuItem();
            item1.Header = "插入图片";
            item1.Click += insertImageClick;
            contextMenu.Items.Add(item1);

            //1.插入文本
            MenuItem item16 = new MenuItem();
            item16.Header = "插入文本";
            item16.Click += insertTextBlockClick;
            contextMenu.Items.Add(item16);

            //2.插入Word文章
            MenuItem item3 = new MenuItem();
            item3.Header = "插入文章";
            item3.Click += insertWordClick;
            contextMenu.Items.Add(item3);

            //4.插入相册
            MenuItem item4 = new MenuItem();
            item4.Header = "插入相册";
            item4.Click += insertTurnPictureClick;
            contextMenu.Items.Add(item4);

            //4.插入流动相册
            MenuItem item12 = new MenuItem();
            item12.Header = "插入流动相册";
            item12.Click += insertMarqueClick;
            contextMenu.Items.Add(item12);

            //4.插入流动相册
            MenuItem item13 = new MenuItem();
            item13.Header = "插入层叠相册";
            item13.Click += insertMarqueLayerClick;
            contextMenu.Items.Add(item13);


            //5.插入视频
            MenuItem item5 = new MenuItem();
            item5.Header = "插入视频";
            item5.Click += insertVideoClick;
            contextMenu.Items.Add(item5);

            //1.插入音频
            MenuItem item19 = new MenuItem();
            item19.Header = "插入音频";
            item19.Click += insertCAudioClick;
            contextMenu.Items.Add(item19);

            //11.小窗口
            MenuItem item15 = new MenuItem();
            item15.Header = "插入小窗口";
            item15.Click += insertCFrameClick;
            contextMenu.Items.Add(item15);

            //1.插入gif
            MenuItem item17 = new MenuItem();
            item17.Header = "插入GIF";
            item17.Click += insertGifClick;
            contextMenu.Items.Add(item17);

            //1.插入日期
            MenuItem item18 = new MenuItem();
            item18.Header = "插入日期";
            item18.Click += insertCCalendarClick;
            contextMenu.Items.Add(item18);



            //5.插入返回按钮
            MenuItem item6 = new MenuItem();
            item6.Header = "插入返回按钮";
            item6.Click += insertBackButtonClick;
            contextMenu.Items.Add(item6);

            //7.插入首页按钮
            MenuItem item7 = new MenuItem();
            item7.Header = "插入首页按钮";
            item7.Click += insertHomeButtonClick;
            contextMenu.Items.Add(item7);


            //9.设置页面背景
            MenuItem item9 = new MenuItem();
            item9.Header = "设置页面背景";
            item9.Click += editBackgroundImageClick;
            contextMenu.Items.Add(item9);

            //9.设置页面背景
            MenuItem item91 = new MenuItem();
            item91.Header = "设置页面视频背景";
            item91.Click += editCVideoBackgroundClick;
            contextMenu.Items.Add(item91);

            //10.粘贴
            MenuItem item10 = new MenuItem();
            item10.Header = "粘贴";
            item10.Click += pastControlClick;
            contextMenu.Items.Add(item10);



            pageTemplate.container.ContextMenu = contextMenu;

        }

        /*
         * 2.1、页面空白处右击，显示菜单
         */
        internal void Page_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            rightClickLocation = e.GetPosition(pageTemplate.container);
            //Point p = Mouse.GetPosition(e.Source as FrameworkElement);
            //Point location = (e.Source as FrameworkElement).PointToScreen(p); 
            pageTemplate.container.ContextMenu.IsOpen = true;
        }


        /*
         * 1.1 右击图片或编辑框
         */
        internal void control_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement ele = (FrameworkElement)sender;
            if (ele.GetType().Name == "Border")
            {
                ele = (FrameworkElement)ele.Tag;
            }
            else
            {
                Border editingBorder = control_GetFocus(sender, e);
            }
            DControl dControl = (DControl)ele.Tag;
            //this.rightClickDControl = dControl;
            rightClickEle = ele;



            //目标
            // ele.ContextMenu.PlacementTarget = this.btnMenu;
            //位置
            // ele.ContextMenu.Placement = PlacementMode.Top;
            //显示菜单

            if (dControl.type == "Image")
            {
                Grid xContextMenu = FrameworkElementUtil.GetChildObject<Grid>(pageTemplate.container, "ImageContextMenu");
                xContextMenu.ContextMenu.IsOpen = true;
            }
            else if (dControl.type == "TurnPicture")
            {
                Grid xContextMenu = FrameworkElementUtil.GetChildObject<Grid>(pageTemplate.container, "TurnPictureContextMenu");
                xContextMenu.ContextMenu.IsOpen = true;
            }
            else if (dControl.type == "Marque")
            {
                Grid xContextMenu = FrameworkElementUtil.GetChildObject<Grid>(pageTemplate.container, "MarqueContextMenu");
                xContextMenu.ContextMenu.IsOpen = true;
            }
            else if (dControl.type == "MarqueLayer")
            {
                Grid xContextMenu = FrameworkElementUtil.GetChildObject<Grid>(pageTemplate.container, "MarqueLayerContextMenu");
                xContextMenu.ContextMenu.IsOpen = true;
            }
            else if (dControl.type == "Word")
            {
                Grid xContextMenu = FrameworkElementUtil.GetChildObject<Grid>(pageTemplate.container, "WordContextMenu");
                xContextMenu.ContextMenu.IsOpen = true;
            }
            else if (dControl.type == "Video")
            {
                Grid xContextMenu = FrameworkElementUtil.GetChildObject<Grid>(pageTemplate.container, "VideoContextMenu");
                xContextMenu.ContextMenu.IsOpen = true;
            }
            else if (dControl.type == "BackButton")
            {
                Grid xContextMenu = FrameworkElementUtil.GetChildObject<Grid>(pageTemplate.container, "BackButtonContextMenu");
                xContextMenu.ContextMenu.IsOpen = true;
            }
            else if (dControl.type == "HomeButton")
            {
                Grid xContextMenu = FrameworkElementUtil.GetChildObject<Grid>(pageTemplate.container, "HomeButtonContextMenu");
                xContextMenu.ContextMenu.IsOpen = true;
            }
            else if (dControl.type == "CFrame")
            {
                Grid xContextMenu = FrameworkElementUtil.GetChildObject<Grid>(pageTemplate.container, "CFrameContextMenu");
                xContextMenu.ContextMenu.IsOpen = true;
            }
            else if (dControl.type == "TextBlock")
            {
                Grid xContextMenu = FrameworkElementUtil.GetChildObject<Grid>(pageTemplate.container, "TextBlockContextMenu");
                xContextMenu.ContextMenu.IsOpen = true;
            }
            else if (dControl.type == "Gif")
            {
                Grid xContextMenu = FrameworkElementUtil.GetChildObject<Grid>(pageTemplate.container, "GifContextMenu");
                xContextMenu.ContextMenu.IsOpen = true;
            }
            else if (dControl.type == "CCalendar")
            {
                Grid xContextMenu = FrameworkElementUtil.GetChildObject<Grid>(pageTemplate.container, "CCalendarContextMenu");
                xContextMenu.ContextMenu.IsOpen = true;
            }
            else if (dControl.type == "CAudio")
            {
                Grid xContextMenu = FrameworkElementUtil.GetChildObject<Grid>(pageTemplate.container, "CAudioContextMenu");
                xContextMenu.ContextMenu.IsOpen = true;
            }
            e.Handled = true;
        }


    }
}
