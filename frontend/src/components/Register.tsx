import React, { useState } from "react";
import { useNavigate, Link } from "react-router-dom";
import "../styles/Register.css";

function Register() {
    const navigate = useNavigate();

    // Form states
    const [naam, setNaam] = useState("");
    const [telefoonnummer, setTelefoonnummer] = useState("");
    const [email, setEmail] = useState("");
    const [paswoord, setPaswoord] = useState("");
    const [confirmPaswoord, setConfirmPaswoord] = useState("");
    const [rol, setRol] = useState("koper"); // "koper" of "aanvoerder"
    const [loading, setLoading] = useState(false);

    // Functie om registratie te versturen
    const postAccount = async () => {
        const url =
            rol === "koper"
                ? "https://localhost:7020/api/kopers"
                : rol === "aanvoerder"
                    ? "https://localhost:7020/api/aanvoerders"
                    : "https://localhost:7020/api/veilingmeesters"

        let body: any = {
            UserName: naam,
            Email: email,
            PhoneNumber: telefoonnummer,
            Password: paswoord,
        };

        if (rol === "koper") {
            body.BankGegevens = "";
            body.Postcode = "";
            body.Adres = "";
        } else if (rol === "aanvoerder") {
            body.NaamVanBedrijf = "";
            body.KvkNummer = "";
            body.Postcode = "";
            body.Adres = "";
            body.BedrijfTelefoonnummer = "";
            body.BedrijfEmail = "";
        } else if (rol === "veilingmeester") {
            body.VeilingVestiging = "";
        }

        const response = await fetch(url, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(body),
        });

        if (!response.ok) {
            const text = await response.text();
            throw new Error(text || "Registratie mislukt");
        }

        return response.json();
    };


    const handleRegister = async () => {
        if (!email || !paswoord || !confirmPaswoord || !naam || !telefoonnummer) {
            alert("Vul alle velden in!");
            return;
        }

        if (paswoord !== confirmPaswoord) {
            alert("Wachtwoorden komen niet overeen!");
            return;
        }

        setLoading(true);

        try {
            await postAccount();
            alert("Account succesvol aangemaakt!");

            // Navigatie op basis van rol
            if (rol === "koper") navigate("/koperdashboard");
            else if (rol === "aanvoerder") navigate("/verkoperDashboard");
            else if (rol === "veilingmeester") navigate("/veilingmeesterdashboard");
        } catch (error: any) {
            console.error(error);
            alert(error.message || "Er ging iets mis bij het registreren.");
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="register-page">
            <div className="register-container">
                <h1 className="register-title">Account aanmaken</h1>

                <div className="register-form">
                    <div className="register-column-left">
                        <div className="form-group">
                            <p>Naam</p>
                            <input
                                type="text"
                                placeholder="Voer je naam in"
                                className="input-field"
                                value={naam}
                                onChange={(e) => setNaam(e.target.value)}
                            />
                        </div>

                        <div className="form-group">
                            <p>Telefoonnummer</p>
                            <input
                                type="tel"
                                placeholder="Voer je telefoonnummer in"
                                className="input-field"
                                value={telefoonnummer}
                                onChange={(e) => setTelefoonnummer(e.target.value)}
                            />
                        </div>

                        <div className="form-group">
                            <p>Email</p>
                            <input
                                type="email"
                                placeholder="Voer je email in"
                                className="input-field"
                                value={email}
                                onChange={(e) => setEmail(e.target.value)}
                            />
                        </div>

                        <div className="form-group">
                            <p>Rol</p>
                            <div className="account-type-buttons">
                                <button
                                    type="button"
                                    className={`account-type-btn ${rol === "koper" ? "active" : ""}`}
                                    onClick={() => setRol("koper")}
                                >
                                    Koper
                                </button>
                                <button
                                    type="button"
                                    className={`account-type-btn ${rol === "aanvoerder" ? "active" : ""}`}
                                    onClick={() => setRol("aanvoerder")}
                                >
                                    Aanvoerder
                                </button>
                                <button
                                    type="button"
                                    className={`account-type-btn ${rol === "veilingmeester" ? "active" : ""}`}
                                    onClick={() => setRol("veilingmeester")}
                                >
                                    Veilingmeester
                                </button>
                            </div>
                        </div>
                    </div>

                    <div className="register-column-right">
                        <div className="form-group">
                            <p>Wachtwoord</p>
                            <input
                                type="password"
                                placeholder="Voer je wachtwoord in"
                                className="input-field"
                                value={paswoord}
                                onChange={(e) => setPaswoord(e.target.value)}
                            />
                        </div>

                        <div className="form-group">
                            <p>Bevestig wachtwoord</p>
                            <input
                                type="password"
                                placeholder="Herhaal je wachtwoord"
                                className="input-field"
                                value={confirmPaswoord}
                                onChange={(e) => setConfirmPaswoord(e.target.value)}
                            />
                        </div>

                        <button
                            className="register-button"
                            onClick={handleRegister}
                            disabled={loading}
                        >
                            {loading ? "Even geduld..." : "Registreren"}
                        </button>

                        <div className="login-section">
                            <p>Heb je al een account?</p>
                            <Link to="/inloggen" className="login-link">
                                Inloggen
                            </Link>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );

}

export default Register;
