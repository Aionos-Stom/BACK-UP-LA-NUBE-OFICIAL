import { useEffect, useState } from 'react';
import {
  Box,
  Typography,
  Paper,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  IconButton,
  Button,
  Chip,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  TextField,
  MenuItem,
  Alert,
  CircularProgress,
  Badge,
  Tooltip,
} from '@mui/material';
import {
  Add,
  CheckCircle,
  Edit,
  Delete,
  Refresh,
  Warning,
} from '@mui/icons-material';
import { format } from 'date-fns';
import { alertaApi } from '../services/api';
import type { ObtenerAlertaDTO, SaveAlertaDTO, UpdateAlertaDTO } from '../types';

export default function Alertas() {
  const [alertas, setAlertas] = useState<ObtenerAlertaDTO[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [openDialog, setOpenDialog] = useState(false);
  const [editingAlerta, setEditingAlerta] = useState<ObtenerAlertaDTO | null>(null);
  const [noReconocidasCount, setNoReconocidasCount] = useState(0);
  const [formData, setFormData] = useState<SaveAlertaDTO>({
    usuarioId: 0,
    jobId: null,
    tipo: '',
    severidad: 'media',
    mensaje: '',
    isAcknowledged: false,
  });

  const loadAlertas = async () => {
    try {
      setLoading(true);
      setError(null);
      const [data, count] = await Promise.all([
        alertaApi.obtenerTodos(),
        alertaApi.contarNoReconocidas(),
      ]);
      setAlertas(data);
      setNoReconocidasCount(count);
    } catch (err) {
      setError('Error al cargar las alertas');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadAlertas();
    const interval = setInterval(loadAlertas, 30000);
    return () => clearInterval(interval);
  }, []);

  const handleOpenDialog = (alerta?: ObtenerAlertaDTO) => {
    if (alerta) {
      setEditingAlerta(alerta);
      setFormData({
        usuarioId: alerta.usuarioId,
        jobId: alerta.jobId,
        tipo: alerta.tipo,
        severidad: alerta.severidad,
        mensaje: alerta.mensaje,
        isAcknowledged: alerta.isAcknowledged,
      });
    } else {
      setEditingAlerta(null);
      setFormData({
        usuarioId: 0,
        jobId: null,
        tipo: '',
        severidad: 'media',
        mensaje: '',
        isAcknowledged: false,
      });
    }
    setOpenDialog(true);
  };

  const handleCloseDialog = () => {
    setOpenDialog(false);
    setEditingAlerta(null);
  };

  const handleSubmit = async () => {
    try {
      if (editingAlerta) {
        const updateData: UpdateAlertaDTO = {
          id: editingAlerta.id,
          jobId: formData.jobId,
          tipo: formData.tipo,
          severidad: formData.severidad,
          mensaje: formData.mensaje,
          isAcknowledged: formData.isAcknowledged,
        };
        await alertaApi.actualizar(editingAlerta.id, updateData);
      } else {
        await alertaApi.crear(formData);
      }
      handleCloseDialog();
      loadAlertas();
    } catch (err) {
      setError('Error al guardar la alerta');
      console.error(err);
    }
  };

  const handleDelete = async (id: number) => {
    if (window.confirm('¿Está seguro de eliminar esta alerta?')) {
      try {
        await alertaApi.eliminar(id);
        loadAlertas();
      } catch (err) {
        setError('Error al eliminar la alerta');
        console.error(err);
      }
    }
  };

  const handleReconocer = async (id: number) => {
    try {
      await alertaApi.marcarComoReconocida(id);
      loadAlertas();
    } catch (err) {
      setError('Error al marcar la alerta como reconocida');
      console.error(err);
    }
  };

  const getSeveridadColor = (severidad: string) => {
    switch (severidad?.toLowerCase()) {
      case 'crítica':
        return 'error';
      case 'alta':
        return 'warning';
      case 'media':
        return 'info';
      case 'baja':
        return 'default';
      default:
        return 'default';
    }
  };

  if (loading && alertas.length === 0) {
    return (
      <Box display="flex" justifyContent="center" alignItems="center" minHeight="400px">
        <CircularProgress />
      </Box>
    );
  }

  return (
    <Box>
      <Box display="flex" justifyContent="space-between" alignItems="center" mb={3}>
        <Box display="flex" alignItems="center" gap={2}>
          <Typography variant="h4" component="h1" fontWeight="bold">
            Alertas
          </Typography>
          <Badge badgeContent={noReconocidasCount} color="error">
            <Warning color="action" />
          </Badge>
        </Box>
        <Box>
          <IconButton onClick={loadAlertas} disabled={loading}>
            <Refresh />
          </IconButton>
          <Button
            variant="contained"
            startIcon={<Add />}
            onClick={() => handleOpenDialog()}
          >
            Nueva Alerta
          </Button>
        </Box>
      </Box>

      {error && (
        <Alert severity="error" onClose={() => setError(null)} sx={{ mb: 2 }}>
          {error}
        </Alert>
      )}

      <TableContainer component={Paper}>
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>ID</TableCell>
              <TableCell>Usuario</TableCell>
              <TableCell>Job</TableCell>
              <TableCell>Tipo</TableCell>
              <TableCell>Severidad</TableCell>
              <TableCell>Mensaje</TableCell>
              <TableCell>Reconocida</TableCell>
              <TableCell>Fecha</TableCell>
              <TableCell align="right">Acciones</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {alertas.map((alerta) => (
              <TableRow key={alerta.id}>
                <TableCell>{alerta.id}</TableCell>
                <TableCell>{alerta.usuarioNombre}</TableCell>
                <TableCell>{alerta.jobNombre || '-'}</TableCell>
                <TableCell>{alerta.tipo}</TableCell>
                <TableCell>
                  <Chip
                    label={alerta.severidad}
                    color={getSeveridadColor(alerta.severidad) as any}
                    size="small"
                  />
                </TableCell>
                <TableCell>
                  <Tooltip title={alerta.mensaje}>
                    <Typography variant="body2" noWrap sx={{ maxWidth: 200 }}>
                      {alerta.mensaje}
                    </Typography>
                  </Tooltip>
                </TableCell>
                <TableCell>
                  <Chip
                    label={alerta.isAcknowledged ? 'Sí' : 'No'}
                    color={alerta.isAcknowledged ? 'success' : 'default'}
                    size="small"
                  />
                </TableCell>
                <TableCell>
                  {format(new Date(alerta.createdAt), 'dd/MM/yyyy HH:mm')}
                </TableCell>
                <TableCell align="right">
                  {!alerta.isAcknowledged && (
                    <IconButton
                      size="small"
                      color="success"
                      onClick={() => handleReconocer(alerta.id)}
                      title="Marcar como reconocida"
                    >
                      <CheckCircle />
                    </IconButton>
                  )}
                  <IconButton
                    size="small"
                    color="primary"
                    onClick={() => handleOpenDialog(alerta)}
                    title="Editar"
                  >
                    <Edit />
                  </IconButton>
                  <IconButton
                    size="small"
                    color="error"
                    onClick={() => handleDelete(alerta.id)}
                    title="Eliminar"
                  >
                    <Delete />
                  </IconButton>
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </TableContainer>

      <Dialog open={openDialog} onClose={handleCloseDialog} maxWidth="sm" fullWidth>
        <DialogTitle>
          {editingAlerta ? 'Editar Alerta' : 'Nueva Alerta'}
        </DialogTitle>
        <DialogContent>
          <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2, mt: 1 }}>
            <TextField
              label="Usuario ID"
              type="number"
              value={formData.usuarioId}
              onChange={(e) =>
                setFormData({ ...formData, usuarioId: parseInt(e.target.value) })
              }
              fullWidth
              required
            />
            <TextField
              label="Job ID"
              type="number"
              value={formData.jobId || ''}
              onChange={(e) =>
                setFormData({
                  ...formData,
                  jobId: e.target.value ? parseInt(e.target.value) : null,
                })
              }
              fullWidth
            />
            <TextField
              label="Tipo"
              value={formData.tipo}
              onChange={(e) => setFormData({ ...formData, tipo: e.target.value })}
              fullWidth
              required
            />
            <TextField
              label="Severidad"
              select
              value={formData.severidad}
              onChange={(e) => setFormData({ ...formData, severidad: e.target.value })}
              fullWidth
              required
            >
              <MenuItem value="baja">Baja</MenuItem>
              <MenuItem value="media">Media</MenuItem>
              <MenuItem value="alta">Alta</MenuItem>
              <MenuItem value="crítica">Crítica</MenuItem>
            </TextField>
            <TextField
              label="Mensaje"
              value={formData.mensaje}
              onChange={(e) => setFormData({ ...formData, mensaje: e.target.value })}
              fullWidth
              required
              multiline
              rows={3}
            />
            {editingAlerta && (
              <TextField
                label="Reconocida"
                select
                value={formData.isAcknowledged ? 'true' : 'false'}
                onChange={(e) =>
                  setFormData({ ...formData, isAcknowledged: e.target.value === 'true' })
                }
                fullWidth
              >
                <MenuItem value="false">No</MenuItem>
                <MenuItem value="true">Sí</MenuItem>
              </TextField>
            )}
          </Box>
        </DialogContent>
        <DialogActions>
          <Button onClick={handleCloseDialog}>Cancelar</Button>
          <Button onClick={handleSubmit} variant="contained">
            {editingAlerta ? 'Actualizar' : 'Crear'}
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
}

