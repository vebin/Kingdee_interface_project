using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kingdee.UploadDownload
{
    public interface IUploadDownload
    {
       string Upload(string filePath);
       bool Download(string fileId, string  Filename,string tempName);
    }
}
