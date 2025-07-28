import React, { useState } from 'react';
import axios from 'axios';
import { API_BASE_URL } from '../config/Api';
import { useNavigate } from 'react-router-dom';

const RegisterPage = () => {
    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState('');
    const [success, setSuccess] = useState(false);
    const navigate = useNavigate();

    const handleSubmit = async (e) => {
        e.preventDefault();
        setLoading(true);
        setError('');
        try {
            await axios.post(`${API_BASE_URL}/auth/register`, {
                username,
                password
            });
            setSuccess(true);
            setTimeout(() => {
                navigate('/'); // Redirect to login page after successful registration
            }, 2000); // Simulate a delay for better UX
        } catch (err) {
            setError('Registration failed. Please check your information.');
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="RegisterWrapper">
            {success && (
                <div className='register-success-box'>
                    Registration successful! Redirecting to login...
                </div>
            )}
            <h2 className="register-title">Register</h2>
            <form className="RegisterForm" onSubmit={handleSubmit}>
                <input
                    className="register-input"
                    type="text"
                    placeholder="Username"
                    value={username}
                    onChange={(e) => setUsername(e.target.value)}
                    required
                    disabled={loading}
                />
                <input
                    className="register-input"
                    type="password"
                    placeholder="Password"
                    value={password}
                    onChange={(e) => setPassword(e.target.value)}
                    required
                    disabled={loading}
                />
                <button className="register-btn" type="submit" disabled={loading}>
                    {loading ? 'Registering...' : 'Register'}
                </button>
                {error && <div className="register-error">{error}</div>}
            </form>
        </div>
    );
};

export default RegisterPage;