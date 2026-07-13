import { useState, useEffect } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import {
  Box, Drawer, AppBar, Toolbar, List, Typography, Divider,
  IconButton, ListItem, ListItemButton, ListItemIcon, ListItemText,
  Container, Chip, Tooltip, Badge,
} from '@mui/material';
import MenuIcon from '@mui/icons-material/Menu';
import DashboardIcon from '@mui/icons-material/Dashboard';
import BackupIcon from '@mui/icons-material/Backup';
import WarningIcon from '@mui/icons-material/Warning';
import CloudQueueIcon from '@mui/icons-material/CloudQueue';
import PolicyIcon from '@mui/icons-material/Policy';
import RestoreIcon from '@mui/icons-material/Restore';
import VerifiedIcon from '@mui/icons-material/Verified';
import LogoutIcon from '@mui/icons-material/Logout';
import AdminPanelSettingsIcon from '@mui/icons-material/AdminPanelSettings';
import AccountCircleIcon from '@mui/icons-material/AccountCircle';
import NotificationsIcon from '@mui/icons-material/Notifications';
import PersonIcon from '@mui/icons-material/Person';
import CreditCardIcon from '@mui/icons-material/CreditCard';
import AssignmentIcon from '@mui/icons-material/Assignment';
import GroupIcon from '@mui/icons-material/Group';
import axios from 'axios';
import { useAuth } from '../contexts/AuthContext';

const drawerWidth = 240;

interface LayoutProps {
  children: React.ReactNode;
}

const userMenuItems = [
  { text: 'Dashboard', icon: <DashboardIcon />, path: '/' },
  { text: 'Jobs de Backup', icon: <BackupIcon />, path: '/jobs' },
  { text: 'Alertas', icon: <WarningIcon />, path: '/alertas' },
  { text: 'Cloud Storage', icon: <CloudQueueIcon />, path: '/cloud-storage' },
  { text: 'Políticas', icon: <PolicyIcon />, path: '/politicas' },
  { text: 'Recuperaciones', icon: <RestoreIcon />, path: '/recuperaciones' },
  { text: 'Verificación', icon: <VerifiedIcon />, path: '/verificacion-integridad' },
  { text: 'Mis Planes', icon: <CreditCardIcon />, path: '/planes' },
  { text: 'Mi Perfil', icon: <PersonIcon />, path: '/perfil' },
];

const adminMenuItems = [
  { text: 'Panel de Administración', icon: <AdminPanelSettingsIcon />, path: '/admin' },
  { text: 'Usuarios y Planes', icon: <GroupIcon />, path: '/admin' },
  { text: 'Audit Log', icon: <AssignmentIcon />, path: '/admin/audit' },
  { text: 'Mi Perfil Admin', icon: <PersonIcon />, path: '/perfil' },
];

const auditorMenuItems = [
  { text: 'Audit Log', icon: <AssignmentIcon />, path: '/admin/audit' },
  { text: 'Mi Perfil', icon: <PersonIcon />, path: '/perfil' },
];

export default function Layout({ children }: LayoutProps) {
  const [mobileOpen, setMobileOpen] = useState(false);
  const [notifCount, setNotifCount] = useState(0);
  const navigate = useNavigate();
  const location = useLocation();
  const { usuario, logout, isAdmin } = useAuth();

  const isAuditor = usuario?.rol === 'auditor';
  const activeMenuItems = isAdmin ? adminMenuItems : isAuditor ? auditorMenuItems : userMenuItems;

  useEffect(() => {
    if (isAdmin || isAuditor) return;
    const cargar = () => {
      axios.get('/api/notificaciones', { withCredentials: true })
        .then(r => setNotifCount(r.data.contador))
        .catch(() => {});
    };
    cargar();
    const interval = setInterval(cargar, 30000);
    return () => clearInterval(interval);
  }, [isAdmin, isAuditor]);

  const handleDrawerToggle = () => setMobileOpen(!mobileOpen);

  const handleLogout = async () => {
    await logout();
    navigate('/login');
  };

  const drawer = (
    <div>
      <Toolbar>
        <Box>
          <Typography
            variant="h6"
            noWrap
            component="div"
            sx={{
              fontWeight: 700,
              background: 'linear-gradient(135deg, #00d4ff 0%, #7c3aed 100%)',
              WebkitBackgroundClip: 'text',
              WebkitTextFillColor: 'transparent',
              fontSize: '1.4rem',
            }}
          >
            AnBackUp
          </Typography>
          {isAdmin && (
            <Typography variant="caption" sx={{ color: '#ffb800', display: 'block', mt: -0.5 }}>
              Panel Administrativo
            </Typography>
          )}
          {isAuditor && (
            <Typography variant="caption" sx={{ color: '#00d4ff', display: 'block', mt: -0.5 }}>
              Auditoría
            </Typography>
          )}
        </Box>
      </Toolbar>
      <Divider />
      <List>
        {activeMenuItems.map((item) => (
          <ListItem key={item.text} disablePadding>
            <ListItemButton
              selected={location.pathname === item.path}
              onClick={() => {
                navigate(item.path);
                setMobileOpen(false);
              }}
              sx={{
                borderRadius: '12px',
                mx: 1,
                mb: 0.5,
                '&.Mui-selected': {
                  background: 'linear-gradient(135deg, rgba(0, 212, 255, 0.2) 0%, rgba(124, 58, 237, 0.2) 100%)',
                  border: '1px solid rgba(0, 212, 255, 0.3)',
                  boxShadow: '0 0 20px rgba(0, 212, 255, 0.2)',
                  '&:hover': {
                    background: 'linear-gradient(135deg, rgba(0, 212, 255, 0.3) 0%, rgba(124, 58, 237, 0.3) 100%)',
                  },
                  '& .MuiListItemIcon-root': { color: '#00d4ff' },
                  '& .MuiListItemText-primary': { color: '#00d4ff', fontWeight: 600 },
                },
                '&:hover': {
                  background: 'rgba(0, 212, 255, 0.1)',
                  border: '1px solid rgba(0, 212, 255, 0.2)',
                },
                transition: 'all 0.3s ease',
              }}
            >
              <ListItemIcon sx={{ color: location.pathname === item.path ? '#00d4ff' : '#b0d4ff' }}>
                {item.icon}
              </ListItemIcon>
              <ListItemText
                primary={item.text}
                primaryTypographyProps={{
                  sx: {
                    color: location.pathname === item.path ? '#00d4ff' : '#b0d4ff',
                    fontWeight: location.pathname === item.path ? 600 : 400,
                    fontSize: '0.875rem',
                  },
                }}
              />
            </ListItemButton>
          </ListItem>
        ))}
      </List>
    </div>
  );

  return (
    <Box sx={{ display: 'flex' }}>
      <AppBar
        position="fixed"
        sx={{
          width: { sm: `calc(100% - ${drawerWidth}px)` },
          ml: { sm: `${drawerWidth}px` },
        }}
      >
        <Toolbar>
          <IconButton
            color="inherit"
            aria-label="open drawer"
            edge="start"
            onClick={handleDrawerToggle}
            sx={{ mr: 2, display: { sm: 'none' } }}
          >
            <MenuIcon />
          </IconButton>
          <Typography
            variant="h6"
            noWrap
            component="div"
            sx={{
              background: 'linear-gradient(135deg, #00d4ff 0%, #7c3aed 100%)',
              WebkitBackgroundClip: 'text',
              WebkitTextFillColor: 'transparent',
              fontWeight: 600,
            }}
          >
            {isAdmin ? 'Administración' : isAuditor ? 'Auditoría' : 'Sistema de Backup'}
          </Typography>

          <Box sx={{ flexGrow: 1 }} />

          {usuario && (
            <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
              <AccountCircleIcon sx={{ color: '#b0d4ff', fontSize: 20 }} />
              <Typography variant="body2" sx={{ color: '#b0d4ff', display: { xs: 'none', md: 'block' } }}>
                {usuario.nombre}
              </Typography>
              <Chip
                label={usuario.rol.toUpperCase()}
                size="small"
                sx={{
                  bgcolor: isAdmin ? 'rgba(255,184,0,0.15)' : 'rgba(0,212,255,0.15)',
                  color: isAdmin ? '#ffb800' : '#00d4ff',
                  border: `1px solid ${isAdmin ? 'rgba(255,184,0,0.3)' : 'rgba(0,212,255,0.3)'}`,
                  display: { xs: 'none', sm: 'flex' },
                  fontSize: '0.65rem',
                  fontWeight: 700,
                }}
              />
              {!isAdmin && !isAuditor && usuario.plan && (
                <Chip
                  label={usuario.plan.nombre}
                  size="small"
                  sx={{
                    bgcolor: 'rgba(0,212,255,0.1)',
                    color: '#00d4ff',
                    border: '1px solid rgba(0,212,255,0.2)',
                    display: { xs: 'none', sm: 'flex' },
                    fontSize: '0.65rem',
                  }}
                />
              )}
              {!isAdmin && !isAuditor && (
                <Tooltip title="Notificaciones">
                  <IconButton sx={{ color: '#b0d4ff', mr: 1 }}>
                    <Badge badgeContent={notifCount} color="error" max={99}>
                      <NotificationsIcon />
                    </Badge>
                  </IconButton>
                </Tooltip>
              )}
              <Tooltip title="Cerrar Sesión">
                <IconButton size="small" onClick={handleLogout} sx={{ color: '#ff006e' }}>
                  <LogoutIcon fontSize="small" />
                </IconButton>
              </Tooltip>
            </Box>
          )}
        </Toolbar>
      </AppBar>
      <Box
        component="nav"
        sx={{ width: { sm: drawerWidth }, flexShrink: { sm: 0 } }}
      >
        <Drawer
          variant="temporary"
          open={mobileOpen}
          onClose={handleDrawerToggle}
          ModalProps={{ keepMounted: true }}
          sx={{
            display: { xs: 'block', sm: 'none' },
            '& .MuiDrawer-paper': { boxSizing: 'border-box', width: drawerWidth },
          }}
        >
          {drawer}
        </Drawer>
        <Drawer
          variant="permanent"
          sx={{
            display: { xs: 'none', sm: 'block' },
            '& .MuiDrawer-paper': { boxSizing: 'border-box', width: drawerWidth },
          }}
          open
        >
          {drawer}
        </Drawer>
      </Box>
      <Box
        component="main"
        sx={{
          flexGrow: 1,
          p: 3,
          width: { sm: `calc(100% - ${drawerWidth}px)` },
          minHeight: '100vh',
          backgroundColor: 'transparent',
        }}
      >
        <Toolbar />
        <Container maxWidth="xl">
          {children}
        </Container>
      </Box>
    </Box>
  );
}
