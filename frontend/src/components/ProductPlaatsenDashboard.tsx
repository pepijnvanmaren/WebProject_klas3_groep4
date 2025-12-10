import "../styles/ProductPlaatsenDashboard.css";
import { useNavigate } from 'react-router-dom';
import { useState, useEffect } from "react";

function SellerDashboard() {
    const navigate = useNavigate();
    const [naam, setNaam] = useState("");
    const [beschrijving, setBeschrijving] = useState("");
    const [fotoFile, setFotoFile] = useState<File | null>(null);
    const [oogstdatum, setOogstdatum] = useState("");
    const [potmaat, setPotmaat] = useState("");
    const [gewicht, setGewicht] = useState("");
    const [steellengte, setSteellengte] = useState("");
    const [hoeveelheid, setHoeveelheid] = useState("");
    const [minimalePrijs, setMinimalePrijs] = useState("");
    const [oogstError, setOogstError] = useState("");
    const [veilingId, setVeilingId] = useState<number | null>(null);
    const [veilingen, setVeilingen] = useState<any[]>([]);
    const [loading, setLoading] = useState(false);

    const localToday = (() => {
        const d = new Date();
        const tzOffset = d.getTimezoneOffset();
        return new Date(d.getTime() - tzOffset * 60000).toISOString().split("T")[0];
    })();

    useEffect(() => {
        const fetchVeilingen = async () => {
            try {
                const response = await fetch("https://localhost:7020/api/veiling", {
                    credentials: "include"
                });
                if (response.ok) {
                    const data = await response.json();
                    setVeilingen(data);
                }
            } catch (err) {
                console.error("Error fetching veilingen:", err);
            }
        };

        fetchVeilingen();
    }, []);

    const handleCreateProduct = async () => {
        if (!naam) {
            alert("Productnaam is verplicht");
            return;
        }

        if (oogstdatum && oogstdatum > localToday) {
            setOogstError("Oogstdatum mag niet in de toekomst liggen.");
            return;
        }

        setLoading(true);

        try {
            let fotoBase64 = null;
            if (fotoFile) {
                fotoBase64 = await new Promise((resolve) => {
                    const reader = new FileReader();
                    reader.onload = (e) => {
                        const result = e.target?.result as string;
                        const base64 = result.split(',')[1];
                        resolve(base64);
                    };
                    reader.readAsDataURL(fotoFile);
                });
            }

            const payload = {
                naam,
                beschrijving,
                foto: fotoBase64,
                oogstdatum: oogstdatum || null,
                potmaat: potmaat ? Number(potmaat) : null,
                gewicht: gewicht ? Number(gewicht) : 0,
                steellengte: steellengte ? Number(steellengte) : null,
                hoeveelheid: hoeveelheid ? Number(hoeveelheid) : 0,
                minimalePrijs: minimalePrijs ? Number(minimalePrijs) : 0,
                veilingId: veilingId || null
            };

            const token = localStorage.getItem("token");
            const resp = await fetch("https://localhost:7020/api/Product", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    "Authorization": `Bearer ${token}`
                },
                body: JSON.stringify(payload)
            });

            if (!resp.ok) {
                const text = await resp.text();
                console.error("Upload failed:", text);
                alert("Upload mislukt: " + resp.statusText);
                setLoading(false);
                return;
            }

            const data = await resp.json();
            console.log("Product aangemaakt:", data);
            alert("Uw product is succesvol aangemaakt!");

            setNaam("");
            setBeschrijving("");
            setFotoFile(null);
            setOogstdatum("");
            setPotmaat("");
            setGewicht("");
            setSteellengte("");
            setHoeveelheid("");
            setMinimalePrijs("");
            setVeilingId(null);
            setOogstError("");

            navigate("/VerkoperDashboard");

        } catch (err) {
            console.error(err);
            alert("Er is iets misgegaan met het versturen.");
        } finally {
            setLoading(false);
        }
    };

    // Handelt de Terug knop
    const handleGoBack = () => navigate('/VerkoperDashboard');

    // Handle voor Oogstdatum
    const handleOogstdatumChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const val = e.target.value;
        if (!val) {
            setOogstdatum("");
            setOogstError("");
            return;
        }

        if (val > localToday) {
            setOogstdatum(localToday);
            setOogstError("Oogstdatum kon niet in de toekomst liggen; is ingesteld op vandaag.");
        } else {
            setOogstdatum(val);
            setOogstError("");
        }
    };

    return (
        <main className="pp_dashboard-container">
            <h1 className="pp_title">Product aanmaken</h1>

            <div className="pp_dashboard-box pp_dashboard-flex">

                {/*Linker colom*/}
                <div className="pp_dashboard-column">
                    <label>
                        <h2>Naam *</h2>
                        <input
                            className="pp_input-container"
                            placeholder="Voer je Naam in"
                            value={naam}
                            onChange={e => setNaam(e.target.value)}
                        />
                    </label>

                    <label>
                        <h2>Beschrijving</h2>
                        <textarea
                            className="pp_input-container-beschrijving"
                            placeholder="Voer je Beschrijving in"
                            value={beschrijving}
                            onChange={e => setBeschrijving(e.target.value)}
                        />
                    </label>

                    <label>
                        <h2>Foto</h2>
                        <input
                            className="pp_input-container"
                            type="file"
                            accept="image/*"
                            onChange={e => setFotoFile(e.target.files && e.target.files[0] ? e.target.files[0] : null)}
                        />
                    </label>

                    <label>
                        <h2>Oogstdatum</h2>
                        <input
                            className="pp_input-container"
                            type="date"
                            value={oogstdatum}
                            max={localToday}
                            onChange={handleOogstdatumChange}
                        />
                        {oogstError && <div style={{ color: 'red', marginTop: 6 }}>{oogstError}</div>}
                    </label>
                </div>

                {/*Rechter colom*/}
                <div className="pp_dashboard-column">
                    <label>
                        <h2>Potmaat (cm)</h2>
                        <input
                            className="pp_input-container"
                            type="number"
                            placeholder="Voer de potmaat in"
                            value={potmaat}
                            onChange={e => setPotmaat(e.target.value)}
                        />
                    </label>

                    <label>
                        <h2>Gewicht (kg)</h2>
                        <input
                            className="pp_input-container"
                            type="number"
                            placeholder="Voer het gewicht in"
                            value={gewicht}
                            onChange={e => setGewicht(e.target.value)}
                        />
                    </label>

                    <label>
                        <h2>Steellengte (cm)</h2>
                        <input
                            className="pp_input-container"
                            type="number"
                            placeholder="Voer de steellengte in"
                            value={steellengte}
                            onChange={e => setSteellengte(e.target.value)}
                        />
                    </label>

                    <label>
                        <h2>Hoeveelheid</h2>
                        <input
                            className="pp_input-container"
                            type="number"
                            placeholder="Voer de hoeveelheid in"
                            value={hoeveelheid}
                            onChange={e => setHoeveelheid(e.target.value)}
                        />
                    </label>

                    <label>
                        <h2>Minimale prijs (€) *</h2>
                        <input
                            className="pp_input-container"
                            type="number"
                            placeholder="Voer de minimale prijs in"
                            value={minimalePrijs}
                            onChange={e => setMinimalePrijs(e.target.value)}
                        />
                    </label>

                    <label>
                        <h2>Veiling (optioneel)</h2>
                        <select
                            className="pp_input-container"
                            value={veilingId || ""}
                            onChange={e => setVeilingId(e.target.value ? Number(e.target.value) : null)}
                        >
                            <option value="">-- Geen veiling --</option>
                            {veilingen.map((veiling) => (
                                <option key={veiling.id} value={veiling.id}>
                                    {veiling.starTijd} - {veiling.klokLocatie}
                                </option>
                            ))}
                        </select>
                    </label>
                </div>
            </div>

            {/*Buttons*/}
            <div className="pp_buttons-row">
                <button
                    className="pp_btn"
                    onClick={handleCreateProduct}
                    disabled={loading}
                >
                    {loading ? "Bezig met uploaden..." : "Product Maken"}
                </button>
                <button
                    className="pp_btn"
                    onClick={handleGoBack}
                    disabled={loading}
                >
                    Terug
                </button>
            </div>
        </main>
    );
}

export default SellerDashboard;