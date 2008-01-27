function @@FUNCTION_NAME@@()
{
    input = document.getElementById('@@INPUT_CLIENTID@@');
    
    result = true;
    if (input.value!=null && input.value!='')
    {
        if (!isNaN(input.value)){
            portValue = parseInt(input.value);
            result = portValue >= @@MIN_VALUE@@ && portValue<= @@MAX_VALUE@@;
        }
        else
        {
            result = false;
        }
       
        
    }   
    else
    {
        result = false;
    }
    
    return result;
}