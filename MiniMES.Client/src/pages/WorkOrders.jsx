import { useState, useEffect } from 'react';
import { workOrderAPI, machineAPI, employeeAPI } from '../api';
import Toast from '../components/Toast';
import { translateMachineName } from '../utils/machineTranslations';
import { Plus, Play, Pause, CheckCircle, Edit, Trash2 } from 'lucide-react';

function WorkOrders({ user }) {
  const [orders, setOrders] = useState([]);
  const [machines, setMachines] = useState([]);
  const [employees, setEmployees] = useState([]);
  const [loading, setLoading] = useState(true);
  const [showModal, setShowModal] = useState(false);
  const [showEdit, setShowEdit] = useState(false);
  const [selectedOrder, setSelectedOrder] = useState(null);
  const [flash, setFlash] = useState('');
  const [createForm, setCreateForm] = useState({ orderNumber: '', productName: '', quantityPlanned: 0, dueDate: '', priority: 1, notes: '', assignedMachineId: '', assignedEmployeeId: '' });
  const [createErrors, setCreateErrors] = useState({});
  const [createErrorMessage, setCreateErrorMessage] = useState('');
  const [editForm, setEditForm] = useState({ orderNumber: '', productName: '', quantityPlanned: 0, dueDate: '', priority: 1, notes: '', assignedMachineId: '', assignedEmployeeId: '' });
  const [editErrors, setEditErrors] = useState({});
  const [editErrorMessage, setEditErrorMessage] = useState('');

  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    try {
      const [ordersRes, machinesRes, employeesRes] = await Promise.all([
        workOrderAPI.getAll(),
        machineAPI.getAll(),
        employeeAPI.getAll()
      ]);
      
      setOrders(ordersRes.data);
      setMachines(machinesRes.data);
      setEmployees(employeesRes.data);
    } catch (error) {
      console.error('Error loading data:', error);
    } finally {
      setLoading(false);
    }
  };

  const handleStart = async (orderId) => {
    if (confirm('Spustit tuto zakázku?')) {
      try {
        await workOrderAPI.start(orderId);
        loadData();
      } catch (error) {
        alert('Chyba při spuštění zakázky');
      }
    }
  };

  const handlePause = async (orderId) => {
    const reason = prompt('Důvod pozastavení:');
    if (reason) {
      try {
        await workOrderAPI.pause(orderId, reason);
        loadData();
      } catch (error) {
        alert('Chyba při pozastavení zakázky');
      }
    }
  };

  const handleComplete = async (orderId) => {
    if (confirm('Dokončit tuto zakázku?')) {
      try {
        await workOrderAPI.complete(orderId);
        loadData();
      } catch (error) {
        alert('Chyba při dokončování zakázky');
      }
    }
  };

  const handleDelete = async (orderId) => {
    if (confirm('Opravdu smazat tuto zakázku?')) {
      try {
        await workOrderAPI.delete(orderId);
        loadData();
      } catch (error) {
        alert('Chyba při mazání zakázky');
      }
    }
  };

  const canManage = user?.role === 'Admin' || user?.role === 'Manager';

  const openEdit = (order) => {
    setSelectedOrder(order);
    setEditForm({
      orderNumber: order.orderNumber || '',
      productName: order.productName || '',
      quantityPlanned: order.quantityPlanned ?? 0,
      dueDate: formatDateInput(order.dueDate),
      priority: typeof order.priority === 'number' ? order.priority : Number(order.priority) || 1,
      notes: order.notes || '',
      assignedMachineId: order.assignedMachine?.id ? String(order.assignedMachine.id) : '',
      assignedEmployeeId: order.assignedEmployee?.id ? String(order.assignedEmployee.id) : ''
    });
    setEditErrors({});
    setEditErrorMessage('');
    setShowEdit(true);
  };

  const handleEdit = async () => {
    if (!selectedOrder) return;
    const newErrors = {};
    if (!editForm.orderNumber?.trim()) newErrors.orderNumber = 'Číslo zakázky je povinné';
    if (!editForm.productName?.trim()) newErrors.productName = 'Název produktu je povinný';
    if (!editForm.quantityPlanned || editForm.quantityPlanned <= 0) newErrors.quantityPlanned = 'Počet kusů musí být větší než 0';
    if (!editForm.dueDate) newErrors.dueDate = 'Termín je povinný';
    setEditErrors(newErrors);
    setEditErrorMessage('');
    if (Object.keys(newErrors).length > 0) return;
    try {
      const payload = {
        orderNumber: editForm.orderNumber,
        productName: editForm.productName,
        quantityPlanned: Number(editForm.quantityPlanned),
        dueDate: editForm.dueDate,
        priority: Number(editForm.priority),
        notes: editForm.notes || null,
        assignedMachineId: editForm.assignedMachineId ? Number(editForm.assignedMachineId) : null,
        assignedEmployeeId: editForm.assignedEmployeeId ? Number(editForm.assignedEmployeeId) : null
      };
      await workOrderAPI.update(selectedOrder.id, payload);
      setShowEdit(false);
      setSelectedOrder(null);
      await loadData();
      setFlash('Zakázka byla úspěšně upravena.');
    } catch (error) {
      setEditErrorMessage(getApiErrorMessage(error, 'Chyba při ukládání zakázky.'));
      console.error(error);
    }
  };

  const handleCreate = async () => {
    const newErrors = {};
    if (!createForm.orderNumber?.trim()) newErrors.orderNumber = 'Číslo zakázky je povinné';
    if (!createForm.productName?.trim()) newErrors.productName = 'Název produktu je povinný';
    if (!createForm.quantityPlanned || createForm.quantityPlanned <= 0) newErrors.quantityPlanned = 'Počet kusů musí být větší než 0';
    if (!createForm.dueDate) newErrors.dueDate = 'Termín je povinný';
    setCreateErrors(newErrors);
    setCreateErrorMessage('');
    if (Object.keys(newErrors).length > 0) return;
    try {
      const payload = {
        orderNumber: createForm.orderNumber,
        productName: createForm.productName,
        quantityPlanned: Number(createForm.quantityPlanned),
        dueDate: createForm.dueDate,
        priority: Number(createForm.priority),
        notes: createForm.notes || null,
        assignedMachineId: createForm.assignedMachineId ? Number(createForm.assignedMachineId) : null,
        assignedEmployeeId: createForm.assignedEmployeeId ? Number(createForm.assignedEmployeeId) : null
      };
      await workOrderAPI.create(payload);
      setShowModal(false);
      setCreateForm({ orderNumber: '', productName: '', quantityPlanned: 0, dueDate: '', priority: 1, notes: '', assignedMachineId: '', assignedEmployeeId: '' });
      await loadData();
      setFlash('Nová zakázka byla úspěšně vytvořena.');
    } catch (error) {
      setCreateErrorMessage(getApiErrorMessage(error, 'Chyba při vytváření zakázky.'));
      console.error(error);
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
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '2rem' }}>
        <h1>Výrobní zakázky</h1>
        {canManage && (
          <button className="btn btn-primary" onClick={() => setShowModal(true)}>
            <Plus size={18} />
            Nová zakázka
          </button>
        )}
      </div>

      <Toast message={flash} type="success" onClose={() => setFlash('')} />

      <div className="card">
        <div className="table-container">
          <table>
            <thead>
              <tr>
                <th>Číslo</th>
                <th>Produkt</th>
                <th>Status</th>
                <th>Priorita</th>
                <th>Postup</th>
                <th>Stroj</th>
                <th>Pracovník</th>
                <th>Termín</th>
                <th className="actions-header">Akce</th>
              </tr>
            </thead>
            <tbody>
              {orders.map((order) => (
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
                    <div style={{ minWidth: '150px' }}>
                      <div style={{ display: 'flex', justifyContent: 'space-between', fontSize: '0.75rem', marginBottom: '0.25rem' }}>
                        <span>{order.quantityProduced} / {order.quantityPlanned}</span>
                        <span>{Math.round(order.completionPercentage)}%</span>
                      </div>
                      <div className="progress-bar">
                        <div 
                          className={`progress-fill ${order.completionPercentage >= 75 ? 'success' : order.completionPercentage >= 50 ? 'warning' : ''}`}
                          style={{ width: `${order.completionPercentage}%` }}
                        ></div>
                      </div>
                    </div>
                  </td>
                  <td>{translateMachineName(order.assignedMachine?.name) || '-'}</td>
                  <td>{order.assignedEmployee?.fullName || '-'}</td>
                  <td>{new Date(order.dueDate).toLocaleDateString('cs-CZ')}</td>
                  <td>
                    <div style={{ display: 'flex', gap: '0.5rem' }}>
                      {order.status === 0 && (
                        <button 
                          className="btn btn-success btn-sm" 
                          onClick={() => handleStart(order.id)}
                          title="Spustit"
                        >
                          <Play size={14} />
                        </button>
                      )}
                      {order.status === 1 && (
                        <>
                          <button 
                            className="btn btn-secondary btn-sm" 
                            onClick={() => handlePause(order.id)}
                            title="Pozastavit"
                          >
                            <Pause size={14} />
                          </button>
                          <button 
                            className="btn btn-success btn-sm" 
                            onClick={() => handleComplete(order.id)}
                            title="Dokončit"
                          >
                            <CheckCircle size={14} />
                          </button>
                        </>
                      )}
                      {canManage && order.status !== 1 && (
                        <button 
                          className="btn btn-danger btn-sm" 
                          onClick={() => handleDelete(order.id)}
                          title="Smazat"
                        >
                          <Trash2 size={14} />
                        </button>
                      )}
                      {canManage && (
                        <button
                          className="btn btn-secondary btn-sm"
                          onClick={() => openEdit(order)}
                          title="Upravit"
                        >
                          <Edit size={14} />
                        </button>
                      )}
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>
      {showEdit && selectedOrder && (
        <Modal title="Upravit zakázku" onClose={() => setShowEdit(false)}>
          <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '1rem' }}>
            <div className="form-group">
              <label className="form-label">Číslo zakázky</label>
              <input className="form-input" value={editForm.orderNumber} onChange={(e) => setEditForm({ ...editForm, orderNumber: e.target.value })} />
              {editErrors.orderNumber && <div style={{ color: 'var(--danger)', fontSize: '0.875rem' }}>{editErrors.orderNumber}</div>}
            </div>
            <div className="form-group">
              <label className="form-label">Název produktu</label>
              <input className="form-input" value={editForm.productName} onChange={(e) => setEditForm({ ...editForm, productName: e.target.value })} />
              {editErrors.productName && <div style={{ color: 'var(--danger)', fontSize: '0.875rem' }}>{editErrors.productName}</div>}
            </div>
            <div className="form-group">
              <label className="form-label">Počet kusů</label>
              <input className="form-input" type="number" min="1" value={editForm.quantityPlanned} onChange={(e) => setEditForm({ ...editForm, quantityPlanned: e.target.value })} />
              {editErrors.quantityPlanned && <div style={{ color: 'var(--danger)', fontSize: '0.875rem' }}>{editErrors.quantityPlanned}</div>}
            </div>
            <div className="form-group">
              <label className="form-label">Termín</label>
              <input className="form-input" type="date" value={editForm.dueDate} onChange={(e) => setEditForm({ ...editForm, dueDate: e.target.value })} />
              {editErrors.dueDate && <div style={{ color: 'var(--danger)', fontSize: '0.875rem' }}>{editErrors.dueDate}</div>}
            </div>
            <div className="form-group">
              <label className="form-label">Priorita</label>
              <select className="form-select" value={editForm.priority} onChange={(e) => setEditForm({ ...editForm, priority: e.target.value })}>
                <option value={0}>Nízká</option>
                <option value={1}>Normální</option>
                <option value={2}>Vysoká</option>
                <option value={3}>Urgentní</option>
              </select>
            </div>
            <div className="form-group">
              <label className="form-label">Stroj</label>
              <select className="form-select" value={editForm.assignedMachineId} onChange={(e) => setEditForm({ ...editForm, assignedMachineId: e.target.value })}>
                <option value="">Neprirazeno</option>
                {machines.map((machine) => (
                  <option key={machine.id} value={machine.id}>
                    {translateMachineName(machine.name)}
                  </option>
                ))}
              </select>
            </div>
            <div className="form-group">
              <label className="form-label">Pracovník</label>
              <select className="form-select" value={editForm.assignedEmployeeId} onChange={(e) => setEditForm({ ...editForm, assignedEmployeeId: e.target.value })}>
                <option value="">Neprirazeno</option>
                {employees.map((employee) => (
                  <option key={employee.id} value={employee.id}>
                    {employee.fullName}
                  </option>
                ))}
              </select>
            </div>
            <div className="form-group" style={{ gridColumn: '1 / -1' }}>
              <label className="form-label">Poznámky</label>
              <textarea className="form-textarea" rows={3} value={editForm.notes} onChange={(e) => setEditForm({ ...editForm, notes: e.target.value })} />
            </div>
          </div>
          <div style={{ display: 'flex', justifyContent: 'flex-end', gap: '0.5rem', marginTop: '1rem' }}>
            <button className="btn btn-secondary" onClick={() => setShowEdit(false)}>Zrušit</button>
            <button className="btn btn-primary" onClick={handleEdit}>Uložit změny</button>
          </div>
          {editErrorMessage && (
            <div style={{ color: 'var(--danger)', fontSize: '0.875rem', marginTop: '0.75rem' }}>
              {editErrorMessage}
            </div>
          )}
        </Modal>
      )}
      {showModal && (
        <Modal title="Vytvořit novou zakázku" onClose={() => setShowModal(false)}>
          <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '1rem' }}>
            <div className="form-group">
              <label className="form-label">Číslo zakázky</label>
              <input className="form-input" value={createForm.orderNumber} onChange={(e) => setCreateForm({ ...createForm, orderNumber: e.target.value })} />
              {createErrors.orderNumber && <div style={{ color: 'var(--danger)', fontSize: '0.875rem' }}>{createErrors.orderNumber}</div>}
            </div>
            <div className="form-group">
              <label className="form-label">Název produktu</label>
              <input className="form-input" value={createForm.productName} onChange={(e) => setCreateForm({ ...createForm, productName: e.target.value })} />
              {createErrors.productName && <div style={{ color: 'var(--danger)', fontSize: '0.875rem' }}>{createErrors.productName}</div>}
            </div>
            <div className="form-group">
              <label className="form-label">Počet kusů</label>
              <input className="form-input" type="number" min="1" value={createForm.quantityPlanned} onChange={(e) => setCreateForm({ ...createForm, quantityPlanned: e.target.value })} />
              {createErrors.quantityPlanned && <div style={{ color: 'var(--danger)', fontSize: '0.875rem' }}>{createErrors.quantityPlanned}</div>}
            </div>
            <div className="form-group">
              <label className="form-label">Termín</label>
              <input className="form-input" type="date" value={createForm.dueDate} onChange={(e) => setCreateForm({ ...createForm, dueDate: e.target.value })} />
              {createErrors.dueDate && <div style={{ color: 'var(--danger)', fontSize: '0.875rem' }}>{createErrors.dueDate}</div>}
            </div>
            <div className="form-group">
              <label className="form-label">Priorita</label>
              <select className="form-select" value={createForm.priority} onChange={(e) => setCreateForm({ ...createForm, priority: e.target.value })}>
                <option value={0}>Nízká</option>
                <option value={1}>Normální</option>
                <option value={2}>Vysoká</option>
                <option value={3}>Urgentní</option>
              </select>
            </div>
            <div className="form-group">
              <label className="form-label">Stroj</label>
              <select className="form-select" value={createForm.assignedMachineId} onChange={(e) => setCreateForm({ ...createForm, assignedMachineId: e.target.value })}>
                <option value="">Neprirazeno</option>
                {machines.map((machine) => (
                  <option key={machine.id} value={machine.id}>
                    {translateMachineName(machine.name)}
                  </option>
                ))}
              </select>
            </div>
            <div className="form-group">
              <label className="form-label">Pracovník</label>
              <select className="form-select" value={createForm.assignedEmployeeId} onChange={(e) => setCreateForm({ ...createForm, assignedEmployeeId: e.target.value })}>
                <option value="">Neprirazeno</option>
                {employees.map((employee) => (
                  <option key={employee.id} value={employee.id}>
                    {employee.fullName}
                  </option>
                ))}
              </select>
            </div>
            <div className="form-group" style={{ gridColumn: '1 / -1' }}>
              <label className="form-label">Poznámky</label>
              <textarea className="form-textarea" rows={3} value={createForm.notes} onChange={(e) => setCreateForm({ ...createForm, notes: e.target.value })} />
            </div>
          </div>
          <div style={{ display: 'flex', justifyContent: 'flex-end', gap: '0.5rem', marginTop: '1rem' }}>
            <button className="btn btn-secondary" onClick={() => setShowModal(false)}>Zrušit</button>
            <button className="btn btn-primary" onClick={handleCreate}>Vytvořit zakázku</button>
          </div>
          {createErrorMessage && (
            <div style={{ color: 'var(--danger)', fontSize: '0.875rem', marginTop: '0.75rem' }}>
              {createErrorMessage}
            </div>
          )}
        </Modal>
      )}
    </div>
  );
}

function Modal({ title, children, onClose }) {
  return (
    <div style={{ position: 'fixed', inset: 0, background: 'rgba(0,0,0,0.25)', display: 'flex', alignItems: 'center', justifyContent: 'center', zIndex: 1000 }}>
      <div className="card" style={{ width: '720px' }}>
        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1rem' }}>
          <h2 style={{ margin: 0 }}>{title}</h2>
          <button className="btn btn-secondary btn-sm" onClick={onClose}>Zavřít</button>
        </div>
        {children}
      </div>
    </div>
  );
}

function getStatusLabel(status) {
  const labels = { 0: 'Čeká', 1: 'Probíhá', 2: 'Pozastaveno', 3: 'Dokončeno', 4: 'Zrušeno' };
  return labels[status] || 'Neznámý';
}

function getStatusColor(status) {
  const colors = { 0: 'secondary', 1: 'primary', 2: 'warning', 3: 'success', 4: 'danger' };
  return colors[status] || 'secondary';
}

function getPriorityLabel(priority) {
  const labels = { 0: 'Nízká', 1: 'Normální', 2: 'Vysoká', 3: 'Urgentní' };
  return labels[priority] || 'Neznámá';
}

function getPriorityColor(priority) {
  const colors = { 0: 'secondary', 1: 'primary', 2: 'warning', 3: 'danger' };
  return colors[priority] || 'secondary';
}

function formatDateInput(dateValue) {
  if (!dateValue) return '';
  const date = new Date(dateValue);
  if (Number.isNaN(date.getTime())) return '';
  return date.toISOString().slice(0, 10);
}

function getApiErrorMessage(error, fallbackMessage) {
  const apiMessage = error?.response?.data?.message;
  if (typeof apiMessage === 'string' && apiMessage.trim().length > 0) {
    return apiMessage;
  }
  return fallbackMessage;
}

export default WorkOrders;
