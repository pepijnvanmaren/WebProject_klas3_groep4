import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import '../styles/VeilingMeesterDashboard.css';
import { useEffect } from 'react';



const apiBase = 'https://localhost:5174';
type User = {
    id: number;
    userName: string;
    email: string;
    phoneNumber: string;
    rol: string;
    veilingVestiging: string | null;
};
function VeilingMeesterDashboard() {
    const navigate = useNavigate();
    const [isLoggedIn, setIsLoggedIn] = useState(false);
    const [user, setUser] = useState<User | null>(null);

    useEffect(() => {
        window.scrollTo(0, 0);
    }, [])


    // NIEUW: Fetch gebruikersgegevens bij laden
    useEffect(() => {
        const checkLoginStatus = async () => {
            const loggedIn = localStorage.getItem("loggedIn") === "true";
            console.log("Is logged in:", loggedIn);
            setIsLoggedIn(loggedIn);

            if (loggedIn) {
                try {
                    const response = await fetch("https://localhost:7020/api/Auth/me", {
                        credentials: "include",
                    });
                    console.log("Me endpoint response status:", response.status);
                    if (response.ok) {
                        const userData = await response.json();
                        console.log("User data fetched:", userData);
                        setUser(userData);
                    } else {
                        console.log("Me endpoint response not ok:", response.status);
                    }
                } catch (err) {
                    console.error("Error fetching user data:", err);
                }
            }
        };

        checkLoginStatus();
    }, []);

    useEffect(() => {
        window.scrollTo(0, 0);
    }, [])

    const VeiulingPlaatsenKnop = () => {
        navigate('/VeilingPlaatsen')
    }

    const VeilingenTonenKnop = () => {
        navigate('/VeilingTonen')
    }

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

            {/* NIEUW: Gebruiker welkom bericht */}
            {isLoggedIn && user && (
                <div className="user-welcome">
                    <p>Welkom, <strong>{user.userName}</strong>!</p>
                </div>
            )}

            <main className="dashboard-container">
                <h1 className="VerkoperDashboard-title">Veilingmeester dashboard</h1>
                <div className="VL_dashboard-box">
                    <button className="VL_pp_btn" onClick={VeiulingPlaatsenKnop}>
                        Veilingen Plaatsen
                    </button>
                    <button className="VL_pp_btn" onClick={VeilingenTonenKnop}>
                        Veilingen Tonen
                    </button>
                </div>
            </main>
        </>
    );
}


export default VeilingMeesterDashboard;