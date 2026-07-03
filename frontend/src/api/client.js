import axios from 'axios';

const API_BASE = import.meta.env.VITE_API_URL || 'http://localhost:5000/api';

const api = axios.create({
  baseURL: API_BASE,
  headers: {
    'Content-Type': 'application/json'
  }
});

// Add token to requests
api.interceptors.request.use((config) => {
  const token = localStorage.getItem('token');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

export const authAPI = {
  register: (username, email, password) =>
    api.post('/auth/register', { username, email, password }),
  login: (username, password) =>
    api.post('/auth/login', { username, password })
};

export const classesAPI = {
  getAll: () => api.get('/classes'),
  getOne: (id) => api.get(`/classes/${id}`),
  create: (data) => api.post('/classes', data),
  update: (id, data) => api.put(`/classes/${id}`, data),
  delete: (id) => api.delete(`/classes/${id}`)
};

export const bookingsAPI = {
  getMyBookings: () => api.get('/bookings'),
  getAllBookings: () => api.get('/bookings/all'),
  create: (classId) => api.post('/bookings', { classId }),
  cancel: (id) => api.delete(`/bookings/${id}`)
};

export default api;
