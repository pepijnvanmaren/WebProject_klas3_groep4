import React, { useState, useEffect } from "react";
import { Link, useNavigate } from "react-router-dom";
import "../styles/Navbar.css";
import Logo from "../assets/RoyaleFloraLogo.svg";
import Trees from "../assets/treePicture.png";
import UserIcon from "../assets/userIcon3.png";

function Navbar() {
    const [isLoggedIn, setIsLoggedIn] = useState(false);
    const [userRole, setUserRole] = useState(null);
    const [homePath, setHomePath] = useState("/");
    const navigate = useNavigate();

    useEffect(() => {
        checkLoginStatus();

        window.addEventListener("storage", checkLoginStatus);
        return () => {
            window.removeEventListener("storage", checkLoginStatus);
        };
    }, []);

    useEffect(() => {
        if (userRole === "Aanvoerder") {
            setHomePath("/verkoperDashboard");

        } else if (userRole === "Veilingmeester") {
            setHomePath("/VeilingMeesterDashboard")
        } else {
            setHomePath("/Koperdashboard");
        }
    }, [userRole]);

    const checkLoginStatus = async () => {
        const token = localStorage.getItem("token");

        if (!token) {
            setIsLoggedIn(false);
            setUserRole(null);
            return;
        }

        setIsLoggedIn(true);
        await fetchUserRole(token);
    };

    const fetchUserRole = async (token) => {
        try {
            const response = await fetch("https://localhost:7020/api/auth/me", {
                method: "GET",
                headers: {
                    Authorization: "Bearer " + token,
                    "Content-Type": "application/json"
                }
            });

            if (response.ok) {
                const data = await response.json();
                setUserRole(data.rol);

                if (data.rol) {
                    localStorage.setItem("userRole", data.rol);
                }
            } else {
                handleLogout();
            }
        } catch (error) {
            console.error("Error loading user role:", error);
        }
    };

    const handleLogout = () => {
        localStorage.removeItem("token");
        localStorage.removeItem("userRole");
        localStorage.removeItem("userName");
        localStorage.removeItem("loggedIn");

        setIsLoggedIn(false);
        setUserRole(null);

        navigate("/");
    };

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
                            style={{ cursor: "pointer" }}
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
