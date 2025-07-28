import React, { useState } from 'react';
import axios from 'axios';
import { API_BASE_URL } from '../config/Api';
import { useNavigate } from 'react-router-dom';

const UserLoginPage = () => {
    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState('');
    const navigate = useNavigate();

    const handleSubmit = async (e) => {
        e.preventDefault();
        setLoading(true);
        setError('');
        try {
            const response = await axios.post(`${API_BASE_URL}/auth/login`, {
                username,
                password
            });
            // JWT token response.data.token
            localStorage.setItem('token', response.data.token);
            navigate('/todos');
        } catch (err) {
            setError('Login failed. Please check your credentials.');
        } finally {
            setLoading(false);
        }
    };

    const handleRegisterClick = () => {
        navigate('/register'); // register sayfasına yönlendiriyoruz
    }

    return (
        <div className="LoginWrapper">
            <h2 className="login-title">Login</h2>
            <form className="LoginForm" onSubmit={handleSubmit}>
                <input
                    className="login-input"
                    type="text"
                    placeholder="Username or Email"
                    value={username}
                    onChange={(e) => setUsername(e.target.value)}
                    required
                    disabled={loading}
                />
                <input
                    className="login-input"
                    type="password"
                    placeholder="Password"
                    value={password}
                    onChange={(e) => setPassword(e.target.value)}
                    required
                    disabled={loading}
                />
                <button className="login-btn" type="submit" disabled={loading}>
                    {loading ? 'Logging in...' : 'Login'}
                </button>
                {error && <div className="login-error">{error}</div>}
            </form>
            <div className='login-register-link'>
                <span>Don't have an account</span>
                <button className='register-link-btn' onClick={handleRegisterClick}>Register</button>
            </div>
        </div>
    );
};

export default UserLoginPage;