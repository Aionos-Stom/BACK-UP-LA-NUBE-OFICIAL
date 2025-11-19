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

CREATE TABLE USERS (
    id INT PRIMARY KEY IDENTITY(1,1),
    organizacion_id INT NOT NULL,
    email VARCHAR(255) UNIQUE NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    nombre VARCHAR(255) NOT NULL,
    rol VARCHAR(20) NOT NULL CHECK (rol IN ('admin', 'usuario', 'auditor')) DEFAULT 'usuario',
    mfa_habilitado Bit DEFAULT 0,
    mfa_secret VARCHAR(255),
    last_login DATETIME,
	PhoneNumber nvarchar(20), 
    is_active bit DEFAULT 1,
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (organizacion_id) REFERENCES ORGANIZACION(id) ON DELETE CASCADE
);

CREATE TABLE JOB_BACKUP (
    id INT PRIMARY KEY IDENTITY(1,1),
    politica_id INT NOT NULL,
    cloud_storage_id INT NOT NULL,
    estado VARCHAR(20) NOT NULL CHECK (estado IN ('programado', 'ejecutando', 'completado', 'fallado')) DEFAULT 'programado',
    fecha_programada DATETIME,
    fecha_ejecucion DATETIME,
    fecha_completado DATETIME,
    tamano_bytes BIGINT,
    duracion_segundos INT,
    source_data TEXT,
    error_message TEXT,
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (politica_id) REFERENCES POLITICA_BACKUP(id) ON DELETE NO ACTION,
    FOREIGN KEY (cloud_storage_id) REFERENCES CLOUD_STORAGE(id) ON DELETE NO ACTION
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
    FOREIGN KEY (usuario_id) REFERENCES USERS(id),
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
    FOREIGN KEY (usuario_id) REFERENCES USERS(id),
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
    FOREIGN KEY (usuario_id) REFERENCES USERS(id)
);

ALTER TABLE POLITICA_BACKUP ADD descripcion TEXT NULL;

CREATE INDEX idx_job_backup_estado ON JOB_BACKUP(estado);
CREATE INDEX idx_job_backup_fecha_programada ON JOB_BACKUP(fecha_programada);
CREATE INDEX idx_usuario_email ON USERS(email);
CREATE INDEX idx_alerta_severidad ON ALERTA(severidad);
CREATE INDEX idx_sesion_expires ON SESION(expires_at);
SELECT id, nombre FROM POLITICA_BACKUP;


INSERT INTO ORGANIZACION (nombre, configuracion, licencia_valida_hasta, max_usuarios)
VALUES
('DataSecure Corp', '{"pais": "RD", "industria": "Finanzas"}', '2026-12-31', 100),
('SafeCloud Solutions', '{"pais": "USA", "industria": "Tecnología"}', '2026-06-15', 250),
('MultiBackup Inc', '{"pais": "México", "industria": "Salud"}', '2025-12-31', 50),
('CloudShield', '{"pais": "Chile", "industria": "Educación"}', '2026-08-20', 75),
('RecoveryOne', '{"pais": "Colombia", "industria": "Retail"}', '2027-01-10', 150);

INSERT INTO CLOUD_STORAGE (organizacion_id, proveedor, configuration, tier_actual, costo_mensual)
VALUES
(1, 'aws', '{"bucket": "backup-aws-1"}', 'frecuente', 150.00),
(1, 'azure', '{"container": "backup-azure-1"}', 'infrecuente', 75.50),
(1, 'gcp', '{"bucket": "backup-gcp-1"}', 'frecuente', 120.00),
(2, 'aws', '{"bucket": "backup-aws-2"}', 'archivo', 45.25),
(2, 'azure', '{"container": "backup-azure-2"}', 'frecuente', 89.99);

-- 4. INSERTAR POLITICA_BACKUP
INSERT INTO POLITICA_BACKUP (organizacion_id, nombre, descripcion, frecuencia, tipo_backup, retencion_dias, rpo_minutes, rto_minutes, ventana_ejecucion)
VALUES
(1, 'Backup Financiero Diario', 'Backup de datos financieros críticos', 'diaria', 'completo', 30, 1440, 60, '00:00-06:00'),
(1, 'Backup Usuarios Semanal', 'Backup de datos de usuarios', 'semanal', 'incremental', 90, 10080, 120, '01:00-04:00'),
(1, 'Backup SQL Horario', 'Backup de bases de datos SQL', 'horaria', 'diferencial', 7, 60, 30, '00:00-23:59'),
(2, 'Backup Sucursal Norte', 'Backup general sucursal norte', 'diaria', 'completo', 15, 1440, 90, '02:00-05:00'),
(2, 'Backup Archivos Mensual', 'Backup de archivos importantes', 'mensual', 'completo', 365, 43200, 240, '22:00-06:00');

INSERT INTO Users (organizacion_id, email, password_hash, nombre, rol, mfa_habilitado)
VALUES
(1, 'admin@datasecure.com', 'hash123', 'Carlos Méndez', 'admin', 1),
(2, 'user1@datasecure.com', 'hash456', 'Ana López', 'usuario', 0),
(3, 'admin@safecloud.com', 'hash789', 'María Torres', 'admin', 1),
(4, 'auditor@multibackup.com', 'hash321', 'Luis Pérez', 'auditor', 0),
(5, 'user@cloudshield.com', 'hash654', 'Laura Díaz', 'usuario', 0);


INSERT INTO JOB_BACKUP (
    politica_id, 
    cloud_storage_id, 
    estado, 
    fecha_programada, 
    fecha_ejecucion, 
    fecha_completado, 
    tamano_bytes, 
    duracion_segundos, 
    source_data
)
VALUES
(6, 6, 'completado', '2025-10-10 00:00:00', '2025-10-10 00:05:00', '2025-10-10 00:45:00', 5000000000, 2400, 'C:\data\finance'),
(7, 8, 'fallado', '2025-10-11 00:00:00', '2025-10-11 00:05:00', NULL, 0, 120, 'C:\data\finance'),
(8, 9, 'completado', '2025-10-09 01:00:00', '2025-10-09 01:10:00', '2025-10-09 01:40:00', 8000000000, 1800, 'D:\backups\users'),
(9, 10, 'ejecutando', '2025-10-12 02:00:00', '2025-10-12 02:10:00', NULL, 20000000000, 300, 'E:\server\sql'),
(10, 7, 'programado', '2025-10-15 03:00:00', NULL, NULL, 0, NULL, 'F:\backup\db');

SELECT id FROM JOB_BACKUP;
SELECT * FROM ORGANIZACION;
SELECT id, proveedor, organizacion_id FROM CLOUD_STORAGE;


INSERT INTO RECUPERACION (usuario_id, job_id, tipo_recuperacion, punto_tiempo, input_path, estado, is_simulacion)
VALUES
(1, 24, 'completa', '2025-10-10 01:00:00', 'C:\restore\finance', 'completado', 0),
(2, 27, 'granular', '2025-10-11 01:30:00', 'C:\restore\partial', 'fallado', 0),
(3, 28, 'punto_tiempo', '2025-10-09 02:00:00', 'D:\restore\userdb', 'completado', 0),
(1, 26, 'simulacion', '2025-10-12 02:15:00', 'E:\restore\sim', 'en_progreso', 1),
(2, 25, 'completa', '2025-10-13 03:00:00', 'F:\restore\db', 'pendiente', 0);



INSERT INTO VERIFICACION_INTEGRIDAD (job_id, checksum_sha256, resultado, detalles, integrity_score)
VALUES
(24, 'a1b2c3d4e5f6', 1, '{"bloques_corruptos":0}', 100.0),
(28, 'f7g8h9i0j1k2', 0, '{"bloques_corruptos":3}', 75.5),
(27, 'l3m4n5o6p7q8', 1, '{"bloques_corruptos":0}', 99.9),
(26, 'r9s0t1u2v3w4', 1, '{"bloques_corruptos":1}', 97.5),
(25, 'x5y6z7a8b9c0', 1, '{"bloques_corruptos":0}', 100.0);

INSERT INTO ALERTA (usuario_id, job_id, tipo, severidad, mensaje)
VALUES
(1, 28, 'Fallo de Backup', 'crítica', 'El backup programado ha fallado.'),
(2, 27, 'Verificación', 'media', 'Checksum con desviación detectada.'),
(3, 24, 'Recuperación', 'baja', 'Simulación de recuperación iniciada.'),
(4, 25, 'Backup Pendiente', 'media', 'Backup programado aún no se ejecuta.'),
(5, 26, 'Integridad', 'alta', 'Verificación completada con éxito.');


INSERT INTO SESION (usuario_id, token, expires_at, ip_address, user_agent)
VALUES
(1, 'token123', '2025-10-13 12:00', '192.168.1.10', 'Mozilla/5.0'),
(2, 'token456', '2025-10-13 13:00', '192.168.1.11', 'Chrome/120.0'),
(3, 'token789', '2025-10-13 14:00', '192.168.1.12', 'Edge/121.0'),
(4, 'token321', '2025-10-13 15:00', '192.168.1.13', 'Safari/17.0'),
(5, 'token654', '2025-10-13 16:00', '192.168.1.14', 'Opera/105.0');




SELECT 
    pb.nombre as Nombre,
    CAST(COALESCE(jb.tamano_bytes, 0) / 1073741824.0 AS DECIMAL(10,2)) as TamanioGB,
    cs.proveedor as Proveedor,
    jb.estado as Estado,
    jb.fecha_ejecucion as HoraEjecucion
FROM JOB_BACKUP jb
INNER JOIN POLITICA_BACKUP pb ON jb.politica_id = pb.id
INNER JOIN CLOUD_STORAGE cs ON jb.cloud_storage_id = cs.id
WHERE jb.fecha_ejecucion IS NOT NULL
ORDER BY jb.fecha_ejecucion DESC;


SELECT 
    cs.proveedor as Proveedor,
    CAST(COALESCE(SUM(jb.tamano_bytes), 0) / 1099511627776.0 AS DECIMAL(10,2)) as UsadoTB,
    

    CASE 
        WHEN cs.proveedor = 'aws' THEN 1000.00  -- 1000 TB disponibles
        WHEN cs.proveedor = 'azure' THEN 500.00  -- 500 TB disponibles  
        WHEN cs.proveedor = 'gcp' THEN 750.00    -- 750 TB disponibles
    END as TotalTB,
    
    CASE 
        WHEN COUNT(jb.id) = 0 THEN 'Inactivo'
        WHEN COUNT(CASE WHEN jb.estado = 'ejecutando' THEN 1 END) > 0 THEN 'Activo - Ejecutando'
        ELSE 'Activo'
    END as Estado
    
FROM CLOUD_STORAGE cs
LEFT JOIN JOB_BACKUP jb ON cs.id = jb.cloud_storage_id AND jb.estado IN ('completado', 'ejecutando')
WHERE cs.is_active = 1
GROUP BY cs.proveedor, cs.id;



EXEC sp_rename 'ORGANIZACION', 'Organizacion';
EXEC sp_rename 'CLOUD_STORAGE', 'CloudStorage';
EXEC sp_rename 'POLITICA_BACKUP', 'PoliticaBackup';
EXEC sp_rename 'USERS', 'Usuario';
EXEC sp_rename 'JOB_BACKUP', 'JobBackup';
EXEC sp_rename 'RECUPERACION', 'Recuperacion';
EXEC sp_rename 'VERIFICACION_INTEGRIDAD', 'VerificacionIntegridad';
EXEC sp_rename 'ALERTA', 'Alerta';
EXEC sp_rename 'SESION', 'Sesion';


SELECT name FROM sys.tables WHERE name IN (
    'Organizacion', 'CloudStorage', 'PoliticaBackup', 'Usuario', 
    'JobBackup', 'Recuperacion', 'VerificacionIntegridad', 'Alerta', 'Sesion'
);


CREATE VIEW vw_MetricasCalculadas AS
SELECT 
    CAST(COALESCE(SUM(jb.tamano_bytes), 0) / 1099511627776.0 AS DECIMAL(10,2)) as TotalAlmacenadoTB,
    COUNT(CASE WHEN CAST(jb.fecha_ejecucion AS DATE) = CAST(GETDATE() AS DATE) THEN 1 END) as BackupsHoy,
    CAST(
        COUNT(CASE WHEN jb.estado = 'completado' THEN 1 END) * 100.0 / 
        NULLIF(COUNT(*), 0) 
    AS DECIMAL(5,2)) as TasaExitoPorcentaje,
    MAX(jb.fecha_completado) as UltimaActualizacion,
    CAST(
        (SUM(CASE WHEN jb.fecha_completado >= DATEADD(MONTH, -1, GETDATE()) THEN jb.tamano_bytes ELSE 0 END) -
         SUM(CASE WHEN jb.fecha_completado BETWEEN DATEADD(MONTH, -2, GETDATE()) AND DATEADD(MONTH, -1, GETDATE()) 
              THEN jb.tamano_bytes ELSE 0 END)) / 1099511627776.0
    AS DECIMAL(10,2)) as IncrementoAlmacenamiento,
    CAST(
        (COUNT(CASE WHEN jb.fecha_completado >= DATEADD(MONTH, -1, GETDATE()) THEN 1 END) -
         COUNT(CASE WHEN jb.fecha_completado BETWEEN DATEADD(MONTH, -2, GETDATE()) AND DATEADD(MONTH, -1, GETDATE()) THEN 1 END)) * 1.0
    AS DECIMAL(10,2)) as IncrementoBackups,
    CAST(
        (COUNT(CASE WHEN jb.fecha_completado >= DATEADD(MONTH, -1, GETDATE()) AND jb.estado = 'completado' THEN 1 END) * 100.0 / 
         NULLIF(COUNT(CASE WHEN jb.fecha_completado >= DATEADD(MONTH, -1, GETDATE()) THEN 1 END), 0)) -
        (COUNT(CASE WHEN jb.fecha_completado BETWEEN DATEADD(MONTH, -2, GETDATE()) AND DATEADD(MONTH, -1, GETDATE()) 
               AND jb.estado = 'completado' THEN 1 END) * 100.0 / 
         NULLIF(COUNT(CASE WHEN jb.fecha_completado BETWEEN DATEADD(MONTH, -2, GETDATE()) AND DATEADD(MONTH, -1, GETDATE()) THEN 1 END), 0))
    AS DECIMAL(5,2)) as IncrementoTasaExito
FROM JobBackup jb  -- NUEVO nombre
WHERE jb.fecha_completado IS NOT NULL;
GO

CREATE VIEW vw_ProveedorStorage AS
SELECT 
    cs.proveedor as Proveedor,
    CAST(COALESCE(SUM(jb.tamano_bytes), 0) / 1099511627776.0 AS DECIMAL(10,2)) as UsadoTB,
    CASE 
        WHEN cs.proveedor = 'aws' THEN 1000.00
        WHEN cs.proveedor = 'azure' THEN 500.00  
        WHEN cs.proveedor = 'gcp' THEN 750.00
    END as TotalTB,
    CASE 
        WHEN COUNT(jb.id) = 0 THEN 'Inactivo'
        WHEN COUNT(CASE WHEN jb.estado = 'ejecutando' THEN 1 END) > 0 THEN 'Activo - Ejecutando'
        ELSE 'Activo'
    END as Estado
FROM CloudStorage cs 
LEFT JOIN JobBackup jb ON cs.id = jb.cloud_storage_id AND jb.estado IN ('completado', 'ejecutando')
WHERE cs.is_active = 1
GROUP BY cs.proveedor, cs.id;
GO

CREATE VIEW vw_BackupReciente AS
SELECT 
    pb.nombre as Nombre,
    CAST(COALESCE(jb.tamano_bytes, 0) / 1073741824.0 AS DECIMAL(10,2)) as TamanioGB,
    cs.proveedor as Proveedor,
    jb.estado as Estado,
    jb.fecha_ejecucion as HoraEjecucion
FROM JobBackup jb  -- NUEVO nombre
INNER JOIN PoliticaBackup pb ON jb.politica_id = pb.id  
INNER JOIN CloudStorage cs ON jb.cloud_storage_id = cs.id  
WHERE jb.fecha_ejecucion IS NOT NULL;
GO

SELECT 
    (SELECT * FROM vw_MetricasCalculadas FOR JSON PATH, WITHOUT_ARRAY_WRAPPER) as Metricas,
    (SELECT * FROM vw_ProveedorStorage FOR JSON PATH) as Proveedores,
    (SELECT TOP 10 * FROM vw_BackupReciente ORDER BY HoraEjecucion DESC FOR JSON PATH) as BackupsRecientes
FOR JSON PATH, WITHOUT_ARRAY_WRAPPER;


SELECT 
    name as Objeto,
    type_desc as Tipo
FROM sys.objects 
WHERE name IN ('vw_MetricasCalculadas', 'vw_ProveedorStorage', 'vw_BackupReciente', 'sp_CrearBackup')
   OR name LIKE '%Metricas%' OR name LIKE '%Backup%' OR name LIKE '%Proveedor%';


SELECT 'Metricas' as Tipo, COUNT(*) as Cantidad FROM vw_MetricasCalculadas
UNION ALL
SELECT 'Proveedores', COUNT(*) FROM vw_ProveedorStorage
UNION ALL
SELECT 'Backups Recientes', COUNT(*) FROM vw_BackupReciente;


IF OBJECT_ID('vw_MetricasCalculadas', 'V') IS NOT NULL
    DROP VIEW vw_MetricasCalculadas;
GO

IF OBJECT_ID('vw_ProveedorStorage', 'V') IS NOT NULL
    DROP VIEW vw_ProveedorStorage;
GO

IF OBJECT_ID('vw_BackupReciente', 'V') IS NOT NULL
    DROP VIEW vw_BackupReciente;
GO


CREATE PROCEDURE sp_CrearBackup
    @Nombre VARCHAR(255),
    @Descripcion TEXT,
    @RutaOrigen TEXT,
    @PoliticaId INT,
    @CloudStorageId INT,
    @FechaProgramada DATETIME
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
       
        IF NOT EXISTS (SELECT 1 FROM PoliticaBackup WHERE id = @PoliticaId AND is_active = 1)
        BEGIN
            RAISERROR('La política de backup especificada no existe o no está activa.', 16, 1);
            RETURN -1;
        END
        
  
        IF NOT EXISTS (SELECT 1 FROM CloudStorage WHERE id = @CloudStorageId AND is_active = 1)
        BEGIN
            RAISERROR('El cloud storage especificado no existe o no está activo.', 16, 1);
            RETURN -1;
        END
        
       
        IF @FechaProgramada < GETDATE()
        BEGIN
            RAISERROR('La fecha programada no puede ser en el pasado.', 16, 1);
            RETURN -1;
        END
        
        BEGIN TRANSACTION;
        
     
        INSERT INTO JobBackup (
            politica_id, 
            cloud_storage_id, 
            estado, 
            fecha_programada, 
            source_data
        )
        VALUES (
            @PoliticaId,
            @CloudStorageId,
            'programado',
            @FechaProgramada,
            @RutaOrigen
        );
        
      
        IF @Descripcion IS NOT NULL
        BEGIN
            UPDATE PoliticaBackup 
            SET descripcion = @Descripcion 
            WHERE id = @PoliticaId;
        END
        
        COMMIT TRANSACTION;
        
       
        SELECT SCOPE_IDENTITY() as JobId;
        RETURN 0;
        
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();
        
        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
        RETURN -1;
    END CATCH
END
GO

EXEC sp_CrearBackup 
    @Nombre = 'Backup Test',
    @Descripcion = 'Backup de prueba',
    @RutaOrigen = 'C:\test\data',
    @PoliticaId = 9,
    @CloudStorageId = 10,
    @FechaProgramada = '2027-12-20 02:00:00';

	
SELECT id, proveedor, organizacion_id, is_active 
FROM CloudStorage 
ORDER BY id;

 
SELECT id, nombre, organizacion_id, is_active 
FROM PoliticaBackup 
ORDER BY id;


SELECT TOP 5 * FROM CloudStorage;
SELECT TOP 5 * FROM PoliticaBackup;
SELECT TOP 5 * FROM JobBackup;
SELECT  * FROM Organizacion;


