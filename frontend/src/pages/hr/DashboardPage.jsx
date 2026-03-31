import React, { useState, useEffect, useMemo } from 'react';
import {
  ResponsiveContainer,
  BarChart,
  Bar,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  Legend,
} from 'recharts';
import { analyticsService } from '../../services/analyticsService';

function formatStatusLabel(status) {
  if (!status) return '';
  return status.replace(/([A-Z])/g, ' $1').trim();
}

const DashboardPage = () => {
  const [stats, setStats] = useState(null);
  const [pipeline, setPipeline] = useState([]);
  const [monthly, setMonthly] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    (async () => {
      try {
        const [dash, pipe, months] = await Promise.all([
          analyticsService.getDashboardStats(),
          analyticsService.getPipelineFunnel(),
          analyticsService.getApplicationsPerMonth(6),
        ]);
        if (dash.success) setStats(dash.data);
        if (pipe.success) setPipeline(Array.isArray(pipe.data) ? pipe.data : []);
        if (months.success) setMonthly(Array.isArray(months.data) ? months.data : []);
      } catch (err) {
        console.error(err);
      } finally {
        setLoading(false);
      }
    })();
  }, []);

  const pipelineChartData = useMemo(
    () =>
      pipeline.map((p) => ({
        name: formatStatusLabel(p.status),
        count: p.count,
        pct: p.percentage,
      })),
    [pipeline]
  );

  const monthlyChartData = useMemo(
    () =>
      monthly.map((m) => ({
        month: m.monthLabel,
        applications: m.totalApplications,
        accepted: m.accepted,
        rejected: m.rejected,
      })),
    [monthly]
  );

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
        <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(180px, 1fr))', gap: '1.5rem', marginBottom: '2rem' }}>
          <Card title="Open jobs" value={stats.openJobs} color="var(--primary)" />
          <Card title="Total jobs" value={stats.totalJobs} color="#6366F1" />
          <Card title="Applications" value={stats.totalApplications} color="var(--secondary)" />
          <Card title="Candidates" value={stats.totalCandidates} color="#8B5CF6" />
          <Card title="Acceptance rate" value={`${stats.acceptanceRate ?? 0}%`} color="#F59E0B" />
          <Card title="Avg. days to hire" value={stats.avgDaysToHire != null ? `${stats.avgDaysToHire}` : '—'} color="var(--danger)" />
          <Card title="Upcoming interviews" value={stats.upcomingInterviews ?? 0} color="#10B981" />
        </div>
      )}

      <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(320px, 1fr))', gap: '1.5rem', marginBottom: '2rem' }}>
        <div className="glass" style={{ padding: '1.5rem', borderRadius: 'var(--radius-lg)', minHeight: '320px' }}>
          <h3 style={{ marginTop: 0, marginBottom: '1rem', fontSize: '1.1rem' }}>Pipeline (by status)</h3>
          {pipelineChartData.length === 0 ? (
            <p style={{ color: 'var(--text-muted)' }}>No application data yet.</p>
          ) : (
            <ResponsiveContainer width="100%" height={280}>
              <BarChart data={pipelineChartData} margin={{ top: 8, right: 8, left: 0, bottom: 32 }}>
                <CartesianGrid strokeDasharray="3 3" stroke="var(--border)" />
                <XAxis dataKey="name" tick={{ fontSize: 11 }} interval={0} angle={-20} textAnchor="end" height={60} />
                <YAxis allowDecimals={false} tick={{ fontSize: 11 }} />
                <Tooltip
                  contentStyle={{ background: 'var(--surface)', border: '1px solid var(--border)' }}
                  formatter={(value, name) => (name === 'pct' ? [`${value}%`, 'Share'] : [value, 'Count'])}
                />
                <Legend />
                <Bar dataKey="count" name="Applications" fill="var(--primary)" radius={[4, 4, 0, 0]} />
              </BarChart>
            </ResponsiveContainer>
          )}
        </div>

        <div className="glass" style={{ padding: '1.5rem', borderRadius: 'var(--radius-lg)', minHeight: '320px' }}>
          <h3 style={{ marginTop: 0, marginBottom: '1rem', fontSize: '1.1rem' }}>Applications (last 6 months)</h3>
          {monthlyChartData.length === 0 ? (
            <p style={{ color: 'var(--text-muted)' }}>No monthly data yet.</p>
          ) : (
            <ResponsiveContainer width="100%" height={280}>
              <BarChart data={monthlyChartData} margin={{ top: 8, right: 8, left: 0, bottom: 8 }}>
                <CartesianGrid strokeDasharray="3 3" stroke="var(--border)" />
                <XAxis dataKey="month" tick={{ fontSize: 11 }} />
                <YAxis allowDecimals={false} tick={{ fontSize: 11 }} />
                <Tooltip contentStyle={{ background: 'var(--surface)', border: '1px solid var(--border)' }} />
                <Legend />
                <Bar dataKey="applications" name="Total" fill="#6366F1" radius={[4, 4, 0, 0]} />
                <Bar dataKey="accepted" name="Accepted" fill="#10B981" radius={[4, 4, 0, 0]} />
                <Bar dataKey="rejected" name="Rejected" fill="#EF4444" radius={[4, 4, 0, 0]} />
              </BarChart>
            </ResponsiveContainer>
          )}
        </div>
      </div>
    </div>
  );
};

export default DashboardPage;
