#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common.Serialization;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Client
{
	public abstract class OrderDetailViewComponent : DHtmlComponent
	{
		// Internal data contract used for jscript deserialization
		[DataContract]
		public class OrderContext : DataContractBase
		{
			public OrderContext(EntityRef orderRef)
			{
				this.OrderRef = orderRef;
			}

			[DataMember]
			public EntityRef OrderRef;
		}

        protected DataContractBase _context;

		public OrderDetailViewComponent()
			: this(null)
		{
		}

		public OrderDetailViewComponent(EntityRef orderRef)
		{
			_context = orderRef == null ? null : new OrderContext(orderRef);
		}

		public override void Start()
		{
			SetUrl(this.PageUrl);
			base.Start();
		}

		protected override DataContractBase GetHealthcareContext()
		{
			return _context;
		}

		protected abstract string PageUrl { get; }

        public virtual DataContractBase Context
		{
			get { return _context; }
			set
			{
				_context = value;
				if(this.IsStarted)
				{
					NotifyAllPropertiesChanged();
				}
			}
		}
	}
}
