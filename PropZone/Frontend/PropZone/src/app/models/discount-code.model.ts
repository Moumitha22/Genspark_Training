import { DiscountCodeOption } from "./discount-code-option.model";

export interface DiscountCode{
    id: string;
    code: string;
    discountValue: number;
    isPercentage: boolean;
    fromDate: string;
    toDate: string;
    isActive: boolean;
    isDeleted?:boolean;
    maxListerLimit?: number;
    options: DiscountCodeOption[];
}