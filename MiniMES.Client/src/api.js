import axios from 'axios';

const API_URL = 'http://localhost:5000/api';

const api = axios.create({
  baseURL: API_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Add token to requests
api.interceptors.request.use((config) => {
  const token = localStorage.getItem('token');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

// Auth
export const authAPI = {
  login: (data) => api.post('/auth/login', data),
  register: (data) => api.post('/auth/register', data),
  getCurrentUser: () => api.get('/auth/me'),
};

// Dashboard
export const dashboardAPI = {
  getStats: () => api.get('/dashboard/stats'),
};

// Work Orders
export const workOrderAPI = {
  getAll: () => api.get('/workorders'),
  getActive: () => api.get('/workorders/active'),
  getById: (id) => api.get(`/workorders/${id}`),
  create: (data) => api.post('/workorders', data),
  update: (id, data) => api.put(`/workorders/${id}`, data),
  delete: (id) => api.delete(`/workorders/${id}`),
  start: (id, machineId, employeeId) => 
    api.post(`/workorders/${id}/start`, null, { params: { machineId, employeeId } }),
  pause: (id, reason) => 
    api.post(`/workorders/${id}/pause`, null, { params: { reason } }),
  complete: (id) => api.post(`/workorders/${id}/complete`),
  updateProgress: (id, quantityProduced, quantityRejected, notes) =>
    api.post(`/workorders/${id}/progress`, null, { 
      params: { quantityProduced, quantityRejected, notes } 
    }),
};

// Machines
export const machineAPI = {
  getAll: () => api.get('/machines'),
  getAvailable: () => api.get('/machines/available'),
  getById: (id) => api.get(`/machines/${id}`),
  create: (data) => api.post('/machines', data),
  update: (id, data) => api.put(`/machines/${id}`, data),
  delete: (id) => api.delete(`/machines/${id}`),
  updateStatus: (id, status) => 
    api.patch(`/machines/${id}/status`, null, { params: { status } }),
};

// Employees
export const employeeAPI = {
  getAll: () => api.get('/employees'),
  getAvailable: () => api.get('/employees/available'),
  getById: (id) => api.get(`/employees/${id}`),
  create: (data) => api.post('/employees', data),
  update: (id, data) => api.put(`/employees/${id}`, data),
  delete: (id) => api.delete(`/employees/${id}`),
  updateStatus: (id, status) => 
    api.patch(`/employees/${id}/status`, null, { params: { status } }),
};

// Production Logs
export const productionLogAPI = {
  getAll: () => api.get('/productionlogs'),
  getById: (id) => api.get(`/productionlogs/${id}`),
  getByWorkOrder: (workOrderId) => api.get(`/productionlogs/workorder/${workOrderId}`),
  create: (data) => api.post('/productionlogs', data),
};

export default api;
