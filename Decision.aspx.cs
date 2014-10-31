using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CBStudy
{

    /*
     *  returnJson
     *  
     *  Create JSON string from server to send info to client-side
     *  memebers
     *  currentUserNum  :   the current user on the current turn of a session(EX)
     *  redirect        :   let the client knows if all users are ready to start the current turn
     */
    public class returnJson
    {
        public int currentUserNum { get; set; }
        public bool redirect { get; set; }
    }

    public partial class Decision : System.Web.UI.Page
    {   
        LINQ.clsPeriodsBuyingDataContext db;

        
        /*
         *  maxUserNum
         *  {EX_SET id, maxUserNum}
         */
        static private Hashtable maxUserNum = new Hashtable(){
            {1, 3},{2, 3},{3, 3},{4, 3},{5, 3},{6, 3}
        };

        /*
         * ex_setTable
         * key  :   EX_SET
         * value:   Hashtable<SessionId,Hashtable>
         */
        static public Hashtable ex_setTable = new Hashtable();





        /*
         * GetPageStatus
         * check/respond if the users are ready to play the turn
         * 
         * Params
         * set : the current set
         * turn : the current turn
         * 
         * return
         * the notification to the client from AJAX with JSON data 
         */
        [System.Web.Services.WebMethod]
        public static string GetPageStatus(string set, string session, string turn)
        {


            JavaScriptSerializer serializer = new JavaScriptSerializer();
            bool redirect = false;
            int turnNum = Int32.Parse(turn);
            int ex_setId = Int32.Parse(set);
            int sessionId = Int32.Parse(session);
            returnJson result = null;



            Hashtable sessionTable = (Hashtable)ex_setTable[ex_setId];
            Hashtable currentUserNum = (Hashtable)sessionTable[sessionId];

            if (maxUserNum.ContainsKey(ex_setId) && currentUserNum.ContainsKey(turnNum))
            {
                if ((int)maxUserNum[ex_setId] <= (int)currentUserNum[turnNum])
                {
                    redirect = true;
                }
                else
                {

                    redirect = false;
                }

                result = new returnJson { currentUserNum = (int)currentUserNum[turnNum], redirect = redirect };
                
            }


            return serializer.Serialize(result);
        }

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

            if (!IsPostBack)
            {
                SetInforBind(int.Parse(this.hdTURN.Text),int.Parse(this.hdEX_SET.Text));

                //if (this.hdGROUPID.Text == "1" || this.hdGROUPID.Text == "4" || this.hdGROUPID.Text == "0")
                //{
                //    this.lblConfidence.Visible = true;
                //    //this.dbConfidence.Visible = true;
                //    this.rbConfidence1.Visible = true;
                //    this.rbConfidence2.Visible = true;
                //    this.rbConfidence3.Visible = true;
                //    this.rbConfidence4.Visible = true;
                //    this.rbConfidence5.Visible = true;
                //}
                //else
                //{
                //    this.lblConfidence.Visible = false;
                //    //this.dbConfidence.Visible = false;
                //    this.rbConfidence1.Visible = false;
                //    this.rbConfidence2.Visible = false;
                //    this.rbConfidence3.Visible = false;
                //    this.rbConfidence4.Visible = false;
                //    this.rbConfidence5.Visible = false;
                //}

                //not going to check the confidence level. 2012.04.05
                /*
                this.lblConfidence.Visible = false;
                //this.dbConfidence.Visible = false;
                this.rbConfidence1.Visible = false;
                this.rbConfidence2.Visible = false;
                this.rbConfidence3.Visible = false;
                this.rbConfidence4.Visible = false;
                this.rbConfidence5.Visible = false;*/
            }



            if (!ex_setTable.ContainsKey(int.Parse(this.hdEX_SET.Text)))
            {
                ex_setTable.Add(int.Parse(this.hdEX_SET.Text), new Hashtable());
            }

            Hashtable sessionTable = (Hashtable)ex_setTable[int.Parse(this.hdEX_SET.Text)];
            if(!sessionTable.ContainsKey(int.Parse(this.lblGroupID.Text))){
                sessionTable.Add(int.Parse(this.lblGroupID.Text), new Hashtable());
            }

            Hashtable currentUserNum = (Hashtable)sessionTable[int.Parse(this.lblGroupID.Text)];
            //If it is the 1st user
            if (!currentUserNum.ContainsKey(int.Parse(this.hdTURN.Text)))
            {
                currentUserNum[int.Parse(this.hdTURN.Text)] = 1;
            }
            else
            {
                currentUserNum[int.Parse(this.hdTURN.Text)] = (int)currentUserNum[int.Parse(this.hdTURN.Text)] + 1;
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
            
                        where p.TURN == n
                        where p.EX_SET == ex
                        select p;

            if (infos != null)
            {
                foreach (var v in infos)
                {
                    this.lblK.Text = v.C_K.ToString();      // Number of Products
                    this.lblN1.Text = v.C_N1.ToString();      // Number of Customers
                    this.lblN2.Text = v.C_N2.ToString();      // Number of Customers
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
        /// LINQ를 사용하여 데이터 저장
        /// </summary>
        //public void Create(int profit)
        public void Create(int profit)
        {
            /*int confidence_level = 0;

            //Confidence level을 입력받는다
            if (this.rbConfidence1.Checked)
            {
                confidence_level = 5;
            }
            else if (this.rbConfidence2.Checked)
            {
                confidence_level = 4;
            }
            else if (this.rbConfidence3.Checked)
            {
                confidence_level = 3;
            }
            else if (this.rbConfidence4.Checked)
            {
                confidence_level = 2;
            }
            else if (this.rbConfidence5.Checked)
            {
                confidence_level = 1;
            }*/

            // Create a new TBInfor object.
            LINQ.TBSimulation2 info = new LINQ.TBSimulation2
            {
                USERID = this.lblUSERID.Text,                   // 학생ID
                DECISION = this.rbDecision1.Checked ? "1" : "2",// first period : 1, second period : 2
                //RESULTYN = profit > 0 ? "1" : "2",              // 결과값을 얻었으면 : 1, 그렇지 않으면 : 2
                RESULTYN = lblResult.Text.ToString(),              // 결과값을 얻었으면 : 1, 그렇지 않으면 : 2
                RESULT = profit,                    // 결과값
                TURN = int.Parse(this.hdTURN.Text), // 순번
                GROUPID = this.hdGROUPID.Text,       // 그룹정보
                //CONFIDENCE = int.Parse(this.dbConfidence.SelectedItem.Value.ToString())                
                //CONFIDENCE = confidence_level,
                TRIAL = int.Parse(this.lblTrial.Text),
                TIME = System.DateTime.Now.ToString(),
                EX_SET = int.Parse(this.hdEX_SET.Text)
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
            int profit;

            if (this.rbDecision1.Checked)
            {
                profit = int.Parse(this.lblV.Text) - int.Parse(this.lblP1.Text);
            }
            else
            {

                if (lblResult.Text == "1")
                {
                    //Result가 1일 경우에는 Second Period에 물건을 가지게 됨
                    profit = int.Parse(this.lblV.Text) - int.Parse(this.lblP2.Text);
                }
                else
                {
                    //Result가 0일 경우에는 Second Period에 가지지 못하므로 Profit = 0
                    profit = 0;
                }
            }

            //결과값을 DB에 저장
            Create(profit);

            string DecisionPeriod;

            if (this.rbDecision1.Checked)
            {
                DecisionPeriod = "1";
                
            }
            else
            {
                DecisionPeriod = "2";
            }

            Response.Redirect("Result.aspx?TURN=" + this.hdTURN.Text + "&USERID=" + this.lblUSERID.Text + "&DECISION=" + DecisionPeriod + "&PROFIT=" + profit.ToString() + "&EX_SET=" + this.hdEX_SET.Text);
        }
    }
}