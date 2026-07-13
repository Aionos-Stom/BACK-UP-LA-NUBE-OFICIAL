import { Box, Typography, Button } from '@mui/material';
import { useNavigate } from 'react-router-dom';

export default function NotFound() {
  const navigate = useNavigate();
  return (
    <Box
      sx={{
        minHeight: '100vh',
        bgcolor: '#0a0e27',
        display: 'flex',
        flexDirection: 'column',
        alignItems: 'center',
        justifyContent: 'center',
      }}
    >
      <Typography
        variant="h1"
        sx={{
          fontSize: '8rem',
          fontWeight: 900,
          background: 'linear-gradient(135deg, #00d4ff 0%, #7c3aed 100%)',
          WebkitBackgroundClip: 'text',
          WebkitTextFillColor: 'transparent',
        }}
      >
        404
      </Typography>
      <Typography variant="h5" sx={{ color: '#b0d4ff', mb: 3 }}>
        Página no encontrada
      </Typography>
      <Button
        variant="contained"
        onClick={() => navigate('/')}
        sx={{ background: 'linear-gradient(135deg, #00d4ff, #7c3aed)' }}
      >
        Volver al inicio
      </Button>
    </Box>
  );
}
