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
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ClearCanvas.ImageServer.Web.Application.Controls
{
    /// <summary>
    /// Information Section Panel.
    /// </summary>
    /// <remarks>
    /// A <see cref="SectionPanel"/> is a panel with heading on top. The heading text and style can be set via <see cref="HeadingText"/> and <see cref="HeadingCSS"/>. 
    /// The content of the section is specified in <see cref="SectionContentTemplate"/>. The controls in the content area can be accessed
    /// using <see cref="SectionContentContainer"/> FindControls() method.
    /// 
    /// <example>
    /// 
    /// <code>
    /// <%@ Register Src="~/Common/Controls/SectionPanel.ascx" TagName="SectionPanel" TagPrefix="uc4" %>
    /// ...
    /// <uc4:SectionPanel ID="StudySectionPanel" runat="server" HeadingText="Button Section" HeadingCSS="CSSStudyHeading"
    ///    Width="100%" CssClass="CSSSection">
    ///    <SectionContentTemplate>
    ///        <asp:Button ID="Button1" Text="Click here"/>
    ///    </SectionContentTemplate>
    ///</uc4:SectionPanel>
    /// 
    /// ....
    /// Button b = (Button) StudySectionPanel.SectionContentContainer.FindControl("Button1");
    /// 
    /// 
    /// </code>
    /// </example>
    /// </remarks>
    public partial class SectionPanel : UserControl
    {
        /// <summary>
        /// The template container class
        /// </summary>
        [ParseChildren(true)]
        private class SectionContentContainer : Control, INamingContainer
        {

        }

        #region Private Members
        private Unit _width;
        private string _cssClass;
    	private ITemplate _contentTemplate = null;
        private readonly SectionContentContainer _contentContainer = new SectionContentContainer();
        #endregion Private Members

        #region Public Properties
        [TemplateContainer(typeof(SectionContentContainer))]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        [TemplateInstance(TemplateInstance.Single)]
        public ITemplate SectionContentTemplate
        {
            get
            {
                return _contentTemplate;
            }
            set
            {
                _contentTemplate = value;
            }
        }

        public Unit Width
        {
            get { return _width; }
            set { 
                _width = value;
                Container.Width = value;
            }
        }

        public string CssClass
        {
            get { return _cssClass; }
            set
            {
                _cssClass = value;
                Container.CssClass = value;
            }
        }

        public override Control FindControl(string id)
        {
            Control ctrl = base.FindControl(id);
            if (ctrl == null)
                ctrl = _contentContainer.FindControl(id);

            return ctrl;
        }

        #endregion Public Properties

        #region Protected Methods
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            _contentContainer.ID = "SectionContentContainer";
            _contentTemplate.InstantiateIn(_contentContainer);
            placeholder.Controls.Add(_contentContainer);
        }

        #endregion Protected Methods

    }
}