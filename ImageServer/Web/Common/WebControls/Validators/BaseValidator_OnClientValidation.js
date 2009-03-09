function @@CLIENTID@@_OnClientSideValidation()
{
    validator = new @@CLIENTID@@_ClientSideEvaluator();
    result = validator.OnEvaluate();
    
    if (result.OK)
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

