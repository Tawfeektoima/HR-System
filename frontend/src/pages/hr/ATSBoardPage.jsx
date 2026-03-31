import React, { useState, useEffect } from 'react';
import { applicationsService } from '../../services/applicationsService';

const STAGES = ['Applied', 'PhoneInterview', 'TechnicalInterview', 'FinalInterview', 'Accepted', 'Rejected'];

const ATSBoardPage = () => {
  const [applications, setApplications] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetchApplications();
  }, []);

  const fetchApplications = async () => {
    try {
      const resp = await applicationsService.getAllApplications();
      if (resp.success) {
        // Handle both direct array or PagedResult structure
        const data = resp.data.items || resp.data;
        setApplications(Array.isArray(data) ? data : []);
      }
    } catch (err) {
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const handleDragStart = (e, appId) => {
    e.dataTransfer.setData("appId", appId);
  };

  const handleDrop = async (e, newStatus) => {
    e.preventDefault();
    const appId = e.dataTransfer.getData("appId");
    
    // Optimistic UI Update
    setApplications(prev => prev.map(app => 
      app.id.toString() === appId ? { ...app, status: newStatus } : app
    ));

    try {
      await applicationsService.updateStatus(appId, {
        status: newStatus,
        hrNotes: 'Moved via Kanban board',
      });
    } catch (err) {
      console.error("Failed to update status", err);
      // Revert if error occurs logically
      fetchApplications();
    }
  };

  const handleDragOver = (e) => {
    e.preventDefault();
  };

  if (loading) return <div className="animate-pulse-slow">Loading board...</div>;

  return (
    <div className="animate-fade-in" style={{ height: '100%', display: 'flex', flexDirection: 'column' }}>
      <h1 style={{ marginBottom: '2rem' }}>ATS Pipeline</h1>
      
      <div style={{ display: 'flex', gap: '1rem', overflowX: 'auto', flex: 1, paddingBottom: '1rem' }}>
        {STAGES.map(stage => (
          <div 
            key={stage} 
            className="glass"
            style={{ minWidth: '300px', backgroundColor: 'var(--surface)', borderRadius: 'var(--radius-md)', display: 'flex', flexDirection: 'column' }}
            onDrop={(e) => handleDrop(e, stage)}
            onDragOver={handleDragOver}
          >
            <div style={{ padding: '1rem', borderBottom: '1px solid var(--border)', fontWeight: 600 }}>
              {stage.replace(/([A-Z])/g, ' $1').trim()} 
              <span style={{ marginLeft: '0.5rem', fontSize: '0.75rem', padding: '0.1rem 0.4rem', backgroundColor: 'var(--border)', borderRadius: '1rem' }}>
                {applications.filter(a => a.status === stage).length}
              </span>
            </div>
            
            <div style={{ padding: '1rem', flex: 1, overflowY: 'auto', display: 'flex', flexDirection: 'column', gap: '0.75rem' }}>
              {applications.filter(a => a.status === stage).map(app => (
                <div 
                  key={app.id}
                  draggable
                  onDragStart={(e) => handleDragStart(e, app.id)}
                  className="hover-lift"
                  style={{ backgroundColor: 'var(--background)', padding: '1rem', borderRadius: 'var(--radius-lg)', border: '1px solid var(--border)', cursor: 'grab' }}
                >
                  <div style={{ fontWeight: 600, marginBottom: '0.25rem' }}>{app.candidateName}</div>
                  <div style={{ fontSize: '0.875rem', color: 'var(--text-muted)', marginBottom: '0.75rem' }}>{app.jobTitle}</div>
                  <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                    <div style={{ fontSize: '0.75rem', color: 'var(--primary)', padding: '0.1rem 0.5rem', background: 'rgba(79, 70, 229, 0.1)', borderRadius: 'var(--radius-full)' }}>
                      Score: {app.cvScore ?? 'N/A'}%
                    </div>
                  </div>
                </div>
              ))}
            </div>
          </div>
        ))}
      </div>
    </div>
  );
};

export default ATSBoardPage;
