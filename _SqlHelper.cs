using System;
using System.Data;
using System.Xml;
using System.Data.SqlClient;
using System.Collections;
using System.Configuration;
//using Microsoft.ApplicationBlocks.ExceptionManagement;

namespace CBStudy
{
	/// <summary>
	/// SqlHelper 클래스는 SqlClient의 일반적 활용을 위한 효과적인 고성능 확장 기술을 캡술화한다.
    /// - sealed -
    /// 클래스에 적용될 경우 sealed 한정자는 다른 클래스가 해당 클래스에서 상속하지 않도록 합니다. 
    /// 다음 예제에서 B 클래스는 A 클래스에서 상속하지만 B 클래스에서 상속할 수 있는 클래스는 없습니다
	/// </summary>
	//public sealed class SqlHelper
	public class _SqlHelper
	{
		// SqlConnection String 문자값
        public static readonly string CONN_STRING_NON_DTC = ConfigurationManager.AppSettings["ConnectionString"];
        	
		// Hashtable to store cached parameters
		private static Hashtable parmCache = Hashtable.Synchronized(new Hashtable());

        #region 2009/03/12 - DB연결(Transaction) 처리 및 연결정보 설정 by 최보현

        private SqlConnection _sqlConn = null;
        private SqlTransaction _sqlTx = null;        
   
        /// <summary>
        /// SQL Connection 속성
        /// </summary>
        protected SqlConnection SqlConn
        {
            get
            {
                return _sqlConn;
            }
            set
            {
                _sqlConn = value;
            }
        }

        /// <summary>
        /// SQL Transaction 속성
        /// </summary>
        protected SqlTransaction SqlTx
        {
            get
            {
                return _sqlTx;
            }
            set
            {
                _sqlTx = value;
            }
        }

        /// <summary>
        /// Transaction을 사용하기 위한 DB 연결정보
        /// </summary>
        /// <param name="sqlTx">트랜젝션 여부</param>
        protected void DBConnection(bool sqlTx)
        {
            try
            {
                _sqlConn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);
                _sqlConn.Open();
                if (sqlTx)
                {
                    _sqlTx = _sqlConn.BeginTransaction();
                }
            }
            catch
            {
                //ExceptionManager.Publish(ex);
            }
            finally
            {
            }
        }

        /// <summary>
        /// SQL Connection 닫기
        /// </summary>
        protected void DBConnClose()
        {
            if (_sqlConn != null)
            {
                if (_sqlConn.State != ConnectionState.Closed)
                {
                    if (_sqlTx != null)
                    {
                        _sqlTx.Dispose();
                        _sqlTx = null;
                    }
                    _sqlConn.Close();
                    _sqlConn = null;
                }
            }
        }

        /// <summary>
        /// Transaction Rollback 
        /// </summary>
        protected void DBSetAbort()
        {
            if (_sqlTx != null)
            {
                if (_sqlTx.Connection != null && _sqlTx.Connection.State != ConnectionState.Closed)
                {
                    _sqlTx.Rollback();
                }
            }
        }

        /// <summary>
        /// Transaction Commit
        /// </summary>
        protected void DBSetComplete()
        {
            if (_sqlTx != null)
            {
                if (_sqlTx.Connection != null && _sqlTx.Connection.State != ConnectionState.Closed)
                {
                    _sqlTx.Commit();
                }
            }
        }

        #endregion
 
		#region private utility methods & constructors

		// Since this class provides only static methods, make the default constructor private to prevent instances from being created with "new SqlHelper()"
		// 이 클래스는 static 메소드만 제공하기 때문에 디폴트 생성자를 private로 만들어 "new SqlHelper()"의 새로운 인스턴스 생성을 막는다.
        // 2009/03/10 - Business Class 상속을 받기 위해 주석처리 함. by bhchoi
		// private SqlHelper() { }

		/// <summary>
		/// This method is used to attach array of SqlParameters to a SqlCommand.
		/// 이 메소드는 SqlParameter의 배열을 SqlCommand로 전환한다.
		/// 
		/// This method will assign a value of DbNull to any parameter with a direction of
		/// InputOutput and a value of null.  
		/// 이 메소드는 변수에 DbNull 값과 InputOutput 값을 지정한다.
		/// 
		/// This behavior will prevent default values from being used, but
		/// this will be the less common case than an intended pure output parameter (derived as InputOutput)
		/// where the user provided no input value.
		/// 기본값 사용을 금함
		/// </summary>
		/// <param name="command">The command to which the parameters will be added</param>
		/// <param name="command">command에 추가될 변수<param>
		/// <param name="commandParameters">An array of SqlParameters to be added to command</param>
		/// <param name="commandParameters">command에 추가될 SqlParameters 배열</param>
		private static void AttachParameters(SqlCommand command, SqlParameter[] commandParameters)
		{
			if (command == null) throw new ArgumentNullException("command");
			if (commandParameters != null)
			{
				foreach (SqlParameter p in commandParameters)
				{
					if (p != null)
					{
						// Check for derived output value with no value assigned
						if ((p.Direction == ParameterDirection.InputOutput ||
							p.Direction == ParameterDirection.Input) &&
							(p.Value == null))
						{
							p.Value = DBNull.Value;
						}
						command.Parameters.Add(p);
					}
				}
			}
		}

		/// <summary>
		/// This method assigns dataRow column values to an array of SqlParameters
		/// 이 메소드는 dataRow 열의 값들을 SqlParameter배열에 지정시킨다.
		/// </summary>
		/// <param name="commandParameters">Array of SqlParameters to be assigned values</param>
		/// <param name="commandParameters">값이 지정될 SqlParameters 배열</param>
		/// <param name="dataRow">The dataRow used to hold the stored procedure's parameter values</param>
		/// <param name="dataRow">절차상 변수값 임시저장 공간으로 사용된 dataRow</param>
		private static void AssignParameterValues(SqlParameter[] commandParameters, DataRow dataRow)
		{
			if ((commandParameters == null) || (dataRow == null))
			{
				// Do nothing if we get no data
				return;
			}

			int i = 0;
			// Set the parameters values
			// 변수값 지정
			foreach (SqlParameter commandParameter in commandParameters)
			{
				// Check the parameter name
				if (commandParameter.ParameterName == null ||
					commandParameter.ParameterName.Length <= 1)
					throw new Exception(
						string.Format(
						"Please provide a valid parameter name on the parameter #{0}, the ParameterName property has the following value: '{1}'.",
						i, commandParameter.ParameterName));
				if (dataRow.Table.Columns.IndexOf(commandParameter.ParameterName.Substring(1)) != -1)
					commandParameter.Value = dataRow[commandParameter.ParameterName.Substring(1)];
				i++;
			}
		}

		/// <summary>
		/// This method assigns an array of values to an array of SqlParameters
		/// 이 메소드는 배열값들을 SqlParameter 배열에 지정
		/// </summary>
		/// <param name="commandParameters">Array of SqlParameters to be assigned values</param>
		/// <param name="commandParameters">값이 지정될 SqlParameters 배열</param>
		/// <param name="parameterValues">Array of objects holding the values to be assigned</param>
		/// <param name="parameterValues">지정값을 갖고 있는 객체의 배열</param>
		private static void AssignParameterValues(SqlParameter[] commandParameters, object[] parameterValues)
		{
			if ((commandParameters == null) || (parameterValues == null))
			{
				// Do nothing if we get no data
				// 데이타가 없을경우 아무것도 하지 않는다.
				return;
			}

			// We must have the same number of values as we pave parameters to put them in
			// 변수의 갯수와 값의 갯수가 같아야 한다.
			if (commandParameters.Length != parameterValues.Length)
			{
				throw new ArgumentException("Parameter count does not match Parameter Value count.");
			}

			// Iterate through the SqlParameters, assigning the values from the corresponding position in the 
			// value array
			// SqlParameter를 반복하면서 배열의 위치에 맞게 값들을 지정
			for (int i = 0, j = commandParameters.Length; i < j; i++)
			{
				// If the current array value derives from IDbDataParameter, then assign its Value property
				// 현재의 배열값이 IDbDataParameter에 속하면 값 속성을 지정한다.
				if (parameterValues[i] is IDbDataParameter)
				{
					IDbDataParameter paramInstance = (IDbDataParameter)parameterValues[i];
					if (paramInstance.Value == null)
					{
						commandParameters[i].Value = DBNull.Value;
					}
					else
					{
						commandParameters[i].Value = paramInstance.Value;
					}
				}
				else if (parameterValues[i] == null)
				{
					commandParameters[i].Value = DBNull.Value;
				}
				else
				{
					commandParameters[i].Value = parameterValues[i];
				}
			}
		}

		/// <summary>
		/// This method opens (if necessary) and assigns a connection, transaction, command type and parameters 
		/// to the provided command
		/// 이 메소드는 필요할 경우, 커넥션, 변동자료, 명령 type, 변수 등을 지정
		/// </summary>
		/// <param name="command">The SqlCommand to be prepared</param>
		/// <param name="command">준비할 SqlCommand</param>
		/// <param name="connection">A valid SqlConnection, on which to execute this command</param>
		/// <param name="connection">명령을 실행할 유효한 SqlConnection</param>
		/// <param name="transaction">A valid SqlTransaction, or 'null'</param>
		/// <param name="transaction">유효한 SqlTransaction, 혹은 'null'</param>
		/// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandType">stored procedure, 텍스트, 기타 등의 CommandType</param>
		/// <param name="commandText">The stored procedure name or T-SQL command</param>
		/// <param name="commandText">stored procedure명, 혹은 T-SQL 명령</param>
		/// <param name="commandParameters">An array of SqlParameters to be associated with the command or 'null' if no parameters are required</param>
		/// <param name="commandParameters">변수값 불필요시, SqlParameter 배열에 'null'값을 연동한다</param>
		/// <param name="mustCloseConnection"><c>true</c> if the connection was opened by the method, otherwose is false.</param>
		/// <param name="mustCloseConnection">메소드를 사용하여 커넥션 실행했을 시 <c>true</c>, 아니면 false</param>
		private static void PrepareCommand(SqlCommand command, SqlConnection connection, SqlTransaction transaction, CommandType commandType, string commandText, SqlParameter[] commandParameters, out bool mustCloseConnection)
		{
			if (command == null) throw new ArgumentNullException("command");
			if (commandText == null || commandText.Length == 0) throw new ArgumentNullException("commandText");

			// If the provided connection is not open, we will open it
			// 커넥션이 연결되어있지 않으면, 연결한다.
			if (connection.State != ConnectionState.Open)
			{
				mustCloseConnection = true;
				connection.Open();
			}
			else
			{
				mustCloseConnection = false;
			}

			// Associate the connection with the command
			// 명령을 커넥션과 연동시킴
			command.Connection = connection;

			// Set the command text (stored procedure name or SQL statement)
			// 명령 텍스트 지정(stored procedure명 혹은 SQL 명령문)
			command.CommandText = commandText;

			// If we were provided a transaction, assign it
			// transaction이 있을 경우 대입시킴
			if (transaction != null)
			{
				if (transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
				command.Transaction = transaction;
			}

			// Set the command type
			// 명령 유형 지정
			command.CommandType = commandType;

			// Attach the command parameters if they are provided
			// 필요시 command parameter 추가
			if (commandParameters != null)
			{
				AttachParameters(command, commandParameters);
			}
			return;
		}

		#endregion private utility methods & constructors

        #region ExecuteNonQuery - Non SqlTransaction

        /// <summary>
		/// Execute a SqlCommand (that returns no resultset and takes no parameters) against the database specified in 
		/// the connection string
		/// 커넥션 문자열에 지정된 데이터베이스의 (resultset를 반환하지 않고 변수값이 없는) SqlCommand를 실행시킨다. 
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  int result = ExecuteNonQuery(connString, CommandType.StoredProcedure, "PublishOrders");
		/// </remarks>
		/// <param name="connectionString">A valid connection string for a SqlConnection</param>
		/// <param name="connectionString">SqlConnection을 위한 유효한 커넥션 문자열</param>
		/// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandType">stored procedure, 텍스트, 기타 등의 CommandType</param>
		/// <param name="commandText">The stored procedure name or T-SQL command</param>
		/// <param name="commandText">stored procedure명, 혹은 T-SQL 명령</param>
		/// <returns>An int representing the number of rows affected by the command</returns>
		/// <returns>명령에 영향을 받은 열의 수를 나타내는 int값</returns>
		public static int ExecuteNonQuery(string connectionString, CommandType commandType, string commandText)
		{
			// Pass through the call providing null for the set of SqlParameters
			// 호출를 무시하고 SqlParameter에 null값 제공
			return ExecuteNonQuery(connectionString, commandType, commandText, (SqlParameter[])null);
		}

		/// <summary>
		/// Execute a SqlCommand (that returns no resultset) against the database specified in the connection string 
		/// using the provided parameters
		/// 커넥션 문자열에 지정된 데이터베이스의 (resultset을 반환하지 않는) SqlCommand 값을 실행시킨다.
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  int result = ExecuteNonQuery(connString, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24));
		/// </remarks>
		/// <param name="connectionString">A valid connection string for a SqlConnection</param>
		/// <param name="connectionString">SqlConnection을 위한 유효한 커넥션 문자열</param>
		/// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandType">stored procedure, 텍스트, 기타 등의 CommandType</param>
		/// <param name="commandText">The stored procedure name or T-SQL command</param>
		/// <param name="commandText">stored procedure명, 혹은 T-SQL 명령</param>
		/// <param name="commandParameters">An array of SqlParamters used to execute the command</param>
		/// <param name="commandParameters">명령 실행을 위한 SqlParamters 배열</param>
		/// <returns>An int representing the number of rows affected by the command</returns>
		/// <returns>명령에 영향을 받은 열의 수를 나타내는 int값</returns>
		public static int ExecuteNonQuery(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
		{
			if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");

			// Create & open a SqlConnection, and dispose of it after we are done
			// SqlConnection을 생성하고 실행한 다음, 사용이 끝난 후 처분
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				connection.Open();

				// Call the overload that takes a connection in place of the connection string
				// 커넥션 문자열 대신에 차지하는 커넥션의 overload 호출
				return ExecuteNonQuery(connection, commandType, commandText, commandParameters);
			}
		}

		/// <summary>
		/// Execute a stored procedure via a SqlCommand (that returns no resultset) against the database specified in 
		/// the connection string using the provided parameter values.  This method will query the database to discover the parameters for the 
		/// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
		/// 변수값을 사용하여 커넥션 문자열에 지정된 데이타베이스의 (resultset 반환값이 없는) stored procedure를 SqlCommand을 통해 실행시킴.
		/// 이 메소드는 데이타베이스를 조회하여 stored procedure의 변수값을 찾아내고, 변수의 순서에 따라 값을 대입한다.
		/// </summary>
		/// <remarks>
		/// This method provides no access to output parameters or the stored procedure's return value parameter.
		/// 이 메소드는 출력 변수, 혹은 stored procedure 반환 변수값에 대한 접근을 금지한다.
		/// 
		/// e.g.:  
		///  int result = ExecuteNonQuery(connString, "PublishOrders", 24, 36);
		/// </remarks>
		/// <param name="connectionString">A valid connection string for a SqlConnection</param>
		/// <param name="connectionString">SqlConnection을 위한 유효한 커넥션 문자열</param>
		/// <param name="spName">The name of the stored prcedure</param>
		/// <param name="spName">stored procedure명</param>
		/// <param name="parameterValues">An array of objects to be assigned as the input values of the stored procedure</param>
		/// <param name="parameterValues">stored procedure 입력값으로 대입될 객체들의 배열</param>
		/// <returns>An int representing the number of rows affected by the command</returns>
		/// <returns>명령에 영향을 받은 열의 수를 나타내는 int값</returns>
		public static int ExecuteNonQuery(string connectionString, string spName, params object[] parameterValues)
		{
			if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
			if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

			// If we receive parameter values, we need to figure out where they go
			// 변수값을 받을 경우, 경로를 찾아내야 함
			if ((parameterValues != null) && (parameterValues.Length > 0))
			{
				// Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
				// parameter cache에서 stored procedure 해당 변수값을 끌어온다. (혹은 발견하여 cache에 저장)
				SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);

				// Assign the provided values to these parameters based on parameter order
				// 변수의 순서에 따라 이 값들을 변수에 대입한다.
				AssignParameterValues(commandParameters, parameterValues);

				// Call the overload that takes an array of SqlParameters
				// SqlParameter 배열을 차지하는 overload를 호출
				return ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName, commandParameters);
			}
			else
			{
				// Otherwise we can just call the SP without params
				// 아니면 변수없이 SP를 호출한다.
				return ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName);
			}
		}

		/// <summary>
		/// Execute a SqlCommand (that returns no resultset and takes no parameters) against the provided SqlConnection. 
		/// 주어진 SqlConnection에 대한 (resultset를 반환하지 않고 변수값이 없는) SqlCommand 값을 실행시킨다.
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  int result = ExecuteNonQuery(conn, CommandType.StoredProcedure, "PublishOrders");
		/// </remarks>
		/// <param name="connection">A valid SqlConnection</param>
		/// <param name="connection">유효한 SqlConnection</param>
		/// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandType">stored procedure, 텍스트, 기타 등의 CommandType</param>
		/// <param name="commandText">The stored procedure name or T-SQL command</param>
		/// <param name="commandText">stored procedure명, 혹은 T-SQL 명령</param>
		/// <returns>An int representing the number of rows affected by the command</returns>
		/// <returns>명령에 영향을 받은 열의 수를 나타내는 int값</returns>
		public static int ExecuteNonQuery(SqlConnection connection, CommandType commandType, string commandText)
		{
			// Pass through the call providing null for the set of SqlParameters
			// 호출을 통과하여 SqlParameter 세트에 null 값을 제공함
			return ExecuteNonQuery(connection, commandType, commandText, (SqlParameter[])null);
		}

		/// <summary>
		/// Execute a SqlCommand (that returns no resultset) against the specified SqlConnection 
		/// using the provided parameters.
		/// 주어진 변수를 이용해 지정된 SqlConnection에 대한 (resultset을 반환하지 않는) SqlCommand를 실행시킨다.
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  int result = ExecuteNonQuery(conn, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24));
		/// </remarks>
		/// <param name="connection">A valid SqlConnection</param>
		/// <param name="connection">유효한 SqlConnection</param>
		/// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandType">stored procedure, 텍스트, 기타 등의 CommandType</param>
		/// <param name="commandText">The stored procedure name or T-SQL command</param>
		/// <param name="commandText">stored procedure명, 혹은 T-SQL 명령</param>
		/// <param name="commandParameters">An array of SqlParameters used to execute the command</param>
		/// <param name="commandParameters">명령 실행을 위해 쓰여진 SqlParameters 배열</param>
		/// <returns>An int representing the number of rows affected by the command</returns>
		/// <returns>명령에 영향을 받은 열의 수를 나타내는 int값</returns>
		public static int ExecuteNonQuery(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
		{
			if (connection == null) throw new ArgumentNullException("connection");

			// Create a command and prepare it for execution
			// 명령 생성 및 실행 준비
			SqlCommand cmd = new SqlCommand();
			bool mustCloseConnection = false;
			PrepareCommand(cmd, connection, (SqlTransaction)null, commandType, commandText, commandParameters, out mustCloseConnection);

            // 2009/03/27 - Timeout 오류가 발생하여 시간(단위:초) 설정. by bhchoi
            cmd.CommandTimeout = 14400; // 4시간

			// Finally, execute the command
			// 명령을 실행함
			int retval = cmd.ExecuteNonQuery();

			// Detach the SqlParameters from the command object, so they can be used again
			// 재활용을 위해 SqlParameter을 명령 객체에서 때어냄
			cmd.Parameters.Clear();
			if (mustCloseConnection)
				connection.Close();
			return retval;
		}

		/// <summary>
		/// Execute a stored procedure via a SqlCommand (that returns no resultset) against the specified SqlConnection 
		/// using the provided parameter values.  This method will query the database to discover the parameters for the 
		/// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
		/// 변수값을 이용해 지정된 SqlConnection에 대한 (resultset 반환값이 없는) stored procedure를 SqlCommand을 통해 실행시킴
		/// 이 메소드는 데이타베이스를 조회하여 stored procedure의 변수값을 찾아내고(각 stored procedure 처음 호출시),
		/// 변수의 순서에 따라 값을 대입한다.
		/// </summary>
		/// <remarks>
		/// This method provides no access to output parameters or the stored procedure's return value parameter.
		/// 이 메소드는 출력변수, 혹은 stored procedure 반환변수의 접근을 제공하지 않는다.
		/// 
		/// e.g.:  
		///  int result = ExecuteNonQuery(conn, "PublishOrders", 24, 36);
		/// </remarks>
		/// <param name="connection">A valid SqlConnection</param>
		/// <param name="connection">유효한 SqlConnection</param>
		/// <param name="spName">The name of the stored procedure</param>
		/// <param name="spName">stored procedure명</param>
		/// <param name="parameterValues">An array of objects to be assigned as the input values of the stored procedure</param>
		/// <param name="parameterValues">stored procedure의 입력값으로 대입될 객체의 배열</param>
		/// <returns>An int representing the number of rows affected by the command</returns>
		/// <returns>명령에 영향을 받은 열의 수를 나타내는 int값</returns>
		public static int ExecuteNonQuery(SqlConnection connection, string spName, params object[] parameterValues)
		{
			if (connection == null) throw new ArgumentNullException("connection");
			if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

			// If we receive parameter values, we need to figure out where they go
			// 변수값을 받을 경우, 경로를 찾아내야 함
			if ((parameterValues != null) && (parameterValues.Length > 0))
			{
				// Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
				// parameter cache에서 stored procedure 해당 변수값을 끌어온다. (혹은 발견하여 cache에 저장)
				SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connection, spName);

				// Assign the provided values to these parameters based on parameter order
				// 변수의 순서에 따라 이 값들을 변수에 대입한다.
				AssignParameterValues(commandParameters, parameterValues);

				// Call the overload that takes an array of SqlParameters
				// SqlParameter 배열을 차지하는 overload를 호출
				return ExecuteNonQuery(connection, CommandType.StoredProcedure, spName, commandParameters);
			}
			else
			{
				// Otherwise we can just call the SP without params
				// 아니면 변수없이 SP를 호출한다.
				return ExecuteNonQuery(connection, CommandType.StoredProcedure, spName);
			}
		}

        #endregion

        #region ExecuteNonQuery - SqlTransaction
        /// <summary>
		/// Execute a SqlCommand (that returns no resultset and takes no parameters) against the provided SqlTransaction. 
		/// 지정된 SqlTransaction에 대한 (resultset를 반환하지 않고 변수값이 없는) SqlCommand를 실행시킨다.
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  int result = ExecuteNonQuery(trans, CommandType.StoredProcedure, "PublishOrders");
		/// </remarks>
		/// <param name="transaction">A valid SqlTransaction</param>
		/// <param name="transaction">유효한 SqlTransaction</param>
		/// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandType">stored procedure, 텍스트, 기타 등의 CommandType</param>
		/// <param name="commandText">The stored procedure name or T-SQL command</param>
		/// <param name="commandText">stored procedure명, 혹은 T-SQL 명령</param>
		/// <returns>An int representing the number of rows affected by the command</returns>
		/// <returns>명령에 영향을 받은 열의 수를 나타내는 int값</returns>
		public static int ExecuteNonQuery(SqlTransaction transaction, CommandType commandType, string commandText)
		{
			// Pass through the call providing null for the set of SqlParameters
			// 호출을 통과하여 SqlParameter 세트에 null 값을 제공함
			return ExecuteNonQuery(transaction, commandType, commandText, (SqlParameter[])null);
		}

		/// <summary>
		/// Execute a SqlCommand (that returns no resultset) against the specified SqlTransaction
		/// using the provided parameters.
		/// 주어징 변수를 이용해 지정된 SqlTransaction에 대한 (resultset을 반환하지 않는) SqlCommand를 실행시킨다.
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  int result = ExecuteNonQuery(trans, CommandType.StoredProcedure, "GetOrders", new SqlParameter("@prodid", 24));
		/// </remarks>
		/// <param name="transaction">A valid SqlTransaction</param>
		/// <param name="transaction">유효한 SqlTransaction</param>
		/// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandType">stored procedure, 텍스트, 기타 등의 CommandType</param>
		/// <param name="commandText">The stored procedure name or T-SQL command</param>
		/// <param name="commandText">stored procedure명, 혹은 T-SQL 명령</param>
		/// <param name="commandParameters">An array of SqlParamters used to execute the command</param>
		/// <param name="commandParameters">명령 실행을 위해 쓰여진 SqlParameters 배열</param>
		/// <returns>An int representing the number of rows affected by the command</returns>
		/// <returns>명령에 영향을 받은 열의 수를 나타내는 int값</returns>
		public static int ExecuteNonQuery(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
		{
			if (transaction == null) throw new ArgumentNullException("transaction");
			if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");

			// Create a command and prepare it for execution
			// 명령 생성 및 실행 준비
			SqlCommand cmd = new SqlCommand();
			bool mustCloseConnection = false;
			PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters, out mustCloseConnection);

            // 2009/03/27 - Timeout 오류가 발생하여 시간(단위:초) 설정. by bhchoi
            cmd.CommandTimeout = 14400; // 4시간

			// Finally, execute the command
			// 명령 실행
			int retval = cmd.ExecuteNonQuery();

			// Detach the SqlParameters from the command object, so they can be used again
			// 재활용을 위해 SqlParameter을 명령 객체에서 때어냄
			cmd.Parameters.Clear();
			return retval;
		}

		/// <summary>
		/// Execute a stored procedure via a SqlCommand (that returns no resultset) against the specified 
		/// SqlTransaction using the provided parameter values.  This method will query the database to discover the parameters for the 
		/// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
		/// 변수값을 이용해 지정된 SqlTransaction에 대한 (resultset를 반환하지 않는) stored procedure를 SqlCommand을 통해 실행시킴
		/// 이 메소드는 데이타베이스를 조회하여 stored procedure의 변수값을 찾아내고(각 stored procedure 처음 호출시),
		/// 변수의 순서에 따라 값을 대입한다.
		/// </summary>
		/// <remarks>
		/// This method provides no access to output parameters or the stored procedure's return value parameter.
		/// 이 메소드는 출력변수, 혹은 stored procedure 반환변수의 접근을 제공하지 않는다.
		/// 
		/// e.g.:  
		///  int result = ExecuteNonQuery(conn, trans, "PublishOrders", 24, 36);
		/// </remarks>
		/// <param name="transaction">A valid SqlTransaction</param>
		/// <param name="transaction">유효한 SqlTransaction</param>
		/// <param name="spName">The name of the stored procedure</param>
		/// <param name="spName">stored procedure명</param>
		/// <param name="parameterValues">An array of objects to be assigned as the input values of the stored procedure</param>
		/// <param name="parameterValues">stored procedure의 입력값으로 대입될 객체의 배열</param>
		/// <returns>An int representing the number of rows affected by the command</returns>
		/// <returns>명령에 영향을 받은 열의 수를 나타내는 int값</returns>
		public static int ExecuteNonQuery(SqlTransaction transaction, string spName, params object[] parameterValues)
		{
			if (transaction == null) throw new ArgumentNullException("transaction");
			if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
			if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

			// If we receive parameter values, we need to figure out where they go
			// 변수값을 받을 경우, 경로를 찾아내야 함
			if ((parameterValues != null) && (parameterValues.Length > 0))
			{
				// Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
				// parameter cache에서 stored procedure 해당 변수값을 끌어온다. (혹은 발견하여 cache에 저장)
				SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);

				// Assign the provided values to these parameters based on parameter order
				// 변수의 순서에 따라 이 값들을 변수에 대입한다.
				AssignParameterValues(commandParameters, parameterValues);

				// Call the overload that takes an array of SqlParameters
				// SqlParameter 배열을 차지하는 overload를 호출
				return ExecuteNonQuery(transaction, CommandType.StoredProcedure, spName, commandParameters);
			}
			else
			{
				// Otherwise we can just call the SP without params
				// 아니면 변수없이 SP를 호출한다.
				return ExecuteNonQuery(transaction, CommandType.StoredProcedure, spName);
			}
		}

      

		#endregion ExecuteNonQuery

		#region ExecuteDataset

		/// <summary>
		/// Execute a SqlCommand (that returns a resultset and takes no parameters) against the database specified in 
		/// the connection string. 
		/// 커넥션 문자열에 지정된 데이타베이스에 대한 (resultset를 반환하고 변수값이 없는) SqlCommand 값을 실행시킨다.
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  DataSet ds = ExecuteDataset(connString, CommandType.StoredProcedure, "GetOrders");
		/// </remarks>
		/// <param name="connectionString">A valid connection string for a SqlConnection</param>
		/// <param name="connectionString">SqlConnection을 위한 유효한 커넥션 문자열</param>
		/// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandType">stored procedure, 텍스트, 기타 등의 CommandType</param>
		/// <param name="commandText">The stored procedure name or T-SQL command</param>
		/// <param name="commandText">stored procedure명, 혹은 T-SQL 명령</param>
		/// <returns>A dataset containing the resultset generated by the command</returns>
		/// <returns>명령으로 생성된 resultset가 포함되어있는 dataset</returns>
		public static DataSet ExecuteDataset(string connectionString, CommandType commandType, string commandText)
		{
			// Pass through the call providing null for the set of SqlParameters
			// 호출을 통과하여 SqlParameter 세트에 null 값을 제공함
			return ExecuteDataset(connectionString, commandType, commandText, (SqlParameter[])null);
		}

		/// <summary>
		/// Execute a SqlCommand (that returns a resultset) against the database specified in the connection string 
		/// using the provided parameters.
		/// 주어진 변수값을 이용해 커넥션 문자열에 지정된 데이타베이스에 대한 (resultset을 반환하는) SqlCommand를 실행시킨다.
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  DataSet ds = ExecuteDataset(connString, CommandType.StoredProcedure, "GetOrders", new SqlParameter("@prodid", 24));
		/// </remarks>
		/// <param name="connectionString">A valid connection string for a SqlConnection</param>
		/// <param name="connectionString">SqlConnection를 위한 유효한 connection string</param>
		/// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandType">stored procedure, 텍스트, 기타 등의 CommandType</param>
		/// <param name="commandText">The stored procedure name or T-SQL command</param>
		/// <param name="commandText">stored procedure명, 혹은 T-SQL 명령</param>
		/// <param name="commandParameters">An array of SqlParamters used to execute the command</param>
		/// <param name="commandParameters">명령 실행을 위해 쓰여진 SqlParameters 배열</param>
		/// <returns>A dataset containing the resultset generated by the command</returns>
		/// <returns>명령으로 생성된 resultset가 포함되어있는 dataset</returns>
		public static DataSet ExecuteDataset(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
		{
			if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");

			// Create & open a SqlConnection, and dispose of it after we are done
			// SqlConnection을 생성하고 실행한 다음, 사용이 끝난 후 처분
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				connection.Open();

				// Call the overload that takes a connection in place of the connection string
				// 커넥션 문자열 대신에 차지하는 커넥션의 overload 호출
				return ExecuteDataset(connection, commandType, commandText, commandParameters);
			}
		}

		/// <summary>
		/// Execute a stored procedure via a SqlCommand (that returns a resultset) against the database specified in 
		/// the connection string using the provided parameter values.  This method will query the database to discover the parameters for the 
		/// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
		/// 변수값을 사용하여 커넥션 문자열에 지정된 데이타베이스의 (resultset를 반환하는) stored procedure를 SqlCommand을 통해 실행시킴.
		/// 이 메소드는 데이타베이스를 조회하여 stored procedure의 변수값을 찾아내고, 변수의 순서에 따라 값을 대입한다.
		/// </summary>
		/// <remarks>
		/// This method provides no access to output parameters or the stored procedure's return value parameter.
		/// 이 메소드는 출력변수, 혹은 stored procedure 반환변수의 접근을 제공하지 않는다.
		/// e.g.:  
		///  DataSet ds = ExecuteDataset(connString, "GetOrders", 24, 36);
		/// </remarks>
		/// <param name="connectionString">A valid connection string for a SqlConnection</param>
		/// <param name="spName">The name of the stored procedure</param>
		/// <param name="parameterValues">An array of objects to be assigned as the input values of the stored procedure</param>
		/// <returns>A dataset containing the resultset generated by the command</returns>
		public static DataSet ExecuteDataset(string connectionString, string spName, params object[] parameterValues)
		{
			if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
			if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

			// If we receive parameter values, we need to figure out where they go
			// 변수값을 받을 경우, 경로를 찾아내야 함
			if ((parameterValues != null) && (parameterValues.Length > 0))
			{
				// Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
				// parameter cache에서 stored procedure 해당 변수값을 끌어온다. (혹은 발견하여 cache에 저장)
				SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);

				// Assign the provided values to these parameters based on parameter order
				// 변수의 순서에 따라 이 값들을 변수에 대입한다.
				AssignParameterValues(commandParameters, parameterValues);

				// Call the overload that takes an array of SqlParameters
				// SqlParameter 배열을 차지하는 overload를 호출
				return ExecuteDataset(connectionString, CommandType.StoredProcedure, spName, commandParameters);
			}
			else
			{
				// Otherwise we can just call the SP without params
				// 아니면 변수없이 SP를 호출한다.
				return ExecuteDataset(connectionString, CommandType.StoredProcedure, spName);
			}
		}

		/// <summary>
		/// Execute a SqlCommand (that returns a resultset and takes no parameters) against the provided SqlConnection. 
		/// 주어진 SqlConnection에 대한 (resultset를 반환하고 변수값이 없는) SqlCommand 값을 실행시킨다.
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  DataSet ds = ExecuteDataset(conn, CommandType.StoredProcedure, "GetOrders");
		/// </remarks>
		/// <param name="connection">A valid SqlConnection</param>
		/// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">The stored procedure name or T-SQL command</param>
		/// <returns>A dataset containing the resultset generated by the command</returns>
		public static DataSet ExecuteDataset(SqlConnection connection, CommandType commandType, string commandText)
		{
			// Pass through the call providing null for the set of SqlParameters
			// 호출을 통과하여 SqlParameter 세트에 null 값을 제공함
			return ExecuteDataset(connection, commandType, commandText, (SqlParameter[])null);
		}

		/// <summary>
		/// Execute a SqlCommand (that returns a resultset) against the specified SqlConnection 
		/// using the provided parameters.
		/// 주어진 변수값을 이용해 지정된 SqlConnection에 대한 (resultset를 반환하는) SqlCommand를 실행시킨다.
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  DataSet ds = ExecuteDataset(conn, CommandType.StoredProcedure, "GetOrders", new SqlParameter("@prodid", 24));
		/// </remarks>
		/// <param name="connection">A valid SqlConnection</param>
		/// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">The stored procedure name or T-SQL command</param>
		/// <param name="commandParameters">An array of SqlParamters used to execute the command</param>
		/// <returns>A dataset containing the resultset generated by the command</returns>
		public static DataSet ExecuteDataset(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
		{
			if (connection == null) throw new ArgumentNullException("connection");

			// Create a command and prepare it for execution
			// 명령 생성 및 실행 준비
			SqlCommand cmd = new SqlCommand();
			bool mustCloseConnection = false;
			PrepareCommand(cmd, connection, (SqlTransaction)null, commandType, commandText, commandParameters, out mustCloseConnection);

			// Create the DataAdapter & DataSet
			// DataAdapter 및 DataSet 생성
			using (SqlDataAdapter da = new SqlDataAdapter(cmd))
			{
				DataSet ds = new DataSet();

				// Fill the DataSet using default values for DataTable names, etc
				// DataTable names 기본값으로 DataSet을 채워나감
				da.Fill(ds);

				// Detach the SqlParameters from the command object, so they can be used again
				// 재활용을 위해 SqlParameter을 명령 객체에서 때어냄
				cmd.Parameters.Clear();

				if (mustCloseConnection)
					connection.Close();

				// Return the dataset
				// dataset 반환
				return ds;
			}
		}

		/// <summary>
		/// Execute a stored procedure via a SqlCommand (that returns a resultset) against the specified SqlConnection 
		/// using the provided parameter values.  This method will query the database to discover the parameters for the 
		/// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
		/// 변수값을 이용해 지정된 SqlConnection에 대한 (resultset를 반환하는) stored procedure를 SqlCommand을 통해 실행시킴
		/// 이 메소드는 데이타베이스를 조회하여 stored procedure의 변수값을 찾아내고(각 stored procedure 처음 호출시),
		/// 변수의 순서에 따라 값을 대입한다.
		/// </summary>
		/// <remarks>
		/// This method provides no access to output parameters or the stored procedure's return value parameter.
		/// 이 메소드는 출력변수, 혹은 stored procedure 반환변수의 접근을 제공하지 않는다.
		/// 
		/// e.g.:  
		///  DataSet ds = ExecuteDataset(conn, "GetOrders", 24, 36);
		/// </remarks>
		/// <param name="connection">A valid SqlConnection</param>
		/// <param name="spName">The name of the stored procedure</param>
		/// <param name="parameterValues">An array of objects to be assigned as the input values of the stored procedure</param>
		/// <returns>A dataset containing the resultset generated by the command</returns>
		public static DataSet ExecuteDataset(SqlConnection connection, string spName, params object[] parameterValues)
		{
			if (connection == null) throw new ArgumentNullException("connection");
			if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

			// If we receive parameter values, we need to figure out where they go
			// 변수값을 받을 경우, 경로를 찾아내야 함
			if ((parameterValues != null) && (parameterValues.Length > 0))
			{
				// Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
				// parameter cache에서 stored procedure 해당 변수값을 끌어온다. (혹은 발견하여 cache에 저장)
				SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connection, spName);

				// Assign the provided values to these parameters based on parameter order
				// 변수의 순서에 따라 이 값들을 변수에 대입한다.
				AssignParameterValues(commandParameters, parameterValues);

				// Call the overload that takes an array of SqlParameters
				// SqlParameter 배열을 차지하는 overload를 호출
				return ExecuteDataset(connection, CommandType.StoredProcedure, spName, commandParameters);
			}
			else
			{
				// Otherwise we can just call the SP without params
				// 아니면 변수없이 SP를 호출한다.
				return ExecuteDataset(connection, CommandType.StoredProcedure, spName);
			}
		}

		/// <summary>
		/// Execute a SqlCommand (that returns a resultset and takes no parameters) against the provided SqlTransaction. 
		/// 주어진 SqlTransaction에 대한 (resultset를 반환하고 변수값이 없는) SqlCommand 값을 실행시킨다.
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  DataSet ds = ExecuteDataset(trans, CommandType.StoredProcedure, "GetOrders");
		/// </remarks>
		/// <param name="transaction">A valid SqlTransaction</param>
		/// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">The stored procedure name or T-SQL command</param>
		/// <returns>A dataset containing the resultset generated by the command</returns>
		public static DataSet ExecuteDataset(SqlTransaction transaction, CommandType commandType, string commandText)
		{
			// Pass through the call providing null for the set of SqlParameters
			// 호출을 통과하여 SqlParameter 세트에 null 값을 제공함
			return ExecuteDataset(transaction, commandType, commandText, (SqlParameter[])null);
		}

		/// <summary>
		/// Execute a SqlCommand (that returns a resultset) against the specified SqlTransaction
		/// using the provided parameters.
		/// 주어진 변수값을 이용해 지정된 SqlTransaction에 대한 (resultset을 반환하는) SqlCommand를 실행시킨다.
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  DataSet ds = ExecuteDataset(trans, CommandType.StoredProcedure, "GetOrders", new SqlParameter("@prodid", 24));
		/// </remarks>
		/// <param name="transaction">A valid SqlTransaction</param>
		/// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">The stored procedure name or T-SQL command</param>
		/// <param name="commandParameters">An array of SqlParamters used to execute the command</param>
		/// <returns>A dataset containing the resultset generated by the command</returns>
		public static DataSet ExecuteDataset(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
		{
			if (transaction == null) throw new ArgumentNullException("transaction");
			if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");

			// Create a command and prepare it for execution
			// 명령 생성 및 실행 준비
			SqlCommand cmd = new SqlCommand();
			bool mustCloseConnection = false;
			PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters, out mustCloseConnection);

			// Create the DataAdapter & DataSet
			// DataAdapter 및 DataSet 생성
			using (SqlDataAdapter da = new SqlDataAdapter(cmd))
			{
				DataSet ds = new DataSet();

				// Fill the DataSet using default values for DataTable names, etc
				// DataTable names 기본값으로 DataSet을 채워나감
				da.Fill(ds);

				// Detach the SqlParameters from the command object, so they can be used again
				// 재활용을 위해 SqlParameter을 명령 객체에서 때어냄
				cmd.Parameters.Clear();

				// Return the dataset
				// dataset 반환
				return ds;
			}
		}

		/// <summary>
		/// Execute a stored procedure via a SqlCommand (that returns a resultset) against the specified 
		/// SqlTransaction using the provided parameter values.  This method will query the database to discover the parameters for the 
		/// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
		/// 변수값을 이용해 지정된 SqlTransaction에 대한 (resultset를 반환하는) stored procedure를 SqlCommand을 통해 실행시킴
		/// 이 메소드는 데이타베이스를 조회하여 stored procedure의 변수값을 찾아내고(각 stored procedure 처음 호출시),
		/// 변수의 순서에 따라 값을 대입한다.
		/// </summary>
		/// <remarks>
		/// This method provides no access to output parameters or the stored procedure's return value parameter.
		/// 이 메소드는 출력변수, 혹은 stored procedure 반환변수의 접근을 제공하지 않는다.
		/// 
		/// e.g.:  
		///  DataSet ds = ExecuteDataset(trans, "GetOrders", 24, 36);
		/// </remarks>
		/// <param name="transaction">A valid SqlTransaction</param>
		/// <param name="spName">The name of the stored procedure</param>
		/// <param name="parameterValues">An array of objects to be assigned as the input values of the stored procedure</param>
		/// <returns>A dataset containing the resultset generated by the command</returns>
		public static DataSet ExecuteDataset(SqlTransaction transaction, string spName, params object[] parameterValues)
		{
			if (transaction == null) throw new ArgumentNullException("transaction");
			if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
			if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

			// If we receive parameter values, we need to figure out where they go
			// 변수값을 받을 경우, 경로를 찾아내야 함
			if ((parameterValues != null) && (parameterValues.Length > 0))
			{
				// Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
				// parameter cache에서 stored procedure 해당 변수값을 끌어온다. (혹은 발견하여 cache에 저장)
				SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);

				// Assign the provided values to these parameters based on parameter order
				// 변수의 순서에 따라 이 값들을 변수에 대입한다.
				AssignParameterValues(commandParameters, parameterValues);

				// Call the overload that takes an array of SqlParameters
				// SqlParameter 배열을 차지하는 overload를 호출
				return ExecuteDataset(transaction, CommandType.StoredProcedure, spName, commandParameters);
			}
			else
			{
				// Otherwise we can just call the SP without params
				// 아니면 변수없이 SP를 호출한다.
				return ExecuteDataset(transaction, CommandType.StoredProcedure, spName);
			}
		}

		#endregion ExecuteDataset

		#region ExecuteReader

		/// <summary>
		/// This enum is used to indicate whether the connection was provided by the caller, or created by SqlHelper, so that
		/// we can set the appropriate CommandBehavior when calling ExecuteReader()
		/// 이 enum을 통해 커넥션이 호출자에게서 제공되었는지 SqlHelper에게서 제공되었는지의 여부를 알 수 있으며,
		/// 적절한 CommandBehavior의 지정하여 ExecuteReader()를 실행할 수 있다.
		/// </summary>
		private enum SqlConnectionOwnership
		{
			/// <summary>Connection is owned and managed by SqlHelper</summary>
			/// <summary>커넥션은 SqlHelper가 소유하고 관리하고 있음</summary>
			Internal,
			/// <summary>Connection is owned and managed by the caller</summary>
			/// <summary>커넥션은 호출자가 소유하고 관리하고 있음</summary>
			External
		}

		/// <summary>
		/// Create and prepare a SqlCommand, and call ExecuteReader with the appropriate CommandBehavior.
		/// SqlCommand를 생성하고 준비하여, 절절한 CommandBehavior으로 ExecuteReader를 호출함
		/// </summary>
		/// <remarks>
		/// If we created and opened the connection, we want the connection to be closed when the DataReader is closed.
		/// 우리가 커넥션을 생성하고 열었을 시, DataReader가 닫힐때 커넥션도 닫게 함
		/// 
		/// If the caller provided the connection, we want to leave it to them to manage.
		/// 호출자가 커넥션을 제공하였을시, 그들이 관리하도록 설정
		/// </remarks>
		/// <param name="connection">A valid SqlConnection, on which to execute this command</param>
		/// <param name="transaction">A valid SqlTransaction, or 'null'</param>
		/// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">The stored procedure name or T-SQL command</param>
		/// <param name="commandParameters">An array of SqlParameters to be associated with the command or 'null' if no parameters are required</param>
		/// <param name="connectionOwnership">Indicates whether the connection parameter was provided by the caller, or created by SqlHelper</param>
		/// <returns>SqlDataReader containing the results of the command</returns>
		/// <returns>명령 결과값을 갖고 있는 SqlDataReader</returns>
		private static SqlDataReader ExecuteReader(SqlConnection connection, SqlTransaction transaction, CommandType commandType, string commandText, SqlParameter[] commandParameters, SqlConnectionOwnership connectionOwnership)
		{
			if (connection == null) throw new ArgumentNullException("connection");

			bool mustCloseConnection = false;
			// Create a command and prepare it for execution
			// 명령 생성 및 실행 준비
			SqlCommand cmd = new SqlCommand();
			try
			{
				PrepareCommand(cmd, connection, transaction, commandType, commandText, commandParameters, out mustCloseConnection);

				// Create a reader
				// reader 생성
				SqlDataReader dataReader;

				// Call ExecuteReader with the appropriate CommandBehavior
				// 절절한 CommandBehavior를 갖고 있는 ExecuteReader 호출
				if (connectionOwnership == SqlConnectionOwnership.External)
				{
					dataReader = cmd.ExecuteReader();
				}
				else
				{
					dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
				}

				// Detach the SqlParameters from the command object, so they can be used again.
				// HACK: There is a problem here, the output parameter values are fletched 
				// when the reader is closed, so if the parameters are detached from the command
				// then the SqlReader can큧 set its values. 
				// When this happen, the parameters can큧 be used again in other command.
				// 재활용을 위해 SqlParameter을 명령 객체에서 분리
				// HACK: 이곳에 문제가 있습니다. reader가 닫혀있을 경우, 출력 변수값이 fletch 되어진다.
				// 즉, 변수들이 명령으로부터 분리될 경우, SqlReader의 값을 지정하지 못한다.
				bool canClear = true;
				foreach (SqlParameter commandParameter in cmd.Parameters)
				{
					if (commandParameter.Direction != ParameterDirection.Input)
						canClear = false;
				}

				if (canClear)
				{
					cmd.Parameters.Clear();
				}

				return dataReader;
			}
			catch
			{
				if (mustCloseConnection)
					connection.Close();
				throw;
			}
		}

		/// <summary>
		/// Execute a SqlCommand (that returns a resultset and takes no parameters) against the database specified in 
		/// the connection string. 
		/// 커넥션 문자열에 지정된 데이타베이스에 대한 (resultset를 반환하고 변수값이 없는) SqlCommand 값을 실행시킨다.
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  SqlDataReader dr = ExecuteReader(connString, CommandType.StoredProcedure, "GetOrders");
		/// </remarks>
		/// <param name="connectionString">A valid connection string for a SqlConnection</param>
		/// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">The stored procedure name or T-SQL command</param>
		/// <returns>A SqlDataReader containing the resultset generated by the command</returns>
		public static SqlDataReader ExecuteReader(string connectionString, CommandType commandType, string commandText)
		{
			// Pass through the call providing null for the set of SqlParameters
			// 호출을 통과하여 SqlParameter 세트에 null 값을 제공함
			return ExecuteReader(connectionString, commandType, commandText, (SqlParameter[])null);
		}

		/// <summary>
		/// Execute a SqlCommand (that returns a resultset) against the database specified in the connection string 
		/// using the provided parameters.
		/// 주어진 변수값을 이용해 커넥션 문자열에 지정된 데이타베이스에 대한 (resultset을 반환하는) SqlCommand를 실행시킨다.
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  SqlDataReader dr = ExecuteReader(connString, CommandType.StoredProcedure, "GetOrders", new SqlParameter("@prodid", 24));
		/// </remarks>
		/// <param name="connectionString">A valid connection string for a SqlConnection</param>
		/// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">The stored procedure name or T-SQL command</param>
		/// <param name="commandParameters">An array of SqlParamters used to execute the command</param>
		/// <returns>A SqlDataReader containing the resultset generated by the command</returns>
		public static SqlDataReader ExecuteReader(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
		{
			if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
			SqlConnection connection = null;
			try
			{
				connection = new SqlConnection(connectionString);
				connection.Open();

				// Call the private overload that takes an internally owned connection in place of the connection string
				// 커넥션 문자열 대신에 자체 내부 커넥션을 갖는 private overload 호출
				return ExecuteReader(connection, null, commandType, commandText, commandParameters, SqlConnectionOwnership.Internal);
			}
			catch
			{
				// If we fail to return the SqlDatReader, we need to close the connection ourselves
				// SqlDatReader 반환 실패시, 커넥션 방치(굳이 끊을 필요 없음)
				if (connection != null) connection.Close();
				throw;
			}

		}

		/// <summary>
		/// Execute a stored procedure via a SqlCommand (that returns a resultset) against the database specified in 
		/// the connection string using the provided parameter values.  This method will query the database to discover the parameters for the 
		/// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
		/// 변수값을 사용하여 커넥션 문자열에 지정된 데이타베이스의 (resultset를 반환하는) stored procedure를 SqlCommand을 통해 실행시킴.
		/// 이 메소드는 데이타베이스를 조회하여 stored procedure의 변수값을 찾아내고, 변수의 순서에 따라 값을 대입한다.
		/// </summary>
		/// <remarks>
		/// This method provides no access to output parameters or the stored procedure's return value parameter.
		/// 이 메소드는 출력변수, 혹은 stored procedure 반환변수의 접근을 제공하지 않는다.
		/// 
		/// e.g.:  
		///  SqlDataReader dr = ExecuteReader(connString, "GetOrders", 24, 36);
		/// </remarks>
		/// <param name="connectionString">A valid connection string for a SqlConnection</param>
		/// <param name="spName">The name of the stored procedure</param>
		/// <param name="parameterValues">An array of objects to be assigned as the input values of the stored procedure</param>
		/// <returns>A SqlDataReader containing the resultset generated by the command</returns>
		public static SqlDataReader ExecuteReader(string connectionString, string spName, params object[] parameterValues)
		{
			if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
			if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

			// If we receive parameter values, we need to figure out where they go
			// 변수값을 받을 경우, 경로를 찾아내야 함
			if ((parameterValues != null) && (parameterValues.Length > 0))
			{
				SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);

				AssignParameterValues(commandParameters, parameterValues);

				return ExecuteReader(connectionString, CommandType.StoredProcedure, spName, commandParameters);
			}
			else
			{
				// Otherwise we can just call the SP without params
				// 아니면 변수없이 SP를 호출한다.
				return ExecuteReader(connectionString, CommandType.StoredProcedure, spName);
			}
		}

		/// <summary>
		/// Execute a SqlCommand (that returns a resultset and takes no parameters) against the provided SqlConnection. 
		/// 주어진 SqlConnection에 대한 (resultset를 반환하고 변수값이 없는) SqlCommand 값을 실행시킨다.
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  SqlDataReader dr = ExecuteReader(conn, CommandType.StoredProcedure, "GetOrders");
		/// </remarks>
		/// <param name="connection">A valid SqlConnection</param>
		/// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">The stored procedure name or T-SQL command</param>
		/// <returns>A SqlDataReader containing the resultset generated by the command</returns>
		public static SqlDataReader ExecuteReader(SqlConnection connection, CommandType commandType, string commandText)
		{
			// Pass through the call providing null for the set of SqlParameters
			// 호출을 통과하여 SqlParameter 세트에 null 값을 제공함
			return ExecuteReader(connection, commandType, commandText, (SqlParameter[])null);
		}

		/// <summary>
		/// Execute a SqlCommand (that returns a resultset) against the specified SqlConnection 
		/// using the provided parameters.
		/// 주어진 변수값을 이용해 지정된 SqlConnection에 대한 (resultset을 반환하는) SqlCommand를 실행시킨다.
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  SqlDataReader dr = ExecuteReader(conn, CommandType.StoredProcedure, "GetOrders", new SqlParameter("@prodid", 24));
		/// </remarks>
		/// <param name="connection">A valid SqlConnection</param>
		/// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">The stored procedure name or T-SQL command</param>
		/// <param name="commandParameters">An array of SqlParamters used to execute the command</param>
		/// <returns>A SqlDataReader containing the resultset generated by the command</returns>
		public static SqlDataReader ExecuteReader(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
		{
			// Pass through the call to the private overload using a null transaction value and an externally owned connection
			// null transaction과 자체 외부커넥션을 사용하여 overload를 private 시킴
			return ExecuteReader(connection, (SqlTransaction)null, commandType, commandText, commandParameters, SqlConnectionOwnership.External);
		}

		/// <summary>
		/// Execute a stored procedure via a SqlCommand (that returns a resultset) against the specified SqlConnection 
		/// using the provided parameter values.  This method will query the database to discover the parameters for the 
		/// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
		/// 변수값을 이용해 지정된 SqlConnection에 대한 (resultset를 반환하는) stored procedure를 SqlCommand을 통해 실행시킴
		/// 이 메소드는 데이타베이스를 조회하여 stored procedure의 변수값을 찾아내고(각 stored procedure 처음 호출시),
		/// 변수의 순서에 따라 값을 대입한다.
		/// </summary>
		/// <remarks>
		/// This method provides no access to output parameters or the stored procedure's return value parameter.
		/// 이 메소드는 출력변수, 혹은 stored procedure 반환변수의 접근을 제공하지 않는다.
		/// 
		/// e.g.:  
		///  SqlDataReader dr = ExecuteReader(conn, "GetOrders", 24, 36);
		/// </remarks>
		/// <param name="connection">A valid SqlConnection</param>
		/// <param name="spName">The name of the stored procedure</param>
		/// <param name="parameterValues">An array of objects to be assigned as the input values of the stored procedure</param>
		/// <returns>A SqlDataReader containing the resultset generated by the command</returns>
		public static SqlDataReader ExecuteReader(SqlConnection connection, string spName, params object[] parameterValues)
		{
			if (connection == null) throw new ArgumentNullException("connection");
			if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

			// If we receive parameter values, we need to figure out where they go
			// 변수값을 받을 경우, 경로를 찾아내야 함
			if ((parameterValues != null) && (parameterValues.Length > 0))
			{
				SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connection, spName);

				AssignParameterValues(commandParameters, parameterValues);

				return ExecuteReader(connection, CommandType.StoredProcedure, spName, commandParameters);
			}
			else
			{
				// Otherwise we can just call the SP without params
				// 아니면 변수없이 SP를 호출한다.
				return ExecuteReader(connection, CommandType.StoredProcedure, spName);
			}
		}

		/// <summary>
		/// Execute a SqlCommand (that returns a resultset and takes no parameters) against the provided SqlTransaction.
		/// 주어진 SqlTransaction에 대한 (resultset를 반환하고 변수값이 없는) SqlCommand 값을 실행시킨다.
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  SqlDataReader dr = ExecuteReader(trans, CommandType.StoredProcedure, "GetOrders");
		/// </remarks>
		/// <param name="transaction">A valid SqlTransaction</param>
		/// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">The stored procedure name or T-SQL command</param>
		/// <returns>A SqlDataReader containing the resultset generated by the command</returns>
		public static SqlDataReader ExecuteReader(SqlTransaction transaction, CommandType commandType, string commandText)
		{
			// Pass through the call providing null for the set of SqlParameters
			// 호출을 통과하여 SqlParameter 세트에 null 값을 제공함
			return ExecuteReader(transaction, commandType, commandText, (SqlParameter[])null);
		}

		/// <summary>
		/// Execute a SqlCommand (that returns a resultset) against the specified SqlTransaction
		/// using the provided parameters.
		/// 주어진 변수값을 이용해 지정된 SqlTransaction에 대한 (resultset을 반환하는) SqlCommand를 실행시킨다.
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///   SqlDataReader dr = ExecuteReader(trans, CommandType.StoredProcedure, "GetOrders", new SqlParameter("@prodid", 24));
		/// </remarks>
		/// <param name="transaction">A valid SqlTransaction</param>
		/// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">The stored procedure name or T-SQL command</param>
		/// <param name="commandParameters">An array of SqlParamters used to execute the command</param>
		/// <returns>A SqlDataReader containing the resultset generated by the command</returns>
		public static SqlDataReader ExecuteReader(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
		{
			if (transaction == null) throw new ArgumentNullException("transaction");
			if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");

			// Pass through to private overload, indicating that the connection is owned by the caller
			// private overload를 통과하여 커넥션이 호출자의 자체 커넥션인 것을 확인
			return ExecuteReader(transaction.Connection, transaction, commandType, commandText, commandParameters, SqlConnectionOwnership.External);
		}

		/// <summary>
		/// Execute a stored procedure via a SqlCommand (that returns a resultset) against the specified
		/// SqlTransaction using the provided parameter values.  This method will query the database to discover the parameters for the 
		/// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
		/// 변수값을 이용해 지정된 SqlTransaction에 대한 (resultset 반환값이 없는) stored procedure를 SqlCommand을 통해 실행시킴
		/// 이 메소드는 데이타베이스를 조회하여 stored procedure의 변수값을 찾아내고(각 stored procedure 처음 호출시),
		/// 변수의 순서에 따라 값을 대입한다.
		/// </summary>
		/// <remarks>
		/// This method provides no access to output parameters or the stored procedure's return value parameter.
		/// 이 메소드는 출력변수, 혹은 stored procedure 반환변수의 접근을 제공하지 않는다.
		/// 
		/// e.g.:  
		///  SqlDataReader dr = ExecuteReader(trans, "GetOrders", 24, 36);
		/// </remarks>
		/// <param name="transaction">A valid SqlTransaction</param>
		/// <param name="spName">The name of the stored procedure</param>
		/// <param name="parameterValues">An array of objects to be assigned as the input values of the stored procedure</param>
		/// <returns>A SqlDataReader containing the resultset generated by the command</returns>
		public static SqlDataReader ExecuteReader(SqlTransaction transaction, string spName, params object[] parameterValues)
		{
			if (transaction == null) throw new ArgumentNullException("transaction");
			if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
			if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

			// If we receive parameter values, we need to figure out where they go
			// 변수값을 받을 경우, 경로를 찾아내야 함
			if ((parameterValues != null) && (parameterValues.Length > 0))
			{
				SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);

				AssignParameterValues(commandParameters, parameterValues);

				return ExecuteReader(transaction, CommandType.StoredProcedure, spName, commandParameters);
			}
			else
			{
				// Otherwise we can just call the SP without params
				// 아니면 변수없이 SP를 호출한다.
				return ExecuteReader(transaction, CommandType.StoredProcedure, spName);
			}
		}

		#endregion ExecuteReader

		#region ExecuteScalar

		/// <summary>
		/// Execute a SqlCommand (that returns a 1x1 resultset and takes no parameters) against the database specified in 
		/// the connection string. 
		/// 커넥션 문자열에 지정된 데이타베이스에 대한 (1x1 resultset을 반환하며 변수값이 없는) SqlCommand를 실행시킴
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  int orderCount = (int)ExecuteScalar(connString, CommandType.StoredProcedure, "GetOrderCount");
		/// </remarks>
		/// <param name="connectionString">A valid connection string for a SqlConnection</param>
		/// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">The stored procedure name or T-SQL command</param>
		/// <returns>An object containing the value in the 1x1 resultset generated by the command</returns>
		public static object ExecuteScalar(string connectionString, CommandType commandType, string commandText)
		{
			// Pass through the call providing null for the set of SqlParameters
			// 호출을 통과하여 SqlParameter 세트에 null 값을 제공함
			return ExecuteScalar(connectionString, commandType, commandText, (SqlParameter[])null);
		}

		/// <summary>
		/// Execute a SqlCommand (that returns a 1x1 resultset) against the database specified in the connection string 
		/// using the provided parameters.
		/// 주어진 변수들로 커넥션 문자열에 지정된 데이타베이스에 대한 (1x1 resultset을 반환하는) SqlCommand를 실행시킴
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  int orderCount = (int)ExecuteScalar(connString, CommandType.StoredProcedure, "GetOrderCount", new SqlParameter("@prodid", 24));
		/// </remarks>
		/// <param name="connectionString">A valid connection string for a SqlConnection</param>
		/// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">The stored procedure name or T-SQL command</param>
		/// <param name="commandParameters">An array of SqlParamters used to execute the command</param>
		/// <returns>An object containing the value in the 1x1 resultset generated by the command</returns>
		public static object ExecuteScalar(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
		{
			if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
			// Create & open a SqlConnection, and dispose of it after we are done
			// SqlConnection을 생성하고 실행한 다음, 사용이 끝난 후 처분
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				connection.Open();

				// Call the overload that takes a connection in place of the connection string
				// 커넥션 문자열 대신에 차지하는 커넥션의 overload 호출
				return ExecuteScalar(connection, commandType, commandText, commandParameters);
			}
		}

		/// <summary>
		/// Execute a stored procedure via a SqlCommand (that returns a 1x1 resultset) against the database specified in 
		/// the connection string using the provided parameter values.  This method will query the database to discover the parameters for the 
		/// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
		///
		/// 변수값을 이용해 커넥션 문자열에 지정된 데이타베이스의 stored procedure를 (1x1 resultset을 반환하는) SqlCommand을 통해 실행시킴
		/// 이 메소드는 데이타베이스를 조회하여 stored procedure의 변수값을 찾아내고(각 stored procedure 처음 호출시),
		/// 변수의 순서에 따라 값을 대입한다.
		/// </summary>
		/// <remarks>
		/// This method provides no access to output parameters or the stored procedure's return value parameter.
		/// 이 메소드는 출력변수, 혹은 stored procedure 반환변수의 접근을 제공하지 않는다.
		/// 
		/// e.g.:  
		///  int orderCount = (int)ExecuteScalar(connString, "GetOrderCount", 24, 36);
		/// </remarks>
		/// <param name="connectionString">A valid connection string for a SqlConnection</param>
		/// <param name="spName">The name of the stored procedure</param>
		/// <param name="parameterValues">An array of objects to be assigned as the input values of the stored procedure</param>
		/// <returns>An object containing the value in the 1x1 resultset generated by the command</returns>
		public static object ExecuteScalar(string connectionString, string spName, params object[] parameterValues)
		{
			if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
			if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

			// If we receive parameter values, we need to figure out where they go
			// 변수값을 받을 경우, 경로를 찾아내야 함
			if ((parameterValues != null) && (parameterValues.Length > 0))
			{
				// Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
				// parameter cache에서 stored procedure 해당 변수값을 끌어온다. (혹은 발견하여 cache에 저장)
				SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);

				// Assign the provided values to these parameters based on parameter order
				// 변수의 순서에 따라 이 값들을 변수에 대입한다.
				AssignParameterValues(commandParameters, parameterValues);

				// Call the overload that takes an array of SqlParameters
				// SqlParameter 배열을 차지하는 overload를 호출
				return ExecuteScalar(connectionString, CommandType.StoredProcedure, spName, commandParameters);
			}
			else
			{
				// Otherwise we can just call the SP without params
				// 아니면 변수없이 SP를 호출한다.
				return ExecuteScalar(connectionString, CommandType.StoredProcedure, spName);
			}
		}

		/// <summary>
		/// Execute a SqlCommand (that returns a 1x1 resultset and takes no parameters) against the provided SqlConnection. 
		/// 주어진 SqlConnection에 대한 (1x1 resultset을 반환하며 변수가 없는)  SqlCommand를 실행시킴
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  int orderCount = (int)ExecuteScalar(conn, CommandType.StoredProcedure, "GetOrderCount");
		/// </remarks>
		/// <param name="connection">A valid SqlConnection</param>
		/// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">The stored procedure name or T-SQL command</param>
		/// <returns>An object containing the value in the 1x1 resultset generated by the command</returns>
		public static object ExecuteScalar(SqlConnection connection, CommandType commandType, string commandText)
		{
			// Pass through the call providing null for the set of SqlParameters
			// 호출을 통과하여 SqlParameter 세트에 null 값을 제공함
			return ExecuteScalar(connection, commandType, commandText, (SqlParameter[])null);
		}

		/// <summary>
		/// Execute a SqlCommand (that returns a 1x1 resultset) against the specified SqlConnection 
		/// using the provided parameters.
		/// 주어진 변수값으로 지정된 SqlConnection에 대한 (1x1 resultset를 반환하는) SqlCommand를 실행시킴
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  int orderCount = (int)ExecuteScalar(conn, CommandType.StoredProcedure, "GetOrderCount", new SqlParameter("@prodid", 24));
		/// </remarks>
		/// <param name="connection">A valid SqlConnection</param>
		/// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">The stored procedure name or T-SQL command</param>
		/// <param name="commandParameters">An array of SqlParamters used to execute the command</param>
		/// <returns>An object containing the value in the 1x1 resultset generated by the command</returns>
		public static object ExecuteScalar(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
		{
			if (connection == null) throw new ArgumentNullException("connection");

			// Create a command and prepare it for execution
			// 명령 생성 및 실행 준비
			SqlCommand cmd = new SqlCommand();

			bool mustCloseConnection = false;
			PrepareCommand(cmd, connection, (SqlTransaction)null, commandType, commandText, commandParameters, out mustCloseConnection);

			// Execute the command & return the results
			// 명령 실행 및 결과값 반환
			object retval = cmd.ExecuteScalar();

			// Detach the SqlParameters from the command object, so they can be used again
			// 재활용을 위해 SqlParameter을 명령 객체에서 때어냄
			cmd.Parameters.Clear();

			if (mustCloseConnection)
				connection.Close();

			return retval;
		}

		/// <summary>
		/// Execute a stored procedure via a SqlCommand (that returns a 1x1 resultset) against the specified SqlConnection 
		/// using the provided parameter values.  This method will query the database to discover the parameters for the 
		/// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
		///
		/// 변수값을 이용해 지정된 SqlConnection에 대한 stored procedure를 (1x1 resultset을 반환하는) SqlCommand을 통해 실행시킴
		/// 이 메소드는 데이타베이스를 조회하여 stored procedure의 변수값을 찾아내고(각 stored procedure 처음 호출시),
		/// 변수의 순서에 따라 값을 대입한다.
		/// </summary>
		/// <remarks>
		/// This method provides no access to output parameters or the stored procedure's return value parameter.
		/// 이 메소드는 출력변수, 혹은 stored procedure 반환변수의 접근을 제공하지 않는다.
		/// 
		/// e.g.:  
		///  int orderCount = (int)ExecuteScalar(conn, "GetOrderCount", 24, 36);
		/// </remarks>
		/// <param name="connection">A valid SqlConnection</param>
		/// <param name="spName">The name of the stored procedure</param>
		/// <param name="parameterValues">An array of objects to be assigned as the input values of the stored procedure</param>
		/// <returns>An object containing the value in the 1x1 resultset generated by the command</returns>
		public static object ExecuteScalar(SqlConnection connection, string spName, params object[] parameterValues)
		{
			if (connection == null) throw new ArgumentNullException("connection");
			if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

			// If we receive parameter values, we need to figure out where they go
			// 변수값을 받을 경우, 경로를 찾아내야 함
			if ((parameterValues != null) && (parameterValues.Length > 0))
			{
				// Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
				// parameter cache에서 stored procedure 해당 변수값을 끌어온다. (혹은 발견하여 cache에 저장)
				SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connection, spName);

				// Assign the provided values to these parameters based on parameter order
				// 변수의 순서에 따라 이 값들을 변수에 대입한다.
				AssignParameterValues(commandParameters, parameterValues);

				// Call the overload that takes an array of SqlParameters
				// SqlParameter 배열을 차지하는 overload를 호출
				return ExecuteScalar(connection, CommandType.StoredProcedure, spName, commandParameters);
			}
			else
			{
				// Otherwise we can just call the SP without params
				// 아니면 변수없이 SP를 호출한다.
				return ExecuteScalar(connection, CommandType.StoredProcedure, spName);
			}
		}

		/// <summary>
		/// Execute a SqlCommand (that returns a 1x1 resultset and takes no parameters) against the provided SqlTransaction. 
		/// 주어진 SqlTransaction에 대한 (1x1 resultset을 반환하며 변수가 없는)  SqlCommand를 실행시킴
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  int orderCount = (int)ExecuteScalar(trans, CommandType.StoredProcedure, "GetOrderCount");
		/// </remarks>
		/// <param name="transaction">A valid SqlTransaction</param>
		/// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">The stored procedure name or T-SQL command</param>
		/// <returns>An object containing the value in the 1x1 resultset generated by the command</returns>
		public static object ExecuteScalar(SqlTransaction transaction, CommandType commandType, string commandText)
		{
			// Pass through the call providing null for the set of SqlParameters
			// 호출을 통과하여 SqlParameter 세트에 null 값을 제공함
			return ExecuteScalar(transaction, commandType, commandText, (SqlParameter[])null);
		}

		/// <summary>
		/// Execute a SqlCommand (that returns a 1x1 resultset) against the specified SqlTransaction
		/// using the provided parameters.
		/// 주어진 SqlTransaction에 대한 (1x1 resultset을 반환하는)  SqlCommand를 실행시킴
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  int orderCount = (int)ExecuteScalar(trans, CommandType.StoredProcedure, "GetOrderCount", new SqlParameter("@prodid", 24));
		/// </remarks>
		/// <param name="transaction">A valid SqlTransaction</param>
		/// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">The stored procedure name or T-SQL command</param>
		/// <param name="commandParameters">An array of SqlParamters used to execute the command</param>
		/// <returns>An object containing the value in the 1x1 resultset generated by the command</returns>
		public static object ExecuteScalar(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
		{
			if (transaction == null) throw new ArgumentNullException("transaction");
			if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");

			// Create a command and prepare it for execution
			// 명령 생성 및 실행 준비
			SqlCommand cmd = new SqlCommand();
			bool mustCloseConnection = false;
			PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters, out mustCloseConnection);

			// Execute the command & return the results
			// 명령 실행 및 결과값 반환
			object retval = cmd.ExecuteScalar();

			// Detach the SqlParameters from the command object, so they can be used again
			// 재활용을 위해 SqlParameter을 명령 객체에서 때어냄
			cmd.Parameters.Clear();
			return retval;
		}

		/// <summary>
		/// Execute a stored procedure via a SqlCommand (that returns a 1x1 resultset) against the specified
		/// SqlTransaction using the provided parameter values.  This method will query the database to discover the parameters for the 
		/// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
		///
		/// 변수값을 이용해 지정된 SqlTransaction에 대한 stored procedure를 (1x1 resultset을 반환하는) SqlCommand을 통해 실행시킴
		/// 이 메소드는 데이타베이스를 조회하여 stored procedure의 변수값을 찾아내고(각 stored procedure 처음 호출시),
		/// 변수의 순서에 따라 값을 대입한다.
		/// </summary>
		/// <remarks>
		/// This method provides no access to output parameters or the stored procedure's return value parameter.
		/// 이 메소드는 출력변수, 혹은 stored procedure 반환변수의 접근을 제공하지 않는다.
		/// 
		/// e.g.:  
		///  int orderCount = (int)ExecuteScalar(trans, "GetOrderCount", 24, 36);
		/// </remarks>
		/// <param name="transaction">A valid SqlTransaction</param>
		/// <param name="spName">The name of the stored procedure</param>
		/// <param name="parameterValues">An array of objects to be assigned as the input values of the stored procedure</param>
		/// <returns>An object containing the value in the 1x1 resultset generated by the command</returns>
		public static object ExecuteScalar(SqlTransaction transaction, string spName, params object[] parameterValues)
		{
			if (transaction == null) throw new ArgumentNullException("transaction");
			if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
			if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

			// If we receive parameter values, we need to figure out where they go
			// 변수값을 받을 경우, 경로를 찾아내야 함
			if ((parameterValues != null) && (parameterValues.Length > 0))
			{
				// Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
				// parameter cache에서 stored procedure 해당 변수값을 끌어온다. (혹은 발견하여 cache에 저장)
				SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);

				// Assign the provided values to these parameters based on parameter order
				// 변수의 순서에 따라 이 값들을 변수에 대입한다.
				AssignParameterValues(commandParameters, parameterValues);

				// Call the overload that takes an array of SqlParameters
				// SqlParameter 배열을 차지하는 overload를 호출
				return ExecuteScalar(transaction, CommandType.StoredProcedure, spName, commandParameters);
			}
			else
			{
				// Otherwise we can just call the SP without params
				// 아니면 변수없이 SP를 호출한다.
				return ExecuteScalar(transaction, CommandType.StoredProcedure, spName);
			}
		}

		#endregion ExecuteScalar

		#region ExecuteXmlReader
		/// <summary>
		/// Execute a SqlCommand (that returns a resultset and takes no parameters) against the provided SqlConnection.
		/// 지정된 SqlConnection에 대한 (resultset을 반환하고 변수값이 없는) SqlCommand를 실행시킨다.
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  XmlReader r = ExecuteXmlReader(conn, CommandType.StoredProcedure, "GetOrders");
		/// </remarks>
		/// <param name="connection">A valid SqlConnection</param>
		/// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">The stored procedure name or T-SQL command using "FOR XML AUTO"</param>
		/// <returns>An XmlReader containing the resultset generated by the command</returns>
		public static XmlReader ExecuteXmlReader(SqlConnection connection, CommandType commandType, string commandText)
		{
			// Pass through the call providing null for the set of SqlParameters
			// 호출을 통과하여 SqlParameter 세트에 null 값을 제공함
			return ExecuteXmlReader(connection, commandType, commandText, (SqlParameter[])null);
		}

		/// <summary>
		/// Execute a SqlCommand (that returns a resultset) against the specified SqlConnection 
		/// using the provided parameters.
		/// 주어진 변수값을 이용해 지정된 SqlConnection에 대한 (resultset을 반환하는) SqlCommand를 실행시킨다.
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  XmlReader r = ExecuteXmlReader(conn, CommandType.StoredProcedure, "GetOrders", new SqlParameter("@prodid", 24));
		/// </remarks>
		/// <param name="connection">A valid SqlConnection</param>
		/// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">The stored procedure name or T-SQL command using "FOR XML AUTO"</param>
		/// <param name="commandParameters">An array of SqlParamters used to execute the command</param>
		/// <returns>An XmlReader containing the resultset generated by the command</returns>
		public static XmlReader ExecuteXmlReader(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
		{
			if (connection == null) throw new ArgumentNullException("connection");

			bool mustCloseConnection = false;
			// Create a command and prepare it for execution
			// 명령 생성 및 실행 준비
			SqlCommand cmd = new SqlCommand();
			try
			{
				PrepareCommand(cmd, connection, (SqlTransaction)null, commandType, commandText, commandParameters, out mustCloseConnection);

				// Create the DataAdapter & DataSet
				// DataAdapter 와 DataSet 생성
				XmlReader retval = cmd.ExecuteXmlReader();

				// Detach the SqlParameters from the command object, so they can be used again
				// 재활용을 위해 SqlParameter을 명령 객체에서 때어냄
				cmd.Parameters.Clear();

				return retval;
			}
			catch
			{
				if (mustCloseConnection)
					connection.Close();
				throw;
			}
		}

		/// <summary>
		/// Execute a stored procedure via a SqlCommand (that returns a resultset) against the specified SqlConnection 
		/// using the provided parameter values.  This method will query the database to discover the parameters for the 
		/// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
		///
		/// 변수값을 이용해 지정된 SqlConnection에 대한 (resultset를 반환하는) stored procedure를 SqlCommand을 통해 실행시킴
		/// 이 메소드는 데이타베이스를 조회하여 stored procedure의 변수값을 찾아내고(각 stored procedure 처음 호출시),
		/// 변수의 순서에 따라 값을 대입한다.
		/// </summary>
		/// <remarks>
		/// This method provides no access to output parameters or the stored procedure's return value parameter.
		/// 이 메소드는 출력변수, 혹은 stored procedure 반환변수의 접근을 제공하지 않는다.
		/// 
		/// e.g.:  
		///  XmlReader r = ExecuteXmlReader(conn, "GetOrders", 24, 36);
		/// </remarks>
		/// <param name="connection">A valid SqlConnection</param>
		/// <param name="spName">The name of the stored procedure using "FOR XML AUTO"</param>
		/// <param name="parameterValues">An array of objects to be assigned as the input values of the stored procedure</param>
		/// <returns>An XmlReader containing the resultset generated by the command</returns>
		public static XmlReader ExecuteXmlReader(SqlConnection connection, string spName, params object[] parameterValues)
		{
			if (connection == null) throw new ArgumentNullException("connection");
			if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

			// If we receive parameter values, we need to figure out where they go
			// 변수값을 받을 경우, 경로를 찾아내야 함
			if ((parameterValues != null) && (parameterValues.Length > 0))
			{
				// Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
				// parameter cache에서 stored procedure 해당 변수값을 끌어온다. (혹은 발견하여 cache에 저장)
				SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connection, spName);

				// Assign the provided values to these parameters based on parameter order
				// 변수의 순서에 따라 이 값들을 변수에 대입한다.
				AssignParameterValues(commandParameters, parameterValues);

				// Call the overload that takes an array of SqlParameters
				// SqlParameter 배열을 차지하는 overload를 호출
				return ExecuteXmlReader(connection, CommandType.StoredProcedure, spName, commandParameters);
			}
			else
			{
				// Otherwise we can just call the SP without params
				// 아니면 변수없이 SP를 호출한다.
				return ExecuteXmlReader(connection, CommandType.StoredProcedure, spName);
			}
		}

		/// <summary>
		/// Execute a SqlCommand (that returns a resultset and takes no parameters) against the provided SqlTransaction.
		/// 주어진 SqlTransaction에 대한 (resultset를 반환하고 변수값이 없는) SqlCommand 값을 실행시킨다.
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  XmlReader r = ExecuteXmlReader(trans, CommandType.StoredProcedure, "GetOrders");
		/// </remarks>
		/// <param name="transaction">A valid SqlTransaction</param>
		/// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">The stored procedure name or T-SQL command using "FOR XML AUTO"</param>
		/// <returns>An XmlReader containing the resultset generated by the command</returns>
		public static XmlReader ExecuteXmlReader(SqlTransaction transaction, CommandType commandType, string commandText)
		{
			// Pass through the call providing null for the set of SqlParameters
			// 호출을 통과하여 SqlParameter 세트에 null 값을 제공함
			return ExecuteXmlReader(transaction, commandType, commandText, (SqlParameter[])null);
		}

		/// <summary>
		/// Execute a SqlCommand (that returns a resultset) against the specified SqlTransaction
		/// using the provided parameters.
		/// 주어진 변수값을 이용해 지정된 SqlTransaction에 대한 (resultset를 반환하는) SqlCommand를 실행시킨다.
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  XmlReader r = ExecuteXmlReader(trans, CommandType.StoredProcedure, "GetOrders", new SqlParameter("@prodid", 24));
		/// </remarks>
		/// <param name="transaction">A valid SqlTransaction</param>
		/// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">The stored procedure name or T-SQL command using "FOR XML AUTO"</param>
		/// <param name="commandParameters">An array of SqlParamters used to execute the command</param>
		/// <returns>An XmlReader containing the resultset generated by the command</returns>
		public static XmlReader ExecuteXmlReader(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
		{
			if (transaction == null) throw new ArgumentNullException("transaction");
			if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");

			// Create a command and prepare it for execution
			// 명령 생성 및 실행 준비
			SqlCommand cmd = new SqlCommand();
			bool mustCloseConnection = false;
			PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters, out mustCloseConnection);

			// Create the DataAdapter & DataSet
			// DataAdapter 와 DataSet 생성
			XmlReader retval = cmd.ExecuteXmlReader();

			// Detach the SqlParameters from the command object, so they can be used again
			// 재활용을 위해 SqlParameter을 명령 객체에서 때어냄
			cmd.Parameters.Clear();
			return retval;
		}

		/// <summary>
		/// Execute a stored procedure via a SqlCommand (that returns a resultset) against the specified 
		/// SqlTransaction using the provided parameter values.  This method will query the database to discover the parameters for the 
		/// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
		///
		/// 변수값을 이용해 지정된 SqlTransaction에 대한 stored procedure를 (resultset를 반환하는) SqlCommand을 통해 실행시킴
		/// 이 메소드는 데이타베이스를 조회하여 stored procedure의 변수값을 찾아내고(각 stored procedure 처음 호출시),
		/// 변수의 순서에 따라 값을 대입한다.
		/// </summary>
		/// <remarks>
		/// This method provides no access to output parameters or the stored procedure's return value parameter.
		/// 이 메소드는 출력변수, 혹은 stored procedure 반환변수의 접근을 제공하지 않는다.
		/// 
		/// e.g.:  
		///  XmlReader r = ExecuteXmlReader(trans, "GetOrders", 24, 36);
		/// </remarks>
		/// <param name="transaction">A valid SqlTransaction</param>
		/// <param name="spName">The name of the stored procedure</param>
		/// <param name="parameterValues">An array of objects to be assigned as the input values of the stored procedure</param>
		/// <returns>A dataset containing the resultset generated by the command</returns>
		public static XmlReader ExecuteXmlReader(SqlTransaction transaction, string spName, params object[] parameterValues)
		{
			if (transaction == null) throw new ArgumentNullException("transaction");
			if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
			if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

			// If we receive parameter values, we need to figure out where they go
			// 변수값을 받을 경우, 경로를 찾아내야 함
			if ((parameterValues != null) && (parameterValues.Length > 0))
			{
				// Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
				// parameter cache에서 stored procedure 해당 변수값을 끌어온다. (혹은 발견하여 cache에 저장)
				SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);

				// Assign the provided values to these parameters based on parameter order
				// 변수의 순서에 따라 이 값들을 변수에 대입한다.
				AssignParameterValues(commandParameters, parameterValues);

				// Call the overload that takes an array of SqlParameters
				// SqlParameter 배열을 차지하는 overload를 호출
				return ExecuteXmlReader(transaction, CommandType.StoredProcedure, spName, commandParameters);
			}
			else
			{
				// Otherwise we can just call the SP without params
				// 아니면 변수없이 SP를 호출한다.
				return ExecuteXmlReader(transaction, CommandType.StoredProcedure, spName);
			}
		}

		#endregion ExecuteXmlReader

		#region FillDataset
		/// <summary>
		/// Execute a SqlCommand (that returns a resultset and takes no parameters) against the database specified in 
		/// the connection string. 
		/// 커넥션 문자열에 지정된 데이타베이스에 대한 (resultset을 반환하며 변수값이 없는) SqlCommand를 실행시킴
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  FillDataset(connString, CommandType.StoredProcedure, "GetOrders", ds, new string[] {"orders"});
		/// </remarks>
		/// <param name="connectionString">A valid connection string for a SqlConnection</param>
		/// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">The stored procedure name or T-SQL command</param>
		/// <param name="dataSet">A dataset wich will contain the resultset generated by the command</param>
		/// <param name="tableNames">This array will be used to create table mappings allowing the DataTables to be referenced
		/// by a user defined name (probably the actual table name)</param>
		public static void FillDataset(string connectionString, CommandType commandType, string commandText, DataSet dataSet, string[] tableNames)
		{
			if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
			if (dataSet == null) throw new ArgumentNullException("dataSet");

			// Create & open a SqlConnection, and dispose of it after we are done
			// SqlConnection을 생성하고 실행한 다음, 사용이 끝난 후 처분
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				connection.Open();

				// Call the overload that takes a connection in place of the connection string
				// 커넥션 문자열 대신에 차지하는 커넥션의 overload 호출
				FillDataset(connection, commandType, commandText, dataSet, tableNames);
			}
		}





		/// <summary>
		/// Execute a SqlCommand (that returns a resultset) against the database specified in the connection string 
		/// using the provided parameters.
		/// 주어진 변수들로 커넥션 문자열에 지정된 데이타베이스에 대한 (resultset을 반환하는) SqlCommand를 실행시킴
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  FillDataset(connString, CommandType.StoredProcedure, "GetOrders", ds, new string[] {"orders"}, new SqlParameter("@prodid", 24));
		/// </remarks>
		/// <param name="connectionString">A valid connection string for a SqlConnection</param>
		/// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">The stored procedure name or T-SQL command</param>
		/// <param name="commandParameters">An array of SqlParamters used to execute the command</param>
		/// <param name="dataSet">A dataset wich will contain the resultset generated by the command</param>
		/// <param name="tableNames">This array will be used to create table mappings allowing the DataTables to be referenced
		/// by a user defined name (probably the actual table name)
		/// </param>
		public static void FillDataset(string connectionString, CommandType commandType,
			string commandText, DataSet dataSet, string[] tableNames,
			params SqlParameter[] commandParameters)
		{
			if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
			if (dataSet == null) throw new ArgumentNullException("dataSet");
			// Create & open a SqlConnection, and dispose of it after we are done
			// SqlConnection을 생성하고 실행한 다음, 사용이 끝난 후 처분
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				connection.Open();

				// Call the overload that takes a connection in place of the connection string
				// 커넥션 문자열 대신에 차지하는 커넥션의 overload 호출
				FillDataset(connection, commandType, commandText, dataSet, tableNames, commandParameters);
			}
		}

		/// <summary>
		/// Execute a stored procedure via a SqlCommand (that returns a resultset) against the database specified in 
		/// the connection string using the provided parameter values.  This method will query the database to discover the parameters for the 
		/// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
		///
		/// 변수값을 이용해 커넥션 문자열에 지정된 데이타베이스의 stored procedure를 (resultset을 반환하는) SqlCommand을 통해 실행시킴
		/// 이 메소드는 데이타베이스를 조회하여 stored procedure의 변수값을 찾아내고(각 stored procedure 처음 호출시),
		/// 변수의 순서에 따라 값을 대입한다.
		/// </summary>
		/// <remarks>
		/// This method provides no access to output parameters or the stored procedure's return value parameter.
		/// 이 메소드는 출력변수, 혹은 stored procedure 반환변수의 접근을 제공하지 않는다.
		/// 
		/// e.g.:  
		///  FillDataset(connString, CommandType.StoredProcedure, "GetOrders", ds, new string[] {"orders"}, 24);
		/// </remarks>
		/// <param name="connectionString">A valid connection string for a SqlConnection</param>
		/// <param name="spName">The name of the stored procedure</param>
		/// <param name="dataSet">A dataset wich will contain the resultset generated by the command</param>
		/// <param name="tableNames">This array will be used to create table mappings allowing the DataTables to be referenced
		/// by a user defined name (probably the actual table name)
		/// </param>    
		/// <param name="parameterValues">An array of objects to be assigned as the input values of the stored procedure</param>
		public static void FillDataset(string connectionString, string spName,
			DataSet dataSet, string[] tableNames,
			params object[] parameterValues)
		{
			if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
			if (dataSet == null) throw new ArgumentNullException("dataSet");
			// Create & open a SqlConnection, and dispose of it after we are done
			// SqlConnection을 생성하고 실행한 다음, 사용이 끝난 후 처분
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				connection.Open();

				// Call the overload that takes a connection in place of the connection string
				// 커넥션 문자열 대신에 차지하는 커넥션의 overload 호출
				FillDataset(connection, spName, dataSet, tableNames, parameterValues);
			}
		}

		/// <summary>
		/// Execute a SqlCommand (that returns a resultset and takes no parameters) against the provided SqlConnection. 
		/// 지정된 SqlConnection에 대한 (resultset을 반환하고 변수값이 없는) SqlCommand를 실행시킨다.
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  FillDataset(conn, CommandType.StoredProcedure, "GetOrders", ds, new string[] {"orders"});
		/// </remarks>
		/// <param name="connection">A valid SqlConnection</param>
		/// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">The stored procedure name or T-SQL command</param>
		/// <param name="dataSet">A dataset wich will contain the resultset generated by the command</param>
		/// <param name="tableNames">This array will be used to create table mappings allowing the DataTables to be referenced
		/// by a user defined name (probably the actual table name)
		/// </param>    
		public static void FillDataset(SqlConnection connection, CommandType commandType,
			string commandText, DataSet dataSet, string[] tableNames)
		{
			FillDataset(connection, commandType, commandText, dataSet, tableNames, null);
		}

		/// <summary>
		/// Execute a SqlCommand (that returns a resultset) against the specified SqlConnection 
		/// using the provided parameters.
		/// 지정된 SqlConnection에 대한 (resultset을 반환하는) SqlCommand를 실행시킨다.
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  FillDataset(conn, CommandType.StoredProcedure, "GetOrders", ds, new string[] {"orders"}, new SqlParameter("@prodid", 24));
		/// </remarks>
		/// <param name="connection">A valid SqlConnection</param>
		/// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">The stored procedure name or T-SQL command</param>
		/// <param name="dataSet">A dataset wich will contain the resultset generated by the command</param>
		/// <param name="tableNames">This array will be used to create table mappings allowing the DataTables to be referenced
		/// by a user defined name (probably the actual table name)
		/// </param>
		/// <param name="commandParameters">An array of SqlParamters used to execute the command</param>
		public static void FillDataset(SqlConnection connection, CommandType commandType,
			string commandText, DataSet dataSet, string[] tableNames,
			params SqlParameter[] commandParameters)
		{
			FillDataset(connection, null, commandType, commandText, dataSet, tableNames, commandParameters);
		}

		/// <summary>
		/// Execute a stored procedure via a SqlCommand (that returns a resultset) against the specified SqlConnection 
		/// using the provided parameter values.  This method will query the database to discover the parameters for the 
		/// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
		///
		/// 변수값을 이용해 지정된 SqlConnection에 대한 stored procedure를 (resultset를 반환하는) SqlCommand을 통해 실행시킴
		/// 이 메소드는 데이타베이스를 조회하여 stored procedure의 변수값을 찾아내고(각 stored procedure 처음 호출시),
		/// 변수의 순서에 따라 값을 대입한다.
		/// </summary>
		/// <remarks>
		/// This method provides no access to output parameters or the stored procedure's return value parameter.
		/// 이 메소드는 출력변수, 혹은 stored procedure 반환변수의 접근을 제공하지 않는다.
		/// 
		/// e.g.:  
		///  FillDataset(conn, "GetOrders", ds, new string[] {"orders"}, 24, 36);
		/// </remarks>
		/// <param name="connection">A valid SqlConnection</param>
		/// <param name="spName">The name of the stored procedure</param>
		/// <param name="dataSet">A dataset wich will contain the resultset generated by the command</param>
		/// <param name="tableNames">This array will be used to create table mappings allowing the DataTables to be referenced
		/// by a user defined name (probably the actual table name)
		/// </param>
		/// <param name="parameterValues">An array of objects to be assigned as the input values of the stored procedure</param>
		public static void FillDataset(SqlConnection connection, string spName,
			DataSet dataSet, string[] tableNames,
			params object[] parameterValues)
		{
			if (connection == null) throw new ArgumentNullException("connection");
			if (dataSet == null) throw new ArgumentNullException("dataSet");
			if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

			// If we receive parameter values, we need to figure out where they go
			// 변수값을 받을 경우, 경로를 찾아내야 함
			if ((parameterValues != null) && (parameterValues.Length > 0))
			{
				// Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
				// parameter cache에서 stored procedure 해당 변수값을 끌어온다. (혹은 발견하여 cache에 저장)
				SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connection, spName);

				// Assign the provided values to these parameters based on parameter order
				// 변수의 순서에 따라 이 값들을 변수에 대입한다.
				AssignParameterValues(commandParameters, parameterValues);

				// Call the overload that takes an array of SqlParameters
				// SqlParameter 배열을 차지하는 overload를 호출
				FillDataset(connection, CommandType.StoredProcedure, spName, dataSet, tableNames, commandParameters);
			}
			else
			{
				// Otherwise we can just call the SP without params
				// 아니면 변수없이 SP를 호출한다.
				FillDataset(connection, CommandType.StoredProcedure, spName, dataSet, tableNames);
			}
		}

		/// <summary>
		/// Execute a SqlCommand (that returns a resultset and takes no parameters) against the provided SqlTransaction.
		/// 주어진 SqlTransaction에 대한 (resultset를 반환하고 변수값이 없는) SqlCommand 값을 실행시킨다.
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  FillDataset(trans, CommandType.StoredProcedure, "GetOrders", ds, new string[] {"orders"});
		/// </remarks>
		/// <param name="transaction">A valid SqlTransaction</param>
		/// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">The stored procedure name or T-SQL command</param>
		/// <param name="dataSet">A dataset wich will contain the resultset generated by the command</param>
		/// <param name="tableNames">This array will be used to create table mappings allowing the DataTables to be referenced
		/// by a user defined name (probably the actual table name)
		/// </param>
		public static void FillDataset(SqlTransaction transaction, CommandType commandType,
			string commandText,
			DataSet dataSet, string[] tableNames)
		{
			FillDataset(transaction, commandType, commandText, dataSet, tableNames, null);
		}

		/// <summary>
		/// Execute a SqlCommand (that returns a resultset) against the specified SqlTransaction
		/// using the provided parameters.
		/// 주어진 변수값을 이용해 지정된 SqlTransaction에 대한 (resultset를 반환하는) SqlCommand를 실행시킨다.
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  FillDataset(trans, CommandType.StoredProcedure, "GetOrders", ds, new string[] {"orders"}, new SqlParameter("@prodid", 24));
		/// </remarks>
		/// <param name="transaction">A valid SqlTransaction</param>
		/// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">The stored procedure name or T-SQL command</param>
		/// <param name="dataSet">A dataset wich will contain the resultset generated by the command</param>
		/// <param name="tableNames">This array will be used to create table mappings allowing the DataTables to be referenced
		/// by a user defined name (probably the actual table name)
		/// </param>
		/// <param name="commandParameters">An array of SqlParamters used to execute the command</param>
		public static void FillDataset(SqlTransaction transaction, CommandType commandType,
			string commandText, DataSet dataSet, string[] tableNames,
			params SqlParameter[] commandParameters)
		{
			FillDataset(transaction.Connection, transaction, commandType, commandText, dataSet, tableNames, commandParameters);
		}

		/// <summary>
		/// Execute a stored procedure via a SqlCommand (that returns a resultset) against the specified 
		/// SqlTransaction using the provided parameter values.  This method will query the database to discover the parameters for the 
		/// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
		///
		/// 변수값을 이용해 지정된 SqlTransaction에 대한 stored procedure를 (resultset 반환하는) SqlCommand을 통해 실행시킴
		/// 이 메소드는 데이타베이스를 조회하여 stored procedure의 변수값을 찾아내고(각 stored procedure 처음 호출시),
		/// 변수의 순서에 따라 값을 대입한다.
		/// </summary>
		/// <remarks>
		/// This method provides no access to output parameters or the stored procedure's return value parameter.
		/// 이 메소드는 출력변수, 혹은 stored procedure 반환변수의 접근을 제공하지 않는다.
		/// 
		/// e.g.:  
		///  FillDataset(trans, "GetOrders", ds, new string[]{"orders"}, 24, 36);
		/// </remarks>
		/// <param name="transaction">A valid SqlTransaction</param>
		/// <param name="spName">The name of the stored procedure</param>
		/// <param name="dataSet">A dataset wich will contain the resultset generated by the command</param>
		/// <param name="tableNames">This array will be used to create table mappings allowing the DataTables to be referenced
		/// by a user defined name (probably the actual table name)
		/// </param>
		/// <param name="parameterValues">An array of objects to be assigned as the input values of the stored procedure</param>
		public static void FillDataset(SqlTransaction transaction, string spName,
			DataSet dataSet, string[] tableNames,
			params object[] parameterValues)
		{
			if (transaction == null) throw new ArgumentNullException("transaction");
			if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
			if (dataSet == null) throw new ArgumentNullException("dataSet");
			if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

			// If we receive parameter values, we need to figure out where they go
			// 변수값을 받을 경우, 경로를 찾아내야 함
			if ((parameterValues != null) && (parameterValues.Length > 0))
			{
				// Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
				// parameter cache에서 stored procedure 해당 변수값을 끌어온다. (혹은 발견하여 cache에 저장)
				SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);

				// Assign the provided values to these parameters based on parameter order
				// 변수의 순서에 따라 이 값들을 변수에 대입한다.
				AssignParameterValues(commandParameters, parameterValues);

				// Call the overload that takes an array of SqlParameters
				// SqlParameter 배열을 차지하는 overload를 호출
				FillDataset(transaction, CommandType.StoredProcedure, spName, dataSet, tableNames, commandParameters);
			}
			else
			{
				// Otherwise we can just call the SP without params
				// 아니면 변수없이 SP를 호출한다.
				FillDataset(transaction, CommandType.StoredProcedure, spName, dataSet, tableNames);
			}
		}

		/// <summary>
		/// Private helper method that execute a SqlCommand (that returns a resultset) against the specified SqlTransaction and SqlConnection
		/// using the provided parameters.
		/// 주어진 변수값을 이용해 지정된 SqlTransaction과 SqlConnection에 대한 (resultset을 반환하는) SqlCommand를 실행시키는 private 도움 메소드
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  FillDataset(conn, trans, CommandType.StoredProcedure, "GetOrders", ds, new string[] {"orders"}, new SqlParameter("@prodid", 24));
		/// </remarks>
		/// <param name="connection">A valid SqlConnection</param>
		/// <param name="transaction">A valid SqlTransaction</param>
		/// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">The stored procedure name or T-SQL command</param>
		/// <param name="dataSet">A dataset wich will contain the resultset generated by the command</param>
		/// <param name="tableNames">This array will be used to create table mappings allowing the DataTables to be referenced
		/// by a user defined name (probably the actual table name)
		/// </param>
		/// <param name="commandParameters">An array of SqlParamters used to execute the command</param>
		private static void FillDataset(SqlConnection connection, SqlTransaction transaction, CommandType commandType,
			string commandText, DataSet dataSet, string[] tableNames,
			params SqlParameter[] commandParameters)
		{
			if (connection == null) throw new ArgumentNullException("connection");
			if (dataSet == null) throw new ArgumentNullException("dataSet");

			// Create a command and prepare it for execution
			// 명령 생성 및 실행 준비
			SqlCommand command = new SqlCommand();
			bool mustCloseConnection = false;
			PrepareCommand(command, connection, transaction, commandType, commandText, commandParameters, out mustCloseConnection);

			// Create the DataAdapter & DataSet
			// DataAdapter 와 DataSet 생성
			using (SqlDataAdapter dataAdapter = new SqlDataAdapter(command))
			{

				// Add the table mappings specified by the user
				// 유저가 지정한 table mapping을 추가
				if (tableNames != null && tableNames.Length > 0)
				{
					string tableName = "Table";
					for (int index = 0; index < tableNames.Length; index++)
					{
						if (tableNames[index] == null || tableNames[index].Length == 0) throw new ArgumentException("The tableNames parameter must contain a list of tables, a value was provided as null or empty string.", "tableNames");
						dataAdapter.TableMappings.Add(tableName, tableNames[index]);
						tableName += (index + 1).ToString();
					}
				}

				// Fill the DataSet using default values for DataTable names, etc
				// DataTable names 기본값으로 DataSet을 채워나감
				dataAdapter.Fill(dataSet);

				// Detach the SqlParameters from the command object, so they can be used again
				// 재활용을 위해 SqlParameter을 명령 객체에서 때어냄
				command.Parameters.Clear();
			}

			if (mustCloseConnection)
				connection.Close();
		}
		#endregion

		#region UpdateDataset
		/// <summary>
		/// Executes the respective command for each inserted, updated, or deleted row in the DataSet.
		/// DataSet에서 추가, 업데이트, 삭제된 열에 해당하는 명령을 각각 실행시킴.
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  UpdateDataset(conn, insertCommand, deleteCommand, updateCommand, dataSet, "Order");
		/// </remarks>
		/// <param name="insertCommand">A valid transact-SQL statement or stored procedure to insert new records into the data source</param>
		/// <param name="deleteCommand">A valid transact-SQL statement or stored procedure to delete records from the data source</param>
		/// <param name="updateCommand">A valid transact-SQL statement or stored procedure used to update records in the data source</param>
		/// <param name="dataSet">The DataSet used to update the data source</param>
		/// <param name="tableName">The DataTable used to update the data source.</param>
		public static void UpdateDataset(SqlCommand insertCommand, SqlCommand deleteCommand, SqlCommand updateCommand, DataSet dataSet, string tableName)
		{
			if (insertCommand == null) throw new ArgumentNullException("insertCommand");
			if (deleteCommand == null) throw new ArgumentNullException("deleteCommand");
			if (updateCommand == null) throw new ArgumentNullException("updateCommand");
			if (tableName == null || tableName.Length == 0) throw new ArgumentNullException("tableName");

			// Create a SqlDataAdapter, and dispose of it after we are done
			// SqlDataAdapter를 생성하고 사용이 끝난 후 처분
			using (SqlDataAdapter dataAdapter = new SqlDataAdapter())
			{
				// Set the data adapter commands
				// 데이타 접속기(data adapter) 명령 지정
				dataAdapter.UpdateCommand = updateCommand;
				dataAdapter.InsertCommand = insertCommand;
				dataAdapter.DeleteCommand = deleteCommand;

				// Update the dataset changes in the data source
				// 데이타 소스상의 dataset 변경부분 업데이트
				dataAdapter.Update(dataSet, tableName);

				// Commit all the changes made to the DataSet
				// DataSet의 변경내용 모두 실행
				dataSet.AcceptChanges();
			}
		}
		#endregion

		#region CreateCommand
		/// <summary>
		/// Simplify the creation of a Sql command object by allowing
		/// a stored procedure and optional parameters to be provided
		/// stored procedure와 임의의 변수를 제공하여 Sql 명령 객체 생성을 최소화
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  SqlCommand command = CreateCommand(conn, "AddCustomer", "CustomerID", "CustomerName");
		/// </remarks>
		/// <param name="connection">A valid SqlConnection object</param>
		/// <param name="spName">The name of the stored procedure</param>
		/// <param name="sourceColumns">An array of string to be assigned as the source columns of the stored procedure parameters</param>
		/// <returns>A valid SqlCommand object</returns>
		public static SqlCommand CreateCommand(SqlConnection connection, string spName, params string[] sourceColumns)
		{
			if (connection == null) throw new ArgumentNullException("connection");
			if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

			// Create a SqlCommand
			// SqlCommand 생성
			SqlCommand cmd = new SqlCommand(spName, connection);
			cmd.CommandType = CommandType.StoredProcedure;

			// If we receive parameter values, we need to figure out where they go
			// 변수값을 받을 경우, 경로를 찾아내야 함
			if ((sourceColumns != null) && (sourceColumns.Length > 0))
			{
				// Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
				// parameter cache에서 stored procedure 해당 변수값을 끌어온다. (혹은 발견하여 cache에 저장)
				SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connection, spName);

				// Assign the provided source columns to these parameters based on parameter order
				// 주어진 source 열들을 변수에 순서에 따라 지정 
				for (int index = 0; index < sourceColumns.Length; index++)
					commandParameters[index].SourceColumn = sourceColumns[index];

				// Attach the discovered parameters to the SqlCommand object
				// 발견한 변수들을 SqlCommand 객체에 추가
				AttachParameters(cmd, commandParameters);
			}

			return cmd;
		}
		#endregion

		#region ExecuteNonQueryTypedParams
		/// <summary>
		/// Execute a stored procedure via a SqlCommand (that returns no resultset) against the database specified in 
		/// the connection string using the dataRow column values as the stored procedure's parameters values.
		/// This method will query the database to discover the parameters for the 
		/// stored procedure (the first time each stored procedure is called), and assign the values based on row values.
		///
		/// dataRow 열의 값들을 stored procedure 변수값으로 사용하여
		/// 커넥션 문자열에 지정된 데이타베이스의 stored procedure를 (resultset 반환하지 않는) SqlCommand을 통해 실행시킴
		/// 이 메소드는 데이타베이스를 조회하여 stored procedure의 변수값을 찾아내고(각 stored procedure 처음 호출시),
		/// 열의 순서에 따라 값을 대입한다.
		/// </summary>
		/// <param name="connectionString">A valid connection string for a SqlConnection</param>
		/// <param name="spName">The name of the stored procedure</param>
		/// <param name="dataRow">The dataRow used to hold the stored procedure's parameter values.</param>
		/// <returns>An int representing the number of rows affected by the command</returns>
		public static int ExecuteNonQueryTypedParams(String connectionString, String spName, DataRow dataRow)
		{
			if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
			if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

			// If the row has values, the store procedure parameters must be initialized
			// 열이 값을 포함하고 있으면, store procedure 변수 초기화
			if (dataRow != null && dataRow.ItemArray.Length > 0)
			{
				// Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
				// parameter cache에서 stored procedure 해당 변수값을 끌어온다. (혹은 발견하여 cache에 저장)
				SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);

				// Set the parameters values
				// 변수값 지정
				AssignParameterValues(commandParameters, dataRow);

				return _SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName, commandParameters);
			}
			else
			{
				return _SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName);
			}
		}

		/// <summary>
		/// Execute a stored procedure via a SqlCommand (that returns no resultset) against the specified SqlConnection 
		/// using the dataRow column values as the stored procedure's parameters values.  
		/// This method will query the database to discover the parameters for the 
		/// stored procedure (the first time each stored procedure is called), and assign the values based on row values.
		///
		/// dataRow 열의 값들을 stored procedure 변수값으로 사용하여
		/// 지정된 SqlConnection에 대해 stored procedure를 (resultset 반환하지 않는) SqlCommand을 통해 실행시킴
		/// 이 메소드는 데이타베이스를 조회하여 stored procedure의 변수값을 찾아내고(각 stored procedure 처음 호출시),
		/// 열의 순서에 따라 값을 대입한다.
		/// </summary>
		/// <param name="connection">A valid SqlConnection object</param>
		/// <param name="spName">The name of the stored procedure</param>
		/// <param name="dataRow">The dataRow used to hold the stored procedure's parameter values.</param>
		/// <returns>An int representing the number of rows affected by the command</returns>
		public static int ExecuteNonQueryTypedParams(SqlConnection connection, String spName, DataRow dataRow)
		{
			if (connection == null) throw new ArgumentNullException("connection");
			if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

			// If the row has values, the store procedure parameters must be initialized
			// 열이 값을 포함하고 있으면, store procedure 변수 초기화
			if (dataRow != null && dataRow.ItemArray.Length > 0)
			{
				// Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
				// parameter cache에서 stored procedure 해당 변수값을 끌어온다. (혹은 발견하여 cache에 저장)
				SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connection, spName);

				// Set the parameters values
				// 변수값 지정
				AssignParameterValues(commandParameters, dataRow);

				return _SqlHelper.ExecuteNonQuery(connection, CommandType.StoredProcedure, spName, commandParameters);
			}
			else
			{
				return _SqlHelper.ExecuteNonQuery(connection, CommandType.StoredProcedure, spName);
			}
		}

		/// <summary>
		/// Execute a stored procedure via a SqlCommand (that returns no resultset) against the specified
		/// SqlTransaction using the dataRow column values as the stored procedure's parameters values.
		/// This method will query the database to discover the parameters for the 
		/// stored procedure (the first time each stored procedure is called), and assign the values based on row values.
		///
		/// dataRow 열의 값들을 stored procedure 변수값으로 사용하여
		/// 지정된 SqlTransaction에 대해 stored procedure를 (resultset 반환하지 않는) SqlCommand을 통해 실행시킴
		/// 이 메소드는 데이타베이스를 조회하여 stored procedure의 변수값을 찾아내고(각 stored procedure 처음 호출시),
		/// 열의 순서에 따라 값을 대입한다.
		/// </summary>
		/// <param name="transaction">A valid SqlTransaction object</param>
		/// <param name="spName">The name of the stored procedure</param>
		/// <param name="dataRow">The dataRow used to hold the stored procedure's parameter values.</param>
		/// <returns>An int representing the number of rows affected by the command</returns>
		public static int ExecuteNonQueryTypedParams(SqlTransaction transaction, String spName, DataRow dataRow)
		{
			if (transaction == null) throw new ArgumentNullException("transaction");
			if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
			if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

			// If the row has values, the store procedure parameters must be initialized
			// 열이 값을 포함하고 있으면, store procedure 변수 초기화
			if (dataRow != null && dataRow.ItemArray.Length > 0)
			{
				// Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
				// parameter cache에서 stored procedure 해당 변수값을 끌어온다. (혹은 발견하여 cache에 저장)
				SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);

				// Set the parameters values
				// 변수값 지정
				AssignParameterValues(commandParameters, dataRow);

				return _SqlHelper.ExecuteNonQuery(transaction, CommandType.StoredProcedure, spName, commandParameters);
			}
			else
			{
				return _SqlHelper.ExecuteNonQuery(transaction, CommandType.StoredProcedure, spName);
			}
		}
		#endregion

		#region ExecuteDatasetTypedParams
		/// <summary>
		/// Execute a stored procedure via a SqlCommand (that returns a resultset) against the database specified in 
		/// the connection string using the dataRow column values as the stored procedure's parameters values.
		/// This method will query the database to discover the parameters for the 
		/// stored procedure (the first time each stored procedure is called), and assign the values based on row values.
		///
		/// dataRow 열의 값들을 stored procedure 변수값으로 사용하여
		/// 커넥션 문자열에 지정된 데이타베이스의 stored procedure를 (resultset를 반환하는) SqlCommand을 통해 실행시킴
		/// 이 메소드는 데이타베이스를 조회하여 stored procedure의 변수값을 찾아내고(각 stored procedure 처음 호출시),
		/// 열의 순서에 따라 값을 대입한다.
		/// </summary>
		/// <param name="connectionString">A valid connection string for a SqlConnection</param>
		/// <param name="spName">The name of the stored procedure</param>
		/// <param name="dataRow">The dataRow used to hold the stored procedure's parameter values.</param>
		/// <returns>A dataset containing the resultset generated by the command</returns>
		public static DataSet ExecuteDatasetTypedParams(string connectionString, String spName, DataRow dataRow)
		{
			if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
			if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

			//If the row has values, the store procedure parameters must be initialized
			// 열이 값을 포함하고 있으면, store procedure 변수 초기화
			if (dataRow != null && dataRow.ItemArray.Length > 0)
			{
				// Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
				// parameter cache에서 stored procedure 해당 변수값을 끌어온다. (혹은 발견하여 cache에 저장)
				SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);

				// Set the parameters values
				// 변수값 지정
				AssignParameterValues(commandParameters, dataRow);

				return _SqlHelper.ExecuteDataset(connectionString, CommandType.StoredProcedure, spName, commandParameters);
			}
			else
			{
				return _SqlHelper.ExecuteDataset(connectionString, CommandType.StoredProcedure, spName);
			}
		}

		/// <summary>
		/// Execute a stored procedure via a SqlCommand (that returns a resultset) against the specified SqlConnection 
		/// using the dataRow column values as the store procedure's parameters values.
		/// This method will query the database to discover the parameters for the 
		/// stored procedure (the first time each stored procedure is called), and assign the values based on row values.
		/// 
		/// stored procedure의 변수값으로 dataRow 열을 이용하여
		/// 지정된 SqlConnection의 (resultset를 반환하는) stored procedure를 SqlCommand을 통해 실행시킴.
		/// 이 메소드는 데이타베이스를 조회하여 stored procedure의 변수값을 찾아내고(각 stored procedure 처음 호출시),
		/// 열의 순서에 따라 값을 대입한다.
		/// </summary>
		/// <param name="connection">A valid SqlConnection object</param>
		/// <param name="spName">The name of the stored procedure</param>
		/// <param name="dataRow">The dataRow used to hold the stored procedure's parameter values.</param>
		/// <returns>A dataset containing the resultset generated by the command</returns>
		public static DataSet ExecuteDatasetTypedParams(SqlConnection connection, String spName, DataRow dataRow)
		{
			if (connection == null) throw new ArgumentNullException("connection");
			if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

			// If the row has values, the store procedure parameters must be initialized
			// 열이 값을 포함하고 있으면, store procedure 변수 초기화
			if (dataRow != null && dataRow.ItemArray.Length > 0)
			{
				// Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
				// parameter cache에서 stored procedure 해당 변수값을 끌어온다. (혹은 발견하여 cache에 저장)
				SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connection, spName);

				// Set the parameters values
				// 변수값 지정
				AssignParameterValues(commandParameters, dataRow);

				return _SqlHelper.ExecuteDataset(connection, CommandType.StoredProcedure, spName, commandParameters);
			}
			else
			{
				return _SqlHelper.ExecuteDataset(connection, CommandType.StoredProcedure, spName);
			}
		}

		/// <summary>
		/// Execute a stored procedure via a SqlCommand (that returns a resultset) against the specified SqlTransaction 
		/// using the dataRow column values as the stored procedure's parameters values.
		/// This method will query the database to discover the parameters for the 
		/// stored procedure (the first time each stored procedure is called), and assign the values based on row values.
		///
		/// stored procedure의 변수값으로 dataRow 열을 이용하여
		/// 지정된 SqlTransaction의 (resultset를 반환하는) stored procedure를 SqlCommand을 통해 실행시킴.
		/// 이 메소드는 데이타베이스를 조회하여 stored procedure의 변수값을 찾아내고(각 stored procedure 처음 호출시),
		/// 열의 순서에 따라 값을 대입한다.
		/// </summary>
		/// <param name="transaction">A valid SqlTransaction object</param>
		/// <param name="spName">The name of the stored procedure</param>
		/// <param name="dataRow">The dataRow used to hold the stored procedure's parameter values.</param>
		/// <returns>A dataset containing the resultset generated by the command</returns>
		public static DataSet ExecuteDatasetTypedParams(SqlTransaction transaction, String spName, DataRow dataRow)
		{
			if (transaction == null) throw new ArgumentNullException("transaction");
			if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
			if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

			// If the row has values, the store procedure parameters must be initialized
			// 열이 값을 포함하고 있으면, store procedure 변수 초기화
			if (dataRow != null && dataRow.ItemArray.Length > 0)
			{
				// Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
				// parameter cache에서 stored procedure 해당 변수값을 끌어온다. (혹은 발견하여 cache에 저장)
				SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);

				// Set the parameters values
				// 변수값 지정
				AssignParameterValues(commandParameters, dataRow);

				return _SqlHelper.ExecuteDataset(transaction, CommandType.StoredProcedure, spName, commandParameters);
			}
			else
			{
				return _SqlHelper.ExecuteDataset(transaction, CommandType.StoredProcedure, spName);
			}
		}

		#endregion

		#region ExecuteReaderTypedParams
		/// <summary>
		/// Execute a stored procedure via a SqlCommand (that returns a resultset) against the database specified in 
		/// the connection string using the dataRow column values as the stored procedure's parameters values.
		/// This method will query the database to discover the parameters for the 
		/// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
		///
		/// dataRow 열의 값들을 stored procedure 변수값으로 사용하여
		/// 커넥션 문자열에 지정된 데이타베이스의 stored procedure를 (resultset를 반환하는) SqlCommand을 통해 실행시킴
		/// 이 메소드는 데이타베이스를 조회하여 stored procedure의 변수값을 찾아내고(각 stored procedure 처음 호출시),
		/// 열의 순서에 따라 값을 대입한다.
		/// </summary>
		/// <param name="connectionString">A valid connection string for a SqlConnection</param>
		/// <param name="spName">The name of the stored procedure</param>
		/// <param name="dataRow">The dataRow used to hold the stored procedure's parameter values.</param>
		/// <returns>A SqlDataReader containing the resultset generated by the command</returns>
		public static SqlDataReader ExecuteReaderTypedParams(String connectionString, String spName, DataRow dataRow)
		{
			if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
			if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

			// If the row has values, the store procedure parameters must be initialized
			// 열이 값을 포함하고 있으면, store procedure 변수 초기화
			if (dataRow != null && dataRow.ItemArray.Length > 0)
			{
				// Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
				// parameter cache에서 stored procedure 해당 변수값을 끌어온다. (혹은 발견하여 cache에 저장)
				SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);

				// Set the parameters values
				// 변수값 지정
				AssignParameterValues(commandParameters, dataRow);

				return _SqlHelper.ExecuteReader(connectionString, CommandType.StoredProcedure, spName, commandParameters);
			}
			else
			{
				return _SqlHelper.ExecuteReader(connectionString, CommandType.StoredProcedure, spName);
			}
		}


		/// <summary>
		/// Execute a stored procedure via a SqlCommand (that returns a resultset) against the specified SqlConnection 
		/// using the dataRow column values as the stored procedure's parameters values.
		/// This method will query the database to discover the parameters for the 
		/// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
		///
		/// stored procedure의 변수값으로 dataRow 열을 사용하여
		/// 지정된 SqlConnection의 (resultset를 반환하는) stored procedure를 SqlCommand을 통해 실행시킴.
		/// 이 메소드는 데이타베이스를 조회하여 stored procedure의 변수값을 찾아내고(각 stored procedure 처음 호출시),
		/// 변수의 순서에 따라 값을 대입한다.
		/// </summary>
		/// <param name="connection">A valid SqlConnection object</param>
		/// <param name="spName">The name of the stored procedure</param>
		/// <param name="dataRow">The dataRow used to hold the stored procedure's parameter values.</param>
		/// <returns>A SqlDataReader containing the resultset generated by the command</returns>
		public static SqlDataReader ExecuteReaderTypedParams(SqlConnection connection, String spName, DataRow dataRow)
		{
			if (connection == null) throw new ArgumentNullException("connection");
			if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

			// If the row has values, the store procedure parameters must be initialized
			// 열이 값을 포함하고 있으면, store procedure 변수 초기화
			if (dataRow != null && dataRow.ItemArray.Length > 0)
			{
				// Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
				// parameter cache에서 stored procedure 해당 변수값을 끌어온다. (혹은 발견하여 cache에 저장)
				SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connection, spName);

				// Set the parameters values
				// 변수값 지정
				AssignParameterValues(commandParameters, dataRow);

				return _SqlHelper.ExecuteReader(connection, CommandType.StoredProcedure, spName, commandParameters);
			}
			else
			{
				return _SqlHelper.ExecuteReader(connection, CommandType.StoredProcedure, spName);
			}
		}

		/// <summary>
		/// Execute a stored procedure via a SqlCommand (that returns a resultset) against the specified SqlTransaction 
		/// using the dataRow column values as the stored procedure's parameters values.
		/// This method will query the database to discover the parameters for the 
		/// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
		///
		/// stored procedure의 변수값으로 dataRow 열을 사용하여
		/// 지정된 SqlTransaction의 (resultset를 반환하는) stored procedure를 SqlCommand을 통해 실행시킴.
		/// 이 메소드는 데이타베이스를 조회하여 stored procedure의 변수값을 찾아내고(각 stored procedure 처음 호출시),
		/// 변수의 순서에 따라 값을 대입한다.
		/// </summary>
		/// <param name="transaction">A valid SqlTransaction object</param>
		/// <param name="spName">The name of the stored procedure</param>
		/// <param name="dataRow">The dataRow used to hold the stored procedure's parameter values.</param>
		/// <returns>A SqlDataReader containing the resultset generated by the command</returns>
		public static SqlDataReader ExecuteReaderTypedParams(SqlTransaction transaction, String spName, DataRow dataRow)
		{
			if (transaction == null) throw new ArgumentNullException("transaction");
			if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
			if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

			// If the row has values, the store procedure parameters must be initialized
			// 열이 값을 포함하고 있으면, store procedure 변수 초기화
			if (dataRow != null && dataRow.ItemArray.Length > 0)
			{
				// Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
				// parameter cache에서 stored procedure 해당 변수값을 끌어온다. (혹은 발견하여 cache에 저장)
				SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);

				// Set the parameters values
				// 변수값 지정
				AssignParameterValues(commandParameters, dataRow);

				return _SqlHelper.ExecuteReader(transaction, CommandType.StoredProcedure, spName, commandParameters);
			}
			else
			{
				return _SqlHelper.ExecuteReader(transaction, CommandType.StoredProcedure, spName);
			}
		}
		#endregion

		#region ExecuteScalarTypedParams
		/// <summary>
		/// Execute a stored procedure via a SqlCommand (that returns a 1x1 resultset) against the database specified in 
		/// the connection string using the dataRow column values as the stored procedure's parameters values.
		/// This method will query the database to discover the parameters for the 
		/// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
		///
		/// stored procedure의 변수값으로 dataRow 열값을 이용하여
		/// 커넥션 문자열에 지정된 데이타베이스의 stored procedure를 (1x1 resultset을 반환하는) SqlCommand을 통해 실행시킴
		/// 이 메소드는 데이타베이스를 조회하여 stored procedure의 변수값을 찾아내고(각 stored procedure 처음 호출시),
		/// 변수의 순서에 따라 값을 대입한다.
		/// </summary>
		/// <param name="connectionString">A valid connection string for a SqlConnection</param>
		/// <param name="spName">The name of the stored procedure</param>
		/// <param name="dataRow">The dataRow used to hold the stored procedure's parameter values.</param>
		/// <returns>An object containing the value in the 1x1 resultset generated by the command</returns>
		public static object ExecuteScalarTypedParams(String connectionString, String spName, DataRow dataRow)
		{
			if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
			if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

			// If the row has values, the store procedure parameters must be initialized
			// 열이 값을 포함하고 있으면, store procedure 변수 초기화
			if (dataRow != null && dataRow.ItemArray.Length > 0)
			{
				// Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
				// parameter cache에서 stored procedure 해당 변수값을 끌어온다. (혹은 발견하여 cache에 저장)
				SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);

				// Set the parameters values
				// 변수값 지정
				AssignParameterValues(commandParameters, dataRow);

				return _SqlHelper.ExecuteScalar(connectionString, CommandType.StoredProcedure, spName, commandParameters);
			}
			else
			{
				return _SqlHelper.ExecuteScalar(connectionString, CommandType.StoredProcedure, spName);
			}
		}

		/// <summary>
		/// Execute a stored procedure via a SqlCommand (that returns a 1x1 resultset) against the specified SqlConnection 
		/// using the dataRow column values as the stored procedure's parameters values.
		/// This method will query the database to discover the parameters for the 
		/// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
		///
		/// stored procedure의 변수값으로 dataRow 열값을 이용하여
		/// 지정된 SqlConnection에 대한 stored procedure를 (1x1 resultset을 반환하는) SqlCommand을 통해 실행시킴
		/// 이 메소드는 데이타베이스를 조회하여 stored procedure의 변수값을 찾아내고(각 stored procedure 처음 호출시),
		/// 변수의 순서에 따라 값을 대입한다.
		/// </summary>
		/// <param name="connection">A valid SqlConnection object</param>
		/// <param name="spName">The name of the stored procedure</param>
		/// <param name="dataRow">The dataRow used to hold the stored procedure's parameter values.</param>
		/// <returns>An object containing the value in the 1x1 resultset generated by the command</returns>
		public static object ExecuteScalarTypedParams(SqlConnection connection, String spName, DataRow dataRow)
		{
			if (connection == null) throw new ArgumentNullException("connection");
			if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

			// If the row has values, the store procedure parameters must be initialized
			// 열이 값을 포함하고 있으면, store procedure 변수 초기화
			if (dataRow != null && dataRow.ItemArray.Length > 0)
			{
				// Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
				// parameter cache에서 stored procedure 해당 변수값을 끌어온다. (혹은 발견하여 cache에 저장)
				SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connection, spName);

				// Set the parameters values
				// 변수값 지정
				AssignParameterValues(commandParameters, dataRow);

				return _SqlHelper.ExecuteScalar(connection, CommandType.StoredProcedure, spName, commandParameters);
			}
			else
			{
				return _SqlHelper.ExecuteScalar(connection, CommandType.StoredProcedure, spName);
			}
		}

		/// <summary>
		/// Execute a stored procedure via a SqlCommand (that returns a 1x1 resultset) against the specified SqlTransaction
		/// using the dataRow column values as the stored procedure's parameters values.
		/// This method will query the database to discover the parameters for the 
		/// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
		///
		/// stored procedure의 변수값으로 dataRow 열값을 이용하여
		/// 지정된 SqlTransaction에 대한 stored procedure를 (1x1 resultset을 반환하는) SqlCommand을 통해 실행시킴
		/// 이 메소드는 데이타베이스를 조회하여 stored procedure의 변수값을 찾아내고(각 stored procedure 처음 호출시),
		/// 변수의 순서에 따라 값을 대입한다.
		/// </summary>
		/// <param name="transaction">A valid SqlTransaction object</param>
		/// <param name="spName">The name of the stored procedure</param>
		/// <param name="dataRow">The dataRow used to hold the stored procedure's parameter values.</param>
		/// <returns>An object containing the value in the 1x1 resultset generated by the command</returns>
		public static object ExecuteScalarTypedParams(SqlTransaction transaction, String spName, DataRow dataRow)
		{
			if (transaction == null) throw new ArgumentNullException("transaction");
			if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
			if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

			// If the row has values, the store procedure parameters must be initialized
			// 열이 값을 포함하고 있으면, store procedure 변수 초기화
			if (dataRow != null && dataRow.ItemArray.Length > 0)
			{
				// Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
				// parameter cache에서 stored procedure 해당 변수값을 끌어온다. (혹은 발견하여 cache에 저장)
				SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);

				// Set the parameters values
				// 변수값 지정
				AssignParameterValues(commandParameters, dataRow);

				return _SqlHelper.ExecuteScalar(transaction, CommandType.StoredProcedure, spName, commandParameters);
			}
			else
			{
				return _SqlHelper.ExecuteScalar(transaction, CommandType.StoredProcedure, spName);
			}
		}
		#endregion

		#region ExecuteXmlReaderTypedParams
		/// <summary>
		/// Execute a stored procedure via a SqlCommand (that returns a resultset) against the specified SqlConnection 
		/// using the dataRow column values as the stored procedure's parameters values.
		/// This method will query the database to discover the parameters for the 
		/// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
		///
		/// stored procedure의 변수값으로 dataRow 열을 사용하여
		/// 지정된 SqlConnection의 stored procedure를 (resultset를 반환하는) SqlCommand을 통해 실행시킴.
		/// 이 메소드는 데이타베이스를 조회하여 stored procedure의 변수값을 찾아내고(각 stored procedure 처음 호출시),
		/// 변수의 순서에 따라 값을 대입한다.
		/// </summary>
		/// <param name="connection">A valid SqlConnection object</param>
		/// <param name="spName">The name of the stored procedure</param>
		/// <param name="dataRow">The dataRow used to hold the stored procedure's parameter values.</param>
		/// <returns>An XmlReader containing the resultset generated by the command</returns>
		public static XmlReader ExecuteXmlReaderTypedParams(SqlConnection connection, String spName, DataRow dataRow)
		{
			if (connection == null) throw new ArgumentNullException("connection");
			if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

			// If the row has values, the store procedure parameters must be initialized
			// 열이 값을 포함하고 있으면, store procedure 변수 초기화
			if (dataRow != null && dataRow.ItemArray.Length > 0)
			{
				// Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
				// parameter cache에서 stored procedure 해당 변수값을 끌어온다. (혹은 발견하여 cache에 저장)
				SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connection, spName);

				// Set the parameters values
				// 변수값 지정
				AssignParameterValues(commandParameters, dataRow);

				return _SqlHelper.ExecuteXmlReader(connection, CommandType.StoredProcedure, spName, commandParameters);
			}
			else
			{
				return _SqlHelper.ExecuteXmlReader(connection, CommandType.StoredProcedure, spName);
			}
		}

		/// <summary>
		/// Execute a stored procedure via a SqlCommand (that returns a resultset) against the specified SqlTransaction 
		/// using the dataRow column values as the stored procedure's parameters values.
		/// This method will query the database to discover the parameters for the 
		/// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
		///
		/// stored procedure의 변수값으로 dataRow 열을 사용하여
		/// 지정된 SqlTransaction의 stored procedure를 (resultset를 반환하는) SqlCommand을 통해 실행시킴.
		/// 이 메소드는 데이타베이스를 조회하여 stored procedure의 변수값을 찾아내고(각 stored procedure 처음 호출시),
		/// 변수의 순서에 따라 값을 대입한다.
		/// </summary>
		/// <param name="transaction">A valid SqlTransaction object</param>
		/// <param name="spName">The name of the stored procedure</param>
		/// <param name="dataRow">The dataRow used to hold the stored procedure's parameter values.</param>
		/// <returns>An XmlReader containing the resultset generated by the command</returns>
		public static XmlReader ExecuteXmlReaderTypedParams(SqlTransaction transaction, String spName, DataRow dataRow)
		{
			if (transaction == null) throw new ArgumentNullException("transaction");
			if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
			if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

			// If the row has values, the store procedure parameters must be initialized
			// 열이 값을 포함하고 있으면, store procedure 변수 초기화
			if (dataRow != null && dataRow.ItemArray.Length > 0)
			{
				// Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
				// parameter cache에서 stored procedure 해당 변수값을 끌어온다. (혹은 발견하여 cache에 저장)
				SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);

				// Set the parameters values
				// 변수값 지정
				AssignParameterValues(commandParameters, dataRow);

				return _SqlHelper.ExecuteXmlReader(transaction, CommandType.StoredProcedure, spName, commandParameters);
			}
			else
			{
				return _SqlHelper.ExecuteXmlReader(transaction, CommandType.StoredProcedure, spName);
			}
		}
		#endregion

		#region CacheParameters
		/// <summary>
		/// add parameter array to the cache
		/// </summary>
		/// <param name="cacheKey">Key to the parameter cache</param>
		/// <param name="cmdParms">an array of SqlParamters to be cached</param>
		public static void CacheParameters(string cacheKey, params SqlParameter[] cmdParms)
		{
			parmCache[cacheKey] = cmdParms;
		}

		/// <summary>
		/// Retrieve cached parameters
		/// </summary>
		/// <param name="cacheKey">key used to lookup parameters</param>
		/// <returns>Cached SqlParamters array</returns>
		public static SqlParameter[] GetCachedParameters(string cacheKey)
		{
			SqlParameter[] cachedParms = (SqlParameter[])parmCache[cacheKey];

			if (cachedParms == null)
				return null;

			SqlParameter[] clonedParms = new SqlParameter[cachedParms.Length];

			for (int i = 0, j = cachedParms.Length; i < j; i++)
				clonedParms[i] = (SqlParameter)((ICloneable)cachedParms[i]).Clone();

			return clonedParms;
		}

		#endregion       

	}

	/// <summary>
	/// SqlHelperParameterCache provides functions to leverage a static cache of procedure parameters, and the ability to discover parameters for stored procedures at run-time.
	/// SqlHelperParameterCache는 procedure 변수의 static cache 함수를 제공한다. 또한 런타임시 stored procedure의 변수들을 찾아낸다.
	/// </summary>
	public sealed class SqlHelperParameterCache
	{
		#region private methods, variables, and constructors

		//Since this class provides only static methods, make the default constructor private to prevent 
		//instances from being created with "new SqlHelperParameterCache()"
		// 이 클래스는 static 메소드만 제공하기 때문에, 디폴트 생성자를 private하여 "new SqlHelperParameterCache()"의 새로운 instance 생성 방지
		private SqlHelperParameterCache() { }

		private static Hashtable paramCache = Hashtable.Synchronized(new Hashtable());

		/// <summary>
		/// Resolve at run time the appropriate set of SqlParameters for a stored procedure
		/// stored procedure를 위한 적절한 SqlParameter 세트를 런타임시 해결
		/// </summary>
		/// <param name="connection">A valid SqlConnection object</param>
		/// <param name="spName">The name of the stored procedure</param>
		/// <param name="includeReturnValueParameter">Whether or not to include their return value parameter</param>
		/// <returns>The parameter array discovered.</returns>
		private static SqlParameter[] DiscoverSpParameterSet(SqlConnection connection, string spName, bool includeReturnValueParameter)
		{
			if (connection == null) throw new ArgumentNullException("connection");
			if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

			SqlCommand cmd = new SqlCommand(spName, connection);
			cmd.CommandType = CommandType.StoredProcedure;

			connection.Open();

			SqlCommandBuilder.DeriveParameters(cmd);

			connection.Close();

			if (!includeReturnValueParameter)
			{
				cmd.Parameters.RemoveAt(0);
			}

			SqlParameter[] discoveredParameters = new SqlParameter[cmd.Parameters.Count];

			cmd.Parameters.CopyTo(discoveredParameters, 0);

			// Init the parameters with a DBNull value
			// DBNull 값인 변수들 초기화
			foreach (SqlParameter discoveredParameter in discoveredParameters)
			{
				discoveredParameter.Value = DBNull.Value;
			}
			return discoveredParameters;
		}

		/// <summary>
		/// Deep copy of cached SqlParameter array
		/// 캐쉬된 SqlParameter 배열의 사본
		/// </summary>
		/// <param name="originalParameters"></param>
		/// <returns></returns>
		private static SqlParameter[] CloneParameters(SqlParameter[] originalParameters)
		{
			SqlParameter[] clonedParameters = new SqlParameter[originalParameters.Length];

			for (int i = 0, j = originalParameters.Length; i < j; i++)
			{
				clonedParameters[i] = (SqlParameter)((ICloneable)originalParameters[i]).Clone();
			}

			return clonedParameters;
		}

		#endregion private methods, variables, and constructors

		#region caching functions

		/// <summary>
		/// Add parameter array to the cache
		/// 변수배열 캐쉬에 추가
		/// </summary>
		/// <param name="connectionString">A valid connection string for a SqlConnection</param>
		/// <param name="commandText">The stored procedure name or T-SQL command</param>
		/// <param name="commandParameters">An array of SqlParamters to be cached</param>
		public static void CacheParameterSet(string connectionString, string commandText, params SqlParameter[] commandParameters)
		{
			if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
			if (commandText == null || commandText.Length == 0) throw new ArgumentNullException("commandText");

			string hashKey = connectionString + ":" + commandText;

			paramCache[hashKey] = commandParameters;
		}

		/// <summary>
		/// Retrieve a parameter array from the cache
		/// 변수배열 캐쉬에서 회수
		/// </summary>
		/// <param name="connectionString">A valid connection string for a SqlConnection</param>
		/// <param name="commandText">The stored procedure name or T-SQL command</param>
		/// <returns>An array of SqlParamters</returns>
		public static SqlParameter[] GetCachedParameterSet(string connectionString, string commandText)
		{
			if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
			if (commandText == null || commandText.Length == 0) throw new ArgumentNullException("commandText");

			string hashKey = connectionString + ":" + commandText;

			SqlParameter[] cachedParameters = paramCache[hashKey] as SqlParameter[];
			if (cachedParameters == null)
			{
				return null;
			}
			else
			{
				return CloneParameters(cachedParameters);
			}
		}

		#endregion caching functions

		#region Parameter Discovery Functions

		/// <summary>
		/// Retrieves the set of SqlParameters appropriate for the stored procedure
		/// stored procedure에 적절한 SqlParameter 세트 회수
		/// </summary>
		/// <remarks>
		/// This method will query the database for this information, and then store it in a cache for future requests.
		/// 이 메소드는 데이타베이스를 쿼리 한 후, 앞으로를 위해 캐쉬에 저장함
		/// </remarks>
		/// <param name="connectionString">A valid connection string for a SqlConnection</param>
		/// <param name="spName">The name of the stored procedure</param>
		/// <returns>An array of SqlParameters</returns>
		public static SqlParameter[] GetSpParameterSet(string connectionString, string spName)
		{
			return GetSpParameterSet(connectionString, spName, false);
		}

		/// <summary>
		/// Retrieves the set of SqlParameters appropriate for the stored procedure
		/// stored procedure에 적절한 SqlParameter 세트 회수
		/// </summary>
		/// <remarks>
		/// This method will query the database for this information, and then store it in a cache for future requests.
		/// 이 메소드는 데이타베이스를 쿼리 한 후, 앞으로를 위해 캐쉬에 저장함
		/// </remarks>
		/// <param name="connectionString">A valid connection string for a SqlConnection</param>
		/// <param name="spName">The name of the stored procedure</param>
		/// <param name="includeReturnValueParameter">A bool value indicating whether the return value parameter should be included in the results</param>
		/// <returns>An array of SqlParameters</returns>
		public static SqlParameter[] GetSpParameterSet(string connectionString, string spName, bool includeReturnValueParameter)
		{
			if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
			if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				return GetSpParameterSetInternal(connection, spName, includeReturnValueParameter);
			}
		}

		/// <summary>
		/// Retrieves the set of SqlParameters appropriate for the stored procedure
		/// stored procedure에 적절한 SqlParameter 세트 회수
		/// </summary>
		/// <remarks>
		/// This method will query the database for this information, and then store it in a cache for future requests.
		/// 이 메소드는 데이타베이스를 쿼리 한 후, 앞으로를 위해 캐쉬에 저장함
		/// </remarks>
		/// <param name="connection">A valid SqlConnection object</param>
		/// <param name="spName">The name of the stored procedure</param>
		/// <returns>An array of SqlParameters</returns>
		internal static SqlParameter[] GetSpParameterSet(SqlConnection connection, string spName)
		{
			return GetSpParameterSet(connection, spName, false);
		}

		/// <summary>
		/// Retrieves the set of SqlParameters appropriate for the stored procedure
		/// stored procedure에 적절한 SqlParameter 세트 회수
		/// </summary>
		/// <remarks>
		/// This method will query the database for this information, and then store it in a cache for future requests.
		/// 이 메소드는 데이타베이스를 쿼리 한 후, 앞으로를 위해 캐쉬에 저장함
		/// </remarks>
		/// <param name="connection">A valid SqlConnection object</param>
		/// <param name="spName">The name of the stored procedure</param>
		/// <param name="includeReturnValueParameter">A bool value indicating whether the return value parameter should be included in the results</param>
		/// <param name="includeReturnValueParameter">리턴값 변수가 결과에 include 됐는지 보여주는 bool 값</param>
		/// <returns>An array of SqlParameters</returns>
		internal static SqlParameter[] GetSpParameterSet(SqlConnection connection, string spName, bool includeReturnValueParameter)
		{
			if (connection == null) throw new ArgumentNullException("connection");
			using (SqlConnection clonedConnection = (SqlConnection)((ICloneable)connection).Clone())
			{
				return GetSpParameterSetInternal(clonedConnection, spName, includeReturnValueParameter);
			}
		}

		/// <summary>
		/// Retrieves the set of SqlParameters appropriate for the stored procedure
		/// stored procedure에 적절한 SqlParameter 세트 회수
		/// </summary>
		/// <param name="connection">A valid SqlConnection object</param>
		/// <param name="spName">The name of the stored procedure</param>
		/// <param name="includeReturnValueParameter">A bool value indicating whether the return value parameter should be included in the results</param>
		/// <returns>An array of SqlParameters</returns>
		private static SqlParameter[] GetSpParameterSetInternal(SqlConnection connection, string spName, bool includeReturnValueParameter)
		{
			if (connection == null) throw new ArgumentNullException("connection");
			if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

			string hashKey = connection.ConnectionString + ":" + spName + (includeReturnValueParameter ? ":include ReturnValue Parameter" : "");

			SqlParameter[] cachedParameters;

			cachedParameters = paramCache[hashKey] as SqlParameter[];
			if (cachedParameters == null)
			{
				SqlParameter[] spParameters = DiscoverSpParameterSet(connection, spName, includeReturnValueParameter);
				paramCache[hashKey] = spParameters;
				cachedParameters = spParameters;
			}

			return CloneParameters(cachedParameters);
		}

		#endregion Parameter Discovery Functions
	}
}