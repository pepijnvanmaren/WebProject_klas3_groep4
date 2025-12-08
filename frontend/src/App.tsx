import { BrowserRouter, Routes, Route } from "react-router-dom";
import ProtectedRoute from "./components/ProtectedRoute";
import IndexPage from "./pages/IndexPage.jsx";
import LoginPage from "./pages/LoginPage.jsx";
import RegisterPage from "./pages/RegisterPage.jsx";
import SellerDashboardPage from "./pages/SellerDashboardPage.jsx";
import AccountInfoPage from "./pages/AccountInfoPage.jsx";
import ProductToneneDashboardPage from "./pages/ProductToneneDashboard.jsx";
import ProductPlaatsenDashboardPage from "./pages/ProductPlaatsenDashboard.jsx";
import PrivacyPage from "./pages/PrivacyPage.jsx";
import BuyerDashboardPage from "./pages/BuyerDashboardPage.js";
import VeilingMeesterDashboardPage from "./pages/VeilingMeesterDashboardPage.jsx";
import VeilingTonenPage from "./pages/VeilingTonenPage.jsx";
import VeilingPlaatsenPage from "./pages/VeilingPlaatsenPage.jsx";
function App() {
    return (
        <BrowserRouter>
            <Routes>
                {/* Publieke routes */}
                <Route path="/registreren" element={<RegisterPage />} />
                <Route path="/inloggen" element={<LoginPage />} />
                <Route path="/privacy" element={<PrivacyPage />} />

                {/* Home pagina - voor iedereen toegankelijk */}
                <Route path="/" element={<IndexPage />} />

                {/* Koper routes */}
                <Route
                    path="/koperdashboard"
                    element={
                        <ProtectedRoute requiredRoles={["Koper"]}>
                            <BuyerDashboardPage />
                        </ProtectedRoute>
                    }
                />

                {/* Aanvoerder (Verkoper) routes */}
                <Route
                    path="/VerkoperDashboard"
                    element={
                        <ProtectedRoute requiredRoles={["Aanvoerder"]}>
                            <SellerDashboardPage />
                        </ProtectedRoute>
                    }
                />
                <Route
                    path="/ProductDashboard"
                    element={
                        <ProtectedRoute requiredRoles={["Aanvoerder"]}>
                            <ProductToneneDashboardPage />
                        </ProtectedRoute>
                    }
                />
                <Route
                    path="/ProductMakenDashboard"
                    element={
                        <ProtectedRoute requiredRoles={["Aanvoerder"]}>
                            <ProductPlaatsenDashboardPage />
                        </ProtectedRoute>
                    }
                />

                {/* Account info - voor ingelogde gebruikers */}
                <Route
                    path="/AccountInfo"
                    element={
                        <ProtectedRoute requiredRoles={["Koper", "Aanvoerder", "Veilingmeester", "Admin"]}>
                            <AccountInfoPage />
                        </ProtectedRoute>
                    }
                />

                <Route
                    path="/VeilingMeesterDashboard"
                    element={
                    <ProtectedRoute requiredRoles={["Veilingmeester"]}>
                        <VeilingMeesterDashboardPage />
                </ProtectedRoute>
                    }                
                />
                <Route
                    path="/VeilingPlaatsen"
                    element={
                        <ProtectedRoute requiredRoles={["Veilingmeester"]}>
                            <VeilingPlaatsenPage />
                        </ProtectedRoute>
                    }
                />
                <Route
                    path="/VeilingTonen"
                    element={
                        <ProtectedRoute requiredRoles={["Veilingmeester"]}>
                            <VeilingTonenPage />
                        </ProtectedRoute>
                    }
                />
            </Routes>
        </BrowserRouter>
    );
}

export default App;