<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SagePay.aspx.cs" Inherits="JCE.Sage.SagePay" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body style="overflow:hidden" onload="go()">
    <asp:HtmlIframe id="sagepay" src="" width="690px" height="690px" runat="server"></asp:HtmlIframe>

    <script type="text/javascript">
        function go() {

            window.addEventListener("message", receiveMessage, false);
        }

        function receiveMessage() {
            window.open('', '_self', '');
            window.close();
        }
    </script>
    </body>
</html>
