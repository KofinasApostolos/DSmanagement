import {
  Component,
  OnInit,
  ViewChild,
  ChangeDetectorRef,
  OnDestroy,
} from "@angular/core";
import {
  MatPaginator,
  MatTableDataSource,
  MatSort,
  MatDialog,
} from "@angular/material";
import { AlertifyService } from "src/app/_services/alertify.service";
import { Lessons } from "src/app/_models/lessons";
import { LessonsService } from "src/app/_services/lessons.service";
import { Subscriptions } from "src/app/_models/subscriptions";
import { SubscriptionsService } from "src/app/_services/subscriptions.service";
import { SubscriptionModalComponent } from "../subscription-modal/subscription-modal.component";
import { NgxSpinnerService } from "ngx-spinner";
import { Subscription } from "rxjs";

@Component({
  selector: "app-subscribe-management",
  templateUrl: "./subscribe-management.component.html",
  styleUrls: ["./subscribe-management.component.css"],
})
export class SubscribeManagementComponent implements OnInit, OnDestroy {
  model: any = {};

  lesson: Lessons[];
  subscriptions: Subscriptions[];

  discount: boolean = false;

  dataSource: MatTableDataSource<Subscriptions>;

  subscriptionSub: Subscription;
  lessonsSub: Subscription;

  discounts: any[] = [
    { value: 0, viewValue: "No" },
    { value: 1, viewValue: "Yes" },
  ];

  displayedColumns = [
    "Id",
    "Lesson",
    "Duration",
    "Price",
    "Discount",
    "Discprice",
    "actionsColumn",
  ];

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;

  constructor(
    private alertify: AlertifyService,
    private dialog: MatDialog,
    private lessonsService: LessonsService,
    private subscriptionsService: SubscriptionsService,
    private changeDetectorRefs: ChangeDetectorRef,
    private spinner: NgxSpinnerService
  ) {}

  ngOnDestroy(): void {
    if (this.subscriptionSub) this.subscriptionSub.unsubscribe();
    if (this.lessonsSub) this.lessonsSub.unsubscribe();
  }

  ngOnInit(): void {
    this.getLessons();
    this.getSubscriptions();
  }

  refresh(): void {
    this.subscriptionSub = this.subscriptionsService
      .getSubscriptions()
      .subscribe((subscriptions: Subscriptions[]) => {
        this.subscriptions = subscriptions;
        this.dataSource = new MatTableDataSource(this.subscriptions);
        this.dataSource.paginator = this.paginator;
        this.dataSource.sort = this.sort;
        this.changeDetectorRefs.detectChanges();
      });
  }

  getSubscriptions(): void {
    this.spinner.show();
    this.subscriptionSub = this.subscriptionsService
      .getSubscriptions()
      .subscribe(
        (subscriptions: Subscriptions[]) => {
          this.spinner.hide();
          this.subscriptions = subscriptions;
          this.dataSource = new MatTableDataSource(this.subscriptions);
          this.dataSource.paginator = this.paginator;
          this.dataSource.sort = this.sort;
        },
        (error) => {
          this.spinner.hide();
          this.alertify.error(error);
        }
      );
  }

  getLessons(): void {
    this.lessonsSub = this.lessonsService.getLessons().subscribe(
      (lesson: Lessons[]) => {
        this.lesson = lesson;
      },
      (error) => {
        this.alertify.error(error);
      }
    );
  }

  applyFilter(filterValue: string): void {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // Datasource defaults to lowercase matches
    this.dataSource.filter = filterValue;
  }

  discountChangeEvent(value) {
    if (value === 0) {
      this.discount = false;
    } else {
      this.discount = true;
    }
  }

  async deleteSubscription(Id: number): Promise<void> {
    await this.alertify
      .openConfirmDialog(
        "Are you sure you want to delete this record?",
        "Delete"
      )
      .then((res) => {
        if (res.value) {
          this.spinner.show();
          this.subscriptionSub = this.subscriptionsService
            .deleteSubscription(Id)
            .subscribe(
              (res) => {
                this.alertify.success(res.Message);
                this.refresh();
                this.spinner.hide();
              },
              (error) => {
                this.spinner.hide();
                this.alertify.error(error);
              }
            );
        }
      });
  }

  createSubscription(subscriptions: Subscriptions): void {
    const initialState = {
      subscriptions,
    };
    const dialogRef = this.dialog.open(SubscriptionModalComponent, {
      data: { initialState },
    });
    dialogRef.afterClosed().subscribe((result) => {
      if (result === true) {
        console.log(result);
        this.spinner.show();
        this.refresh();
        this.spinner.hide();
      }
    });
  }

  async updateSubscription(
    Id: string,
    Lessonid: string,
    Duration: string,
    Price: string,
    Discount: string,
    Discprice: string
  ): Promise<void> {
    await this.alertify
      .openConfirmDialog(
        "Are you sure you want to update this record",
        "Update"
      )
      .then((res) => {
        if (res.value) {
          this.spinner.show();
          this.model.Id = Id;
          this.model.Lessonid = Lessonid;
          this.model.Duration = Duration;
          this.model.Price = Price;
          this.model.Discount = Discount;
          if (Discprice !== "" && Discprice !== null && Discprice) {
            this.model.Discprice = Discprice;
          } else {
            this.model.Discprice = "00.00";
          }
          this.subscriptionSub = this.subscriptionsService
            .updateSubscription(this.model)
            .subscribe(
              (res) => {
                //this.subscriptions = subscriptions;
                this.alertify.success(res.Message);
                this.refresh();
                this.spinner.hide();
              },
              (error) => {
                this.spinner.hide();
                this.alertify.error(error);
              }
            );
        }
      });
  }
}
