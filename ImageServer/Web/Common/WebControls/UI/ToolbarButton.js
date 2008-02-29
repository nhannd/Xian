// Register the namespace for the control.
Type.registerNamespace('ClearCanvas.ImageServer.Web.Common.WebControls.UI');

//
// Define the control properties.
//
ClearCanvas.ImageServer.Web.Common.WebControls.UI.ToolbarButton = function(element) { 
    ClearCanvas.ImageServer.Web.Common.WebControls.UI.ToolbarButton.initializeBase(this, [element]);
    
    this._EnabledImageUrl = null;
    this._DisabledImageUrl = null;
}

//
// Create the prototype for the control.
//

ClearCanvas.ImageServer.Web.Common.WebControls.UI.ToolbarButton.prototype = 
{
    initialize : function() {
        ClearCanvas.ImageServer.Web.Common.WebControls.UI.ToolbarButton.callBaseMethod(this, 'initialize');

    },
   
    
    add_onClientClick : function(handler) {
        this.get_events().addHandler('onClientClick', handler);
    },
    remove_onClientClick : function(handler) {
        this.get_events().removeHandler('onClientClick', handler);
    },
    raiseonClientClick : function(eventArgs) {   
        var handler = this.get_events().getHandler('onClientClick');
        if (handler) {
            handler(this, eventArgs);
        }
    },
    
   

    dispose : function() {
        $clearHandlers(this.get_element());

        ClearCanvas.ImageServer.Web.Common.WebControls.UI.ToolbarButton.callBaseMethod(this, 'dispose');
    },

    //
    // Event delegates
    //

    _onFocus : function(e) {
        if (this.get_element() && !this.get_element().disabled) {
            //this.get_element().className = this._highlightCssClass;          
        }
    },
    
    _onClientSideClick : function(e) {
        alert('hey');
    },
    
    _onBlur : function(e) {
        if (this.get_element() && !this.get_element().disabled) {
            // this.get_element().className = this._nohighlightCssClass;          
        }
    },


    //
    // Control properties
    //
    
    set_enable : function(value) {
        
        if (value)
            this.get_element().removeAttribute('disabled');
        else
            this.get_element().setAttribute('disabled',  'disabled');
        this.get_element().setAttribute('src', value? this._EnabledImageUrl:this._DisabledImageUrl);
            
        this.raisePropertyChanged('enabled');
    },

    get_EnabledImageUrl : function() {
        return this._EnabledImageUrl;
    },

    set_EnabledImageUrl : function(value) {
        this._EnabledImageUrl = value;
        this.raisePropertyChanged('EnabledImageUrl');
    },

    get_DisabledImageUrl : function() {
        return this._DisabledImageUrl;
    },

    set_DisabledImageUrl : function(value) {
        this._DisabledImageUrl = value;
        this.raisePropertyChanged('DisabledImageUrl');
    }


}

// Optional descriptor for JSON serialization.
//ClearCanvas.ImageServer.Web.Common.WebControls.UI.ToolbarButton.descriptor = {
//    properties: [   {name: 'onClientRowClick', type: events}                    ]
//}

// Register the class as a type that inherits from Sys.UI.Control.
ClearCanvas.ImageServer.Web.Common.WebControls.UI.ToolbarButton.registerClass('ClearCanvas.ImageServer.Web.Common.WebControls.UI.ToolbarButton', Sys.UI.Control);

if (typeof(Sys) !== 'undefined') Sys.Application.notifyScriptLoaded();
