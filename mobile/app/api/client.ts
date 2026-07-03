import axios from 'axios';
import AsyncStorage from '@react-native-async-storage/async-storage';

const API_BASE = process.env.EXPO_PUBLIC_API_URL || 'http://localhost:5000/api';

const api = axios.create({
  baseURL: API_BASE,
  timeout: 30000,
});

api.interceptors.request.use(async (config) => {
  const token = await AsyncStorage.getItem('auth_token');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

api.interceptors.response.use(
  (response) => response,
  async (error) => {
    if (error.response?.status === 401) {
      await AsyncStorage.removeItem('auth_token');
      await AsyncStorage.removeItem('user');
    }
    return Promise.reject(error);
  }
);

export const authAPI = {
  register: (username: string, email: string, password: string) =>
    api.post('/auth/register', { username, email, password }),
  login: (username: string, password: string) =>
    api.post('/auth/login', { username, password }),
};

export const classesAPI = {
  list: () => api.get('/classes'),
  get: (id: number) => api.get(`/classes/${id}`),
  book: (classId: number) => api.post('/bookings', { classId }),
};

export const bookingsAPI = {
  myBookings: () => api.get('/bookings'),
  cancel: (id: number) => api.delete(`/bookings/${id}`),
};

export const waitlistAPI = {
  join: (classId: number) => api.post('/waitlist', { classId }),
  myWaitlist: () => api.get('/waitlist/my-waitlist'),
  remove: (id: number) => api.delete(`/waitlist/${id}`),
};

export const paymentsAPI = {
  plans: () => api.get('/payments/membership-plans'),
  createMembership: (type: string) => api.post('/payments/memberships', { type }),
  getActive: () => api.get('/payments/active-membership'),
  process: (amount: number, method: string) =>
    api.post('/payments/process', { amount, method }),
  history: () => api.get('/payments/history'),
};

export default api;
