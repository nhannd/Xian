function addLoadEvent(func) {
    var oldonload = window.onload;
    if (typeof window.onload != 'function') {
        window.onload = func;
    } else {
        window.onload = function() {
          if (oldonload) {
              oldonload();
          }
          func();
        }
    }
}

function initwebservice()
{
    service.useService('@@WEBSERVICE_URL@@?WSDL','ValidationServices');
    service.onserviceavailable = onserviceavailable();
}
   
function onserviceavailable(){
    //alert('web service ready');
}
   
addLoadEvent(function() {
    //alert('Adding load event');
    initwebservice();
});     
