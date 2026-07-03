import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuthStore } from '../stores/authStore';
import { authAPI } from '../api/client';

export default function RegisterPage() {
  const [form, setForm] = useState({ username: '', email: '', password: '', passwordConfirm: '' });
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);
  const { setUser } = useAuthStore();
  const navigate = useNavigate();

  const handleRegister = async (e) => {
    e.preventDefault();
    setError('');

    if (form.password !== form.passwordConfirm) {
      setError('Passwords do not match');
      return;
    }

    setLoading(true);
    try {
      const response = await authAPI.register(form.username, form.email, form.password);
      const { id, username: user, email, role, token } = response.data;
      setUser({ id, username: user, email, role }, token);
      navigate('/');
    } catch (err) {
      setError(err.response?.data || 'Registration failed');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-gray-100">
      <div className="card w-full max-w-md">
        <h2 className="text-2xl font-bold mb-6 text-center">Register</h2>
        {error && <div className="mb-4 p-3 bg-red-100 text-red-700 rounded">{error}</div>}
        <form onSubmit={handleRegister} className="space-y-4">
          <div>
            <label className="block text-sm font-medium mb-1">Username</label>
            <input
              type="text"
              className="input-field"
              value={form.username}
              onChange={(e) => setForm({ ...form, username: e.target.value })}
              required
            />
          </div>
          <div>
            <label className="block text-sm font-medium mb-1">Email</label>
            <input
              type="email"
              className="input-field"
              value={form.email}
              onChange={(e) => setForm({ ...form, email: e.target.value })}
              required
            />
          </div>
          <div>
            <label className="block text-sm font-medium mb-1">Password</label>
            <input
              type="password"
              className="input-field"
              value={form.password}
              onChange={(e) => setForm({ ...form, password: e.target.value })}
              required
            />
          </div>
          <div>
            <label className="block text-sm font-medium mb-1">Confirm Password</label>
            <input
              type="password"
              className="input-field"
              value={form.passwordConfirm}
              onChange={(e) => setForm({ ...form, passwordConfirm: e.target.value })}
              required
            />
          </div>
          <button
            type="submit"
            disabled={loading}
            className="btn-primary w-full"
          >
            {loading ? 'Creating account...' : 'Register'}
          </button>
        </form>
        <p className="mt-4 text-center text-sm">
          Already have an account?{' '}
          <button
            onClick={() => navigate('/login')}
            className="text-blue-600 hover:underline"
          >
            Login
          </button>
        </p>
      </div>
    </div>
  );
}
