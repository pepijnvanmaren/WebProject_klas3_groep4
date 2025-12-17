import "../styles/index.css";
import React, { useEffect, useState, useRef } from "react";
import { useNavigate } from "react-router-dom";

// Refereerd naar de DTO
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
    const euroFormatter = new Intl.NumberFormat("nl-NL", {
        style: "currency",currency: "EUR",});
    const [isLoggedIn, setIsLoggedIn] = useState(false);
    const [veilingStatus, setVeilingStatus] = useState<VeilingStatus | null>(null);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const [priceHistoryExpanded, setPriceHistoryExpanded] = useState(false);

    const intervalRef = useRef<ReturnType<typeof setInterval> | null>(null);
    const [price, setPrice] = useState(0);
    const [isRunning, setIsRunning] = useState(false);
    const [purchased, setPurchased] = useState(false);
    const [isPaused, setIsPaused] = useState(false);
    const [pauseCountdown, setPauseCountdown] = useState(30);
    const lastProductIdRef = useRef<number | null>(null);
    const mockCurrentSupplierHistory = [
        { date: "2024-05-12", price: 12.95 },
        { date: "2024-05-11", price: 13.10 },
        { date: "2024-05-10", price: 12.80 },
        { date: "2024-05-09", price: 12.60 },
        { date: "2024-05-08", price: 12.75 },
        { date: "2024-05-07", price: 12.90 },
        { date: "2024-05-06", price: 12.85 },
        { date: "2024-05-05", price: 12.70 },
        { date: "2024-05-04", price: 12.65 },
        { date: "2024-05-03", price: 12.50 },
    ];

    const mockAllSuppliersHistory = [
        { date: "2024-05-12", price: 11.90 },
        { date: "2024-05-11", price: 12.10 },
        { date: "2024-05-10", price: 12.00 },
        { date: "2024-05-09", price: 11.85 },
        { date: "2024-05-08", price: 11.95 },
        { date: "2024-05-07", price: 11.80 },
        { date: "2024-05-06", price: 11.75 },
        { date: "2024-05-05", price: 11.70 },
        { date: "2024-05-04", price: 11.65 },
        { date: "2024-05-03", price: 11.60 },
    ];

    const currentProductTitleRef = useRef<HTMLHeadingElement | null>(null);

    const scrollToCurrentProduct = () => {
        if (!currentProductTitleRef.current) return;

        const headerOffset = window.innerHeight * 0.18;
        const elementPosition =
            currentProductTitleRef.current.getBoundingClientRect().top + window.scrollY;
        const offsetPosition = elementPosition - headerOffset;

        window.scrollTo({
            top: offsetPosition,
            behavior: "smooth",
        });
    };


    useEffect(() => {
        const checkLoginStatus = async () => {
            try {
                const userRole = localStorage.getItem("userRole");
                if (userRole == "Koper") {
                    setIsLoggedIn(true);
                }
            } catch {
                setIsLoggedIn(false);
            }
        };

        checkLoginStatus();
    }, []);

    const fetchVeilingStatus = async () => {
        try {
            const response = await fetch("https://localhost:7020/api/veiling-process/status", {
                credentials: "include"
            });
            if (!response.ok) throw new Error("Kon veiling status niet ophalen");

            const data: VeilingStatus = await response.json();
            setVeilingStatus(data);

            const newProductId = data.huidigProduct?.id ?? null;
            const wasDifferentProduct = lastProductIdRef.current !== newProductId;

            if (data.huidigProduct && data.isActief && !data.isInPauze) {
                if (wasDifferentProduct) {
                    lastProductIdRef.current = newProductId;
                    setPrice(data.huidigProduct.minimalePrijs * 10);
                    setPurchased(false);
                }
                setIsRunning(true);
                setIsPaused(false);
            } else if (data.huidigProduct && data.isInPauze) {
                setIsRunning(false);
                setIsPaused(true);
                setPurchased(true);
                const remaining = data.pauzeRemainingSeconds ?? 30;
                setPauseCountdown(remaining);
            } else {
                setIsRunning(false);
                setIsPaused(false);
            }
        } catch (err: any) {
            console.error(err);
            setError(err?.message ?? "Unknown error");
        }
    };

    useEffect(() => {
        fetchVeilingStatus();
        const pollInterval = setInterval(fetchVeilingStatus, 2000);

        return () => clearInterval(pollInterval);
    }, []);

    useEffect(() => {
        if (!isRunning || !veilingStatus?.huidigProduct || isPaused) {
            if (intervalRef.current) {
                clearInterval(intervalRef.current);
                intervalRef.current = null;
            }
            return;
        }

        const minPrice = veilingStatus.huidigProduct.minimalePrijs;

        if (intervalRef.current) clearInterval(intervalRef.current);

        intervalRef.current = setInterval(() => {
            setPrice(prev => {
                const current = prev > 0 ? prev : (minPrice * 10);

                const nextPrice = current * 0.98;
                if (nextPrice <= minPrice) {
                    clearInterval(intervalRef.current!);
                    intervalRef.current = null;
                    return minPrice;
                }
                return parseFloat(nextPrice.toFixed(2));
            });
        }, 1000);

        return () => {
            if (intervalRef.current) {
                clearInterval(intervalRef.current);
                intervalRef.current = null;
            }
        };
    }, [isRunning, veilingStatus?.huidigProduct, isPaused]);

    useEffect(() => {
        if (!isPaused) return;

        const countdownInterval = setInterval(() => {
            setPauseCountdown(prev => {
                if (prev <= 1) {
                    clearInterval(countdownInterval);
                    handleVolgendProduct();
                    return 30;
                }
                return prev - 1;
            });
        }, 1000);

        return () => clearInterval(countdownInterval);
    }, [isPaused]);

    const handleStartVeiling = async () => {
        setLoading(true);
        try {
            const response = await fetch("https://localhost:7020/api/veiling-process/start", {
                method: "POST",
                credentials: "include"
            });

            if (!response.ok) {
                const text = await response.text();
                throw new Error(text || "Kon veiling niet starten");
            }

            await fetchVeilingStatus();
            alert("Veiling gestart!");
        } catch (err: any) {
            alert(err.message);
        } finally {
            setLoading(false);
        }
    };

    const handleStopVeiling = async () => {
        const confirmStop = window.confirm("Weet je zeker dat je de veiling wilt stoppen?");
        if (!confirmStop) return;

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
        if (!isLoggedIn) {
            navigate("/inloggen");
            return;
        }

        if (!veilingStatus?.huidigProduct) return;

        setLoading(true);
        try {
            const token = localStorage.getItem("token");
            const response = await fetch("https://localhost:7020/api/veiling-process/koop", {
                method: "POST",
                
                headers: {
                    "Content-Type": "application/json",
                    "Authorization": `Bearer ${token}`
                },
                body: JSON.stringify({
                    ProductId: veilingStatus.huidigProduct.id,
                    Prijs: price
                })
            });

            if (!response.ok) {
                const text = await response.text();
                throw new Error(text || "Kon product niet kopen");
            }

            const data = await response.json();

            if (intervalRef.current) {
                clearInterval(intervalRef.current);
                intervalRef.current = null;
            }

            setIsRunning(false);
            setPurchased(true);

            alert(`Product "${veilingStatus.huidigProduct.naam}" gekocht voor EUR ${euroFormatter.format(2)}!`);

            const pauze = data.pauzeDurationSeconds ?? 30;
            setIsPaused(true);
            setPauseCountdown(pauze);

            await fetchVeilingStatus();

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
                await fetchVeilingStatus();
            }
        } catch (err: any) {
            alert(err.message);
            setIsRunning(false);
            setIsPaused(false);
        }
    };

    const minPrice = veilingStatus?.huidigProduct?.minimalePrijs ?? 0;
    const maxPrice = minPrice * 10;
    const progress = maxPrice > minPrice ? (price - minPrice) / (maxPrice - minPrice) : 0;
    const barColor = `rgb(${Math.round(255 * (1 - progress))}, ${Math.round(255 * progress)}, 0)`;

    const ProductImage = ({ product }: { product: Product }) =>
        product?.foto ? (
            <img src={getImageSrc(product.foto)} alt={product.naam} className="Roses" />
        ) : (
            <div className="no-image">No image available</div>
        );


    return (
        <>
         <div className="page">
                {veilingStatus?.isActief && veilingStatus.huidigProduct && (
                    <div className="container">
                        <div className="box">
                            <ProductImage product={veilingStatus.huidigProduct} />
                        </div>

                        <div className="box box-description">
                            <h2 className="product-name">{veilingStatus.huidigProduct.naam}</h2>
                            <p className="description">{veilingStatus.huidigProduct.beschrijving}</p>
                        </div>

                        <div className="box">{veilingStatus.huidigProduct.hoeveelheid} stuks</div>

                        <div className="box box-price">
                            <span className="price">EUR {euroFormatter.format(2)}</span>
                            <button
                                className="button"
                                onClick={handleBuy}
                                disabled={loading}
                            >
                                Koop
                            </button>
                        </div>
                    </div>
                )}
            </div>

            {/* Vast prijs-geschiedenis paneel */}
            {isLoggedIn && (
                <div>
                    <button
                        style={{
                            padding: '10px 20px',
                            backgroundColor: '#047B00',
                            color: 'white',
                            border: 'none',
                            borderRadius: '5px',
                            cursor: loading ? 'not-allowed' : 'pointer',
                            fontWeight: 'bold'
                        }}
                        onClick={() => setPriceHistoryExpanded(prev => !prev)}
                        disabled={loading}
                    >
                        Prijs Geschiedenis {priceHistoryExpanded ? "▲" : "▼"}
                    </button>

                    {priceHistoryExpanded && (
                        <>
                            <div className="price-history-section">
                                <h4>Huidige aanvoerder</h4>
                                <p><strong>Gemiddelde prijs:</strong> 12,78</p>

                                <table className="price-history-table">
                                    <thead>
                                        <tr>
                                            <th>Datum</th>
                                            <th>Prijs</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        {mockCurrentSupplierHistory.map((item, index) => (
                                            <tr key={index}>
                                                <td>{item.date}</td>
                                                <td>{item.price.toFixed(2)}</td>
                                            </tr>
                                        ))}
                                    </tbody>
                                </table>
                            </div>

                            <hr />

                            <div className="price-history-section">
                                <h4>Alle aanvoerders</h4>
                                <p><strong>Gemiddelde prijs:</strong> 11,85</p>

                                <table className="price-history-table">
                                    <thead>
                                        <tr>
                                            <th>Datum</th>
                                            <th>Prijs</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        {mockAllSuppliersHistory.map((item, index) => (
                                            <tr key={index}>
                                                <td>{item.date}</td>
                                                <td>{item.price.toFixed(2)}</td>
                                            </tr>
                                        ))}
                                    </tbody>
                                </table>
                            </div>
                        </>
                    )}
    </div>
)}
        <div className="page">
            <div style={{
                position: 'fixed',
                top: '20vh',
                right: '20px',
                zIndex: 1000,
                display: 'flex',
                flexDirection: 'column',
                gap: '10px'
            }}>
                {!veilingStatus?.isActief ? (
                    <button
                        onClick={handleStartVeiling}
                        disabled={loading}
                        style={{
                            padding: '10px 20px',
                            backgroundColor: '#047B00',
                            color: 'white',
                            border: 'none',
                            borderRadius: '5px',
                            cursor: loading ? 'not-allowed' : 'pointer',
                            fontWeight: 'bold'
                        }}
                    >
                        {loading ? "Bezig..." : "Start Veiling"}
                    </button>
                ) : (
                    <button
                        onClick={handleStopVeiling}
                        disabled={loading}
                        style={{
                            padding: '10px 20px',
                            backgroundColor: '#dc3545',
                            color: 'white',
                            border: 'none',
                            borderRadius: '5px',
                            cursor: loading ? 'not-allowed' : 'pointer',
                            fontWeight: 'bold'
                        }}
                    >
                        {loading ? "Bezig..." : "Stop Veiling"}
                    </button>
                )}

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
                        <div className="box">
                            <ProductImage product={veilingStatus.huidigProduct} />
                        </div>

                        <div className="box box-description">
                            <h2 className="product-name">{veilingStatus.huidigProduct.naam}</h2>
                            <p className="description">{veilingStatus.huidigProduct.beschrijving}</p>
                        </div>

                        <div className="box">{veilingStatus.huidigProduct.hoeveelheid} stuks</div>

                        <div className="box box-price">
                            <div className="price-row">
                                <span className="price">EUR {euroFormatter.format(2)}</span>
                                <button
                                    className="button"
                                    onClick={handleBuy}
                                    disabled={purchased || isPaused || loading}
                                >
                                    {purchased ? "Gekocht" :
                                        isPaused ? "Wacht..." :
                                            isLoggedIn ? "Koop" : "Inloggen"}
                                </button>
                                <button
                                    className="price-history-btn"
                                    onClick={() => setPriceHistoryExpanded(prev => !prev)}
                                    disabled={loading}
                                >
                                    Prijs geschiedenis
                                </button>
                            </div>
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
                                <div className="box">
                                    <ProductImage product={veilingStatus.volgendProduct} />
                                </div>
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
        </>
    );
}

export default Index;
