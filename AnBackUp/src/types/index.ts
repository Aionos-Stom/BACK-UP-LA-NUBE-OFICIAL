// Dashboard Types
export interface MetricasDTO {
  totalAlmacenadoTB: number;
  backupsHoy: number;
  tasaExitoPorcentaje: number;
  ultimaActualizacion: string;
  incrementoAlmacenamiento: number;
  incrementoBackups: number;
  incrementoTasaExito: number;
}

export interface ProveedorStorageDTO {
  proveedor: string;
  usadoTB: number;
  totalTB: number;
  estado: string;
}

export interface BackupRecienteDTO {
  nombre: string;
  tamanioGB: number;
  proveedor: string;
  estado: string;
  horaEjecucion: string | null;
}

export interface DashboardDTO {
  metricas: MetricasDTO;
  proveedores: ProveedorStorageDTO[];
  backupsRecientes: BackupRecienteDTO[];
}

export interface CrearBackupDTO {
  politicaId: number;
  cloudStorageId: number;
  fechaProgramada?: string;
  sourceData?: string;
}

// Job Backup Types
export interface ObtenerJobBackupDTO {
  id: number;
  politicaId: number;
  cloudStorageId: number;
  estado: string;
  fechaProgramada: string | null;
  fechaEjecucion: string | null;
  fechaCompletado: string | null;
  tamanoBytes: number | null;
  duracionSegundos: number | null;
  sourceData: string | null;
  errorMessage: string | null;
  fechaCreacion: string;
}

export interface SaveJobBackupDTO {
  politicaId: number;
  cloudStorageId: number;
  fechaProgramada?: string;
  sourceData?: string;
}

export interface UpdateJobBackupDTO {
  id: number;
  politicaId?: number;
  cloudStorageId?: number;
  fechaProgramada?: string;
  sourceData?: string;
}

// Alerta Types
export interface ObtenerAlertaDTO {
  id: number;
  usuarioId: number;
  usuarioNombre: string;
  jobId: number | null;
  jobNombre: string | null;
  tipo: string;
  severidad: string;
  mensaje: string;
  isAcknowledged: boolean;
  createdAt: string;
}

export interface SaveAlertaDTO {
  usuarioId: number;
  jobId?: number | null;
  tipo: string;
  severidad: string;
  mensaje: string;
  isAcknowledged?: boolean;
}

export interface UpdateAlertaDTO {
  id: number;
  jobId?: number | null;
  tipo?: string;
  severidad?: string;
  mensaje?: string;
  isAcknowledged?: boolean;
}

// Cloud Storage Types
export interface ObtenerCloudStorageDTO {
  id: number;
  organizacionId: number;
  proveedor: string;
  nombre: string;
  tipoAlmacenamiento: string;
  capacidadTB: number;
  usadoTB: number;
  credenciales: string;
  configuracion: string;
  estado: string;
  fechaCreacion: string;
}

export interface SaveCloudStorageDTO {
  organizacionId: number;
  proveedor: string;
  nombre: string;
  tipoAlmacenamiento: string;
  capacidadTB: number;
  credenciales: string;
  configuracion: string;
}

export interface UpdateCloudStorageDTO {
  id: number;
  organizacionId?: number;
  proveedor?: string;
  nombre?: string;
  tipoAlmacenamiento?: string;
  capacidadTB?: number;
  credenciales?: string;
  configuracion?: string;
  estado?: string;
}

// Recuperacion Types
export interface ObtenerRecuperacionDTO {
  id: number;
  jobId: number;
  usuarioId: number;
  destino: string;
  estado: string;
  fechaInicio: string | null;
  fechaCompletado: string | null;
  porcentajeCompletado: number;
  errorMessage: string | null;
  fechaCreacion: string;
}

export interface SaveRecuperacionDTO {
  jobId: number;
  usuarioId: number;
  destino: string;
}

// Politica Backup Types
export interface ObtenerPoliticaBackupDTO {
  id: number;
  organizacionId: number;
  nombre: string;
  tipoBackup: string;
  frecuencia: string;
  horaEjecucion: string;
  retencionDias: number;
  compresion: boolean;
  encriptacion: boolean;
  activo: boolean;
  fechaCreacion: string;
}

export interface SavePoliticaBackupDTO {
  organizacionId: number;
  nombre: string;
  tipoBackup: string;
  frecuencia: string;
  horaEjecucion: string;
  retencionDias: number;
  compresion: boolean;
  encriptacion: boolean;
  activo?: boolean;
}

export interface UpdatePoliticaBackupDTO {
  id: number;
  organizacionId?: number;
  nombre?: string;
  tipoBackup?: string;
  frecuencia?: string;
  horaEjecucion?: string;
  retencionDias?: number;
  compresion?: boolean;
  encriptacion?: boolean;
  activo?: boolean;
}

// Verificacion Integridad Types
export interface ObtenerVerificacionIntegridadDTO {
  id: number;
  jobId: number;
  resultado: string;
  hashOriginal: string | null;
  hashVerificado: string | null;
  integridadOk: boolean;
  fechaVerificacion: string;
  observaciones: string | null;
}

export interface SaveVerificacionIntegridadDTO {
  jobId: number;
  hashOriginal?: string;
  observaciones?: string;
}

// Operation Result
export interface OperationResult<T = any> {
  isSuccess: boolean;
  message: string;
  data?: T;
}

