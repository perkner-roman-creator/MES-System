import { Link, useLocation } from 'react-router-dom';
import { Home, ClipboardList, Package, Users, FileText, LogOut } from 'lucide-react';

function Navbar({ user, onLogout }) {
  const location = useLocation();

  const isActive = (path) => location.pathname === path ? 'active' : '';

  return (
    <nav className="navbar">
      <Link to="/" className="navbar-brand">
        MES System
      </Link>
      
      <ul className="navbar-nav">
        <li>
          <Link to="/" className={`nav-link ${isActive('/')}`}>
            <Home size={18} />
            Přehled
          </Link>
        </li>
        <li>
          <Link to="/workorders" className={`nav-link ${isActive('/workorders')}`}>
            <ClipboardList size={18} />
            Zakázky
          </Link>
        </li>
        <li>
          <Link to="/machines" className={`nav-link ${isActive('/machines')}`}>
            <Package size={18} />
            Stroje
          </Link>
        </li>
        <li>
          <Link to="/employees" className={`nav-link ${isActive('/employees')}`}>
            <Users size={18} />
            Pracovníci
          </Link>
        </li>
        <li>
          <Link to="/logs" className={`nav-link ${isActive('/logs')}`}>
            <FileText size={18} />
            Logy
          </Link>
        </li>
        <li>
          <span className="nav-link">
            {user?.fullName} ({user?.role})
          </span>
        </li>
        <li>
          <button onClick={onLogout} className="btn btn-secondary btn-sm">
            <LogOut size={16} />
            Odhlásit
          </button>
        </li>
      </ul>
    </nav>
  );
}

export default Navbar;
