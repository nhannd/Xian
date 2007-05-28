/*
    Ris
*/
var Ris = {
    
    // message - confirmation message to display
    // type - a string containing either "YesNo" or "OkCancel" (not case-sensitive)
    // returns true if the user pressed Yes or OK, false otherwise
    confirm: function(message, type)
    {
        return window.external.Confirm(message || "", type || "OkCancel");
    },
    
    resolveStaffName: function(query)
    {
        return window.external.ResolveStaffName(query || "");
    },
    
    getData: function()
    {
        return window.external.Data;
    },
    
    setData: function(data)
    {
        window.external.Data = data;
    }
    
};

// redefine global "confirm" to use Ris if possible
if(window.external)
{
   window.confirm = Ris.confirm;
}
