import '../styles/ProductTonenDashboard.css';
import { useNavigate } from 'react-router-dom';

function SellerDashboard() {
    const navigate = useNavigate();

    const ProductMakenKnop = () => {
        navigate('/verkoperDashboard')
    }

    const ProductGemaaktAlert = () => {
        alert("Uw prodcut is gemaakt.");
    }

    return (
        <>
            <main className="dashboard-container">
                <h1 className="title">Product</h1>
                <div className="pt_dashboard-box ">
                    {/*Rechts*/}z
                    <div className="PT-box">
                    <h1>
                        Product naam
                    </h1>
                    <p className="input-field-Naam">
                        nep tekst
                    </p>
                    <h1>
                        Product infomatie
                    </h1>
                    <p   className="input-field-infomatie">
                        nep tekst
                    </p>
                    <h1>
                        Product foto
                    </h1>
                    <img src="" alt="Foto van het gekozen product">
                    </img>
                    </div>

                    {/*Rechts*/}
                    <div className="PT-box">
                        <h1>
                            Oogstdatum
                        </h1>
                        <p className="input-field-Naam">
                            nep tekst
                        </p>
                        <h1>
                            Potmaat (cm)
                        </h1>
                        <p className="input-field-Naam">
                            nep tekst
                        </p>
                        <h1>
                            Gewicht (kg)
                        </h1>
                        <p className="input-field-Naam">
                            nep tekst
                        </p>
                        <h1>
                            Steellengte (cm)
                        </h1>
                        <p className="input-field-Naam">
                            nep tekst
                        </p>
                        <h1>
                            Hoeveelheid
                        </h1>
                        <p className="input-field-Naam">
                            nep tekst
                        </p>
                        <h1>
                            Minimale prijs
                        </h1>
                        <p className="input-field-Naam">
                            nep tekst
                        </p>
                    </div>
                </div>
                <button className="login-button" onClick={ProductMakenKnop} >
                    Terug
                </button>

            </main>
        </>
    );
}


export default SellerDashboard;