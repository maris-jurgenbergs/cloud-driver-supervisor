import { Injectable } from "@angular/core";
import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Observable } from "rxjs/Observable";

import { ITransportationResult } from "./transportation.model";
import { AuthService } from "../../../core/services/auth/auth.service";
import { ConfigService } from "../../../core/services/config/config.service";

@Injectable()
export class TransportationService {
    constructor(private http: HttpClient, private authService: AuthService, private configService: ConfigService ) {
    }

    getTransportations(sasUri: string): Observable<ITransportationResult[]> {
        return this.http.get<ITransportationResult[]>(sasUri);
    }

    getSasUri(): Observable<any> {

        let periodStart = new Date();
        periodStart.setDate(periodStart.getDate() - 17);
        let periodEnd = new Date();
        let url = `${this.configService.getFabricEndpoint()}:8419/api/Transportation/all/${periodStart.toISOString()}/${periodEnd.toISOString()}`;
        console.log(url);
        return this.http.get(
            url,
            { headers: this.getAuthorizationHeader() });
    }

    getTransportationDetails(transportationId: string): Observable<any> {
        return this.http.get(
            this.configService.getFabricEndpoint() + `:8419/api/Transportation/${transportationId}/details`,
            { headers: this.getAuthorizationHeader() });
    }

    private getAuthorizationHeader(): HttpHeaders {
        let headers = new HttpHeaders();
        headers = headers.append("Authorization", `Bearer ${this.authService.getToken()}`);
        return headers;
    }
}