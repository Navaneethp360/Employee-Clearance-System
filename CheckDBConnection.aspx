<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CheckDBConnection.aspx.cs" Inherits="MedicalSystem.CheckDBConnection" %>

<!DOCTYPE html>
<html lang="en">
<head runat="server">
    <meta charset="utf-8" />
    <title>Database Dashboard</title>
    <style>
      /* Your existing CSS remains unchanged */
      :root {
        --shadow-offset: 0;
        --shadow-blur: 20px;
        --shadow-spread: -5px;
        --shadow-color: rgba(255, 255, 255, 0.7);
        --tint-color: 255, 255, 255;
        --tint-opacity: 0.3;
        --frost-blur: 6px;
        --outer-shadow-blur: 24px;
      }
      html, body {
        margin: 0;
        padding: 0;
        font-family: -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, sans-serif;
        height: 100vh;
        background: url('sonoma.jpg') no-repeat center center fixed;
        background-size: cover;
        overflow: hidden;
      }
header {
    margin-top: 16px;
    margin-left: 25px;
    margin-right: 25px;
    padding: 16px 0px;
    font-size: 2.8rem;
    font-weight: bolder;
    color: #000;
    background: rgba(255, 255, 255, 0.3);
    backdrop-filter: blur(10px);
    -webkit-backdrop-filter: blur(10px);
    border-radius: 20px;
    box-shadow: 0 1px 6px rgba(0, 0, 0, 0.1);
    border: 1px solid black;
    text-align: center;
    box-sizing: border-box;
}
      .dashboard-grid {
        display: grid;
        grid-template-columns: 260px 1fr;
        grid-gap: 24px;
        padding: 20px 24px;
        box-sizing: border-box;
        align-items: flex-start;
      }
      .metrics-bar {
        display: flex;
        flex-direction: column;
        gap: 14px;
      }
      .metric-box {
        position: relative;
        padding: 18px 20px;
        border-radius: 20px;
        color: #000;
        font-weight: 700;
        overflow: hidden;
        isolation: isolate;
        background: rgba(255, 255, 255, 0.3);
        box-shadow: 0px 6px var(--outer-shadow-blur) rgba(0, 0, 0, 0.2);
        border: 1px solid black;
      }
      .metric-box::before {
        content: '';
        position: absolute;
        inset: 0;
        border-radius: 20px;
        z-index: 1;
        box-shadow: inset var(--shadow-offset) var(--shadow-offset) var(--shadow-blur) var(--shadow-spread) var(--shadow-color);
        background: rgba(255, 255, 255, 0.15);
      }
      .metric-box::after {
        content: '';
        position: absolute;
        inset: 0;
        z-index: 0;
        border-radius: 20px;
        backdrop-filter: blur(var(--frost-blur));
      }
      .metric-title, .metric-value {
        position: relative;
        z-index: 2;
      }
      .metric-title {
        font-size: 1rem;
        opacity: 0.85;
        margin-bottom: 4px;
      }
      .metric-value {
        font-size: 1.6rem;
        font-weight: bold;
      }
      .main-content {
        display: flex;
        margin-bottom: 25px;
        flex-direction: column;
        backdrop-filter: blur(20px) saturate(180%);
        background: rgba(255, 255, 255, 0.4);
        border-radius: 20px;
        padding: 12px 30px;
        box-shadow: 0 10px 30px rgba(0,0,0,0.1);
        overflow: hidden;
        border: 1px solid black;
        color: #000;
      }
      .status-label {
        color: #000;
        font-weight: 600;
        margin-bottom: 12px;
      }
      .table-container h2 {
        margin: 0 0 14px;
        font-size: 1.5rem;
        color: #000;
      }
      .table {
        width: 100%;
        border-collapse: collapse;
        font-size: 0.85rem;
        color: #000;
        margin-bottom: 12px;
      }
      .table th, .table td {
        background: rgba(255, 255, 255, 0.4);
        backdrop-filter: blur(6px);
        padding: 6px 10px;
        border: 1px solid black;
        text-align: left;
      }
      .table th {
        font-weight: 600;
        color: #000;
      }
      .table tr:hover td {
        background: rgba(255, 255, 255, 0.6);
      }
      .btn {
        padding: 10px 24px;
        font-size: 1rem;
        font-weight: 600;
        border-radius: 12px;
        width: 160px;
        text-align: center;
        display: inline-block;
        transition: background 0.3s;
      }
      .btn-primary {
        background-color: #e9221f;
        color: white;
        border: none;
      }
      .btn-sync {
        background-color: #28a745;
        color: white;
        border: none;
      }
      .btn-primary:hover {
        background-color: #ff0000;
      }
      .btn-sync:hover {
        background-color: #218838;
      }
    </style>
</head>
<body>
    <form id="form1" runat="server">

        <!-- 🔒 Login Panel -->
<asp:Panel ID="pnlLogin" runat="server" Visible="true">
    <div style="display: flex; align-items: center; justify-content: center; height: 100vh;">
        <div style="display: flex; flex-direction: column; gap: 20px; padding: 40px; width: 100%; max-width: 400px;
                    background: rgba(255, 255, 255, 0.28); 
                    border-radius: 20px; 
                    border: 1px solid rgba(255,255,255,0.25); 
                    box-shadow: 0 8px 30px rgba(0, 0, 0, 0.2); 
                    backdrop-filter: blur(5px); 
                    -webkit-backdrop-filter: blur(5px);">
            
            <h2 style="margin: 0; text-align: center; font-size: 1.8rem; color: #000;">🔒 Admin Login</h2>

            <asp:Label ID="lblLoginStatus" runat="server" ForeColor="Red" Style="text-align:center;" />

            <div style="width: 100%;">
                <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" placeholder="Enter Master Password"
                    CssClass="form-control" style="width: 100%; padding: 14px; font-size: 1.1rem; border-radius: 12px; 
                    border: 1px solid #ccc; box-sizing: border-box;" />
            </div>

            <asp:Button ID="btnLogin" runat="server" Text="Login" CssClass="btn btn-primary"
                style="padding: 12px; font-size: 1.1rem; border-radius: 12px; width: 100%;" OnClick="btnLogin_Click" />
        </div>
    </div>
</asp:Panel>

        <!-- 📊 Dashboard Panel -->
        <asp:Panel ID="pnlDashboard" runat="server" Visible="false">
            <div style="text-align: center;">
  <header>Database Dashboard</header>
</div>
            <div class="dashboard-grid">
                <div class="metrics-bar">
                    <div class="metric-box"><div class="metric-title">Total Tables</div><div class="metric-value"><asp:Label ID="lblTotalTables" runat="server" Text="0" /></div></div>
                    <div class="metric-box"><div class="metric-title">Total Rows</div><div class="metric-value"><asp:Label ID="lblTotalRows" runat="server" Text="0" /></div></div>
                    <div class="metric-box"><div class="metric-title">Empty Tables</div><div class="metric-value"><asp:Label ID="lblEmptyTables" runat="server" Text="0" /></div></div>
                    <div class="metric-box"><div class="metric-title">Last Refreshed</div><div class="metric-value"><asp:Label ID="lblLastRefreshed" runat="server" Text="-" /></div></div>
                </div>
                <div class="main-content">
                    <asp:Label ID="lblStatus" runat="server" CssClass="status-label" EnableViewState="false" />
                    <div class="table-container">
                        <h2>Database Tables</h2>
                        <asp:GridView ID="gvTables" runat="server" AutoGenerateColumns="False" CssClass="table" EnableViewState="false">
                            <Columns>
                                <asp:BoundField DataField="TableName" HeaderText="Table Name" />
                                <asp:BoundField DataField="RowCount" HeaderText="Row Count" />
                            </Columns>
                        </asp:GridView>
                    </div>
                    <div style="display: flex; gap: 10px; margin-top: 14px; justify-content: flex-end;">
                        <asp:Button ID="btnSyncData" runat="server" Text="Sync" CssClass="btn btn-sync" OnClick="btnSyncData_Click" />
                        <asp:Button ID="btnFlushData" runat="server" Text="Flush Data" CssClass="btn btn-primary" OnClick="btnFlushData_Click" />
                    </div>
                </div>
            </div>
        </asp:Panel>

    </form>
</body>
</html>
