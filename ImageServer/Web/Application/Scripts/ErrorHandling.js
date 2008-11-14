Sys.Application.add_load(AppLoad);
 
function AppLoad()
{
  Sys.WebForms.PageRequestManager.getInstance().add_endRequest(EndRequest);
  Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(BeginRequest);
}
 
function BeginRequest(sender, args) {
  // Clear the error if it's visible from a previous request.
  if ($get('ErrorPanel').style.visibility == "visible")
    CloseError();
}
 
function EndRequest(sender, args) {
  // Check to see if there's an error on this request.
  if (args.get_error() != undefined)
  {
    // If there is, show the custom error.
    $get('ErrorPanel').style.visibility = "visible";
    $get('ErrorPanel').innerHTML = args.get_error();
 
    // Let the framework know that the error is handled, 
    //  so it doesn't throw the JavaScript alert.
    args.set_errorHandled(true);
  }
}
 
function CloseError() {
  // Hide the error div.
  $get('ErrorPanel').style.visibility = "hidden";
}