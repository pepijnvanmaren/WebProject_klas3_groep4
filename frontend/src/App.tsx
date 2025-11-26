import { BrowserRouter, Routes, Route } from "react-router-dom";
import IndexPage from "./pages/IndexPage.jsx";
import LoginPage from "./pages/LoginPage.jsx";
import RegisterPage from "./pages/RegisterPage.jsx";
import SellerDashboardPage from "./pages/SellerDashboardPage.jsx";
import AccountInfoPage from "./pages/AccountInfoPage.jsx";
import ProductToneneDashboardPage from "./pages/ProductToneneDashboard.jsx";
import ProductPlaatsenDashboardPage from "./pages/ProductPlaatsenDashboard.jsx";
import PrivacyPage from "./pages/PrivacyPage.jsx";


function App() {
    return (
        <BrowserRouter>
            <Routes>
                <Route path="/ProductDashboard" element={<ProductToneneDashboardPage />} />
                <Route path="/ProductMakenDashboard" element={<ProductPlaatsenDashboardPage />} />
                <Route path="/registreren" element={<RegisterPage />} />
                <Route path="/inloggen" element={<LoginPage />} />
                <Route path="/VerkoperDashboard" element={<SellerDashboardPage />} />
                <Route path="/" element={<IndexPage />} />
                <Route path="/AccountInfo" element={<AccountInfoPage />} />
                <Route path="/privacy" element={<PrivacyPage />} />
            </Routes>
        </BrowserRouter>
    );
}

export default App;

//cd frontend
//npm install
//npm run dev   