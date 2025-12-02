import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import '../styles/VeilingMeesterDashboard.css';
import { useEffect } from 'react';

const apiBase = 'https://localhost:5174';

function VeilingMeesterDashboard() {
    const navigate = useNavigate();
    const [products, setProducts] = useState([]);
    const [loading, setLoading] = useState(false);

    useEffect(() => {
        window.scrollTo(0, 0);
    }, [])

    const VeiulingPlaatsenKnop = () => {
        navigate('/VeilingPlaatsen')
    }

    const VeilingenTonenKnop = () => {
        navigate('/VeilingTonen')
    }

    const VeilingBewerkenKnop = () => {
        navigate('/VeilingBewerken')
    }

    return (
        <>
            <main className="dashboard-container">
                <h1 className="VerkoperDashboard-title">Veilingmeester dashboard</h1>
                <div className="dashboard-box">
                    <button className="btn" onClick={VeiulingPlaatsenKnop}>
                        Veilingen Plaatsen
                    </button>
                    <button className="btn" onClick={VeilingenTonenKnop}>
                        Veilingen Tonen
                    </button>
                    <button className="btn" onClick={VeilingBewerkenKnop}>
                        Producten Toevoegen
                    </button>
                </div>
            </main>
        </>
    );
}


export default VeilingMeesterDashboard;