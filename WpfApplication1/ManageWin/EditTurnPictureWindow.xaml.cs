using Bll;
using Common;
using Common.control;
using Common.Data;
using Common.util;
using Model;
using Model.dto;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WpfApplication1.manage;

namespace WpfApplication1.ManageWin
{
    /// <summary>
    /// EditTurnPictureWindow.xaml 的交互逻辑
    /// </summary>
    public partial class EditTurnPictureWindow : Window
    {
        private readonly StorageImageBll storageImageBll = new StorageImageBll();
        private readonly TurnPictureImagesBll turnPictureImagesBll = new TurnPictureImagesBll();
        private readonly DControlBll dControlBll = new DControlBll();
        //默认图片背景
        private readonly string defaultBg = "/myfile/sysimg/ico-add-image.png";
        private readonly FrameworkElement currElement;
        private readonly DControl currDControl;
        private readonly Editing editing;
        public EditTurnPictureWindow(Editing editing, FrameworkElement currElement)
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            //  string imgs = "/myfile/upload/image/20200403/9a5d403792444230ac7aa853e34f3333/1-1Z521142300c1.jpg,/myfile/upload/image/20200403/d3964390c52641b0a35b34d6de6ec10a/1.png,/myfile/upload/image/20200403/9b2d0d40469d47fea6d3cc761f842b9f/2.png";
            //TurnPicture turnPicture = (TurnPicture) currentEle;
            this.editing = editing;
            this.currElement = currElement;
            currDControl = (DControl)currElement.Tag;
            List<TurnPictureImagesDto> list = turnPictureImagesBll.getByDControlId(currDControl.id);
            for (int i = 0; i < list.Count; i++)
            {
                TurnPictureImagesDto dto = list[i];
                if (dto == null) { list.Remove(dto); }
                string imgNotExists = Params.ImageNotExists;
                if (i < 6 && dto.storageImageId == 0)
                {
                    imgNotExists = Params.TurnPictureNotExists[i];
                }
                string imgFullPath = FileUtil.notExistsShowDefault(dto.url, imgNotExists);
                dto.url = imgFullPath;
            }
            init(list);
        }

        private void init(List<TurnPictureImagesDto> list)
        {


            foreach (TurnPictureImagesDto dto in list)
            {
                Canvas canvas = initOneImage(dto);
                imgList.Children.Add(canvas);
                // imgList.Children.Insert(0, canvas);
            }

            Canvas emptycanvas = initOneImage(null);
            imgList.Children.Add(emptycanvas);


        }

        /*
         * 初始化一个图片控件
         */
        private Canvas initOneImage(TurnPictureImagesDto dto)
        {

            Canvas imgCanvas = new Canvas();
            imgCanvas.Name = "imgCanvas";
            imgCanvas.Width = 140;
            imgCanvas.Height = 140;
            imgCanvas.Margin = new Thickness(4, 10, 4, 10); 
            imgCanvas.Tag = dto;

            //1.按钮 
            string imgPath = (dto == null ? App.localStorage.icoAddImage : dto.url);
            string imgFullPath = FileUtil.notExistsShowDefault(imgPath, Params.ImageNotExists);
            imgFullPath = AppDomain.CurrentDomain.BaseDirectory + imgFullPath;
            Button btn = new Button();
            btn.Name = "imgBtn";
            btn.Width = 140;
            btn.Height = 140;
            btn.Background = new ImageBrush
            {
                ImageSource = FileUtil.readImage2(imgFullPath, 200)
                ,
                Stretch = Stretch.Uniform
            };
            btn.Click += openDialogClick;

            //2.按钮行
            Canvas bg = new Canvas();
            bg.Name = "bg";
            bg.Background = Brushes.Black;
            bg.Width = 140;
            bg.Height = 24;
            bg.Opacity = 0.6;
            bg.SetValue(Canvas.BottomProperty, 0.0);
            bg.SetValue(Canvas.LeftProperty, 0.0);
            bg.Visibility = Visibility.Visible;
            if (dto == null)
            {
                bg.Visibility = Visibility.Collapsed;
            }


            Button lbtn = new Button();
            lbtn.Width = 20;
            lbtn.Height = 20;
            lbtn.BorderThickness = new Thickness(0);
            lbtn.Background = new ImageBrush
            {
                ImageSource = FileUtil.readImage(AppDomain.CurrentDomain.BaseDirectory + "/myfile/sysimg/ico-index-left.png")
              ,
                Stretch = Stretch.Uniform
            };
            lbtn.SetValue(Canvas.LeftProperty, 0.0);
            lbtn.SetValue(Canvas.TopProperty, 2.0);
            bg.Children.Add(lbtn);
            lbtn.Click += lbtnClick;


            Button rbtn = new Button();
            rbtn.Width = 20;
            rbtn.Height = 20;
            rbtn.BorderThickness = new Thickness(0);
            rbtn.Background = new ImageBrush
            {
                ImageSource = FileUtil.readImage(AppDomain.CurrentDomain.BaseDirectory + "/myfile/sysimg/ico-index-right.png")
                ,
                Stretch = Stretch.Uniform
            };
            rbtn.SetValue(Canvas.RightProperty, 0.0);
            rbtn.SetValue(Canvas.TopProperty, 2.0);
            bg.Children.Add(rbtn);
            rbtn.Click += rbtnClick;


            Button removeBtn = new Button();
            removeBtn.Width = 20;
            removeBtn.Height = 20;
            removeBtn.BorderThickness = new Thickness(0);
            removeBtn.Background = new ImageBrush
            {
                ImageSource = FileUtil.readImage(AppDomain.CurrentDomain.BaseDirectory + "/myfile/sysimg/ico-image-remove.png")
                ,
                Stretch = Stretch.Uniform
            };
            removeBtn.SetValue(Canvas.LeftProperty, 40.0);
            removeBtn.SetValue(Canvas.TopProperty, 2.0);
            bg.Children.Add(removeBtn);
            removeBtn.Click += removeBtnClick;


            Button linkBtn = new Button();
            linkBtn.Width = 20;
            linkBtn.Height = 20;
            linkBtn.BorderThickness = new Thickness(0);
            if (dto != null)
            {
                if (dto.isLink)
                {
                    linkBtn.Background = new ImageBrush
                    {
                        ImageSource = FileUtil.readImage(AppDomain.CurrentDomain.BaseDirectory + "/myfile/sysimg/ico-image-link-active.png")
                                          ,
                        Stretch = Stretch.Uniform
                    };
                }
                else
                {
                    linkBtn.Background = new ImageBrush
                    {
                        ImageSource = FileUtil.readImage(AppDomain.CurrentDomain.BaseDirectory + "/myfile/sysimg/ico-image-link.png")
                                    ,
                        Stretch = Stretch.Uniform
                    }; 
                }
            }
            linkBtn.SetValue(Canvas.LeftProperty, 80.0);
            linkBtn.SetValue(Canvas.TopProperty, 2.0);
            bg.Children.Add(linkBtn);
            linkBtn.Click += linkBtnClick;
             

            imgCanvas.Children.Add(btn);
            imgCanvas.Children.Add(bg); 

            return imgCanvas;
        }

        /*
         * 清空控件中的图片
         */
        private void removeBtnClick(object sender, RoutedEventArgs e)
        {
            Button removeBtn = (Button)sender;
            Canvas bg = (Canvas)VisualTreeHelper.GetParent(removeBtn);
            Canvas imgCanvas = (Canvas)VisualTreeHelper.GetParent(bg);
            object tag = imgCanvas.Tag;
            if (tag == null) return;
            TurnPictureImagesDto dto = (TurnPictureImagesDto)tag;
            dto.url = "";
            dto.storageImageId = 0;
            imgCanvas.Tag = dto;
            Button imgBtn = (Button)FrameworkElementUtil.getByName(imgCanvas, "imgBtn");
            imgBtn.Background = new ImageBrush
            {
                ImageSource = FileUtil.readImage(AppDomain.CurrentDomain.BaseDirectory + defaultBg)
                ,
                Stretch = Stretch.Uniform
            };
        }
        /*
        * 图片链接到
        */
        private void linkBtnClick(object sender, RoutedEventArgs e)
        {
            Button linkBtn = (Button)sender;
            Canvas bg = (Canvas)VisualTreeHelper.GetParent(linkBtn);
            Canvas imgCanvas = (Canvas)VisualTreeHelper.GetParent(bg);
            TurnPictureImagesDto dto = (TurnPictureImagesDto)imgCanvas.Tag;

            EditTurnPictureLinkToWindow win = new EditTurnPictureLinkToWindow(linkBtn, dto);
            win.ShowDialog();
        }
        /*
         * 向右移动
         */
        private void rbtnClick(object sender, RoutedEventArgs e)
        {
            Button rbtn = (Button)sender;
            Canvas bg = (Canvas)VisualTreeHelper.GetParent(rbtn);
            Canvas canvas = (Canvas)VisualTreeHelper.GetParent(bg);
            //WrapPanel imgList = (WrapPanel)VisualTreeHelper.GetParent(canvas);

            int count = imgList.Children.Count;
            int idx = imgList.Children.IndexOf(canvas);
            if (idx < count - 1)
            {
                imgList.Children.Remove(canvas);
                imgList.Children.Insert(idx + 1, canvas);
            }
        }

        /*
         * 向左移动
         */
        private void lbtnClick(object sender, RoutedEventArgs e)
        {
            Button lbtn = (Button)sender;
            Canvas bg = (Canvas)VisualTreeHelper.GetParent(lbtn);
            Canvas canvas = (Canvas)VisualTreeHelper.GetParent(bg);
            //WrapPanel imgList = (WrapPanel)VisualTreeHelper.GetParent(canvas);

            int idx = imgList.Children.IndexOf(canvas);
            if (idx >= 1)
            {
                imgList.Children.Remove(canvas);
                imgList.Children.Insert(idx - 1, canvas);
            }


        }
         

        /*
         * 点击打开上传图片
         */
        private void openDialogClick(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;

            Canvas imgCanvas = (Canvas)VisualTreeHelper.GetParent(btn);
            int idx = imgList.Children.IndexOf(imgCanvas);
            object tag = imgCanvas.Tag;


            System.Windows.Forms.OpenFileDialog ofld = new System.Windows.Forms.OpenFileDialog();
            ofld.Filter = "图片|*.jpg;*.jpeg;*.png;*.gif;*.bmp";
            ofld.Multiselect = true;
            if (ofld.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                int i = 0;


                string[] fileNames = ofld.FileNames;
                foreach (string filename in fileNames)
                {
                    if (filename != "" || filename != null)
                    {
                        i = i + 1;
                        System.Drawing.Image sourceImage = System.Drawing.Image.FromFile(filename);
                        String url = uploadImage(btn, filename);
                        StorageImage storageImage = storageImageBll.insert(filename, url, sourceImage.Width, sourceImage.Height, 1);

                        //1.如果i>1，则直接在其后插入新的
                        if (i > 1)
                        {

                            TurnPictureImagesDto dto = new TurnPictureImagesDto();
                            dto.storageImageId = storageImage.id;
                            dto.dControlId = currDControl.id;
                            dto.url = storageImage.url;
                            TurnPictureImages turnPictureImages = turnPictureImagesBll.insert(dto);
                            dto.id = turnPictureImages.id;

                            Canvas canvas = initOneImage(dto);
                            imgList.Children.Insert(idx, canvas);
                            idx = idx + 1;
                            continue;
                        }
                        //2.如果Tag==null，则在其前面插入新的，
                        if (tag == null)
                        {
                            TurnPictureImagesDto dto = new TurnPictureImagesDto();
                            dto.storageImageId = storageImage.id;
                            dto.dControlId = currDControl.id;
                            dto.url = storageImage.url;
                            TurnPictureImages turnPictureImages = turnPictureImagesBll.insert(dto);
                            dto.id = turnPictureImages.id;
                            Canvas canvas = initOneImage(dto);
                            imgList.Children.Insert(idx, canvas);
                            idx = idx + 1;
                        }
                        //3.直接tag!=null,则更新当前图片
                        else
                        {
                            TurnPictureImagesDto dto = (TurnPictureImagesDto)tag;
                            dto.storageImageId = storageImage.id;
                            dto.dControlId = currDControl.id;
                            dto.url = storageImage.url;
                            imgCanvas.Tag = dto;
                            turnPictureImagesBll.update(dto);
                            btn.Background = new ImageBrush
                            {
                                ImageSource = FileUtil.readImage2(AppDomain.CurrentDomain.BaseDirectory + url, currDControl.width)
                               ,
                                Stretch = Stretch.Uniform
                            };
                        }

                    }

                }



            }
        }
        /*
        * 上传图片，复制的软件目录下
        */
        private String uploadImage(Button btn, string sourceFilePath)
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
         * 保存数据 
         */
        private void Submit_Button_Click(object sender, RoutedEventArgs e)
        {
            //过滤掉空图片
            List<TurnPictureImagesDto> list = new List<TurnPictureImagesDto>();
            foreach (Canvas imgCanvas in imgList.Children)
            {
                object tag = imgCanvas.Tag;
                if (tag == null) continue;
                TurnPictureImagesDto dto = (TurnPictureImagesDto)tag;
                if (dto.storageImageId == 0 && dto.url == "")
                {
                    turnPictureImagesBll.delete(dto.id);
                    continue;
                }
                list.Add(dto);
            }
            //更新到数据库
            turnPictureImagesBll.updateByDControlId(currDControl, list);


            //更新页面控件信息  
            TurnPicture turnPicture = (TurnPicture)currElement;
            turnPicture.updateElement(currDControl, true, list);

            Close();

        }



    }
}
