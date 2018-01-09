import { NgModule } from "@angular/core";
import { RouterModule } from "@angular/router";
import { LeafletModule } from "@asymmetrik/ngx-leaflet";

import { AuthGuardService as AuthGuard } from '../core/services/auth/auth-guard.service';
import { TransportationService } from "./services/transportation/transportation.service";
import { TransportationHub } from "./services/transportation/transportation.hub";
import { MapsComponent } from "./pages/maps.page";
import { MapService } from "./services/maps.service";
import { RoleGuardService as RoleGuard } from "../core/services/auth/role-guard.service";
import { MapTransportationDialogComponent } from "./components/map-transportation-dialog/map-transportation-dialog.component";
import { SharedModule } from "../shared/shared.module";

@NgModule({
    declarations: [
        MapsComponent,
        MapTransportationDialogComponent
    ],
    imports: [
        SharedModule,
        RouterModule.forChild([
            { path: 'maps', component: MapsComponent, canActivate: [AuthGuard, RoleGuard] }
        ]),
        LeafletModule.forRoot()
    ],
    providers: [
        TransportationService,
        TransportationHub,
        MapService
    ],
    entryComponents: [
        MapTransportationDialogComponent        
    ]
})
export class MapsModule {
}
