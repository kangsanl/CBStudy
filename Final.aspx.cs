﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CBStudy
{
    public partial class Final : System.Web.UI.Page
    {
        LINQ.clsPeriodsBuyingDataContext db;

        protected void Page_Load(object sender, EventArgs e)
        {
            db = new LINQ.clsPeriodsBuyingDataContext();
            if (Request.QueryString["TURN"] != null) // 순번
            {
                this.hdTURN.Text= Request.QueryString["TURN"].ToString();
            }
            if (Request.QueryString["TMAX"] != null) // 전체순번
            {
                this.hdMAX.Text = Request.QueryString["TMAX"].ToString();
            }
            if (Request.QueryString["USERID"] != null) // 학생ID
            {
                this.hdUSERID.Text = Request.QueryString["USERID"].ToString();
            } 
 
            if (!IsPostBack)
            {
                SetInforBind();
            }
        }

        /// <summary>
        /// information biding by using LINQ query
        /// </summary>
        /// <param name="n"></param>
        //public void SetInforBind(int n)
        public void SetInforBind()
        {
            //// LINQ query
            //var infos = from p in db.TBInfor
            //            where p.TURN == n
            //            select p;

            //if (infos != null)
            //{
            //    foreach (var v in infos)
            //    {
            //        this.lblK.Text = v.C_K.ToString();      // Number of Products
            //        this.lblN.Text = v.C_N.ToString();      // Number of Customers
            //        this.lblP1.Text = v.C_P1.ToString();    // First period price
            //        this.lblP2.Text = v.C_P2.ToString();    // Second period price                    
            //        this.lblV.Text = v.C_V.ToString();      // Your Retail Price
            //        this.lblTURN.Text = this.hdTURN.Text;   // 순번
            //    }
            //}

            ////int iProfit = 0;
            //// profit = Your Retail Price - First period price
            ////iProfit = int.Parse(this.lblV.Text) - int.Parse(this.lblP1.Text);
            ////this.lblPROFIT.Text = iProfit.ToString();
            ////this.lblPROFIT.Text = iProfit.ToString();

            // 2011/08/03 - LINQ query (SUM)
            var tSum = from p in db.TBSimulation    // from절
                       where p.USERID == this.hdUSERID.Text
                            && p.GROUPID == "A"     // where절
                       select p.RESULT;             // select절

            // Total Profit
            this.lblTPROFIT.Text = tSum.Sum().ToString();
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            /*
            int iTurn = 0;

            // 전체순번과 현재순번+1이 같으면 종료
            if (int.Parse(this.hdMAX.Text) == int.Parse(this.hdTURN.Text) + 1)
            {
                Response.Redirect("Final.aspx");
            }
            else
            {
                iTurn = int.Parse(this.hdTURN.Text) + 1;
                // Next 버튼 클릭시 첫 화면(Information)으로 이동
                // 파라미터 : 순번,학생ID
                Response.Redirect("Main.aspx?TURN=" + iTurn.ToString() + "&USERID=" + this.hdUSERID.Text);
            }
             */
        }
    }
}
