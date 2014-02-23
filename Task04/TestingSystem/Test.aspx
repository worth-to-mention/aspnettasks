<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Test.aspx.cs" Inherits="TestingSystem.Test" MasterPageFile="~/BasicLayout.master"%>

<asp:Content id="TestPlaceholder" ContentPlaceHolderID="PageContent" runat="server">
    <div class="content-list"  runat="server">
        <asp:PlaceHolder ID="ContentPlaceholder" runat="server" />
    </div>    
</asp:Content>