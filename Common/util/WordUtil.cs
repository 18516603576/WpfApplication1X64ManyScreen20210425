using Common.control;
using System;
using System.IO;
using System.IO.Packaging;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Xps.Packaging;

namespace Common.util
{
    public class WordUtil
    {

        /*
         * 加载word 文件
         * 
         * @param DocumentViewer 显示容器
         * 
         * @param DControl ctl 控件信息
         */
        public static IDocumentPaginatorSource loadWord(string wordFullFile)
        {
            try
            {
                XpsDocument xpsDoc = new XpsDocument(wordFullFile, FileAccess.Read, CompressionOption.SuperFast);
                FixedDocumentSequence fixedDoc = xpsDoc.GetFixedDocumentSequence();
                return fixedDoc as IDocumentPaginatorSource;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /*
         * 加载word 文件
         * 
         * @param DocumentViewer 显示容器
         * 
         * @param DControl ctl 控件信息
         */
        public static void loadWord(CWord cWord, string wordFullFile)
        {
            cWord.clearContent();
            string xpsFile = wordFullFile;
            XpsDocument xpsDoc = new XpsDocument(xpsFile, FileAccess.Read);
            FixedDocumentSequence fds = xpsDoc.GetFixedDocumentSequence();
            System.Windows.Size size = fds.DocumentPaginator.PageSize;

            foreach (DocumentReference DocRef in fds.References)
            {
                bool bForceReload = false;
                FixedDocument DocFd = DocRef.GetDocument(bForceReload);

                foreach (PageContent DocFpPc in DocFd.Pages)
                {

                    FixedPage fp = new FixedPage();
                    fp.SetValue(DockPanel.DockProperty, Dock.Top);
                    fp.Width = size.Width;
                    fp.Height = size.Height;
                    fp.Background = System.Windows.Media.Brushes.Red;

                    //FixedPage DocFp = DocFpPc.GetPageRoot(bForceReload);
                    //for (int i = 0; i < DocFp.Children.Count; i++)
                    //{
                    //    UIElement DocFpUiElem = DocFp.Children[i];
                    //    DocFp.Children.Remove(DocFpUiElem);
                    //    fp.Children.Add(DocFpUiElem);
                    //}
                    cWord.insertContent(fp);
                }
            }
            xpsDoc.Close();//这个地方需要注意关闭，否则的话会出现莫名其妙的错误  
        }


        public static Boolean ConvertToXPS(string DocName, string xpsDocName)
        {
            Boolean b = false;

            string ext = FileUtil.getExt(DocName);
            if (ext == "doc" || ext == "docx")
            {
                b = ConvertWordToXPS2(DocName, xpsDocName);
            }
            else if (ext == "pdf")
            {

                b = ConvertPdfToXPS(DocName, xpsDocName);
            }
            return b;
        }


        /// <summary>
        /// 将Pdf文档转换为xps文档
        /// </summary>
        /// <param name="wordDocName">word文档全路径</param>
        /// <param name="xpsDocName">xps文档全路径</param>
        /// <returns></returns>
        public static Boolean ConvertPdfToXPS(string pdfDocName, string xpsDocName)
        {
            Boolean b = false;

            try
            {
                //读取doc文档           

                Aspose.Pdf.Document doc = new Aspose.Pdf.Document(pdfDocName);

                //保存为PDF文件，此处的SaveFormat支持很多种格式，如图片，epb

                //rtf 等等       
                doc.Save(xpsDocName, Aspose.Pdf.SaveFormat.Xps);

                b = true;
            }
            catch (Exception e1)
            {
                b = false;
                System.Windows.MessageBox.Show("pdf转换失败：" + e1.Message);
            }
            return b;
        }

        /// <summary>
        /// 将Pdf文档转换为xps文档
        /// </summary>
        /// <param name="wordDocName">word文档全路径</param>
        /// <param name="xpsDocName">xps文档全路径</param>
        /// <returns></returns>
        public static Boolean ConvertWordToXPS2(string wordDocName, string xpsDocName)
        {
            Boolean b = false;
            try
            {
                //读取doc文档           

                Aspose.Words.Document doc = new Aspose.Words.Document(wordDocName);

                //保存为PDF文件，此处的SaveFormat支持很多种格式，如图片，epb, 

                doc.Save(xpsDocName, Aspose.Words.SaveFormat.Xps);
                b = true;
            }
            catch (Exception e1)
            {
                b = false;
                System.Windows.MessageBox.Show("word转换失败：" + e1.Message);
            }

            return b;
        }


        /*
        * Word容器尺寸变化时
        */
        public static void Word_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (sender is DocumentViewer)
            {
                DocumentViewer tmp = (DocumentViewer)sender;
                tmp.FitToWidth();
            }
        }


        public static Boolean ConvertWordToJpg(string wordDocName, string jpgPath)
        {
            Boolean b = false;
            try
            {
                Aspose.Words.Document doc = new Aspose.Words.Document(wordDocName);

                Aspose.Words.Saving.ImageSaveOptions iso = new Aspose.Words.Saving.ImageSaveOptions(Aspose.Words.SaveFormat.Png);
                iso.Resolution = 768;
                iso.PaperColor = System.Drawing.Color.Transparent;
                iso.PrettyFormat = true;
                iso.UseAntiAliasing = true;
                for (int i = 0; i < doc.PageCount; i++)
                {
                    iso.PageIndex = i;
                    doc.Save(jpgPath + i + ".png", iso);
                }
                b = true;
            }
            catch (Exception e1)
            {
                b = false;
                System.Windows.MessageBox.Show("word转换失败：" + e1.Message);
            }
            return b;
        }

    }
}
