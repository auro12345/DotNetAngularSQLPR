import { TestBed } from '@angular/core/testing';
import { Observable, of, throwError, Subject } from 'rxjs';
import { Product } from '../../models/product';
import { ProductService } from '../../services/product';
import { ProductManagement } from './product-management';

class ProductServiceStub {
  deleteCalls: number[] = [];
  deleteResult = new Subject<void>();

  getAll() { return of([]); }
  create() { return of({} as Product); }
  update() { return of({} as Product); }
  delete(id: number): Observable<void> {
    this.deleteCalls.push(id);
    return this.deleteResult;
  }
}

describe('ProductManagement delete behavior', () => {
  let component: ProductManagement;
  let service: ProductServiceStub;

  beforeEach(async () => {
    service = new ProductServiceStub();
    await TestBed.configureTestingModule({
      imports: [ProductManagement],
      providers: [{ provide: ProductService, useValue: service }],
    }).compileComponents();

    const fixture = TestBed.createComponent(ProductManagement);
    component = fixture.componentInstance;
    component.products = [{ id: 7, name: 'Laptop', description: '', price: 50000, category: 'Electronics' }];
  });

  it('[AIK-005-AC-4] cancellation sends no delete request', () => {
    vi.spyOn(window, 'confirm').mockReturnValue(false);

    component.deleteProduct(component.products[0]);

    expect(service.deleteCalls).toEqual([]);
  });

  it('[AIK-005-AC-5] successful deletion reloads products', () => {
    vi.spyOn(window, 'confirm').mockReturnValue(true);
    service.getAll = () => of([]);

    component.deleteProduct(component.products[0]);
    service.deleteResult.next();
    service.deleteResult.complete();

    expect(service.deleteCalls).toEqual([7]);
    expect(component.products).toEqual([]);
  });

  it('[AIK-005-AC-7] API failure leaves the displayed product unchanged', () => {
    vi.spyOn(window, 'confirm').mockReturnValue(true);
    service.delete = () => throwError(() => new Error('API failed'));

    component.deleteProduct(component.products[0]);

    expect(component.products.length).toBe(1);
    expect(component.errorMessage).toContain('Could not delete');
  });
});
