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

if (window.__registeredTypes['ClearCanvas.ImageServer.Web.Application.Pages.Studies.SearchPanel']==null)
{

    Type.registerNamespace('ClearCanvas.ImageServer.Web.Application.Pages.Studies');

    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Constructor
    //
    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    ClearCanvas.ImageServer.Web.Application.Pages.Studies.SearchPanel = function(element) { 
        ClearCanvas.ImageServer.Web.Application.Pages.Studies.SearchPanel.initializeBase(this, [element]);
       
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Create the prototype for the control.
    //
    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    ClearCanvas.ImageServer.Web.Application.Pages.Studies.SearchPanel.prototype = 
    {
        initialize : function() {
            ClearCanvas.ImageServer.Web.Application.Pages.Studies.SearchPanel.callBaseMethod(this, 'initialize');        
            
            this._OnOpenButtonClickedHandler = Function.createDelegate(this,this._OnOpenButtonClicked);
            this._OnSendButtonClickedHandler = Function.createDelegate(this,this._OnSendButtonClicked);   
            this._OnStudyListRowClickedHandler = Function.createDelegate(this,this._OnStudyListRowClicked);
            this._OnStudyListRowDblClickedHandler = Function.createDelegate(this,this._OnStudyListRowDblClicked);
            this._OnLoadHandler = Function.createDelegate(this,this._OnLoad);
            Sys.Application.add_load(this._OnLoadHandler);
                 
        },
        
        dispose : function() {
            $clearHandlers(this.get_element());

            ClearCanvas.ImageServer.Web.Application.Pages.Studies.SearchPanel.callBaseMethod(this, 'dispose');
            
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
            if(openButton != null) openButton.add_onClientClick( this._OnOpenButtonClickedHandler );   

            var sendButton = $find(this._SendButtonClientID);
            if(sendButton != null) sendButton.add_onClientClick( this._OnSendButtonClickedHandler );   
                 
            var studylist = $find(this._StudyListClientID);
            studylist.add_onClientRowClick(this._OnStudyListRowClickedHandler);
            
            if(this._CanViewStudyDetails) {          
                studylist.add_onClientRowDblClick(this._OnStudyListRowDblClickedHandler);
            }
            
            this._updateToolbarButtonStates();
        },
        
        // called when the Open Study button is clicked
        _OnOpenButtonClicked : function(src, event)
        {
            this._openSelectedStudies();            
        },
        
        // called when the Send Study button is clicked
        _OnSendButtonClicked : function(src, event)
        {
            this._sendSelectedStudies();            
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
                
        
        _canRestoreStudy:function(row)
        {
            //"canrestore" is a custom attribute injected by the study list control
            return row.getAttribute('canrestore')=='true';
        },
        
         _canDeleteStudy:function(row)
        {
            //"candelete" is a custom attribute injected by the study list control
            return row.getAttribute('candelete')=='true';
        },
        
        _canMoveStudy:function(row)
        {
            //"canmove" is a custom attribute injected by the study list control
            return row.getAttribute('canmove')=='true';
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

        _sendSelectedStudies : function()
        {
            var studylist = $find(this._StudyListClientID);

            if (studylist!=null )
            {
                var rows = studylist.getSelectedRowElements();
                if (rows.length>0)
                {
                    var urlCount = 1;
                    
                    var url = "";
                    
                    for(i=0; i<rows.length; i++)
                    {
                        var instanceuid = this._getInstanceUid(rows[i]);
                        var serverae = this._getServerPartitionAE(rows[i]);
                        if (instanceuid!=undefined && serverae!=undefined)
                        {
                            if (urlCount == 1)
                            url = String.format('{0}?serverae={1}&studyuid{3}={2}', this._SendStudyPageUrl, serverae, instanceuid, urlCount);
                            else
                            url = String.format('{0}&studyuid{3}={2}', url, serverae, instanceuid, urlCount);
                            urlCount++;
                        }
                    }
                    window.open(url);
                }
            }
        },
        
        _updateToolbarButtonStates : function()
        {
            var studylist = $find(this._StudyListClientID);
                      
            this._enableDeleteButton(false);
            this._enableOpenStudyButton(false);
            this._enableSendStudyButton(false);
            this._enableRestoreButton(false);
                
            if (studylist!=null )
            {
                var rows = studylist.getSelectedRowElements();
                
                if (rows!=null && rows.length>0)
                {
		            var selectedStudyCount = rows.length; 
                    var canMoveCount=0;   
		            var canDeleteCount=0; 
		            var canRestoreCount = 0;                  
                    if (rows.length>0)
                    {
					    for(i=0; i<rows.length; i++)
                        {
                            if (this._canMoveStudy(rows[i]))
                            {
                                canMoveCount++;
                            }
                            if (this._canDeleteStudy(rows[i]))
                            {
                                canDeleteCount++;
                            }
                            if (this._canRestoreStudy(rows[i]))
                            {
                                canRestoreCount++;
                            }
                        }
                    }
                    // always enabled open button when a row is selected
                    this._enableOpenStudyButton(true);
    				
                    this._enableDeleteButton(canDeleteCount==selectedStudyCount);
                    this._enableSendStudyButton(canMoveCount==selectedStudyCount);
                    this._enableRestoreButton(canRestoreCount==selectedStudyCount);
                    
                }
                
            }
        },
        
        
        
        _enableDeleteButton : function(en)
        {
            var deleteButton = $find(this._DeleteButtonClientID);
            if(deleteButton != null) deleteButton.set_enable(en);
        },
        
        _enableOpenStudyButton : function(en)
        {
            var openButton = $find(this._OpenButtonClientID);
            if(openButton != null) openButton.set_enable(en);
        },
        
        _enableSendStudyButton : function(en)
        {
            var sendButton = $find(this._SendButtonClientID);
            if(sendButton != null) sendButton.set_enable(en);
        },
        
        _enableRestoreButton : function(en)
        {
            var deleteButton = $find(this._RestoreButtonClientID);
            if(deleteButton != null) deleteButton.set_enable(en);
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
        
        get_RestoreButtonClientID : function() {
            return this._RestoreButtonClientID;
        },

        set_RestoreButtonClientID : function(value) {
            this._RestoreButtonClientID = value;
            this.raisePropertyChanged('RestoreButtonClientID');
        },
        
        get_DeleteButtonClientID : function() {
            return this._DeleteButtonClientID;
        },

        set_DeleteButtonClientID : function(value) {
            this._DeleteButtonClientID = value;
            this.raisePropertyChanged('DeleteButtonClientID');
        },
        
        get_SendButtonClientID : function() {
            return this._SendButtonClientID;
        },

        set_SendButtonClientID : function(value) {
            this._SendButtonClientID = value;
            this.raisePropertyChanged('SendButtonClientID');
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
        },
        
        get_SendStudyPageUrl : function() {
            return this._SendStudyPageUrl;
        },
       
        set_SendStudyPageUrl : function(value) {
            this._SendStudyPageUrl = value;
            this.raisePropertyChanged('SendStudyPageUrl');
        },
        
        get_CanViewStudyDetails : function() {
            return this._CanViewStudyDetails;
        },
       
        set_CanViewStudyDetails : function(value) {
            this._CanViewStudyDetails = value;
            this.raisePropertyChanged('CanViewStudyDetails');
        }
    }

    // Register the class as a type that inherits from Sys.UI.Control.

        ClearCanvas.ImageServer.Web.Application.Pages.Studies.SearchPanel.registerClass('ClearCanvas.ImageServer.Web.Application.Pages.Studies.SearchPanel', Sys.UI.Control);
     

    if (typeof(Sys) !== 'undefined') Sys.Application.notifyScriptLoaded();

}