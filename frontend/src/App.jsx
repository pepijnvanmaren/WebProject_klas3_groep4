import { BrowserRouter, Routes, Route } from "react-router-dom";
import IndexPage from "./pages/IndexPage";
import LoginPage from "./pages/LoginPage";
import RegisterPage from "./pages/RegisterPage";
import SellerDashboardPage from "./pages/SellerDashboardPage";
import BuyerDashboardPage from "./pages/BuyerDashboardPage";

function App() {
    return (
        <BrowserRouter>
            <Routes>
                <Route path="/registreren" element={<RegisterPage />} />
                <Route path="/" element={<LoginPage />} />
                <Route path="/VerkoperDashboard" element={<SellerDashboardPage />} />
                <Route path="/KoperDashboard" element={<BuyerDashboardPage />} />
            </Routes>
        </BrowserRouter>
    );
}

export default App;

//cd frontend
//npm install
//npm run dev