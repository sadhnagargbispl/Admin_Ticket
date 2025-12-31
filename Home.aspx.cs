using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.ServiceModel.Activities;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Home : System.Web.UI.Page
{
    string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
    string constr1 = ConfigurationManager.ConnectionStrings["constr1"].ConnectionString;
    DAL obj = new DAL();
    DataTable dt = new DataTable();
    DataSet ds = new DataSet();
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if ( Session["AStatus"] !=  null )
            {
                if (!Page.IsPostBack)
                {
                    FillData();
                    //FillWalletSummary();
                }
            }
            else
            {
                Response.Redirect("Default.aspx");
            }
        }
        catch (Exception ex)
        {
            
        }
    }


    protected void FillData()
    {
        try
        {
            DataTable dt = new DataTable();

            string sql;
            sql = obj.IsoStart + "  Exec AdminDashBoard " + obj.IsoEnd;
            dt = SqlHelper.ExecuteDataset(constr1, CommandType.Text, sql).Tables[0];

            if (dt.Rows.Count > 0)
            {
                RepMemberData.DataSource = dt;
                RepMemberData.DataBind();
            }
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
 
}