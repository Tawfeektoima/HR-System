import React from 'react';
import { Outlet, Link } from 'react-router-dom';

const PublicLayout = () => {
  const headerStyle = {
    display: 'flex',
    justifyContent: 'space-between',
    alignItems: 'center',
    padding: '1.5rem 5%',
    backgroundColor: 'var(--surface)',
    borderBottom: '1px solid var(--border)',
    position: 'sticky',
    top: 0,
    zIndex: 50,
  };

  const logoStyle = {
    fontSize: '1.5rem',
    fontWeight: '700',
    color: 'var(--primary)',
    display: 'flex',
    alignItems: 'center',
    gap: '0.5rem',
  };

  const mainStyle = {
    minHeight: 'calc(100vh - 80px)', // adjust based on header height
    padding: '2rem 5%',
  };

  return (
    <div className="animate-fade-in" style={{ backgroundColor: 'var(--background)' }}>
      <header className="glass" style={headerStyle}>
        <Link to="/" style={logoStyle}>
          🚀 IntelliHire
        </Link>
        <nav style={{ display: 'flex', gap: '1.5rem', alignItems: 'center' }}>
          <Link to="/" style={{ color: 'var(--text-main)', fontWeight: 500 }}>Find Jobs</Link>
          <Link to="/login" style={{ color: 'var(--text-muted)', fontSize: '0.875rem' }}>HR Login</Link>
        </nav>
      </header>
      
      <main style={mainStyle}>
        <Outlet />
      </main>

      <footer style={{ padding: '2rem 5%', textAlign: 'center', color: 'var(--text-muted)', borderTop: '1px solid var(--border)' }}>
        <p>© {new Date().getFullYear()} IntelliHire AI System. All rights reserved.</p>
      </footer>
    </div>
  );
};

export default PublicLayout;
