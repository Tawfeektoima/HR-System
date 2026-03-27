import React from 'react';

const Button = React.forwardRef(({ 
  children, 
  variant = 'primary', 
  size = 'md', 
  className = '', 
  isLoading = false,
  ...props 
}, ref) => {
  
  const baseStyle = {
    display: 'inline-flex',
    alignItems: 'center',
    justifyContent: 'center',
    borderRadius: 'var(--radius-md)',
    fontWeight: '500',
    transition: 'all 0.2s',
    border: '1px solid transparent',
    outline: 'none',
  };

  const variants = {
    primary: {
      backgroundColor: 'var(--primary)',
      color: '#ffffff',
      border: '1px solid transparent'
    },
    secondary: {
      backgroundColor: 'var(--surface)',
      color: 'var(--text-main)',
      border: '1px solid var(--border)'
    },
    danger: {
      backgroundColor: 'var(--danger)',
      color: '#ffffff',
      border: '1px solid transparent'
    },
    ghost: {
      backgroundColor: 'transparent',
      color: 'var(--text-main)',
      border: '1px solid transparent'
    }
  };

  const sizes = {
    sm: { padding: '0.25rem 0.75rem', fontSize: '0.875rem' },
    md: { padding: '0.5rem 1rem', fontSize: '1rem' },
    lg: { padding: '0.75rem 1.5rem', fontSize: '1.125rem' }
  };

  // Combine inline styles as a fallback, though in real apps we'd use robust CSS modules/classes
  const composedStyle = {
    ...baseStyle,
    ...variants[variant],
    ...sizes[size],
    opacity: isLoading || props.disabled ? 0.7 : 1,
    cursor: isLoading || props.disabled ? 'not-allowed' : 'pointer',
  };

  return (
    <button
      ref={ref}
      style={composedStyle}
      disabled={isLoading || props.disabled}
      className={`hover-lift ${className}`}
      {...props}
    >
      {isLoading ? (
        <span style={{ marginRight: '0.5rem', animation: 'pulse 1s infinite' }}>⏳</span>
      ) : null}
      {children}
    </button>
  );
});

Button.displayName = 'Button';
export default Button;
