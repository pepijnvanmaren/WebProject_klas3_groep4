import "../styles/index.css";
import React, { useEffect, useState, useRef } from "react";
import { useNavigate } from "react-router-dom";

//Refereerd naar de DTO
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
};

//Zet de image nvarchar om naar een image.
const getImageSrc = (foto?: string | null) => {
    if (!foto) return "";
    const s = foto.trim().replace(/^"|"$/g, "").replace(/\r?\n/g, "");
    if (s.startsWith("data:")) return s;
    if (s.startsWith("/9j/")) return `data:image/jpeg;base64,${s}`;
    if (s.startsWith("iVBOR")) return `data:image/png;base64,${s}`;
    return `data:image/*;base64,${s}`;
};

function Index() {
    const navigate = useNavigate();
    const [isLoggedIn, setIsLoggedIn] = useState(false);
    const [products, setProducts] = useState<Product[]>([]);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);

    //De timer bar
    const intervalRef = useRef<NodeJS.Timeout | null>(null);
    const [price, setPrice] = useState(0);
    const [isRunning, setIsRunning] = useState(true);
    const [purchased, setPurchased] = useState(false);

    //Houd bij de product volgorde
    const currentProduct = products[0];
    const nextProduct = products[1];

    //Referentie voor het automatisch scrollen naar beneden
    const currentProductTitleRef = useRef<HTMLHeadingElement | null>(null);

    //Scroll functie
    const scrollToCurrentProduct = () => {
        if (!currentProductTitleRef.current) return;

        const headerOffset = window.innerHeight * 0.18; // account for 18vh header
        const elementPosition =
            currentProductTitleRef.current.getBoundingClientRect().top + window.scrollY;
        const offsetPosition = elementPosition - headerOffset;

        window.scrollTo({
            top: offsetPosition,
            behavior: "smooth",
        });
    };

    //Update de login status
    useEffect(() => {
        setIsLoggedIn(localStorage.getItem("loggedIn") === "true");
    }, []);

    //Fetch Products
    useEffect(() => {
        const getProducts = async () => {
            setLoading(true);
            setError(null);

            try {
                const response = await fetch("https://localhost:7020/api/Product");
                if (!response.ok) throw new Error("Could not load products");

                const data = (await response.json()) as Product[];
                setProducts(data);

                if (data[0]) setPrice(data[0].minimalePrijs * 10);
            } catch (err: any) {
                console.error(err);
                setError(err?.message ?? "Unknown error");
            } finally {
                setLoading(false);
            }
        };

        getProducts();
    }, []);

    //prijs berekeningen voor max/stapgrootte gebaseeerd op de min)
    const minPrice = currentProduct?.minimalePrijs ?? 0;
    const maxPrice = minPrice * 10;
    const progress =
        maxPrice > minPrice ? (price - minPrice) / (maxPrice - minPrice) : 0;
    const barColor = `rgb(${Math.round(255 * (1 - progress))}, ${Math.round(
        255 * progress
    )}, 0)`;

    //Timer updater
    useEffect(() => {
        if (!isRunning || !currentProduct) return;

        intervalRef.current = setInterval(() => {
            setPrice(prev => {
                const nextPrice = prev * 0.98; // decrease by 2%
                if (nextPrice <= minPrice) {
                    clearInterval(intervalRef.current!);
                    return minPrice;
                }
                return parseFloat(nextPrice.toFixed(2));
            });
        }, 1000);

        return () => clearInterval(intervalRef.current!);
    }, [isRunning, currentProduct, minPrice]);

    //Handle voor koopknop
    //Als je niet ingelogd bent dan navigeer je naar inloggen
    const handleBuy = () => {
        if (!isLoggedIn) {
            navigate("/inloggen");
            return;
        }
        else {
            clearInterval(intervalRef.current!);
            setIsRunning(false);
            setPurchased(true);
        }
    };

    //error handling voor producten ophalen
    if (loading) return <p>Laad producten...</p>;
    if (error) return <p>Error laad producten: {error}</p>;
    if (!currentProduct) return <p>Geen producten gevonden</p>;

    //Product foto laden
    const ProductImage = ({ product }: { product: Product }) =>
        product?.foto ? (
            <img src={getImageSrc(product.foto)} alt={product.naam} className="Roses" />
        ) : (
            <div className="no-image">No image available</div>
        );

    return (
        <div className="home-page">
            {/* Current Product */}
            <h2 className="home-title" ref={currentProductTitleRef}>
                Current product
            </h2>

            <div className="home-container home-container-current">
                <div className="home-box">
                    <ProductImage product={currentProduct} />
                </div>

                <div className="home-box home-box-description">
                    <h2 className="home-product-name">{currentProduct.naam}</h2>
                    <p className="home-description">{currentProduct.beschrijving}</p>
                </div>

                <div className="home-box">
                    {currentProduct.hoeveelheid} stuks
                </div>

                <div className="home-box home-box-price">
                    <div className="home-price-row">
                        <span className="home-price">
                            EUR {price.toFixed(2)}
                        </span>
                        <button
                            className="home-button"
                            onClick={handleBuy}
                            disabled={purchased}
                        >
                            {purchased ? "Gekocht" : "Koop"}
                        </button>
                    </div>
                </div>

                <div className="home-progress-container">
                    <div
                        className="home-progress-bar"
                        style={{
                            width: `${progress * 100}%`,
                            backgroundColor: barColor,
                            transition: "width 1s linear, background-color 1s linear",
                        }}
                    />
                </div>
            </div>

            {/* Next Product */}
            <h2 className="home-title">Volgend product</h2>

            <div className="home-container">
                <div className="home-box">
                    {nextProduct ? <ProductImage product={nextProduct} /> : "No next product"}
                </div>

                <div className="home-box home-box-description">
                    <h2 className="home-product-name">{nextProduct?.naam}</h2>
                    <p className="home-description">{nextProduct?.beschrijving}</p>
                </div>

                <div className="home-box">
                    {nextProduct?.hoeveelheid ?? "Onbekend aantal"} stuks
                </div>

                <div className="home-box"></div>
            </div>
        </div>
    );

}

export default Index;