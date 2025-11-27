import React, { useState, useEffect } from "react";
import { Link } from "react-router-dom";
import "../styles/Navbar.css";
import Logo from "../assets/RoyaleFloraLogo.svg";
import Trees from "../assets/treePicture.png";
import UserIcon from "../assets/userIcon3.png";

function Navbar() {
    // Auth state -- heeft nog code nodig voor authenticatie.
    const [isLoggedIn, setIsLoggedIn] = useState(false);

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

                        <Link to="/inloggen">
                            {isLoggedIn ? "Uitloggen" : "Inloggen"}
                        </Link>

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