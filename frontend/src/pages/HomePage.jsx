import React, { useEffect, useState } from 'react';
import { useAuthStore } from '../stores/authStore';
import { useClassesStore } from '../stores/classesStore';
import { useBookingsStore } from '../stores/bookingsStore';
import { classesAPI, bookingsAPI } from '../api/client';
import { format } from 'date-fns';

export default function HomePage() {
  const { user } = useAuthStore();
  const { classes, setClasses, setLoading: setClassesLoading } = useClassesStore();
  const { bookings, addBooking, removeBooking, setBookings } = useBookingsStore();
  const [error, setError] = useState('');
  const [successMsg, setSuccessMsg] = useState('');

  useEffect(() => {
    loadClasses();
    if (user) loadMyBookings();
  }, [user]);

  const loadClasses = async () => {
    try {
      setClassesLoading(true);
      const response = await classesAPI.getAll();
      setClasses(response.data);
    } catch (err) {
      setError('Failed to load classes');
    } finally {
      setClassesLoading(false);
    }
  };

  const loadMyBookings = async () => {
    try {
      const response = await bookingsAPI.getMyBookings();
      setBookings(response.data);
    } catch (err) {
      console.log('No bookings yet');
    }
  };

  const handleBook = async (classId) => {
    try {
      setError('');
      setSuccessMsg('');
      const response = await bookingsAPI.create(classId);
      addBooking(response.data);
      setSuccessMsg('Booked successfully!');
      setTimeout(() => setSuccessMsg(''), 3000);
      loadClasses(); // Refresh class list
    } catch (err) {
      setError(err.response?.data || 'Booking failed');
    }
  };

  const handleCancel = async (bookingId) => {
    try {
      setError('');
      await bookingsAPI.cancel(bookingId);
      removeBooking(bookingId);
      setSuccessMsg('Booking cancelled');
      setTimeout(() => setSuccessMsg(''), 3000);
      loadClasses();
    } catch (err) {
      setError('Failed to cancel booking');
    }
  };

  if (!user) {
    return (
      <div className="text-center py-12">
        <p className="text-gray-600">Please login to book classes</p>
      </div>
    );
  }

  const bookedClassIds = new Set(bookings.map(b => b.classId));

  return (
    <div className="max-w-7xl mx-auto px-4 py-8">
      {error && <div className="mb-4 p-4 bg-red-100 text-red-700 rounded">{error}</div>}
      {successMsg && <div className="mb-4 p-4 bg-green-100 text-green-700 rounded">{successMsg}</div>}

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
        {/* Classes */}
        <div className="lg:col-span-2">
          <h2 className="text-2xl font-bold mb-6">Available Classes</h2>
          <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
            {classes.map((cls) => (
              <div key={cls.id} className="card">
                <h3 className="text-xl font-bold mb-2">{cls.name}</h3>
                <p className="text-gray-600 mb-4">{cls.description}</p>
                <p className="text-sm mb-2">
                  <strong>Instructor:</strong> {cls.instructorName}
                </p>
                <p className="text-sm mb-2">
                  <strong>Time:</strong> {cls.timeRange}
                </p>
                <p className={`text-sm font-semibold mb-4 ${cls.availableSpots === 0 ? 'text-red-600' : 'text-green-600'}`}>
                  {cls.status}
                </p>
                {bookedClassIds.has(cls.id) ? (
                  <button
                    onClick={() => {
                      const booking = bookings.find(b => b.classId === cls.id);
                      if (booking) handleCancel(booking.id);
                    }}
                    className="btn-danger w-full"
                  >
                    Cancel Booking
                  </button>
                ) : (
                  <button
                    onClick={() => handleBook(cls.id)}
                    disabled={cls.availableSpots === 0}
                    className={`w-full ${cls.availableSpots === 0 ? 'btn-secondary' : 'btn-primary'}`}
                  >
                    {cls.availableSpots === 0 ? 'Class Full' : 'Book Now'}
                  </button>
                )}
              </div>
            ))}
          </div>
        </div>

        {/* Bookings */}
        <div>
          <h2 className="text-2xl font-bold mb-6">My Bookings</h2>
          <div className="card">
            {bookings.length === 0 ? (
              <p className="text-gray-600">No bookings yet</p>
            ) : (
              <ul className="space-y-4">
                {bookings.map((booking) => (
                  <li key={booking.id} className="pb-4 border-b last:border-b-0">
                    <p className="font-semibold">{booking.className}</p>
                    <p className="text-sm text-gray-600">
                      {format(new Date(booking.bookedAt), 'MMM dd, yyyy HH:mm')}
                    </p>
                    <span className="text-xs bg-blue-100 text-blue-800 px-2 py-1 rounded mt-2 inline-block">
                      {booking.status}
                    </span>
                  </li>
                ))}
              </ul>
            )}
          </div>
        </div>
      </div>
    </div>
  );
}
