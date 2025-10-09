export type UserRole = 'Lister' | 'Buyer';

export interface RegisterRequest {
  name: string;
  email: string;
  password: string;
  phoneNumber?: string;
  role: UserRole;
}
