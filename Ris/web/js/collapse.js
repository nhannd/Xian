function toggleCollapsed(loc)
{	
   if(document.getElementById)
   {
      var foc = loc.firstChild;

      foc = loc.firstChild.innerHTML ? loc.firstChild : loc.firstChild.nextSibling;
      foc.innerHTML = foc.innerHTML == '+' ? '-' : '+';
      foc = loc.parentNode.nextSibling.style ? loc.parentNode.nextSibling : loc.parentNode.nextSibling.nextSibling;
      foc.style.display = foc.style.display == 'block' ? 'none' : 'block';
   }
}

/*
 * toggleCollapsedSection
 * 
 * Shows/Hides the content of a section container on a preview page
 * 
 * NOTE: This could probably be refactored to use an id instead of a location, and then navigate
 *       through the DOM based on the id, making the code less susceptible to breaking if the DOM
 *       changes unexpectedly. 
 */

function toggleCollapsedSection(loc)
{	
   if(document.getElementById)
   {
      var foc = loc.parentNode.parentNode.nextSibling.firstChild.firstChild;
	  var imageFoc = loc.firstChild;		
			
	  if(foc.style.display == 'block') {
	  	imageFoc.src = imagePath + "/Expand.png";
		foc.style.display = 'none';
	  }	 else {
	  	imageFoc.src = imagePath + "/Collapse.png";
	  	foc.style.display = 'block';	  	
	  }
   }
}  

/*
 * collapseSection
 * 
 * Hides/Shows the content of a section container on a preview page based on the provided boolean
 * 
 * NOTE: This could probably be refactored to use an id instead of a location, and then navigate
 *       through the DOM based on the id, making the code less susceptible to breaking if the DOM
 *       changes unexpectedly.
 */

function collapseSection(loc, collapse)
{	
   if(document.getElementById)
   {
      var foc = loc.firstChild.firstChild.nextSibling.firstChild.firstChild;	  
	  var imageFoc = loc.firstChild.firstChild.firstChild.nextSibling.firstChild;
	  	    	  
	  if(collapse) {
		  imageFoc.src = imagePath + "/Expand.png";	  	
		  foc.style.display = 'none';
	  } else {
  		  imageFoc.src = imagePath + "/Collapse.png";	  	
		  foc.style.display = 'block';
	  }
   }
}  