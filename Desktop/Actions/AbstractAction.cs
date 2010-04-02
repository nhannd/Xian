﻿#region License

// Copyright (c) 2010, ClearCanvas Inc.
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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.Actions
{
	/// <summary>
	/// A view-less implementation of <see cref="IAction"/>.
	/// </summary>
	internal class AbstractAction : IAction
	{
		#region Static Helpers

		/// <summary>
		/// Creates an <see cref="AbstractAction"/> from a concrete <see cref="IAction"/>.
		/// </summary>
		/// <param name="action"></param>
		/// <returns></returns>
		public static AbstractAction Create(IAction action)
		{
			if (action is IClickAction)
				return new AbstractClickAction((IClickAction) action);
			return new AbstractAction(action);
		}

		public static AbstractAction Create(string id, string path, bool isClickAction)
		{
			if (isClickAction)
				return new AbstractClickAction(id, path);
			return new AbstractAction(id, path);
		}

		#endregion

		private event EventHandler _availableChanged;

		private static readonly IResourceResolver _globalResourceResolver = new ResourceResolver(AppDomain.CurrentDomain.GetAssemblies());

		private readonly IResourceResolver _resourceResolver;
		private readonly IconSet _iconSet;
		private readonly string _actionId;
		private readonly string _label;
		private readonly string _tooltip;
		private readonly bool _permissible;

		private ActionPath _path;
		private GroupHint _groupHint;
		private bool _available;

		private AbstractAction(string id, string path)
		{
			Platform.CheckForEmptyString(id, "id");
			Platform.CheckForEmptyString(path, "path");

			_resourceResolver = _globalResourceResolver;
			_actionId = id;
			_path = new ActionPath(path, _globalResourceResolver);
			_groupHint = new GroupHint(string.Empty);
			_label = string.Empty;
			_tooltip = string.Empty;
			_iconSet = null;
			_available = true;
			_permissible = false;
		}

		private AbstractAction(IAction concreteAction)
		{
			Platform.CheckForNullReference(concreteAction, "concreteAction");
			Platform.CheckTrue(concreteAction.Persistent, "Action must be persistent.");

			_resourceResolver = concreteAction.ResourceResolver;
			_actionId = concreteAction.ActionID;
			_path = new ActionPath(concreteAction.Path.ToString(), concreteAction.ResourceResolver);
			_groupHint = new GroupHint(concreteAction.GroupHint.Hint);
			_label = concreteAction.Label;
			_tooltip = concreteAction.Tooltip;
			_iconSet = concreteAction.IconSet;
			_available = concreteAction.Available;
			_permissible = concreteAction.Permissible;
		}

		public string ActionId
		{
			get { return _actionId; }
		}

		string IAction.ActionID
		{
			get { return this.ActionId; }
		}

		public ActionPath Path
		{
			get { return _path; }
			set { _path = value; }
		}

		public string GroupHint
		{
			get { return _groupHint.Hint; }
			set { _groupHint = new GroupHint(value); }
		}

		GroupHint IAction.GroupHint
		{
			get { return _groupHint; }
			set { _groupHint = value; }
		}

		public bool Available
		{
			get { return _available; }
			set
			{
				if (_available != value)
				{
					_available = value;
					EventsHelper.Fire(_availableChanged, this, EventArgs.Empty);
				}
			}
		}

		public event EventHandler AvailableChanged
		{
			add { _availableChanged += value; }
			remove { _availableChanged -= value; }
		}

		/// <summary>
		/// <see cref="Label"/> is currently not persisted in the action model.
		/// </summary>
		public string Label
		{
			get { return _label; }
		}

		public event EventHandler LabelChanged
		{
			add { }
			remove { }
		}

		/// <summary>
		/// <see cref="Tooltip"/> is currently not persisted in the action model.
		/// </summary>
		public string Tooltip
		{
			get { return _tooltip; }
		}

		public event EventHandler TooltipChanged
		{
			add { }
			remove { }
		}

		/// <summary>
		/// <see cref="IconSet"/> is currently not persisted in the action model.
		/// </summary>
		public IconSet IconSet
		{
			get { return _iconSet; }
		}

		public event EventHandler IconSetChanged
		{
			add { }
			remove { }
		}

		/// <summary>
		/// This value is always determined at runtime based on the user's access permissions.
		/// </summary>
		public bool Permissible
		{
			get { return _permissible; }
		}

		/// <summary>
		/// This value is always based on the assembly defining the action.
		/// </summary>
		public IResourceResolver ResourceResolver
		{
			get { return _resourceResolver; }
		}

		/// <summary>
		/// This value is always true, otherwise there's point in configuring it's position/properties in the persisted action model.
		/// </summary>
		bool IAction.Persistent
		{
			get { return true; }
		}

		/// <summary>
		/// This value is always dynamically tool-controlled and may not be overriden by the persisted action model.
		/// </summary>
		bool IAction.Enabled
		{
			get { return true; }
		}

		/// <summary>
		/// This event never fires becauses <see cref="Enabled"/> is not persistable.
		/// </summary>
		event EventHandler IAction.EnabledChanged
		{
			add { }
			remove { }
		}

		/// <summary>
		/// This value is always dynamically tool-controlled and may not be overriden by the persisted action model.
		/// </summary>
		bool IAction.Visible
		{
			get { return true; }
		}

		/// <summary>
		/// This event never fires becauses <see cref="Visible"/> is not persistable.
		/// </summary>
		event EventHandler IAction.VisibleChanged
		{
			add { }
			remove { }
		}

		#region AbstractClickAction Class

		/// <summary>
		/// A view-less implementation of <see cref="IClickAction"/>.
		/// </summary>
		private class AbstractClickAction : AbstractAction, IClickAction
		{
			private XKeys _keyStroke;

			public AbstractClickAction(IClickAction concreteAction)
				: base(concreteAction)
			{
				_keyStroke = concreteAction.KeyStroke;
			}

			public AbstractClickAction(string id, string path)
				: base(id, path)
			{
				_keyStroke = XKeys.None;
			}

			public XKeys KeyStroke
			{
				get { return _keyStroke; }
				set { _keyStroke = value; }
			}

			/// <summary>
			/// This value is always dynamically tool-controlled and may not be overriden by the persisted action model.
			/// </summary>
			bool IClickAction.Checked
			{
				get { return false; }
			}

			/// <summary>
			/// This event never fires becauses <see cref="Checked"/> is not persistable.
			/// </summary>
			event EventHandler IClickAction.CheckedChanged
			{
				add { }
				remove { }
			}

			/// <summary>
			/// This value describes behaviour that is prescribed by the tool and may not be overriden by the persisted action model.
			/// </summary>
			bool IClickAction.IsCheckAction
			{
				get { return false; }
			}

			/// <summary>
			/// This value describes behaviour that is prescribed by the tool and may not be overriden by the persisted action model.
			/// </summary>
			bool IClickAction.CheckParents
			{
				get { return false; }
			}

			/// <summary>
			/// This method invokes behaviour that is performed by the tool and may not be overriden by the persisted action model.
			/// </summary>
			void IClickAction.Click() {}
		}

		#endregion
	}
}