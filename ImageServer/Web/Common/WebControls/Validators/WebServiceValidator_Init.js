function addLoadEvent(func) {
    if (window.attachEvent) { window.attachEvent('onload', func); }
    else if (window.addEventListener) { window.addEventListener('load', func, false); }
    else { document.addEventListener('load', func, false); }
}

function initwebservice() {
    //service is defined in the Master Page. This method only works in IE, other browsers will be server-side validated.
    if (navigator.appName == "Microsoft Internet Explorer") {
        service.useService('@@WEBSERVICE_URL@@?WSDL', 'ValidationServices');
        service.onserviceavailable = onserviceavailable();
    }
}
   
function onserviceavailable(){
    //alert('web service ready');
}
   
addLoadEvent(function() {
    //alert('Adding load event');
    initwebservice();
});     
