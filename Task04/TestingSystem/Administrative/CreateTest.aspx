<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CreateTest.aspx.cs" Inherits="TestingSystem.Administrative.CreateTest" MasterPageFile="../BasicLayout.master" %>

<asp:Content ID="PageContent" ContentPlaceHolderID="PageContent" runat="server">
    <div class="content-list">
        <div class="content-list-item content-list-header">
            I gave up trying to build a test constructor.
            Load xml file with test data:
        </div>
        <div class="content-list-item">
            <asp:FileUpload ID="TestXmlFileUpload" runat="server" />
            <asp:Button ID="UploadButton" runat="server" Text="Upload" OnClick="UploadButton_Click"/>
        </div>
    </div>
</asp:Content>