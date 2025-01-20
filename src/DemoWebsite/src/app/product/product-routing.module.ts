import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

const routes: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./components/product-list/product-list.component')
        .then(m => m.ProductListComponent)
  },
  {
    path: 'create',
    loadComponent: () =>
      import('./components/product-create/product-create.component')
        .then(m => m.ProductCreateComponent)
  },
  {
    path: 'edit/:id',
    loadComponent: () =>
      import('./components/product-edit/product-edit.component')
        .then(m => m.ProductEditComponent)
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ProductRoutingModule { }
