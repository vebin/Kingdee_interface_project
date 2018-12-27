using Kingdee.BOS.Core.Bill.PlugIn;
using Kingdee.BOS.Core.DynamicForm.PlugIn;
using Kingdee.BOS.Core.DynamicForm.PlugIn.Args;
using Kingdee.BOS.Core.Metadata;
using Kingdee.BOS.Core.Metadata.BarElement;
using Kingdee.BOS.Core.Metadata.Expression.FuncDefine;
using Kingdee.BOS.Core.Warn.PlugIn.Args;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kingdee.K3.cd.Business.PlugIn
{
    /// <summary>
    /// 销售订单 单据维护插件
    /// </summary>
    public class uploadbutton : AbstractBillPlugIn
    {
        public override void BarItemClick(BarItemClickEventArgs e)
        {
            base.BarItemClick(e);
            if(e.BarItemKey.ToLower() == "tbbutton_upload")
            {
                HttpHelperx HttpHelper = new HttpHelperx();
                string billno = "";
                if (this.Model.GetValue("FBillNo") != null)
                {
                    billno = this.Model.GetValue("FBillNo").ToString();
                }
                if (billno == "")
                {
                    View.ShowMessage("还未生成单据编号,请先保存");
                    return;
                }
                try
                {
                    HttpHelper.Get("http://118.122.122.35:9081/api/upload?billno=" + billno);
                    View.ShowMessage("提交成功，请耐心等待，预计3分钟完成。请刷新查看结果");
                }
                catch(Exception ex)
                {
                    View.ShowMessage("发生异常："+ex.Message);
                }
                
            }
         
        }

    }
}