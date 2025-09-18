<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CustomContacts.aspx.cs" Inherits="PhoneDir.CustomContacts" MasterPageFile="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<style>
:root {
    --glass-bg: rgba(255,255,255,0.65);
    --glass-border: rgba(200,200,200,0.4);
    --glass-shadow: 0 8px 20px rgba(0,0,0,0.05);
    --blur-amt: blur(10px);
    --primary: #2c7db1;
    --text-main: #1c1c1c;
}

.page-content {
    background: var(--glass-bg);
    backdrop-filter: var(--blur-amt);
    -webkit-backdrop-filter: var(--blur-amt);
    border: 1px solid var(--glass-border);
    border-radius: 16px;
    padding: 20px;
    box-shadow: var(--glass-shadow);
    margin-top: 20px;
    color: var(--text-main);
}

.page-header {
    margin-bottom: 15px;
    padding: 10px 15px;
    border-bottom: 1px solid #e9f2fa;
    background-color: #e9f2fa;
    border-radius: 8px;
}

.page-header h2 {
    color: var(--primary);
    font-size: 1.5rem;
    margin-bottom: 2px;
    display: flex;
    align-items: center;
}

.form-group { margin-bottom: 12px; }
.form-control { width: 100%; padding: 8px 10px; border: 1px solid #cfdfee; border-radius: 6px; font-size: 0.9rem; }
.form-control:focus { border-color: var(--primary); outline:none; }

.btn {
    padding: 8px 16px;
    background: linear-gradient(135deg,#4a90e2 0%,#357ABD 100%);
    color:white; border:1.5px solid var(--primary); border-radius:6px;
    cursor:pointer; font-weight:600; font-size:0.9rem; box-shadow:0 2px 6px rgba(44,125,177,0.35);
}

.btn:hover { background: linear-gradient(135deg,#357ABD 0%,#255A88 100%); }

.table { width:100%; border-collapse:collapse; margin-top:15px; background: rgba(255,255,255,0.75); border:1px solid #cfe5f9; border-radius:10px; overflow:hidden; box-shadow:var(--glass-shadow); }
.table th, .table td { padding:10px 12px; border-bottom:1px solid #e2effa; text-align:left; }
.table th { background:#f0f8ff; font-weight:600; color:var(--primary); }
.table tr:hover { background: rgba(240,248,255,0.3); }

.status-message { display:block; padding:10px 16px; margin-bottom:15px; border-radius:6px; font-weight:500; font-size:0.9rem; border:1px solid; background-color:#e8f5e9; color:#2e7d32; border-color:#c8e6c9; box-shadow:0 1px 4px rgba(0,0,0,0.04); }
.status-message.error { background-color:#fcebea; color:#c62828; border-color:#f5c6cb; }

.emp-row { display:flex; gap:8px; margin-bottom:5px; flex-wrap:wrap; }
.emp-row input { flex:1 1 120px; }

/* Delete button inside GridView */
.table .btn-delete {
    padding:4px 8px;
    background:#f44336;
    border:none;
    border-radius:4px;
    color:white;
    cursor:pointer;
    font-size:0.8rem;
}

.table .btn-delete:hover {
    background:#c62828;
}
</style>

<div class="page-content">
    <div class="page-header">
        <h2>📇 Manage Custom Contacts</h2>
    </div>

    <asp:Label ID="lblMessage" runat="server" CssClass="status-message" Visible="false"></asp:Label>

    <div class="form-group">
        <label for="ddlCompany">Company:</label>
        <asp:DropDownList ID="ddlCompany" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlCompany_SelectedIndexChanged"></asp:DropDownList>
        <label>Or enter new:</label>
        <asp:TextBox ID="txtCompanyNew" runat="server" CssClass="form-control" />
    </div>

    <div class="form-group">
        <label for="ddlDepartment">Department:</label>
        <asp:DropDownList ID="ddlDepartment" runat="server" CssClass="form-control"></asp:DropDownList>
        <label>Or enter new:</label>
        <asp:TextBox ID="txtDeptNew" runat="server" CssClass="form-control" />
    </div>

    <h4>Details</h4>
    <div class="emp-row">
        <asp:TextBox ID="txtEmpID" runat="server" CssClass="form-control" Placeholder="EmpID" />
        <asp:TextBox ID="txtName" runat="server" CssClass="form-control" Placeholder="Name" />
        <asp:TextBox ID="txtDesignation" runat="server" CssClass="form-control" Placeholder="Designation" />
        <asp:TextBox ID="txtExtension" runat="server" CssClass="form-control" Placeholder="Extension" />
        <asp:TextBox ID="txtMobile" runat="server" CssClass="form-control" Placeholder="Mobile" />
        <asp:TextBox ID="txtLocation" runat="server" CssClass="form-control" Placeholder="Location" />
        <asp:TextBox ID="txtSubDept" runat="server" CssClass="form-control" Placeholder="SubDept" />
    </div>

    <br />
    <asp:Button ID="btnSave" runat="server" Text="Save Contact" CssClass="btn" OnClick="btnSave_Click" />

    <h4 style="margin-top:30px;">Existing Custom Contacts</h4>
    <asp:GridView ID="gvCustomContacts" runat="server" AutoGenerateColumns="False" CssClass="table" OnRowCommand="gvCustomContacts_RowCommand" DataKeyNames="ID">
        <Columns>
            <asp:BoundField HeaderText="Company" DataField="Company" />
            <asp:BoundField HeaderText="Department" DataField="Department" />
            <asp:BoundField HeaderText="Name" DataField="Name" />
            <asp:BoundField HeaderText="Designation" DataField="Designation" />
            <asp:BoundField HeaderText="Mobile" DataField="Mobile" />
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:Button ID="btnDelete" runat="server" Text="Delete" CssClass="btn-delete" CommandName="DeleteContact" CommandArgument='<%# Eval("ID") %>' />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
</div>

<script>
    document.addEventListener('DOMContentLoaded', function () {
        // Company dropdown disables textbox if any value selected
        const ddlCompany = document.getElementById('<%= ddlCompany.ClientID %>');
    const txtCompany = document.getElementById('<%= txtCompanyNew.ClientID %>');

    function toggleCompanyTextbox() {
        // Disable textbox if dropdown selected index > 0
        txtCompany.disabled = ddlCompany.selectedIndex > 0;
    }
    ddlCompany.addEventListener('change', toggleCompanyTextbox);
    toggleCompanyTextbox(); // initial check on page load

    // Department dropdown disables textbox if any value selected
    const ddlDept = document.getElementById('<%= ddlDepartment.ClientID %>');
    const txtDept = document.getElementById('<%= txtDeptNew.ClientID %>');

    function toggleDeptTextbox() {
        txtDept.disabled = ddlDept.selectedIndex > 0;
    }
    ddlDept.addEventListener('change', toggleDeptTextbox);
    toggleDeptTextbox(); // initial check on page load
});
</script>

</asp:Content>
