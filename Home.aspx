<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="GenericSystem.Home" MasterPageFile="~/Site.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .content {
            background: url('https://images.unsplash.com/photo-1697618213042-97183851b47b?q=80&w=1932&auto=format&fit=crop&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D') center/cover no-repeat;
            padding: 40px;
        }

        .glass {
            background: rgba(255, 255, 255, 0.2);
            backdrop-filter: blur(15px);
            -webkit-backdrop-filter: blur(15px);
            border-radius: 16px;
            border: 1px solid rgba(255, 255, 255, 0.3);
            box-shadow: 0 8px 20px rgba(0, 0, 0, 0.15);
        }

        .dashboard-container { padding: 20px 0; }
        .welcome-banner { padding: 30px; margin-bottom: 30px; color: #2c7db1; display: flex; justify-content: space-between; align-items: center; }
        .welcome-content { flex: 1; }
        .welcome-banner h1 { font-size: 28px; margin-bottom: 10px; font-weight: 700; }
        .welcome-subtitle { font-size: 16px; opacity: 0.9; margin-bottom: 0; }
        .stats-container { display: grid; grid-template-columns: repeat(auto-fit, minmax(250px, 1fr)); gap: 20px; margin-bottom: 30px; }
        .stat-card { padding: 20px; display: flex; transition: transform 0.3s ease, box-shadow 0.3s ease; border-left: 4px solid #2c7db1; }
        .stat-card:hover { transform: translateY(-5px); box-shadow: 0 6px 15px rgba(0, 0, 0, 0.1); }
        .stat-icon { margin-right: 15px; display: flex; align-items: center; justify-content: center; }
        .stat-content h3 { font-size: 18px; margin-bottom: 5px; color: #2c7db1; }
        .stat-content p { font-size: 14px; color: #333; margin-bottom: 10px; }
        .stat-link { display: inline-block; color: #2c7db1; font-weight: 500; text-decoration: none; font-size: 14px; }
        .stat-link:hover { text-decoration: underline; }
        .recent-activity { margin-bottom: 30px; }
        .recent-activity h2 { font-size: 22px; margin-bottom: 20px; color: #333; border-bottom: 2px solid #e9f2fa; padding-bottom: 10px; }
        .activity-container { display: grid; grid-template-columns: repeat(auto-fit, minmax(300px, 1fr)); gap: 20px; }
        .activity-card { padding: 20px; }
        .activity-card h3 { font-size: 18px; margin-bottom: 15px; color: #2c7db1; }
        .action-list { list-style: none; padding: 0; margin: 0; }
        .action-list li { margin-bottom: 12px; }
        .action-list a { text-decoration: none; color: #333; display: flex; align-items: center; transition: color 0.3s ease; }
        .action-list a:hover { color: #2c7db1; }
        .action-icon { margin-right: 10px; font-style: normal; }
        .info-text { color: #333; line-height: 1.6; margin-bottom: 15px; }
    </style>

    <div class="dashboard-container">
        <!-- Banner -->
        <div class="welcome-banner glass">
            <div class="welcome-content">
                <h1>Welcome to the Employee Clearance System</h1>
                <p class="welcome-subtitle">Hello, <strong><%: Session["Username"] ?? "Guest" %></strong>! Use this dashboard to navigate through system modules.</p>
            </div>
            <div class="welcome-image">
                <img src="https://img.icons8.com/color/96/000000/dashboard.png" alt="Dashboard" />
            </div>
        </div>
<div class="stats-container">
    <div class="stat-card glass">
        <div class="stat-icon">
            <img src="https://img.icons8.com/color/48/000000/folder-invoices.png" alt="Module 1" />
        </div>
        <div class="stat-content">
            <h3>Module 1</h3>
            <p>Generic description for module 1</p>
            <a href="#" class="stat-link">Go to Module</a>
        </div>
    </div>
    <div class="stat-card glass">
        <div class="stat-icon">
            <img src="https://img.icons8.com/color/48/000000/folder-invoices.png" alt="Module 2" />
        </div>
        <div class="stat-content">
            <h3>Module 2</h3>
            <p>Generic description for module 2</p>
            <a href="#" class="stat-link">Go to Module</a>
        </div>
    </div>
    <div class="stat-card glass">
        <div class="stat-icon">
            <img src="https://img.icons8.com/color/48/000000/folder-invoices.png" alt="Module 3" />
        </div>
        <div class="stat-content">
            <h3>Module 3</h3>
            <p>Generic description for module 3</p>
            <a href="#" class="stat-link">Go to Module</a>
        </div>
    </div>
</div>


        <div class="recent-activity">
            <h2>Quick Actions</h2>
            <div class="activity-container">
                <div class="activity-card glass">
                    <h3>Actions</h3>
                    <ul class="action-list">
                        <li><a href="#"><i class="action-icon">➕</i> Action 1</a></li>
                        <li><a href="#"><i class="action-icon">📋</i> Action 2</a></li>
                        <li><a href="#"><i class="action-icon">📊</i> Action 3</a></li>
                        <li><a href="#"><i class="action-icon">⚙️</i> Action 4</a></li>
                    </ul>
                </div>

                <div class="activity-card glass">
                    <h3>System Info</h3>
                    <p class="info-text">This is a generic WebForms dashboard template. Replace modules and actions with your actual application functionality.</p>
                    <p class="info-text">Use the navigation menu to access different sections of the system.</p>
                </div>
            </div>
        </div>
    </div>

    <script>
      document.addEventListener('DOMContentLoaded', function () {
        showNotification({
          title: "Login Successful",
          message: "Welcome back, <%: Session["Username"] %>!",
            meta: "You have successfully logged in.",
            time: new Date().toLocaleTimeString()
        });
      });
    </script>
</asp:Content>
