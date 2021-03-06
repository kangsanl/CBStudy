﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18444
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CBStudy.LINQ
{
	using System.Data.Linq;
	using System.Data.Linq.Mapping;
	using System.Data;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Linq;
	using System.Linq.Expressions;
	using System.ComponentModel;
	using System;
	
	
	[global::System.Data.Linq.Mapping.DatabaseAttribute(Name="PeriodsBuying")]
	public partial class clsPeriodsBuyingDataContext : System.Data.Linq.DataContext
	{
		
		private static System.Data.Linq.Mapping.MappingSource mappingSource = new AttributeMappingSource();
		
    #region Extensibility Method Definitions
    partial void OnCreated();
    partial void InsertTBInfor2(TBInfor2 instance);
    partial void UpdateTBInfor2(TBInfor2 instance);
    partial void DeleteTBInfor2(TBInfor2 instance);
    partial void InsertTBSimulation2old(TBSimulation2old instance);
    partial void UpdateTBSimulation2old(TBSimulation2old instance);
    partial void DeleteTBSimulation2old(TBSimulation2old instance);
    partial void InsertTBSimulation2(TBSimulation2 instance);
    partial void UpdateTBSimulation2(TBSimulation2 instance);
    partial void DeleteTBSimulation2(TBSimulation2 instance);
    #endregion
		
		public clsPeriodsBuyingDataContext() : 
				base(global::System.Configuration.ConfigurationManager.ConnectionStrings["PeriodsBuyingConnectionString"].ConnectionString, mappingSource)
		{
			OnCreated();
		}
		
		public clsPeriodsBuyingDataContext(string connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public clsPeriodsBuyingDataContext(System.Data.IDbConnection connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public clsPeriodsBuyingDataContext(string connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public clsPeriodsBuyingDataContext(System.Data.IDbConnection connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public System.Data.Linq.Table<TBInfor2> TBInfor2s
		{
			get
			{
				return this.GetTable<TBInfor2>();
			}
		}
		
		public System.Data.Linq.Table<TBSimulation2old> TBSimulation2olds
		{
			get
			{
				return this.GetTable<TBSimulation2old>();
			}
		}
		
		public System.Data.Linq.Table<TBInfor2_latest> TBInfor2_latests
		{
			get
			{
				return this.GetTable<TBInfor2_latest>();
			}
		}
		
		public System.Data.Linq.Table<TBSimulation2> TBSimulation2s
		{
			get
			{
				return this.GetTable<TBSimulation2>();
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.TBInfor2")]
	public partial class TBInfor2 : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _TURN;
		
		private System.Nullable<byte> _C_K;
		
		private System.Nullable<byte> _C_N;
		
		private System.Nullable<byte> _C_P1;
		
		private System.Nullable<byte> _C_P2;
		
		private System.Nullable<byte> _C_Y;
		
		private System.Nullable<byte> _C_V;
		
		private string _GROUPID;
		
		private System.Nullable<int> _TRIAL;
		
		private System.Nullable<int> _ISLAST;
		
		private System.Nullable<int> _Result;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnTURNChanging(int value);
    partial void OnTURNChanged();
    partial void OnC_KChanging(System.Nullable<byte> value);
    partial void OnC_KChanged();
    partial void OnC_NChanging(System.Nullable<byte> value);
    partial void OnC_NChanged();
    partial void OnC_P1Changing(System.Nullable<byte> value);
    partial void OnC_P1Changed();
    partial void OnC_P2Changing(System.Nullable<byte> value);
    partial void OnC_P2Changed();
    partial void OnC_YChanging(System.Nullable<byte> value);
    partial void OnC_YChanged();
    partial void OnC_VChanging(System.Nullable<byte> value);
    partial void OnC_VChanged();
    partial void OnGROUPIDChanging(string value);
    partial void OnGROUPIDChanged();
    partial void OnTRIALChanging(System.Nullable<int> value);
    partial void OnTRIALChanged();
    partial void OnISLASTChanging(System.Nullable<int> value);
    partial void OnISLASTChanged();
    partial void OnResultChanging(System.Nullable<int> value);
    partial void OnResultChanged();
    #endregion
		
		public TBInfor2()
		{
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_TURN", DbType="Int NOT NULL", IsPrimaryKey=true)]
		public int TURN
		{
			get
			{
				return this._TURN;
			}
			set
			{
				if ((this._TURN != value))
				{
					this.OnTURNChanging(value);
					this.SendPropertyChanging();
					this._TURN = value;
					this.SendPropertyChanged("TURN");
					this.OnTURNChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_C_K", DbType="TinyInt")]
		public System.Nullable<byte> C_K
		{
			get
			{
				return this._C_K;
			}
			set
			{
				if ((this._C_K != value))
				{
					this.OnC_KChanging(value);
					this.SendPropertyChanging();
					this._C_K = value;
					this.SendPropertyChanged("C_K");
					this.OnC_KChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_C_N", DbType="TinyInt")]
		public System.Nullable<byte> C_N
		{
			get
			{
				return this._C_N;
			}
			set
			{
				if ((this._C_N != value))
				{
					this.OnC_NChanging(value);
					this.SendPropertyChanging();
					this._C_N = value;
					this.SendPropertyChanged("C_N");
					this.OnC_NChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_C_P1", DbType="TinyInt")]
		public System.Nullable<byte> C_P1
		{
			get
			{
				return this._C_P1;
			}
			set
			{
				if ((this._C_P1 != value))
				{
					this.OnC_P1Changing(value);
					this.SendPropertyChanging();
					this._C_P1 = value;
					this.SendPropertyChanged("C_P1");
					this.OnC_P1Changed();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_C_P2", DbType="TinyInt")]
		public System.Nullable<byte> C_P2
		{
			get
			{
				return this._C_P2;
			}
			set
			{
				if ((this._C_P2 != value))
				{
					this.OnC_P2Changing(value);
					this.SendPropertyChanging();
					this._C_P2 = value;
					this.SendPropertyChanged("C_P2");
					this.OnC_P2Changed();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_C_Y", DbType="TinyInt")]
		public System.Nullable<byte> C_Y
		{
			get
			{
				return this._C_Y;
			}
			set
			{
				if ((this._C_Y != value))
				{
					this.OnC_YChanging(value);
					this.SendPropertyChanging();
					this._C_Y = value;
					this.SendPropertyChanged("C_Y");
					this.OnC_YChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_C_V", DbType="TinyInt")]
		public System.Nullable<byte> C_V
		{
			get
			{
				return this._C_V;
			}
			set
			{
				if ((this._C_V != value))
				{
					this.OnC_VChanging(value);
					this.SendPropertyChanging();
					this._C_V = value;
					this.SendPropertyChanged("C_V");
					this.OnC_VChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_GROUPID", DbType="VarChar(50)")]
		public string GROUPID
		{
			get
			{
				return this._GROUPID;
			}
			set
			{
				if ((this._GROUPID != value))
				{
					this.OnGROUPIDChanging(value);
					this.SendPropertyChanging();
					this._GROUPID = value;
					this.SendPropertyChanged("GROUPID");
					this.OnGROUPIDChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_TRIAL", DbType="Int")]
		public System.Nullable<int> TRIAL
		{
			get
			{
				return this._TRIAL;
			}
			set
			{
				if ((this._TRIAL != value))
				{
					this.OnTRIALChanging(value);
					this.SendPropertyChanging();
					this._TRIAL = value;
					this.SendPropertyChanged("TRIAL");
					this.OnTRIALChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ISLAST", DbType="Int")]
		public System.Nullable<int> ISLAST
		{
			get
			{
				return this._ISLAST;
			}
			set
			{
				if ((this._ISLAST != value))
				{
					this.OnISLASTChanging(value);
					this.SendPropertyChanging();
					this._ISLAST = value;
					this.SendPropertyChanged("ISLAST");
					this.OnISLASTChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Result", DbType="Int")]
		public System.Nullable<int> Result
		{
			get
			{
				return this._Result;
			}
			set
			{
				if ((this._Result != value))
				{
					this.OnResultChanging(value);
					this.SendPropertyChanging();
					this._Result = value;
					this.SendPropertyChanged("Result");
					this.OnResultChanged();
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.TBSimulation2")]
	public partial class TBSimulation2old : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _KEY;
		
		private int _TURN;
		
		private string _USERID;
		
		private string _DECISION;
		
		private string _RESULTYN;
		
		private System.Nullable<int> _RESULT;
		
		private string _GROUPID;
		
		private System.Nullable<int> _TRIAL;
		
		private System.Nullable<int> _CONFIDENCE;
		
		private string _TIME;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnKEYChanging(int value);
    partial void OnKEYChanged();
    partial void OnTURNChanging(int value);
    partial void OnTURNChanged();
    partial void OnUSERIDChanging(string value);
    partial void OnUSERIDChanged();
    partial void OnDECISIONChanging(string value);
    partial void OnDECISIONChanged();
    partial void OnRESULTYNChanging(string value);
    partial void OnRESULTYNChanged();
    partial void OnRESULTChanging(System.Nullable<int> value);
    partial void OnRESULTChanged();
    partial void OnGROUPIDChanging(string value);
    partial void OnGROUPIDChanged();
    partial void OnTRIALChanging(System.Nullable<int> value);
    partial void OnTRIALChanged();
    partial void OnCONFIDENCEChanging(System.Nullable<int> value);
    partial void OnCONFIDENCEChanged();
    partial void OnTIMEChanging(string value);
    partial void OnTIMEChanged();
    #endregion
		
		public TBSimulation2old()
		{
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Name="[KEY]", Storage="_KEY", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int KEY
		{
			get
			{
				return this._KEY;
			}
			set
			{
				if ((this._KEY != value))
				{
					this.OnKEYChanging(value);
					this.SendPropertyChanging();
					this._KEY = value;
					this.SendPropertyChanged("KEY");
					this.OnKEYChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_TURN", DbType="Int NOT NULL")]
		public int TURN
		{
			get
			{
				return this._TURN;
			}
			set
			{
				if ((this._TURN != value))
				{
					this.OnTURNChanging(value);
					this.SendPropertyChanging();
					this._TURN = value;
					this.SendPropertyChanged("TURN");
					this.OnTURNChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_USERID", DbType="VarChar(20)")]
		public string USERID
		{
			get
			{
				return this._USERID;
			}
			set
			{
				if ((this._USERID != value))
				{
					this.OnUSERIDChanging(value);
					this.SendPropertyChanging();
					this._USERID = value;
					this.SendPropertyChanged("USERID");
					this.OnUSERIDChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_DECISION", DbType="VarChar(1)")]
		public string DECISION
		{
			get
			{
				return this._DECISION;
			}
			set
			{
				if ((this._DECISION != value))
				{
					this.OnDECISIONChanging(value);
					this.SendPropertyChanging();
					this._DECISION = value;
					this.SendPropertyChanged("DECISION");
					this.OnDECISIONChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_RESULTYN", DbType="VarChar(1)")]
		public string RESULTYN
		{
			get
			{
				return this._RESULTYN;
			}
			set
			{
				if ((this._RESULTYN != value))
				{
					this.OnRESULTYNChanging(value);
					this.SendPropertyChanging();
					this._RESULTYN = value;
					this.SendPropertyChanged("RESULTYN");
					this.OnRESULTYNChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_RESULT", DbType="Int")]
		public System.Nullable<int> RESULT
		{
			get
			{
				return this._RESULT;
			}
			set
			{
				if ((this._RESULT != value))
				{
					this.OnRESULTChanging(value);
					this.SendPropertyChanging();
					this._RESULT = value;
					this.SendPropertyChanged("RESULT");
					this.OnRESULTChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_GROUPID", DbType="VarChar(50)")]
		public string GROUPID
		{
			get
			{
				return this._GROUPID;
			}
			set
			{
				if ((this._GROUPID != value))
				{
					this.OnGROUPIDChanging(value);
					this.SendPropertyChanging();
					this._GROUPID = value;
					this.SendPropertyChanged("GROUPID");
					this.OnGROUPIDChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_TRIAL", DbType="Int")]
		public System.Nullable<int> TRIAL
		{
			get
			{
				return this._TRIAL;
			}
			set
			{
				if ((this._TRIAL != value))
				{
					this.OnTRIALChanging(value);
					this.SendPropertyChanging();
					this._TRIAL = value;
					this.SendPropertyChanged("TRIAL");
					this.OnTRIALChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_CONFIDENCE", DbType="Int")]
		public System.Nullable<int> CONFIDENCE
		{
			get
			{
				return this._CONFIDENCE;
			}
			set
			{
				if ((this._CONFIDENCE != value))
				{
					this.OnCONFIDENCEChanging(value);
					this.SendPropertyChanging();
					this._CONFIDENCE = value;
					this.SendPropertyChanged("CONFIDENCE");
					this.OnCONFIDENCEChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_TIME", DbType="NVarChar(50)")]
		public string TIME
		{
			get
			{
				return this._TIME;
			}
			set
			{
				if ((this._TIME != value))
				{
					this.OnTIMEChanging(value);
					this.SendPropertyChanging();
					this._TIME = value;
					this.SendPropertyChanged("TIME");
					this.OnTIMEChanged();
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.TBInfor2_latest")]
	public partial class TBInfor2_latest
	{
		
		private int _TURN;
		
		private System.Nullable<int> _C_K;
		
		private System.Nullable<int> _C_N1;
		
		private System.Nullable<int> _C_N2;
		
		private System.Nullable<int> _C_P1;
		
		private System.Nullable<int> _C_P2;
		
		private System.Nullable<int> _C_Y;
		
		private System.Nullable<int> _C_V;
		
		private string _GROUPID;
		
		private System.Nullable<int> _TRIAL;
		
		private System.Nullable<int> _ISLAST;
		
		private System.Nullable<int> _Result;
		
		private System.Nullable<int> _EX_SET;
		
		public TBInfor2_latest()
		{
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_TURN", DbType="Int NOT NULL")]
		public int TURN
		{
			get
			{
				return this._TURN;
			}
			set
			{
				if ((this._TURN != value))
				{
					this._TURN = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_C_K", DbType="Int")]
		public System.Nullable<int> C_K
		{
			get
			{
				return this._C_K;
			}
			set
			{
				if ((this._C_K != value))
				{
					this._C_K = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_C_N1", DbType="Int")]
		public System.Nullable<int> C_N1
		{
			get
			{
				return this._C_N1;
			}
			set
			{
				if ((this._C_N1 != value))
				{
					this._C_N1 = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_C_N2", DbType="Int")]
		public System.Nullable<int> C_N2
		{
			get
			{
				return this._C_N2;
			}
			set
			{
				if ((this._C_N2 != value))
				{
					this._C_N2 = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_C_P1", DbType="Int")]
		public System.Nullable<int> C_P1
		{
			get
			{
				return this._C_P1;
			}
			set
			{
				if ((this._C_P1 != value))
				{
					this._C_P1 = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_C_P2", DbType="Int")]
		public System.Nullable<int> C_P2
		{
			get
			{
				return this._C_P2;
			}
			set
			{
				if ((this._C_P2 != value))
				{
					this._C_P2 = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_C_Y", DbType="Int")]
		public System.Nullable<int> C_Y
		{
			get
			{
				return this._C_Y;
			}
			set
			{
				if ((this._C_Y != value))
				{
					this._C_Y = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_C_V", DbType="Int")]
		public System.Nullable<int> C_V
		{
			get
			{
				return this._C_V;
			}
			set
			{
				if ((this._C_V != value))
				{
					this._C_V = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_GROUPID", DbType="VarChar(50)")]
		public string GROUPID
		{
			get
			{
				return this._GROUPID;
			}
			set
			{
				if ((this._GROUPID != value))
				{
					this._GROUPID = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_TRIAL", DbType="Int")]
		public System.Nullable<int> TRIAL
		{
			get
			{
				return this._TRIAL;
			}
			set
			{
				if ((this._TRIAL != value))
				{
					this._TRIAL = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ISLAST", DbType="Int")]
		public System.Nullable<int> ISLAST
		{
			get
			{
				return this._ISLAST;
			}
			set
			{
				if ((this._ISLAST != value))
				{
					this._ISLAST = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Result", DbType="Int")]
		public System.Nullable<int> Result
		{
			get
			{
				return this._Result;
			}
			set
			{
				if ((this._Result != value))
				{
					this._Result = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_EX_SET", DbType="Int")]
		public System.Nullable<int> EX_SET
		{
			get
			{
				return this._EX_SET;
			}
			set
			{
				if ((this._EX_SET != value))
				{
					this._EX_SET = value;
				}
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.TBSimulation2")]
	public partial class TBSimulation2 : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _KEY;
		
		private int _TURN;
		
		private string _USERID;
		
		private string _DECISION;
		
		private string _RESULTYN;
		
		private System.Nullable<int> _RESULT;
		
		private string _GROUPID;
		
		private System.Nullable<int> _TRIAL;
		
		private System.Nullable<int> _CONFIDENCE;
		
		private string _TIME;
		
		private System.Nullable<int> _EX_SET;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnKEYChanging(int value);
    partial void OnKEYChanged();
    partial void OnTURNChanging(int value);
    partial void OnTURNChanged();
    partial void OnUSERIDChanging(string value);
    partial void OnUSERIDChanged();
    partial void OnDECISIONChanging(string value);
    partial void OnDECISIONChanged();
    partial void OnRESULTYNChanging(string value);
    partial void OnRESULTYNChanged();
    partial void OnRESULTChanging(System.Nullable<int> value);
    partial void OnRESULTChanged();
    partial void OnGROUPIDChanging(string value);
    partial void OnGROUPIDChanged();
    partial void OnTRIALChanging(System.Nullable<int> value);
    partial void OnTRIALChanged();
    partial void OnCONFIDENCEChanging(System.Nullable<int> value);
    partial void OnCONFIDENCEChanged();
    partial void OnTIMEChanging(string value);
    partial void OnTIMEChanged();
    partial void OnEX_SETChanging(System.Nullable<int> value);
    partial void OnEX_SETChanged();
    #endregion
		
		public TBSimulation2()
		{
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Name="[KEY]", Storage="_KEY", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int KEY
		{
			get
			{
				return this._KEY;
			}
			set
			{
				if ((this._KEY != value))
				{
					this.OnKEYChanging(value);
					this.SendPropertyChanging();
					this._KEY = value;
					this.SendPropertyChanged("KEY");
					this.OnKEYChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_TURN", DbType="Int NOT NULL")]
		public int TURN
		{
			get
			{
				return this._TURN;
			}
			set
			{
				if ((this._TURN != value))
				{
					this.OnTURNChanging(value);
					this.SendPropertyChanging();
					this._TURN = value;
					this.SendPropertyChanged("TURN");
					this.OnTURNChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_USERID", DbType="VarChar(20)")]
		public string USERID
		{
			get
			{
				return this._USERID;
			}
			set
			{
				if ((this._USERID != value))
				{
					this.OnUSERIDChanging(value);
					this.SendPropertyChanging();
					this._USERID = value;
					this.SendPropertyChanged("USERID");
					this.OnUSERIDChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_DECISION", DbType="VarChar(1)")]
		public string DECISION
		{
			get
			{
				return this._DECISION;
			}
			set
			{
				if ((this._DECISION != value))
				{
					this.OnDECISIONChanging(value);
					this.SendPropertyChanging();
					this._DECISION = value;
					this.SendPropertyChanged("DECISION");
					this.OnDECISIONChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_RESULTYN", DbType="VarChar(1)")]
		public string RESULTYN
		{
			get
			{
				return this._RESULTYN;
			}
			set
			{
				if ((this._RESULTYN != value))
				{
					this.OnRESULTYNChanging(value);
					this.SendPropertyChanging();
					this._RESULTYN = value;
					this.SendPropertyChanged("RESULTYN");
					this.OnRESULTYNChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_RESULT", DbType="Int")]
		public System.Nullable<int> RESULT
		{
			get
			{
				return this._RESULT;
			}
			set
			{
				if ((this._RESULT != value))
				{
					this.OnRESULTChanging(value);
					this.SendPropertyChanging();
					this._RESULT = value;
					this.SendPropertyChanged("RESULT");
					this.OnRESULTChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_GROUPID", DbType="VarChar(50)")]
		public string GROUPID
		{
			get
			{
				return this._GROUPID;
			}
			set
			{
				if ((this._GROUPID != value))
				{
					this.OnGROUPIDChanging(value);
					this.SendPropertyChanging();
					this._GROUPID = value;
					this.SendPropertyChanged("GROUPID");
					this.OnGROUPIDChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_TRIAL", DbType="Int")]
		public System.Nullable<int> TRIAL
		{
			get
			{
				return this._TRIAL;
			}
			set
			{
				if ((this._TRIAL != value))
				{
					this.OnTRIALChanging(value);
					this.SendPropertyChanging();
					this._TRIAL = value;
					this.SendPropertyChanged("TRIAL");
					this.OnTRIALChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_CONFIDENCE", DbType="Int")]
		public System.Nullable<int> CONFIDENCE
		{
			get
			{
				return this._CONFIDENCE;
			}
			set
			{
				if ((this._CONFIDENCE != value))
				{
					this.OnCONFIDENCEChanging(value);
					this.SendPropertyChanging();
					this._CONFIDENCE = value;
					this.SendPropertyChanged("CONFIDENCE");
					this.OnCONFIDENCEChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_TIME", DbType="NVarChar(50)")]
		public string TIME
		{
			get
			{
				return this._TIME;
			}
			set
			{
				if ((this._TIME != value))
				{
					this.OnTIMEChanging(value);
					this.SendPropertyChanging();
					this._TIME = value;
					this.SendPropertyChanged("TIME");
					this.OnTIMEChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_EX_SET", DbType="Int")]
		public System.Nullable<int> EX_SET
		{
			get
			{
				return this._EX_SET;
			}
			set
			{
				if ((this._EX_SET != value))
				{
					this.OnEX_SETChanging(value);
					this.SendPropertyChanging();
					this._EX_SET = value;
					this.SendPropertyChanged("EX_SET");
					this.OnEX_SETChanged();
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
#pragma warning restore 1591
