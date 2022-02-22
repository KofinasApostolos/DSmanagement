import { Component, OnDestroy, OnInit, ViewChild } from "@angular/core";
import { Userbookings } from "../_models/userbookings";
import { MatPaginator, MatSort, MatTableDataSource } from "@angular/material";
import { AlertifyService } from "src/app/_services/alertify.service";
import { BookingService } from "src/app/_services/booking.service";
import { SubscriptionState } from "../_models/subscriptionstate";
import { NgxSpinnerService } from "ngx-spinner";
import { Subscription } from "rxjs";
import { Helpers } from "../_helpers/helper";

@Component({
  selector: "app-mysubscriptions",
  templateUrl: "./mysubscriptions.component.html",
  styleUrls: ["./mysubscriptions.component.css"],
})
export class MySubscriptionsComponent implements OnInit, OnDestroy {
  model: any = {};

  userbookings: Userbookings[];
  subscriptionstate: SubscriptionState[];

  dataSource: MatTableDataSource<Userbookings>;

  displayedColumns = [
    "Lesson",
    "Duration",
    "Price",
    "Date",
    "ExpDate",
    "State",
    "Discount",
    "Days",
  ];

  bookingsSub: Subscription;

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;

  constructor(
    private alertify: AlertifyService,
    private bookingService: BookingService,
    private spinner: NgxSpinnerService
  ) {}

  ngOnDestroy(): void {
    if (this.bookingsSub) this.bookingsSub.unsubscribe();
  }

  ngOnInit(): void {
    this.getUserBookings(
      parseInt(JSON.parse(localStorage.getItem("decodedToken")).nameid)
    );
  }

  applyFilter(filterValue: string): void {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // Datasource defaults to lowercase matches
    this.dataSource.filter = filterValue;
  }

  getUserBookings(id: number): void {
    this.spinner.show();
    this.bookingsSub = this.bookingService.getBookings(id).subscribe(
      (userbookings: Userbookings[]) => {
        this.userbookings = userbookings;
        this.userbookings.forEach((booking) => {
          booking.Date = Helpers.dateConvert(booking.Date.toString());
          booking.ExpDate = Helpers.dateConvert(booking.ExpDate.toString());
        });
        this.dataSource = new MatTableDataSource(this.userbookings);
        this.dataSource.paginator = this.paginator;
        this.dataSource.sort = this.sort;
        this.spinner.hide();
      },
      (error) => {
        this.alertify.error(error);
        this.spinner.hide();
      }
    );
  }
}
