using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace ClearCanvas.Enterprise.Hibernate.Ddl
{
	public class EnumerationMemberInfo : ElementInfo
	{
		private string _code;
		private string _value;
		private string _description;
		private float _displayOrder;
		private bool _deactivated;

		public EnumerationMemberInfo()
		{
		}


		public EnumerationMemberInfo(string code, string value, string description, float displayOrder, bool deactivated)
		{
			_code = code;
			_value = value;
			_description = description;
			_displayOrder = displayOrder;
			_deactivated = deactivated;
		}

		[DataMember]
		public string Code
		{
			get { return _code; }
			private set { _code = value; }
		}

		[DataMember]
		public string Value
		{
			get { return _value; }
			private set { _value = value; }
		}

		[DataMember]
		public string Description
		{
			get { return _description; }
			private set { _description = value; }
		}

		[DataMember]
		public float DisplayOrder
		{
			get { return _displayOrder; }
			private set { _displayOrder = value; }
		}

		[DataMember]
		public bool Deactivated
		{
			get { return _deactivated; }
			private set { _deactivated = value; }
		}

		public override string Identity
		{
			get { return _code; }
		}
	}


	public class EnumerationInfo : ElementInfo
	{
		private string _enumerationClass;
		private List<EnumerationMemberInfo> _members;
		private bool _isHard;
		private string _table;

		public EnumerationInfo()
		{

		}

		public EnumerationInfo(string enumerationClass, string table, bool isHard, List<EnumerationMemberInfo> members)
		{
			_enumerationClass = enumerationClass;
			_members = members;
			_isHard = isHard;
			_table = table;
		}

		[DataMember]
		public string EnumerationClass
		{
			get { return _enumerationClass; }
			private set { _enumerationClass = value; }
		}

		[DataMember]
		public string Table
		{
			get { return _table; }
			private set { _table = value; }
		}

		[DataMember]
		public List<EnumerationMemberInfo> Members
		{
			get { return _members; }
			private set { _members = value; }
		}

		[DataMember]
		public bool IsHard
		{
			get { return _isHard; }
			private set { _isHard = value; }
		}

		public override string Identity
		{
			get { return _enumerationClass; }
		}
	}
}
