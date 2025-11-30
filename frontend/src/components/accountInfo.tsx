import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import '../styles/AccountInfo.css';

type User = {
    id: number;
    userName: string;
    email: string;
    phoneNumber: string;
    rol: string;
    veilingVestiging: string | null;
};

function AccountInfo() {
    const navigate = useNavigate();
    const [user, setUser] = useState<User | null>(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    // Fetch gebruikersgegevens
    useEffect(() => {
        const fetchUserData = async () => {
            const loggedIn = localStorage.getItem("loggedIn") === "true";

            if (!loggedIn) {
                navigate('/inloggen');
                return;
            }

            try {
                const response = await fetch("https://localhost:7020/api/Auth/me", {
                    credentials: "include",
                });

                if (response.ok) {
                    const userData = await response.json();
                    setUser(userData);
                } else {
                    setError("Kon gebruikersgegevens niet ophalen");
                }
            } catch (err) {
                console.error("Error fetching user data:", err);
                setError("Er is een fout opgetreden bij het ophalen van gegevens");
            } finally {
                setLoading(false);
            }
        };

        fetchUserData();
    }, [navigate]);

    const handleLogout = () => {
        localStorage.removeItem("loggedIn");
        navigate('/inloggen');
    };

    const handleBackToDashboard = () => {
        if (user?.rol === "Aanvoerder") {
            navigate('/SellerDashboard');
        } else if (user?.rol === "Koper") {
            navigate('/');
        } else {
            navigate('/');
        }
    };

    if (loading) {
        return (
            <div className="account-info-page">
                <div className="loading">Laden...</div>
            </div>
        );
    }

    if (error) {
        return (
            <div className="account-info-page">
                <div className="error-message">{error}</div>
                <button onClick={() => navigate('/inloggen')} className="btn-primary">
                    Terug naar inloggen
                </button>
            </div>
        );
    }

    if (!user) {
        return (
            <div className="account-info-page">
                <div className="error-message">Geen gebruikersgegevens gevonden</div>
            </div>
        );
    }

    return (
        <div className="account-info-page">
            <div className="account-container">
                <h1 className="account-title">Mijn Account</h1>

                <div className="account-card">
                    <div className="account-header">
                        <div className="user-avatar">
                            {user.userName.charAt(0).toUpperCase()}
                        </div>
                        <h2>{user.userName}</h2>
                        <span className={`role-badge ${user.rol.toLowerCase()}`}>
                            {user.rol}
                        </span>
                    </div>

                    <div className="account-details">
                        <div className="detail-row">
                            <span className="detail-label">Gebruikersnaam:</span>
                            <span className="detail-value">{user.userName}</span>
                        </div>

                        <div className="detail-row">
                            <span className="detail-label">Email:</span>
                            <span className="detail-value">{user.email}</span>
                        </div>

                        <div className="detail-row">
                            <span className="detail-label">Telefoonnummer:</span>
                            <span className="detail-value">
                                {user.phoneNumber || 'Niet ingevuld'}
                            </span>
                        </div>

                        <div className="detail-row">
                            <span className="detail-label">Rol:</span>
                            <span className="detail-value">{user.rol}</span>
                        </div>

                        {user.veilingVestiging && (
                            <div className="detail-row">
                                <span className="detail-label">Veilingvestiging:</span>
                                <span className="detail-value">{user.veilingVestiging}</span>
                            </div>
                        )}

                        <div className="detail-row">
                            <span className="detail-label">Account ID:</span>
                            <span className="detail-value">{user.id}</span>
                        </div>
                    </div>

                    <div className="account-actions">
                        <button
                            onClick={handleBackToDashboard}
                            className="btn-secondary"
                        >
                            Terug naar Dashboard
                        </button>
                        <button
                            onClick={handleLogout}
                            className="btn-danger"
                        >
                            Uitloggen
                        </button>
                    </div>
                </div>
            </div>
        </div>
    );
}

export default AccountInfo;