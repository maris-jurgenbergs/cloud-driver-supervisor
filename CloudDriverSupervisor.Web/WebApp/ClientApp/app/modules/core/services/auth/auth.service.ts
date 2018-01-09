import { Injectable } from "@angular/core";
import { Adal4Service } from "adal-angular4";
import { Observable } from "rxjs/Observable";

import { ConfigService } from "../config/config.service";         

@Injectable()
export class AuthService {
    constructor(private adalService: Adal4Service, private configService: ConfigService) {
        this.adalService.init(this.configService.getAuthConfig()); 
    }

    public isAuthenticated(): boolean {
        return this.adalService.userInfo.authenticated;
    }

    public authenticate() {
        this.adalService.clearCache();
        this.adalService.login();
    }

    public handleWindowCallback() {
        this.adalService.handleWindowCallback();
    }

    public getToken(): string {
        return this.adalService.userInfo.token;
    }

    public getUserProfile() {
        return this.adalService.userInfo.profile;
    }
}