import { CanActivate } from '@angular/router';
import { Injectable } from '@angular/core';

import { UserRepositoryService } from '../user-repository.service';
import { AuthService } from './auth.service';

@Injectable()
export class RoleGuardService implements CanActivate {
    constructor(private authService: AuthService, private userRepository: UserRepositoryService) { }

    public canActivate() {
        var userProfile = this.authService.getUserProfile();
        return this.userRepository.getUserRoles(userProfile.oid).map((data) => {
            return data.some(this.HasSupervisorRole);
        });
    }

    private HasSupervisorRole(element: any, index: any, array: any) {
        return element === "Supervisor";
      }      
}