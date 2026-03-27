import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { authService } from '../../services/authService';
import Input from '../../components/ui/Input';
import Button from '../../components/ui/Button';

const LoginPage = () => {
  const navigate = useNavigate();
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const [isLoading, setIsLoading] = useState(false);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setIsLoading(true);
    setError('');
    try {
      const resp = await authService.login({ email, password });
      if (resp.success) {
        navigate('/hr/dashboard');
      } else {
        setError(resp.message || 'Login failed.');
      }
    } catch (err) {
      if (err.response?.status === 401) {
        setError('Invalid Email or Password');
      } else {
        setError('Oups, something went wrong!');
      }
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="animate-fade-in" style={{ minHeight: '100vh', display: 'flex', alignItems: 'center', justifyContent: 'center', background: 'var(--surface)' }}>
      <div className="glass hover-lift" style={{ padding: '3rem 2.5rem', borderRadius: 'var(--radius-lg)', width: '100%', maxWidth: '400px', backgroundColor: 'var(--background)' }}>
        <div style={{ textAlign: 'center', marginBottom: '2rem' }}>
          <h1 style={{ color: 'var(--primary)', marginBottom: '0.5rem' }}>IntelliHire AI</h1>
          <p style={{ color: 'var(--text-muted)' }}>Sign in to Human Resources Portal</p>
        </div>

        {error && <div style={{ padding: '0.75rem', marginBottom: '1.5rem', backgroundColor: 'rgba(239, 68, 68, 0.1)', color: 'var(--danger)', borderRadius: 'var(--radius-sm)', fontSize: '0.875rem' }}>{error}</div>}

        <form onSubmit={handleSubmit}>
          <Input 
            label="Email Address"
            type="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            placeholder="admin@hr.com"
            required
          />
          <Input 
            label="Password"
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            placeholder="••••••••"
            required
          />
          
          <Button type="submit" isLoading={isLoading} style={{ width: '100%', marginTop: '1rem' }} size="lg">
            Sign In
          </Button>
        </form>
      </div>
    </div>
  );
};

export default LoginPage;
