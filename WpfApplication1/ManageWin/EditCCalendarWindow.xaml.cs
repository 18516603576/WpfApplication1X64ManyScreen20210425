using Bll;
using Common.control;
using Model;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;

namespace WpfApplication1.ManageWin
{
    /// <summary>
    /// EditLinkToWebWindow.xaml 的交互逻辑
    /// </summary>
    public partial class EditCCalendarWindow : Window
    {

        private readonly DPageBll dPageBll = new DPageBll();
        private readonly DControlBll dControlBll = new DControlBll();
        private readonly FrameworkElement currElement;
        private DControl currDControl;

        public EditCCalendarWindow(Frame mainFrame, FrameworkElement currElement)
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.currElement = currElement;
            currDControl = (DControl)currElement.Tag;


            loadPageData();
            Closed += This_Close;
        }

        private void loadPageData()
        {
            content.Text = currDControl.content;
            //如果不是弹窗，则显示选择的页面
            initFontFamily();
            content.Text = currDControl.content;
            fontSize.Text = currDControl.fontSize + "像素";
            fontFamily.Text = currDControl.fontFamily;
            fontLineHeight.Text = currDControl.fontLineHeight + "像素";

            if (string.IsNullOrWhiteSpace(currDControl.fontColor))
            {
                fontColor.Tag = "000000";
                fontColor.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#000000"));
            }
            else
            {
                fontColor.Tag = currDControl.fontColor;
                fontColor.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#" + currDControl.fontColor));
            }

            if (currDControl.fontWeight == "Bold")
            {
                fontWeight.IsChecked = true;
            }
            else
            {
                fontWeight.IsChecked = false;
            }
            initfontTextAlignmentCombox(currDControl.fontTextAlignment);
            fontSize.SelectionChanged += fontSize_SelectionChanged;
            fontFamily.SelectionChanged += fontFamily_SelectionChanged;
            fontLineHeight.SelectionChanged += fontLineHeight_SelectionChanged;
            fontWeight.Click += fontWeight_Checked;
            fontTextAlignment.SelectionChanged += fontTextAlignment_SelectionChanged;
            fontColor.Click += fontColor_Click;
            cfgWeek.Click += cfgWeek_Click;
        }

        private void cfgWeek_Click(object sender, RoutedEventArgs e)
        {
            EditCfgWeekWindow win = new EditCfgWeekWindow();
            win.ShowDialog();
        }

        /*
         * 对齐方式改变，更新到页面
         */
        private void fontTextAlignment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cbb = (ComboBox)sender;
            ComboBoxItem item = (ComboBoxItem)cbb.SelectedItem;
            if (item == null) return;

            object tag = item.Tag;
            int fontTextAlignmentVal = 0;
            if (tag != null)
            {
                fontTextAlignmentVal = Convert.ToInt32(tag);
            }
            CCalendar cCalendar = (CCalendar)currElement;
            cCalendar.updateFontTextAlignment(content.Text, fontTextAlignmentVal);
        }

        /*
     * 初始化文字对齐方式
     */
        private void initfontTextAlignmentCombox(int currFontTextAlignment)
        {
            foreach (ComboBoxItem item in fontTextAlignment.Items)
            {
                int tag = Convert.ToInt32(item.Tag);
                if (tag == currFontTextAlignment)
                {
                    item.IsSelected = true;
                }
            }
        }

        private void This_Close(object sender, EventArgs e)
        {
            //同步到页面 
            currElement.Tag = currDControl;
            CCalendar cCalendar = (CCalendar)currElement;
            cCalendar.updateElement(currDControl, true);
        }

        private void fontWeight_Checked(object sender, RoutedEventArgs e)
        {
            Boolean b = (Boolean)fontWeight.IsChecked;
            string fontWeightVal = "normal";
            if (b)
            {
                fontWeightVal = "Bold";
            }
            CCalendar cCalendar = (CCalendar)currElement;
            cCalendar.updateFontWeightVal(content.Text, fontWeightVal);
        }

        private void fontLineHeight_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cbb = (ComboBox)sender;
            object obj = cbb.SelectedValue;
            if (obj == null) return;

            ComboBoxItem cbi = (ComboBoxItem)obj;
            string fontLineHeightText = cbi.Content.ToString();
            int fontLineHeightVal = 24;
            if (fontLineHeightText != null && fontLineHeightText.EndsWith("像素"))
            {
                fontLineHeightText = fontLineHeightText.Replace("像素", "");
                try
                {
                    fontLineHeightVal = int.Parse(fontLineHeightText);
                }
                catch (Exception) { }
            }

            CCalendar cCalendar = (CCalendar)currElement;
            cCalendar.updateFontLineHeightlyVal(content.Text, fontLineHeightVal);

        }

        private void fontFamily_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cbb = (ComboBox)sender;
            object obj = cbb.SelectedValue;
            if (obj == null) return;

            ComboBoxItem cbi = (ComboBoxItem)obj;
            string fontFamilyText = cbi.Content.ToString();
            string fontFamilyVal = "宋体";
            if (fontFamilyText != null)
            {
                fontFamilyVal = fontFamilyText;
            }

            CCalendar cCalendar = (CCalendar)currElement;
            cCalendar.updateFontFamilyVal(content.Text, fontFamilyVal);
        }


        /*
         * 选择文字大小
         */
        private void fontSize_SelectionChanged(object sender, RoutedEventArgs e)
        {
            ComboBox cbb = (ComboBox)sender;
            object obj = cbb.SelectedValue;
            if (obj == null) return;

            ComboBoxItem cbi = (ComboBoxItem)obj;
            string fontSizeText = cbi.Content.ToString();
            int fontSizeVal = 0;
            if (fontSizeText != null && fontSizeText.EndsWith("像素"))
            {
                fontSizeText = fontSizeText.Replace("像素", "");
                try
                {
                    fontSizeVal = int.Parse(fontSizeText);
                }
                catch (Exception) { }
            }
            CCalendar cCalendar = (CCalendar)currElement;
            cCalendar.updateFontSizeVal(content.Text, fontSizeVal);
        }

        /*
         * 选择颜色
         */
        private void fontColor_TextChanged(string fontColorVal)
        {
            CCalendar cCalendar = (CCalendar)currElement;
            cCalendar.updateFontColorVal(content.Text, fontColorVal);
        }



        private void Submit_Button_Click(object sender, RoutedEventArgs e)
        {
            string fontSizeText = fontSize.Text;
            int fontSizeVal = 24;
            if (fontSizeText != null && fontSizeText.EndsWith("像素"))
            {
                fontSizeText = fontSizeText.Replace("像素", "");
                try
                {
                    fontSizeVal = int.Parse(fontSizeText);
                }
                catch (Exception) { }
            }

            string fontLineHeightText = fontLineHeight.Text;
            int fontLineHeightVal = 24;
            if (fontLineHeightText != null && fontLineHeightText.EndsWith("像素"))
            {
                fontLineHeightText = fontLineHeightText.Replace("像素", "");
                try
                {
                    fontLineHeightVal = int.Parse(fontLineHeightText);
                }
                catch (Exception) { }
            }

            string fontFamilyVal = fontFamily.Text;
            string fontWeightVal = "Normal";
            if ((Boolean)fontWeight.IsChecked)
            {
                fontWeightVal = "Bold";
            }

            object tag = fontColor.Tag;
            string fontColorVal = "ff000000";
            if (tag != null)
            {
                string fontColorText = tag.ToString();
                if (!string.IsNullOrWhiteSpace(fontColorText))
                {
                    fontColorVal = fontColorText;
                }
            }

            int fontTextAlignmentVal = 0;
            ComboBoxItem item = (ComboBoxItem)fontTextAlignment.SelectedItem;
            if (item != null)
            {
                if (item.Tag != null)
                {
                    fontTextAlignmentVal = Convert.ToInt32(item.Tag);
                }
            }

            //更新到数据库
            DControl dControl = dControlBll.get(currDControl.id);
            dControl.content = content.Text;
            dControl.fontFamily = fontFamilyVal;
            dControl.fontSize = fontSizeVal;
            dControl.fontLineHeight = fontLineHeightVal;
            dControl.fontColor = fontColorVal;
            dControl.fontWeight = fontWeightVal;
            dControl.fontTextAlignment = fontTextAlignmentVal;
            dControlBll.update(dControl);
            //同步到页面
            currDControl = dControl;
            currElement.Tag = dControl;


            Close();
        }



        //选择字体颜色
        private void fontColor_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.ColorDialog colorDialog = App.colorDialog;
            //允许使用该对话框的自定义颜色
            colorDialog.AllowFullOpen = true;
            colorDialog.FullOpen = true;
            colorDialog.ShowHelp = true;
            //初始化当前文本框中的字体颜色，
            colorDialog.Color = System.Drawing.Color.Pink;
            //当用户在ColorDialog对话框中点击"取消"按钮
            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.Cancel) return;
            System.Drawing.Color color = colorDialog.Color;
            // string fontColorVal = colorDialog.Color.Name;
            string fontColorVal = toHex(color.R) + toHex(color.G) + toHex(color.B);

            fontColor.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#" + fontColorVal));
            fontColor.Tag = fontColorVal;
            fontColor_TextChanged(fontColorVal);
        }

        public string toHex(int digit)
        {
            string hexDigit = digit.ToString("X");
            if (hexDigit.Length == 1)
            {
                hexDigit = "0" + hexDigit;
            }
            return hexDigit;
        }

        //初始化系统字体列表
        private void initFontFamily()
        {

            List<String> listCn = new List<String>();
            List<String> listEn = new List<String>();
            foreach (FontFamily fontfamily in Fonts.SystemFontFamilies)
            {
                LanguageSpecificStringDictionary fontdics = fontfamily.FamilyNames;
                //判断该字体是不是中文字体
                if (fontdics.ContainsKey(XmlLanguage.GetLanguage("zh-cn")))
                {
                    if (fontdics.TryGetValue(XmlLanguage.GetLanguage("zh-cn"), out string fontfamilyname))
                    {
                        listCn.Add(fontfamilyname);
                    }
                }
                //英文字体
                else
                {
                    if (fontdics.TryGetValue(XmlLanguage.GetLanguage("en-us"), out string fontfamilyname))
                    {
                        listEn.Add(fontfamilyname);
                    }
                }
            }

            foreach (string s in listCn)
            {
                ComboBoxItem child = new ComboBoxItem();
                child.Content = s;
                child.Tag = s;
                //if (dc.id == currDControl.showInWhichCFrame)
                //{
                //    child.IsSelected = true;
                //}
                fontFamily.Items.Add(child);
            }
            foreach (string s in listEn)
            {
                ComboBoxItem child = new ComboBoxItem();
                child.Content = s;
                child.Tag = s;
                //if (dc.id == currDControl.showInWhichCFrame)
                //{
                //    child.IsSelected = true;
                //}
                fontFamily.Items.Add(child);
            }



        }

    }
}
