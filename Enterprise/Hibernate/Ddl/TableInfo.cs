using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace ClearCanvas.Enterprise.Hibernate.Ddl
{
    [DataContract]
    public class TableInfo : ElementInfo
    {
		private string _name;
    	private string _schema;
    	private List<ColumnInfo> _columns;
    	private ConstraintInfo _primaryKey;
    	private List<IndexInfo> _indexes;
    	private List<ForeignKeyInfo> _foreignKeys;
    	private List<ConstraintInfo> _uniqueKeys;

    	public TableInfo()
        {

        }

        public TableInfo(string name, string schema, List<ColumnInfo> columns, ConstraintInfo primaryKey, List<IndexInfo> indexes, List<ForeignKeyInfo> foreignKeys, List<ConstraintInfo> uniqueKeys)
        {
            Name = name;
            Schema = schema;
            Columns = columns;
            PrimaryKey = primaryKey;
            Indexes = indexes;
            ForeignKeys = foreignKeys;
            UniqueKeys = uniqueKeys;
        }

    	[DataMember]
    	public string Name
    	{
    		get { return _name;}
			private set { _name = value; }
    	}

        [DataMember]
        public string Schema
    	{
    		get { return _schema; }
			private set { _schema = value; }
    	}

        [DataMember]
        public List<ColumnInfo> Columns
    	{
    		get { return _columns;}
			private set { _columns = value; }
    	}

        [DataMember]
        public ConstraintInfo PrimaryKey
    	{
    		get { return _primaryKey;}
			private set { _primaryKey = value; }
    	}

        [DataMember]
        public List<IndexInfo> Indexes
     	{
    		get { return _indexes;}
			private set { _indexes = value; }
    	}

        [DataMember]
        public List<ForeignKeyInfo> ForeignKeys
    	{
    		get { return _foreignKeys;}
			private set { _foreignKeys = value; }
    	}

        [DataMember]
        public List<ConstraintInfo> UniqueKeys
	    {
    		get { return _uniqueKeys;}
			private set { _uniqueKeys = value; }
    	}


        public override string Identity
        {
            get { return Name; }
        }
    }
}
