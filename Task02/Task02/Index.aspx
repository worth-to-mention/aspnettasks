<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="Task02.Index" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:Image ID="captchaImage" runat="server" />
        <asp:TextBox ID="captchaTextBox" runat="server" />
        <asp:Button Text="Send" ID="captchaSendButton" runat="server" OnClick="captchaSendButton_Click"/>
    </div>
    </form>
</body>
</html>
