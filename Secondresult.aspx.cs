using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CBStudy
{
    public partial class Secondresult : System.Web.UI.Page
    {
        LINQ.clsPeriodsBuyingDataContext db;
        protected void Page_Load(object sender, EventArgs e)
        {
            db = new LINQ.clsPeriodsBuyingDataContext();

            // Request parameter
            if (Request.QueryString["TURN"] != null)
            {
                this.hdTURN.Text = Request.QueryString["TURN"].ToString();
            }
            if (Request.QueryString["TMAX"] != null) // 전체순번
            {
                this.hdMAX.Text = Request.QueryString["TMAX"].ToString();
            }
            if (Request.QueryString["USERID"] != null)
            {
                this.hdUSERID.Text = Request.QueryString["USERID"].ToString();
            }
            if (Request.QueryString["PROFIT"] != null)
            {
                this.hdPROFIT.Text = Request.QueryString["PROFIT"].ToString();
            } 

            if (!IsPostBack)
            {
                SetInforBind(int.Parse(this.hdTURN.Text));
            }
        }

        /// <summary>
        /// information biding by using LINQ query
        /// </summary>
        /// <param name="n"></param>
        public void SetInforBind(int n)
        {           
            // LINQ query
            var infos = from p in db.TBInfor
                        where p.TURN == n
                        select p;

            if (infos != null)
            {
                foreach (var v in infos)
                {
                    this.lblK.Text = v.C_K.ToString();      // Number of Products
                    this.lblN.Text = v.C_N.ToString();      // Number of Customers
                    this.lblP1.Text = v.C_P1.ToString();    // First period price
                    this.lblP2.Text = v.C_P2.ToString();    // Second period price
                    //this.lblV.Text = v.C_Y.ToString();      // Your Retail Price
                    this.lblV.Text = v.C_V.ToString();      // Your Retail Price
                    this.lblTURN.Text = this.hdTURN.Text
                        + "/"
                        + this.hdMAX.Text;   // 순번/전체순번
                }
            }

            // 결과메시지
            //2011.08.05 메시지 수정
            //string result1 = "You got the product in the first period Your profit is ";
            string result1 = "You got the product in the second period <br />    Your profit is ";
            string result2 = "You didn’t get the product <br />    Your profit is ";
            // 시뮬레이션 결과에 의한 profit
            int iProfit = int.Parse(this.hdPROFIT.Text);

            // 시뮬레이션 이후 결과를 얻은 경우
            if (iProfit > 0)
            {
                this.lblRESULT.Text = result1 + iProfit.ToString();
            }
            else
            {
                this.lblRESULT.Text = result2 + iProfit.ToString();
            }

            // 2011/07/13 - LINQ query (SUM)
            var tSum = from p in db.TBSimulation    // from절
                       where p.USERID == this.hdUSERID.Text
                            && p.GROUPID == "A"     // where절
                       select p.RESULT;             // select절

            // Total Profit
            this.lblTPROFIT.Text = tSum.Sum().ToString();
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            int iTurn = 0;

            // 전체순번과 현재순번+1이 크면 종료
            if (int.Parse(this.hdMAX.Text) < int.Parse(this.hdTURN.Text) + 1)
            {
                //2011.08.03 마지막 화면에서 Total Profit보여주기 위해 수정
                //Response.Redirect("Final.aspx");
                Response.Redirect("Final.aspx?&USERID=" + this.hdUSERID.Text);
            }
            else
            {
                iTurn = int.Parse(this.hdTURN.Text) + 1;
                // Next 버튼 클릭시 첫 화면(Information)으로 이동
                // 파라미터 : 순번,학생ID
                Response.Redirect("Main.aspx?TURN=" + iTurn.ToString() + "&USERID=" + this.hdUSERID.Text);
            }
        }
    }
}
