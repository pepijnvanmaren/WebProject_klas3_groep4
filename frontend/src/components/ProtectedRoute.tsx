import React from "react";
import { Navigate } from "react-router-dom";

type ProtectedRouteProps = {
    children: React.ReactNode;
    requiredRoles: string[];
};

function ProtectedRoute({ children, requiredRoles }: ProtectedRouteProps) {
    const isLoggedIn = localStorage.getItem("loggedIn") === "true";
    const userRole = localStorage.getItem("userRole");



    // Niet ingelogd? Ga naar login
    if (!isLoggedIn) {
        return <Navigate to="/inloggen" replace />;
    }

    // Ingelogd maar verkeerde rol? Toon access denied
    if (!userRole || !requiredRoles.includes(userRole)) {
        return (
            <div style={{
                padding: "40px 20px",
                textAlign: "center",
                minHeight: "100vh",
                display: "flex",
                flexDirection: "column",
                justifyContent: "center",
                alignItems: "center",
                backgroundColor: "#f5f5f5"
            }}>
                <h1> Toegang geweigerd</h1>
                <p style={{ fontSize: "16px", color: "#666", marginBottom: "20px" }}>
                    Je hebt geen toestemming om deze pagina te bekijken.
                </p>
                <p style={{ fontSize: "14px", color: "#999", marginBottom: "20px" }}>
                    Jouw rol: <strong>{userRole || "Onbekend"}</strong>
                </p>
                <a href="/" style={{
                    display: "inline-block",
                    padding: "10px 20px",
                    backgroundColor: "#007bff",
                    color: "white",
                    textDecoration: "none",
                    borderRadius: "5px",
                    cursor: "pointer"
                }}>
                    Terug naar home
                </a>
            </div>
        );
    }

    // Alles ok, toon de pagina
    return children;
}

export default ProtectedRoute;