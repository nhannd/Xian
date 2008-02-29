// Register the namespace for the control.
//alert('gridview');

Type.registerNamespace('ClearCanvas.ImageServer.Web.Common.WebControls.UI');

//
// Define the control properties.
//
ClearCanvas.ImageServer.Web.Common.WebControls.UI.GridView = function(element) { 
    ClearCanvas.ImageServer.Web.Common.WebControls.UI.GridView.initializeBase(this, [element]);
    
    this._SelectedRowIndicesField = null;
    this._SelectedRowStyle = null;
    this._SelectedRowCSS = null;
    this._UnSelectedRowStyle = null;
    this._UnSelectedRowCSS = null;
}

//
// Create the prototype for the control.
//

ClearCanvas.ImageServer.Web.Common.WebControls.UI.GridView.prototype = 
{
    initialize : function() {
        ClearCanvas.ImageServer.Web.Common.WebControls.UI.GridView.callBaseMethod(this, 'initialize');

        
        for(i = 0; i < this.get_element().rows.length; i++)
        {
            r =this.get_element().rows[i];
            $addHandlers(r, 
                     { 
                        'click' : this._onCellClick  ,
                        'dblclick' : this._onCellDblClick
                     }, this);
        }
    },
    
    
    
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
    
    
    getSelectedRowElements : function() {
        r = this.get_element().rows;
        r2=new Array();
        for(i=0; i< r.length; i++)
        {
            if (r[i].getAttribute('isdatarow')=='true' && r[i].getAttribute('selected')=='true')
                r2[r2.length]=r[i];
        }
        
        return r2;
    },
    
    _onCellDblClick : function(e) {
        row = e.target.parentNode ;
        if (this.get_element() && !this.get_element().disabled) 
        {
            var ev = new Sys.EventArgs();
            ev.row = row;
            this.raiseonClientRowDblClick(ev);
        }
    },
    
    _clearSelections : function(){
        // unselect those currently selected
        var rows = this.getSelectedRowElements();
        for(i=0; i<rows.length; i++)
        {
            var row = rows[i];
            row.style.cssText = this._UnSelectedRowStyle;
            row.className = this._UnSelectedRowCSS;
            row.setAttribute('selected', 'false');
        }
        
        var f = $get(this._SelectedRowIndicesField);
        f.value='';
                        
    },
    
    _onCellClick : function(e) {
        row = e.target.parentNode ;
        //alert(row);
        if (this.get_element() && !this.get_element().disabled ) 
        {
            if (row.getAttribute('isdatarow')=='true')
            {
                multipleSelectionMode = true;//e.altKey;
                if (multipleSelectionMode)
                {
                    alreadyselected = row.getAttribute('selected')!=undefined && row.getAttribute('selected')=='true';
                    if (alreadyselected)
                    {
                        // unselect it
                        row.style.cssText = this._UnSelectedRowStyle;
                        
                        row.className = this._UnSelectedRowCSS;
                        
                        f = $get(this._SelectedRowIndicesField);
                        
                        f2 = f.value.split(',');
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
                    else
                    {
                        row.style.cssText = this._SelectedRowStyle;
                        
                        row.className = this._SelectedRowCSS;
                        
                        
                        f = $get(this._SelectedRowIndicesField);

                        if (f.value==null || f.value=='')
                            f.value=row.getAttribute('rowIndex');
                        else
                            f.value +=  ',' + row.getAttribute('rowIndex');
                        row.setAttribute('selected', 'true');
                    }
                    
                    
                }
                else
                {
                    alreadyselected = row.getAttribute('selected')!=undefined && row.getAttribute('selected')=='true';
                    if (alreadyselected)
                    {
                        // unselect it
                        row.style.cssText = this._UnSelectedRowStyle;
                        
                        row.className = this._UnSelectedRowCSS;
                        
                        f = $get(this._SelectedRowIndicesField);
                        
                        f2 = f.value.split(',');
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
                    else
                    {
                        this._clearSelections();
                        
                        row.style.cssText = this._SelectedRowStyle;                        
                        row.className = this._SelectedRowCSS;                        
                        
                        f = $get(this._SelectedRowIndicesField);
                        f.value=row.getAttribute('rowIndex');                        
                        row.setAttribute('selected', 'true');
                        
                        
                        
                    }
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
