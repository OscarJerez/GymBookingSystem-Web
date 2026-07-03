import React from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuthStore } from '../stores/authStore';

export default function ProtectedRoute({ children }) {
  const { user } = useAuthStore();
  const navigate = useNavigate();

  React.useEffect(() => {
    if (!user) {
      navigate('/login');
    }
  }, [user, navigate]);

  return user ? children : null;
}
