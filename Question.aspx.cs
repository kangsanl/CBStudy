using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CBStudy
{
    public partial class Question : System.Web.UI.Page
    {
        LINQ.clsPeriodsBuyingDataContext db;

        protected void Page_Load(object sender, EventArgs e)
        {
            db = new LINQ.clsPeriodsBuyingDataContext();

            if (Request.QueryString["TURN"] != null)// 순번
            {
                this.hdTURN.Text = Request.QueryString["TURN"].ToString();
            }
            else
            {
                this.hdTURN.Text = "1";
            }

            if (Request.QueryString["USERID"] != null)// 학생ID
            {
                this.lblUSERID.Text = Request.QueryString["USERID"].ToString();
            }
            if (Request.QueryString["TMAX"] != null)// 전체순번
            {
                this.hdMAX.Text = Request.QueryString["TMAX"].ToString();
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
                    this.lblY.Text = v.C_Y.ToString();      // Simulation value
                    this.lblV.Text = v.C_V.ToString();      // Your Retail Price
                    this.hdGROUPID.Text = v.GROUPID;        // 그룹정보
                    this.lblTURN.Text = this.hdTURN.Text 
                        + "/" 
                        + this.hdMAX.Text;   // 순번/전체순번
                }
            }
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            // 시뮬레이션 실행
            Simulation();
        }

        private void Simulation()
        {
            int K = int.Parse(lblK.Text.ToString());
            int N = int.Parse(lblN.Text.ToString());
            int P1 = int.Parse(lblP1.Text.ToString());
            int P2 = int.Parse(lblP2.Text.ToString());
            int Y = int.Parse(lblY.Text.ToString());
            int V = int.Parse(lblV.Text.ToString());

            Random rRand1 = new Random();
            double value = 0;
            int profit = 0;
            int iCustBuyFirstPeriod = 0;
            int iCustBuySecondPeriod = 0;

            //시뮬레이션 로직 1
            //여기서 value란 retail price를 의미함. 
            for (int i = 1; i < N; i++)
            {
                value = rRand1.NextDouble() * 100;

                if (value >= Y)
                {
                    iCustBuyFirstPeriod++;
                }
                else if (value >= P1)
                {
                    iCustBuySecondPeriod++;
                }
            }

            bool bGetProduct = true; // decision condition


            /*
            if (this.rbDecision1.Checked)
            {
                bGetProduct = true;
            }
            else
            {
             */
            //시뮬레이션 로직 2
            if (iCustBuyFirstPeriod >= K)
            {
                profit = 0;
                bGetProduct = false;
            }
            else
            {
                //두번째 시기에 사려고하는 고객이 상품 수 - 첫번째 시기보다 많은 경우에는 추첨을 통하여 상품을 갖는다                 
                //if (iCustBuySecondPeriod > K - 1 - iCustBuyFirstPeriod)
                if (iCustBuySecondPeriod + 1 >= K - iCustBuyFirstPeriod)
                {

                    //if (rRand1.NextDouble() <= (K - 1 - iCustBuyFirstPeriod) / iCustBuySecondPeriod)
                    if (rRand1.NextDouble() <= (K - iCustBuyFirstPeriod) / (iCustBuySecondPeriod + 1))
                    {
                        profit = V - P2;
                        bGetProduct = true;
                    }
                    else
                    {
                        profit = 0;
                        bGetProduct = false;
                    }
                }
                else
                {
                    profit = V - P2;
                    bGetProduct = true;
                }
            }
            //}

            /*
            if (bGetProduct)
            {
                lbResult1.Text = "You got the product";
            }
            else
            {
                lbResult1.Text = "You didn't get the product";
            }

            lbResult2.Text = "Your profit is " + profit.ToString();

            //int TotalProfit = int.Parse(tbTotalProfit.Text.ToString()) + profit;
            int TotalProfit = int.Parse(lbTotalProfit.Text.ToString()) + profit;
            //this.tbTotalProfit.Text = TotalProfit.ToString();
            this.lbTotalProfit.Text = TotalProfit.ToString();
             */

            if (this.rbDecision1.Checked)
            {
                // 시뮬레이션 결과 저장 (first인 경우 계산식 = Your Retail Price - First period price)
                int fProfit = int.Parse(this.lblV.Text) - int.Parse(this.lblP1.Text);

                //Create(fProfit);
                Create(fProfit, bGetProduct);
                // 파라미터 : 순번
                Response.Redirect("Firstresult.aspx?TURN=" + this.hdTURN.Text + "&USERID=" + this.lblUSERID.Text + "&TMAX=" + this.hdMAX.Text + "&PRODUCTYN=" + bGetProduct.ToString());
            }
            else
            {
                // 시뮬레이션 결과 저장
                //Create(profit);
                Create(profit, bGetProduct);

                // 파라미터 : 이익
                Response.Redirect("Secondresult.aspx?PROFIT=" + profit.ToString() + "&TURN=" + this.hdTURN.Text + "&USERID=" + this.lblUSERID.Text + "&TMAX=" + this.hdMAX.Text);
            }
        }


        /// <summary>
        /// LINQ를 사용하여 데이터 저장
        /// </summary>
        //public void Create(int profit)
        public void Create(int profit, bool bGetProduct)
        {
            // Create a new TBInfor object.
            LINQ.TBSimulation info = new LINQ.TBSimulation
            {
                USERID = this.lblUSERID.Text,                   // 학생ID
                DECISION = this.rbDecision1.Checked ? "1" : "2",// first period : 1, second period : 2
                //RESULTYN = profit > 0 ? "1" : "2",              // 결과값을 얻었으면 : 1, 그렇지 않으면 : 2
                RESULTYN = bGetProduct ? "1" : "2",              // 결과값을 얻었으면 : 1, 그렇지 않으면 : 2
                RESULT = profit,                    // 결과값
                TURN = int.Parse(this.hdTURN.Text), // 순번
                GROUPID = this.hdGROUPID.Text       // 그룹정보
            };

            // Add the new object to the Orders collection.
            db.TBSimulation.InsertOnSubmit(info);

            // Submit the change to the database.
            try
            {
                db.SubmitChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                // Make some adjustments.
                // Try again.
                db.SubmitChanges();
            }

        }
    }
}
