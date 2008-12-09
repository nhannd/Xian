<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CodeMirror.aspx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.CodeMirror" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml"> 
  <head> 
    <script src="Scripts/CodeMirror/js/codemirror.js" type="text/javascript"></script> 
    <title>CodeMirror: HTML/XML demonstration</title> 
    <link rel="stylesheet" type="text/css" href="Scripts/CodeMirror/css/docs.css"/> 
  </head> 
  <body style="padding: 20px;"> 
 
<p>This is a simple demonstration of the XML/HTML indentation module
for <a href="index.html">CodeMirror</a>. The <a 
href="js/parsexml.js">javascript</a> file contains some comments with
more information.</p> 
 
<div style="border: 1px solid black; padding: 3px;"> 
<textarea id="code" cols="120" rows="30"> 
&lt;html style="color: green"&gt;
  &lt;!-- this is a comment --&gt;
  &lt;head&gt;
    &lt;title&gt;HTML Example&lt;/title&gt;
  &lt;/head&gt;
  &lt;body&gt;
    The indentation tries to be &lt;em&gt;somewhat &amp;quot;do what
    I mean&amp;quot;&lt;/em&gt;... but might not match your style.
  &lt;/body&gt;
&lt;/html&gt;
</textarea> 
</div> 
 
<script type="text/javascript"> 
  var editor = CodeMirror.fromTextArea('code', {
    height: "350px",
    parserfile: "parsexml.js",
    stylesheet: "Scripts/CodeMirror/css/xmlcolors.css",
    path: "Scripts/CodeMirror/js/",
    continuousScanning: 500
  });
</script> 
  </body> 
