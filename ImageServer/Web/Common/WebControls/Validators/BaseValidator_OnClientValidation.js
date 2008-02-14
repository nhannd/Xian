function @@CLIENTID@@_OnClientSideValidation()
{
    //alert('Calling @@CLIENTID@@_ClientSideEvaluator');
    validator = new @@CLIENTID@@_ClientSideEvaluator();
    result = validator.OnEvaluate();
    
    if (result.OK ==true)
    {
        validator.OnValidationPassed();
        return true;
     }
    else
    {
        validator.OnValidationFailed(result);
        return false;
    }
    
}

