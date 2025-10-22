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
                        <li><a href="#services">Diensten</a></li>
                        <li><a href="#contact">Contact</a></li>
                    </ul>
                </div>
            </nav>

            <div className="inlogGegevens">
                <h1>Inloggen</h1>
                <p>E-Mail</p>
                <input
                    type="text"
                    placeholder="Voer je E-Mail in"
                    className="input-field"
                />
            </div>
        </div>
    )
}

export default App