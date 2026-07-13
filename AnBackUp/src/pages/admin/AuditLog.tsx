import { useState, useEffect, useCallback } from 'react';
import {
  Box, Typography, Card, Table, TableHead, TableBody, TableRow, TableCell,
  TextField, Button, Chip, Pagination, Grid, InputAdornment, Select,
  MenuItem, FormControl, InputLabel, Collapse, IconButton, Tooltip,
  Dialog, DialogTitle, DialogContent, DialogActions, Divider,
} from '@mui/material';
import SearchIcon from '@mui/icons-material/Search';
import FilterListIcon from '@mui/icons-material/FilterList';
import CloseIcon from '@mui/icons-material/Close';
import RefreshIcon from '@mui/icons-material/Refresh';
import InfoIcon from '@mui/icons-material/Info';
import ComputerIcon from '@mui/icons-material/Computer';
import PhoneAndroidIcon from '@mui/icons-material/PhoneAndroid';
import TabletIcon from '@mui/icons-material/Tablet';
import axios from 'axios';

const api = axios.create({ baseURL: '/api', withCredentials: true });

interface LogEntry {
  id: number;
  accion: string;
  descripcionLegible?: string;
  entidad: string;
  entidadId?: string;
  email?: string;
  usuarioId?: number;
  ipAddress?: string;
  navegador?: string;
  sistemaOperativo?: string;
  dispositivo?: string;
  userAgent?: string;
  datosAnteriores?: string;
  datosNuevos?: string;
  creadoEn: string;
}

const PAGE_SIZE = 25;

const accionColor = (a: string) => {
  if (a === 'LOGIN') return '#00ff88';
  if (a === 'LOGOUT') return '#b0d4ff';
  if (a.includes('DELETE') || a.includes('DESACTIVAR') || a.includes('FORZAR')) return '#ff6b35';
  if (a.includes('ERROR') || a.includes('FAIL')) return '#ff006e';
  if (a.includes('CREAR') || a.includes('REGISTER') || a.includes('OTORGAR')) return '#7c3aed';
  if (a.includes('CAMBIO') || a.includes('ACTUALIZAR') || a.includes('REVOCAR')) return '#ffb800';
  return '#00d4ff';
};

const accionLabel = (a: string) => {
  const labels: Record<string, string> = {
    LOGIN: 'Inicio de sesión',
    LOGOUT: 'Cierre de sesión',
    REGISTER: 'Registro',
    CAMBIO_ROL: 'Cambio de rol',
    OTORGAR_PLAN_GRATIS: 'Plan gratuito',
    REVOCAR_PLAN_GRATIS: 'Revocar plan',
    ACTIVAR_USUARIO: 'Activar cuenta',
    DESACTIVAR_USUARIO: 'Desactivar cuenta',
    FORZAR_LOGOUT: 'Logout forzado',
    ACTUALIZAR_PERFIL: 'Actualizar perfil',
    SUBIR_FOTO: 'Foto de perfil',
    CAMBIO_CONTRASEÑA: 'Cambio contraseña',
    REVOCAR_SESION: 'Revocar sesión',
    REVOCAR_TODAS_SESIONES: 'Revocar sesiones',
    CREAR_USUARIO_ADMIN: 'Crear usuario',
    SUSCRIPCION: 'Suscripción',
  };
  return labels[a] ?? a;
};

const dispositivoIcon = (d?: string) => {
  if (d === 'Móvil') return <PhoneAndroidIcon sx={{ fontSize: 16 }} />;
  if (d === 'Tablet') return <TabletIcon sx={{ fontSize: 16 }} />;
  return <ComputerIcon sx={{ fontSize: 16 }} />;
};

export default function AuditLogPage() {
  const [logs, setLogs] = useState<LogEntry[]>([]);
  const [total, setTotal] = useState(0);
  const [page, setPage] = useState(1);
  const [loading, setLoading] = useState(false);
  const [mostrarFiltros, setMostrarFiltros] = useState(true);
  const [logDetalle, setLogDetalle] = useState<LogEntry | null>(null);
  const [accionesDisponibles, setAccionesDisponibles] = useState<string[]>([]);

  const [filtros, setFiltros] = useState({
    accion: '',
    email: '',
    ip: '',
    navegador: '',
    dispositivo: '',
    desde: '',
    hasta: '',
  });

  const cargar = useCallback(async (p = 1) => {
    setLoading(true);
    try {
      const params = new URLSearchParams({ page: String(p), pageSize: String(PAGE_SIZE) });
      if (filtros.accion) params.append('accion', filtros.accion);
      if (filtros.email) params.append('email', filtros.email);
      if (filtros.ip) params.append('ip', filtros.ip);
      if (filtros.navegador) params.append('navegador', filtros.navegador);
      if (filtros.dispositivo) params.append('dispositivo', filtros.dispositivo);
      if (filtros.desde) params.append('desde', filtros.desde);
      if (filtros.hasta) params.append('hasta', filtros.hasta);
      const r = await api.get(`/audit?${params}`);
      setLogs(r.data.logs);
      setTotal(r.data.total);
    } finally {
      setLoading(false);
    }
  }, [filtros]);

  useEffect(() => {
    api.get('/audit/acciones').then(r => setAccionesDisponibles(r.data)).catch(() => {});
  }, []);

  useEffect(() => { cargar(page); }, [page]); // eslint-disable-line

  const buscar = () => { setPage(1); cargar(1); };
  const limpiar = () => {
    const empty = { accion: '', email: '', ip: '', navegador: '', dispositivo: '', desde: '', hasta: '' };
    setFiltros(empty);
    setPage(1);
    setTimeout(() => cargar(1), 50);
  };

  const totalPaginas = Math.ceil(total / PAGE_SIZE);

  return (
    <Box>
      {/* Header */}
      <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', mb: 3 }}>
        <Box>
          <Typography variant="h4" sx={{ fontWeight: 800, background: 'linear-gradient(135deg, #00d4ff 0%, #7c3aed 100%)', WebkitBackgroundClip: 'text', WebkitTextFillColor: 'transparent' }}>
            Log de Auditoría
          </Typography>
          <Typography variant="body2" sx={{ color: '#b0d4ff', mt: 0.5 }}>
            Registro completo de actividad — {total.toLocaleString()} eventos registrados
          </Typography>
        </Box>
        <Box sx={{ display: 'flex', gap: 1 }}>
          <Tooltip title="Filtros avanzados">
            <IconButton onClick={() => setMostrarFiltros(f => !f)} sx={{ color: mostrarFiltros ? '#00d4ff' : '#b0d4ff', border: `1px solid ${mostrarFiltros ? 'rgba(0,212,255,0.4)' : 'rgba(255,255,255,0.1)'}`, borderRadius: 2 }}>
              <FilterListIcon />
            </IconButton>
          </Tooltip>
          <Tooltip title="Actualizar">
            <IconButton onClick={() => cargar(page)} disabled={loading} sx={{ color: '#00d4ff', border: '1px solid rgba(0,212,255,0.3)', borderRadius: 2 }}>
              <RefreshIcon />
            </IconButton>
          </Tooltip>
        </Box>
      </Box>

      {/* Filtros */}
      <Collapse in={mostrarFiltros}>
        <Card sx={{ mb: 3, border: '1px solid rgba(0,212,255,0.2)', bgcolor: '#0f1629' }}>
          <Box sx={{ p: 2.5, pb: 0 }}>
            <Typography variant="subtitle2" sx={{ color: '#00d4ff', mb: 2, fontWeight: 700, display: 'flex', alignItems: 'center', gap: 1 }}>
              <FilterListIcon sx={{ fontSize: 18 }} /> Filtros Avanzados
            </Typography>
          </Box>
          <Grid container spacing={2} sx={{ px: 2.5, pb: 2.5 }}>
            <Grid item xs={12} sm={6} md={3}>
              <FormControl fullWidth size="medium">
                <InputLabel>Tipo de Acción</InputLabel>
                <Select value={filtros.accion} label="Tipo de Acción" onChange={e => setFiltros(f => ({ ...f, accion: e.target.value }))} sx={{ borderRadius: 2 }}>
                  <MenuItem value="">Todas las acciones</MenuItem>
                  {accionesDisponibles.map(a => (
                    <MenuItem key={a} value={a}>
                      <Chip label={accionLabel(a)} size="small" sx={{ bgcolor: `${accionColor(a)}22`, color: accionColor(a), fontSize: '0.75rem' }} />
                    </MenuItem>
                  ))}
                </Select>
              </FormControl>
            </Grid>
            <Grid item xs={12} sm={6} md={3}>
              <TextField label="Email del usuario" value={filtros.email} onChange={e => setFiltros(f => ({ ...f, email: e.target.value }))}
                fullWidth size="medium" placeholder="usuario@ejemplo.com"
                InputProps={{ startAdornment: <InputAdornment position="start"><SearchIcon sx={{ color: '#b0d4ff', fontSize: 18 }} /></InputAdornment> }}
                sx={{ '& .MuiOutlinedInput-root': { borderRadius: 2 } }} />
            </Grid>
            <Grid item xs={12} sm={6} md={2}>
              <TextField label="Dirección IP" value={filtros.ip} onChange={e => setFiltros(f => ({ ...f, ip: e.target.value }))}
                fullWidth size="medium" placeholder="192.168.x.x"
                sx={{ '& .MuiOutlinedInput-root': { borderRadius: 2 } }} />
            </Grid>
            <Grid item xs={12} sm={6} md={2}>
              <FormControl fullWidth size="medium">
                <InputLabel>Dispositivo</InputLabel>
                <Select value={filtros.dispositivo} label="Dispositivo" onChange={e => setFiltros(f => ({ ...f, dispositivo: e.target.value }))} sx={{ borderRadius: 2 }}>
                  <MenuItem value="">Todos</MenuItem>
                  <MenuItem value="Escritorio">🖥️ Escritorio</MenuItem>
                  <MenuItem value="Móvil">📱 Móvil</MenuItem>
                  <MenuItem value="Tablet">📲 Tablet</MenuItem>
                </Select>
              </FormControl>
            </Grid>
            <Grid item xs={12} sm={6} md={2}>
              <TextField label="Navegador" value={filtros.navegador} onChange={e => setFiltros(f => ({ ...f, navegador: e.target.value }))}
                fullWidth size="medium" placeholder="Chrome, Firefox..."
                sx={{ '& .MuiOutlinedInput-root': { borderRadius: 2 } }} />
            </Grid>
            <Grid item xs={12} sm={6} md={3}>
              <TextField label="Fecha desde" type="date" value={filtros.desde} onChange={e => setFiltros(f => ({ ...f, desde: e.target.value }))}
                fullWidth size="medium" InputLabelProps={{ shrink: true }}
                sx={{ '& .MuiOutlinedInput-root': { borderRadius: 2 } }} />
            </Grid>
            <Grid item xs={12} sm={6} md={3}>
              <TextField label="Fecha hasta" type="date" value={filtros.hasta} onChange={e => setFiltros(f => ({ ...f, hasta: e.target.value }))}
                fullWidth size="medium" InputLabelProps={{ shrink: true }}
                sx={{ '& .MuiOutlinedInput-root': { borderRadius: 2 } }} />
            </Grid>
            <Grid item xs={12} sm={12} md={6} sx={{ display: 'flex', gap: 1.5, alignItems: 'center' }}>
              <Button variant="contained" onClick={buscar} size="large" startIcon={<SearchIcon />}
                sx={{ background: 'linear-gradient(135deg, #00d4ff 0%, #7c3aed 100%)', fontWeight: 700, px: 4 }}>
                Buscar
              </Button>
              <Button variant="outlined" onClick={limpiar} size="large" startIcon={<CloseIcon />}
                sx={{ color: '#b0d4ff', borderColor: 'rgba(255,255,255,0.2)' }}>
                Limpiar
              </Button>
            </Grid>
          </Grid>
        </Card>
      </Collapse>

      {/* Tabla */}
      <Card sx={{ border: '1px solid rgba(0,212,255,0.15)', bgcolor: '#0f1629' }}>
        <Box sx={{ overflowX: 'auto' }}>
          <Table>
            <TableHead>
              <TableRow sx={{ bgcolor: 'rgba(0,212,255,0.06)' }}>
                {['Fecha y Hora', 'Usuario', 'Acción / Descripción', 'Entidad', 'IP / Red', 'Navegador / SO', 'Dispositivo', 'Detalle'].map(h => (
                  <TableCell key={h} sx={{ color: '#00d4ff', fontWeight: 700, fontSize: '0.82rem', py: 2, borderColor: 'rgba(0,212,255,0.12)', whiteSpace: 'nowrap' }}>{h}</TableCell>
                ))}
              </TableRow>
            </TableHead>
            <TableBody>
              {logs.map(l => (
                <TableRow key={l.id} sx={{ '&:hover': { bgcolor: 'rgba(0,212,255,0.03)' }, transition: 'background 0.15s' }}>
                  <TableCell sx={{ borderColor: 'rgba(255,255,255,0.04)', py: 1.8, whiteSpace: 'nowrap' }}>
                    <Typography sx={{ color: '#fff', fontSize: '0.85rem', fontWeight: 500 }}>
                      {new Date(l.creadoEn).toLocaleDateString('es-ES', { day: '2-digit', month: 'short', year: 'numeric' })}
                    </Typography>
                    <Typography variant="caption" sx={{ color: '#b0d4ff' }}>
                      {new Date(l.creadoEn).toLocaleTimeString('es-ES', { hour: '2-digit', minute: '2-digit', second: '2-digit' })}
                    </Typography>
                  </TableCell>
                  <TableCell sx={{ borderColor: 'rgba(255,255,255,0.04)', py: 1.8 }}>
                    <Typography sx={{ color: '#fff', fontSize: '0.85rem', fontWeight: 500 }}>{l.email || '—'}</Typography>
                    {l.usuarioId && <Typography variant="caption" sx={{ color: '#b0d4ff66' }}>ID #{l.usuarioId}</Typography>}
                  </TableCell>
                  <TableCell sx={{ borderColor: 'rgba(255,255,255,0.04)', py: 1.8, maxWidth: 260 }}>
                    <Chip
                      label={accionLabel(l.accion)}
                      size="small"
                      sx={{ bgcolor: `${accionColor(l.accion)}22`, color: accionColor(l.accion), fontWeight: 600, fontSize: '0.72rem', mb: 0.5 }}
                    />
                    {l.descripcionLegible && (
                      <Typography variant="caption" sx={{ color: '#b0d4ff', display: 'block', lineHeight: 1.3 }}>
                        {l.descripcionLegible}
                      </Typography>
                    )}
                  </TableCell>
                  <TableCell sx={{ borderColor: 'rgba(255,255,255,0.04)', py: 1.8 }}>
                    <Typography sx={{ color: '#b0d4ff', fontSize: '0.82rem' }}>{l.entidad}</Typography>
                    {l.entidadId && <Typography variant="caption" sx={{ color: '#b0d4ff66' }}>#{l.entidadId}</Typography>}
                  </TableCell>
                  <TableCell sx={{ borderColor: 'rgba(255,255,255,0.04)', py: 1.8 }}>
                    <Typography sx={{ color: '#00d4ff', fontFamily: 'monospace', fontSize: '0.82rem', fontWeight: 600 }}>
                      {l.ipAddress || '—'}
                    </Typography>
                  </TableCell>
                  <TableCell sx={{ borderColor: 'rgba(255,255,255,0.04)', py: 1.8 }}>
                    <Typography sx={{ color: '#fff', fontSize: '0.82rem' }}>{l.navegador || '—'}</Typography>
                    <Typography variant="caption" sx={{ color: '#b0d4ff' }}>{l.sistemaOperativo || ''}</Typography>
                  </TableCell>
                  <TableCell sx={{ borderColor: 'rgba(255,255,255,0.04)', py: 1.8 }}>
                    {l.dispositivo ? (
                      <Chip
                        icon={dispositivoIcon(l.dispositivo)}
                        label={l.dispositivo}
                        size="small"
                        sx={{ bgcolor: 'rgba(0,212,255,0.1)', color: '#b0d4ff', fontSize: '0.75rem' }}
                      />
                    ) : '—'}
                  </TableCell>
                  <TableCell sx={{ borderColor: 'rgba(255,255,255,0.04)', py: 1.8 }}>
                    {(l.datosAnteriores || l.datosNuevos || l.userAgent) && (
                      <Tooltip title="Ver detalles completos">
                        <IconButton size="small" onClick={() => setLogDetalle(l)} sx={{ color: '#00d4ff' }}>
                          <InfoIcon sx={{ fontSize: 18 }} />
                        </IconButton>
                      </Tooltip>
                    )}
                  </TableCell>
                </TableRow>
              ))}
              {logs.length === 0 && !loading && (
                <TableRow>
                  <TableCell colSpan={8} sx={{ textAlign: 'center', color: '#b0d4ff', py: 6, fontSize: '1rem' }}>
                    No se encontraron registros con los filtros aplicados.
                  </TableCell>
                </TableRow>
              )}
            </TableBody>
          </Table>
        </Box>
      </Card>

      {/* Paginación */}
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mt: 3 }}>
        <Typography sx={{ color: '#b0d4ff', fontSize: '0.85rem' }}>
          Mostrando {total === 0 ? 0 : Math.min((page - 1) * PAGE_SIZE + 1, total)}–{Math.min(page * PAGE_SIZE, total)} de {total.toLocaleString()} registros
        </Typography>
        <Pagination
          count={totalPaginas}
          page={page}
          onChange={(_, p) => setPage(p)}
          color="primary"
          size="large"
          showFirstButton
          showLastButton
          sx={{ '& .MuiPaginationItem-root': { color: '#b0d4ff', borderColor: 'rgba(0,212,255,0.2)' }, '& .Mui-selected': { bgcolor: 'rgba(0,212,255,0.2) !important', color: '#00d4ff' } }}
        />
      </Box>

      {/* Dialog: Detalle del log */}
      <Dialog open={!!logDetalle} onClose={() => setLogDetalle(null)} maxWidth="md" fullWidth
        PaperProps={{ sx: { bgcolor: '#0f1629', border: '1px solid rgba(0,212,255,0.3)', borderRadius: 3 } }}>
        <DialogTitle sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
          <Box>
            <Typography sx={{ color: '#00d4ff', fontWeight: 700, fontSize: '1.1rem' }}>Detalle del Evento</Typography>
            {logDetalle && (
              <Typography variant="caption" sx={{ color: '#b0d4ff' }}>
                {new Date(logDetalle.creadoEn).toLocaleString('es-ES')} · {logDetalle.email || 'Usuario desconocido'}
              </Typography>
            )}
          </Box>
          <IconButton onClick={() => setLogDetalle(null)} sx={{ color: '#b0d4ff' }}><CloseIcon /></IconButton>
        </DialogTitle>
        <Divider sx={{ borderColor: 'rgba(0,212,255,0.15)' }} />
        {logDetalle && (
          <DialogContent sx={{ pt: 3 }}>
            <Grid container spacing={3}>
              <Grid item xs={12} md={6}>
                <Typography variant="subtitle2" sx={{ color: '#00d4ff', mb: 1.5, fontWeight: 700 }}>Información del Evento</Typography>
                {[
                  { label: 'Acción', value: accionLabel(logDetalle.accion) },
                  { label: 'Descripción', value: logDetalle.descripcionLegible || '—' },
                  { label: 'Entidad', value: logDetalle.entidad },
                  { label: 'ID de Entidad', value: logDetalle.entidadId || '—' },
                ].map(item => (
                  <Box key={item.label} sx={{ mb: 1.5 }}>
                    <Typography variant="caption" sx={{ color: '#b0d4ff66', textTransform: 'uppercase', letterSpacing: 0.5 }}>{item.label}</Typography>
                    <Typography sx={{ color: '#fff', fontSize: '0.9rem' }}>{item.value}</Typography>
                  </Box>
                ))}
              </Grid>
              <Grid item xs={12} md={6}>
                <Typography variant="subtitle2" sx={{ color: '#00d4ff', mb: 1.5, fontWeight: 700 }}>Información Técnica</Typography>
                {[
                  { label: 'Dirección IP', value: logDetalle.ipAddress || '—', mono: true },
                  { label: 'Navegador', value: logDetalle.navegador || '—', mono: false },
                  { label: 'Sistema Operativo', value: logDetalle.sistemaOperativo || '—', mono: false },
                  { label: 'Tipo de Dispositivo', value: logDetalle.dispositivo || '—', mono: false },
                ].map(item => (
                  <Box key={item.label} sx={{ mb: 1.5 }}>
                    <Typography variant="caption" sx={{ color: '#b0d4ff66', textTransform: 'uppercase', letterSpacing: 0.5 }}>{item.label}</Typography>
                    <Typography sx={{ color: '#fff', fontSize: '0.9rem', fontFamily: item.mono ? 'monospace' : 'inherit' }}>{item.value}</Typography>
                  </Box>
                ))}
                {logDetalle.userAgent && (
                  <Box>
                    <Typography variant="caption" sx={{ color: '#b0d4ff66', textTransform: 'uppercase', letterSpacing: 0.5 }}>User-Agent Completo</Typography>
                    <Typography sx={{ color: '#b0d4ff', fontSize: '0.75rem', wordBreak: 'break-all', mt: 0.5 }}>{logDetalle.userAgent}</Typography>
                  </Box>
                )}
              </Grid>
              {(logDetalle.datosAnteriores || logDetalle.datosNuevos) && (
                <>
                  <Grid item xs={12}><Divider sx={{ borderColor: 'rgba(255,255,255,0.08)' }} /></Grid>
                  {logDetalle.datosAnteriores && (
                    <Grid item xs={12} md={6}>
                      <Typography variant="subtitle2" sx={{ color: '#ff6b35', mb: 1, fontWeight: 700 }}>Datos Anteriores</Typography>
                      <Box sx={{ bgcolor: 'rgba(255,107,53,0.08)', borderRadius: 2, p: 2, border: '1px solid rgba(255,107,53,0.2)' }}>
                        <pre style={{ color: '#ffb86c', fontSize: '0.78rem', margin: 0, whiteSpace: 'pre-wrap', wordBreak: 'break-all' }}>
                          {tryPrettyJson(logDetalle.datosAnteriores)}
                        </pre>
                      </Box>
                    </Grid>
                  )}
                  {logDetalle.datosNuevos && (
                    <Grid item xs={12} md={6}>
                      <Typography variant="subtitle2" sx={{ color: '#00ff88', mb: 1, fontWeight: 700 }}>Datos Nuevos</Typography>
                      <Box sx={{ bgcolor: 'rgba(0,255,136,0.06)', borderRadius: 2, p: 2, border: '1px solid rgba(0,255,136,0.2)' }}>
                        <pre style={{ color: '#a8ff78', fontSize: '0.78rem', margin: 0, whiteSpace: 'pre-wrap', wordBreak: 'break-all' }}>
                          {tryPrettyJson(logDetalle.datosNuevos)}
                        </pre>
                      </Box>
                    </Grid>
                  )}
                </>
              )}
            </Grid>
          </DialogContent>
        )}
        <DialogActions sx={{ px: 3, pb: 2 }}>
          <Button onClick={() => setLogDetalle(null)} variant="outlined" sx={{ color: '#b0d4ff', borderColor: 'rgba(255,255,255,0.2)' }}>Cerrar</Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
}

function tryPrettyJson(s: string) {
  try { return JSON.stringify(JSON.parse(s), null, 2); } catch { return s; }
}
