import { Component, EventEmitter, Input, Output, OnInit, OnDestroy } from '@angular/core';
import { FormControl } from '@angular/forms';
import { debounceTime, distinctUntilChanged, Subscription } from 'rxjs';

@Component({
  selector: 'app-search-bar',
  standalone: false,
  template: `
    <div class="search-container">
      <div class="search-wrapper">
        <i class="fas fa-search search-icon"></i>
        <input
          type="text"
          [formControl]="searchControl"
          [placeholder]="placeholder"
          class="search-input"
        />
        <button *ngIf="searchControl.value" (click)="clearSearch()" class="clear-btn">
          <i class="fas fa-times"></i>
        </button>
      </div>
    </div>
  `,
  styles: [`
    .search-container {
      width: 100%;
      max-width: 400px;
    }
    .search-wrapper {
      position: relative;
      display: flex;
      align-items: center;
      background: white;
      border: 1px solid #e2e8f0;
      border-radius: 12px;
      padding: 0 16px;
      transition: all 0.3s ease;
      box-shadow: 0 2px 4px rgba(0,0,0,0.02);

      &:focus-within {
        border-color: #3b82f6;
        box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.1);
      }
    }
    .search-icon {
      color: #94a3b8;
      font-size: 0.9rem;
    }
    .search-input {
      width: 100%;
      border: none;
      padding: 12px 12px;
      font-size: 0.95rem;
      color: #1e293b;
      outline: none;
      background: transparent;

      &::placeholder {
        color: #94a3b8;
      }
    }
    .clear-btn {
      background: none;
      border: none;
      color: #94a3b8;
      cursor: pointer;
      padding: 4px;
      display: flex;
      align-items: center;
      justify-content: center;
      transition: color 0.2s;

      &:hover {
        color: #64748b;
      }
    }
  `]
})
export class SearchBarComponent implements OnInit, OnDestroy {
  @Input() placeholder: string = 'Search...';
  @Input() initialValue: string = '';
  @Input() debounceTime: number = 400;
  @Output() onSearch = new EventEmitter<string>();

  searchControl = new FormControl('');
  private sub?: Subscription;

  ngOnInit(): void {
    if (this.initialValue) {
      this.searchControl.setValue(this.initialValue, { emitEvent: false });
    }

    this.sub = this.searchControl.valueChanges.pipe(
      debounceTime(this.debounceTime),
      distinctUntilChanged()
    ).subscribe(value => {
      this.onSearch.emit(value || '');
    });
  }

  clearSearch(): void {
    this.searchControl.setValue('');
  }

  ngOnDestroy(): void {
    this.sub?.unsubscribe();
  }
}
