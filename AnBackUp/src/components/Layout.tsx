import { useState } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import {
  Box,
  Drawer,
  AppBar,
  Toolbar,
  List,
  Typography,
  Divider,
  IconButton,
  ListItem,
  ListItemButton,
  ListItemIcon,
  ListItemText,
  Container,
} from '@mui/material';
import MenuIcon from '@mui/icons-material/Menu';
import DashboardIcon from '@mui/icons-material/Dashboard';
import BackupIcon from '@mui/icons-material/Backup';
import WarningIcon from '@mui/icons-material/Warning';
import CloudQueueIcon from '@mui/icons-material/CloudQueue';
import PolicyIcon from '@mui/icons-material/Policy';
import RestoreIcon from '@mui/icons-material/Restore';
import VerifiedIcon from '@mui/icons-material/Verified';

const drawerWidth = 240;

interface LayoutProps {
  children: React.ReactNode;
}

const menuItems = [
  { text: 'Dashboard', icon: <DashboardIcon />, path: '/' },
  { text: 'Jobs de Backup', icon: <BackupIcon />, path: '/jobs' },
  { text: 'Alertas', icon: <WarningIcon />, path: '/alertas' },
  { text: 'Cloud Storage', icon: <CloudQueueIcon />, path: '/cloud-storage' },
  { text: 'Políticas', icon: <PolicyIcon />, path: '/politicas' },
  { text: 'Recuperaciones', icon: <RestoreIcon />, path: '/recuperaciones' },
  { text: 'Verificación Integridad', icon: <VerifiedIcon />, path: '/verificacion-integridad' },
];

export default function Layout({ children }: LayoutProps) {
  const [mobileOpen, setMobileOpen] = useState(false);
  const navigate = useNavigate();
  const location = useLocation();

  const handleDrawerToggle = () => {
    setMobileOpen(!mobileOpen);
  };

  const drawer = (
    <div>
      <Toolbar>
        <Typography 
          variant="h6" 
          noWrap 
          component="div" 
          sx={{ 
            fontWeight: 700,
            background: 'linear-gradient(135deg, #00d4ff 0%, #7c3aed 100%)',
            WebkitBackgroundClip: 'text',
            WebkitTextFillColor: 'transparent',
            fontSize: '1.5rem',
          }}
        >
          AnBackUp
        </Typography>
      </Toolbar>
      <Divider />
      <List>
        {menuItems.map((item) => (
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
                  '& .MuiListItemIcon-root': {
                    color: '#00d4ff',
                  },
                  '& .MuiListItemText-primary': {
                    color: '#00d4ff',
                    fontWeight: 600,
                  },
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
            Sistema de Backup
          </Typography>
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
          ModalProps={{
            keepMounted: true,
          }}
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

