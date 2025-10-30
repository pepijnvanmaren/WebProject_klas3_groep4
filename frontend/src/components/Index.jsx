import "../styles/index.css";

function Index() {
    return (
        <div className="main_div">

            <div className="content_div">
                <div className="content_div_title">Huidig Product</div>

                <div className="content_layout">
                    <div className="left_image_div"></div>

                    <div className="right_text_div">
                        <p>Lorem ipsum dolor sit amet. Eos voluptatibus corrupti et odio accusantium et praesentium dolore. In laudantium expedita et voluptas illo ea nisi dicta in nemo architecto cum temporibus quasi. A beatae facilis eum quibusdam odio nam eius deleniti ut consequuntur sint ex possimus numquam.</p>
                    </div>
                </div>

                <div className="bottom_text_div">
                    <div className="price_buy_container">
                        <p>Huidige prijs: 200</p>
                        <button className="buy_button">Koop Nu</button>
                    </div>
                </div>
            </div>

            <div className="content_div">
                <div className="content_div_title">Volgend Product</div>

                <div className="content_layout">
                    <div className="left_image_div"></div>

                    <div className="right_text_div">
                        <p>Lorem ipsum dolor sit amet. Eos voluptatibus corrupti et odio accusantium et praesentium dolore. In laudantium expedita et voluptas illo ea nisi dicta in nemo architecto cum temporibus quasi. A beatae facilis eum quibusdam odio nam eius deleniti ut consequuntur sint ex possimus numquam.</p>
                    </div>
                </div>
            </div>

        </div>
    );
}

export default Index;
