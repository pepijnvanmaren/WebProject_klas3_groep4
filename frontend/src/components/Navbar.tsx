import React, { useState, useEffect } from "react";
import { Link, useNavigate } from "react-router-dom";
import "../styles/Navbar.css";
import Logo from "../assets/RoyaleFloraLogo.svg";
import Trees from "../assets/treePicture.png";
import UserIcon from "../assets/userIcon3.png";

function Navbar() {
    const [isLoggedIn, setIsLoggedIn] = useState(false);
    const [userRole, setUserRole] = useState<string | null>(null);
    const [homePath, setHomePath] = useState("/");
    const navigate = useNavigate();

    // Check of gebruiker is ingelogd bij het laden van de component
    useEffect(() => {
        checkLoginStatus();

        // Luister naar localStorage changes
        window.addEventListener('storage', checkLoginStatus);

        return () => {
            window.removeEventListener('storage', checkLoginStatus);
        };
    }, []);

    // Update home path wanneer rol verandert
    useEffect(() => {
        if (userRole === "Aanvoerder") {
            setHomePath("/verkoperDashboard");

        } else if (userRole === "Veilingmeester") {
            setHomePath("/VeilingMeesterDashboard")
        } else {
            setHomePath("/Koperdashboard");
        }
    }, [userRole]);

    // Functie om login status en rol te checken
    const checkLoginStatus = async () => {
        const loggedIn = localStorage.getItem("loggedIn") === "true";

        if (loggedIn) {
            setIsLoggedIn(true);

            // Haal rol op van de server
            await fetchUserRole();
        } else {
            setIsLoggedIn(false);
            setUserRole(null);
        }
    };

    // Functie om gebruikersrol op te halen
    const fetchUserRole = async () => {
        try {
            const response = await fetch("https://localhost:7020/api/auth/me", {
                method: "GET",
                credentials: "include",
                headers: {
                    "Content-Type": "application/json"
                }
            });

            if (response.ok) {
                const data = await response.json();
                setUserRole(data.rol);

                // Optioneel: sla rol ook op in localStorage voor snellere toegang
                if (data.rol) {
                    localStorage.setItem("userRole", data.rol);
                }
            } else {
                // Cookie is ongeldig
                handleLogout();
            }
        } catch (error) {
            console.error("Fout bij ophalen gebruikersrol:", error);
        }
    };

    // Functie om uit te loggen
    const handleLogout = async () => {
        try {
            await fetch("https://localhost:7020/api/auth/logout", {
                method: "POST",
                credentials: "include"
            });
        } catch (error) {
            console.error("Logout error:", error);
        } finally {
            // Verwijder gegevens
            localStorage.removeItem("loggedIn");
            localStorage.removeItem("userRole");

            setIsLoggedIn(false);
            setUserRole(null);
            navigate("/");
        }
    };

    // Functie om login/logout te handelen
    const handleAuthClick = () => {
        if (isLoggedIn) {
            handleLogout();
        } else {
            navigate("/inloggen");
        }
    };

    return (
        <header className="header-container">
            <img src={Trees} alt="tree picture" className="tree-picture" />
            <nav className="navbar">
                <div className="nav-content">
                    <li>
                        <Link to={homePath}>
                            <img src={Logo} alt="Royale Flora" className="nav-logo" />
                        </Link>
                    </li>
                    <ul className="nav-links">
                        <li
                            className="auth-button"
                            onClick={handleAuthClick}
                            style={{ cursor: 'pointer' }}
                        >
                            {isLoggedIn ? "Uitloggen" : "Inloggen"}
                        </li>

                        {!isLoggedIn && (
                            <Link to="/registreren">Registreren</Link>
                        )}

                        {isLoggedIn && (
                            <Link to="/AccountInfo">
                                <img
                                    src={UserIcon}
                                    alt="Account"
                                    className="nav-userIcon"
                                />
                            </Link>
                        )}
                    </ul>
                </div>
            </nav>
        </header>
    );
}

export default Navbar;