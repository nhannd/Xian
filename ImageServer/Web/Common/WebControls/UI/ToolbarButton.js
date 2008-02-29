// Register the namespace for the control.
Type.registerNamespace('ClearCanvas.ImageServer.Web.Common.WebControls.UI');

//
// Define the control properties.
//
ClearCanvas.ImageServer.Web.Common.WebControls.UI.ToolbarButton = function(element) { 
    ClearCanvas.ImageServer.Web.Common.WebControls.UI.ToolbarButton.initializeBase(this, [element]);
    
    this._EnabledImageUrl = null;
    this._DisabledImageUrl = null;
    this._HoverImageUrl = null;
}

//
// Create the prototype for the control.
//

ClearCanvas.ImageServer.Web.Common.WebControls.UI.ToolbarButton.prototype = 
{
    initialize : function() {
        ClearCanvas.ImageServer.Web.Common.WebControls.UI.ToolbarButton.callBaseMethod(this, 'initialize');
        
         $addHandlers(this.get_element(), 
                     { 
                        'mouseover' : this._onMouseOver,
                        'mouseout'  : this._onMouseOut
                     }, 
                     this);
                     
    },
   
 
   

    dispose : function() {
        $clearHandlers(this.get_element());

        ClearCanvas.ImageServer.Web.Common.WebControls.UI.ToolbarButton.callBaseMethod(this, 'dispose');
    },

    //
    // Event handlers
    //

    _onMouseOver : function(e) 
    {
        if (this.get_element() && !this.get_element().disabled) {     
            if (this._HoverImageUrl!=undefined && this._HoverImageUrl!=null  && this._HoverImageUrl!='')
                this.get_element().src =  this._HoverImageUrl;
        }
    },

    _onMouseOut : function(e) 
    {
        if (this.get_element() && !this.get_element().disabled) {     
            this.get_element().src =  this._EnabledImageUrl;
        }
        else
        {
            this.get_element().src =  this._DisabledImageUrl;
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
    },
    
    
    get_HoverImageUrl : function() {
        return this._HoverImageUrl;
    },

    set_HoverImageUrl : function(value) {
        this._HoverImageUrl = value;
        this.raisePropertyChanged('HoverImageUrl');
    }


}

// Optional descriptor for JSON serialization.
//ClearCanvas.ImageServer.Web.Common.WebControls.UI.ToolbarButton.descriptor = {
//    properties: [   {name: 'onClientRowClick', type: events}                    ]
//}

// Register the class as a type that inherits from Sys.UI.Control.
ClearCanvas.ImageServer.Web.Common.WebControls.UI.ToolbarButton.registerClass('ClearCanvas.ImageServer.Web.Common.WebControls.UI.ToolbarButton', Sys.UI.Control);

if (typeof(Sys) !== 'undefined') Sys.Application.notifyScriptLoaded();
