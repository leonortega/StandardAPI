import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { ProductService } from '../../../services/product.service';
import { Product } from '../../../models/product.model';

@Component({
  standalone: true,
  selector: 'app-product-create',
  imports: [CommonModule, FormsModule, RouterModule],
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
