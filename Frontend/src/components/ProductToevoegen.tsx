import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import '../styles/ProductToevoegen.css';

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

function ProductToevoegen() {
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
                    `https://localhost:7020/api/Product/legeProducten`,
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
        navigate(-1);
    };

    const handleToevoegen = async (id: number) => {
        if (!window.confirm("Weet je zeker dat je dit product wilt toevoegen?")) {
            return;
        }
        try {
            const token = localStorage.getItem("token");
            const response = await fetch(`https://localhost:7020/api/veiling/veilingToevoegen/${id}`, {
                method: "PUT",
                headers: {
                    "Content-Type": "application/json",
                    "Authorization": `Bearer ${token}`
                }
            });

            if (response.ok || response.status === 204) {
                window.location.reload();
            } else {
                alert("Kon product niet toevoegen");
            }
        } catch (err) {
            console.error("Error toevoegen product:", err);
            alert("Er is een fout opgetreden bij het toevoegen van een product");
        }
    };

    if (loading) {
        return (
            <div className="PT_product-dashboard">
                <div className="PT_loading-container">
                    <div className="PT_spinner"></div>
                    <p>Laden...</p>
                </div>
            </div>
        );
    }

    if (error) {
        return (
            <div className="PT_product-dashboard">
                <div className="PT_error-container">
                    <p className="PT_error-message">{error}</p>
                    <button onClick={handleBack} className="PT_btn-back">
                        Terug naar Dashboard
                    </button>
                </div>
            </div>
        );
    }

    return (
        <div className="PT_product-dashboard">
            <header className="PT_dashboard-header">
                <div className="PT_header-content">
                    <h1>Mijn Producten</h1>
                    {user && (
                        <p className="PT_welcome-text">
                            Ingelogd als: <strong>{user.userName}</strong>
                        </p>
                    )}
                </div>
            </header>

            <main className="PT_dashboard-main">
                <div className="PT_dashboard-controls">
                    <button onClick={handleBack} className="PT_btn-back">
                        Terug naar Dashboard
                    </button>
                </div>

                {products.length === 0 ? (
                    <div className="PT_no-products">
                        <div className="PT_no-products-icon">[Geen producten]</div>
                        <h2>Geen producten gevonden</h2>
                        <p>Je hebt nog geen producten om toe te voegen.</p>
                    </div>
                ) : (
                        <div className="PT_products-grid">
                        {products.map((product) => (
                            <div key={product.id} className="PT_product-card">
                                <div className="PT_product-image-container">
                                    {product.foto ? (
                                        <img
                                            src={getImageSrc(product.foto)}
                                            alt={product.naam}
                                            className="PT_product-image"
                                        />
                                    ) : (
                                            <div className="PT_product-no-image">
                                            <span>Geen foto</span>
                                        </div>
                                    )}
                                </div>

                                <div className="PT_product-content">
                                    <h3 className="PT_product-name">{product.naam}</h3>
                                    <p className="PT_product-description">
                                        {product.beschrijving || "Geen beschrijving"}
                                    </p>

                                    <div className="PT_product-details">
                                        <div className="PT_detail-item">
                                            <span className="PT_detail-label">Hoeveelheid:</span>
                                            <span className="PT_detail-value">{product.hoeveelheid}</span>
                                        </div>
                                        <div className="PT_detail-item">
                                            <span className="PT_detail-label">Prijs:</span>
                                            <span className="PT_detail-value">{product.minimalePrijs} EUR</span>
                                        </div>
                                        {product.potmaat && (
                                            <div className="PT_detail-item">
                                                <span className="PT_detail-label">Potmaat:</span>
                                                <span className="PT_detail-value">{product.potmaat}</span>
                                            </div>
                                        )}
                                        <div className="PT_detail-item">
                                            <span className="PT_detail-label">Gewicht:</span>
                                            <span className="PT_detail-value">{product.gewicht}kg</span>
                                        </div>
                                        {product.steellengte > 0 && (
                                            <div className="PT_detail-item">
                                                <span className="PT_detail-label">Steellengte:</span>
                                                <span className="PT_detail-value">{product.steellengte}cm</span>
                                            </div>
                                        )}
                                        {product.oogstdatum && (
                                            <div className="PT_detail-item">
                                                <span className="PT_detail-label">Oogstdatum:</span>
                                                <span className="PT_detail-value">
                                                    {new Date(product.oogstdatum).toLocaleDateString('nl-NL')}
                                                </span>
                                            </div>
                                        )}
                                    </div>

                                    <div className="PT_product-actions">
                                        <button
                                            onClick={(e) => { 
                                                e.stopPropagation();
                                                handleToevoegen(product.id)}
                                                }
                                            className="PT_btn-delete"
                                        >
                                            Product Toevoegen
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

export default ProductToevoegen;