import { Component, OnInit, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Product, ProductRequest } from '../../models/product';
import { ProductService } from '../../services/product';

@Component({
  selector: 'app-product-management',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './product-management.html',
  styleUrl: './product-management.css'
})
export class ProductManagement implements OnInit {
  private readonly productService = inject(ProductService);
  private readonly cdr = inject(ChangeDetectorRef);

  products: Product[] = [];
  form: ProductRequest = this.emptyForm();
  editingId: number | null = null;
  loading = false;
  saving = false;
  errorMessage = '';
  successMessage = '';

  ngOnInit(): void { this.loadProducts(); }

  loadProducts(): void {
    this.loading = true;
    this.errorMessage = '';

    this.productService.getAll().subscribe({
      next: products => {
        this.products = products;
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.errorMessage = 'Could not load products. Check that the API is running and CORS is configured.';
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }

  saveProduct(): void {
    this.saving = true;
    this.errorMessage = '';
    this.successMessage = '';

    const request = { ...this.form, name: this.form.name.trim(), category: this.form.category.trim(), description: this.form.description.trim() };
    const operation = this.editingId === null ? this.productService.create(request) : this.productService.update(this.editingId, request);

    operation.subscribe({
      next: () => {
        this.successMessage = this.editingId === null ? 'Product added successfully.' : 'Product updated successfully.';
        this.cancelEdit();
        this.loadProducts();
        this.saving = false;
        this.cdr.detectChanges();
      },
      error: error => {
        this.errorMessage = error?.error?.message ?? 'Could not save the product. Check the entered values and API.';
        this.saving = false;
        this.cdr.detectChanges();
      }
    });
  }

  editProduct(product: Product): void {
    this.editingId = product.id;
    this.form = { name: product.name, description: product.description ?? '', price: product.price, category: product.category };
    this.errorMessage = '';
    this.successMessage = '';
  }

  cancelEdit(): void { this.editingId = null; this.form = this.emptyForm(); }

  productsTotal(): number { return this.products.reduce((total, product) => total + product.price, 0); }

  private emptyForm(): ProductRequest { return { name: '', description: '', price: 0, category: '' }; }
}