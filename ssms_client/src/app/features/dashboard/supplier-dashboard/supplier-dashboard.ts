import { Component, inject, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { MaterialItemService } from '../../../core/services/material-item';
import { MaterialOrderService } from '../../../core/services/material-order';
import { AuthService } from '../../../core/auth/auth';
import { MaterialItemResponse, MaterialOrderResponse, OrderStatus } from '../../../core/models/material.model';
import { Card } from '../../../shared/components/card/card';
import { LoadingSpinner } from '../../../shared/components/loading-spinner/loading-spinner';

@Component({
  selector: 'app-supplier-dashboard',
  standalone: true,
  imports: [RouterLink, Card, LoadingSpinner],
  templateUrl: './supplier-dashboard.html',
  styleUrl: './supplier-dashboard.scss'
})
export class SupplierDashboard implements OnInit {
  private materialItemService = inject(MaterialItemService);
  private materialOrderService = inject(MaterialOrderService);
  authService = inject(AuthService);

  readonly OrderStatus = OrderStatus;
  items = signal<MaterialItemResponse[]>([]);
  orders = signal<MaterialOrderResponse[]>([]);
  isLoading = signal(true);
  actionLoading = signal<string | null>(null);

  ngOnInit(): void {
    this.materialItemService.getMine().subscribe(items => this.items.set(items));
    this.materialOrderService.getMine().subscribe({
      next: (orders) => { this.orders.set(orders); this.isLoading.set(false); },
      error: () => this.isLoading.set(false)
    });
  }

  get pendingOrdersCount(): number {
    return this.orders().filter(o => o.status === OrderStatus.Pending).length;
  }

  get lowStockCount(): number {
    return this.items().filter(i => i.stockQuantity < 5).length;
  }

  onConfirm(orderId: string): void {
    this.actionLoading.set(orderId);
    this.materialOrderService.confirm(orderId).subscribe({
      next: () => this.refreshOrders(),
      error: () => this.actionLoading.set(null)
    });
  }

  onFulfill(orderId: string): void {
    this.actionLoading.set(orderId);
    this.materialOrderService.fulfill(orderId).subscribe({
      next: () => this.refreshOrders(),
      error: () => this.actionLoading.set(null)
    });
  }

  private refreshOrders(): void {
    this.materialOrderService.getMine().subscribe(orders => {
      this.orders.set(orders);
      this.actionLoading.set(null);
    });
  }
}