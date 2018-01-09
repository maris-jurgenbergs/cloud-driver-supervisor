import { Component, AfterViewInit, ViewChild, EventEmitter } from '@angular/core';
import { MatTableDataSource, MatPaginator, MatSort, MatDialog } from '@angular/material';
import {Observable} from 'rxjs/Observable';
import { merge } from 'rxjs/Observable/merge';
import {map} from 'rxjs/operators/map';
import {of as observableOf} from 'rxjs/observable/of';
import { startWith } from 'rxjs/operators/startWith';
import { switchMap } from 'rxjs/operators/switchMap';
import {catchError} from 'rxjs/operators/catchError';

import { UserRoleDialogComponent } from '../user-role-dialog/user-role-dialog.component';
import { UserDeleteDialogComponent } from '../user-delete-dialog/user-delete-dialog.component';
import { UserAddDialogComponent } from '../user-add-dialog/user-add-dialog.component';
import { AzureAdUsersService } from '../../services/azure-ad-users.service';
import { UserRepositoryService } from '../../../core/services/user-repository.service';

@Component({
    selector: 'user-search-table',
    styleUrls: ['./user-search-table.component.css'],
    templateUrl: './user-search-table.component.html',
})
export class UserSearchTableComponent implements AfterViewInit {
    displayedColumns = ['name', 'surname', 'email', 'roles', 'customActions'];
    dataSource = new MatTableDataSource();

    resultsLength = 0;
    isLoadingResults = true;

    testEmitter = new EventEmitter<any>();

    @ViewChild(MatPaginator) paginator: MatPaginator;
    @ViewChild(MatSort) sort: MatSort;

    constructor(private userManagementService: AzureAdUsersService, private dialog: MatDialog){}
    
    ngAfterViewInit() {
        merge(this.sort.sortChange, this.paginator.page).merge(this.testEmitter)
        .pipe(
          startWith({}),
          switchMap(() => {
            this.isLoadingResults = true;
            return this.userManagementService.searchUsers("");
          }),
          map(data => {
            this.isLoadingResults = false;
            this.resultsLength = data.total_count;
  
            return data.userDtos;
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
        this.dataSource.filter = filterValue;
    }
}