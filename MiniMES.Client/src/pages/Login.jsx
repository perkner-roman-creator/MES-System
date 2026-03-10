import { useState } from 'react';
import { authAPI } from '../api';

function Login({ onLogin }) {
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    setLoading(true);

    try {
      const response = await authAPI.login({ username, password });
      const { token, username: user, email, role, fullName } = response.data;
      
      onLogin(token, { username: user, email, role, fullName });
    } catch (err) {
      setError(err.response?.data?.message || 'Přihlášení selhalo');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="container" style={{ maxWidth: '400px', marginTop: '5rem' }}>
      <div className="card">
        <div className="card-header">
          <h1 style={{ textAlign: 'center' }}>Mini-MES System</h1>
          <p style={{ textAlign: 'center', color: 'var(--text-secondary)' }}>
            Přihlaste se do systému
          </p>
        </div>

        {error && (
          <div className="error-message">
            {error}
          </div>
        )}

        <form onSubmit={handleSubmit}>
          <div className="form-group">
            <label className="form-label">Uživatelské jméno</label>
            <input
              type="text"
              className="form-input"
              value={username}
              onChange={(e) => setUsername(e.target.value)}
              required
              disabled={loading}
            />
          </div>

          <div className="form-group">
            <label className="form-label">Heslo</label>
            <input
              type="password"
              className="form-input"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              required
              disabled={loading}
            />
          </div>

          <button 
            type="submit" 
            className="btn btn-primary" 
            style={{ width: '100%' }}
            disabled={loading}
          >
            {loading ? 'Přihlašování...' : 'Přihlásit se'}
          </button>
        </form>

        <div style={{ marginTop: '1.5rem', padding: '1rem', background: 'var(--bg-primary)', borderRadius: '0.375rem' }}>
          <p style={{ fontSize: '0.875rem', marginBottom: '0.5rem' }}><strong>Testovací účty:</strong></p>
          <p style={{ fontSize: '0.75rem', marginBottom: '0.25rem' }}>Admin: admin / admin123</p>
          <p style={{ fontSize: '0.75rem', marginBottom: '0.25rem' }}>Manager: manager / manager123</p>
          <p style={{ fontSize: '0.75rem' }}>Operátor: operator / operator123</p>
        </div>
      </div>
    </div>
  );
}

export default Login;
