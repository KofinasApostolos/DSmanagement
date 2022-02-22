import { Component, OnInit } from '@angular/core';
import { NgxSpinnerService } from 'ngx-spinner';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { ClosedayService } from 'src/app/_services/closeday.service';

@Component({
  selector: 'app-closeday',
  templateUrl: './closeday.component.html',
  styleUrls: ['./closeday.component.css']
})
export class ClosedayComponent implements OnInit {
  isLoading = false;

  constructor(
    private closeDayService: ClosedayService,
    private alertify: AlertifyService,
    private spinner: NgxSpinnerService
  ) { }

  ngOnInit(): void {
  }

  closeDay(): void {
    this.spinner.show();
    this.closeDayService.closeDay().subscribe(
      () => {
        this.alertify.success("Operation completed!");
        this.spinner.hide();
      },
      error => {
        this.spinner.hide();
        this.alertify.error(error);
      }
    );
  }

}
