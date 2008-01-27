function @@FUNCTION_NAME@@()
{
        input = document.getElementById('@@INPUT_CLIENTID@@');
        
        if (input['validatorscounter']=='1')
        {

            // only myself is attached to this input, it's ok to clear the background
            input.style.backgroundColor ='@@INPUT_NORMAL_BKCOLOR@@';

        }
        else
        {
            if (input['calledvalidatorcounter']==0)
            {
                // I am the first validator called to check the input, it's safe to clear the background
                input.style.backgroundColor ='@@INPUT_NORMAL_BKCOLOR@@';
            }
            
                
        }


        helpCtrl = @@INVALID_INPUT_POPUP_CONTROL@@;
        if (helpCtrl!=null)
        {
            // I am not sharing the popup help control with any other validators. It's safe to hide it 
            if (helpCtrl['shared']!='true')
                helpCtrl.style.visibility= 'hidden'; 
            else
            {
                if (input['calledvalidatorcounter']==0)
                {
                    // I am sharing the popup help control with any other validators, and I am the first one to validate the input
                    // So it's safe to hide it 
                    helpCtrl.style.visibility= 'hidden'; 
                }
            }
        }

        input['calledvalidatorcounter']++;
        if (input['calledvalidatorcounter']==input['validatorscounter'])
        {
            // no more validator in the pipe, let's reset the calledvalidatorcounter value so that 
            // next time we validate the input we start from 0.
            input['calledvalidatorcounter']=0;
        }
}
