import "../styles/AccountInfo.css";
import { Link } from 'react-router-dom';

function AccountInfo() {
    return (
        <div className="AccountInfo-page">
            <h1 className="AccountInfo-title">Account Informatie</h1>
            <div className="info-box">
                <p>Gebruikersnaam</p>
                <input
                    type="gebruikersnaam"
                    placeholder="Je gebruikersnaam(aanpassen)"
                    className="change-accountinfo"

                />   

                <p>E-mailadres</p>
                <input
                    type="email"
                    placeholder="Je E-mail(aanpassen)"
                    className="change-accountinfo"

                /> 

                <p>Wachtwoord</p>
                <input
                    type="password"
                    placeholder="Je wachtwoord(aanpassen)"
                    className="change-accountinfo"

                /> 
            </div>
            <div className = "AccountSaveDeleteButtons">
                <button className="saveAccountInfo" >
                    Opslaan
                </button>

                <Link to="/registreren" className="deleteAccount">
                    Account verwijderen
                </Link>
            </div>
        </div>
    );
};

export default AccountInfo;