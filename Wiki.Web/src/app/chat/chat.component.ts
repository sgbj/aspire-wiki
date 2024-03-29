import { Component, ElementRef, inject, viewChild } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatToolbarModule } from '@angular/material/toolbar';
import { ChatMessage, ChatService } from '../shared/chat/chat.service';

@Component({
  selector: 'app-chat',
  standalone: true,
  imports: [
    MatButtonModule,
    MatFormFieldModule,
    MatIconModule,
    MatInputModule,
    MatProgressSpinnerModule,
    MatToolbarModule,
    MatTooltipModule,
  ],
  templateUrl: './chat.component.html',
  styleUrl: './chat.component.scss',
})
export class ChatComponent {
  #chatService = inject(ChatService);

  messageContainer = viewChild<ElementRef<any>>('messageContainer');

  loading = false;
  chatId: string | null = null;
  messages: ChatMessage[] = [];

  ngOnInit() {
    this.chatId = this.#chatService.getChatId();
    if (!this.chatId) {
      this.#chatService.createChat().subscribe((chat) => {
        this.chatId = chat.id;
        this.#chatService.setChatId(this.chatId);
      });
    } else {
      this.getMessages();
    }
  }

  getMessages() {
    this.loading = true;

    this.#chatService.getMessages(this.chatId!).subscribe({
      next: (messages) => {
        this.messages = messages;

        setTimeout(() => {
          const messageContainer = this.messageContainer()?.nativeElement;
          if (messageContainer) {
            messageContainer.scrollTop = messageContainer.scrollHeight;
          }
        }, 0);
      },
      complete: () => {
        this.loading = false;
      },
    });
  }

  onSubmit(event: Event, message: HTMLTextAreaElement) {
    if (!message.value) {
      return;
    }

    this.loading = true;

    this.#chatService
      .createMessage(this.chatId!, {
        role: 'user',
        content: message.value,
      })
      .subscribe({
        next: () => {
          this.getMessages();
        },
        complete: () => {
          this.loading = false;
        },
      });

    message.value = '';
    event.preventDefault();
  }

  onClear() {
    this.chatId = null;
    this.#chatService.setChatId(null);
    this.messages = [];
    this.#chatService.createChat().subscribe((chat) => {
      this.chatId = chat.id;
      this.#chatService.setChatId(chat.id);
    });
  }
}
