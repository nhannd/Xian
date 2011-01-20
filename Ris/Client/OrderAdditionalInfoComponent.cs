#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Client
{
	public class OrderAdditionalInfoComponent : DHtmlComponent
	{
		private IDictionary<string, string> _orderExtendedProperties;
		private DataContractBase _healthcareContext;
		private readonly bool _readOnly;

		public OrderAdditionalInfoComponent()
			: this(false)
		{
		}

		public OrderAdditionalInfoComponent(bool readOnly)
		{
			_orderExtendedProperties = new Dictionary<string, string>();
			_readOnly = readOnly;
		}

		/// <summary>
		/// Gets or sets the dictionary of order extended properties that this component will
		/// use to store data.
		/// </summary>
		public IDictionary<string, string> OrderExtendedProperties
		{
			get { return _orderExtendedProperties; }
			set { _orderExtendedProperties = value; }
		}

		public override void Start()
		{
			SetUrl(WebResourcesSettings.Default.OrderAdditionalInfoPageUrl);
			base.Start();
		}

		protected override IDictionary<string, string> TagData
		{
			get
			{
				return _orderExtendedProperties;
			}
		}

		protected override DataContractBase GetHealthcareContext()
		{
			return _healthcareContext;
		}

		public DataContractBase HealthcareContext
		{
			get { return _healthcareContext; }
			set
			{
				_healthcareContext = value;
				NotifyAllPropertiesChanged();
			}
		}

		protected override string GetTag(string tag)
		{
			if (string.Equals("ReadOnly", tag))
			{
				return _readOnly ? "true" : "false";
			}
			else
			{
				return base.GetTag(tag);
			}
		}
	}
}
