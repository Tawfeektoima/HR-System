import React, { useState, useEffect } from 'react';
import { jobsService } from '../../services/jobsService';
import Button from '../../components/ui/Button';
import Input from '../../components/ui/Input';

const emptyForm = {
  title: '',
  department: '',
  description: '',
  requirements: '',
  salaryMin: '',
  salaryMax: '',
  location: '',
  isRemote: false,
  deadlineAt: '',
};

const HRJobsPage = () => {
  const [jobs, setJobs] = useState([]);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState('');
  const [showForm, setShowForm] = useState(false);
  const [form, setForm] = useState(emptyForm);
  const [comparingId, setComparingId] = useState(null);

  const load = async () => {
    setLoading(true);
    setError('');
    try {
      const resp = await jobsService.getAllJobs({ page: 1, pageSize: 100 });
      if (resp.success) {
        const data = resp.data?.items ?? resp.data;
        setJobs(Array.isArray(data) ? data : []);
      }
    } catch (e) {
      setError(e.response?.data?.message || 'Failed to load jobs.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    load();
  }, []);

  const handleFormChange = (e) => {
    const { id, value, type, checked } = e.target;
    setForm((prev) => ({
      ...prev,
      [id]: type === 'checkbox' ? checked : value,
    }));
  };

  const handleCreate = async (e) => {
    e.preventDefault();
    setSaving(true);
    setError('');
    try {
      const payload = {
        title: form.title.trim(),
        department: form.department.trim(),
        description: form.description.trim(),
        requirements: form.requirements.trim(),
        location: form.location.trim() || null,
        isRemote: form.isRemote,
        salaryMin: form.salaryMin ? Number(form.salaryMin) : null,
        salaryMax: form.salaryMax ? Number(form.salaryMax) : null,
        deadlineAt: form.deadlineAt ? new Date(form.deadlineAt).toISOString() : null,
      };
      const resp = await jobsService.createJob(payload);
      if (resp.success) {
        setForm(emptyForm);
        setShowForm(false);
        await load();
      } else {
        setError(resp.message || 'Could not create job.');
      }
    } catch (err) {
      setError(err.response?.data?.message || 'Could not create job.');
    } finally {
      setSaving(false);
    }
  };

  const handleDelete = async (id) => {
    if (!window.confirm('Delete this job? Only allowed for Admin accounts.')) return;
    try {
      const resp = await jobsService.deleteJob(id);
      if (resp.success) await load();
      else alert(resp.message || 'Delete failed');
    } catch (err) {
      alert(err.response?.data?.message || 'Delete failed (check your role).');
    }
  };

  const handleCompareCvs = async (jobId, appCount) => {
    if (appCount < 1) {
      alert('No applications to compare.');
      return;
    }
    if (!window.confirm('Compare all CVs for this job with DeepSeek? This may take a minute and will update CV scores.')) return;
    setComparingId(jobId);
    setError('');
    try {
      const resp = await jobsService.compareCandidates(jobId);
      if (resp.success && resp.data) {
        const lines = resp.data.scores?.map(
          (s) => `${s.candidateName}: ${s.score} — ${s.reason || ''}`
        ) || [];
        alert(`${resp.data.overallSummary || ''}\n\n${lines.join('\n')}`);
        await load(); // REFRESH THE LIST TO SHOW UPDATED SCORES/DATA
      } else {
        setError(resp.message || 'Comparison failed.');
      }
    } catch (err) {
      setError(err.response?.data?.message || err.message || 'Comparison failed. Set DeepSeek:ApiKey in appsettings.');
    } finally {
      setComparingId(null);
    }
  };

  if (loading) return <div className="animate-pulse-slow">Loading jobs…</div>;

  return (
    <div className="animate-fade-in">
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '2rem' }}>
        <h1 style={{ margin: 0 }}>Jobs</h1>
        <Button type="button" onClick={() => setShowForm((s) => !s)}>
          {showForm ? 'Cancel' : 'New job'}
        </Button>
      </div>

      {error && (
        <div style={{ padding: '1rem', background: 'rgba(239,68,68,0.1)', color: 'var(--danger)', borderRadius: 'var(--radius-md)', marginBottom: '1rem' }}>
          {error}
        </div>
      )}

      {showForm && (
        <form onSubmit={handleCreate} className="glass" style={{ padding: '1.5rem', borderRadius: 'var(--radius-lg)', marginBottom: '2rem' }}>
          <h3 style={{ marginTop: 0 }}>Create job</h3>
          <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(220px, 1fr))', gap: '1rem' }}>
            <Input id="title" label="Title" value={form.title} onChange={handleFormChange} required />
            <Input id="department" label="Department" value={form.department} onChange={handleFormChange} required />
            <Input id="location" label="Location" value={form.location} onChange={handleFormChange} />
            <Input id="salaryMin" type="number" label="Salary min" value={form.salaryMin} onChange={handleFormChange} />
            <Input id="salaryMax" type="number" label="Salary max" value={form.salaryMax} onChange={handleFormChange} />
            <Input id="deadlineAt" type="datetime-local" label="Deadline" value={form.deadlineAt} onChange={handleFormChange} />
          </div>
          <Input id="description" label="Description" value={form.description} onChange={handleFormChange} required />
          <Input id="requirements" label="Requirements" value={form.requirements} onChange={handleFormChange} required />
          <label style={{ display: 'flex', alignItems: 'center', gap: '0.5rem', marginBottom: '1rem' }}>
            <input id="isRemote" type="checkbox" checked={form.isRemote} onChange={handleFormChange} />
            Remote
          </label>
          <Button type="submit" isLoading={saving}>Create</Button>
        </form>
      )}

      <div className="glass" style={{ borderRadius: 'var(--radius-lg)', overflow: 'hidden' }}>
        <table style={{ width: '100%', borderCollapse: 'collapse', fontSize: '0.9rem' }}>
          <thead>
            <tr style={{ textAlign: 'left', borderBottom: '1px solid var(--border)', background: 'var(--surface)' }}>
              <th style={{ padding: '0.75rem 1rem' }}>Title</th>
              <th style={{ padding: '0.75rem 1rem' }}>Department</th>
              <th style={{ padding: '0.75rem 1rem' }}>Status</th>
              <th style={{ padding: '0.75rem 1rem' }}>Location</th>
              <th style={{ padding: '0.75rem 1rem' }}>Apps</th>
              <th style={{ padding: '0.75rem 1rem' }} />
            </tr>
          </thead>
          <tbody>
            {jobs.map((job) => (
              <tr key={job.id} style={{ borderBottom: '1px solid var(--border)' }}>
                <td style={{ padding: '0.75rem 1rem', fontWeight: 600 }}>{job.title}</td>
                <td style={{ padding: '0.75rem 1rem' }}>{job.department}</td>
                <td style={{ padding: '0.75rem 1rem' }}>
                  <span style={{ fontSize: '0.75rem', padding: '0.2rem 0.5rem', borderRadius: '999px', background: 'rgba(79,70,229,0.1)' }}>
                    {job.status}
                  </span>
                </td>
                <td style={{ padding: '0.75rem 1rem', color: 'var(--text-muted)' }}>{job.location || '—'}</td>
                <td style={{ padding: '0.75rem 1rem' }}>{job.applicationCount ?? 0}</td>
                <td style={{ padding: '0.75rem 1rem', display: 'flex', flexWrap: 'wrap', gap: '0.35rem' }}>
                  <button
                    type="button"
                    disabled={comparingId === job.id || (job.applicationCount ?? 0) < 1}
                    onClick={() => handleCompareCvs(job.id, job.applicationCount ?? 0)}
                    title="Uses DeepSeek to score each applicant vs this job"
                    style={{
                      background: 'transparent',
                      border: '1px solid var(--primary)',
                      color: 'var(--primary)',
                      borderRadius: 'var(--radius-sm)',
                      padding: '0.25rem 0.5rem',
                      cursor: comparingId === job.id ? 'wait' : 'pointer',
                      fontSize: '0.75rem',
                      opacity: (job.applicationCount ?? 0) < 1 ? 0.5 : 1,
                    }}
                  >
                    {comparingId === job.id ? 'Comparing…' : 'AI compare'}
                  </button>
                  <button
                    type="button"
                    onClick={() => handleDelete(job.id)}
                    style={{ background: 'transparent', border: '1px solid var(--danger)', color: 'var(--danger)', borderRadius: 'var(--radius-sm)', padding: '0.25rem 0.5rem', cursor: 'pointer', fontSize: '0.75rem' }}
                  >
                    Delete
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
        {jobs.length === 0 && (
          <div style={{ padding: '2rem', textAlign: 'center', color: 'var(--text-muted)' }}>No jobs yet.</div>
        )}
      </div>
    </div>
  );
};

export default HRJobsPage;
