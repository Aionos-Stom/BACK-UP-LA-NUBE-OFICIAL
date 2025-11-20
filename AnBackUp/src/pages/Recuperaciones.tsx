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
  Alert,
  CircularProgress,
  LinearProgress,
  Tooltip,
} from '@mui/material';
import {
  Add,
  PlayArrow,
  //Edit,
  Delete,
  Refresh,
  Science,
} from '@mui/icons-material';
import { format } from 'date-fns';
import { recuperacionApi } from '../services/api';
import type { ObtenerRecuperacionDTO, SaveRecuperacionDTO } from '../types';

export default function Recuperaciones() {
  const [recuperaciones, setRecuperaciones] = useState<ObtenerRecuperacionDTO[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [openDialog, setOpenDialog] = useState(false);
  const [formData, setFormData] = useState<SaveRecuperacionDTO>({
    jobId: 0,
    usuarioId: 0,
    destino: '',
  });

  const loadRecuperaciones = async () => {
    try {
      setLoading(true);
      setError(null);
      const result = await recuperacionApi.obtenerTodos();
      if (result.isSuccess && result.data) {
        setRecuperaciones(result.data);
      } else {
        setError(result.message || 'Error al cargar las recuperaciones');
      }
    } catch (err) {
      setError('Error al cargar las recuperaciones');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadRecuperaciones();
  }, []);

  const handleOpenDialog = () => {
    setFormData({
      jobId: 0,
      usuarioId: 0,
      destino: '',
    });
    setOpenDialog(true);
  };

  const handleCloseDialog = () => {
    setOpenDialog(false);
  };

  const handleSubmit = async () => {
    try {
      await recuperacionApi.crear(formData);
      handleCloseDialog();
      loadRecuperaciones();
    } catch (err) {
      setError('Error al crear la recuperación');
      console.error(err);
    }
  };

  const handleDelete = async (id: number) => {
    if (window.confirm('¿Está seguro de eliminar esta recuperación?')) {
      try {
        await recuperacionApi.eliminar(id);
        loadRecuperaciones();
      } catch (err) {
        setError('Error al eliminar la recuperación');
        console.error(err);
      }
    }
  };

  const handleEjecutar = async (id: number) => {
    try {
      await recuperacionApi.ejecutar(id);
      loadRecuperaciones();
    } catch (err) {
      setError('Error al ejecutar la recuperación');
      console.error(err);
    }
  };

  const handleSimular = async (id: number) => {
    try {
      await recuperacionApi.simular(id);
      loadRecuperaciones();
    } catch (err) {
      setError('Error al simular la recuperación');
      console.error(err);
    }
  };

  const getEstadoColor = (estado: string) => {
    switch (estado?.toLowerCase()) {
      case 'completado':
        return 'success';
      case 'error':
      case 'fallido':
        return 'error';
      case 'en_proceso':
      case 'ejecutando':
        return 'warning';
      default:
        return 'default';
    }
  };

  if (loading && recuperaciones.length === 0) {
    return (
      <Box display="flex" justifyContent="center" alignItems="center" minHeight="400px">
        <CircularProgress />
      </Box>
    );
  }

  return (
    <Box>
      <Box display="flex" justifyContent="space-between" alignItems="center" mb={3}>
        <Typography variant="h4" component="h1" fontWeight="bold">
          Recuperaciones
        </Typography>
        <Box>
          <IconButton onClick={loadRecuperaciones} disabled={loading}>
            <Refresh />
          </IconButton>
          <Button
            variant="contained"
            startIcon={<Add />}
            onClick={handleOpenDialog}
          >
            Nueva Recuperación
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
              <TableCell>Job ID</TableCell>
              <TableCell>Usuario ID</TableCell>
              <TableCell>Destino</TableCell>
              <TableCell>Estado</TableCell>
              <TableCell>Progreso</TableCell>
              <TableCell>Fecha Inicio</TableCell>
              <TableCell>Fecha Completado</TableCell>
              <TableCell>Error</TableCell>
              <TableCell>Fecha Creación</TableCell>
              <TableCell align="right">Acciones</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
      {recuperaciones.map((recuperacion) => (
        <TableRow key={recuperacion.id}>
          <TableCell>{recuperacion.id}</TableCell>
          <TableCell>{recuperacion.jobId}</TableCell>
          <TableCell>{recuperacion.usuarioId}</TableCell>
          <TableCell>
            <Tooltip title={recuperacion.destino || 'Sin destino'}>
              <Typography variant="body2" noWrap sx={{ maxWidth: 150 }}>
                {recuperacion.destino || 'N/A'}
              </Typography>
            </Tooltip>
          </TableCell>
          <TableCell>
            <Chip
          label={recuperacion.estado || 'Desconocido'}
          color={getEstadoColor(recuperacion.estado) as any}
          size="small"
        />
      </TableCell>
      <TableCell>
        <Box sx={{ width: 100 }}>
          <LinearProgress
            variant="determinate"
            value={recuperacion.porcentajeCompletado || 0}
            color={
              recuperacion.porcentajeCompletado === 100
                ? 'success'
                : (recuperacion.porcentajeCompletado || 0) > 0
                ? 'primary'
                : 'inherit'
            }
          />
          <Typography variant="caption" color="textSecondary">
            {recuperacion.porcentajeCompletado || 0}%
          </Typography>
        </Box>
      </TableCell>
      <TableCell>
        {recuperacion.fechaInicio && !isNaN(new Date(recuperacion.fechaInicio).getTime())
          ? format(new Date(recuperacion.fechaInicio), 'dd/MM/yyyy HH:mm')
          : '-'}
      </TableCell>
      <TableCell>
        {recuperacion.fechaCompletado && !isNaN(new Date(recuperacion.fechaCompletado).getTime())
          ? format(new Date(recuperacion.fechaCompletado), 'dd/MM/yyyy HH:mm')
          : '-'}
      </TableCell>
      <TableCell>
        {recuperacion.errorMessage ? (
          <Tooltip title={recuperacion.errorMessage}>
            <Typography variant="body2" color="error" noWrap sx={{ maxWidth: 150 }}>
              {recuperacion.errorMessage}
            </Typography>
          </Tooltip>
        ) : (
          '-'
        )}
      </TableCell>
      <TableCell>
        {recuperacion.fechaCreacion && !isNaN(new Date(recuperacion.fechaCreacion).getTime())
          ? format(new Date(recuperacion.fechaCreacion), 'dd/MM/yyyy HH:mm')
          : 'Fecha inválida'}
      </TableCell>
      <TableCell align="right">
        <IconButton
          size="small"
          color="primary"
          onClick={() => handleEjecutar(recuperacion.id)}
          title="Ejecutar"
        >
          <PlayArrow />
        </IconButton>
        <IconButton
          size="small"
          color="info"
          onClick={() => handleSimular(recuperacion.id)}
          title="Simular"
        >
          <Science />
        </IconButton>
        <IconButton
          size="small"
          color="error"
          onClick={() => handleDelete(recuperacion.id)}
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
        <DialogTitle>Nueva Recuperación</DialogTitle>
        <DialogContent>
          <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2, mt: 1 }}>
            <TextField
              label="Job ID"
              type="number"
              value={formData.jobId}
              onChange={(e) =>
                setFormData({ ...formData, jobId: parseInt(e.target.value) })
              }
              fullWidth
              required
            />
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
              label="Destino"
              value={formData.destino}
              onChange={(e) => setFormData({ ...formData, destino: e.target.value })}
              fullWidth
              required
              multiline
              rows={2}
            />
          </Box>
        </DialogContent>
        <DialogActions>
          <Button onClick={handleCloseDialog}>Cancelar</Button>
          <Button onClick={handleSubmit} variant="contained">
            Crear
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
}

