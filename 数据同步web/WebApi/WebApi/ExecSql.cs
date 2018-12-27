using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alex.Kingdee.Cloud.WebAPI.Core.Extension.WebAPI;
using Alex.Kingdee.Cloud.WebAPI.Core.Utils;
using Newtonsoft.Json.Linq;

namespace downData
{
    public static class getdata
    {
        //反写同步异常并反写异常详情
        public static JArray updatebillremarks(string fbillno, string result)
        {
            return WebApiUtils.GetClient().ExecuteSql("update T_IV_SALESIC set F_PAEZ_UPLOADTIME='"+DateTime.Now.ToLongTimeString()+"', F_PAEZ_uploadresult = '" + result + "',FUPSTATUS = 'B' where FBILLNO like '%" + fbillno + "%'");
        }
        //反写同步成功
        public static JArray updatebillstatus(string fbillno)
        {
            return WebApiUtils.GetClient().ExecuteSql("update T_IV_SALESIC set F_PAEZ_UPLOADTIME='" + DateTime.Now.ToLongTimeString() + "', FUPSTATUS = 'C' where FBILLNO like '%" + fbillno + "%' ");
        }

        //未同步结算单句
        public static JArray getunloadbillno()
        {
            return WebApiUtils.GetClient().ExecuteSql("select fbillno from T_IV_SALESIC ");//where FUPSTATUS <> 'C'
        }


        //获取附件清单
        public static JArray GetattachementList(string fbillno)
        {
            //查询附件单据编号
            JArray list = WebApiUtils.GetClient().ExecuteSql("exec getattaList '"+ fbillno + "'");
            string attabillno = "";
            int itemrow = 0;
            foreach (JObject item in list)
            {
                itemrow++;
               
                if (item["FBILLNO"].ToString() == "")
                {
                    continue;
                }
                attabillno += "'" + item["FBILLNO"].ToString() + "'";
                if (itemrow != list.Count)
                {
                    attabillno += ",";
                }
            }
            string attaListsql = "select FATTACHMENTNAME, FFILEID, FATTACHMENTSIZE from T_BAS_ATTACHMENT where FBILLTYPE like 'IV_SALESIC' and FBILLNO in (" + attabillno + ")";
            return WebApiUtils.GetClient().ExecuteSql(attaListsql); 
        }

        //获取结算单信息
        public static JArray getT_IV_SALESIC(string fbillno)//FMAINBOOKSTDCURRID  b1.FTAXREGISTERCODE FOPENBANKNAME
        {
            return WebApiUtils.GetClient().ExecuteSql("select '1' as docTp , FBILLNO as docNo,FID as docId,c1.FNAME as cstNm,'C' as cstCrdTp," +
                "c.F_PAEZ_BUSINESSNUM as cstCrdtNo,b.fname as  cntprNm,'C' as  cntprCrdtTp,b1.FTAXREGISTERCODE as cntprCrdtNo,'CNY' as ccy," +
                "FAFTERTOTALTAXFOR as docAmt, FDATE as docDt, F_PAEZ_BACKDATE as rfndDt,'中国人民银行' as issuBillBnk, FBANKCODE as bnkAcc ," +
                "F_PAEZ_Paytype as paymentType,F_SHA_CONTRACTNUMBER as contractNo " +
                " from T_IV_SALESIC a" +
                " left join T_BD_CUSTOMER_L b on a.FCUSTOMERID = b.FCUSTID" +
                " left join T_BD_CUSTOMER b1 on a.FCUSTOMERID = b1.FCUSTID" +
                " left join T_ORG_ORGANIZATIONS c on a.FSETTLEORGID = c.FORGID " +
                " left join T_ORG_ORGANIZATIONS_L c1 on a.FSETTLEORGID = c1.FORGID and c1.FLOCALEID = '2052'" +
                " where fbillno like '%" + fbillno + "%'");
        }

        //获取结算单信息
        public static JArray getT_IV_SALESICEntry(string fbillno)
        {
            return WebApiUtils.GetClient().ExecuteSql("select b1.FNUMBER as materNum,b.FNAME as materName,c.FNAME as unit,FPRICEQTY as realWeight,F_SHA_REALQTYD as settlementWeight," +
                " F_SHA_NOTAXPRICE as clientSettleNotaxPrice, FAMOUNTFOR as clientSettleNotaxAmount, a.FAUXPRICE as clientNotaxPrice, F_SHA_PurchasePrice as upSuppersettleTaxprice, FDETAILTAXAMOUNTFOR as clientNotaxAmount," +
                " FAUXTAXPRICE as clientSettleweightTaxprice, FALLAMOUNTFOR as clientSettletotalPrice, FNOTAXAMOUNT as clientNotaxLocalamount, FALLAMOUNT as clientTotalLocalamount, FDETAILTAXAMOUNT as clientTaxLocal," +
                " FTAXRATE as clientTaxrate, F_SHA_PURCHASEAMOUNTD as upSuppersettleTotalamount, F_SHA_MAYPAYAMOUNT as useInterimpayment, F_SHA_GROSSPROFIT as grossMargin, FCOSTAMTSUM as clientCostamount," +
                " F_SHA_RECEIVEDAMOUNTD as amountReceived" +
                " from T_IV_SALESICENTRY a" +
                " left join T_IV_SALESICENTRY_O a1 on a.FENTRYID = a1.FENTRYID" +
                " left join T_IV_SALESIC a2 on a.fid = a2.fid " +
                " left join T_BD_MATERIAL_L b on a.FMATERIALID = b.FMATERIALID" +
                " left join T_BD_MATERIAL b1 on a.FMATERIALID = b1.FMATERIALID " +
                " left join T_BD_UNIT_L c on a.FPRICEUNITID = c.FUNITID " +
                " where FBILLNO like '%" + fbillno + "%'");
        }
        //获取上游榜单信息
        public static JArray getupBillEntry(string fbillno)
        {
            string sql = "select t31.FNUMBER as materNum, t3.FNAME as materName, F_SHA_TRANSPORTMETHOD as transportType, F_SHA_PLATENUMBER as carNum, F_SHA_DRIVER as driverName," +
                "F_SHA_TELEPHONE as driverPhone, t4.fname as unit, FACTRECEIVEQTY as upSupperdepartQty, FPREDELIVERYDATE as departTime, F_SHA_PURCHASELBS as upSupperbillNo," +
                "FTAXRATE as upSuppertaxRate, FPRICE as upSuppernoTaxprice, FAMOUNT as upSuppernoTaxamount, FTAXAMOUNT as upSuppertaxAmount, FALLAMOUNT as upSuppertotalAmount," +
                "F_SHA_MAYPAYAMOUNT as useupSupperinterimPayment, F_SHA_PAYEDAMOUNT as receivedupSupperinterimPayment, F_SHA_SALETAXPRICE  as clientTotalprice, F_SHA_SALETAXRATE as clientTaxrate, F_SHA_SALENOTAXPRICE as clientNotaxPrice," +
                "F_SHA_SALENOTAXAMOUNT  as clientNotaxAmount, F_SHA_SALESUMAMOUNT as  clientTotalamount" +
                " from T_PUR_ReceiveEntry t1" +
                " left join T_PUR_ReceiveEntry_F t11 on t1.FENTRYID = t11.FENTRYID" +
                " left join T_PUR_Receive t2 on t1.fid = t2.fid" +
                " left join T_BD_MATERIAL_L t3 on t1.FMATERIALID = t3.FMATERIALID" +
                " left join T_BD_MATERIAL t31 on t1.FMATERIALID = t31.FMATERIALID" +
                " left join T_BD_UNIT_L t4 on t11.FPRICEUNITID = t4.FUNITID" +
                " left join T_BD_STOCK_L t5 on t1.FSTOCKID = t5.FSTOCKID" +
                " where t1.FENTRYID in (select h.FENTRYID" +
                " from T_IV_SALESICENTRY a" +
                " left join T_IV_SALESICENTRY_O a1 on a.FENTRYID = a1.FENTRYID" +
                " left join T_IV_SALESIC a2 on a.fid = a2.fid" +
                " left join T_IV_SALESICENTRY_LK d on d.FENTRYID = a.FENTRYID" +
                " left join t_AR_receivableEntry_LK e on d.FSID = e.FENTRYID" +
                " left join T_SAL_OUTSTOCKENTRY_LK f on e.FSID = f.FENTRYID" +
                " left join T_STK_INSTOCKENTRY_LK g on f.FSID = g.FENTRYID" +
                " left join T_PUR_ReceiveEntry h on g.FSID = h.FEntryID" +
                " where FBILLNO like '" + fbillno + "')";
            return WebApiUtils.GetClient().ExecuteSql(sql);
        }

        //获取下游榜单信息
        public static JArray getdownBillEntry(string fbillno)
        {
            return WebApiUtils.GetClient().ExecuteSql("select t31.FNUMBER as  materNum,t3.FNAME as materName,FMUSTQTY as upSupperdepartQty,t1.FREALQTY as clientReceivedQty,t4.FNAME as unit," +
                "FPRICE as upSuppernoTaxprice, FAMOUNT as upSuppernoTaxamount, t5.FNAME as stock, FTAXPRICE as upSupperTaxprice, FTAXRATE as upSuppertaxRate," +
                "FTAXAMOUNT as upSuppertaxAmount, FALLAMOUNT as upSuppertotalAmount, FBASEAPJOINQTY as qtyPayable, F_SHA_TRANSPORTMETHOD as transportType, F_SHA_DRIVER as driverName," +
                " F_SHA_PLATENUMBER as carNum, F_SHA_TELEPHONE as driverPhone, F_SHA_SHIPDATE as departTime, F_SHA_MAYPAYAMOUNT as driverupSupperinterimPayment, F_SHA_PAYEDAMOUNT as receivedupSupperinterimPayment," +
                "t6.FNAME as carrier, F_SHA_ARQTYD as checkBillqty, F_SHA_PURCHASELBS as upBillNo, F_SHA_SALELBSS as downBillNo, F_SHA_SALETAXPRICE as clientTaxprice," +
                "F_SHA_SALETAXRATE as clientTaxrate, F_SHA_SALENOTAXPRICE as clientNotaxPrice, F_SHA_SALENOTAXAMOUNT as clientNotaxAmount, F_SHA_SALETAXAMOUNT as clientTaxAmount, F_SHA_SALESUMAMOUNT as clientTotalamount" +
                " from T_STK_INSTOCKENTRY t1" +
                " left join T_STK_INSTOCKENTRY_F t11 on t1.FENTRYID = t11.FENTRYID" +
                " left join T_STK_INSTOCK t2 on t1.fid = t2.fid" +
                " left join T_BD_MATERIAL_L t3 on t1.FMATERIALID = t3.FMATERIALID" +
                " left join T_BD_MATERIAL t31 on t1.FMATERIALID = t31.FMATERIALID" +
                " left join T_BD_UNIT_L t4 on t11.FPRICEUNITID = t4.FUNITID" +
                " left join T_BD_STOCK_L t5 on t1.FSTOCKID = t5.FSTOCKID" +
                " left join T_BD_SUPPLIER_L t6 on t1.F_SHA_TRANSPORTER = t6.FSUPPLIERID" +
                " where t1.FENTRYID in (" +
                " select g.FENTRYID" +
                " from T_IV_SALESICENTRY a" +
                " left join T_IV_SALESICENTRY_O a1 on a.FENTRYID = a1.FENTRYID" +
                " left join T_IV_SALESIC a2 on a.fid = a2.fid" +
                " left join T_IV_SALESICENTRY_LK d on d.FENTRYID = a.FENTRYID" +
                " left join t_AR_receivableEntry_LK e on d.FSID = e.FENTRYID" +
                " left join T_SAL_OUTSTOCKENTRY_LK f on e.FSID = f.FENTRYID" +
                " left join T_STK_INSTOCKENTRY g on f.FSID = g.FENTRYID" +
                " where FBILLNO like '" + fbillno + "')");
        }
    }
}
