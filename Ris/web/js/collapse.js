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