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
            get { return ResultsMessage.Text;}
            set{ ResultsMessage.Text = value;}
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            if (_suggestionTemplate != null)
            {
                _suggestionTemplate.InstantiateIn(_suggestionPanelContainer);
                SuggestionPlaceHolder.Controls.Add(_suggestionPanelContainer);
            }

            SuggestionPanel.Visible = _suggestionTemplate != null;
        }


    }
}