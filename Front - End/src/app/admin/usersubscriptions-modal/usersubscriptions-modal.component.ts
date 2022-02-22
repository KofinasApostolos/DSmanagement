import {
  Component,
  EventEmitter,
  OnDestroy,
  OnInit,
  Output,
} from "@angular/core";
import { AlertifyService } from "src/app/_services/alertify.service";
import { LessonsService } from "src/app/_services/lessons.service";
import { User } from "src/app/_models/user";
import { Lessons } from "src/app/_models/lessons";
import { UserService } from "src/app/_services/user.service";
import { UsersubscriptionsService } from "src/app/_services/usersubscriptions.service";
import { DaysService } from "src/app/_services/days.service";
import { Subject, Subscription } from "rxjs";
import { MatSelectChange } from "@angular/material";
import { CDays } from "src/app/_models/cdays";
import { Helpers } from "src/app/_helpers/helper";

@Component({
  selector: "app-usersubscriptions-modal",
  templateUrl: "./usersubscriptions-modal.component.html",
  styleUrls: ["./usersubscriptions-modal.component.css"],
})
export class UsersubscriptionsModalComponent implements OnInit, OnDestroy {
  model: any = {};

  users: User[];
  lessons: Lessons[];

  check: boolean;
  hasvalue: boolean = false;

  day: CDays[];
  filterarray: CDays[];

  public onClose: Subject<boolean>;

  daysSub: Subscription;
  userSubscriptionSub: Subscription;
  usersSub: Subscription;
  lessonsSub: Subscription;

  @Output() selectionChange: EventEmitter<MatSelectChange>;

  constructor(
    // public bsModalRef: BsModalRef,
    private userService: UserService,
    private lessonsService: LessonsService,
    private daysService: DaysService,
    private userSubscriptionService: UsersubscriptionsService,
    private alertify: AlertifyService
  ) {}

  ngOnDestroy(): void {
    if (this.daysSub) this.daysSub.unsubscribe();
    if (this.userSubscriptionSub) this.userSubscriptionSub.unsubscribe();
    if (this.usersSub) this.usersSub.unsubscribe();
    if (this.lessonsSub) this.lessonsSub.unsubscribe();
  }

  changedSelection(event): void {
    this.filterarray = this.day.filter((x) => x.Lesson === event);
    this.hasvalue = true;
  }

  ngOnInit(): void {
    this.onClose = new Subject();
    this.getUsers();
    this.getLessons();
    this.getCustomDays();
  }

  getUsers(): void {
    this.usersSub = this.userService.getUsers().subscribe(
      (users: User[]) => {
        this.users = users;
        this.users = this.users.filter((item) => item.IsAdmin === 0);
      },
      (error) => {
        this.alertify.error(error);
      }
    );
  }

  public onCancel(): void {
    this.onClose.next(false);
    // this.bsModalRef.hide();
  }

  getCustomDays(): void {
    this.daysSub = this.daysService.getCustomDays().subscribe(
      (day: CDays[]) => {
        this.day = day;
        this.filterarray = day;
      },
      (error) => {
        this.alertify.error(error);
      }
    );
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

  getErrorMessage(controlName: string, errorType: string) {
    return Helpers.getErrorMessageTemplateDrivenForm(controlName, errorType);
  }

  showOptions(event): void {
    this.check = event.checked;
  }

  changed(event): void {
    this.model.Day = event.value;
  }

  public onConfirm(): void {
    this.userSubscriptionSub = this.userSubscriptionService
      .registerUsersubscription(this.model)
      .subscribe(
        (res) => {
          this.onClose.next(true);
          // this.bsModalRef.hide();
          this.alertify.success(res.Message);
        },
        (error) => {
          this.alertify.error(error);
        }
      );
  }
}
