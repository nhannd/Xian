using System;
using System.Collections.Generic;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using AjaxControlToolkit;

namespace ClearCanvas.ImageServer.Web.Application.Common
{
    
    /// <summary>
    /// A generic modal popup dialog box
    /// </summary>
    /// <remarks>
    /// A <see cref="ModalDialog"/> control provide a generic  has a <see cref="TitleBarTemplate"/> and a <see cref="ContentTemplate"/> sections. The content of the dialog can
    /// be 
    /// </remarks>
    public partial class ModalDialog : System.Web.UI.UserControl
    {
        public enum ShowState
        {
            Hide,
            Show
        }
        [ParseChildren(true)]
        public class DialogTitleBarContainer : Panel, INamingContainer { }
        
        [ParseChildren(true)]
        public class DialogContentContainer : Panel, INamingContainer { }

        private Dictionary<string, Control> _ctrlIDCache = new Dictionary<string, Control>();
        
        /// <summary>
        /// The template container class
        /// </summary>
        private Unit _width;
        private string _title;
        private string _titleBarCSS;
        private string _dialogContentCSS;
        private string _backgroundCSS;
        private bool _dropShadow;
        private ITemplate _titleBarTemplate = null;
        private ITemplate _contentTemplate = null;
        private DialogContentContainer _contentPanelContainer = new DialogContentContainer();
        private DialogTitleBarContainer _titleBarPanelContainer = new DialogTitleBarContainer();
        
       
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



        public Unit Width
        {
            get { return _width; }
            set
            {
                _width = value;
            }
        }


        public string TitleBarCSS
        {
            get { return _titleBarCSS; }
            set
            {
                _titleBarCSS = value;
                
            }
        }

        public string ContentCSS
        {
            get { return _dialogContentCSS; }
            set
            {
                _dialogContentCSS = value;
                
            }
        }

        public bool DropShadow
        {
            get { return _dropShadow; }
            set { 
                _dropShadow = value;
            }
        }

        public string BackgroundCSS
        {
            get { return _backgroundCSS; }
            set { 
                _backgroundCSS = value;
        
            }
        }

        /// <summary>
        /// The template container class
        /// </summary>
        public string Title
        {
            get { return _title; }
            set { _title = value; 
                if (TitleLabel!=null)
                    TitleLabel.Text = value;
            }
        }

        public DialogContentContainer ContentPanelContainer
        {
            get { return _contentPanelContainer; }
            set { _contentPanelContainer = value; }
        }

        public DialogTitleBarContainer TitleBarPanelContainer
        {
            get { return _titleBarPanelContainer; }
            set { _titleBarPanelContainer = value; }
        }

        public ShowState State
        {
            get {
                if (ViewState[ClientID + "_State"] != null)
                    return (ShowState)ViewState[ClientID + "_State"];
                else
                    return ShowState.Hide;
            }
            set { ViewState[ClientID+"_State"] = value; }
        }


        
        protected Control FindControlHelper(string id)
        {
            Control c = null;
            if (_ctrlIDCache.ContainsKey(id))
            {
                c = _ctrlIDCache[id];
            }
            else
            {
                c = base.FindControl(id);  // Use "base." to avoid calling self in an infinite loop
                Control nc = NamingContainer;
                while ((null == c) && (null != nc))
                {
                    c = nc.FindControl(id);
                    nc = nc.NamingContainer;
                }

                // search inside the template containers
                if (c == null)
                    c = ContentPanelContainer.FindControl(id);

                if (c==null)
                    c = TitleBarPanelContainer.FindControl(id);

                
                if (null != c)
                {
                    _ctrlIDCache[id] = c;
                }
            }
            return c;
        }

        public override Control FindControl(string id)
        {
            return FindControlHelper(id);
        }


        #region Protected Methods
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DialogContainer.Width = Width;
            if (!String.IsNullOrEmpty(Title))
                TitleLabel.Text = Title;


            Container.Width = Width;

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

            ModalPopupExtender1.BackgroundCssClass = BackgroundCSS;
            ModalPopupExtender1.DropShadow = DropShadow;
           
            if (CloseButton.Visible)
            {
                ModalPopupExtender1.CancelControlID = CloseButton.ClientID;
                CloseButton.Click += new ImageClickEventHandler(CloseButton_Click);
            }
            
            
            
        }

        void CloseButton_Click(object sender, ImageClickEventArgs e)
        {
            Hide();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

        #endregion Protected Methods

        public void Show()
        {
            ModalPopupExtender1.Show();
            UpdatePanel1.Update();

            State = ShowState.Show;
        }

        public void Hide()
        {
            ModalPopupExtender1.Hide();
            UpdatePanel1.Update();
            State = ShowState.Hide;
        }
    }
}