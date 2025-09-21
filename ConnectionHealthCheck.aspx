<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ConnectionHealthCheck.aspx.cs" Inherits="MedicalSystem.ConnectionHealthCheck" %>
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
}

.btn {
    padding: 10px 20px;
    background: linear-gradient(135deg,#4a90e2 0%,#357ABD 100%);
    color:white; 
    border:1.5px solid var(--primary); 
    border-radius:6px;
    cursor:pointer; 
    font-weight:600; 
    font-size:0.95rem; 
    box-shadow:0 2px 6px rgba(44,125,177,0.35);
    margin-top: 10px;
    transition: all 0.3s ease;
}
.btn:hover { 
    background: linear-gradient(135deg,#357ABD 0%,#255A88 100%); 
}

/* Section boxes */
.section-box {
    background: rgba(255,255,255,0.35);
    border: 1px solid rgba(200,200,200,0.4);
    border-radius: 12px;
    padding: 15px;
    margin-top: 20px;
    box-shadow: 0 4px 10px rgba(0,0,0,0.05);
}

.section-box h3 {
    margin-bottom: 12px;
    color: var(--primary);
    font-size: 1.2rem;
    border-bottom: 1px solid rgba(0,0,0,0.1);
    padding-bottom: 5px;
}

/* Connections inside section */
.connections {
    display: flex;
    gap: 12px;
    flex-wrap: wrap;
}

.connection-box {
    flex: 1;
    min-width: 100px;
    padding: 10px;
    border-radius: 8px;
    background: rgba(255,255,255,0.3);
    backdrop-filter: blur(8px);
    border: 1px solid rgba(255,255,255,0.4);
    font-weight: 600;
    text-align: center;
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
</style>

<div class="page-content">
    <div class="page-header">
        <h2>🖥️ Connection Health Check</h2>
        <p>All connections for MIS, PMMS, and NONERP will be tested.</p>
    </div>

    <asp:Button ID="btnCheckConnections" runat="server" CssClass="btn" Text="Check Connections" OnClick="btnCheckConnections_Click" />

    <!-- MIS Section -->
    <div class="section-box">
        <h3>MIS Connections</h3>
        <div class="connections">
            <asp:Label ID="mis1" runat="server" CssClass="connection-box">HEISCO</asp:Label>
            <asp:Label ID="mis2" runat="server" CssClass="connection-box">GULF DREDGING</asp:Label>
            <asp:Label ID="mis3" runat="server" CssClass="connection-box">HEISCO RESOURCES</asp:Label>
            <asp:Label ID="mis4" runat="server" CssClass="connection-box">HEISCO KSA</asp:Label>
            <asp:Label ID="mis5" runat="server" CssClass="connection-box">GULF SKY KSA</asp:Label>
        </div>
    </div>

    <!-- PMMS Section -->
    <div class="section-box">
        <h3>PMMS Connections</h3>
        <div class="connections">
            <asp:Label ID="pmms1" runat="server" CssClass="connection-box">HEISCO</asp:Label>
            <asp:Label ID="pmms2" runat="server" CssClass="connection-box">GULF DREDGING</asp:Label>
            <asp:Label ID="pmms3" runat="server" CssClass="connection-box">HEISCO RESOURCE</asp:Label>
            <asp:Label ID="pmms4" runat="server" CssClass="connection-box">HEISCO KSA</asp:Label>
            <asp:Label ID="pmms5" runat="server" CssClass="connection-box">GULF SKY KSA</asp:Label>
        </div>
    </div>

    <!-- NONERP Section -->
    <div class="section-box">
        <h3>NONERP Connections</h3>
        <div class="connections">
            <asp:Label ID="nonerp1" runat="server" CssClass="connection-box">SAL</asp:Label>
            <asp:Label ID="nonerp2" runat="server" CssClass="connection-box">OSR</asp:Label>
            <asp:Label ID="nonerp3" runat="server" CssClass="connection-box">TRF</asp:Label>
        </div>
    </div>
</div>

</asp:Content>
