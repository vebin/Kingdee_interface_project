using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace LogHelper
{
   public class LogHelper
    {
        public void WriteLine(string msg)
        {
            string filePath = AppDomain.CurrentDomain.BaseDirectory + "Log";
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
            string logPath = AppDomain.CurrentDomain.BaseDirectory + "Log\\" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
            try
            { using (StreamWriter sw = File.AppendText(logPath))
                {
                    sw.WriteLine("消息：" + msg);
                    sw.WriteLine("时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")); sw.WriteLine("**************************************************");
                    sw.WriteLine(); sw.Flush(); sw.Close(); sw.Dispose();
                }
            }
            catch (IOException e)
            {
                using(StreamWriter sw = File.AppendText(logPath))
                {
                    sw.WriteLine("异常：" + e.Message);
                    sw.WriteLine("时间：" + DateTime.Now.ToString("yyy-MM-dd HH:mm:ss"));
                    sw.WriteLine("**************************************************");
                    sw.WriteLine(); sw.Flush(); sw.Close(); sw.Dispose();
                }
            }
        }
 }
}