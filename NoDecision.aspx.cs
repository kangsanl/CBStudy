using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CBStudy
{
    public partial class NoDecision : System.Web.UI.Page
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

            if (!IsPostBack)
            {
                SetInforBind(int.Parse(this.hdTURN.Text));
            }

            this.lblRESULT1.Text = "Price Below First Period Price: No Decision Needed";
        }

        /// <summary>
        /// information biding by using LINQ query
        /// </summary>
        /// <param name="n"></param>
        public void SetInforBind(int n)
        {
            // LINQ query
            var infos = from p in db.TBInfor2s
            //var infos = from p in db.TBInfor2_2As
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
                    this.lblY.Text = v.C_Y.ToString();      // Threshold value
                    this.lblV.Text = v.C_V.ToString();      // Your Retail Price
                    this.hdGROUPID.Text = v.GROUPID;        // 그룹정보
                    this.lblResult.Text = v.Result.ToString();
                    this.lblGroupID.Text = v.GROUPID;
                    this.lblTrial.Text = v.TRIAL.ToString();//몇번째 Trial인지
                }
            }
        }

        /// <summary>
        /// LINQ를 사용하여 데이터 저장.
        /// 설령 선택의 여지가 없더라고 데이터는 저장한다.
        /// </summary>
        //public void Create(int profit)
        public void Create()
        {
            // Create a new TBInfor object.
            LINQ.TBSimulation2 info = new LINQ.TBSimulation2
            {
                USERID = this.lblUSERID.Text,                   // 학생ID
                DECISION = "0", // Decision이 필요없으므로 0로 입력한다.
                RESULTYN = "0",              // 결과값을 얻었으면 : 1, 그렇지 않으면 : 2
                RESULT = 0,                    // 결과값
                TURN = int.Parse(this.hdTURN.Text), // 순번
                GROUPID = this.hdGROUPID.Text,       // 그룹정보
                CONFIDENCE = 0, //Confidence 도 입력할 필요가 없으므로 0로 입력
                TRIAL = int.Parse(this.lblTrial.Text),
                TIME = System.DateTime.Now.ToString()
            };

            // Add the new object to the Orders collection.
            db.TBSimulation2s.InsertOnSubmit(info);

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

        protected void Button2_Click(object sender, EventArgs e)
        {
            //결과값을 DB에 저장
            Create();

            string DecisionPeriod = "0";
            Response.Redirect("Result.aspx?TURN=" + this.hdTURN.Text + "&USERID=" + this.lblUSERID.Text + "&DECISION=" + DecisionPeriod);
        }
    }

    

}