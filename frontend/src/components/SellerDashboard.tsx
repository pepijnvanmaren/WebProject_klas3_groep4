import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import '../styles/SellerDashboard.css';

const apiBase = 'https://localhost:5174';

export interface Product {
    id: number;
    name: string;
}

function SellerDashboard() {
    const navigate = useNavigate();

    const [products, setProducts] = useState<Product[]>([]);
    const [loading, setLoading] = useState<boolean>(false);

    useEffect(() => {
        fetchProducts();
    }, []);

    const ProductTonenKnop = () => {
        navigate('/ProductDashboard');
    };

    const ProductMakenKnop = () => {
        navigate('/ProductMakenDashboard');
    };

    async function fetchProducts(): Promise<void> {
        setLoading(true);
        try {
            const res = await fetch(`${apiBase}/api/products`);
            if (!res.ok) throw new Error("Failed to fetch");
            const data: Product[] = await res.json();
            setProducts(data);
        } catch (err) {
            console.error(err);
            alert("Kon producten niet laden.");
        } finally {
            setLoading(false);
        }
    }

    async function deleteProduct(id: number): Promise<void> {
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
        <>
            <div className="dashboard-root">
                <header className="dashboard-header">
                    <div className="header-inner">
                        <img src="/header-trees.jpg" alt="header" className="header-image" />
                        <nav className="header-nav">
                            <a href="/registreren">Registreren</a>
                            <a href="/login">Inloggen</a>
                        </nav>
                    </div>
                </header>
            </div>

            <main className="dashboard-container">
                <h1 className="VerkoperDashboard-title">Dashboard</h1>
                <div className="dashboard-box">
                    <button className="btn primary" onClick={ProductMakenKnop}>
                        Product Plaatsen
                    </button>
                    <button className="btn secondary" onClick={ProductTonenKnop}>
                        Product Tonen
                    </button>
                    <button
                        className="btn danger"
                        onClick={() => {
                            if (products.length === 0) {
                                alert("Geen producten om te verwijderen.");
                                return;
                            }
                            const lastProduct = products[products.length - 1];
                            if (lastProduct) deleteProduct(lastProduct.id);
                        }}
                    >
                        Product Verwijderen
                    </button>
                </div>
            </main>

            <div className="product-list-box">
                {loading ? (
                    <div className="loader">Laden...</div>
                ) : products.length === 0 ? (
                    <div className="placeholder">Geen producten beschikbaar</div>
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