import { AfterViewInit, Component, EventEmitter, ViewChild } from '@angular/core';
import { MatDialog, MatPaginator, MatSort, MatTableDataSource, MatDialogRef } from '@angular/material';
import { merge } from 'rxjs/Observable/merge';
import { of as observableOf } from 'rxjs/observable/of';
import { catchError } from 'rxjs/operators/catchError';
import { map } from 'rxjs/operators/map';
import { startWith } from 'rxjs/operators/startWith';
import { switchMap } from 'rxjs/operators/switchMap';

import { AzureAdUsersService } from '../../services/azure-ad-users.service';
import { UnixTimeService } from '../../../shared/services/unix-time/unix-time.service';
import { UserRepositoryService } from '../../../core/services/user-repository.service';

@Component({
  selector: 'user-add-dialog',
  styleUrls: ['./user-add-dialog.component.css'],
  templateUrl: './user-add-dialog.component.html',
})
export class UserAddDialogComponent implements AfterViewInit {
  displayedColumns = ['name', 'surname', 'email', 'telephone', 'custom-actions'];
  dataSource = new MatTableDataSource();

  resultsLength = 0;
  isLoadingResults = true;
  filterValue = "s";

  testEmitter = new EventEmitter<any>();

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;

  constructor(public dialogRef: MatDialogRef<UserAddDialogComponent>, private userManagementService: AzureAdUsersService, private dialog: MatDialog, private userRepository: UserRepositoryService, private unixTimeService: UnixTimeService) { }

  ngAfterViewInit() {
    merge(this.sort.sortChange, this.paginator.page).merge(this.testEmitter)
      .pipe(
      startWith({}),
      switchMap(() => {
        this.isLoadingResults = true;
        return this.userManagementService.searchUsers(this.filterValue);
      }),
      map(data => {
        this.isLoadingResults = false;
        this.resultsLength = data.length;

        return data;
      }),
      catchError(() => {
        this.isLoadingResults = false;
        return observableOf([]);
      })
      ).subscribe(data => this.dataSource.data = data);
  }

  applyFilter(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // MatTableDataSource defaults to lowercase matches
    this.filterValue = filterValue;
    this.dataSource.filter = filterValue;
  }

  addUser(element: any) {
    element.phone = element.telephoneNumber;
    element.createdAt = this.unixTimeService.getUnixTime();
      this.userRepository.addUser(element).subscribe(() => {
        this.dialogRef.close("refresh-table");
      });
  }
}