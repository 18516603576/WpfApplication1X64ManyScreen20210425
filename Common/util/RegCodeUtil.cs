using Common.Data;
using Model;
using System;

namespace Common.util
{
    public class RegCodeUtil
    {


        /*
         * 生成注册码
         */
        public static string generate(string machineCode)
        {
            string code1 = machineCode + Params.salt1;
            string code2 = Params.salt2 + machineCode;

            string md51 = Md5Util.generate(code1);
            string md52 = Md5Util.generate(code2);
            string md53 = Md5Util.generate(md51 + md52);
            return md53;
        }

        /*
         * 生成有效期注册码
         */
        public static string generateWithLimitDay(int day, string machineCode)
        {
            string code1 = machineCode + Params.salt1 + day;
            string code2 = Params.salt2 + machineCode;

            string md51 = Md5Util.generate(code1);
            string md52 = Md5Util.generate(code2);
            string md53 = Md5Util.generate(md51 + md52);
            return md53;
        }

        /*
         * 注册码是否正确
        */
        public static bool isRightOfRegCode(string regCode, string machineCode)
        {
            if (string.IsNullOrWhiteSpace(regCode)) return false;
            if (string.IsNullOrWhiteSpace(machineCode)) return false;
            string rightRegCode = generate(machineCode);

            if (rightRegCode != regCode) return false;
            return true;
        }

        /*
        * 是否为有效期注册码
        * 
        * <=0 ,为否
        * 
        * >0 ,返回截止日期
        */
        public static int isRightOfLimitDayRegCode(string regCode, string machineCode)
        {

            if (string.IsNullOrWhiteSpace(regCode)) return 0;
            if (string.IsNullOrWhiteSpace(machineCode)) return 0;


            TimeSpan ts1 = DateTime.Now - DateTime.Parse("1970-1-1");
            int currDay = (int)Math.Floor(ts1.TotalDays);


            int ld = 0;
            for (int i = 1; i < 90; i++)
            {
                int x = currDay + i;
                string tmp = generateWithLimitDay(x, machineCode);
                if (regCode == tmp)
                {
                    ld = x;
                    break;
                }
            }

            return ld;
        }



        /*
         * 数据完整性校验
         */
        public static Boolean validate(Cfg2 cfg2, string machineCode)
        {
            string rightVaidateCode = generateValidateCode(cfg2, machineCode);
            if (cfg2.validateCode == rightVaidateCode)
            {
                return true;
            }

            return false;
        }


        /*
         * 生成校验码
         */
        public static string generateValidateCode(Cfg2 cfg2, string machineCode)
        {
            string AppSecret = Md5Util.generate(Params.salt1 + machineCode + Params.salt2);
            string str = cfg2.sd;
            str = str + "&" + cfg2.cd;
            str = str + "&" + cfg2.ld;
            str = str + "&" + cfg2.rg1;
            str = str + "&" + cfg2.rg3;
            str = str + "&" + AppSecret;

            return Md5Util.generate(str);
        }
    }
}
