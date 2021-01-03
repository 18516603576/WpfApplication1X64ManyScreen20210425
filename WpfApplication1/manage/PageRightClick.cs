using Common;
using Common.util;
using Model;
using Model.dto;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using WpfApplication1.ManageWin;

namespace WpfApplication1.manage
{
    /*
     * 页面空白处右击 - 触发的事件
     */
    public partial class Editing
    {
        /*
        * 1 右击-插入图片到页面
        */
        public void insertImageClick(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog ofld = new System.Windows.Forms.OpenFileDialog();
            ofld.Filter = "图片|*.jpg;*.jpeg;*.png;*.gif;*.bmp";
            if (ofld.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string text = ofld.FileName;
                if (text != "" || text != null)
                {
                    System.Drawing.Image sourceImage = System.Drawing.Image.FromFile(text);
                    DControl dControl = new DControl();
                    dControl.pageId = App.localStorage.currPageId; /////////
                    dControl.name = "Image";
                    dControl.content = "";
                    dControl.width = sourceImage.Width;
                    dControl.height = sourceImage.Height;
                    dControl.left = (int)rightClickLocation.X;/////////
                    dControl.top = (int)rightClickLocation.Y;/////////
                    dControl.type = "Image";
                    // dControl.idx = 1;//////// 


                    //1、复制文件
                    String url = uploadImage(text);
                    double showWidth = sourceImage.Width;
                    double showHeight = sourceImage.Height;
                    double w = sourceImage.Width / App.localStorage.cfg.screenWidth;
                    double h = sourceImage.Height / App.localStorage.cfg.screenHeight;
                    if (w > 1 && h > 1)
                    {
                        if (w > h)
                        {
                            showWidth = App.localStorage.cfg.screenWidth;
                            showHeight = sourceImage.Height / w;
                        }
                        else
                        {
                            showHeight = App.localStorage.cfg.screenHeight;
                            showWidth = sourceImage.Width / h;
                        }
                    }
                    else if (w > 1)
                    {
                        showWidth = App.localStorage.cfg.screenWidth;
                        showHeight = sourceImage.Height / w;
                    }
                    else if (h > 1)
                    {
                        showHeight = App.localStorage.cfg.screenHeight;
                        showWidth = sourceImage.Width / h;
                    }
                    dControl.width = (int)showWidth;
                    dControl.height = (int)showHeight;


                    //2、插入文件库
                    StorageImage storageImage = storageImageBll.insert(text, url, sourceImage.Width, sourceImage.Height, 1);
                    //3、插入控件库
                    dControl.storageId = storageImage.id;
                    dControl = dControlBll.insert(dControl);
                    //4、插入页面  
                    insertImageToPage(dControl);
                }

            }
        }

        /*
         * 上传图片，复制的软件目录下
         */
        private String uploadImage(string sourceFilePath)
        {
            //1.复制到软件目录
            String origFilename = FileUtil.getFilename(sourceFilePath);
            string yyyyMMdd = DateTime.Now.ToString("yyyyMMdd");
            string guid = System.Guid.NewGuid().ToString("N");
            String destFilePath = "/myfile/upload/image/" + yyyyMMdd + "/" + guid + "/" + origFilename;

            String fullDestFilePath = AppDomain.CurrentDomain.BaseDirectory + destFilePath;
            FileUtil.createDirectoryIfNotExits(fullDestFilePath);
            File.Copy(sourceFilePath, fullDestFilePath);

            return destFilePath;
        }

        /*
         * 2 右击-插入Word文章
        */
        public void insertTextBlockClick(object sender, RoutedEventArgs e)
        {
            DControl dControl = new DControl();
            dControl.pageId = App.localStorage.currPageId; /////////
            dControl.name = "TextBlock";
            dControl.content = "这是一行文字";
            dControl.width = 235;
            dControl.height = 48;
            dControl.fontSize = 36;
            dControl.fontColor = "000000";
            dControl.fontLineHeight = 48;
            dControl.fontWeight = "Bold";
            dControl.fontFamily = "宋体";
            dControl.left = (int)rightClickLocation.X;/////////
            dControl.top = (int)rightClickLocation.Y;/////////
            dControl.type = "TextBlock";
            //  dControl.url = "/myfile/sysimg/Word/default.xps";//////////
            //  dControl.imgs = "";
            dControl.idx = 1;////////
            dControl.storageId = 1;


            //1、复制文件
            //String url = this.uploadImage(text);
            //2、插入文件库
            //StorageImage storageImage = storageImageBll.insert(text, url);
            //3、插入控件库
            // dControl.url = url;
            dControl = dControlBll.insert(dControl);
            //4、插入页面  
            insertTextBlockToPage(dControl);
        }

        /*
       * 3 击-插入Word文章
       */
        public void insertWordClick(object sender, RoutedEventArgs e)
        {
            DControl dControl = new DControl();
            dControl.pageId = App.localStorage.currPageId; /////////
            dControl.name = "Word";
            dControl.content = "";
            dControl.width = 560;
            dControl.height = 720;
            dControl.left = (int)rightClickLocation.X;/////////
            dControl.top = (int)rightClickLocation.Y;/////////
            dControl.type = "Word";
            //  dControl.url = "/myfile/sysimg/Word/default.xps";//////////
            //  dControl.imgs = "";
            dControl.idx = 1;////////
            dControl.storageId = 1;


            //1、复制文件
            //String url = this.uploadImage(text);
            //2、插入文件库
            //StorageImage storageImage = storageImageBll.insert(text, url);
            //3、插入控件库
            // dControl.url = url;
            dControl = dControlBll.insert(dControl);
            //4、插入页面  
            insertWordToPage(dControl);
        }


        /*
        * 4 右击-插入相册
        */
        public void insertTurnPictureClick(object sender, RoutedEventArgs e)
        {
            DControl dControl = new DControl();
            dControl.pageId = App.localStorage.currPageId; /////////
            dControl.name = "TurnPicture";
            dControl.content = "";
            dControl.width = 576;
            dControl.height = 324;
            dControl.left = (int)rightClickLocation.X;/////////
            dControl.top = (int)rightClickLocation.Y;/////////
            dControl.type = "TurnPicture";
            // dControl.url = "";//////////
            // dControl.imgs = "/myfile/sysimg/TurnPicture/1.jpg,/myfile/sysimg/TurnPicture/2.jpg,/myfile/sysimg/TurnPicture/3.jpg,/myfile/sysimg/TurnPicture/4.jpg,/myfile/sysimg/TurnPicture/5.jpg";
            dControl.idx = 1;////////
            dControl.autoplay = true;
            dControl.loop = true;
            dControl.turnPictureSpeed = 8;
            dControl.isShowTurnPictureArrow = true;


            //1、复制文件
            //String url = this.uploadImage(text);
            //2、插入文件库
            //StorageImage storageImage = storageImageBll.insert(text, url);


            //3、插入控件库
            // dControl.url = url;
            dControl = dControlBll.insert(dControl);
            //插入相册图片列表  
            List<TurnPictureImagesDto> list = new List<TurnPictureImagesDto>();
            TurnPictureImagesDto dto = new TurnPictureImagesDto();
            dto.dControlId = dControl.id;
            dto.storageImageId = 0;
            list.Add(dto);
            list.Add(dto);
            list.Add(dto);
            list.Add(dto);
            list.Add(dto);
            list.Add(dto);
            int rows = turnPictureImagesBll.insert(list);
            //4、插入页面  
            insertTurnPictureToPage(dControl);

        }
        /*
        * 5 右击-插入流动相册
        */
        public void insertMarqueClick(object sender, RoutedEventArgs e)
        {
            DControl dControl = new DControl();
            dControl.pageId = App.localStorage.currPageId; /////////
            dControl.name = "Marque";
            dControl.content = "";
            dControl.width = 1600;
            dControl.height = 220;
            dControl.left = (int)rightClickLocation.X;/////////
            dControl.top = (int)rightClickLocation.Y;/////////
            dControl.type = "Marque";
            // dControl.url = "";//////////
            // dControl.imgs = "/myfile/sysimg/TurnPicture/1.jpg,/myfile/sysimg/TurnPicture/2.jpg,/myfile/sysimg/TurnPicture/3.jpg,/myfile/sysimg/TurnPicture/4.jpg,/myfile/sysimg/TurnPicture/5.jpg";
            dControl.idx = 1;////////
            dControl.autoplay = true;
            dControl.loop = true;
            dControl.turnPictureSpeed = 1000;  //流动速度
            dControl.rowNum = 5;  //一行显示几个
            dControl.spacing = 20;  //两图间距  
            dControl.isShowTurnPictureArrow = true;

            //1、复制文件
            //String url = this.uploadImage(text);
            //2、插入文件库
            //StorageImage storageImage = storageImageBll.insert(text, url);


            //3、插入控件库
            // dControl.url = url;
            dControl = dControlBll.insert(dControl);
            //插入相册图片列表  
            List<TurnPictureImagesDto> list = new List<TurnPictureImagesDto>();
            TurnPictureImagesDto dto = new TurnPictureImagesDto();
            dto.dControlId = dControl.id;
            dto.storageImageId = 0;
            list.Add(dto);
            list.Add(dto);
            list.Add(dto);
            list.Add(dto);
            list.Add(dto);
            list.Add(dto);
            int rows = turnPictureImagesBll.insert(list);
            //4、插入页面  
            insertMarqueToPage(dControl);

        }
        /*
      * 6 右击-插入层叠相册
      */
        public void insertMarqueLayerClick(object sender, RoutedEventArgs e)
        {
            DControl dControl = new DControl();
            dControl.pageId = App.localStorage.currPageId; /////////
            dControl.name = "MarqueLayer";
            dControl.content = "";
            dControl.width = 1260;
            dControl.height = 450;
            dControl.left = (int)rightClickLocation.X;/////////
            dControl.top = (int)rightClickLocation.Y;/////////
            dControl.type = "MarqueLayer";
            // dControl.url = "";//////////
            // dControl.imgs = "/myfile/sysimg/TurnPicture/1.jpg,/myfile/sysimg/TurnPicture/2.jpg,/myfile/sysimg/TurnPicture/3.jpg,/myfile/sysimg/TurnPicture/4.jpg,/myfile/sysimg/TurnPicture/5.jpg";
            dControl.idx = 1;////////
            dControl.autoplay = true;
            dControl.loop = true;
            dControl.turnPictureSpeed = 6000;  //流动速度
            dControl.rowNum = 5;  //一行显示几个
            dControl.spacing = 20;  //两图间距  
            dControl.isShowTurnPictureArrow = true;


            //1、复制文件
            //String url = this.uploadImage(text);
            //2、插入文件库
            //StorageImage storageImage = storageImageBll.insert(text, url);


            //3、插入控件库
            // dControl.url = url;
            dControl = dControlBll.insert(dControl);
            //插入相册图片列表  
            List<TurnPictureImagesDto> list = new List<TurnPictureImagesDto>();
            TurnPictureImagesDto dto = new TurnPictureImagesDto();
            dto.dControlId = dControl.id;
            dto.storageImageId = 0;
            list.Add(dto);
            list.Add(dto);
            list.Add(dto);
            list.Add(dto);
            list.Add(dto);
            list.Add(dto);
            int rows = turnPictureImagesBll.insert(list);
            //4、插入页面  
            insertMarqueLayerToPage(dControl);

        }

        /*
        * 7 右击 - 插入视频
        */
        public void insertVideoClick(object sender, RoutedEventArgs e)
        {
            DControl dControl = new DControl();
            dControl.pageId = App.localStorage.currPageId; /////////
            dControl.name = "Video";
            dControl.content = "";
            dControl.width = 500;
            dControl.height = 400;
            dControl.left = (int)rightClickLocation.X;/////////
            dControl.top = (int)rightClickLocation.Y;/////////
            dControl.type = "Video";
            dControl.idx = 1;////////
            dControl.autoplay = false;
            dControl.loop = true;
            dControl.storageId = 0;



            //1、复制文件
            //String url = this.uploadImage(text);
            //2、插入文件库
            //StorageImage storageImage = storageImageBll.insert(text, url);
            //3、插入控件库
            // dControl.url = url;
            dControl = dControlBll.insert(dControl);
            //4、插入页面  
            insertVideoToPage(dControl);
        }

        /*
         * 8 插入小窗口
         */
        public void insertCFrameClick(object sender, RoutedEventArgs e)
        {
            DControl dControl = new DControl();
            dControl.pageId = App.localStorage.currPageId; /////////
            dControl.name = "CFrame";
            dControl.content = "小窗口";
            dControl.width = 800;
            dControl.height = 600;
            dControl.left = (int)rightClickLocation.X;/////////
            dControl.top = (int)rightClickLocation.Y;/////////
            dControl.type = "CFrame";
            dControl.linkToPageId = 0;  //首页

            dControl.idx = 1;////////  




            //3、插入控件库
            dControl = dControlBll.insert(dControl);
            dControl.content = dControl.content + dControl.id;
            dControlBll.update(dControl);
            //4、插入页面  
            insertCFrameToPage(dControl);

        }

        /*
         * 9插入Gif 
         */
        public void insertGifClick(object sender, RoutedEventArgs e)
        {
            DControl dControl = new DControl();
            dControl.pageId = App.localStorage.currPageId; /////////
            dControl.name = "Gif";
            dControl.content = "";
            dControl.width = 275;
            dControl.height = 200;
            dControl.left = (int)rightClickLocation.X;/////////
            dControl.top = (int)rightClickLocation.Y;/////////
            dControl.type = "Gif";
            dControl.idx = 1;
            dControl.storageId = 0;


            //3、插入控件库 
            dControl = dControlBll.insert(dControl);

            //4、插入页面  
            insertGifToPage(dControl);

        }

        /*
         * 10插入日期
         */

        public void insertCCalendarClick(object sender, RoutedEventArgs e)
        {
            DControl dControl = new DControl();
            dControl.pageId = App.localStorage.currPageId; /////////
            dControl.name = "CCalendar";
            dControl.content = "yyyy年MM月dd日\n H时mm分ss秒\nweek";
            dControl.width = 436;
            dControl.height = 160;
            dControl.left = (int)rightClickLocation.X;/////////
            dControl.top = (int)rightClickLocation.Y;/////////
            dControl.type = "CCalendar";
            //  dControl.url = "/myfile/sysimg/ico-back-button.png";//////////
            //   dControl.imgs = "";
            dControl.idx = 1;//////// 
            dControl.storageId = 0;
            dControl.fontSize = 36;
            dControl.fontColor = "000080";
            dControl.fontLineHeight = 60;
            dControl.fontWeight = "Bold";
            dControl.fontFamily = "宋体";
            dControl.fontTextAlignment = 1;



            //1、复制文件
            //String url = this.uploadImage(text);
            //2、插入文件库
            //StorageImage storageImage = storageImageBll.insert(text, url);
            //3、插入控件库
            // dControl.url = url;
            dControl = dControlBll.insert(dControl);
            //4、插入页面   
            insertCCalendarToPage(dControl);
        }
        /*
        * 10插入日期
        */

        public void insertCAudioClick(object sender, RoutedEventArgs e)
        {
            DControl dControl = new DControl();
            dControl.pageId = App.localStorage.currPageId; /////////
            dControl.name = "CAudio";
            dControl.content = "";
            dControl.width = 80;
            dControl.height = 80;
            dControl.left = (int)rightClickLocation.X;/////////
            dControl.top = (int)rightClickLocation.Y;/////////
            dControl.type = "CAudio";
            dControl.idx = 1;//////// 
            dControl.storageId = 0;
            dControl.autoplay = true;
            dControl.loop = false;



            //1、复制文件
            //String url = this.uploadImage(text);
            //2、插入文件库
            //StorageImage storageImage = storageImageBll.insert(text, url);
            //3、插入控件库
            // dControl.url = url;
            dControl = dControlBll.insert(dControl);
            //4、插入页面   
            insertCAudioToPage(dControl);
        }

        /*
         * 11 插入返回按钮
         */

        public void insertBackButtonClick(object sender, RoutedEventArgs e)
        {
            DControl dControl = new DControl();
            dControl.pageId = App.localStorage.currPageId; /////////
            dControl.name = "BackButton";
            dControl.content = "";
            dControl.width = 167;
            dControl.height = 48;
            dControl.left = (int)rightClickLocation.X;/////////
            dControl.top = (int)rightClickLocation.Y;/////////
            dControl.type = "BackButton";
            //  dControl.url = "/myfile/sysimg/ico-back-button.png";//////////
            //   dControl.imgs = "";
            dControl.idx = 1;//////// 
            dControl.storageId = 0;


            //1、复制文件
            //String url = this.uploadImage(text);
            //2、插入文件库
            //StorageImage storageImage = storageImageBll.insert(text, url);
            //3、插入控件库
            // dControl.url = url;
            dControl = dControlBll.insert(dControl);
            //4、插入页面   
            insertBackButtonToPage(dControl);
        }

        /*
         * 12插入首页 
         */
        public void insertHomeButtonClick(object sender, RoutedEventArgs e)
        {
            DControl dControl = new DControl();
            dControl.pageId = App.localStorage.currPageId; /////////
            dControl.name = "HomeButton";
            dControl.content = "";
            dControl.width = 167;
            dControl.height = 48;
            dControl.left = (int)rightClickLocation.X;/////////
            dControl.top = (int)rightClickLocation.Y;/////////
            dControl.type = "HomeButton";
            // dControl.url = "/myfile/sysimg/ico-home-button.png";//////////
            //  dControl.imgs = "";
            dControl.idx = 1;//////// 
            dControl.storageId = 0;


            //1、复制文件
            //String url = this.uploadImage(text);
            //2、插入文件库
            //StorageImage storageImage = storageImageBll.insert(text, url);
            //3、插入控件库
            // dControl.url = url;
            dControl = dControlBll.insert(dControl);
            //4、插入页面   
            insertHomeButtonToPage(dControl);
        }

        /*
       * 13 右击 - 设置背景
       */
        public void editBackgroundImageClick(object sender, RoutedEventArgs e)
        {
            PageTemplate pageTemplate = (PageTemplate)mainFrame.Content;
            EditBackgroundImageWindow win = new EditBackgroundImageWindow(pageTemplate, pageTemplate.dPage, this);
            win.ShowDialog();
        }

        /*
       * 14右击 - 设置视频背景
       */
        public void editCVideoBackgroundClick(object sender, RoutedEventArgs e)
        {
            PageTemplate pageTemplate = (PageTemplate)mainFrame.Content;
            EditCVideoBackgroundWindow win = new EditCVideoBackgroundWindow(pageTemplate, pageTemplate.dPage, this);
            win.ShowDialog();
        }

        /*
       * 15 粘贴复制的控件
       */
        public void pastControlClick(object sender, RoutedEventArgs e)
        {
            if (App.localStorage.currCopiedEle != null)
            {
                DControl dControl = (DControl)App.localStorage.currCopiedEle.Tag;
                DControl newDControl = DControlUtil.createFrom(dControl);
                newDControl.pageId = pageTemplate.dPage.id;
                newDControl.left = (int)rightClickLocation.X;
                newDControl.top = (int)rightClickLocation.Y;
                newDControl.parentId = 0;

                if (dControl.type == "CFrame")
                {
                    Boolean result = dControlBll.isNestedOfCurrPageId(dControl.linkToPageId, pageTemplate.dPage.id);
                    if (result)
                    {
                        MessageBox.Show("此控件，嵌套了当前页面，不可粘贴");
                        return;
                    }
                }



                dControlBll.pasteDControl(dControl, newDControl, pageTemplate.dPage);

                //4、插入页面
                insertOneControl(newDControl);
            }
        }
    }
}
