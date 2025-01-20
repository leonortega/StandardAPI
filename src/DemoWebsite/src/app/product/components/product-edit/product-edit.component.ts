import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ProductService } from '../../../services/product.service';
import { Product } from '../../../models/product.model';

@Component({
  standalone: true,
  selector: 'app-product-edit',
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './product-edit.component.html',
  styleUrls: ['./product-edit.component.scss']
})
export class ProductEditComponent implements OnInit {
  product: Product = {
    id: '',
    name: '',
    price: 0,
    description: ''
  };

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private productService: ProductService
  ) { }

  ngOnInit(): void {
    // Retrieve the 'id' param (string)
    this.route.paramMap.subscribe(params => {
      const id = params.get('id');
      if (id) {
        this.loadProduct(id);
      }
    });
  }

  loadProduct(id: string): void {
    this.productService.getById(id).subscribe({
      next: (data) => {
        this.product = data;
      },
      error: (err) => {
        console.error('Error fetching product:', err);
      }
    });
  }

  updateProduct(): void {
    if (!this.product.id) return; // ensure we have an ID

    this.productService.update(this.product.id, this.product).subscribe({
      next: (data) => {
        console.log('Product updated successfully:', data);
        this.router.navigate(['/products']);
      },
      error: (err) => {
        console.error('Error updating product:', err);
      }
    });
  }
}
