import "../styles/index.css";
import React, { useEffect, useState, useRef } from "react";
import { useNavigate } from "react-router-dom";

//Refereerd naar de DTO
type Review = {
    id: number;
    naam: string;
    tekst: string;
    sterren: number;
};



const dummyReviews: Review[] = [
    { id: 1, naam: "Sophie V.", tekst: "Fantastische service en prachtige bloemen!", sterren: 5 },
    { id: 2, naam: "Jan K.", tekst: "Snelle levering en goede kwaliteit.", sterren: 4 },
    { id: 3, naam: "Lotte M.", tekst: "Zeer tevreden, zeker een aanrader.", sterren: 5 },
    { id: 4, naam: "Emma T.", tekst: "Goede prijzen en vriendelijke klantenservice.", sterren: 4 },
    { id: 5, naam: "Mark D.", tekst: "De bloemen waren vers en mooi verpakt.", sterren: 5 },
];

function Index() {



    return (
        <div className="page">
            {/* Over ons */}
            <div className="user-welcome">
                <h1>Welkom bij Floriday!</h1>
            </div>

            
            

            {/* Reviews */}
            <div className="reviews-section">
                <h2>Wat onze klanten zeggen</h2>
                <div className="reviews-container">
                    {dummyReviews.map((review) => (
                        <div key={review.id} className="review-card">
                            <p className="review-text">"{review.tekst}"</p>
                            <p className="review-name">- {review.naam}</p>
                            <p className="review-rating">
                                {"⭐".repeat(review.sterren)}{" "}
                                {"☆".repeat(5 - review.sterren)}
                            </p>
                        </div>
                    ))}
                </div>
            </div>
        </div>
    );
}


export default Index;