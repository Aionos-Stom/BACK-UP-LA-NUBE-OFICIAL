# AnBackUp Dashboard

Dashboard frontend moderno para el sistema de backup AnBackUp, construido con React, TypeScript y Material-UI.

## Características

- 📊 **Dashboard Principal**: Métricas en tiempo real, gráficos y visualización de backups recientes
- 🔄 **Gestión de Jobs de Backup**: Crear, editar, ejecutar y eliminar jobs de backup
- ⚠️ **Sistema de Alertas**: Monitoreo y gestión de alertas con diferentes niveles de severidad
- ☁️ **Cloud Storage**: Administración de proveedores de almacenamiento en la nube
- 📋 **Políticas de Backup**: Configuración y gestión de políticas de backup
- 🔄 **Recuperaciones**: Gestión de procesos de recuperación con simulación
- ✅ **Verificación de Integridad**: Verificación y monitoreo de integridad de backups

## Tecnologías

- **React 18** - Biblioteca de UI
- **TypeScript** - Tipado estático
- **Material-UI (MUI)** - Componentes de UI
- **React Router** - Enrutamiento
- **Axios** - Cliente HTTP
- **Recharts** - Gráficos y visualizaciones
- **Vite** - Build tool y dev server
- **date-fns** - Manejo de fechas

## Instalación

1. Instalar dependencias:
```bash
npm install
```

2. Configurar la URL del backend en `vite.config.ts` (por defecto: `http://localhost:5271`)

3. Iniciar el servidor de desarrollo:
```bash
npm run dev
```

4. Abrir en el navegador: `http://localhost:3000`

## Estructura del Proyecto

```
AnBackUp/
├── src/
│   ├── components/      # Componentes reutilizables
│   │   └── Layout.tsx   # Layout principal con navegación
│   ├── pages/           # Páginas principales
│   │   ├── Dashboard.tsx
│   │   ├── JobBackups.tsx
│   │   ├── Alertas.tsx
│   │   ├── CloudStorage.tsx
│   │   ├── Politicas.tsx
│   │   ├── Recuperaciones.tsx
│   │   └── VerificacionIntegridad.tsx
│   ├── services/        # Servicios API
│   │   └── api.ts       # Cliente API y endpoints
│   ├── types/           # Definiciones TypeScript
│   │   └── index.ts     # Interfaces y tipos
│   ├── App.tsx          # Componente principal
│   └── main.tsx         # Punto de entrada
├── index.html
├── package.json
├── tsconfig.json
└── vite.config.ts
```

## API Endpoints

El frontend se conecta a los siguientes endpoints del backend:

- `/api/dashboard` - Dashboard y métricas
- `/api/jobbackup` - Gestión de jobs de backup
- `/api/alerta` - Gestión de alertas
- `/api/cloudstorage` - Gestión de cloud storage
- `/api/politicabackup` - Gestión de políticas
- `/api/recuperacion` - Gestión de recuperaciones
- `/api/verificacionintegridad` - Verificación de integridad
- `/api/backup` - Operaciones de backup

## Scripts Disponibles

- `npm run dev` - Inicia el servidor de desarrollo
- `npm run build` - Construye la aplicación para producción
- `npm run preview` - Previsualiza el build de producción
- `npm run lint` - Ejecuta el linter

## Configuración del Backend

Asegúrate de que el backend esté corriendo en `http://localhost:5271` (o actualiza la configuración del proxy en `vite.config.ts`).

El backend debe tener habilitado CORS para permitir las peticiones desde el frontend.

## Características del Dashboard

### Dashboard Principal
- Métricas en tiempo real (almacenamiento, backups, tasa de éxito)
- Gráficos de backups recientes
- Distribución por proveedor (gráfico de pastel)
- Tabla de proveedores de storage con barras de progreso
- Lista de backups recientes

### Gestión de Jobs
- Crear, editar y eliminar jobs
- Ejecutar jobs manualmente
- Visualización de estado, fechas y tamaños
- Filtrado y búsqueda

### Sistema de Alertas
- Contador de alertas no reconocidas
- Filtrado por severidad, usuario, job
- Marcar alertas como reconocidas
- Gestión completa de alertas

### Cloud Storage
- Gestión de proveedores
- Visualización de uso con barras de progreso
- Estados y capacidades

### Políticas de Backup
- Crear y editar políticas
- Activar/desactivar políticas
- Configuración de frecuencia, retención, compresión, encriptación

### Recuperaciones
- Crear nuevas recuperaciones
- Ejecutar y simular recuperaciones
- Monitoreo de progreso
- Gestión de errores

### Verificación de Integridad
- Verificar integridad de backups
- Visualización de hashes
- Resultados de verificación

## Desarrollo

El proyecto usa Vite como build tool, que proporciona:
- Hot Module Replacement (HMR) rápido
- Build optimizado para producción
- Proxy configurado para desarrollo

## Notas

- El frontend está configurado para conectarse al backend en `http://localhost:5271`
- Todas las peticiones pasan por el proxy configurado en Vite
- El proyecto usa TypeScript estricto para mayor seguridad de tipos

