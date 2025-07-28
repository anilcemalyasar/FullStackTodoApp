import { BrowserRouter, Routes, Route, useLocation } from "react-router-dom";
import TodoWrapper from "./components/TodoWrapper";
import "./App.css";
import TodoDetail from "./components/TodoDetail";
import UserLoginPage from "./components/UserLoginPage";
import RegisterPage from "./components/RegisterPage";
import ProtectedRoute from "./components/ProtectedRoute";
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faSignOut } from '@fortawesome/free-solid-svg-icons';

function AppContent() {
    const location = useLocation(); // Hook'u burada çağır
    const token = localStorage.getItem("token");

    const handleSignOut = () => {
        localStorage.removeItem("token");
        window.location.href = '/'; // Redirect to login page after sign out
    };

    // Sadece korumalı sayfalarda göster
    const showSignOut = token && location.pathname.startsWith("/todos");

    return (
        <div className="App">
            {showSignOut && (
                <div className="signout-btn-wrapper">
                    <span
                        className="signout-btn"
                        title="Sign Out"
                        onClick={handleSignOut}
                    >
                        <FontAwesomeIcon icon={faSignOut} /> Sign Out
                    </span>
                </div>
            )}
            <Routes>
                <Route path="/" element={<UserLoginPage />} />
                <Route path="/register" element={<RegisterPage />} />
                <Route element={<ProtectedRoute />}> 
                    <Route path="/todos" element={<TodoWrapper />} />
                    <Route path="/todos/:id" element={<TodoDetail />} />
                </Route>
            </Routes>
        </div>
    );
}

// Ana App bileşeni BrowserRouter'ı sarmalar
function App() {
    return (
        <BrowserRouter>
            <AppContent />
        </BrowserRouter>
    );
}

export default App;