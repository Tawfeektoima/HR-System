import React, { useState, useCallback } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useDropzone } from 'react-dropzone';
import { applicationsService } from '../../services/applicationsService';
import Input from '../../components/ui/Input';
import Button from '../../components/ui/Button';

const ApplyPage = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  
  const [formData, setFormData] = useState({
    firstName: '',
    lastName: '',
    email: '',
    phone: '',
    linkedInUrl: ''
  });
  const [file, setFile] = useState(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  const onDrop = useCallback(acceptedFiles => {
    if (acceptedFiles.length > 0) {
      setFile(acceptedFiles[0]);
    }
  }, []);
  
  const { getRootProps, getInputProps, isDragActive } = useDropzone({
    onDrop,
    accept: {
      'application/pdf': ['.pdf'],
      'application/vnd.openxmlformats-officedocument.wordprocessingml.document': ['.docx']
    },
    maxFiles: 1
  });

  const handleChange = (e) => {
    const { id, value } = e.target;
    // Input component adds random ID if not provided, so we'll match by name or we provide explicit IDs
    setFormData(prev => ({ ...prev, [id]: value }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (!file) {
      setError('Please attach your CV/Resume');
      return;
    }
    
    setLoading(true);
    setError('');
    
    try {
      const resp = await applicationsService.applyForJob({
        jobId: id,
        ...formData,
        cvFile: file
      });
      
      if (resp.success) {
        alert('Application submitted successfully!'); // Will replace with Toast later
        navigate('/');
      } else {
        setError(resp.message || 'Failed to submit application.');
      }
    } catch (err) {
      setError(err.response?.data?.message || 'An unexpected error occurred.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="animate-fade-in" style={{ maxWidth: '600px', margin: '0 auto' }}>
      <h1 style={{ marginBottom: '0.5rem' }}>Submit Your Application</h1>
      <p style={{ color: 'var(--text-muted)', marginBottom: '2rem' }}>Fill out the form below to apply for Job #{id}</p>

      {error && <div style={{ padding: '1rem', backgroundColor: 'var(--danger)', color: 'white', borderRadius: 'var(--radius-md)', marginBottom: '1.5rem' }}>{error}</div>}

      <form onSubmit={handleSubmit} className="glass" style={{ padding: '2rem', borderRadius: 'var(--radius-lg)' }}>
        <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '1rem' }}>
          <Input id="firstName" label="First Name" value={formData.firstName} onChange={handleChange} required />
          <Input id="lastName" label="Last Name" value={formData.lastName} onChange={handleChange} required />
        </div>
        <Input id="email" type="email" label="Email Address" value={formData.email} onChange={handleChange} required />
        <Input id="phone" type="tel" label="Phone Number" value={formData.phone} onChange={handleChange} required />
        <Input id="linkedInUrl" type="url" label="LinkedIn Profile (Optional)" value={formData.linkedInUrl} onChange={handleChange} />

        <div style={{ marginTop: '1.5rem', marginBottom: '1.5rem' }}>
          <label style={{ fontSize: '0.875rem', fontWeight: 500, marginBottom: '0.5rem', display: 'block' }}>Resume / CV</label>
          <div 
            {...getRootProps()} 
            style={{ 
              border: `2px dashed ${isDragActive ? 'var(--primary)' : 'var(--border)'}`, 
              borderRadius: 'var(--radius-md)', 
              padding: '2rem', 
              textAlign: 'center',
              cursor: 'pointer',
              backgroundColor: isDragActive ? 'rgba(79, 70, 229, 0.05)' : 'var(--surface)',
              transition: 'all 0.2s'
            }}
          >
            <input {...getInputProps()} />
            {file ? (
              <p style={{ color: 'var(--primary)', fontWeight: 600 }}>📄 {file.name}</p>
            ) : (
              <p style={{ color: 'var(--text-muted)' }}>Drag & drop your PDF or DOCX here, or click to select</p>
            )}
          </div>
        </div>

        <Button type="submit" isLoading={loading} style={{ width: '100%' }} size="lg">Submit Application</Button>
      </form>
    </div>
  );
};

export default ApplyPage;
