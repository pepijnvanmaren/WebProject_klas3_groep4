import { useState } from 'react'
import { BrowserRouter, Routes, Route, Link } from 'react-router-dom'
import treePicture from './assets/treePicture.png'
import royaleFloraLogo from './assets/royaleFloraLogo.svg'
import Register from './Register'
import './App.css'

function Login() {
    const [email, setEmail] = useState('')
    const [password, setPassword] = useState('')

    const handleLogin = () => {
        if (email && password) {
            console.log('Inloggen met:', email)
            // Hier komt de login logica
        } else {
            alert('Vul beide velden in!')
        }
    }

    return (
        <div className="app-container">
            <nav className="navbar">
                <div className="nav-content">
                    <img src={royaleFloraLogo} alt="Royale Flora" className="nav-logo" />
                    <ul className="nav-links">
                        <li><a href="#home">Home</a></li>
                        <li><a href="#about">Over ons</a></li>
                        <li><a href="#contact">Contact</a></li>
                    </ul>
                </div>
            </nav>

            <img src={treePicture} alt="tree picture" className="tree-picture" />

            <div className="login-container">
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
                    <Link to="/registreren" className="signup-link">Account aanmaken</Link>
                </div>
            </div>
        </div>
    )
}

function App() {
    return (
        <BrowserRouter>
            <Routes>
                <Route path="/" element={<Login />} />
                <Route path="/registreren" element={<Register />} />
            </Routes>
        </BrowserRouter>
    )
}

export default App

//cd frotnend
//npm install
//npm run dev