import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from "react-router-dom";
import '../styles/ProductInVeiling.css';

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

// Image helper functie
const getImageSrc = (foto?: string | null) => {
    if (!foto) return "";
    const s = foto.trim().replace(/^"|"$/g, "").replace(/\r?\n/g, "");
    if (s.startsWith("data:")) return s;
    if (s.startsWith("/9j/")) return `data:image/jpeg;base64,${s}`;
    if (s.startsWith("iVBOR")) return `data:image/png;base64,${s}`;
    return `data:image/*;base64,${s}`;
};

//const productsInVeiling = [
//    {
//        id: 1,
//        naam: "Rozen Rood",
//        beschrijving: "Verse rode rozen van hoge kwaliteit.",
//        afbeelding: null,
//    },
//    {
//        id: 2,
//        naam: "Tulpen Mix",
//        beschrijving: "Kleurrijke tulpen, perfect voor elk seizoen.",
//        afbeelding: null,
//    },
//];


function ProductInVeiling() {
    const { veilingId } = useParams();
    const navigate = useNavigate();
    const [products, setProducts] = useState<Product[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    

    useEffect(() => {
        const fetchdata = async () => {
            const loggedIn = localStorage.getItem("loggedIn") === "true";
            const token = localStorage.getItem("token");

            if (!loggedIn || !token) {
                alert("Je bent niet ingelogd!")
                navigate('/inloggen');
                return;
            }

            try {

                // Haal producten op van deze aanvoerder
                const productsResponse = await fetch(
                    `https://localhost:7020/api/Product/veiling/${veilingId}`,
                    {
                        headers: {
                            "Content-Type": "application/json",
                            Authorization: `Bearer ${token}`,
                        },
                    }
                );


                if(!productsResponse.ok) throw new Error("Kon producten niet ophalen");
                const productsData = await productsResponse.json();
                setProducts(productsData);

            }
            catch (err) {
                console.error("Error fetching data:", err);
                setError("Er is een fout opgetreden");
            }
            finally {setLoading(false);}

        }

        fetchdata();
    }, [navigate, veilingId]);


    const handleBack = () => {
        navigate('/veilingtonen');
    };

    const handleDeleteProduct = async (productId: number) => {
        if (!window.confirm("Weet je zeker dat je dit product wilt verwijderen?")) {
            return;
        }

        try {
            const token = localStorage.getItem("token");
            const response = await fetch(`https://localhost:7020/api/Product/${productId}`, {
                method: "DELETE",
                headers: {
                    "Content-Type": "application/json",
                    "Authorization": `Bearer ${token}`,
                }
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
                        Terug naar veilingen
                    </button>
                </div>
            </div>
        );
    }

    return (
        <div className="piv">
            <header className="piv__header">
                <div className="piv__header-content">
                    <h1>Producten in veiling</h1>
                    <p>Overzicht van alle producten in deze veiling</p>
                </div>
            </header>

            <main className="piv__main">
                <div className="piv__controls">
                    <button className="piv__btn-back" onClick={() => navigate("/VeilingTonen")}>
                        Terug naar veilingen
                    </button>
                </div>

                <div className="piv__grid">
                    {products.map((product) => (
                        <div key={product.id} className="piv__card">
                            <div className="piv__image-container">
                                {product.foto ? (
                                    <img
                                        src={getImageSrc(product.foto)}
                                        alt={product.naam}
                                        className="piv__image"
                                    />
                                ) : (
                                    <div className="piv__no-image">Geen afbeelding</div>
                                )}
                            </div>

                            <div className="piv__content">
                                <h3 className="piv__name">{product.naam}</h3>
                                <p className="piv__description">
                                    {product.beschrijving || "Geen beschrijving"}
                                </p>

                                <button
                                    onClick={() => handleDeleteProduct(product.id)}

                                    className="piv__btn-delete"
                                >
                                    Verwijderen
                                </button>
                            </div>
                        </div>
                    ))}
                </div>
            </main>
        </div>
    );
}

export default ProductInVeiling;