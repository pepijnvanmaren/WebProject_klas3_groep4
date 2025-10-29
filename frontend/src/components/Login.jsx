import React, { useState } from "react"; 
import { Link } from "react-router-dom";
import "../styles/Login.css";

function Login() {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');

    const handleLogin = () => {
        if (email && password) {
            console.log('Inloggen met:', email);
            // Hier komt de login logica
        } else {
            alert('Vul beide velden in!');
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

            <button className="login-button" onClick={handleLogin}>
                Inloggen
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
