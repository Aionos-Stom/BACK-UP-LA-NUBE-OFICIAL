import { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import {
  Box, Card, CardContent, TextField, Button, Typography,
  Alert, CircularProgress, Divider, Chip, Grid, Collapse,
} from '@mui/material';
import ExpandMoreIcon from '@mui/icons-material/ExpandMore';
import ExpandLessIcon from '@mui/icons-material/ExpandLess';
import axios from 'axios';

const planesInfo = [
  { nombre: 'Gratuito', precio: '$0/mes', almacenamiento: '5 GB', color: '#555', features: ['2 jobs concurrentes', '3 políticas', 'Sin soporte prioritario'] },
  { nombre: 'Básico', precio: '$9.99/mes', almacenamiento: '50 GB', color: '#00d4ff', features: ['5 jobs concurrentes', '10 políticas', 'Backup automático'] },
  { nombre: 'Pro', precio: '$29.99/mes', almacenamiento: '500 GB', color: '#7c3aed', features: ['20 jobs concurrentes', '50 políticas', 'Soporte prioritario'] },
  { nombre: 'Empresa', precio: '$99.99/mes', almacenamiento: '5 TB', color: '#ff8c00', features: ['100 jobs concurrentes', 'Políticas ilimitadas', 'SLA garantizado'] },
];

const api = axios.create({ baseURL: '/api', withCredentials: true });

export default function Register() {
  const navigate = useNavigate();
  const [step, setStep] = useState(0);
  const [mostrarOpcionales, setMostrarOpcionales] = useState(false);
  const [form, setForm] = useState({
    nombre: '', email: '', password: '', confirmar: '',
    ciudad: '', pais: '', cargo: '', empresa: '',
  });
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  const handleRegister = async (e: React.FormEvent) => {
    e.preventDefault();
    if (form.password !== form.confirmar) { setError('Las contraseñas no coinciden.'); return; }
    if (form.password.length < 8) { setError('La contraseña debe tener al menos 8 caracteres.'); return; }
    setLoading(true);
    setError('');
    try {
      await api.post('/auth/register', {
        nombre: form.nombre,
        email: form.email,
        password: form.password,
        organizacionId: 1,
      });
      setStep(1);
    } catch (err: any) {
      setError(err.response?.data?.message || 'Error al registrarse.');
    } finally {
      setLoading(false);
    }
  };

  if (step === 1) {
    return (
      <Box sx={{ minHeight: '100vh', bgcolor: '#0a0e27', display: 'flex', alignItems: 'center', justifyContent: 'center', p: 2 }}>
        <Box sx={{ maxWidth: 920, width: '100%' }}>
          <Typography variant="h4" align="center" sx={{ mb: 1, fontWeight: 700, background: 'linear-gradient(135deg, #00d4ff 0%, #7c3aed 100%)', WebkitBackgroundClip: 'text', WebkitTextFillColor: 'transparent' }}>
            Elige tu Plan
          </Typography>
          <Typography align="center" sx={{ color: '#b0d4ff', mb: 4 }}>
            Empieza con el plan Gratuito o elige uno para acceder a todas las funciones
          </Typography>
          <Box sx={{ display: 'grid', gridTemplateColumns: { xs: '1fr', sm: '1fr 1fr', md: 'repeat(4, 1fr)' }, gap: 2, mb: 3 }}>
            {planesInfo.map(p => (
              <Card key={p.nombre} sx={{ bgcolor: '#0f1629', border: `1px solid ${p.color}`, borderRadius: 3, cursor: 'pointer', transition: 'transform 0.2s', '&:hover': { transform: 'translateY(-4px)' } }}
                onClick={() => navigate('/')}>
                <CardContent sx={{ p: 3 }}>
                  <Typography variant="h6" sx={{ color: p.color, fontWeight: 700 }}>{p.nombre}</Typography>
                  <Typography variant="h5" sx={{ color: '#fff', fontWeight: 800, my: 1 }}>{p.precio}</Typography>
                  <Chip label={p.almacenamiento} size="small" sx={{ bgcolor: `${p.color}22`, color: p.color, mb: 2 }} />
                  {p.features.map(f => (
                    <Typography key={f} variant="body2" sx={{ color: '#b0d4ff', mb: 0.5 }}>✓ {f}</Typography>
                  ))}
                  <Button fullWidth variant="outlined" sx={{ mt: 2, borderColor: p.color, color: p.color }}>
                    {p.nombre === 'Gratuito' ? 'Continuar gratis' : 'Seleccionar'}
                  </Button>
                </CardContent>
              </Card>
            ))}
          </Box>
          <Typography align="center" sx={{ color: '#555', fontSize: '0.8rem' }}>
            Puedes cambiar tu plan en cualquier momento desde tu perfil.
          </Typography>
        </Box>
      </Box>
    );
  }

  return (
    <Box sx={{ minHeight: '100vh', bgcolor: '#0a0e27', display: 'flex', alignItems: 'center', justifyContent: 'center', p: 2 }}>
      <Card sx={{ maxWidth: 560, width: '100%', bgcolor: '#0f1629', border: '1px solid rgba(0,212,255,0.2)', borderRadius: 3 }}>
        <CardContent sx={{ p: 4 }}>
          <Typography variant="h5" align="center" sx={{ fontWeight: 700, mb: 3, background: 'linear-gradient(135deg, #00d4ff 0%, #7c3aed 100%)', WebkitBackgroundClip: 'text', WebkitTextFillColor: 'transparent' }}>
            Crear Cuenta
          </Typography>
          {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}
          <form onSubmit={handleRegister}>
            <Grid container spacing={2}>
              <Grid item xs={12}>
                <TextField fullWidth label="Nombre completo" value={form.nombre}
                  onChange={e => setForm(f => ({ ...f, nombre: e.target.value }))} required />
              </Grid>
              <Grid item xs={12}>
                <TextField fullWidth label="Email" type="email" value={form.email}
                  onChange={e => setForm(f => ({ ...f, email: e.target.value }))} required />
              </Grid>
              <Grid item xs={12} sm={6}>
                <TextField fullWidth label="Contraseña" type="password" value={form.password}
                  onChange={e => setForm(f => ({ ...f, password: e.target.value }))}
                  required helperText="Mínimo 8 caracteres" />
              </Grid>
              <Grid item xs={12} sm={6}>
                <TextField fullWidth label="Confirmar contraseña" type="password" value={form.confirmar}
                  onChange={e => setForm(f => ({ ...f, confirmar: e.target.value }))} required />
              </Grid>

              {/* Campos opcionales */}
              <Grid item xs={12}>
                <Button
                  size="small"
                  onClick={() => setMostrarOpcionales(v => !v)}
                  endIcon={mostrarOpcionales ? <ExpandLessIcon /> : <ExpandMoreIcon />}
                  sx={{ color: '#b0d4ff', textTransform: 'none', px: 0 }}
                >
                  {mostrarOpcionales ? 'Ocultar información adicional' : 'Agregar información de perfil (opcional)'}
                </Button>
              </Grid>

              <Grid item xs={12}>
                <Collapse in={mostrarOpcionales}>
                  <Grid container spacing={2}>
                    <Grid item xs={12} sm={6}>
                      <TextField fullWidth label="Ciudad" value={form.ciudad}
                        onChange={e => setForm(f => ({ ...f, ciudad: e.target.value }))} placeholder="Bogotá" />
                    </Grid>
                    <Grid item xs={12} sm={6}>
                      <TextField fullWidth label="País" value={form.pais}
                        onChange={e => setForm(f => ({ ...f, pais: e.target.value }))} placeholder="Colombia" />
                    </Grid>
                    <Grid item xs={12} sm={6}>
                      <TextField fullWidth label="Cargo / Título" value={form.cargo}
                        onChange={e => setForm(f => ({ ...f, cargo: e.target.value }))} placeholder="Desarrollador Senior" />
                    </Grid>
                    <Grid item xs={12} sm={6}>
                      <TextField fullWidth label="Empresa" value={form.empresa}
                        onChange={e => setForm(f => ({ ...f, empresa: e.target.value }))} placeholder="Acme Corp" />
                    </Grid>
                  </Grid>
                </Collapse>
              </Grid>

              <Grid item xs={12}>
                <Button fullWidth type="submit" variant="contained" disabled={loading}
                  sx={{ py: 1.5, background: 'linear-gradient(135deg, #00d4ff 0%, #7c3aed 100%)', fontWeight: 700 }}>
                  {loading ? <CircularProgress size={24} /> : 'Crear Cuenta'}
                </Button>
              </Grid>
            </Grid>
          </form>
          <Divider sx={{ my: 3, borderColor: 'rgba(255,255,255,0.1)' }} />
          <Typography variant="body2" align="center" sx={{ color: '#b0d4ff' }}>
            ¿Ya tienes cuenta?{' '}
            <Link to="/login" style={{ color: '#00d4ff', textDecoration: 'none' }}>Inicia sesión</Link>
          </Typography>
        </CardContent>
      </Card>
    </Box>
  );
}
