﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OrderSuccessful.aspx.cs" Inherits="JCE.Sage.OrderSuccessful" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        .style1
        {
            font-family: Arial, Helvetica, sans-serif;
            color: #0000FF;
        }
        .style2
        {
            text-align: center;
        }
        .style3
        {
            text-align: center;
            font-family: Arial, Helvetica, sans-serif;
            color: #0000FF;
        }
          .style4
        {
            text-align: center;
            color: #0000FF;
        }
    </style>
</head>
<body style="background-color: #A7C0DC">
<script type="text/javascript">
    function windowClose() {
        var o = new Object();
        o.status = "ok";
        window.returnValue = o;
        window.parent.postMessage("close", "*"); 
    }</script>
    <form id="form1" runat="server">
    <div>
        <h1 class="style2">
            &nbsp;</h1>
        <h1 class="style2">
            &nbsp;</h1>
        <h1 class="style2">
            &nbsp;</h1>
        <h1 class="style2">
            &nbsp;</h1>
        <h1 class="style2">
            <span class="style1">Payment Successfully Taken</span><br class="style1" />
            </h1>
         <h3 class="style4">
            Please click Ok to close to window.</h3>
        <div style="text-align: center">
            <input type="button" onclick="javascript:windowClose();" value="Ok" 
                style="width:100px; font-weight: 700; background-color: #D7D7D7;" />
        </div>
    </div>
    </form>
</body>
</html>
