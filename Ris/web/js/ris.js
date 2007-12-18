/*
    Ris
*/

if(window.external)
{
    var Ris = {
    
        // parse filter to customize some aspects of JSML parsing without modifying the JSML.js script
        _jsmlParserFilter: function(key, value)
        {
            // if it has the properties of a staff person, replace it with a Staff object
            if(value && value.hasOwnProperty('staffId') && value.hasOwnProperty('staffName'))
                return new Staff(value.staffId, value.staffName, value.staffType);
            return value;
        },
        
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
            var staffSummary = JSML.parse(window.external.ResolveStaffName(query || ""));
            if(staffSummary == null)
                return null;
            
            return new Staff(
                staffSummary.StaffId,
                staffSummary.Name.FamilyName + ", " + staffSummary.Name.GivenName,
                staffSummary.StaffType.Value);
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
        
        getTag: function(tag)
        {
            return window.external.GetTag(tag);
        },
        
        setTag: function(tag, data)
        {
            window.external.SetTag(tag, data);
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
		},
		
		// obtains a proxy to a RIS web-service
		getService: function(serviceContractName)
		{
		    var innerProxy = window.external.GetServiceProxy(serviceContractName);
		    var operations = JSML.parse(innerProxy.GetOperationNames());
		    
		    var proxy = { _innerProxy: innerProxy };
		    operations.each(
		        function(operation)
		        {
		            // create camelCase (as opposed to PascalCase) version of the operation name
		            var ccOperation = operation.slice(0,1).toLowerCase() + operation.slice(1);
		            
		            // allow the operation to be invoked via either camel or pascal casing
		            proxy[operation] = proxy[ccOperation] = 
		                function(request)
		                {
		                    return JSML.parse( this._innerProxy.InvokeOperation(operation, JSML.create(request, "requestData")) );
		                };
		        });
		    return proxy;
		},
		
		getPatientDataService: function()
		{
		    return this.getService("ClearCanvas.Ris.Application.Common.BrowsePatientData.IBrowsePatientDataService, ClearCanvas.Ris.Application.Common");
		},
		
		// gets the healthcare context in which the page is running
		// the healthcare context is an object that contains all entity-refs, etc., that the page
		// can use as keys to load data
		getHealthcareContext: function()
		{
		    return JSML.parse(window.external.GetHealthcareContext());
		}
    };
    
    // install global JSML parser filter
    JSML.setParseFilter(Ris._jsmlParserFilter);
    
    // redefine some browser functions to use Ris versions
    window.confirm = Ris.confirm;
    window.alert = Ris.alert;
}

/*
    Staff class
*/
function Staff(id, name, type)
{
    this.staffId = id;
    this.staffName = name;
    this.staffType = type;
}
// override the toString function - this just makes it work seamlessly with the Table view                 
Staff.prototype.toString = function() { return this.staffName; }           
