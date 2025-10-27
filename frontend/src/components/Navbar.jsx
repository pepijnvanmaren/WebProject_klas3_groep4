import React from "react";
import "../styles/Navbar.css";
import Logo from "../assets/RoyaleFloraLogo.svg";
import Trees from "../assets/treePicture.png";

function Navbar() {
    return (
        <header className="header-container">
            <img src={Trees} alt="tree picture" className="tree-picture" />
            <nav className="navbar">
                <div className="nav-content">
                    <img src={Logo} alt="Royale Flora" className="nav-logo" />
                    <ul className="nav-links">
                        <li><a href="#home">Home</a></li>
                        <li><a href="#about">Over ons</a></li>
                        <li><a href="#contact">Contact</a></li>
                    </ul>
                </div>
            </nav>
        </header>
    );
}

export default Navbar;
