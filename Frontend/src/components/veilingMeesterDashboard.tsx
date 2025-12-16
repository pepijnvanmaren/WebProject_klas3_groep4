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
                    const token = localStorage.getItem("token");
                    const response = await fetch("https://localhost:7020/api/Auth/me", {
                        
                        headers: {
                            "Content-Type": "application/json",
                            "Authorization": `Bearer ${token}`
                        }
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
        <div className="auctioneer-dashboard-page">

            {/* WELCOME */}
            {isLoggedIn && user && (
                <div className="auctioneer-dashboard-welcome">
                    <p>
                        Welkom, <strong>{user.userName}</strong>!
                    </p>
                </div>
            )}

            {/* MAIN */}
            <main className="auctioneer-dashboard-container">
                <h1 className="auctioneer-dashboard-title">
                    Veilingmeester dashboard
                </h1>

                <div className="auctioneer-dashboard-box">
                    <button
                        className="auctioneer-dashboard-btn"
                        onClick={VeiulingPlaatsenKnop}
                    >
                        Veilingen plaatsen
                    </button>

                    <button
                        className="auctioneer-dashboard-btn"
                        onClick={VeilingenTonenKnop}
                    >
                        Veilingen tonen
                    </button>
                </div>
            </main>
        </div>
    );

}


export default VeilingMeesterDashboard;