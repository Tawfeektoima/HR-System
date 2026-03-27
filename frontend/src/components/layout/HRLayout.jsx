import React from 'react';
import { Outlet, Link, useNavigate, useLocation } from 'react-router-dom';

const HRLayout = () => {
  const navigate = useNavigate();
  const location = useLocation();
  const userStr = localStorage.getItem('user');
  const user = userStr ? JSON.parse(userStr) : { name: 'HR Admin' };

  const handleLogout = () => {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    navigate('/login');
  };

  const containerStyle = {
    display: 'flex',
    minHeight: '100vh',
    backgroundColor: 'var(--background)',
  };

  const sidebarStyle = {
    width: '260px',
    backgroundColor: 'var(--surface)',
    borderRight: '1px solid var(--border)',
    display: 'flex',
    flexDirection: 'column',
    position: 'fixed',
    height: '100vh',
    left: 0,
    top: 0,
  };

  const mainContentStyle = {
    flex: 1,
    marginLeft: '260px',
    display: 'flex',
    flexDirection: 'column',
  };

  const topbarStyle = {
    height: '70px',
    borderBottom: '1px solid var(--border)',
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'space-between',
    padding: '0 2rem',
    backgroundColor: 'rgba(var(--background-rgb), 0.8)',
    position: 'sticky',
    top: 0,
    zIndex: 10,
  };

  const navLinks = [
    { name: 'Dashboard', path: '/hr/dashboard', icon: '📊' },
    { name: 'Jobs', path: '/hr/jobs', icon: '💼' },
    { name: 'ATS Board', path: '/hr/applications', icon: '📋' },
    { name: 'Candidates', path: '/hr/candidates', icon: '👥' },
    { name: 'Interviews', path: '/hr/interviews', icon: '📅' },
  ];

  return (
    <div style={containerStyle} className="animate-fade-in">
      <aside style={sidebarStyle}>
        <div style={{ padding: '1.5rem', fontSize: '1.5rem', fontWeight: 700, color: 'var(--primary)' }}>
           IntelliHire HR
        </div>
        
        <nav style={{ flex: 1, display: 'flex', flexDirection: 'column', gap: '0.5rem', padding: '1rem' }}>
          {navLinks.map((link) => {
            const isActive = location.pathname.includes(link.path);
            return (
              <Link 
                key={link.path}
                to={link.path}
                style={{
                  padding: '0.75rem 1rem',
                  borderRadius: 'var(--radius-md)',
                  backgroundColor: isActive ? 'var(--primary)' : 'transparent',
                  color: isActive ? '#fff' : 'var(--text-main)',
                  display: 'flex',
                  alignItems: 'center',
                  gap: '0.75rem',
                  fontWeight: isActive ? 600 : 400,
                  transition: 'background-color 0.2s',
                }}
                className={!isActive ? 'hover-lift' : ''}
              >
                <span>{link.icon}</span> {link.name}
              </Link>
            );
          })}
        </nav>

        <div style={{ padding: '1.5rem', borderTop: '1px solid var(--border)' }}>
          <div style={{ display: 'flex', alignItems: 'center', gap: '0.75rem', marginBottom: '1rem' }}>
            <div style={{ width: '40px', height: '40px', borderRadius: '50%', backgroundColor: 'var(--primary)', color: '#fff', display: 'flex', alignItems: 'center', justifyContent: 'center', fontWeight: 'bold' }}>
              {(user.fullName || user.name || 'H').charAt(0)}
            </div>
            <div>
              <div style={{ fontWeight: 600, fontSize: '0.875rem' }}>{user.fullName || user.name || 'HR Admin'}</div>
              <div style={{ color: 'var(--text-muted)', fontSize: '0.75rem' }}>Administrator</div>
            </div>
          </div>
          <button 
            onClick={handleLogout}
            style={{ width: '100%', padding: '0.5rem', background: 'transparent', border: '1px solid var(--danger)', color: 'var(--danger)', borderRadius: 'var(--radius-md)', cursor: 'pointer' }}
          >
            Sign Out
          </button>
        </div>
      </aside>

      <main style={mainContentStyle}>
        <header className="glass" style={topbarStyle}>
          <h2 style={{ fontSize: '1.25rem', margin: 0 }}>HR Control Panel</h2>
          <div style={{ display: 'flex', gap: '1rem', alignItems: 'center' }}>
            {/* Theme Toggle placeholder */}
            <span style={{ cursor: 'pointer', fontSize: '1.25rem' }}>🌓</span>
            <span style={{ cursor: 'pointer', fontSize: '1.25rem' }}>🔔</span>
          </div>
        </header>
        
        <div style={{ padding: '2rem', flex: 1 }}>
          <Outlet />
        </div>
      </main>
    </div>
  );
};

export default HRLayout;
