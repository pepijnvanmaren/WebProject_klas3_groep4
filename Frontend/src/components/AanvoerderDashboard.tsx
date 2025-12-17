import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import '../styles/AanvoerderDashboard.css';

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
                    const token = localStorage.getItem("token");
                    const response = await fetch("https://localhost:7020/api/Auth/me", {
                        
                        headers: {
                            "Content-Type": "application/json",
                            "Authorization": `Bearer ${token}`
                        }
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

    return (
        <div className="product-dashboard-page">
            {isLoggedIn && user && (
                <div className="user-welcome">
                    <p>
                        Welkom, <strong>{user.userName}</strong>!
                    </p>
                </div>
            )}

            <main className="dashboard-container">
                <h1 className="dashboard-title">Dashboard</h1>

                <div className="dashboard-box">
                    <button
                        className="dashboard-btn primary"
                        onClick={ProductMakenKnop}
                    >
                        Product Plaatsen
                    </button>

                    <button
                        className="dashboard-btn secondary"
                        onClick={ProductTonenKnop}
                    >
                        Product Tonen
                    </button>
                </div>
            </main>
        </div>
    );

}

export default SellerDashboard;