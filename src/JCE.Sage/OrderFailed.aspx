<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OrderFailed.aspx.cs" Inherits="JCE.Sage.OrderFailed" %>

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
        .style5
        {
            color: #0000FF;
        }
    </style>
</head>
<body style="background-color: #E9EDF1">
    <script type="text/javascript">
        function windowClose() {
            var o = new Object();
            o.status = "An error occurred.";
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
            <span class="style1">Payment was unsuccessful</span><br class="style1" />
        </h1>
        <h2 class="style3">
            <asp:Label runat="server" ID="ErrorDescription" style="font-size: medium"></asp:Label></h2>
            <h3 class="style2">
            <asp:Label runat="server" ID="ErrorHelp" CssClass="style5"></asp:Label></h3>
        <h3 class="style4">
            Please click Ok to close to window.</h3>
        <div style="text-align: center">
            <input type="button" onclick="javascript:windowClose()" value="Ok" 
                style="width:100px; font-weight: 700; background-color: #D7D7D7;" />
        </div>
    </div>
    </form>
</body>
</html>
