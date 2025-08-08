export interface NewsModel {
  id: number;
  userId?: number;
  title: string;
  shortDescription?: string;
  image?: string;
  content?: string;
  createdDate?: Date;
  status?: number;
}
