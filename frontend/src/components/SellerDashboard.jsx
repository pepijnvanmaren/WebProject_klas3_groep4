import React, { useState } from 'react';
import '../styles/SellerDashboard.css';

const apiBase = 'https://localhost:5174';

function SellerDashboard() {
    const [products, setProducts] = useState([]);
    const [loading, setLoading] = useState(false);



    async function fetchProducts() {
        setLoading(true);
        try {
            const res = await fetch(`${apiBase}/api/products`);
            if (!res.ok) throw new Error("Failed to fetch");
            const data = await res.json();
            setProducts(data);
        } catch (err) {
            console.error(err);
            alert("Kon producten niet laden.");
        } finally {
            setLoading(false);
        }
    }

    async function createProduct() {
        const sample = { name: "Nieuw product " + (products.length + 1) };
        try {
            const res = await fetch(`${apiBase}/api/products`, {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(sample),
            });
            if (!res.ok) throw new Error("Failed to create");
            await fetchProducts();
        } catch (err) {
            console.error(err);
            alert("Kon product niet aanmaken.");
        }
    }

    async function deleteProduct(id) {
        if (!window.confirm("Weet je zeker dat je dit product wilt verwijderen?")) return;
        try {
            const res = await fetch(`${apiBase}/api/products/${id}`, {
                method: "DELETE",
            });
            if (!res.ok) throw new Error("Failed to delete");
            await fetchProducts();
        } catch (err) {
            console.error(err);
            alert("Kon product niet verwijderen.");
        }
    }

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
                    <button className="btn primary" onClick={createProduct}>
                        Product Plaatsen
                    </button>
                    <button className="btn secondary" onClick={fetchProducts}>
                        Product Tonen
                    </button>
                    <button
                        className="btn danger"
                        onClick={() => {
                            if (products.length === 0) {
                                alert("Geen producten om te verwijderen.");
                                return;
                            }
                            deleteProduct(products[products.length - 1].id);
                        }}>
                        Product Verwijderen
                    </button>
                </div>
            </main>
            <div className="product-list-box">
                {loading ? (
                    <div className="loader">Laden...</div>
                ) : products.length === 0 ? (
                    <div className="placeholder">
                        Geen producten beschikbaar
                    </div>
                ) : (
                    <ul className="product-list">
                        {products.map((product) => (
                            <li key={product.id} className="product-item">
                                <span>{product.name}</span>
                            </li>
                        ))}
                    </ul>
                )}
            </div>
        </>
    );
}


export default SellerDashboard;