export interface User {
  id: number;
  fullName: string;
  email: string;
  companyName: string;
  phoneNumber: string;
  createdAt: Date;
  isActive: boolean;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  fullName: string;
  email: string;
  password: string;
  confirmPassword: string;
  companyName: string;
  phoneNumber: string;
}

export interface AuthResponse {
  token: string;
  message: string;
}
