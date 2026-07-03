import React from 'react';
import { useAuthStore } from '../stores/authStore';
import { useNavigate } from 'react-router-dom';

export default function Header() {
  const { user, logout } = useAuthStore();
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  return (
    <header className="bg-blue-600 text-white shadow">
      <div className="max-w-7xl mx-auto px-4 py-4 flex justify-between items-center">
        <h1 className="text-2xl font-bold cursor-pointer" onClick={() => navigate('/')}>
          💪 Gym Booking
        </h1>
        {user ? (
          <div className="flex items-center gap-4">
            <span className="text-sm">{user.username} ({user.role})</span>
            <button
              onClick={handleLogout}
              className="bg-red-600 hover:bg-red-700 px-4 py-2 rounded-lg transition"
            >
              Logout
            </button>
          </div>
        ) : (
          <div className="flex gap-2">
            <button
              onClick={() => navigate('/login')}
              className="hover:bg-blue-700 px-4 py-2 rounded transition"
            >
              Login
            </button>
            <button
              onClick={() => navigate('/register')}
              className="bg-green-600 hover:bg-green-700 px-4 py-2 rounded transition"
            >
              Register
            </button>
          </div>
        )}
      </div>
    </header>
  );
}
