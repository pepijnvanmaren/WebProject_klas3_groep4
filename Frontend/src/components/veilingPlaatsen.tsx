import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import '../styles/VeilingPlaatsen.css';
import { useEffect } from 'react';



function VeilingPlaatsen() {
    const navigate = useNavigate();

    useEffect(() => {
        window.scrollTo(0, 0);
    }, [])


    //States voor elk veld
    const [starTijd, setStarTijd] = useState("");
    const [beschrijving, setBeschrijving] = useState("");
    const [startDatum, setStartDatum] = useState("");
    const [klokLocatie, setKlokLocatie] = useState("");
    const [StartDatumError, setStartDatumError] = useState("");


    const handleCreateVeiling = async () => {
        if (startDatum && startDatum < localToday) {
            setStartDatumError("StartDatum mag niet in het verleden liggen.");
            return;
        }

        const payload = {
            starTijd,
            bechrijving: beschrijving,
            startDatum,
            klokLocatie,
            aantalProducten: 1,
            huidigeSituatieVanVeiling: "gesloten",
            veilingmeesterId: 1 
        };

        try {
            const resp = await fetch("https://localhost:7020/api/Veiling", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                credentials: "include",
                body: JSON.stringify(payload)
            });

            if (!resp.ok) {
                const text = await resp.text();
                console.error("Upload failed:", text);
                alert("Upload failed: " + resp.statusText);
                return;
            }

            const data = await resp.json();
            alert("Uw veiling is gemaakt.");

            // Reset
            setStarTijd("");
            setBeschrijving("");
            setStartDatum("");
            setKlokLocatie("");

        } catch (err) {
            console.error(err);
            alert("Er is iets misgegaan met het versturen.");
        }
    };


    useEffect(() => {
        window.scrollTo(0, 0);
    }, [])

    const handleGoBack = () => navigate('/VeilingMeesterDashboard');

    //Maakt een locaal atribuut aan voor de datum van vandaag
    const localToday = (() => {
        const d = new Date();
        const tzOffset = d.getTimezoneOffset();
        return new Date(d.getTime() - tzOffset * 60000).toISOString().split("T")[0];
    })();

    //HTML REACT
    return (
        <main className="vp_dashboard-container">
            <h1 className="pp_title">Veiling aanmaken</h1>

            <div className="vp_dashboard-box vp_dashboard-flex">

                {/*Linker colom*/}
                <div className="pp_dashboard-column">
                    <label>
                        <h2>Beschrijving</h2>
                        <textarea
                            className="pp_input-container-beschrijving"
                            placeholder="Voer je Beschrijving in"
                            onChange={e => setBeschrijving(e.target.value)}
                        />
                    </label>
                </div>
                {/*Rechter colom*/}
                <div className="pp_dashboard-column">
                    <label>
                        <h2>KlokLocatie</h2>
                        <select
                            name="Locatie selector"
                            className="vp_selector"
                            value={klokLocatie}
                            onChange={e => setKlokLocatie(e.target.value)}
                        >
                            <option value="" disabled>
                                Select uw optie
                            </option>
                            <option value="Den Haag">Den Haag</option>
                            <option value="Rotterdam">Rotterdam</option>
                            <option value="Amsterdam">Amsterdam</option>
                            <option value="Leiden">Leiden</option>
                        </select>
                    </label>

                    <label>
                        <h2>StartDatum</h2>
                        <input
                            className="pp_input-container"
                            type="date"
                            min={localToday}
                            onChange={e => setStartDatum(e.target.value)}
                        />
                        {StartDatumError && <div style={{ color: 'red', marginTop: 6 }}>{StartDatumError}</div>}
                    </label>

                    <label>
                        <h2>StarTijd</h2>
                        <select name="Tijd selector" className="vp_selector"
                            value={starTijd}
                            onChange={e => setStarTijd(e.target.value)}
                        >
                            <option value="" disabled>Select uw optie</option>
                            <option value="07:00">07:00</option>
                            <option value="08:00">08:00</option>
                            <option value="09:00">09:00</option>
                            <option value="10:00">10:00</option>
                            <option value="11:00">11:00</option>
                            <option value="12:00">12:00</option>
                        </select>
                    </label>
                </div>
            </div>

            {/*Buttons*/}
            <div className="pp_buttons-row">
                <button className="pp_btn" onClick={handleCreateVeiling} >Veiling Maken</button>
                <button className="pp_btn" onClick={handleGoBack}>Terug</button>
            </div>
        </main>
    )
}


export default VeilingPlaatsen;
