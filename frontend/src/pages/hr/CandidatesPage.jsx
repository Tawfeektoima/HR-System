import React, { useState, useEffect } from 'react';
import { candidatesService } from '../../services/candidatesService';

const CandidatesPage = () => {
  const [rows, setRows] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    (async () => {
      try {
        const resp = await candidatesService.getAll({ page: 1, pageSize: 100 });
        if (resp.success) {
          const data = resp.data?.items ?? resp.data;
          setRows(Array.isArray(data) ? data : []);
        }
      } catch (e) {
        console.error(e);
      } finally {
        setLoading(false);
      }
    })();
  }, []);

  if (loading) return <div className="animate-pulse-slow">Loading candidates…</div>;

  return (
    <div className="animate-fade-in">
      <h1 style={{ marginBottom: '2rem' }}>Candidates</h1>
      <div className="glass" style={{ borderRadius: 'var(--radius-lg)', overflow: 'auto' }}>
        <table style={{ width: '100%', borderCollapse: 'collapse', fontSize: '0.9rem' }}>
          <thead>
            <tr style={{ textAlign: 'left', borderBottom: '1px solid var(--border)' }}>
              <th style={{ padding: '0.75rem 1rem' }}>Name</th>
              <th style={{ padding: '0.75rem 1rem' }}>Email</th>
              <th style={{ padding: '0.75rem 1rem' }}>Score</th>
              <th style={{ padding: '0.75rem 1rem' }}>Experience (y)</th>
              <th style={{ padding: '0.75rem 1rem' }}>Education</th>
              <th style={{ padding: '0.75rem 1rem' }}>Joined</th>
            </tr>
          </thead>
          <tbody>
            {rows.map((c) => (
              <tr key={c.id} style={{ borderBottom: '1px solid var(--border)' }}>
                <td style={{ padding: '0.75rem 1rem', fontWeight: 600 }}>{c.fullName}</td>
                <td style={{ padding: '0.75rem 1rem' }}>{c.email}</td>
                <td style={{ padding: '0.75rem 1rem' }}>{c.totalScore != null ? `${c.totalScore}` : '—'}</td>
                <td style={{ padding: '0.75rem 1rem' }}>{c.experienceYears ?? '—'}</td>
                <td style={{ padding: '0.75rem 1rem', color: 'var(--text-muted)' }}>{c.educationLevel || '—'}</td>
                <td style={{ padding: '0.75rem 1rem', color: 'var(--text-muted)', whiteSpace: 'nowrap' }}>
                  {c.createdAt ? new Date(c.createdAt).toLocaleDateString() : '—'}
                </td>
              </tr>
            ))}
          </tbody>
        </table>
        {rows.length === 0 && (
          <div style={{ padding: '2rem', textAlign: 'center', color: 'var(--text-muted)' }}>No candidates yet.</div>
        )}
      </div>
    </div>
  );
};

export default CandidatesPage;
