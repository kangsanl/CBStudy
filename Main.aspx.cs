using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CBStudy
{
    public partial class Main : System.Web.UI.Page
    {
        // LINQ 사용을 위한 데이터베이스 클래스 선언
        LINQ.clsPeriodsBuyingDataContext db;

        protected void Page_Load(object sender, EventArgs e)
        {
            db = new LINQ.clsPeriodsBuyingDataContext();

            // 순번 체크하는 부분
            if (Request.QueryString["TURN"] != null)
            {
                this.hdTURN.Text = Request.QueryString["TURN"].ToString();
            }
            else
            {
                this.hdTURN.Text = "1";
            }

            // 학생id가 존재하는 경우에는 readonly로 변경.
            // 존재하지 않으면 입력하도록.
            if (Request.QueryString["USERID"] != null)
            {
                this.txtUSERID.Text = Request.QueryString["USERID"].ToString();
                this.txtUSERID.ReadOnly = true;
                //this.txtUSERID.BackColor = Color.FromArgb(240, 240, 240);
                this.txtUSERID.BackColor = ColorTranslator.FromHtml("#F0F0F0");
            }
            else
            {
                this.txtUSERID.ReadOnly = false;
                //this.txtUSERID.BackColor = Color.FromArgb(255, 255, 255);
                this.txtUSERID.BackColor = ColorTranslator.FromHtml("#FFFFFF");
            }

            if (!IsPostBack) {
                SetInforBind(int.Parse(this.hdTURN.Text));
            }
        }

        /// <summary>
        /// LINQ를 사용하여 데이터 조회
        /// </summary>
        /// <param name="n"></param>
        public void SetInforBind(int n)
        {
            // LINQ query
            var infos = from p in db.TBInfor    // from절
                        where p.TURN == n        // where절
                        select p;               // select절

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
                    this.lblTURN.Text = this.hdTURN.Text;   // 순번
                }
            }

            // LINQ query (max)
            var turnMax = from p in db.TBInfor    // from절
                        where p.GROUPID == "A"    // where절
                        select p.TURN;            // select절

            // 순번 max값을 다른 페이지에서 사용하도록 hidden값으로 구성
            this.hdMAX.Text = turnMax.Max().ToString();
            this.lblTURN.Text = this.lblTURN.Text 
                + "/"
                + this.hdMAX.Text; // 순번/전체순번 ==> 으로 표현
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            // 파라미너 : 순번
            Response.Redirect("Question.aspx?TURN=" + this.hdTURN.Text + "&USERID=" + this.txtUSERID.Text + "&TMAX=" + this.hdMAX.Text);
        }
    }
}
