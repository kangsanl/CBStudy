using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CBStudy
{
    public partial class Result : System.Web.UI.Page
    {
        LINQ.clsPeriodsBuyingDataContext db;

        protected void Page_Load(object sender, EventArgs e)
        {
            db = new LINQ.clsPeriodsBuyingDataContext();
            if (Request.QueryString["TURN"] != null) // 순번
            {
                this.hdTURN.Text = Request.QueryString["TURN"].ToString();
            }
            if (Request.QueryString["USERID"] != null) // 학생ID
            {
                this.lblUSERID.Text = Request.QueryString["USERID"].ToString();
            }
            if (Request.QueryString["DECISION"] != null) // DECISION(1:First Period, 0:Second Period)
            {
                this.lblDecision.Text = Request.QueryString["DECISION"].ToString();
            }
            if (Request.QueryString["PROFIT"] != null) // Profit
            {
                this.lblProfit.Text = Request.QueryString["PROFIT"].ToString();
            }
            if (Request.QueryString["EX_SET"] != null)// 실험 SET 번호
            {
                this.hdEX_SET.Text = Request.QueryString["EX_SET"].ToString();
            } 
            
            if (!IsPostBack)
            {
                SetInforBind(int.Parse(this.hdTURN.Text), int.Parse(this.hdEX_SET.Text));
            }

            //First Period로 선택한 경우 -> Result에 관계없이 물건을 가지고 Profit이 생김
            if (this.lblDecision.Text == "1")
            {
                this.lblRESULT1.Text = "You got the product in the first period";
                this.lblRESULT2.Text = "Your Profit is $ " + this.lblProfit.Text;
                if (this.lblResult.Text == "1") //물건이 있어서 Profit이 있는경우
                {
                    //this.lblRESULT3.Text = "If you had decided to wait, you would have gotten the";
                    //this.lblRESULT4.Text = "product in the second period";
                    //this.lblRESULT3.Text = "In the second period";
                    //this.lblRESULT4.Text = "Product was available in the stock";
                    this.lblRESULT3.Text = "Product In Stock";
                    
                }
                else
                {
                    //this.lblRESULT3.Text = "If you had decided to wait, you would have not gotten the";
                    //this.lblRESULT4.Text = "product in the second period";
                    //this.lblRESULT3.Text = "In the second period";
                    //this.lblRESULT4.Text = "Product was stocked out";
                    this.lblRESULT3.Text = "Product Out of Stock";
                }                
            }
            else if (this.lblDecision.Text == "2") //Second Period로 선택한 경우
            {
                //Result에 따라서 Profit이 달라짐
                if (this.lblResult.Text == "1") //물건이 있어서 Profit이 있는경우
                {
                    this.lblRESULT1.Text = "You got the product in the second period";
                    this.lblRESULT2.Text = "Your Profit is $ " + this.lblProfit.Text;

                    //this.lblRESULT3.Text = "In the second period";
                    //this.lblRESULT4.Text = "Product was available in the stock";
                    this.lblRESULT3.Text = "Product In Stock";
                }
                else
                {
                    //this.lblRESULT1.Text = "You didn't get the product in the second period";
                    //this.lblRESULT2.Text = "Your Profit is " + this.lblProfit.Text;
                    this.lblRESULT1.Text = "You didn't get the product in the second period";
                    this.lblRESULT2.Text = "Your Profit is $ " + this.lblProfit.Text;

                    //this.lblRESULT3.Text = "In the second period";
                    //this.lblRESULT4.Text = "Product was stocked out";
                    this.lblRESULT3.Text = "Product Out of Stock";
                }
            }
            else//NoDecision에서 넘어온 경우. //해당 케이스틑 새로운 버전에서는 존재하지 않음. 2014/10/14
            {
                if (this.lblResult.Text == "1") //물건이 있어서 Profit이 있는경우
                {
                    //this.lblRESULT3.Text = "In the second period";
                    //this.lblRESULT4.Text = "Product was available in the stock";
                    this.lblRESULT3.Text = "Product In Stock";

                    this.lblRESULT2.Text = "Not Applicable";
                }
                else
                {
                    //this.lblRESULT3.Text = "In the second period";
                    //this.lblRESULT4.Text = "Product was stocked out";
                    this.lblRESULT3.Text = "Product Out of Stock";
                    this.lblRESULT2.Text = "Not Applicable";
                }   
            }
        }

        /// <summary>
        /// information biding by using LINQ query
        /// </summary>
        /// <param name="n"></param>
        public void SetInforBind(int n, int ex)
        {
            // LINQ query
            //var infos = from p in db.TBInfor2s
            var infos = from p in db.TBInfor2_latests
            //var infos = from p in db.TBInfor2_2As
                        where p.TURN == n
                        where p.EX_SET == ex
                        select p;

            if (infos != null)
            {
                foreach (var v in infos)
                {
                    this.lblK.Text = v.C_K.ToString();      // Number of Products
                    //this.lblN.Text = v.C_N.ToString();      // Number of Customers
                    this.lblN1.Text = v.C_N1.ToString();      // Number of Customers
                    this.lblN2.Text = v.C_N2.ToString();      // Number of Customers
                    this.lblP1.Text = v.C_P1.ToString();    // First period price
                    this.lblP2.Text = v.C_P2.ToString();    // Second period price
                    //this.lblY.Text = v.C_Y.ToString();      // Threshold value
                    this.lblV.Text = v.C_V.ToString();      // Your Retail Price
                    //this.hdGROUPID.Text = v.GROUPID;        // 그룹정보
                    this.lblResult.Text = v.Result.ToString();
                    this.lblIsLast.Text = v.ISLAST.ToString();
                    this.lblGroupID.Text = v.GROUPID;
                    this.lblTrial.Text = v.TRIAL.ToString();//몇번째 Trial인지
                }
            }
        }

        /*
        public int CheckValue(int n, int ex)
        {
            // LINQ query
            //var infos = from p in db.TBInfor2s   // from절
            var infos = from p in db.TBInfor2_latests   // from절
            //var infos = from p in db.TBInfor2_2As   // from절
                        where p.TURN == n        // where절
                        where p.EX_SET == ex
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
        }*/

        protected void Button2_Click(object sender, EventArgs e)
        {            
            int iTurn = int.Parse(this.hdTURN.Text) + 1;

            //last인 경우에는 해당 treatment가 끝났다는것을 보여주는 화면으로 간다.
            //last가 2 인경우에는 실험이 완전 종료되었음을 의미한다.
            if (this.lblIsLast.Text == "1")
            {
                iTurn = iTurn - 1;
                Response.Redirect("GroupEnd.aspx?TURN=" + iTurn.ToString() + "&USERID=" + this.lblUSERID.Text + "&EX_SET=" + this.hdEX_SET.Text);
            }
            else if (this.lblIsLast.Text == "2")
            {
                Response.Redirect("SessionEnd.aspx");
            }
            else //last가 아닌경우에는 Decision Page 또는 No Decision Page로 돌아간다.
            {

                //다음 데이터 셋의 가격에 따라, Decision 또는 NoDecision으로 갈지를 결정 
                // No Decision 사용안함. 2014/10/23

                //if (CheckValue(iTurn) == 1)
                //{
                Response.Redirect("Decision.aspx?TURN=" + iTurn.ToString() + "&USERID=" + this.lblUSERID.Text + "&EX_SET=" + this.hdEX_SET.Text);
                //}
                //else
                //{
                //    Response.Redirect("NoDecision.aspx?TURN=" + iTurn.ToString() + "&USERID=" + this.lblUSERID.Text);
                //}
            }
        }
    }
}