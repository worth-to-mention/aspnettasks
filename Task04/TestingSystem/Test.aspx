<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Test.aspx.cs" Inherits="TestingSystem.Test" MasterPageFile="~/Main.Master" Trace="true"%>

<asp:Content id="TestPlaceholder" ContentPlaceHolderID="PageContent" runat="server">
    <h2>Test title: <asp:Literal id="testHeader" runat="server"/></h2>
    <label for="UserName">Enter your name: </label>
    <asp:TextBox id="UserName"  runat="server"/>
    <asp:PlaceHolder id="TestQuestions" runat="server" />
    <asp:Button id="TestSubmit" Text="Send" runat="server"/>
</asp:Content>