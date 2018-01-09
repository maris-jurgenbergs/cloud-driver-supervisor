import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule, MatFormFieldModule, MatInputModule, MatProgressSpinnerModule, MatPaginatorModule, MatSortModule, MatDialogModule, MatCheckboxModule, MatButtonModule, MatSelectModule } from "@angular/material";

import { UnixTimeService } from './services/unix-time/unix-time.service';

@NgModule({
  imports: [
    CommonModule,
    MatTableModule,
    MatPaginatorModule,
    MatFormFieldModule,
    MatInputModule,
    MatProgressSpinnerModule,
    MatSortModule,
    MatDialogModule,
    MatCheckboxModule,
    MatButtonModule,
    MatSelectModule
  ],
  declarations: [],
  providers:[
    UnixTimeService
  ],
  exports: [
    CommonModule,
    MatTableModule,
    MatPaginatorModule,
    MatFormFieldModule,
    MatInputModule,
    MatProgressSpinnerModule,
    MatSortModule,
    MatDialogModule,
    MatCheckboxModule,
    MatButtonModule,
    MatSelectModule
  ]
})
export class SharedModule { }