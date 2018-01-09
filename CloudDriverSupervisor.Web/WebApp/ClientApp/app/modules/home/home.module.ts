import { NgModule } from "@angular/core";
import { RouterModule } from "@angular/router";

import { HomeComponent } from "./pages/home.page";
import { AuthGuardService as AuthGuard } from "../core/services/auth/auth-guard.service";
import { RoleGuardService as RoleGuard } from "../core/services/auth/role-guard.service";

@NgModule({
    declarations: [
        HomeComponent
    ],
    imports: [
        RouterModule.forChild([
            { path: "home", component: HomeComponent, canActivate: [AuthGuard, RoleGuard] }
        ]),
    ],
    providers: []
})
export class HomeModule {

}