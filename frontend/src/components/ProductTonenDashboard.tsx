import '../styles/ProductTonenDashboard.css';
import { useNavigate } from 'react-router-dom';
import logo from '../assets/tulp.png';

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
                <h1 className="title">Productinformatie</h1>
                <div className="pt_dashboard-box ">
                    {/*Rechts*/}
                    <div className="PT-box">
                    <h1>
                        Product naam
                    </h1>
                    <p className="input-field-Naam">
                        Tulp Royal Ruby
                    </p>
                    <h1>
                        Product infomatie
                    </h1>
                    <p   className="input-field-infomatie">
                            Breng een vleugje voorjaar in huis of tuin met de prachtige Tulp Royal Ruby. Deze klassieke tulp valt op door haar diepe robijnrode kleur en perfecte, slanke bloemvorm. De stevige stelen maken haar ideaal voor zowel de tuin als in een vaas: ze blijft lang mooi en rechtop staan.
                    </p>
                    <h1>
                        Product foto
                    </h1>
                        <img className="BloemFoto" src={logo} alt="Foto van het gekozen product">
                    </img>
                    </div>

                    {/*Rechts*/}
                    <div className="PT-box">
                        <h1>
                            Oogstdatum
                        </h1>
                        <p className="input-field-Naam">
                            13/04/2025
                        </p>
                        <h1>
                            Potmaat (cm)
                        </h1>
                        <p className="input-field-Naam">
                            40 cm
                        </p>
                        <h1>
                            Gewicht (kg)
                        </h1>
                        <p className="input-field-Naam">
                            5 kg
                        </p>
                        <h1>
                            Steellengte (cm)
                        </h1>
                        <p className="input-field-Naam">
                            5 cm
                        </p>
                        <h1>
                            Hoeveelheid
                        </h1>
                        <p className="input-field-Naam">
                            20
                        </p>
                        <h1>
                            Minimale prijs
                        </h1>
                        <p className="input-field-Naam">
                            5 euro
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