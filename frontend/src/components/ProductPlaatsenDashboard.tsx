import "../styles/ProductPlaatsenDashboard.css";
import { useNavigate } from 'react-router-dom';
import { useState } from "react";

function SellerDashboard() {
    const navigate = useNavigate();

    // --- State for all input fields ---
    const [naam, setNaam] = useState("");
    const [beschrijving, setBeschrijving] = useState("");
    const [foto, setFoto] = useState("");
    const [oogstdatum, setOogstdatum] = useState("");
    const [potmaat, setPotmaat] = useState("");
    const [gewicht, setGewicht] = useState("");
    const [steellengte, setSteellengte] = useState("");
    const [hoeveelheid, setHoeveelheid] = useState("");
    const [minimalePrijs, setMinimalePrijs] = useState("");

    const handleCreateProduct = () => {
        const product = {
            naam,
            beschrijving,
            foto,
            oogstdatum,
            potmaat: Number(potmaat),
            gewicht: Number(gewicht),
            steellengte: Number(steellengte),
            hoeveelheid: Number(hoeveelheid),
            minimalePrijs: Number(minimalePrijs)
        };
        console.log(product);
        alert("Uw product is gemaakt.");
    };

    const handleGoBack = () => navigate('/verkoperDashboard');

    return (
        <main className="pp_dashboard-container">
            <h1 className="pp_title">Product aanmaken</h1>

            <div className="pp_dashboard-box pp_dashboard-flex">

                {/* Left column */}
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
                            placeholder="Voer je Foto in"
                            value={foto}
                            onChange={e => setFoto(e.target.value)}
                        />
                    </label>

                    <label>
                        <h2>Oogstdatum</h2>
                        <input
                            className="pp_input-container"
                            type="date"
                            value={oogstdatum}
                            onChange={e => setOogstdatum(e.target.value)}
                        />
                    </label>
                </div>

                {/* Right column */}
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

            {/* Buttons underneath */}
            <div className="pp_buttons-row">
                <button className="pp_btn" onClick={handleCreateProduct}>Product Maken</button>
                <button className="pp_btn" onClick={handleGoBack}>Terug</button>
            </div>
        </main>
    );
}

export default SellerDashboard;
