var _IE = document.all;


function scaleWidth()
{
/*
 * This is the pattern of the old function, which tries to format lines of
 * text into an optimal width, for readability. It doesn't apply to us, since
 * we use tables almost everywhare, but it'll have to do for now.
 *
 */
	var windowWidth = document.body.clientWidth;
	var minimumTextHeight = "9px";
	var optimalLineLength = "70em";
	var extraAccounting = "0em";
	var optimalSize = windowWidth / (parseInt(optimalLineLength) + parseInt(extraAccounting));

	if (optimalSize >= parseInt(minimumTextHeight))
	{
		document.body.style.fontSize = optimalSize + "px";
	}
	else
	{
		document.body.style.fontSize = parseInt(minimumTextHeight) + "px";
	}
	
	return true;
}
