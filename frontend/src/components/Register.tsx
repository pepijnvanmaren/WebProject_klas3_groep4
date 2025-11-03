import React, { useState } from "react";
import { useNavigate } from 'react-router-dom';
import { Link } from "react-router-dom";
import "../styles/Register.css";

function Register() {
    const navigate = useNavigate();
    const [email, setEmail] = useState('')
    const [password, setPassword] = useState('')
    const [confirmPassword, setConfirmPassword] = useState('')
    const [naam, setNaam] = useState('')
    const [accountType, setAccountType] = useState('koper')

    const handleRegister = () => {
        if (!email || !password || !confirmPassword || !naam) {
            alert('Vul alle velden in!')
            return
        }

        if (password !== confirmPassword) {
            alert('Wachtwoorden komen niet overeen!')
            return
        }

        if (accountType === 'koper') {
            navigate('/')
        }

        if (accountType === 'verkoper') {
            navigate('/verkoperDashboard')
        }
    }

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
                            <p>Account type</p>
                            <div className="account-type-buttons">
                                <button
                                    type="button"
                                    className={`account-type-btn ${accountType === 'koper' ? 'active' : ''}`}
                                    onClick={() => setAccountType('koper')}
                                >
                                    Koper
                                </button>
                                <button
                                    type="button"
                                    className={`account-type-btn ${accountType === 'verkoper' ? 'active' : ''}`}
                                    onClick={() => setAccountType('verkoper')}
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
                                value={password}
                                onChange={(e) => setPassword(e.target.value)}
                            />
                        </div>

                        <div className="form-group">
                            <p>Bevestig wachtwoord</p>
                            <input
                                type="password"
                                placeholder="Voer je wachtwoord opnieuw in"
                                className="input-field-Password"
                                value={confirmPassword}
                                onChange={(e) => setConfirmPassword(e.target.value)}
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
    )
}

export default Register;
