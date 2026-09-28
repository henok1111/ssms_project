import { Component, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { JobService } from '../../../core/services/job';
import { AuthService } from '../../../core/auth/auth';
import { JobResponse, JobStatus, JobApplicationResponse, ApplicationStatus } from '../../../core/models/job.model';
import { Card } from '../../../shared/components/card/card';
import { LoadingSpinner } from '../../../shared/components/loading-spinner/loading-spinner';
import { OnDestroy, ElementRef, ViewChild, AfterViewChecked } from '@angular/core';
import { MessageService } from '../../../core/services/message';
import { SignalrService } from '../../../core/signalr/signalr';
import { MessageResponse } from '../../../core/models/message.model';
import { QuoteService } from '../../../core/services/quote';
import { PaymentService } from '../../../core/services/payment';
import { QuoteResponse, QuoteStatus } from '../../../core/models/quote.model';
import { PaymentResponse, PaymentStatus } from '../../../core/models/payment.model';
import { DisputeService } from '../../../core/services/dispute';
@Component({
  selector: 'app-job-detail',
  standalone: true,
  imports: [ReactiveFormsModule, Card, LoadingSpinner],
  templateUrl: './job-detail.html',
  styleUrl: './job-detail.scss'
})
export class JobDetail implements OnInit, OnDestroy, AfterViewChecked {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private jobService = inject(JobService);
  private fb = inject(FormBuilder);
  authService = inject(AuthService);
  private messageService = inject(MessageService);
  private signalr = inject(SignalrService);
  private quoteService = inject(QuoteService);
  private paymentService = inject(PaymentService);
private disputeService = inject(DisputeService);

disputeForm = this.fb.group({
  reason: ['', [Validators.required]]
});

showDisputeForm = signal(false);

  @ViewChild('messagesEnd') messagesEnd?: ElementRef<HTMLDivElement>;

  messages = signal<MessageResponse[]>([]);
  private shouldScroll = false;

  messageForm = this.fb.group({
    content: ['', [Validators.required]]
  });

  get canChat(): boolean {
    return this.isOwner || this.isAssignedWorker;
  }
  
  readonly JobStatus = JobStatus;
  readonly ApplicationStatus = ApplicationStatus;
  readonly QuoteStatus = QuoteStatus;
  readonly PaymentStatus = PaymentStatus;

  job = signal<JobResponse | null>(null);
  applications = signal<JobApplicationResponse[]>([]);
  quote = signal<QuoteResponse | null>(null);
  payment = signal<PaymentResponse | null>(null);

  isLoading = signal(true);
  actionError = signal('');
  actionLoading = signal(false);

  applyForm = this.fb.group({
    proposedPrice: [0, [Validators.required, Validators.min(1)]],
    message: ['']
  });

  quoteForm = this.fb.group({
    laborCost: [0, [Validators.required, Validators.min(1)]]
  });

  private get jobId(): string {
    return this.route.snapshot.paramMap.get('id')!;
  }

  get currentUserId(): string | undefined {
    return this.authService.currentUser()?.userId;
  }

  get isOwner(): boolean {
  return this.job()?.clientUserId === this.currentUserId;
}

get isAssignedWorker(): boolean {
  return this.job()?.assignedWorkerUserId === this.currentUserId;
}

  get userRole(): string | undefined {
    return this.authService.currentUser()?.role;
  }

  ngOnInit(): void {
    this.loadJob();
  }

  loadJob(): void {
    this.isLoading.set(true);
    this.jobService.getById(this.jobId).subscribe({
      next: (job) => {
        this.job.set(job);
        this.isLoading.set(false);
        if (this.isOwner) this.loadApplications();
        if (this.canChat) this.loadMessages();
        this.loadQuoteAndPayment();
      },
      error: () => this.isLoading.set(false)
    });
  }

  loadApplications(): void {
    this.jobService.getApplications(this.jobId).subscribe(apps => this.applications.set(apps));
  }

  loadQuoteAndPayment(): void {
    this.quoteService.getForJob(this.jobId).subscribe({
      next: (q) => this.quote.set(q),
      error: () => this.quote.set(null)
    });
    this.paymentService.getForJob(this.jobId).subscribe({
      next: (p) => this.payment.set(p),
      error: () => this.payment.set(null)
    });
  }

  onApply(): void {
    if (this.applyForm.invalid) return;
    this.actionLoading.set(true);
    this.actionError.set('');

    const raw = this.applyForm.getRawValue();
    this.jobService.apply(this.jobId, {
      proposedPrice: raw.proposedPrice!,
      message: raw.message || null
    }).subscribe({
      next: () => {
        this.actionLoading.set(false);
        this.applyForm.reset();
        alert('Application submitted!');
      },
      error: (err) => {
        this.actionLoading.set(false);
        this.actionError.set(err.error?.message ?? 'Failed to apply.');
      }
    });
  }

  onAcceptApplication(applicationId: string): void {
    this.actionLoading.set(true);
    this.jobService.acceptApplication(this.jobId, applicationId).subscribe({
      next: () => {
        this.actionLoading.set(false);
        this.loadJob();
      },
      error: (err) => {
        this.actionLoading.set(false);
        this.actionError.set(err.error?.message ?? 'Failed to accept application.');
      }
    });
  }

  onStartJob(): void {
    this.actionLoading.set(true);
    this.jobService.start(this.jobId).subscribe({
      next: () => { this.actionLoading.set(false); this.loadJob(); },
      error: (err) => { this.actionLoading.set(false); this.actionError.set(err.error?.message ?? 'Failed to start job.'); }
    });
  }

  onCompleteJob(): void {
    this.actionLoading.set(true);
    this.jobService.complete(this.jobId).subscribe({
      next: () => { this.actionLoading.set(false); this.loadJob(); },
      error: (err) => { this.actionLoading.set(false); this.actionError.set(err.error?.message ?? 'Failed to complete job.'); }
    });
  }

  onGenerateQuote(): void {
    if (this.quoteForm.invalid) return;
    this.actionLoading.set(true);
    const laborCost = this.quoteForm.getRawValue().laborCost!;
    this.quoteService.generate(this.jobId, { laborCost }).subscribe({
      next: () => { this.actionLoading.set(false); this.loadQuoteAndPayment(); },
      error: (err) => { this.actionLoading.set(false); this.actionError.set(err.error?.message ?? 'Failed to generate quote.'); }
    });
  }

  onApproveQuote(): void {
    const q = this.quote();
    if (!q) return;
    this.actionLoading.set(true);
    this.quoteService.approve(this.jobId, q.id).subscribe({
      next: () => { this.actionLoading.set(false); this.loadQuoteAndPayment(); },
      error: (err) => { this.actionLoading.set(false); this.actionError.set(err.error?.message ?? 'Failed to approve quote.'); }
    });
  }

  onInitiatePayment(): void {
    const q = this.quote();
    if (!q) return;
    this.actionLoading.set(true);
    this.paymentService.initiate(q.id).subscribe({
      next: (result) => {
        this.actionLoading.set(false);
        window.location.href = result.checkoutUrl; // redirect to the real Chapa checkout page
      },
      error: (err) => { this.actionLoading.set(false); this.actionError.set(err.error?.message ?? 'Failed to initiate payment.'); }
    });
  }

  onReleasePayment(): void {
    const p = this.payment();
    if (!p) return;
    this.actionLoading.set(true);
    this.paymentService.release(p.id).subscribe({
      next: () => { this.actionLoading.set(false); this.loadJob(); this.loadQuoteAndPayment(); },
      error: (err) => { this.actionLoading.set(false); this.actionError.set(err.error?.message ?? 'Failed to release payment.'); }
    });
  }

  loadMessages(): void {
    this.signalr.connect();
    this.signalr.joinJobGroup(this.jobId);
    this.messageService.getForJob(this.jobId).subscribe(msgs => {
      this.messages.set(msgs);
      this.shouldScroll = true;
    });
    this.signalr.onJobMessage((message: MessageResponse) => {
      if (message.jobId === this.jobId) {
        this.messages.update(msgs => [...msgs, message]);
        this.shouldScroll = true;
      }
    });
  }

  ngOnDestroy(): void {
    this.signalr.leaveJobGroup(this.jobId);
    this.signalr.offJobMessage();
  }

  ngAfterViewChecked(): void {
    if (this.shouldScroll) {
      this.messagesEnd?.nativeElement.scrollIntoView({ behavior: 'smooth' });
      this.shouldScroll = false;
    }
  }

  onSendMessage(): void {
    if (this.messageForm.invalid) return;
    const content = this.messageForm.getRawValue().content!;
    this.messageService.send(this.jobId, { content }).subscribe(() => {
      this.messageForm.reset();
    });
  }


onRaiseDispute(): void {
  if (this.disputeForm.invalid) return;
  this.actionLoading.set(true);
  const reason = this.disputeForm.getRawValue().reason!;
  this.disputeService.raise({ jobId: this.jobId, reason }).subscribe({
    next: () => {
      this.actionLoading.set(false);
      this.showDisputeForm.set(false);
      this.disputeForm.reset();
      alert('Dispute raised. An admin will review it.');
    },
    error: (err) => {
      this.actionLoading.set(false);
      this.actionError.set(err.error?.message ?? 'Failed to raise dispute.');
    }
  });
}
}