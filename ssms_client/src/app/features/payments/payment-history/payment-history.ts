import { Component, inject, OnInit, signal } from '@angular/core';
import { JobService } from '../../../core/services/job';
import { PaymentService } from '../../../core/services/payment';
import { JobResponse } from '../../../core/models/job.model';
import { PaymentResponse, PaymentStatus } from '../../../core/models/payment.model';
import { Card } from '../../../shared/components/card/card';
import { LoadingSpinner } from '../../../shared/components/loading-spinner/loading-spinner';

interface JobPayment {
  job: JobResponse;
  payment: PaymentResponse | null;
}

@Component({
  selector: 'app-payment-history',
  standalone: true,
  imports: [Card, LoadingSpinner],
  templateUrl: './payment-history.html',
  styleUrl: './payment-history.scss'
})
export class PaymentHistory implements OnInit {
  private jobService = inject(JobService);
  private paymentService = inject(PaymentService);

  readonly PaymentStatus = PaymentStatus;
  records = signal<JobPayment[]>([]);
  isLoading = signal(true);

  ngOnInit(): void {
    this.jobService.getMine().subscribe(jobs => {
      const relevant = jobs.filter(j => j.status === 3 || j.status === 4); // Completed or Closed
      if (relevant.length === 0) { this.isLoading.set(false); return; }

      let completed = 0;
      const results: JobPayment[] = [];
      relevant.forEach(job => {
        this.paymentService.getForJob(job.id).subscribe({
          next: (payment) => { results.push({ job, payment }); finish(); },
          error: () => { results.push({ job, payment: null }); finish(); }
        });
      });

      const finish = () => {
        completed++;
        if (completed === relevant.length) {
          this.records.set(results);
          this.isLoading.set(false);
        }
      };
    });
  }
}