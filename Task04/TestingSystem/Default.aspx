<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="TestingSystem.Default" MasterPageFile="~/BasicLayout.master" %>

<asp:Content ContentPlaceHolderID="PageContent" runat="server">
        <div class="content-list-item content-list-header">
            <p>
                It is a home page of the Sample Testing System.
            </p>
        </div>
        <div class="content-list-item">
            You can find all the available tests <a href="Test.aspx">here</a>.
            Or you can view a statistic information <a href="Results.aspx">here</a>.
        </div>
</asp:Content>