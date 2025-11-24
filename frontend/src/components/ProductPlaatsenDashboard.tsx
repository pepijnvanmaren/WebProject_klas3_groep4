import "../styles/ProductPlaatsenDashboard.css";
import { useNavigate } from 'react-router-dom';
import { useState } from "react";

function SellerDashboard() {
    const navigate = useNavigate();

    //States voor elk veld
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
   

    // compute local today string (yyyy-mm-dd) to avoid UTC offset issues
    const localToday = (() => {
        const d = new Date();
        const tzOffset = d.getTimezoneOffset();
        return new Date(d.getTime() - tzOffset * 60000).toISOString().split("T")[0];
    })();

    //Handle aanmaak knop
    const handleCreateProduct = async () => {
        // final validation before submit
        if (oogstdatum && oogstdatum > localToday) {
            setOogstError("Oogstdatum mag niet in de toekomst liggen.");
            return;
        }

        const form = new FormData();
        form.append("naam", naam);
        form.append("beschrijving", beschrijving);
        if (fotoFile) form.append("foto", fotoFile);
        form.append("oogstdatum", oogstdatum);
        form.append("potmaat", potmaat);
        form.append("gewicht", gewicht);
        form.append("steellengte", steellengte);
        form.append("hoeveelheid", hoeveelheid);
        form.append("minimalePrijs", minimalePrijs);

        try {
            const resp = await fetch("https://localhost:7020/api/Product", {
                method: "POST",
                body: form
                // DO NOT set Content-Type header — browser will set multipart boundary
            });

            if (!resp.ok) {
                const text = await resp.text();
                console.error("Upload failed:", text);
                alert("Upload failed: " + resp.statusText);
                return;
            }

            const data = await resp.json();
            console.log("Server response:", data);
            alert("Uw product is gemaakt.");
        } catch (err) {
            console.error(err);
            alert("Er is iets misgegaan met het versturen.");
        }
    };

    const handleGoBack = () => navigate('/verkoperDashboard');

    //Handle voor Oogstdatum
    const handleOogstdatumChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const val = e.target.value;
        if (!val) {
            setOogstdatum("");
            setOogstError("");
            return;
        }

        //Compare datums (Vandaag/Invoer)
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
                        <h2>Naam</h2>
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
                        <h2>Minimale prijs (€)</h2>
                        <input
                            className="pp_input-container"
                            type="number"
                            placeholder="Voer de minimale prijs in"
                            value={minimalePrijs}
                            onChange={e => setMinimalePrijs(e.target.value)}
                        />
                    </label>
                </div>
            </div>

            {/*Buttons*/}
            <div className="pp_buttons-row">
                <button className="pp_btn" onClick={handleCreateProduct}>Product Maken</button>
                <button className="pp_btn" onClick={handleGoBack}>Terug</button>
            </div>
        </main>
    );
}

export default SellerDashboard;
