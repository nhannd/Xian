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

		public virtual bool Enabled
		{
			get
			{
				ICollection<TSummaryItem> items = ((IWorkflowItemToolContext<TSummaryItem>) this.ContextBase).SelectedItems;
				return items != null && items.Count == 1;
			}
		}

		public event EventHandler EnabledChanged
		{
			add { ((IWorkflowItemToolContext<TSummaryItem>)this.ContextBase).SelectionChanged += value; }
			remove { ((IWorkflowItemToolContext<TSummaryItem>)this.ContextBase).SelectionChanged -= value; }
		}

		/// <summary>
		/// Opens an <see cref="OrderNoteConversationComponent"/>
		/// </summary>
		public void Open()
		{
			if(this.OrderRef == null)
				return;

			try
			{
				OrderNoteConversationComponent component = new OrderNoteConversationComponent(this.OrderRef, this.OrderNoteCategories);
				component.Body = this.InitialNoteText;
				ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(this.Context.DesktopWindow,
					component,
					GetTitle());
				OnDialogClosed(exitCode);
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Context.DesktopWindow);
			}
		}

		protected virtual void OnDialogClosed(ApplicationComponentExitCode exitCode)
		{
		}

		protected virtual string InitialNoteText
		{
			get { return ""; }
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
			sb.Append(this.TitleContextDescription);

			return sb.ToString();
		}
	}

	/// <summary>
	/// Tool which opens an <see cref="OrderNoteConversationComponent"/> from an <see cref="IOrderNoteboxItemToolContext"/>
	/// </summary>
	[MenuAction("pd", "folderexplorer-items-contextmenu/Open Conversation", "Open")]
	[ButtonAction("pd", "folderexplorer-items-toolbar/Open Conversation", "Open")]
	[Tooltip("pd", "Review/reply to the selected note.")]
	[EnabledStateObserver("pd", "Enabled", "EnabledChanged")]
	[IconSet("pd", IconScheme.Colour, "Icons.OrderNoteConversationSmall.png", "Icons.OrderNoteConversationMedium.png", "Icons.OrderNoteConversationLarge.png")]
	[ExtensionOf(typeof(OrderNoteboxItemToolExtensionPoint))]
	public class OrderNoteConversationTool : OrderNoteConversationToolBase<OrderNoteboxItemSummary, IOrderNoteboxItemToolContext>
	{
        public override void Initialize()
        {
            base.Initialize();

			this.Context.RegisterDoubleClickHandler(
                (IClickAction) CollectionUtils.SelectFirst(this.Actions, 
                    delegate(IAction a) { return a is IClickAction && a.ActionID.EndsWith("pd"); }));
        }

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

		protected override void OnDialogClosed(ApplicationComponentExitCode exitCode)
		{
			// invalidate the sent items folder in case any notes were posted
			this.Context.InvalidateFolders(typeof(SentItemsFolder));
			this.Context.InvalidateFolders(typeof(PersonalInboxFolder));
			this.Context.InvalidateFolders(typeof(GroupInboxFolder));

			base.OnDialogClosed(exitCode);
		}
	}
}