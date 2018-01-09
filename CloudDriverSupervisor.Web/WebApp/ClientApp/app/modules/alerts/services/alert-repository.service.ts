import { Observable } from 'rxjs/Rx';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';

import { AuthService } from '../../core/services/auth/auth.service';
import { IAlertListResult, IAlert, IAlertResult } from './alert.model';
import { ConfigService } from '../../core/services/config/config.service';

@Injectable()
export class AlertRepositoryService {

constructor(private http: HttpClient, private authService: AuthService, private configService: ConfigService) {}

    getAlertListSasUri(): Observable<any> {
        return this.http.get(
            this.configService.getFabricEndpoint() + ":8419/api/Alert", {
                headers: this.getAuthorizationHeader()
            });
    }

    getAlertList(sasUri: string): Observable<IAlertListResult[]> {
        return this.http.get<IAlertListResult[]>(sasUri);
    }

    getAlertSasUri(alertId: string): Observable<any>{
        return this.http.get(
            this.configService.getFabricEndpoint() + `:8419/api/Alert/${alertId}`, {
                headers: this.getAuthorizationHeader()
            });
    }

    getAlert(sasUri: string): Observable<any> {
        return this.http.get<IAlertResult>(sasUri);
    }

    patchAlertSeverityLevel(alertId: string, severityLevel: number): Observable<any>{
        return this.http.patch(
            this.configService.getFabricEndpoint() + `:8419/api/Alert/${alertId}/severity/${severityLevel}`, 
            { headers: this.getAuthorizationHeader() });
    }

    private getAuthorizationHeader(): HttpHeaders {
        let headers = new HttpHeaders();
        headers = headers.append("Authorization", `Bearer ${this.authService.getToken()}`);
        return headers;
    }
}