using Spire.Pdf;
using System;
using System.Collections.Generic;
using System.Text;

namespace SDoc
{
    public  class DocUtil
    {
        

        public static Boolean ConvertToXPS(string DocName, string xpsDocName)
        {
            Boolean b = false;
            //string ext = FileUtil.getExt(DocName);
            string ext = "doc";
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
                //创建一个PdfDocument类实例，并加载文档
                PdfDocument doc = new PdfDocument();
                doc.LoadFromFile(pdfDocName);
                PdfPageBase firstPage = doc.Pages.Add();
                doc.Pages.Remove(firstPage);
                //保存文件为XPS
                doc.SaveToFile(xpsDocName, FileFormat.XPS);
                b = true;
            }
            catch (Exception e1)
            {
                b = false;
              //  System.Windows.MessageBox.Show("pdf转换失败：" + e1.Message);
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
                //初始化String类，元素为需要转换的Word文档

                //创建一个Document类对象，加载sample文件
                Spire.Doc.Document doc = new Spire.Doc.Document();
                doc.LoadFromFile(wordDocName);

                //  Spire.Doc.Section section =  new Spire.Doc.Section(null);
                // doc.Sections.Add(section);


                //设置文档的背景填充模式为颜色填充
                //  doc.Background.Type = BackgroundType.Color;
                //设置背景颜色
                //   doc.Background.Color = System.Drawing.Color.Transparent;

                //将Word文件保存为XPS，并运行生成的文档
                doc.SaveToFile(xpsDocName, Spire.Doc.FileFormat.XPS);
                b = true;
            }
            catch (Exception e1)
            {
                b = false;
              //  System.Windows.MessageBox.Show("word转换失败：" + e1.Message);
            }
            return b;
        }

         
    }
}
