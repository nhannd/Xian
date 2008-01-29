function @@FUNCTION_NAME@@()
{              
    control = document.getElementById('@@INPUT_CLIENTID@@');

    //alert('sending web service request');             
    var res;
    callObj = service.createCallOptions();
    callObj.funcName = '@@WEBSERVICE_OPERATION@@';
    callObj.async = false;
    callObj.timeOut = 1000;
    callObj.params = @@PARAMETER_FUNCTION@@();

    //callObj.params = 'path=' + control.value;
    
    resObj = service.ValidationServices.callService(callObj, res);

    if (!resObj.error)
    {
        result = eval(resObj.value); 

        if (!result.Success)
        {
            if (result.ErrorCode == -5000)
            {
                var ans = window.confirm('Unable to validate @@INPUT_NAME@@ : ' + result.ErrorText + '\nDo you want to ignore it? You can re-validate it in the future');
                if (ans)
                {
                    return true;
                }
            }
        }
        else
        {
        }
        return result.Success;
    }
    else
    {
        alert('Error occured while calling @@WEBSERVICE_OPERATION@@ at @@WEBSERVICE_URL@@ : ' + resObj.errorDetail.string);
    }
}
