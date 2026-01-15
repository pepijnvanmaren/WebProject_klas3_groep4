import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import '../styles/VeilingTonen.css';


type Veiling = {
    id: number;
    starTijd: string;
    startDatum: string;
    aantalProducten: number;
    klokLocatie: string;
    huidigeSituatieVanVeiling: string;
    bechrijving: string;
    veilingmeesterId: number;
    veilingmeesterNaam: string;
    producten?: Array<any>;
};

type User = {
    id: number;
    userName: string;
    email: string;
    phoneNumber: string;
    rol: string;
    veilingVestiging: string | null;
};

function VeilingTonen() {
    const navigate = useNavigate();
    const [veilingen, setVeilingen] = useState<Veiling[]>([]);
    const [loading, setLoading] = useState<boolean>(true);
    const [error, setError] = useState<string | null>(null);
    const [user, setUser] = useState<User | null>(null);


    useEffect(() => {
        window.scrollTo(0, 0);
    }, [])

    useEffect(() => {
        const fetchVeilingen = async () => {
            const loggedIn = localStorage.getItem("loggedIn") === "true";

            if (!loggedIn) {
                navigate('/inloggen');
                return;
            }

            try {
                const token = localStorage.getItem("token");
                const userResponse = await fetch("https://localhost:7020/api/Auth/me", {
                    
                    headers: {
                        "Content-Type": "application/json",
                        "Authorization": `Bearer ${token}`
                    }
                });

                if (userResponse.ok) {
                    const userData = await userResponse.json();
                    setUser(userData);

                    const productsResponse = await fetch(
                        `https://localhost:7020/api/veiling/veilingmeester/${userData.id}`

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

    const handleDeleteVeiling = async (id: number) => {
        if (!window.confirm("Weet je zeker dat je dit veiling wilt verwijderen?")) {
            return;
        }

        try {
            const token = localStorage.getItem("token");
            const response = await fetch(`https://localhost:7020/api/veiling/${id}`, {
                method: "DELETE",
                
                headers: {
                    "Content-Type": "application/json",
                    "Authorization": `Bearer ${token}`
                }
            });

            if (response.ok || response.status === 204) {
                alert("Veiling succesvol verwijderd!");
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

    const handleStartVeiling = async (id: number) => {
        try {
            const token = localStorage.getItem("token");
            const resp = await fetch(`https://localhost:7020/api/veiling-process/${id}/start`, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    "Authorization": `Bearer ${token}`
                },
            });

            if (!resp.ok) {
                const errorText = await resp.text();
                throw new Error(errorText || "Kon veiling niet starten");
            }

            const data = await resp.json();
            console.log("Veiling gestart:", data);

            alert("Veiling gestart!");
        } catch (err: any) {
            alert(err.message || "Er is een fout opgetreden");
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
            <header className="dashboard-Veiling-Tonen-header">
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
                        + Nieuwe Veiling
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
                            Voeg je eerste Veiling toe
                        </button>
                    </div>
                ) : (
                    <div className="products-grid">
                        {veilingen.map((veiling) => (
                            <div key={veiling.id} className="product-card"
                                onClick={() => navigate(`/veiling/${veiling.id}`)}
                                style={{ cursor: "pointer" }}
                            >
                                <div className="product-content">
                                    <p className="product-description">
                                        {veiling.bechrijving || "Geen beschrijving"}
                                    </p>

                                    <div className="product-details">
                                        <div className="detail-item">
                                            <span className="detail-label">StarTijd:</span>
                                            <span className="detail-value">{veiling.starTijd}</span>
                                        </div>
                                        <div className="detail-item">
                                            <span className="detail-label">HuidigeSituatieVanVeiling:</span>
                                            <span className="detail-value">{veiling.huidigeSituatieVanVeiling}</span>
                                        </div>
                                        <div className="detail-item">
                                            <span className="detail-label">KlokLocatie:</span>
                                            <span className="detail-value">{veiling.klokLocatie}</span>
                                        </div>
                                        {veiling.startDatum && (
                                            <div className="detail-item">
                                                <span className="detail-label">StartDatum:</span>
                                                <span className="detail-value">
                                                    {new Date(veiling.startDatum || "Geen beschrijving" ).toLocaleDateString('nl-NL')}
                                                </span>
                                            </div>
                                        )}
                                    </div>

                                    <div className="product-actions">
                                        <button
                                            onClick={() => handleStartVeiling(veiling.id)}
                                            className="btn-start"
                                            disabled={veiling.huidigeSituatieVanVeiling === "Gestart"}
                                        >
                                            {veiling.huidigeSituatieVanVeiling === "Gestart" ? "Gestart" : "Veiling Starten"}
                                        </button>
                                        <button
                                            onClick={() => handleDeleteVeiling(veiling.id)}
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