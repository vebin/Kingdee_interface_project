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

    public class reportController : ApiController
    {
        string signUrl = ConfigurationManager.AppSettings["signUrl"];
        string bankurl = ConfigurationManager.AppSettings["BankUrl"];
        string orgNum;//组织id

        LogHelper.LogHelper logHelper = new LogHelper.LogHelper();
        public HttpResponseMessage Get(string orgNum)
        {
            try
            {
                this.orgNum = orgNum;
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

        //构建报文内容
        private string getjson(string orgNum)
        {
            string custQty_value = ""; //客户数量值
            string orderqty_value = "";//结算单数量
            string orderamount_value = "";
            //查询客户数量
            JArray custQty = reportExecSql.getCustomerQty(orgNum,null);
            JArray orderQty = reportExecSql.getorderqty(orgNum, null);
            int custQty_temp = 0;
            foreach (JObject item in custQty)
            {
                custQty_temp++;
            }
            custQty_value = custQty_temp.ToString();
            foreach (JObject item in orderQty)
            {
                orderamount_value = item["amount"].ToString();
                orderqty_value = item["orderqty"].ToString();
            }

            float everyorderamouont = 0.00f; //平均订单价
            everyorderamouont = float.Parse(orderamount_value) / float.Parse(orderqty_value);
            float everycustpice = 0.00f;//客单价
            everycustpice = float.Parse(orderamount_value) / float.Parse(custQty_value);
            float ordrRto = 0.00f; //订单比例
            ordrRto = float.Parse(orderqty_value) / float.Parse(custQty_value) * 100;

            JArray bankcustqty = reportExecSql.getbankcustqty(orgNum);//银行客户数
            int bankcustqty_value = 0;
            foreach (JObject item in bankcustqty)
            {
                bankcustqty_value++;
            }

            //融资客户数强设置为1
            bankcustqty_value = 1;

            string bankorderqty_value = "";//上传银行结算单数量
            string bankorderamount_value = "";//上传银行结算单金额

            JArray bankorder = reportExecSql.getorderqty(orgNum, "FUPSTATUS='C'");

            foreach (JObject item in bankorder)
            {
                bankorderamount_value = item["amount"].ToString();
                bankorderqty_value = item["orderqty"].ToString();
            }

            float newCnvsRate = 0.00f;//客户转化率
            newCnvsRate = float.Parse(bankcustqty_value.ToString()) / float.Parse(custQty_value) * 100;

            float fncCnvsRate = 0.00f; //融资传化率
            fncCnvsRate = float.Parse(bankorderqty_value) / float.Parse(orderqty_value) * 100;

            string newCstNum = "";//每日新增客户数
            JArray newCstNum_obj = reportExecSql.getEverydayNewCustomerqty(orgNum);
            foreach (JObject item in newCstNum_obj)
            {
                try
                {
                    newCstNum = item["custqty"].ToString();
                }
                catch
                {
                    newCstNum = "0";
                }

            }
            DateTime now = DateTime.Now;
            JArray newTxnAmtobj = reportExecSql.getorderqty(orgNum, " year(a.FCREATEDATE) = '"+ now.Year+ "' and MONTH(a.FCREATEDATE) = '"+now.Month+"' and day(a.FCREATEDATE) = '"+now.Day+"'");


            string dayneworderqty_value = "";//上传银行结算单数量
            string dayneworderamount_value = "";//上传银行结算单金额
            foreach (JObject item in newTxnAmtobj)
            {
                try
                {
                    dayneworderamount_value = item["amount"].ToString();
                    dayneworderqty_value = item["orderqty"].ToString();
                }
                catch
                {
                    dayneworderamount_value = "0";
                    dayneworderqty_value = "0";
                }
            }

            float newOrdrRto = 0.00f;
            if (int.Parse(newCstNum) != 0)
            {
                newOrdrRto = float.Parse(dayneworderqty_value) / float.Parse(newCstNum) * 100;
            }
            JObject obj = new JObject();

            


            obj.Add("pltfrmCd", ConfigurationManager.AppSettings["pltfrmCd"].ToString());//平台代码   
            obj.Add("pltfrmNm", ConfigurationManager.AppSettings["pltfrmNm"].ToString());//平台名称
            obj.Add("blngDept", ConfigurationManager.AppSettings["blngDept"].ToString());//平台所属事业部

            obj.Add("rgstCstNum", int.Parse(custQty_value));//注册客户数量
            obj.Add("txCstNum", int.Parse(custQty_value));//交易客户数量
            obj.Add("actvCstNum", int.Parse(custQty_value));//激活客户数量
            obj.Add("toTxnAmt", float.Parse(orderamount_value));//总交易金额
            obj.Add("ordrNum", int.Parse(orderqty_value));//订单数量
            obj.Add("avrgOrdrAmt", everyorderamouont);//平均订单金额
            obj.Add("atv", everycustpice);//客单价
            obj.Add("ordrRto", ordrRto);//订单比率 %
            obj.Add("failOrdrNum", 0);//失败订单数量
            obj.Add("failOrdrAmt", 0);//失败订单金额
            obj.Add("bnkCstNum", bankcustqty_value);//银行客户数
            obj.Add("fncDnum", int.Parse(bankorderqty_value));//融资笔数
            obj.Add("totFncAmt", float.Parse(bankorderamount_value));//总融资金额
            obj.Add("newCnvsRate", float.Parse(string.Format("{0:F}", newCnvsRate)));//客户转化率 %
            obj.Add("fncCnvsRate", float.Parse(string.Format("{0:F}", fncCnvsRate)));//融资转化率
            obj.Add("newCstNum", int.Parse(newCstNum));//新增客户数（每天）
            obj.Add("newTxnAmt", float.Parse(dayneworderamount_value));//新增交易金额（每天）
            obj.Add("newOrdrNum", int.Parse(dayneworderqty_value));//新增订单数（每天）
            obj.Add("newOrdrRto", float.Parse(string.Format("{0:F}", newOrdrRto)));//新增订单比率 %（每天）
            obj.Add("cplnNum", 0);//投诉数量
            obj.Add("cplnRate", 0.0);//投诉率
            obj.Add("quarDayAmt", 0);//我行交易性存款余额（季日均）
            obj.Add("intradayAmt", 0);//我行交易性存款余额（时点）





            //构建结束
            return obj.ToString();

        }
        //子线程执行方法
        private void querythread()
        {
            try
            {
                string body = getjson(orgNum);
                JObject obj = new JObject();
                //构建方法
                //获取加密Sign
                logHelper.WriteLine(orgNum + "：获取加密Sign");

                obj.Add("App_Id", ConfigurationManager.AppSettings["App_Id"].ToString());
                obj.Add("Format", "json");
                obj.Add("Charset", "utf8");
                obj.Add("Version", ConfigurationManager.AppSettings["Version"].ToString());
                obj.Add("Tx_CD", ConfigurationManager.AppSettings["ReportTx_CD"].ToString());
                obj.Add("Tx_Dt", DateTime.Now.ToString("yyyyMMdd"));
                obj.Add("TxnSrlNo", Guid.NewGuid());
                obj.Add("To_SysTem_ID", ConfigurationManager.AppSettings["To_SysTem_ID"].ToString());

                //加签：
                string poststr = "head=" + obj.ToString().Replace("\n", "").Replace("\r", "") + "&body=" + JToken.Parse(body).ToString().Replace("\n", "").Replace("\r", "");
                poststr = poststr.Replace("      ", "");
                logHelper.WriteLine(orgNum + "：生成json:" + poststr);
                logHelper.WriteLine(orgNum + "：加签");

                string postobj_str = HttpHelper.Post1(signUrl + "shunhongan", poststr);
                logHelper.WriteLine(orgNum + "：生成密文:" + postobj_str);

                logHelper.WriteLine(orgNum + "：上传至天府银行");
                string backresult = HttpHelper.Post(bankurl, postobj_str);
                logHelper.WriteLine(orgNum + "：" + backresult);

                JObject backresult_obj = JObject.Parse(backresult);
                backresult_obj = JObject.Parse(backresult_obj["head"].ToString());
                backresult_obj = JObject.Parse(backresult_obj["retResult"].ToString());
                //成功
                logHelper.WriteLine(orgNum + "：回传处理结果");
                if (backresult_obj["rsp_code"].ToString() == "000000")
                {
                    logHelper.WriteLine(orgNum + "：" + "上传成功");
                    //标记上传结果:
                }
                else
                {

                   
                }

            }
            catch (Exception ex)
            {
                logHelper.WriteLine(orgNum + "：异常:" + ex.Message);
            }

        }

    }
}
