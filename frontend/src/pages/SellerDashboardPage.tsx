import React from "react";
import Navbar from "../components/Navbar.jsx";
import SellerDashboard from "../components/AanvoerderDashboard.js";
import Footer from "../components/Footer.jsx";

export default function SellerDashboardPage() {
    return (
        <>
            <Navbar />
            <SellerDashboard />
            <Footer />
        </>
    );
}
