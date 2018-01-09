import { Component } from '@angular/core';
import { OnInit } from '@angular/core/src/metadata/lifecycle_hooks';

import { AuthService } from '../../core/services/auth/auth.service';

@Component({
    selector: 'home',
    templateUrl: './home.page.html',
    styleUrls: ['./home.page.css']
})
export class HomeComponent implements OnInit {
    userFirstName: string;
    userSurname: string;
    userEmail: string;
    constructor(private authService: AuthService){}

    ngOnInit(): void {
        
        var userProfile = this.authService.getUserProfile();
        this.userFirstName = userProfile.given_name;
        this.userSurname = userProfile.family_name;
        this.userEmail = userProfile.unique_name;
    }
}
