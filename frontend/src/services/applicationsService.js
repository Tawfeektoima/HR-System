import api from './api';

export const applicationsService = {
  applyForJob: async (applicationData) => {
    // Requires multipart/form-data for CV Upload
    const formData = new FormData();
    Object.keys(applicationData).forEach(key => {
      formData.append(key, applicationData[key]);
    });
    
    const response = await api.post('/applications', formData, {
      headers: {
        'Content-Type': 'multipart/form-data'
      }
    });
    return response.data;
  },
  
  getAllApplications: async (params = {}) => {
    const response = await api.get('/applications', { params });
    return response.data;
  },
  
  updateStatus: async (id, statusData) => {
    const response = await api.put(`/applications/${id}/status`, statusData);
    return response.data;
  },
  
  getApplicationById: async (id) => {
    const response = await api.get(`/applications/${id}`);
    return response.data;
  }
};
