import api from './api';

export const candidatesService = {
  getAll: async (params = {}) => {
    const response = await api.get('/candidates', { params });
    return response.data;
  },

  getById: async (id) => {
    const response = await api.get(`/candidates/${id}`);
    return response.data;
  },
};
