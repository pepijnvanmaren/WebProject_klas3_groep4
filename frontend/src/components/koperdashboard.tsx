 import "../styles/index.css";
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
    huidigePrijs?: number;
};

type VeilingStatus = {
    isActief: boolean;
    isInPauze: boolean;
    remainingSeconds: number;
    huidigProduct: Product | null;
    volgendProduct: Product | null;
    aantalInWachtrij: number;
};

const VEILING_DUUR_SECONDS = 30;
const PAUZE_DUUR_SECONDS = 10;


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
    const [isRunning, setIsRunning] = useState(false);
    const [purchased, setPurchased] = useState(false);
    const [isPaused, setIsPaused] = useState(false);
    const [pauseCountdown, setPauseCountdown] = useState(10);
    const [error, setError] = useState<string | null>(null);

    const [aantal, setAantal] = useState<number>(1);
    const [currentTotalPrice, setCurrentTotalPrice] = useState<number>(0);

    const intervalRef = useRef<NodeJS.Timeout | null>(null);
    const lastProductIdRef = useRef<number | null>(null);
    const lastProductAmountRef = useRef<number | null>(null);
    const currentProductTitleRef = useRef<HTMLHeadingElement | null>(null);
    const [pauzeSeconds, setPauzeSeconds] = useState<number>(0);
    const pauzeIntervalRef = useRef<NodeJS.Timeout | null>(null);
    const [priceHistoryExpanded, setPriceHistoryExpanded] = useState(false);
    const [productGeschiedenis, setProductGeschiedenis] = useState([]);
    const [alleProductGeschiedenis, setAlleProductGeschiedenis] = useState([]);
    const [gemiddeldePrijsAlles, setGemiddeldePrijsAlles] = useState(0);
    const [gemiddeldePrijsHuidige, setGemiddeldePrijsHuidige] = useState(0);
    const prijsPerStuk = veilingStatus?.huidigProduct?.huidigePrijs ?? 0;
    const totalePrijs = prijsPerStuk * aantal;

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
                throw new Error("Er zijn geen gegevens van dit product");
            }

            const data = await response.json();
            setGemiddeldePrijsHuidige(data);
        } catch (error) {
            console.error(error);
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
                throw new Error("Er zijn geen gegevens van dit product");
            }

            const data = await response.json();
            setProductGeschiedenis(data);
        } catch (error) {
            console.error(error);
        }
    };

    //Haal de geschiedenis op wanneer het product verandert
    useEffect(() => {
        if (!veilingStatus?.huidigProduct) return;

        fetchData();
        fetchAlleData();
        fetchProductGeschiedenis();
        fetchAlleGeschiedenis();
    }, [veilingStatus?.huidigProduct?.id]);

    useEffect(() => {
        const userRole = localStorage.getItem("userRole");
        setIsLoggedIn(userRole === "Koper");
        fetchAlleData();
        fetchAlleGeschiedenis();
    }, []);

    // ======================
    // fetch veiling status
    // ======================
    const fetchVeilingStatus = async () => {
        try {
            const response = await fetch("https://localhost:7020/api/veiling-process/status");
            if (!response.ok) throw new Error("Kon veilingstatus niet ophalen");

            const data: VeilingStatus = await response.json();

            // ✅ Initialiseer huidigePrijs als het ontbreekt
            if (data.huidigProduct) {
                const min = data.huidigProduct.minimalePrijs;
                data.huidigProduct.huidigePrijs = min * 10; // startprijs
            }

            setVeilingStatus(prev => {
                if (prev?.huidigProduct?.id !== data.huidigProduct?.id) {
                    setAantal(1);
                    setPurchased(false);
                }
                return data;
            });
        } catch (err: any) {
            console.error(err);
        }
    };

    // ======================
    // Polling + prijs timer
    // ======================
    useEffect(() => {
        fetchVeilingStatus(); // direct ophalen

        const interval = setInterval(() => {
            setVeilingStatus(prev => {
                if (
                    !prev ||
                    !prev.huidigProduct ||
                    prev.remainingSeconds <= 0 ||
                    purchased ||
                    prev.isInPauze
                ) {
                    return prev;
                }


                const newRemaining = prev.remainingSeconds - 1;
                const min = prev.huidigProduct.minimalePrijs;
                const max = min * 10;

                const elapsed = VEILING_DUUR_SECONDS - newRemaining;
                const progress = Math.max(0, Math.min(1, elapsed / VEILING_DUUR_SECONDS));
                const actuelePrijs = !isNaN(min) && !isNaN(max)
                    ? Math.round((max - (max - min) * progress) * 100) / 100
                    : min;

                return {
                    ...prev,
                    remainingSeconds: newRemaining,
                    huidigProduct: {
                        ...prev.huidigProduct,
                        huidigePrijs: actuelePrijs
                    }
                };
            });
        }, 1000);

        return () => clearInterval(interval);
    }, []);

    //  =========================================
    //  volgende product wanneer timer voorbij is
    //  =========================================
    useEffect(() => {
        if (
            veilingStatus &&
            veilingStatus.remainingSeconds === 0 &&
            !purchased &&
            veilingStatus.isActief
        ) {
            // Tijd voorbij zonder koop, start pauze
            setPauzeSeconds(PAUZE_DUUR_SECONDS);

            if (pauzeIntervalRef.current) {
                clearInterval(pauzeIntervalRef.current);
            }

            pauzeIntervalRef.current = setInterval(() => {
                setPauzeSeconds(prev => prev - 1);
            }, 1000);
        }
    }, [veilingStatus?.remainingSeconds]);


    // ======================
    // Koop product
    // ======================
    const handleBuy = async () => {
        if (!isLoggedIn) return navigate("/inloggen");
        if (!veilingStatus?.huidigProduct) return;

        setLoading(true);
        try {
            const token = localStorage.getItem("token");
            const product = veilingStatus.huidigProduct;

            const response = await fetch("https://localhost:7020/api/veiling-process/koop", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    "Authorization": `Bearer ${token}`
                },
                body: JSON.stringify({
                    ProductId: product.id,
                    Prijs: product.huidigProduct?.huidigePrijs,
                    Aantal: aantal
                })
            });

            if (!response.ok) {
                const msg = await response.text();
                throw new Error(msg || "Kon product niet kopen");
            }
            setPurchased(true);

            // Start pauze van 10 seconden
            setPauzeSeconds(PAUZE_DUUR_SECONDS);

            // Start aftellen
            if (pauzeIntervalRef.current) {
                clearInterval(pauzeIntervalRef.current);
            }

            pauzeIntervalRef.current = setInterval(() => {
                setPauzeSeconds(prev => prev - 1);
            }, 1000);

        } catch (err: any) {
            alert(err.message);
        } finally {
            setLoading(false);
        }
    };

    const startVolgendProduct = async () => {
        try {
            const token = localStorage.getItem("token");

            await fetch("https://localhost:7020/api/veiling-process/volgend-product", {
                method: "POST",
                headers: {
                    "Authorization": `Bearer ${token}`
                }
            });

            // Reset states
            setPurchased(false);
            setPauzeSeconds(0);

            // Haal nieuwe status op
            fetchVeilingStatus();
        } catch (err) {
            console.error("Kon volgend product niet starten", err);
        }
    };


    // ======================
    // Controle login
    // ======================
    useEffect(() => {
        const userRole = localStorage.getItem("userRole");
        setIsLoggedIn(userRole === "Koper");
    }, []);


    // ======================
    // start volgende product
    // ======================
    useEffect(() => {
        if (pauzeSeconds === 0 && (purchased || veilingStatus?.remainingSeconds === 0)) {
            startVolgendProduct();
        }
    }, [pauzeSeconds]);



    // ======================
    // Render
    // ======================
    return (
        <div className="page">

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

            
            {!veilingStatus?.isActief && (
                <div style={{ textAlign: "center", padding: "50px" }}>
                    <h2>Er is momenteel geen actieve veiling</h2>
                    <p>Kom later terug of wacht tot de veiling start!</p>
                </div>
            )}

            {veilingStatus?.isActief && veilingStatus.huidigProduct && (
                <>
                    <h2 className="page-title">
                        {purchased
                            ? "Product verkocht!"
                            : veilingStatus.remainingSeconds === 0
                                ? "Tijd voorbij! "
                                : "Huidig product"}
                    </h2>


                    {purchased && pauzeSeconds > 0 && (
                        <div
                            style={{
                                textAlign: "center",
                                fontSize: "2rem",
                                fontWeight: "bold",
                                color: "#047B00",
                                marginBottom: "20px"
                            }}
                        >
                            Volgend product over {pauzeSeconds} seconden
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
                        <div className="box">{veilingStatus.huidigProduct.oogstdatum} geoogst</div>
                        <div className="box">{veilingStatus.huidigProduct.potmaat} cm potmaat</div>
                        <div className="box">{veilingStatus.huidigProduct.gewicht} kg gewicht</div>
                        <div className="box">{veilingStatus.huidigProduct.steellengte} cm steellengte</div>
                        <div className="box box-price">
                            <div className="price-row">
                                <span className="price">
                                    EUR {totalePrijs.toFixed(2)}
                                    <div style={{ fontSize: "0.8rem", color: "#555" }}>
                                        (€{prijsPerStuk.toFixed(2)} per stuk)
                                    </div>
                                </span>

                                <button
                                    className="button"
                                    onClick={handleBuy}
                                    disabled={
                                        loading ||
                                        purchased ||
                                        veilingStatus.isInPauze ||
                                        !veilingStatus.isActief
                                    }
                                >
                                    {purchased
                                        ? "Gekocht"
                                        : veilingStatus.isInPauze
                                            ? "Wacht..."
                                            : isLoggedIn
                                                ? "Koop"
                                                : "Inloggen"}
                                </button>
                            </div>
                            <input
                                type="number"
                                className="input-field"
                                value={aantal}
                                min={1}
                                max={veilingStatus.huidigProduct.hoeveelheid}
                                onChange={e => {
                                    let value = Number(e.target.value);
                                    if (!veilingStatus?.huidigProduct) return;
                                    if (value < 1) value = 1;
                                    if (value > veilingStatus.huidigProduct.hoeveelheid)
                                        value = veilingStatus.huidigProduct.hoeveelheid;
                                    setAantal(value);
                                }}
                            />
                        </div>

                        {!veilingStatus.isInPauze && (
                            <div className="progress-bar-container integrated-bar">
                                <div
                                    className="progress-bar"
                                    style={{
                                        width: `${Math.max(0, Math.min(1, veilingStatus.remainingSeconds / VEILING_DUUR_SECONDS)) *
                                            100
                                            }%`,
                                        backgroundColor: `rgb(${Math.round(
                                            255 * (1 - veilingStatus.remainingSeconds / VEILING_DUUR_SECONDS)
                                        )}, ${Math.round(
                                            255 * (veilingStatus.remainingSeconds / VEILING_DUUR_SECONDS)
                                        )}, 0)`,
                                        transition: "width 1s linear, background-color 1s linear"
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
    );
}

export default Index;
