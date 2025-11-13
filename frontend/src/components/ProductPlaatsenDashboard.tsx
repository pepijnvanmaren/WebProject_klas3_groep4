import "../styles/ProductPlaatsenDashboard.css";
import React, { useState } from "react";
import { useNavigate } from "react-router-dom";

function SellerDashboard() {
    const navigate = useNavigate();

    // Input states
    const [naam, setNaam] = useState("");
    const [beschrijving, setBeschrijving] = useState("");
    const [foto, setFoto] = useState("");

    // Navigate back
    const handleTerug = () => {
        navigate("/verkoperDashboard");
    };

    // Submit handler
    const handleSubmit = async () => {
        if (!naam || !beschrijving || !foto) {
            alert("Vul alle velden in.");
            return;
        }

        try {
            const response = await fetch("https://localhost:7020/api/product", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify({
                    naam: naam,
                    foto: foto,
                    beschrijving: beschrijving,
                }),
            });

            if (!response.ok) {
                const text = await response.text();
                console.error("Server response:", text);
                throw new Error("Product aanvoeren mislukt.");
            }

            alert("Uw product is succesvol aangemaakt!");
            navigate("/verkoperDashboard");
        } catch (error) {
            console.error("Fout bij product aanvoeren:", error);
            alert("Er ging iets mis bij het aanmaken van het product.");
        }
    };

    return (
        <>
            <div className="dashboard-root">
                <header className="dashboard-header">
                    <div className="header-inner">
                        <img
                            src="/header-trees.jpg"
                            alt="header"
                            className="header-image"
                        />
                        <nav className="header-nav">
                            <a href="/registreren">Registreren</a>
                            <a href="/login">Inloggen</a>
                        </nav>
                    </div>
                </header>
            </div>

            <main className="dashboard-container">
                <h1 className="title">Product aanmaken</h1>
                <div className="dashboard-box">
                    <h2>Product naam</h2>
                    <input
                        className="input-container"
                        placeholder="Voer je Naam in"
                        value={naam}
                        onChange={(e) => setNaam(e.target.value)}
                    />

                    <h2>Product informatie</h2>
                    <textarea
                        className="input-container-beschrijving"
                        placeholder="Voer je Beschrijving in"
                        value={beschrijving}
                        onChange={(e) => setBeschrijving(e.target.value)}
                    />

                    <h2>Product foto (URL)</h2>
                    <input
                        className="input-container"
                        placeholder="Voer je Foto-URL in"
                        value={foto}
                        onChange={(e) => setFoto(e.target.value)}
                    />

                    <button className="login-button" onClick={handleSubmit}>
                        Product Maken
                    </button>
                    <button className="login-button" onClick={handleTerug}>
                        Terug
                    </button>
                </div>
            </main>
        </>
    );
}

export default SellerDashboard;
