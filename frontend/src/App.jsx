import React, { useEffect } from 'react';
import { BrowserRouter, Routes, Route } from 'react-router-dom';
import { useAuthStore } from './stores/authStore';
import Header from './components/Header';
import LoginPage from './pages/LoginPage';
import RegisterPage from './pages/RegisterPage';
import HomePage from './pages/HomePage';
import './index.css';

function App() {
  const { restoreSession } = useAuthStore();

  useEffect(() => {
    restoreSession();
  }, []);

  return (
    <BrowserRouter>
      <Header />
      <Routes>
        <Route path="/login" element={<LoginPage />} />
        <Route path="/register" element={<RegisterPage />} />
        <Route path="/" element={<HomePage />} />
      </Routes>
    </BrowserRouter>
  );
}

export default App;
