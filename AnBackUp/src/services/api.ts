import axios from 'axios';
import type {
  DashboardDTO,
  CrearBackupDTO,
  ObtenerJobBackupDTO,
  SaveJobBackupDTO,
  UpdateJobBackupDTO,
  ObtenerAlertaDTO,
  SaveAlertaDTO,
  UpdateAlertaDTO,
  ObtenerCloudStorageDTO,
  SaveCloudStorageDTO,
  UpdateCloudStorageDTO,
  ObtenerRecuperacionDTO,
  SaveRecuperacionDTO,
  ObtenerPoliticaBackupDTO,
  SavePoliticaBackupDTO,
  UpdatePoliticaBackupDTO,
  ObtenerVerificacionIntegridadDTO,
  SaveVerificacionIntegridadDTO,
  MetricasDTO,
  ProveedorStorageDTO,
  BackupRecienteDTO,
  OperationResult,
} from '../types';

const api = axios.create({
  baseURL: '/api',
  headers: {
    'Content-Type': 'application/json',
  },
});

// Interceptor para manejar errores
api.interceptors.response.use(
  (response) => response,
  (error) => {
    console.error('API Error:', error.response?.data || error.message);
    return Promise.reject(error);
  }
);

// Helper function to convert PascalCase to camelCase
const toCamelCase = (obj: any): any => {
  if (obj === null || obj === undefined) return obj;
  if (Array.isArray(obj)) {
    return obj.map(item => toCamelCase(item));
  }
  if (typeof obj === 'object') {
    const camelObj: any = {};
    for (const key in obj) {
      const camelKey = key.charAt(0).toLowerCase() + key.slice(1);
      camelObj[camelKey] = toCamelCase(obj[key]);
    }
    return camelObj;
  }
  return obj;
};

// Dashboard API
export const dashboardApi = {
  obtenerDashboard: async (): Promise<DashboardDTO> => {
    const response = await api.get('/dashboard');
    return toCamelCase(response.data);
  },
  obtenerMetricas: async (): Promise<MetricasDTO> => {
    const response = await api.get('/dashboard/metricas');
    return toCamelCase(response.data);
  },
  obtenerProveedores: async (): Promise<ProveedorStorageDTO[]> => {
    const response = await api.get('/dashboard/proveedores');
    return toCamelCase(response.data);
  },
  obtenerBackupsRecientes: async (): Promise<BackupRecienteDTO[]> => {
    const response = await api.get('/dashboard/backups-recientes');
    return toCamelCase(response.data);
  },
  crearBackup: async (backup: CrearBackupDTO): Promise<void> => {
    await api.post('/dashboard/crear-backup', backup);
  },
};

// Job Backup API
export const jobBackupApi = {
  obtenerTodos: async (): Promise<ObtenerJobBackupDTO[]> => {
    const response = await api.get('/jobbackup');
    return toCamelCase(response.data);
  },
  obtenerPorId: async (id: number): Promise<ObtenerJobBackupDTO> => {
    const response = await api.get(`/jobbackup/${id}`);
    return toCamelCase(response.data);
  },
  obtenerProgramados: async (): Promise<ObtenerJobBackupDTO[]> => {
    const response = await api.get('/jobbackup/programados');
    return toCamelCase(response.data);
  },
  crear: async (job: SaveJobBackupDTO): Promise<number> => {
    const response = await api.post<number>('/jobbackup', job);
    return response.data;
  },
  actualizar: async (job: UpdateJobBackupDTO): Promise<void> => {
    await api.put('/jobbackup', job);
  },
  eliminar: async (id: number): Promise<void> => {
    await api.delete(`/jobbackup/${id}`);
  },
  ejecutar: async (id: number): Promise<void> => {
    await api.post(`/jobbackup/${id}/ejecutar`);
  },
};

// Alerta API
export const alertaApi = {
  obtenerTodos: async (): Promise<ObtenerAlertaDTO[]> => {
    const response = await api.get('/alerta');
    return toCamelCase(response.data);
  },
  obtenerPorId: async (id: number): Promise<ObtenerAlertaDTO> => {
    const response = await api.get(`/alerta/${id}`);
    return toCamelCase(response.data);
  },
  obtenerPorUsuario: async (usuarioId: number): Promise<ObtenerAlertaDTO[]> => {
    const response = await api.get(`/alerta/usuario/${usuarioId}`);
    return toCamelCase(response.data);
  },
  obtenerNoReconocidas: async (): Promise<ObtenerAlertaDTO[]> => {
    const response = await api.get('/alerta/no-reconocidas');
    return toCamelCase(response.data);
  },
  obtenerPorJob: async (jobId: number): Promise<ObtenerAlertaDTO[]> => {
    const response = await api.get(`/alerta/job/${jobId}`);
    return toCamelCase(response.data);
  },
  obtenerPorSeveridad: async (severidad: string): Promise<ObtenerAlertaDTO[]> => {
    const response = await api.get(`/alerta/severidad/${severidad}`);
    return toCamelCase(response.data);
  },
  crear: async (alerta: SaveAlertaDTO): Promise<ObtenerAlertaDTO> => {
    const response = await api.post('/alerta', alerta);
    return toCamelCase(response.data);
  },
  actualizar: async (id: number, alerta: UpdateAlertaDTO): Promise<ObtenerAlertaDTO> => {
    const response = await api.put(`/alerta/${id}`, alerta);
    return toCamelCase(response.data);
  },
  eliminar: async (id: number): Promise<void> => {
    await api.delete(`/alerta/${id}`);
  },
  marcarComoReconocida: async (id: number): Promise<void> => {
    await api.patch(`/alerta/${id}/reconocer`);
  },
  contarNoReconocidas: async (): Promise<number> => {
    const response = await api.get<number>('/alerta/contador/no-reconocidas');
    return response.data;
  },
};

// Cloud Storage API
export const cloudStorageApi = {
  obtenerTodos: async (): Promise<ObtenerCloudStorageDTO[]> => {
    const response = await api.get('/cloudstorage');
    return toCamelCase(response.data);
  },
  obtenerPorId: async (id: number): Promise<ObtenerCloudStorageDTO> => {
    const response = await api.get(`/cloudstorage/${id}`);
    return toCamelCase(response.data);
  },
  obtenerPorOrganizacion: async (organizacionId: number): Promise<ObtenerCloudStorageDTO[]> => {
    const response = await api.get(`/cloudstorage/organizacion/${organizacionId}`);
    return toCamelCase(response.data);
  },
  crear: async (storage: SaveCloudStorageDTO): Promise<ObtenerCloudStorageDTO> => {
    const response = await api.post('/cloudstorage', storage);
    return toCamelCase(response.data);
  },
  actualizar: async (id: number, storage: UpdateCloudStorageDTO): Promise<ObtenerCloudStorageDTO> => {
    const response = await api.put(`/cloudstorage/${id}`, storage);
    return toCamelCase(response.data);
  },
  eliminar: async (id: number): Promise<void> => {
    await api.delete(`/cloudstorage/${id}`);
  },
};

// Recuperacion API
export const recuperacionApi = {
  obtenerTodos: async (): Promise<OperationResult<ObtenerRecuperacionDTO[]>> => {
    const response = await api.get('/recuperacion');
    const result = toCamelCase(response.data);
    if (result.data) {
      result.data = toCamelCase(result.data);
    }
    return result;
  },
  obtenerPorId: async (id: number): Promise<OperationResult<ObtenerRecuperacionDTO>> => {
    const response = await api.get(`/recuperacion/${id}`);
    const result = toCamelCase(response.data);
    if (result.data) {
      result.data = toCamelCase(result.data);
    }
    return result;
  },
  obtenerPorUsuario: async (usuarioId: number): Promise<OperationResult<ObtenerRecuperacionDTO[]>> => {
    const response = await api.get(`/recuperacion/usuario/${usuarioId}`);
    const result = toCamelCase(response.data);
    if (result.data) {
      result.data = toCamelCase(result.data);
    }
    return result;
  },
  obtenerPorJob: async (jobId: number): Promise<OperationResult<ObtenerRecuperacionDTO[]>> => {
    const response = await api.get(`/recuperacion/job/${jobId}`);
    const result = toCamelCase(response.data);
    if (result.data) {
      result.data = toCamelCase(result.data);
    }
    return result;
  },
  obtenerPorEstado: async (estado: string): Promise<OperationResult<ObtenerRecuperacionDTO[]>> => {
    const response = await api.get(`/recuperacion/estado/${estado}`);
    const result = toCamelCase(response.data);
    if (result.data) {
      result.data = toCamelCase(result.data);
    }
    return result;
  },
  crear: async (recuperacion: SaveRecuperacionDTO): Promise<OperationResult<number>> => {
    const response = await api.post('/recuperacion', recuperacion);
    return toCamelCase(response.data);
  },
  actualizar: async (id: number, recuperacion: Partial<SaveRecuperacionDTO>): Promise<OperationResult> => {
    const response = await api.put(`/recuperacion/${id}`, recuperacion);
    return toCamelCase(response.data);
  },
  eliminar: async (id: number): Promise<OperationResult> => {
    const response = await api.delete(`/recuperacion/${id}`);
    return toCamelCase(response.data);
  },
  ejecutar: async (id: number): Promise<OperationResult> => {
    const response = await api.post(`/recuperacion/${id}/ejecutar`);
    return toCamelCase(response.data);
  },
  simular: async (id: number): Promise<OperationResult> => {
    const response = await api.post(`/recuperacion/${id}/simular`);
    return toCamelCase(response.data);
  },
};

// Politica Backup API
export const politicaBackupApi = {
  obtenerTodos: async (): Promise<OperationResult<ObtenerPoliticaBackupDTO[]>> => {
    const response = await api.get('/politicabackup');
    const result = toCamelCase(response.data);
    if (result.data) {
      result.data = toCamelCase(result.data);
    }
    return result;
  },
  obtenerPorId: async (id: number): Promise<OperationResult<ObtenerPoliticaBackupDTO>> => {
    const response = await api.get(`/politicabackup/${id}`);
    const result = toCamelCase(response.data);
    if (result.data) {
      result.data = toCamelCase(result.data);
    }
    return result;
  },
  obtenerPorOrganizacion: async (organizacionId: number): Promise<OperationResult<ObtenerPoliticaBackupDTO[]>> => {
    const response = await api.get(`/politicabackup/organizacion/${organizacionId}`);
    const result = toCamelCase(response.data);
    if (result.data) {
      result.data = toCamelCase(result.data);
    }
    return result;
  },
  obtenerActivas: async (): Promise<OperationResult<ObtenerPoliticaBackupDTO[]>> => {
    const response = await api.get('/politicabackup/activas');
    const result = toCamelCase(response.data);
    if (result.data) {
      result.data = toCamelCase(result.data);
    }
    return result;
  },
  obtenerPorTipo: async (tipoBackup: string): Promise<OperationResult<ObtenerPoliticaBackupDTO[]>> => {
    const response = await api.get(`/politicabackup/tipo/${tipoBackup}`);
    const result = toCamelCase(response.data);
    if (result.data) {
      result.data = toCamelCase(result.data);
    }
    return result;
  },
  crear: async (politica: SavePoliticaBackupDTO): Promise<OperationResult<number>> => {
    const response = await api.post('/politicabackup', politica);
    return toCamelCase(response.data);
  },
  actualizar: async (politica: UpdatePoliticaBackupDTO): Promise<OperationResult> => {
    const response = await api.put('/politicabackup', politica);
    return toCamelCase(response.data);
  },
  eliminar: async (id: number): Promise<OperationResult> => {
    const response = await api.delete(`/politicabackup/${id}`);
    return toCamelCase(response.data);
  },
  cambiarEstado: async (id: number, activo: boolean): Promise<OperationResult> => {
    const response = await api.patch(`/politicabackup/${id}/estado`, activo);
    return toCamelCase(response.data);
  },
};

// Verificacion Integridad API
export const verificacionIntegridadApi = {
  obtenerTodos: async (): Promise<OperationResult<ObtenerVerificacionIntegridadDTO[]>> => {
    const response = await api.get('/verificacionintegridad');
    const result = toCamelCase(response.data);
    if (result.data) {
      result.data = toCamelCase(result.data);
    }
    return result;
  },
  obtenerPorId: async (id: number): Promise<OperationResult<ObtenerVerificacionIntegridadDTO>> => {
    const response = await api.get(`/verificacionintegridad/${id}`);
    const result = toCamelCase(response.data);
    if (result.data) {
      result.data = toCamelCase(result.data);
    }
    return result;
  },
  obtenerPorJob: async (jobId: number): Promise<OperationResult<ObtenerVerificacionIntegridadDTO[]>> => {
    const response = await api.get(`/verificacionintegridad/job/${jobId}`);
    const result = toCamelCase(response.data);
    if (result.data) {
      result.data = toCamelCase(result.data);
    }
    return result;
  },
  obtenerUltimaPorJob: async (jobId: number): Promise<ObtenerVerificacionIntegridadDTO> => {
    const response = await api.get(`/verificacionintegridad/job/${jobId}/ultima`);
    return toCamelCase(response.data);
  },
  crear: async (verificacion: SaveVerificacionIntegridadDTO): Promise<OperationResult<number>> => {
    const response = await api.post('/verificacionintegridad', verificacion);
    return toCamelCase(response.data);
  },
  verificarIntegridad: async (jobId: number): Promise<OperationResult> => {
    const response = await api.post(`/verificacionintegridad/job/${jobId}/verificar`);
    return toCamelCase(response.data);
  },
  eliminar: async (id: number): Promise<OperationResult> => {
    const response = await api.delete(`/verificacionintegridad/${id}`);
    return toCamelCase(response.data);
  },
};

// Backup API
export const backupApi = {
  restaurar: async (id: number, destino: string): Promise<OperationResult> => {
    const response = await api.post(`/backup/${id}/restaurar`, destino);
    return toCamelCase(response.data);
  },
  verificarIntegridad: async (id: number): Promise<OperationResult> => {
    const response = await api.post(`/backup/${id}/verificar-integridad`);
    return toCamelCase(response.data);
  },
};

export default api;

