import { NgModule } from "@angular/core";
import { RouterModule } from "@angular/router";

import { AuthGuardService as AuthGuard } from '../core/services/auth/auth-guard.service';
import { RoleGuardService as RoleGuard } from "../core/services/auth/role-guard.service";
import { UserTableComponent } from "./components/user-table/user-table.component";
import { UserRoleDialogComponent } from "./components/user-role-dialog/user-role-dialog.component";
import { UserDeleteDialogComponent } from "./components/user-delete-dialog/user-delete-dialog.component";
import { UserAddDialogComponent } from "./components/user-add-dialog/user-add-dialog.component";
import { AzureAdUsersService } from "./services/azure-ad-users.service";
import { UserManagementPage } from "./pages/user-management.page";
import { SharedModule } from "../shared/shared.module";

@NgModule({
    declarations: [
        UserManagementPage,
        UserTableComponent,
        UserRoleDialogComponent,
        UserDeleteDialogComponent,
        UserAddDialogComponent
    ],
    imports: [
        SharedModule,
        RouterModule.forChild([
            { path: 'user-management', component: UserManagementPage, canActivate: [AuthGuard, RoleGuard] },
        ])
    ],
    providers: [
        AzureAdUsersService
    ],
    entryComponents: [
        UserRoleDialogComponent,
        UserDeleteDialogComponent,
        UserAddDialogComponent
    ]
})
export class UserManagementModule {
    
}