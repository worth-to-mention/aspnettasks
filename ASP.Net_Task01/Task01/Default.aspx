<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Task01.Default"  MasterPageFile="~/MasterPage.Master"%>
<asp:Content ID="default" runat="server" ContentPlaceHolderID="defaultPlaceholder">
    <%@ MasterType VirtualPath="~/MasterPage.Master" %>
    <asp:TextBox runat="server"  ID="HeaderTextBox" />
    <asp:Button runat="server" name="ChangeHeaderButton" Text="Change header" OnClick="ChangeHeaderButton_Click" />
</asp:Content>
