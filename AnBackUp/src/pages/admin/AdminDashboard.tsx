import { useState, useEffect, useCallback } from 'react';
import {
  Box, Grid, Card, CardContent, Typography, Table, TableBody, TableCell,
  TableHead, TableRow, Chip, Button, IconButton, Dialog, DialogTitle,
  DialogContent, DialogActions, Select, MenuItem, FormControl, InputLabel,
  Alert, Tabs, Tab, CircularProgress, Tooltip, TextField, InputAdornment,
  Divider, Avatar, LinearProgress, Stack,
} from '@mui/material';
import PersonOffIcon from '@mui/icons-material/PersonOff';
import PersonAddIcon from '@mui/icons-material/PersonAdd';
import PersonIcon from '@mui/icons-material/Person';
import CardGiftcardIcon from '@mui/icons-material/CardGiftcard';
import RefreshIcon from '@mui/icons-material/Refresh';
import CancelIcon from '@mui/icons-material/Cancel';
import SearchIcon from '@mui/icons-material/Search';
import LogoutIcon from '@mui/icons-material/Logout';
import InfoIcon from '@mui/icons-material/Info';
import PeopleIcon from '@mui/icons-material/People';
import CheckCircleIcon from '@mui/icons-material/CheckCircle';
import WorkIcon from '@mui/icons-material/Work';
import AttachMoneyIcon from '@mui/icons-material/AttachMoney';
import LockIcon from '@mui/icons-material/Lock';
import EditIcon from '@mui/icons-material/Edit';
import axios from 'axios';

const api = axios.create({ baseURL: '/api', withCredentials: true });

interface UsuarioAdmin {
  id: number;
  nombre: string;
  email: string;
  rol: string;
  isActive: boolean;
  organizacion: string;
  plan: string;
  suscripcionEstado: string;
  esGratisAdmin: boolean;
  lastLogin?: string;
}

interface DetalleUsuario {
  id: number;
  nombre: string;
  email: string;
  rol: string;
  isActive: boolean;
  lastLogin?: string;
  telefono?: string;
  organizacion?: string;
  fotoPerfil?: string;
  bio?: string;
  ciudad?: string;
  pais?: string;
  cargo?: string;
  empresa?: string;
  sesionesActivas: number;
  totalPagos: number;
  totalJobs: number;
  suscripciones: { id: number; plan: string; estado: string; fechaInicio: string; fechaFin?: string; esGratisAdminGranted: boolean }[];
}

interface Stats {
  totalUsuarios: number;
  usuariosActivos: number;
  totalJobs: number;
  jobsCompletados: number;
  ingresosMes: number;
  suscripcionesPorPlan: { plan: string; cantidad: number }[];
}

interface Plan {
  id: number;
  nombre: string;
  precioMensual: number;
  limiteAlmacenamientoBytes?: number;
  isActive?: boolean;
}

const rolColor = (rol: string) =>
  ({ superadmin: 'error', admin: 'warning', auditor: 'info', usuario: 'default' } as any)[rol] ?? 'default';

const rolLabel = (rol: string) =>
  ({ superadmin: 'Super Admin', admin: 'Admin', auditor: 'Auditor', usuario: 'Usuario' } as any)[rol] ?? rol;

const estadoColor = (estado: string) =>
  ({ activa: 'success', gratis_admin: 'warning', cancelada: 'error', vencida: 'default' } as any)[estado] ?? 'default';

export default function AdminDashboard() {
  const [tab, setTab] = useState(0);
  const [usuarios, setUsuarios] = useState<UsuarioAdmin[]>([]);
  const [stats, setStats] = useState<Stats | null>(null);
  const [planes, setPlanes] = useState<Plan[]>([]);
  const [pagos, setPagos] = useState<any[]>([]);
  const [loading, setLoading] = useState(false);
  const [busqueda, setBusqueda] = useState('');
  const [filtroPago, setFiltroPago] = useState('');
  const [filtroRol, setFiltroRol] = useState('todos');
  const [filtroEstado, setFiltroEstado] = useState('todos');

  // Dialogs
  const [dialogRol, setDialogRol] = useState<{ open: boolean; usuario: UsuarioAdmin | null; nuevoRol: string }>({ open: false, usuario: null, nuevoRol: '' });
  const [dialogPlan, setDialogPlan] = useState<{ open: boolean; usuario: UsuarioAdmin | null; planId: number }>({ open: false, usuario: null, planId: 0 });
  const [dialogDetalle, setDialogDetalle] = useState<{ open: boolean; detalle: DetalleUsuario | null }>({ open: false, detalle: null });
  const [dialogCrear, setDialogCrear] = useState(false);
  const [nuevoUsuario, setNuevoUsuario] = useState({ email: '', password: '', nombre: '', rol: 'usuario' });

  const [msg, setMsg] = useState<{ texto: string; tipo: 'success' | 'error' } | null>(null);

  const mostrarMsg = (texto: string, tipo: 'success' | 'error' = 'success') => {
    setMsg({ texto, tipo });
    setTimeout(() => setMsg(null), 5000);
  };

  const cargar = useCallback(async () => {
    setLoading(true);
    try {
      const [u, s, pl, pg] = await Promise.all([
        api.get('/admin/usuarios'),
        api.get('/admin/estadisticas'),
        api.get('/planes'),
        api.get('/admin/pagos'),
      ]);
      setUsuarios(u.data);
      setStats(s.data);
      setPlanes(pl.data);
      setPagos(pg.data);
    } catch {
      mostrarMsg('Error al cargar datos del panel.', 'error');
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => { cargar(); }, [cargar]);

  const cambiarRol = async () => {
    if (!dialogRol.usuario) return;
    try {
      await api.put(`/admin/usuarios/${dialogRol.usuario.id}/rol`, { nuevoRol: dialogRol.nuevoRol });
      setDialogRol({ open: false, usuario: null, nuevoRol: '' });
      mostrarMsg('Rol actualizado correctamente.');
      await cargar();
    } catch (err: any) {
      mostrarMsg(err?.response?.data?.message ?? 'Error al cambiar rol.', 'error');
    }
  };

  const toggleActivo = async (u: UsuarioAdmin) => {
    try {
      await api.put(`/admin/usuarios/${u.id}/${u.isActive ? 'desactivar' : 'activar'}`);
      mostrarMsg(`Usuario ${u.isActive ? 'desactivado' : 'activado'} correctamente.`);
      await cargar();
    } catch (err: any) {
      mostrarMsg(err?.response?.data?.message ?? 'Error al cambiar estado.', 'error');
    }
  };

  const otorgarPlan = async () => {
    if (!dialogPlan.usuario || !dialogPlan.planId) return;
    try {
      const res = await api.post(`/admin/usuarios/${dialogPlan.usuario.id}/otorgar-plan-gratis`, { planId: dialogPlan.planId, fechaFin: null });
      setDialogPlan({ open: false, usuario: null, planId: 0 });
      mostrarMsg(res.data.message ?? 'Plan otorgado gratuitamente.');
      await cargar();
    } catch (err: any) {
      mostrarMsg(err?.response?.data?.message ?? 'Error al otorgar plan.', 'error');
    }
  };

  const revocarPlan = async (u: UsuarioAdmin) => {
    try {
      await api.post(`/admin/usuarios/${u.id}/revocar-plan-gratis`);
      mostrarMsg(`Plan gratuito revocado a ${u.nombre}.`);
      await cargar();
    } catch (err: any) {
      mostrarMsg(err?.response?.data?.message ?? 'Error al revocar plan.', 'error');
    }
  };

  const forzarLogout = async (u: UsuarioAdmin) => {
    try {
      await api.delete(`/admin/usuarios/${u.id}/sesiones`);
      mostrarMsg(`Sesiones de ${u.nombre} cerradas forzosamente.`);
      if (dialogDetalle.open) {
        const res = await api.get(`/admin/usuarios/${u.id}/detalle`);
        setDialogDetalle({ open: true, detalle: res.data });
      }
      await cargar();
    } catch (err: any) {
      mostrarMsg(err?.response?.data?.message ?? 'Error al cerrar sesiones.', 'error');
    }
  };

  const verDetalle = async (u: UsuarioAdmin) => {
    try {
      const res = await api.get(`/admin/usuarios/${u.id}/detalle`);
      setDialogDetalle({ open: true, detalle: res.data });
    } catch {
      mostrarMsg('Error al cargar detalle del usuario.', 'error');
    }
  };

  const crearUsuario = async () => {
    try {
      const res = await api.post('/admin/usuarios', nuevoUsuario);
      setDialogCrear(false);
      setNuevoUsuario({ email: '', password: '', nombre: '', rol: 'usuario' });
      mostrarMsg(res.data.message ?? 'Usuario creado correctamente.');
      await cargar();
    } catch (err: any) {
      mostrarMsg(err?.response?.data?.message ?? 'Error al crear usuario.', 'error');
    }
  };

  // Filtro local de usuarios
  const usuariosFiltrados = usuarios.filter(u => {
    const matchBusqueda = !busqueda || u.nombre.toLowerCase().includes(busqueda.toLowerCase()) || u.email.toLowerCase().includes(busqueda.toLowerCase());
    const matchRol = filtroRol === 'todos' || u.rol === filtroRol;
    const matchEstado = filtroEstado === 'todos' || (filtroEstado === 'activo' ? u.isActive : !u.isActive);
    return matchBusqueda && matchRol && matchEstado;
  });

  const pagosFiltrados = pagos.filter(p =>
    !filtroPago || p.usuario?.toLowerCase().includes(filtroPago.toLowerCase()) || p.emailUsuario?.toLowerCase().includes(filtroPago.toLowerCase())
  );

  const statCards = stats ? [
    { label: 'Total Usuarios', value: stats.totalUsuarios, icon: <PeopleIcon />, color: '#00d4ff', sub: `${stats.usuariosActivos} activos` },
    { label: 'Usuarios Activos', value: stats.usuariosActivos, icon: <CheckCircleIcon />, color: '#00ff88', sub: `${stats.totalUsuarios - stats.usuariosActivos} inactivos` },
    { label: 'Jobs Completados', value: stats.jobsCompletados, icon: <WorkIcon />, color: '#7c3aed', sub: `de ${stats.totalJobs} totales` },
    { label: 'Ingresos del Mes', value: `$${stats.ingresosMes.toFixed(2)}`, icon: <AttachMoneyIcon />, color: '#ffb800', sub: 'USD' },
  ] : [];

  return (
    <Box>
      {/* Header */}
      <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', mb: 4 }}>
        <Box>
          <Typography variant="h4" sx={{ fontWeight: 800, background: 'linear-gradient(135deg, #00d4ff 0%, #7c3aed 100%)', WebkitBackgroundClip: 'text', WebkitTextFillColor: 'transparent' }}>
            Panel de Administración
          </Typography>
          <Typography variant="body2" sx={{ color: '#b0d4ff', mt: 0.5 }}>
            Gestión completa de usuarios, planes y actividad del sistema
          </Typography>
        </Box>
        <Button
          variant="outlined"
          startIcon={loading ? <CircularProgress size={16} sx={{ color: '#00d4ff' }} /> : <RefreshIcon />}
          onClick={cargar}
          disabled={loading}
          sx={{ color: '#00d4ff', borderColor: 'rgba(0,212,255,0.4)', '&:hover': { borderColor: '#00d4ff', bgcolor: 'rgba(0,212,255,0.08)' } }}
        >
          Actualizar
        </Button>
      </Box>

      {msg && (
        <Alert severity={msg.tipo} sx={{ mb: 3, fontSize: '0.95rem' }} onClose={() => setMsg(null)}>
          {msg.texto}
        </Alert>
      )}

      {/* Stat Cards */}
      <Grid container spacing={3} sx={{ mb: 4 }}>
        {statCards.map(item => (
          <Grid item xs={12} sm={6} md={3} key={item.label}>
            <Card sx={{ background: `linear-gradient(135deg, rgba(15,22,41,0.95) 0%, rgba(10,14,39,0.95) 100%)`, border: `1px solid ${item.color}44`, position: 'relative', overflow: 'hidden' }}>
              <Box sx={{ position: 'absolute', top: 0, right: 0, width: 80, height: 80, borderRadius: '0 0 0 80px', bgcolor: `${item.color}11` }} />
              <CardContent sx={{ p: 3 }}>
                <Box sx={{ display: 'flex', alignItems: 'flex-start', justifyContent: 'space-between' }}>
                  <Box>
                    <Typography variant="body2" sx={{ color: '#b0d4ff', mb: 1, fontSize: '0.85rem', textTransform: 'uppercase', letterSpacing: 0.5 }}>{item.label}</Typography>
                    <Typography variant="h3" sx={{ color: item.color, fontWeight: 800, lineHeight: 1 }}>{item.value}</Typography>
                    <Typography variant="caption" sx={{ color: '#b0d4ff66', mt: 0.5, display: 'block' }}>{item.sub}</Typography>
                  </Box>
                  <Box sx={{ color: `${item.color}88`, '& svg': { fontSize: 36 } }}>{item.icon}</Box>
                </Box>
              </CardContent>
            </Card>
          </Grid>
        ))}
      </Grid>

      {/* Tabs */}
      <Tabs
        value={tab}
        onChange={(_, v) => setTab(v)}
        sx={{ mb: 3, borderBottom: '1px solid rgba(255,255,255,0.1)', '& .MuiTab-root': { fontSize: '0.95rem', fontWeight: 600, minHeight: 52 }, '& .Mui-selected': { color: '#00d4ff !important' }, '& .MuiTabs-indicator': { bgcolor: '#00d4ff', height: 3, borderRadius: 2 } }}
      >
        <Tab label="👥 Usuarios" />
        <Tab label="💳 Pagos" />
        <Tab label="📊 Planes y Distribución" />
      </Tabs>

      {/* TAB: USUARIOS */}
      {tab === 0 && (
        <Box>
          {/* Barra de herramientas */}
          <Box sx={{ display: 'flex', gap: 2, mb: 3, flexWrap: 'wrap', alignItems: 'center' }}>
            <TextField
              placeholder="Buscar por nombre o email..."
              value={busqueda}
              onChange={e => setBusqueda(e.target.value)}
              size="medium"
              sx={{ flexGrow: 1, minWidth: 240, '& .MuiOutlinedInput-root': { borderRadius: 2, fontSize: '0.95rem' } }}
              InputProps={{ startAdornment: <InputAdornment position="start"><SearchIcon sx={{ color: '#b0d4ff' }} /></InputAdornment> }}
            />
            <FormControl size="medium" sx={{ minWidth: 150 }}>
              <InputLabel>Rol</InputLabel>
              <Select value={filtroRol} label="Rol" onChange={e => setFiltroRol(e.target.value)} sx={{ borderRadius: 2 }}>
                <MenuItem value="todos">Todos los roles</MenuItem>
                <MenuItem value="superadmin">Super Admin</MenuItem>
                <MenuItem value="admin">Admin</MenuItem>
                <MenuItem value="auditor">Auditor</MenuItem>
                <MenuItem value="usuario">Usuario</MenuItem>
              </Select>
            </FormControl>
            <FormControl size="medium" sx={{ minWidth: 150 }}>
              <InputLabel>Estado</InputLabel>
              <Select value={filtroEstado} label="Estado" onChange={e => setFiltroEstado(e.target.value)} sx={{ borderRadius: 2 }}>
                <MenuItem value="todos">Todos</MenuItem>
                <MenuItem value="activo">Activos</MenuItem>
                <MenuItem value="inactivo">Inactivos</MenuItem>
              </Select>
            </FormControl>
            <Button
              variant="contained"
              startIcon={<PersonAddIcon />}
              onClick={() => setDialogCrear(true)}
              size="large"
              sx={{ background: 'linear-gradient(135deg, #00d4ff 0%, #7c3aed 100%)', fontWeight: 700, px: 3, whiteSpace: 'nowrap' }}
            >
              Crear Usuario
            </Button>
          </Box>

          <Typography variant="body2" sx={{ color: '#b0d4ff', mb: 2 }}>
            Mostrando {usuariosFiltrados.length} de {usuarios.length} usuarios
          </Typography>

          <Card sx={{ border: '1px solid rgba(0,212,255,0.15)' }}>
            <Box sx={{ overflowX: 'auto' }}>
              <Table>
                <TableHead>
                  <TableRow sx={{ bgcolor: 'rgba(0,212,255,0.05)' }}>
                    {['Usuario', 'Rol', 'Plan', 'Estado', 'Último Acceso', 'Acciones'].map(h => (
                      <TableCell key={h} sx={{ color: '#00d4ff', fontWeight: 700, fontSize: '0.9rem', py: 2, borderColor: 'rgba(0,212,255,0.15)', whiteSpace: 'nowrap' }}>
                        {h}
                      </TableCell>
                    ))}
                  </TableRow>
                </TableHead>
                <TableBody>
                  {usuariosFiltrados.map(u => (
                    <TableRow key={u.id} sx={{ '&:hover': { bgcolor: 'rgba(0,212,255,0.04)' }, transition: 'background 0.2s' }}>
                      <TableCell sx={{ borderColor: 'rgba(255,255,255,0.05)', py: 2 }}>
                        <Box sx={{ display: 'flex', alignItems: 'center', gap: 2 }}>
                          <Avatar sx={{ bgcolor: u.isActive ? 'rgba(0,212,255,0.2)' : 'rgba(255,0,110,0.2)', color: u.isActive ? '#00d4ff' : '#ff006e', width: 40, height: 40, fontSize: '1rem', fontWeight: 700 }}>
                            {u.nombre.charAt(0).toUpperCase()}
                          </Avatar>
                          <Box>
                            <Typography sx={{ color: '#fff', fontWeight: 600, fontSize: '0.95rem' }}>{u.nombre}</Typography>
                            <Typography variant="caption" sx={{ color: '#b0d4ff' }}>{u.email}</Typography>
                          </Box>
                        </Box>
                      </TableCell>
                      <TableCell sx={{ borderColor: 'rgba(255,255,255,0.05)' }}>
                        <Chip label={rolLabel(u.rol)} color={rolColor(u.rol)} size="medium" sx={{ fontWeight: 600, fontSize: '0.82rem', px: 0.5 }} />
                      </TableCell>
                      <TableCell sx={{ borderColor: 'rgba(255,255,255,0.05)' }}>
                        <Chip
                          label={u.plan}
                          size="medium"
                          color={u.esGratisAdmin ? 'warning' : 'default'}
                          icon={u.esGratisAdmin ? <CardGiftcardIcon /> : undefined}
                          sx={{ fontSize: '0.82rem' }}
                        />
                      </TableCell>
                      <TableCell sx={{ borderColor: 'rgba(255,255,255,0.05)' }}>
                        <Chip label={u.isActive ? 'Activo' : 'Inactivo'} color={u.isActive ? 'success' : 'error'} size="medium" sx={{ fontWeight: 600, fontSize: '0.82rem' }} />
                      </TableCell>
                      <TableCell sx={{ color: '#b0d4ff', borderColor: 'rgba(255,255,255,0.05)', fontSize: '0.85rem' }}>
                        {u.lastLogin ? new Date(u.lastLogin).toLocaleString() : 'Nunca'}
                      </TableCell>
                      <TableCell sx={{ borderColor: 'rgba(255,255,255,0.05)', whiteSpace: 'nowrap' }}>
                        <Stack direction="row" spacing={1}>
                          <Tooltip title="Ver detalle completo">
                            <Button size="small" variant="outlined" startIcon={<InfoIcon />} onClick={() => verDetalle(u)}
                              sx={{ color: '#00d4ff', borderColor: 'rgba(0,212,255,0.4)', fontSize: '0.78rem', px: 1.5 }}>
                              Detalle
                            </Button>
                          </Tooltip>
                          <Tooltip title="Cambiar rol">
                            <Button size="small" variant="outlined" startIcon={<EditIcon />} onClick={() => setDialogRol({ open: true, usuario: u, nuevoRol: u.rol })}
                              sx={{ color: '#ffb800', borderColor: 'rgba(255,184,0,0.4)', fontSize: '0.78rem', px: 1.5 }}>
                              Rol
                            </Button>
                          </Tooltip>
                          <Tooltip title="Otorgar plan gratis">
                            <Button size="small" variant="outlined" startIcon={<CardGiftcardIcon />} onClick={() => setDialogPlan({ open: true, usuario: u, planId: 0 })}
                              sx={{ color: '#7c3aed', borderColor: 'rgba(124,58,237,0.4)', fontSize: '0.78rem', px: 1.5 }}>
                              Plan
                            </Button>
                          </Tooltip>
                          {u.esGratisAdmin && (
                            <Tooltip title="Revocar plan gratuito">
                              <Button size="small" variant="outlined" startIcon={<CancelIcon />} onClick={() => revocarPlan(u)}
                                sx={{ color: '#ff4444', borderColor: 'rgba(255,68,68,0.4)', fontSize: '0.78rem', px: 1.5 }}>
                                Revocar
                              </Button>
                            </Tooltip>
                          )}
                          <Tooltip title={u.isActive ? 'Desactivar cuenta' : 'Activar cuenta'}>
                            <Button size="small" variant="outlined"
                              startIcon={u.isActive ? <PersonOffIcon /> : <PersonIcon />}
                              onClick={() => toggleActivo(u)}
                              sx={{ color: u.isActive ? '#ff006e' : '#00ff88', borderColor: u.isActive ? 'rgba(255,0,110,0.4)' : 'rgba(0,255,136,0.4)', fontSize: '0.78rem', px: 1.5 }}>
                              {u.isActive ? 'Desactivar' : 'Activar'}
                            </Button>
                          </Tooltip>
                        </Stack>
                      </TableCell>
                    </TableRow>
                  ))}
                  {usuariosFiltrados.length === 0 && !loading && (
                    <TableRow>
                      <TableCell colSpan={6} sx={{ textAlign: 'center', color: '#b0d4ff', py: 5, fontSize: '1rem' }}>
                        {busqueda ? `Sin resultados para "${busqueda}"` : 'No hay usuarios registrados.'}
                      </TableCell>
                    </TableRow>
                  )}
                </TableBody>
              </Table>
            </Box>
          </Card>
        </Box>
      )}

      {/* TAB: PAGOS */}
      {tab === 1 && (
        <Box>
          <Box sx={{ display: 'flex', gap: 2, mb: 3 }}>
            <TextField
              placeholder="Buscar por usuario o email..."
              value={filtroPago}
              onChange={e => setFiltroPago(e.target.value)}
              size="medium"
              sx={{ maxWidth: 400, '& .MuiOutlinedInput-root': { borderRadius: 2 } }}
              InputProps={{ startAdornment: <InputAdornment position="start"><SearchIcon sx={{ color: '#b0d4ff' }} /></InputAdornment> }}
            />
            <Typography variant="body2" sx={{ color: '#b0d4ff', alignSelf: 'center' }}>
              {pagosFiltrados.length} registros
            </Typography>
          </Box>
          <Card sx={{ border: '1px solid rgba(0,212,255,0.15)' }}>
            <Box sx={{ overflowX: 'auto' }}>
              <Table>
                <TableHead>
                  <TableRow sx={{ bgcolor: 'rgba(0,212,255,0.05)' }}>
                    {['Usuario', 'Plan', 'Monto', 'Estado', 'Método de Pago', 'Fecha'].map(h => (
                      <TableCell key={h} sx={{ color: '#00d4ff', fontWeight: 700, fontSize: '0.9rem', py: 2.5, borderColor: 'rgba(0,212,255,0.15)' }}>{h}</TableCell>
                    ))}
                  </TableRow>
                </TableHead>
                <TableBody>
                  {pagosFiltrados.map((p: any) => (
                    <TableRow key={p.id} sx={{ '&:hover': { bgcolor: 'rgba(0,212,255,0.03)' } }}>
                      <TableCell sx={{ borderColor: 'rgba(255,255,255,0.05)', py: 2 }}>
                        <Typography sx={{ color: '#fff', fontWeight: 600 }}>{p.usuario}</Typography>
                        <Typography variant="caption" sx={{ color: '#b0d4ff' }}>{p.emailUsuario}</Typography>
                      </TableCell>
                      <TableCell sx={{ color: '#b0d4ff', borderColor: 'rgba(255,255,255,0.05)', fontSize: '0.9rem' }}>{p.plan}</TableCell>
                      <TableCell sx={{ borderColor: 'rgba(255,255,255,0.05)' }}>
                        <Typography sx={{ color: '#00ff88', fontWeight: 700, fontSize: '1rem' }}>${p.monto}</Typography>
                        <Typography variant="caption" sx={{ color: '#b0d4ff' }}>{p.moneda}</Typography>
                      </TableCell>
                      <TableCell sx={{ borderColor: 'rgba(255,255,255,0.05)' }}>
                        <Chip label={p.estado} color={p.estado === 'completado' ? 'success' : p.estado === 'fallido' ? 'error' : 'default'} size="medium" sx={{ fontWeight: 600 }} />
                      </TableCell>
                      <TableCell sx={{ color: '#b0d4ff', borderColor: 'rgba(255,255,255,0.05)', fontSize: '0.9rem' }}>{p.metodoPago}</TableCell>
                      <TableCell sx={{ color: '#b0d4ff', borderColor: 'rgba(255,255,255,0.05)', fontSize: '0.9rem' }}>
                        {new Date(p.fechaPago).toLocaleDateString('es-ES', { day: '2-digit', month: 'short', year: 'numeric' })}
                      </TableCell>
                    </TableRow>
                  ))}
                  {pagosFiltrados.length === 0 && (
                    <TableRow>
                      <TableCell colSpan={6} sx={{ textAlign: 'center', color: '#b0d4ff', py: 5, fontSize: '1rem' }}>
                        No hay pagos registrados.
                      </TableCell>
                    </TableRow>
                  )}
                </TableBody>
              </Table>
            </Box>
          </Card>
        </Box>
      )}

      {/* TAB: PLANES Y DISTRIBUCIÓN */}
      {tab === 2 && (
        <Box>
          <Typography variant="h6" sx={{ color: '#00d4ff', mb: 3, fontWeight: 700 }}>Planes disponibles</Typography>
          <Grid container spacing={3} sx={{ mb: 5 }}>
            {planes.map(p => (
              <Grid item xs={12} sm={6} md={4} key={p.id}>
                <Card sx={{ border: '1px solid rgba(124,58,237,0.3)', position: 'relative', overflow: 'hidden' }}>
                  <Box sx={{ position: 'absolute', top: 0, left: 0, right: 0, height: 4, background: 'linear-gradient(135deg, #00d4ff 0%, #7c3aed 100%)' }} />
                  <CardContent sx={{ p: 3 }}>
                    <Typography variant="h5" sx={{ color: '#fff', fontWeight: 800, mb: 1 }}>{p.nombre}</Typography>
                    <Typography variant="h4" sx={{ color: '#00d4ff', fontWeight: 800, mb: 2 }}>
                      {p.precioMensual === 0 ? 'Gratis' : `$${p.precioMensual}`}
                      {p.precioMensual > 0 && <Typography component="span" variant="body2" sx={{ color: '#b0d4ff' }}>/mes</Typography>}
                    </Typography>
                    {p.limiteAlmacenamientoBytes && (
                      <Typography variant="body2" sx={{ color: '#b0d4ff' }}>
                        {p.limiteAlmacenamientoBytes === -1 ? '∞ Almacenamiento ilimitado' : `${Math.round(p.limiteAlmacenamientoBytes / 1073741824)} GB de almacenamiento`}
                      </Typography>
                    )}
                    <Box sx={{ mt: 2 }}>
                      {stats?.suscripcionesPorPlan.find(s => s.plan === p.nombre) ? (
                        <Chip
                          label={`${stats.suscripcionesPorPlan.find(s => s.plan === p.nombre)?.cantidad ?? 0} suscriptores`}
                          color="success" size="medium" sx={{ fontWeight: 600 }}
                        />
                      ) : (
                        <Chip label="0 suscriptores" size="medium" sx={{ color: '#b0d4ff' }} />
                      )}
                    </Box>
                  </CardContent>
                </Card>
              </Grid>
            ))}
          </Grid>

          <Typography variant="h6" sx={{ color: '#00d4ff', mb: 3, fontWeight: 700 }}>Distribución de suscripciones</Typography>
          {stats && stats.suscripcionesPorPlan.length > 0 ? (
            <Grid container spacing={3}>
              {stats.suscripcionesPorPlan.map(s => {
                const total = stats.suscripcionesPorPlan.reduce((acc, x) => acc + x.cantidad, 0);
                const pct = total > 0 ? (s.cantidad / total) * 100 : 0;
                return (
                  <Grid item xs={12} sm={6} md={4} key={s.plan}>
                    <Card sx={{ border: '1px solid rgba(124,58,237,0.2)', p: 0 }}>
                      <CardContent sx={{ p: 3 }}>
                        <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 2 }}>
                          <Typography sx={{ color: '#fff', fontWeight: 700, fontSize: '1rem' }}>{s.plan}</Typography>
                          <Typography variant="h5" sx={{ color: '#7c3aed', fontWeight: 800 }}>{s.cantidad}</Typography>
                        </Box>
                        <LinearProgress
                          variant="determinate"
                          value={pct}
                          sx={{ height: 8, borderRadius: 4, bgcolor: 'rgba(124,58,237,0.2)', '& .MuiLinearProgress-bar': { borderRadius: 4, background: 'linear-gradient(90deg, #00d4ff, #7c3aed)' } }}
                        />
                        <Typography variant="caption" sx={{ color: '#b0d4ff', mt: 1, display: 'block' }}>{pct.toFixed(1)}% del total</Typography>
                      </CardContent>
                    </Card>
                  </Grid>
                );
              })}
            </Grid>
          ) : (
            <Typography sx={{ color: '#b0d4ff', textAlign: 'center', py: 4 }}>No hay suscripciones activas.</Typography>
          )}
        </Box>
      )}

      {/* DIALOG: Cambiar Rol */}
      <Dialog open={dialogRol.open} onClose={() => setDialogRol({ open: false, usuario: null, nuevoRol: '' })}
        PaperProps={{ sx: { bgcolor: '#0f1629', border: '1px solid rgba(255,184,0,0.3)', minWidth: 400, borderRadius: 3 } }}>
        <DialogTitle sx={{ color: '#ffb800', fontSize: '1.2rem', fontWeight: 700 }}>
          Cambiar Rol de Usuario
        </DialogTitle>
        <Divider sx={{ borderColor: 'rgba(255,184,0,0.2)' }} />
        <DialogContent sx={{ pt: 3 }}>
          <Box sx={{ display: 'flex', alignItems: 'center', gap: 2, mb: 3, p: 2, bgcolor: 'rgba(255,184,0,0.05)', borderRadius: 2, border: '1px solid rgba(255,184,0,0.15)' }}>
            <Avatar sx={{ bgcolor: 'rgba(255,184,0,0.2)', color: '#ffb800', width: 48, height: 48, fontSize: '1.2rem', fontWeight: 700 }}>
              {dialogRol.usuario?.nombre.charAt(0).toUpperCase()}
            </Avatar>
            <Box>
              <Typography sx={{ color: '#fff', fontWeight: 600 }}>{dialogRol.usuario?.nombre}</Typography>
              <Typography variant="caption" sx={{ color: '#b0d4ff' }}>{dialogRol.usuario?.email}</Typography>
            </Box>
            <Chip label={rolLabel(dialogRol.usuario?.rol ?? '')} color={rolColor(dialogRol.usuario?.rol ?? '')} size="small" sx={{ ml: 'auto' }} />
          </Box>
          <FormControl fullWidth>
            <InputLabel sx={{ fontSize: '1rem' }}>Nuevo Rol</InputLabel>
            <Select value={dialogRol.nuevoRol} label="Nuevo Rol" onChange={e => setDialogRol(d => ({ ...d, nuevoRol: e.target.value }))} sx={{ fontSize: '0.95rem' }}>
              <MenuItem value="usuario">👤 Usuario (consumidor)</MenuItem>
              <MenuItem value="auditor">🔍 Auditor (solo lectura)</MenuItem>
              <MenuItem value="admin">🛡️ Admin (gestión completa)</MenuItem>
            </Select>
          </FormControl>
        </DialogContent>
        <DialogActions sx={{ px: 3, pb: 3, gap: 1 }}>
          <Button onClick={() => setDialogRol({ open: false, usuario: null, nuevoRol: '' })} size="large" sx={{ color: '#b0d4ff' }}>Cancelar</Button>
          <Button onClick={cambiarRol} variant="contained" size="large" sx={{ background: 'linear-gradient(135deg, #ffb800, #ff8800)', px: 4, fontWeight: 700 }}>
            Guardar Cambio
          </Button>
        </DialogActions>
      </Dialog>

      {/* DIALOG: Otorgar Plan Gratis */}
      <Dialog open={dialogPlan.open} onClose={() => setDialogPlan({ open: false, usuario: null, planId: 0 })}
        PaperProps={{ sx: { bgcolor: '#0f1629', border: '1px solid rgba(124,58,237,0.4)', minWidth: 420, borderRadius: 3 } }}>
        <DialogTitle sx={{ color: '#7c3aed', fontSize: '1.2rem', fontWeight: 700 }}>
          Otorgar Plan Gratuito
        </DialogTitle>
        <Divider sx={{ borderColor: 'rgba(124,58,237,0.2)' }} />
        <DialogContent sx={{ pt: 3 }}>
          <Box sx={{ display: 'flex', alignItems: 'center', gap: 2, mb: 3, p: 2, bgcolor: 'rgba(124,58,237,0.05)', borderRadius: 2, border: '1px solid rgba(124,58,237,0.15)' }}>
            <Avatar sx={{ bgcolor: 'rgba(124,58,237,0.2)', color: '#a78bfa', width: 48, height: 48, fontSize: '1.2rem', fontWeight: 700 }}>
              {dialogPlan.usuario?.nombre.charAt(0).toUpperCase()}
            </Avatar>
            <Box>
              <Typography sx={{ color: '#fff', fontWeight: 600 }}>{dialogPlan.usuario?.nombre}</Typography>
              <Typography variant="caption" sx={{ color: '#b0d4ff' }}>Plan actual: <strong style={{ color: '#fff' }}>{dialogPlan.usuario?.plan}</strong></Typography>
            </Box>
            {dialogPlan.usuario?.esGratisAdmin && (
              <Chip label="Ya tiene plan admin" size="small" color="warning" sx={{ ml: 'auto' }} />
            )}
          </Box>
          <FormControl fullWidth>
            <InputLabel sx={{ fontSize: '1rem' }}>Selecciona el Plan a Otorgar</InputLabel>
            <Select value={dialogPlan.planId} label="Selecciona el Plan a Otorgar" onChange={e => setDialogPlan(d => ({ ...d, planId: Number(e.target.value) }))} sx={{ fontSize: '0.95rem' }}>
              <MenuItem value={0} disabled>— Elige un plan —</MenuItem>
              {planes.map(p => (
                <MenuItem key={p.id} value={p.id}>
                  <Box sx={{ display: 'flex', alignItems: 'center', gap: 2, width: '100%' }}>
                    <Box sx={{ flexGrow: 1 }}>{p.nombre}</Box>
                    <Chip label={p.precioMensual === 0 ? 'Gratis' : `$${p.precioMensual}/mes`} size="small" color={p.precioMensual === 0 ? 'default' : 'warning'} />
                  </Box>
                </MenuItem>
              ))}
            </Select>
          </FormControl>
          <Alert severity="info" sx={{ mt: 2, fontSize: '0.85rem' }}>
            El usuario recibirá este plan sin costo. Podrás revocarlo en cualquier momento.
          </Alert>
        </DialogContent>
        <DialogActions sx={{ px: 3, pb: 3, gap: 1 }}>
          <Button onClick={() => setDialogPlan({ open: false, usuario: null, planId: 0 })} size="large" sx={{ color: '#b0d4ff' }}>Cancelar</Button>
          <Button onClick={otorgarPlan} variant="contained" size="large" disabled={!dialogPlan.planId}
            sx={{ background: 'linear-gradient(135deg, #7c3aed, #a78bfa)', px: 4, fontWeight: 700 }}>
            Otorgar Plan Gratis
          </Button>
        </DialogActions>
      </Dialog>

      {/* DIALOG: Detalle Usuario */}
      <Dialog open={dialogDetalle.open} onClose={() => setDialogDetalle({ open: false, detalle: null })}
        maxWidth="md" fullWidth
        PaperProps={{ sx: { bgcolor: '#0f1629', border: '1px solid rgba(0,212,255,0.3)', borderRadius: 3 } }}>
        <DialogTitle sx={{ color: '#00d4ff', fontSize: '1.3rem', fontWeight: 700 }}>
          Detalle del Usuario
        </DialogTitle>
        <Divider sx={{ borderColor: 'rgba(0,212,255,0.2)' }} />
        {dialogDetalle.detalle && (
          <DialogContent sx={{ pt: 3 }}>
            <Grid container spacing={3}>
              {/* Info principal */}
              <Grid item xs={12} md={5}>
                <Box sx={{ display: 'flex', alignItems: 'center', gap: 2, mb: 3 }}>
                  <Avatar
                    src={dialogDetalle.detalle.fotoPerfil || undefined}
                    sx={{ bgcolor: 'rgba(0,212,255,0.2)', color: '#00d4ff', width: 64, height: 64, fontSize: '1.6rem', fontWeight: 700, border: '2px solid rgba(0,212,255,0.3)' }}
                  >
                    {!dialogDetalle.detalle.fotoPerfil && dialogDetalle.detalle.nombre.charAt(0).toUpperCase()}
                  </Avatar>
                  <Box>
                    <Typography sx={{ color: '#fff', fontWeight: 700, fontSize: '1.1rem' }}>{dialogDetalle.detalle.nombre}</Typography>
                    <Typography variant="body2" sx={{ color: '#b0d4ff' }}>{dialogDetalle.detalle.email}</Typography>
                    {dialogDetalle.detalle.cargo && <Typography variant="caption" sx={{ color: '#7c3aed' }}>{dialogDetalle.detalle.cargo}</Typography>}
                    {dialogDetalle.detalle.empresa && <Typography variant="caption" sx={{ color: '#b0d4ff', display: 'block' }}>{dialogDetalle.detalle.empresa}</Typography>}
                    <Chip label={rolLabel(dialogDetalle.detalle.rol)} color={rolColor(dialogDetalle.detalle.rol)} size="small" sx={{ mt: 0.5 }} />
                  </Box>
                </Box>
                {dialogDetalle.detalle.bio && (
                  <Box sx={{ mb: 2, p: 1.5, bgcolor: 'rgba(0,0,0,0.2)', borderRadius: 2, borderLeft: '3px solid rgba(0,212,255,0.3)' }}>
                    <Typography sx={{ color: '#b0d4ff', fontSize: '0.85rem', fontStyle: 'italic' }}>"{dialogDetalle.detalle.bio}"</Typography>
                  </Box>
                )}
                <Grid container spacing={2}>
                  {[
                    { label: 'Sesiones activas', value: dialogDetalle.detalle.sesionesActivas, color: dialogDetalle.detalle.sesionesActivas > 0 ? '#00ff88' : '#b0d4ff' },
                    { label: 'Jobs creados', value: dialogDetalle.detalle.totalJobs, color: '#00d4ff' },
                    { label: 'Total pagado', value: `$${dialogDetalle.detalle.totalPagos.toFixed(2)}`, color: '#ffb800' },
                  ].map(item => (
                    <Grid item xs={4} key={item.label}>
                      <Box sx={{ textAlign: 'center', p: 1.5, bgcolor: 'rgba(0,0,0,0.3)', borderRadius: 2 }}>
                        <Typography variant="h5" sx={{ color: item.color, fontWeight: 800 }}>{item.value}</Typography>
                        <Typography variant="caption" sx={{ color: '#b0d4ff', display: 'block', lineHeight: 1.2 }}>{item.label}</Typography>
                      </Box>
                    </Grid>
                  ))}
                </Grid>
                <Box sx={{ mt: 2, p: 2, bgcolor: 'rgba(0,0,0,0.2)', borderRadius: 2 }}>
                  {[
                    { label: 'Último acceso', value: dialogDetalle.detalle.lastLogin ? new Date(dialogDetalle.detalle.lastLogin).toLocaleString('es-ES') : 'Nunca' },
                    ...(dialogDetalle.detalle.telefono ? [{ label: 'Teléfono', value: dialogDetalle.detalle.telefono }] : []),
                    ...(dialogDetalle.detalle.organizacion ? [{ label: 'Organización', value: dialogDetalle.detalle.organizacion }] : []),
                    ...((dialogDetalle.detalle.ciudad || dialogDetalle.detalle.pais) ? [{ label: 'Ubicación', value: [dialogDetalle.detalle.ciudad, dialogDetalle.detalle.pais].filter(Boolean).join(', ') }] : []),
                  ].map(item => (
                    <Box key={item.label} sx={{ mb: 1 }}>
                      <Typography variant="caption" sx={{ color: '#b0d4ff66', textTransform: 'uppercase', letterSpacing: 0.5 }}>{item.label}</Typography>
                      <Typography sx={{ color: '#fff', fontSize: '0.88rem' }}>{item.value}</Typography>
                    </Box>
                  ))}
                </Box>
              </Grid>

              {/* Historial suscripciones */}
              <Grid item xs={12} md={7}>
                <Typography variant="h6" sx={{ color: '#fff', fontWeight: 700, mb: 2 }}>Historial de Suscripciones</Typography>
                {dialogDetalle.detalle.suscripciones.length === 0 ? (
                  <Typography sx={{ color: '#b0d4ff', py: 2 }}>Sin suscripciones.</Typography>
                ) : (
                  dialogDetalle.detalle.suscripciones.map(s => (
                    <Box key={s.id} sx={{ display: 'flex', alignItems: 'center', gap: 2, mb: 1.5, p: 1.5, bgcolor: 'rgba(0,0,0,0.2)', borderRadius: 2, border: '1px solid rgba(255,255,255,0.06)' }}>
                      <Box sx={{ flexGrow: 1 }}>
                        <Typography sx={{ color: '#fff', fontWeight: 600 }}>{s.plan}</Typography>
                        <Typography variant="caption" sx={{ color: '#b0d4ff' }}>
                          Desde {new Date(s.fechaInicio).toLocaleDateString()}
                          {s.fechaFin && ` · Hasta ${new Date(s.fechaFin).toLocaleDateString()}`}
                        </Typography>
                      </Box>
                      <Chip label={s.estado} color={estadoColor(s.estado)} size="small" sx={{ fontWeight: 600 }} />
                      {s.esGratisAdminGranted && <Chip label="Admin" icon={<CardGiftcardIcon />} size="small" color="warning" />}
                    </Box>
                  ))
                )}
              </Grid>
            </Grid>
          </DialogContent>
        )}
        <Divider sx={{ borderColor: 'rgba(0,212,255,0.1)' }} />
        <DialogActions sx={{ px: 3, py: 2, gap: 1, flexWrap: 'wrap' }}>
          {dialogDetalle.detalle && dialogDetalle.detalle.sesionesActivas > 0 && (
            <Button
              variant="outlined"
              startIcon={<LogoutIcon />}
              onClick={() => { const u = usuarios.find(x => x.id === dialogDetalle.detalle!.id); if (u) forzarLogout(u); }}
              sx={{ color: '#ff006e', borderColor: 'rgba(255,0,110,0.4)', fontWeight: 600 }}
            >
              Forzar Logout ({dialogDetalle.detalle.sesionesActivas} sesiones)
            </Button>
          )}
          <Button
            variant="outlined"
            startIcon={<LockIcon />}
            onClick={() => { const u = usuarios.find(x => x.id === dialogDetalle.detalle!.id); if (u) { setDialogDetalle({ open: false, detalle: null }); toggleActivo(u); } }}
            sx={{ color: dialogDetalle.detalle?.isActive ? '#ff006e' : '#00ff88', borderColor: dialogDetalle.detalle?.isActive ? 'rgba(255,0,110,0.4)' : 'rgba(0,255,136,0.4)', fontWeight: 600 }}
          >
            {dialogDetalle.detalle?.isActive ? 'Desactivar Cuenta' : 'Activar Cuenta'}
          </Button>
          <Box sx={{ flexGrow: 1 }} />
          <Button onClick={() => setDialogDetalle({ open: false, detalle: null })} size="large" sx={{ color: '#b0d4ff', fontWeight: 600 }}>
            Cerrar
          </Button>
        </DialogActions>
      </Dialog>

      {/* DIALOG: Crear Usuario */}
      <Dialog open={dialogCrear} onClose={() => setDialogCrear(false)}
        PaperProps={{ sx: { bgcolor: '#0f1629', border: '1px solid rgba(0,212,255,0.3)', minWidth: 420, borderRadius: 3 } }}>
        <DialogTitle sx={{ color: '#00d4ff', fontSize: '1.2rem', fontWeight: 700 }}>
          Crear Nuevo Usuario
        </DialogTitle>
        <Divider sx={{ borderColor: 'rgba(0,212,255,0.2)' }} />
        <DialogContent sx={{ pt: 3 }}>
          <Stack spacing={2.5}>
            <TextField label="Nombre completo" value={nuevoUsuario.nombre} onChange={e => setNuevoUsuario(n => ({ ...n, nombre: e.target.value }))}
              fullWidth size="medium" sx={{ '& .MuiOutlinedInput-root': { borderRadius: 2 } }} />
            <TextField label="Email" type="email" value={nuevoUsuario.email} onChange={e => setNuevoUsuario(n => ({ ...n, email: e.target.value }))}
              fullWidth size="medium" sx={{ '& .MuiOutlinedInput-root': { borderRadius: 2 } }} />
            <TextField label="Contraseña" type="password" value={nuevoUsuario.password} onChange={e => setNuevoUsuario(n => ({ ...n, password: e.target.value }))}
              fullWidth size="medium" sx={{ '& .MuiOutlinedInput-root': { borderRadius: 2 } }} />
            <FormControl fullWidth>
              <InputLabel>Rol inicial</InputLabel>
              <Select value={nuevoUsuario.rol} label="Rol inicial" onChange={e => setNuevoUsuario(n => ({ ...n, rol: e.target.value }))} sx={{ borderRadius: 2 }}>
                <MenuItem value="usuario">👤 Usuario (consumidor)</MenuItem>
                <MenuItem value="auditor">🔍 Auditor</MenuItem>
                <MenuItem value="admin">🛡️ Admin</MenuItem>
              </Select>
            </FormControl>
          </Stack>
        </DialogContent>
        <DialogActions sx={{ px: 3, pb: 3, gap: 1 }}>
          <Button onClick={() => setDialogCrear(false)} size="large" sx={{ color: '#b0d4ff' }}>Cancelar</Button>
          <Button
            onClick={crearUsuario}
            variant="contained"
            size="large"
            disabled={!nuevoUsuario.email || !nuevoUsuario.password || !nuevoUsuario.nombre}
            sx={{ background: 'linear-gradient(135deg, #00d4ff 0%, #7c3aed 100%)', px: 4, fontWeight: 700 }}
          >
            Crear Usuario
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
}
