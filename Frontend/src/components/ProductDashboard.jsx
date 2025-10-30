import React, { useState } from 'react';
import '../styles/ProductDashboard.css';

const apiBase = 'https://localhost:5174'

function SellerDashboard() {

    return (
        <><div className="dashboard-root">
            <header className="dashboard-header">
                <div className="header-inner">
                    <img
                        src="/header-trees.jpg"
                        alt="header"
                        className="header-image" />
                    <nav className="header-nav">
                        <a href="/registreren">Registreren</a>
                        <a href="/login">Inloggen</a>
                    </nav>
                </div>
            </header>
        </div>
            <main className="dashboard-container">
                <h1 className="title">Dashboard</h1>
                <div className="dashboard-box">
                    <h1>
                        Product naam
                    </h1>
                    <h1>
                        Product infomatie
                    </h1>
                    <h1>
                        Product foto
                    </h1>
                </div>
            </main>
        </>
    );
}


export default SellerDashboard;