import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import '../styles/VeilingTonen.css';
import { useEffect } from 'react';

function VeilingTonen({ id }) {
    const navigate = useNavigate();
    const [veiling, setVeiling] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);

    useEffect(() => {
        fetch(`https://localhost:5001/api/veiling/${id}`)
            .then(res => {
                if (!res.ok) {
                    throw new Error("Veiling not found");
                }
                return res.json();
            })
            .then(data => {
                setVeiling(data);
                setLoading(false);
            })
            .catch(err => {
                setError(err.message);
                setLoading(false);
            });
    }, [id]);

    if (loading) return (
        <>
            <main className="dashboard-container">
                <p>Loading...</p>
            </main>
        </>
    )

    if (error) return
    (
        <>
            <main className="dashboard-container">
                <p>Error</p>
            </main>
        </>
    )

    useEffect(() => {
        window.scrollTo(0, 0);
    }, [])
    return (
        <>
            <main className="dashboard-container">
                <h1 className="title">Veilinginformatie</h1>
                <div className="pt_dashboard-box ">
                    {/*Rechts*/}
                    <div className="PT-box">
                        <h1>
                            Veiling infomatie
                        </h1>
                        <p className="input-field-infomatie">
                            <strong>Beschrijving:</strong> {veiling.bechrijving}
                        </p>
                    </div>

                    {/*Rechts*/}
                    <div className="PT-box">
                        <h1>
                            Kloklocatie veiling
                        </h1>
                        <p className="input-field-Naam">
                            <strong>Locatie:</strong> {veiling.klokLocatie}
                        </p>
                        <h1>
                            Startdatum veiling
                        </h1>
                        <p className="input-field-Naam">
                            <strong>Startdatum:</strong> {veiling.startDatum}
                        </p>
                        <h1>
                            Starttijd veiling
                        </h1>
                        <p className="input-field-Naam">
                            <strong>Starttijd:</strong> {veiling.starTijd}
                        </p>
                    </div>
                </div>
                <div className="Veiling_product-list-box">
                    {loading ? (
                        <div className="loader">Laden...</div>
                    ) : products.length === 0 ? (
                        <div className="placeholder">
                            Geen Producten beschikbaar
                        </div>
                    ) : (
                        <ul className="product-list">
                            {products.map((product: { id: React.Key | null | undefined; name: string | number | bigint | boolean | React.ReactElement<unknown, string | React.JSXElementConstructor<any>> | Iterable<React.ReactNode> | React.ReactPortal | Promise<string | number | bigint | boolean | React.ReactPortal | React.ReactElement<unknown, string | React.JSXElementConstructor<any>> | Iterable<React.ReactNode> | null | undefined> | null | undefined; }) => (
                                <li key={product.id} className="product-item">
                                    <span>{product.name}</span>
                                </li>
                            ))}
                        </ul>
                    )}
                </div>
                <button className="pp_btn" onClick={handleGoBack}>Terug</button>
            </main>
        </>
    );
}

export default VeilingTonen;