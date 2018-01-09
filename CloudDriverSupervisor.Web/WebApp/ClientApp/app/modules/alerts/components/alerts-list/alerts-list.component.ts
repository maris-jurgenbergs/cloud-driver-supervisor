import { AfterViewInit, Component, EventEmitter, ViewChild } from '@angular/core';
import { MatTableDataSource, MatPaginator, MatSort, MatDialog } from '@angular/material';
import { merge } from 'rxjs/Observable/merge';
import { map } from 'rxjs/operators/map';
import { of as observableOf } from 'rxjs/observable/of';
import { startWith } from 'rxjs/operators/startWith';
import { switchMap } from 'rxjs/operators/switchMap';
import { catchError } from 'rxjs/operators/catchError';

import { AlertRepositoryService } from '../../services/alert-repository.service';
import { AlertStatus, AlertType, SeverityLevel } from '../../services/alert.model';
import { UnixTimeService } from '../../../shared/services/unix-time/unix-time.service';
import { AlertDetailsDialogComponent } from '../alert-details-dialog/alert-details-dialog.component';

@Component({
  selector: 'alerts-list',
  templateUrl: './alerts-list.component.html',
  styleUrls: ['./alerts-list.component.css']
})
export class AlertsListComponent implements AfterViewInit {
  displayedColumns = ['status', 'created-at', 'full-name', 'type', 'severity-level', 'custom-actions'];
  dataSource = new MatTableDataSource();

  resultsLength = 0;
  isLoadingResults = true;

  AlertStatus = AlertStatus;
  AlertType = AlertType;
  SeverityLevel = SeverityLevel;

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  tableRefreshEmitter = new EventEmitter < any > ();

  constructor(private alertRepository: AlertRepositoryService, private unixTimeService: UnixTimeService, private dialog: MatDialog) {}

  ngAfterViewInit() {
    merge(this.sort.sortChange, this.paginator.page).merge(this.tableRefreshEmitter)
      .pipe(
        startWith({}),
        switchMap(() => {
          this.isLoadingResults = true;

          return this.alertRepository.getAlertListSasUri().switchMap(data => {
            return this.alertRepository.getAlertList(data.alertListSasUri);
          });
        }),
        map(data => {
          // Flip flag to show that loading has finished.
          this.isLoadingResults = false;
          // this.isRateLimitReached = false;
          let listResults: any[] = [];
          data.forEach((result: any) => {
            result.alerts.forEach((alert: any) => {
              let alertRecord: any = {
                fullName: `${result.user.name} ${result.user.surname}`,
                status: alert.status,
                createdAt: alert.createdAt,
                type: alert.type,
                severityLevel: alert.severityLevel,
                alertId: alert.alertId
              };
              listResults.push(alertRecord);
            });
          });
          listResults.sort()
          this.resultsLength = listResults.length;
          return listResults;
        }),
        catchError((error) => {
          this.isLoadingResults = false;
          console.log(error);
          return observableOf([]);
        })
      ).subscribe(data => this.dataSource.data = data);
  }

  applyFilter(filterValue: string) {
    filterValue = filterValue.toLowerCase();
    this.dataSource.filter = filterValue;
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
      this.tableRefreshEmitter.emit();
    });
  }

  private pad(num: number, size: number) {
    var s = num + "";
    while (s.length < size) s = "0" + s;
    return s;
  }

  private compareAlerts(a: any, b: any) {
    const coordA = a.capturedDateTimeUtc;
    const coordB = b.capturedDateTimeUtc;

    let comparison = 0;
    if (coordA > coordB) {
      comparison = 1;
    } else if (coordA < coordB) {
      comparison = -1;
    }
    return comparison;
  }
}
