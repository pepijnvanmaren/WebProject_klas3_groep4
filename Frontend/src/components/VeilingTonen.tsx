import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import '../styles/VeilingTonen.css';


type Veiling = {
    id: number;
    StarTijd: string;
    StartDatum: string;
    AantalProducten: number;
    KlokLocatie: number;
    HuidigeSituatieVanVeiling: string;
    Bechrijving: string;
    VeilingmeesterId: number;
    Veilingmeester: string;
    Producten: Array<any>;
};

function VeilingTonen() {
    const navigate = useNavigate();
    const [veilingen, setVeilingen] = useState<Veiling[]>([]);
    const [loading, setLoading] = useState<boolean>(true);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        const fetchVeilingen = async () => {
            const loggedIn = localStorage.getItem("loggedIn") === "true";

            if (!loggedIn) {
                navigate('/inloggen');
                return;
            }

            try {
                const userResponse = await fetch("https://localhost:7020/api/Auth/me", {
                    credentials: "include",
                });

                if (userResponse.ok) {

                    const productsResponse = await fetch(
                        `https://localhost:7020/api/veiling`
                    );
                    if (productsResponse.ok) {
                        const veilingData = await productsResponse.json();
                        setVeilingen(veilingData);
                    } else {
                        setError("Kon veilingen niet ophalen");
                    }
                } else {
                    setError("Kon gebruikersgegevens niet ophalen");
                }
            } catch (err) {
                console.error("Error fetching data:", err);
                setError("Er is een fout opgetreden");
            } finally {
                setLoading(false);
            }
        };
        fetchVeilingen();
    }, [navigate]);

    const handleBack = () => {
        navigate('/VeilingMeesterDashboard');
    };

    const handleDeleteProduct = async (id: number) => {
        if (!window.confirm("Weet je zeker dat je dit product wilt verwijderen?")) {
            return;
        }

        try {
            const response = await fetch(`https://localhost:7020/api/veiling/${id}`, {
                method: "DELETE",
                credentials: "include",
            });

            if (response.ok || response.status === 204) {
                alert("Product succesvol verwijderd!");
                // Verwijder product uit state
                setVeilingen(veilingen.filter(p => p.id !== id));
            } else {
                alert("Kon veiling niet verwijderen");
            }
        } catch (err) {
            console.error("Error deleting veiling:", err);
            alert("Er is een fout opgetreden bij het verwijderen");
        }
    };

    if (loading) {
        return (
            <div className="product-dashboard">
                <div className="loading-container">
                    <div className="spinner"></div>
                    <p>Laden...</p>
                </div>
            </div>
        );
    }

    if (error) {
        return (
            <div className="product-dashboard">
                <div className="error-container">
                    <p className="error-message">{error}</p>
                    <button onClick={handleBack} className="btn-back">
                        Terug naar Dashboard
                    </button>
                </div>
            </div>
        );
    }

    return (
        <div className="product-dashboard">
            <header className="dashboard-header">
                <div className="header-content">
                    <h1>Mijn Veilingen</h1>
                </div>
            </header>

            <main className="dashboard-main">
                <div className="dashboard-controls">
                    <button onClick={handleBack} className="btn-back">
                        Terug naar Dashboard
                    </button>
                    <button onClick={() => navigate('/VeilingPlaatsen')} className="btn-add">
                        + Nieuw Product
                    </button>
                </div>

                {veilingen.length === 0 ? (
                    <div className="no-products">
                        <div className="no-products-icon">[Geen veilingen]</div>
                        <h2>Geen veilingen gevonden</h2>
                        <p>Je hebt nog geen veilingen toegevoegd.</p>
                        <button
                            onClick={() => navigate('/VeilingPlaatsen')}
                            className="btn-add-first"
                        >
                            Voeg je eerste product toe
                        </button>
                    </div>
                ) : (
                    <div className="products-grid">
                        {veilingen.map((veiling) => (
                            <div key={veiling.id} className="product-card">

                                <div className="product-content">
                                    <p className="product-description">
                                        {veiling.Bechrijving || "Geen beschrijving"}
                                    </p>

                                    <div className="product-details">
                                        <div className="detail-item">
                                            <span className="detail-label">StarTijd:</span>
                                            <span className="detail-value">{veiling.StarTijd}</span>
                                        </div>
                                        <div className="detail-item">
                                            <span className="detail-label">HuidigeSituatieVanVeiling:</span>
                                            <span className="detail-value">{veiling.HuidigeSituatieVanVeiling}</span>
                                        </div>
                                        <div className="detail-item">
                                            <span className="detail-label">KlokLocatie:</span>
                                            <span className="detail-value">{veiling.KlokLocatie}</span>
                                        </div>
                                        {veiling.StarTijd && (
                                            <div className="detail-item">
                                                <span className="detail-label">StarTijd:</span>
                                                <span className="detail-value">
                                                    {new Date(veiling.StarTijd).toLocaleDateString('nl-NL')}
                                                </span>
                                            </div>
                                        )}
                                    </div>

                                    <div className="product-actions">
                                        <button
                                            onClick={() => handleDeleteProduct(veiling.id)}
                                            className="btn-delete"
                                        >
                                            Verwijderen
                                        </button>
                                    </div>
                                </div>
                            </div>
                        ))}
                    </div>
                )}
            </main>
        </div>
    );
}

export default VeilingTonen;