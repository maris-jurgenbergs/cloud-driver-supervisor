import { Component, Inject } from '@angular/core';
import {MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { OnInit } from 'adal-angular4/node_modules/@angular/core/src/metadata/lifecycle_hooks';

import { UserRepositoryService } from '../../../core/services/user-repository.service';

@Component({
  selector: 'app-user-role',
  templateUrl: './user-role-dialog.component.html',
  styleUrls: ['./user-role-dialog.component.css']
})
export class UserRoleDialogComponent implements OnInit{
  driverSelected: boolean;
  supervisorSelected: boolean;
  isLoadingResults: boolean = false;
  constructor(public dialogRef: MatDialogRef<UserRoleDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any, private userRepository: UserRepositoryService) { }

    ngOnInit(): void {
      this.driverSelected = this.data.driverRoleSelected;
      this.supervisorSelected = this.data.supervisorRoleSelected;
    }

    onCancelClick(): void {
      this.dialogRef.close();
    }

    onSaveClick(): void{
      this.isLoadingResults = true;
      let roles = [];
      if(this.driverSelected){
        roles.push("Driver");
      }

      if(this.supervisorSelected){
        roles.push("Supervisor");
      }

      this.userRepository.addUserRoles(this.data.userId, roles).subscribe(data => {
        this.dialogRef.close("refresh-table");
      });
    }
}
