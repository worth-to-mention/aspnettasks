<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Test.aspx.cs" Inherits="TestingSystem.Test" MasterPageFile="~/Main.Master"%>

<asp:Content id="TestPlaceholder" ContentPlaceHolderID="PageContent" runat="server">
    <div>
        <h2>Test title: <asp:Literal id="testHeader" runat="server"/></h2>        
        <asp:HyperLink NavigateUrl="~/Default.aspx" runat="server">Go to the tests</asp:HyperLink>
        <asp:HyperLink NavigateUrl="~/Results.aspx" runat="server">Go to the results</asp:HyperLink>
    </div>
    <asp:PlaceHolder id="TestPlaceHolder" runat="server">
        <label for="UserName">Enter your name: </label>
        <asp:TextBox id="UserName"  runat="server"/>
        <asp:RequiredFieldValidator runat="server"
            ControlToValidate="UserName"
            ForeColor="Red"
            ErrorMessage="You must specify your name" />
        <asp:PlaceHolder id="TestQuestions" runat="server" />
        <asp:Button id="TestSubmit" Text="Send" runat="server"/>
    </asp:PlaceHolder>
    
</asp:Content>