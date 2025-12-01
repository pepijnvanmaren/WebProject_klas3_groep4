import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import '../styles/VeilingMeesterDashboard.css';
import { useEffect } from 'react';

const apiBase = 'https://localhost:5174';

function VeilingMeesterDashboard() {
    const navigate = useNavigate();
    const [products, setProducts] = useState([]);
    const [loading, setLoading] = useState(false);

    useEffect(() => {
        window.scrollTo(0, 0);
    }, [])

    const VeiulingPlaatsenKnop = () => {
        navigate('/VeilingPlaatsen')
    }

    const VeilingenTonenKnop = () => {
        navigate('/VeilingTonen')
    }

    const VeilingBewerkenKnop = () => {
        navigate('/VeilingBewerken')
    }

    return (
        <>
            <main className="dashboard-container">
                <h1 className="VerkoperDashboard-title">Veilingmeester dashboard</h1>
                <div className="dashboard-box">
                    <button className="btn" onClick={VeiulingPlaatsenKnop}>
                        Veilingen Plaatsen
                    </button>
                    <button className="btn" onClick={VeilingenTonenKnop}>
                        Veilingen Tonen
                    </button>
                    <button className="btn" onClick={VeilingBewerkenKnop}>
                        Producten Toevoegen
                    </button>
                    <button
                        className="btn"
                        onClick={() => {
                            if (products.length === 0) {
                                alert("Geen producten om te verwijderen.");
                                return;
                            }
                            //   deleteProduct(products[products.length - 1].id);
                        }}>
                        Veilingen Verwijderen
                    </button>
                </div>
            </main>
            <div className="product-list-box">
                {loading ? (
                    <div className="loader">Laden...</div>
                ) : products.length === 0 ? (
                    <div className="placeholder">
                        Geen Veilingen beschikbaar
                    </div>
                ) : (
                    <ul className="product-list">
                        {products.map((product) => (
                            <li key={product.id} className="product-item">
                                <span>{product.name}</span>
                            </li>
                        ))}
                    </ul>
                )}
            </div>
        </>
    );
}


export default VeilingMeesterDashboard;