import React, { useState, useEffect } from "react";
import { useNavigate } from 'react-router-dom';
import { Eye, EyeOff } from 'lucide-react';
import "../styles/AccountInfo.css";

function AccountInfo() {
    const navigate = useNavigate();

    const [username, setUsername] = useState("");
    const [email, setEmail] = useState("");
    const [currentPassword, setCurrentPassword] = useState("");
    const [newPassword, setNewPassword] = useState("");
    const [userId, setUserId] = useState<number | null>(null);

    const [originalUsername, setOriginalUsername] = useState("");
    const [originalEmail, setOriginalEmail] = useState("");

    const [showCurrentPassword, setShowCurrentPassword] = useState(false);
    const [showNewPassword, setShowNewPassword] = useState(false);

    const [loading, setLoading] = useState(false);

    useEffect(() => {
        fetchAccountInfo();
    }, []);

    const fetchAccountInfo = async () => {
        const loggedIn = localStorage.getItem("loggedIn") === "true";
        const token = localStorage.getItem("token");

        if (!loggedIn || !token) {
            alert("Je bent niet ingelogd");
            navigate("/inloggen");
            return;
        }

        try {
            const response = await fetch("https://localhost:7020/api/auth/me", {
                method: "GET",
                headers: {
                    "Content-Type": "application/json",
                    "Authorization": `Bearer ${token}`
                }
            });

            if (!response.ok) {
                throw new Error("Kon accountgegevens niet ophalen");
            }

            const data = await response.json();
            setUserId(data.id);
            setUsername(data.userName);
            setEmail(data.email);
            setOriginalUsername(data.userName);
            setOriginalEmail(data.email);
        } catch (error) {
            console.error(error);
            alert("Fout bij het ophalen van accountgegevens");
        }
    };

    const handleSave = async () => {
        const loggedIn = localStorage.getItem("loggedIn") === "true";
        const token = localStorage.getItem("token");

        if (!loggedIn || !token) {
            alert("Je bent niet ingelogd");
            navigate("/inloggen");
            return;
        }

        if (!userId) {
            alert("Gebruikers-ID niet gevonden");
            return;
        }

        setLoading(true);

        try {
            // 1. Update username/email
            const updateResponse = await fetch(`https://localhost:7020/api/gebruikers/${userId}`, {
                method: "PUT",
                headers: {
                    "Content-Type": "application/json",
                    "Authorization": `Bearer ${token}`
                },
                body: JSON.stringify({
                    userName: username,
                    email: email,
                    phoneNumber: null,
                    newPassword: null
                })
            });

            if (!updateResponse.ok) {
                const error = await updateResponse.text();
                throw new Error(error || "Kon account niet bijwerken");
            }

            // 2. Update password if both fields are filled
            if (currentPassword && newPassword) {
                const passwordResponse = await fetch(
                    `https://localhost:7020/api/gebruikers/${userId}/update-password`,
                    {
                        method: "PUT",
                        headers: {
                            "Content-Type": "application/json",
                            "Authorization": `Bearer ${token}`
                        },
                        body: JSON.stringify({
                            CurrentPassword: currentPassword,
                            NewPassword: newPassword
                        })
                    }
                );

                if (!passwordResponse.ok) {
                    const error = await passwordResponse.text();
                    throw new Error(error || "Kon wachtwoord niet wijzigen");
                }

                setCurrentPassword("");
                setNewPassword("");
            }

            alert("Account succesvol bijgewerkt!");
            fetchAccountInfo();
        } catch (error: any) {
            console.error(error);
            alert(error.message || "Fout bij het opslaan");
        } finally {
            setLoading(false);
        }
    };

    const handleDeleteAccount = async () => {
        const confirmDelete = window.confirm(
            "Weet je zeker dat je je account wilt verwijderen? Deze actie kan niet ongedaan worden gemaakt."
        );

        if (!confirmDelete) return;

        const token = localStorage.getItem("token");

        if (!userId) {
            alert("Geen gebruikers-ID gevonden");
            return;
        }

        try {
            const response = await fetch(`https://localhost:7020/api/gebruikers/${userId}`, {
                method: "DELETE",
                headers: {
                    "Authorization": `Bearer ${token}`
                }
            });

            if (!response.ok) {
                const error = await response.text();
                throw new Error(error || "Kon account niet verwijderen");
            }

            alert("Account succesvol verwijderd");
            localStorage.removeItem("loggedIn");
            localStorage.removeItem("token");
            navigate("/");
        } catch (error: any) {
            console.error(error);
            alert(error.message || "Fout bij het verwijderen van account");
        }
    };

    return (
        <div className="AccountInfo-page">
            <h1 className="AccountInfo-title">Account Informatie</h1>
            <div className="info-box">
                <div className="form-group">
                    <p>Gebruikersnaam</p>
                    <input
                        type="text"
                        placeholder="Je gebruikersnaam"
                        className="change-accountinfo"
                        value={username}
                        onChange={(e) => setUsername(e.target.value)}
                    />
                </div>

                <div className="form-group">
                    <p>E-mailadres</p>
                    <input
                        type="email"
                        placeholder="Je E-mail"
                        className="change-accountinfo"
                        value={email}
                        onChange={(e) => setEmail(e.target.value)}
                    />
                </div>

                <div className="form-group">
                    <p>Huidig Wachtwoord (verplicht bij wijziging)</p>
                    <div className="password-input-container">
                        <input
                            type={showCurrentPassword ? "text" : "password"}
                            placeholder="Huidig wachtwoord"
                            className="change-accountinfo"
                            value={currentPassword}
                            onChange={(e) => setCurrentPassword(e.target.value)}
                        />
                        <button
                            type="button"
                            className="password-toggle"
                            onClick={() => setShowCurrentPassword(!showCurrentPassword)}
                        >
                            {showCurrentPassword ? <EyeOff size={20} /> : <Eye size={20} />}
                        </button>
                    </div>
                </div>

                <div className="form-group">
                    <p>Nieuw Wachtwoord</p>
                    <div className="password-input-container">
                        <input
                            type={showNewPassword ? "text" : "password"}
                            placeholder="Nieuw wachtwoord"
                            className="change-accountinfo"
                            value={newPassword}
                            onChange={(e) => setNewPassword(e.target.value)}
                        />
                        <button
                            type="button"
                            className="password-toggle"
                            onClick={() => setShowNewPassword(!showNewPassword)}
                        >
                            {showNewPassword ? <EyeOff size={20} /> : <Eye size={20} />}
                        </button>
                    </div>
                </div>
            </div>

            <div className="AccountSaveDeleteButtons">
                <button
                    className="saveAccountInfo"
                    onClick={handleSave}
                    disabled={loading}
                >
                    {loading ? "Bezig met opslaan..." : "Opslaan"}
                </button>
                <button
                    className="deleteAccount"
                    onClick={handleDeleteAccount}
                >
                    Account verwijderen
                </button>
            </div>
        </div>
    );
}

export default AccountInfo;
