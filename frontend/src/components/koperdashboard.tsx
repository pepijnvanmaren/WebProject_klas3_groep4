import "../styles/index.css";
import React, { useEffect, useState, useRef } from "react";
import { useNavigate } from "react-router-dom";

// DTO Types
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

type VeilingStatus = {
    isActief: boolean;
    huidigProduct: Product | null;
    volgendProduct: Product | null;
    aantalInWachtrij: number;
    isInPauze?: boolean;
    pauzeRemainingSeconds?: number;
};

// Helper: Convert base64 string to image
const getImageSrc = (foto?: string | null) => {
    if (!foto) return "";
    const s = foto.trim().replace(/^"|"$/g, "").replace(/\r?\n/g, "");
    if (s.startsWith("data:")) return s;
    if (s.startsWith("/9j/")) return `data:image/jpeg;base64,${s}`;
    if (s.startsWith("iVBOR")) return `data:image/png;base64,${s}`;
    return `data:image/*;base64,${s}`;
};

// Product Image Component
const ProductImage = ({ product }: { product: Product }) =>
    product?.foto ? (
        <img src={getImageSrc(product.foto)} alt={product.naam} className="Roses" />
    ) : (
        <div className="no-image">No image available</div>
    );

function Index() {
    const navigate = useNavigate();

    // State
    const [isLoggedIn, setIsLoggedIn] = useState(false);
    const [veilingStatus, setVeilingStatus] = useState<VeilingStatus | null>(null);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const [isRunning, setIsRunning] = useState(false);
    const [purchased, setPurchased] = useState(false);
    const [isPaused, setIsPaused] = useState(false);
    const [pauseCountdown, setPauseCountdown] = useState(30);
    const [currentPricePerUnit, setCurrentPricePerUnit] = useState(0);
    const [hoeveelheid, setHoeveelheid] = useState(0);
    const [aantal, setAantal] = useState<number>(1);

    // Refs
    const intervalRef = useRef<NodeJS.Timeout | null>(null);
    const lastProductIdRef = useRef<number | null>(null);
    const currentProductTitleRef = useRef<HTMLHeadingElement | null>(null);

    // Check login status
    useEffect(() => {
        const userRole = localStorage.getItem("userRole");
        setIsLoggedIn(userRole === "Koper");
    }, []);

    // Fetch veiling status
    const fetchVeilingStatus = async () => {
        try {
            const response = await fetch("https://localhost:7020/api/veiling-process/status", {
                credentials: "include"
            });
            if (!response.ok) throw new Error("Kon veiling status niet ophalen");

            const data: VeilingStatus = await response.json();
            setVeilingStatus(data);

            const newProductId = data.huidigProduct?.id ?? null;
            const isNewProduct = lastProductIdRef.current !== newProductId;

            if (data.huidigProduct && data.isActief && !data.isInPauze) {
                if (isNewProduct) {
                    lastProductIdRef.current = newProductId;
                    setCurrentPricePerUnit((data.huidigProduct.minimalePrijs ?? 1) * 10);
                    setAantal(1);
                    setPurchased(false);
                }
                setIsRunning(true);
                setIsPaused(false);
            } else if (data.huidigProduct && data.isInPauze) {
                setIsRunning(false);
                setIsPaused(true);
                setPurchased(true);
                setPauseCountdown(data.pauzeRemainingSeconds ?? 30);
            } else {
                setIsRunning(false);
                setIsPaused(false);
            }
        } catch (err: any) {
            console.error(err);
            setError(err?.message ?? "Unknown error");
        }
    };

    // Poll veiling status every 2 seconds
    useEffect(() => {
        fetchVeilingStatus();
        const interval = setInterval(fetchVeilingStatus, 2000);
        return () => clearInterval(interval);
    }, []);

    // Veiling countdown
    useEffect(() => {
        if (!isRunning || !veilingStatus?.huidigProduct || isPaused) {
            intervalRef.current && clearInterval(intervalRef.current);
            intervalRef.current = null;
            return;
        }

        const minPrice = veilingStatus.huidigProduct.minimalePrijs;
        const maxPrice = minPrice * 10;

        intervalRef.current && clearInterval(intervalRef.current);

        intervalRef.current = setInterval(() => {
            setCurrentPricePerUnit(prev => {
                const startPrice = prev > 0 ? prev : maxPrice;
                const next = +(startPrice * 0.98).toFixed(2);
                if (next <= minPrice) {
                    clearInterval(intervalRef.current!);
                    return minPrice;
                }
                return next;
            });
        }, 1000);

        return () => intervalRef.current && clearInterval(intervalRef.current);
    }, [isRunning, veilingStatus?.huidigProduct, isPaused]);

    // Pauze countdown
    useEffect(() => {
        if (!isPaused) return;

        const countdown = setInterval(() => {
            setPauseCountdown(prev => {
                if (prev <= 1) {
                    clearInterval(countdown);
                    handleVolgendProduct();
                    return 30;
                }
                return prev - 1;
            });
        }, 1000);

        return () => clearInterval(countdown);
    }, [isPaused]);

    // Handlers
    const handleStartVeiling = async () => {
        setLoading(true);
        try {
            const response = await fetch("https://localhost:7020/api/veiling-process/start", {
                method: "POST",
                credentials: "include"
            });
            if (!response.ok) throw new Error("Kon veiling niet starten");

            await fetchVeilingStatus();
            alert("Veiling gestart!");
        } catch (err: any) {
            alert(err.message);
        } finally {
            setLoading(false);
        }
    };

    const handleStopVeiling = async () => {
        if (!window.confirm("Weet je zeker dat je de veiling wilt stoppen?")) return;

        setLoading(true);
        try {
            const token = localStorage.getItem("token");
            const response = await fetch("https://localhost:7020/api/veiling-process/stop", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    "Authorization": `Bearer ${token}`
                }
            });
            if (!response.ok) throw new Error("Kon veiling niet stoppen");

            setIsRunning(false);
            setIsPaused(false);
            await fetchVeilingStatus();
            alert("Veiling gestopt!");
        } catch (err: any) {
            alert(err.message);
        } finally {
            setLoading(false);
        }
    };

    const handleBuy = async () => {
        if (!isLoggedIn) return navigate("/inloggen");
        if (!veilingStatus?.huidigProduct) return;

        setLoading(true);
        try {
            const token = localStorage.getItem("token");
            const totalPrice = +(currentPricePerUnit * aantal).toFixed(2);

            // Koop via veiling-process
            const response = await fetch("https://localhost:7020/api/veiling-process/koop", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    "Authorization": `Bearer ${token}`
                },
                body: JSON.stringify({
                    ProductId: veilingStatus.huidigProduct.id,
                    Prijs: totalPrice,
                    Aantal: aantal
                })
            });

            if (!response.ok) throw new Error(await response.text() || "Kon product niet kopen");

            // Verkochte producten
            await fetch("https://localhost:7020/api/VerkochteProducten", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    "Authorization": `Bearer ${token}`
                },
                body: JSON.stringify({
                    productId: veilingStatus.huidigProduct.id,
                    hoeveelHeid: aantal,
                    verkochtePrijs: totalPrice
                })
            });

            setPurchased(true);
            setIsRunning(false);
            alert(`Product "${veilingStatus.huidigProduct.naam}" gekocht voor €${totalPrice.toFixed(2)}`);
        } catch (err: any) {
            alert(err.message);
        } finally {
            setLoading(false);
        }
    };

    const handleVolgendProduct = async () => {
        try {
            const token = localStorage.getItem("token");
            const response = await fetch("https://localhost:7020/api/veiling-process/volgende", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    "Authorization": `Bearer ${token}`
                }
            });

            if (!response.ok) throw new Error("Kon niet naar volgend product");

            const data = await response.json();
            if (data.veilingAfgelopen) {
                alert("Veiling afgelopen! Geen producten meer.");
                setIsRunning(false);
                setIsPaused(false);
                lastProductIdRef.current = null;
            } else {
                setIsPaused(false);
                setPurchased(false);
                setPauseCountdown(30);
                setAantal(1);
                await fetchVeilingStatus();
            }
        } catch (err: any) {
            alert(err.message);
            setIsRunning(false);
            setIsPaused(false);
        }
    };

    // Derived values
    const totalPrice = +(currentPricePerUnit * aantal).toFixed(2);
    const minPrice = veilingStatus?.huidigProduct?.minimalePrijs ?? 0;
    const maxPrice = veilingStatus?.huidigProduct ? minPrice * 10 : 0;
    const progress = maxPrice > minPrice ? (currentPricePerUnit - minPrice) / (maxPrice - minPrice) : 0;
    const barColor = `rgb(${Math.round(255 * (1 - progress))}, ${Math.round(255 * progress)}, 0)`;

    return (
        <div className="page">
            {/* Sidebar */}
            <div style={{
                position: 'fixed',
                top: '20vh',
                right: '20px',
                zIndex: 1000,
                display: 'flex',
                flexDirection: 'column',
                gap: '10px'
            }}>
                <button
                    onClick={veilingStatus?.isActief ? handleStopVeiling : handleStartVeiling}
                    disabled={loading}
                    style={{
                        padding: '10px 20px',
                        backgroundColor: veilingStatus?.isActief ? '#dc3545' : '#047B00',
                        color: 'white',
                        border: 'none',
                        borderRadius: '5px',
                        cursor: loading ? 'not-allowed' : 'pointer',
                        fontWeight: 'bold'
                    }}
                >
                    {loading ? "Bezig..." : veilingStatus?.isActief ? "Stop Veiling" : "Start Veiling"}
                </button>

                {veilingStatus?.isActief && (
                    <div style={{
                        padding: '10px',
                        backgroundColor: 'rgba(255, 255, 255, 0.9)',
                        borderRadius: '5px',
                        fontSize: '14px'
                    }}>
                        <strong>Status:</strong> Actief<br />
                        <strong>In wachtrij:</strong> {veilingStatus.aantalInWachtrij}
                    </div>
                )}
            </div>

            {!veilingStatus?.isActief && (
                <div style={{ textAlign: 'center', padding: '50px' }}>
                    <h2>Er is momenteel geen actieve veiling</h2>
                    <p>Kom later terug of wacht tot de veiling start!</p>
                </div>
            )}

            {veilingStatus?.isActief && veilingStatus.huidigProduct && (
                <>
                    <h2 className="page-title" ref={currentProductTitleRef}>
                        {isPaused ? "Product verkocht! Volgend product over..." : "Huidig product"}
                    </h2>

                    {isPaused && (
                        <div style={{
                            textAlign: 'center',
                            fontSize: '2rem',
                            fontWeight: 'bold',
                            color: '#047B00',
                            marginBottom: '20px'
                        }}>
                            {pauseCountdown} seconden
                        </div>
                    )}

                    <div className="container">
                        <div className="box"><ProductImage product={veilingStatus.huidigProduct} /></div>
                        <div className="box box-description">
                            <h2 className="product-name">{veilingStatus.huidigProduct.naam}</h2>
                            <p className="description">{veilingStatus.huidigProduct.beschrijving}</p>
                        </div>
                        <div className="box">{veilingStatus.huidigProduct.hoeveelheid} stuks</div>
                        <div className="box box-price">
                            <div className="price-row">
                                <span className="price">EUR {totalPrice.toFixed(2)}</span>
                                <button
                                    className="button"
                                    onClick={handleBuy}
                                    disabled={purchased || isPaused || loading}
                                >
                                    {purchased ? "Gekocht" :
                                        isPaused ? "Wacht..." :
                                            isLoggedIn ? "Koop" : "Inloggen"}
                                </button>
                            </div>
                            <input
                                type="number"
                                className="input-field"
                                value={aantal}
                                min={1}
                                max={veilingStatus.huidigProduct.hoeveelheid}
                                onChange={(e) => setAantal(Number(e.target.value))}
                            />
                        </div>

                        {!isPaused && (
                            <div className="progress-bar-container integrated-bar">
                                <div
                                    className="progress-bar"
                                    style={{
                                        width: `${Math.max(0, Math.min(1, progress)) * 100}%`,
                                        backgroundColor: barColor,
                                        transition: "width 1s linear, background-color 1s linear",
                                    }}
                                />
                            </div>
                        )}
                    </div>

                    {veilingStatus.volgendProduct && (
                        <>
                            <h2 className="page-title">Volgend product</h2>
                            <div className="container">
                                <div className="box"><ProductImage product={veilingStatus.volgendProduct} /></div>
                                <div className="box box-description">
                                    <h2 className="product-name">{veilingStatus.volgendProduct.naam}</h2>
                                    <p className="description">{veilingStatus.volgendProduct.beschrijving}</p>
                                </div>
                                <div className="box">{veilingStatus.volgendProduct.hoeveelheid} stuks</div>
                                <div className="box"></div>
                            </div>
                        </>
                    )}
                </>
            )}
        </div>
    );
}

export default Index;
