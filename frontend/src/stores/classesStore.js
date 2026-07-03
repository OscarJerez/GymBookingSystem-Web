import { create } from 'zustand';

export const useClassesStore = create((set) => ({
  classes: [],
  loading: false,
  error: null,

  setClasses: (classes) => set({ classes, error: null }),
  setLoading: (loading) => set({ loading }),
  setError: (error) => set({ error })
}));
