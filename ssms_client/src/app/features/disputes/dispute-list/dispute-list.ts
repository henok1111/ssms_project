import { Component, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { DisputeService } from '../../../core/services/dispute';
import { DisputeResponse } from '../../../core/models/dispute.model';
import { Card } from '../../../shared/components/card/card';
import { LoadingSpinner } from '../../../shared/components/loading-spinner/loading-spinner';

@Component({
  selector: 'app-dispute-list',
  standalone: true,
  imports: [ReactiveFormsModule, Card, LoadingSpinner],
  templateUrl: './dispute-list.html',
  styleUrl: './dispute-list.scss'
})
export class DisputeList implements OnInit {
  private disputeService = inject(DisputeService);
  private fb = inject(FormBuilder);

  disputes = signal<DisputeResponse[]>([]);
  isLoading = signal(true);
  actionLoading = signal<string | null>(null);

  resolveForm = this.fb.group({
    adminResolutionNote: ['']
  });

  ngOnInit(): void {
    this.loadDisputes();
  }

  loadDisputes(): void {
    this.disputeService.getAllOpen().subscribe({
      next: (d) => { this.disputes.set(d); this.isLoading.set(false); },
      error: () => this.isLoading.set(false)
    });
  }

  onResolve(disputeId: string, approve: boolean): void {
    this.actionLoading.set(disputeId);
    const note = this.resolveForm.getRawValue().adminResolutionNote || (approve ? 'Dispute upheld.' : 'Dispute dismissed.');
    this.disputeService.resolve(disputeId, { adminResolutionNote: note, approve }).subscribe({
      next: () => { this.actionLoading.set(null); this.loadDisputes(); },
      error: () => this.actionLoading.set(null)
    });
  }
}