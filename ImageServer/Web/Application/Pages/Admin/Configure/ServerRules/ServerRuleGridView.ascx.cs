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
using System.Web.UI;
using System.Web.UI.WebControls;
using ClearCanvas.ImageServer.Model;
using GridView = ClearCanvas.ImageServer.Web.Common.WebControls.UI.GridView;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.ServerRules
{
	public partial class ServerRuleGridView : UserControl
	{
		#region private members
		// list of devices to display
		private IList<ServerRule> _serverRules;
		private ServerRulePanel _serverRulePanel;
		private Unit _height;
		#endregion Private members

     
		#region public properties

		/// <summary>
		/// Gets/Sets the server rule panel
		/// </summary>
		public ServerRulePanel ServerRulePanel
		{
			get { return _serverRulePanel; }
			set { _serverRulePanel = value; }
		}

		/// <summary>
		/// Gets a reference to the server rule list grid control
		/// </summary>
		public GridView TheGrid
		{
			get { return this.GridView; }
		}

		/// <summary>
		/// Gets/Sets the height of the server rule list panel
		/// </summary>
		public Unit Height
		{
			get
			{
				if (ContainerTable != null)
					return ContainerTable.Height;
				else
					return _height;
			}
			set
			{
				_height = value;
				if (ContainerTable != null)
					ContainerTable.Height = value;
			}
		}

		/// <summary>
		/// Gets/Sets the current selected device.
		/// </summary>
		public ServerRule SelectedRule
		{
			get
			{
				if (ServerRules==null || ServerRules.Count == 0 || GridView.SelectedIndex < 0)
					return null;

				// SelectedIndex is for the current page. Must convert to the index of the entire list
				int index = GridView.PageIndex * GridView.PageSize + GridView.SelectedIndex;

				if (index < 0 || index > ServerRules.Count - 1)
					return null;

				return ServerRules[index];
			}
			set
			{

				GridView.SelectedIndex = ServerRules.IndexOf(value);
				//  if (OnStudySelectionChanged != null)
				//     OnDeviceSelectionChanged(this, value);
			}
		}

		/// <summary>
		/// Gets/Sets the list of devices rendered on the screen.
		/// </summary>
		public IList<ServerRule> ServerRules
		{
			get
			{
				return _serverRules;
			}
			set
			{
				_serverRules = value;
				GridView.DataSource = _serverRules; // must manually call DataBind() later
			}
		}
		#endregion

		#region Protected Methods

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			if (Height != Unit.Empty)
				ContainerTable.Height = _height;
		}

		protected void GridView_RowDataBound(object sender, GridViewRowEventArgs e)
		{
			if (GridView.EditIndex != e.Row.RowIndex)
			{
				if (e.Row.RowType == DataControlRowType.DataRow)
				{
					CustomizeColumns(e);
				}
			}
		}
		protected void CustomizeColumns(GridViewRowEventArgs e)
		{
			ServerRule fs = e.Row.DataItem as ServerRule;
			Label lbl = e.Row.FindControl("ServerRuleApplyTimeEnum") as Label; // The label is added in the template
			lbl.Text = fs.ServerRuleApplyTimeEnum.Description;

			lbl = e.Row.FindControl("ServerRuleTypeEnum") as Label; // The label is added in the template
			lbl.Text = fs.ServerRuleTypeEnum.Description;

      
			Image img = ((Image)e.Row.FindControl("EnabledImage"));
			if (img != null)
			{
				if (fs.Enabled)
                {
                    img.ImageUrl = ImageServerConstants.ImageURLs.Checked;
                }
                else
                {
                    img.ImageUrl = ImageServerConstants.ImageURLs.Unchecked;
                }
			}

			img = ((Image)e.Row.FindControl("DefaultImage"));
			if (img != null)
			{
				if (fs.DefaultRule)
                {
                    img.ImageUrl = ImageServerConstants.ImageURLs.Checked;
                }
                else
                {
                    img.ImageUrl = ImageServerConstants.ImageURLs.Unchecked;
                }
			}
			img = ((Image)e.Row.FindControl("ExemptImage"));
			if (img != null)
			{
				if (fs.ExemptRule)
				{
					img.ImageUrl = ImageServerConstants.ImageURLs.Checked;
				}
				else
				{
                    img.ImageUrl = ImageServerConstants.ImageURLs.Unchecked;
				}
			}
		}

		protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
		{
			GridView.PageIndex = e.NewPageIndex;
			DataBind();
		}

		protected void GridView_DataBound(object sender, EventArgs e)
		{

		}

		#endregion Protected Methods


		#region public methods
		/// <summary>
		/// Binds the list to the control.
		/// </summary>
		/// <remarks>
		/// This method must be called after setting <seeaslo cref="Devices"/> to update the grid with the list.
		/// </remarks>
		public override void DataBind()
		{
			GridView.DataBind();
		}

		#endregion // public methods

  
	}
}