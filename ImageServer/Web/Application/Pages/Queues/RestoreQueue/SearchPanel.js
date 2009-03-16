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

if (window.__registeredTypes['ClearCanvas.ImageServer.Web.Application.Pages.Queues.RestoreQueue.SearchPanel']==null)
{
        Type.registerNamespace('ClearCanvas.ImageServer.Web.Application.Pages.Queues.RestoreQueue');
        
        
    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Constructor
    //
    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    ClearCanvas.ImageServer.Web.Application.Pages.Queues.RestoreQueue.SearchPanel = function(element) { 
        ClearCanvas.ImageServer.Web.Application.Pages.Queues.RestoreQueue.SearchPanel.initializeBase(this, [element]);
    }
   

    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Create the prototype for the control.
    //
    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    ClearCanvas.ImageServer.Web.Application.Pages.Queues.RestoreQueue.SearchPanel.prototype = 
    {
        initialize : function() {
            ClearCanvas.ImageServer.Web.Application.Pages.Queues.RestoreQueue.SearchPanel.callBaseMethod(this, 'initialize');        
            
            this._OnItemListRowClickedHandler = Function.createDelegate(this,this._OnItemListRowClicked);
            this._OnItemListRowDblClickedHandler = Function.createDelegate(this,this._OnItemListRowDblClicked);
            this._OnLoadHandler = Function.createDelegate(this,this._OnLoad);
            Sys.Application.add_load(this._OnLoadHandler);
                 
        },
        
                dispose : function() {
            $clearHandlers(this.get_element());

            ClearCanvas.ImageServer.Web.Application.Pages.Queues.RestoreQueue.SearchPanel.callBaseMethod(this, 'dispose');
            
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
            var itemlist = $find(this._ItemListClientID);
            itemlist.add_onClientRowClick(this._OnItemListRowClickedHandler);
            itemlist.add_onClientRowDblClick(this._OnItemListRowDblClickedHandler);
            
            this._updateToolbarButtonStates();
        },
                     
        // called when user clicked on a row in the study list
        _OnItemListRowClicked : function(sender, event)
        {    
            this._updateToolbarButtonStates();        
        },
        
        // called when user double-clicked on a row in the study list
        _OnItemListRowDblClicked : function(sender, event)
        {
            this._updateToolbarButtonStates();
        },
                      
        _updateToolbarButtonStates : function()
        {
            var itemlist = $find(this._ItemListClientID);
                      
            if (itemlist!=null )
            {
                var rows = itemlist.getSelectedRowElements();
                if (rows.length>0)
                {
                    this._enableDeleteButton(true);
                }
                else
                {
                    this._enableDeleteButton(false);
                }
            }
            else
            {
                this._enableDeleteButton(false);
            }
        },     
        
        _enableDeleteButton : function(en)
        {
            var deleteButton = $find(this._DeleteButtonClientID);
            if(deleteButton != null) deleteButton.set_enable(en);
        },
                     
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
                        
        get_ItemListClientID : function() {
            return this._ItemListClientID;
        },

        set_ItemListClientID : function(value) {
            this._ItemListClientID = value;
            this.raisePropertyChanged('ItemListClientID');
        }
   }
   
   // Register the class as a type that inherits from Sys.UI.Control.

   ClearCanvas.ImageServer.Web.Application.Pages.Queues.RestoreQueue.SearchPanel.registerClass('ClearCanvas.ImageServer.Web.Application.Pages.Queues.RestoreQueue.SearchPanel', Sys.UI.Control);

   if (typeof(Sys) !== 'undefined') Sys.Application.notifyScriptLoaded();

}