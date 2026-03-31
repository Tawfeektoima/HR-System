import api from './api';

export const jobsService = {
  getPublicJobs: async (params = {}) => {
    const response = await api.get('/jobs/public', { params });
    return response.data;
  },
  
  getAllJobs: async (params = {}) => {
    const response = await api.get('/jobs', { params });
    return response.data;
  },
  
  getJobById: async (id) => {
    const response = await api.get(`/jobs/${id}`);
    return response.data;
  },
  
  createJob: async (jobData) => {
    const response = await api.post('/jobs', jobData);
    return response.data;
  },
  
  updateJob: async (id, jobData) => {
    const response = await api.put(`/jobs/${id}`, jobData);
    return response.data;
  },
  
  deleteJob: async (id) => {
    const response = await api.delete(`/jobs/${id}`);
    return response.data;
  },

  /** DeepSeek: compare all CVs for this job and update CvScore per application */
  compareCandidates: async (jobId) => {
    const response = await api.post(`/jobs/${jobId}/compare-candidates`);
    return response.data;
  },
};
