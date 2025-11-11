import { useState, useEffect, useRef } from "react";
import "../styles/index.css";
import Roses from "../assets/Rozen.png";

function Index() {
    const [price, setPrice] = useState(30.0);
    const [isRunning, setIsRunning] = useState(true);
    const [purchased, setPurchased] = useState(false); // new state
    const intervalRef = useRef(null);

    const minPrice = 5.0;
    const maxPrice = 30.0;

    const progress = (price - minPrice) / (maxPrice - minPrice);
    const barColor = `rgb(${Math.round(255 * (1 - progress))}, ${Math.round(255 * progress)}, 0)`;

    useEffect(() => {
        if (isRunning) {
            intervalRef.current = setInterval(() => {
                setPrice((prevPrice) => {
                    if (prevPrice <= minPrice) {
                        clearInterval(intervalRef.current);
                        return minPrice;
                    }
                    return parseFloat((prevPrice - 0.1).toFixed(2));
                });
            }, 1000);
        }

        return () => clearInterval(intervalRef.current);
    }, [isRunning]);

    const handleStop = () => {
        clearInterval(intervalRef.current);
        setIsRunning(false);
        setPurchased(true); // mark as purchased
    };

    return (
        <div className="page">
            <h1 className="page-title">Huidig product</h1>

            <div className="container">
                <div className="box">
                    <img src={Roses} alt="Roses" className="Roses" />
                </div>

                <div className="box box-description">
                    <div>
                        <h2 className="product-name">Rozen</h2>
                        <p className="description">
                            Lorem ipsum dolor sit amet, consectetur adipiscing elit.
                            Vivamus id nulla vitae urna elementum commodo.
                        </p>
                    </div>
                </div>

                <div className="box">Go Roos Yourself B.V.</div>
                <div className="box">500 stuks</div>

                <div className="box box-price">
                    <div className="price-row">
                        <span className="price">EUR {price.toFixed(2)}</span>
                        <button className="button" onClick={handleStop}>
                            {purchased ? "Gekocht" : "Koop"}
                        </button>
                    </div>
                </div>

                <div className="progress-bar-container integrated-bar">
                    <div
                        className="progress-bar"
                        style={{
                            width: `${progress * 100}%`,
                            backgroundColor: barColor,
                            transition: "width 1s linear, background-color 1s linear",
                        }}
                    ></div>
                </div>
            </div>

            <h1 className="page-title">Volgend product</h1>
            <div className="container">
                <div className="box">
                    <img src={Roses} alt="Roses" className="Roses" />
                </div>

                <div className="box box-description">
                    <div>
                        <h2 className="product-name">Rozen</h2>
                        <p className="description">
                            Lorem ipsum dolor sit amet, consectetur adipiscing elit.
                            Vivamus id nulla vitae urna elementum commodo.
                        </p>
                    </div>
                </div>
                <div className="box">Go Roos Yourself B.V.</div>
                <div className="box">500 stuks</div>
                <div className="box"></div>
            </div>
        </div>
    );
}

export default Index;
