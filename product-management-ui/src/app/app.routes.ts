import { Routes } from '@angular/router';
import { ProductManagement } from './products/product-management/product-management';

export const routes: Routes = [
{ path: '', component: ProductManagement },
{ path: '**', redirectTo: '' }
];