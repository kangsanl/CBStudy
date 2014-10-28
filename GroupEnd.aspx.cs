using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CBStudy
{
    public partial class GroupEnd : System.Web.UI.Page
    {
        LINQ.clsPeriodsBuyingDataContext db;

        protected void Page_Load(object sender, EventArgs e)
        {
            db = new LINQ.clsPeriodsBuyingDataContext();

            if (Request.QueryString["TURN"] != null)// 순번
            {
                this.hdTURN.Text = Request.QueryString["TURN"].ToString();
            }

            if (Request.QueryString["USERID"] != null)// 학생ID
            {
                this.lblUSERID.Text = Request.QueryString["USERID"].ToString();
            }

            if (Request.QueryString["EX_SET"] != null)// 실험 SET 번호
            {
                this.hdEX_SET.Text = Request.QueryString["EX_SET"].ToString();
            }
        }

        public int CheckValue(int n)
        {
            // LINQ query
            var infos = from p in db.TBInfor2s // from절
            //var infos = from p in db.TBInfor2_2As // from절
                        where p.TURN == n        // where절
                        select p;               // select절

            int ValueType;
            int Val = 0;
            int P1 = 0;
            int P2 = 0;

            if (infos != null)
            {
                foreach (var v in infos)
                {
                    //this.lblK.Text = v.C_K.ToString();      // Number of Products
                    //this.lblN.Text = v.C_N.ToString();      // Number of Customers
                    //this.lblP1.Text = v.C_P1.ToString();    // First period price
                    //this.lblP2.Text = v.C_P2.ToString();    // Second period price
                    ////this.lblV.Text = v.C_Y.ToString();      // Your Retail Price
                    //this.lblV.Text = v.C_V.ToString();      // Your Retail Price
                    //this.lblTURN.Text = this.hdTURN.Text;   // 순번

                    Val = int.Parse(v.C_V.ToString());      // Your Retail Price
                    P1 = int.Parse(v.C_P1.ToString());      // First period price
                    P2 = int.Parse(v.C_P2.ToString());      // Second period price
                }
            }

            if (Val > P1)
            {
                ValueType = 1;
            }
            else
            {
                ValueType = 2;
            }

            return ValueType;
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            int iTurn = int.Parse(this.hdTURN.Text) + 1;

            //다음 데이터 셋의 가격에 따라, Decision 또는 NoDecision으로 갈지를 결정

            //if (CheckValue(iTurn) == 1)
            //{
                Response.Redirect("Decision.aspx?TURN=" + iTurn.ToString() + "&USERID=" + this.lblUSERID.Text + "&EX_SET=" + this.hdEX_SET.Text);
            //}
            //else
            //{
            //    Response.Redirect("NoDecision.aspx?TURN=" + iTurn.ToString() + "&USERID=" + this.lblUSERID.Text + "&EX_SET=" + this.hdEX_SET.Text);
            //}

        }
    }
}