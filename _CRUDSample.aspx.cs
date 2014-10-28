using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CBStudy
{
    public partial class _CRUDSample : System.Web.UI.Page
    {
        _clsSample _cls = null;
        protected void Page_Load(object sender, EventArgs e)
        {
        }
        
        /// <summary>
        /// 저장/수정시 ExecuteNonQuery
        /// </summary>
        /// <returns></returns>
        private int Create()
        {
            int STDID = 0;

            try
            {
                SqlParameter[] arParms = new SqlParameter[3];

                arParms[0] = new SqlParameter("@STDID", SqlDbType.Int);
                arParms[1] = new SqlParameter("@STDNAME", SqlDbType.NVarChar, 100);
                arParms[2] = new SqlParameter("@RETURNKEY", SqlDbType.Int);

                arParms[0] = new SqlParameter("@STDID", 0);
                arParms[1] = new SqlParameter("@STDNAME", "");
                arParms[2].Direction = ParameterDirection.Output;

                _cls.ExecuteNonQuery("USP700_01CU", arParms);

                STDID = Convert.ToInt32(arParms[2].Value);
            }
            catch
            //catch (Exception ex)
            {
                //이벤트 뷰어에 상세 에러 메시지를 등록시키며, MessageBox로 에러메시지 반환
                //MessageBox.Show(ex.Message);
            }

            return STDID;
        }

        /// <summary>
        /// 조회시 ExecuteDataset
        /// </summary>
        private void Select()
        {
            /*
            DataSet rtnDS = new DataSet();

            SqlParameter[] arParams = new SqlParameter[1];
            arParams[0] = new SqlParameter("STDNAME", "");

            rtnDS = _cls.ExecuteDataset("USP700_04R", arParams);

            cboSTDNAME.DataSource = rtnDS.Tables[0]; // 콤보박스 바인딩
            cboSTDNAME.ValueMember = "STDNAME";
            cboSTDNAME.DisplayMember = "STDNAME";
             */
        }
    }
}