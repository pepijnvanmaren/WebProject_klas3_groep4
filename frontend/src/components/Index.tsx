import "../styles/index.css";
import React, { useEffect, useState, useRef } from "react";

// Matches ProductOutputDto from backend
type Product = {
    id: number;
    naam: string;
    foto: string | null;
    beschrijving: string | null;
    oogstdatum: string;           // DateOnly comes as ISO string
    potmaat: number | null;
    gewicht: number;
    steellengte: number;
    hoeveelheid: number;
    minimalePrijs: number;
};

const getImageSrc = (foto?: string | null) => {
    if (!foto) return "";
    const s = foto.trim();

    if (s.startsWith("data:")) return s;

    const cleaned = s.replace(/^"|"$/g, "").replace(/\r?\n/g, "");

    if (cleaned.startsWith("/9j/")) return `data:image/jpeg;base64,${cleaned}`;
    if (cleaned.startsWith("iVBOR")) return `data:image/png;base64,${cleaned}`;

    return `data:image/*;base64,${cleaned}`;
};

function Index() {
    // Timer
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
                setPrice((prev) => {
                    if (prev <= minPrice) {
                        clearInterval(intervalRef.current!);
                        return minPrice;
                    }
                    return parseFloat((prev - 0.1).toFixed(2));
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

    // Fetch products
    const [products, setProducts] = useState<Product[]>([]);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);

    const getProducts = async () => {
        setLoading(true);
        setError(null);

        try {
            const response = await fetch("https://localhost:7020/api/Product");
            if (!response.ok) throw new Error("Could not load products");

            const data = (await response.json()) as Product[];
            setProducts(data);
        } catch (err: any) {
            console.error("Error fetching products:", err);
            setError(err?.message ?? "Unknown error");
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        getProducts();
    }, []);

    // Loading states
    if (loading) return <p>Loading products...</p>;
    if (error) return <p>Error loading products: {error}</p>;
    if (products.length === 0) return <p>No products found.</p>;

    const currentProduct = products[0];
    const nextProduct = products[1];

    return (
        <div className="page">
            <h1 className="page-title">Current product</h1>

            <div className="container">
                {/* IMAGE */}
                <div className="box">
                    {currentProduct?.foto ? (
                        <img
                            src={getImageSrc(currentProduct.foto)}
                            alt={currentProduct.naam}
                            className="Roses"
                        />
                    ) : (
                        <div className="no-image">No image available</div>
                    )}
                </div>

                {/* DESCRIPTION */}
                <div className="box box-description">
                    <h2 className="product-name">{currentProduct.naam}</h2>
                    <p className="description">{currentProduct.beschrijving}</p>
                </div>

                {/* COMPANY + STOCK */}
                <div className="box">Go Roos Yourself B.V.</div>
                <div className="box">{currentProduct.hoeveelheid} units</div>

                {/* PRICE + BUY */}
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

                {/* PROGRESS BAR */}
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

            {/* NEXT PRODUCT */}
            <h1 className="page-title">Next product</h1>
            <div className="container">
                <div className="box">
                    {nextProduct?.foto ? (
                        <img
                            src={getImageSrc(nextProduct.foto)}
                            alt={nextProduct.naam}
                            className="Roses"
                        />
                    ) : (
                        <div className="no-image">No image available</div>
                    )}
                </div>

                <div className="box box-description">
                    <h2 className="product-name">{nextProduct?.naam}</h2>
                    <p className="description">{nextProduct?.beschrijving}</p>
                </div>

                <div className="box">Go Roos Yourself B.V.</div>
                <div className="box">{nextProduct?.hoeveelheid ?? "Unknown"} units</div>
                <div className="box"></div>
            </div>
        </div>
    );
}

export default Index;
