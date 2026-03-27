import React, { useState, useEffect } from 'react';
import { analyticsService } from '../../services/analyticsService';

const DashboardPage = () => {
  const [stats, setStats] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetchStats();
  }, []);

  const fetchStats = async () => {
    try {
      const resp = await analyticsService.getDashboardStats();
      if(resp.success) setStats(resp.data);
    } catch(err) {
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  if (loading) return <div className="animate-pulse-slow">Loading dashboard data...</div>;

  const Card = ({ title, value, color }) => (
    <div className="glass hover-lift" style={{ padding: '1.5rem', borderRadius: 'var(--radius-lg)', borderTop: `4px solid ${color}` }}>
      <h4 style={{ color: 'var(--text-muted)', fontSize: '0.875rem', margin: '0 0 0.5rem 0' }}>{title}</h4>
      <div style={{ fontSize: '2rem', fontWeight: 'bold', color: 'var(--text-main)' }}>{value}</div>
    </div>
  );

  return (
    <div className="animate-fade-in stagger-1">
      <h1 style={{ marginBottom: '2rem' }}>Overview</h1>
      
      {stats && (
        <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(200px, 1fr))', gap: '1.5rem', marginBottom: '3rem' }}>
          <Card title="Total Jobs" value={stats.totalJobs} color="var(--primary)" />
          <Card title="Total Applications" value={stats.totalApplications} color="var(--secondary)" />
          <Card title="Acceptance Rate" value={`${stats.acceptanceRate}%`} color="#F59E0B" />
          <Card title="Avg. Time to Hire" value={`${stats.avgTimeToHireDays} days`} color="var(--danger)" />
        </div>
      )}

      <div className="glass" style={{ padding: '2rem', borderRadius: 'var(--radius-lg)', minHeight: '300px', display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
        <p style={{ color: 'var(--text-muted)' }}>[Recharts Pipeline Funnel Placeholder: Run "npm install" inside frontend to enable Recharts features here later.]</p>
      </div>
    </div>
  );
};

export default DashboardPage;
