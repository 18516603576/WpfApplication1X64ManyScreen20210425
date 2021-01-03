using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Common.util
{
    public class FontUtil
    {

        /*
         * 设置TextBox字体大小 
         */
        public static int getFontSize(int val, int defaultVal = 24)
        {
            int result = defaultVal;
            if (val > 0)
            {
                result = val;
            }
            return result;
        }

        /*
         * 获取字体
         */
        public static FontFamily getFontFamily(string val, string defalutVal = "宋体")
        {
            FontFamily result = new FontFamily(defalutVal);
            if (!string.IsNullOrWhiteSpace(val))
            {
                try
                {
                    result = new FontFamily(val);
                }
                catch (ArgumentNullException ex)
                {
                    Console.WriteLine("设置字体异常：" + ex.Message);
                }
            }
            return result;
        }

        /*
         * 设置TextBox颜色
        */
        public static SolidColorBrush getFontColor(string val, string defaultVal = "000000")
        {
            SolidColorBrush result = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#" + defaultVal));
            if (!string.IsNullOrWhiteSpace(val))
            {
                try
                {
                    result = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#" + val));
                }
                catch (Exception ex)
                {
                    Console.WriteLine("设置文字颜色异常：" + ex.Message);
                }
            }

            return result;
        }

        /*
        * 设置TextBox粗细
        */
        public static FontWeight getFontWeight(string val)
        {
            FontWeight result = FontWeights.Normal;
            if (val == "Bold")
            {
                result = FontWeights.Bold;
            }
            return result;
        }

        /*
         * 设置TextBox行高
        */
        public static int getFontLineHeight(int val, int defaultVal = 36)
        {
            int result = defaultVal;
            if (val > 0)
            {
                result = val;
            }
            return result;
        }




        /*
         * 获取对齐方式
         */
        public static TextAlignment getFontTextAlignment(int val)
        {
            TextAlignment result = TextAlignment.Left;
            if (val == 0)
            {
                result = TextAlignment.Left;
            }
            else if (val == 1)
            {
                result = TextAlignment.Right;
            }
            else if (val == 2)
            {
                result = TextAlignment.Center;
            }
            else if (val == 3)
            {
                result = TextAlignment.Justify;
            }
            return result;
        }



        /*
         * 设置TextBox粗细
         */
        public static void setFontWeight(TextBlock textBox, string val)
        {
            if (val == "Bold")
            {
                textBox.FontWeight = FontWeights.Bold;
            }
            else
            {
                textBox.FontWeight = FontWeights.Normal;
            }
        }

        /*
       * 设置TextBox颜色
       */
        public static void setFontColor(TextBlock textBox, string val, string defaultVal = "000000")
        {
            SolidColorBrush brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#" + defaultVal));
            if (!string.IsNullOrWhiteSpace(val))
            {
                brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#" + val));
            }
            textBox.Foreground = brush;
        }

        /*
       * 设置TextBox字体大小
       */
        public static void setFontSize(TextBlock textBox, int val)
        {
            if (val > 0)
            {
                textBox.FontSize = val;
            }
            else
            {
                textBox.FontSize = 24;
            }
        }

        /*
        * 设置TextBox字体大小
        */
        public static void setFontFamily(TextBlock textBox, string val)
        {
            string tmp = "宋体";
            if (!string.IsNullOrWhiteSpace(val))
            {
                tmp = val;
            }
            try
            {
                textBox.FontFamily = new FontFamily(tmp);
            }
            catch (ArgumentNullException ex1)
            {
                Console.WriteLine("设置字体异常：" + ex1.Message);
            }

        }


        /*
     * 设置TextBox行高
     */
        public static void setFontLineHeight(TextBlock textBox, int val)
        {
            int tmp = 24;
            if (val > 0)
            {
                tmp = val;
            }
            try
            {
                TextBlock.SetLineHeight(textBox, val);
            }
            catch (ArgumentNullException ex1)
            {
                Console.WriteLine("设置字体行高：" + ex1.Message);
            }
            catch (ArgumentException ex1)
            {
                Console.WriteLine("设置字体行高：" + ex1.Message);
            }
        }



    }
}
