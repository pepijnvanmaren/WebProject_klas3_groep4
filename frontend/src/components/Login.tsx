import React, { useState, useEffect } from "react";
import { Link, useNavigate } from "react-router-dom";
import "../styles/Login.css";

function Login() {
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [loading, setLoading] = useState(false);

    const navigate = useNavigate();

    useEffect(() => {
        window.scrollTo(0, 0);
    }, [])

    const handleLogin = async () => {
        if (!email || !password) {
            alert("Vul beide velden in!");
            return;
        }

        setLoading(true);

        try {
            // Login
            const loginResponse = await fetch("https://localhost:7020/api/auth/login", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                credentials: "include",
                body: JSON.stringify({ email, password })
            });

            if (!loginResponse.ok) {
                const msg = await loginResponse.text();
                alert(msg || "Login mislukt");
                setLoading(false);
                return;
            }

            // User ophalen
            const meResponse = await fetch("https://localhost:7020/api/auth/me", {
                method: "GET",
                credentials: "include"
            });

            if (!meResponse.ok) {
                alert("Kon gebruiker niet ophalen.");
                setLoading(false);
                return;
            }

            const user = await meResponse.json();

            // Opslaan in localStorage
            localStorage.setItem("loggedIn", "true");
            localStorage.setItem("userRole", user.rol);

            // Navigatie op basis van rol
            switch (user.rol) {
                case "Koper":
                    navigate("/koperdashboard");
                    break;

                case "Aanvoerder":
                    navigate("/verkoperDashboard");
                    break;

                case "Veilingmeester":
                    navigate("/veilingmeesterdashboard");
                    break;

                case "Admin":
                    navigate("/admindashboard");
                    break;

                default:
                    alert("Onbekende rol: " + user.rol);
                    break;
            }

        } catch (err) {
            console.error(err);
            alert("Er ging iets mis bij het verbinden met de server.");
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
