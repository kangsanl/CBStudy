using System;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using Framework.DBCore;
//using Framework.Common;
//using Microsoft.ApplicationBlocks.ExceptionManagement; // EMAB (Exception Management Application Block)
//using Microsoft.Practices.EnterpriseLibrary.Data;                        // DAAB (Data Access Application Block)
//using Microsoft.Practices.EnterpriseLibrary.Common.Configuration; // DAAB Database 클래스 참고 DLL


namespace CBStudy
{
    /// <summary>    
    /// 작성일 : 2009/03/16
    /// 작성자 : bhchoi
    /// 내  용 : DB 연결 및 실행 클래스 (표준화대상)
    /// </summary>    
    public class _clsSample : _SqlHelper
    {
        // SqlConnection String 문자값
        public static string INPUT_CONN_STRING ;

        public _clsSample() : base()
		{
        }

        /// <summary>
        /// 저장프로시져의 Select 문장에 의한 DataSet 리턴
        /// </summary>
        /// <param name="storedProcName">프로시저명</param>
        /// <param name="sqlParams">프로시저 파라미터 정의</param>
        /// <returns>DataSet</returns>
        public DataSet ExecuteDataset(string storedProcName, params SqlParameter[] sqlParams)
        {
            DataSet ds = new DataSet();

            try
            {
                this.DBConnection(false);
                ds = _SqlHelper.ExecuteDataset(_SqlHelper.CONN_STRING_NON_DTC, storedProcName, sqlParams);
            }
            catch
            {
                ds.Dispose();
                // Event 로그 생성
                //Common.ExceptionMessage(ex);
            }
            finally
            {
                this.DBConnClose();
                ds.Dispose();
            }

            return ds;
        }

        /// <summary>
        /// 저장프로시져의 Select 문장에 의한 Scalar 리턴
        /// </summary>
        /// <param name="storedProcName">프로시저명</param>
        /// <param name="sqlParams">프로시저 파라미터 정의</param>
        /// <returns>string</returns>
        public object ExecuteScalar(string storedProcName, params SqlParameter[] sqlParams)
        {
            object obj = null;
            try
            {
                this.DBConnection(false);
                obj = _SqlHelper.ExecuteScalar(_SqlHelper.CONN_STRING_NON_DTC, storedProcName, sqlParams);
            }
            catch
            {
                // Event 로그 생성
                //Common.ExceptionMessage(ex);
            }
            finally
            {
                this.DBConnClose(); 
            }

            return obj;
        }

        /// <summary>
        /// 저장프로시져의 insert,update,delete 데이터 처리
        /// </summary>
        /// <param name="storedProcName">프로시저명</param>
        /// <param name="sqlParams">파라미터</param>
        public int ExecuteNonQuery(string storedProcName, params SqlParameter[] sqlParams)
        {
            int reVal = 0;

            try
            {
                // 트랜젝션 처리를 하기 위해 "this.DBConnection(true)"를 사용함.
                // DBConnection Open - Transaction
                this.DBConnection(true);

                reVal = _SqlHelper.ExecuteNonQuery(this.SqlTx, CommandType.StoredProcedure, storedProcName, sqlParams);

                // Transaction Commit
                this.DBSetComplete();
            }
            catch
            {
                // Transaction Rollback
                this.DBSetAbort();

                // Event 로그 생성
                //Common.ExceptionMessage(ex);

                reVal = -1;
            }
            finally
            {
                // DBConnetion Close
                this.DBConnClose();
            }

            return reVal;
        }

        /// <summary>
        /// 저장프로시져의 insert,update,delete 데이터 처리 (트랜젝션을 사용하지 않음.)
        /// </summary>
        /// <param name="storedProcName">프로시저명</param>
        /// <param name="sqlParams">파라미터</param>
        public int ExecuteNonQuery(string storedProcName, bool nonTx, params SqlParameter[] sqlParams)
        {
            int reVal = 0;

            try
            {
                // DBConnection Open - Non Transaction
                reVal = _SqlHelper.ExecuteNonQuery(_SqlHelper.CONN_STRING_NON_DTC, storedProcName, sqlParams);
            }
            catch
            {
                // Event 로그 생성
                //Common.ExceptionMessage(ex);
            }
            finally
            {
            }

            return reVal;
        }

        /// <summary>
        /// 작성일 : 2009/03/23
        /// 작성자 : bhchoi
        /// <remarks>저장프로시져의 Select 문장에 의한 SqlDataReader 리턴</remarks>
        /// </summary>
        /// <param name="storedProcName">프로시저명</param>
        /// <param name="sqlParams">프로시저 파라미터 정의</param>
        /// <returns>SqlDataReader</returns>
        public SqlDataReader ExecuteReader(string storedProcName, params SqlParameter[] sqlParams)
        {
            SqlDataReader sqlDR = null;

            try
            {
                this.DBConnection(false);
                sqlDR = _SqlHelper.ExecuteReader(_SqlHelper.CONN_STRING_NON_DTC, storedProcName, sqlParams);
            }
            catch
            {
                sqlDR.Dispose();
                // Event 로그 생성
                //Common.ExceptionMessage(ex);
            }
            finally
            {
                this.DBConnClose();
            }

            return sqlDR;
        }

        /// <summary>
        /// 작성일 : 2009/03/25
        /// 작성자 : bhchoi
        /// <remarks>기타 외부 데이터 가져오기에서 타서버에 있는 데이터 조회.</remarks>
        /// </summary>
        /// <param name="conStr">Connection String</param>
        /// <returns>DataTable</returns>
        public DataTable ExecuteScalar_Query(string conStr, string sSql)
        {
            DataTable dtTable = null;
            try
            {
                this.DBConnection(false);
                dtTable = _SqlHelper.ExecuteDataset(conStr, CommandType.Text, sSql).Tables[0];
            }
            catch
            {
                // Event 로그 생성
                //Common.ExceptionMessage(ex);
            }
            finally
            {
                this.DBConnClose();
            }

            return dtTable;
        }

        /// <summary>
        /// 작성일 : 2009/03/23
        /// 작성자 : bhchoi
        /// <remarks>데이터베이스 목록 반환.</remarks>
        /// </summary>
        /// <param name="conStr">Connection String</param>
        /// <returns>DataTable</returns>
        public DataTable GetDataBaseList(string conStr)
        {
            DataTable dtTable = null ;
            try
            {
                using (SqlConnection sqlConx = new SqlConnection(conStr))
                {
                    sqlConx.Open();
                    dtTable = sqlConx.GetSchema("Databases");
                    sqlConx.Close();
                }
            }
            catch
            {
                //Common.ExceptionMessage(ex);
            }
            finally
            {
            }

            return dtTable;
        }

        /// <summary>
        /// 작성일 : 2009/03/23
        /// 작성자 : bhchoi
        /// <remarks>테이블 목록 반환.</remarks>
        /// </summary>
        /// <param name="conStr">Connection String</param>
        /// <returns>DataTable</returns>
        public DataTable GetTableList(string conStr)
        {
            DataTable dtTable = null;

            try
            {
                using (SqlConnection sqlConx = new SqlConnection(conStr))
                {
                    sqlConx.Open();
                    dtTable = sqlConx.GetSchema("Tables");
                    sqlConx.Close();
                }
            }
            catch
            {
                //Common.ExceptionMessage(ex);
            }
            finally
            {
            }

            return dtTable;
        }

        /// <summary>
        /// 작성일 : 2009/03/23
        /// 작성자 : bhchoi
        /// <remarks>컬럼 목록 반환.</remarks>
        /// </summary>
        /// <param name="conStr">Connection String</param>
        /// <param name="tableName">테이블명</param>
        /// <returns>DataTable</returns>
        public DataTable GetColumnList(string conStr, string tableName)
        {
            DataTable dtTable = null;

            try
            {
                using (SqlConnection sqlConx = new SqlConnection(conStr))
                {
                    sqlConx.Open();
                    dtTable = sqlConx.GetSchema("Columns", new string[4] { null, null, tableName, null });
                    sqlConx.Close();
                }
            }
            //catch (Exception ex)
            catch
            {
                //Common.ExceptionMessage(ex);
            }
            finally
            {
            }

            return dtTable;
        }

        #region Dispose() 관리되지 않는 리소스의 확보, 해제 또는 다시 설정과 관련된 응용 프로그램 정의 작업을 수행합니다.
        // Pointer to an external unmanaged resource.
        private IntPtr handle;

        // Other managed resource this class uses.
        private Component component = new Component();

        // Track whether Dispose has been called.
        private bool disposed = false;

        // The class constructor.
        public _clsSample(IntPtr handle)
        {
            this.handle = handle;
        }

        // Implement IDisposable.
        // Do not make this method virtual.
        // A derived class should not be able to override this method.
        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        // Dispose(bool disposing) executes in two distinct scenarios.
        // If disposing equals true, the method has been called directly
        // or indirectly by a user's code. Managed and unmanaged resources
        // can be disposed.
        // If disposing equals false, the method has been called by the
        // runtime from inside the finalizer and you should not reference
        // other objects. Only unmanaged resources can be disposed.
        private void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if(!this.disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if(disposing)
                {
                    // Dispose managed resources.
                    component.Dispose();
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                // If disposing is false,
                // only the following code is executed.
                CloseHandle(handle);
                handle = IntPtr.Zero;

                // Note disposing has been done.
                disposed = true;

            }
        }

         // Use interop to call the method necessary
        // to clean up the unmanaged resource.
        [System.Runtime.InteropServices.DllImport("Kernel32")]
        private extern static Boolean CloseHandle(IntPtr handle);

        // Use C# destructor syntax for finalization code.
        // This destructor will run only if the Dispose method
        // does not get called.
        // It gives your base class the opportunity to finalize.
        // Do not provide destructors in types derived from this class.
        ~_clsSample()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion

    }
}
