using System;

namespace Dal
{
    public class DataType
    {

        public static Int32 ToInt32(string str)
        {
            Int32 result = 0;
            try
            {
                result = Int32.Parse(str);
            }
            catch (Exception)
            {

            }

            return result;

        }
    }
}
