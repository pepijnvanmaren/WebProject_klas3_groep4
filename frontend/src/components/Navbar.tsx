import React, { useState, useEffect } from "react";
import { Link, useNavigate } from "react-router-dom";
import "../styles/Navbar.css";
import Logo from "../assets/RoyaleFloraLogo.svg";
import Trees from "../assets/treePicture.png";
import UserIcon from "../assets/userIcon3.png";

function Navbar() {
    const [isLoggedIn, setIsLoggedIn] = useState(false);
    const navigate = useNavigate();

    // Check login status wanneer component mount
    useEffect(() => {
        const loggedIn = localStorage.getItem("loggedIn") === "true";
        setIsLoggedIn(loggedIn);
    }, []);

    // Handle logout
    const handleLogout = async () => {
        try {
            await fetch("https://localhost:7020/api/auth/logout", {
                method: "POST",
                credentials: "include"
            });
        } catch (error) {
            console.error("Logout error:", error);
        } finally {
            localStorage.removeItem("loggedIn");
            localStorage.removeItem("userRole");
            setIsLoggedIn(false);
            navigate("/inloggen");
        }
    };

    return (
        <header className="header-container">
            <img src={Trees} alt="tree picture" className="tree-picture" />
            <nav className="navbar">
                <div className="nav-content">
                    <li>
                        <Link to="/">
                            <img src={Logo} alt="Royale Flora" className="nav-logo" />
                        </Link>
                    </li>
                    <ul className="nav-links">
                        {isLoggedIn ? (
                            <button
                                onClick={handleLogout}
                                className="logout-button"
                            >
                                Uitloggen
                            </button>
                        ) : (
                            <Link to="/inloggen">Inloggen</Link>
                        )}
                        {!isLoggedIn && (
                            <Link to="/registreren">Registreren</Link>
                        )}
                        {isLoggedIn && (
                            <Link to="/AccountInfo">
                                <img
                                    src={UserIcon}
                                    alt="Royale Flora"
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