import "../styles/koperdashboard.css";
import React, { useEffect, useState, useRef } from "react";
import { useNavigate } from "react-router-dom";

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

const getImageSrc = (foto?: string | null) => {
    if (!foto) return "";
    const s = foto.trim().replace(/^"|"$/g, "").replace(/\r?\n/g, "");
    if (s.startsWith("data:")) return s;
    if (s.startsWith("/9j/")) return `data:image/jpeg;base64,${s}`;
    if (s.startsWith("iVBOR")) return `data:image/png;base64,${s}`;
    return `data:image/*;base64,${s}`;
};

const ProductImage = ({ product }: { product: Product }) =>
    product?.foto ? (
        <img src={getImageSrc(product.foto)} alt={product.naam} className="Roses" />
    ) : (
        <div className="no-image">No image available</div>
    );

function Index() {
    const navigate = useNavigate();

    const [isLoggedIn, setIsLoggedIn] = useState(false);
    const [veilingStatus, setVeilingStatus] = useState<VeilingStatus | null>(null);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const [priceHistoryExpanded, setPriceHistoryExpanded] = useState(false);

    const intervalRef = useRef<NodeJS.Timeout | null>(null);
    const [price, setPrice] = useState(0);
    const [isRunning, setIsRunning] = useState(false);
    const [purchased, setPurchased] = useState(false);
    const [isPaused, setIsPaused] = useState(false);
    const [pauseCountdown, setPauseCountdown] = useState(30);
    const lastProductIdRef = useRef<number | null>(null);

 
    const currentProductTitleRef = useRef<HTMLHeadingElement | null>(null);
    const [aantal, setAantal] = useState<number>(1);
    const [productGeschiedenis, setProductGeschiedenis] = useState([]);
    const [alleProductGeschiedenis, setAlleProductGeschiedenis] = useState([]);
    const [gemiddeldePrijsAlles, setGemiddeldePrijsAlles] = useState(0);
    const [gemiddeldePrijsHuidige, setGemiddeldePrijsHuidige] = useState(0);

    const [currentTotalPrice, setCurrentTotalPrice] = useState<number>(0);
    const lastProductAmountRef = useRef<number | null>(null);


    //Gemdeddilde prijs van alle producten bij elkaar 
    const fetchAlleData = async () => {
        try {
            const response = await fetch("https://localhost:7020/api/VerkochteProducten/GetallGemiddeldeAlles"
            );

            if (!response.ok) {
                throw new Error("Kon gegevens niet ophalen");
            }

            const data = await response.json();
            setGemiddeldePrijsAlles(data);
        } catch (error) {
            console.error(error);
            alert("Er is iets verkeerd gegaan");
        }
    };
    //Gemdeddilde prijs van huidige product bij elkaar
    const fetchData = async () => {
        if (!veilingStatus?.huidigProduct) {
            throw new Error("Kon geen product vinden");
        }

        try {
            const response = await fetch(
                `https://localhost:7020/api/VerkochteProducten/GetallGemiddeldeProduct/${veilingStatus.huidigProduct.id}`
            );

            if (!response.ok) {
                throw new Error("Kon gegevens niet ophalen");
            }

            const data = await response.json();
            setGemiddeldePrijsHuidige(data);
        } catch (error) {
            console.error(error);
            alert("Er is iets verkeerd gegaan");
        }
    };

    //Geschiedenis van alle producten
    const fetchAlleGeschiedenis = async () => {
        try {
            const response = await fetch("https://localhost:7020/api/VerkochteProducten/GetallAllProducten"
            );

            if (!response.ok) {
                throw new Error("Kon gegevens niet ophalen");
            }

            const data = await response.json();
            setAlleProductGeschiedenis(data);
        } catch (error) {
            console.error(error);
            alert("Er is iets verkeerd gegaan");
        }
    };

    //Gemdeddilde prijs van huidige product bij elkaar
    const fetchProductGeschiedenis = async () => {
        if (!veilingStatus?.huidigProduct) {
            throw new Error("Kon geen product vinden");
        }

        try {
            const response = await fetch(
                `https://localhost:7020/api/VerkochteProducten/GetallProducten/${veilingStatus.huidigProduct.id}`
            );

            if (!response.ok) {
                throw new Error("Kon gegevens niet ophalen");
            }

            const data = await response.json();
            setProductGeschiedenis(data);
        } catch (error) {
            console.error(error);
            alert("Er is iets verkeerd gegaan");
        }
    };

    //Haal de geschiedenis op wanneer het product verandert
    useEffect(() => {
        if (!veilingStatus?.huidigProduct) return;

        fetchData();
        fetchProductGeschiedenis();
    }, [veilingStatus?.huidigProduct?.id]);

    //Controleer of de gebruiker is ingelogd
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
        fetchAlleData();
        fetchAlleGeschiedenis();
    }, []);


    useEffect(() => {
        const userRole = localStorage.getItem("userRole");
        setIsLoggedIn(userRole === "Koper");
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
            const newProductAmount = data.huidigProduct?.hoeveelheid ?? null;

            const isNewProduct =
                lastProductIdRef.current !== newProductId ||
                lastProductAmountRef.current !== newProductAmount;

            if (data.huidigProduct && data.isActief && !data.isInPauze) {
                if (isNewProduct) {
                    lastProductIdRef.current = newProductId;
                    lastProductAmountRef.current = newProductAmount;

                    const startTotalPrice = data.huidigProduct.minimalePrijs * data.huidigProduct.hoeveelheid;
                    setCurrentTotalPrice(startTotalPrice);
                    setAantal(1);
                    setPurchased(false);
                }

                setIsRunning(true);
                setIsPaused(false);
            }
        } catch (error: any) {
            console.error(error);
            setError(error?.message ?? "Unknown error");
        }
    };

    useEffect(() => {
        fetchVeilingStatus();
        const interval = setInterval(fetchVeilingStatus, 2000);
        return () => clearInterval(interval);
    }, []);

    useEffect(() => {
        if (!isRunning || !veilingStatus?.huidigProduct || isPaused) {
            if (intervalRef.current) {
                clearInterval(intervalRef.current);
                intervalRef.current = null;
            }
            return;
        }

        const minTotalPrice = veilingStatus.huidigProduct.minimalePrijs;
        const maxTotalPrice = minTotalPrice * 10;

        if (intervalRef.current) clearInterval(intervalRef.current);

        intervalRef.current = setInterval(() => {
            setCurrentTotalPrice(prev => {
                const current = prev > 0 ? prev : maxTotalPrice;
                const next = +(current * 0.98).toFixed(2);

                if (next <= minTotalPrice) {
                    clearInterval(intervalRef.current!);
                    return minTotalPrice;
                }
                return next;
            });
        }, 1000);

        return () => {
            if (intervalRef.current) {
                clearInterval(intervalRef.current);
                intervalRef.current = null;
            }
        };
    }, [isRunning, isPaused, veilingStatus?.huidigProduct]);

    useEffect(() => {
        if (!isPaused) return;

        const countdown = setInterval(() => {
            setPauseCountdown(prev => {
                if (prev <= 1) {
                    clearInterval(countdown);
                    handleVolgendProduct();
                    return 0;
                }
                return prev - 1;
            });
        }, 1000);

        return () => clearInterval(countdown);
    }, [isPaused]);

    const handleStartVeiling = async () => {
        setLoading(true);
        try {
            const token = localStorage.getItem("token");
            const response = await fetch("https://localhost:7020/api/veiling-process/start", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    "Authorization": `Bearer ${token}`
                }
            });
            if (!response.ok) throw new Error("Kon veiling niet starten");

            await fetchVeilingStatus();
        } catch (error: any) {
            alert(error.message);
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
            const product = veilingStatus.huidigProduct;
            const priceForInput = +(currentTotalPrice * aantal).toFixed(2);

            const response = await fetch("https://localhost:7020/api/veiling-process/koop", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    "Authorization": `Bearer ${token}`
                },
                body: JSON.stringify({
                    ProductId: product.id,
                    Prijs: totalPrice,
                    Aantal: aantal
                })
            });

            if (!response.ok) throw new Error(await response.text() || "Kon product niet kopen");

            await fetch("https://localhost:7020/api/VerkochteProducten", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    "Authorization": `Bearer ${token}`
                },
                body: JSON.stringify({
                    productId: product.id,
                    hoeveelHeid: aantal,
                    verkochtePrijs: priceForInput
                })
            });

            const remainingAmount = product.hoeveelheid - aantal;

            if (remainingAmount > 0) {
                await fetch("https://localhost:7020/api/veiling-process/herstart", {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json",
                        "Authorization": `Bearer ${token}`
                    },
                    body: JSON.stringify({
                        ProductId: product.id,
                        NieuweHoeveelheid: remainingAmount
                    })
                });

                await fetchVeilingStatus();
            } else {
                startPauseCountdown();
            }

            setPurchased(true);

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
            } else if (data.product) {
                const startTotalPrice = data.product.minimalePrijs * 10;
                setCurrentTotalPrice(startTotalPrice);
                setAantal(1);
                setPurchased(false);
                setIsPaused(false);
            }

            await fetchVeilingStatus();
        } catch (err: any) {
            alert(err.message);
            setIsRunning(false);
            setIsPaused(false);
        }
    };

    const startPauseCountdown = () => {
        setPauseCountdown(30);
        setIsPaused(true);
    };

    const minPricePerUnit = veilingStatus?.huidigProduct?.minimalePrijs ?? 0;
    const maxTotalPrice = minPricePerUnit * 10;

    const currentPrice = +(currentTotalPrice / veilingStatus?.huidigProduct?.hoeveelheid! * aantal).toFixed(2);

    const progress =
        maxTotalPrice > minPricePerUnit
            ? (currentTotalPrice - minPricePerUnit) / (maxTotalPrice - minPricePerUnit)
            : 0;

    const barColor = `rgb(${Math.round(255 * (1 - progress))}, ${Math.round(
        255 * progress
    )}, 0)`;

    const totalPrice = progress * aantal;

    return (
        <div className="buyer-dashboard-page">

            {/* Prijsgeschiedenis knop links - vaste positie */}
            {isLoggedIn && (
                <div style={{
                    position: 'fixed',
                    top: '20vh',
                    left: '20px',
                    zIndex: 1000,
                    display: 'flex',
                    flexDirection: 'column',
                    gap: '10px'
                }}>
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
                        <div style={{
                            padding: '15px',
                            backgroundColor: 'rgba(255, 255, 255, 0.95)',
                            borderRadius: '5px',
                            maxWidth: '350px',
                            maxHeight: '70vh',
                            overflowY: 'auto'
                        }}>
                            <div className="price-history-section">
                                <h4>Huidige aanvoerder</h4>
                                <p><strong>Gemiddelde prijs:</strong> €{gemiddeldePrijsHuidige.toLocaleString('nl-NL', { minimumFractionDigits: 2, maximumFractionDigits: 2 })}</p>

                                <table className="price-history-table" style={{
                                    width: '100%',
                                    borderCollapse: 'collapse',
                                    fontSize: '14px'
                                }}>
                                    <thead>
                                        <tr>
                                            <th style={{ borderBottom: '2px solid #ddd', padding: '8px', textAlign: 'left' }}>Datum</th>
                                            <th style={{ borderBottom: '2px solid #ddd', padding: '8px', textAlign: 'left' }}>Prijs</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        {productGeschiedenis.map(x => (
                                            <tr key={`${x.verkoopDatum}-${x.name}`}>
                                                <td style={{ borderBottom: '1px solid #eee', padding: '6px' }}>  {new Date(x.verkoopDatum).toLocaleDateString('nl-NL')}</td>
                                                <td style={{ borderBottom: '1px solid #eee', padding: '6px' }}>{x.result.toFixed(2)}</td>
                                            </tr>
                                        ))}
                                    </tbody>
                                </table>
                            </div>

                            <hr style={{ margin: '15px 0' }} />

                            <div className="price-history-section">
                                <h4>Alle aanvoerders</h4>
                                <p><strong>Gemiddelde prijs:</strong> €{gemiddeldePrijsAlles.toLocaleString('nl-NL', { minimumFractionDigits: 2, maximumFractionDigits: 2 })}
                                </p>

                                <table className="price-history-table" style={{
                                    width: '100%',
                                    borderCollapse: 'collapse',
                                    fontSize: '14px'
                                }}>
                                    <thead>
                                        <tr>
                                            <th style={{ borderBottom: '2px solid #ddd', padding: '8px', textAlign: 'left' }}>Datum</th>
                                            <th style={{ borderBottom: '2px solid #ddd', padding: '8px', textAlign: 'left' }}>Prijs</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        {alleProductGeschiedenis.map(x => (
                                            <tr key={`${x.verkoopDatum}-${x.name}`}>
                                                <td style={{ borderBottom: '1px solid #eee', padding: '6px' }}>{new Date(x.verkoopDatum).toLocaleDateString('nl-NL')}</td>
                                                <td style={{ borderBottom: '1px solid #eee', padding: '6px' }}>{x.result.toFixed(2)}</td>
                                            </tr>
                                        ))}
                                    </tbody>
                                </table>
                            </div>
                        </div>
                    )}
                </div>
            )}
            {/* Floating controls */}
            <div className="buyer-dashboard-controls">
                {!veilingStatus?.isActief ? (
                    <button
                        className="buyer-dashboard-btn buyer-dashboard-btn-start"
                        onClick={handleStartVeiling}
                        disabled={loading}
                    >
                        {loading ? "Bezig..." : "Start Veiling"}
                    </button>
                ) : (
                    <button
                        className="buyer-dashboard-btn buyer-dashboard-btn-stop"
                        onClick={handleStopVeiling}
                        disabled={loading}
                    >
                        {loading ? "Bezig..." : "Stop Veiling"}
                    </button>
                )}

                {veilingStatus?.isActief && (
                    <div className="buyer-dashboard-status">
                        <strong>Status:</strong> Actief<br />
                        <strong>In wachtrij:</strong> {veilingStatus.aantalInWachtrij}
                    </div>
                )}
            </div>

            {!veilingStatus?.isActief && (
                <div className="buyer-dashboard-no-auction">
                    <h2>Er is momenteel geen actieve veiling</h2>
                    <p>Kom later terug of wacht tot de veiling start!</p>
                </div>
            )}

            {veilingStatus?.isActief && veilingStatus.huidigProduct && (
                <>
                    <h2 className="buyer-dashboard-title" ref={currentProductTitleRef}>
                        {isPaused
                            ? "Product verkocht! Volgend product over..."
                            : "Huidig product"}
                    </h2>

                    {isPaused && (
                        <div className="buyer-dashboard-pause-timer">
                            {pauseCountdown} seconden
                        </div>
                    )}

                    <div className="buyer-dashboard-container">
                        <div className="buyer-dashboard-box">
                            <ProductImage product={veilingStatus.huidigProduct} />
                        </div>

                        <div className="buyer-dashboard-box buyer-dashboard-box-description">
                            <h2 className="buyer-dashboard-product-name">
                                {veilingStatus.huidigProduct.naam}
                            </h2>
                            <p className="buyer-dashboard-description">
                                {veilingStatus.huidigProduct.beschrijving}
                            </p>
                        </div>

                        <div className="buyer-dashboard-box">
                            {veilingStatus.huidigProduct.hoeveelheid} stuks
                        </div>

                        <div className="buyer-dashboard-box buyer-dashboard-box-price">
                            <div className="buyer-dashboard-price-row">
                                <span className="buyer-dashboard-price">
                                    EUR {totalPrice.toFixed(2)}
                                </span>
                                <button
                                    className="buyer-dashboard-buy-btn"
                                    onClick={handleBuy}
                                    disabled={purchased || isPaused || loading}
                                >
                                    {purchased
                                        ? "Gekocht"
                                        : isPaused
                                            ? "Wacht..."
                                            : isLoggedIn
                                                ? "Koop"
                                                : "Inloggen"}
                                </button>
                            </div>

                            <input
                                type="number"
                                className="buyer-dashboard-input"
                                placeholder="Voer het aantal in"
                                value={aantal}
                                min={1}
                                max={veilingStatus?.huidigProduct?.hoeveelheid ?? 1}
                                onChange={(e) => setAantal(Number(e.target.value))}
                            />
                        </div>

                        {!isPaused && (
                            <div className="buyer-dashboard-progress-container">
                                <div
                                    className="buyer-dashboard-progress-bar"
                                    style={{
                                        width: `${Math.max(0, Math.min(1, progress)) * 100}%`,
                                        backgroundColor: barColor,
                                    }}
                                />
                            </div>
                        )}
                    </div>

                    {veilingStatus.volgendProduct && (
                        <>
                            <h2 className="buyer-dashboard-title">Volgend product</h2>

                            <div className="buyer-dashboard-container">
                                <div className="buyer-dashboard-box">
                                    <ProductImage product={veilingStatus.volgendProduct} />
                                </div>

                                <div className="buyer-dashboard-box buyer-dashboard-box-description">
                                    <h2 className="buyer-dashboard-product-name">
                                        {veilingStatus.volgendProduct.naam}
                                    </h2>
                                    <p className="buyer-dashboard-description">
                                        {veilingStatus.volgendProduct.beschrijving}
                                    </p>
                                </div>

                                <div className="buyer-dashboard-box">
                                    {veilingStatus.volgendProduct.hoeveelheid} stuks
                                </div>

                                <div className="buyer-dashboard-box"></div>
                            </div>
                        </>
                    )}
                </>
            )}
        </div>
    );

}


export default Index;
