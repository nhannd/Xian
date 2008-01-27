function @@CLIENT_SIDE_VALIDATION_FUNCTION@@()
{

        result = true;

        input = document.getElementById('@@INPUT_CLIENTID@@');
        
        if (input.value==null || input.value.length<@@MIN_LENGTH@@ || input.value.length>@@MAX_LENGTH@@)
        {
            result = false;
        }
    

        return  result;
}