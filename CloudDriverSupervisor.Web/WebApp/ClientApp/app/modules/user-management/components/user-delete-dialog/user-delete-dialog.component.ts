import { Component, OnInit, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material';

import { UserRepositoryService } from '../../../core/services/user-repository.service';

@Component({
  selector: 'app-user-delete-dialog',
  templateUrl: './user-delete-dialog.component.html',
  styleUrls: ['./user-delete-dialog.component.css']
})
export class UserDeleteDialogComponent{
  isLoadingResults: boolean = false;
  constructor(public dialogRef: MatDialogRef<UserDeleteDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any, private userRepository: UserRepositoryService) { }

    onCancelClick(): void {
      this.dialogRef.close();
    }

    onDeleteClick(): void {
      this.isLoadingResults = true;
      this.userRepository.deleteUser(this.data.azureId).subscribe(() => {
        this.dialogRef.close("refresh-table");
      });      
    }
}
