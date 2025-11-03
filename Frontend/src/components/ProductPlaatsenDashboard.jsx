import '../styles/ProductTonenDashboard.css';
import { useNavigate } from 'react-router-dom';

function SellerDashboard() {
    const navigate = useNavigate();

    const ProductMakenKnop = () => {
        navigate('/verkoperDashboard')
    }

    const ProductGemaaktAlert = () => {
        alert("Uw prodcut is gemaakt.");
    }

    return (
        <><div className="dashboard-root">
            <header className="dashboard-header">
                <div className="header-inner">
                    <img
                        src="/header-trees.jpg"
                        alt="header"
                        className="header-image" />
                    <nav className="header-nav">
                        <a href="/registreren">Registreren</a>
                        <a href="/login">Inloggen</a>
                    </nav>
                </div>
            </header>
        </div>
            <main className="dashboard-container">
                <h1 className="title">Product aanmaken</h1>
                <div className="dashboard-box">
                    <h1>
                        Product naam
                    </h1>
                    <input
                        placeholder="Voer je Naam in"
                        className="input-field-Naam"
                    />
                    <h1>
                        Product infomatie
                    </h1>
                    <input
                        placeholder="Voer je Bescrhijving in"
                        className="input-field-Naam"
                    />
                    <h1>
                        Product foto
                    </h1>
                    <input
                        placeholder="Voer je Foto in"
                        className="input-field-Naam"
                    />
                    <button className="login-button" onClick={ProductGemaaktAlert}>
                        Product Maken
                    </button>
                    <button className="login-button" onClick={ProductMakenKnop} >
                        Terug
                    </button>
                </div>
            </main>
        </>
    );
}


export default SellerDashboard;