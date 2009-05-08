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
            this._OnItemListRowClickedHandler = Function.createDelegate(this,this._OnItemListRowClicked);
            this._OnItemListRowDblClickedHandler = Function.createDelegate(this,this._OnItemListRowDblClicked);
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
        
        _updateToolbarButtonStates : function()
        {
            var itemlist = $find(this._ItemListClientID);
                      
            this._enableViewDetailsButton(false);
            this._enableRescheduleButton(false);
            this._enableResetButton(false);
            this._enableDeleteButton(false);
            this._enableReprocessButton(false);
            
            if (itemlist!=null )
            {
                var rows = itemlist.getSelectedRowElements();
                if (rows.length>0)
                {
                    for(i=0; i<rows.length; i++)
                    {
                        this._enableViewDetailsButton(true);
                        this._enableRescheduleButton(this._canRescheduleItem(rows[i]));
                        this._enableResetButton(this._canResetItem(rows[i]));
                        this._enableDeleteButton(this._canDeleteItem(rows[i]));
                        this._enableReprocessButton(this._canReprocessItem(rows[i]));
                    }
                }
            }
        },     
                
        _canRescheduleItem:function(row)
        {
            //"canreconcile" is a custom attribute injected by the list control
            return row.getAttribute('canreschedule')=='true';
        },
        
        _canResetItem:function(row)
        {
            //"canreconcile" is a custom attribute injected by the list control
            return row.getAttribute('canreset')=='true';
        },
        
        _canDeleteItem:function(row)
        {
            //"canreconcile" is a custom attribute injected by the list control
            return row.getAttribute('candelete')=='true';
        },
        
        _canReprocessItem:function(row)
        {
            //"canreconcile" is a custom attribute injected by the list control
            return row.getAttribute('canreprocess')=='true';
        },
        
        _enableViewDetailsButton : function(en)
        {
            var viewDetailsButton = $find(this._ViewDetailsButtonClientID);
            if(viewDetailsButton != null) viewDetailsButton.set_enable(en);
        },
        
        _enableRescheduleButton : function(en)
        {
            var rescheduleButton = $find(this._RescheduleButtonClientID);
            if(rescheduleButton != null) rescheduleButton.set_enable(en);
        },
        
        _enableDeleteButton : function(en)
        {
            var deleteButton = $find(this._DeleteButtonClientID);
            if(deleteButton != null) deleteButton.set_enable(en);
        },
        
        _enableReprocessButton : function(en)
        {
            var reprocessButton = $find(this._ReprocessButtonClientID);
            if(reprocessButton != null) reprocessButton.set_enable(en);
        },
        
        _enableResetButton : function(en)
        {
            var resetButton = $find(this._ResetButtonClientID);
            if(resetButton != null) resetButton.set_enable(en);
        },

        /////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        // Properties
        //
        /////////////////////////////////////////////////////////////////////////////////////////////////////////
     
        get_ItemListClientID : function() {
            return this._ItemListClientID;
        },

        set_ItemListClientID : function(value) {
            this._ItemListClientID = value;
            this.raisePropertyChanged('ItemListClientID');
        },
        
        get_ViewDetailsButtonClientID : function() {
            return this._ViewDetailsButtonClientID;
        },

        set_ViewDetailsButtonClientID : function(value) {
            this._ViewDetailsButtonClientID = value;
            this.raisePropertyChanged('ViewDetailsButtonClientID');
        },
        
        get_RescheduleButtonClientID : function() {
            return this._RescheduleButtonClientID;
        },

        set_RescheduleButtonClientID : function(value) {
            this._RescheduleButtonClientID = value;
            this.raisePropertyChanged('RescheduleButtonClientID');
        },
        
        get_ResetButtonClientID : function() {
            return this._ResetButtonClientID;
        },

        set_ResetButtonClientID : function(value) {
            this._ResetButtonClientID = value;
            this.raisePropertyChanged('ResetButtonClientID');
        },
        
        get_DeleteButtonClientID : function() {
            return this._DeleteButtonClientID;
        },

        set_DeleteButtonClientID : function(value) {
            this._DeleteButtonClientID = value;
            this.raisePropertyChanged('DeleteButtonClientID');
        },
        
        get_ReprocessButtonClientID : function() {
            return this._ReprocessButtonClientID;
        },

        set_ReprocessButtonClientID : function(value) {
            this._ReprocessButtonClientID = value;
            this.raisePropertyChanged('ReprocessButtonClientID');
        }         
    }

    // Register the class as a type that inherits from Sys.UI.Control.

        ClearCanvas.ImageServer.Web.Application.Pages.Queues.WorkQueue.SearchPanel.registerClass('ClearCanvas.ImageServer.Web.Application.Pages.Queues.WorkQueue.SearchPanel', Sys.UI.Control);
     

    if (typeof(Sys) !== 'undefined') Sys.Application.notifyScriptLoaded();

}