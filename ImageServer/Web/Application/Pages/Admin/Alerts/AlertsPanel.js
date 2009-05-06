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

if (window.__registeredTypes['ClearCanvas.ImageServer.Web.Application.Pages.Admin.Alerts.AlertsPanel']==null)
{

    Type.registerNamespace('ClearCanvas.ImageServer.Web.Application.Pages.Admin.Alerts');

    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Constructor
    //
    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    ClearCanvas.ImageServer.Web.Application.Pages.Admin.Alerts.AlertsPanel = function(element) { 
        ClearCanvas.ImageServer.Web.Application.Pages.Admin.Alerts.AlertsPanel.initializeBase(this, [element]);
       
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Create the prototype for the control.
    //
    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    ClearCanvas.ImageServer.Web.Application.Pages.Admin.Alerts.AlertsPanel.prototype = 
    {
        initialize : function() {
            ClearCanvas.ImageServer.Web.Application.Pages.Admin.Alerts.AlertsPanel.callBaseMethod(this, 'initialize');        
            
            this._OnAlertListRowClickedHandler = Function.createDelegate(this,this._OnAlertListRowClicked);
            
            this._OnLoadHandler = Function.createDelegate(this,this._OnLoad);
            Sys.Application.add_load(this._OnLoadHandler);
                 
        },
        
        dispose : function() {
            $clearHandlers(this.get_element());

            ClearCanvas.ImageServer.Web.Application.Pages.Admin.Alerts.AlertsPanel.callBaseMethod(this, 'dispose');
            
            Sys.Application.remove_load(this._OnLoadHandler);
        },
        
        
        /////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        // Events
        //
        /////////////////////////////////////////////////////////////////////////////////////////////////////////
        
        _OnLoad : function()
        {                    
            var userlist = $find(this._AlertListClientID);
            userlist.add_onClientRowClick(this._OnAlertListRowClickedHandler);
                 
            this._updateToolbarButtonStates();
        },
        
        // called when user clicked on a row in the study list
        _OnAlertListRowClicked : function(sender, event)
        {    
            this._updateToolbarButtonStates();        
        },
                       
        /////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        // Private Methods
        //
        /////////////////////////////////////////////////////////////////////////////////////////////////////////
        
        _updateToolbarButtonStates : function()
        {        
            var alertlist = $find(this._AlertListClientID);
                      
            this._enableDeleteButton(false);
            this._enableDeleteAllButton(false);
                               
            if (alertlist!=null )
            {
                var rows = alertlist.getSelectedRowElements();

                if(rows != null && rows.length > 0) {
                    this._enableDeleteButton(true);
                }
                
                if(alertlist.getNumberOfRows() > 0) {
                    this._enableDeleteAllButton(true);
                }
            }
        },
        
        _enableDeleteButton : function(en)
        {
            var deleteButton = $find(this._DeleteButtonClientID);
            if(deleteButton != null) deleteButton.set_enable(en);
        },
        
        _enableDeleteAllButton : function(en)
        {
            var deleteAllButton = $find(this._DeleteAllButtonClientID);
            if(deleteAllButton != null) deleteAllButton.set_enable(en);
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
               
        get_DeleteButtonClientID : function() {
            return this._DeleteButtonClientID;
        },

        set_DeleteButtonClientID : function(value) {
            this._DeleteButtonClientID = value;
            this.raisePropertyChanged('DeleteButtonClientID');
        },
        
        get_DeleteAllButtonClientID : function() {
            return this._DeleteAllButtonClientID;
        },

        set_DeleteAllButtonClientID : function(value) {
            this._DeleteAllButtonClientID = value;
            this.raisePropertyChanged('DeleteAllButtonClientID');
        },
                
        get_AlertListClientID : function() {
            return this._AlertListClientID;
        },

        set_AlertListClientID : function(value) {
            this._AlertListClientID = value;
            this.raisePropertyChanged('AlertListClientID');
        }
    }

    // Register the class as a type that inherits from Sys.UI.Control.
    ClearCanvas.ImageServer.Web.Application.Pages.Admin.Alerts.AlertsPanel.registerClass('ClearCanvas.ImageServer.Web.Application.Pages.Admin.Alerts.AlertsPanel', Sys.UI.Control);
     

    if (typeof(Sys) !== 'undefined') Sys.Application.notifyScriptLoaded();

}