# Docker — BackUp La Nube

Guía para ejecutar el proyecto completo en contenedores con Docker Desktop.

## ¿Qué incluye?

| Contenedor | Imagen / build | Puerto | Descripción |
|------------|----------------|--------|-------------|
| `backup-db` | SQL Server 2022 | `1433` | Base de datos |
| `backup-db-init` | (temporal) | — | Crea la BD y ejecuta `DATABASE NUBE.sql` |
| `backup-api` | .NET 8 (`BackUp.API`) | `5271` | API REST + Swagger |
| `backup-frontend` | React + Nginx (`AnBackUp`) | `3000` | Dashboard web |

```
Navegador → Frontend (Nginx :3000) → API (.NET :5271) → SQL Server (:1433)
```

## Prerrequisitos

- [Docker Desktop](https://www.docker.com/products/docker-desktop/) instalado y **en ejecución**
- En Windows: **WSL 2** habilitado (lo configura el instalador de Docker)

Verifica la instalación:

```bash
docker --version
docker compose version
```

No necesitas instalar .NET, Node.js ni SQL Server en tu máquina para usar Docker.

---

## Inicio rápido

```bash
# 1. Clonar e ir al proyecto
git clone <url-del-repositorio>
cd BACK-UP-LA-NUBE-OFICIAL-main

# 2. Crear archivo de entorno (solo la primera vez)
copy .env.example .env

# 3. Levantar todo
docker compose up -d
```

La primera ejecución puede tardar **5–10 minutos** (descarga de imágenes + inicialización de SQL Server).

### URLs

| Servicio | URL |
|----------|-----|
| Frontend | http://localhost:3000 |
| API / Swagger | http://localhost:5271/swagger |
| SQL Server | `localhost:1433` (usuario `sa`, contraseña en `.env`) |

---

## Configuración (`.env`)

Copia `.env.example` a `.env`:

```env
SA_PASSWORD=BackUp_Nube2024!
```

La contraseña debe cumplir los requisitos de SQL Server:

- Mínimo 8 caracteres
- Mayúscula, minúscula, número y símbolo

> **Importante:** No subas `.env` a git. Ya está incluido en `.gitignore`.

---

## Comandos útiles

```bash
# Ver estado de los contenedores
docker compose ps

# Ver logs de todos los servicios
docker compose logs -f

# Logs de un servicio concreto
docker compose logs -f api
docker compose logs db-init

# Reconstruir imágenes tras cambios en el código
docker compose up -d --build

# Reiniciar servicios
docker compose restart

# Detener contenedores (conserva la BD)
docker compose down

# Detener y borrar la base de datos (reinicio limpio)
docker compose down -v
docker compose up -d
```

---

## Archivos Docker del proyecto

```
BACK-UP-LA-NUBE-OFICIAL-main/
├── docker-compose.yml          # Orquestación de servicios
├── .env                        # Contraseña SQL Server (local, no en git)
├── .env.example                # Plantilla de variables
├── .dockerignore
├── DATABASE NUBE.sql           # Esquema y datos iniciales
├── docker/
│   └── init-db.sh              # Script de inicialización de BD
├── BackUp.API/
│   └── Dockerfile              # Imagen de la API .NET 8
└── AnBackUp/
    ├── Dockerfile              # Build React + Nginx
    └── nginx.conf              # Proxy /api → contenedor api
```

### Inicialización de la base de datos

El contenedor `db-init` se ejecuta **una sola vez** al arrancar:

1. Espera a que SQL Server esté listo (`healthcheck`)
2. Comprueba si la base de datos `n` ya existe
3. Si no existe, la crea y ejecuta `DATABASE NUBE.sql`

Para forzar una reinicialización completa:

```bash
docker compose down -v
docker compose up -d
```

---

## Desarrollo con VS Code

1. Abre la carpeta del proyecto en VS Code.
2. Instala las extensiones **Docker** y **C# Dev Kit**.
3. En terminal integrada:

```bash
docker compose up -d
```

### Solo base de datos en Docker (depurar API/frontend en local)

```bash
# Levantar únicamente SQL Server
docker compose up db -d

# API en local (otra terminal)
dotnet run --project BackUp.API

# Frontend en local (otra terminal)
cd AnBackUp
npm install
npm run dev
```

Connection string para la API local apuntando al contenedor:

```
Server=localhost,1433;Database=n;User Id=sa;Password=BackUp_Nube2024!;TrustServerCertificate=True;
```

### Evitar conflictos de puertos

No ejecutes la misma API o frontend **a la vez** en Docker y en local:

| Puerto | Servicio |
|--------|----------|
| `3000` | Frontend |
| `5271` | API |
| `1433` | SQL Server |

---

## Cómo verificar que funciona

### 1. Contenedores activos

```bash
docker compose ps
```

Resultado esperado:

- `backup-db` → **Up (healthy)**
- `backup-api` → **Up**
- `backup-frontend` → **Up**

### 2. API responde

Abre http://localhost:5271/swagger y prueba **GET `/api/dashboard`**.

Respuesta esperada: **200** con JSON (puede estar vacío si no hay datos de prueba).

### 3. Frontend carga

Abre http://localhost:3000. Debe mostrarse el dashboard y el menú lateral.

### 4. Frontend conectado a la API

En el navegador: **F12 → Network → recargar**.

Las peticiones a `/api/*` deben devolver **200**.

> Métricas en cero o tablas vacías **no significa que esté roto**; indica que la BD no tiene datos de prueba. Usa `docker compose down -v` y `docker compose up -d` para reinicializar.

---

## Solución de problemas

### Docker Desktop no responde

Abre Docker Desktop y espera a que el icono indique que está en ejecución antes de usar `docker compose`.

### Puerto 3000, 5271 o 1433 ocupado

```bash
docker compose down
```

Cierra procesos locales que usen esos puertos (otra instancia de la API, `npm run dev`, etc.).

### `backup-db` no pasa a healthy

- SQL Server necesita al menos **2 GB de RAM** asignados a Docker.
- En Docker Desktop: **Settings → Resources** → aumenta memoria si es necesario.
- Revisa logs: `docker compose logs db`

### Error en `db-init`

```bash
docker compose logs db-init
```

Si la BD quedó a medias, reinicia con volumen limpio:

```bash
docker compose down -v
docker compose up -d
```

### API responde 500

- Comprueba que `db-init` terminó correctamente.
- Revisa logs de la API: `docker compose logs api`
- Verifica que `.env` existe y `SA_PASSWORD` cumple los requisitos.

### Frontend carga pero no hay datos

1. Comprueba Swagger: http://localhost:5271/swagger
2. Revisa F12 → Network en el navegador
3. Reinicializa la BD: `docker compose down -v && docker compose up -d`

### Cambios en el código no se reflejan

Reconstruye las imágenes:

```bash
docker compose up -d --build
```

---

## Detalles técnicos

### API

- Imagen base: `mcr.microsoft.com/dotnet/aspnet:8.0`
- Puerto interno: `8080` → expuesto como `5271`
- Connection string inyectada por variable de entorno en `docker-compose.yml`
- Swagger habilitado en entorno `Development`

### Frontend

- Build con Node 20 + Vite
- Servido con Nginx en producción
- Nginx hace proxy de `/api/` hacia `http://api:8080/api/`

### Red Docker

Todos los servicios comparten la red por defecto de Compose. El frontend resuelve el hostname `api` y la API resuelve `db`.

---

**Última actualización:** Junio 2025
