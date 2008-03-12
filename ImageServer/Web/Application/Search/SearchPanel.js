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

if (window.__registeredTypes['ClearCanvas.ImageServer.Web.Application.Search.SearchPanel']==null)
{

    Type.registerNamespace('ClearCanvas.ImageServer.Web.Application.Search');

    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Constructor
    //
    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    ClearCanvas.ImageServer.Web.Application.Search.SearchPanel = function(element) { 
        ClearCanvas.ImageServer.Web.Application.Search.SearchPanel.initializeBase(this, [element]);
       
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Create the prototype for the control.
    //
    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    ClearCanvas.ImageServer.Web.Application.Search.SearchPanel.prototype = 
    {
        initialize : function() {
            ClearCanvas.ImageServer.Web.Application.Search.SearchPanel.callBaseMethod(this, 'initialize');        
            
            this._OnOpenButtonClickedHandler = Function.createDelegate(this,this._OnOpenButtonClicked);
            this._OnStudyListRowClickedHandler = Function.createDelegate(this,this._OnStudyListRowClicked);
            this._OnStudyListRowDblClickedHandler = Function.createDelegate(this,this._OnStudyListRowDblClicked);
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
            var openButton = $find(this._OpenButtonClientID);
            openButton.add_onClientClick( this._OnOpenButtonClickedHandler );   
                 
            var studylist = $find(this._StudyListClientID);
            studylist.add_onClientRowClick(this._OnStudyListRowClickedHandler);
            studylist.add_onClientRowDblClick(this._OnStudyListRowDblClickedHandler);
            
            this._updateToolbarButtonStates();
        },
        
        // called when the Open Study button is clicked
        _OnOpenButtonClicked : function(src, event)
        {
            this._openSelectedStudies();
            
        },
        
        // called when user clicked on a row in the study list
        _OnStudyListRowClicked : function(sender, event)
        {    
            this._updateToolbarButtonStates();        
        },
        
        // called when user double-clicked on a row in the study list
        _OnStudyListRowDblClicked : function(sender, event)
        {
            this._updateToolbarButtonStates();
            this._openSelectedStudies();
        },
        
        
        /////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        // Private Methods
        //
        /////////////////////////////////////////////////////////////////////////////////////////////////////////
        
        // return the study instance uid of the row 
        _getInstanceUid : function(row)
        {
            //"instanceuid" is a custom attribute injected by the study list control
            return row.getAttribute('instanceuid');
        },
        
        _getServerPartitionAE : function(row)
        {
            //"serverae" is a custom attribute injected by the study list control
            return row.getAttribute('serverae');
        },
        
        _studyIsDeleted : function(row)
        {
            //"deleted" is a custom attribute injected by the study list control
            return row.getAttribute('deleted')=='true';
        },
        
        _openSelectedStudies : function()
        {
            var studylist = $find(this._StudyListClientID);
            // open the selected studies
            if (studylist!=null )
            {
                var rows = studylist.getSelectedRowElements();
                if (rows.length>0)
                {
                    for(i=0; i<rows.length; i++)
                    {
                        var instanceuid = this._getInstanceUid(rows[i]);
                        var serverae = this._getServerPartitionAE(rows[i]);
                        if (instanceuid!=undefined && serverae!=undefined)
                        {
                            var url= String.format('{0}?serverae={1}&siuid={2}', this._OpenStudyPageUrl, serverae, instanceuid);
                            window.open(url);
                        }
                    }
                    
                }
            }
        },

        
        _updateToolbarButtonStates : function()
        {
            var studylist = $find(this._StudyListClientID);
            
            if (studylist!=null )
            {
                var rows = studylist.getSelectedRowElements();
                if (rows.length>0)
                {
                    this._enableOpenStudyButton(true);
                    
                    for(i=0; i<rows.length; i++)
                    {
                        if (this._studyIsDeleted(rows[i])) 
                        {
                            this._enableDeleteButton(false);
                            return;
                        }
                    }
                    this._enableDeleteButton(true);
                }
                else
                {
                    this._enableDeleteButton(false);
                    this._enableOpenStudyButton(false);
                }
            }
            else
            {
                this._enableDeleteButton(false);
                this._enableOpenStudyButton(false);
            }
        },
        
        
        
        _enableDeleteButton : function(en)
        {
            var deleteButton = $find(this._DeleteButtonClientID);
            deleteButton.set_enable(en);
        },
        
        _enableOpenStudyButton : function(en)
        {
            var openButton = $find(this._OpenButtonClientID);
            openButton.set_enable(en);
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
        
        
        get_OpenButtonClientID : function() {
            return this._OpenButtonClientID;
        },

        set_OpenButtonClientID : function(value) {
            this._OpenButtonClientID = value;
            this.raisePropertyChanged('OpenButtonClientID');
        },
        
        get_StudyListClientID : function() {
            return this._StudyListClientID;
        },

        set_StudyListClientID : function(value) {
            this._StudyListClientID = value;
            this.raisePropertyChanged('StudyListClientID');
        },
        
        get_OpenStudyPageUrl : function() {
            return this._OpenStudyPageUrl;
        },
       
        set_OpenStudyPageUrl : function(value) {
            this._OpenStudyPageUrl = value;
            this.raisePropertyChanged('OpenStudyPageUrl');
        }

    }

    // Register the class as a type that inherits from Sys.UI.Control.

        ClearCanvas.ImageServer.Web.Application.Search.SearchPanel.registerClass('ClearCanvas.ImageServer.Web.Application.Search.SearchPanel', Sys.UI.Control);
     

    if (typeof(Sys) !== 'undefined') Sys.Application.notifyScriptLoaded();

}