import { useState, useEffect } from 'react';
import { machineAPI } from '../api';
import Toast from '../components/Toast';
import { translateMachineName, translateMachineType, translateLocation, translateMachineNote } from '../utils/machineTranslations';
import { Plus, Edit, Trash2, Activity } from 'lucide-react';

function Machines({ user }) {
  const [machines, setMachines] = useState([]);
  const [loading, setLoading] = useState(true);
  const [showEdit, setShowEdit] = useState(false);
  const [selectedMachine, setSelectedMachine] = useState(null);
  const [editForm, setEditForm] = useState({ name: '', type: '', location: '', notes: '', efficiencyRate: '', status: 0 });
  const [errors, setErrors] = useState({});
  const [flash, setFlash] = useState('');
  const [showCreate, setShowCreate] = useState(false);
  const [createForm, setCreateForm] = useState({ machineCode: '', name: '', type: '', location: '', notes: '' });
  const [createErrors, setCreateErrors] = useState({});

  useEffect(() => {
    loadMachines();
  }, []);

  const loadMachines = async () => {
    try {
      const response = await machineAPI.getAll();
      setMachines(response.data);
    } catch (error) {
      console.error('Error loading machines:', error);
    } finally {
      setLoading(false);
    }
  };

  const openEdit = (machine) => {
    setSelectedMachine(machine);
    setEditForm({
      name: translateMachineName(machine.name) || '',
      type: translateMachineType(machine.type) || '',
      location: translateLocation(machine.location) || '',
      notes: machine.notes || '',
      efficiencyRate: machine.efficiencyRate ?? '',
      status: typeof machine.status === 'number' ? machine.status : 0
    });
    setShowEdit(true);
  };

  const saveEdit = async () => {
    if (!selectedMachine) return;
    const newErrors = {};
    if (!editForm.name?.trim()) newErrors.name = 'Název je povinný';
    if (!editForm.type?.trim()) newErrors.type = 'Typ je povinný';
    if (editForm.efficiencyRate !== '' && (isNaN(Number(editForm.efficiencyRate)) || Number(editForm.efficiencyRate) < 0 || Number(editForm.efficiencyRate) > 100)) {
      newErrors.efficiencyRate = 'Efektivita musí být mezi 0 a 100';
    }
    setErrors(newErrors);
    if (Object.keys(newErrors).length > 0) return;
    try {
      const payload = {
        name: editForm.name || null,
        type: editForm.type || null,
        location: editForm.location || null,
        notes: editForm.notes || null,
        efficiencyRate: editForm.efficiencyRate === '' ? null : Number(editForm.efficiencyRate)
      };
      await machineAPI.update(selectedMachine.id, payload);
      if (editForm.status !== selectedMachine.status) {
        await machineAPI.updateStatus(selectedMachine.id, editForm.status);
      }
      setShowEdit(false);
      setSelectedMachine(null);
      await loadMachines();
      setFlash('Změny stroje byly úspěšně uloženy.');
    } catch (error) {
      alert('Chyba při ukládání změn stroje');
      console.error(error);
    }
  };

  const handleCreate = async () => {
    const newErrors = {};
    if (!createForm.machineCode?.trim()) newErrors.machineCode = 'Kód stroje je povinný';
    if (!createForm.name?.trim()) newErrors.name = 'Název je povinný';
    if (!createForm.type?.trim()) newErrors.type = 'Typ je povinný';
    setCreateErrors(newErrors);
    if (Object.keys(newErrors).length > 0) return;
    try {
      const payload = {
        machineCode: createForm.machineCode,
        name: createForm.name,
        type: createForm.type,
        location: createForm.location || null,
        notes: createForm.notes || null
      };
      await machineAPI.create(payload);
      setShowCreate(false);
      setCreateForm({ machineCode: '', name: '', type: '', location: '', notes: '' });
      await loadMachines();
      setFlash('Nový stroj byl úspěšně vytvořen.');
    } catch (error) {
      alert('Chyba při vytváření stroje');
      console.error(error);
    }
  };

  const handleDelete = async (id) => {
    if (confirm('Opravdu smazat tento stroj?')) {
      try {
        await machineAPI.delete(id);
        loadMachines();
      } catch (error) {
        alert('Chyba při mazání stroje');
      }
    }
  };

  const canManage = user?.role === 'Admin' || user?.role === 'Manager';

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
        <h1>Stroje a zařízení</h1>
        {canManage && (
          <button className="btn btn-primary" onClick={() => setShowCreate(true)}>
            <Plus size={18} />
            Nový stroj
          </button>
        )}
      </div>
      <Toast message={flash} type="success" onClose={() => setFlash('')} />

      <div className="grid grid-3">
        {machines.map((machine) => (
          <div key={machine.id} className="card">
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'start', marginBottom: '1rem' }}>
              <div>
                <h3 style={{ marginBottom: '0.25rem' }}>{translateMachineName(machine.name)}</h3>
                <p style={{ fontSize: '0.875rem', color: 'var(--text-secondary)' }}>
                  {machine.machineCode}
                </p>
              </div>
              <span className={`badge badge-${getStatusColor(machine.status)}`}>
                {getStatusLabel(machine.status)}
              </span>
            </div>

            <div style={{ marginBottom: '1rem' }}>
              <div style={{ display: 'flex', justifyContent: 'space-between', fontSize: '0.875rem', marginBottom: '0.5rem' }}>
                <span style={{ color: 'var(--text-secondary)' }}>Typ:</span>
                <strong>{translateMachineType(machine.type)}</strong>
              </div>
              <div style={{ display: 'flex', justifyContent: 'space-between', fontSize: '0.875rem', marginBottom: '0.5rem' }}>
                <span style={{ color: 'var(--text-secondary)' }}>Lokace:</span>
                <strong>{translateLocation(machine.location) || '-'}</strong>
              </div>
              <div style={{ display: 'flex', justifyContent: 'space-between', fontSize: '0.875rem' }}>
                <span style={{ color: 'var(--text-secondary)' }}>Efektivita:</span>
                <strong>{machine.efficiencyRate}%</strong>
              </div>
              {machine.notes && (
                <div style={{ display: 'flex', justifyContent: 'space-between', fontSize: '0.875rem', marginTop: '0.5rem' }}>
                  <span style={{ color: 'var(--text-secondary)' }}>Poznámky:</span>
                  <strong>{translateMachineNote(machine.notes)}</strong>
                </div>
              )}
            </div>

            <div className="progress-bar">
              <div 
                className={`progress-fill ${machine.efficiencyRate >= 90 ? 'success' : machine.efficiencyRate >= 70 ? 'warning' : 'danger'}`}
                style={{ width: `${machine.efficiencyRate}%` }}
              ></div>
            </div>

            {canManage && (
              <div style={{ display: 'flex', gap: '0.5rem', marginTop: '1rem' }}>
                <button className="btn btn-secondary btn-sm" style={{ flex: 1 }} onClick={() => openEdit(machine)}>
                  <Edit size={14} />
                  Upravit
                </button>
                {user?.role === 'Admin' && (
                  <button 
                    className="btn btn-danger btn-sm"
                    onClick={() => handleDelete(machine.id)}
                  >
                    <Trash2 size={14} />
                  </button>
                )}
              </div>
            )}
          </div>
        ))}
      </div>

      {machines.length === 0 && (
        <div className="card">
          <p style={{ textAlign: 'center', color: 'var(--text-secondary)', padding: '2rem' }}>
            Žádné stroje v systému
          </p>
        </div>
      )}
      {showCreate && (
        <Modal title="Vytvořit nový stroj" onClose={() => setShowCreate(false)}>
          <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '1rem' }}>
            <div className="form-group">
              <label className="form-label">Kód stroje</label>
              <input className="form-input" value={createForm.machineCode} onChange={(e) => setCreateForm({ ...createForm, machineCode: e.target.value })} />
              {createErrors.machineCode && <div style={{ color: 'var(--danger)', fontSize: '0.875rem' }}>{createErrors.machineCode}</div>}
            </div>
            <div className="form-group">
              <label className="form-label">Název</label>
              <input className="form-input" value={createForm.name} onChange={(e) => setCreateForm({ ...createForm, name: e.target.value })} />
              {createErrors.name && <div style={{ color: 'var(--danger)', fontSize: '0.875rem' }}>{createErrors.name}</div>}
            </div>
            <div className="form-group">
              <label className="form-label">Typ</label>
              <input className="form-input" value={createForm.type} onChange={(e) => setCreateForm({ ...createForm, type: e.target.value })} />
              {createErrors.type && <div style={{ color: 'var(--danger)', fontSize: '0.875rem' }}>{createErrors.type}</div>}
            </div>
            <div className="form-group">
              <label className="form-label">Lokace</label>
              <input className="form-input" value={createForm.location} onChange={(e) => setCreateForm({ ...createForm, location: e.target.value })} />
            </div>
            <div className="form-group" style={{ gridColumn: '1 / -1' }}>
              <label className="form-label">Poznámky</label>
              <textarea className="form-textarea" rows={3} value={createForm.notes} onChange={(e) => setCreateForm({ ...createForm, notes: e.target.value })} />
            </div>
          </div>
          <div style={{ display: 'flex', justifyContent: 'flex-end', gap: '0.5rem', marginTop: '1rem' }}>
            <button className="btn btn-secondary" onClick={() => setShowCreate(false)}>Zrušit</button>
            <button className="btn btn-primary" onClick={handleCreate}>Vytvořit stroj</button>
          </div>
        </Modal>
      )}
      {showEdit && selectedMachine && (
        <Modal title={`Upravit stroj: ${translateMachineName(selectedMachine.name)}`} onClose={() => setShowEdit(false)}>
          <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '1rem' }}>
            <div className="form-group">
              <label className="form-label">Název</label>
              <input className="form-input" value={editForm.name} onChange={(e) => setEditForm({ ...editForm, name: e.target.value })} />
              {errors.name && <div style={{ color: 'var(--danger)', fontSize: '0.875rem' }}>{errors.name}</div>}
            </div>
            <div className="form-group">
              <label className="form-label">Typ</label>
              <input className="form-input" value={editForm.type} onChange={(e) => setEditForm({ ...editForm, type: e.target.value })} />
              {errors.type && <div style={{ color: 'var(--danger)', fontSize: '0.875rem' }}>{errors.type}</div>}
            </div>
            <div className="form-group">
              <label className="form-label">Lokace</label>
              <input className="form-input" value={editForm.location} onChange={(e) => setEditForm({ ...editForm, location: e.target.value })} />
            </div>
            <div className="form-group">
              <label className="form-label">Efektivita (%)</label>
              <input className="form-input" type="number" step="0.1" min="0" max="100" value={editForm.efficiencyRate} onChange={(e) => setEditForm({ ...editForm, efficiencyRate: e.target.value })} />
              {errors.efficiencyRate && <div style={{ color: 'var(--danger)', fontSize: '0.875rem' }}>{errors.efficiencyRate}</div>}
            </div>
            <div className="form-group">
              <label className="form-label">Stav</label>
              <select className="form-select" value={editForm.status} onChange={(e) => setEditForm({ ...editForm, status: Number(e.target.value) })}>
                <option value={0}>Nečinný</option>
                <option value={1}>Běží</option>
                <option value={2}>Údržba</option>
                <option value={3}>Chyba</option>
                <option value={4}>Offline</option>
              </select>
            </div>
            <div className="form-group" style={{ gridColumn: '1 / -1' }}>
              <label className="form-label">Poznámky</label>
              <textarea className="form-textarea" rows={3} value={editForm.notes} onChange={(e) => setEditForm({ ...editForm, notes: e.target.value })} />
            </div>
          </div>
          <div style={{ display: 'flex', justifyContent: 'flex-end', gap: '0.5rem', marginTop: '1rem' }}>
            <button className="btn btn-secondary" onClick={() => setShowEdit(false)}>Zrušit</button>
            <button className="btn btn-primary" onClick={saveEdit}>Uložit změny</button>
          </div>
        </Modal>
      )}
    </div>
  );
}

function getStatusLabel(status) {
  const labels = {
    0: 'Nečinný',
    1: 'Běží',
    2: 'Údržba',
    3: 'Chyba',
    4: 'Offline'
  };
  return labels[status] || 'Neznámý';
}

function getStatusColor(status) {
  const colors = {
    0: 'secondary',
    1: 'success',
    2: 'warning',
    3: 'danger',
    4: 'secondary'
  };
  return colors[status] || 'secondary';
}

export default Machines;

// Simple modal styles inline to match existing design
function Modal({ title, children, onClose }) {
  return (
    <div style={{ position: 'fixed', inset: 0, background: 'rgba(0,0,0,0.25)', display: 'flex', alignItems: 'center', justifyContent: 'center', zIndex: 1000 }}>
      <div className="card" style={{ width: '640px' }}>
        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1rem' }}>
          <h2 style={{ margin: 0 }}>{title}</h2>
          <button className="btn btn-secondary btn-sm" onClick={onClose}>Zavřít</button>
        </div>
        {children}
      </div>
    </div>
  );
}

