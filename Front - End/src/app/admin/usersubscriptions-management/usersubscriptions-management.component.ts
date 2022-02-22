import {
  Component,
  OnInit,
  ViewChild,
  ChangeDetectorRef,
  EventEmitter,
  Output,
  OnDestroy,
} from "@angular/core";
import { User } from "../../_models/user";
import { AlertifyService } from "src/app/_services/alertify.service";
import { UserService } from "src/app/_services/user.service";
import {
  MatDialog,
  MatPaginator,
  MatSelectChange,
  MatSort,
  MatTableDataSource,
} from "@angular/material";
import { LessonsService } from "src/app/_services/lessons.service";
import { Lessons } from "src/app/_models/lessons";
import { UserSubscription } from "src/app/_models/usersubscription";
import { UsersubscriptionsService } from "src/app/_services/usersubscriptions.service";
import { UsersubscriptionsModalComponent } from "../usersubscriptions-modal/usersubscriptions-modal.component";
import { SubscriptionsService } from "src/app/_services/subscriptions.service";
import { SubscriptionState } from "src/app/_models/subscriptionstate";
import * as fileSaver from "file-saver";
import { NgxSpinnerService } from "ngx-spinner";
import { DaysService } from "src/app/_services/days.service";
import { Subscription } from "rxjs";
import { Helpers } from "src/app/_helpers/helper";

@Component({
  selector: "app-usersubscriptions-management",
  templateUrl: "./usersubscriptions-management.component.html",
  styleUrls: ["./usersubscriptions-management.component.css"],
})
export class UsersubscriptionsManagementComponent implements OnInit, OnDestroy {
  private model: any = {};
  day: any[];

  private users: User[];
  lessons: Lessons[];
  subscriptionstate: SubscriptionState[];
  usersubscription: UserSubscription[];

  // private bsModalRef: BsModalRef;

  dataSource: MatTableDataSource<UserSubscription>;

  check: string;

  subscriptionStateSub: Subscription;
  daysSub: Subscription;
  userSubscriptionSub: Subscription;
  usersSub: Subscription;
  lessonsSub: Subscription;

  displayedColumns = [
    "Id",
    "User",
    "Lesson",
    "Duration",
    "Price",
    "Date",
    "ExpDate",
    "Discount",
    "State",
    "Days",
    "actionsColumn",
  ];

  discounts: any[] = [
    { value: 0, viewValue: "No" },
    { value: 1, viewValue: "Yes" },
  ];

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  @Output() selectionChange: EventEmitter<MatSelectChange>;

  constructor(
    private alertify: AlertifyService,
    private userService: UserService,
    private dialog: MatDialog,
    private subscriptionstateService: SubscriptionsService,
    private lessonsService: LessonsService,
    private daysService: DaysService,
    private userSubscriptionsService: UsersubscriptionsService,
    private changeDetectorRefs: ChangeDetectorRef,
    private spinner: NgxSpinnerService
  ) {}

  ngOnDestroy(): void {
    if (this.subscriptionStateSub) this.subscriptionStateSub.unsubscribe();
    if (this.daysSub) this.daysSub.unsubscribe();
    if (this.userSubscriptionSub) this.userSubscriptionSub.unsubscribe();
    if (this.usersSub) this.usersSub.unsubscribe();
    if (this.lessonsSub) this.lessonsSub.unsubscribe();
  }

  ngOnInit(): void {
    this.initStatesDays();
    this.getLessons();
    this.getUsers();
    this.getUsersubscriptions();
  }

  async initStatesDays(): Promise<void> {
    await this.getStates();
    await this.getDays();
  }

  async getStates(): Promise<void> {
    this.subscriptionStateSub = this.subscriptionstateService
      .getSubscriptionStates()
      .subscribe(
        (subscriptionstate: SubscriptionState[]) => {
          this.subscriptionstate = subscriptionstate;
        },
        (error) => {
          this.alertify.error(error);
        }
      );
  }

  async getDays(): Promise<void> {
    this.daysSub = this.daysService.getDays().subscribe(
      (day: any[]) => {
        this.day = day;
      },
      (error) => {
        this.alertify.error(error);
      }
    );
  }

  refresh(): void {
    this.userSubscriptionSub = this.userSubscriptionsService
      .getUsersubscriptions()
      .subscribe((usersubscription: UserSubscription[]) => {
        this.usersubscription = usersubscription;
        this.usersubscription.forEach((subs) => {
          subs.Date = Helpers.dateConvert(subs.Date.toString());
          subs.ExpDate = Helpers.dateConvert(subs.ExpDate.toString());
        });
        this.dataSource = new MatTableDataSource(this.usersubscription);
        this.dataSource.paginator = this.paginator;
        this.dataSource.sort = this.sort;
        this.changeDetectorRefs.detectChanges();
      });
  }

  exportXLSUsersSubs(): void {
    if (this.usersubscription != null) {
      this.spinner.show();
      this.userSubscriptionSub = this.userSubscriptionsService
        .getUserSubsXLS(
          JSON.parse(localStorage.getItem("decodedToken")).nameid,
          this.usersubscription
        )
        .subscribe(
          (response: any) => {
            if (response) {
              var blob = Helpers.base64ToBlob(response.Obj, "text/plain");
              fileSaver.saveAs(blob, "usersubs.xlsx");
            }
            this.spinner.hide();
            this.alertify.success(response.Message);
          },
          (error) => {
            this.spinner.hide();
            this.alertify.error(error);
          }
        );
    }
  }

  getLessons(): void {
    this.lessonsSub = this.lessonsService.getLessons().subscribe(
      (lessons: Lessons[]) => {
        this.lessons = lessons;
      },
      (error) => {
        this.alertify.error(error);
      }
    );
  }

  someMethod(selectionChange): void {
    this.selectionChange = selectionChange;
  }

  getUsers(): void {
    this.usersSub = this.userService.getUsers().subscribe(
      (users: User[]) => {
        this.users = users;
      },
      (error) => {
        this.alertify.error(error);
      }
    );
  }

  getUsersubscriptions(): void {
    this.spinner.show();
    this.userSubscriptionSub = this.userSubscriptionsService
      .getUsersubscriptions()
      .subscribe(
        (usersubscription: UserSubscription[]) => {
          this.usersubscription = usersubscription;
          this.usersubscription.forEach((subs) => {
            subs.Date = Helpers.dateConvert(subs.Date.toString());
            subs.ExpDate = Helpers.dateConvert(subs.ExpDate.toString());
          });
          this.dataSource = new MatTableDataSource(this.usersubscription);
          this.dataSource.paginator = this.paginator;
          this.dataSource.sort = this.sort;
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

  async deleteUsersubscription(Id: number): Promise<void> {
    await this.alertify
      .openConfirmDialog(
        "Are you sure you want to delete this record?",
        "Delete"
      )
      .then((res) => {
        if (res.value) {
          this.spinner.show();
          this.userSubscriptionSub = this.userSubscriptionsService
            .deleteUsersubscription(Id)
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

  async updateUsersubscription(
    Id: String,
    Userid: number,
    Lessonid: String,
    Duration: String,
    Price: String,
    Date: String,
    Discount: String,
    Day: string[],
    State: string
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
          this.model.Userid = Userid;
          this.model.Lessonid = Lessonid;
          this.model.Duration = Duration;
          this.model.Price = Price;
          this.model.Date = Date;
          this.model.Discount = Discount;
          this.model.Day = Day;
          this.model.State = State;
          this.userSubscriptionSub = this.userSubscriptionsService
            .updateUsersubscription(this.model)
            .subscribe(
              (res) => {
                //this.usersubscription = usersubscription;
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

  ShowOptions(event): void {
    this.check = event.checked;
  }

  createUsersubscription(usersubscription: UserSubscription): void {
    const initialState = {
      usersubscription,
    };
    const dialogRef = this.dialog.open(UsersubscriptionsModalComponent, {
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
}
