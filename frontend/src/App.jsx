import treePicture from './assets/treePicture.png'
import royaleFloraLogo from './assets/royaleFloraLogo.svg' // voeg deze regel toe
import './App.css'

function App() {
    return (
        <div className="app-container">
            <img src={treePicture} alt="tree picture" className="tree-picture" />

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

            <div className="giveEmail">
                <h1>Inloggen</h1>
                <p>E-Mail</p>
                <input
                    type="text"
                    placeholder="Voer je E-Mail in"
                    className="input-field-Email"
                />
            </div>

            <div className="givePassword">
                <p>Wachtwoord</p>
                <input
                    type="password"
                    placeholder="Voer je wachtwoord in"
                    className="input-field-Password"
                />
            </div>

            <button className="login-button">Inloggen</button>

            <div className="signup-section">
                <p>Heb je nog geen account?</p>
                <a href="/registreren" className="signup-link">Account aanmaken</a>
            </div>
        </div>
    )
}

export default App

// cd frontend
//npm install
// npm run dev