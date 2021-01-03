using Bll;
using Common;
using Common.Data;
using Common.util;
using Model;
using System;

namespace ShowBox.manage
{
    public class RegChecking
    {

        private readonly Cfg2Bll cfg2Bll = new Cfg2Bll();
        public RegChecking()
        {

        }

        /*
         * 验证是否注册
         * 
         * errorCode=1001,提示用户，点击关闭软件（需要注册）
         * 
         * errorCode=1002,提示用户，可以继续试用（非静默试用）
         * 
         * errorCode=1003,不提示用户，继续试用(静默试用)
         * 
         */
        public BaseResult isReg()
        {
            BaseResult baseResult = new BaseResult();
            baseResult.result = false;
            baseResult.errorCode = 1001;
            baseResult.message = "未知";

            //1.获取数据库信息,并验证数据完整性
            Cfg2 cfg2 = cfg2Bll.get(1);
            string machineCode = MachineCodeUtil.GetMachineCodeString();

            //3.注册码是否存在，存在则验证
            if (!string.IsNullOrWhiteSpace(cfg2.rg3))
            {
                if (RegCodeUtil.isRightOfRegCode(cfg2.rg3, machineCode))
                {
                    baseResult.result = true;
                    baseResult.errorCode = 0;
                    baseResult.message = "注册码正确";
                    return baseResult;
                }
                else
                {
                    baseResult.result = false;
                    baseResult.errorCode = 1001;
                    baseResult.message = "注册码无效";
                    return baseResult;
                }
            }

            //4.验证数据库注册日期是否存在，不存在则填写 
            if (!string.IsNullOrWhiteSpace(cfg2.sd))
            {
                if (cfg2.sd == "sgfdo_fdgrexfhtreKUFSSA")
                {
                    //第一次打开软件
                    TimeSpan ts1 = DateTime.Now - DateTime.Parse("1970-1-1");
                    int regDay = (int)Math.Floor(ts1.TotalDays);
                    cfg2.sd = EncryptionUtil.encode(regDay.ToString(), machineCode);
                    cfg2.cd = cfg2.sd;
                    if (string.IsNullOrWhiteSpace(cfg2.ld))
                    {
                        int limitDay = regDay + 30;
                        cfg2.ld = EncryptionUtil.encode(limitDay.ToString(), machineCode);
                    }

                    string validateCode1 = RegCodeUtil.generateValidateCode(cfg2, machineCode);
                    cfg2.validateCode = validateCode1;
                    cfg2 = cfg2Bll.update(cfg2);
                    baseResult.result = false;
                    baseResult.errorCode = 1003;
                    baseResult.message = "第一次打开软件";
                    return baseResult;
                }
            }
            else
            {
                baseResult.result = false;
                baseResult.errorCode = 1001;
                baseResult.message = "注册日期异常（空），试用结束";
                return baseResult;
            }

            //2.数据完整性校验
            if (RegCodeUtil.validate(cfg2, machineCode) == false)
            {
                baseResult.result = false;
                baseResult.errorCode = 1001;
                baseResult.message = "数据完整性校验失败";
                return baseResult;
            }


            //5.1 获取开始时间
            string sdStr = EncryptionUtil.decode(cfg2.sd, machineCode);
            if (!DataUtil.isInt(sdStr))
            {
                baseResult.result = false;
                baseResult.errorCode = 1001;
                baseResult.message = "注册日期异常（非整数），试用结束";
                return baseResult;
            }
            else if (Int32.Parse(sdStr) < 0)
            {
                baseResult.result = false;
                baseResult.errorCode = 1001;
                baseResult.message = "注册日期异常（小于0），试用结束";
                return baseResult;
            }
            int sd = DataUtil.ToInt(sdStr);

            //5.2获取数据库中截止日期
            string ldStr = EncryptionUtil.decode(cfg2.ld, machineCode);
            int ld = DataUtil.ToInt(ldStr);

            //5.3获取数据库中当前时间
            string cdStr_old = EncryptionUtil.decode(cfg2.cd, machineCode);
            int cd_old = DataUtil.ToInt(cdStr_old);

            //5.4 获取当前时间，并比较
            TimeSpan ts2 = DateTime.Now - DateTime.Parse("1970-1-1");
            int cd = (int)Math.Floor(ts2.TotalDays);
            if (cd_old > cd)
            {
                baseResult.result = false;
                baseResult.errorCode = 1001;
                baseResult.message = "计算机时间异常，试用结束";
                return baseResult;
            }


            //6.注册日期>截止日期，试用结束
            if (cd > ld)
            {
                baseResult.result = false;
                baseResult.errorCode = 1001;
                baseResult.message = "试用结束";
                return baseResult;
            }


            //7. 更新当前日期 
            cfg2.cd = EncryptionUtil.encode(cd.ToString(), machineCode);
            string validateCode2 = RegCodeUtil.generateValidateCode(cfg2, machineCode);
            cfg2.validateCode = validateCode2;
            cfg2 = cfg2Bll.update(cfg2);

            //8. 剩余试用天数，当restDays<10,则弹窗提示
            int restDays = ld - cd;
            if (restDays < 10)
            {
                baseResult.result = false;
                baseResult.errorCode = 1002;
                baseResult.message = restDays + " 天后，试用结束";
                return baseResult;
            }
            baseResult.result = false;
            baseResult.errorCode = 1003;
            baseResult.message = restDays + " 天后，试用结束";
            return baseResult;


            //baseResult.result = false;
            //baseResult.errorCode = 1002;
            //baseResult.message = restDays + " 天后，试用结束";
            //return baseResult;
        }



    }
}
