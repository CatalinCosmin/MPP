// src/pages/LoginPage.tsx

import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { AuthService } from '../services/AuthService';
import { useAuth } from '../pages/AuthContext'; // import context
import './LoginPage.css';

export const LoginPage = () => {
  const { login } = useAuth(); // use context for login
  const navigate = useNavigate();
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');

  useEffect(() => {
    const token = localStorage.getItem("jwt");
      console.log(token)
      if (token) {
        login(token);
        navigate('/home');
      }
  }, [])

  const handleLogin = async () => {
    try {
      const token = await AuthService.login(username, password);
      console.log(token)
      if (token) {
        login(token);
        navigate('/home');
      } else {
        setError('Invalid credentials');
      }
    } catch (err) {
      setError('Login failed');
    }
  };

  return (
    <div className="login-container">
      <h2>Login</h2>
      <input
        type="text"
        placeholder="Username"
        value={username}
        onChange={(e) => setUsername(e.target.value)}
      />
      <input
        type="password"
        placeholder="Password"
        value={password}
        onChange={(e) => setPassword(e.target.value)}
      />
      <button onClick={handleLogin}>Login</button>
      {error && <p>{error}</p>}
    </div>
  );
};
