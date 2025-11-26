import React from 'react';
import '../styles/Footer.css';

const Footer: React.FC = () => {
    const currentYear = new Date().getFullYear();

    return (
        <footer className="footer">
            <div className="footer-container">
                <div className="footer-section">
                    <h3 className="footer-title">Over Ons</h3>
                    <p className="footer-text">
                        Wij leveren innovatieve oplossingen voor jouw digitale behoeften.
                    </p>
                </div>

                <div className="footer-section">
                    <h3 className="footer-title">Link</h3>
                    <ul className="footer-links">
                        <li><a href="/privacy">Privacy</a></li>
                    </ul>
                </div>

                <div className="footer-section">
                    <h3 className="footer-title">Contact</h3>
                    <ul className="footer-contact">
                        <li>Email: info@example.nl</li>
                        <li>Tel: +31 6 12345678</li>
                        <li>Den Haag, Nederland</li>
                    </ul>
                </div>

                <div className="footer-section">
                    <h3 className="footer-title">Volg Ons</h3>
                    <div className="footer-social">
                        <a href="https://facebook.com" target="_blank" rel="noopener noreferrer">
                            Facebook
                        </a>
                        <a href="https://twitter.com" target="_blank" rel="noopener noreferrer">
                            Twitter
                        </a>
                        <a href="https://linkedin.com" target="_blank" rel="noopener noreferrer">
                            LinkedIn
                        </a>
                    </div>
                </div>
            </div>

            <div className="footer-bottom">
                <p>&copy; {currentYear} Royal Flora Holland. Alle rechten voorbehouden.</p>
            </div>
        </footer>
    );
};

export default Footer;