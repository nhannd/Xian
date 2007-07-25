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
        
		getActionHtml: function(labelSearch, actionLabel)
		{
			return window.external.GetActionHtml(labelSearch, actionLabel);
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
        
		getResponseData: function(requestObject, requestObjectName)
		{
            var requestJsml = JSML.create(requestObject, requestObjectName);
			var responseJsml = window.external.GetJsmlData(requestJsml);
			var responseObject = JSML.parse(responseJsml);
			return responseObject;
		},
		
        getData: function(tag)
        {
            return window.external.GetData(tag);
        },
        
        setData: function(tag, data)
        {
            window.external.SetData(tag, data);
        },
		
		formatDate: function(date)
		{
			return date ? window.external.FormatDate(date.toISOString()) : "";
		},

		formatTime: function(date)
		{
			return date ? window.external.FormatTime(date.toISOString()) : "";
		},

		formatDateTime: function(date)
		{
			return date ? window.external.FormatDateTime(date.toISOString()) : "";
		},

		formatAddress: function(address)
		{
			return address ? window.external.FormatAddress(JSML.create(address, "Address")) : "";
		},

		formatHealthcard: function(healthcard)
		{
			return healthcard ? window.external.FormatHealthcard(JSML.create(healthcard, "Healthcard")) : "";
		},

		formatMrn: function(mrn)
		{
			return mrn ? window.external.FormatMrn(JSML.create(mrn, "Mrn")) : "";
		},

		formatPersonName: function(personName)
		{
			return personName ? window.external.FormatPersonName(JSML.create(personName, "PersonName")) : "";
		},

		formatTelephone: function(telephone)
		{
			return telephone ? window.external.FormatTelephone(JSML.create(telephone, "Telephone")) : "";
		}
    };
    
    // redefine some browser functions to use Ris versions
    window.confirm = Ris.confirm;
    window.alert = Ris.alert;
}