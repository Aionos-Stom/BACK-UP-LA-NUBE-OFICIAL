import { createContext, useContext, useState, useEffect, type ReactNode } from 'react';
import axios from 'axios';

interface UsuarioInfo {
  id: number;
  nombre: string;
  email: string;
  rol: string;
  plan?: { nombre: string; limiteBytes: number; usadoBytes: number; esGratis: boolean };
}

interface AuthContextType {
  usuario: UsuarioInfo | null;
  login: (email: string, password: string) => Promise<void>;
  logout: () => Promise<void>;
  isLoading: boolean;
  isAdmin: boolean;
  isSuperAdmin: boolean;
}

const AuthContext = createContext<AuthContextType | null>(null);

// axios con credentials para enviar/recibir cookies httpOnly automáticamente
const api = axios.create({
  baseURL: '/api',
  withCredentials: true, // CRÍTICO: envía las cookies en cada petición
});

export function AuthProvider({ children }: { children: ReactNode }) {
  const [usuario, setUsuario] = useState<UsuarioInfo | null>(null);
  const [isLoading, setIsLoading] = useState(true);

  // Al cargar la app, verifica si hay sesión activa via cookie httpOnly
  useEffect(() => {
    api.get('/auth/me')
      .then(r => setUsuario(r.data))
      .catch(() => setUsuario(null))
      .finally(() => setIsLoading(false));
  }, []);

  const login = async (email: string, password: string) => {
    const r = await api.post('/auth/login', { email, password });
    // El servidor setea las cookies httpOnly automáticamente
    setUsuario(r.data.usuario);
  };

  const logout = async () => {
    await api.post('/auth/logout').catch(() => {});
    // El servidor borra las cookies httpOnly
    setUsuario(null);
  };

  return (
    <AuthContext.Provider value={{
      usuario,
      login,
      logout,
      isLoading,
      isAdmin: usuario?.rol === 'admin' || usuario?.rol === 'superadmin',
      isSuperAdmin: usuario?.rol === 'superadmin'
    }}>
      {children}
    </AuthContext.Provider>
  );
}

export const useAuth = () => {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error('useAuth debe usarse dentro de AuthProvider');
  return ctx;
};
