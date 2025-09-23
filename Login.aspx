<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="MedicalSystem.Account.Login" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Employee Clearance - Login</title>
    <link href="Content/bootstrap.min.css" rel="stylesheet" />
    <script type="text/javascript">
        function validateLoginForm() {
            var username = document.getElementById('<%= txtUsername.ClientID %>').value.trim();
            var password = document.getElementById('<%= txtPassword.ClientID %>').value.trim();

            if (username === '' || password === '') {
                alert("Both username and password are required.");
                return false;
            }
            return true;
        }
    </script>
    <style>
        :root {
            --frost-blur: 6px;
        }

body {
    font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
    background: url('https://images.unsplash.com/photo-1497215728101-856f4ea42174?q=80&w=1170&auto=format&fit=crop&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D') center/cover no-repeat;
    margin: 0;
    padding: 0;
    height: 100vh;
    display: flex;
    align-items: center;
    justify-content: center;
    color: #333;
    overflow: hidden;
}

body::before {
    content: "";
    position: fixed;
    top: 0;
    left: 0;
    right: 0;
    bottom: 0;
    background: url('https://images.unsplash.com/photo-1497215728101-856f4ea42174?q=80&w=1170&auto=format&fit=crop&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D') center/cover no-repeat;
    filter: blur(1.8px);
    z-index: -1;
    transform: scale(1.05);
}
       .login-page {
            width: 100%;
            max-width: 1200px;
            display: flex;
            border-radius: 20px;
            overflow: hidden;
            backdrop-filter: blur(var(--frost-blur));
            -webkit-backdrop-filter: blur(var(--frost-blur));
            border: 1px solid rgba(0,0,0,0.3);
            box-shadow: 0 8px 30px rgba(0,0,0,0.3);
            isolation: isolate;
        }
        .login-branding {
            flex: 1;
            background-color: rgba(44, 125, 177, 0.40);
            padding: 40px;
            color: white;
            display: flex;
            flex-direction: column;
            justify-content: center;
            backdrop-filter: blur(var(--frost-blur));
            -webkit-backdrop-filter: blur(var(--frost-blur));
        }

        .login-form-container {
            flex: 1;
            padding: 40px;
            background-color: rgba(255,255,255,0.35);
            backdrop-filter: blur(var(--frost-blur));
            -webkit-backdrop-filter: blur(var(--frost-blur));
            display: flex;
            flex-direction: column;
            justify-content: center;
        }

        .login-logo {
            font-size: 28px;
            font-weight: 700;
            margin-bottom: 20px;
            letter-spacing: 1px;
        }

        .login-tagline {
            font-size: 18px;
            margin-bottom: 30px;
            opacity: 0.9;
        }

        .login-features {
            margin-top: 40px;
        }

        .feature-item {
            display: flex;
            align-items: center;
            margin-bottom: 15px;
        }

        .feature-icon {
            width: 24px;
            height: 24px;
            background-color: rgba(255, 255, 255, 0.3);
            border-radius: 50%;
            display: flex;
            align-items: center;
            justify-content: center;
            margin-right: 15px;
        }

        .login-form {
            width: 100%;
        }

        .form-title {
    font-size: 24px;
    font-weight: 600;
    margin-bottom: 30px;
    color: white;
}


        .form-group {
            margin-bottom: 25px;
            margin-top: 15px;
        }

        .form-control {
            width: 100%;
            padding: 12px 15px;
            border: 1px solid #d1e1f0;
            border-radius: 6px;
            font-size: 16px;
            transition: all 0.3s ease;
            background-color: rgba(255, 255, 255, 0.6);
        }

        .form-control:focus {
            border-color: #2c7db1;
            box-shadow: 0 0 0 3px rgba(44, 125, 177, 0.1);
            outline: none;
        }
        .uiverse {
  --duration: 2s;
  --easing: linear;
  /* Slightly desaturated colors */
  --c-color-1: rgba(26, 163, 255, 0.55);   /* blue - same */
  --c-color-2: #66cc66;   /* softer, lighter green */
  --c-color-3: #ff8fb3;   /* lighter, less saturated pink */
  --c-color-4: rgba(26, 232, 255, 0.55);   /* blue - same */
  --c-shadow: rgba(87, 223, 255, 0.3);     /* softer shadow */
  --c-shadow-inset-top: rgba(100, 180, 210, 0.6);
  --c-shadow-inset-bottom: rgba(190, 230, 240, 0.5);
  --c-radial-inner: #65a6c7;
  --c-radial-outer: #9acee1;
  --c-color: #fff;

  -webkit-tap-highlight-color: transparent;
  -webkit-appearance: none;
  outline: none;
  position: relative;
  cursor: pointer;
  border: none;
  display: table;
  border-radius: 24px;
  padding: 0;
  margin: 0;
  text-align: center;
  font-weight: 600;
  font-size: 16px;
  letter-spacing: 0.02em;
  line-height: 1.5;
  color: var(--c-color);

  /* Glass background */
  background: rgba(255, 255, 255, 0.15);
  backdrop-filter: blur(10px);
  -webkit-backdrop-filter: blur(10px);

  /* subtle radial gradient overlay */
  background-image: radial-gradient(circle at center, var(--c-radial-inner), var(--c-radial-outer) 80%);
  box-shadow:
    0 8px 32px rgba(0, 0, 0, 0.12),
    inset 0 0 0 1px rgba(255, 255, 255, 0.15);

  width: 100%;       
  height: 52px;      
  overflow: hidden;  
  box-sizing: border-box;
}

.uiverse:before {
  content: '';
  pointer-events: none;
  position: absolute;
  z-index: 3;
  left: 0;
  top: 0;
  right: 0;
  bottom: 0;
  border-radius: 24px;
  box-shadow: inset 0 3px 12px var(--c-shadow-inset-top), inset 0 -3px 4px var(--c-shadow-inset-bottom);
}

.uiverse .wrapper {
  -webkit-mask-image: -webkit-radial-gradient(white, black);
  overflow: hidden;
  border-radius: 24px;
  width: 100%;
  height: 52px;
  position: relative;
  padding: 12px 0;
  display: flex;
  align-items: center;
  justify-content: center;
}

.uiverse .wrapper span {
  position: relative;
  z-index: 10;
  flex-grow: 1;
  text-align: center;
  color: var(--c-color);
  font-weight: 600;
  user-select: none;
  text-shadow: 0 0 4px rgba(255, 255, 255, 0.7);
}

.uiverse .wrapper .circle {
  position: absolute;
  top: 6px;
  width: 40px;
  height: 40px;
  border-radius: 50%;
  filter: blur(var(--blur, 8px));
  background: var(--background, transparent);
  animation: var(--animation, none) var(--duration) var(--easing) infinite;
  will-change: transform;
}

/* softer color variations */
.uiverse .wrapper .circle.circle-1, 
.uiverse .wrapper .circle.circle-9, 
.uiverse .wrapper .circle.circle-10 {
  --background: var(--c-color-4);
}

.uiverse .wrapper .circle.circle-3, 
.uiverse .wrapper .circle.circle-4 {
  --background: var(--c-color-2);
  --blur: 14px;
}

.uiverse .wrapper .circle.circle-5, 
.uiverse .wrapper .circle.circle-6 {
  --background: var(--c-color-3);
  --blur: 16px;
}

.uiverse .wrapper .circle.circle-2, 
.uiverse .wrapper .circle.circle-7, 
.uiverse .wrapper .circle.circle-8, 
.uiverse .wrapper .circle.circle-11, 
.uiverse .wrapper .circle.circle-12 {
  --background: var(--c-color-1);
  --blur: 12px;
}

/* circle positions */
.uiverse .wrapper .circle.circle-1 { left: 0%; --animation: circle-1; }
.uiverse .wrapper .circle.circle-2 { left: 9%; --animation: circle-2; }
.uiverse .wrapper .circle.circle-3 { left: 18%; --animation: circle-3; }
.uiverse .wrapper .circle.circle-4 { left: 27%; --animation: circle-4; }
.uiverse .wrapper .circle.circle-5 { left: 36%; --animation: circle-5; }
.uiverse .wrapper .circle.circle-6 { left: 45%; --animation: circle-6; }
.uiverse .wrapper .circle.circle-7 { left: 54%; --animation: circle-7; }
.uiverse .wrapper .circle.circle-8 { left: 63%; --animation: circle-8; }
.uiverse .wrapper .circle.circle-9 { left: 72%; --animation: circle-9; }
.uiverse .wrapper .circle.circle-10 { left: 81%; --animation: circle-10; }
.uiverse .wrapper .circle.circle-11 { left: 90%; --animation: circle-11; }
.uiverse .wrapper .circle.circle-12 { left: 99%; --animation: circle-12; --blur: 14px; }

/* vertical animations */
@keyframes circle-1 {
  33% { transform: translateY(16px); }
  66% { transform: translateY(40px); }
}
@keyframes circle-2 {
  33% { transform: translateY(-10px); }
  66% { transform: translateY(-30px); }
}
@keyframes circle-3 {
  33% { transform: translateY(12px); }
  66% { transform: translateY(4px); }
}
@keyframes circle-4 {
  33% { transform: translateY(-12px); }
  66% { transform: translateY(-20px); }
}
@keyframes circle-5 {
  33% { transform: translateY(28px); }
  66% { transform: translateY(-32px); }
}
@keyframes circle-6 {
  33% { transform: translateY(-16px); }
  66% { transform: translateY(-56px); }
}
@keyframes circle-7 {
  33% { transform: translateY(28px); }
  66% { transform: translateY(-60px); }
}
@keyframes circle-8 {
  33% { transform: translateY(-4px); }
  66% { transform: translateY(-20px); }
}
@keyframes circle-9 {
  33% { transform: translateY(-12px); }
  66% { transform: translateY(-8px); }
}
@keyframes circle-10 {
  33% { transform: translateY(20px); }
  66% { transform: translateY(28px); }
}
@keyframes circle-11 {
  33% { transform: translateY(4px); }
  66% { transform: translateY(20px); }
}
@keyframes circle-12 {
  33% { transform: translateY(0px); }
  66% { transform: translateY(-32px); }
}


        .message {
            color: #e53e3e;
            text-align: center;
            padding: 8px;
            background-color: rgba(255, 245, 245, 0.8);
            border-radius: 6px;
            border-left: 4px solid #e53e3e;
        }

        .footer {
            text-align: center;
            margin-top: 30px;
            font-size: 14px;
            color: #000;
        }

        @media (max-width: 992px) {
            .login-page {
                flex-direction: column;
                max-width: 500px;
                margin: 20px;
            }
        }

        @media (max-width: 576px) {
            body {
                background-color: white;
            }

            .login-page {
                box-shadow: none;
                margin: 0;
                border-radius: 0;
            }
        }
        .dev-pill {
    position: fixed;
    bottom: 5px;
    left: 50%;
    transform: translateX(-50%);
    background: rgba(255, 255, 255, 0.15);
    backdrop-filter: blur(6px);
    -webkit-backdrop-filter: blur(6px);
    padding: 6px 14px;
    border-radius: 30px;
    font-size: 12px;
    font-weight: 500;
    color: #000;
    box-shadow: 0 2px 6px rgba(0,0,0,0.1);
    z-index: 9999;
    text-align: center;
}

.dev-pill a {
    color: #ffffff;
    text-decoration: none;
}



    </style>
</head>
<body>
    <form id="form1" runat="server" onsubmit="return validateLoginForm();">
        <div class="login-page">
<div class="login-branding" style="text-align: center; align-items:center">
    <img src="https://img.icons8.com/color/96/000000/company.png" alt="Company Logo" style="height: 90px; width: 70px; margin-bottom: 15px;">
    <div class="login-logo">Employee Clearance System</div>
    <div class="login-tagline">Smooth and Streamlined employee clearance</div>
    <div class="login-features">
        <div class="feature-item">
            <div class="feature-icon">✓</div>
            <div>Verify active users</div>
        </div>
        <div class="feature-item">
            <div class="feature-icon">✓</div>
            <div>Ensure clearance compliance</div>
        </div>
        <div class="feature-item">
            <div class="feature-icon">✓</div>
            <div>Manage accounts</div>
        </div>
    </div>
</div>

            <div class="login-form-container">
                <div class="login-form">
                    <h2 class="form-title">Sign in to your account</h2>
                    <asp:Label ID="lblMessage" runat="server" CssClass="message" Visible="false"></asp:Label>
                    <div class="form-group">
                        <asp:TextBox ID="txtUsername" runat="server" placeholder="Username" CssClass="form-control" />
                    </div>
                    <div class="form-group">
                        <asp:TextBox ID="txtPassword" runat="server" placeholder="Password" TextMode="Password" CssClass="form-control" />
                    </div>
                    <button runat="server" id="btnLogin" class="uiverse" type="submit" onserverclick="BtnLogin_Click">
                        <div class="wrapper">
                            <span>Sign In</span>
                            <div class="circle circle-12"></div>
                            <div class="circle circle-11"></div>
                            <div class="circle circle-10"></div>
                            <div class="circle circle-9"></div>
                            <div class="circle circle-8"></div>
                            <div class="circle circle-7"></div>
                            <div class="circle circle-6"></div>
                            <div class="circle circle-5"></div>
                            <div class="circle circle-4"></div>
                            <div class="circle circle-3"></div>
                            <div class="circle circle-2"></div>
                            <div class="circle circle-1"></div>
                        </div>
                    </button>

                    <div class="footer">
                        <p>&copy; <%= DateTime.Now.Year %> Employee Clearance System</p>
                        <p>
                            <a href="#" target="_blank" style="color: black; text-decoration: none;">
                                📄 User Manual
                            </a>
                        </p>
                    </div>
                </div>
            </div>
        </div>
    </form>
    <div class="dev-pill">
        <a href="#" target="_blank">Made with ❤️</a>
    </div>
</body>
</html>