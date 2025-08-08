import { Routes } from '@angular/router';
import { ProductsComponent } from './pages/products/products';
import { PostProductComponent } from './pages/post-product/post-product';
import { EditProductComponent } from './pages/edit-product/edit-product';
import { CartComponent } from './pages/cart/cart';
import { PlaceOrderComponent } from './pages/place-order/place-order';
import { MyOrdersComponent } from './pages/my-orders/my-orders';
import { PostNewsComponent } from './components/post-news/post-news';
import { NewsListComponent } from './pages/news/news';
import { ContactUsComponent } from './pages/contact-us/contact-us';
import { ManageAttributesComponent } from './pages/manage-attributes/manage-attributes';
import { LoginComponent } from './pages/login/login';
import { SignupComponent } from './pages/signup/signup';
import { MyProductsComponent } from './pages/my-products/my-products';

export const routes: Routes = [
  { path: '', component: ProductsComponent, pathMatch: 'full' },
  { path: 'products', component: ProductsComponent },
  { path: 'login', component: LoginComponent },
  { path: 'signup', component: SignupComponent },
  { 
    path: 'products',
    component: ProductsComponent 
  },
  { 
    path: 'my-products',
    component: MyProductsComponent
  },
  {
    path: 'news',
    component: NewsListComponent,
  },
  {
    path: 'post-news',
    component: PostNewsComponent,
  },
  {
    path: 'post-product',
    component: PostProductComponent,
  },
  {
    path: 'edit-product/:id',
    component: EditProductComponent,
  },
//   {
//     path: 'product-details/:id',
//     component: ProductDetailsComponent 
//   }
{
  path: 'cart',
  component: CartComponent
},
{ 
  path: 'place-order', 
  component: PlaceOrderComponent 
},
  {
    path: 'my-orders',
    component: MyOrdersComponent
  },
  {
    path: 'contact-us',
    component: ContactUsComponent
  },
  { 
    path: 'manage-attributes', 
    component: ManageAttributesComponent
  }

];
