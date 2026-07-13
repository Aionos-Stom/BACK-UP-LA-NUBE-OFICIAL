# BackUp La Nube — Sistema de Gestión de Respaldos en la Nube

Plataforma completa de gestión de backups en la nube con autenticación JWT segura, jerarquía de roles, sistema de planes/suscripciones y panel de administración.

---

## Índice

1. [Arquitectura del Sistema](#1-arquitectura-del-sistema)
2. [Stack Tecnológico](#2-stack-tecnológico)
3. [Estructura del Proyecto](#3-estructura-del-proyecto)
4. [Base de Datos — Esquema Completo](#4-base-de-datos--esquema-completo)
5. [Registros de Semilla en la BD](#5-registros-de-semilla-en-la-bd)
6. [Autenticación y Roles](#6-autenticación-y-roles)
7. [Sistema de Planes](#7-sistema-de-planes)
8. [Variables de Entorno](#8-variables-de-entorno)
9. [Despliegue con Docker — Paso a Paso](#9-despliegue-con-docker--paso-a-paso)
10. [Desarrollo Local (sin Docker)](#10-desarrollo-local-sin-docker)
11. [API — Endpoints](#11-api--endpoints)
12. [Rutas del Frontend](#12-rutas-del-frontend)
13. [Funcionalidades Implementadas](#13-funcionalidades-implementadas)
14. [Informe de Ciberseguridad](#14-informe-de-ciberseguridad)
15. [Bugs Críticos Corregidos](#15-bugs-críticos-corregidos)
16. [Mejoras Futuras Recomendadas](#16-mejoras-futuras-recomendadas)

---

## 1. Arquitectura del Sistema

```
Navegador
    │  Cookie httpOnly (access_token)
    ▼
Nginx :3001  (React SPA)
    │  proxy /api/ → http://api:8080/api/
    ▼
ASP.NET Core 8 API :8080
    │  JWT Bearer + Cookie
    ▼
SQL Server 2022 :1433  (solo accesible dentro de Docker, no expuesto al host)
```

### Capas del Backend (Clean Architecture)

```
BackUp.API/             → Controladores, Middleware, Program.cs
BackUp.Aplication/      → Servicios, DTOs, Interfaces
BackUp.Domainn/         → Entidades de dominio, reglas de negocio
BackUp.Persistence/     → DbContext (EF Core), repositorios
BackUp.IOC1/            → Inyección de dependencias
BackUp.Infraestructure/ → Implementaciones de infraestructura
```

### Contenedores Docker

| Contenedor | Imagen | Puerto Host | Descripción |
|------------|--------|-------------|-------------|
| `backup-db` | SQL Server 2022 | — (solo interno) | Base de datos principal |
| `backup-db-init` | SQL Server 2022 (one-shot) | — | Inicializa esquema y datos semilla |
| `backup-api` | .NET 8 (build local) | 5271 → 8080 | API REST |
| `backup-frontend` | React + Nginx (build local) | 3001 → 80 | Dashboard web |

---

## 2. Stack Tecnológico

### Backend
- **ASP.NET Core 8** — Framework web
- **Entity Framework Core 8** — ORM
- **SQL Server 2022** — Base de datos
- **JWT Bearer** — Autenticación stateless vía cookies httpOnly
- **BCrypt.Net-Next** — Hash de contraseñas (workFactor: 12)
- **Rate Limiting** — Integrado en ASP.NET Core 8 (5 req/min auth, 100 req/min global)

### Frontend
- **React 18** + **Vite** + **TypeScript**
- **Material UI (MUI) 5** — Tema oscuro personalizado
- **React Router v6** — Enrutamiento con rutas protegidas por rol
- **Axios** — Cliente HTTP (`withCredentials: true`)

### Infraestructura
- **Docker** + **Docker Compose** — Orquestación
- **Nginx** — Servidor web + proxy reverso para la API

---

## 3. Estructura del Proyecto

```
BACK-UP-LA-NUBE-OFICIAL-main/
├── docker-compose.yml              # Orquestación de 4 servicios
├── .env                            # Variables de entorno (SA_PASSWORD, JWT_SECRET)
├── DATABASE NUBE.sql               # Script completo: esquema + datos de semilla
├── docker/
│   └── init-db.sh                  # Script bash de inicialización de BD
│
├── BackUp.API/                     # API ASP.NET Core 8
│   ├── Controllers/                # Un controlador por dominio
│   │   ├── AuthController.cs       # Login, Register, Refresh, Logout, Me, Setup
│   │   ├── AdminController.cs      # Gestión de usuarios y planes (admin/superadmin)
│   │   ├── PlanController.cs       # CRUD planes y suscripciones
│   │   ├── PerfilController.cs     # Perfil, sesiones activas, historial pagos
│   │   ├── AuditController.cs      # AuditLog paginado y filtrable
│   │   ├── NotificacionController.cs
│   │   ├── JobBackupController.cs
│   │   ├── AlertaController.cs
│   │   ├── CloudStorageController.cs
│   │   ├── PoliticaBackupController.cs
│   │   ├── RecuperacionController.cs
│   │   ├── BackupController.cs
│   │   ├── DashboardController.cs
│   │   ├── OrganizacionController.cs
│   │   ├── SesionController.cs
│   │   └── VerificacionIntegridadController.cs
│   ├── Services/
│   │   ├── JwtService.cs           # Generación y validación de JWT
│   │   └── AuditService.cs         # Registro de acciones en AuditLog
│   ├── Dockerfile
│   └── Program.cs                  # Configuración de middleware y servicios
│
├── BackUp.Domainn/                 # Entidades del dominio
│   └── Entities/
│       ├── Users/
│       │   ├── Usuario.cs
│       │   └── RefreshToken.cs
│       └── Bac/
│           ├── Plan.cs
│           ├── Suscripcion.cs
│           └── Pago.cs
│
├── BackUp.Persistence/             # EF Core
│   ├── Context/BackUpDbContext.cs  # DbSets + configuración de relaciones
│   └── Repositories/              # Repositorios por entidad
│
├── BackUp.Aplication/             # Capa de aplicación
├── BackUp.Infraestructure/        # Implementaciones de infraestructura
├── BackUp.IOC1/                   # Contenedor de dependencias (DI)
│
└── AnBackUp/                       # Frontend React/TypeScript
    ├── src/
    │   ├── contexts/AuthContext.tsx # Estado global de auth (CERO localStorage)
    │   ├── services/api.ts          # Axios con withCredentials: true
    │   ├── components/
    │   │   ├── Layout.tsx           # Sidebar con menú filtrado por rol
    │   │   └── Toast.tsx            # Notificaciones toast
    │   └── pages/
    │       ├── Login.tsx
    │       ├── Register.tsx
    │       ├── Dashboard.tsx
    │       ├── JobBackups.tsx
    │       ├── Alertas.tsx
    │       ├── CloudStorage.tsx
    │       ├── Politicas.tsx
    │       ├── Recuperaciones.tsx
    │       ├── VerificacionIntegridad.tsx
    │       ├── Perfil.tsx
    │       ├── Planes.tsx
    │       └── admin/
    │           ├── AdminDashboard.tsx
    │           └── AuditLog.tsx
    ├── nginx.conf                  # Proxy /api/ → contenedor API
    └── Dockerfile                  # Build React con Vite + Nginx
```

---

## 4. Base de Datos — Esquema Completo

**Nombre de la BD:** `n`  
**Motor:** Microsoft SQL Server 2022  
**Script:** `DATABASE NUBE.sql` (raíz del proyecto)  
**Inicialización:** automática con `docker compose up` (solo si la BD no existe)

### Diagrama de relaciones

```
Organizacion ──┬── Usuario ──┬── Suscripcion ── PlanSuscripcion
               │             ├── RefreshToken
               │             ├── AuditLog
               │             ├── Alerta
               │             ├── Sesion
               │             └── Recuperacion
               ├── CloudStorage
               └── PoliticaBackup ── JobBackup ──┬── Recuperacion
                                                  ├── VerificacionIntegridad
                                                  └── Alerta

PlanSuscripcion ── Suscripcion ── Pago
```

---

### Tabla: `Organizacion`

Entidad raíz. Todas las demás entidades pertenecen a una organización.

| Columna | Tipo | Default | Descripción |
|---------|------|---------|-------------|
| id | INT IDENTITY PK | — | Clave primaria autoincremental |
| nombre | VARCHAR(255) NOT NULL | — | Nombre de la empresa |
| configuracion | NVARCHAR(MAX) | NULL | JSON con país e industria |
| licencia_valida_hasta | DATE | NULL | Vencimiento de licencia |
| max_usuarios | INT | NULL | Límite de usuarios permitidos |
| fecha_creacion | DATETIME | CURRENT_TIMESTAMP | Alta en el sistema |
| activo | BIT | 1 | Estado activo/inactivo |

---

### Tabla: `Usuario`

> Las contraseñas se almacenan como hash **BCrypt** (workFactor 12). Nunca se guarda la contraseña en texto plano.

| Columna | Tipo | Default | Descripción |
|---------|------|---------|-------------|
| id | INT IDENTITY PK | — | Clave primaria |
| organizacion_id | INT FK NOT NULL | — | Organización a la que pertenece |
| email | VARCHAR(255) UNIQUE NOT NULL | — | Email de acceso |
| password_hash | VARCHAR(255) NOT NULL | — | Hash BCrypt de la contraseña |
| nombre | VARCHAR(255) NOT NULL | — | Nombre completo |
| rol | VARCHAR(20) NOT NULL | `'usuario'` | `superadmin` / `admin` / `usuario` / `auditor` |
| mfa_habilitado | BIT | 0 | Si tiene MFA activo |
| mfa_secret | VARCHAR(255) | NULL | Secreto TOTP para MFA |
| last_login | DATETIME | NULL | Último acceso registrado |
| PhoneNumber | NVARCHAR(20) | NULL | Teléfono (opcional) |
| is_active | BIT | 1 | Cuenta habilitada |
| CreatedAt | DATETIME | CURRENT_TIMESTAMP | Fecha de creación |

**Índice:** `idx_usuario_email ON Usuario(email)`  
**Constraint:** `CHECK (rol IN ('superadmin', 'admin', 'usuario', 'auditor'))`

---

### Tabla: `CloudStorage`

| Columna | Tipo | Default | Descripción |
|---------|------|---------|-------------|
| id | INT IDENTITY PK | — | Clave primaria |
| organizacion_id | INT FK NOT NULL | — | Organización propietaria |
| proveedor | VARCHAR(20) NOT NULL | — | `aws` / `azure` / `gcp` |
| configuration | NVARCHAR(MAX) | NULL | JSON con config de bucket/container |
| endpoint_url | TEXT | NULL | URL personalizada (opcional) |
| tier_actual | VARCHAR(20) NOT NULL | — | `frecuente` / `infrecuente` / `archivo` |
| costo_mensual | DECIMAL(10,2) | 0.00 | Costo mensual en USD |
| is_active | BIT | 1 | Activo |
| fecha_creacion | DATETIME | CURRENT_TIMESTAMP | Alta |

**ON DELETE:** CASCADE desde Organizacion

---

### Tabla: `PoliticaBackup`

| Columna | Tipo | Default | Descripción |
|---------|------|---------|-------------|
| id | INT IDENTITY PK | — | Clave primaria |
| organizacion_id | INT FK NOT NULL | — | Organización dueña |
| nombre | VARCHAR(255) NOT NULL | — | Nombre descriptivo |
| descripcion | TEXT | NULL | Descripción (añadida con ALTER TABLE) |
| frecuencia | VARCHAR(20) NOT NULL | — | `horaria` / `diaria` / `semanal` / `mensual` |
| tipo_backup | VARCHAR(20) NOT NULL | — | `completo` / `incremental` / `diferencial` |
| retencion_dias | INT NOT NULL | — | Días de retención de backups |
| rpo_minutes | INT NOT NULL | — | Recovery Point Objective (minutos) |
| rto_minutes | INT NOT NULL | — | Recovery Time Objective (minutos) |
| ventana_ejecucion | NVARCHAR(MAX) | NULL | Rango horario (ej: `00:00-06:00`) |
| is_active | BIT | 1 | Política activa |
| fecha_creacion | DATETIME | CURRENT_TIMESTAMP | Alta |

**ON DELETE:** CASCADE desde Organizacion

---

### Tabla: `JobBackup`

| Columna | Tipo | Default | Descripción |
|---------|------|---------|-------------|
| id | INT IDENTITY PK | — | Clave primaria |
| politica_id | INT FK NOT NULL | — | Política que originó el job |
| cloud_storage_id | INT FK NOT NULL | — | Destino de almacenamiento |
| estado | VARCHAR(20) NOT NULL | `'programado'` | `programado` / `ejecutando` / `completado` / `fallado` |
| fecha_programada | DATETIME | NULL | Cuándo está agendado |
| fecha_ejecucion | DATETIME | NULL | Cuándo inició |
| fecha_completado | DATETIME | NULL | Cuándo terminó |
| tamano_bytes | BIGINT | NULL | Tamaño del backup en bytes |
| duracion_segundos | INT | NULL | Duración en segundos |
| source_data | TEXT | NULL | Ruta de origen de los datos |
| error_message | TEXT | NULL | Mensaje de error si falló |
| created_at | DATETIME | CURRENT_TIMESTAMP | Creación del registro |

**ON DELETE:** NO ACTION desde PoliticaBackup y CloudStorage  
**Índices:** `idx_job_backup_estado`, `idx_job_backup_fecha_programada`

---

### Tabla: `Recuperacion`

| Columna | Tipo | Default | Descripción |
|---------|------|---------|-------------|
| id | INT IDENTITY PK | — | Clave primaria |
| usuario_id | INT FK NOT NULL | — | Usuario que solicitó la recuperación |
| job_id | INT FK NOT NULL | — | Job de backup origen |
| tipo_recuperacion | VARCHAR(20) | NULL | `completa` / `granular` / `punto_tiempo` / `simulacion` |
| punto_tiempo | DATETIME | NULL | Punto exacto a restaurar |
| input_path | TEXT | NULL | Ruta de destino de restauración |
| estado | VARCHAR(20) | NULL | Estado del proceso |
| is_simulacion | BIT | 0 | Si es test/simulacro (no restaura datos reales) |
| created_at | DATETIME | CURRENT_TIMESTAMP | Fecha de solicitud |
| completed_at | DATETIME | NULL | Fecha de finalización |

---

### Tabla: `VerificacionIntegridad`

| Columna | Tipo | Default | Descripción |
|---------|------|---------|-------------|
| id | INT IDENTITY PK | — | Clave primaria |
| job_id | INT FK NOT NULL | — | Job verificado |
| checksum_sha256 | VARCHAR(64) | NULL | Hash SHA-256 del backup |
| resultado | BIT | NULL | 1 = íntegro, 0 = corrupto |
| fecha_verificacion | DATETIME | CURRENT_TIMESTAMP | Cuándo se verificó |
| detalles | NVARCHAR(MAX) | NULL | JSON (ej: `{"bloques_corruptos":0}`) |
| integrity_score | DECIMAL(5,2) | NULL | Puntuación 0–100 |

**ON DELETE:** CASCADE desde JobBackup

---

### Tabla: `Alerta`

| Columna | Tipo | Default | Descripción |
|---------|------|---------|-------------|
| id | INT IDENTITY PK | — | Clave primaria |
| usuario_id | INT FK NOT NULL | — | Usuario destinatario |
| job_id | INT FK | NULL | Job relacionado (puede ser NULL) |
| tipo | VARCHAR(50) | NULL | Tipo de alerta |
| severidad | VARCHAR(20) | NULL | `baja` / `media` / `alta` / `crítica` |
| mensaje | TEXT | NULL | Mensaje descriptivo |
| is_acknowledged | BIT | 0 | Si el usuario la reconoció |
| created_at | DATETIME | CURRENT_TIMESTAMP | Timestamp |

**Índice:** `idx_alerta_severidad`

---

### Tabla: `Sesion`

Tabla legacy de sesiones (coexiste con RefreshToken para compatibilidad).

| Columna | Tipo | Default | Descripción |
|---------|------|---------|-------------|
| id | INT IDENTITY PK | — | Clave primaria |
| usuario_id | INT FK NOT NULL | — | Usuario de la sesión |
| token | TEXT NOT NULL | — | Token de sesión |
| expires_at | DATETIME NOT NULL | — | Expiración |
| ip_address | VARCHAR(45) | NULL | IP del cliente |
| user_agent | TEXT | NULL | Navegador/cliente |
| created_at | DATETIME | CURRENT_TIMESTAMP | Creación |

**Índice:** `idx_sesion_expires`

---

### Tabla: `PlanSuscripcion`

| Columna | Tipo | Default | Descripción |
|---------|------|---------|-------------|
| id | INT IDENTITY PK | — | Clave primaria |
| nombre | VARCHAR(100) NOT NULL | — | Nombre del plan |
| descripcion | NVARCHAR(500) | NULL | Descripción |
| precio_mensual | DECIMAL(10,2) NOT NULL | 0 | Precio en USD |
| limite_almacenamiento_bytes | BIGINT NOT NULL | 5368709120 | Límite de almacenamiento (5 GB default) |
| max_jobs_concurrentes | INT NOT NULL | 2 | Jobs paralelos permitidos |
| max_politicas | INT NOT NULL | 3 | Políticas de backup máximas |
| backup_automatico | BIT NOT NULL | 0 | Incluye backup automático |
| soporte_prioritario | BIT NOT NULL | 0 | Incluye soporte prioritario |
| es_gratuito | BIT NOT NULL | 0 | Es el plan gratuito base |
| is_active | BIT NOT NULL | 1 | Plan disponible en el catálogo |

---

### Tabla: `Suscripcion`

| Columna | Tipo | Default | Descripción |
|---------|------|---------|-------------|
| id | INT IDENTITY PK | — | Clave primaria |
| usuario_id | INT FK NOT NULL | — | Usuario suscriptor |
| plan_id | INT FK NOT NULL | — | Plan activo |
| estado | VARCHAR(30) NOT NULL | `'activa'` | `activa` / `gratis_admin` / `cancelada` / `pendiente` |
| fecha_inicio | DATETIME NOT NULL | CURRENT_TIMESTAMP | Inicio de la suscripción |
| fecha_fin | DATETIME | NULL | Fin (NULL = sin vencimiento) |
| es_gratis_admin_granted | BIT NOT NULL | 0 | Asignado gratuitamente por un admin |
| otorgada_por_admin_id | INT | NULL | ID del admin que otorgó el plan gratis |
| almacenamiento_usado_bytes | BIGINT NOT NULL | 0 | Bytes consumidos actualmente |

---

### Tabla: `Pago`

| Columna | Tipo | Default | Descripción |
|---------|------|---------|-------------|
| id | INT IDENTITY PK | — | Clave primaria |
| suscripcion_id | INT FK NOT NULL | — | Suscripción asociada |
| monto | DECIMAL(10,2) NOT NULL | — | Monto pagado |
| moneda | VARCHAR(10) NOT NULL | `'USD'` | Moneda |
| estado | VARCHAR(20) NOT NULL | `'pendiente'` | `pendiente` / `completado` / `fallido` |
| metodo_pago | VARCHAR(50) NOT NULL | — | Método de pago utilizado |
| referencia_externa | NVARCHAR(255) | NULL | ID en el gateway de pago externo |
| fecha_pago | DATETIME NOT NULL | CURRENT_TIMESTAMP | Timestamp del pago |
| descripcion | NVARCHAR(500) | NULL | Descripción del cobro |

---

### Tabla: `RefreshToken`

| Columna | Tipo | Default | Descripción |
|---------|------|---------|-------------|
| id | INT IDENTITY PK | — | Clave primaria |
| usuario_id | INT FK NOT NULL | — | Usuario propietario |
| token | VARCHAR(512) NOT NULL | — | Token generado aleatoriamente |
| expiracion | DATETIME NOT NULL | — | Expiración a 7 días |
| revocado | BIT NOT NULL | 0 | Si fue revocado explícitamente |
| creado_en | DATETIME NOT NULL | CURRENT_TIMESTAMP | Timestamp de creación |
| reemplazado_por | VARCHAR(512) | NULL | Token que lo reemplazó (rotación) |

---

### Tabla: `AuditLog`

| Columna | Tipo | Default | Descripción |
|---------|------|---------|-------------|
| id | INT IDENTITY PK | — | Clave primaria |
| usuario_id | INT FK | NULL | Usuario que ejecutó la acción |
| accion | VARCHAR(100) NOT NULL | — | `LOGIN`, `LOGOUT`, `REGISTER`, `CAMBIAR_ROL`, etc. |
| entidad | VARCHAR(100) NOT NULL | — | Entidad afectada (ej: `Usuario`) |
| entidad_id | VARCHAR(50) | NULL | ID del registro afectado |
| datos_anteriores | NVARCHAR(MAX) | NULL | Estado previo en JSON |
| datos_nuevos | NVARCHAR(MAX) | NULL | Estado nuevo en JSON |
| ip_address | VARCHAR(45) | NULL | IP del cliente |
| user_agent | NVARCHAR(500) | NULL | Navegador/cliente |
| email | VARCHAR(255) | NULL | Email del actor de la acción |
| creado_en | DATETIME NOT NULL | CURRENT_TIMESTAMP | Timestamp UTC |

**ON DELETE:** SET NULL (si el usuario se elimina, el log se conserva)  
**Índices:** `idx_audit_usuario`, `idx_audit_accion`, `idx_audit_fecha`

---

### Vistas SQL

#### `vw_MetricasCalculadas`
KPIs para el dashboard principal:
- `TotalAlmacenadoTB` — Total en terabytes de todos los backups completados
- `BackupsHoy` — Cantidad de backups ejecutados hoy
- `TasaExitoPorcentaje` — Porcentaje de jobs exitosos (completado vs total)
- `UltimaActualizacion` — Timestamp del último backup completado
- `IncrementoAlmacenamiento` — Diferencia de almacenamiento vs mes anterior
- `IncrementoBackups` — Diferencia de cantidad de backups vs mes anterior
- `IncrementoTasaExito` — Diferencia de tasa de éxito vs mes anterior

#### `vw_ProveedorStorage`
Uso por proveedor cloud:
- `Proveedor`, `UsadoTB`, `TotalTB` (capacidades fijas: AWS=1000, Azure=500, GCP=750), `Estado`

#### `vw_BackupReciente`
Últimos jobs ejecutados:
- `Nombre` (política), `TamanioGB`, `Proveedor`, `Estado`, `HoraEjecucion`

---

### Procedimiento Almacenado: `sp_CrearBackup`

Crea un `JobBackup` con validaciones transaccionales integradas.

**Parámetros:**

| Parámetro | Tipo | Descripción |
|-----------|------|-------------|
| @Nombre | VARCHAR(255) | Nombre del backup |
| @Descripcion | TEXT | Descripción (opcional) |
| @RutaOrigen | TEXT | Ruta de los datos a respaldar |
| @PoliticaId | INT | ID de la política de backup |
| @CloudStorageId | INT | ID del cloud storage destino |
| @FechaProgramada | DATETIME | Fecha/hora (debe ser futura) |

**Retorna:** `JobId` del registro creado (SCOPE_IDENTITY).

**Validaciones:**
- La política debe existir y tener `is_active = 1`.
- El cloud storage debe existir y tener `is_active = 1`.
- La `FechaProgramada` debe ser mayor a `GETDATE()`.
- Todo se ejecuta dentro de una transacción con rollback automático.

---

## 5. Registros de Semilla en la BD

Estos datos se insertan automáticamente la primera vez que Docker levanta la base de datos desde el script `DATABASE NUBE.sql`.

> **Todos estos registros son datos de demostración ficticios** (empresas, emails, IPs, hashes de contraseña placeholder) pensados únicamente para poblar el esquema y poder ver la aplicación funcionando de inmediato. No representan datos reales de ninguna organización ni usuario. El script `DATABASE NUBE.sql` (esquema + semilla demo) es lo único que se necesita para levantar una base de datos nueva desde cero — no se requiere ni se sube ningún respaldo/export con datos reales.

### Organizaciones

| ID | Nombre | País | Industria | Licencia hasta | Máx. Usuarios |
|----|--------|------|-----------|---------------|---------------|
| 1 | DataSecure Corp | RD | Finanzas | 2026-12-31 | 100 |
| 2 | SafeCloud Solutions | USA | Tecnología | 2026-06-15 | 250 |
| 3 | MultiBackup Inc | México | Salud | 2025-12-31 | 50 |
| 4 | CloudShield | Chile | Educación | 2026-08-20 | 75 |
| 5 | RecoveryOne | Colombia | Retail | 2027-01-10 | 150 |

---

### Usuarios de Semilla

> **IMPORTANTE — Los usuarios de semilla NO pueden iniciar sesión.**
>
> El script inserta usuarios con `password_hash` de marcador de posición (`hash123`, `hash456`, etc.). Estos **NO son hashes BCrypt válidos**. El sistema usa `BCrypt.Net.BCrypt.Verify(password, hash)` para autenticar, y un valor como `hash123` siempre falla esa verificación.
>
> Sirven únicamente como datos de referencia para las relaciones de FK (Jobs, Alertas, Recuperaciones).
>
> **Para tener usuarios funcionales:**
> - Crear el superadmin con `POST /api/auth/setup` (ver Sección 9, Paso 4)
> - Registrar usuarios normales con `POST /api/auth/register` desde el frontend

| ID | Email | Nombre | Rol | Org | MFA | Password hash en BD |
|----|-------|--------|-----|-----|-----|---------------------|
| 1 | admin@datasecure.com | Carlos Méndez | admin | 1 | Sí | `hash123` (no funcional) |
| 2 | user1@datasecure.com | Ana López | usuario | 2 | No | `hash456` (no funcional) |
| 3 | admin@safecloud.com | María Torres | admin | 3 | Sí | `hash789` (no funcional) |
| 4 | auditor@multibackup.com | Luis Pérez | auditor | 4 | No | `hash321` (no funcional) |
| 5 | user@cloudshield.com | Laura Díaz | usuario | 5 | No | `hash654` (no funcional) |

---

### Planes de Suscripción (semilla funcional)

Estos registros SÍ son correctos y funcionales desde el primer arranque.

| ID | Nombre | Precio/mes | Almacenamiento | Jobs Simultáneos | Políticas | Auto-Backup | Soporte |
|----|--------|-----------|---------------|-----------------|-----------|-------------|---------|
| 1 | Gratuito | $0.00 | 5 GB | 2 | 3 | No | No |
| 2 | Basico | $9.99 | 50 GB | 5 | 10 | Sí | No |
| 3 | Pro | $29.99 | 500 GB | 20 | 50 | Sí | Sí |
| 4 | Empresa | $99.99 | 5 TB | 100 | 999 | Sí | Sí |
| 5 | Ilimitado | $0.00 | Sin límite (9.2 EB) | 999 | 9999 | Sí | Sí |

> El plan **Ilimitado** tiene precio $0 porque solo se asigna manualmente por un superadmin/admin.

---

### Cloud Storage de Semilla

| ID | Organización | Proveedor | Tier | Costo/mes |
|----|-------------|-----------|------|-----------|
| 1 | DataSecure Corp (org 1) | AWS | frecuente | $150.00 |
| 2 | DataSecure Corp (org 1) | Azure | infrecuente | $75.50 |
| 3 | DataSecure Corp (org 1) | GCP | frecuente | $120.00 |
| 4 | SafeCloud Solutions (org 2) | AWS | archivo | $45.25 |
| 5 | SafeCloud Solutions (org 2) | Azure | frecuente | $89.99 |

---

### Políticas de Backup de Semilla

| ID | Org | Nombre | Frecuencia | Tipo | Retención | RPO | RTO |
|----|-----|--------|-----------|------|-----------|-----|-----|
| 6 | 1 | Backup Financiero Diario | diaria | completo | 30 días | 1440 min | 60 min |
| 7 | 1 | Backup Usuarios Semanal | semanal | incremental | 90 días | 10080 min | 120 min |
| 8 | 1 | Backup SQL Horario | horaria | diferencial | 7 días | 60 min | 30 min |
| 9 | 2 | Backup Sucursal Norte | diaria | completo | 15 días | 1440 min | 90 min |
| 10 | 2 | Backup Archivos Mensual | mensual | completo | 365 días | 43200 min | 240 min |

---

### Jobs de Backup de Semilla

| Política | Storage | Estado | Tamaño | Origen |
|---------|---------|--------|--------|--------|
| 6 | 6 | completado | 5 GB | C:\data\finance |
| 7 | 8 | fallado | 0 B | C:\data\finance |
| 8 | 9 | completado | 8 GB | D:\backups\users |
| 9 | 10 | ejecutando | 20 GB | E:\server\sql |
| 10 | 7 | programado | — | F:\backup\db |

---

## 6. Autenticación y Roles

El sistema usa **JWT en cookies httpOnly**. Los tokens nunca se exponen a JavaScript (protección contra XSS).

### Flujo de autenticación

```
1. POST /api/auth/login  →  API valida email/contraseña con BCrypt
                         →  Genera access_token (60 min) + refresh_token (7 días)
                         →  Almacena refresh_token en BD (tabla RefreshToken)
                         →  Setea ambas cookies httpOnly en el navegador

2. Requests autenticados →  Navegador envía cookie access_token automáticamente
                         →  Middleware JWT valida la firma y claims

3. Token expirado        →  Frontend llama POST /api/auth/refresh
                         →  API valida el refresh_token en BD
                         →  Rota: revoca el anterior, genera nuevo par
                         →  Setea las nuevas cookies

4. POST /api/auth/logout →  Revoca refresh_token en BD (BIT revocado=1)
                         →  Elimina ambas cookies del navegador
```

### Roles del sistema

| Rol | Descripción |
|-----|-------------|
| `superadmin` | Acceso total. Se crea vía `POST /api/auth/setup` (solo una vez) |
| `admin` | Gestiona usuarios, asigna planes, ve AuditLog |
| `auditor` | Solo lectura del AuditLog y su propio perfil |
| `usuario` | Consumidor: backups, alertas, recuperaciones, planes |

### Permisos por Rol

| Funcionalidad | Usuario | Auditor | Admin | SuperAdmin |
|---------------|:-------:|:-------:|:-----:|:----------:|
| Dashboard de backups | ✓ | — | — | ✓ |
| Jobs de Backup | ✓ | — | — | ✓ |
| Alertas | ✓ | — | — | ✓ |
| Cloud Storage | ✓ | — | — | ✓ |
| Políticas de Backup | ✓ | — | — | ✓ |
| Recuperaciones | ✓ | — | — | ✓ |
| Verificación Integridad | ✓ | — | — | ✓ |
| Mis Planes / Suscripción | ✓ | — | — | ✓ |
| Mi Perfil | ✓ | ✓ | ✓ | ✓ |
| Panel de Administración | — | — | ✓ | ✓ |
| Gestión de Usuarios | — | — | ✓ | ✓ |
| Cambiar Roles | — | — | ✓ | ✓ |
| Otorgar / Revocar Planes Gratis | — | — | ✓ | ✓ |
| Estadísticas del sistema | — | — | ✓ | ✓ |
| Ver Pagos | — | — | ✓ | ✓ |
| AuditLog | — | ✓ | ✓ | ✓ |
| Crear/Editar Planes | — | — | ✓ | ✓ |

> Los roles `admin` y `auditor` **no tienen acceso** a las funciones de backup (Jobs, Alertas, etc.). Son roles de gestión y auditoría exclusivamente.

---

## 7. Sistema de Planes

| Plan | Precio/mes | Almacenamiento | Jobs Simultáneos | Auto-Backup | Soporte |
|------|-----------|---------------|-----------------|-------------|---------|
| Gratuito | $0.00 | 5 GB | 2 | No | No |
| Basico | $9.99 | 50 GB | 5 | Sí | No |
| Pro | $29.99 | 500 GB | 20 | Sí | Sí |
| Empresa | $99.99 | 5 TB | 100 | Sí | Sí |
| Ilimitado | $0.00 (solo admin) | Sin límite | 999 | Sí | Sí |

### Flujo de Suscripción

```
Registro               →  Plan Gratuito asignado automáticamente (estado: "activa")
Usuario elige plan     →  Pago registrado → estado "activa"
Admin otorga plan      →  Sin pago → estado "gratis_admin"
Admin revoca plan      →  Suscripción cancelada → vuelve al Plan Gratuito
```

---

## 8. Variables de Entorno

### Archivo `.env` (raíz del proyecto — NO se sube al repositorio)

El repositorio incluye una plantilla `.env.example`. Antes de levantar los contenedores, copiala como `.env` y completá tus propios valores:

```bash
# Linux/Mac
cp .env.example .env

# Windows PowerShell
copy .env.example .env
```

```env
SA_PASSWORD=TuPasswordSeguraPropia123!
JWT_SECRET=TuSecretoAleatorioDeAlMenos32Caracteres
```

| Variable | Descripción | Requerimiento |
|----------|-------------|---------------|
| `SA_PASSWORD` | Contraseña del usuario `sa` de SQL Server | Mínimo 8 chars, mayúscula, número y símbolo especial |
| `JWT_SECRET` | Clave secreta para firmar tokens JWT | Mínimo 32 caracteres. Generar con `openssl rand -base64 48` |

> El archivo `.env` está en `.gitignore` — nunca se sube al repositorio. Cada persona que clone el proyecto debe crear el suyo con sus propios valores. **No reutilices los valores de ejemplo en producción.**

### Variables inyectadas a la API desde `docker-compose.yml`

```yaml
ASPNETCORE_ENVIRONMENT: Production
ConnectionStrings__DefaultConnection: "Server=db,1433;Database=n;User Id=sa;Password=${SA_PASSWORD};TrustServerCertificate=True;"
Jwt__Secret: "${JWT_SECRET}"
Jwt__Issuer: "BackUpNube"
Jwt__Audience: "BackUpNubeClient"
```

---

## 9. Despliegue con Docker — Paso a Paso

### Requisitos previos

- **Docker Engine** v24+ instalado y corriendo (`docker --version`)
- **Docker Compose** v2+ (`docker compose version`)
- Puertos **3001** y **5271** libres en el host
- Mínimo **4 GB RAM** asignados a Docker (SQL Server necesita al menos 2 GB)

### Paso 1 — Preparar el archivo `.env`

Cloná el repositorio y creá tu propio `.env` a partir de la plantilla `.env.example` (el `.env` real nunca se sube a GitHub):

```bash
git clone https://github.com/Aionos-Stom/BACK-UP-LA-NUBE-OFICIAL.git
cd BACK-UP-LA-NUBE-OFICIAL

# Linux/Mac
cp .env.example .env

# Windows
copy .env.example .env
```

Editá el `.env` recién creado y poné tus propios valores (ver Sección 8):
```env
SA_PASSWORD=TuPasswordSeguraPropia123!
JWT_SECRET=TuSecretoAleatorioDeAlMenos32Caracteres
```

### Paso 2 — Construir y levantar todos los contenedores

```bash
# Desde la carpeta raíz del proyecto
docker compose up --build -d
```

Docker ejecuta los servicios en este orden:

```
1. backup-db (SQL Server)
      ↓ espera healthcheck "SELECT 1" cada 10s
2. backup-db-init (one-shot)
      ↓ verifica si BD 'n' existe
      ↓ si no existe: CREATE DATABASE n + ejecuta DATABASE NUBE.sql
      ↓ si ya existe: no hace nada (idempotente)
3. backup-api (.NET 8)
      ↓ espera que db-init termine con exit code 0
4. backup-frontend (Nginx + React)
```

### Paso 3 — Verificar el estado

```bash
docker compose ps
```

Salida esperada:
```
NAME               STATUS              PORTS
backup-db          running (healthy)   1433/tcp
backup-db-init     exited (0)
backup-api         running             0.0.0.0:5271->8080/tcp
backup-frontend    running             0.0.0.0:3001->80/tcp
```

> `backup-db-init` con `exited (0)` es correcto — es un contenedor de un solo uso.  
> Si muestra `exited (1)`, ver logs: `docker compose logs db-init`

### Paso 4 — Crear el SuperAdmin (OBLIGATORIO, solo la primera vez)

El sistema **no tiene superadmin funcional** al iniciarse. Este paso es requerido para poder usar el sistema.

**Linux / Mac:**
```bash
curl -X POST http://localhost:5271/api/auth/setup \
  -H "Content-Type: application/json" \
  -d '{"email":"superadmin@tuempresa.com","password":"Admin2024!","nombre":"Super Administrador"}'
```

**Windows PowerShell:**
```powershell
Invoke-RestMethod -Uri "http://localhost:5271/api/auth/setup" `
  -Method POST `
  -ContentType "application/json" `
  -Body '{"email":"superadmin@tuempresa.com","password":"Admin2024!","nombre":"Super Administrador"}'
```

**Respuesta exitosa:**
```json
{
  "message": "SuperAdmin 'Super Administrador' creado exitosamente. Ya puedes iniciar sesión."
}
```

> Este endpoint se **desactiva automáticamente** cuando ya existe un superadmin en el sistema. Cualquier intento posterior devuelve:
> ```json
> {"message": "El sistema ya está configurado. Este endpoint está deshabilitado."}
> ```

### Paso 5 — Acceder a la aplicación

| Servicio | URL | Descripción |
|----------|-----|-------------|
| **Frontend** | http://localhost:3001 | Interfaz web completa |
| **API directa** | http://localhost:5271 | API REST (para pruebas con curl/Postman) |
| **Swagger UI** | http://localhost:5271/swagger | Solo si `ASPNETCORE_ENVIRONMENT: Development` |

### Habilitar Swagger en Docker (opcional)

Por defecto Swagger está deshabilitado en modo `Production`. Para activarlo durante desarrollo, cambiar en `docker-compose.yml`:

```yaml
api:
  environment:
    ASPNETCORE_ENVIRONMENT: Development   # cambiar de "Production" a "Development"
```

Luego reiniciar solo la API:
```bash
docker compose up -d --no-deps api
```

### Conectar SQL Server con SSMS o Azure Data Studio (opcional)

Por defecto el puerto 1433 NO está expuesto al host. Para acceder con una herramienta gráfica de BD, agregar en `docker-compose.yml`:

```yaml
services:
  db:
    ports:
      - "1433:1433"    # agregar esta línea
```

Reiniciar: `docker compose up -d db`

Datos de conexión:
- **Server:** `localhost,1433`
- **Authentication:** SQL Server Authentication
- **User:** `sa`
- **Password:** el valor que hayas puesto en `SA_PASSWORD` dentro de tu `.env`
- **Database:** `n`

### Comandos útiles

```bash
# Ver estado de todos los contenedores
docker compose ps

# Ver logs en tiempo real de todos los servicios
docker compose logs -f

# Ver logs de un servicio específico
docker compose logs -f api
docker compose logs -f db-init
docker compose logs -f frontend

# Reiniciar un servicio sin reconstruir
docker compose restart api

# Reconstruir y reiniciar un servicio específico (tras cambios en código)
docker compose up --build -d api
docker compose up --build -d frontend

# Detener sin borrar datos (volúmenes conservados)
docker compose down

# REINICIO COMPLETO — borra BD y recrea todo desde cero
docker compose down -v
docker compose up --build -d

# Ver uso de CPU/RAM de los contenedores
docker stats
```

### ¿Qué hacen los Dockerfiles?

**`BackUp.API/Dockerfile`** — Multi-stage build .NET 8:
```
Stage 1 (build):   mcr.microsoft.com/dotnet/sdk:8.0
                   → dotnet restore
                   → dotnet publish -c Release

Stage 2 (runtime): mcr.microsoft.com/dotnet/aspnet:8.0
                   → copia /app/publish
                   → EXPOSE 8080
                   → ENTRYPOINT dotnet BackUp.API.dll
```

**`AnBackUp/Dockerfile`** — Multi-stage build React:
```
Stage 1 (build):   node:20-alpine
                   → npm ci
                   → npx vite build  (genera /app/dist)

Stage 2 (runtime): nginx:alpine
                   → copia /app/dist a /usr/share/nginx/html
                   → copia nginx.conf
                   → EXPOSE 80
```

**`docker/init-db.sh`** — Script de inicialización (idempotente):
```bash
# 1. Espera a que SQL Server responda en :1433
# 2. Consulta sys.databases: ¿existe la BD 'n'?
# 3. Si no existe:
#    → CREATE DATABASE n
#    → Ejecuta DATABASE NUBE.sql (schema + semilla)
# 4. Si ya existe: no hace nada
```

### Proxy Nginx (`AnBackUp/nginx.conf`)

```nginx
server {
    listen 80;

    # Proxy de /api/ al contenedor de la API
    location /api/ {
        proxy_pass http://api:8080/api/;
        proxy_http_version 1.1;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_pass_header Set-Cookie;         # Pasa las cookies httpOnly al cliente
        proxy_cookie_path / "/; SameSite=Lax";
    }

    # SPA fallback para React Router
    location / {
        try_files $uri $uri/ /index.html;
    }
}
```

---

## 10. Desarrollo Local (sin Docker)

### Backend (.NET 8)

**Requisitos:** .NET 8 SDK, SQL Server local o en la red.

```bash
# Verificar versión
dotnet --version  # debe ser 8.x

# Correr la API (desde la carpeta raíz del proyecto)
dotnet run --project BackUp.API/BackUp.API.csproj

# La API queda disponible en:
# http://localhost:5271
# Swagger en: http://localhost:5271/swagger
```

La conexión en `BackUp.API/appsettings.json` apunta a `Server=localhost` con autenticación de Windows (Integrated Security). Cambiar el nombre del servidor al tuyo si usás una instancia con nombre (ej: `localhost\SQLEXPRESS`) o un servidor remoto:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=TU_SERVIDOR;Database=n;Integrated Security=True;TrustServerCertificate=True;"
}
```

Crear la BD ejecutando `DATABASE NUBE.sql` en SSMS o Azure Data Studio manualmente.

### Frontend (React + Vite)

**Requisitos:** Node.js 20+

```bash
cd AnBackUp
npm install
npm run dev

# Frontend disponible en: http://localhost:5173
```

---

## 11. API — Endpoints

> **Base URL:** `http://localhost:3001/api` (vía Nginx) o `http://localhost:5271/api` (directo)  
> **Autenticación:** Cookie httpOnly `access_token` enviada automáticamente por el navegador.

### Autenticación — `POST /api/auth/...`

| Método | Ruta | Auth | Descripción |
|--------|------|------|-------------|
| POST | `/setup` | No | Crear superadmin — solo funciona una vez |
| POST | `/register` | No | Registrar nuevo usuario — asigna Plan Gratuito |
| POST | `/login` | No | Iniciar sesión — setea cookies httpOnly |
| POST | `/logout` | Sí | Cerrar sesión — revoca refresh_token en BD |
| POST | `/refresh` | Cookie | Renovar access_token con rotación de refresh_token |
| GET | `/me` | Sí | Datos del usuario autenticado + plan activo |
| POST | `/change-password` | Sí | Cambiar contraseña (requiere contraseña actual) |

### Administración — `/api/admin` *(admin, superadmin)*

| Método | Ruta | Descripción |
|--------|------|-------------|
| GET | `/usuarios` | Lista todos los usuarios con su plan activo |
| PUT | `/usuarios/{id}/rol` | Cambiar el rol de un usuario |
| PUT | `/usuarios/{id}/activar` | Activar una cuenta deshabilitada |
| PUT | `/usuarios/{id}/desactivar` | Deshabilitar una cuenta |
| POST | `/usuarios/{id}/otorgar-plan-gratis` | Asignar plan sin cobro |
| POST | `/usuarios/{id}/revocar-plan-gratis` | Revocar plan gratuito, regresa al Plan Gratuito |
| GET | `/estadisticas` | Dashboard: usuarios, suscripciones, distribución por plan |
| GET | `/pagos` | Historial de pagos de todos los usuarios |

### Planes y Suscripciones — `/api/plan`

| Método | Ruta | Auth | Descripción |
|--------|------|------|-------------|
| GET | `/` | No | Lista planes disponibles |
| GET | `/{id}` | No | Detalle de un plan |
| POST | `/` | admin | Crear nuevo plan |
| PUT | `/{id}` | admin | Actualizar plan |
| POST | `/suscribir` | Sí | Suscribir al usuario a un plan |
| GET | `/mi-suscripcion` | Sí | Suscripción activa del usuario autenticado |

### Perfil — `/api/perfil`

| Método | Ruta | Descripción |
|--------|------|-------------|
| GET | `/` | Datos del perfil y plan activo |
| PUT | `/` | Actualizar nombre, teléfono y otros datos |
| GET | `/sesiones` | Lista refresh tokens activos del usuario |
| DELETE | `/sesiones/{id}` | Revocar una sesión específica |
| DELETE | `/sesiones` | Revocar todas las sesiones (cerrar sesión en todos los dispositivos) |
| GET | `/historial-pagos` | Pagos realizados por el usuario |

### Auditoría — `/api/audit` *(admin, superadmin)*

| Método | Ruta | Parámetros | Descripción |
|--------|------|------------|-------------|
| GET | `/` | `page`, `pageSize`, `accion`, `email` | Log de auditoría paginado y filtrable |

### Jobs de Backup — `/api/jobbackup`

| Método | Ruta | Descripción |
|--------|------|-------------|
| GET | `/` | Lista jobs (parámetros: `page`, `pageSize`, `estado`) |
| GET | `/{id}` | Detalle de un job |
| GET | `/programados` | Jobs con fecha de ejecución futura |
| POST | `/` | Crear nuevo job |
| PUT | `/` | Actualizar job |
| DELETE | `/{id}` | Eliminar job |
| POST | `/{id}/ejecutar` | Ejecutar job manualmente |

### Otros módulos

| Prefijo | Descripción |
|---------|-------------|
| `/api/alerta` | CRUD de alertas |
| `/api/cloudstorage` | Configuraciones de cloud storage |
| `/api/politicabackup` | Políticas de backup |
| `/api/recuperacion` | Procesos de recuperación |
| `/api/verificacionintegridad` | Verificaciones de integridad |
| `/api/notificaciones` | Contador y lista de notificaciones |
| `/api/dashboard` | Métricas generales (vistas SQL) |
| `/api/organizacion` | Gestión de organizaciones |
| `/api/backup` | Endpoint de creación de backup (usa sp_CrearBackup) |
| `/api/sesion` | Gestión de sesiones legacy |

---

## 12. Rutas del Frontend

| Ruta | Componente | Acceso |
|------|-----------|--------|
| `/login` | Login | Público |
| `/register` | Register | Público |
| `/` | Dashboard | Autenticado (usuario/superadmin) |
| `/jobs` | JobBackups | Autenticado |
| `/alertas` | Alertas | Autenticado |
| `/cloud-storage` | CloudStorage | Autenticado |
| `/politicas` | Politicas | Autenticado |
| `/recuperaciones` | Recuperaciones | Autenticado |
| `/verificacion-integridad` | VerificacionIntegridad | Autenticado |
| `/perfil` | Perfil | Autenticado |
| `/planes` | Planes | Autenticado |
| `/admin` | AdminDashboard | Admin / SuperAdmin / Auditor |
| `/admin/audit` | AuditLog | Admin / SuperAdmin / Auditor |
| `*` | NotFound (404) | Público |

> Los usuarios `admin` y `auditor` son redirigidos automáticamente a `/admin` al iniciar sesión.

---

## 13. Funcionalidades Implementadas

### Para Usuarios (rol `usuario` / `superadmin`)

- Registro con asignación automática de Plan Gratuito
- Login/logout con cookies httpOnly
- Dashboard con métricas de backup (KPIs, gráficos, últimos jobs)
- Gestión de Jobs de Backup (crear, editar, ejecutar, eliminar, filtrar por estado)
- Sistema de Alertas con notificaciones
- Cloud Storage — configuración de proveedores (AWS/Azure/GCP)
- Políticas de Backup — retención, frecuencia, ventana horaria
- Recuperaciones — iniciar y monitorear procesos de restauración
- Verificación de Integridad — validación de checksums SHA-256
- Mi Perfil — datos personales, sesiones activas con revocación individual/total, historial de pagos
- Planes — ver planes disponibles y gestionar suscripción

### Para Administradores (rol `admin` / `superadmin`)

- Panel de Administración con estadísticas globales
- Tabla de usuarios con acciones inline
- Cambio de roles con AuditLog automático
- Activar / Desactivar cuentas de usuario
- Otorgar planes gratuitos (con validación de duplicados)
- Revocar planes gratuitos
- Historial de pagos de todos los usuarios
- AuditLog con paginación y filtros por acción y email

### Para Auditores (rol `auditor`)

- AuditLog de solo lectura
- Mi Perfil

---

## 14. Informe de Ciberseguridad

### 14.1 Vulnerabilidades Encontradas y Corregidas

#### CRÍTICAS (CVSS 9.0–10.0)

| # | Vulnerabilidad | Descripción | Estado |
|---|---------------|-------------|--------|
| C-01 | Sin autenticación en endpoints | Todos los controladores eran accesibles sin token | **CORREGIDO** |
| C-02 | Tokens JWT en localStorage | Expuestos a XSS — cualquier script malicioso podía robarlos | **CORREGIDO** |
| C-03 | Sin validación de roles | No existía jerarquía — todos los usuarios tenían los mismos permisos | **CORREGIDO** |
| C-04 | Hash de contraseña débil | MD5/SHA1 sin salt — reversible con rainbow tables | **CORREGIDO** |
| C-05 | Superadmin hardcodeado en SQL | Hash del superadmin expuesto en el repositorio | **CORREGIDO** |
| C-06 | CORS inválido | `AllowAnyOrigin()` + `AllowCredentials()` — expone cookies a cualquier origen | **CORREGIDO** |

#### ALTAS (CVSS 7.0–8.9)

| # | Vulnerabilidad | Descripción | Estado |
|---|---------------|-------------|--------|
| A-01 | Filtros fake en RecuperacionController | Los 3 métodos de filtrado ignoraban el parámetro y devolvían TODOS los registros | **CORREGIDO** |
| A-02 | Cast dinámico en VerificacionIntegridadController | Acceso a repo privado vía `dynamic` causaba crash en runtime | **CORREGIDO** |
| A-03 | Sin Refresh Token | Al expirar el JWT el usuario perdía la sesión sin renovación | **CORREGIDO** |
| A-04 | Sin protección CSRF | Rutas de mutación sin protección Cross-Site Request Forgery | **PARCIAL** (SameSite=Lax mitiga la mayoría) |
| A-05 | Sin rate limiting | Endpoints de auth atacables con fuerza bruta sin límite | **CORREGIDO** |
| A-06 | Puerto SQL Server expuesto | Puerto 1433 mapeado al host, accesible desde la red local | **CORREGIDO** |
| A-07 | Colisión de serialización JSON | `NotificacionController` producía JSON con claves duplicadas → errores 500 | **CORREGIDO** |

#### MEDIAS (CVSS 4.0–6.9)

| # | Vulnerabilidad | Descripción | Estado |
|---|---------------|-------------|--------|
| M-01 | Sin headers de seguridad HTTP | Ausencia de `X-Frame-Options`, `X-Content-Type-Options`, etc. | **CORREGIDO** |
| M-02 | Paginación rota en JobBackup | Frontend esperaba array, API devolvía objeto paginado → `TypeError: e.map is not a function` | **CORREGIDO** |
| M-03 | Claim `sub` no mapeado | `userId` siempre era 0 — todos los endpoints autenticados actuaban como usuario 0 | **CORREGIDO** |
| M-04 | Sin AuditLog | No había registro de acciones críticas | **CORREGIDO** |
| M-05 | Sin gestión de sesiones | Token robado permanecía válido hasta su expiración natural | **CORREGIDO** |
| M-06 | Menú sin restricción de roles | Usuarios admin veían menú de backup (no les corresponde) | **CORREGIDO** |
| M-07 | Planes duplicables | Admin podía asignar el mismo plan múltiples veces | **CORREGIDO** |

### 14.2 Implementaciones de Seguridad

#### JWT en Cookies httpOnly
```
access_token:   httpOnly, SameSite=Lax, Path=/,        expira 60 min
refresh_token:  httpOnly, SameSite=Lax, Path=/api/auth, expira 7 días
```

#### Hash de Contraseñas
```csharp
BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12)
// ~250ms por hash — resistente a fuerza bruta
// Salt automático incluido en el hash
```

#### Security Headers en cada respuesta
```
X-Content-Type-Options: nosniff
X-Frame-Options: DENY
X-XSS-Protection: 1; mode=block
Referrer-Policy: strict-origin-when-cross-origin
Permissions-Policy: camera=(), microphone=(), geolocation=()
```

#### Rate Limiting
```
Auth endpoints:    5 peticiones / minuto por IP
API general:      100 peticiones / minuto por IP
HTTP 429 si se excede el límite
```

#### AuditLog — Acciones registradas
`LOGIN`, `LOGOUT`, `REGISTER`, `CAMBIAR_ROL`, `ACTIVAR_USUARIO`, `DESACTIVAR_USUARIO`, `OTORGAR_PLAN_GRATIS`, `REVOCAR_PLAN_GRATIS`, `ACTUALIZAR_PERFIL`, `REVOCAR_TODAS_SESIONES`, `CAMBIO_CONTRASEÑA`

---

## 15. Bugs Críticos Corregidos

### 1. `dynamic` cast en VerificacionIntegridadController
```csharp
// Antes (RuntimeBinderException en producción):
dynamic repo = _service;
var result = repo.GetByJobId(jobId); // crash

// Después (EF Core directo):
var verificaciones = await _context.VerificacionIntegridad
    .Where(v => v.job_id == jobId)
    .OrderByDescending(v => v.FechaVerificacion)
    .ToListAsync();
```

### 2. Filtros fake en RecuperacionController
```csharp
// Antes (ignoraba el filtro, devolvía todo):
var recs = await _recuperacionService.ObtenerTodosAsync();

// Después (filtra correctamente con WHERE):
var recs = await _context.Recuperacion
    .Where(r => r.UsuarioId == usuarioId)
    .OrderByDescending(r => r.Id)
    .ToListAsync();
```

### 3. Claim `sub` no mapeado → userId = 0

ASP.NET Core mapea automáticamente `sub` → `ClaimTypes.NameIdentifier`. Usar `"sub"` directamente siempre devuelve null.

```csharp
// Antes (retornaba 0 siempre):
var userId = int.Parse(User.FindFirstValue("sub") ?? "0");

// Después (correcto):
var userId = int.Parse(
    User.FindFirstValue(ClaimTypes.NameIdentifier) ??
    User.FindFirstValue("sub") ?? "0"
);
```

### 4. Colisión JSON en NotificacionController
```csharp
// Antes (claves duplicadas → excepción 500):
.Select(a => new { a.Tipo, tipo = "alerta" })

// Después (claves distintas):
.Select(a => new { tipoAlerta = a.Tipo, categoria = "alerta" })
```

### 5. `TypeError: e.map is not a function` en Jobs

`JobBackupController` devuelve objeto paginado `{total, page, pageSize, data:[...]}` pero el frontend llamaba `.map()` sobre el objeto raíz.

```typescript
// Después (extrae el array del objeto paginado):
const response = await api.get('/jobbackup');
const data = response.data?.data ?? response.data;
```

---

## 16. Mejoras Futuras Recomendadas

### Seguridad (Alta Prioridad)

- [ ] **HTTPS / TLS** — Configurar SSL en Nginx. Cambiar `Secure=false` → `Secure=true` en cookies.
- [ ] **MFA (TOTP)** — La columna `mfa_habilitado` ya existe. Implementar el flujo con Google Authenticator.
- [ ] **CSRF Token explícito** — Implementar `AntiForgery` para endpoints de mutación.
- [ ] **Secretos en Vault** — Mover JWT secret y connection string a HashiCorp Vault o Azure Key Vault.

### Funcionalidades (Media Prioridad)

- [ ] **Notificaciones en tiempo real** — WebSocket o Server-Sent Events (reemplazar polling de 30s).
- [ ] **Pasarela de Pago** — Integrar Stripe o PayPal.
- [ ] **Verificación de Email** — Confirmación al registrarse.
- [ ] **Recuperación de Contraseña** — Flujo con token temporal por email.
- [ ] **Exportación AuditLog** — Descarga en CSV/PDF.

### Infraestructura (Media Prioridad)

- [ ] **Logging estructurado** — Serilog con sink a archivo o Seq.
- [ ] **Health checks** — Endpoints `/health` y `/ready`.
- [ ] **CI/CD** — GitHub Actions: build, test, push a registry, deploy.
- [ ] **Backup de la BD** — Script automático del volumen de SQL Server.
- [ ] **Redis** — Cache y rate limiting distribuido (actualmente en memoria).

---

## Contacto y Soporte

Para errores del sistema:
```bash
docker compose logs -f api        # logs de la API
docker compose logs -f db-init    # logs de inicialización de BD
docker compose logs -f frontend   # logs de Nginx

# Reinicio completo (borra todos los datos):
docker compose down -v && docker compose up --build -d
```

---

*Versión: 2.0.0 — Actualizado: Julio 2026*  
*Stack: React 18 + TypeScript + ASP.NET Core 8 + SQL Server 2022 + Docker*
