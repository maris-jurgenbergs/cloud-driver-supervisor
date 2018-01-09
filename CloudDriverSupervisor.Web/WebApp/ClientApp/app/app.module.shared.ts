import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpModule, Http } from '@angular/http';
import { RouterModule } from '@angular/router';
import { LeafletModule } from '@asymmetrik/ngx-leaflet';

import { AppComponent } from './components/app/app.component';
import { MapsModule } from './modules/maps/maps.module';
import { UserManagementModule } from './modules/user-management/user-management.module';
import { HomeModule } from './modules/home/home.module';
import { CoreModule } from './modules/core/core.module';
import { AuthService } from './modules/core/services/auth/auth.service';
import { AuthGuardService as AuthGuard } from './modules/core/services/auth/auth-guard.service';
import { AlertsModule } from './modules/alerts/alerts.module';


@NgModule({
    declarations: [
        AppComponent
    ],
    imports: [
        CommonModule,
        HttpModule,
        FormsModule,
        LeafletModule.forRoot(),
        MapsModule,
        UserManagementModule,
        HomeModule,
        AlertsModule,
        CoreModule,
    ],
    providers: [],
    exports: [
        RouterModule
    ]
})
export class AppModuleShared {
}
