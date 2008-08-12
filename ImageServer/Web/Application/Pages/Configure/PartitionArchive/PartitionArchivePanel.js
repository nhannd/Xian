/////////////////////////////////////////////////////////////////////////////////////////////////////////
///
/// This script contains the javascript component class for the Partition Archive toolbar panel
/// 
/////////////////////////////////////////////////////////////////////////////////////////////////////////

// Define and register the control type.
//
// Only define and register the type if it doens't exist. Otherwise "... does not derive from Sys.Component" error 
// will show up if multiple instance of the controls must be created. The error is misleading. It looks like the type 
// is RE-define for the 2nd instance but registerClass() will fail so the type will be essential undefined when the object
// is instantiated.
//

if (window.__registeredTypes['ClearCanvas.ImageServer.Web.Application.Pages.Configure.PartitionArchive.PartitionArchivePanel']==null)
{

    Type.registerNamespace('ClearCanvas.ImageServer.Web.Application.Pages.Configure.PartitionArchive');

    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Constructor
    //
    /////////////////////////////////////////////////////////////////////////////////////////////////////////

    ClearCanvas.ImageServer.Web.Application.Pages.Configure.PartitionArchive.PartitionArchivePanel = function(element) { 
        ClearCanvas.ImageServer.Web.Application.Pages.Configure.PartitionArchive.PartitionArchivePanel.initializeBase(this, [element]);
       
    }
    
    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Create the prototype for the control.
    //
    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    ClearCanvas.ImageServer.Web.Application.Pages.Configure.PartitionArchive.PartitionArchivePanel.prototype = 
    {
        initialize : function() {
               
            ClearCanvas.ImageServer.Web.Application.Pages.Configure.PartitionArchive.PartitionArchivePanel.callBaseMethod(this, 'initialize');        
            
            this._OnAddButtonClickedHandler = Function.createDelegate(this,this._OnAddButtonClicked);
            this._OnEditButtonClickedHandler = Function.createDelegate(this,this._OnEditButtonClicked);   
            this._OnDeleteButtonClickedHandler = Function.createDelegate(this,this._OnDeleteButtonClicked);   
            this._OnPartitionArchiveListRowClickedHandler = Function.createDelegate(this,this._OnPartitionArchiveListRowClicked);
            this._OnPartitionArchiveListRowDblClickedHandler = Function.createDelegate(this,this._OnPartitionArchiveListRowDblClicked);
            this._OnLoadHandler = Function.createDelegate(this,this._OnLoad);
            Sys.Application.add_load(this._OnLoadHandler);
                 
        },
        
        dispose : function() {
            $clearHandlers(this.get_element());

            ClearCanvas.ImageServer.Web.Application.Pages.Configure.PartitionArchive.PartitionArchivePanel.callBaseMethod(this, 'dispose');
            
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
            // because NEW instances of the button and the partitionArchive list components
            // may have been created as the result of the post-back
            var addButton = $find(this._AddButtonClientID);
            addButton.add_onClientClick( this._OnAddButtonClickedHandler );   

            var editButton = $find(this._EditButtonClientID);
            editButton.add_onClientClick( this._OnEditButtonClickedHandler );   
            
            var deleteButton = $find(this._DeleteButtonClientID);
            deleteButton.add_onClientClick( this._OnDeleteButtonClickedHandler );   
                 
            var partitionArchivelist = $find(this._PartitionArchiveListClientID);
            partitionArchivelist.add_onClientRowClick(this._OnPartitionArchiveListRowClickedHandler);
            partitionArchivelist.add_onClientRowDblClick(this._OnPartitionArchiveListRowDblClickedHandler);
            
            this._updateToolbarButtonStates();
        },
        
        // called when the Open PartitionArchive button is clicked
        _OnAddButtonClicked : function(src, event)
        {
        },
        
        // called when the Send PartitionArchive button is clicked
        _OnEditButtonClicked : function(src, event)
        {
        },
        
        // called when the Send PartitionArchive button is clicked
        _OnDeleteButtonClicked : function(src, event)
        {
        },
        
        // called when user clicked on a row in the partitionArchive list
        _OnPartitionArchiveListRowClicked : function(sender, event)
        {    
            this._updateToolbarButtonStates();        
        },
        
        // called when user double-clicked on a row in the partitionArchive list
        _OnPartitionArchiveListRowDblClicked : function(sender, event)
        {
            this._updateToolbarButtonStates();
            this._editSelectedPartitionArchives();
        },
        
        
        /////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        // Private Methods
        //
        /////////////////////////////////////////////////////////////////////////////////////////////////////////
           
        _updateToolbarButtonStates : function()
        {
            var partitionArchivelist = $find(this._PartitionArchiveListClientID);
                      
            if (partitionArchivelist!=null && partitionArchivelist.getSelectedRowElements().length > 0)
            {
                this._enableEditButton(true);
    			this._enableDeleteButton(true);                    
            }
            else
            {
                this._enableEditButton(false);
                this._enableDeleteButton(false);
            }
        },
        
        
        _enableAddButton : function(en)
        {
            var addButton = $find(this._AddButtonClientID);
            addButton.set_enable(en);
        },
        
        _enableEditButtonButton : function(en)
        {
            var editButton = $find(this._EditButtonClientID);
            editButton.set_enable(en);
        },
        
        _enableDeleteButton : function(en)
        {
            var deleteButton = $find(this._DeleteButtonClientID);
            deleteButton.set_enable(en);
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
        
        get_AddButtonClientID : function() {
            return this._AddButtonClientID;
        },

        set_AddButtonClientID : function(value) {
            this._AddButtonClientID = value;
            this.raisePropertyChanged('AddButtonClientID');
        },

        get_EditButtonClientID : function() {
            return this._EditButtonClientID;
        },

        set_EditButtonClientID : function(value) {
            this._EditButtonClientID = value;
            this.raisePropertyChanged('EditButtonClientID');
        },
                        
        get_DeleteButtonClientID : function() {
            return this._DeleteButtonClientID;
        },

        set_DeleteButtonClientID : function(value) {
            this._DeleteButtonClientID = value;
            this.raisePropertyChanged('DeleteButtonClientID');
        },
                
        get_PartitionArchiveListClientID : function() {
            return this._PartitionArchiveListClientID;
        },

        set_PartitionArchiveListClientID : function(value) {
            this._PartitionArchiveListClientID = value;
            this.raisePropertyChanged('PartitionArchiveListClientID');
        },
    }

    // Register the class as a type that inherits from Sys.UI.Control.

        ClearCanvas.ImageServer.Web.Application.Pages.Configure.PartitionArchive.PartitionArchivePanel.registerClass('ClearCanvas.ImageServer.Web.Application.Pages.Configure.PartitionArchive.PartitionArchivePanel', Sys.UI.Control);
     

    if (typeof(Sys) !== 'undefined') Sys.Application.notifyScriptLoaded();

}