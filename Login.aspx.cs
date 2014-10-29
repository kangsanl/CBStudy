using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CBStudy
{
    public class returnJson {
        public int currentUserNum { get; set; }
        public bool redirect { get; set; }

        internal string ToJSON()
        {
            throw new NotImplementedException();
        }
    }

    public partial class Login : System.Web.UI.Page
    {
        // LINQ 사용을 위한 데이터베이스 클래스 선언
        LINQ.clsPeriodsBuyingDataContext db;
        static int count = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            db = new LINQ.clsPeriodsBuyingDataContext();

            if (Request.QueryString["EX_SET"] != null)// 실험 SET 번호
            {
                this.hdEX_SET.Text = Request.QueryString["EX_SET"].ToString();
            }

            Login.count++;

            

        }

        /// <summary>
        /// LINQ를 사용하여 데이터 조회
        /// </summary>
        /// <param name="n"></param>
        /// 2012.04.04 SetInforBind seems to be not used
        //public void SetInforBind(int n)
        //{
        //    // LINQ query
        //    var infos = from p in db.TBInfor2s    // from절
        //                where p.TURN == n        // where절
        //                select p;               // select절

        //    if (infos != null)
        //    {
        //        foreach (var v in infos)
        //        {
        //            //this.lblK.Text = v.C_K.ToString();      // Number of Products
        //            //this.lblN.Text = v.C_N.ToString();      // Number of Customers
        //            //this.lblP1.Text = v.C_P1.ToString();    // First period price
        //            //this.lblP2.Text = v.C_P2.ToString();    // Second period price
        //            ////this.lblV.Text = v.C_Y.ToString();      // Your Retail Price
        //            //this.lblV.Text = v.C_V.ToString();      // Your Retail Price
        //            //this.lblTURN.Text = this.hdTURN.Text;   // 순번
        //        }
        //    }

        //    //// LINQ query (max)
        //    //var turnMax = from p in db.TBInfor    // from절
        //    //              where p.GROUPID == "A"    // where절
        //    //              select p.TURN;            // select절

        //}
        
        /*
       /// <summary>
       /// Value의 크기에 따라 다른 화면으로 Redirect하기 위한 함수
       /// </summary>
       /// <param name="n"></param>
 

        public int CheckValue(int n)
        {
            // LINQ query
            var infos = from p in db.TBInfor2s   // from절
            //var infos = from p in db.TBInfor2_2As   // from절
                        where p.TURN == n        // where절
                        select p;               // select절

            int ValueType;
            int Val=0;
            int P1=0;
            int P2=0;

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

            if(Val > P1)
            {
                ValueType = 1;
            }
            else
            {
                ValueType = 2;
            }

            return ValueType;
        }*/



        protected void Button1_Click(object sender, EventArgs e)
        { 
            // No Decision 사용안함. 2014/10/23
            
            //if(CheckValue(1)==1)
            //{
                Response.Redirect("Decision.aspx?TURN=1" + "&USERID=" + this.txtUSERID.Text + "&EX_SET=" + this.hdEX_SET.Text);
            //}
            //else
            //{
            //    Response.Redirect("NoDecision.aspx?TURN=1" + "&USERID=" + this.txtUSERID.Text + "&EX_SET=" + this.hdEX_SET.Text);
            //}            
        }


        [System.Web.Services.WebMethod]
        public static string GetPageStatus(string set, string turn, string groupId)
        {


            JavaScriptSerializer serializer = new JavaScriptSerializer();
            bool redirect = false;
            if(Login.count == 2){
                redirect = true;
                
            }
            else
            {
                
                redirect = false;
            }

            returnJson result = new returnJson { currentUserNum = Login.count, redirect = redirect };


            return serializer.Serialize(result);
        }

    }
}