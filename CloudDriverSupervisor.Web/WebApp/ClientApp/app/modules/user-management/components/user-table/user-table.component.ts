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
import { UserRepositoryService } from '../../../core/services/user-repository.service';

@Component({
    selector: 'user-table',
    styleUrls: ['./user-table.component.css'],
    templateUrl: './user-table.component.html',
})
export class UserTableComponent implements AfterViewInit {
    displayedColumns = ['name', 'surname', 'email', 'roles', 'custom-actions'];
    dataSource = new MatTableDataSource();

    resultsLength = 0;
    isLoadingResults = true;

    tableRefreshEmitter = new EventEmitter<any>();

    @ViewChild(MatPaginator) paginator: MatPaginator;
    @ViewChild(MatSort) sort: MatSort;

    constructor(private userManagementService: UserRepositoryService, private dialog: MatDialog){}
    
    ngAfterViewInit() {
        merge(this.sort.sortChange, this.paginator.page).merge(this.tableRefreshEmitter)
        .pipe(
          startWith({}),
          switchMap(() => {
            this.isLoadingResults = true;
            return this.userManagementService.getUsers();
          }),
          map(data => {
            this.isLoadingResults = false;
            this.resultsLength = data.userDtos.length;
  
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

    openRoleDialog(roles :string[], userId: string): void{
      let  driverRoleSelected = roles.some(role => role == "Driver");
      let  supervisorRoleSelected = roles.some(role => role == "Supervisor");
       
      let dialogRef = this.dialog.open(UserRoleDialogComponent, {
        width: '250px',
        data: { driverRoleSelected, supervisorRoleSelected, userId },
      });
  
      dialogRef.afterClosed().subscribe(result => {
        if(result == "refresh-table"){
          this.tableRefreshEmitter.emit();
        }
      });
    }

    openDeleteDialog(element : any){
      let dialogRef = this.dialog.open(UserDeleteDialogComponent, {
        width: '500px',
        data: { azureId: element.azureId, 
          name: element.name, 
          surname: element.surname, 
          email: element.email },
      });
  
      dialogRef.afterClosed().subscribe(result => {
        if(result == "refresh-table"){
          this.tableRefreshEmitter.emit();
        }
      });
    }

    onAddUserClick(){
      let dialogRef = this.dialog.open(UserAddDialogComponent, {
        width: '1024px'
      });
  
      dialogRef.afterClosed().subscribe(result => {
        if(result == "refresh-table"){
          this.tableRefreshEmitter.emit();
        }
      });
    }
}