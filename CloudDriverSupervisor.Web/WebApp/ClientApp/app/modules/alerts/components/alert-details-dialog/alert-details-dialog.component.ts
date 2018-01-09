import { Component, Inject } from '@angular/core';
import {MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { OnInit } from 'adal-angular4/node_modules/@angular/core/src/metadata/lifecycle_hooks';

import { UserRepositoryService } from '../../../core/services/user-repository.service';
import { AlertRepositoryService } from '../../services/alert-repository.service';
import { AlertStatus, AlertType, SeverityLevel } from '../../services/alert.model';
import { UnixTimeService } from '../../../shared/services/unix-time/unix-time.service';
import { AlertEvaluationDialogComponent } from '../alert-evaluation-dialog/alert-evaluation-dialog.component';

@Component({
  templateUrl: './alert-details-dialog.component.html',
  styleUrls: ['./alert-details-dialog.component.css']
})
export class AlertDetailsDialogComponent implements OnInit{
  isLoadingResults: boolean = false;
  viewResult: any = {};
  AlertStatus = AlertStatus;
  AlertType = AlertType;
  SeverityLevel = SeverityLevel;
  
  constructor(public dialogRef: MatDialogRef<AlertDetailsDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any, private alertRepository: AlertRepositoryService, private unixTimeService: UnixTimeService, private dialog: MatDialog) { }

    ngOnInit(): void {
      this.fillViewData();
    }

    private fillViewData() {
      this.alertRepository.getAlertSasUri(this.data.alertId).switchMap(data => this.alertRepository.getAlert(data.alertSasUri))
      .subscribe(data => {
       this.viewResult.status = data[0].alert.status;
       this.viewResult.createdAt = data[0].alert.createdAt;
       this.viewResult.description = data[0].alert.description;
       this.viewResult.severityLevel = data[0].alert.severityLevel;
       this.viewResult.type = data[0].alert.type;
       this.viewResult.fullName = `${data[0].user.name} ${data[0].user.surname}`;
       this.viewResult.phone = data[0].user.phone;
      });
    }

    formatUnixTime(unixTime: number): string {
      let date = this.unixTimeService.parseUnixTime(unixTime);
      date.toLocaleTimeString()
      var formattedDate = `${this.pad(date.getDate(), 2)}/${this.pad(date.getMonth() + 1, 2)}/${date.getFullYear()} 
      ${this.pad(date.getHours(), 2)}:${this.pad(date.getMinutes(), 2)}:${this.pad(date.getSeconds(), 2)}`;
      return formattedDate;
    }

    private pad(num: number, size: number) {
      var s = num + "";
      while (s.length < size) s = "0" + s;
      return s;
    }

    onSetSeverityLevelClick(severityLevel: number): void {
      let dialogRef = this.dialog.open(AlertEvaluationDialogComponent, {
        width: '400px',
        data: {
          alertId: this.data.alertId,
          severityLevel
        },
      });

      dialogRef.afterClosed().subscribe(result => {
        if (result == "refresh-table") {
          this.fillViewData();
        }
      });
    }
}
