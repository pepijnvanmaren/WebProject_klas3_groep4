import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import '../styles/ProductTonenDashboard.css';

type Product = {
    id: number;
    naam: string;
    foto: string | null;
    beschrijving: string | null;
    oogstdatum: string;
    potmaat: number | null;
    gewicht: number;
    steellengte: number;
    hoeveelheid: number;
    minimalePrijs: number;
    aanvoerderId: number;
    aanvoerderNaam: string | null;
    veilingId: number | null;
};

type User = {
    id: number;
    userName: string;
    email: string;
    phoneNumber: string;
    rol: string;
    veilingVestiging: string | null;
};

// Image helper functie
const getImageSrc = (foto?: string | null) => {
    if (!foto) return "";
    const s = foto.trim().replace(/^"|"$/g, "").replace(/\r?\n/g, "");
    if (s.startsWith("data:")) return s;
    if (s.startsWith("/9j/")) return `data:image/jpeg;base64,${s}`;
    if (s.startsWith("iVBOR")) return `data:image/png;base64,${s}`;
    return `data:image/*;base64,${s}`;
};

function ProductDashboard() {
    const navigate = useNavigate();
    const [user, setUser] = useState<User | null>(null);
    const [products, setProducts] = useState<Product[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    // Fetch gebruiker en producten
    useEffect(() => {
        const fetchData = async () => {
            const loggedIn = localStorage.getItem("loggedIn") === "true";
            const token = localStorage.getItem("token");
                
            if (!loggedIn || !token) {
                alert("Je bent niet ingelogd!")
                navigate('/inloggen');
                return;
            }

            try {
                // Haal gebruikersgegevens op
                const userResponse = await fetch("https://localhost:7020/api/Auth/me", {
                    headers: {
                        "Content-Type": "application/json",
                        "Authorization": `Bearer ${token}`,
                    }
                });

                if (!userResponse.ok) throw new Error("Kon gebruikersgegevens niet ophalen");
                const userData = await userResponse.json();
                setUser(userData);

                // Haal producten op van deze aanvoerder
                const productsResponse = await fetch(
                    `https://localhost:7020/api/Product/aanvoerder/${userData.id}`,
                    {
                        headers: {
                            "Content-Type": "application/json",
                            "Authorization": `Bearer ${token}`,
                        }
                    }
                );

                if (!productsResponse.ok) throw new Error("Kon producten niet ophalen");
                const productsData = await productsResponse.json();
                setProducts(productsData);
            } catch (err) {
                console.error("Error fetching data:", err);
                setError("Er is een fout opgetreden");
            } finally {
                setLoading(false);
            }
        };

        fetchData();
    }, [navigate]);

    const handleBack = () => {
        navigate('/VerkoperDashboard');
    };

    const handleDeleteProduct = async (productId: number) => {
        if (!window.confirm("Weet je zeker dat je dit product wilt verwijderen?")) {
            return;
        }

        try {
            const response = await fetch(`https://localhost:7020/api/Product/${productId}`, {
                method: "DELETE",
                credentials: "include",
            });

            if (response.ok || response.status === 204) {
                alert("Product succesvol verwijderd!");
                // Verwijder product uit state
                setProducts(products.filter(p => p.id !== productId));
            } else {
                alert("Kon product niet verwijderen");
            }
        } catch (err) {
            console.error("Error deleting product:", err);
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
                    <h1>Mijn Producten</h1>
                    {user && (
                        <p className="welcome-text">
                            Ingelogd als: <strong>{user.userName}</strong>
                        </p>
                    )}
                </div>
            </header>

            <main className="dashboard-main">
                <div className="dashboard-controls">
                    <button onClick={handleBack} className="btn-back">
                        Terug naar Dashboard
                    </button>
                    <button onClick={() => navigate('/ProductMakenDashboard')} className="btn-add">
                        + Nieuw Product
                    </button>
                </div>

                {products.length === 0 ? (
                    <div className="no-products">
                        <div className="no-products-icon">[Geen producten]</div>
                        <h2>Geen producten gevonden</h2>
                        <p>Je hebt nog geen producten toegevoegd.</p>
                        <button
                            onClick={() => navigate('/ProductMakenDashboard')}
                            className="btn-add-first"
                        >
                            Voeg je eerste product toe
                        </button>
                    </div>
                ) : (
                    <div className="products-grid">
                        {products.map((product) => (
                            <div key={product.id} className="product-card">
                                <div className="product-image-container">
                                    {product.foto ? (
                                        <img
                                            src={getImageSrc(product.foto)}
                                            alt={product.naam}
                                            className="product-image"
                                        />
                                    ) : (
                                        <div className="product-no-image">
                                            <span>Geen foto</span>
                                        </div>
                                    )}
                                </div>

                                <div className="product-content">
                                    <h3 className="product-name">{product.naam}</h3>
                                    <p className="product-description">
                                        {product.beschrijving || "Geen beschrijving"}
                                    </p>

                                    <div className="product-details">
                                        <div className="detail-item">
                                            <span className="detail-label">Hoeveelheid:</span>
                                            <span className="detail-value">{product.hoeveelheid}</span>
                                        </div>
                                        <div className="detail-item">
                                            <span className="detail-label">Prijs:</span>
                                            <span className="detail-value">{product.minimalePrijs} EUR</span>
                                        </div>
                                        {product.potmaat && (
                                            <div className="detail-item">
                                                <span className="detail-label">Potmaat:</span>
                                                <span className="detail-value">{product.potmaat}</span>
                                            </div>
                                        )}
                                        <div className="detail-item">
                                            <span className="detail-label">Gewicht:</span>
                                            <span className="detail-value">{product.gewicht}kg</span>
                                        </div>
                                        {product.steellengte > 0 && (
                                            <div className="detail-item">
                                                <span className="detail-label">Steellengte:</span>
                                                <span className="detail-value">{product.steellengte}cm</span>
                                            </div>
                                        )}
                                        {product.oogstdatum && (
                                            <div className="detail-item">
                                                <span className="detail-label">Oogstdatum:</span>
                                                <span className="detail-value">
                                                    {new Date(product.oogstdatum).toLocaleDateString('nl-NL')}
                                                </span>
                                            </div>
                                        )}
                                    </div>

                                    <div className="product-actions">
                                        <button
                                            onClick={() => handleDeleteProduct(product.id)}
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

                <div className="products-summary">
                    <p>Totaal aantal producten: <strong>{products.length}</strong></p>
                </div>
            </main>
        </div>
    );
}

export default ProductDashboard;