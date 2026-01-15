import "../styles/index.css";
import React, { useEffect, useState, useRef } from "react";
import { useNavigate } from "react-router-dom";

//Refereerd naar DTO
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
}

type Review = {
    id: number;
    naam: string;
    tekst: string;
    sterren: number; 
};

const dummyReviews: Review[] = [
    { id: 1, naam: "Sophie V.", tekst: "Fantastische service en prachtige bloemen!", sterren: 5 },
    { id: 2, naam: "Jan K.", tekst: "Snelle levering en goede kwaliteit.", sterren: 4 },
    { id: 3, naam: "Lotte M.", tekst: "Zeer tevreden, zeker een aanrader.", sterren: 5 },
    { id: 4, naam: "Emma T.", tekst: "Goede prijzen en vriendelijke klantenservice.", sterren: 4 },
    { id: 5, naam: "Mark D.", tekst: "De bloemen waren vers en mooi verpakt.", sterren: 5 },
];

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

    //Houd de product volgorde bij
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
                if (!response.ok) throw new Error("Kon producten niet laden.");

                const data = (await response.json()) as Product[];
                setProducts(data);

                if (data[0]) setPrice(data[0].minimalePrijs * 10);
            } catch (err: any) {
                console.error(err);
                setError(err?.message ?? "Onbekende error");
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
                const nextPrice = prev * 0.98; // verminder met 2%
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
            <div className="no-image">Geen afbeelding gevonden</div>
        );

    return (
        <div className="page">
            {/* Over ons */}
            <div className="user-welcome">
                    <h1>Welkom bij Floriday!</h1>
                <p>
                    Bekijk ons huidige product en profiteer van de dalende prijs.
                </p>
            </div>

                {isLoggedIn && (
                    <div className="login-register">
                    <button
                        className="button login-button"
                            onClick={() => navigate("/inloggen")}
                        >
                            Inloggen
                        </button>

                    <button
                        className="button login-button"
                            onClick={() => navigate("/registreren")}
                        >
                            Registreren
                        </button>
                    </div>
                )}

            {/* Houdig product */}
            <div className="container">
                <div className="box">
                    <ProductImage product={currentProduct} /> </div>

                <div className="box box-description">
                    <h2 className="product-name">{currentProduct.naam}</h2>
                    <p className="description">{currentProduct.beschrijving}</p>
                </div>

                <div className="box">{currentProduct.hoeveelheid} stuks</div>
                <div className="box">{currentProduct.oogstdatum} geoogst</div>

                <div className="box">
                    <p>{currentProduct.potmaat} cm potmaat</p>
                    <p>{currentProduct.gewicht} kg gewicht</p>
                    <p>{currentProduct.steellengte} cm steellengte</p>
                </div>

                <div className="box box-price">
                    <div className="price-row">
                        <span className="price">EUR {price.toFixed(2)}</span>
                        <button
                            className="button"
                            onClick={handleBuy}
                            disabled={purchased}
                        >
                            {purchased ? "Gekocht" : "Koop"}
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
                    />
                </div>
            </div>

            {/* Next Product */}
            <div className="container">
                <div className="box">
                    {nextProduct ? <ProductImage product={nextProduct} /> : "Geen volgend product"}
                </div>
                <div className="box box-description">
                    <h2 className="product-name">{nextProduct?.naam}</h2>
                    <p className="description">{nextProduct?.beschrijving}</p>
                </div>
                <div className="box">{nextProduct?.hoeveelheid ?? "Onbekend aantal"} stuks</div>
                <div className="box">{nextProduct?.oogstdatum} geoogst</div>
                <div className="box">
                    <p>{nextProduct?.potmaat} cm potmaat</p>
                    <p>{nextProduct?.gewicht} kg gewicht</p>
                    <p>{nextProduct?.steellengte} cm steellengte</p>
                </div>
            </div>

{/* Reviews */ }
<div className="reviews-section">
    <h2>Wat onze klanten zeggen</h2>
    <div className="reviews-container">
        {dummyReviews.map((review) => (
            <div key={review.id} className="review-card">
                <p className="review-text">"{review.tekst}"</p>
                <p className="review-name">- {review.naam}</p>
                <p className="review-rating">
                    {"⭐".repeat(review.sterren)}{" "}
                    {"☆".repeat(5 - review.sterren)}
                </p>
            </div>
        ))}
    </div>
</div>
        </div>
    );
}


export default Index;