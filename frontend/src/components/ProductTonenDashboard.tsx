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
                <h1 className="title">Product</h1>
                <div className="dashboard-box">
                    <h1>
                        Product naam
                    </h1>
                    <p className="input-field-Naam">
                        nep tekst
                    </p>
                    <h1>
                        Product infomatie
                    </h1>
                    <p   className="input-field-infomatie">
                        nep tekst
                    </p>
                    <h1>
                        Product foto
                    </h1>
                    <img src="" alt="Foto van het gekozen product">
                    </img>
                    <button className="login-button" onClick={ProductMakenKnop} >
                        Terug
                    </button>
                </div>
            </main>
        </>
    );
}


export default SellerDashboard;