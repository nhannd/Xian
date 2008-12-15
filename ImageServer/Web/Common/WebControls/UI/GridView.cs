#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;

[assembly: WebResource("ClearCanvas.ImageServer.Web.Common.WebControls.UI.GridView.js", "text/javascript")]

namespace ClearCanvas.ImageServer.Web.Common.WebControls.UI
{
    /// <summary>
    /// Custom ASP.NET AJAX grid view control.
    /// </summary>
    /// <remarks>
    /// This custom grid view control offers a couple of improvements over the standard ASP.NET GridView:
    /// multiple selection and client-side item selection (ie. not postback to the server). 
    /// </remarks>
    [ToolboxData("<{0}:GridView runat=server></{0}:GridView>")]
    public class GridView : System.Web.UI.WebControls.GridView , IScriptControl
    {
        public enum SelectionModeEnum
        {
            Disabled,
            Single,
            Multiple
        }

        #region Private members
        private string _onClientRowClick;
        private string _onClientRowDblClick;
        private SelectionModeEnum _selectionMode = SelectionModeEnum.Single;
        private Hashtable _selectedRows = new Hashtable();
        #endregion Private members

        #region Public Properties
        /// <summary>
        /// Sets or gets the selection mode.
        /// </summary>
        /// <remark>
        /// </remark>
        public SelectionModeEnum SelectionMode
        {
            get { return _selectionMode; }
            set { _selectionMode = value; }
        }


        /// <summary>
        /// Sets or gets the client-side script function that will be called on the client when a row in selected.
        /// </summary>
        /// <remark>
        /// The row will be selected before the specified function is called.
        /// </remark>
        public string OnClientRowClick
        {
            get { return _onClientRowClick; }
            set { _onClientRowClick = value; }
        }

        /// <summary>
        /// Sets or gets the client-side script function name that will be called in the client browser when users double-click on a row.
        /// </summary>
        /// <remark>
        /// 
        /// </remark>
        public string OnClientRowDblClick
        {
            get { return _onClientRowDblClick; }
            set { _onClientRowDblClick = value; }
        }

        /// <summary>
        /// Sets or gets the Mouseover highlighting enabled/disabled.
        /// </summary>
        /// <remark>
        /// 
        /// </remark>
        public bool MouseHoverRowHighlightEnabled
        {
            get
            {
                if (ViewState["MouseHoverRowHighlightEnabled"] != null)
                    return (bool)ViewState["MouseHoverRowHighlightEnabled"];
                else
                    return true;
            }
            set { ViewState["MouseHoverRowHighlightEnabled"] = value; }
        }

        /// <summary>
        /// Sets or gets the Highlight color of a row  
        /// </summary>
        /// <remark>
        /// 
        /// </remark>
        public Color RowHighlightColor
        {
            get
            {
                if (ViewState["RowHighlightColor"] != null)
                    return (Color)ViewState["RowHighlightColor"];
                else
                {
                    // default color
                    return Color.FromArgb(238,238,238);
                }
            }

            set { ViewState["RowHighlightColor"] = value; }
        }


        /// <summary>
        /// Gets the index of the first selected item.
        /// </summary>
        /// <remarsk>
        /// Use <see cref="SelectedIndices"/> instead.
        /// </remarsk>
        public override int SelectedIndex
        {
            get
            {
                int[] indices = SelectedIndices;
                if (indices == null || indices.Length == 0)
                    return -1;

                return indices[0];
            }
        }

        /// <summary>
        /// Gets the indices of the rows being selected
        /// </summary>
        /// <remarks>
        /// <see cref="SelectedIndices"/>=<b>null</b> or <see cref="SelectedIndices"/>.Length=0 if no row is being selected.
        /// </remarks>
        public int[] SelectedIndices
        {
            get
            {
                if (_selectedRows.Keys.Count == 0)
                    return null;

                int[] rows = new int[_selectedRows.Keys.Count];
                int i = 0;
                foreach (int r in _selectedRows.Keys)
                    rows[i++] = r;

                return rows;
            }
        }

        #endregion Public Properties


        #region Public Methods

        public override void RenderControl(HtmlTextWriter writer)
        {
            SaveState();
            base.RenderControl(writer);
        }

        protected virtual void SaveState()
        {
            string stateValue = "";
            JavaScriptSerializer serializer = new JavaScriptSerializer();

            int[] rows = SelectedIndices;
            if (rows != null)
                stateValue = serializer.Serialize(rows);

            Page.ClientScript.RegisterHiddenField(ClientID + "SelectedRowIndices", stateValue);
        }

        protected virtual void LoadState()
        {
            string statesValue = Page.Request[ClientID + "SelectedRowIndices"];
            if (statesValue != null && statesValue != "")
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                int[] rows = serializer.Deserialize<int[]>(statesValue);
                foreach (int r in rows)
                {
                    _selectedRows[r] = true;
                }

            }
        }


        /// <summary>
        /// Clear the current selections.
        /// </summary>
        public void ClearSelections()
        {
            int[] rows = SelectedIndices;
            if (rows != null && Rows != null)
            {
               if (Rows.Count > 0)
               {
                        for (int i = 0; i < rows.Length; i++)
                        {
                            int rowIndex = rows[i];
							if (rowIndex < Rows.Count)
							{
								Rows[rowIndex].RowState = DataControlRowState.Normal;
								Rows[rowIndex].Attributes["selected"] = "false";
							}
                        }
                    }
            }

            _selectedRows = new Hashtable();

        }

        #endregion Public Methods


        
        #region IScriptControl Members

        
        public IEnumerable<ScriptDescriptor> GetScriptDescriptors()
        {
            ScriptControlDescriptor desc = new ScriptControlDescriptor(GetType().FullName, ClientID);

            desc.AddProperty("clientStateFieldID", ClientID + "SelectedRowIndices");
            desc.AddProperty("SelectionMode", SelectionMode.ToString());

            string style = StyleToString(SelectedRowStyle);
            desc.AddProperty("SelectedRowStyle", style);
            desc.AddProperty("SelectedRowCSS", SelectedRowStyle.CssClass);

            style = StyleToString(RowStyle);
            desc.AddProperty("UnSelectedRowStyle", style);
            desc.AddProperty("UnSelectedRowCSS", RowStyle.CssClass);


            if (AlternatingRowStyle!=null)
            {
                style = StyleToString(AlternatingRowStyle);
                desc.AddProperty("AlternatingRowStyle", style);
                desc.AddProperty("AlternatingRowCSS", AlternatingRowStyle.CssClass); 
            }
            else
            {
                // use the normal row style
                style = StyleToString(RowStyle);
                desc.AddProperty("UnSelectedRowStyle", style);
                desc.AddProperty("UnSelectedRowCSS", RowStyle.CssClass);

            }
            
            if (OnClientRowClick!=null)
                desc.AddEvent("onClientRowClick", OnClientRowClick);

            if (OnClientRowDblClick!=null)
                desc.AddEvent("onClientRowDblClick", OnClientRowDblClick);
            
            return new ScriptDescriptor[] { desc };
        }

        public IEnumerable<ScriptReference> GetScriptReferences()
        {
            ScriptReference reference = new ScriptReference();

            reference.Path = Page.ClientScript.GetWebResourceUrl(GetType(), "ClearCanvas.ImageServer.Web.Common.WebControls.UI.GridView.js");
            return new ScriptReference[] { reference };
        }

        #endregion IScriptControl Members


        #region Protected Methods

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            
            foreach(GridViewRow row in Rows)
            {
                if (row.RowType==DataControlRowType.DataRow)
                {
                    // the following lines will disable text select on the row.
                    // We need to disable it because IE and firefox interpret Ctrl-Click as text selection and will display
                    // a box around the cell users click on.

                    // Firefox way:
                    row.Attributes["onmousedown"] = "if (event.ctrlKey && typeof (event.preventDefault) != 'undefined') {event.preventDefault();}";
                    // IE way:
                    row.Attributes["onselectstart"] = "return false;"; // disable select
                    
                    row.Attributes["isdatarow"] = "true";
                    row.Attributes["rowIndex"] = (row.RowIndex).ToString();

                    if (_selectedRows.ContainsKey(row.RowIndex))
                    {
                        row.RowState = DataControlRowState.Selected;
                        row.Attributes["selected"] = "true";

                    }
                    else
                    {
                        row.Attributes["selected"] = "false";
                    }

                }
            }
            
            if (!DesignMode)
            {
                ScriptManager sm = ScriptManager.GetCurrent(Page);
                sm.RegisterScriptControl(this);    
            }
            
        }

        protected override void Render(HtmlTextWriter writer)
        {
            if (!DesignMode)
            {
                ScriptManager sm = ScriptManager.GetCurrent(Page);
                sm.RegisterScriptDescriptors(this);
            }
            base.Render(writer);
        }

        protected override void OnSelectedIndexChanging(GridViewSelectEventArgs e)
        {

            base.OnSelectedIndexChanging(e);

            int rowIndex = e.NewSelectedIndex;

            SelectRow(rowIndex);
        
        }
        
        public void SelectRow(int rowIndex)
        {
            Rows[rowIndex].RowState = DataControlRowState.Selected;
            Rows[rowIndex].Attributes["selected"] = "true";

            
             
            _selectedRows[rowIndex] = true;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
           
            if (!DesignMode)
            {
                if (Page.IsPostBack)
                {
                    LoadState();
                }
                else if (SelectedIndex>=0)
                {
                    _selectedRows[SelectedIndex] = true;
                }

                PagerSettings.Visible = false;
            }
                    
        }

        protected override void OnRowCreated(GridViewRowEventArgs e)
        {
            base.OnRowCreated(e);

            if (e.Row.RowType == DataControlRowType.DataRow && MouseHoverRowHighlightEnabled)
            {
                string onMouseOver = string.Format("this.style.cursor='pointer';this.originalstyle=this.style.backgroundColor;this.style.backgroundColor='{0}';", ColorTranslator.ToHtml(RowHighlightColor));
                string onMouseOut = "this.style.backgroundColor=this.originalstyle;";
                e.Row.Attributes.Add("onmouseover", onMouseOver);
                e.Row.Attributes.Add("onmouseout", onMouseOut);
            }
        }

        public void PageIndexChangedHandler(object sender, EventArgs e)
        {
            DataBind();
        }

        public void PageIndexChangingHandler(object sender, GridViewPageEventArgs e)
        {
            PageIndex = e.NewPageIndex;
            DataBind();
        }
        
        #endregion Protected Methods

        static string StyleToString(Style s)
        {
            StringBuilder sb = new StringBuilder();
            
            if (s.BackColor != Color.Empty)
                sb.Append("background-color:"
                   + ColorTranslator.ToHtml(s.BackColor) + ";");
            if (s.ForeColor != Color.Empty)
                sb.Append("color:"
                   + ColorTranslator.ToHtml(s.ForeColor) + ";");

            #region -- Border --
            string color = "black";
            string width = "";
            string style = "solid";
            if (s.BorderColor != Color.Empty)
                color = ColorTranslator.ToHtml(s.BorderColor);
            if (s.BorderStyle != BorderStyle.NotSet)
                style = s.BorderStyle.ToString();
            if (s.BorderWidth != Unit.Empty)
                width = s.BorderWidth.ToString();
            if (color != "" && width != "" && style != "")
                sb.Append("border:" + color + " " + width + " " + style + ";");
            #endregion

            #region -- Font --

            #region -- Font General --
            if (s.Font.Size != FontUnit.Empty)
                sb.Append("font-size:" + s.Font.Size + ";");
            if (s.Font.Bold == true)
                sb.Append("font-weight:Bold;");
            if (s.Font.Italic == true)
                sb.Append("font-style:Italic;");
            #endregion

            #region -- Font Names --
            ArrayList fontList = new ArrayList();
            if (s.Font.Name.Length != 0)
                fontList.Add(s.Font.Name);
            foreach (string f in s.Font.Names)
                fontList.Add(f);
            if (fontList.Count > 0)
            {
                string fontString = "";
                for (int i = 0; i < fontList.Count; i++)
                {
                    if (i == 0)
                        fontString = (string)fontList[i];
                    else
                        fontString += ", " + fontList[i];
                }
                sb.Append("font-family:" + fontString + ";");
            }
            #endregion

            #region - Text Decoration --
            ArrayList decorList = new ArrayList();
            if (s.Font.Underline == true)
                decorList.Add("underline");
            if (s.Font.Overline == true)
                decorList.Add("overline");
            if (s.Font.Strikeout == true)
                decorList.Add("line-through");
            if (decorList.Count > 0)
            {
                string strDecor = "";
                for (int i = 0; i < decorList.Count; i++)
                {
                    if (i == 0)
                        strDecor = (string)decorList[i];
                    else
                        strDecor += ", " + decorList[i];
                }
                sb.Append("text-decoration:" + strDecor + ";");
            }
            #endregion

            #endregion

            #region -- Height and Width --
            if (!s.Height.IsEmpty)
                sb.Append("height:" + s.Height.ToString() + ";");
            if (!s.Width.IsEmpty)
                sb.Append("width:" + s.Width.ToString() + ";");
            #endregion

            return sb.ToString();

        }
    }
}
