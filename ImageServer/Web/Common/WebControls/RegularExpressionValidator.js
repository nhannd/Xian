function @@FUNCTION_NAME@@()
{
                                
    input = document.getElementById('@@INPUT_CLIENTID@@');
   
    var re = new RegExp('@@REGULAR_EXPRESSION@@');
    
    if (input.value=='')
    {
        result = true;
    }
    else if (input.value.match(re)) {
        result = true;
    } else {
        result = false;
    }
    
    return result;

}