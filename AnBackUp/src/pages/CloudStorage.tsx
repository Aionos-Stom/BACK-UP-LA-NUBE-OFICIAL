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
} from '@mui/material';
import {
  Add,
  Edit,
  Delete,
  Refresh,
} from '@mui/icons-material';
import { format } from 'date-fns';
import { cloudStorageApi } from '../services/api';
import type { ObtenerCloudStorageDTO, SaveCloudStorageDTO, UpdateCloudStorageDTO } from '../types';

export default function CloudStorage() {
  const [storages, setStorages] = useState<ObtenerCloudStorageDTO[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [openDialog, setOpenDialog] = useState(false);
  const [editingStorage, setEditingStorage] = useState<ObtenerCloudStorageDTO | null>(null);
  const [formData, setFormData] = useState<SaveCloudStorageDTO>({
    organizacionId: 0,
    proveedor: '',
    nombre: '',
    tipoAlmacenamiento: '',
    capacidadTB: 0,
    credenciales: '',
    configuracion: '',
  });

  const loadStorages = async () => {
    try {
      setLoading(true);
      setError(null);
      const data = await cloudStorageApi.obtenerTodos();
      setStorages(data);
    } catch (err) {
      setError('Error al cargar los cloud storages');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadStorages();
  }, []);

  const handleOpenDialog = (storage?: ObtenerCloudStorageDTO) => {
    if (storage) {
      setEditingStorage(storage);
      setFormData({
        organizacionId: storage.organizacionId,
        proveedor: storage.proveedor,
        nombre: storage.nombre,
        tipoAlmacenamiento: storage.tipoAlmacenamiento,
        capacidadTB: storage.capacidadTB,
        credenciales: storage.credenciales,
        configuracion: storage.configuracion,
      });
    } else {
      setEditingStorage(null);
      setFormData({
        organizacionId: 0,
        proveedor: '',
        nombre: '',
        tipoAlmacenamiento: '',
        capacidadTB: 0,
        credenciales: '',
        configuracion: '',
      });
    }
    setOpenDialog(true);
  };

  const handleCloseDialog = () => {
    setOpenDialog(false);
    setEditingStorage(null);
  };

  const handleSubmit = async () => {
    try {
      if (editingStorage) {
        const updateData: UpdateCloudStorageDTO = {
          id: editingStorage.id,
          ...formData,
        };
        await cloudStorageApi.actualizar(editingStorage.id, updateData);
      } else {
        await cloudStorageApi.crear(formData);
      }
      handleCloseDialog();
      loadStorages();
    } catch (err) {
      setError('Error al guardar el cloud storage');
      console.error(err);
    }
  };

  const handleDelete = async (id: number) => {
    if (window.confirm('¿Está seguro de eliminar este cloud storage?')) {
      try {
        await cloudStorageApi.eliminar(id);
        loadStorages();
      } catch (err) {
        setError('Error al eliminar el cloud storage');
        console.error(err);
      }
    }
  };

  const getEstadoColor = (estado: string) => {
    switch (estado?.toLowerCase()) {
      case 'activo':
        return 'success';
      case 'inactivo':
        return 'default';
      case 'error':
        return 'error';
      default:
        return 'default';
    }
  };

  const getPorcentajeUso = (usado: number, total: number) => {
    return total > 0 ? (usado / total) * 100 : 0;
  };

  if (loading && storages.length === 0) {
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
          Cloud Storage
        </Typography>
        <Box>
          <IconButton onClick={loadStorages} disabled={loading}>
            <Refresh />
          </IconButton>
          <Button
            variant="contained"
            startIcon={<Add />}
            onClick={() => handleOpenDialog()}
          >
            Nuevo Storage
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
              <TableCell>Proveedor</TableCell>
              <TableCell>Tipo</TableCell>
              <TableCell>Organización ID</TableCell>
              <TableCell>Capacidad</TableCell>
              <TableCell>Usado</TableCell>
              <TableCell>Uso</TableCell>
              <TableCell>Estado</TableCell>
              <TableCell>Fecha Creación</TableCell>
              <TableCell align="right">Acciones</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
  {storages.map((storage) => {
    const porcentaje = getPorcentajeUso(storage.usadoTB || 0, storage.capacidadTB || 1);
    return (
      <TableRow key={storage.id}>
        <TableCell>{storage.id}</TableCell>
        <TableCell>{storage.configuracion}</TableCell>
        <TableCell>{storage.proveedor}</TableCell>
        <TableCell>{storage.estado}</TableCell>
        <TableCell>{storage.organizacionId || 0}</TableCell>
        <TableCell>{(storage.capacidadTB || 0).toFixed(2)} TB</TableCell>
        <TableCell>{(storage.usadoTB || 0).toFixed(2)} TB</TableCell>
        <TableCell>
          <Box sx={{ width: 100 }}>
            <LinearProgress
              variant="determinate"
              value={porcentaje}
              color={porcentaje > 90 ? 'error' : porcentaje > 70 ? 'warning' : 'primary'}
            />
            <Typography variant="caption" color="textSecondary">
              {porcentaje.toFixed(1)}%
            </Typography>
          </Box>
        </TableCell>
        <TableCell>
          <Chip
            label={storage.estado || 'Desconocido'}
            color={getEstadoColor(storage.estado) as any}
            size="small"
          />
        </TableCell>
        <TableCell>
          {storage.fechaCreacion ? format(new Date(storage.fechaCreacion), 'dd/MM/yyyy') : 'N/A'}
        </TableCell>
        <TableCell align="right">
          <IconButton
            size="small"
            color="primary"
            onClick={() => handleOpenDialog(storage)}
            title="Editar"
          >
            <Edit />
          </IconButton>
          <IconButton
            size="small"
            color="error"
            onClick={() => handleDelete(storage.id)}
            title="Eliminar"
          >
            <Delete />
          </IconButton>
        </TableCell>
      </TableRow>
    );
  })}
</TableBody>
        </Table>
      </TableContainer>

      <Dialog open={openDialog} onClose={handleCloseDialog} maxWidth="md" fullWidth>
        <DialogTitle>
          {editingStorage ? 'Editar Cloud Storage' : 'Nuevo Cloud Storage'}
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
              label="Proveedor"
              value={formData.proveedor}
              onChange={(e) => setFormData({ ...formData, proveedor: e.target.value })}
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
              label="Tipo de Almacenamiento"
              value={formData.tipoAlmacenamiento}
              onChange={(e) => setFormData({ ...formData, tipoAlmacenamiento: e.target.value })}
              fullWidth
              required
            />
            <TextField
              label="Capacidad (TB)"
              type="number"
              value={formData.capacidadTB}
              onChange={(e) =>
                setFormData({ ...formData, capacidadTB: parseFloat(e.target.value) })
              }
              fullWidth
              required
            />
            <TextField
              label="Credenciales"
              value={formData.credenciales}
              onChange={(e) => setFormData({ ...formData, credenciales: e.target.value })}
              fullWidth
              required
              multiline
              rows={2}
            />
            <TextField
              label="Configuración"
              value={formData.configuracion}
              onChange={(e) => setFormData({ ...formData, configuracion: e.target.value })}
              fullWidth
              required
              multiline
              rows={3}
            />
          </Box>
        </DialogContent>
        <DialogActions>
          <Button onClick={handleCloseDialog}>Cancelar</Button>
          <Button onClick={handleSubmit} variant="contained">
            {editingStorage ? 'Actualizar' : 'Crear'}
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
}

