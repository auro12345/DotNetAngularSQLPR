import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Product, ProductRequest } from '../models/product';

@Injectable({
  providedIn: 'root',
})
export class ProductService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = 'https://localhost:7193/api/Products';

  getAll(): Observable<Product[]> { return this.http.get<Product[]>(this.apiUrl); }

  create(product: ProductRequest): Observable<Product> { return this.http.post<Product>(this.apiUrl, product); }

  update(id: number, product: ProductRequest): Observable<Product> { return this.http.put<Product>(`${this.apiUrl}/${id}`, product); }

  delete(id: number): Observable<void> { return this.http.delete<void>(`${this.apiUrl}/${id}`); }
}