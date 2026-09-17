import { Component, inject, OnInit, OnDestroy, signal, ElementRef, ViewChild, AfterViewChecked } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ListingChatService } from '../../../core/services/marketplace/listing-chat';
import { SignalrService } from '../../../core/signalr/signalr';
import { AuthService } from '../../../core/auth/auth';
import { ListingConversationResponse, ListingMessageResponse } from '../../../core/models/marketplace/listing-message.model';
import { Card } from '../../../shared/components/card/card';
import { LoadingSpinner } from '../../../shared/components/loading-spinner/loading-spinner';

import { CommonModule } from '@angular/common';


@Component({
  selector: 'app-listing-chat',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, Card, LoadingSpinner],
  templateUrl: './listing-chat.html',
  styleUrl: './listing-chat.scss'
})
export class ListingChat implements OnInit, OnDestroy, AfterViewChecked {
  private route = inject(ActivatedRoute);
  private fb = inject(FormBuilder);
  private chatService = inject(ListingChatService);
  private signalr = inject(SignalrService);
  authService = inject(AuthService);

  @ViewChild('messagesEnd') messagesEnd!: ElementRef<HTMLDivElement>;

  conversation = signal<ListingConversationResponse | null>(null);
  messages = signal<ListingMessageResponse[]>([]);
  isLoading = signal(true);
  isSending = signal(false);
  private shouldScroll = false;

  messageForm = this.fb.group({
    content: ['', [Validators.required]]
  });

  get currentUserId(): string | undefined {
    return this.authService.currentUser()?.userId;
  }

  ngOnInit(): void {
    const listingId = this.route.snapshot.queryParamMap.get('listingId');
    const conversationId = this.route.snapshot.queryParamMap.get('conversationId');

    this.signalr.connect();

    if (conversationId) {
      this.loadConversationById(conversationId);
    } else if (listingId) {
      this.chatService.startConversation(listingId).subscribe(conv => {
        this.conversation.set(conv);
        this.loadMessages(conv.id);
      });
    }

    this.signalr.onListingMessage((message: ListingMessageResponse) => {
      if (message.conversationId === this.conversation()?.id) {
        this.messages.update(msgs => [...msgs, message]);
        this.shouldScroll = true;
      }
    });
  }

  ngOnDestroy(): void {
    const convId = this.conversation()?.id;
    if (convId) this.signalr.leaveListingConversation(convId);
    this.signalr.offListingMessage();
  }

  ngAfterViewChecked(): void {
    if (this.shouldScroll) {
      this.messagesEnd?.nativeElement.scrollIntoView({ behavior: 'smooth' });
      this.shouldScroll = false;
    }
  }

  private loadConversationById(conversationId: string): void {
    this.chatService.getMyConversations().subscribe(convs => {
      const conv = convs.find(c => c.id === conversationId);
      if (conv) {
        this.conversation.set(conv);
        this.loadMessages(conv.id);
      } else {
        this.isLoading.set(false);
      }
    });
  }

  private loadMessages(conversationId: string): void {
    this.signalr.joinListingConversation(conversationId);
    this.chatService.getMessages(conversationId).subscribe({
      next: (msgs) => {
        this.messages.set(msgs);
        this.isLoading.set(false);
        this.shouldScroll = true;
      },
      error: () => this.isLoading.set(false)
    });
  }

  onSend(): void {
    const conv = this.conversation();
    if (this.messageForm.invalid || !conv) return;

    this.isSending.set(true);
    const content = this.messageForm.getRawValue().content!;

    this.chatService.sendMessage(conv.id, { content }).subscribe({
      next: () => {
        this.isSending.set(false);
        this.messageForm.reset();
        // No need to manually add the message here — the SignalR push will add it.
      },
      error: () => this.isSending.set(false)
    });
  }
}