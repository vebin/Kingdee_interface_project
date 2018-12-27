using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Alex.Kingdee.Cloud.WebAPI.Core.Extension.WebAPI;
using Newtonsoft.Json.Linq;
using downData;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Kingdee.UploadDownload;
using System.Threading;
using Utils;
using Kingdee.BOS.JSON;
using Kingdee.BOS.KDHttpUtility;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.InteropServices.WindowsRuntime;

namespace WebApi.Controllers
{
 
    public class uploadController : ApiController
    {
        string keyName = "docTp,docNo,docId,cstNm,cstCrdTp,cstCrdtNo,cntprNm,cntprCrdtTp,cntprCrdtNo,ccy,docAmt,docDt,rfndDt,issuBillBnk,bnkAcc,paymentType,contractNo,cmdtyDtl,materNum,materName,unit,realWeight,settlementWeight,clientSettleNotaxPrice,clientSettleNotaxAmount,clientNotaxPrice,upSuppersettleTaxprice,clientNotaxAmount,clientSettleweightTaxprice,clientSettletotalPrice,clientNotaxLocalamount,clientTotalLocalamount,clientTaxLocal,clientTaxrate,upSuppersettleTotalamount,useInterimpayment,grossMargin,clientCostamount,amountReceived,upBillDetail,materNum,materName,transportType,carNum,driverName,driverPhone,unit,upSupperdepartQty,departTime,upSupperbillNo,upSuppertaxRate,upSuppernoTaxprice,upSuppernoTaxamount,upSuppertaxAmount,upSuppertotalAmount,useupSupperinterimPayment,receivedupSupperinterimPayment,clientTotalprice,clientTaxrate,clientNotaxPrice,clientNotaxAmount,clientTotalamount,downBillDetail,materNum,materName,upSupperdepartQty,clientReceivedQty,unit,upSuppernoTaxprice,upSuppernoTaxamount,stock,upSupperTaxprice,upSuppertaxRate,upSuppertaxAmount,upSuppertotalAmount,qtyPayable,transportType,driverName,carNum,driverPhone,departTime,driverupSupperinterimPayment,receivedupSupperinterimPayment,carrier,checkBillqty,upBillNo,downBillNo,clientTaxprice,clientTaxrate,clientNotaxPrice,clientNotaxAmount,clientTaxAmount,clientTotalamount,fileInfList,fileTransType,fileType,fileName,fileAddr,file";//因为金蝶云返回的sql执行结果字段名全部为小写 ,通过程序转成小写；
        string signUrl = ConfigurationManager.AppSettings["signUrl"];
        string bankurl = ConfigurationManager.AppSettings["BankUrl"];
        string billno;
        
        
        //将返回的sql查询结果中的key转换为字段规则的key
        private string tokeyname(string lowerkey)
        {
            string[] split = keyName.Split(',');
            for(int i = 0;i< split.Length;i++)
            {
                if(split[i].ToLower() == lowerkey.ToLower())
                {
                    return split[i].ToString();
                }
            }
            return "unknown";
        }

        LogHelper.LogHelper logHelper = new LogHelper.LogHelper();
        public HttpResponseMessage Get(string billno)
        {
            try
            {
                this.billno = billno;
                Thread thread = new Thread(querythread);
                thread.Start();
                string str = "{\"code\":\"200\"}";
                HttpResponseMessage result = new HttpResponseMessage { Content = new StringContent(str, Encoding.GetEncoding("UTF-8"), "application/json") };

                return result;

            }
            catch (Exception ex)
            {
                string str = "{\"code\":\"5000\"}";
                HttpResponseMessage result = new HttpResponseMessage { Content = new StringContent(str, Encoding.GetEncoding("UTF-8"), "application/json") };

                return result;
            }
        }

        private string getjson(string fbillno)
        {
            string result_json = "{";
            //构建结算单表头
            JArray array = getdata.getT_IV_SALESIC(fbillno);
            foreach (JObject item in array)
            {
                int culom = 0;
                foreach (var obj in item)
                {
                    culom++;
                    if(obj.Key.ToLower() == "docdt" || obj.Key.ToLower() == "rfnddt")
                    {
                        result_json += "\"" + tokeyname(obj.Key) + "\":\"" + DateTime.Parse(obj.Value.ToString()).ToString("yyyyMMdd") + "\"";
                    }
                    else
                    {
                        result_json += "\"" + tokeyname(obj.Key) + "\":\"" + obj.Value + "\"";
                    }
                   
                    if (item.Count != culom)
                    {
                        result_json += ",";
                    }
                }
            }
            //结算单明细
            result_json += ",\"cmdtyDtl\":[";
            JArray arrayentry = getdata.getT_IV_SALESICEntry(fbillno);
            int itemrow = 0;
            foreach (JObject item in arrayentry)
            {
                itemrow++;
                result_json += "{";
                int culom = 0;
                foreach (var obj in item)
                {
                    culom++;
                    result_json += "\"" + tokeyname(obj.Key) + "\":\"" + obj.Value + "\"";
                    if (item.Count != culom)
                    {
                        result_json += ",";
                    }
                }
                result_json += "}";
                if (itemrow != arrayentry.Count)
                {
                    result_json += ",";
                }
            }
            result_json += "],";
            //result_json += "\"upBillDetail\":[";

            ////上游榜单明细
            //JArray upbillEntry = getdata.getupBillEntry(fbillno);
            //itemrow = 0;
            //foreach (JObject item in upbillEntry)
            //{
            //    itemrow++;
            //    result_json += "{";
            //    int culom = 0;
            //    foreach (var obj in item)
            //    {
            //        culom++;
            //        result_json += "\"" + tokeyname(obj.Key) + "\":\"" + obj.Value + "\"";
            //        if (item.Count != culom)
            //        {
            //            result_json += ",";
            //        }
            //    }
            //    result_json += "}";
            //    if (itemrow != upbillEntry.Count)
            //    {
            //        result_json += ",";
            //    }
            //}
            //result_json += "],";
            //result_json += "\"downBillDetail\":[";

            ////下游榜单明细
            //JArray downbillEntry = getdata.getdownBillEntry(fbillno);
            //itemrow = 0;
            //foreach (JObject item in downbillEntry)
            //{
            //    itemrow++;
            //    result_json += "{";
            //    int culom = 0;
            //    foreach (var obj in item)
            //    {
            //        culom++;
            //        result_json += "\"" + tokeyname(obj.Key) + "\":\"" + obj.Value + "\"";
            //        if (item.Count != culom)
            //        {
            //            result_json += ",";
            //        }
            //    }
            //    result_json += "}";
            //    if (itemrow != downbillEntry.Count)
            //    {
            //        result_json += ",";
            //    }
            //}
            //result_json += "],";
            result_json += "\"fileInfList\":[";

            //附件
            JArray Atch = getdata.GetattachementList(fbillno);
            itemrow = 0;
            string uuid = System.Guid.NewGuid().ToString();
            foreach (JObject item in Atch)
            {
                itemrow++;
                result_json += "{";
                //附件内容:
                //附件list
                UploadDownload download = new UploadDownload();
                //下载相应附件
                download.Download(item["FFILEID"].ToString(), item["FATTACHMENTNAME"].ToString(), uuid);
                string fileurl = "http://118.122.122.35:9081/downtemp/" + uuid + "/" + item["FATTACHMENTNAME"].ToString();
                fileurl = System.Web.HttpUtility.UrlEncode(fileurl);
                result_json += "\"fileTransType\":\"URL\",\"fileType\":\"" + 0 + "\",\"fileName\":\"" + item["FATTACHMENTNAME"].ToString() + "\",\"fileAddr\":\"" + fileurl + "\"";
                result_json += "}";
                if (itemrow != Atch.Count)
                {
                    result_json += ",";
                }
            }
            result_json += "]";

            result_json += "}";
            //构建结束
            return result_json;

        }
        //子线程执行方法
        private void querythread()
        {
            try
            {
                    string body = getjson(billno);
                    JObject obj = new JObject();
                    //构建方法
                    //获取加密Sign
                    logHelper.WriteLine(billno+"：获取加密Sign");
                    
                    obj.Add("App_Id", ConfigurationManager.AppSettings["App_Id"].ToString());
                    obj.Add("Format", "json");
                    obj.Add("Charset", "utf8");
                    obj.Add("Version", ConfigurationManager.AppSettings["Version"].ToString());
                    obj.Add("Tx_CD", ConfigurationManager.AppSettings["Tx_CD"].ToString());
                    obj.Add("Tx_Dt", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                    obj.Add("TxnSrlNo", Guid.NewGuid());
                    obj.Add("To_SysTem_ID", ConfigurationManager.AppSettings["To_SysTem_ID"].ToString());

                     //加签：
                   string poststr = "head=" + obj.ToString().Replace("\n", "").Replace("\r", "") + "&body=" + JToken.Parse(body).ToString().Replace("\n", "").Replace("\r", "");
                   poststr = poststr.Replace("      ", "");
                   logHelper.WriteLine(billno + "：生成json:" + poststr);
                   logHelper.WriteLine(billno + "：加签");

                   string postobj_str = HttpHelper.Post1(signUrl + "shunhongan", poststr);
                     logHelper.WriteLine(billno + "：生成密文:" + postobj_str);

                    logHelper.WriteLine(billno + "：上传至天府银行");
                    string backresult = HttpHelper.Post(bankurl, postobj_str);
                    logHelper.WriteLine(billno +"："+ backresult);
                
                    JObject backresult_obj = JObject.Parse(backresult); 
                    backresult_obj = JObject.Parse(backresult_obj["head"].ToString());
                    backresult_obj = JObject.Parse(backresult_obj["retResult"].ToString());
                    //成功
                    logHelper.WriteLine(billno + "：回传处理结果");
                    if (backresult_obj["rsp_code"].ToString()== "000000")
                    {
                         logHelper.WriteLine(billno + "："+ getdata.updatebillstatus(billno));
                    //标记上传结果:
                    }
                    else
                    {

                    logHelper.WriteLine(billno + "：" + getdata.updatebillremarks(billno, backresult));
                        //标记上传结果:
                     }
                   


                    logHelper.WriteLine(billno + "：处理完成");

            }
            catch (Exception ex)
            {
                logHelper.WriteLine(billno + "：异常:" + ex.Message);
                getdata.updatebillremarks(billno, ex.Message);
            }

        }

    }
}
