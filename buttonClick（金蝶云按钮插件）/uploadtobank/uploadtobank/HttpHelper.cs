using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Security;

namespace Kingdee.K3.cd.Business.PlugIn
{
    public  class HttpHelperx
    {
   
        public  string Get(string url)
        {
            return Get(url, Encoding.UTF8);
        }

        public  string Get(string url, Encoding encoding)
        {
            try
            {
                var wc = new WebClient { Encoding = encoding };
                var readStream = wc.OpenRead(url);
                using (var sr = new StreamReader(readStream, encoding))
                {
                    var result = sr.ReadToEnd();
                    return result;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}