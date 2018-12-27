using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Kingdee.UploadDownload
{
    public class APIClient
    {
        public CookieContainer cookieContainer;
        public string CloudUrl;
        public string DBID;
        public string UserName;
        public string Password;
        public APIClient()
        { }
        public APIClient(string CloudUrl, string DBID, string UserName, string Password, CookieContainer cookieContainer)
        {
            this.CloudUrl = CloudUrl;
            this.DBID = DBID;
            this.UserName = UserName;
            this.Password = Password;
            this.cookieContainer = cookieContainer;
        }
        public string Login()
        {
            ClientBase httpClient = new ClientBase();
            httpClient.Url = string.Concat(CloudUrl, "Kingdee.BOS.WebApi.ServicesStub.AuthService.ValidateUser.common.kdsvc");
            httpClient.Cookie = this.cookieContainer;

            List<object> Parameters = new List<object>();
            Parameters.Add(this.DBID);//帐套Id
            Parameters.Add(this.UserName);//用户名
            Parameters.Add(this.Password);//密码
            Parameters.Add(2052);

            httpClient.Content = JsonConvert.SerializeObject(Parameters);

            return httpClient.SysncRequest();

        }
    }
}
