using System;
using System.Management;

namespace Common.util
{
    public class MachineCodeUtil
    {

        public static MachineCodeUtil machineCodeUtil;

        public static string GetMachineCodeString()
        {
            string machineCodeString = string.Empty;
            if (machineCodeUtil == null)
            {
                machineCodeUtil = new MachineCodeUtil();
            }
            machineCodeString = "PC." + machineCodeUtil.GetCpuInfo()
                               + "." + machineCodeUtil.GetHardId()
                              // + "." + machineCodeUtil.GetMacAddress()
                              ;
            machineCodeString = Md5Util.generate(machineCodeString);
            return machineCodeString;
        }

        ///   <summary> 
        ///   获取cpu序列号     
        ///   </summary> 
        ///   <returns> string </returns>  
        public string GetCpuInfo()
        {
            string cpuInfo = "";
            try
            {
                using (ManagementClass cimobject = new ManagementClass("Win32_Processor"))
                {
                    ManagementObjectCollection moc = cimobject.GetInstances();
                    foreach (ManagementObject mo in moc)
                    {
                        cpuInfo = cpuInfo + mo.Properties["ProcessorId"].Value?.ToString();
                        mo.Dispose();
                        break;
                    }
                }
            }
            catch (Exception)
            {
                //不处理
            }
            return cpuInfo;
        }

        ///   <summary> 
        ///   获取硬盘ID     
        ///   </summary> 
        ///   <returns> string </returns> 
        public string GetHardId()
        {
            string HDid = "";
            try
            {
                using (ManagementClass cimobject1 = new ManagementClass("Win32_DiskDrive"))
                {
                    ManagementObjectCollection moc1 = cimobject1.GetInstances();
                    foreach (ManagementObject mo in moc1)
                    {
                        HDid = HDid + mo.Properties["SerialNumber"].Value?.ToString().Trim();
                        mo.Dispose();
                        break;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return HDid;
        }

        ///   <summary> 
        ///   获取网卡硬件地址 
        ///   </summary> 
        ///   <returns> string </returns> 
        public string GetMacAddress()
        {
            string MoAddress = "";
            try
            {
                using (ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration"))
                {
                    ManagementObjectCollection moc2 = mc.GetInstances();
                    foreach (ManagementObject mo in moc2)
                    {
                        if ((bool)mo["IPEnabled"] == true)
                        {
                            MoAddress = MoAddress + mo["MacAddress"].ToString();
                            mo.Dispose();
                            break;
                        }

                        mo.Dispose();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return MoAddress;
        }
    }
}
