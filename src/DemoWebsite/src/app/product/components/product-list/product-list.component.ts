import { Component, OnInit, AfterViewInit, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { Product } from '../../../models/product.model';
import { ProductService } from '../../../services/product.service';

import { MatToolbarModule } from '@angular/material/toolbar';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatPaginator, MatPaginatorModule, PageEvent } from '@angular/material/paginator';

@Component({
  standalone: true,
  selector: 'app-product-list',
  imports: [
    CommonModule,
    RouterModule,
    MatToolbarModule,
    MatCardModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatPaginatorModule,
  ], // so we can use *ngFor, routerLink, etc.
  templateUrl: './product-list.component.html',
  styleUrls: ['./product-list.component.scss']
})
export class ProductListComponent implements OnInit, AfterViewInit {
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  displayedColumns: string[] = ['id', 'name', 'price', 'description', 'actions'];

  products: Product[] = [];           // Full array of products
  displayedProducts: Product[] = [];  // Slice of products for the current page

  // Paginator settings
  pageSizeOptions: number[] = [5, 10, 25];
  pageSize = 5;      // Default page size
  pageIndex = 0;     // Default page index

  constructor(private productService: ProductService) { }

  ngOnInit(): void {
    this.fetchProducts();
  }

  ngAfterViewInit(): void {
    // Subscribe to paginator changes
    this.paginator.page.subscribe((event: PageEvent) => {
      // Update pageSize and pageIndex from the event
      this.pageSize = event.pageSize;
      this.pageIndex = event.pageIndex;

      // Update the displayedProducts slice
      this.updateDisplayedProducts();
    });
  }

  fetchProducts(): void {
    this.productService.getAll().subscribe({
      next: (data) => {
        this.products = data;
        this.updateDisplayedProducts();
      },
      error: (err) => {
        console.error('Error fetching products:', err);
      }
    });
  }

  /**
 * Slice the products array based on the current pageIndex and pageSize.
 */
  updateDisplayedProducts(): void {
    const startIndex = this.pageIndex * this.pageSize;
    const endIndex = startIndex + this.pageSize;
    this.displayedProducts = this.products.slice(startIndex, endIndex);
  }

  deleteProduct(id: string): void {
    if (confirm('Are you sure you want to delete this product?')) {
      this.productService.delete(id).subscribe({
        next: () => {
          this.products = this.products.filter(product => product.id !== id);
        },
        error: (err) => {
          console.error('Error deleting product:', err);
        }
      });
    }
  }
}
