#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca

// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Runtime.Serialization;

namespace ClearCanvas.Common.Configuration
{
	/// <summary>
	/// Represents a configuration document query.
	/// </summary>
	[DataContract]
	public class ConfigurationDocumentQuery
	{
		/// <summary>
		/// Defines the set of supported string operators.
		/// </summary>
		public enum StringOperator
		{
			/// <summary>
			/// Exact match
			/// </summary>
			Exact,

			/// <summary>
			/// Starts with
			/// </summary>
			StartsWith
		}

		/// <summary>
		/// Defines the set of supported date-time operations.
		/// </summary>
		public enum DateTimeOperator
		{
			/// <summary>
			/// Before
			/// </summary>
			Before,

			/// <summary>
			/// After
			/// </summary>
			After
		}

		/// <summary>
		/// Defines the set of document user types.
		/// </summary>
		public enum DocumentUserType
		{
			/// <summary>
			/// Documents owned by the current user.
			/// </summary>
			User,

			/// <summary>
			/// Shared documents.
			/// </summary>
			Shared,

		}

		/// <summary>
		/// Abstract base class for criteria objects.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		[DataContract]
		public class Criteria<T>
		{
			[DataMember]
			private T _value;

			/// <summary>
			/// Clears this criteria.
			/// </summary>
			public virtual void Clear()
			{
				this.Value = default(T);
				this.IsSet = false;
			}

			/// <summary>
			/// Gets or sets the value of this criteria.
			/// </summary>
			public T Value
			{
				get { return _value; }
				set
				{
					_value = value;
					this.IsSet = true;
				}
			}

			/// <summary>
			/// Gets a value indicating whether this criteria has been set.
			/// </summary>
			[DataMember]
			public bool IsSet { get; private set; }
		}

		/// <summary>
		/// Document name criteria class.
		/// </summary>
		[DataContract]
		public class DocumentNameCriteria : Criteria<string>
		{
			/// <summary>
			/// Gets or sets the matching operator.
			/// </summary>
			[DataMember]
			public StringOperator Operator { get; set; } 
		}

		/// <summary>
		/// Document version criteria class.
		/// </summary>
		[DataContract]
		public class VersionCriteria : Criteria<Version>
		{
		}

		/// <summary>
		/// User name criteria class.
		/// </summary>
		[DataContract]
		public class UserCriteria : Criteria<DocumentUserType>
		{
		}

		/// <summary>
		/// Instance key criteria class.
		/// </summary>
		[DataContract]
		public class InstanceKeyCriteria : Criteria<string>
		{
		}

		/// <summary>
		/// Date time criteria class.
		/// </summary>
		[DataContract]
		public class DateTimeCriteria : Criteria<DateTime>
		{
			/// <summary>
			/// Gets or sets the matching operator.
			/// </summary>
			[DataMember]
			public DateTimeOperator Operator { get; set; }
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public ConfigurationDocumentQuery()
		{
			this.DocumentName = new DocumentNameCriteria();
			this.Version = new VersionCriteria();
			this.InstanceKey = new InstanceKeyCriteria();
			this.CreationTime = new DateTimeCriteria();
			this.ModifiedTime = new DateTimeCriteria();
			this.FirstResult = 0;
			this.MaxResults = 100;
		}

		/// <summary>
		/// Gets the criteria for the name of the document.
		/// </summary>
		[DataMember]
		public DocumentNameCriteria DocumentName { get; private set; }

		/// <summary>
		/// Gets the criteria for the version of the document.
		/// </summary>
		[DataMember]
		public VersionCriteria Version { get; private set; }

		/// <summary>
		/// Gets the criteria for the owner of the document.
		/// </summary>
		[DataMember]
		public DocumentUserType UserType { get; set; }

		/// <summary>
		/// Gets the criteria for the instance key of the document.
		/// </summary>
		[DataMember]
		public InstanceKeyCriteria InstanceKey { get; private set; }

		/// <summary>
		/// Gets the criteria for the document creation time.
		/// </summary>
		[DataMember]
		public DateTimeCriteria CreationTime { get; private set; }

		/// <summary>
		/// Gets the criteria for the document last modification time.
		/// </summary>
		[DataMember]
		public DateTimeCriteria ModifiedTime { get; private set; }

		/// <summary>
		/// Gets or sets the index of the first result to return.
		/// </summary>
		[DataMember]
		public int FirstResult { get; set; }

		/// <summary>
		/// Gets or sets the maximum number of results to return.
		/// </summary>
		[DataMember]
		public int MaxResults { get; set; }
	}
}
