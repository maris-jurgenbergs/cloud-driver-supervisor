import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';

import { AlertRepositoryService } from '../../services/alert-repository.service';

@Component({
  selector: 'app-alert-evaluation-dialog',
  templateUrl: './alert-evaluation-dialog.component.html',
  styleUrls: ['./alert-evaluation-dialog.component.css']
})
export class AlertEvaluationDialogComponent implements OnInit {
  selected = "1";
  constructor(public dialogRef: MatDialogRef<AlertEvaluationDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any, private alertRepository: AlertRepositoryService) { }

  ngOnInit() {
    if(this.data.severityLevel == 0){
      this.selected = "1";
    } else{
      this.selected = this.data.severityLevel.toString();
    }
  }

  onEvaluateClick(){
    this.alertRepository.patchAlertSeverityLevel(this.data.alertId, parseInt(this.selected))
    .subscribe(() => this.dialogRef.close("refresh-table"));
  }
}
