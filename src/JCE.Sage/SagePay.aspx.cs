namespace JCE.Sage
{
    #region Using Statements
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    #endregion

    public partial class SagePay : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            sagepay.Attributes["src"] = "RegisterTransaction.aspx?paymentid=" + Request["paymentid"];
        }
    }
}