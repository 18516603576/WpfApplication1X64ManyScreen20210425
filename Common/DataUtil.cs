using System;
using System.Text.RegularExpressions;

namespace Common
{
    public class DataUtil
    {
        /*
         * 是否为整数
         */
        public static Boolean isInt(string value)
        {
            return Regex.IsMatch(value, @"^\d*$");
        }
        /*
        * 字符串是否是double
        */
        public static bool IsDouble(string value)
        {
            return Regex.IsMatch(value, @"^\d+(\.\d+)?$");
        }


        public static Int32 ToInt(string str)
        {
            Int32 result = 0;
            try
            {
                result = Int32.Parse(str);
            }
            catch (Exception ex)
            {

            }

            return result;

        }

        /*
         * 字符串转double
         */
        public static double ToDouble(string str)
        {
            double result = 0;
            try
            {
                result = Double.Parse(str);
            }
            catch (Exception)
            {

            }
            return result;
        }
    }
}
