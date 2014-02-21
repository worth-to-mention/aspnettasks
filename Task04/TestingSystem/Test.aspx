<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Test.aspx.cs" Inherits="TestingSystem.Test" MasterPageFile="~/BasicLayout.master"%>

<asp:Content id="TestPlaceholder" ContentPlaceHolderID="PageContent" runat="server">
    <asp:PlaceHolder ID="TestPageContent" runat="server">
        <div class="test"> 
            <div class="test-header">
                <h2>Test title: <asp:Literal id="testHeader" runat="server"/></h2>        
            </div>
            <asp:PlaceHolder id="TestContent" runat="server">
                <div class="test-content"> 
                    <label for="UserName">Enter your name: </label>
                    <asp:TextBox id="UserName"  runat="server" CssClass="test-userName"/>
                    <asp:RequiredFieldValidator runat="server"
                        ControlToValidate="UserName"
                        ForeColor="Red"
                        ErrorMessage="You must specify your name">
                         It would be nice if you have specified your name.
                    </asp:RequiredFieldValidator>
                    <asp:PlaceHolder id="TestQuestions" runat="server" />
                    <asp:Button id="TestSubmit" Text="Send" runat="server" CssClass="test-submit"/>
                </div>
            </asp:PlaceHolder>
        </div>
    </asp:PlaceHolder>
    
</asp:Content>