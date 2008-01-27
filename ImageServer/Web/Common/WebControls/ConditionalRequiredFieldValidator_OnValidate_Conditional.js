function @@FUNCTION_NAME@@()
{
        result = true;

        condition = document.getElementById('@@CONDITIONAL_CONTROL_CLIENTID@@');
        input = document.getElementById('@@INPUT_CLIENTID@@');
        
        
        if (condition!=null && condition.checked == '@@REQUIRED_WHEN_CHECKED@@')
        {
            if (input.value==null || input.value=='')
            {
                result = false;
            }
        }

        return  result;
}