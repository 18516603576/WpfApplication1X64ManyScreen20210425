using Common.Data;
using Common.MultiThread;
using Common.util;
using Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows.Xps.Packaging;

namespace Common.control
{
    /// <summary>
    /// CWord.xaml 的交互逻辑
    /// </summary>
    public partial class CWord : UserControl
    {

        private readonly string wordFullPath;
        private Size size;


        private int currPage = 1;
        private readonly List<string> imageList = new List<string>();
        private readonly DControl currDControl;
        //待执行的队列
        private readonly List<CWordPageQueue> pageQueueList = new List<CWordPageQueue>();
        public CWord(string wordFullPath, DControl ctl)
        {
            InitializeComponent();
            currDControl = ctl;
            this.wordFullPath = wordFullPath;
            loadWord();

            SizeChanged += CWordSizeChanged;

        }


        /*
        * 加载word 文件
        * 
        * @param DocumentViewer 显示容器
        * 
        * @param DControl ctl 控件信息
        */
        private void loadWord()
        {
            try
            {
                XpsDocument xpsDoc = new XpsDocument(wordFullPath, FileAccess.Read, CompressionOption.SuperFast);
                FixedDocumentSequence fixedDocumentSequence = xpsDoc.GetFixedDocumentSequence();
                DocumentPaginator documentPaginator = fixedDocumentSequence.DocumentPaginator;
                size = fixedDocumentSequence.DocumentPaginator.PageSize;
                for (int i = 0; i < documentPaginator.PageCount; i++)
                {
                    DispatcherContainer dispatcherContainer = new DispatcherContainer();
                    dispatcherContainer.Width = size.Width;
                    dispatcherContainer.Height = size.Height;
                    dispatcherContainer.Tag = i;
                    contentStackPanel.Children.Add(dispatcherContainer);

                    CWordPageQueue cwordPageQueue = new CWordPageQueue();
                    pageQueueList.Add(cwordPageQueue);

                }
                xpsDoc.Close();
            }
            catch (Exception)
            {

            }
        }

        private void CWordSizeChanged(object sender, SizeChangedEventArgs e)
        {

        }

        /*
         * 插入内容
         */
        public void insertContent(FixedPage fixedPage)
        {
            contentStackPanel.Children.Add(fixedPage);
        }

        /*
         * 更新内容
         */
        public void updateContent(int n, FixedPage fixedPage)
        {
            if (n >= contentStackPanel.Children.Count) return;
            FixedPage currFixedPage = (FixedPage)contentStackPanel.Children[n];


            for (int i = 0; i < fixedPage.Children.Count; i++)
            {
                UIElement DocFpUiElem = fixedPage.Children[i];
                fixedPage.Children.Remove(DocFpUiElem);
                currFixedPage.Children.Add(DocFpUiElem);
            }



        }

        public void clearContent()
        {
            contentStackPanel.Children.Clear();
        }

        /*
         *  宽度自适应
         */
        public void FitToWidth()
        {
            double maxWidth = 0.0;
            double maxHeight = 0.0;
            foreach (FrameworkElement ele in contentStackPanel.Children)
            {
                if (ele is FixedPage)
                {
                    if (ele.Width > maxWidth)
                    {
                        maxWidth = ele.Width;
                    }
                    if (ele.Height > maxHeight)
                    {
                        maxHeight = ele.Height;
                    }
                }
            }

            if (maxWidth > 0)
            {
                double scaleX = Width / maxWidth;
                double h = cWordStackPanel.Height;

                contentStackPanel.RenderTransform = new ScaleTransform(scaleX, scaleX);
                cWordScrollViewer.Width = Width;
                cWordStackPanel.Height = maxHeight * contentStackPanel.Children.Count * scaleX;
            }

        }

        private void ManipulationBoundaryFeedbackEvent(object sender, ManipulationBoundaryFeedbackEventArgs e)
        {
            e.Handled = true;
        }

        /*
         * 更换word
         */
        public void updateElement(DControl dControl, bool v, string wordFullPath)
        {
            WordUtil.loadWord(this, wordFullPath);
            FitToWidth();
        }
        /*
         * 更新Word属性
         */
        public void updateElementAttr(DControl dControl, bool v)
        {
            Width = dControl.width;
            Height = dControl.height;
            Margin = new Thickness(dControl.left, dControl.top, 0, 0);
            Opacity = dControl.opacity / 100.0;
            FitToWidth();
        }
        private async void CWord_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {

            double topOffset = cWordScrollViewer.VerticalOffset;
            int page = (int)Math.Floor(topOffset / size.Height);
            if (page == currPage) return;
            if (page >= contentStackPanel.Children.Count) return;
            //if (page < currPage) return;
            currPage = page;


            //DispatcherContainer target = (DispatcherContainer)contentStackPanel.Children[this.currPage];

            //await target.SetChildAsync(() =>
            //{ 
            //    XpsDocument xpsDoc = new XpsDocument(wordFullPath, FileAccess.Read, CompressionOption.SuperFast);
            //    FixedDocumentSequence fixedDocumentSequence = xpsDoc.GetFixedDocumentSequence();
            //    foreach (DocumentReference DocRef in fixedDocumentSequence.References)
            //    {
            //        bool bForceReload = false;
            //        FixedDocument DocFd = DocRef.GetDocument(bForceReload);

            //        return DocFd.Pages[currPage].GetPageRoot(false);
            //    }
            //    xpsDoc.Close();//这个地方需要注意关闭，否则的话会出现莫名其妙的错误  
            //    return null;
            //});

            // lock (this) {  

            for (int b = 0; b < contentStackPanel.Children.Count; b++)
            {
                DispatcherContainer ele = (DispatcherContainer)contentStackPanel.Children[b];

                if (b >= currPage - 2 && b <= currPage + 1)
                {
                    //显示前面3页，后面5页  
                    if (ele.Child == null || ele.Child is Border)
                    {
                        //1.向队列中添加任务
                        int count = pageQueueList[b].list.Count;
                        if (count == 0)
                        {
                            TaskInfo taskInfo = new TaskInfo();
                            taskInfo.taskStatus = 0;
                            taskInfo.isLoadPage = true;
                            pageQueueList[b].list.Add(taskInfo);
                            LoadPageTask(b, taskInfo);
                        }
                        else if (count == 1)
                        {
                            TaskInfo taskInfo = new TaskInfo();
                            taskInfo.taskStatus = 0;
                            taskInfo.isLoadPage = true;
                            pageQueueList[b].list.Add(taskInfo);
                        }
                        else
                        {
                            TaskInfo taskInfo = new TaskInfo();
                            taskInfo.taskStatus = 0;
                            taskInfo.isLoadPage = true;
                            pageQueueList[b].list[1] = taskInfo;

                        }
                    }
                }
                else
                {
                    if (ele.Child != null && (ele.Child is FixedPage))
                    {

                        //1.向队列中添加任务
                        int count2 = pageQueueList[b].list.Count;
                        if (count2 == 0)
                        {
                            TaskInfo taskInfo = new TaskInfo();
                            taskInfo.taskStatus = 0;
                            taskInfo.isLoadPage = false;
                            pageQueueList[b].list.Add(taskInfo);
                            RemovePageTask(b, taskInfo);
                        }
                        else if (count2 == 1)
                        {
                            TaskInfo taskInfo = new TaskInfo();
                            taskInfo.taskStatus = 0;
                            taskInfo.isLoadPage = false;
                            pageQueueList[b].list.Add(taskInfo);
                        }
                        else
                        {
                            TaskInfo taskInfo = new TaskInfo();
                            taskInfo.taskStatus = 0;
                            taskInfo.isLoadPage = false;
                            pageQueueList[b].list[1] = taskInfo;
                        }

                        //UIElement uiElement = ele.Child;
                        //uiElement = null;
                        //ele.SetValue(ContentPresenter.ContentProperty, null);
                        ////没有断开父元素与子元素链接
                        //ele.SetChildAsync(() =>
                        //{
                        //    return new Border();
                        //});

                    }
                }


            }


            //}
        }



        private async void LoadPageTask(int pageId, TaskInfo taskInfo)
        {
            DispatcherContainer target = (DispatcherContainer)contentStackPanel.Children[pageId];
            // if (target.Child != null && target.Child is FixedPage) return;
            //  if (target.GetIsUpdatingChild()) return;  
            // Console.WriteLine("获取页面：" + pageId);
            Console.WriteLine("获取页面：" + pageId + "___ThreadId：" + Thread.CurrentThread.ManagedThreadId);

            //await 回调，执行下一个
            await target.SetChildAsync<FixedPage>(() =>
            {

                FixedPage fp = new FixedPage();
                try
                {

                    XpsDocument xpsDoc = new XpsDocument(wordFullPath, FileAccess.Read);
                    FixedDocumentSequence fixedDocumentSequence = xpsDoc.GetFixedDocumentSequence();
                    foreach (DocumentReference DocRef in fixedDocumentSequence.References)
                    {
                        Console.WriteLine("获取页面await：" + pageId + "___ThreadId：" + Thread.CurrentThread.ManagedThreadId);
                        Console.WriteLine();
                        bool bForceReload = false;
                        FixedDocument DocFd = DocRef.GetDocument(bForceReload);
                        PageContent pageContent = DocFd.Pages[pageId];
                        fp = pageContent.GetPageRoot(false);

                        // pageContent.SetValue(ContentPresenter.ContentProperty, null);

                        //for (int i = 0; i < fixedPage.Children.Count; i++)
                        //{
                        //    UIElement uiElement = fp.Children[i];
                        //    fixedPage.Children.Remove(uiElement);
                        //    fp.Children.Add(uiElement);
                        //}
                    }
                    // xpsDoc.Close();//这个地方需要注意关闭，否则的话会出现莫名其妙的错误  
                    return fp;
                }
                catch (Exception)
                {
                    return fp;
                }


            }
            , () =>
            {
                FixedPage fp = new FixedPage();
                pageQueueList[pageId].list.Remove(taskInfo);
                if (pageQueueList[pageId].list.Count > 0)
                {
                    LoadPageTask(pageId, pageQueueList[pageId].list[0]);
                }
                return fp;
            });


            //  Console.WriteLine("任务是否完成："+task.IsCompleted);
        }

        private async void RemovePageTask(int pageId, TaskInfo taskInfo)
        {
            DispatcherContainer target = (DispatcherContainer)contentStackPanel.Children[pageId];
            Console.WriteLine("移除页面：" + pageId + "___ThreadId：" + Thread.CurrentThread.ManagedThreadId);
            UIElement uiElement = target.Child;
            uiElement = null;
            target.SetValue(ContentPresenter.ContentProperty, null);

            await target.SetChildAsync<Border>(() =>
            {
                Console.WriteLine("移除页面await：" + pageId + "___ThreadId：" + Thread.CurrentThread.ManagedThreadId);
                return new Border();
            }
            , () =>
            {
                Border bd = new Border();
                pageQueueList[pageId].list.Remove(taskInfo);
                if (pageQueueList[pageId].list.Count > 0)
                {
                    RemovePageTask(pageId, pageQueueList[pageId].list[0]);
                }
                return bd;
            });
        }

        public void DoEvents()
        {
            DispatcherFrame frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,
                new DispatcherOperationCallback(ExitFrames), frame);
            Dispatcher.PushFrame(frame);
        }

        public object ExitFrames(object f)
        {
            ((DispatcherFrame)f).Continue = false;

            return null;
        }
        private void Action1<FixedPage>()
        {


            //  XpsDocument xpsDoc = new XpsDocument(wordFullPath, FileAccess.Read, CompressionOption.SuperFast);

            //  //  xpsDoc.FixedDocumentSequenceReader.GetFixedDocument(new Uri(wordFullPath)); 
            //    IXpsFixedDocumentSequenceReader docSeq = xpsDoc.FixedDocumentSequenceReader; 
            //    IXpsFixedDocumentReader docReader = docSeq.FixedDocuments[0]; 

            //    IXpsFixedPageReader fp0 = docReader.FixedPages[1];
            //// XmlReader reader = fp0.XmlReader;
            //FixedPage fp = new FixedPage();

            //this.Dispatcher.Invoke(() => {
            //    fp = new FixedPage();
            //    fp.Background = Brushes.Red;
            //    fp.Width = this.currDControl.width;
            //    fp.Height = this.currDControl.height;
            //    Label label1 = new Label();
            //    label1.Content = "内容。。。。。。。。";

            //    fp.Children.Add(label1);

            //});







            //while (await reader.ReadAsync())
            //    {
            //        switch (reader.NodeType)
            //        {
            //            case XmlNodeType.Element:
            //                Console.WriteLine("Start Element {0}", reader.Name);
            //                break;
            //            case XmlNodeType.Text:
            //                Console.WriteLine("Text Node: {0}",
            //                         await reader.GetValueAsync());
            //                break;
            //            case XmlNodeType.EndElement:
            //                Console.WriteLine("End Element {0}", reader.Name);
            //                break;
            //            default:
            //                Console.WriteLine("Other node {0} with value {1}",
            //                                reader.NodeType, reader.Value);
            //                break;
            //        }
            //    }


            //FixedDocumentSequence fixedDocumentSequence = xpsDoc.GetFixedDocumentSequence();

            //foreach (DocumentReference DocRef in fixedDocumentSequence.References)
            //{
            //    bool bForceReload = false;
            //    FixedDocument DocFd = DocRef.GetDocument(bForceReload);
            //    int a = 0;
            //    foreach (PageContent DocFpPc in DocFd.Pages)
            //    {
            //        if (a == currPage)
            //        {

            //            //fp.SetValue(DockPanel.DockProperty, Dock.Top);
            //            //fp.Width = size.Width;
            //            //fp.Height = size.Height;
            //            //fp.Background = System.Windows.Media.Brushes.Blue;

            //            FixedPage DocFp = DocFpPc.GetPageRoot(bForceReload);
            //            for (int i = 0; i < DocFp.Children.Count; i++)
            //            {
            //                UIElement DocFpUiElem = DocFp.Children[i];
            //                DocFp.Children.Remove(DocFpUiElem);
            //                //fp.Children.Add(DocFpUiElem);
            //            }
            //        }
            //        a = a + 1;

            //    }
            //}
            // xpsDoc.Close();//这个地方需要注意关闭，否则的话会出现莫名其妙的错误  



            // return fp;
        }

        //private async void CWord_ScrollChanged(object sender, ScrollChangedEventArgs e)
        //{
        //     double topOffset  = cWordScrollViewer.VerticalOffset; 

        //     int page =  (int)Math.Floor(topOffset / size.Height) +1 ;
        //     if (page == currPage) return;
        //     if (page >= contentStackPanel.Children.Count)   return;

        //     this.currPage = page;

        //    FixedPage fixedPage = null;
        //    FixedPage fp = new FixedPage();
        //    Border outBordre = new Border();
        //    foreach (DocumentReference DocRef in this.fixedDocumentSequence.References)
        //    {
        //        bool bForceReload = false;
        //        FixedDocument DocFd = DocRef.GetDocument(bForceReload);
        //        PageContent pageContent = DocFd.Pages[currPage];

        //      List<PageContent> list =  DocFd.Pages.ToList<PageContent>();

        //     PageContent pc =   list[currPage];
        //        list.Remove(pc);
        //        pc.Child = null;
        //            fixedPage = pc.GetPageRoot(bForceReload);


        //            for (int i = 0; i < fixedPage.Children.Count; i++)
        //            {
        //                UIElement DocFpUiElem = fixedPage.Children[i];
        //                fixedPage.Children.Remove(DocFpUiElem);
        //                fp.Children.Add(DocFpUiElem);
        //                // currFixedPage.Children.Add(DocFpUiElem);
        //            }


        //        outBordre.BorderBrush = Brushes.White;
        //        outBordre.BorderThickness = new Thickness(0.5);
        //        outBordre.Child = fixedPage;






        //    };
        //      FixedPage currFixedPage = (FixedPage)contentStackPanel.Children[this.currPage];

        //    contentStackPanel.Children.Remove(currFixedPage);
        //    contentStackPanel.Children.Insert(this.currPage, outBordre);






        //    //foreach (DocumentReference DocRef in this.fixedDocumentSequence.References)
        //    //{
        //    //    bool bForceReload = false;
        //    //    FixedDocument DocFd = DocRef.GetDocument(bForceReload);
        //    //    PageContent pageContent = DocFd.Pages[currPage];
        //    //    FixedPage fixedPage = pageContent.GetPageRoot(bForceReload);

        //    //    FixedPage currFixedPage = (FixedPage)contentStackPanel.Children[this.currPage];


        //    //FixedPage fp = new FixedPage();
        //    //for (int i = 0; i < fixedPage.Children.Count; i++)
        //    //{
        //    //    UIElement DocFpUiElem = fixedPage.Children[i];
        //    //    fixedPage.Children.Remove(DocFpUiElem);
        //    //  //   fp.Children.Add(DocFpUiElem);
        //    //   // currFixedPage.Children.Add(DocFpUiElem);
        //    //}


        //    // } ; 
        //    // contentStackPanel.Children[this.currPage] = source;

        //}
    }
}
