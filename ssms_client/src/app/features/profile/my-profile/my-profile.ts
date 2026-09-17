import { Component, inject, OnInit, signal } from '@angular/core';
import { AuthService } from '../../../core/auth/auth';
import { WorkerSkillService } from '../../../core/services/worker-skill';
import { CategoryService } from '../../../core/services/category';
import { WorkerSkillResponse } from '../../../core/models/skill.model';
import { CategoryResponse } from '../../../core/models/category.model';
import { Card } from '../../../shared/components/card/card';
import { LoadingSpinner } from '../../../shared/components/loading-spinner/loading-spinner';

@Component({
  selector: 'app-my-profile',
  standalone: true,
  imports: [Card, LoadingSpinner],
  templateUrl: './my-profile.html',
  styleUrl: './my-profile.scss'
})
export class MyProfile implements OnInit {
  authService = inject(AuthService);
  private workerSkillService = inject(WorkerSkillService);
  private categoryService = inject(CategoryService);

  skills = signal<WorkerSkillResponse[]>([]);
  availableCategories = signal<CategoryResponse[]>([]);
  isLoading = signal(true);
  uploadError = signal('');
  selectedFile: File | null = null;

  get isWorker(): boolean {
    return this.authService.currentUser()?.role === 'Worker';
  }

  ngOnInit(): void {
    if (this.isWorker) {
      this.workerSkillService.getMine().subscribe(skills => {
        this.skills.set(skills);
        this.isLoading.set(false);
      });
      this.categoryService.getAll(true).subscribe(cats => this.availableCategories.set(cats));
    } else {
      this.isLoading.set(false);
    }
  }

  get unaddedCategories(): CategoryResponse[] {
    const addedIds = new Set(this.skills().map(s => s.categoryId));
    return this.availableCategories().filter(c => !addedIds.has(c.id));
  }

  onAddSkill(categoryId: string): void {
    if (!categoryId) return;
    this.workerSkillService.add({ categoryId }).subscribe(() => {
      this.workerSkillService.getMine().subscribe(skills => this.skills.set(skills));
    });
  }

  onRemoveSkill(categoryId: string): void {
    this.workerSkillService.remove(categoryId).subscribe(() => {
      this.skills.update(list => list.filter(s => s.categoryId !== categoryId));
    });
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    this.selectedFile = input.files?.[0] ?? null;
  }

  onUploadPicture(): void {
    if (!this.selectedFile) return;
    this.uploadError.set('');
    this.authService.uploadProfilePicture(this.selectedFile).subscribe({
      next: (res) => {
        const current = this.authService.currentUser();
        if (current) {
          this.authService.currentUser.set({ ...current, profilePictureUrl: res.profilePictureUrl });
        }
        this.selectedFile = null;
      },
      error: () => this.uploadError.set('Failed to upload picture.')
    });
  }
}