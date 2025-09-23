<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="NONERP.aspx.cs" Inherits="MedicalSystem.NONERP" %>
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
    padding: 25px;
    box-shadow: var(--glass-shadow);
    margin-top: 25px;
    color: var(--text-main);
}

.page-header {
    margin-bottom: 20px;
    padding: 10px 15px;
    border-bottom: 1px solid #e9f2fa;
    background-color: #e9f2fa;
    border-radius: 8px;
}

.page-header h2 {
    color: var(--primary);
    font-size: 1.5rem;
    margin-bottom: 5px;
    display: flex;
    align-items: center;
}

.status-message { 
    display: none;
    padding:10px 16px; 
    margin-top:15px; 
    border-radius:6px; 
    font-weight:500; 
    font-size:0.9rem; 
    border:1px solid; 
    background-color:#e8f5e9; 
    color:#2e7d32; 
    border-color:#c8e6c9; 
    box-shadow:0 1px 4px rgba(0,0,0,0.04); 
}
.status-message.error { 
    background-color:#fcebea; 
    color:#c62828; 
    border-color:#f5c6cb; 
}

.section-description {
    margin-top: 6px;
    font-size: 0.9rem;
    color: #333;
}

/* Inputs & Buttons */
.input-group {
    display: flex;
    justify-content: center;
    margin-top: 20px;
    gap: 15px;
    flex-wrap: wrap;
    max-width: 650px;
    margin-left: auto;
    margin-right: auto;
}

.txt-input, .ddl-company {
    flex: 1 1 200px;
    padding: 12px 16px;
    font-size: 1rem;
    border-radius: 8px;
    border: 1px solid rgba(200,200,200,0.4);
    background: rgba(255,255,255,0.35);
    backdrop-filter: blur(12px);
    color: var(--text-main);
    transition: all 0.3s ease;
    appearance: none;
}

.txt-input:focus, .ddl-company:focus {
    background: rgba(255,255,255,0.45);
    box-shadow: inset 0 0 5px rgba(0,0,0,0.1);
    outline: none;
}

.btn-clearance {
    padding: 12px 28px;
    font-size: 0.95rem;
    font-weight: 600;
    border: none;
    border-radius: 8px;
    background: linear-gradient(135deg, #4a90e2 0%, #357ABD 100%);
    color: #fff;
    cursor: pointer;
    transition: all 0.3s ease;
}

.btn-clearance:hover {
    background: linear-gradient(135deg, #357ABD 0%, #255A88 100%);
}

/* Connections */
.connections {
    margin-top: 20px;
    display: flex;
    flex-direction: column;
    gap: 12px;
}

.conn-link {
    display: block;
    text-decoration: none;
    width: 100%;
    color: inherit;
}

.connection-box {
    display: block;
    width: 98%;
    padding: 15px;
    border-radius: 8px;
    background: rgba(255,255,255,0.3);
    backdrop-filter: blur(8px);
    border: 1px solid rgba(255,255,255,0.4);
    font-weight: 600;
    text-align: left;
    transition: all 0.3s ease;
    cursor: pointer;
    color: #000;
}

.connection-box:hover {
    transform: translateY(-2px);
    box-shadow: 0 4px 10px rgba(0,0,0,0.08);
}

/* Status colors */
.connection-box.green {
    background: #d4edda;
    color: #155724;
}
.connection-box.red {
    background: #f8d7da;
    color: #721c24;
}
.connection-box.gray {
    background: #e2e3e5;
    color: #6c757d;
}
.legend {
    margin-bottom: 10px;
    font-size: 14px;
    display: flex;
    gap: 30px; /* More space between each group */
    align-items: center;
}

.legend-group {
    margin-top:10px;
    display: flex;
    align-items: center;
    gap: 5px; /* Less space between box and text */
}

.legend-item {
    display: inline-block;
    width: 14px;
    height: 14px;
    border-radius: 3px;
}

.legend-item.green { background-color: #4CAF50; }
.legend-item.red { background-color: #F44336; }
.legend-item.gray { background-color: #9E9E9E; }
</style>

<div class="page-content">
    <div class="page-header">
        <h2>🛡️ Employee Clearance Check - NON-ERP</h2>
        <p class="section-description">Select company and enter Employee ID to check across all NON-ERP systems.</p>
    </div>

    <div class="input-group">
        <asp:DropDownList ID="ddlCompany" runat="server" CssClass="ddl-company">
            <asp:ListItem Text="Select Company" Value="" />
            <asp:ListItem Value="H">HEISCO</asp:ListItem>
            <asp:ListItem Value="G">GULF DREDGING</asp:ListItem>
            <asp:ListItem Value="T">HEISCO RESOURCES</asp:ListItem>
            <asp:ListItem Value="K">HEISCO KSA</asp:ListItem>
            <asp:ListItem Value="S">GULF SKY KSA</asp:ListItem>
        </asp:DropDownList>

        <asp:TextBox ID="txtEmployeeID" runat="server" CssClass="txt-input" placeholder="Enter Employee ID"></asp:TextBox>
        <asp:Button ID="btnCheckClearance" runat="server" CssClass="btn-clearance" Text="Check Clearance" OnClick="btnCheckClearance_Click" />
    </div>

    <asp:Label ID="lblStatus" runat="server" CssClass="status-message"></asp:Label>

    <div class="legend">
    <div class="legend-group">
        <span class="legend-item green"></span>Active Account
    </div>
    <div class="legend-group">
        <span class="legend-item red"></span>Deactivated Account
    </div>
    <div class="legend-group">
        <span class="legend-item gray"></span>No Account / Connection Failed
    </div>
</div>

    <div class="connections">
        <a href="javascript:void(0)" onclick="openApp('http://salarycertificate.heisco.com/sal/login.aspx')" class="conn-link">
            <asp:Label ID="conn1" runat="server" CssClass="connection-box">Salary Certificate</asp:Label>
        </a>
        <a href="javascript:void(0)" onclick="openApp('http://sermgr.heisco.com/ser/login.aspx')" class="conn-link">
            <asp:Label ID="conn2" runat="server" CssClass="connection-box">Facilities Management</asp:Label>
        </a>
        <a href="javascript:void(0)" onclick="openApp('http://ess.heisco.com/trf/login.aspx')" class="conn-link">
            <asp:Label ID="conn3" runat="server" CssClass="connection-box">Employee Training (TRF)</asp:Label>
        </a>
    </div>
</div>

<script>
    function openApp(url) {
        const user = "";
        const pass = "";
        const encoded = btoa(`${user}:${pass}`);
        const sep = url.includes("?") ? "&" : "?";
        window.open(`${url}${sep}data=${encodeURIComponent(encoded)}`, "_blank");
    }
</script>

</asp:Content>
