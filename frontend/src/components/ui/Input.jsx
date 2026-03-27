import React from 'react';

const Input = React.forwardRef(({ 
  label, 
  error, 
  className = '', 
  id,
  type = 'text',
  ...props 
}, ref) => {
  
  const inputId = id || `input-${Math.random().toString(36).substring(2, 9)}`;

  const containerStyle = {
    display: 'flex',
    flexDirection: 'column',
    gap: '0.25rem',
    marginBottom: '1rem',
  };

  const labelStyle = {
    fontSize: '0.875rem',
    fontWeight: '500',
    color: 'var(--text-main)',
  };

  const inputStyle = {
    padding: '0.5rem 0.75rem',
    borderRadius: 'var(--radius-md)',
    border: `1px solid ${error ? 'var(--danger)' : 'var(--border)'}`,
    backgroundColor: 'var(--surface)',
    color: 'var(--text-main)',
    fontSize: '1rem',
    transition: 'border-color 0.2s',
    outline: 'none',
  };

  const errorStyle = {
    fontSize: '0.75rem',
    color: 'var(--danger)',
    marginTop: '0.25rem'
  };

  return (
    <div style={containerStyle} className={className}>
      {label && <label htmlFor={inputId} style={labelStyle}>{label}</label>}
      <input
        ref={ref}
        id={inputId}
        type={type}
        style={inputStyle}
        {...props}
      />
      {error && <span style={errorStyle}>{error}</span>}
    </div>
  );
});

Input.displayName = 'Input';
export default Input;
