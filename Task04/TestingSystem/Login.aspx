<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="TestingSystem.Login" MasterPageFile="~/BasicLayout.master" %>

<asp:Content ID="LoginContent" ContentPlaceHolderID="PageContent" runat="server" >
    <div class="content-list">
        <div class="content-list-item content-list-header">
            Sign in to your account.
        </div>
        <div class="content-list-item">
                <asp:Login 
                    ID="LoginBlock" 
                    runat="server" 
                    TitleText="" 
                    LoginButtonText=" Sign in" 
                    ValidatorTextStyle-CssClass="loginbox-validatorText" 
                    LoginButtonStyle-CssClass="loginbox-signinButton" 
                    CheckBoxStyle-CssClass="loginbox-checkbox" 
                    CssClass="loginbox" 
                    HyperLinkStyle-CssClass="loginbox-hyperlink" 
                    LabelStyle-CssClass="loginbox-label" 
                    TextBoxStyle-CssClass="loginbox-textbox" 
                    TitleTextStyle-CssClass="loginbox-titleText" Orientation="Horizontal" />
        </div>
    </div>
    

</asp:Content>
