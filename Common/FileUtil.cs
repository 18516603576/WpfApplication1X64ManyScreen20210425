
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Vlc.DotNet.Wpf;

namespace Common
{
    public class FileUtil
    {
        /*
        * 获取文件路径中的文件名
        * 
        * @param fullFilePath  如：c:/soft/a.jpg  
        * 
        * 返回结果：a.jpg 
        */
        public static String getFilename(string fullFilePath)
        {
            if (string.IsNullOrWhiteSpace(fullFilePath)) return "";

            fullFilePath = fullFilePath.Replace(@"/", @"\");
            int pos = fullFilePath.LastIndexOf(@"\");
            string filename = fullFilePath.Substring(pos + 1);

            return filename;
        }

        /*
        * 获取文件路径中的文件名-标题
        * 
        * @param fullFilePath  如：c:/soft/xinxilan.jpg  
        * 
        * 返回结果：xinxilan
        */
        public static String getFilenameTitle(string fullFilePath)
        {
            if (string.IsNullOrWhiteSpace(fullFilePath)) return "";

            fullFilePath = fullFilePath.Replace(@"/", @"\");
            int pos = fullFilePath.LastIndexOf(@"\");
            string filename = fullFilePath.Substring(pos + 1);


            if (string.IsNullOrWhiteSpace(filename)) return "";
            int pos2 = filename.LastIndexOf(@".");
            string filenameTitle = filename.Substring(0, pos2);



            return filenameTitle;
        }

        /*
         * 获取文件路径中的扩展名
         * 
         * @param fullFilePath  如：c:/soft/a.jpg  
         * 
         * 返回结果：jpg 
         */
        public static String getExt(string fullFilePath)
        {
            if (string.IsNullOrWhiteSpace(fullFilePath)) return "";
            int pos = fullFilePath.LastIndexOf(@".");
            string ext = fullFilePath.Substring(pos + 1);
            return ext.ToLower();
        }

        /*
         * 替换文件路径中的扩展名
         * 
         * @param fullFilePath  如：c:/soft/a.docx  
         * 
         * 返回结果：c:/soft/a.xps   
         */
        public static String replaceExt(string fullFilePath, string newExt)
        {
            if (string.IsNullOrWhiteSpace(fullFilePath)) return "";

            int pos = fullFilePath.LastIndexOf(@".");
            string before = fullFilePath.Substring(0, pos + 1);

            string result = before + newExt;
            return result;
        }

        /*
         * 如果文件所在的文件夹不存在，则创建
         */
        public static void createDirectoryIfNotExits(string file)
        {
            file = file.Replace(@"/", @"\");
            int pos = file.LastIndexOf(@"\");
            string dir = file.Substring(0, pos + 1);
            try
            {
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException("文件路径异常:" + ex.Message);
            }
        }
        /*
         * 如果文件所在的文件夹不存在，则创建
         * 
         * @param  file 如：C:/app/1.txt
         */
        public static string getDirectory(string file)
        {
            file = file.Replace(@"/", @"\");
            int pos = file.LastIndexOf(@"\");
            string dir = file.Substring(0, pos + 1);
            return dir;
        }
        /*
         *  文件不存在，则显示noUrl
         *  
         *  @param string url  文件地址  /f/a.jpg
         *  
         *  @param string nourl  文件不存在显示此地址   /notExists/video.mp4
         */
        public static string notExistsShowDefault(string url, string noUrl)
        {
            if (string.IsNullOrEmpty(url)) return noUrl;
            string result = url;
            Boolean b = File.Exists(AppDomain.CurrentDomain.BaseDirectory + url);
            if (!b)
            {
                result = noUrl;
            }
            return result;
        }

        /*
         *  文件是否存在
         */
        public static Boolean imageIsExists(string url)
        {
            if (string.IsNullOrEmpty(url)) return false;
            Boolean b = File.Exists(AppDomain.CurrentDomain.BaseDirectory + url);
            return b;
        }

        /*
         * 图片加载到内存中去，解决不同进程读取同一张图片的问题
         */
        public static BitmapImage readImage(string file)
        {
            BitmapImage image = new BitmapImage();
            using (Stream stream = new FileStream(file, FileMode.Open))
            {
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = stream;
                image.EndInit();
                image.Freeze();
            }
            return image;
        }

        /*
         * 图片加载到内存中去，解决不同进程读取同一张图片的问题
         */
        public static BitmapImage readImage2(string file, int pixelWidth)
        {
            BitmapImage image = new BitmapImage();
            using (Stream stream = new MemoryStream(File.ReadAllBytes(file)))
            {
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = stream;
                image.DecodePixelWidth = pixelWidth;
                image.EndInit();
                image.Freeze();
            }
            return image;
        }
        public static async void readImage2Button(Button btn, string file, int pixelWidth, Stretch stretch)
        { 
            BitmapImage source = await Task.Run<BitmapImage>(() =>
            {
                BitmapImage bitmapImage = new BitmapImage();
                //打开文件流
                // using (Stream stream = File.OpenRead(imgUrl))
                using (Stream stream = new MemoryStream(File.ReadAllBytes(file)))
                {
                    bitmapImage.BeginInit();
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.StreamSource = stream;
                    bitmapImage.DecodePixelWidth = pixelWidth;
                    bitmapImage.EndInit();
                    //这一句很重要，少了UI线程就不认了。
                    bitmapImage.Freeze();
                }
                return bitmapImage;
            });
            if (btn == null) return;
            //异步进程，使用Dispatcher更新到UI进程
            btn.Dispatcher.Invoke(() =>
            {
                // string name1 = btn.Name; 
                btn.Background = new ImageBrush
                {
                    ImageSource = source
                    , Stretch = stretch
                };  
            });
        }

        public static async void readImage2Image(Image image, string file, int pixelWidth, Stretch stretch)
        {
            BitmapImage source = await Task.Run<BitmapImage>(() =>
            {
                BitmapImage bitmapImage = new BitmapImage();
                //打开文件流
                // using (Stream stream = File.OpenRead(imgUrl))
                using (Stream stream = new MemoryStream(File.ReadAllBytes(file)))
                {
                    bitmapImage.BeginInit();
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.StreamSource = stream;
                    bitmapImage.DecodePixelWidth = pixelWidth;
                    bitmapImage.EndInit();
                    //这一句很重要，少了UI线程就不认了。
                    bitmapImage.Freeze();
                }
                return bitmapImage;
            });
            if (image == null) return;

            image.Dispatcher.Invoke(() =>
            {
                image.Source = source;
                image.Stretch = stretch;
            });


        }

        //public static async void readImage2Grid(Grid grid, string file, int pixelWidth, Stretch stretch)
        //{
        //    BitmapImage source = await Task.Run<BitmapImage>(() =>
        //    {
        //        BitmapImage bitmapImage = new BitmapImage();
        //        //打开文件流
        //        // using (Stream stream = File.OpenRead(imgUrl))
        //        using (Stream stream = new MemoryStream(File.ReadAllBytes(file)))
        //        {
        //            bitmapImage.BeginInit();
        //            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
        //            bitmapImage.StreamSource = stream;
        //            bitmapImage.DecodePixelWidth = pixelWidth;
        //            bitmapImage.EndInit();
        //            //这一句很重要，少了UI线程就不认了。
        //            bitmapImage.Freeze();
        //        }
        //        return bitmapImage;
        //    });

        //    if (grid == null) return;
        //    grid.Background = new ImageBrush
        //    {
        //        ImageSource = source
        //       ,
        //        Stretch = stretch
        //    };

        //}

        public static async void readImage2Page(Page page, string file, int pixelWidth, Stretch stretch)
        {
            
            BitmapImage source = await Task.Run<BitmapImage>(() =>
            {
                BitmapImage bitmapImage = new BitmapImage();
                //打开文件流 
                using (Stream stream = new MemoryStream(File.ReadAllBytes(file)))
                {
                    bitmapImage.BeginInit();
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.StreamSource = stream;
                    bitmapImage.DecodePixelWidth = pixelWidth;
                    bitmapImage.EndInit();
                    //这一句很重要，少了UI线程就不认了。
                    bitmapImage.Freeze();
                }
                return bitmapImage;
            });

            if (page == null) return;

            page.Dispatcher.Invoke(() =>
            { 
                page.Background = new ImageBrush
                {
                    ImageSource = source
               ,
                    Stretch = stretch
                };
            });
            //ImageBrush imageBrush = new ImageBrush
            //{
            //    ImageSource = source
            //   ,
            //    Stretch = stretch
            //};
            //page.Background = imageBrush;
 

        }

        //public static async void readImage2VlcControl(VlcControl vlc, string file, int pixelWidth, Stretch stretch)
        //{
        //    BitmapImage source = await Task.Run<BitmapImage>(() =>
        //    {
        //        BitmapImage bitmapImage = new BitmapImage();
        //        //打开文件流
        //        // using (Stream stream = File.OpenRead(imgUrl))
        //        using (Stream stream = new MemoryStream(File.ReadAllBytes(file)))
        //        {
        //            bitmapImage.BeginInit();
        //            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
        //            bitmapImage.StreamSource = stream;
        //            bitmapImage.DecodePixelWidth = pixelWidth;
        //            bitmapImage.EndInit();
        //            //这一句很重要，少了UI线程就不认了。
        //            bitmapImage.Freeze();
        //        }
        //        return bitmapImage;
        //    });
        //    if (vlc == null) return;
        //    vlc.Background = new ImageBrush
        //    {
        //        ImageSource = source
        //           ,
        //        Stretch = stretch
        //    };

        //}


        /*
         * 字节转换成可阅读的单位
         */
        public static string ByteToKB(Int32 bt)
        {
            int result = 1;
            double k = bt * 1.0 / 1024;
            if (k > 1)
            {
                result = (Int32)Math.Ceiling(k);
            }
            return result + "KB";
        }
        /*
         * 图片加载到内存中去，解决不同进程读取同一张图片的问题
         */
        public static System.Drawing.Bitmap readBitmap(string file)
        {
            System.Drawing.Bitmap bitmap = null;
            using (Stream stream = new FileStream(file, FileMode.Open))
            {
                byte[] data = new byte[stream.Length];
                stream.Read(data, 0, data.Length);

                MemoryStream ms = new MemoryStream(data);
                bitmap = new System.Drawing.Bitmap(ms);
            }
            return bitmap;

        }

        /*
         * 打开文件夹
         */
        public static void openFile(string fullPath) { 
            ProcessStartInfo StartInformation = new ProcessStartInfo();
            StartInformation.FileName = fullPath;
            Process process = Process.Start(StartInformation);
        }
    }
}
