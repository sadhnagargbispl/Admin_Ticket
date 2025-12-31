using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class MyPurchaseReport : System.Web.UI.Page
{
    DAL objDAL = new DAL();
    DataSet Ds = new DataSet();
    string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
    string constr1 = ConfigurationManager.ConnectionStrings["constr1"].ConnectionString;
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                if (Session["AStatus"] != null)
                {
                    FillStackList();
                    FillReport();
                }
                else
                {
                    Response.Redirect("Default.aspx");
                }
            }
        }
        catch (Exception ex)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", "alert('" + ex.Message + "')", true);
        }
    }
    private void FillStackList()
    {
        try
        {
            //Ds = SqlHelper.ExecuteDataset(constr1, "sp_ddlStacktype");
            Ds = SqlHelper.ExecuteDataset(constr1, "sp_ddlStacktype");
            DDLStackType.DataSource = Ds.Tables[0];
            DDLStackType.DataValueField = "Value";
            DDLStackType.DataTextField = "Name";
            DDLStackType.DataBind();
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
    public void FillReport()
    {
        string sql = "exec sp_GetPurchaseReport '" + DDLStackType.SelectedValue + "'";

        DataTable dtData = new DataTable();
        Ds = SqlHelper.ExecuteDataset(constr1, CommandType.Text, sql);
        GvData1.DataSource = Ds.Tables[0];
        GvData1.DataBind();
        Session["GData"] = Ds.Tables[0];
        ViewState["WithDrawDate"] = "BankCode";
        ViewState["Sort_Order"] = "ASC";
        int recordCount = Convert.ToInt32(Ds.Tables[1].Rows[0]["RecordCount"]);
        if (Ds.Tables[0].Rows.Count > 0)
        {
            lblCount.Text = "Total Record: " + Ds.Tables[1].Rows[0]["RecordCount"].ToString();
            GvData1.Visible = true;
            lblCount.Visible = true;
            lblinv.Visible = true;
            btnExport.Enabled = true;
        }
        else
        {
            lblError.Text = "No Record Found!!";
            GvData1.Visible = false;
            lblCount.Visible = false;
            lblinv.Visible = false;
            btnExport.Enabled = false;
        }
        //btnExport.Enabled = dtData.Rows.Count > 0;
    }
    protected void BtnShow_Click(object sender, EventArgs e)
    {
        try
        {
            FillReport();
        }
        catch (Exception ex)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", "alert('" + ex.Message + "')", true);
        }
    }
    protected void DDLStackType_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            FillReport();
        }
        catch (Exception ex)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", "alert('" + ex.Message + "')", true);
        }
    }
    protected void btnExport_Click(object sender, EventArgs e)
    {
        try
        {
           
            string sql = "exec sp_GetPurchaseReport '" + DDLStackType.SelectedValue + "'";

            DataTable dtData = new DataTable();
            dtData = SqlHelper.ExecuteDataset(constr1, CommandType.Text, sql).Tables[0];
            Session["MyPurchaseReportExcel"] = dtData;
            ExportExcel();
        }
        catch (Exception ex)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", "alert('" + ex.Message + "')", true);
        }
    }
    private void ExportExcel()
    {
        try
        {
            DataTable dt = (DataTable)Session["MyPurchaseReportExcel"];
            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt, "TicketReport");
                Response.Clear();
                Response.Buffer = true;
                Response.Charset = "";
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;filename=TicketReport.xlsx");
                using (MemoryStream MyMemoryStream = new MemoryStream())
                {
                    wb.SaveAs(MyMemoryStream);
                    MyMemoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }
            }
        }
        catch (Exception ex)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", "alert('" + ex.Message + "')", true);
        }
    }
    protected void GrdTotal1_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        try
        {
            GvData1.PageIndex = e.NewPageIndex;
            FillReport();
        }
        catch (Exception ex)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", "alert('" + ex.Message + "')", true);
        }
    }
}
