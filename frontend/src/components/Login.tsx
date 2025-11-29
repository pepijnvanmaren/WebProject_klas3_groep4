import React, { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import "../styles/Login.css";

function Login() {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [loading, setLoading] = useState(false);
    const navigate = useNavigate();

    const handleLogin = async () => {
        if (!email || !password) {
            alert("Vul beide velden in!");
            return;
        }

        setLoading(true);
        try {
            const loginResponse = await fetch("https://localhost:7020/api/auth/login", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                credentials: "include",
                body: JSON.stringify({ email, password })
            });

            if (!loginResponse.ok) {
                const message = await loginResponse.text();
                alert(message || "Login mislukt");
                setLoading(false);
                return;
            }

            const meResponse = await fetch("https://localhost:7020/api/auth/me", {
                method: "GET",
                credentials: "include"
            });

            if (!meResponse.ok) {
                alert("Kon gebruiker niet ophalen");
                setLoading(false);
                return;
            }

            const user = await meResponse.json();

            // Sla login status op in localStorage
            localStorage.setItem("loggedIn", "true");
            localStorage.setItem("userRole", user.rol);

            if (user.rol === "Koper") {
                navigate("/koperdashboard");
            } else if (user.rol === "Aanvoerder") {
                navigate("/verkoperDashboard");
            } else if (user.rol === "Veilingmeester") {
                navigate("/");
            } else if (user.rol === "Admin") {
                navigate("/");
            }
        } catch (error) {
            console.error(error);
            alert("Er ging iets mis bij het verbinden met de server.");
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="app-container">
            <div className="giveEmail">
                <h1>Inloggen</h1>
                <p>E-Mail</p>
                <input
                    type="email"
                    placeholder="Voer je E-Mail in"
                    className="input-field-Email"
                    value={email}
                    onChange={(e) => setEmail(e.target.value)}
                />
            </div>
            <div className="givePassword">
                <p>Wachtwoord</p>
                <input
                    type="password"
                    placeholder="Voer je wachtwoord in"
                    className="input-field-Password"
                    value={password}
                    onChange={(e) => setPassword(e.target.value)}
                />
            </div>
            <button
                className="login-button"
                onClick={handleLogin}
                disabled={loading}
            >
                {loading ? "Even geduld..." : "Inloggen"}
            </button>
            <div className="signup-section">
                <p>Heb je nog geen account?</p>
                <Link to="/registreren" className="signup-link">
                    Account aanmaken
                </Link>
            </div>
        </div>
    );
}

export default Login;