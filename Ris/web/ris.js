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
        }
    };
    
    // redefine some window functions to use Ris versions
    window.confirm = Ris.confirm;
    window.alert = Ris.alert;
}