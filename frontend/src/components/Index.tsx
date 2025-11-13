import "../styles/index.css";
import React, { useEffect, useState, useRef } from "react";

// Raw API type (handles backend naming variations)
type RawProduct = {
    ID?: number;
    Naam?: string;
    Foto?: string;
    Beschrijving?: string | null;
    id?: number;
    naam?: string;
    foto?: string | null;
    beschrijving?: string | null;
};

// Normalized Product type
type Product = {
    id: number;
    naam?: string | null;
    foto?: string | null;
    beschrijving?: string | null;
};

function Index() {
    // --- PRICE TIMER LOGIC ---
    const [price, setPrice] = useState(30.0);
    const [isRunning, setIsRunning] = useState(true);
    const [purchased, setPurchased] = useState(false);
    const intervalRef = useRef<NodeJS.Timeout | null>(null);

    const minPrice = 5.0;
    const maxPrice = 30.0;

    const progress = (price - minPrice) / (maxPrice - minPrice);
    const barColor = `rgb(${Math.round(255 * (1 - progress))}, ${Math.round(
        255 * progress
    )}, 0)`;

    useEffect(() => {
        if (isRunning) {
            intervalRef.current = setInterval(() => {
                setPrice((prevPrice) => {
                    if (prevPrice <= minPrice) {
                        clearInterval(intervalRef.current!);
                        return minPrice;
                    }
                    return parseFloat((prevPrice - 0.1).toFixed(2));
                });
            }, 1000);
        }
        return () => clearInterval(intervalRef.current!);
    }, [isRunning]);

    const handleStop = () => {
        clearInterval(intervalRef.current!);
        setIsRunning(false);
        setPurchased(true);
    };

    // --- FETCH PRODUCTS LOGIC ---
    const [products, setProducts] = useState<Product[]>([]);
    const [loading, setLoading] = useState<boolean>(false);
    const [error, setError] = useState<string | null>(null);

    const getProducts = async () => {
        setLoading(true);
        setError(null);
        try {
            const response = await fetch("https://localhost:7020/api/Product");
            if (!response.ok) {
                const txt = await response.text();
                console.error("Server response:", txt);
                throw new Error("Could not load products");
            }

            const data: RawProduct[] = await response.json();

            // Normalize inconsistent backend naming
            const mapped = data.map((p) => ({
                id: p.id ?? p.ID ?? 0,
                naam: p.naam ?? p.Naam ?? "Unknown product",
                // If your API returns relative paths, prefix them with your backend URL
                foto: p.foto
                    ? p.foto.startsWith("http")
                        ? p.foto
                        : `https://localhost:7020/${p.foto}`
                    : null,
                beschrijving:
                    p.beschrijving ?? p.Beschrijving ?? "No description available.",
            }));

            setProducts(mapped);
        } catch (err: any) {
            console.error("Error fetching products:", err);
            setError(err?.message ?? "Unknown error");
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        void getProducts();
    }, []);

    // --- LOADING & ERROR STATES ---
    if (loading) return <p>Loading products...</p>;
    if (error) return <p>Error loading products: {error}</p>;

    // --- DEFINE CURRENT & NEXT PRODUCT ---
    const currentProduct = products[0];
    const nextProduct = products[1];

    return (
        <div className="page">
            <h1 className="page-title">Current product</h1>

            <div className="container">
                <div className="box">
                    {currentProduct?.foto ? (
                        <img
                            src={currentProduct.foto}
                            alt={currentProduct.naam ?? "Product image"}
                            className="Roses"
                        />
                    ) : (
                        <div className="no-image">No image available</div>
                    )}
                </div>

                <div className="box box-description">
                    <div>
                        <h2 className="product-name">{currentProduct?.naam ?? "Unknown"}</h2>
                        <p className="description">
                            {currentProduct?.beschrijving ?? "No description available."}
                        </p>
                    </div>
                </div>

                <div className="box">Go Roos Yourself B.V.</div>
                <div className="box">500 units</div>

                <div className="box box-price">
                    <div className="price-row">
                        <span className="price">EUR {price.toFixed(2)}</span>
                        <button
                            className="button"
                            onClick={handleStop}
                            disabled={purchased}
                        >
                            {purchased ? "Purchased" : "Buy"}
                        </button>
                    </div>
                </div>

                <div className="progress-bar-container integrated-bar">
                    <div
                        className="progress-bar"
                        style={{
                            width: `${progress * 100}%`,
                            backgroundColor: barColor,
                            transition: "width 1s linear, background-color 1s linear",
                        }}
                    ></div>
                </div>
            </div>

            <h1 className="page-title">Next product</h1>
            <div className="container">
                <div className="box">
                    {nextProduct?.foto ? (
                        <img
                            src={nextProduct.foto}
                            alt={nextProduct.naam ?? "Next product"}
                            className="Roses"
                        />
                    ) : (
                        <div className="no-image">No image available</div>
                    )}
                </div>

                <div className="box box-description">
                    <div>
                        <h2 className="product-name">{nextProduct?.naam ?? "Unknown"}</h2>
                        <p className="description">
                            {nextProduct?.beschrijving ?? "No description available."}
                        </p>
                    </div>
                </div>

                <div className="box">Go Roos Yourself B.V.</div>
                <div className="box">500 units</div>
                <div className="box"></div>
            </div>
        </div>
    );
}

export default Index;
