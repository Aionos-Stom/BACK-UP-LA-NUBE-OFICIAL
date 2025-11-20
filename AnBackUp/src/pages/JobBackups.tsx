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
  Tooltip,
} from '@mui/material';
import {
  Add,
  PlayArrow,
  Edit,
  Delete,
  Refresh,
} from '@mui/icons-material';
import { format } from 'date-fns';
import { jobBackupApi } from '../services/api';
import type { ObtenerJobBackupDTO, SaveJobBackupDTO } from '../types';

export default function JobBackups() {
  const [jobs, setJobs] = useState<ObtenerJobBackupDTO[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [openDialog, setOpenDialog] = useState(false);
  const [editingJob, setEditingJob] = useState<ObtenerJobBackupDTO | null>(null);
  const [formData, setFormData] = useState<SaveJobBackupDTO>({
    politicaId: 0,
    cloudStorageId: 0,
    fechaProgramada: undefined,
    sourceData: '',
  });

  const loadJobs = async () => {
    try {
      setLoading(true);
      setError(null);
      const data = await jobBackupApi.obtenerTodos();
      setJobs(data);
    } catch (err) {
      setError('Error al cargar los jobs de backup');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadJobs();
  }, []);

  const handleOpenDialog = (job?: ObtenerJobBackupDTO) => {
    if (job) {
      setEditingJob(job);
      setFormData({
        politicaId: job.politicaId,
        cloudStorageId: job.cloudStorageId,
        fechaProgramada: job.fechaProgramada || undefined,
        sourceData: job.sourceData || '',
      });
    } else {
      setEditingJob(null);
      setFormData({
        politicaId: 0,
        cloudStorageId: 0,
        fechaProgramada: undefined,
        sourceData: '',
      });
    }
    setOpenDialog(true);
  };

  const handleCloseDialog = () => {
    setOpenDialog(false);
    setEditingJob(null);
  };

  const handleSubmit = async () => {
    try {
      if (editingJob) {
        await jobBackupApi.actualizar({
          id: editingJob.id,
          ...formData,
        });
      } else {
        await jobBackupApi.crear(formData);
      }
      handleCloseDialog();
      loadJobs();
    } catch (err) {
      setError('Error al guardar el job');
      console.error(err);
    }
  };

  const handleDelete = async (id: number) => {
    if (window.confirm('¿Está seguro de eliminar este job?')) {
      try {
        await jobBackupApi.eliminar(id);
        loadJobs();
      } catch (err) {
        setError('Error al eliminar el job');
        console.error(err);
      }
    }
  };

  const handleEjecutar = async (id: number) => {
    try {
      await jobBackupApi.ejecutar(id);
      loadJobs();
    } catch (err) {
      setError('Error al ejecutar el job');
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

  const formatBytes = (bytes: number | null) => {
    if (!bytes) return '-';
    const gb = bytes / (1024 * 1024 * 1024);
    return `${gb.toFixed(2)} GB`;
  };

  const formatDuration = (seconds: number | null) => {
    if (!seconds) return '-';
    const mins = Math.floor(seconds / 60);
    const secs = seconds % 60;
    return `${mins}m ${secs}s`;
  };

  if (loading && jobs.length === 0) {
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
          Jobs de Backup
        </Typography>
        <Box>
          <IconButton onClick={loadJobs} disabled={loading}>
            <Refresh />
          </IconButton>
          <Button
            variant="contained"
            startIcon={<Add />}
            onClick={() => handleOpenDialog()}
          >
            Nuevo Job
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
              <TableCell>Política ID</TableCell>
              <TableCell>Cloud Storage ID</TableCell>
              <TableCell>Estado</TableCell>
              <TableCell>Fecha Programada</TableCell>
              <TableCell>Fecha Ejecución</TableCell>
              <TableCell>Fecha Completado</TableCell>
              <TableCell>Tamaño</TableCell>
              <TableCell>Duración</TableCell>
              <TableCell>Source Data</TableCell>
              <TableCell align="right">Acciones</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {jobs.map((job) => (
              <TableRow key={job.id}>
                <TableCell>{job.id}</TableCell>
                <TableCell>{job.politicaId}</TableCell>
                <TableCell>{job.cloudStorageId}</TableCell>
                <TableCell>
                  <Chip
                    label={job.estado}
                    color={getEstadoColor(job.estado) as any}
                    size="small"
                  />
                </TableCell>
                <TableCell>
                  {job.fechaProgramada
                    ? format(new Date(job.fechaProgramada), 'dd/MM/yyyy HH:mm')
                    : '-'}
                </TableCell>
                <TableCell>
                  {job.fechaEjecucion
                    ? format(new Date(job.fechaEjecucion), 'dd/MM/yyyy HH:mm')
                    : '-'}
                </TableCell>
                <TableCell>
                  {job.fechaCompletado
                    ? format(new Date(job.fechaCompletado), 'dd/MM/yyyy HH:mm')
                    : '-'}
                </TableCell>
                <TableCell>{formatBytes(job.tamanoBytes)}</TableCell>
                <TableCell>{formatDuration(job.duracionSegundos)}</TableCell>
                <TableCell>
                  <Tooltip title={job.sourceData || '-'}>
                    <Typography variant="body2" noWrap sx={{ maxWidth: 150 }}>
                      {job.sourceData || '-'}
                    </Typography>
                  </Tooltip>
                </TableCell>
                <TableCell align="right">
                  <IconButton
                    size="small"
                    color="primary"
                    onClick={() => handleEjecutar(job.id)}
                    title="Ejecutar"
                  >
                    <PlayArrow />
                  </IconButton>
                  <IconButton
                    size="small"
                    color="primary"
                    onClick={() => handleOpenDialog(job)}
                    title="Editar"
                  >
                    <Edit />
                  </IconButton>
                  <IconButton
                    size="small"
                    color="error"
                    onClick={() => handleDelete(job.id)}
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
          {editingJob ? 'Editar Job de Backup' : 'Nuevo Job de Backup'}
        </DialogTitle>
        <DialogContent>
          <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2, mt: 1 }}>
            <TextField
              label="Política ID"
              type="number"
              value={formData.politicaId}
              onChange={(e) =>
                setFormData({ ...formData, politicaId: parseInt(e.target.value) })
              }
              fullWidth
              required
            />
            <TextField
              label="Cloud Storage ID"
              type="number"
              value={formData.cloudStorageId}
              onChange={(e) =>
                setFormData({ ...formData, cloudStorageId: parseInt(e.target.value) })
              }
              fullWidth
              required
            />
            <TextField
              label="Fecha Programada"
              type="datetime-local"
              value={formData.fechaProgramada ? new Date(formData.fechaProgramada).toISOString().slice(0, 16) : ''}
              onChange={(e) =>
                setFormData({
                  ...formData,
                  fechaProgramada: e.target.value ? new Date(e.target.value).toISOString() : undefined,
                })
              }
              fullWidth
              InputLabelProps={{ shrink: true }}
            />
            <TextField
              label="Source Data"
              value={formData.sourceData}
              onChange={(e) => setFormData({ ...formData, sourceData: e.target.value })}
              fullWidth
              multiline
              rows={3}
            />
          </Box>
        </DialogContent>
        <DialogActions>
          <Button onClick={handleCloseDialog}>Cancelar</Button>
          <Button onClick={handleSubmit} variant="contained">
            {editingJob ? 'Actualizar' : 'Crear'}
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
}

