import { create } from 'zustand';

export const useAuthStore = create((set) => ({
  user: null,
  token: null,
  loading: false,
  error: null,

  setUser: (user, token) => {
    localStorage.setItem('token', token);
    localStorage.setItem('user', JSON.stringify(user));
    set({ user, token, error: null });
  },

  logout: () => {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    set({ user: null, token: null });
  },

  restoreSession: () => {
    const token = localStorage.getItem('token');
    const user = localStorage.getItem('user');
    if (token && user) {
      set({ user: JSON.parse(user), token });
    }
  },

  setError: (error) => set({ error }),
  setLoading: (loading) => set({ loading })
}));
