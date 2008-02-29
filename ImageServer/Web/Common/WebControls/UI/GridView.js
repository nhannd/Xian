/// This script contains the javascript component class for the gridview class
/// 
/// The control contains the following public methods:
///     getSelectedRowElements() : returns list of row elements (TR) which are being selected.
///     _selectRow() : select a specifi


// Register the namespace for the control.
//
Type.registerNamespace('ClearCanvas.ImageServer.Web.Common.WebControls.UI');

//
// Define the control properties.
//
ClearCanvas.ImageServer.Web.Common.WebControls.UI.GridView = function(element) { 
    ClearCanvas.ImageServer.Web.Common.WebControls.UI.GridView.initializeBase(this, [element]);
    
    this._SelectedRowIndicesField = null;
    this._SelectedRowStyle = null;
    this._SelectedRowCSS = null;
    this._AlternateRowStyle = null;
    this._AlternateRowCSS = null;
    this._UnSelectedRowStyle = null;
    this._UnSelectedRowCSS = null;
}

//
// Create the prototype for the control.
//
//
ClearCanvas.ImageServer.Web.Common.WebControls.UI.GridView.prototype = 
{
    initialize : function() {
        ClearCanvas.ImageServer.Web.Common.WebControls.UI.GridView.callBaseMethod(this, 'initialize');

        // add click and double-click handlers on each row
        for(i = 0; i < this.get_element().rows.length; i++)
        {
            var row =this.get_element().rows[i];
            $addHandlers(row, 
                     { 
                        'click' : this._onCellClick,
                        'dblclick' : this._onCellDblClick
                     }, 
                     this);
        }
    },
    
    //
    // public methods
    //
    getSelectedRowElements : function() {
        var rows = this.get_element().rows;
        var selectedRows = new Array();
        for(var i=0; i< rows.length; i++)
        {
            if (rows[i].getAttribute('isdatarow')=='true' && rows[i].getAttribute('selected')=='true')
                selectedRows[selectedRows.length]=rows[i];
        }
        
        return selectedRows;
    },
    
    selectRow : function (rowIndex)
    {
        var row = this.get_element().rows[rowIndex];
        this._selectRow(row);
    },
    
    unselectRow : function (rowIndex)
    {
        var row = this.get_element().rows[rowIndex];
        this._unselectRow(row);
    },
    
    clearSelections : function()
    {
        // unselect those currently selected
        var rows = this.getSelectedRowElements();
        for(var i=0; i<rows.length; i++)
        {
            var row = rows[i];            
            this._unselectRow(row);           
        }
        
        var f = $get(this._SelectedRowIndicesField);
        f.value='';
                        
    },
    
    
    //
    // Events
    //
    
    add_onClientRowClick : function(handler) {
        this.get_events().addHandler('onClientRowClick', handler);
    },
    remove_onClientRowClick : function(handler) {
        this.get_events().removeHandler('onClientRowClick', handler);
    },
    raiseonClientRowClick : function(eventArgs) {   
        var handler = this.get_events().getHandler('onClientRowClick');
        if (handler) {
            handler(this, eventArgs);
        }
    },
    
    
    add_onClientRowDblClick : function(handler) {
        this.get_events().addHandler('onClientRowDblClick', handler);
    },
    remove_onClientRowDblClick : function(handler) {
        this.get_events().removeHandler('onClientRowDblClick', handler);
    },
    raiseonClientRowDblClick : function(eventArgs) {   
        var handler = this.get_events().getHandler('onClientRowDblClick');
        if (handler) {
            handler(this, eventArgs);
        }
    },
    
    dispose : function() {
        $clearHandlers(this.get_element());

        ClearCanvas.ImageServer.Web.Common.WebControls.UI.GridView.callBaseMethod(this, 'dispose');
    },

    //
    // Event delegates
    //

    _onFocus : function(e) {
        if (this.get_element() && !this.get_element().disabled) {       
        }
    },
   
    
    _onCellDblClick : function(e) {
        var row = e.target.parentNode ;
        if (this.get_element() && !this.get_element().disabled) 
        {
            this._selectRow(row);            
            
            var ev = new Sys.EventArgs();
            ev.row = row;            
            this.raiseonClientRowDblClick(ev);
        }
    },
    
    
    _onCellClick : function(e) {
        var row = e.target.parentNode ;
        //alert(row);
        if (this.get_element() && !this.get_element().disabled ) 
        {
            if (row.getAttribute('isdatarow')=='true')
            {
                var multipleSelectionMode = e.ctrlKey;
                if (multipleSelectionMode)
                {
                    var alreadyselected = row.getAttribute('selected')!=undefined && row.getAttribute('selected')=='true';
                    if (alreadyselected)
                    {
                        // unselect it
                        this._unselectRow(row);                                                
                        
                    }
                    else
                    {
                        this._selectRow(row);
                    }
                    
                    
                }
                else
                {
                    this.clearSelections();
                    this._selectRow(row);
                }
            }
            
            var ev = new Sys.EventArgs();
            ev.row = row;
            this.raiseonClientRowClick(ev);
            
        }
    },
    

    _onBlur : function(e) {
        if (this.get_element() && !this.get_element().disabled) {      
        }
    },

    //
    // Private methods
    //
    _selectRow : function (row)
    {
        if (this.get_element() && !this.get_element().disabled ) 
        {
            row.style.cssText = this._SelectedRowStyle;                    
            row.className = this._SelectedRowCSS;
            
            
            var f = $get(this._SelectedRowIndicesField);

            if (f.value==null || f.value=='')
                f.value=row.getAttribute('rowIndex');
            else
                f.value +=  ',' + row.getAttribute('rowIndex');
                
            row.setAttribute('selected', 'true');
        }
        
    },
    
    _unselectRow : function (row)
    {
        if (this.get_element() && !this.get_element().disabled ) 
        {
            var rowIndex = parseInt(row.getAttribute('rowIndex'));
            if (rowIndex%2==0)
            {
                row.style.cssText = this._UnSelectedRowStyle ;                       
                row.className = this._UnSelectedRowCSS;
            }
            else
            {
                row.style.cssText = (this._AlternatingRowStyle!=null && this._AlternatingRowStyle!=undefined) ? this._AlternatingRowStyle: this._UnSelectedRowStyle;                        
                row.className = (this._AlternatingRowCSS!=null && this._AlternatingRowCSS!=undefined)? this._AlternatingRowCSS:this._UnSelectedRowCSS;
            
            }
            
            //update the hidden field which stores the selected row indices
            var f = $get(this._SelectedRowIndicesField);                        
            var f2 = f.value.split(',');
            f.value='';
            for(i =0; i<f2.length; i++)
            {
                if (row.getAttribute('rowIndex')!=f2[i])
                {
                    if (f.value=='')
                        f.value =   f2[i];
                    else
                        f.value +=  ',' + f2[i];
                }    
                    
            }
            
             row.setAttribute('selected', 'false');
        }
        
    },
    
    

    //
    // Control properties
    //

    get_SelectedRowIndicesField : function() {
        return this._SelectedRowIndicesField;
    },

    set_SelectedRowIndicesField : function(value) {
        this._SelectedRowIndicesField = value;
        this.raisePropertyChanged('SelectedRowIndicesField');
    },

    get_SelectedRowStyle : function() {
        return this._SelectedRowStyle;
    },

    set_SelectedRowStyle : function(value) {
        this._SelectedRowStyle = value;
        this.raisePropertyChanged('SelectedRowStyle');
    },
    
    
    get_SelectedRowCSS : function() {
        return this._SelectedRowCSS;
    },

    set_SelectedRowCSS : function(value) {
        this._SelectedRowCSS = value;
        this.raisePropertyChanged('SelectedRowCSS');
    },
    
    get_AlternatingRowStyle : function() {
        return this._AlternatingRowStyle;
    },

    set_AlternatingRowStyle : function(value) {
        this._AlternatingRowStyle = value;
        this.raisePropertyChanged('AlternatingRowStyle');
    },
    
    
    get_AlternatingRowCSS : function() {
        return this._AlternatingRowCSS;
    },

    set_AlternatingRowCSS : function(value) {
        this._AlternatingRowCSS = value;
        this.raisePropertyChanged('AlternatingRowCSS');
    },
    
    get_UnSelectedRowStyle : function() {
        return this._UnSelectedRowStyle;
    },

    set_UnSelectedRowStyle : function(value) {
        this._UnSelectedRowStyle = value;
        this.raisePropertyChanged('UnSelectedRowStyle');
    },
    
    
    get_UnSelectedRowCSS : function() {
        return this._UnSelectedRowCSS;
    },

    set_UnSelectedRowCSS : function(value) {
        this._UnSelectedRowCSS = value;
        this.raisePropertyChanged('UnSelectedRowCSS');
    }




}

// Optional descriptor for JSON serialization.
//ClearCanvas.ImageServer.Web.Common.WebControls.UI.GridView.descriptor = {
//    properties: [   {name: 'onClientRowClick', type: events}                    ]
//}

// Register the class as a type that inherits from Sys.UI.Control.
ClearCanvas.ImageServer.Web.Common.WebControls.UI.GridView.registerClass('ClearCanvas.ImageServer.Web.Common.WebControls.UI.GridView', Sys.UI.Control);

if (typeof(Sys) !== 'undefined') Sys.Application.notifyScriptLoaded();
