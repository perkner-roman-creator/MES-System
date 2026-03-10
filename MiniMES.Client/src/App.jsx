import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { useState, useEffect } from 'react';
import Navbar from './components/Navbar';
import Login from './pages/Login';
import Dashboard from './pages/Dashboard';
import WorkOrders from './pages/WorkOrders';
import Machines from './pages/Machines';
import Employees from './pages/Employees';
import ProductionLogs from './pages/ProductionLogs';

function App() {
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const token = localStorage.getItem('token');
    const userData = localStorage.getItem('user');
    
    if (token && userData) {
      setIsAuthenticated(true);
      setUser(JSON.parse(userData));
    }
    setLoading(false);
  }, []);

  const handleLogin = (token, userData) => {
    localStorage.setItem('token', token);
    localStorage.setItem('user', JSON.stringify(userData));
    setIsAuthenticated(true);
    setUser(userData);
  };

  const handleLogout = () => {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    setIsAuthenticated(false);
    setUser(null);
  };

  if (loading) {
    return (
      <div className="loading">
        <div className="spinner"></div>
      </div>
    );
  }

  return (
    <BrowserRouter>
      <div className="app">
        {isAuthenticated && <Navbar user={user} onLogout={handleLogout} />}
        
        <Routes>
          <Route 
            path="/login" 
            element={
              isAuthenticated ? 
                <Navigate to="/" /> : 
                <Login onLogin={handleLogin} />
            } 
          />
          
          <Route 
            path="/" 
            element={
              isAuthenticated ? 
                <Dashboard user={user} /> : 
                <Navigate to="/login" />
            } 
          />
          
          <Route 
            path="/workorders" 
            element={
              isAuthenticated ? 
                <WorkOrders user={user} /> : 
                <Navigate to="/login" />
            } 
          />
          
          <Route 
            path="/machines" 
            element={
              isAuthenticated ? 
                <Machines user={user} /> : 
                <Navigate to="/login" />
            } 
          />
          
          <Route 
            path="/employees" 
            element={
              isAuthenticated ? 
                <Employees user={user} /> : 
                <Navigate to="/login" />
            } 
          />
          
          <Route 
            path="/logs" 
            element={
              isAuthenticated ? 
                <ProductionLogs /> : 
                <Navigate to="/login" />
            } 
          />
        </Routes>
      </div>
    </BrowserRouter>
  );
}

export default App;
