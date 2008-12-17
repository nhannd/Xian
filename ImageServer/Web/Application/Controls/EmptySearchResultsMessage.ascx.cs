using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ClearCanvas.ImageServer.Web.Application.Controls
{
    
    public partial class EmptySearchResultsMessage : System.Web.UI.UserControl
    {
        [ParseChildren(true)]
        public class SuggestionPanelContainer: Panel, INamingContainer
        {
        }
        
        private string _message = String.Empty;
        private readonly SuggestionPanelContainer _suggestionPanelContainer = new SuggestionPanelContainer();
        private ITemplate _suggestionTemplate = null;


        [TemplateContainer(typeof(SuggestionPanelContainer))]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        [TemplateInstance(TemplateInstance.Single)]
        public ITemplate SuggestionTemplate
        {
            get
            {
                return _suggestionTemplate;
            }
            set
            {
                _suggestionTemplate = value;
            }
        }

        public string Message
        { 
            get { return _message;}
            set{ _message = value;}
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            ResultsMessage.Text = Message;

            if (_suggestionTemplate != null)
            {
                _suggestionTemplate.InstantiateIn(_suggestionPanelContainer);
                SuggestionPlaceHolder.Controls.Add(_suggestionPanelContainer);
            }

            SuggestionPanel.Visible = _suggestionTemplate != null;
        }


    }
}