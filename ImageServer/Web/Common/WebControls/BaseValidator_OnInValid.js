function GetStringWidth(ElemStyle, text) 
{
    var newSpan = document.createElement('span');
    newSpan.style.fontSize = ElemStyle.fontSize;
    newSpan.style.fontFamily = ElemStyle.fontFamily;
    newSpan.style.fontStyle = ElemStyle.fontStyle;
    newSpan.style.fontVariant = ElemStyle.fontVariant;
    newSpan.style.fontWieght = ElemStyle.fontWieght;
    newSpan.innerText = text;
    document.body.appendChild(newSpan);
    var iStringWidth = newSpan.offsetWidth;
    newSpan.removeNode(true);
    return iStringWidth;
}

function @@ERROR_FUNCTION_NAME@@(error)
{
    input = document.getElementById('@@INPUT_CLIENTID@@');
    
    input.style.backgroundColor ='@@INPUT_NORMAL_BKCOLOR@@';

    helpCtrl = @@INVALID_INPUT_POPUP_CONTROL@@;
    if (helpCtrl!=null)
    {
        helpCtrl.style.visibility= 'visible';                                
        tooltip = @@INVALID_INPUT_POPUP_TOOLTIP@@;
        if (tooltip!=null)
        {
            tooltip.innerText= error;
            tooltip.style.width = GetStringWidth(tooltip.style, error);
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

