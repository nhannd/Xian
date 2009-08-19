/////////////////////////////////////////////////////////////////////////////////////////////////////////
///
/// This script contains the javascript component class for the work queue search panel
/// 
/////////////////////////////////////////////////////////////////////////////////////////////////////////

// Define and register the control type.
//
// Only define and register the type if it doens't exist. Otherwise "... does not derive from Sys.Component" error 
// will show up if multiple instance of the controls must be created. The error is misleading. It looks like the type 
// is RE-define for the 2nd instance but registerClass() will fail so the type will be essential undefined when the object
// is instantiated.
//
if (window.__registeredTypes['ClearCanvas.ImageServer.Web.Application.Pages.Queues.WorkQueue.Edit.WorkQueueItemDetailsPanel']==null)
{
    Type.registerNamespace('ClearCanvas.ImageServer.Web.Application.Pages.Queues.WorkQueue.Edit');

    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Constructor
    //
    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    ClearCanvas.ImageServer.Web.Application.Pages.Queues.WorkQueue.Edit.WorkQueueItemDetailsPanel = function(element) { 
        ClearCanvas.ImageServer.Web.Application.Pages.Queues.WorkQueue.Edit.WorkQueueItemDetailsPanel.initializeBase(this, [element]);       
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Create the prototype for the control.
    //
    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    ClearCanvas.ImageServer.Web.Application.Pages.Queues.WorkQueue.Edit.WorkQueueItemDetailsPanel.prototype = 
    {
        initialize : function() {
        
            ClearCanvas.ImageServer.Web.Application.Pages.Queues.WorkQueue.Edit.WorkQueueItemDetailsPanel.callBaseMethod(this, 'initialize');        
            
            this._OnLoadHandler = Function.createDelegate(this,this._OnLoad);
            this._OnViewDetailsButtonClickedHandler = Function.createDelegate(this,this._OnViewDetailsButtonClickedHandler);
                        
            Sys.Application.add_load(this._OnLoadHandler);
                 
        },
        
        dispose : function() {
            $clearHandlers(this.get_element());

            ClearCanvas.ImageServer.Web.Application.Pages.Queues.WorkQueue.Edit.WorkQueueItemDetailsPanel.callBaseMethod(this, 'dispose');
            
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
            var viewDetailsButton = $find(this._ViewStudiesButtonClientID);
            if(viewDetailsButton != null) viewDetailsButton.add_onClientClick( this._OnViewDetailsButtonClickedHandler );  
        },    

        /////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        // Public methods
        //
        /////////////////////////////////////////////////////////////////////////////////////////////////////////
               
        _OnViewDetailsButtonClickedHandler : function(sender, event)
        {
            this._openStudy();
        },
                
        /////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        // Private Methods
        //
        /////////////////////////////////////////////////////////////////////////////////////////////////////////
                
        _openStudy : function()
        {
            var url = String.format('{0}?serverae={1}&siuid={2}', this._OpenStudyPageUrl, this._ServerAE, this._StudyInstanceUid);
            window.open(url);
        },
                
        /////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        // Properties
        //
        /////////////////////////////////////////////////////////////////////////////////////////////////////////
            
        get_ViewStudiesButtonClientID : function() {
            return this._ViewStudiesButtonClientID;
        },

        set_ViewStudiesButtonClientID : function(value) {
            this._ViewStudiesButtonClientID = value;
            this.raisePropertyChanged('ViewStudiesButtonClientID');
        },
        
        get_OpenStudyPageUrl : function() {
            return this._OpenStudyPageUrl;
        },
       
        set_OpenStudyPageUrl : function(value) {
            this._OpenStudyPageUrl = value;
            this.raisePropertyChanged('OpenStudyPageUrl');
        },
        
        get_ServerAE : function() {
            return this._ServerAE;
        },
       
        set_ServerAE : function(value) {
            this._ServerAE = value;
            this.raisePropertyChanged('ServerAE');
        },      
        
        get_StudyInstanceUid : function() {
            return this._StudyInstanceUid;
        },
       
        set_StudyInstanceUid : function(value) {
            this._StudyInstanceUid = value;
            this.raisePropertyChanged('StudyInstanceUid');
        }                              
    }

    // Register the class as a type that inherits from Sys.UI.Control.

        ClearCanvas.ImageServer.Web.Application.Pages.Queues.WorkQueue.Edit.WorkQueueItemDetailsPanel.registerClass('ClearCanvas.ImageServer.Web.Application.Pages.Queues.WorkQueue.Edit.WorkQueueItemDetailsPanel', Sys.UI.Control);
     

    if (typeof(Sys) !== 'undefined') Sys.Application.notifyScriptLoaded();

}