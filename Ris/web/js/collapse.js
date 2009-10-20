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
 */

function toggleCollapsedSection(loc)
{	
   if(document.getElementById)
   {
      var foc = loc.parentNode.parentNode.nextSibling.style ? loc.parentNode.parentNode.nextSibling : loc.parentNode.parentNode.nextSibling.nextSibling;
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
 */

function collapseSection(loc, collapse)
{	
   if(document.getElementById)
   {
      var foc = loc.firstChild.firstChild.nextSibling;	  
	  var imageFoc = loc.firstChild.firstChild.nextSibling;
	  
	  if(collapse) {
		  imageFoc.src = imagePath + "/Expand.png";	  	
		  foc.style.display = 'none';
	  } else {
  		  imageFoc.src = imagePath + "/Collapse.png";	  	
		  foc.style.display = 'block';
	  }
   }
}  