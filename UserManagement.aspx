<%@ Page Title="User Management" Language="C#" AutoEventWireup="true" CodeBehind="UserManagement.aspx.cs" Inherits="MedicalSystem.UserManagement" MasterPageFile="~/Site.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    
    <style>
.alert-message {
    display: block;
    padding: 8px 12px;
    font-weight: 500;
    margin-bottom: 12px;
    border-radius: 5px;
    font-size: 0.9rem;
    border: 1px solid transparent;
    box-shadow: 0 1px 4px rgba(0, 0, 0, 0.04);
    width: 100%;
    max-width: 100%;
}

/* Success */
.alert-success {
    background-color: #d4edda;
    border-color: #c3e6cb;
    color: #155724;
}

/* Error */
.alert-error {
    background-color: #f8d7da;
    border-color: #f5c6cb;
    color: #721c24;
}

  .page-header {
    margin-bottom: 10px;
    padding: 10px 15px;
    border-bottom: 1px solid #e9f2fa;
    background-color: #e9f2fa;
    border-radius: 8px;
}

.page-header h2 {
    color: #2c7db1;
    font-size: 1.5rem;
    margin-bottom: 2px;
    display: flex;
    align-items: center;
}

.container-row {
    display: flex;
    gap: 20px;
    flex-wrap: wrap;
}

.form-container {
    flex: 1;
    padding: 15px;
    background-color: #e9f2fa; /* Accent color */
    border-radius: 8px;
    box-shadow: 0 1px 4px rgba(44, 125, 177, 0.15);
    border: 1px solid #d1e1f0; /* Border color */
    min-width: 280px;
}

.form-container h3, .form-container h4 {
    color: #2c7db1; /* Primary color */
    margin-bottom: 10px;
    font-size: 1.2rem;
}

.form-group label {
    font-weight: 700;
    color: #333333; /* Text color */
    display: block;
    margin-top: 6px;
    margin-bottom: 4px;
    font-size: 0.9rem;
}

.form-control {
    width: 98%;
    padding: 6px 8px;
    font-size: 0.9rem;
    border: 1px solid #d1e1f0; /* Border color */
    border-radius: 4px;
    background-color: white;
    margin-bottom: 8px;
    box-sizing: border-box;
}

.dropdown {
    background-color: #fff;
    color: #333333; /* Text color */
}

.btn-submit {
    padding: 8px;
    background: linear-gradient(135deg, #4a90e2 0%, #357ABD 100%);
    color: #fff;
    border: 1.5px solid #2c7db1;
    border-radius: 5px;
    font-size: 0.95rem;
    cursor: pointer;
    margin-top: 8px;
    width: 100%;
    transition: background-color 0.25s ease, box-shadow 0.25s ease, transform 0.25s ease;
    box-shadow: 0 2px 6px rgba(44, 125, 177, 0.35);
    user-select: none;
}

.btn-submit:hover {
    background: linear-gradient(135deg, #357ABD 0%, #255A88 100%);
    
}
.medical-icon:before {
    content: '🖥️';
    margin-right: 6px;
    font-size: 1.4rem;
    display: inline-flex;
    align-items: center;
    line-height: 1;
    font-style: normal !important;   
    font-family: "Segoe UI Emoji", "Apple Color Emoji", "Noto Color Emoji", sans-serif !important;
}



.message {
    color: red;
    font-weight: 700;
    margin-bottom: 10px;
    font-size: 0.9rem;
}

.checkbox-group {
    display: flex;
    flex-wrap: wrap;
    gap: 15px;
    margin-top: 8px;
}

.checkbox-group label {
    color: #333333; /* Text color */
    font-weight: 500;
    font-size: 0.9rem;
}

.checkbox-group input {
    margin-right: 4px;
}

.checkbox-group li {
    display: inline-flex;
    align-items: center;
    margin-bottom: 8px;
    gap: 4px;
    font-size: 0.9rem;
}
    </style>
    <div class="page-header">
    <h2><span class="medical-icon"></span>User Management</h2>
        <p class="subtitle">Create and manage application users, assign roles, and control access</p>
</div>


   <asp:Label ID="lblMessage" runat="server" CssClass="alert-message" Visible="false" EnableViewState="false" />

    <div class="container-row">
        <!-- Add New User -->
        <div class="form-container">
            <h3>Add New User</h3>
            <div class="form-group">
                <label for="txtNewUsername">Username:</label>
                <asp:TextBox ID="txtNewUsername" runat="server" CssClass="form-control" />

                <label for="txtNewPassword">Password:</label>
                <asp:TextBox ID="txtNewPassword" runat="server" TextMode="Password" CssClass="form-control" />

                <label for="ddlNewUserRole">Role:</label>
                <asp:DropDownList ID="ddlNewUserRole" runat="server" CssClass="form-control dropdown" />

                <label for="ddlCompany">Clinic:</label>
                <asp:DropDownList ID="ddlCompany" runat="server" CssClass="form-control dropdown" />

                <label for="ddlNewUserCostCenter">Cost Center:</label>
        <asp:DropDownList ID="ddlNewUserCostCenter" runat="server" CssClass="form-control dropdown" />

                <asp:Button ID="btnAddUser" runat="server" Text="Add User" OnClick="btnAddUser_Click" CssClass="btn-submit" />
            </div>
        </div>

        <!-- Edit User Section -->
        <div class="form-container">
            <h3>Edit Users</h3>
            <div class="form-group">
                <label for="ddlUsers">Select User:</label>
                <asp:DropDownList ID="ddlUsers" runat="server" CssClass="form-control dropdown" />
                <asp:Button ID="btnDeleteUser" runat="server" Text="Delete User" OnClick="btnDeleteUser_Click" CssClass="btn-submit" />
            </div>
        </div>

        <!-- Edit Role Permissions Section -->
        <div class="form-container">
            <h3>Edit Role Permissions</h3>
            <div class="form-group">
                <label for="ddlRoles">Select Role:</label>
                <asp:DropDownList ID="ddlRoles" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlRoles_SelectedIndexChanged" CssClass="form-control dropdown" />
            </div>

            <div id="permissionsPanel" runat="server">
                <h4>Assign Screen Access</h4>
                <asp:CheckBoxList ID="chkRolePermissions" runat="server" RepeatLayout="Table" CssClass="checkbox-group" RepeatDirection="Vertical">
                </asp:CheckBoxList>
                <asp:Button ID="btnSaveRolePermissions" runat="server" Text="Save Permissions" OnClick="btnSaveRolePermissions_Click" CssClass="btn-submit" />
            </div>
        </div>
        <!-- Role Management -->
<div class="form-container">
    <h3>Manage Roles</h3>
    <div class="form-group">
        <label for="txtNewRoleName">New Role Name:</label>
        <asp:TextBox ID="txtNewRoleName" runat="server" CssClass="form-control" />
        <asp:Button ID="btnAddRole" runat="server" Text="Add Role" OnClick="btnAddRole_Click" CssClass="btn-submit" />
    </div>

    <div class="form-group">
        <label for="ddlExistingRoles">Delete Existing Role:</label>
        <asp:DropDownList ID="ddlExistingRoles" runat="server" CssClass="form-control dropdown" />
        <asp:Button ID="btnDeleteRole" runat="server" Text="Delete Role" OnClick="btnDeleteRole_Click" CssClass="btn-submit" />
    </div>
</div>
<!-- Extra Screens Section -->
<div class="form-container">
    <h3>Assign Extra Screens to User</h3>
    <div class="form-group">
        <label for="ddlUsersExtra">Select User:</label>
        <asp:DropDownList ID="ddlUsersExtra" runat="server" CssClass="form-control dropdown"
            AutoPostBack="true" OnSelectedIndexChanged="ddlUsersExtra_SelectedIndexChanged">
        </asp:DropDownList>
    </div>

    <div id="extraScreensPanel" runat="server">
        <h4>Select Extra Screens</h4>
        <asp:CheckBoxList ID="chkExtraScreens" runat="server" RepeatLayout="Table"
            CssClass="checkbox-group" RepeatDirection="Vertical">
        </asp:CheckBoxList>

        <asp:Button ID="btnSaveExtraScreens" runat="server" Text="Save Extra Screens"
            OnClick="btnSaveExtraScreens_Click" CssClass="btn-submit" />
    </div>
</div>


    </div>

</asp:Content>
