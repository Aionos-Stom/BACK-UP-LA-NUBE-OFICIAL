import { useState, useEffect, useRef } from 'react';
import {
  Box, Card, CardContent, Typography, Grid, TextField, Button, Chip, Alert,
  LinearProgress, List, ListItem, ListItemText, ListItemSecondaryAction,
  IconButton, Dialog, DialogTitle, DialogContent, DialogActions, CircularProgress,
  Avatar, Tooltip, Divider,
} from '@mui/material';
import DeleteIcon from '@mui/icons-material/Delete';
import SecurityIcon from '@mui/icons-material/Security';
import StorageIcon from '@mui/icons-material/Storage';
import PersonIcon from '@mui/icons-material/Person';
import CameraAltIcon from '@mui/icons-material/CameraAlt';
import DeleteForeverIcon from '@mui/icons-material/DeleteForever';
import LinkedInIcon from '@mui/icons-material/LinkedIn';
import BusinessIcon from '@mui/icons-material/Business';
import LocationOnIcon from '@mui/icons-material/LocationOn';
import WorkIcon from '@mui/icons-material/Work';
import axios from 'axios';
import { useAuth } from '../contexts/AuthContext';

const api = axios.create({ baseURL: '/api', withCredentials: true });

export default function Perfil() {
  const { logout } = useAuth();
  const [perfil, setPerfil] = useState<any>(null);
  const [sesiones, setSesiones] = useState<any[]>([]);
  const [pagos, setPagos] = useState<any[]>([]);
  const [form, setForm] = useState({
    nombre: '', telefono: '', bio: '', ciudad: '', pais: '',
    cargo: '', empresa: '', linkedin: '', fechaNacimiento: '',
  });
  const [pwForm, setPwForm] = useState({ actual: '', nuevo: '', confirmar: '' });
  const [msg, setMsg] = useState('');
  const [err, setErr] = useState('');
  const [loading, setLoading] = useState(true);
  const [savingFoto, setSavingFoto] = useState(false);
  const [dialogLogout, setDialogLogout] = useState(false);
  const fileInputRef = useRef<HTMLInputElement>(null);

  const cargarPerfil = async () => {
    const [p, s, pg] = await Promise.all([
      api.get('/perfil'),
      api.get('/perfil/sesiones'),
      api.get('/perfil/historial-pagos'),
    ]);
    setPerfil(p.data);
    setSesiones(s.data);
    setPagos(pg.data);
    setForm({
      nombre: p.data.nombre || '',
      telefono: p.data.telefono || '',
      bio: p.data.bio || '',
      ciudad: p.data.ciudad || '',
      pais: p.data.pais || '',
      cargo: p.data.cargo || '',
      empresa: p.data.empresa || '',
      linkedin: p.data.linkedin || '',
      fechaNacimiento: p.data.fechaNacimiento ? p.data.fechaNacimiento.substring(0, 10) : '',
    });
  };

  useEffect(() => {
    cargarPerfil().finally(() => setLoading(false));
  }, []);

  const guardarPerfil = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      await api.put('/perfil', {
        nombre: form.nombre,
        telefono: form.telefono,
        bio: form.bio || null,
        ciudad: form.ciudad || null,
        pais: form.pais || null,
        cargo: form.cargo || null,
        empresa: form.empresa || null,
        linkedin: form.linkedin || null,
        fechaNacimiento: form.fechaNacimiento || null,
      });
      setMsg('Perfil actualizado correctamente.');
      setErr('');
      await cargarPerfil();
    } catch {
      setErr('Error al actualizar el perfil.');
    }
  };

  const subirFoto = async (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (!file) return;
    if (file.size > 400_000) {
      setErr('La imagen no puede superar 400KB. Redúcela antes de subir.');
      return;
    }
    setSavingFoto(true);
    const reader = new FileReader();
    reader.onload = async () => {
      try {
        await api.post('/perfil/foto', { fotoBase64: reader.result as string });
        setMsg('Foto de perfil actualizada.');
        setErr('');
        await cargarPerfil();
      } catch (ex: any) {
        setErr(ex.response?.data?.message || 'Error al subir la foto.');
      } finally {
        setSavingFoto(false);
        if (fileInputRef.current) fileInputRef.current.value = '';
      }
    };
    reader.readAsDataURL(file);
  };

  const eliminarFoto = async () => {
    try {
      await api.delete('/perfil/foto');
      setMsg('Foto eliminada.');
      await cargarPerfil();
    } catch { setErr('Error al eliminar la foto.'); }
  };

  const cambiarPassword = async (e: React.FormEvent) => {
    e.preventDefault();
    if (pwForm.nuevo !== pwForm.confirmar) { setErr('Las contraseñas no coinciden.'); return; }
    try {
      await api.post('/auth/change-password', { passwordActual: pwForm.actual, nuevoPassword: pwForm.nuevo });
      setMsg('Contraseña cambiada correctamente.');
      setPwForm({ actual: '', nuevo: '', confirmar: '' });
      setErr('');
    } catch (ex: any) {
      setErr(ex.response?.data?.message || 'Error al cambiar contraseña.');
    }
  };

  const revocarSesion = async (id: number) => {
    await api.delete(`/perfil/sesiones/${id}`);
    setSesiones(s => s.filter(x => x.id !== id));
  };

  const cerrarTodasSesiones = async () => {
    await api.delete('/perfil/sesiones');
    setDialogLogout(false);
    logout();
  };

  if (loading) {
    return <Box sx={{ p: 4, display: 'flex', justifyContent: 'center' }}><CircularProgress /></Box>;
  }

  const iniciales = perfil?.nombre?.split(' ').map((n: string) => n[0]).join('').substring(0, 2).toUpperCase() || '??';

  return (
    <Box sx={{ maxWidth: 1100, mx: 'auto' }}>
      <Typography variant="h4" sx={{ mb: 3, fontWeight: 800, background: 'linear-gradient(135deg, #00d4ff 0%, #7c3aed 100%)', WebkitBackgroundClip: 'text', WebkitTextFillColor: 'transparent' }}>
        Mi Perfil
      </Typography>

      {msg && <Alert severity="success" sx={{ mb: 2 }} onClose={() => setMsg('')}>{msg}</Alert>}
      {err && <Alert severity="error" sx={{ mb: 2 }} onClose={() => setErr('')}>{err}</Alert>}

      <Grid container spacing={3}>
        {/* Panel izquierdo: avatar + plan */}
        <Grid item xs={12} md={4}>
          {/* Avatar con foto */}
          <Card sx={{ bgcolor: '#0f1629', border: '1px solid rgba(0,212,255,0.2)', mb: 2 }}>
            <CardContent sx={{ textAlign: 'center', py: 3 }}>
              <Box sx={{ position: 'relative', display: 'inline-block', mb: 2 }}>
                <Avatar
                  src={perfil?.fotoPerfil || undefined}
                  sx={{ width: 100, height: 100, mx: 'auto', fontSize: '2rem', background: 'linear-gradient(135deg, #00d4ff 0%, #7c3aed 100%)', border: '3px solid rgba(0,212,255,0.4)' }}
                >
                  {!perfil?.fotoPerfil && iniciales}
                </Avatar>
                {savingFoto && (
                  <CircularProgress size={24} sx={{ position: 'absolute', bottom: 0, right: 0 }} />
                )}
                <Tooltip title="Cambiar foto">
                  <IconButton
                    size="small"
                    onClick={() => fileInputRef.current?.click()}
                    sx={{ position: 'absolute', bottom: -4, right: -4, bgcolor: '#00d4ff', color: '#000', width: 28, height: 28, '&:hover': { bgcolor: '#00b8d9' } }}
                  >
                    <CameraAltIcon sx={{ fontSize: 14 }} />
                  </IconButton>
                </Tooltip>
              </Box>
              <input ref={fileInputRef} type="file" accept="image/jpeg,image/png,image/webp" style={{ display: 'none' }} onChange={subirFoto} />

              <Typography variant="h6" sx={{ color: '#fff', fontWeight: 700 }}>{perfil?.nombre}</Typography>
              {perfil?.cargo && <Typography sx={{ color: '#b0d4ff', fontSize: '0.85rem' }}>{perfil.cargo}</Typography>}
              {perfil?.empresa && (
                <Typography sx={{ color: '#7c3aed', fontSize: '0.8rem', display: 'flex', alignItems: 'center', justifyContent: 'center', gap: 0.5, mt: 0.5 }}>
                  <BusinessIcon sx={{ fontSize: 14 }} />{perfil.empresa}
                </Typography>
              )}
              <Typography sx={{ color: '#b0d4ff', fontSize: '0.82rem', mt: 0.5 }}>{perfil?.email}</Typography>
              <Chip label={perfil?.rol?.toUpperCase()} size="small" color="primary" sx={{ mt: 1 }} />

              {(perfil?.ciudad || perfil?.pais) && (
                <Typography sx={{ color: '#555', fontSize: '0.78rem', mt: 1, display: 'flex', alignItems: 'center', justifyContent: 'center', gap: 0.4 }}>
                  <LocationOnIcon sx={{ fontSize: 13 }} />
                  {[perfil.ciudad, perfil.pais].filter(Boolean).join(', ')}
                </Typography>
              )}
              {perfil?.linkedin && (
                <Button
                  href={perfil.linkedin.startsWith('http') ? perfil.linkedin : `https://${perfil.linkedin}`}
                  target="_blank"
                  size="small"
                  startIcon={<LinkedInIcon />}
                  sx={{ mt: 1, color: '#0077b5', fontSize: '0.75rem', textTransform: 'none' }}
                >
                  LinkedIn
                </Button>
              )}
              {perfil?.bio && (
                <Typography sx={{ color: '#b0d4ff', fontSize: '0.82rem', mt: 1.5, fontStyle: 'italic', px: 1 }}>
                  "{perfil.bio}"
                </Typography>
              )}
              {perfil?.fotoPerfil && (
                <Button size="small" color="error" startIcon={<DeleteForeverIcon />} onClick={eliminarFoto} sx={{ mt: 1.5, fontSize: '0.75rem' }}>
                  Quitar foto
                </Button>
              )}
              {perfil?.lastLogin && (
                <Typography sx={{ color: '#555', fontSize: '0.72rem', mt: 1.5 }}>
                  Último acceso: {new Date(perfil.lastLogin).toLocaleString('es-ES')}
                </Typography>
              )}
            </CardContent>
          </Card>

          {/* Plan */}
          {perfil?.plan && (
            <Card sx={{ bgcolor: '#0f1629', border: '1px solid rgba(124,58,237,0.3)', mb: 2 }}>
              <CardContent>
                <Box sx={{ display: 'flex', alignItems: 'center', mb: 1 }}>
                  <StorageIcon sx={{ color: '#7c3aed', mr: 1 }} />
                  <Typography variant="subtitle1" sx={{ color: '#7c3aed', fontWeight: 700 }}>
                    Plan {perfil.plan.nombre}
                  </Typography>
                </Box>
                {perfil.plan.esGratisAdmin && <Chip label="Otorgado por Admin" size="small" color="warning" sx={{ mb: 1 }} />}
                <Typography sx={{ color: '#b0d4ff', fontSize: '0.8rem' }}>
                  {perfil.plan.usadoGB} GB / {perfil.plan.limiteGB} GB usados
                </Typography>
                <LinearProgress
                  variant="determinate"
                  value={Math.min(perfil.plan.porcentajeUso, 100)}
                  sx={{ mt: 1, height: 8, borderRadius: 4, bgcolor: 'rgba(124,58,237,0.2)', '& .MuiLinearProgress-bar': { background: perfil.plan.porcentajeUso > 80 ? 'linear-gradient(90deg, #ff8c00, #ff006e)' : 'linear-gradient(90deg, #00d4ff, #7c3aed)' } }}
                />
                <Typography sx={{ color: '#555', fontSize: '0.75rem', mt: 0.5 }}>{perfil.plan.porcentajeUso}% usado</Typography>
                {perfil.plan.fechaFin && (
                  <Typography sx={{ color: '#555', fontSize: '0.75rem' }}>
                    Vence: {new Date(perfil.plan.fechaFin).toLocaleDateString('es-ES')}
                  </Typography>
                )}
              </CardContent>
            </Card>
          )}
        </Grid>

        <Grid item xs={12} md={8}>
          {/* Editar perfil completo */}
          <Card sx={{ bgcolor: '#0f1629', border: '1px solid rgba(0,212,255,0.1)', mb: 3 }}>
            <CardContent>
              <Box sx={{ display: 'flex', alignItems: 'center', mb: 2.5 }}>
                <PersonIcon sx={{ color: '#00d4ff', mr: 1 }} />
                <Typography variant="h6" sx={{ color: '#00d4ff' }}>Información Personal</Typography>
              </Box>
              <form onSubmit={guardarPerfil}>
                <Grid container spacing={2}>
                  <Grid item xs={12} sm={6}>
                    <TextField fullWidth label="Nombre completo" value={form.nombre}
                      onChange={e => setForm(f => ({ ...f, nombre: e.target.value }))} required />
                  </Grid>
                  <Grid item xs={12} sm={6}>
                    <TextField fullWidth label="Teléfono" value={form.telefono}
                      onChange={e => setForm(f => ({ ...f, telefono: e.target.value }))} placeholder="+57 300 000 0000" />
                  </Grid>
                  <Grid item xs={12}>
                    <TextField fullWidth multiline rows={2} label="Biografía / Descripción personal" value={form.bio}
                      onChange={e => setForm(f => ({ ...f, bio: e.target.value }))} placeholder="Cuéntanos sobre ti..."
                      inputProps={{ maxLength: 500 }} helperText={`${form.bio.length}/500`} />
                  </Grid>

                  <Grid item xs={12}><Divider sx={{ borderColor: 'rgba(255,255,255,0.08)' }} /></Grid>

                  <Grid item xs={12}>
                    <Typography variant="caption" sx={{ color: '#b0d4ff66', textTransform: 'uppercase', letterSpacing: 0.5, display: 'flex', alignItems: 'center', gap: 0.5 }}>
                      <WorkIcon sx={{ fontSize: 14 }} /> Información Profesional
                    </Typography>
                  </Grid>
                  <Grid item xs={12} sm={6}>
                    <TextField fullWidth label="Cargo / Título" value={form.cargo}
                      onChange={e => setForm(f => ({ ...f, cargo: e.target.value }))} placeholder="Ej: Desarrollador Senior" />
                  </Grid>
                  <Grid item xs={12} sm={6}>
                    <TextField fullWidth label="Empresa / Organización" value={form.empresa}
                      onChange={e => setForm(f => ({ ...f, empresa: e.target.value }))} placeholder="Ej: Acme Corp" />
                  </Grid>
                  <Grid item xs={12}>
                    <TextField fullWidth label="LinkedIn" value={form.linkedin}
                      onChange={e => setForm(f => ({ ...f, linkedin: e.target.value }))}
                      placeholder="linkedin.com/in/tu-perfil"
                      InputProps={{ startAdornment: <LinkedInIcon sx={{ color: '#0077b5', mr: 1, fontSize: 20 }} /> }} />
                  </Grid>

                  <Grid item xs={12}><Divider sx={{ borderColor: 'rgba(255,255,255,0.08)' }} /></Grid>

                  <Grid item xs={12}>
                    <Typography variant="caption" sx={{ color: '#b0d4ff66', textTransform: 'uppercase', letterSpacing: 0.5, display: 'flex', alignItems: 'center', gap: 0.5 }}>
                      <LocationOnIcon sx={{ fontSize: 14 }} /> Ubicación y Datos Personales
                    </Typography>
                  </Grid>
                  <Grid item xs={12} sm={6}>
                    <TextField fullWidth label="Ciudad" value={form.ciudad}
                      onChange={e => setForm(f => ({ ...f, ciudad: e.target.value }))} placeholder="Bogotá" />
                  </Grid>
                  <Grid item xs={12} sm={6}>
                    <TextField fullWidth label="País" value={form.pais}
                      onChange={e => setForm(f => ({ ...f, pais: e.target.value }))} placeholder="Colombia" />
                  </Grid>
                  <Grid item xs={12} sm={6}>
                    <TextField fullWidth type="date" label="Fecha de nacimiento" value={form.fechaNacimiento}
                      onChange={e => setForm(f => ({ ...f, fechaNacimiento: e.target.value }))}
                      InputLabelProps={{ shrink: true }} />
                  </Grid>

                  <Grid item xs={12}>
                    <Button type="submit" variant="contained" size="large"
                      sx={{ background: 'linear-gradient(135deg, #00d4ff, #7c3aed)', fontWeight: 700 }}>
                      Guardar Cambios
                    </Button>
                  </Grid>
                </Grid>
              </form>
            </CardContent>
          </Card>

          {/* Seguridad */}
          <Card sx={{ bgcolor: '#0f1629', border: '1px solid rgba(124,58,237,0.2)', mb: 3 }}>
            <CardContent>
              <Box sx={{ display: 'flex', alignItems: 'center', mb: 2 }}>
                <SecurityIcon sx={{ color: '#7c3aed', mr: 1 }} />
                <Typography variant="h6" sx={{ color: '#7c3aed' }}>Seguridad</Typography>
              </Box>
              <form onSubmit={cambiarPassword}>
                <Grid container spacing={2}>
                  <Grid item xs={12}>
                    <TextField fullWidth type="password" label="Contraseña actual" value={pwForm.actual}
                      onChange={e => setPwForm(f => ({ ...f, actual: e.target.value }))} />
                  </Grid>
                  <Grid item xs={12} sm={6}>
                    <TextField fullWidth type="password" label="Nueva contraseña" value={pwForm.nuevo}
                      onChange={e => setPwForm(f => ({ ...f, nuevo: e.target.value }))} />
                  </Grid>
                  <Grid item xs={12} sm={6}>
                    <TextField fullWidth type="password" label="Confirmar contraseña" value={pwForm.confirmar}
                      onChange={e => setPwForm(f => ({ ...f, confirmar: e.target.value }))} />
                  </Grid>
                  <Grid item xs={12}>
                    <Button type="submit" variant="outlined" color="secondary">Cambiar Contraseña</Button>
                  </Grid>
                </Grid>
              </form>
            </CardContent>
          </Card>

          {/* Sesiones activas */}
          <Card sx={{ bgcolor: '#0f1629', border: '1px solid rgba(255,0,110,0.2)', mb: 3 }}>
            <CardContent>
              <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 2 }}>
                <Typography variant="h6" sx={{ color: '#ff006e' }}>Sesiones Activas ({sesiones.length})</Typography>
                <Button size="small" color="error" variant="outlined" onClick={() => setDialogLogout(true)}>Cerrar todas</Button>
              </Box>
              <List dense>
                {sesiones.map(s => (
                  <ListItem key={s.id} sx={{ borderBottom: '1px solid rgba(255,255,255,0.05)' }}>
                    <ListItemText
                      primary={<Typography sx={{ color: '#fff', fontSize: '0.85rem' }}>Sesión #{s.id}</Typography>}
                      secondary={<Typography sx={{ color: '#555', fontSize: '0.75rem' }}>
                        Creada: {new Date(s.creadoEn).toLocaleString('es-ES')} · Expira: {new Date(s.expiracion).toLocaleString('es-ES')}
                      </Typography>}
                    />
                    <ListItemSecondaryAction>
                      <IconButton size="small" onClick={() => revocarSesion(s.id)} sx={{ color: '#ff006e' }}>
                        <DeleteIcon fontSize="small" />
                      </IconButton>
                    </ListItemSecondaryAction>
                  </ListItem>
                ))}
                {sesiones.length === 0 && <Typography sx={{ color: '#555', fontSize: '0.85rem' }}>Sin sesiones activas adicionales.</Typography>}
              </List>
            </CardContent>
          </Card>

          {/* Historial pagos */}
          {pagos.length > 0 && (
            <Card sx={{ bgcolor: '#0f1629', border: '1px solid rgba(0,255,136,0.2)' }}>
              <CardContent>
                <Typography variant="h6" sx={{ color: '#00ff88', mb: 2 }}>Historial de Pagos</Typography>
                {pagos.map(p => (
                  <Box key={p.id} sx={{ display: 'flex', justifyContent: 'space-between', py: 1, borderBottom: '1px solid rgba(255,255,255,0.05)' }}>
                    <Box>
                      <Typography sx={{ color: '#fff', fontSize: '0.85rem' }}>{p.plan}</Typography>
                      <Typography sx={{ color: '#555', fontSize: '0.75rem' }}>{new Date(p.fechaPago).toLocaleDateString('es-ES')} · {p.metodoPago}</Typography>
                    </Box>
                    <Box sx={{ textAlign: 'right' }}>
                      <Typography sx={{ color: '#00ff88', fontWeight: 700 }}>${p.monto} {p.moneda}</Typography>
                      <Chip label={p.estado} size="small" color={p.estado === 'completado' ? 'success' : 'error'} />
                    </Box>
                  </Box>
                ))}
              </CardContent>
            </Card>
          )}
        </Grid>
      </Grid>

      <Dialog open={dialogLogout} onClose={() => setDialogLogout(false)} PaperProps={{ sx: { bgcolor: '#0f1629' } }}>
        <DialogTitle sx={{ color: '#ff006e' }}>¿Cerrar todas las sesiones?</DialogTitle>
        <DialogContent>
          <Typography sx={{ color: '#b0d4ff' }}>Se cerrarán todas tus sesiones activas, incluyendo esta. Tendrás que volver a iniciar sesión.</Typography>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setDialogLogout(false)}>Cancelar</Button>
          <Button onClick={cerrarTodasSesiones} color="error" variant="contained">Cerrar todas</Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
}
