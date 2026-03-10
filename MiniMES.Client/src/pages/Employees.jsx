import { useState, useEffect } from 'react';
import { employeeAPI } from '../api';
import Toast from '../components/Toast';
import { Plus, Edit, Trash2, User } from 'lucide-react';

function Employees({ user }) {
  const [employees, setEmployees] = useState([]);
  const [loading, setLoading] = useState(true);
  const [showEdit, setShowEdit] = useState(false);
  const [showCreate, setShowCreate] = useState(false);
  const [selectedEmployee, setSelectedEmployee] = useState(null);
  const [editForm, setEditForm] = useState({
    firstName: '',
    lastName: '',
    email: '',
    phone: '',
    position: '',
    department: '',
    skills: '',
    status: 0
  });
  const [createForm, setCreateForm] = useState({
    firstName: '',
    lastName: '',
    email: '',
    phone: '',
    position: '',
    department: '',
    skills: ''
  });
  const [errors, setErrors] = useState({});
  const [flash, setFlash] = useState('');

  useEffect(() => {
    loadEmployees();
  }, []);

  const loadEmployees = async () => {
    try {
      const response = await employeeAPI.getAll();
      setEmployees(response.data);
    } catch (error) {
      console.error('Error loading employees:', error);
    } finally {
      setLoading(false);
    }
  };

  // Překlad pozice (angličtina -> čeština)
  const translatePosition = (pos) => {
    if (!pos) return pos;
    const map = {
      'CNC Operator': 'Operátor CNC',
      'Welder': 'Svářeč',
      'Assembly Technician': 'Technik montáže',
      'Quality Inspector': 'Kontrolor kvality'
    };
    return map[pos] || pos;
  };

  // Překlad dovedností – postupné nahrazování známých výrazů
  const translateSkills = (skills) => {
    if (!skills) return skills;
    const replacements = [
      { en: 'CNC programming', cs: 'Programování CNC' },
      { en: 'quality control', cs: 'kontrola kvality' },
      { en: 'Quality inspection', cs: 'Kontrola kvality' },
      { en: 'documentation', cs: 'dokumentace' },
      { en: 'MIG welding', cs: 'svařování MIG' },
      { en: 'TIG welding', cs: 'svařování TIG' },
      { en: 'Assembly', cs: 'Montáž' },
      { en: 'testing', cs: 'testování' },
      { en: 'packaging', cs: 'balení' }
    ];
    let out = skills;
    for (const r of replacements) {
      out = out.replaceAll(r.en, r.cs);
    }
    return out;
  };

  // Překlad oddělení (angličtina -> čeština)
  const translateDepartment = (dept) => {
    if (!dept) return dept;
    const map = {
      'Production': 'Výroba',
      'Quality': 'Kvalita'
    };
    return map[dept] || dept;
  };

  const handleDelete = async (id) => {
    if (confirm('Opravdu smazat tohoto pracovníka?')) {
      try {
        await employeeAPI.delete(id);
        loadEmployees();
      } catch (error) {
        alert('Chyba při mazání pracovníka');
      }
    }
  };

  const openEdit = (employee) => {
    setSelectedEmployee(employee);
    setEditForm({
      firstName: employee.firstName || '',
      lastName: employee.lastName || '',
      email: employee.email || '',
      phone: employee.phone || '',
      position: translatePosition(employee.position) || '',
      department: translateDepartment(employee.department) || '',
      skills: translateSkills(employee.skills) || '',
      status: typeof employee.status === 'number' ? employee.status : 0
    });
    setShowEdit(true);
  };

  const openCreate = () => {
    setErrors({});
    setCreateForm({
      firstName: '',
      lastName: '',
      email: '',
      phone: '',
      position: '',
      department: '',
      skills: ''
    });
    setShowCreate(true);
  };

  const saveCreate = async () => {
    const newErrors = {};
    if (!createForm.firstName?.trim()) newErrors.firstName = 'Jméno je povinné';
    if (!createForm.lastName?.trim()) newErrors.lastName = 'Příjmení je povinné';
    if (!createForm.email?.trim() || !/^\S+@\S+\.\S+$/.test(createForm.email)) newErrors.email = 'Zadej platný email';
    if (!createForm.position?.trim()) newErrors.position = 'Pozice je povinná';
    if (!createForm.department?.trim()) newErrors.department = 'Oddělení je povinné';
    setErrors(newErrors);
    if (Object.keys(newErrors).length > 0) return;
    try {
      const payload = {
        firstName: createForm.firstName || null,
        lastName: createForm.lastName || null,
        email: createForm.email || null,
        phone: createForm.phone || null,
        position: createForm.position || null,
        department: createForm.department || null,
        skills: createForm.skills || null
      };
      await employeeAPI.create(payload);
      setShowCreate(false);
      await loadEmployees();
      setFlash('Nový pracovník byl úspěšně vytvořen.');
    } catch (error) {
      alert('Chyba při vytváření nového pracovníka');
      console.error(error);
    }
  };

  const saveEdit = async () => {
    if (!selectedEmployee) return;
    const newErrors = {};
    if (!editForm.firstName?.trim()) newErrors.firstName = 'Jméno je povinné';
    if (!editForm.lastName?.trim()) newErrors.lastName = 'Příjmení je povinné';
    if (!editForm.email?.trim() || !/^\S+@\S+\.\S+$/.test(editForm.email)) newErrors.email = 'Zadej platný email';
    if (!editForm.position?.trim()) newErrors.position = 'Pozice je povinná';
    if (!editForm.department?.trim()) newErrors.department = 'Oddělení je povinné';
    setErrors(newErrors);
    if (Object.keys(newErrors).length > 0) return;
    try {
      const payload = {
        firstName: editForm.firstName || null,
        lastName: editForm.lastName || null,
        email: editForm.email || null,
        phone: editForm.phone || null,
        position: editForm.position || null,
        department: editForm.department || null,
        skills: editForm.skills || null
      };
      await employeeAPI.update(selectedEmployee.id, payload);
      if (editForm.status !== selectedEmployee.status) {
        await employeeAPI.updateStatus(selectedEmployee.id, editForm.status);
      }
      setShowEdit(false);
      setSelectedEmployee(null);
      await loadEmployees();
      setFlash('Změny pracovníka byly úspěšně uloženy.');
    } catch (error) {
      alert('Chyba při ukládání změn pracovníka');
      console.error(error);
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
        <h1>Pracovníci</h1>
        {canManage && (
          <button className="btn btn-primary" onClick={openCreate}>
            <Plus size={18} />
            Nový pracovník
          </button>
        )}
      </div>
      <Toast message={flash} type="success" onClose={() => setFlash('')} />

      <div className="card">
        <div className="table-container">
          <table>
            <thead>
              <tr>
                <th>Kód</th>
                <th>Jméno</th>
                <th>Email</th>
                <th>Pozice</th>
                <th>Oddělení</th>
                <th>Status</th>
                <th>Dovednosti</th>
                {canManage && <th>Akce</th>}
              </tr>
            </thead>
            <tbody>
              {employees.map((employee) => (
                <tr key={employee.id}>
                  <td><strong>{employee.employeeCode}</strong></td>
                  <td>{employee.fullName}</td>
                  <td>{employee.email}</td>
                  <td>{translatePosition(employee.position)}</td>
                  <td>{translateDepartment(employee.department)}</td>
                  <td>
                    <span className={`badge badge-${getStatusColor(employee.status)}`}>
                      {getStatusLabel(employee.status)}
                    </span>
                  </td>
                  <td style={{ fontSize: '0.875rem', color: 'var(--text-secondary)' }}>
                    {translateSkills(employee.skills) || '-'}
                  </td>
                  {canManage && (
                    <td>
                      <div style={{ display: 'flex', gap: '0.5rem' }}>
                        <button className="btn btn-secondary btn-sm" onClick={() => openEdit(employee)}>
                          <Edit size={14} />
                        </button>
                        {user?.role === 'Admin' && (
                          <button 
                            className="btn btn-danger btn-sm"
                            onClick={() => handleDelete(employee.id)}
                          >
                            <Trash2 size={14} />
                          </button>
                        )}
                      </div>
                    </td>
                  )}
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>

      {employees.length === 0 && (
        <div className="card">
          <p style={{ textAlign: 'center', color: 'var(--text-secondary)', padding: '2rem' }}>
            Žádní pracovníci v systému
          </p>
        </div>
      )}

      {showEdit && selectedEmployee && (
        <Modal title={`Upravit pracovníka: ${selectedEmployee.fullName}`} onClose={() => setShowEdit(false)}>
          <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '1rem' }}>
            <div className="form-group">
              <label className="form-label">Jméno</label>
              <input className="form-input" value={editForm.firstName} onChange={(e) => setEditForm({ ...editForm, firstName: e.target.value })} />
              {errors.firstName && <div style={{ color: 'var(--danger)', fontSize: '0.875rem' }}>{errors.firstName}</div>}
            </div>
            <div className="form-group">
              <label className="form-label">Příjmení</label>
              <input className="form-input" value={editForm.lastName} onChange={(e) => setEditForm({ ...editForm, lastName: e.target.value })} />
              {errors.lastName && <div style={{ color: 'var(--danger)', fontSize: '0.875rem' }}>{errors.lastName}</div>}
            </div>
            <div className="form-group">
              <label className="form-label">Email</label>
              <input className="form-input" type="email" value={editForm.email} onChange={(e) => setEditForm({ ...editForm, email: e.target.value })} />
              {errors.email && <div style={{ color: 'var(--danger)', fontSize: '0.875rem' }}>{errors.email}</div>}
            </div>
            <div className="form-group">
              <label className="form-label">Telefon</label>
              <input className="form-input" value={editForm.phone} onChange={(e) => setEditForm({ ...editForm, phone: e.target.value })} />
            </div>
            <div className="form-group">
              <label className="form-label">Pozice</label>
              <input className="form-input" value={editForm.position} onChange={(e) => setEditForm({ ...editForm, position: e.target.value })} />
              {errors.position && <div style={{ color: 'var(--danger)', fontSize: '0.875rem' }}>{errors.position}</div>}
            </div>
            <div className="form-group">
              <label className="form-label">Oddělení</label>
              <input className="form-input" value={editForm.department} onChange={(e) => setEditForm({ ...editForm, department: e.target.value })} />
              {errors.department && <div style={{ color: 'var(--danger)', fontSize: '0.875rem' }}>{errors.department}</div>}
            </div>
            <div className="form-group">
              <label className="form-label">Status</label>
              <select className="form-select" value={editForm.status} onChange={(e) => setEditForm({ ...editForm, status: Number(e.target.value) })}>
                <option value={0}>Dostupný</option>
                <option value={1}>Pracuje</option>
                <option value={2}>Přestávka</option>
                <option value={3}>Nepřítomen</option>
              </select>
            </div>
            <div className="form-group" style={{ gridColumn: '1 / -1' }}>
              <label className="form-label">Dovednosti</label>
              <textarea className="form-textarea" rows={3} value={editForm.skills} onChange={(e) => setEditForm({ ...editForm, skills: e.target.value })} />
            </div>
          </div>
          <div style={{ display: 'flex', justifyContent: 'flex-end', gap: '0.5rem', marginTop: '1rem' }}>
            <button className="btn btn-secondary" onClick={() => setShowEdit(false)}>Zrušit</button>
            <button className="btn btn-primary" onClick={saveEdit}>Uložit změny</button>
          </div>
        </Modal>
      )}

      {showCreate && (
        <Modal title="Vytvořit nového pracovníka" onClose={() => setShowCreate(false)}>
          <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '1rem' }}>
            <div className="form-group">
              <label className="form-label">Jméno</label>
              <input className="form-input" value={createForm.firstName} onChange={(e) => setCreateForm({ ...createForm, firstName: e.target.value })} />
              {errors.firstName && <div style={{ color: 'var(--danger)', fontSize: '0.875rem' }}>{errors.firstName}</div>}
            </div>
            <div className="form-group">
              <label className="form-label">Příjmení</label>
              <input className="form-input" value={createForm.lastName} onChange={(e) => setCreateForm({ ...createForm, lastName: e.target.value })} />
              {errors.lastName && <div style={{ color: 'var(--danger)', fontSize: '0.875rem' }}>{errors.lastName}</div>}
            </div>
            <div className="form-group">
              <label className="form-label">Email</label>
              <input className="form-input" type="email" value={createForm.email} onChange={(e) => setCreateForm({ ...createForm, email: e.target.value })} />
              {errors.email && <div style={{ color: 'var(--danger)', fontSize: '0.875rem' }}>{errors.email}</div>}
            </div>
            <div className="form-group">
              <label className="form-label">Telefon</label>
              <input className="form-input" value={createForm.phone} onChange={(e) => setCreateForm({ ...createForm, phone: e.target.value })} />
            </div>
            <div className="form-group">
              <label className="form-label">Pozice</label>
              <input className="form-input" value={createForm.position} onChange={(e) => setCreateForm({ ...createForm, position: e.target.value })} />
              {errors.position && <div style={{ color: 'var(--danger)', fontSize: '0.875rem' }}>{errors.position}</div>}
            </div>
            <div className="form-group">
              <label className="form-label">Oddělení</label>
              <input className="form-input" value={createForm.department} onChange={(e) => setCreateForm({ ...createForm, department: e.target.value })} />
              {errors.department && <div style={{ color: 'var(--danger)', fontSize: '0.875rem' }}>{errors.department}</div>}
            </div>
            <div className="form-group" style={{ gridColumn: '1 / -1' }}>
              <label className="form-label">Dovednosti</label>
              <textarea className="form-textarea" rows={3} value={createForm.skills} onChange={(e) => setCreateForm({ ...createForm, skills: e.target.value })} />
            </div>
          </div>
          <div style={{ display: 'flex', justifyContent: 'flex-end', gap: '0.5rem', marginTop: '1rem' }}>
            <button className="btn btn-secondary" onClick={() => setShowCreate(false)}>Zrušit</button>
            <button className="btn btn-primary" onClick={saveCreate}>Vytvořit pracovníka</button>
          </div>
        </Modal>
      )}
    </div>
  );
}

function getStatusLabel(status) {
  const labels = {
    0: 'Dostupný',
    1: 'Pracuje',
    2: 'Přestávka',
    3: 'Nepřítomen'
  };
  return labels[status] || 'Neznámý';
}

function getStatusColor(status) {
  const colors = {
    0: 'success',
    1: 'primary',
    2: 'warning',
    3: 'secondary'
  };
  return colors[status] || 'secondary';
}

export default Employees;

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
