import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { ThemeProvider, createTheme } from '@mui/material/styles';
import CssBaseline from '@mui/material/CssBaseline';
import { CircularProgress, Box } from '@mui/material';
import { AuthProvider, useAuth } from './contexts/AuthContext';
import { ToastProvider } from './components/Toast';
import Layout from './components/Layout';
import Dashboard from './pages/Dashboard';
import JobBackups from './pages/JobBackups';
import Alertas from './pages/Alertas';
import CloudStorage from './pages/CloudStorage';
import Politicas from './pages/Politicas';
import Recuperaciones from './pages/Recuperaciones';
import VerificacionIntegridad from './pages/VerificacionIntegridad';
import Login from './pages/Login';
import Register from './pages/Register';
import AdminDashboard from './pages/admin/AdminDashboard';
import AuditLogPage from './pages/admin/AuditLog';
import Perfil from './pages/Perfil';
import Planes from './pages/Planes';
import NotFound from './pages/NotFound';

const theme = createTheme({
  palette: {
    mode: 'dark',
    primary: { main: '#00d4ff', light: '#5dffff', dark: '#00a2cc' },
    secondary: { main: '#7c3aed', light: '#a78bfa', dark: '#5b21b6' },
    background: { default: '#0a0e27', paper: '#0f1629' },
    text: { primary: '#ffffff', secondary: '#b0d4ff' },
    success: { main: '#00ff88' },
    warning: { main: '#ffb800' },
    error: { main: '#ff006e' },
  },
  typography: {
    fontFamily: [
      '-apple-system', 'BlinkMacSystemFont', '"Segoe UI"', 'Roboto',
      '"Helvetica Neue"', 'Arial', 'sans-serif',
    ].join(','),
    h4: {
      fontWeight: 700,
      background: 'linear-gradient(135deg, #00d4ff 0%, #7c3aed 100%)',
      WebkitBackgroundClip: 'text',
      WebkitTextFillColor: 'transparent',
    },
    h6: { fontWeight: 600 },
  },
  components: {
    MuiCard: {
      styleOverrides: {
        root: {
          background: 'linear-gradient(135deg, rgba(15, 22, 41, 0.9) 0%, rgba(10, 14, 39, 0.9) 100%)',
          border: '1px solid rgba(0, 212, 255, 0.2)',
          borderRadius: '16px',
          boxShadow: '0 8px 32px rgba(0, 212, 255, 0.1), 0 0 20px rgba(124, 58, 237, 0.1)',
          transition: 'all 0.3s ease',
          '&:hover': {
            borderColor: 'rgba(0, 212, 255, 0.4)',
            boxShadow: '0 8px 32px rgba(0, 212, 255, 0.2), 0 0 30px rgba(124, 58, 237, 0.2)',
            transform: 'translateY(-2px)',
          },
        },
      },
    },
    MuiPaper: {
      styleOverrides: {
        root: {
          background: 'linear-gradient(135deg, rgba(15, 22, 41, 0.95) 0%, rgba(10, 14, 39, 0.95) 100%)',
          border: '1px solid rgba(0, 212, 255, 0.2)',
          borderRadius: '16px',
          boxShadow: '0 8px 32px rgba(0, 212, 255, 0.1)',
        },
      },
    },
    MuiButton: {
      styleOverrides: {
        root: {
          borderRadius: '12px',
          textTransform: 'none',
          fontWeight: 600,
          padding: '10px 24px',
          transition: 'all 0.3s ease',
        },
        contained: {
          background: 'linear-gradient(135deg, #00d4ff 0%, #7c3aed 100%)',
          boxShadow: '0 4px 20px rgba(0, 212, 255, 0.4)',
          '&:hover': {
            background: 'linear-gradient(135deg, #5dffff 0%, #a78bfa 100%)',
            boxShadow: '0 6px 30px rgba(0, 212, 255, 0.6)',
            transform: 'translateY(-2px)',
          },
        },
      },
    },
    MuiChip: {
      styleOverrides: {
        root: { borderRadius: '8px', fontWeight: 500 },
      },
    },
    MuiAppBar: {
      styleOverrides: {
        root: {
          background: 'linear-gradient(135deg, rgba(15, 22, 41, 0.95) 0%, rgba(10, 14, 39, 0.95) 100%)',
          borderBottom: '1px solid rgba(0, 212, 255, 0.2)',
          boxShadow: '0 4px 20px rgba(0, 212, 255, 0.1)',
        },
      },
    },
    MuiDrawer: {
      styleOverrides: {
        paper: {
          background: 'linear-gradient(180deg, rgba(15, 22, 41, 0.98) 0%, rgba(10, 14, 39, 0.98) 100%)',
          borderRight: '1px solid rgba(0, 212, 255, 0.2)',
        },
      },
    },
  },
});

function LoadingScreen() {
  return (
    <Box sx={{
      display: 'flex',
      justifyContent: 'center',
      alignItems: 'center',
      height: '100vh',
      bgcolor: '#0a0e27'
    }}>
      <CircularProgress sx={{ color: '#00d4ff' }} />
    </Box>
  );
}

function ProtectedRoute({ children }: { children: React.ReactNode }) {
  const { usuario, isLoading } = useAuth();
  if (isLoading) return <LoadingScreen />;
  if (!usuario) return <Navigate to="/login" replace />;
  return <>{children}</>;
}

function AdminRoute({ children }: { children: React.ReactNode }) {
  const { usuario, isAdmin, isLoading } = useAuth();
  if (isLoading) return <LoadingScreen />;
  if (!usuario) return <Navigate to="/login" replace />;
  if (!isAdmin && usuario.rol !== 'auditor') return <Navigate to="/" replace />;
  return <>{children}</>;
}

function HomeRoute() {
  const { usuario, isAdmin, isLoading } = useAuth();
  if (isLoading) return <LoadingScreen />;
  if (!usuario) return <Navigate to="/login" replace />;
  if (isAdmin || usuario.rol === 'auditor') return <Navigate to="/admin" replace />;
  return <Layout><Dashboard /></Layout>;
}

function AppRoutes() {
  return (
    <Routes>
      <Route path="/login" element={<Login />} />
      <Route path="/register" element={<Register />} />
      <Route path="/" element={<HomeRoute />} />
      <Route path="/jobs" element={<ProtectedRoute><Layout><JobBackups /></Layout></ProtectedRoute>} />
      <Route path="/alertas" element={<ProtectedRoute><Layout><Alertas /></Layout></ProtectedRoute>} />
      <Route path="/cloud-storage" element={<ProtectedRoute><Layout><CloudStorage /></Layout></ProtectedRoute>} />
      <Route path="/politicas" element={<ProtectedRoute><Layout><Politicas /></Layout></ProtectedRoute>} />
      <Route path="/recuperaciones" element={<ProtectedRoute><Layout><Recuperaciones /></Layout></ProtectedRoute>} />
      <Route path="/verificacion-integridad" element={<ProtectedRoute><Layout><VerificacionIntegridad /></Layout></ProtectedRoute>} />
      <Route path="/perfil" element={<ProtectedRoute><Layout><Perfil /></Layout></ProtectedRoute>} />
      <Route path="/planes" element={<ProtectedRoute><Layout><Planes /></Layout></ProtectedRoute>} />
      <Route path="/admin" element={<AdminRoute><Layout><AdminDashboard /></Layout></AdminRoute>} />
      <Route path="/admin/audit" element={<AdminRoute><Layout><AuditLogPage /></Layout></AdminRoute>} />
      <Route path="*" element={<NotFound />} />
    </Routes>
  );
}

export default function App() {
  return (
    <ThemeProvider theme={theme}>
      <CssBaseline />
      <AuthProvider>
        <ToastProvider>
          <Router>
            <AppRoutes />
          </Router>
        </ToastProvider>
      </AuthProvider>
    </ThemeProvider>
  );
}
