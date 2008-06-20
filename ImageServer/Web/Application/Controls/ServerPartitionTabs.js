alert("In Javascript");

if (window.__registeredTypes['ClearCanvas.ImageServer.Web.Application.Common.Controls.ServerPartitionTabs']==null)
{

    debugger;
    alert("In Javascript");

    Type.registerNamespace('ClearCanvas.ImageServer.Web.Application.Common.Controls');

    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Constructor
    //
    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    ClearCanvas.ImageServer.Web.Application.Common.Controls.ServerPartitionTabs = function(element) { 
        ClearCanvas.ImageServer.Web.Application.Common.Controls.ServerPartitionTabs.initializeBase(this, [element]);
       
    }
    
    ClearCanvas.ImageServer.Web.Application.Pages.Search.SearchPanel.prototype = 
    {
        initialize : function() {
            alert("initialize");
            this._OnTabClickedHandler = Function.createDelegate(this,this._OnTabClicked);        
        },
        
        dispose : function() {
            $clearHandlers(this.get_element());

            ClearCanvas.ImageServer.Web.Application.Common.Controls.ServerPartitionTabs.callBaseMethod(this, 'dispose');
            
            Sys.Application.remove_load(this._OnLoadHandler);
        },
        
        _onLoad : function() {
            alert("onLoad");
            var tabPanel = $find(this._TabPanelClientID);
            tabPanel.onClientClick = this._OnTabPanelClickedHandler;   
        
        },
        
        _onTabClicked : function() {
        
            alert("Tab Clicked!");
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        // Properties
        //
        /////////////////////////////////////////////////////////////////////////////////////////////////////////
        
        get_TabPanelClientID : function() {
            return this._TabPanelClientID;
        },

        set_TabPanelClientID : function(value) {
            this._TabPAnelClientID = value;
            this.raisePropertyChanged('TabPanelClientID');
        },
    }
    
    // Register the class as a type that inherits from Sys.UI.Control.

    ClearCanvas.ImageServer.Web.Application.Common.Controls.ServerPartitionTabs.registerClass('ClearCanvas.ImageServer.Web.Application.Common.Controls.ServerPartitionTabs', Sys.UI.Control);    

    if (typeof(Sys) !== 'undefined') Sys.Application.notifyScriptLoaded();
}
