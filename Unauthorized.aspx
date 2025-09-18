<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Unauthorized.aspx.cs" Inherits="MedicalSystem.Unauthorized" MasterPageFile="~/Site.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="unauthorized-container">
        <div class="unauthorized-banner">
            <div class="unauth-icon">
                <img src="https://img.icons8.com/emoji/96/no-entry-emoji.png" alt="Unauthorized" />
            </div>
            <div class="unauth-message">
                <h1>Access Denied</h1>
                <p class="subtitle">
                    Sorry, <strong><asp:Literal ID="litUsername" runat="server"></asp:Literal></strong>. 
                    You do not have permission to view this page.
                </p>
                <p> Contact System Administrator for assistance.</p>
                <a href="Home.aspx" class="back-link">← Return to Dashboard</a>
            </div>
        </div>
    </div>

    <style>
.unauthorized-container {
    max-width: 900px;
    margin: 60px auto;
    padding: 40px 20px;
}
.unauthorized-banner {
    position: relative;
    background: linear-gradient(
        135deg,
        rgba(255, 0, 0, 0.35),
        rgba(255, 60, 60, 0.7)
    );
    border-left: 6px solid #d9534f;
    padding: 30px;
    border-radius: 12px;
    display: flex;
    align-items: center;
    box-shadow: 0 10px 25px rgba(255, 0, 0, 0.2);
    backdrop-filter: blur(14px) saturate(160%);
    -webkit-backdrop-filter: blur(14px) saturate(160%);
    border: 1px solid rgba(255, 0, 0, 0.25);
    color: white;
    background-blend-mode: screen, lighten;
    overflow: hidden;
}

.unauthorized-banner::before {
    content: "";
    position: absolute;
    top: 0;
    left: -40%;
    width: 200%;
    height: 100%;
    background: linear-gradient(
        120deg,
        rgba(255, 255, 255, 0.1) 0%,
        rgba(255, 255, 255, 0.5) 50%,
        rgba(255, 255, 255, 0.1) 100%
    );
    transform: skewX(-25deg);
    opacity: 0.5;
    pointer-events: none;
}


.unauth-icon {
    margin-right: 30px;
}

.unauth-message h1 {
    color: white;
    font-size: 28px;
    margin-bottom: 10px;
    font-weight: 700;
    text-transform: uppercase;
    letter-spacing: 1px;
}

     
.subtitle,
.unauth-message p {
    font-size: 16px;
    color: rgba(255, 255, 255, 0.95);
    margin-bottom: 15px;
}


.back-link {
    color: #ffffff;
    background-color: rgba(255, 255, 255, 0.15);
    padding: 8px 14px;
    border-radius: 6px;
    font-weight: 500;
    font-size: 14px;
    text-decoration: none;
    transition: background 0.2s ease;
    display: inline-block;
    margin-top: 10px;
    border: 1px solid rgba(255, 255, 255, 0.25);
}

.back-link:hover {
    background-color: rgba(255, 255, 255, 0.25);
    text-decoration: none;
}

@media (max-width: 768px) {
    .unauthorized-banner {
        flex-direction: column;
        text-align: center;
    }

    .unauth-icon {
        margin-bottom: 20px;
    }
}
    </style>
</asp:Content>
