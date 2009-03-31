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

if (window.__registeredTypes['ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.FileSystems.FileSystemsPanel']==null)
{
    Type.registerNamespace('ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.FileSystems');

    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Constructor
    //
    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.FileSystems.FileSystemsPanel = function(element) { 
        ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.FileSystems.FileSystemsPanel.initializeBase(this, [element]);
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Create the prototype for the control.
    //
    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.FileSystems.FileSystemsPanel.prototype = 
    {
        initialize : function() {
                       
            ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.FileSystems.FileSystemsPanel.callBaseMethod(this, 'initialize');        
            
            this._OnFileSystemListRowClickedHandler = Function.createDelegate(this,this._OnFileSystemListRowClicked);
            
            this._OnLoadHandler = Function.createDelegate(this,this._OnLoad);
            Sys.Application.add_load(this._OnLoadHandler);
                 
        },
        
        dispose : function() {
            $clearHandlers(this.get_element());

            ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.FileSystems.FileSystemsPanel.callBaseMethod(this, 'dispose');
            
            Sys.Application.remove_load(this._OnLoadHandler);
        },
        
        
        /////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        // Events
        //
        /////////////////////////////////////////////////////////////////////////////////////////////////////////
        
        _OnLoad : function()
        {                    
            var filesystemlist = $find(this._FileSystemListClientID);
            filesystemlist.add_onClientRowClick(this._OnFileSystemListRowClickedHandler);
                           
            this._updateToolbarButtonStates();
        },
        
        // called when user clicked on a row in the study list
        _OnFileSystemListRowClicked : function(sender, event)
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
            var filesystemlist = $find(this._FileSystemListClientID);
                               
            this._enableEditButton(false);
                                           
            if (filesystemlist!=null )
            {
                var rows = filesystemlist.getSelectedRowElements();

                if(rows != null && rows.length > 0) {
                    this._enableEditButton(true);
                }
            }
        },

        _enableEditButton : function(en)
        {
            var editButton = $find(this._EditButtonClientID);
            editButton.set_enable(en);
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
                      
        get_EditButtonClientID : function() {
            return this._EditButtonClientID;
        },

        set_EditButtonClientID : function(value) {
            this._EditButtonClientID = value;
            this.raisePropertyChanged('EditButtonClientID');
        },
               
        get_FileSystemListClientID : function() {
            return this._FileSystemListClientID;
        },

        set_FileSystemListClientID : function(value) {
            this._FileSystemListClientID = value;
            this.raisePropertyChanged('FileSystemListClientID');
        }
    }

    // Register the class as a type that inherits from Sys.UI.Control.
    ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.FileSystems.FileSystemsPanel.registerClass('ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.FileSystems.FileSystemsPanel', Sys.UI.Control);
     

    if (typeof(Sys) !== 'undefined') Sys.Application.notifyScriptLoaded();

}