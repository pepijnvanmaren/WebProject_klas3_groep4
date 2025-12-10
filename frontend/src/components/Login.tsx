import React, { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import "../styles/Login.css";

function Login() {
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [loading, setLoading] = useState(false);
    const navigate = useNavigate(); //gebruikt voor redirect

    //runs als je op login klikt
    const handleLogin = async () => {
        if (!email || !password) {      //Checkt of de velden ingevuld zijn
            alert("Fill in both fields");
            return;
        }

        setLoading(true);

        try {
            //Stuurt de credentials naar de backend
            const loginResponse = await fetch("https://localhost:7020/api/auth/login", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ email, password })
            });

            //Returns de foutmelding uit de backend
            if (!loginResponse.ok) {
                const message = await loginResponse.text();
                alert(message || "Login failed");
                setLoading(false);
                return;
            }

            //Haalt token en rol op
            const loginData = await loginResponse.json();
            const token = loginData.token;
            const role = loginData.rol;

            if (!token) {
                alert("No token received");
                setLoading(false);
                return;
            }

            //slaat token en rol op
            localStorage.setItem("token", token);
            localStorage.setItem("userRole", role);

            //Fetch voor user
            const meResponse = await fetch("https://localhost:7020/api/auth/me", {
                method: "GET",
                headers: {
                    Authorization: `Bearer ${token}`,   //checkt de bearer token jwt
                    "Content-Type": "application/json"
                }
            });

            if (meResponse.ok) {
                const user = await meResponse.json();
                localStorage.setItem("userName", user.userName);    //Slaat username op
            } else {
                console.warn("Failed to fetch user info. Status:", meResponse.status);
            }
            
            //Navigatie naar de juiste pagina op basis van rol
            switch (role) {
                case "Koper":
                    navigate("/koperdashboard");
                    break;
                case "Aanvoerder":
                    navigate("/verkoperDashboard");
                    break;
                case "Admin":
                    navigate("/adminDashboard");
                    break;
                default:
                    navigate("/");
            }
        } catch (error) {
            console.error("Login error:", error);
            alert("Cannot connect to server.");
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="app-container">
            <div className="giveEmail">
                <h1>Inloggen</h1>

                <p>E-mail</p>
                <input
                    type="email"
                    placeholder="Voer je E-mail in"
                    className="input-field-Email"
                    value={email}
                    onChange={(e) => setEmail(e.target.value)}
                />
            </div>

            <div className="givePassword">
                <p>Password</p>
                <input
                    type="password"
                    placeholder="Enter your password"
                    value={password}
                    onChange={(e) => setPassword(e.target.value)}
                />
            </div>

            <button
                className="login-button"
                onClick={handleLogin}
                disabled={loading}
            >
                {loading ? "Please wait..." : "Login"}
            </button>

            <div className="signup-section">
                <p>Don't have an account?</p>
                <Link to="/registreren" className="signup-link">
                    Create account
                </Link>
            </div>
        </div>
    );
}

export default Login;
