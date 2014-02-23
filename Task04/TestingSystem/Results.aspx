<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Results.aspx.cs" Inherits="TestingSystem.Results" MasterPageFile="~/BasicLayout.master"%>

<asp:Content ContentPlaceHolderID="PageContent" runat="server">
    <div class="content-list">
        <div class="content-list-item content-list-header">            
            <h2>Testing results</h2>
        </div>
        <asp:PlaceHolder ID="ResultsContent" runat="server" />
    </div>
</asp:Content>
