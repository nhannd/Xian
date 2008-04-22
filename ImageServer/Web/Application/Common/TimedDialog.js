
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

if (window.__registeredTypes['ClearCanvas.ImageServer.Web.Application.Common.TimedDialog']==null)
{

    Type.registerNamespace('ClearCanvas.ImageServer.Web.Application.Common.TimedDialog');

    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Constructor
    //
    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    ClearCanvas.ImageServer.Web.Application.Common.TimedDialog = function(element) { 
        ClearCanvas.ImageServer.Web.Application.Common.TimedDialog.initializeBase(this, [element]);
       
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Create the prototype for the control.
    //
    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    ClearCanvas.ImageServer.Web.Application.Common.TimedDialog.prototype = 
    {
        initialize : function() {
            ClearCanvas.ImageServer.Web.Application.Common.TimedDialog.callBaseMethod(this, 'initialize');        
            
            this._OnLoadHandler = Function.createDelegate(this,this._OnLoad);
            Sys.Application.add_load(this._OnLoadHandler);
                 
        },
        
        dispose : function() {
            $clearHandlers(this.get_element());

            ClearCanvas.ImageServer.Web.Application.Search.SearchPanel.callBaseMethod(this, 'dispose');
            
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
            // hook up the events... It is necessary to do this every time 
            // because NEW instances of the button and the study list components
            // may have been created as the result of the post-back
            setTimeout('Redirect()',4000);
        },
        
        
        /////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        // Private Methods
        //
        /////////////////////////////////////////////////////////////////////////////////////////////////////////
        
        function Redirect()
        {
             location.href = '~/Search/SearchPage.aspx';
        }
        

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
        
        
        get_OpenButtonClientID : function() {
            return this._OpenButtonClientID;
        },

        set_OpenButtonClientID : function(value) {
            this._OpenButtonClientID = value;
            this.raisePropertyChanged('OpenButtonClientID');
        },
        
    }

    // Register the class as a type that inherits from Sys.UI.Control.

    ClearCanvas.ImageServer.Web.Application.Common.TimedDialog.registerClass('ClearCanvas.ImageServer.Web.Application.Common.TimedDialog', Sys.UI.Control);
     
    if (typeof(Sys) !== 'undefined') Sys.Application.notifyScriptLoaded();
}