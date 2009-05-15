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

if (window.__registeredTypes['ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.ServerPartitions.ServerPartitionPanel']==null)
{

    Type.registerNamespace('ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.ServerPartitions');

    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Constructor
    //
    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.ServerPartitions.ServerPartitionPanel = function(element) { 
        ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.ServerPartitions.ServerPartitionPanel.initializeBase(this, [element]);
       
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Create the prototype for the control.
    //
    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.ServerPartitions.ServerPartitionPanel.prototype = 
    {
        initialize : function() {
       
            ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.ServerPartitions.ServerPartitionPanel.callBaseMethod(this, 'initialize');        
            
            this._OnServerPartitionListRowClickedHandler = Function.createDelegate(this,this._OnServerPartitionListRowClicked);
            
            this._OnLoadHandler = Function.createDelegate(this,this._OnLoad);
            Sys.Application.add_load(this._OnLoadHandler);
                 
        },
        
        dispose : function() {
            $clearHandlers(this.get_element());

            ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.ServerPartitions.ServerPartitionPanel.callBaseMethod(this, 'dispose');
            
            Sys.Application.remove_load(this._OnLoadHandler);
        },
        
        
        /////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        // Events
        //
        /////////////////////////////////////////////////////////////////////////////////////////////////////////
        
        _OnLoad : function()
        {                    
            var serverpartitionlist = $find(this._ServerPartitionListClientID);
            serverpartitionlist.add_onClientRowClick(this._OnServerPartitionListRowClickedHandler);
                 
            this._updateToolbarButtonStates();
        },
        
        // called when user clicked on a row in the study list
        _OnServerPartitionListRowClicked : function(sender, event)
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
            var serverpartitionlist = $find(this._ServerPartitionListClientID);
                                         
            this._enableEditButton(false);
            this._enableDeleteButton(false);
                                           
            if (serverpartitionlist!=null )
            {
                var rows = serverpartitionlist.getSelectedRowElements();

                if (rows!=null && rows.length>0)
                {
		            var selectedPartitionCount = rows.length; 
		            var canDeleteCount=0; 
                    if (rows.length>0)
                    {
					    for(i=0; i<rows.length; i++)
                        {
                            if (this._canDeletePartition(rows[i]))
                            {
                                canDeleteCount++;
                            }
                        }
                    }
                    // always enabled open button when a row is selected
                    this._enableEditButton(true);
    				
                    this._enableDeleteButton(canDeleteCount==selectedPartitionCount);
                }
            }
        },
        
        _canDeletePartition:function(row)
        {
            //"candelete" is a custom attribute injected by the list control
            return row.getAttribute('candelete')=='true';
        },

        _enableDeleteButton : function(en)
        {
            var deleteButton = $find(this._DeleteButtonClientID);
            deleteButton.set_enable(en);
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
               
        get_DeleteButtonClientID : function() {
            return this._DeleteButtonClientID;
        },

        set_DeleteButtonClientID : function(value) {
            this._DeleteButtonClientID = value;
            this.raisePropertyChanged('DeleteButtonClientID');
        },
        
        get_EditButtonClientID : function() {
            return this._EditButtonClientID;
        },

        set_EditButtonClientID : function(value) {
            this._EditButtonClientID = value;
            this.raisePropertyChanged('EditButtonClientID');
        },
               
        get_ServerPartitionListClientID : function() {
            return this._ServerPartitionListClientID;
        },

        set_ServerPartitionListClientID : function(value) {
            this._ServerPartitionListClientID = value;
            this.raisePropertyChanged('ServerPartitionListClientID');
        }
    }

    // Register the class as a type that inherits from Sys.UI.Control.
    ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.ServerPartitions.ServerPartitionPanel.registerClass('ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.ServerPartitions.ServerPartitionPanel', Sys.UI.Control);
     

    if (typeof(Sys) !== 'undefined') Sys.Application.notifyScriptLoaded();

}