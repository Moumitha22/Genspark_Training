export interface ProductModel {
  id: number;
  productName: string;
  image: string;
  price?: number;

  userId?: number;
  categoryId?: number;
  colorId?: number;
  modelId?: number;
  storageId?: number;

  sellStartDate?: Date; 
  sellEndDate?: Date;

  categoryName?: string;
  colorName?: string;
  modelName?: string;
  storageName?: string;

  isNew?: boolean;
}
