import { HttpHeaders, HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs/Observable";

import { AuthService } from "../../core/services/auth/auth.service";
import { ConfigService } from "./config/config.service";

@Injectable()
export class UserRepositoryService {
    constructor(private http: HttpClient, private authService: AuthService, private configService: ConfigService) { }
    
    getUsers(): Observable<any> {
        return this.http.get(
            this.configService.getFabricEndpoint() + ":8419/api/User",
            { headers: this.getAuthorizationHeader() });
    }

    addUserRoles(userId: string, roles: string[]): Observable<any> {
        return this.http.post(
            this.configService.getFabricEndpoint() + `:8419/api/User/${userId}/roles`, roles,
            { headers: this.getAuthorizationHeader() });
    }   

    getUserRoles(userId: string): Observable<any> {
        return this.http.get(
            this.configService.getFabricEndpoint() + `:8419/api/User/${userId}/roles`,
            { headers: this.getAuthorizationHeader() });
    }

    deleteUser(userId: string) {
        return this.http.delete(
            this.configService.getFabricEndpoint() + `:8419/api/User/${userId}`,
            { headers: this.getAuthorizationHeader() });
    }

    addUser(user: any): Observable<any> {
        return this.http.post(
            this.configService.getFabricEndpoint() + `:8419/api/User/`, user,
            { headers: this.getAuthorizationHeader() });
    }

    private getAuthorizationHeader(): HttpHeaders {
        let headers = new HttpHeaders();
        headers = headers.append("Authorization", `Bearer ${this.authService.getToken()}`);
        return headers;
    }
}