import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

import { AuthService } from '../services/auth/auth.service';

@Component({
    selector: 'login',
    templateUrl: './login.page.html'
})
export class LoginComponent implements OnInit {
    loggingText = "";
    constructor(private authService: AuthService, private router: Router) { }

    ngOnInit(): void {
        this.authService.handleWindowCallback();

        if (this.authService.isAuthenticated()) {
            this.router.navigate(["home"]);
        }
    }

    loginButtonClick() {
        this.loggingText = "Logging in..";
        this.authService.authenticate();
    }
}
