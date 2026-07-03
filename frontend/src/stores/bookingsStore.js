import { create } from 'zustand';

export const useBookingsStore = create((set) => ({
  bookings: [],
  loading: false,
  error: null,

  setBookings: (bookings) => set({ bookings, error: null }),
  addBooking: (booking) =>
    set((state) => ({ bookings: [...state.bookings, booking] })),
  removeBooking: (id) =>
    set((state) => ({
      bookings: state.bookings.filter((b) => b.id !== id)
    })),
  setLoading: (loading) => set({ loading }),
  setError: (error) => set({ error })
}));
