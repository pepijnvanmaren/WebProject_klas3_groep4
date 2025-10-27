import { BrowserRouter, Routes, Route } from "react-router-dom";
import IndexPage from "./pages/IndexPage";
import LoginPage from "./pages/LoginPage";
import RegisterPage from "./pages/RegisterPage";

function App() {
    return (
        <BrowserRouter>
            <Routes>
                <Route path="/" element={<IndexPage />} />
                <Route path="/registreren" element={<RegisterPage />} />
                <Route path="/login" element={<LoginPage />} />
                <Route path="/register" element={<RegisterPage />} />
            </Routes>
        </BrowserRouter>
    );
}

export default App;

//cd frotnend
//npm install
//npm run dev