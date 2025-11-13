import React, { useState } from "react";
import { useNavigate, Link } from "react-router-dom";
import "../styles/Register.css";

function Register() {
    const navigate = useNavigate();

    // States die overeenkomen met je model
    const [naam, setNaam] = useState('');
    const [telefoonnummer, setTelefoonnummer] = useState('');
    const [email, setEmail] = useState('');
    const [paswoord, setPaswoord] = useState('');
    const [confirmPaswoord, setConfirmPaswoord] = useState('');
    const [rol, setRol] = useState('koper'); // komt overeen met "Rol" in model

    // Functie om POST te doen
    const postAccount = async () => {
        try {
            const response = await fetch('https://localhost:7020/api/test', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({
                    naam: naam,
                    telefoonnummer: parseInt(telefoonnummer), // model verwacht int
                    email: email,
                    rol: rol,
                    paswoord: paswoord,
                }),
            });

            if (!response.ok) {
                const text = await response.text();
                console.error("Server response:", text);
                throw new Error('Registratie mislukt');
            }

            return response;
        } catch (error) {
            console.error('Fout bij registreren:', error);
            throw error;
        }
    };

    const handleRegister = async () => {
        // Validatie
        if (!email || !paswoord || !confirmPaswoord || !naam || !telefoonnummer) {
            alert('Vul alle velden in!');
            return;
        }

        if (paswoord !== confirmPaswoord) {
            alert('Wachtwoorden komen niet overeen!');
            return;
        }

        try {
            await postAccount();

            // Als succesvol, navigeer naar juiste pagina
            if (rol === 'koper') {
                navigate('/');
            } else if (rol === 'verkoper') {
                navigate('/verkoperDashboard');
            }
        } catch (error) {
            alert('Er ging iets mis bij het aanmaken van je account.');
        }
    };

    return (
        <div className="register-container">
            <h1 className="register-title">Account aanmaken</h1>

            <div className="register-form">
                {/* Linker kolom */}
                <div className="register-column-left">
                    <div className="form-group">
                        <p>Naam</p>
                        <input
                            type="text"
                            placeholder="Voer je naam in"
                            className="input-field-Email"
                            value={naam}
                            onChange={(e) => setNaam(e.target.value)}
                        />
                    </div>

                    <div className="form-group">
                        <p>Telefoonnummer</p>
                        <input
                            type="tel"
                            placeholder="Voer je telefoonnummer in"
                            className="input-field-Email"
                            value={telefoonnummer}
                            onChange={(e) => setTelefoonnummer(e.target.value)}
                        />
                    </div>

                    <div className="form-group">
                        <p>E-Mail</p>
                        <input
                            type="email"
                            placeholder="Voer je E-Mail in"
                            className="input-field-Email"
                            value={email}
                            onChange={(e) => setEmail(e.target.value)}
                        />
                    </div>

                    <div className="form-group">
                        <p>Rol</p>
                        <div className="account-type-buttons">
                            <button
                                type="button"
                                className={`account-type-btn ${rol === 'koper' ? 'active' : ''}`}
                                onClick={() => setRol('koper')}
                            >
                                Koper
                            </button>
                            <button
                                type="button"
                                className={`account-type-btn ${rol === 'verkoper' ? 'active' : ''}`}
                                onClick={() => setRol('verkoper')}
                            >
                                Verkoper
                            </button>
                        </div>
                    </div>
                </div>

                {/* Rechter kolom */}
                <div className="register-column-right">
                    <div className="form-group">
                        <p>Wachtwoord</p>
                        <input
                            type="password"
                            placeholder="Voer je wachtwoord in"
                            className="input-field-Password"
                            value={paswoord}
                            onChange={(e) => setPaswoord(e.target.value)}
                        />
                    </div>

                    <div className="form-group">
                        <p>Bevestig wachtwoord</p>
                        <input
                            type="password"
                            placeholder="Voer je wachtwoord opnieuw in"
                            className="input-field-Password"
                            value={confirmPaswoord}
                            onChange={(e) => setConfirmPaswoord(e.target.value)}
                        />
                    </div>

                    <button className="login-button" onClick={handleRegister}>
                        Registreren
                    </button>

                    <div className="login-section">
                        <p>Heb je al een account?</p>
                        <Link to="/inloggen" className="signup-link">Inloggen</Link>
                    </div>
                </div>
            </div>
           
        </div>
    );
}

export default Register;
