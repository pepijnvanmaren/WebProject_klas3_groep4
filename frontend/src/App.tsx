import { BrowserRouter, Routes, Route } from "react-router-dom";
import IndexPage from "./pages/IndexPage.js";
import LoginPage from "./pages/LoginPage.js"; 
import RegisterPage from "./pages/RegisterPage.js";
import SellerDashboardPage from "./pages/SellerDashboardPage.js";
import AccountInfoPage from "./pages/AccountInfoPage.js";
import ProductToneneDashboardPage from "./pages/ProductToneneDashboard.js";
import ProductPlaatsenDashboardPage from "./pages/ProductPlaatsenDashboard.js";
import PrivacyPage from './pages/PrivacyPage.js';


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