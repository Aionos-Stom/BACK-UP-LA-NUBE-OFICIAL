CREATE database n
use n
-- Tabla ORGANIZACION
CREATE TABLE ORGANIZACION (
    id INT PRIMARY KEY IDENTITY(1,1),
    nombre VARCHAR(255) NOT NULL,
    configuracion NVARCHAR(MAX),
    licencia_valida_hasta DATE,
    max_usuarios INT,
    fecha_creacion DATETIME DEFAULT CURRENT_TIMESTAMP,
    activo BIT DEFAULT 1
);

-- Tabla CLOUD_STORAGE
CREATE TABLE CLOUD_STORAGE (
    id INT PRIMARY KEY IDENTITY(1,1),
    organizacion_id INT NOT NULL,
    proveedor VARCHAR(20) NOT NULL CHECK (proveedor IN ('aws', 'azure', 'gcp')),
    configuration NVARCHAR(MAX),
    endpoint_url TEXT,
    tier_actual VARCHAR(20) not null CHECK (tier_actual IN ('frecuente', 'infrecuente', 'archivo')),
    costo_mensual DECIMAL(10,2) DEFAULT 0.00,
    is_active Bit DEFAULT 1,
    fecha_creacion DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (organizacion_id) REFERENCES ORGANIZACION(id) ON DELETE CASCADE
);

-- Tabla POLITICA_BACKUP
CREATE TABLE POLITICA_BACKUP (
    id INT PRIMARY KEY IDENTITY(1,1),
    organizacion_id INT NOT NULL,
    nombre VARCHAR(255) NOT NULL,
    frecuencia VARCHAR(20) NOT NULL CHECK (frecuencia IN ('horaria', 'diaria', 'semanal', 'mensual')),
    tipo_backup VARCHAR(20)NOT NULL CHECK (tipo_backup IN ('completo', 'incremental', 'diferencial')),
    retencion_dias INT not null,
    rpo_minutes INT not null,
    rto_minutes INT not null,
    ventana_ejecucion NVARCHAR(MAX),
    is_active Bit DEFAULT 1,
    fecha_creacion DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (organizacion_id) REFERENCES ORGANIZACION(id) ON DELETE CASCADE
);

-- Tabla USUARIO
CREATE TABLE USUARIO (
    id INT PRIMARY KEY IDENTITY(1,1),
    organizacion_id INT NOT NULL,
    email VARCHAR(255) UNIQUE NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    nombre VARCHAR(255) NOT NULL,
    rol VARCHAR(20) NOT NULL CHECK (rol IN ('admin', 'usuario', 'auditor')) DEFAULT 'usuario',
    mfa_habilitado Bit DEFAULT 0,
    mfa_secret VARCHAR(255),
    last_login DATETIME,
    is_active bit DEFAULT 1,
    fecha_creacion DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (organizacion_id) REFERENCES ORGANIZACION(id) ON DELETE CASCADE
);

-- Tabla JOB_BACKUP
CREATE TABLE JOB_BACKUP (
    id INT PRIMARY KEY IDENTITY(1,1),
    politica_id INT NOT NULL,
    cloud_storage_id INT NOT NULL,
    estado VARCHAR(20) NOT NULL CHECK (estado IN ('programado', 'ejecutando', 'completado', 'fallado')) DEFAULT 'PROGRAMADO',
    fecha_programada DATETIME,
    fecha_ejecucion DATETIME,
    fecha_completado DATETIME,
    tamano_bytes BIGINT,
    duracion_segundos INT,
    source_data TEXT,
    error_message TEXT,
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (politica_id) REFERENCES POLITICA_BACKUP(id) ON DELETE CASCADE,
    FOREIGN KEY (cloud_storage_id) REFERENCES CLOUD_STORAGE(id) ON DELETE CASCADE
);

-- Tabla RECUPERACION
CREATE TABLE RECUPERACION (
    id INT PRIMARY KEY IDENTITY(1,1),
    usuario_id INT NOT NULL,
    job_id INT NOT NULL,
    tipo_recuperacion VARCHAR(20),
    punto_tiempo DATETIME,
    input_path TEXT,
    estado VARCHAR(20),
    is_simulacion Bit DEFAULT 0,
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    completed_at DATETIME,
    FOREIGN KEY (usuario_id) REFERENCES USUARIO(id),
    FOREIGN KEY (job_id) REFERENCES JOB_BACKUP(id)
);

-- Tabla VERIFICACION_INTEGRIDAD
CREATE TABLE VERIFICACION_INTEGRIDAD (
    id INT PRIMARY KEY IDENTITY(1,1),
    job_id INT NOT NULL,
    checksum_sha256 VARCHAR(64),
    resultado Bit,
    fecha_verificacion DATETIME DEFAULT CURRENT_TIMESTAMP,
    detalles NVARCHAR(MAX),
    integrity_score DECIMAL(5,2),
    FOREIGN KEY (job_id) REFERENCES JOB_BACKUP(id) ON DELETE CASCADE
);

-- Tabla ALERTA
CREATE TABLE ALERTA (
    id INT PRIMARY KEY IDENTITY(1,1),
    usuario_id INT NOT NULL,
    job_id INT,
    tipo VARCHAR(50),
    severidad VARCHAR(20),
    mensaje TEXT,
    is_acknowledged Bit DEFAULT 0,
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (usuario_id) REFERENCES USUARIO(id),
    FOREIGN KEY (job_id) REFERENCES JOB_BACKUP(id)
);

-- Tabla SESION
CREATE TABLE SESION (
    id INT PRIMARY KEY IDENTITY(1,1),
    usuario_id INT NOT NULL,
    token TEXT NOT NULL,
    expires_at DATETIME NOT NULL,
    ip_address VARCHAR(45),
    user_agent TEXT,
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (usuario_id) REFERENCES USUARIO(id)
);


CREATE INDEX idx_job_backup_estado ON JOB_BACKUP(estado);
CREATE INDEX idx_job_backup_fecha_programada ON JOB_BACKUP(fecha_programada);
CREATE INDEX idx_usuario_email ON USUARIO(email);
CREATE INDEX idx_alerta_severidad ON ALERTA(severidad);
CREATE INDEX idx_sesion_expires ON SESION(expires_at);

INSERT INTO ORGANIZACION (nombre, configuracion, licencia_valida_hasta, max_usuarios)
VALUES
('DataSecure Corp', '{"pais": "RD", "industria": "Finanzas"}', '2026-12-31', 100),
('SafeCloud Solutions', '{"pais": "USA", "industria": "Tecnología"}', '2026-06-15', 250),
('MultiBackup Inc', '{"pais": "México", "industria": "Salud"}', '2025-12-31', 50),
('CloudShield', '{"pais": "Chile", "industria": "Educación"}', '2026-08-20', 75),
('RecoveryOne', '{"pais": "Colombia", "industria": "Retail"}', '2027-01-10', 150);


INSERT INTO CLOUD_STORAGE (organizacion_id, proveedor, configuration, endpoint_url, tier_actual, costo_mensual)
VALUES
(1, 'aws', '{"region": "us-east-1"}', 'https://s3.amazonaws.com/datasecure', 'frecuente', 120.50),
(1, 'azure', '{"region": "eastus"}', 'https://azure.blob.core.windows.net/datasecure', 'infrecuente', 80.75),
(2, 'gcp', '{"region": "us-central1"}', 'https://storage.googleapis.com/safecloud', 'frecuente', 150.00),
(3, 'aws', '{"region": "sa-east-1"}', 'https://s3.amazonaws.com/multibackup', 'archivo', 60.20),
(4, 'azure', '{"region": "southamerica"}', 'https://azure.blob.core.windows.net/cloudshield', 'frecuente', 95.90);

INSERT INTO POLITICA_BACKUP (organizacion_id, nombre, frecuencia, tipo_backup, retencion_dias, rpo_minutes, rto_minutes, ventana_ejecucion)
VALUES
(1, 'Backup Diario Principal', 'diaria', 'completo', 30, 15, 60, '{"inicio": "00:00", "fin": "02:00"}'),
(1, 'Backup Incremental Horario', 'horaria', 'incremental', 7, 5, 30, '{"inicio": "cada_hora"}'),
(2, 'Backup Semanal General', 'semanal', 'completo', 90, 30, 120, '{"dia": "domingo", "hora": "01:00"}'),
(3, 'Backup Mensual Archivo', 'mensual', 'diferencial', 180, 60, 240, '{"dia": "1", "hora": "03:00"}'),
(4, 'Backup de Base de Datos', 'diaria', 'incremental', 15, 10, 45, '{"inicio": "23:00", "fin": "23:30"}');

INSERT INTO USUARIO (organizacion_id, email, password_hash, nombre, rol, mfa_habilitado)
VALUES
(1, 'admin@datasecure.com', 'hash123', 'Carlos Méndez', 'admin', 1),
(1, 'user1@datasecure.com', 'hash456', 'Ana López', 'usuario', 0),
(2, 'admin@safecloud.com', 'hash789', 'María Torres', 'admin', 1),
(3, 'auditor@multibackup.com', 'hash321', 'Luis Pérez', 'auditor', 0),
(4, 'user@cloudshield.com', 'hash654', 'Laura Díaz', 'usuario', 0);

INSERT INTO JOB_BACKUP (politica_id, cloud_storage_id, estado, fecha_programada, fecha_ejecucion, fecha_completado, tamano_bytes, duracion_segundos, source_data)
VALUES
(1, 1, 'completado', '2025-10-10 00:00', '2025-10-10 00:05', '2025-10-10 00:45', 5000000000, 2400, 'C:\\data\\finance'),
(1, 2, 'fallado', '2025-10-11 00:00', '2025-10-11 00:05', NULL, 0, 120, 'C:\\data\\finance'),
(2, 3, 'completado', '2025-10-09 01:00', '2025-10-09 01:10', '2025-10-09 01:40', 8000000000, 1800, 'D:\\backups\\users'),
(3, 4, 'ejecutando', '2025-10-12 02:00', '2025-10-12 02:10', NULL, 20000000000, 300, 'E:\\server\\sql'),
(4, 5, 'programado', '2025-10-15 03:00', NULL, NULL, 0, NULL, 'F:\\backup\\db');

INSERT INTO RECUPERACION (usuario_id, job_id, tipo_recuperacion, punto_tiempo, input_path, estado, is_simulacion)
VALUES
(1, 1, 'completa', '2025-10-10 01:00', 'C:\\restore\\finance', 'completado', 0),
(2, 2, 'granular', '2025-10-11 01:30', 'C:\\restore\\partial', 'fallado', 0),
(3, 3, 'punto_tiempo', '2025-10-09 02:00', 'D:\\restore\\userdb', 'completado', 0),
(4, 4, 'simulacion', '2025-10-12 02:15', 'E:\\restore\\sim', 'en_progreso', 1),
(5, 5, 'completa', '2025-10-13 03:00', 'F:\\restore\\db', 'pendiente', 0);

INSERT INTO VERIFICACION_INTEGRIDAD (job_id, checksum_sha256, resultado, detalles, integrity_score)
VALUES
(1, 'a1b2c3d4e5f6', 1, '{"bloques_corruptos":0}', 100.0),
(2, 'f7g8h9i0j1k2', 0, '{"bloques_corruptos":3}', 75.5),
(3, 'l3m4n5o6p7q8', 1, '{"bloques_corruptos":0}', 99.9),
(4, 'r9s0t1u2v3w4', 1, '{"bloques_corruptos":1}', 97.5),
(5, 'x5y6z7a8b9c0', 1, '{"bloques_corruptos":0}', 100.0);
INSERT INTO VERIFICACION_INTEGRIDAD (job_id, checksum_sha256, resultado, detalles, integrity_score)
VALUES
(1, 'a1b2c3d4e5f6', 1, '{"bloques_corruptos":0}', 100.0),
(2, 'f7g8h9i0j1k2', 0, '{"bloques_corruptos":3}', 75.5),
(3, 'l3m4n5o6p7q8', 1, '{"bloques_corruptos":0}', 99.9),
(4, 'r9s0t1u2v3w4', 1, '{"bloques_corruptos":1}', 97.5),
(5, 'x5y6z7a8b9c0', 1, '{"bloques_corruptos":0}', 100.0);
INSERT INTO ALERTA (usuario_id, job_id, tipo, severidad, mensaje)
VALUES
(1, 2, 'Fallo de Backup', 'crítica', 'El backup programado ha fallado.'),
(2, 3, 'Verificación', 'media', 'Checksum con desviación detectada.'),
(3, 4, 'Recuperación', 'baja', 'Simulación de recuperación iniciada.'),
(4, 5, 'Backup Pendiente', 'media', 'Backup programado aún no se ejecuta.'),
(5, 1, 'Integridad', 'alta', 'Verificación completada con éxito.');


INSERT INTO SESION (usuario_id, token, expires_at, ip_address, user_agent)
VALUES
(1, 'token123', '2025-10-13 12:00', '192.168.1.10', 'Mozilla/5.0'),
(2, 'token456', '2025-10-13 13:00', '192.168.1.11', 'Chrome/120.0'),
(3, 'token789', '2025-10-13 14:00', '192.168.1.12', 'Edge/121.0'),
(4, 'token321', '2025-10-13 15:00', '192.168.1.13', 'Safari/17.0'),
(5, 'token654', '2025-10-13 16:00', '192.168.1.14', 'Opera/105.0');

