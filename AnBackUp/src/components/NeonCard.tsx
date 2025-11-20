import { Box, BoxProps } from '@mui/material';
import { ReactNode } from 'react';

interface NeonCardProps extends BoxProps {
  children: ReactNode;
  glow?: boolean;
  float?: boolean;
}

export default function NeonCard({ children, glow = false, float = false, sx, ...props }: NeonCardProps) {
  return (
    <Box
      {...props}
      sx={{
        position: 'relative',
        background: 'linear-gradient(135deg, rgba(15, 22, 41, 0.9) 0%, rgba(10, 14, 39, 0.9) 100%)',
        border: '1px solid rgba(0, 212, 255, 0.3)',
        borderRadius: '16px',
        padding: 3,
        boxShadow: glow
          ? '0 8px 32px rgba(0, 212, 255, 0.2), 0 0 40px rgba(124, 58, 237, 0.15)'
          : '0 8px 32px rgba(0, 212, 255, 0.1)',
        transition: 'all 0.3s ease',
        animation: float ? 'float 6s ease-in-out infinite' : undefined,
        '&::before': {
          content: '""',
          position: 'absolute',
          top: 0,
          left: 0,
          right: 0,
          bottom: 0,
          borderRadius: '16px',
          padding: '1px',
          background: 'linear-gradient(135deg, rgba(0, 212, 255, 0.5), rgba(124, 58, 237, 0.5))',
          WebkitMask: 'linear-gradient(#fff 0 0) content-box, linear-gradient(#fff 0 0)',
          WebkitMaskComposite: 'xor',
          maskComposite: 'exclude',
          opacity: 0,
          transition: 'opacity 0.3s ease',
        },
        '&:hover': {
          borderColor: 'rgba(0, 212, 255, 0.6)',
          boxShadow: '0 12px 40px rgba(0, 212, 255, 0.3), 0 0 60px rgba(124, 58, 237, 0.2)',
          transform: 'translateY(-4px)',
          '&::before': {
            opacity: 1,
          },
        },
        ...sx,
      }}
    >
      {children}
    </Box>
  );
}

