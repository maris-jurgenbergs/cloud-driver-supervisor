import { Http } from '@angular/http';
import { NgModule } from "@angular/core";
import { RouterModule } from "@angular/router";
import { Adal4Service, Adal4HTTPService } from 'adal-angular4';

import { AuthGuardService as AuthGuard } from "./services/auth/auth-guard.service";
import { RoleGuardService as RoleGuard } from "./services/auth/role-guard.service";
import { AuthService } from "./services/auth/auth.service";
import { LoginComponent } from "./pages/login.page";
import { NavMenuComponent } from "./components/navmenu/navmenu.component";
import { UserRepositoryService } from './services/user-repository.service';
import { ConfigService } from './services/config/config.service';

@NgModule({
    declarations: [
        LoginComponent,
        NavMenuComponent
    ],
    imports: [
        RouterModule.forRoot([
            { path: '', redirectTo: 'home', pathMatch: 'full' },
            { path: 'login', component: LoginComponent, pathMatch: 'prefix' },
            { path: '**', redirectTo: 'home' }
        ]),
    ],
    providers: [
        AuthService,
        AuthGuard,
        RoleGuard,
        Adal4Service,
        {
            provide: Adal4HTTPService,
            useFactory: Adal4HTTPService.factory,
            deps: [Http, Adal4Service]
        },
        UserRepositoryService,
        ConfigService
    ],
    exports: [
        NavMenuComponent,
        RouterModule
    ]
})
export class CoreModule {

}