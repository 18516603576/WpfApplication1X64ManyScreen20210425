using Model;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using WpfApplication1.ManageWin;

namespace WpfApplication1.manage
{
    /*
     * 右击控件 - 触发事件  （公用事件） 
     */
    public partial class Editing
    {
        /*
        * 1 编辑动画
        */
        internal void editImageAnimationClick(object sender, RoutedEventArgs e)
        {
            EditImageAnimationWindow win = new EditImageAnimationWindow(this, rightClickEle);
            win.ShowDialog();
        }
        /*
        * 2 链接到  页面 
        */
        internal void linkToClick(object sender, RoutedEventArgs e)
        {
            EditLinkToWindow win = new EditLinkToWindow(mainFrame, rightClickEle);
            win.ShowDialog();
        }
        /*
         * 3 弹窗
         */
        internal void editFrameDialogToClick(object sender, RoutedEventArgs e)
        {
            EditFrameDialogToWindow win = new EditFrameDialogToWindow(mainFrame, rightClickEle);
            win.ShowDialog();
        }
        /*
        * 4 链接到外部网站
        */
        internal void linkToWebClick(object sender, RoutedEventArgs e)
        {
            EditLinkToWebWindow win = new EditLinkToWebWindow(this, rightClickEle);
            win.ShowDialog();
        }
        /*
        * 5 链接到全屏视频
        */
        internal void editOpenVideoClick(object sender, RoutedEventArgs e)
        {
            EditOpenVideoWindow win = new EditOpenVideoWindow(this, rightClickEle);
            win.ShowDialog();
        }
        /*
         * 6 删除控件
         */
        public void DeleteImageClick(object sender, RoutedEventArgs e)
        {
            //1.从控件库中删除
            DControl currDControl = (DControl)rightClickEle.Tag;
            dControlBll.delete(currDControl.id);

            //2.从页面中删除
            pageTemplate.container.Children.Remove(rightClickEle);

            //3.移除编辑框
            pageTemplate.container.Children.Remove(editingBorder);
        }
        /*
        * 7 复制图片控件
        */
        internal void copyControlClick(object sender, RoutedEventArgs e)
        {

            App.localStorage.currCopiedEle = rightClickEle;

        }
        /*
         * 8 上移一层
         */
        internal void moveupIdxClick(object sender, RoutedEventArgs e)
        {
            DControl dControl = (DControl)rightClickEle.Tag;
            int currIdx = dControl.idx;
            int upCurrIdx = currIdx + 1;

            //2.更新页面控件层次 
            foreach (FrameworkElement ele in pageTemplate.container.Children)
            {
                if (ele.GetType().Name == "Grid" || ele.GetType().Name == "Border") continue;

                DControl eleDControl = (DControl)ele.Tag;
                if (eleDControl.idx == upCurrIdx)
                {

                    Panel.SetZIndex(rightClickEle, upCurrIdx);
                    Panel.SetZIndex(ele, currIdx);

                    dControl.idx = upCurrIdx;
                    eleDControl.idx = currIdx;


                    //1.更新数据库
                    dControlBll.moveUpIdx(dControl, eleDControl);
                    break;
                }

            }

        }
        /*
         * 9下移一层
         */
        internal void movedownIdxClick(object sender, RoutedEventArgs e)
        {
            DControl dControl = (DControl)rightClickEle.Tag;
            int currIdx = dControl.idx;
            int downCurrIdx = currIdx - 1;

            //2.更新页面控件层次 
            foreach (FrameworkElement ele in pageTemplate.container.Children)
            {
                //排除右键菜单 和 编辑框
                string typename = ele.GetType().Name;
                if (ele.GetType().Name == "Grid" || ele.GetType().Name == "Border") continue;

                DControl eleDControl = (DControl)ele.Tag;
                if (eleDControl.idx == downCurrIdx)
                {

                    Panel.SetZIndex(rightClickEle, downCurrIdx);
                    Panel.SetZIndex(ele, currIdx);

                    dControl.idx = downCurrIdx;
                    eleDControl.idx = currIdx;

                    //1.更新数据库
                    dControlBll.moveUpIdx(dControl, eleDControl);
                    break;
                }

            }

        }
        /*
        * 10 置于顶层
        */
        internal void moveupTopIdxClick(object sender, RoutedEventArgs e)
        {
            //1.更新到数据库 
            DControl dControl = (DControl)rightClickEle.Tag;
            List<DControl> list = dControlBll.moveupTopIdx(dControl);

            //2.更新页面控件层次 
            foreach (FrameworkElement ele in pageTemplate.container.Children)
            {
                //排除右键菜单 和 编辑框
                string typename = ele.GetType().Name;
                if (ele.GetType().Name == "Grid" || ele.GetType().Name == "Border") continue;

                DControl eleDControl = (DControl)ele.Tag;
                foreach (DControl dbDControl in list)
                {
                    if (eleDControl.id == dbDControl.id)
                    {
                        ele.Tag = dbDControl;
                        Panel.SetZIndex(ele, dbDControl.idx);
                        break;
                    }
                }

            }
        }
        /*
        * 11 置于底层
        */
        internal void movedownBottomIdxClick(object sender, RoutedEventArgs e)
        {
            //1.更新到数据库 
            DControl dControl = (DControl)rightClickEle.Tag;
            List<DControl> list = dControlBll.movedownBottomIdx(dControl);

            //2.更新页面控件层次 
            foreach (FrameworkElement ele in pageTemplate.container.Children)
            {
                //排除右键菜单 和 编辑框
                string typename = ele.GetType().Name;
                if (ele.GetType().Name == "Grid" || ele.GetType().Name == "Border") continue;

                DControl eleDControl = (DControl)ele.Tag;
                foreach (DControl dbDControl in list)
                {
                    if (eleDControl.id == dbDControl.id)
                    {
                        ele.Tag = dbDControl;
                        Panel.SetZIndex(ele, dbDControl.idx);
                        break;
                    }
                }

            }
        }
    }
}
