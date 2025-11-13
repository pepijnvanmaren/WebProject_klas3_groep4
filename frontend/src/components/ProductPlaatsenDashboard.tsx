import "../styles/ProductPlaatsenDashboard.css";
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

                <div className="dashboard-maken-box">
                    {/* Linker kolom */}
                    <div className="register-column-left">
                        <p>
                            Product naam
                        </p>
                        <input
                            className="input-container"
                            placeholder="Voer je Naam in"
                        />
                        <p>
                            Product foto
                        </p>
                        <input
                            className="input-container"
                            placeholder="Voer je Foto in"
                        />
                        <button className="login-button" onClick={ProductGemaaktAlert}>
                            Product Maken
                        </button>
                    </div>

                    {/* Rechter kolom */}
                    <div className="register-column-right">
                        <p>
                            Product infomatie
                        </p>
                        <textarea
                            className="input-container-beschrijving"
                            placeholder="Voer je Bescrhijving in"
                        />

                        <button className="login-button" onClick={ProductMakenKnop} >
                            Terug
                        </button>
                    </div>
                </div>
            </main>
        </>
    );
}


export default SellerDashboard;