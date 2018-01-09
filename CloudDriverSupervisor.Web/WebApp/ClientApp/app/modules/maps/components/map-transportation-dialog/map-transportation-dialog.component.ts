import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialog } from '@angular/material';

import { TransportationService } from '../../services/transportation/transportation.service';
import { AlertStatus, AlertType, SeverityLevel, ViolationType } from '../../../alerts/services/alert.model';
import { UnixTimeService } from '../../../shared/services/unix-time/unix-time.service';
import { AlertDetailsDialogComponent } from '../../../alerts/components/alert-details-dialog/alert-details-dialog.component';
@Component({
  selector: 'app-map-transportation-dialog',
  templateUrl: './map-transportation-dialog.component.html',
  styleUrls: ['./map-transportation-dialog.component.css']
})
export class MapTransportationDialogComponent implements OnInit {
  viewResult: any = {};
  AlertStatus = AlertStatus;
  AlertType = AlertType;
  SeverityLevel = SeverityLevel;
  ViolationType = ViolationType;
  isLoadingResults = true;
  constructor(public dialogRef: MatDialogRef<MapTransportationDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any, private dialog: MatDialog, private transportationService: TransportationService, private unixTimeService: UnixTimeService) { }

  ngOnInit() {
    this.refreshData();
  }

  private refreshData(){
    this.transportationService.getTransportationDetails(this.data.transportationId)
    .subscribe(data => {
      console.log(data);
      this.isLoadingResults = false;
      this.viewResult = data;
    });
  }

  formatUnixTime(unixTime: number): string {
    let date = this.unixTimeService.parseUnixTime(unixTime);
    date.toLocaleTimeString()
    var formattedDate = `${this.pad(date.getDate(), 2)}/${this.pad(date.getMonth() + 1, 2)}/${date.getFullYear()} 
    ${this.pad(date.getHours(), 2)}:${this.pad(date.getMinutes(), 2)}:${this.pad(date.getSeconds(), 2)}`;
    return formattedDate;
  }

  openAlertDetailsDialog(alertId: string) {
    let dialogRef = this.dialog.open(AlertDetailsDialogComponent, {
      width: '900px',
      data: {
        alertId
      },
    });

    dialogRef.afterClosed().subscribe(result => {
      this.refreshData();
    });
  }

  private pad(num: number, size: number) {
    var s = num + "";
    while (s.length < size) s = "0" + s;
    return s;
  }
}
