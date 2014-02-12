<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="Task03.Forms.Index"  MasterPageFile="~/Forms/MasterPage.Master"%>

<asp:Content ContentPlaceHolderID="PollPlaceHolder" ID="Poll" runat="server">
    <%@ MasterType VirtualPath="~/Forms/MasterPage.Master"%>
    <mcc:Poll SourceFile="/Source/pollSample.xml" runat="server" />
    <asp:Button OnClick="Unnamed_Click" runat="server" Width="200px"/>
</asp:Content>
