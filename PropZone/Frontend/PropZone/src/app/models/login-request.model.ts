export interface LoginRequest {
  email: string;
  password: string;
  role: 'Buyer' | 'Lister' | 'Admin' ;
}
