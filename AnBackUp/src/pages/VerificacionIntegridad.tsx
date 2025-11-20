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
  Tooltip,
} from '@mui/material';
import {
  Add,
  Verified,
  Delete,
  Refresh,
  CheckCircle,
  Cancel,
} from '@mui/icons-material';
import { format } from 'date-fns';
import { verificacionIntegridadApi } from '../services/api';
import type { ObtenerVerificacionIntegridadDTO, SaveVerificacionIntegridadDTO } from '../types';

export default function VerificacionIntegridad() {
  const [verificaciones, setVerificaciones] = useState<ObtenerVerificacionIntegridadDTO[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [openDialog, setOpenDialog] = useState(false);
  const [formData, setFormData] = useState<SaveVerificacionIntegridadDTO>({
    jobId: 0,
    hashOriginal: '',
    observaciones: '',
  });

  const loadVerificaciones = async () => {
    try {
      setLoading(true);
      setError(null);
      const result = await verificacionIntegridadApi.obtenerTodos();
      if (result.isSuccess && result.data) {
        setVerificaciones(result.data);
      } else {
        setError(result.message || 'Error al cargar las verificaciones');
      }
    } catch (err) {
      setError('Error al cargar las verificaciones de integridad');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadVerificaciones();
  }, []);

  const handleOpenDialog = () => {
    setFormData({
      jobId: 0,
      hashOriginal: '',
      observaciones: '',
    });
    setOpenDialog(true);
  };

  const handleCloseDialog = () => {
    setOpenDialog(false);
  };

  const handleSubmit = async () => {
    try {
      await verificacionIntegridadApi.crear(formData);
      handleCloseDialog();
      loadVerificaciones();
    } catch (err) {
      setError('Error al crear la verificación');
      console.error(err);
    }
  };

  const handleDelete = async (id: number) => {
    if (window.confirm('¿Está seguro de eliminar esta verificación?')) {
      try {
        await verificacionIntegridadApi.eliminar(id);
        loadVerificaciones();
      } catch (err) {
        setError('Error al eliminar la verificación');
        console.error(err);
      }
    }
  };

  const handleVerificar = async (jobId: number) => {
    try {
      await verificacionIntegridadApi.verificarIntegridad(jobId);
      loadVerificaciones();
    } catch (err) {
      setError('Error al verificar la integridad');
      console.error(err);
    }
  };

  if (loading && verificaciones.length === 0) {
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
          Verificación de Integridad
        </Typography>
        <Box>
          <IconButton onClick={loadVerificaciones} disabled={loading}>
            <Refresh />
          </IconButton>
          <Button
            variant="contained"
            startIcon={<Add />}
            onClick={handleOpenDialog}
          >
            Nueva Verificación
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
              <TableCell>Resultado</TableCell>
              <TableCell>Integridad OK</TableCell>
              <TableCell>Hash Original</TableCell>
              <TableCell>Hash Verificado</TableCell>
              <TableCell>Fecha Verificación</TableCell>
              <TableCell>Observaciones</TableCell>
              <TableCell align="right">Acciones</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {verificaciones.map((verificacion) => (
              <TableRow key={verificacion.id}>
                <TableCell>{verificacion.id}</TableCell>
                <TableCell>{verificacion.jobId}</TableCell>
                <TableCell>
                  <Chip
                    label={verificacion.resultado}
                    color={verificacion.integridadOk ? 'success' : 'error'}
                    size="small"
                  />
                </TableCell>
                <TableCell>
                  {verificacion.integridadOk ? (
                    <CheckCircle color="success" />
                  ) : (
                    <Cancel color="error" />
                  )}
                </TableCell>
                <TableCell>
                  <Tooltip title={verificacion.hashOriginal || '-'}>
                    <Typography variant="body2" noWrap sx={{ maxWidth: 150, fontFamily: 'monospace' }}>
                      {verificacion.hashOriginal || '-'}
                    </Typography>
                  </Tooltip>
                </TableCell>
                <TableCell>
                  <Tooltip title={verificacion.hashVerificado || '-'}>
                    <Typography variant="body2" noWrap sx={{ maxWidth: 150, fontFamily: 'monospace' }}>
                      {verificacion.hashVerificado || '-'}
                    </Typography>
                  </Tooltip>
                </TableCell>
                <TableCell>
                  {format(new Date(verificacion.fechaVerificacion), 'dd/MM/yyyy HH:mm')}
                </TableCell>
                <TableCell>
                  {verificacion.observaciones ? (
                    <Tooltip title={verificacion.observaciones}>
                      <Typography variant="body2" noWrap sx={{ maxWidth: 150 }}>
                        {verificacion.observaciones}
                      </Typography>
                    </Tooltip>
                  ) : (
                    '-'
                  )}
                </TableCell>
                <TableCell align="right">
                  <IconButton
                    size="small"
                    color="primary"
                    onClick={() => handleVerificar(verificacion.jobId)}
                    title="Verificar Integridad"
                  >
                    <Verified />
                  </IconButton>
                  <IconButton
                    size="small"
                    color="error"
                    onClick={() => handleDelete(verificacion.id)}
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
        <DialogTitle>Nueva Verificación de Integridad</DialogTitle>
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
              label="Hash Original"
              value={formData.hashOriginal}
              onChange={(e) => setFormData({ ...formData, hashOriginal: e.target.value })}
              fullWidth
              multiline
              rows={2}
            />
            <TextField
              label="Observaciones"
              value={formData.observaciones}
              onChange={(e) => setFormData({ ...formData, observaciones: e.target.value })}
              fullWidth
              multiline
              rows={3}
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

