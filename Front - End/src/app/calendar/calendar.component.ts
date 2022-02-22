import {
  Component,
  ChangeDetectionStrategy,
  ChangeDetectorRef,
  OnInit,
  ViewChild,
  OnDestroy,
} from "@angular/core";
import {
  CalendarEvent,
  CalendarViewPeriod,
  CalendarMonthViewBeforeRenderEvent,
  CalendarWeekViewBeforeRenderEvent,
  CalendarDayViewBeforeRenderEvent,
} from "angular-calendar";
import { isSameDay } from "date-fns";
import { MatPaginator, MatSort, MatTableDataSource } from "@angular/material";
import { formatDate } from "@angular/common";
import { AlertifyService } from "../_services/alertify.service";
import { CalendarService } from "../_services/calendar.service";
import { TempRegisters } from "../_models/tempregisters";
import { NgxSpinnerService } from "ngx-spinner";
import { Subscription } from "rxjs";

@Component({
  selector: "app-calendar",
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: "./calendar.component.html",
  styleUrls: ["./calendar.component.css"],
})
export class CalendarComponent implements OnInit, OnDestroy {
  events: any = {};
  private bookings: any = {};
  model: any = {};

  bsModalRef: any;

  view: string = "month";
  private strdt: string;

  viewDate: Date = new Date();

  activeDayIsOpen: boolean = false;

  period: CalendarViewPeriod;

  dataSource: MatTableDataSource<TempRegisters>;

  displayedColumns = ["Username", "Firstname", "Lastname", "Lesson"];

  bookingsSub: Subscription;

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;

  constructor(
    private cdr: ChangeDetectorRef,
    private alertify: AlertifyService,
    private calendarService: CalendarService,
    private spinner: NgxSpinnerService
  ) {}

  ngOnDestroy(): void {
    if (this.bookingsSub) this.bookingsSub.unsubscribe();
  }

  ngOnInit(): void {
    this.spinner.show();
    this.bookingsSub = this.calendarService.getBookings().subscribe(
      (events: CalendarEvent[]) => {
        this.events = events;
        this.events.forEach((element) => {
          this.events.push({
            title: element.title,
            start: new Date(element.start),
          });
        });
        this.spinner.hide();
      },
      (error) => {
        this.spinner.hide();
        this.alertify.error(error);
      }
    );
  }

  applyFilter(filterValue: string): void {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // Datasource defaults to lowercase matches
    this.dataSource.filter = filterValue;
  }

  dayClicked({ date, events }: { date: Date; events: CalendarEvent[] }): void {
    this.spinner.show();
    if (isSameDay(this.viewDate, date) || events.length != 0) {
      this.strdt = formatDate(date, "yyyy/MM/dd", "en").replace(/\//g, "");
      this.bookingsSub = this.calendarService
        .getBookingsByDate(this.strdt)
        .subscribe(
          (tempregisters: TempRegisters[]) => {
            this.bookings = tempregisters;
            this.dataSource = new MatTableDataSource(this.bookings);
            this.dataSource.paginator = this.paginator;
            this.dataSource.sort = this.sort;
            this.spinner.hide();
          },
          (error) => {
            this.spinner.hide();
            this.alertify.error(error);
          }
        );
    } else this.spinner.hide();
  }

  beforeViewRender(
    event:
      | CalendarMonthViewBeforeRenderEvent
      | CalendarWeekViewBeforeRenderEvent
      | CalendarDayViewBeforeRenderEvent
  ): void {
    this.period = event.period;
    this.cdr.detectChanges();
  }
}
