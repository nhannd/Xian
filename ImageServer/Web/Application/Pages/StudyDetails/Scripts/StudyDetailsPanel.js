/////////////////////////////////////////////////////////////////////////////////////////////////////////
///
/// This script contains the javascript component class for the study search panel
/// 
/////////////////////////////////////////////////////////////////////////////////////////////////////////

// Define and register the control type.
//
// Only define and register the type if it doens't exist. Otherwise "... does not derive from Sys.Component" error 
// will show up if multiple instance of the controls must be created. The error is misleading. It looks like the type 
// is RE-define for the 2nd instance but registerClass() will fail so the type will be essential undefined when the object
// is instantiated.
//
if (window.__registeredTypes['ClearCanvas.ImageServer.Web.Application.Pages.StudyDetails.Controls.StudyDetailsPanel']==null)
{
    Type.registerNamespace('ClearCanvas.ImageServer.Web.Application.Pages.StudyDetails.Controls');

    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Constructor
    //
    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    ClearCanvas.ImageServer.Web.Application.Pages.StudyDetails.Controls.StudyDetailsPanel = function(element) { 
        ClearCanvas.ImageServer.Web.Application.Pages.StudyDetails.Controls.StudyDetailsPanel.initializeBase(this, [element]);
       
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Create the prototype for the control.
    //
    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    ClearCanvas.ImageServer.Web.Application.Pages.StudyDetails.Controls.StudyDetailsPanel.prototype = 
    {
        initialize : function() {
            ClearCanvas.ImageServer.Web.Application.Pages.StudyDetails.Controls.StudyDetailsPanel.callBaseMethod(this, 'initialize');        
            
            this._OnLoadHandler = Function.createDelegate(this,this._OnLoad);
            this._OnSeriesListClickedHandler = Function.createDelegate(this,this._OnSeriesListClicked);
            this._OnViewSeriesButtonClickedHandler = Function.createDelegate(this,this._OnViewSeriesButtonClicked);
            
            Sys.Application.add_load(this._OnLoadHandler);
                 
        },
        
        dispose : function() {
            $clearHandlers(this.get_element());

            ClearCanvas.ImageServer.Web.Application.Pages.StudyDetails.Controls.StudyDetailsPanel.callBaseMethod(this, 'dispose');
            
            var serieslist = $find(this._SeriesListClientID);
            if (serieslist!=null)
            {
                serieslist.remove_onClientRowClick(this._OnSeriesListClickedHandler);
            }
            
            var viewSeriesBtn = $find(this._ViewSeriesButtonClientID);
            if (viewSeriesBtn!=null)
            {
                viewSeriesBtn.remove_onClientClick(this._OnViewSeriesButtonClickedHandler);
            }
            
            Sys.Application.remove_load(this._OnLoadHandler);
        },
        
        
        /////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        // Events
        //
        /////////////////////////////////////////////////////////////////////////////////////////////////////////
              
        /// called whenever the page is reloaded or partially reloaded
        _OnLoad : function()
        {
            var serieslist = $find(this._SeriesListClientID);
            if (serieslist!=null)
            {
                serieslist.add_onClientRowClick(this._OnSeriesListClickedHandler);
                // serieslist.add_onClientRowDblClick(this._OnSeriesListDoubleClickedHandler);
            }
            
            var viewSeriesBtn = $find(this._ViewSeriesButtonClientID);
            if (viewSeriesBtn!=null)
            {
                viewSeriesBtn.add_onClientClick(this._OnViewSeriesButtonClickedHandler);
            }
        },
        
        // called when the user clicks on the series list
        _OnSeriesListClicked : function(src, event)
        {
            var serieslist = $find(this._SeriesListClientID);
            if (serieslist!=null)
            {
                var rows = serieslist.getSelectedRowElements();
                var viewSeriesBtn = $find(this._ViewSeriesButtonClientID);
                if (viewSeriesBtn!=null) {
                    viewSeriesBtn.set_enable(rows.length>0);
                }
            }
        },
        
        _OnViewSeriesButtonClicked : function()
        {
            var serieslist = $find(this._SeriesListClientID);
            if (serieslist!=null)
            {
                var rows = serieslist.getSelectedRowElements();
                for(i=0;i<rows.length;i++)
                {
                    var url = String.format("{0}?serverae={1}&studyuid={2}&seriesuid={3}", 
                           this._OpenSeriesPageUrl,
                           this._getServerAE(rows[i]),
                           this._getStudyUid(rows[i]),
                           this._getSeriesUid(rows[i]));
                           
                    window.open(url);
                }
            }
        },
                
        /////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        // Private Methods
        //
        /////////////////////////////////////////////////////////////////////////////////////////////////////////
             
        _getServerAE:function(row)
        {
            return row.getAttribute("serverae");
        },
        
        _getStudyUid:function(row)
        {
            return row.getAttribute("studyuid");
        },
        
        _getSeriesUid:function(row)
        {
            return row.getAttribute("seriesuid");
        },
        
        /////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        // Public methods
        //
        /////////////////////////////////////////////////////////////////////////////////////////////////////////
        
        

        /////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        // Properties
        //
        /////////////////////////////////////////////////////////////////////////////////////////////////////////
        
        get_SeriesListClientID : function() {
            return this._SeriesListClientID;
        },

        set_SeriesListClientID: function(value) {
            this._SeriesListClientID = value;
            this.raisePropertyChanged('SeriesListClientID');
        },
        
        get_OpenSeriesPageUrl : function() {
            return this._OpenSeriesPageUrl;
        },

        set_OpenSeriesPageUrl: function(value) {
            this._OpenSeriesPageUrl = value;
            this.raisePropertyChanged('OpenSeriesPageUrl');
        },
        
        get_ViewSeriesButtonClientID : function() {
            return this._ViewSeriesButtonClientID;
        },
       
        set_ViewSeriesButtonClientID : function(value) {
            this._ViewSeriesButtonClientID = value;
            this.raisePropertyChanged('ViewSeriesButtonClientID');
        },
        
        get_PatientBirthDateClientID : function() {
            return this._PatientBirthDateClientID;
        }

    }

    // Register the class as a type that inherits from Sys.UI.Control.

    ClearCanvas.ImageServer.Web.Application.Pages.StudyDetails.Controls.StudyDetailsPanel.registerClass('ClearCanvas.ImageServer.Web.Application.Pages.StudyDetails.Controls.StudyDetailsPanel', Sys.UI.Control);
     

    if (typeof(Sys) !== 'undefined') Sys.Application.notifyScriptLoaded();

}

//When the BirthDate Changes, set the Age and change the textbox.

/*function BirthDate_Changed()
{
    birthDateTextBox = get_PatientBrithDateClientID());
    
    if (birthDateTextBox != null && birthDateTextBox != "")
    {
        alert(birthDateTextBox.value);
    }
}
*/
    /*
    if (!string.IsNullOrEmpty(study.PatientsBirthDate))
            {
                DateTime birthDate = DateTime.ParseExact(study.StudyDate, ImageServerConstants.DicomDate, null);

                PatientBirthDate.Text = birthDate.ToString(ImageServerConstants.MMDDYYY);

                TimeSpan age = DateTime.Now - birthDate;
                if (age > TimeSpan.FromDays(365))
                {
                    PatientAge.Text = String.Format("{0:0}", age.TotalDays / 365);
                }
                else if (age > TimeSpan.FromDays(30))
                {
                    PatientAge.Text = String.Format("{0:0} month(s)", age.TotalDays / 30);
                }
                else
                {
                    PatientAge.Text = String.Format("{0:0} day(s)", age.TotalDays);
                }
            }
            else
            {
                PatientBirthDate.Text = App_GlobalResources.SR.Unknown;
                PatientAge.Text = App_GlobalResources.SR.Unknown;
            }
            */