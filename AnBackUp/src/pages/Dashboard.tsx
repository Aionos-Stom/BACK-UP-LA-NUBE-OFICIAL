import { useEffect, useState } from 'react';
import {
  Grid,
  Paper,
  Typography,
  Box,
  Card,
  CardContent,
  LinearProgress,
  Chip,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  IconButton,
  CircularProgress,
  Alert,
} from '@mui/material';
import NeonCard from '../components/NeonCard';
import {
  TrendingUp,
  TrendingDown,
  Storage,
  Backup,
  CheckCircle,
  Refresh,
} from '@mui/icons-material';
import { BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer, PieChart, Pie, Cell } from 'recharts';
import { dashboardApi } from '../services/api';
import type { DashboardDTO, MetricasDTO, ProveedorStorageDTO, BackupRecienteDTO } from '../types';
import { format } from 'date-fns';

const COLORS = ['#00d4ff', '#7c3aed', '#00ff88', '#ffb800', '#ff006e', '#5dffff', '#a78bfa'];

export default function Dashboard() {
  const [dashboard, setDashboard] = useState<DashboardDTO | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const loadDashboard = async () => {
    try {
      setLoading(true);
      setError(null);
      const data = await dashboardApi.obtenerDashboard();
      setDashboard(data);
    } catch (err: any) {
      const errorMessage = err.response?.status === 404 
        ? 'Backend no encontrado. Asegúrate de que el servidor esté corriendo en http://localhost:5271'
        : err.response?.data?.message || err.message || 'Error al cargar el dashboard';
      setError(errorMessage);
      console.error('Error loading dashboard:', err);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadDashboard();
    const interval = setInterval(loadDashboard, 30000); // Actualizar cada 30 segundos
    return () => clearInterval(interval);
  }, []);

  if (loading && !dashboard) {
    return (
      <Box display="flex" justifyContent="center" alignItems="center" minHeight="400px">
        <CircularProgress />
      </Box>
    );
  }

  if (error && !dashboard) {
    return (
      <Alert severity="error" action={
        <IconButton onClick={loadDashboard} size="small">
          <Refresh />
        </IconButton>
      }>
        {error}
      </Alert>
    );
  }

  if (!dashboard) return null;

  const { metricas, proveedores, backupsRecientes } = dashboard;

  const chartData = backupsRecientes.slice(0, 7).map((backup, index) => ({
    name: backup.nombre.length > 15 ? backup.nombre.substring(0, 15) + '...' : backup.nombre,
    tamanio: backup.tamanioGB,
    index,
  }));

  const pieData = proveedores.map((p) => ({
    name: p.proveedor,
    value: p.usadoTB,
  }));

  const MetricCard = ({ title, value, unit, trend, trendValue }: {
    title: string;
    value: number;
    unit: string;
    trend?: number;
    trendValue?: number;
  }) => (
    <NeonCard glow float>
      <CardContent sx={{ p: 0, '&:last-child': { pb: 0 } }}>
        <Typography 
          color="text.secondary" 
          gutterBottom 
          variant="body2"
          sx={{ 
            fontSize: '0.875rem',
            opacity: 0.8,
            mb: 1.5
          }}
        >
          {title}
        </Typography>
        <Typography 
          variant="h4" 
          component="div"
          sx={{
            background: 'linear-gradient(135deg, #00d4ff 0%, #7c3aed 100%)',
            WebkitBackgroundClip: 'text',
            WebkitTextFillColor: 'transparent',
            fontWeight: 700,
            mb: 1,
          }}
        >
          {value.toLocaleString('es-ES', { maximumFractionDigits: 2 })} {unit}
        </Typography>
        {trend !== undefined && trendValue !== undefined && (
          <Box display="flex" alignItems="center" mt={1.5}>
            {trend >= 0 ? (
              <TrendingUp 
                sx={{ 
                  color: '#00ff88',
                  filter: 'drop-shadow(0 0 8px rgba(0, 255, 136, 0.5))',
                }} 
                fontSize="small" 
              />
            ) : (
              <TrendingDown 
                sx={{ 
                  color: '#ff006e',
                  filter: 'drop-shadow(0 0 8px rgba(255, 0, 110, 0.5))',
                }} 
                fontSize="small" 
              />
            )}
            <Typography
              variant="body2"
              sx={{ 
                ml: 0.5,
                color: trend >= 0 ? '#00ff88' : '#ff006e',
                fontWeight: 600,
                textShadow: trend >= 0 
                  ? '0 0 10px rgba(0, 255, 136, 0.5)' 
                  : '0 0 10px rgba(255, 0, 110, 0.5)',
              }}
            >
              {trend >= 0 ? '+' : ''}{trendValue.toFixed(2)}%
            </Typography>
          </Box>
        )}
      </CardContent>
    </NeonCard>
  );

  return (
    <Box>
      <Box 
        display="flex" 
        justifyContent="space-between" 
        alignItems="center" 
        mb={4}
        sx={{ animation: 'slideIn 0.5s ease-out' }}
      >
        <Typography 
          variant="h4" 
          component="h1" 
          sx={{
            background: 'linear-gradient(135deg, #00d4ff 0%, #7c3aed 100%)',
            WebkitBackgroundClip: 'text',
            WebkitTextFillColor: 'transparent',
            fontWeight: 700,
            fontSize: '2.5rem',
            textShadow: '0 0 30px rgba(0, 212, 255, 0.3)',
          }}
        >
          Dashboard
        </Typography>
        <IconButton 
          onClick={loadDashboard} 
          disabled={loading}
          sx={{
            background: 'linear-gradient(135deg, rgba(0, 212, 255, 0.2) 0%, rgba(124, 58, 237, 0.2) 100%)',
            border: '1px solid rgba(0, 212, 255, 0.3)',
            color: '#00d4ff',
            '&:hover': {
              background: 'linear-gradient(135deg, rgba(0, 212, 255, 0.3) 0%, rgba(124, 58, 237, 0.3) 100%)',
              boxShadow: '0 0 20px rgba(0, 212, 255, 0.4)',
              transform: 'rotate(180deg)',
            },
            transition: 'all 0.3s ease',
          }}
        >
          <Refresh />
        </IconButton>
      </Box>

      <Grid container spacing={3}>
        {/* Métricas principales */}
        <Grid item xs={12} sm={6} md={3}>
          <MetricCard
            title="Almacenamiento Total"
            value={metricas.totalAlmacenadoTB}
            unit="TB"
            trend={metricas.incrementoAlmacenamiento}
            trendValue={metricas.incrementoAlmacenamiento}
          />
        </Grid>
        <Grid item xs={12} sm={6} md={3}>
          <MetricCard
            title="Backups Hoy"
            value={metricas.backupsHoy}
            unit="backups"
            trend={metricas.incrementoBackups}
            trendValue={metricas.incrementoBackups}
          />
        </Grid>
        <Grid item xs={12} sm={6} md={3}>
          <MetricCard
            title="Tasa de Éxito"
            value={metricas.tasaExitoPorcentaje}
            unit="%"
            trend={metricas.incrementoTasaExito}
            trendValue={metricas.incrementoTasaExito}
          />
        </Grid>
        <Grid item xs={12} sm={6} md={3}>
          <NeonCard>
            <CardContent sx={{ p: 0, '&:last-child': { pb: 0 } }}>
              <Typography 
                color="text.secondary" 
                gutterBottom 
                variant="body2"
                sx={{ 
                  fontSize: '0.875rem',
                  opacity: 0.8,
                  mb: 1.5
                }}
              >
                Última Actualización
              </Typography>
              <Typography 
                variant="body1"
                sx={{
                  color: '#b0d4ff',
                  fontWeight: 500,
                }}
              >
                {format(new Date(metricas.ultimaActualizacion), 'PPpp')}
              </Typography>
            </CardContent>
          </NeonCard>
        </Grid>

        {/* Gráfico de backups recientes */}
        <Grid item xs={12} md={8}>
          <NeonCard sx={{ p: 3, animation: 'slideIn 0.5s ease-out' }}>
            <Typography 
              variant="h6" 
              gutterBottom
              sx={{
                background: 'linear-gradient(135deg, #00d4ff 0%, #7c3aed 100%)',
                WebkitBackgroundClip: 'text',
                WebkitTextFillColor: 'transparent',
                fontWeight: 700,
                mb: 3,
              }}
            >
              Backups Recientes
            </Typography>
            <ResponsiveContainer width="100%" height={300}>
              <BarChart data={chartData}>
                <CartesianGrid strokeDasharray="3 3" stroke="rgba(0, 212, 255, 0.2)" />
                <XAxis 
                  dataKey="name" 
                  stroke="#b0d4ff"
                  style={{ fontSize: '12px' }}
                />
                <YAxis 
                  stroke="#b0d4ff"
                  style={{ fontSize: '12px' }}
                />
                <Tooltip 
                  contentStyle={{
                    background: 'rgba(15, 22, 41, 0.95)',
                    border: '1px solid rgba(0, 212, 255, 0.3)',
                    borderRadius: '8px',
                    color: '#ffffff',
                  }}
                />
                <Legend 
                  wrapperStyle={{ color: '#b0d4ff' }}
                />
                <Bar 
                  dataKey="tamanio" 
                  name="Tamaño (GB)"
                  radius={[8, 8, 0, 0]}
                >
                  {chartData.map((entry, index) => (
                    <Cell 
                      key={`cell-${index}`} 
                      fill={COLORS[index % COLORS.length]}
                    />
                  ))}
                </Bar>
              </BarChart>
            </ResponsiveContainer>
          </NeonCard>
        </Grid>

        {/* Distribución de proveedores */}
        <Grid item xs={12} md={4}>
          <NeonCard sx={{ p: 3, animation: 'slideIn 0.5s ease-out 0.1s both' }}>
            <Typography 
              variant="h6" 
              gutterBottom
              sx={{
                background: 'linear-gradient(135deg, #00d4ff 0%, #7c3aed 100%)',
                WebkitBackgroundClip: 'text',
                WebkitTextFillColor: 'transparent',
                fontWeight: 700,
                mb: 3,
              }}
            >
              Distribución por Proveedor
            </Typography>
            <ResponsiveContainer width="100%" height={300}>
              <PieChart>
                <Pie
                  data={pieData}
                  cx="50%"
                  cy="50%"
                  labelLine={false}
                  label={({ name, percent }) => `${name} ${(percent * 100).toFixed(0)}%`}
                  outerRadius={80}
                  fill="#8884d8"
                  dataKey="value"
                >
                  {pieData.map((entry, index) => (
                    <Cell key={`cell-${index}`} fill={COLORS[index % COLORS.length]} />
                  ))}
                </Pie>
                <Tooltip 
                  contentStyle={{
                    background: 'rgba(15, 22, 41, 0.95)',
                    border: '1px solid rgba(0, 212, 255, 0.3)',
                    borderRadius: '8px',
                    color: '#ffffff',
                  }}
                />
              </PieChart>
            </ResponsiveContainer>
          </NeonCard>
        </Grid>

        {/* Proveedores de Storage */}
        <Grid item xs={12} md={6}>
          <NeonCard sx={{ p: 3, animation: 'slideIn 0.5s ease-out 0.2s both' }}>
            <Typography 
              variant="h6" 
              gutterBottom
              sx={{
                background: 'linear-gradient(135deg, #00d4ff 0%, #7c3aed 100%)',
                WebkitBackgroundClip: 'text',
                WebkitTextFillColor: 'transparent',
                fontWeight: 700,
                mb: 3,
              }}
            >
              Proveedores de Storage
            </Typography>
            <TableContainer>
              <Table size="small">
                <TableHead>
                  <TableRow>
                    <TableCell sx={{ color: '#00d4ff', fontWeight: 600, borderColor: 'rgba(0, 212, 255, 0.2)' }}>Proveedor</TableCell>
                    <TableCell align="right" sx={{ color: '#00d4ff', fontWeight: 600, borderColor: 'rgba(0, 212, 255, 0.2)' }}>Usado</TableCell>
                    <TableCell align="right" sx={{ color: '#00d4ff', fontWeight: 600, borderColor: 'rgba(0, 212, 255, 0.2)' }}>Total</TableCell>
                    <TableCell align="right" sx={{ color: '#00d4ff', fontWeight: 600, borderColor: 'rgba(0, 212, 255, 0.2)' }}>%</TableCell>
                    <TableCell sx={{ color: '#00d4ff', fontWeight: 600, borderColor: 'rgba(0, 212, 255, 0.2)' }}>Estado</TableCell>
                  </TableRow>
                </TableHead>
                <TableBody>
                  {proveedores.map((proveedor, index) => {
                    const porcentaje = (proveedor.usadoTB / proveedor.totalTB) * 100;
                    return (
                      <TableRow 
                        key={index}
                        sx={{
                          '&:hover': {
                            backgroundColor: 'rgba(0, 212, 255, 0.05)',
                          },
                          '& td': {
                            borderColor: 'rgba(0, 212, 255, 0.1)',
                            color: '#b0d4ff',
                          },
                        }}
                      >
                        <TableCell>{proveedor.proveedor}</TableCell>
                        <TableCell align="right">{proveedor.usadoTB.toFixed(2)} TB</TableCell>
                        <TableCell align="right">{proveedor.totalTB.toFixed(2)} TB</TableCell>
                        <TableCell align="right">
                          <Box sx={{ width: '100%', mr: 1 }}>
                            <LinearProgress 
                              variant="determinate" 
                              value={porcentaje}
                              sx={{
                                height: 8,
                                borderRadius: 4,
                                backgroundColor: 'rgba(255, 255, 255, 0.1)',
                                '& .MuiLinearProgress-bar': {
                                  background: porcentaje > 90
                                    ? 'linear-gradient(90deg, #ff006e 0%, #ff4d9a 100%)'
                                    : porcentaje > 70
                                    ? 'linear-gradient(90deg, #ffb800 0%, #ffd54f 100%)'
                                    : 'linear-gradient(90deg, #00d4ff 0%, #7c3aed 100%)',
                                  boxShadow: porcentaje > 90
                                    ? '0 0 10px rgba(255, 0, 110, 0.5)'
                                    : porcentaje > 70
                                    ? '0 0 10px rgba(255, 184, 0, 0.5)'
                                    : '0 0 10px rgba(0, 212, 255, 0.5)',
                                },
                              }}
                            />
                          </Box>
                          <Typography variant="body2" color="textSecondary">
                            {porcentaje.toFixed(1)}%
                          </Typography>
                        </TableCell>
                      <TableCell>
                        <Chip
                          label={proveedor.estado}
                          sx={{
                            background: proveedor.estado === 'Activo' 
                              ? 'linear-gradient(135deg, rgba(0, 255, 136, 0.2) 0%, rgba(0, 255, 136, 0.1) 100%)'
                              : 'rgba(255, 255, 255, 0.1)',
                            color: proveedor.estado === 'Activo' ? '#00ff88' : '#b0d4ff',
                            border: proveedor.estado === 'Activo' 
                              ? '1px solid rgba(0, 255, 136, 0.3)'
                              : '1px solid rgba(255, 255, 255, 0.1)',
                            fontWeight: 600,
                            boxShadow: proveedor.estado === 'Activo' 
                              ? '0 0 10px rgba(0, 255, 136, 0.3)'
                              : 'none',
                          }}
                          size="small"
                        />
                      </TableCell>
                      </TableRow>
                    );
                  })}
                </TableBody>
              </Table>
            </TableContainer>
          </NeonCard>
        </Grid>

        {/* Tabla de backups recientes */}
        <Grid item xs={12} md={6}>
          <NeonCard sx={{ p: 3, animation: 'slideIn 0.5s ease-out 0.3s both' }}>
            <Typography 
              variant="h6" 
              gutterBottom
              sx={{
                background: 'linear-gradient(135deg, #00d4ff 0%, #7c3aed 100%)',
                WebkitBackgroundClip: 'text',
                WebkitTextFillColor: 'transparent',
                fontWeight: 700,
                mb: 3,
              }}
            >
              Últimos Backups
            </Typography>
            <TableContainer>
              <Table size="small">
                <TableHead>
                  <TableRow>
                    <TableCell sx={{ color: '#00d4ff', fontWeight: 600, borderColor: 'rgba(0, 212, 255, 0.2)' }}>Nombre</TableCell>
                    <TableCell align="right" sx={{ color: '#00d4ff', fontWeight: 600, borderColor: 'rgba(0, 212, 255, 0.2)' }}>Tamaño</TableCell>
                    <TableCell sx={{ color: '#00d4ff', fontWeight: 600, borderColor: 'rgba(0, 212, 255, 0.2)' }}>Proveedor</TableCell>
                    <TableCell sx={{ color: '#00d4ff', fontWeight: 600, borderColor: 'rgba(0, 212, 255, 0.2)' }}>Estado</TableCell>
                    <TableCell sx={{ color: '#00d4ff', fontWeight: 600, borderColor: 'rgba(0, 212, 255, 0.2)' }}>Fecha</TableCell>
                  </TableRow>
                </TableHead>
                <TableBody>
                  {backupsRecientes.slice(0, 5).map((backup, index) => (
                    <TableRow 
                      key={index}
                      sx={{
                        '&:hover': {
                          backgroundColor: 'rgba(0, 212, 255, 0.05)',
                        },
                        '& td': {
                          borderColor: 'rgba(0, 212, 255, 0.1)',
                          color: '#b0d4ff',
                        },
                      }}
                    >
                      <TableCell>{backup.nombre}</TableCell>
                      <TableCell align="right">{backup.tamanioGB.toFixed(2)} GB</TableCell>
                      <TableCell>{backup.proveedor}</TableCell>
                      <TableCell>
                        <Chip
                          label={backup.estado}
                          sx={{
                            background: 
                              backup.estado === 'Completado'
                                ? 'linear-gradient(135deg, rgba(0, 255, 136, 0.2) 0%, rgba(0, 255, 136, 0.1) 100%)'
                                : backup.estado === 'Error'
                                ? 'linear-gradient(135deg, rgba(255, 0, 110, 0.2) 0%, rgba(255, 0, 110, 0.1) 100%)'
                                : 'linear-gradient(135deg, rgba(255, 184, 0, 0.2) 0%, rgba(255, 184, 0, 0.1) 100%)',
                            color:
                              backup.estado === 'Completado'
                                ? '#00ff88'
                                : backup.estado === 'Error'
                                ? '#ff006e'
                                : '#ffb800',
                            border:
                              backup.estado === 'Completado'
                                ? '1px solid rgba(0, 255, 136, 0.3)'
                                : backup.estado === 'Error'
                                ? '1px solid rgba(255, 0, 110, 0.3)'
                                : '1px solid rgba(255, 184, 0, 0.3)',
                            fontWeight: 600,
                            boxShadow:
                              backup.estado === 'Completado'
                                ? '0 0 10px rgba(0, 255, 136, 0.3)'
                                : backup.estado === 'Error'
                                ? '0 0 10px rgba(255, 0, 110, 0.3)'
                                : '0 0 10px rgba(255, 184, 0, 0.3)',
                          }}
                          size="small"
                        />
                      </TableCell>
                      <TableCell>
                        {backup.horaEjecucion
                          ? format(new Date(backup.horaEjecucion), 'dd/MM/yyyy HH:mm')
                          : '-'}
                      </TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            </TableContainer>
          </NeonCard>
        </Grid>
      </Grid>
    </Box>
  );
}

