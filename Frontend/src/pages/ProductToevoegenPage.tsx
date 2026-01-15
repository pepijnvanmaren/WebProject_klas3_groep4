import React from "react";
import Navbar from "../components/Navbar.jsx";
import ProductToevoegen from "../components/ProductToevoegen.jsx";
import Footer from "../components/Footer.jsx";

export default function ProductToevoegenPage() {
    return (
        <>
            <Navbar />
            <ProductToevoegen />
            <Footer />
        </>
    );
}