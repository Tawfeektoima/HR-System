import api from './api';

export const interviewsService = {
  getAll: async (upcomingOnly = false) => {
    const response = await api.get('/interviews', { params: { upcomingOnly } });
    return response.data;
  },

  getById: async (id) => {
    const response = await api.get(`/interviews/${id}`);
    return response.data;
  },
};
