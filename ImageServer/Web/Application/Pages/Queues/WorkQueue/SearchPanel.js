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
if (window.__registeredTypes['ClearCanvas.ImageServer.Web.Application.Pages.Queues.WorkQueue.SearchPanel']==null)
{
    Type.registerNamespace('ClearCanvas.ImageServer.Web.Application.Pages.Queues.WorkQueue');

    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Constructor
    //
    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    ClearCanvas.ImageServer.Web.Application.Pages.Queues.WorkQueue.SearchPanel = function(element) { 
        ClearCanvas.ImageServer.Web.Application.Pages.Queues.WorkQueue.SearchPanel.initializeBase(this, [element]);
       
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Create the prototype for the control.
    //
    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    ClearCanvas.ImageServer.Web.Application.Pages.Queues.WorkQueue.SearchPanel.prototype = 
    {
        initialize : function() {
            ClearCanvas.ImageServer.Web.Application.Pages.Queues.WorkQueue.SearchPanel.callBaseMethod(this, 'initialize');        
            
            this._OnLoadHandler = Function.createDelegate(this,this._OnLoad);
            Sys.Application.add_load(this._OnLoadHandler);
                 
        },
        
        dispose : function() {
            $clearHandlers(this.get_element());

            ClearCanvas.ImageServer.Web.Application.Pages.Queues.WorkQueue.SearchPanel.callBaseMethod(this, 'dispose');
            
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
            
        }
        
        
        
        
        /////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        // Private Methods
        //
        /////////////////////////////////////////////////////////////////////////////////////////////////////////
        
        

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
        
    }

    // Register the class as a type that inherits from Sys.UI.Control.

        ClearCanvas.ImageServer.Web.Application.Pages.Queues.WorkQueue.SearchPanel.registerClass('ClearCanvas.ImageServer.Web.Application.Pages.Queues.WorkQueue.SearchPanel', Sys.UI.Control);
     

    if (typeof(Sys) !== 'undefined') Sys.Application.notifyScriptLoaded();

}