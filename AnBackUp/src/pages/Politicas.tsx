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
  Switch,
  FormControlLabel,
  Alert,
  CircularProgress,
} from '@mui/material';
import {
  Add,
  Edit,
  Delete,
  Refresh,
  PowerSettingsNew,
} from '@mui/icons-material';
import { format } from 'date-fns';
import { politicaBackupApi } from '../services/api';
import type { ObtenerPoliticaBackupDTO, SavePoliticaBackupDTO, UpdatePoliticaBackupDTO } from '../types';

export default function Politicas() {
  const [politicas, setPoliticas] = useState<ObtenerPoliticaBackupDTO[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [openDialog, setOpenDialog] = useState(false);
  const [editingPolitica, setEditingPolitica] = useState<ObtenerPoliticaBackupDTO | null>(null);
  const [formData, setFormData] = useState<SavePoliticaBackupDTO>({
    organizacionId: 0,
    nombre: '',
    tipoBackup: '',
    frecuencia: '',
    horaEjecucion: '',
    retencionDias: 0,
    compresion: false,
    encriptacion: false,
    activo: true,
  });

  const loadPoliticas = async () => {
    try {
      setLoading(true);
      setError(null);
      const result = await politicaBackupApi.obtenerTodos();
      if (result.isSuccess && result.data) {
        setPoliticas(result.data);
      } else {
        setError(result.message || 'Error al cargar las políticas');
      }
    } catch (err) {
      setError('Error al cargar las políticas de backup');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadPoliticas();
  }, []);

  const handleOpenDialog = (politica?: ObtenerPoliticaBackupDTO) => {
    if (politica) {
      setEditingPolitica(politica);
      setFormData({
        organizacionId: politica.organizacionId,
        nombre: politica.nombre,
        tipoBackup: politica.tipoBackup,
        frecuencia: politica.frecuencia,
        horaEjecucion: politica.horaEjecucion,
        retencionDias: politica.retencionDias,
        compresion: politica.compresion,
        encriptacion: politica.encriptacion,
        activo: politica.activo,
      });
    } else {
      setEditingPolitica(null);
      setFormData({
        organizacionId: 0,
        nombre: '',
        tipoBackup: '',
        frecuencia: '',
        horaEjecucion: '',
        retencionDias: 0,
        compresion: false,
        encriptacion: false,
        activo: true,
      });
    }
    setOpenDialog(true);
  };

  const handleCloseDialog = () => {
    setOpenDialog(false);
    setEditingPolitica(null);
  };

  const handleSubmit = async () => {
    try {
      if (editingPolitica) {
        const updateData: UpdatePoliticaBackupDTO = {
          id: editingPolitica.id,
          ...formData,
        };
        await politicaBackupApi.actualizar(updateData);
      } else {
        await politicaBackupApi.crear(formData);
      }
      handleCloseDialog();
      loadPoliticas();
    } catch (err) {
      setError('Error al guardar la política');
      console.error(err);
    }
  };

  const handleDelete = async (id: number) => {
    if (window.confirm('¿Está seguro de eliminar esta política?')) {
      try {
        await politicaBackupApi.eliminar(id);
        loadPoliticas();
      } catch (err) {
        setError('Error al eliminar la política');
        console.error(err);
      }
    }
  };

  const handleToggleEstado = async (id: number, activo: boolean) => {
    try {
      await politicaBackupApi.cambiarEstado(id, !activo);
      loadPoliticas();
    } catch (err) {
      setError('Error al cambiar el estado de la política');
      console.error(err);
    }
  };

  if (loading && politicas.length === 0) {
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
          Políticas de Backup
        </Typography>
        <Box>
          <IconButton onClick={loadPoliticas} disabled={loading}>
            <Refresh />
          </IconButton>
          <Button
            variant="contained"
            startIcon={<Add />}
            onClick={() => handleOpenDialog()}
          >
            Nueva Política
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
              <TableCell>Nombre</TableCell>
              <TableCell>Tipo</TableCell>
              <TableCell>Frecuencia</TableCell>
              <TableCell>Hora Ejecución</TableCell>
              <TableCell>Retención (días)</TableCell>
              <TableCell>Compresión</TableCell>
              <TableCell>Encriptación</TableCell>
              <TableCell>Activo</TableCell>
              <TableCell>Fecha Creación</TableCell>
              <TableCell align="right">Acciones</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {politicas.map((politica) => (
              <TableRow key={politica.id}>
                <TableCell>{politica.id}</TableCell>
                <TableCell>{politica.nombre}</TableCell>
                <TableCell>{politica.tipoBackup}</TableCell>
                <TableCell>{politica.frecuencia}</TableCell>
                <TableCell>{politica.horaEjecucion}</TableCell>
                <TableCell>{politica.retencionDias}</TableCell>
                <TableCell>
                  <Chip
                    label={politica.compresion ? 'Sí' : 'No'}
                    color={politica.compresion ? 'success' : 'default'}
                    size="small"
                  />
                </TableCell>
                <TableCell>
                  <Chip
                    label={politica.encriptacion ? 'Sí' : 'No'}
                    color={politica.encriptacion ? 'success' : 'default'}
                    size="small"
                  />
                </TableCell>
                <TableCell>
                  <Chip
                    label={politica.activo ? 'Activo' : 'Inactivo'}
                    color={politica.activo ? 'success' : 'default'}
                    size="small"
                  />
                </TableCell>
                <TableCell>
                  {format(new Date(politica.fechaCreacion), 'dd/MM/yyyy')}
                </TableCell>
                <TableCell align="right">
                  <IconButton
                    size="small"
                    color={politica.activo ? 'default' : 'success'}
                    onClick={() => handleToggleEstado(politica.id, politica.activo)}
                    title={politica.activo ? 'Desactivar' : 'Activar'}
                  >
                    <PowerSettingsNew />
                  </IconButton>
                  <IconButton
                    size="small"
                    color="primary"
                    onClick={() => handleOpenDialog(politica)}
                    title="Editar"
                  >
                    <Edit />
                  </IconButton>
                  <IconButton
                    size="small"
                    color="error"
                    onClick={() => handleDelete(politica.id)}
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

      <Dialog open={openDialog} onClose={handleCloseDialog} maxWidth="md" fullWidth>
        <DialogTitle>
          {editingPolitica ? 'Editar Política de Backup' : 'Nueva Política de Backup'}
        </DialogTitle>
        <DialogContent>
          <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2, mt: 1 }}>
            <TextField
              label="Organización ID"
              type="number"
              value={formData.organizacionId}
              onChange={(e) =>
                setFormData({ ...formData, organizacionId: parseInt(e.target.value) })
              }
              fullWidth
              required
            />
            <TextField
              label="Nombre"
              value={formData.nombre}
              onChange={(e) => setFormData({ ...formData, nombre: e.target.value })}
              fullWidth
              required
            />
            <TextField
              label="Tipo de Backup"
              value={formData.tipoBackup}
              onChange={(e) => setFormData({ ...formData, tipoBackup: e.target.value })}
              fullWidth
              required
            />
            <TextField
              label="Frecuencia"
              value={formData.frecuencia}
              onChange={(e) => setFormData({ ...formData, frecuencia: e.target.value })}
              fullWidth
              required
            />
            <TextField
              label="Hora de Ejecución"
              type="time"
              value={formData.horaEjecucion}
              onChange={(e) => setFormData({ ...formData, horaEjecucion: e.target.value })}
              fullWidth
              required
              InputLabelProps={{ shrink: true }}
            />
            <TextField
              label="Retención (días)"
              type="number"
              value={formData.retencionDias}
              onChange={(e) =>
                setFormData({ ...formData, retencionDias: parseInt(e.target.value) })
              }
              fullWidth
              required
            />
            <FormControlLabel
              control={
                <Switch
                  checked={formData.compresion}
                  onChange={(e) => setFormData({ ...formData, compresion: e.target.checked })}
                />
              }
              label="Compresión"
            />
            <FormControlLabel
              control={
                <Switch
                  checked={formData.encriptacion}
                  onChange={(e) => setFormData({ ...formData, encriptacion: e.target.checked })}
                />
              }
              label="Encriptación"
            />
            <FormControlLabel
              control={
                <Switch
                  checked={formData.activo}
                  onChange={(e) => setFormData({ ...formData, activo: e.target.checked })}
                />
              }
              label="Activo"
            />
          </Box>
        </DialogContent>
        <DialogActions>
          <Button onClick={handleCloseDialog}>Cancelar</Button>
          <Button onClick={handleSubmit} variant="contained">
            {editingPolitica ? 'Actualizar' : 'Crear'}
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
}

