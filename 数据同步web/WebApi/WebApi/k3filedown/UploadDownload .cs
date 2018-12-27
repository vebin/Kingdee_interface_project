using Kingdee.BOS.JSON;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kingdee.UploadDownload
{
    public class UploadDownload : IUploadDownload
    {
        public string ServerUrl= "http://shunhongan.test.ik3cloud.com/k3cloud/";//服务器地址http://172.17.2.100:81/K3Cloud/
        public string DBID= ConfigurationManager.AppSettings["DataCenterId"];//帐套ID
        public string UserName= ConfigurationManager.AppSettings["UserName"];//用户名
        public string Password= ConfigurationManager.AppSettings["UserPassWord"];//密码
        public int LID;//语言类型ID
        private CookieContainer cookieContainer = new CookieContainer();
        private string UploadUrl = "FileUpLoadServices/FileService.svc/upload2attachment/";
        private string DownloadUrl = "FileUpLoadServices/download.aspx/";

        public UploadDownload()
        { }

        public UploadDownload(string ServerUrl, string DBID, string UserName, string Password, int LID)
        {
            this.ServerUrl = ServerUrl;
            this.DBID = DBID;
            this.UserName = UserName;
            this.Password = Password;
            this.LID = LID;
        }
        /// <summary>
        /// 上传接口
        /// </summary>
        /// <param name="filePath">上传的文件物理路径</param>
        /// <returns>上传之后的文件ID</returns>
        public string Upload(string filePath)
        {
            string token = GetToken();
            bool bResult = false;
            UploadUrl = ServerUrl + UploadUrl;
            string fileId = string.Empty;
            int MAXSIZE = 1024 * 4;//分批上传文件最大4M
            long size = 0;
            FileInfo fi = new FileInfo(filePath);
            size = fi.Length;
            string fileName = fi.Name;
            int startIndex = 0;
            byte[] newFile = null;
            while (size > MAXSIZE) //分块上传
            {
                newFile = new byte[MAXSIZE];
                string tempUrl = UploadUrl;

                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    fs.Seek(startIndex, SeekOrigin.Current);
                    fs.Read(newFile, 0, newFile.Length);
                }
                tempUrl = string.Format("{0}?fileName={1}&fileId={2}&token={3}&last={4}", UploadUrl, fileName, fileId, token, size > MAXSIZE ? true : false);
                if (!Upload(newFile, tempUrl, ref fileId))
                    return string.Empty;
                size = size - MAXSIZE;
                startIndex += MAXSIZE;
                newFile = null;
            }
            if (size > 0)
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    newFile = new byte[size];
                    fs.Seek(startIndex, SeekOrigin.Current);
                    fs.Read(newFile, 0, newFile.Length);
                }
                UploadUrl = string.Format("{0}?fileName={1}&fileId={2}&token={3}&last={4}", UploadUrl, fileName, fileId, token, true);
                bResult = Upload(newFile, UploadUrl, ref fileId);
            }
            return bResult ? fileId : string.Empty;
        }

        /// <summary>
        /// 下载接口
        /// </summary>
        /// <param name="fileId">需要下载文件ID</param>
        /// <param name="saveFilePath">文件夹</param>
        public bool Download(string fileId,string Filename,string tempName)
        {
            string token = GetToken();
            DownloadUrl = string.Format("{0}{1}?fileId={2}&token={3}&t={4}&nail={5}", ServerUrl, DownloadUrl, fileId, token, DateTime.Now.Ticks, 0);
            try
            {
                //创建文件夹
                string filePath = AppDomain.CurrentDomain.BaseDirectory +"downtemp\\"+ tempName;
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }
                string filedownPath = filePath + "\\" + Filename;

                System.Net.HttpWebRequest Myrq = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(DownloadUrl); //从URL地址得到一个WEB请求   
                System.Net.HttpWebResponse myrp = (System.Net.HttpWebResponse)Myrq.GetResponse(); //从WEB请求得到WEB响应   
                long totalBytes = myrp.ContentLength; //从WEB响应得到总字节数   
                System.IO.Stream st = myrp.GetResponseStream(); //从WEB请求创建流（读）   
                System.IO.Stream so = new System.IO.FileStream(filedownPath, System.IO.FileMode.Create); //创建文件流（写）   
                long totalDownloadedByte = 0; //下载文件大小   
                byte[] by = new byte[102400];
                int osize = st.Read(by, 0, (int)by.Length); //读流   
                while (osize > 0)
                {
                    totalDownloadedByte = osize + totalDownloadedByte; //更新文件大小   
                    Application.DoEvents();
                    so.Write(by, 0, osize); //写流   
                    osize = st.Read(by, 0, (int)by.Length); //读流   
                }
                so.Close(); //关闭流
                st.Close(); //关闭流
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        /// <summary>
        /// 下载接口
        /// </summary>
        /// <param name="fileId">需要下载文件ID</param>
        /// <param name="saveFilePath">保存的物理地址</param>
        /// 返回字节
        public string Download_btye(string fileId, string Filename, string saveFilePath)
        {
            string token = GetToken();
            DownloadUrl = string.Format("{0}{1}?fileId={2}&token={3}&t={4}&nail={5}", ServerUrl, DownloadUrl, fileId, token, DateTime.Now.Ticks, 0);
            return DownloadUrl;
        }


        private string GetToken()
        {
            string result = string.Empty;
            try
            {
                APIClient client = new APIClient(ServerUrl, DBID, UserName, Password, cookieContainer);
                string lresult = client.Login();
                JObject resultObj = JObject.Parse(lresult);
                if (resultObj["LoginResultType"].ToString() == "1")
                {
                    result = resultObj["Context"]["UserToken"].ToString();
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            return result;

        }
        private bool Upload(byte[] fileData, String url, ref string fileId)
        {
            bool bResult = false;
            using (HttpClient webClient = new HttpClient(cookieContainer) { Encoding = Encoding.UTF8 })
            {
                byte[] responseBytes;
                try
                {
                    responseBytes = webClient.UploadData(url, "POST", fileData);
                    JSONObject jObj = JSONObject.Parse(Encoding.UTF8.GetString(responseBytes));
                    if (jObj.Keys.Contains("Upload2AttachmentResult"))
                    {
                        if ((jObj["Upload2AttachmentResult"] as Kingdee.BOS.JSON.JSONObject)["Success"].ToString().ToLower() == "true")
                        {
                            fileId = (jObj["Upload2AttachmentResult"] as Kingdee.BOS.JSON.JSONObject)["FileId"].ToString();
                            bResult = true;
                        }
                    }
                }
                catch (WebException ex)
                {
                    Stream resp = ex.Response.GetResponseStream();
                    responseBytes = new byte[ex.Response.ContentLength];
                    resp.Read(responseBytes, 0, responseBytes.Length);
                    resp.Close();
                    resp.Dispose();
                    throw new Exception(Encoding.UTF8.GetString(responseBytes));
                }
            }
            return bResult;
        }
    }
}
