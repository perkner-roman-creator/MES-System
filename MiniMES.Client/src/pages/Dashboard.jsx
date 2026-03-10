import { useState, useEffect } from 'react';
import { dashboardAPI, workOrderAPI } from '../api';
import { TrendingUp, Package, Users, AlertCircle } from 'lucide-react';

function Dashboard({ user }) {
  const [stats, setStats] = useState(null);
  const [recentOrders, setRecentOrders] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadDashboard();
  }, []);

  const loadDashboard = async () => {
    try {
      const [statsRes, ordersRes] = await Promise.all([
        dashboardAPI.getStats(),
        workOrderAPI.getActive()
      ]);
      
      setStats(statsRes.data);
      setRecentOrders(ordersRes.data.slice(0, 5));
    } catch (error) {
      console.error('Error loading dashboard:', error);
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return (
      <div className="container">
        <div className="loading">
          <div className="spinner"></div>
        </div>
      </div>
    );
  }

  return (
    <div className="container">
      <h1>Přehled</h1>
      <p style={{ color: 'var(--text-secondary)', marginBottom: '2rem' }}>
        Vítejte zpět, {user?.fullName}!
      </p>

      {/* Stats Grid */}
      <div className="grid grid-4">
        <div className="stat-card">
          <div className="stat-label">Aktivní zakázky</div>
          <div className="stat-value">{stats?.totalActiveOrders || 0}</div>
          <div style={{ fontSize: '0.75rem', color: 'var(--text-secondary)', marginTop: '0.5rem' }}>
            {stats?.totalPendingOrders || 0} čekajících • {stats?.totalInProgressOrders || 0} probíhajících
          </div>
        </div>

        <div className="stat-card success">
          <div className="stat-label">Využití strojů</div>
          <div className="stat-value">{stats?.machineUtilizationRate || 0}%</div>
          <div style={{ fontSize: '0.75rem', color: 'var(--text-secondary)', marginTop: '0.5rem' }}>
            {stats?.machinesRunning || 0} běží • {stats?.machinesIdle || 0} nečinných
          </div>
        </div>

        <div className="stat-card warning">
          <div className="stat-label">Celkový postup</div>
          <div className="stat-value">{stats?.overallCompletionRate || 0}%</div>
          <div style={{ fontSize: '0.75rem', color: 'var(--text-secondary)', marginTop: '0.5rem' }}>
            {stats?.totalQuantityProduced || 0} / {stats?.totalQuantityPlanned || 0} kusů
          </div>
        </div>

        <div className="stat-card">
          <div className="stat-label">Dokončeno dnes</div>
          <div className="stat-value">{stats?.totalCompletedToday || 0}</div>
          <div style={{ fontSize: '0.75rem', color: 'var(--text-secondary)', marginTop: '0.5rem' }}>
            {stats?.productionEventsToday || 0} výrobních událostí
          </div>
        </div>
      </div>

      {/* Recent Orders */}
      <div className="card">
        <div className="card-header">Aktuální zakázky</div>
        {recentOrders.length > 0 ? (
          <div className="table-container">
            <table>
              <thead>
                <tr>
                  <th>Číslo zakázky</th>
                  <th>Produkt</th>
                  <th>Status</th>
                  <th>Priorita</th>
                  <th>Postup</th>
                  <th>Termín</th>
                </tr>
              </thead>
              <tbody>
                {recentOrders.map((order) => (
                  <tr key={order.id}>
                    <td><strong>{order.orderNumber}</strong></td>
                    <td>{order.productName}</td>
                    <td>
                      <span className={`badge badge-${getStatusColor(order.status)}`}>
                        {getStatusLabel(order.status)}
                      </span>
                    </td>
                    <td>
                      <span className={`badge badge-${getPriorityColor(order.priority)}`}>
                        {getPriorityLabel(order.priority)}
                      </span>
                    </td>
                    <td>
                      <div style={{ display: 'flex', alignItems: 'center', gap: '0.5rem' }}>
                        <div className="progress-bar" style={{ flex: 1 }}>
                          <div 
                            className={`progress-fill ${order.completionPercentage >= 75 ? 'success' : order.completionPercentage >= 50 ? 'warning' : ''}`}
                            style={{ width: `${order.completionPercentage}%` }}
                          ></div>
                        </div>
                        <span style={{ fontSize: '0.875rem', minWidth: '3rem' }}>
                          {Math.round(order.completionPercentage)}%
                        </span>
                      </div>
                    </td>
                    <td>{new Date(order.dueDate).toLocaleDateString('cs-CZ')}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        ) : (
          <p style={{ textAlign: 'center', color: 'var(--text-secondary)', padding: '2rem' }}>
            Žádné aktivní zakázky
          </p>
        )}
      </div>
    </div>
  );
}

function getStatusLabel(status) {
  const labels = {
    0: 'Čeká',
    1: 'Probíhá',
    2: 'Pozastaveno',
    3: 'Dokončeno',
    4: 'Zrušeno'
  };
  return labels[status] || 'Neznámý';
}

function getStatusColor(status) {
  const colors = {
    0: 'secondary',
    1: 'primary',
    2: 'warning',
    3: 'success',
    4: 'danger'
  };
  return colors[status] || 'secondary';
}

function getPriorityLabel(priority) {
  const labels = {
    0: 'Nízká',
    1: 'Normální',
    2: 'Vysoká',
    3: 'Urgentní'
  };
  return labels[priority] || 'Neznámá';
}

function getPriorityColor(priority) {
  const colors = {
    0: 'secondary',
    1: 'primary',
    2: 'warning',
    3: 'danger'
  };
  return colors[priority] || 'secondary';
}

export default Dashboard;
