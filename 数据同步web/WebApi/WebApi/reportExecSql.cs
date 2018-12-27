using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alex.Kingdee.Cloud.WebAPI.Core.Extension.WebAPI;
using Alex.Kingdee.Cloud.WebAPI.Core.Utils;
using Newtonsoft.Json.Linq;
using WebGrease.Css.Ast.Selectors;

namespace downData
{
    public static class reportExecSql
    {
        //查询非禁用状态的客户数量
        //参数为组织number
        public static JArray getCustomerQty(string orgNum,string filter)
        {
            string sql = "select count(*) as custqty from T_BD_CUSTOMER a" +
                " left join T_ORG_ORGANIZATIONS b on a.FUSEORGID = b.FORGID" +
                " where b.FNUMBER = '"+ orgNum + "' and  a.FFORBIDSTATUS = 'A' ";
            if(filter != null)
            {
                sql = sql + "and " + filter;
            }
            return WebApiUtils.GetClient().ExecuteSql(sql);
        }


        public static JArray getEverydayNewCustomerqty(string orgNum)
        {
            DateTime now = DateTime.Now;
            string sql = "select COUNT(*) as custqty from T_BD_CUSTOMER a " +
                " left join T_ORG_ORGANIZATIONS b on a.FUSEORGID = b.FORGID " +
                " where a.FFORBIDSTATUS = 'A' and b.FNUMBER = '100' " +
                " and year(a.FCREATEDATE) = '"+now.Year+"' and MONTH(a.FCREATEDATE) = '"+now.Month+"' and day(a.FCREATEDATE) = '"+now.Day+"'";
            return WebApiUtils.GetClient().ExecuteSql(sql);
        }
        //查询非作废状态的结算单数量和金额
        //参数为组织number
        public static JArray getorderqty(string orgNum ,string filter)
        {

            string sql = " select sum(FAFTERTOTALTAXFOR) as amount,count(*) as orderqty from T_IV_SALESIC a " +
                " left join T_ORG_ORGANIZATIONS b on a.FSETTLEORGID = b.FORGID " +
                " where a.FCANCELSTATUS = 'A' and a.FDOCUMENTSTATUS = 'C' and b.FNUMBER = '"+ orgNum + "' ";
            if(filter != null)
            {
                sql = sql + "and " + filter;

            }
            return WebApiUtils.GetClient().ExecuteSql(sql);
        }

        //查询和上传银行的非作废状态的结算单数量和金额
        //参数为组织number
        public static JArray getbankcustqty(string orgNum)
        {
            string sql = "select count(FCUSTOMERID) from T_IV_SALESIC a " +
                " left join T_ORG_ORGANIZATIONS b on a.FSETTLEORGID = b.FORGID " +
                " where a.FCANCELSTATUS = 'A' and a.FDOCUMENTSTATUS = 'C' and b.FNUMBER = '"+orgNum+"' " +
                " group by FCUSTOMERID ";
            return WebApiUtils.GetClient().ExecuteSql(sql);
        }
     
    }
}
