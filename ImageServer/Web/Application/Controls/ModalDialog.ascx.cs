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
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ClearCanvas.ImageServer.Web.Application.Controls
{
    /// <summary>
    /// A generic modal popup dialog box.
    /// </summary>
    /// <remarks>
    /// A <see cref="ModalDialog"/> control provide all basic functionalities of a modal dialog box such as <see cref="Show"/> and <see cref="Hide"/>.
    /// The content of the dialog box can be specified at design time using the <see cref="ContentTemplate"/>. The title bar can also be customized by 
    /// changing <see cref="TitleBarTemplate"/>. The default appearance of the title bar has a <see cref="Title"/> and an "X" button for closing.
    /// <para>
    /// Note <see cref="ModalDialog"/> doesn't fire any events. Customized dialog box control should implement event handlers according to its requirement.
    /// </para> 
    /// 
    /// <example>
    /// The following example illustrate how to define a dialogbox with a "OK" button.
    /// 
    /// aspx code:
    /// 
    /// <%@ Register Src="ModalDialog.ascx" TagName="ModalDialog" TagPrefix="clearcanvas" %>
    /// 
    /// <clearcanvas:ModalDialog ID="ModalDialog1" runat="server" Title="Please Press the button">
    /// <ContentTemplate>
    ///      <asp:Button ID="YesButton" runat="server" OnClick="Button_Click" Text="Click Me" />
    /// </ContentTemplate>
    /// </clearcanvas:ModalDialog>
    /// 
    /// 
    /// C#:
    /// 
    ///  protected void Page_Load(object sender, EventArgs e)
    ///  {
    ///     ModalDialog1.Show();
    ///  }
    /// 
    ///  protected void Button_Click(object sender, EventArgs e)
    ///  {
    ///        // do something...
    ///        ModalDialog1.Close();
    ///  }
    /// 
    /// 
    /// 
    /// </example>
    ///
    /// Note that the ModalDialog control is declared globally in the Web.Config file, and is not
    /// required to be declared locally in an ASPX page. See Web.Config for information about the proper
    /// declaration of the tagPrefix.
    /// 
    /// </remarks>
    public partial class ModalDialog : UserControl
    {
        /// <summary>
        /// State enumeration
        /// </summary>
        public enum ShowState
        {
            Hide,
            Show
        }

        /// <summary>
        /// Template container classes
        /// </summary>
        [ParseChildren(true)]
        public class DialogTitleBarContainer : Panel, INamingContainer { }

        [ParseChildren(true)]
        public class DialogContentContainer : Panel, INamingContainer { }

        #region Private members
        private Dictionary<string, Control> _ctrlIDCache = new Dictionary<string, Control>();
        private Unit _width;
        private string _title;
        private string _titleBarCSS;
        private string _dialogContentCSS;
        private string _backgroundCSS = "DefaultModalDialogBackground";
        private bool _dropShadow = false;
        private bool _showCloseBox = false;
        private ITemplate _titleBarTemplate = null;
        private ITemplate _contentTemplate = null;
        private readonly DialogContentContainer _contentPanelContainer = new DialogContentContainer();
        private readonly DialogTitleBarContainer _titleBarPanelContainer = new DialogTitleBarContainer();
        #endregion Private members

        #region Public Properties
        /// <summary>
        /// The customized titlebar template
        /// </summary>
        [TemplateContainer(typeof(DialogTitleBarContainer))]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        [TemplateInstance(TemplateInstance.Single)]
        public ITemplate TitleBarTemplate
        {
            get
            {
                return _titleBarTemplate;
            }
            set
            {
                _titleBarTemplate = value;
            }
        }


        /// <summary>
        /// The content template
        /// </summary>
        [TemplateContainer(typeof(DialogContentContainer))]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        [TemplateInstance(TemplateInstance.Single)]
        public ITemplate ContentTemplate
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


        /// <summary>
        /// Sets/Gets the title of the dialog box
        /// </summary>
        public string Title
        {
            get {
                return _title;
            }
            set
            {
                _title = value;
                if (TitleLabel!=null)
                    TitleLabel.Text = String.IsNullOrEmpty(value) ? " " : value;
                
                
            }
        }

        /// <summary>
        /// Sets/Gets the size of the dialog box.
        /// </summary>
        public Unit Width
        {
            get { return _width; }
            set
            {
                _width = value;
            }
        }

        /// <summary>
        /// Sets/Gets the CSS for the title bar
        /// </summary>
        public string TitleBarCSS
        {
            get { return _titleBarCSS; }
            set
            {
                _titleBarCSS = value;

            }
        }

        /// <summary>
        /// Gets/Sets the CSS for the dialog box content
        /// </summary>
        public string ContentCSS
        {
            get { return _dialogContentCSS; }
            set
            {
                _dialogContentCSS = value;

            }
        }

        /// <summary>
        /// Gets/Sets a value to indicate whether or not to drop shadow
        /// </summary>
        public bool DropShadow
        {
            get { return _dropShadow; }
            set
            {
                _dropShadow = value;
            }
        }

        /// <summary>
        /// Sets/Gets the CSS for the background area
        /// </summary>
        public string BackgroundCSS
        {
            get { return _backgroundCSS; }
            set
            {
                _backgroundCSS = value;

            }
        }

        /// <summary>
        ///  Gets/Sets the current state of the dialog box.
        /// </summary>
        /// <remarks>
        /// The dialog box will popup automatically when the value of <see cref="State"/> is <see cref="ShowState.Show"/>
        /// </remarks>
        public ShowState State
        {
            get
            {
                if (ViewState[ClientID + "_State"] != null)
                    return (ShowState)ViewState[ClientID + "_State"];
                else
                    return ShowState.Hide;
            }
            set { ViewState[ClientID + "_State"] = value; }
           
        }

        /// <summary>
        /// Gets the container control that contains the content of the dialog box
        /// </summary>
        /// <remarks>
        /// The content control contains everything defined in <see cref="ContentTemplate"/>
        /// </remarks>
        public DialogContentContainer ContentPanelContainer
        {
            get { return _contentPanelContainer; }
        }

        /// <summary>
        /// Gets the container control that contains the content of the title bar
        /// </summary>
        /// <remarks>
        /// The title bar container contains everything defined in <see cref="TitleBarTemplate"/>
        /// </remarks>
        public DialogTitleBarContainer TitleBarPanelContainer
        {
            get { return _titleBarPanelContainer; }
        }

        public bool ShowCloseBox
        {
            get { return _showCloseBox; }
            set { _showCloseBox = value; }
        }

        public string PopupExtenderID
        {
            get { return ModalPopupExtender.ClientID; }
        }

        #endregion Public Properties

        #region Protected Methods
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DialogContainer.Width = Width;
            if (!String.IsNullOrEmpty(_title))
                TitleLabel.Text = Title;

            DialogContainer.Width = Width;

            if (DialogContainer.Width!=Unit.Empty)
            {
                // this will make sure the dialog box 
                //DialogSizeTable.Width = Unit.Percentage(100.0);
            }

            bool useCustomizeTitleBar = _titleBarTemplate != null;

            if (useCustomizeTitleBar)
            {
                CustomizedTitleBarPanel.Visible = true;
                DefaultTitlePanel.Visible = false;
                _titleBarTemplate.InstantiateIn(TitleBarPanelContainer);
                TitlePanelPlaceHolder.Controls.Add(TitleBarPanelContainer);

                if (!String.IsNullOrEmpty(TitleBarCSS))
                    CustomizedTitleBarPanel.CssClass = TitleBarCSS;

            }
            else
            {
                DefaultTitlePanel.Visible = true;
                CustomizedTitleBarPanel.Visible = false;

                if (!String.IsNullOrEmpty(TitleBarCSS))
                    DefaultTitlePanel.CssClass = TitleBarCSS;
            }

            if (_contentTemplate != null)
            {
                _contentTemplate.InstantiateIn(ContentPanelContainer);
                ContentPlaceHolder.Controls.Add(ContentPanelContainer);

                if (!String.IsNullOrEmpty(ContentCSS))
                    ContentPanel.CssClass = ContentCSS;
            }

            ModalPopupExtender.BackgroundCssClass = BackgroundCSS;
            ModalPopupExtender.DropShadow = DropShadow;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (State == ShowState.Show)
                Show();
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (String.IsNullOrEmpty(Title))
                Title = "&nbsp;";
        }

        protected Control FindControlHelper(string id)
        {
            Control ctrl = null;
            if (_ctrlIDCache.ContainsKey(id))
            {
                ctrl = _ctrlIDCache[id];
            }
            else
            {
                ctrl = base.FindControl(id);
                Control nc = NamingContainer;
                while ((ctrl == null) && (nc != null))
                {
                    ctrl = nc.FindControl(id);
                    nc = nc.NamingContainer;
                }

                // search inside the template containers
                if (ctrl == null)
                    ctrl = ContentPanelContainer.FindControl(id);

                if (ctrl == null)
                    ctrl = TitleBarPanelContainer.FindControl(id);


                if (null != ctrl)
                {
                    _ctrlIDCache[id] = ctrl;
                }
            }
            return ctrl;
        }

        protected void CloseButton_Click(object sender, ImageClickEventArgs e)
        {
            Hide();
        }

        #endregion Protected Methods

        #region Public Methods
        public override Control FindControl(string id)
        {
            return FindControlHelper(id);
        }

        /// <summary>
        /// Displays the dialog on the screen and wait for response from the users
        /// </summary>
        public void Show()
        {
            ModalPopupExtender.Show();
            
            if (State==ShowState.Hide)
            {
                // need refresh
                RefreshUI();
            }

            State = ShowState.Show;
        }

        public void RefreshUI()
        {
            ModalDialogUpdatePanel.Update();
            
        }

        /// <summary>
        /// Closes the dialog box
        /// </summary>
        public void Hide()
        {
            ModalPopupExtender.Hide();
            RefreshUI();
            State = ShowState.Hide;
        }

        #endregion Public Methods
    }
}