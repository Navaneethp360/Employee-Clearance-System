<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="PMMS.aspx.cs" Inherits="MedicalSystem.PMMS" %>
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
    display: none; /* hide initially */
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

/* ================== Input and Button ================== */
.input-group {
    display: flex;
    justify-content: center;
    margin-top: 20px;
    gap: 15px;
    flex-wrap: wrap;
    max-width: 600px;
    margin-left: auto;
    margin-right: auto;
}

.txt-input {
    flex: 1 1 250px;
    padding: 12px 16px;
    font-size: 1rem;
    border-radius: 8px;
    border: 1px solid rgba(200,200,200,0.4);
    background: rgba(255,255,255,0.35);
    backdrop-filter: blur(12px);
    color: var(--text-main);
    transition: all 0.3s ease;
}

.txt-input:focus {
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

/* ================== Connections ================== */
.connections {
    margin-top: 20px;
    display: flex;
    flex-direction: column;
    gap: 12px;
}

.connection-box {
    padding: 15px;
    border-radius: 8px;
    background: rgba(255,255,255,0.3);
    backdrop-filter: blur(8px);
    border: 1px solid rgba(255,255,255,0.4);
    font-weight: 600;
    transition: all 0.5s ease;
}

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

</style>

<div class="page-content">
    <div class="page-header">
        <h2>🛡️ Employee Clearance Check - PMMS</h2>
        <p class="section-description">Enter an employee ID to check across all systems. Green indicates no records found.</p>
    </div>

    <div class="input-group">
        <asp:TextBox ID="txtEmployeeID" runat="server" CssClass="txt-input" placeholder="Enter Employee ID"></asp:TextBox>
        <asp:Button ID="btnCheckClearance" runat="server" CssClass="btn-clearance" Text="Check Clearance" OnClick="btnCheckClearance_Click" />
    </div>

    <asp:Label ID="lblStatus" runat="server" CssClass="status-message"></asp:Label>

    <div class="connections">
        <asp:Label ID="conn1" runat="server" CssClass="connection-box">HEISCO</asp:Label>
        <asp:Label ID="conn2" runat="server" CssClass="connection-box">GULF DREDGING</asp:Label>
        <asp:Label ID="conn3" runat="server" CssClass="connection-box">HEISCO RESOURCES</asp:Label>
        <asp:Label ID="conn4" runat="server" CssClass="connection-box">HEISCO KSA</asp:Label>
        <asp:Label ID="conn5" runat="server" CssClass="connection-box">GULF SKY KSA</asp:Label>
    </div>
</div>

</asp:Content>
