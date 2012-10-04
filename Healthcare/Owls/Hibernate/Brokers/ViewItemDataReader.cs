#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Data;
using NHibernate.Cfg;
using System.Collections;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare.Owls.Hibernate.Brokers
{
	/// <summary>
	/// Implementation of <see cref="IDataReader"/> that exposes an in-memory list of view items
	/// as a data reader.
	/// </summary>
	/// <remarks>
	/// This is a helper class that assists in efficiently populating views using the SQL bulk import functionality.
	/// </remarks>
	internal class ViewItemDataReader : IDataReader
	{
		public delegate IList ItemBatchProviderDelegate();

		private readonly ViewItemTableMapping _viewMapping;
		private readonly ItemBatchProviderDelegate _itemBatchProvider;
		private readonly int _oidColumnIndex = -1;
		private IList _currentBatch;
		private int _currentIndex = -1;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="config"></param>
		/// <param name="viewItemClass"></param>
		/// <param name="itemBatchProvider"></param>
		internal ViewItemDataReader(Configuration config, Type viewItemClass, ItemBatchProviderDelegate itemBatchProvider)
		{
			_viewMapping = new ViewItemTableMapping(config, viewItemClass);
			_itemBatchProvider = itemBatchProvider;
			_currentBatch = new object[0];

			// in case we need to auto-gen
			_oidColumnIndex = _viewMapping.ColumnNames.IndexOf("OID_");
		}

		/// <summary>
		/// Gets or sets a value indicating whether the data reader should auto-generate values for the OID column,
		/// if the underlying source data does not provide a value.
		/// </summary>
		public bool AutoGenerateObjectIDs { get; set; }

		/// <summary>
		/// Gets the name of the table that this view item class is mapped to.
		/// </summary>
		public string TableName
		{
			get { return _viewMapping.TableName; }
		}

		#region IDataReader members

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		/// <filterpriority>2</filterpriority>
		void IDisposable.Dispose()
		{
			// nothing to do
		}

		/// <summary>
		/// Gets the name for the field to find.
		/// </summary>
		/// <returns>
		/// The name of the field or the empty string (""), if there is no value to return.
		/// </returns>
		/// <param name="i">The index of the field to find. 
		///                 </param><exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. 
		///                 </exception><filterpriority>2</filterpriority>
		string IDataRecord.GetName(int i)
		{
			return _viewMapping.ColumnNames[i];
		}

		/// <summary>
		/// Return the value of the specified field.
		/// </summary>
		/// <returns>
		/// The <see cref="T:System.Object"/> which will contain the field value upon return.
		/// </returns>
		/// <param name="i">The index of the field to find. 
		///                 </param><exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. 
		///                 </exception><filterpriority>2</filterpriority>
		object IDataRecord.GetValue(int i)
		{
			var item = _currentBatch[_currentIndex];
			var value = _viewMapping.ColumnValueProviders[i](item);

			// do we need to generate an object ID?
			if (i == _oidColumnIndex && value == null && AutoGenerateObjectIDs)
				return Guid.NewGuid();

			if (value is Entity)
				return ((Entity)value).OID;
			if (value is EnumValue)
				return ((EnumValue)value).Code;
			return value;
		}

		/// <summary>
		/// Return the index of the named field.
		/// </summary>
		/// <returns>
		/// The index of the named field.
		/// </returns>
		/// <param name="name">The name of the field to find. 
		///                 </param><filterpriority>2</filterpriority>
		int IDataRecord.GetOrdinal(string name)
		{
			return _viewMapping.ColumnNames.IndexOf(name);
		}

		/// <summary>
		/// Gets the number of columns in the current row.
		/// </summary>
		/// <returns>
		/// When not positioned in a valid recordset, 0; otherwise, the number of columns in the current record. The default is -1.
		/// </returns>
		/// <filterpriority>2</filterpriority>
		int IDataRecord.FieldCount
		{
			get { return _viewMapping.ColumnNames.Count; }
		}

		/// <summary>
		/// Closes the <see cref="T:System.Data.IDataReader"/> Object.
		/// </summary>
		/// <filterpriority>2</filterpriority>
		void IDataReader.Close()
		{
			// nothing to do
		}

		/// <summary>
		/// Advances the data reader to the next result, when reading the results of batch SQL statements.
		/// </summary>
		/// <returns>
		/// true if there are more rows; otherwise, false.
		/// </returns>
		/// <filterpriority>2</filterpriority>
		bool IDataReader.NextResult()
		{
			return false;
		}

		/// <summary>
		/// Advances the <see cref="T:System.Data.IDataReader"/> to the next record.
		/// </summary>
		/// <returns>
		/// true if there are more rows; otherwise, false.
		/// </returns>
		/// <filterpriority>2</filterpriority>
		bool IDataReader.Read()
		{
			// advance location
			_currentIndex++;

			// if we are past the end of the batch, get a new batch
			if (_currentIndex == _currentBatch.Count)
			{
				// get new batch and set location to first item
				_currentBatch = _itemBatchProvider();
				_currentIndex = 0;

				// if the new batch is empty, we are at the end of the dataset
				return _currentBatch.Count > 0;
			}

			return true;
		}

		#endregion

		#region Unused IDataReader members

		string IDataRecord.GetDataTypeName(int i)
		{
			throw new NotImplementedException();
		}

		Type IDataRecord.GetFieldType(int i)
		{
			throw new NotImplementedException();
		}

		int IDataRecord.GetValues(object[] values)
		{
			throw new NotImplementedException();
		}

		bool IDataRecord.GetBoolean(int i)
		{
			throw new NotImplementedException();
		}

		byte IDataRecord.GetByte(int i)
		{
			throw new NotImplementedException();
		}

		long IDataRecord.GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
		{
			throw new NotImplementedException();
		}

		char IDataRecord.GetChar(int i)
		{
			throw new NotImplementedException();
		}

		long IDataRecord.GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
		{
			throw new NotImplementedException();
		}

		Guid IDataRecord.GetGuid(int i)
		{
			throw new NotImplementedException();
		}

		short IDataRecord.GetInt16(int i)
		{
			throw new NotImplementedException();
		}

		int IDataRecord.GetInt32(int i)
		{
			throw new NotImplementedException();
		}

		long IDataRecord.GetInt64(int i)
		{
			throw new NotImplementedException();
		}

		float IDataRecord.GetFloat(int i)
		{
			throw new NotImplementedException();
		}

		double IDataRecord.GetDouble(int i)
		{
			throw new NotImplementedException();
		}

		string IDataRecord.GetString(int i)
		{
			throw new NotImplementedException();
		}

		decimal IDataRecord.GetDecimal(int i)
		{
			throw new NotImplementedException();
		}

		DateTime IDataRecord.GetDateTime(int i)
		{
			throw new NotImplementedException();
		}

		IDataReader IDataRecord.GetData(int i)
		{
			throw new NotImplementedException();
		}

		bool IDataRecord.IsDBNull(int i)
		{
			throw new NotImplementedException();
		}

		object IDataRecord.this[int i]
		{
			get { throw new NotImplementedException(); }
		}

		object IDataRecord.this[string name]
		{
			get { throw new NotImplementedException(); }
		}

		DataTable IDataReader.GetSchemaTable()
		{
			throw new NotImplementedException();
		}

		int IDataReader.Depth
		{
			get { throw new NotImplementedException(); }
		}

		bool IDataReader.IsClosed
		{
			get { throw new NotImplementedException(); }
		}

		int IDataReader.RecordsAffected
		{
			get { throw new NotImplementedException(); }
		}

		#endregion
	}
}
