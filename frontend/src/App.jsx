import { BrowserRouter, Routes, Route } from "react-router-dom";
import IndexPage from "./pages/IndexPage";
import LoginPage from "./pages/LoginPage";
import RegisterPage from "./pages/RegisterPage";
import SellerDashboardPage from "./pages/SellerDashboardPage";
import ProductToneneDashboardPage from "./pages/ProductToneneDashboard";
import ProductPlaatsenDashboardPage from "./pages/ProductPlaatsenDashboard";

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
            </Routes>
        </BrowserRouter>
    );
}

export default App;

//cd frontend
//npm install
//npm run dev