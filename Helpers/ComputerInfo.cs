using Business_Layer.services;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;

namespace sssUmidSrvcs.Helpers
{
    public class ComputerInfo
    {
        public static string ProcIDMachName
        {
            get
            {
                string text = string.Empty;
                string processorID = string.Empty;
                string machineName = System.Environment.MachineName;

                ManagementClass mc = new ManagementClass("win32_processor");
                ManagementObjectCollection moc = mc.GetInstances();

                foreach (ManagementObject mo in moc)
                {
                    processorID = mo.Properties["processorID"].Value.ToString();
                    break;
                }
                text = processorID + machineName;
                return text;
            }
        }


        public void WriteKeys(string text)
        {
            try
            {
                if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + @"Keys\"))
                    Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + @"Keys\");

                string path = AppDomain.CurrentDomain.BaseDirectory + @"Keys\" + "AuthKeys" + ".txt";
                if (!File.Exists(path))
                    using (StreamWriter writer = new StreamWriter(path, true))
                    {
                        writer.Write(text);
                        writer.Flush();
                        writer.Close();
                    }
            }
            catch (Exception err) { }
        }

        public static string ReadKeys
        {
            get
            {

                string path = AppDomain.CurrentDomain.BaseDirectory + @"Keys\" + "AuthKeys" + ".txt";
                string text = System.IO.File.ReadAllText(path);
                return text;
            }
        }


        public bool VerifyKeys()
        {
            Cryptor cryptor = new Cryptor();

            string CryptedText = cryptor.Encrypt(ComputerInfo.ProcIDMachName);
            string ReadKeys = ComputerInfo.ReadKeys;
            if (CryptedText == ReadKeys)
            {
                int res = Res("https://localhost:44326/umid/authkey", ReadKeys);
                if(res == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }


        public int Res(string url, string Keys)
        {
            try
            {
                using (var wb = new WebClient())
                {
                    var data = new NameValueCollection();
                    data["keys"] = Keys;

                    var response = wb.UploadValues(url, "POST", data);
                    string responseInString = Encoding.UTF8.GetString(response);
                    return Convert.ToInt32(responseInString);
                }

            }
            catch (Exception e)
            {
                return 99;
            }
        }




    }
}