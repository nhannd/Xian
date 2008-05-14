using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.OrderNotes;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Base class for tools which open an <see cref="OrderNoteConversationComponent"/>
	/// </summary>
	/// <typeparam name="TSummaryItem">A <see cref="DataContractBase"/> that can provide an OrderRef and an appropriate value for the component's title</typeparam>
	/// <typeparam name="TToolContext">Must be <see cref="IWorkflowItemToolContext{TSummaryItem}"/></typeparam>
	public abstract class OrderNoteConversationToolBase<TSummaryItem, TToolContext> : Tool<TToolContext> 
		where TSummaryItem : DataContractBase
		where TToolContext : IWorkflowItemToolContext<TSummaryItem>
	{
		private bool _enabled;
		private event EventHandler _enabledChanged;

		public override void Initialize()
		{
			base.Initialize();
			_enabled = false;   // disable by default

			((IWorkflowItemToolContext<TSummaryItem>)this.ContextBase).SelectionChanged += delegate
			{
				this.Enabled = (((IWorkflowItemToolContext<TSummaryItem>)this.ContextBase).SelectedItems != null
				&& ((IWorkflowItemToolContext<TSummaryItem>)this.ContextBase).SelectedItems.Count == 1);
			};
		}

		/// <summary>
		/// A title for the <see cref="OrderNoteConversationComponent"/> from the derived class' <see cref="TSummaryItem"/>
		/// </summary>
		protected abstract string TitleContextDescription { get; }

		/// <summary>
		/// An <see cref="EntityRef"/> for an Order from the derived class' <see cref="TSummaryItem"/>.
		/// </summary>
		protected abstract EntityRef OrderRef { get; }

		/// <summary>
		/// Specifies an enumeration of <see cref="OrderNoteCategory"/> which will be displayed in the <see cref="OrderNoteConversationComponent"/>
		/// </summary>
		protected abstract IEnumerable<string> OrderNoteCategories { get; }

		/// <summary>
		/// The first <see cref="TSummaryItem"/> in the current <see cref="TToolContext"/>
		/// </summary>
		protected TSummaryItem SummaryItem
		{
			get
			{
				IWorkflowItemToolContext<TSummaryItem> context = (IWorkflowItemToolContext<TSummaryItem>)this.ContextBase;
				return CollectionUtils.FirstElement(context.SelectedItems);
			}
		}

		public bool Enabled
		{
			get { return _enabled; }
			set
			{
				if (_enabled != value)
				{
					_enabled = value;
					EventsHelper.Fire(_enabledChanged, this, EventArgs.Empty);
				}
			}
		}

		public event EventHandler EnabledChanged
		{
			add { _enabledChanged += value; }
			remove { _enabledChanged -= value; }
		}

		/// <summary>
		/// Opens an <see cref="OrderNoteConversationComponent"/>
		/// </summary>
		public void Open()
		{
			IWorkflowItemToolContext context = (IWorkflowItemToolContext) this.ContextBase;
			Open(this.OrderRef, GetTitle(), this.OrderNoteCategories, context.DesktopWindow);
		}

		private static void Open(EntityRef orderRef, string title, IEnumerable<string> orderNoteCategories, IDesktopWindow desktopWindow)
		{
			Platform.CheckForNullReference(orderRef, "orderRef");

			try
			{
				ApplicationComponent.LaunchAsDialog(desktopWindow,
					new OrderNoteConversationComponent(orderRef, orderNoteCategories),
					title);
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, desktopWindow);
			}
		}

		private string GetTitle()
		{
			StringBuilder sb = new StringBuilder();

			sb.Append(
				CollectionUtils.Reduce<string, string>(
					this.OrderNoteCategories, 
					"", 
					delegate(string categoryKey, string memo)
					{
						OrderNoteCategory category;
						category = OrderNoteCategory.FromKey(categoryKey);
						return memo + (category != null ? category.DisplayValue : "");
					}));
			sb.Append(" Conversation");
			sb.Append(this.TitleContextDescription);

			return sb.ToString();
		}
	}

	/// <summary>
	/// Base class for tools which open an <see cref="OrderNoteConversationComponent"/> from an <see cref="IOrderNoteboxItemToolContext"/>
	/// </summary>
	[MenuAction("pd", "folderexplorer-items-contextmenu/Open Conversation", "Open")]
	[ButtonAction("pd", "folderexplorer-items-toolbar/Open Conversation", "Open")]
	[Tooltip("pd", "Review/reply to the selected note.")]
	[EnabledStateObserver("pd", "Enabled", "EnabledChanged")]
	[IconSet("pd", IconScheme.Colour, "Icons.OrderNoteConversationToolSmall.png", "Icons.OrderNoteConversationToolSmall.png", "Icons.OrderNoteConversationToolSmall.png")]
	public abstract class OrderNoteConversationTool : OrderNoteConversationToolBase<OrderNoteboxItemSummary, IOrderNoteboxItemToolContext>
	{
		protected override EntityRef OrderRef
		{
			get { return this.SummaryItem.OrderRef; }
		}

		protected override string TitleContextDescription
		{
			get
			{
				return string.Format(SR.FormatTitleContextDescriptionOrderNoteConversation,
					PersonNameFormat.Format(this.SummaryItem.PatientName),
					MrnFormat.Format(this.SummaryItem.Mrn),
					AccessionFormat.Format(this.SummaryItem.AccessionNumber));
			}
		}

		protected override IEnumerable<string> OrderNoteCategories
		{
			get { return new string[] { this.SummaryItem.Category }; }
		}
	}
}