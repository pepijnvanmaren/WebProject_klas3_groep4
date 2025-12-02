import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import '../styles/VeilingProductPlaatsen.css';
import { useEffect } from 'react';

const apiBase = 'https://localhost:5174';

function VeilingBewerken() {
    const navigate = useNavigate();
    const [products, setProducts] = useState([]);
    const [loading, setLoading] = useState(false);

    useEffect(() => {
        window.scrollTo(0, 0);
    }, [])
    const handleGoBack = () => navigate('/VeilingMeesterDashboard');

    const ProductToeveogen = () => {
        alert("Product is toegevoegd");
        setTimeout(() => {
            window.location.reload();
        }, 1000);
    };


    return (
        <main className="vpp_dashboard-container">
            <h1 className="VerkoperDashboard-title">Product Toevoegen</h1>

            <div className="vpp_dashboard-box vpp_dashboard-flex">

                {/*Linker colom*/}
                <div className="vpp_dashboard-column">
                    <h1>Veilingen</h1>
                    <select name="Locatie selector" className="vp_selector">
                        <option value="" disabled selected>Select uw optie</option>
                        <option value="Den Haag">Den Haag</option>
                        <option value="Rotterdam">Rotterdam</option>
                        <option value="Amsterdam">Amsterdam</option>
                        <option value="Leiden">Leiden</option>
                    </select>

                    <div className="vpp_Veilingen-list-box">
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

                </div>

                {/*Midden colom*/}
                <div className="vpp_dashboard-column">
                    <h1>Huidige Veiling</h1>

                    <h2>Veiling ID</h2>
                    <p>2</p>

                    <h2>Startdatum veiling</h2>
                    <p>02/02/2026</p>

                    <h2>Starttijd veiling</h2>
                    <p>11:00</p>

                </div>

                {/*Rechter colom*/}
                <div className="vpp_dashboard-column">
                    <h1>Producten</h1>

                    <div className="vpp_product-list-box">
                        {loading ? (
                            <div className="loader">Laden...</div>
                        ) : products.length === 0 ? (
                            <div className="placeholder">
                                Geen Producten beschikbaar
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

                </div>
            </div>

            {/*Buttons*/}
            <div className="pp_buttons-row">
                <button className="pp_btn" onClick={ProductToeveogen}>Product Toevoegen</button>
                <button className="pp_btn" onClick={handleGoBack}>Terug</button>
            </div>
        </main>
    );

}

export default VeilingBewerken;
