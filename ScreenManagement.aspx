<%@ Page Title="Screen Management" Language="C#" AutoEventWireup="true" CodeBehind="ScreenManagement.aspx.cs" Inherits="MedicalSystem.ScreenManagement" MasterPageFile="~/Site.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
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

:root {
    --glass-bg: rgba(255, 255, 255, 0.6);
    --glass-border: rgba(200, 200, 200, 0.3);
    --glass-shadow: 0 8px 24px rgba(0, 0, 0, 0.06);
    --blur-amt: blur(12px);
    --primary: #2c7db1;
    --primary-hover: #216796;
    --danger: #e74c3c;
    --danger-hover: #c0392b;
    --secondary: #4a9cdc;
    --secondary-hover: #337ab7;
    --text-main: #1f2937;
    --text-muted: #6b7280;
}


.page-container {
    padding: 15px 20px; /* Reduced padding */
    color: var(--text-main);
}

.page-title {
    color: #2c7db1;
    margin-bottom: 12px;
    font-size: 1.6rem;
}

.form-container {
    background: var(--glass-bg);
    backdrop-filter: var(--blur-amt);
    -webkit-backdrop-filter: var(--blur-amt);
    border: 1px solid var(--glass-border);
    border-radius: 12px; /* Slightly tighter radius */
    padding: 15px 20px; /* Less padding */
    margin-bottom: 25px; /* Reduced margin */
    box-shadow: var(--glass-shadow);
    max-width: 100%;
}

.section-title {
    font-size: 1.1rem; /* Smaller font */
    font-weight: 600;
    color: var(--text-main);
    margin-bottom: 12px;
}

.form-group label {
    font-weight: 600;
    margin-top: 6px; /* Tighter */
    display: block;
    color: var(--text-main);
    margin-bottom: 4px; /* Less margin */
}

.form-control {
    width: 100%;
    box-sizing: border-box; /* Fix overflow */
    padding: 8px 10px; /* Denser padding */
    border-radius: 6px; /* Smaller radius */
    border: 1px solid #cfd9e3;
    background-color: rgba(255, 255, 255, 0.85);
    font-size: 0.9rem; /* Smaller font */
    color: var(--text-main);
    transition: all 0.3s ease;
    min-width: 0; /* Prevent flex overflow */
    max-width: 100%;
}

.form-control:focus {
    border-color: var(--primary);
    box-shadow: 0 0 0 3px rgba(44, 125, 177, 0.15);
    outline: none;
}

/* Buttons */
.btn-primary, .btn-secondary, .btn-danger {
    border: none;
    padding: 8px 14px; /* Denser */
    border-radius: 6px; /* Smaller radius */
    font-size: 0.85rem; /* Smaller font */
    font-weight: 600;
    cursor: pointer;
    transition: all 0.3s ease;
    box-shadow: 0 2px 5px rgba(0,0,0,0.07);
}

    .btn-primary {
        margin-top:10px;
        background: linear-gradient(135deg, #4a90e2 0%, #357ABD 100%);
        color: white;
        border: 1.5px solid #2c7db1;
        border-radius: 6px;
        font-weight: 700;
        font-size: 0.9rem;
        padding: 8px 18px;
        cursor: pointer;
        box-shadow: 0 2px 6px rgba(44, 125, 177, 0.35);
        transition: background-color 0.25s ease, box-shadow 0.25s ease, transform 0.25s ease;
        display: inline-block;
        min-width: 110px;
        text-align: center;
        user-select: none;
    }

    .btn-primary:hover {
        background: linear-gradient(135deg, #357ABD 0%, #255A88 100%);
        
    }
.btn-secondary {
    background-color: var(--secondary);
    color: white;
    margin-right: 6px;
}

.btn-secondary:hover {
    background-color: var(--secondary-hover);
}

.btn-danger {
    background-color: var(--danger);
    color: white;
}

.btn-danger:hover {
    background-color: var(--danger-hover);
}

/* Grid Table */
.grid {
    width: 100%;
    border-collapse: collapse;
    margin-top: 15px;
    background: rgba(255, 255, 255, 0.75);
    backdrop-filter: blur(10px);
    -webkit-backdrop-filter: blur(10px);
    border: 1px solid rgba(207, 229, 249, 0.7);
    border-radius: 10px;
    overflow: hidden;
    box-shadow: 0 6px 18px rgba(0, 0, 0, 0.04);
    color: #1c1c1c;
}

.grid th,
.grid td {
    padding: 10px 12px; /* Denser */
    border-bottom: 1px solid #e2effa;
    text-align: left;
    font-size: 0.9rem;
}

.grid th {
    background: #f0f8ff;
    font-weight: 600;
    color: #2c7db1;
}

.grid tr:hover {
    background: rgba(240, 248, 255, 0.3);
}

/* Message */
.message {
    padding: 10px 14px;
    background-color: rgba(255, 230, 230, 0.75);
    border: 1px solid #f5c2c7;
    border-left: 5px solid var(--danger);
    color: var(--danger);
    border-radius: 6px;
    margin-bottom: 15px;
    font-weight: 500;
    box-shadow: 0 1px 3px rgba(0,0,0,0.04);
}

/* HR */
hr {
    border: 0;
    border-top: 1px solid #dce5ef;
    margin: 30px 0;
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
/* Responsive fix for inputs inside flex containers */
.form-container, .page-container {
    max-width: 100%;
}

input, select, textarea {
    box-sizing: border-box;
}

.form-control {
    min-width: 0; /* Important for flexbox overflow fix */
}
</style>
    <div class="page-container">
        <div class="page-header">
            <h2><i class="medical-icon"></i>Screen Management</h2>
            <p class="subtitle">Organize and configure system screens for structured navigation and workflows</p>
        </div>

        <asp:Label ID="lblMessage" runat="server" CssClass="message" Visible="false" />

        <div class="form-container">
            <h3 class="section-title">Add New Screen</h3>

            <div class="form-group">
                <label for="txtScreenName">Screen Name:</label>
                <asp:TextBox ID="txtScreenName" runat="server" CssClass="form-control" />
            </div>

            <div class="form-group">
                <label for="txtScreenPath">Screen Path:</label>
                <asp:TextBox ID="txtScreenPath" runat="server" CssClass="form-control" />
            </div>

            <div class="form-group">
                <label for="txtGroupName">Group Name:</label>
                <asp:TextBox ID="txtGroupName" runat="server" CssClass="form-control" />
            </div>

            <div class="form-group">
                <label for="txtDisplayOrder">Display Order:</label>
                <asp:TextBox ID="txtDisplayOrder" runat="server" CssClass="form-control" />
            </div>

            <div class="form-group">
                <asp:Button ID="btnAddScreen" runat="server" Text="Add Screen" OnClick="btnAddScreen_Click" CssClass="btn-primary" />
            </div>
        </div>

        <hr />

        <div class="page-header">
            <h2><i class="medical-icon"></i>Manage Existing Screens</h2>
        </div>

        <div class="form-container">
            <asp:GridView ID="gvScreens" runat="server" AutoGenerateColumns="False" CssClass="grid">
                <Columns>
                    <asp:BoundField DataField="ScreenName" HeaderText="Screen Name" SortExpression="ScreenName" />
                    <asp:BoundField DataField="ScreenPath" HeaderText="Screen Path" SortExpression="ScreenPath" />
                    <asp:BoundField DataField="GroupName" HeaderText="Group Name" SortExpression="GroupName" />
                    <asp:BoundField DataField="DisplayOrder" HeaderText="Order" SortExpression="DisplayOrder" />
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:Button ID="btnEdit" runat="server" Text="Edit" CommandArgument='<%# Eval("ScreenID") %>' OnClick="btnEdit_Click" CssClass="btn-secondary" />
                            <asp:Button ID="btnDelete" runat="server" Text="Delete" CommandArgument='<%# Eval("ScreenID") %>' OnClick="btnDelete_Click" CssClass="btn-danger" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </div>



</asp:Content>
