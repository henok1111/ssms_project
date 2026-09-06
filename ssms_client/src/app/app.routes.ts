import { Routes } from '@angular/router';
import { JobListComponent } from './components/job-list/job-list.component';
import { JobCreateComponent } from './components/job-create/job-create.component';
import { LoginComponent } from './components/login/login.component';
import { RegisterComponent } from './components/register/register.component';
import { MyJobsComponent } from './components/my-jobs/my-jobs.component';
import { authGuard } from './guards/auth.guard';

export const routes: Routes = [
  { path: '', component: JobListComponent },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'jobs/new', component: JobCreateComponent, canActivate: [authGuard] },
  { path: 'my-jobs', component: MyJobsComponent, canActivate: [authGuard] },
];