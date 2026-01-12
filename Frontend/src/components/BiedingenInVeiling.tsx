import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from "react-router-dom";
import '../styles/BiedingenInVeiling.css';

type VerkoopProduct = {
    id: number;
    hoeveelHeid: number;
    verkochtePrijs: number;
    verkoopDatum: string;
    productId: number;
    product: string | null;
    koperId: number;
    koper: string | null;
};
function BiedingenInVeiling() {
    const { ProductId } = useParams();
    const productId = Number(ProductId);
    const navigate = useNavigate();
    const [bieding, setBieding] = useState<VerkoopProduct[]>([]);
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

                // Haal Biedingen op van deze Product
                const biedingResponse = await fetch(
                    `https://localhost:7020/api/VerkochteProducten`,
                    {
                        headers: {
                            "Content-Type": "application/json",
                            Authorization: `Bearer ${token}`,
                        },
                    }
                );


                if (!biedingResponse.ok) throw new Error("Kon Biedingen niet ophalen");
                const biedingData = await biedingResponse.json();
                const filtered = biedingData.filter(
                    v => v.productId === productId   
                );
                setBieding(filtered);

            }
            catch (err) {
                console.error("Error fetching data:", err);
                setError("Er is een fout opgetreden");
            }
            finally {setLoading(false); }

        }

        fetchdata();
    }, [navigate, ProductId]);

    const handleBack = () => {
        navigate('/VeilingTonen');
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
                        Terug naar producten in veiling
                    </button>
                </div>
            </div>
        );
    }


    return (
        <div className="biv">
            <header className="biv__header">
                <div className="biv__header-content">
                    <h1>Biedingen van een product</h1>
                    <p>Overzicht van alle biedingen in dit product</p>
                </div>
            </header>

            <main className="biv__main">
                <div className="biv__controls">
                    <button className="biv__btn-back" onClick={(handleBack)}>
                        Terug naar veilingen
                    </button>
                </div>

                <div className="biv__grid">
                    {bieding.map((VerkoopProduct) => (
                        <div key={VerkoopProduct.id} className="biv__card">
                            <div className="biv__content">
                                <h1 className="biv__name">KoperId: {VerkoopProduct.koperId } </h1>
                                <h1 className="biv__description">HoeveelHeid: {VerkoopProduct.hoeveelHeid} stuks</h1>
                                <h1 className="biv__description">VerkochtePrijs: {VerkoopProduct.verkochtePrijs} euro</h1>
                                <h1 className="biv__description">Datum: {new Date(VerkoopProduct.verkoopDatum).toLocaleDateString('nl-NL')}</h1>
                            </div>
                        </div>
                    ))}
                </div>
            </main>
        </div>
    );
}

export default BiedingenInVeiling;