import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { jobsService } from '../../services/jobsService';
import Button from '../../components/ui/Button';

const JobsListPage = () => {
  const [jobs, setJobs] = useState([]);
  const [loading, setLoading] = useState(true);
  const navigate = useNavigate();

  useEffect(() => {
    fetchJobs();
  }, []);

  const fetchJobs = async () => {
    try {
      const resp = await jobsService.getPublicJobs();
      if(resp.success) {
        setJobs(resp.data);
      }
    } catch (err) {
      console.error('Failed to fetch jobs', err);
    } finally {
      setLoading(false);
    }
  };

  if (loading) return <div className="p-8 text-center animate-pulse-slow">Loading opportunities...</div>;

  return (
    <div className="animate-fade-in" style={{ maxWidth: '1000px', margin: '0 auto' }}>
      <div style={{ textAlign: 'center', marginBottom: '3rem' }}>
        <h1 style={{ fontSize: '2.5rem', color: 'var(--primary)' }}>Find Your Next Big Opportunity</h1>
        <p style={{ color: 'var(--text-muted)' }}>AI-powered recruitment system connecting top talent with amazing jobs.</p>
      </div>

      {jobs.length === 0 ? (
        <div style={{ padding: '3rem', textAlign: 'center', background: 'var(--surface)', borderRadius: 'var(--radius-lg)' }}>
          <h3>No open positions right now.</h3>
          <p style={{ color: 'var(--text-muted)' }}>Please check back later.</p>
        </div>
      ) : (
        <div style={{ display: 'grid', gap: '1.5rem', gridTemplateColumns: 'repeat(auto-fill, minmax(300px, 1fr))' }}>
          {jobs.map(job => (
            <div key={job.id} className="hover-lift glass" style={{ padding: '1.5rem', borderRadius: 'var(--radius-lg)' }}>
              <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: '0.5rem' }}>
                <span style={{ fontSize: '0.75rem', padding: '0.2rem 0.5rem', backgroundColor: 'rgba(79, 70, 229, 0.1)', color: 'var(--primary)', borderRadius: 'var(--radius-full)' }}>
                  {job.department}
                </span>
                {job.isRemote && <span style={{ fontSize: '0.75rem', padding: '0.2rem 0.5rem', backgroundColor: 'rgba(16, 185, 129, 0.1)', color: 'var(--secondary)', borderRadius: 'var(--radius-full)' }}>Remote</span>}
              </div>
              <h3 style={{ fontSize: '1.25rem', marginBottom: '0.5rem' }}>{job.title}</h3>
              <p style={{ color: 'var(--text-muted)', fontSize: '0.875rem', marginBottom: '1.5rem', display: '-webkit-box', WebkitLineClamp: 3, WebkitBoxOrient: 'vertical', overflow: 'hidden' }}>
                {job.description}
              </p>
              <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                <span style={{ fontSize: '0.875rem', fontWeight: 600 }}>{job.location}</span>
                <Button size="sm" onClick={() => navigate(`/jobs/${job.id}/apply`)}>Apply Now</Button>
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
};

export default JobsListPage;
