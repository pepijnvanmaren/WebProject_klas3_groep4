import React from "react";
import { Link } from "react-router-dom";
import "../styles/Navbar.css";
import Logo from "../assets/RoyaleFloraLogo.svg";
import Trees from "../assets/treePicture.png";
import UserIcon from "../assets/userIcon3.png";
    
function Navbar() {
    return (
        <header className="header-container">
            <img src={Trees} alt="tree picture" className="tree-picture" />
            <nav className="navbar">
                <div className="nav-content">
                    <li><a href="#home"><img src={Logo} alt="Royale Flora" className="nav-logo" /></a></li>
                    <ul className="nav-links">
                        <li><a href="#about">Over ons</a></li>
                        <Link to="/inloggen">Uitloggen</Link>
                        <Link to="/AccountInfo"><img src={UserIcon} alt="Royale Flora" className="nav-userIcon" /></Link>
                    </ul>
                </div>
            </nav>
        </header>
    );
}

export default Navbar;
