import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { ProductRoutingModule } from './product-routing.module';
import { ProductListComponent } from './components/product-list/product-list.component';
import { ProductCreateComponent } from './components/product-create/product-create.component';
import { ProductEditComponent } from './components/product-edit/product-edit.component';

@NgModule({
  declarations: [
    ProductListComponent,
    ProductCreateComponent,
    ProductEditComponent
  ],
  imports: [
    CommonModule,           // Needed for *ngFor, *ngIf, etc.
    FormsModule,            // For [(ngModel)] usage
    ProductRoutingModule
  ]
})
export class ProductModule { }
