export interface User {
  id: string;
  name: string;
  email: string;
  phoneNumber?: string;
  role: 'Buyer' | 'Lister' | 'Admin';
  createdAt: string;
  updatedAt: string;
  isDeleted: boolean;
}
