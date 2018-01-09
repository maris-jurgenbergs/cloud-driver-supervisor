import { RouterModule } from '@angular/router';
import { NgModule } from '@angular/core';

import { AuthGuardService as AuthGuard } from '../core/services/auth/auth-guard.service';
import { RoleGuardService as RoleGuard } from "../core/services/auth/role-guard.service";
import { AlertsListComponent } from './components/alerts-list/alerts-list.component';
import { SharedModule } from '../shared/shared.module';
import { AlertDetailsDialogComponent } from './components/alert-details-dialog/alert-details-dialog.component';
import { AlertEvaluationDialogComponent } from './components/alert-evaluation-dialog/alert-evaluation-dialog.component';
import { AlertRepositoryService } from './services/alert-repository.service';
import { AlertsPage } from './pages/alerts.page';

@NgModule({
  imports: [
    RouterModule.forChild([{
      path: 'alerts',
      component: AlertsPage,
      canActivate: [AuthGuard, RoleGuard]
    }]),
    SharedModule
  ],
  declarations: [
    AlertsPage,
    AlertsListComponent,
    AlertDetailsDialogComponent,
    AlertEvaluationDialogComponent
  ],
  providers: [
    AlertRepositoryService
  ],
  entryComponents: [
    AlertDetailsDialogComponent,
    AlertEvaluationDialogComponent
  ]
})
export class AlertsModule {}