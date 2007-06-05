/*
    Ris
*/

if(window.external)
{
    var Ris = {
        
        // message - confirmation message to display
        // type - a string containing either "YesNo" or "OkCancel" (not case-sensitive)
        // returns true if the user pressed Yes or OK, false otherwise
        confirm: function(message, type)
        {
            return window.external.Confirm(message || "", type || "OkCancel");
        },
        
        alert: function(message)
        {
            window.external.Alert(message || "");
        },
        
        resolveStaffName: function(query)
        {
            return window.external.ResolveStaffName(query || "");
        },
        
        getDateFormat: function()
        {
            return window.external.DateFormat;
        },
        
        getTimeFormat: function()
        {
            return window.external.TimeFormat;
        },
        
        getDateTimeFormat: function()
        {
            return window.external.DateTimeFormat;
        },
        
        getData: function(tag)
        {
            return window.external.GetData(tag);
        },
        
        setData: function(tag, data)
        {
            window.external.SetData(tag, data);
        }
    };
    
    // redefine some browser functions to use Ris versions
    window.confirm = Ris.confirm;
    window.alert = Ris.alert;
}