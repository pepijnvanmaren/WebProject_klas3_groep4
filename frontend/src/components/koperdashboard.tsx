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
    const [purchased, setPurchased] = useState(false);
    const [aantal, setAantal] = useState<number>(1);
    const currentProductTitleRef = useRef<HTMLHeadingElement | null>(null);
    const [pauzeSeconds, setPauzeSeconds] = useState<number>(0);
    const pauzeIntervalRef = useRef<NodeJS.Timeout | null>(null);
    const prijsPerStuk = veilingStatus?.huidigProduct?.huidigePrijs ?? 0;
    const totalePrijs = prijsPerStuk * aantal;



    // ======================
    // Fetch veiling status
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
