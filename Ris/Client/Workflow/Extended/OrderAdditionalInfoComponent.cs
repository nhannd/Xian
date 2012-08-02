#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Common;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Client.Workflow.Extended
{
	[ExtensionOf(typeof(OrderEditorPageProviderExtensionPoint))]
	public class OrderEditorAdditionalInfoPageProvider : IOrderEditorPageProvider
	{
		class OrderEditorAdditionalInfoPage : IOrderEditorPage
		{
			private readonly IOrderEditorContext _context;
			private OrderAdditionalInfoComponent _component;

			public OrderEditorAdditionalInfoPage(IOrderEditorContext context)
			{
				_context = context;
			}

			public void Save()
			{
				_component.SaveData();
			}

			public Desktop.Path Path
			{
				get { return new Desktop.Path("Additional Info"); }
			}

			public Desktop.IApplicationComponent GetComponent()
			{
				if (_component == null)
				{
					_component = new OrderAdditionalInfoComponent();
					_component.ModifiedChanged += ((sender, args) => _context.SetModified());
					UpdateComponentData();

					_context.OrderLoaded += (sender, args) => UpdateComponentData();
				}
				return _component;
			}

			private void UpdateComponentData()
			{
				_component.OrderExtendedProperties = _context.OrderExtendedProperties ?? new Dictionary<string, string>();
				_component.Context = _context.OrderRef == null ? null : new OrderAdditionalInfoComponent.HealthcareContext(_context.OrderRef);
			}
		}

		IOrderEditorPage[] IExtensionPageProvider<IOrderEditorPage, IOrderEditorContext>.GetPages(IOrderEditorContext context)
		{
			return new IOrderEditorPage[] { new OrderEditorAdditionalInfoPage(context) };
		}
	}


	[ExtensionOf(typeof(BiographyOrderHistoryPageProviderExtensionPoint))]
	public class BiographyAdditionalInfoPageProvider : IBiographyOrderHistoryPageProvider
	{
		class BiographyAdditionalInfoPage : IBiographyOrderHistoryPage
		{
			private readonly IBiographyOrderHistoryContext _context;
			private OrderAdditionalInfoComponent _component;

			public BiographyAdditionalInfoPage(IBiographyOrderHistoryContext context)
			{
				_context = context;
			}

			public Desktop.Path Path
			{
				get { return new Desktop.Path("Additional Info"); }
			}

			public Desktop.IApplicationComponent GetComponent()
			{
				if (_component == null)
				{
					_component = new OrderAdditionalInfoComponent(true);
					_context.OrderListItemChanged += (sender, args) =>
					{
						_component.OrderExtendedProperties = _context.Order == null ? new Dictionary<string, string>() : _context.Order.ExtendedProperties;
						_component.Context = _context.Order == null ? null : new OrderAdditionalInfoComponent.HealthcareContext(_context.Order.OrderRef);
					};
				}
				return _component;
			}
		}

		IBiographyOrderHistoryPage[] IExtensionPageProvider<IBiographyOrderHistoryPage, IBiographyOrderHistoryContext>.GetPages(IBiographyOrderHistoryContext context)
		{
			return new IBiographyOrderHistoryPage[] { new BiographyAdditionalInfoPage(context) };
		}
	}


	[ExtensionOf(typeof(PerformingDocumentationOrderDetailsPageProviderExtensionPoint))]
	public class PerformingDocumentationAdditionalInfoPageProvider : IPerformingDocumentationOrderDetailsPageProvider
	{
		public class PerformingDocumentationAdditionalInfoPage : IPerformingDocumentationOrderDetailsPage
		{
			private readonly IPerformingDocumentationOrderDetailsContext _context;
			private OrderAdditionalInfoComponent _component;

			public PerformingDocumentationAdditionalInfoPage(IPerformingDocumentationOrderDetailsContext context)
			{
				_context = context;
			}

			public Desktop.Path Path
			{
				get { return new Desktop.Path("Additional Info"); }
			}

			public Desktop.IApplicationComponent GetComponent()
			{
				if (_component == null)
				{
					_component = new OrderAdditionalInfoComponent
									{
										OrderExtendedProperties = _context.OrderExtendedProperties,
										Context = new OrderAdditionalInfoComponent.HealthcareContext(_context.WorklistItem.OrderRef)
									};
				}
				return _component;
			}

			public void Save()
			{
				_component.SaveData();
			}
		}

		IPerformingDocumentationOrderDetailsPage[] IExtensionPageProvider<IPerformingDocumentationOrderDetailsPage, IPerformingDocumentationOrderDetailsContext>.GetPages(IPerformingDocumentationOrderDetailsContext context)
		{
			return new IPerformingDocumentationOrderDetailsPage[] { new PerformingDocumentationAdditionalInfoPage(context) };
		}
	}

	[ExtensionOf(typeof(ReportingPageProviderExtensionPoint))]
	public class ReportingAdditionalInfoPageProvider : IReportingPageProvider
	{
		class ReportingAdditionalInfoPage : IReportingPage
		{
			private readonly IReportingContext _context;
			private OrderAdditionalInfoComponent _component;

			public ReportingAdditionalInfoPage(IReportingContext context)
			{
				_context = context;
			}

			public Desktop.Path Path
			{
				get { return new Desktop.Path("Additional Info"); }
			}

			public Desktop.IApplicationComponent GetComponent()
			{
				if (_component == null)
				{
					_component = new OrderAdditionalInfoComponent(true);
					UpdateComponentData();

					_context.WorklistItemChanged += (sender, args) => UpdateComponentData();
				}
				return _component;
			}

			private void UpdateComponentData()
			{
				_component.OrderExtendedProperties = _context.Order == null ? new Dictionary<string, string>() : _context.Order.ExtendedProperties;
				_component.Context = _context.Order == null ? null : new OrderAdditionalInfoComponent.HealthcareContext(_context.Order.OrderRef);
			}
		}


		public IReportingPage[] GetPages(IReportingContext context)
		{
			return new IReportingPage[] { new ReportingAdditionalInfoPage(context) };
		}
	}

	[ExtensionOf(typeof(MergeOrdersPageProviderExtensionPoint))]
	public class MergeOrdersAdditionalInfoPageProvider : IMergeOrdersPageProvider
	{
		class MergeOrdersAdditionalInfoPage : IMergeOrdersPage
		{
			private readonly IMergeOrdersContext _context;
			private OrderAdditionalInfoComponent _component;

			public MergeOrdersAdditionalInfoPage(IMergeOrdersContext context)
			{
				_context = context;
			}

			public Desktop.Path Path
			{
				get { return new Desktop.Path("Additional Info"); }
			}

			public Desktop.IApplicationComponent GetComponent()
			{
				if (_component == null)
				{
					_component = new OrderAdditionalInfoComponent(true);
					UpdateComponentData();

					_context.DryRunMergedOrderChanged += (sender, args) => UpdateComponentData();
				}
				return _component;
			}

			private void UpdateComponentData()
			{
				_component.OrderExtendedProperties = _context.DryRunMergedOrder == null ? new Dictionary<string, string>() : _context.DryRunMergedOrder.ExtendedProperties;
				_component.Context = _context.DryRunMergedOrder == null ? null : new OrderAdditionalInfoComponent.HealthcareContext(_context.DryRunMergedOrder.OrderRef);
			}
		}

		public IMergeOrdersPage[] GetPages(IMergeOrdersContext context)
		{
			return new IMergeOrdersPage[] { new MergeOrdersAdditionalInfoPage(context) };
		}
	}

	public class OrderAdditionalInfoComponent : DHtmlComponent
	{
		[DataContract]
		public class HealthcareContext : DataContractBase
		{
			public HealthcareContext(EntityRef orderRef)
			{
				this.OrderRef = orderRef;
			}

			[DataMember]
			public EntityRef OrderRef;
		}


		private IDictionary<string, string> _orderExtendedProperties;
		private HealthcareContext _healthcareContext;
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
			set
			{
				_orderExtendedProperties = value;

				// refresh the page
				SetUrl(WebResourcesSettings.Default.OrderAdditionalInfoPageUrl);
			}
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

		public HealthcareContext Context
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
			return base.GetTag(tag);
		}
	}
}
