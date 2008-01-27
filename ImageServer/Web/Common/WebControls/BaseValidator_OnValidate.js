function @@FUNCTION_NAME@@()
{
    result = @@CLIENT_SIDE_EVALUATION_FUNCTION@@();
    if (result==true)
    {
        @@CLIENT_SIDE_ON_VALID_FUNCTION@@();
        return true;
     }
    else
    {
        @@CLIENT_SIDE_ON_INVALID_FUNCTION@@('@@INVALID_INPUT_MESSAGE@@');
        return false;
    }
    
}