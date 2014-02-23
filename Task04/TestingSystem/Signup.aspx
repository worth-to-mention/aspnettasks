<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Signup.aspx.cs" Inherits="TestingSystem.Signup" MasterPageFile="~/BasicLayout.master" %>

<asp:Content ID="SignupContent" ContentPlaceHolderID="PageContent" runat="server">
    <div class="content-list">
        <div class="content-list-item content-list-header">
            Registration.
        </div>
        <asp:CreateUserWizard ID="CreateUserWizard1" runat="server" CssClass="content-list-item" FinishDestinationPageUrl="~/Default.aspx" ContinueDestinationPageUrl="~/Default.aspx" CancelDestinationPageUrl="~/Default.aspx">
            <WizardSteps>
                <asp:CreateUserWizardStep ID="CreateUserWizardStep1" runat="server">
                </asp:CreateUserWizardStep>
                <asp:CompleteWizardStep ID="CompleteWizardStep1" runat="server">
                </asp:CompleteWizardStep>
            </WizardSteps>
        </asp:CreateUserWizard>
    </div>
</asp:Content>