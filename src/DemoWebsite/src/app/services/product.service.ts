import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Product } from '../models/product.model';

@Injectable({
  providedIn: 'root'
})
export class ProductService {

  private baseUrl = 'http://localhost:3000/products';
  // Replace with your actual API endpoint

  constructor(private http: HttpClient) { }

  // Get all products
  getAll(): Observable<Product[]> {
    return this.http.get<Product[]>(this.baseUrl);
  }

  // Get a single product by string ID
  getById(id: string): Observable<Product> {
    return this.http.get<Product>(`${this.baseUrl}/${id}`);
  }

  // Create a new product
  create(product: Product): Observable<Product> {
    return this.http.post<Product>(this.baseUrl, product);
  }

  // Update an existing product
  update(id: string, product: Product): Observable<Product> {
    return this.http.put<Product>(`${this.baseUrl}/${id}`, product);
  }

  // Delete a product
  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
