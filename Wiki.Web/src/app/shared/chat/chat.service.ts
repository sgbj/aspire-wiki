import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';

export type Chat = {
  id: string;
};

export type ChatMessage = {
  role: string;
  content: string;
};

@Injectable({
  providedIn: 'root',
})
export class ChatService {
  #http = inject(HttpClient);

  getChatId() {
    return localStorage['chatId'];
  }

  setChatId(chatId: string | null) {
    localStorage['chatId'] = chatId;
  }

  createChat() {
    return this.#http.post<Chat>('/api/chat', null);
  }

  createMessage(chatId: string, message: ChatMessage) {
    return this.#http.post(`/api/chat/${chatId}/messages`, message);
  }

  getMessages(chatId: string) {
    return this.#http.get<ChatMessage[]>(`/api/chat/${chatId}/messages`);
  }
}
