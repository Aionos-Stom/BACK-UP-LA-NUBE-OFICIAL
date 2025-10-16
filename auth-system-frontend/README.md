# Auth System Frontend

Una aplicación frontend desarrollada con Angular 20 que proporciona una interfaz de usuario moderna y responsiva para un sistema de autenticación completo.

## 🚀 Características

- **Autenticación JWT**: Sistema completo de login y registro
- **Interfaz moderna**: Diseño responsivo con Angular Material
- **Guards de autenticación**: Protección de rutas
- **Servicios HTTP**: Comunicación con el backend API
- **Validación de formularios**: Validación reactiva con Angular Forms
- **Dashboard protegido**: Área privada para usuarios autenticados

## 📋 Prerrequisitos

Antes de comenzar, asegúrate de tener instalado:

- **Node.js** (versión 18 o superior)
- **npm** (viene con Node.js)
- **Angular CLI** (versión 20 o superior)

### Instalación de Angular CLI

```bash
npm install -g @angular/cli
```

## 🛠️ Instalación

1. **Clona el repositorio** (si no lo has hecho ya):
```bash
git clone <url-del-repositorio>
cd auth-system-frontend
```

2. **Instala las dependencias**:
```bash
npm install
```

## 🏃‍♂️ Ejecución

### Modo de desarrollo

```bash
npm start
# o
ng serve
```

La aplicación estará disponible en `http://localhost:4200`

### Compilación para producción

```bash
npm run build
# o
ng build --configuration production
```

Los archivos compilados se generarán en la carpeta `dist/`.

### Ejecutar pruebas

```bash
npm test
# o
ng test
```

## 📁 Estructura del Proyecto

```
src/
├── app/
│   ├── components/          # Componentes de la aplicación
│   │   ├── dashboard/      # Dashboard principal
│   │   ├── login/          # Componente de inicio de sesión
│   │   └── register/       # Componente de registro
│   ├── guards/             # Guards de navegación
│   │   └── auth.guard.ts   # Guard de autenticación
│   ├── models/             # Interfaces y modelos TypeScript
│   │   └── user.model.ts   # Modelo de usuario
│   ├── services/           # Servicios de la aplicación
│   │   └── auth.service.ts # Servicio de autenticación
│   ├── app.config.ts       # Configuración de la aplicación
│   ├── app.routes.ts       # Configuración de rutas
│   ├── app.html            # Template principal
│   └── app.ts              # Componente raíz
├── styles.css              # Estilos globales
└── index.html              # Página principal HTML
```

## 🔧 Configuración

### Variables de Entorno

Para configurar la conexión con el backend, puedes crear un archivo de configuración de entorno:

1. Crea `src/environments/environment.ts`:
```typescript
export const environment = {
  production: false,
  apiUrl: 'https://localhost:7000/api' // URL del backend
};
```

2. Crea `src/environments/environment.prod.ts`:
```typescript
export const environment = {
  production: true,
  apiUrl: 'https://tu-dominio.com/api' // URL del backend en producción
};
```

### Configuración del Backend

Asegúrate de que el backend esté ejecutándose en la URL configurada. Por defecto, el backend debería estar en:
- **Desarrollo**: `https://localhost:7000`
- **Producción**: Tu dominio de producción

## 🔐 Funcionalidades de Autenticación

### Componentes Principales

1. **Login Component** (`src/app/components/login/`)
   - Formulario de inicio de sesión
   - Validación de credenciales
   - Manejo de errores

2. **Register Component** (`src/app/components/register/`)
   - Formulario de registro de usuario
   - Validación de datos
   - Confirmación de contraseña

3. **Dashboard Component** (`src/app/components/dashboard/`)
   - Área protegida para usuarios autenticados
   - Información del usuario
   - Opción de cerrar sesión

### Servicios

1. **AuthService** (`src/app/services/auth.service.ts`)
   - Métodos para login y registro
   - Manejo de tokens JWT
   - Almacenamiento en localStorage
   - Verificación de autenticación

### Guards

1. **AuthGuard** (`src/app/guards/auth.guard.ts`)
   - Protección de rutas privadas
   - Verificación de tokens válidos
   - Redirección a login si no está autenticado

## 🎨 Estilos y Diseño

La aplicación utiliza:
- **Angular Material**: Para componentes UI modernos
- **CSS personalizado**: Para estilos específicos
- **Diseño responsivo**: Compatible con móviles y tablets

## 🔗 Conexión con el Backend

### Configuración de CORS

El backend debe tener configurado CORS para permitir peticiones desde `http://localhost:4200` en desarrollo.

### Endpoints Utilizados

- `POST /api/auth/login` - Inicio de sesión
- `POST /api/auth/register` - Registro de usuario
- `GET /api/auth/me` - Obtener información del usuario actual

### Manejo de Tokens

Los tokens JWT se almacenan en `localStorage` y se incluyen automáticamente en las peticiones HTTP mediante interceptores.

## 🚀 Despliegue

### Preparación para Producción

1. **Actualiza la configuración**:
   - Modifica `environment.prod.ts` con la URL correcta del backend
   - Verifica la configuración de CORS en el backend

2. **Compila la aplicación**:
```bash
ng build --configuration production
```

3. **Despliega los archivos**:
   - Los archivos en `dist/` pueden ser servidos por cualquier servidor web estático
   - Recomendado: Nginx, Apache, o servicios como Vercel, Netlify

### Ejemplo de Nginx

```nginx
server {
    listen 80;
    server_name tu-dominio.com;
    root /path/to/dist;
    index index.html;

    location / {
        try_files $uri $uri/ /index.html;
    }
}
```

## 🧪 Pruebas

La aplicación incluye pruebas unitarias con Jasmine y Karma:

```bash
# Ejecutar todas las pruebas
ng test

# Ejecutar pruebas con coverage
ng test --code-coverage
```

## 📦 Dependencias Principales

- **@angular/core**: Framework principal de Angular
- **@angular/material**: Componentes UI de Material Design
- **@angular/forms**: Manejo de formularios
- **@angular/router**: Sistema de enrutamiento
- **rxjs**: Programación reactiva

## 🤝 Contribución

1. Fork el proyecto
2. Crea una rama para tu feature (`git checkout -b feature/AmazingFeature`)
3. Commit tus cambios (`git commit -m 'Add some AmazingFeature'`)
4. Push a la rama (`git push origin feature/AmazingFeature`)
5. Abre un Pull Request

## 📝 Notas de Desarrollo

- **Angular 20**: Utiliza la versión más reciente de Angular
- **TypeScript**: Todo el código está tipado
- **Lazy Loading**: Las rutas están configuradas para carga perezosa
- **Error Handling**: Manejo robusto de errores en toda la aplicación

## 🐛 Solución de Problemas

### Problemas Comunes

1. **Error de CORS**:
   - Verifica que el backend tenga configurado CORS para `http://localhost:4200`
   - Revisa la configuración en `Program.cs` del backend

2. **Error de conexión al backend**:
   - Verifica que el backend esté ejecutándose
   - Comprueba la URL en `environment.ts`
   - Revisa la configuración de red

3. **Problemas de autenticación**:
   - Verifica que el token se esté almacenando correctamente
   - Revisa la configuración JWT en el backend
   - Comprueba la expiración del token

## 📞 Soporte

Si tienes problemas o preguntas:
1. Revisa la documentación del backend
2. Verifica los logs del navegador (F12)
3. Consulta la documentación de Angular
4. Abre un issue en el repositorio

---

**Versión**: 0.0.0  
**Framework**: Angular 20  
**Última actualización**: Enero 2025