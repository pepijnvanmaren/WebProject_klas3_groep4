import React from 'react';
import "../styles/Privacy.css";

const Privacy: React.FC = () => {
    const lastUpdated = "13 november 2025";

    return (
        <div className="privacy-container">
            <div className="privacy-content">
                <h1 className="privacy-heading">Privacyverklaring</h1>
                <p className="privacy-updated">Laatst bijgewerkt: {lastUpdated}</p>

                <section className="privacy-section">
                    <h2>1. Inleiding</h2>
                    <p>
                        Welkom bij onze privacyverklaring. Wij respecteren uw privacy en doen er alles aan om
                        uw persoonlijke gegevens te beschermen. Deze privacyverklaring legt uit hoe wij omgaan
                        met uw persoonsgegevens wanneer u onze website bezoekt of gebruik maakt van onze diensten.
                    </p>
                </section>

                <section className="privacy-section">
                    <h2>2. Welke gegevens verzamelen wij?</h2>
                    <p>Wij kunnen de volgende gegevens van u verzamelen:</p>
                    <ul>
                        <li>Naam en contactgegevens (e-mailadres, telefoonnummer)</li>
                        <li>Bedrijfsinformatie</li>
                        <li>IP-adres en browserinformatie</li>
                        <li>Gebruiksgegevens van onze website</li>
                        <li>Cookies en vergelijkbare technologieën</li>
                    </ul>
                </section>

                <section className="privacy-section">
                    <h2>3. Waarvoor gebruiken wij uw gegevens?</h2>
                    <p>Wij gebruiken uw persoonlijke gegevens voor de volgende doeleinden:</p>
                    <ul>
                        <li>Het leveren van onze diensten en producten</li>
                        <li>Communicatie over uw account of bestellingen</li>
                        <li>Verbetering van onze website en diensten</li>
                        <li>Verzending van nieuwsbrieven (met uw toestemming)</li>
                        <li>Naleving van wettelijke verplichtingen</li>
                    </ul>
                </section>

                <section className="privacy-section">
                    <h2>4. Hoe lang bewaren wij uw gegevens?</h2>
                    <p>
                        Wij bewaren uw persoonlijke gegevens niet langer dan noodzakelijk voor de doeleinden
                        waarvoor zij zijn verzameld. De bewaartermijn isafhankelijk van de aard van de gegevens
                        en het doel van de verwerking, maar wordt mede bepaald door wettelijke bewaarplichten.
                    </p>
                </section>

                <section className="privacy-section">
                    <h2>5. Delen van gegevens met derden</h2>
                    <p>
                        Wij verkopen uw gegevens niet aan derden. Wij kunnen uw gegevens delen met:
                    </p>
                    <ul>
                        <li>Dienstverleners die ons helpen bij het leveren van onze diensten</li>
                        <li>Overheidsinstanties wanneer dit wettelijk verplicht is</li>
                        <li>Bedrijfspartners (alleen met uw toestemming)</li>
                    </ul>
                </section>

                <section className="privacy-section">
                    <h2>6. Cookies</h2>
                    <p>
                        Onze website maakt gebruik van cookies om uw gebruikservaringen te verbeteren en
                        om statistieken bij te houden. U kunt cookies uitschakelen in uw browserinstellingen,
                        maar dit kan de functionaliteit van onze website beperken.
                    </p>
                </section>

                <section className="privacy-section">
                    <h2>7. Uw rechten</h2>
                    <p>Volgens de AVG (Algemene Verordening Gegevensbescherming) heeft u de volgende rechten:</p>
                    <ul>
                        <li>Recht op inzage van uw gegevens</li>
                        <li>Recht op rectificatie (verbetering van onjuiste gegevens)</li>
                        <li>Recht op verwijdering (recht om vergeten te worden)</li>
                        <li>Recht op beperking van de verwerking</li>
                        <li>Recht op dataportabiliteit</li>
                        <li>Recht van bezwaar tegen verwerking</li>
                    </ul>
                    <p>
                        Om deze rechten uit te oefenen, kunt u contact met ons opnemen via de onderstaande
                        contactgegevens.
                    </p>
                </section>

                <section className="privacy-section">
                    <h2>8. Beveiliging</h2>
                    <p>
                        Wij nemen passende technische en organisatorische maatregelen om uw persoonlijke
                        gegevens te beschermen tegen verlies, misbruik of ongeautoriseerde toegang. Dit omvat
                        onder andere het gebruik van versleuteling en beveiligde servers.
                    </p>
                </section>

                <section className="privacy-section">
                    <h2>9. Wijzigingen in deze privacyverklaring</h2>
                    <p>
                        Wij kunnen deze privacyverklaring van tijd tot tijd aanpassen. De meest recente versie
                        is altijd beschikbaar op onze website. Wij raden u aan regelmatig deze pagina te
                        raadplegen om op de hoogte te blijven van eventuele wijzigingen.
                    </p>
                </section>

                <section className="privacy-section">
                    <h2>10. Contact</h2>
                    <p>
                        Heeft u vragen over deze privacyverklaring of over hoe wij omgaan met uw persoonlijke
                        gegevens? Neem dan contact met ons op:
                    </p>
                    <div className="privacy-contact">
                        <p><strong>E-mail:</strong> privacy@example.nl</p>
                        <p><strong>Telefoon:</strong> +31 6 12345678</p>
                        <p><strong>Adres:</strong> Straatnaam 123, 2500 AA Den Haag</p>
                    </div>
                </section>

                <section className="privacy-section">
                    <h2>11. Klachten</h2>
                    <p>
                        Als u een klacht heeft over de manier waarop wij uw persoonlijke gegevens verwerken,
                        kunt u contact met ons opnemen. U heeft ook het recht om een klacht in te dienen bij
                        de Autoriteit Persoonsgegevens.
                    </p>
                </section>
            </div>
        </div>
    );
};

export default Privacy;