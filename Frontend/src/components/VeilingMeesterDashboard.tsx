import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import '../styles/VeilingPlaatsen.css';
import { useEffect } from 'react';

function VeilingPlaatsen() {
    const navigate = useNavigate();

    //States voor elk veld
    const [starTijd, setStarTijd] = useState("");
    const [beschrijving, setBeschrijving] = useState("");
    const [startDatum, setStartDatum] = useState("");
    const [klokLocatie, setKlokLocatie] = useState("");

    const handleCreateVeiling = async () => {

        const form = new FormData();
        form.append("starTijd", starTijd);
        form.append("beschrijving", beschrijving);
        form.append("startDatum", startDatum);
        form.append("klokLocatie", klokLocatie);

        try {
            const resp = await fetch("https://localhost:7020/api/Veiling", {
                method: "POST",
                body: form
            });

            //Als het niet ok is stuur error text
            if (!resp.ok) {
                const text = await resp.text();
                console.error("Upload failed:", text);
                alert("Upload failed: " + resp.statusText);
                return;
            }
            //Als wel ok is stuur dan een response text
            const data = await resp.json();
            console.log("Server response:", data);
            alert("Uw veiling is gemaakt.");

            //Catch Error
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
                    </label>

                    <label>
                        <h2>StarTijd</h2>
                        <select name="Tijd selector" className="vp_selector"
                            value={startDatum}
                            onChange={e => setStarTijd(e.target.value)}
                        >
                            <option value="" disabled selected>Select uw optie</option>
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