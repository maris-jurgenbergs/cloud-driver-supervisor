import { Injectable } from '@angular/core';
import { HttpHeaders, HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs/Observable';

import { AuthService } from '../../core/services/auth/auth.service';
import { ConfigService } from '../../core/services/config/config.service';

@Injectable()
export class AzureAdUsersService {

    constructor(private http: HttpClient, private authService: AuthService, private configService: ConfigService) { }

    searchUsers(searchValue: string) : Observable<any> {
        return this.http.get(
            this.configService.getFabricEndpoint() + `:8419/api/AzureAd/users/${searchValue}`,
            { headers: this.getAuthorizationHeader() });
    }

    private getAuthorizationHeader(): HttpHeaders {
        let headers = new HttpHeaders();
        headers = headers.append("Authorization", `Bearer ${this.authService.getToken()}`);
        return headers;
    }
}