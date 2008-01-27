function @@FUNCTION_NAME@@()
{
        result = true;

        input = document.getElementById('@@INPUT_CLIENTID@@');       
        
        if (input.value==null || input.value=='')
        {
            result = false;
        }
    
        return  result;
}