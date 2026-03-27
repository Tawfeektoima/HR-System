import api from './api';

export const analyticsService = {
  getDashboardStats: async () => {
    const response = await api.get('/analytics/dashboard');
    return response.data;
  },
  
  getPipelineFunnel: async () => {
    const response = await api.get('/analytics/pipeline');
    return response.data;
  },
  
  getApplicationsPerMonth: async (months = 6) => {
    const response = await api.get(`/analytics/applications-per-month?months=${months}`);
    return response.data;
  },
  
  getTopJobs: async (count = 5) => {
    const response = await api.get(`/analytics/top-jobs?count=${count}`);
    return response.data;
  },
  
  getFullAnalytics: async () => {
    const response = await api.get('/analytics/full');
    return response.data;
  }
};
