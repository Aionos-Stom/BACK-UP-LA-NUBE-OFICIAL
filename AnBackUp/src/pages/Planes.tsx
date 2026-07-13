import { useState, useEffect } from 'react';
import {
  Box, Card, CardContent, Typography, Button, Chip, Grid, Alert,
  Dialog, DialogTitle, DialogContent, DialogActions, TextField, CircularProgress,
} from '@mui/material';
import CheckCircleIcon from '@mui/icons-material/CheckCircle';
import axios from 'axios';

const api = axios.create({ baseURL: '/api', withCredentials: true });

interface Plan {
  id: number;
  nombre: string;
  descripcion: string;
  precioMensual: number;
  limiteAlmacenamientoBytes: number;
  maxJobsConcurrentes: number;
  maxPoliticas: number;
  backupAutomatico: boolean;
  soportePrioritario: boolean;
  esGratuito: boolean;
}

export default function Planes() {
  const [planes, setPlanes] = useState<Plan[]>([]);
  const [miSuscripcion, setMiSuscripcion] = useState<any>(null);
  const [loading, setLoading] = useState(true);
  const [dialog, setDialog] = useState<{ open: boolean; plan: Plan | null }>({ open: false, plan: null });
  const [metodoPago, setMetodoPago] = useState('tarjeta');
  const [msg, setMsg] = useState('');
  const [err, setErr] = useState('');

  useEffect(() => {
    Promise.all([api.get('/planes'), api.get('/planes/mi-suscripcion')])
      .then(([p, s]) => {
        setPlanes(p.data);
        setMiSuscripcion(s.data);
      })
      .finally(() => setLoading(false));
  }, []);

  const suscribir = async () => {
    if (!dialog.plan) return;
    try {
      await api.post(`/planes/${dialog.plan.id}/suscribir`, { metodoPago });
      setMsg(`¡Suscrito exitosamente al plan ${dialog.plan.nombre}!`);
      setDialog({ open: false, plan: null });
      const s = await api.get('/planes/mi-suscripcion');
      setMiSuscripcion(s.data);
    } catch (ex: any) {
      setErr(ex.response?.data?.message || 'Error al suscribirse.');
    }
  };

  const colores: Record<string, string> = {
    Gratuito: '#555',
    Basico: '#00d4ff',
    Pro: '#7c3aed',
    Empresa: '#ff8c00',
    Ilimitado: '#ff006e',
  };

  const formatGB = (bytes: number) => {
    if (bytes > 1e12) return `${(bytes / 1e12).toFixed(0)} TB`;
    if (bytes > 1e9) return `${(bytes / 1e9).toFixed(0)} GB`;
    return `${(bytes / 1e6).toFixed(0)} MB`;
  };

  if (loading) {
    return (
      <Box sx={{ p: 4, display: 'flex', justifyContent: 'center' }}>
        <CircularProgress />
      </Box>
    );
  }

  return (
    <Box sx={{ p: 3 }}>
      <Typography
        variant="h4"
        sx={{
          mb: 1, fontWeight: 700,
          background: 'linear-gradient(135deg, #00d4ff 0%, #7c3aed 100%)',
          WebkitBackgroundClip: 'text',
          WebkitTextFillColor: 'transparent',
        }}
      >
        Planes de Almacenamiento
      </Typography>
      <Typography sx={{ color: '#b0d4ff', mb: 3 }}>Paga por uso — escala cuando lo necesites</Typography>

      {miSuscripcion?.plan && (
        <Alert severity="info" sx={{ mb: 3 }}>
          Plan activo: <strong>{miSuscripcion.plan}</strong> · {miSuscripcion.almacenamientoUsadoGB} GB usados
          de {miSuscripcion.almacenamientoLimiteGB} GB
          {miSuscripcion.esGratisAdmin && ' · Otorgado gratuitamente por un administrador'}
        </Alert>
      )}

      {msg && <Alert severity="success" sx={{ mb: 2 }} onClose={() => setMsg('')}>{msg}</Alert>}
      {err && <Alert severity="error" sx={{ mb: 2 }} onClose={() => setErr('')}>{err}</Alert>}

      <Grid container spacing={3}>
        {planes.filter(p => p.id !== undefined).map(plan => {
          const color = colores[plan.nombre] ?? '#00d4ff';
          const esActual = miSuscripcion?.plan === plan.nombre;
          return (
            <Grid item xs={12} sm={6} md={4} lg={2.4} key={plan.id}>
              <Card
                sx={{
                  bgcolor: '#0f1629',
                  border: `2px solid ${esActual ? color : color + '44'}`,
                  borderRadius: 3,
                  height: '100%',
                  display: 'flex',
                  flexDirection: 'column',
                  transition: 'transform 0.2s, box-shadow 0.2s',
                  '&:hover': {
                    transform: 'translateY(-4px)',
                    boxShadow: `0 8px 24px ${color}33`,
                  },
                }}
              >
                <CardContent sx={{ flexGrow: 1, p: 3 }}>
                  {esActual && (
                    <Chip label="Plan Actual" size="small" sx={{ bgcolor: `${color}33`, color, mb: 1 }} />
                  )}
                  <Typography variant="h6" sx={{ color, fontWeight: 700 }}>{plan.nombre}</Typography>
                  <Typography variant="h4" sx={{ color: '#fff', fontWeight: 800, my: 1 }}>
                    {plan.precioMensual === 0 ? 'Gratis' : `$${plan.precioMensual}`}
                    {plan.precioMensual > 0 && (
                      <span style={{ fontSize: '0.8rem', color: '#b0d4ff' }}>/mes</span>
                    )}
                  </Typography>
                  <Chip
                    label={formatGB(plan.limiteAlmacenamientoBytes)}
                    size="small"
                    sx={{ bgcolor: `${color}22`, color, mb: 2 }}
                  />

                  {[
                    `${plan.maxJobsConcurrentes} jobs simultáneos`,
                    `${plan.maxPoliticas === 9999 ? 'Ilimitadas' : plan.maxPoliticas} políticas`,
                    plan.backupAutomatico ? 'Backup automático' : null,
                    plan.soportePrioritario ? 'Soporte prioritario' : null,
                  ].filter(Boolean).map(f => (
                    <Box key={f as string} sx={{ display: 'flex', alignItems: 'center', mb: 0.5 }}>
                      <CheckCircleIcon sx={{ color, fontSize: 16, mr: 0.5 }} />
                      <Typography variant="body2" sx={{ color: '#b0d4ff' }}>{f}</Typography>
                    </Box>
                  ))}
                </CardContent>
                <Box sx={{ p: 2, pt: 0 }}>
                  <Button
                    fullWidth
                    variant={esActual ? 'outlined' : 'contained'}
                    disabled={esActual || plan.esGratuito}
                    sx={{
                      borderColor: color,
                      color: esActual ? color : '#000',
                      background: esActual ? 'transparent' : color,
                    }}
                    onClick={() => !esActual && !plan.esGratuito && setDialog({ open: true, plan })}
                  >
                    {esActual ? 'Plan actual' : plan.esGratuito ? 'Incluido' : 'Suscribirse'}
                  </Button>
                </Box>
              </Card>
            </Grid>
          );
        })}
      </Grid>

      <Dialog
        open={dialog.open}
        onClose={() => setDialog({ open: false, plan: null })}
        PaperProps={{ sx: { bgcolor: '#0f1629', border: '1px solid rgba(0,212,255,0.3)' } }}
      >
        <DialogTitle sx={{ color: '#00d4ff' }}>Suscribirse a {dialog.plan?.nombre}</DialogTitle>
        <DialogContent>
          <Typography sx={{ color: '#b0d4ff', mb: 2 }}>
            Se te cobrará <strong style={{ color: '#fff' }}>${dialog.plan?.precioMensual}/mes</strong>.
            Tu suscripción actual será cancelada.
          </Typography>
          <TextField
            fullWidth label="Método de pago" value={metodoPago}
            onChange={e => setMetodoPago(e.target.value)}
            select
            SelectProps={{ native: true }}
          >
            <option value="tarjeta">Tarjeta de crédito</option>
            <option value="paypal">PayPal</option>
            <option value="transferencia">Transferencia bancaria</option>
          </TextField>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setDialog({ open: false, plan: null })}>Cancelar</Button>
          <Button
            onClick={suscribir} variant="contained"
            sx={{ background: 'linear-gradient(135deg, #00d4ff, #7c3aed)' }}
          >
            Confirmar — ${dialog.plan?.precioMensual}/mes
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
}
