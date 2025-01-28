import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { ProductService } from '../../../services/product.service';
import { Product } from '../../../models/product.model';

import { MatToolbarModule } from '@angular/material/toolbar';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';

@Component({
  standalone: true,
  selector: 'app-product-create',
  imports: [
    CommonModule,
    FormsModule,
    RouterModule,
    MatToolbarModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule
  ],
  templateUrl: './product-create.component.html',
  styleUrls: ['./product-create.component.scss']
})
export class ProductCreateComponent {
  product: Product = {
    id: '',  // now a string
    name: '',
    price: 0,
    description: ''
  };

  constructor(private productService: ProductService, private router: Router) { }

  createProduct(): void {
    this.productService.create(this.product).subscribe({
      next: (data) => {
        console.log('Product created successfully:', data);
        this.router.navigate(['/products']);
      },
      error: (err) => {
        console.error('Error creating product:', err);
      }
    });
  }
}
