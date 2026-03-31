import React, { useState, useEffect } from 'react';
import { interviewsService } from '../../services/interviewsService';

const InterviewsPage = () => {
  const [rows, setRows] = useState([]);
  const [loading, setLoading] = useState(true);
  const [upcomingOnly, setUpcomingOnly] = useState(false);

  const load = async () => {
    setLoading(true);
    try {
      const resp = await interviewsService.getAll(upcomingOnly);
      if (resp.success) {
        const data = resp.data;
        setRows(Array.isArray(data) ? data : []);
      }
    } catch (e) {
      console.error(e);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    load();
  }, [upcomingOnly]);

  if (loading) return <div className="animate-pulse-slow">Loading interviews…</div>;

  return (
    <div className="animate-fade-in">
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '2rem', flexWrap: 'wrap', gap: '1rem' }}>
        <h1 style={{ margin: 0 }}>Interviews</h1>
        <label style={{ display: 'flex', alignItems: 'center', gap: '0.5rem', cursor: 'pointer' }}>
          <input type="checkbox" checked={upcomingOnly} onChange={(e) => setUpcomingOnly(e.target.checked)} />
          Upcoming only
        </label>
      </div>

      <div className="glass" style={{ borderRadius: 'var(--radius-lg)', overflow: 'auto' }}>
        <table style={{ width: '100%', borderCollapse: 'collapse', fontSize: '0.9rem' }}>
          <thead>
            <tr style={{ textAlign: 'left', borderBottom: '1px solid var(--border)' }}>
              <th style={{ padding: '0.75rem 1rem' }}>Candidate</th>
              <th style={{ padding: '0.75rem 1rem' }}>Job</th>
              <th style={{ padding: '0.75rem 1rem' }}>Type</th>
              <th style={{ padding: '0.75rem 1rem' }}>When</th>
              <th style={{ padding: '0.75rem 1rem' }}>Result</th>
            </tr>
          </thead>
          <tbody>
            {rows.map((i) => (
              <tr key={i.id} style={{ borderBottom: '1px solid var(--border)' }}>
                <td style={{ padding: '0.75rem 1rem', fontWeight: 600 }}>{i.candidateName}</td>
                <td style={{ padding: '0.75rem 1rem' }}>{i.jobTitle}</td>
                <td style={{ padding: '0.75rem 1rem' }}>{i.type}</td>
                <td style={{ padding: '0.75rem 1rem', whiteSpace: 'nowrap' }}>
                  {i.scheduledAt ? new Date(i.scheduledAt).toLocaleString() : '—'}
                </td>
                <td style={{ padding: '0.75rem 1rem', color: 'var(--text-muted)' }}>{i.result || '—'}</td>
              </tr>
            ))}
          </tbody>
        </table>
        {rows.length === 0 && (
          <div style={{ padding: '2rem', textAlign: 'center', color: 'var(--text-muted)' }}>
            No interviews scheduled. Create interviews from the API or seed data.
          </div>
        )}
      </div>
    </div>
  );
};

export default InterviewsPage;
