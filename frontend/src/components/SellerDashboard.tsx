import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import '../styles/SellerDashboard.css';

const apiBase = 'https://localhost:7020'; // Let op: gebruik dezelfde port als je API

type User = {
    id: number;
    userName: string;
    email: string;
    phoneNumber: string;
    rol: string;
    veilingVestiging: string | null;
};

function SellerDashboard() {
    const navigate = useNavigate();
    const [products, setProducts] = useState([]);
    const [loading, setLoading] = useState(false);

    // NIEUW: State voor gebruiker en login status
    const [isLoggedIn, setIsLoggedIn] = useState(false);
    const [user, setUser] = useState<User | null>(null);

    // NIEUW: Fetch gebruikersgegevens bij laden
    useEffect(() => {
        const checkLoginStatus = async () => {
            const loggedIn = localStorage.getItem("loggedIn") === "true";
            console.log("Is logged in:", loggedIn);
            setIsLoggedIn(loggedIn);

            if (loggedIn) {
                try {
                    const response = await fetch("https://localhost:7020/api/Auth/me", {
                        credentials: "include",
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

    const ProductTonenKnop = () => {
        navigate('/ProductDashboard');
    };

    const ProductMakenKnop = () => {
        navigate('/ProductMakenDashboard');
    };

    async function fetchProducts() {
        setLoading(true);
        try {
            const res = await fetch(`${apiBase}/api/Product`);
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

    async function deleteProduct(id: number) {
        if (!window.confirm("Weet je zeker dat je dit product wilt verwijderen?")) return;
        try {
            const res = await fetch(`${apiBase}/api/Product/${id}`, {
                method: "DELETE",
                credentials: "include",
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
                        <img
                            src="/header-trees.jpg"
                            alt="header"
                            className="header-image"
                        />
                        <nav className="header-nav">
                            <a href="/registreren">Registreren</a>
                            <a href="/login">Inloggen</a>
                        </nav>
                    </div>
                </header>
            </div>

            {/* NIEUW: Gebruiker welkom bericht */}
            {isLoggedIn && user && (
                <div className="user-welcome">
                    <p>Welkom, <strong>{user.userName}</strong>!</p>
                </div>
            )}

            <main className="dashboard-container">
                <h1 className="VerkoperDashboard-title">Dashboard</h1>
                <div className="dashboard-box">
                    <button className="btn primary" onClick={ProductMakenKnop}>
                        Product Plaatsen
                    </button>
                    <button className="btn secondary" onClick={ProductTonenKnop}>
                        Product Tonen
                    </button>
                   
                </div>
            </main>

           
        </>
    );
}

export default SellerDashboard;