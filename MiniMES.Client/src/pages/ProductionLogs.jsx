import { useState, useEffect } from 'react';
import { productionLogAPI } from '../api';
import { translateMachineName } from '../utils/machineTranslations';
import { FileText } from 'lucide-react';

function ProductionLogs() {
  const [logs, setLogs] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadLogs();
  }, []);

  const loadLogs = async () => {
    try {
      const response = await productionLogAPI.getAll();
      setLogs(response.data);
    } catch (error) {
      console.error('Error loading logs:', error);
    } finally {
      setLoading(false);
    }
  };

  // Lokalizace poznámek do češtiny
  const translateNote = (note) => {
    if (!note) return note;
    const replacements = [
      { en: 'Batch welding completed', cs: 'Dávka svařování dokončena' },
      { en: 'Second batch completed', cs: 'Druhá dávka dokončena' },
      { en: 'First batch completed', cs: 'První dávka dokončena' },
      { en: 'Work order started', cs: 'Zakázka zahájena' },
      { en: 'Welding operation started', cs: 'Svařovací operace zahájena' }
    ];
    for (const r of replacements) {
      if (note.includes(r.en)) return note.replace(r.en, r.cs);
    }
    return note;
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
      <h1>Výrobní logy</h1>
      <p style={{ color: 'var(--text-secondary)', marginBottom: '2rem' }}>
        Historie všech výrobních událostí
      </p>

      <div className="card">
        <div className="table-container">
          <table>
            <thead>
              <tr>
                <th>Čas</th>
                <th>Zakázka</th>
                <th>Typ události</th>
                <th>Stroj</th>
                <th>Pracovník</th>
                <th>Vyrobeno</th>
                <th>Zmetky</th>
                <th>Poznámka</th>
              </tr>
            </thead>
            <tbody>
              {logs.map((log) => (
                <tr key={log.id}>
                  <td>
                    {new Date(log.timestamp).toLocaleString('cs-CZ')}
                  </td>
                  <td>
                    <strong>{log.workOrder?.orderNumber || '-'}</strong>
                  </td>
                  <td>
                    <span className={`badge badge-${getEventColor(log.eventType)}`}>
                      {getEventLabel(log.eventType)}
                    </span>
                  </td>
                  <td>{translateMachineName(log.machine?.name) || '-'}</td>
                  <td>{log.employee?.fullName || '-'}</td>
                  <td>{log.quantityProduced > 0 ? log.quantityProduced : '-'}</td>
                  <td>
                    {log.quantityRejected > 0 && (
                      <span className="badge badge-danger">
                        {log.quantityRejected}
                      </span>
                    )}
                  </td>
                  <td style={{ fontSize: '0.875rem', color: 'var(--text-secondary)' }}>
                    {translateNote(log.notes) || '-'}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>

      {logs.length === 0 && (
        <div className="card">
          <p style={{ textAlign: 'center', color: 'var(--text-secondary)', padding: '2rem' }}>
            Žádné výrobní logy
          </p>
        </div>
      )}
    </div>
  );
}

function getEventLabel(eventType) {
  const labels = {
    0: 'Start',
    1: 'Výroba',
    2: 'Pauza',
    3: 'Pokračování',
    4: 'Dokončení',
    5: 'Zmetek',
    6: 'Chyba stroje'
  };
  return labels[eventType] || 'Neznámý';
}

function getEventColor(eventType) {
  const colors = {
    0: 'primary',
    1: 'success',
    2: 'warning',
    3: 'primary',
    4: 'success',
    5: 'danger',
    6: 'danger'
  };
  return colors[eventType] || 'secondary';
}

export default ProductionLogs;
