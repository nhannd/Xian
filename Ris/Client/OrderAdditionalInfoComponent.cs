#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

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
