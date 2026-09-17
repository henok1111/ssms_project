import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { ListingConversationResponse, ListingMessageResponse, SendListingMessageRequest } from '../../models/marketplace/listing-message.model';

@Injectable({ providedIn: 'root' })
export class ListingChatService {
  private http = inject(HttpClient);
  private baseUrl = `${environment.apiUrl}/market_place/listing-chat`;

  startConversation(listingId: string): Observable<ListingConversationResponse> {
    return this.http.post<ListingConversationResponse>(`${this.baseUrl}/listings/${listingId}/start`, {});
  }

  getMyConversations(): Observable<ListingConversationResponse[]> {
    return this.http.get<ListingConversationResponse[]>(`${this.baseUrl}/conversations`);
  }

  getMessages(conversationId: string): Observable<ListingMessageResponse[]> {
    return this.http.get<ListingMessageResponse[]>(`${this.baseUrl}/conversations/${conversationId}/messages`);
  }

  sendMessage(conversationId: string, request: SendListingMessageRequest): Observable<ListingMessageResponse> {
    return this.http.post<ListingMessageResponse>(`${this.baseUrl}/conversations/${conversationId}/messages`, request);
  }
}