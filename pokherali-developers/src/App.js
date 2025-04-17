import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import React from 'react';

import Cart from './components/Cart';
import BookDetail from './components/BookDetail';
import MemberDashboard from './components/MemberDashboard';
import AdminDashboard from './components/AdminDashboard';
import Header from './components/Header';
import HomePage from './pages/HomePage';
import Login from './components/Login';
import Register from './components/Register';

export default function App() {
  return (
    <Router>
      <Header />
      <Routes>
        <Route path="/" element={<HomePage />} />
        <Route path="/book/:id" element={<BookDetail />} />
        <Route path="/cart" element={<Cart />} />
        <Route path="/dashboard" element={<MemberDashboard />} />
        <Route path="/admin" element={<AdminDashboard />} />
        <Route path="/login" element={<Login />} />
        <Route path="/register" element={<Register />} />
      </Routes>
    </Router>
  );
}
