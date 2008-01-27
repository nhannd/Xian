function @@FUNCTION_NAME@@()
{
    input = document.getElementById('@@INPUT_CLIENTID@@');
    compareCtrl = document.getElementById('@@COMPARE_INPUT_CLIENTID@@');
   
    result = true;
    if (input.value!=null && input.value!='' && compareCtrl!=null && compareCtrl.value!='')
    {
        compareValue = parseInt(compareCtrl.value);
        controlValue = parseInt(input.value);
        result = controlValue >= @@MIN_VALUE@@ && controlValue<= @@MAX_VALUE@@ && controlValue @@COMPARISON_OP@@ compareValue;
    }   
    else
    {
        result = false;
    }
    
    return result;
}