using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Kingdee.UploadDownload;
namespace downData
{
    public class FileDown
    {
        /// <summary>
        /// 下载服务器文件至客户端
        /// </summary>
        /// <param name="strUrlFilePath">要下载的Web服务器上的文件地址（全路径）</param>
        /// <param name="Dir">下载到的目录（存放位置，机地机器文件夹）</param>
        /// <returns>True/False是否上传成功</returns>
        public bool DownLoadFile(string URL, string strLocalDirPath)
        {
            // 创建WebClient实例
            System.Net.WebClient client = new WebClient();
            //被下载的文件名
            string fileName = URL.Substring(URL.LastIndexOf("/"));
            //另存为的绝对路径＋文件名
            string Path = strLocalDirPath + fileName;
            try
            {
                WebRequest myWebRequest = WebRequest.Create(URL);
            }
            catch (Exception exp)
            {
                //MessageBox.Show("文件下载失败：" + exp.Message, "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            try
            {
                client.DownloadFile(URL, Path);
                return true;
            }
            catch (Exception exp)
            {
               // MessageBox.Show("文件下载失败：" + exp.Message, "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
        }


        /// <summary>
        /// 下载带进度条代码（普通进度条）
        /// </summary>
        /// <param name="URL">网址</param>
        /// <param name="Filename">文件名</param>
        /// <param name="Prog">普通进度条ProgressBar</param>
        /// <returns>True/False是否下载成功</returns>
        public bool DownLoadFile(string URL,  ProgressBar Prog)
        {
            try
            {
                //被下载的文件名
                string Filename = "temp.txt";
                System.Net.HttpWebRequest Myrq = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(URL); //从URL地址得到一个WEB请求   
                System.Net.HttpWebResponse myrp = (System.Net.HttpWebResponse)Myrq.GetResponse(); //从WEB请求得到WEB响应   
                long totalBytes = myrp.ContentLength; //从WEB响应得到总字节数   
                Prog.Maximum = (int)totalBytes; //从总字节数得到进度条的最大值   
                System.IO.Stream st = myrp.GetResponseStream(); //从WEB请求创建流（读）   
                System.IO.Stream so = new System.IO.FileStream(Filename, System.IO.FileMode.Create); //创建文件流（写）   
                long totalDownloadedByte = 0; //下载文件大小   
                byte[] by = new byte[10240];
                int osize = st.Read(by, 0, (int)by.Length); //读流   
                while (osize > 0)
                {
                    totalDownloadedByte = osize + totalDownloadedByte; //更新文件大小   
                    Application.DoEvents();
                    so.Write(by, 0, osize); //写流   
                    Prog.Value = (int)totalDownloadedByte; //更新进度条   
                    osize = st.Read(by, 0, (int)by.Length); //读流   
                }
                so.Close(); //关闭流
                st.Close(); //关闭流
                return true;
            }
            catch
            {
                return false;
            }
        }




        /// <summary>
        /// 下载带进度条代码(状态栏式进度条）
        /// </summary>
        /// <param name="URL">网址</param>
        /// <param name="Filename">文件名</param>
        /// <param name="Prog">状态栏式进度条ToolStripProgressBar</param>
        /// <returns>True/False是否下载成功</returns>
        public bool DownLoadFile(string URL,  ToolStripProgressBar Prog)
        {
            try
            {
                //被下载的文件名
                string Filename = URL.Substring(URL.LastIndexOf("/"));
                System.Net.HttpWebRequest Myrq = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(URL); //从URL地址得到一个WEB请求   
                System.Net.HttpWebResponse myrp = (System.Net.HttpWebResponse)Myrq.GetResponse(); //从WEB请求得到WEB响应   
                long totalBytes = myrp.ContentLength; //从WEB响应得到总字节数   
                Prog.Maximum = (int)totalBytes; //从总字节数得到进度条的最大值   
                System.IO.Stream st = myrp.GetResponseStream(); //从WEB请求创建流（读）   
                System.IO.Stream so = new System.IO.FileStream(Filename, System.IO.FileMode.Create); //创建文件流（写）   
                long totalDownloadedByte = 0; //下载文件大小   
                byte[] by = new byte[10240];
                int osize = st.Read(by, 0, (int)by.Length); //读流   
                while (osize > 0)
                {
                    totalDownloadedByte = osize + totalDownloadedByte; //更新文件大小   
                    Application.DoEvents();
                    so.Write(by, 0, osize); //写流   
                    Prog.Value = (int)totalDownloadedByte; //更新进度条   
                    osize = st.Read(by, 0, (int)by.Length); //读流   
                }
                so.Close(); //关闭流   
                st.Close(); //关闭流   
                return true;
            }
            catch
            {
                return false;
            }

        }



        public void startProcess(string fileName)
        {
            //定义一个ProcessStartInfo实例

            System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo();

            //设置启动进程的初始目录

            info.WorkingDirectory = Application.StartupPath;

            //设置启动进程的应用程序或文档名

            info.FileName = @fileName;

            //设置启动进程的参数

            info.Arguments = "";

            //启动由包含进程启动信息的进程资源

            try
            {

                System.Diagnostics.Process.Start(info);

            }

            catch (System.ComponentModel.Win32Exception we)
            {

               //MessageBox.Show(this, we.Message);

                return;

            }
        }
    }
}
